namespace Framework.Interfaces.Queue
{
    using System.Threading.Tasks;

    public interface IQueueProcessor
    {
        Task ProcessingTask { get; }

        Task StartProcessing();
    }

    public interface IQueueProcessor<in TMessageType> : IQueueProcessor
        where TMessageType : class
    {
    }
}