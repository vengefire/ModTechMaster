namespace Framework.Logic.Queue
{
    using Castle.Core.Logging;
    using Interfaces.Factories;
    using Interfaces.Queue;

    public class WritableQueueProcessorNode<TRequestType> : QueueProcessorNode<TRequestType>,
                                                            IWritableQueueProcessorNode<TRequestType>
        where TRequestType : class
    {
        private readonly IWriteQueueFactory<TRequestType> writeQueueFactory;

        public WritableQueueProcessorNode(
            ILogger logger,
            IReadQueue<TRequestType> requestQueue,
            IGenericFactory<IMessageProcessingLogic<TRequestType>> logicFactory,
            IWriteQueueFactory<TRequestType> writeQueueFactory)
            : base(requestQueue, logicFactory)
        {
            this.writeQueueFactory = writeQueueFactory;
            this.WriteQueue = this.writeQueueFactory.Create();
        }

        public IWriteQueue<TRequestType> WriteQueue { get; }

        public override void Dispose()
        {
            this.writeQueueFactory.Dispose();
            base.Dispose();
        }
    }
}