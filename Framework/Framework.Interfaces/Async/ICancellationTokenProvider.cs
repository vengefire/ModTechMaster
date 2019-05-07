namespace Framework.Interfaces.Async
{
    using System.Threading;

    public interface ICancellationTokenProvider
    {
        CancellationToken CancellationToken { get; }
    }
}