namespace Framework.Logic.Queue
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Castle.Core.Logging;
    using Domain.Queue;
    using Interfaces.Factories;
    using Interfaces.Queue;

    public sealed class QueueProcessor<TRequestType> : IQueueProcessor<TRequestType>, IDisposable
        where TRequestType : class
    {
        private readonly ILogger logger;

        private readonly IGenericFactory<IMessageProcessingLogic<TRequestType>> logicFactory;

        private readonly MessageProcessingMode messageProcessingMode;

        private readonly List<IQueueProcessorNode<TRequestType>> processorNodes;

        private readonly List<Task> processorTasks = new List<Task>();

        private readonly IQueueProcessorNodeFactory<TRequestType> queueProcessorNodeFactory;

        private readonly IReadQueueFactory<TRequestType> readQueueFactory;

        private readonly IWriteQueueFactory<TRequestType> writeQueueFactory;

        public QueueProcessor(
            ILogger logger,
            IGenericFactory<IMessageProcessingLogic<TRequestType>> logicFactory,
            IQueueProcessorNodeFactory<TRequestType> queueProcessorNodeFactory,
            IReadQueueFactory<TRequestType> readQueueFactory,
            IWriteQueueFactory<TRequestType> writeQueueFactory,
            MessageProcessingMode messageProcessingMode = MessageProcessingMode.LocalChildNodeConcurrent,
            int numWorkers = 1)
        {
            if (numWorkers < 1)
            {
                throw new InvalidProgramException(
                                                  string.Format(
                                                                "Queue Processor [{0}] has in invalid number of workers specified [{1}], minumum is 1",
                                                                this,
                                                                numWorkers));
            }

            this.logger = logger;
            this.logicFactory = logicFactory;
            this.queueProcessorNodeFactory = queueProcessorNodeFactory;
            this.readQueueFactory = readQueueFactory;
            this.writeQueueFactory = writeQueueFactory;
            this.messageProcessingMode = messageProcessingMode;

            this.logger.InfoFormat("[{0}] creating [{1}] processor nodes...", this.ToString(), numWorkers);
            this.processorNodes = new List<IQueueProcessorNode<TRequestType>>(numWorkers);

            switch (this.messageProcessingMode)
            {
                case MessageProcessingMode.LocalChildNodeConcurrent:
                {
                    for (var i = 0; i < numWorkers; i++) this.processorNodes.Add(this.CreateProcessorNode());

                    break;
                }
            }

            this.logger.InfoFormat("Created Queue Processor [{0}].", this.ToString());
        }

        public void Dispose()
        {
            this.logicFactory.Dispose();
            this.queueProcessorNodeFactory.Dispose();
            this.readQueueFactory.Dispose();
            this.writeQueueFactory.Dispose();
        }

        public Task ProcessingTask { get; private set; }

        public async Task StartProcessing()
        {
            switch (this.messageProcessingMode)
            {
                case MessageProcessingMode.LocalChildNodeConcurrent:
                    this.processorTasks.AddRange(this.processorNodes.Select(node => node.StartProcessing()));
                    break;
            }

            this.logger.DebugFormat("[{0}] - Processing messages...", this);
            this.ProcessingTask = Task.WhenAll(this.processorTasks.ToArray());
            await this.ProcessingTask;
        }

        private IQueueProcessorNode<TRequestType> CreateProcessorNode()
        {
            return this.queueProcessorNodeFactory.Create(this.readQueueFactory.Create(), this.logicFactory);
        }
    }
}