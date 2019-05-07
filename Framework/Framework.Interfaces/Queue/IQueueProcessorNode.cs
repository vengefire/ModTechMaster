using System.Threading.Tasks;

namespace Framework.Interfaces.Queue
{
    public interface IQueueProcessorNode<TRequestType>
        where TRequestType : class
    {
        Task StartProcessing();
    }
}