namespace Framework.Logic.Queue
{
    using System;
    using System.Messaging;
    using System.Threading.Tasks;
    using Interfaces.Factories;
    using Interfaces.Queue;

    public class QueueProcessorNode<TRequestType> : IQueueProcessorNode<TRequestType>, IDisposable
        where TRequestType : class
    {
        private readonly IGenericFactory<IMessageProcessingLogic<TRequestType>> logicFactory;
        private readonly IReadQueue<TRequestType> requestQueue;

        public QueueProcessorNode(
            IReadQueue<TRequestType> requestQueue,
            IGenericFactory<IMessageProcessingLogic<TRequestType>> logicFactory)
        {
            this.requestQueue = requestQueue;
            this.logicFactory = logicFactory;
            this.requestQueue.QueueMessageHandlerEvent += this.HandleMessageEvent;
        }

        public virtual void Dispose()
        {
        }

        public async Task StartProcessing()
        {
            await this.requestQueue.StartReading();
        }

        private void HandleMessageEvent(
            Message msmqMessage,
            TRequestType message,
            string messageId,
            string correlationId)
        {
            var logic = this.logicFactory.Create();
            logic.DoWork(message, messageId, correlationId);
            this.logicFactory.Release(logic);
        }
    }
}