namespace Framework.Logic.Tasks.Async
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using Castle.Core.Logging;
    using Interfaces.Async;
    using Interfaces.Logging;
    using Interfaces.Providers;
    using Interfaces.Repositories;
    using Interfaces.Tasks;
    using Utils.Extensions.Dictionary;

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
            this.Name = name;
            this.Scheduler = scheduler;
            this.ServiceTask = scheduledTask;
            this.taskRepository = taskRepository;
            this.dateTimeProvider = dateTimeProvider;
            this.exceptionLogger = exceptionLogger;
            this.logger = logger;
            this.CancellationToken = cancellationTokenProvider.CancellationToken;

            var lastRunTime = this.taskRepository.GetLastExecution(this.Name);
            this.LastRunTime = lastRunTime ?? DateTime.MinValue;

            this.FailureCount = 0;
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
            this.Task = new Task(this.Execute, TaskCreationOptions.LongRunning);
            this.Task.Start();
            await this.Task;
        }

        public void Execute()
        {
            var handledMissedSlot = false;

            while (!this.CancellationToken.IsCancellationRequested)
            {
                bool missedSlot;
                var run = this.Scheduler.RunTask(this.LastRunTime, this.dateTimeProvider.Now, out missedSlot);

                if (!handledMissedSlot && missedSlot)
                {
                    this.logger.InfoFormat("Previous execution slot missed. Please check for consistency issues.");
                    handledMissedSlot = true;
                }

                if (run)
                {
                    try
                    {
                        this.logger.InfoFormat("Executing task");

                        var watch = Stopwatch.StartNew();
                        this.ServiceTask.ExecuteTask();

                        if (this.CancellationToken.IsCancellationRequested)
                        {
                            return;
                        }

                        watch.Stop();

                        this.logger.InfoFormat("Execution completed in {0}", watch.Elapsed);
                        this.LastRunTime = this.dateTimeProvider.Now;

                        handledMissedSlot = false;

                        this.PersistLastExecutionTime(this.LastRunTime, watch.Elapsed);
                        this.ResetFailureCount();
                    }
                    catch (Exception ex)
                    {
                        this.LogException(ex);
                        this.IncrementFailureCount();
                    }
                }

                this.Sleep();
            }

            this.logger.InfoFormat("Shut down");
        }

        protected TimeSpan GetSafeFailureSleep(TimeSpan proposedSleep)
        {
            var now = this.dateTimeProvider.Now;

            var expected = now.Add(proposedSleep);
            var actual = this.Scheduler.NextRun(now, now);

            return expected > actual ? new TimeSpan(actual.Ticks - now.Ticks) : proposedSleep;
        }

        private void LogException(Exception ex)
        {
            ex.Data.SafeAdd("Task name", this.Name);
            ex.Data.SafeAdd("Task type", this.ServiceTask.GetType().AssemblyQualifiedName);
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

            if (!this.CancellationToken.IsCancellationRequested)
            {
                this.CancellationToken.WaitHandle.WaitOne(sleep);
            }
        }
    }
}