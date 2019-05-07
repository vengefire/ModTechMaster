using System.Threading;

namespace Framework.Interfaces.Async
{
    public interface ICancellationTokenProvider
    {
        CancellationToken CancellationToken { get; }
    }
}