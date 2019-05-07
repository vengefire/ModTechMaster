using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Castle.Core.Logging;
using Framework.Interfaces.Async;
using Framework.Interfaces.Logging;
using Framework.Interfaces.Providers;
using Framework.Interfaces.Repositories;
using Framework.Interfaces.Tasks;
using Framework.Utils.Extensions.Dictionary;

namespace Framework.Logic.Tasks.Async
{
    public class ScheduledTaskRunner : IServiceTaskRunner
    {
        private readonly IDateTimeProvider dateTimeProvider;

        private readonly IExceptionLogger exceptionLogger;

        private readonly ILogger logger;

        private readonly ITaskRepository taskRepository;

        public ScheduledTaskRunner(
            string name,
            ITaskScheduler scheduler,
            IServiceTask scheduledTask,
            ITaskRepository taskRepository,
            IDateTimeProvider dateTimeProvider,
            IExceptionLogger exceptionLogger,
            ICancellationTokenProvider cancellationTokenProvider,
            ILogger logger)
        {
            Name = name;
            Scheduler = scheduler;
            ServiceTask = scheduledTask;
            this.taskRepository = taskRepository;
            this.dateTimeProvider = dateTimeProvider;
            this.exceptionLogger = exceptionLogger;
            this.logger = logger;
            CancellationToken = cancellationTokenProvider.CancellationToken;

            var lastRunTime = this.taskRepository.GetLastExecution(Name);
            LastRunTime = lastRunTime ?? DateTime.MinValue;

            FailureCount = 0;
        }

        public CancellationToken CancellationToken { get; }

        public IServiceTask ServiceTask { get; }

        public DateTime LastRunTime { get; private set; }

        public ITaskScheduler Scheduler { get; }

        private int FailureCount { get; set; }

        public string Name { get; }

        public Task Task { get; private set; }

        public async Task StartProcessing()
        {
            Task = new Task(Execute, TaskCreationOptions.LongRunning);
            Task.Start();
            await Task;
        }

        public void Execute()
        {
            var handledMissedSlot = false;

            while (!CancellationToken.IsCancellationRequested)
            {
                bool missedSlot;
                var run = Scheduler.RunTask(LastRunTime, dateTimeProvider.Now, out missedSlot);

                if (!handledMissedSlot && missedSlot)
                {
                    logger.InfoFormat("Previous execution slot missed. Please check for consistency issues.");
                    handledMissedSlot = true;
                }

                if (run)
                {
                    try
                    {
                        logger.InfoFormat("Executing task");

                        var watch = Stopwatch.StartNew();
                        ServiceTask.ExecuteTask();

                        if (CancellationToken.IsCancellationRequested)
                        {
                            return;
                        }

                        watch.Stop();

                        logger.InfoFormat("Execution completed in {0}", watch.Elapsed);
                        LastRunTime = dateTimeProvider.Now;

                        handledMissedSlot = false;

                        PersistLastExecutionTime(LastRunTime, watch.Elapsed);
                        ResetFailureCount();
                    }
                    catch (Exception ex)
                    {
                        LogException(ex);
                        IncrementFailureCount();
                    }
                }

                Sleep();
            }

            logger.InfoFormat("Shut down");
        }

        protected TimeSpan GetSafeFailureSleep(TimeSpan proposedSleep)
        {
            var now = dateTimeProvider.Now;

            var expected = now.Add(proposedSleep);
            var actual = Scheduler.NextRun(now, now);

            return expected > actual ? new TimeSpan(actual.Ticks - now.Ticks) : proposedSleep;
        }

        private void LogException(Exception ex)
        {
            ex.Data.SafeAdd("Task name", Name);
            ex.Data.SafeAdd("Task type", ServiceTask.GetType().AssemblyQualifiedName);
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

            //// WaitHandle.WaitOne internally uses TimeSpan.Ticks, which is a double. The internal wait procedure takes an int...
            if (sleep.Ticks >= int.MaxValue)
            {
                sleep = new TimeSpan(int.MaxValue);
            }

            // ensure non-negative and a minimum of 1 second sleep
            if (sleep.TotalSeconds < 1)
            {
                sleep = new TimeSpan(0, 0, 0, 1);
            }

            if (!CancellationToken.IsCancellationRequested)
            {
                CancellationToken.WaitHandle.WaitOne(sleep);
            }
        }
    }
}