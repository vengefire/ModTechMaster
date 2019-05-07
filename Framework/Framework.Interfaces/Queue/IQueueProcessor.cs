using System.Threading.Tasks;

namespace Framework.Interfaces.Queue
{
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