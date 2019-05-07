namespace Framework.Logic.Tasks.Async
{
    using System.Threading;
    using Castle.Core.Logging;
    using Interfaces.Async;
    using Interfaces.Tasks;

    public abstract class ScheduledTask : IScheduledTask
    {
        protected readonly CancellationToken CancellationToken;
        protected readonly ILogger Logger;

        protected ScheduledTask(
            ILogger logger,
            ICancellationTokenProvider cancellationTokenProvider)
        {
            this.Logger = logger;
            this.CancellationToken = cancellationTokenProvider.CancellationToken;
        }

        public abstract void ExecuteTask();
    }
}