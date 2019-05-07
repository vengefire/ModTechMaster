namespace Framework.Interfaces.Queue
{
    public interface IWritableQueueProcessorNode<TRequestType> : IQueueProcessorNode<TRequestType>
        where TRequestType : class
    {
        IWriteQueue<TRequestType> WriteQueue { get; }
    }
}