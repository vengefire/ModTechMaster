using System.Threading;
using Framework.Interfaces.Async;

namespace Framework.Logic.Async
{
    public class CancellationTokenSourceProvider : ICancellationTokenSourceProvider, ICancellationTokenProvider
    {
        public CancellationTokenSourceProvider()
        {
            CancellationTokenSource = new CancellationTokenSource();
        }

        public CancellationTokenSourceProvider(CancellationTokenSource cancellationTokenSource)
        {
            CancellationTokenSource = cancellationTokenSource;
        }

        public CancellationToken CancellationToken
        {
            get { return CancellationTokenSource.Token; }
        }

        public CancellationTokenSource CancellationTokenSource { get; }
    }
}