namespace Framework.Interfaces.Queue
{
    using System.Threading.Tasks;

    public interface IQueueProcessorNode<TRequestType>
        where TRequestType : class
    {
        Task StartProcessing();
    }
}