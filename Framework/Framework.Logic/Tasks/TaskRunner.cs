namespace Framework.Logic.Tasks
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using Interfaces.Injection;
    using Interfaces.Logging;
    using Interfaces.Providers;
    using Interfaces.Repositories;
    using Interfaces.Tasks;
    using Services;
    using Utils.Extensions.Dictionary;

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

        public TaskRunner(
            string name,
            Type targetType,
            ITaskScheduler scheduler,
            IContainer container,
            ITaskRepository taskRepository,
            IDateTimeProvider dateTimeProvider,
            IExceptionLogger exceptionLogger)
        {
            this.Name = name;
            this.Scheduler = scheduler;
            this.container = container;
            this.taskRepository = taskRepository;
            this.dateTimeProvider = dateTimeProvider;
            this.exceptionLogger = exceptionLogger;
            this.TargetType = targetType;

            var lastRunTime = this.taskRepository.GetLastExecution(this.Name);
            this.LastRunTime = lastRunTime ?? DateTime.MinValue;

            this.serviceState = ServiceState.Stopped;
            this.FailureCount = 0;

            this.resetEvent = new AutoResetEvent(false);
        }

        public DateTime LastRunTime { get; private set; }

        public string Name { get; set; }

        public ITaskScheduler Scheduler { get; }

        private int FailureCount { get; set; }

        public void Execute()
        {
            var handledMissedSlot = false;

            while (this.serviceState != ServiceState.ShuttingDown)
                if (this.serviceState == ServiceState.Started)
                {
                    bool missedSlot;
                    var run = this.Scheduler.RunTask(this.LastRunTime, this.dateTimeProvider.Now, out missedSlot);

                    if (!handledMissedSlot && missedSlot)
                    {
                        this.Log("Previous execution slot missed. Please check for consistency issues.");
                        handledMissedSlot = true;
                    }

                    if (run)
                    {
                        try
                        {
                            if (this.serviceTask == null)
                            {
                                this.serviceTask = this.CreateServiceTask();
                            }

                            this.Log("Executing task");

                            var watch = Stopwatch.StartNew();
                            this.serviceTask.ExecuteTask();
                            watch.Stop();

                            this.Log("Execution completed in {0}", watch.Elapsed);
                            this.LastRunTime = this.dateTimeProvider.Now;

                            handledMissedSlot = false;

                            this.PersistLastExecutionTime(this.LastRunTime, watch.Elapsed);
                            this.ResetFailureCount();
                        }
                        catch (Exception ex)
                        {
                            this.serviceTask = null;
                            this.LogException(ex);
                            this.IncrementFailureCount();
                        }
                    }

                    this.Sleep();
                }

            this.Log("Shut down");
            Thread.CurrentThread.Abort();
        }

        public void Shutdown()
        {
            if (this.serviceState == ServiceState.Started)
            {
                this.Log("Shutting down");
            }

            this.serviceState = ServiceState.ShuttingDown;
            this.resetEvent.Set();
        }

        public void Start()
        {
            switch (this.serviceState)
            {
                case ServiceState.Started:
                    this.Log("Already started");
                    break;
                case ServiceState.Stopped:
                    this.Log("Started");
                    this.serviceState = ServiceState.Started;
                    this.resetEvent.Set();
                    break;
                case ServiceState.ShuttingDown:
                    this.Log("Unable to start - service shutting down");
                    break;
            }
        }

        protected TimeSpan GetSafeFailureSleep(TimeSpan proposedSleep)
        {
            var now = this.dateTimeProvider.Now;

            var expected = now.Add(proposedSleep);
            var actual = this.Scheduler.NextRun(now, now);

            return expected > actual ? new TimeSpan(actual.Ticks - now.Ticks) : proposedSleep;
        }

        private IServiceTask CreateServiceTask()
        {
            this.Log("Creating task instance ({0})", this.TargetType);
            return (IServiceTask)this.container.GetInstance(this.TargetType);
        }

        private void Log(string format, params object[] p)
        {
            var message = string.Format(format, p);
            Trace.WriteLine(string.Format("[{0}] {1}", this.Name, message));
        }

        private void LogException(Exception ex)
        {
            ex.Data.SafeAdd("Task name", this.Name);
            ex.Data.SafeAdd("Task type", this.TargetType.AssemblyQualifiedName);
            if (this.LastRunTime != DateTime.MinValue)
            {
                ex.Data.SafeAdd("Last run time", this.LastRunTime);
            }
            else
            {
                ex.Data.SafeAdd("Last run time", "[Never]");
            }

            this.exceptionLogger.Log(ex);
        }

        private void PersistLastExecutionTime(DateTime lastRunTime, TimeSpan duration)
        {
            this.taskRepository.UpdateTaskExecution(this.Name, lastRunTime, duration);
        }

        private void IncrementFailureCount()
        {
            this.FailureCount++;
        }

        private void ResetFailureCount()
        {
            this.FailureCount = 0;
        }

        private void Sleep()
        {
            switch (this.serviceState)
            {
                case ServiceState.Started:
                    TimeSpan sleep;
                    switch (this.FailureCount)
                    {
                        case 0:
                            var nextRun = this.Scheduler.NextRun(this.LastRunTime, this.dateTimeProvider.Now);
                            sleep = nextRun.Subtract(this.dateTimeProvider.Now);
                            break;
                        case 1:
                            sleep = this.GetSafeFailureSleep(new TimeSpan(0, 0, 0, 10));
                            break;
                        case 2:
                            sleep = this.GetSafeFailureSleep(new TimeSpan(0, 0, 1, 0));
                            break;
                        case 3:
                            sleep = this.GetSafeFailureSleep(new TimeSpan(0, 0, 10, 0));
                            break;
                        case 4:
                            sleep = this.GetSafeFailureSleep(new TimeSpan(0, 0, 30, 0));
                            break;
                        default:
                            sleep = this.GetSafeFailureSleep(new TimeSpan(0, 1, 0, 0));
                            break;
                    }

                    // ensure non-negative and a minimum of 1 second sleep
                    if (sleep.TotalSeconds < 1)
                    {
                        sleep = new TimeSpan(0, 0, 0, 1);
                    }

                    this.resetEvent.WaitOne(sleep, false);
                    break;
                case ServiceState.Stopped:
                    this.resetEvent.WaitOne(-1, false);
                    break;
            }
        }
    }
}