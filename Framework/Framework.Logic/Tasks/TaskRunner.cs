using System;
using System.Diagnostics;
using System.Threading;
using Framework.Interfaces.Injection;
using Framework.Interfaces.Logging;
using Framework.Interfaces.Providers;
using Framework.Interfaces.Repositories;
using Framework.Interfaces.Tasks;
using Framework.Logic.Services;
using Framework.Utils.Extensions.Dictionary;

namespace Framework.Logic.Tasks
{
    public class TaskRunner
    {
        private readonly IContainer container;

        private readonly IDateTimeProvider dateTimeProvider;

        private readonly IExceptionLogger exceptionLogger;

        private readonly AutoResetEvent resetEvent;

        internal readonly Type TargetType;

        private readonly ITaskRepository taskRepository;

        private ServiceState serviceState;

        private IServiceTask serviceTask;

        public TaskRunner(string name, Type targetType, ITaskScheduler scheduler, IContainer container,
            ITaskRepository taskRepository, IDateTimeProvider dateTimeProvider, IExceptionLogger exceptionLogger)
        {
            Name = name;
            Scheduler = scheduler;
            this.container = container;
            this.taskRepository = taskRepository;
            this.dateTimeProvider = dateTimeProvider;
            this.exceptionLogger = exceptionLogger;
            TargetType = targetType;

            var lastRunTime = this.taskRepository.GetLastExecution(Name);
            LastRunTime = lastRunTime ?? DateTime.MinValue;

            serviceState = ServiceState.Stopped;
            FailureCount = 0;

            resetEvent = new AutoResetEvent(false);
        }

        public DateTime LastRunTime { get; private set; }

        public string Name { get; set; }

        public ITaskScheduler Scheduler { get; }

        private int FailureCount { get; set; }

        public void Execute()
        {
            var handledMissedSlot = false;

            while (serviceState != ServiceState.ShuttingDown)
            {
                if (serviceState == ServiceState.Started)
                {
                    bool missedSlot;
                    var run = Scheduler.RunTask(LastRunTime, dateTimeProvider.Now, out missedSlot);

                    if (!handledMissedSlot && missedSlot)
                    {
                        Log("Previous execution slot missed. Please check for consistency issues.");
                        handledMissedSlot = true;
                    }

                    if (run)
                    {
                        try
                        {
                            if (serviceTask == null)
                            {
                                serviceTask = CreateServiceTask();
                            }

                            Log("Executing task");

                            var watch = Stopwatch.StartNew();
                            serviceTask.ExecuteTask();
                            watch.Stop();

                            Log("Execution completed in {0}", watch.Elapsed);
                            LastRunTime = dateTimeProvider.Now;

                            handledMissedSlot = false;

                            PersistLastExecutionTime(LastRunTime, watch.Elapsed);
                            ResetFailureCount();
                        }
                        catch (Exception ex)
                        {
                            serviceTask = null;
                            LogException(ex);
                            IncrementFailureCount();
                        }
                    }

                    Sleep();
                }
            }

            Log("Shut down");
            Thread.CurrentThread.Abort();
        }

        public void Shutdown()
        {
            if (serviceState == ServiceState.Started)
            {
                Log("Shutting down");
            }

            serviceState = ServiceState.ShuttingDown;
            resetEvent.Set();
        }

        public void Start()
        {
            switch (serviceState)
            {
                case ServiceState.Started:
                    Log("Already started");
                    break;
                case ServiceState.Stopped:
                    Log("Started");
                    serviceState = ServiceState.Started;
                    resetEvent.Set();
                    break;
                case ServiceState.ShuttingDown:
                    Log("Unable to start - service shutting down");
                    break;
            }
        }

        protected TimeSpan GetSafeFailureSleep(TimeSpan proposedSleep)
        {
            var now = dateTimeProvider.Now;

            var expected = now.Add(proposedSleep);
            var actual = Scheduler.NextRun(now, now);

            return expected > actual ? new TimeSpan(actual.Ticks - now.Ticks) : proposedSleep;
        }

        private IServiceTask CreateServiceTask()
        {
            Log("Creating task instance ({0})", TargetType);
            return (IServiceTask) container.GetInstance(TargetType);
        }

        private void Log(string format, params object[] p)
        {
            var message = string.Format(format, p);
            Trace.WriteLine(string.Format("[{0}] {1}", Name, message));
        }

        private void LogException(Exception ex)
        {
            ex.Data.SafeAdd("Task name", Name);
            ex.Data.SafeAdd("Task type", TargetType.AssemblyQualifiedName);
            if (LastRunTime != DateTime.MinValue)
            {
                ex.Data.SafeAdd("Last run time", LastRunTime);
            }
            else
            {
                ex.Data.SafeAdd("Last run time", "[Never]");
            }

            exceptionLogger.Log(ex);
        }

        private void PersistLastExecutionTime(DateTime lastRunTime, TimeSpan duration)
        {
            taskRepository.UpdateTaskExecution(Name, lastRunTime, duration);
        }

        private void IncrementFailureCount()
        {
            FailureCount++;
        }

        private void ResetFailureCount()
        {
            FailureCount = 0;
        }

        private void Sleep()
        {
            switch (serviceState)
            {
                case ServiceState.Started:
                    TimeSpan sleep;
                    switch (FailureCount)
                    {
                        case 0:
                            var nextRun = Scheduler.NextRun(LastRunTime, dateTimeProvider.Now);
                            sleep = nextRun.Subtract(dateTimeProvider.Now);
                            break;
                        case 1:
                            sleep = GetSafeFailureSleep(new TimeSpan(0, 0, 0, 10));
                            break;
                        case 2:
                            sleep = GetSafeFailureSleep(new TimeSpan(0, 0, 1, 0));
                            break;
                        case 3:
                            sleep = GetSafeFailureSleep(new TimeSpan(0, 0, 10, 0));
                            break;
                        case 4:
                            sleep = GetSafeFailureSleep(new TimeSpan(0, 0, 30, 0));
                            break;
                        default:
                            sleep = GetSafeFailureSleep(new TimeSpan(0, 1, 0, 0));
                            break;
                    }

                    // ensure non-negative and a minimum of 1 second sleep
                    if (sleep.TotalSeconds < 1)
                    {
                        sleep = new TimeSpan(0, 0, 0, 1);
                    }

                    resetEvent.WaitOne(sleep, false);
                    break;
                case ServiceState.Stopped:
                    resetEvent.WaitOne(-1, false);
                    break;
            }
        }
    }
}