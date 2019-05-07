using System.Threading;
using Castle.Core.Logging;
using Framework.Interfaces.Async;
using Framework.Interfaces.Tasks;

namespace Framework.Logic.Tasks.Async
{
    public abstract class BackgroundServiceTask<TEventArgs> : IBackgroundServiceTask<TEventArgs>
    {
        protected readonly CancellationToken CancellationToken;
        protected readonly ILogger Logger;

        protected BackgroundServiceTask(
            ILogger logger,
            ICancellationTokenProvider cancellationTokenProvider)
        {
            Logger = logger;
            CancellationToken = cancellationTokenProvider.CancellationToken;
        }

        public abstract void ExecuteTask(TEventArgs eventArgs);
    }
}