namespace Framework.Logic.Async
{
    using System.Threading;
    using Interfaces.Async;

    public class CancellationTokenSourceProvider : ICancellationTokenSourceProvider, ICancellationTokenProvider
    {
        public CancellationTokenSourceProvider()
        {
            this.CancellationTokenSource = new CancellationTokenSource();
        }

        public CancellationTokenSourceProvider(CancellationTokenSource cancellationTokenSource)
        {
            this.CancellationTokenSource = cancellationTokenSource;
        }

        public CancellationToken CancellationToken => this.CancellationTokenSource.Token;

        public CancellationTokenSource CancellationTokenSource { get; }
    }
}