using System.Threading;

namespace Framework.Interfaces.Async
{
    public interface ICancellationTokenSourceProvider
    {
        CancellationTokenSource CancellationTokenSource { get; }
    }
}