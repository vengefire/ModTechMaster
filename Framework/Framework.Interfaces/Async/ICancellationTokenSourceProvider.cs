namespace Framework.Interfaces.Async
{
    using System.Threading;

    public interface ICancellationTokenSourceProvider
    {
        CancellationTokenSource CancellationTokenSource { get; }
    }
}