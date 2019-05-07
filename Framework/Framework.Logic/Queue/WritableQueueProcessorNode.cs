using Castle.Core.Logging;
using Framework.Interfaces.Factories;
using Framework.Interfaces.Queue;

namespace Framework.Logic.Queue
{
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
            WriteQueue = this.writeQueueFactory.Create();
        }

        public IWriteQueue<TRequestType> WriteQueue { get; }

        public override void Dispose()
        {
            writeQueueFactory.Dispose();
            base.Dispose();
        }
    }
}