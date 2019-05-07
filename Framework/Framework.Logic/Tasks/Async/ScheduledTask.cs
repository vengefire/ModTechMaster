using System.Threading;
using Castle.Core.Logging;
using Framework.Interfaces.Async;
using Framework.Interfaces.Tasks;

namespace Framework.Logic.Tasks.Async
{
    public abstract class ScheduledTask : IScheduledTask
    {
        protected readonly CancellationToken CancellationToken;
        protected readonly ILogger Logger;

        protected ScheduledTask(
            ILogger logger,
            ICancellationTokenProvider cancellationTokenProvider)
        {
            Logger = logger;
            CancellationToken = cancellationTokenProvider.CancellationToken;
        }

        public abstract void ExecuteTask();
    }
}