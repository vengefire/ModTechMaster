using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Castle.Core.Logging;
using Framework.Domain.Queue;
using Framework.Interfaces.Factories;
using Framework.Interfaces.Queue;

namespace Framework.Logic.Queue
{
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
                        "Queue Processor [{0}] has in invalid number of workers specified [{1}], minumum is 1", this,
                        numWorkers));
            }

            this.logger = logger;
            this.logicFactory = logicFactory;
            this.queueProcessorNodeFactory = queueProcessorNodeFactory;
            this.readQueueFactory = readQueueFactory;
            this.writeQueueFactory = writeQueueFactory;
            this.messageProcessingMode = messageProcessingMode;

            this.logger.InfoFormat("[{0}] creating [{1}] processor nodes...", ToString(), numWorkers);
            processorNodes = new List<IQueueProcessorNode<TRequestType>>(numWorkers);

            switch (this.messageProcessingMode)
            {
                case MessageProcessingMode.LocalChildNodeConcurrent:
                {
                    for (var i = 0; i < numWorkers; i++)
                    {
                        processorNodes.Add(CreateProcessorNode());
                    }

                    break;
                }
            }

            this.logger.InfoFormat("Created Queue Processor [{0}].", ToString());
        }

        public void Dispose()
        {
            logicFactory.Dispose();
            queueProcessorNodeFactory.Dispose();
            readQueueFactory.Dispose();
            writeQueueFactory.Dispose();
        }

        public Task ProcessingTask { get; private set; }

        public async Task StartProcessing()
        {
            switch (messageProcessingMode)
            {
                case MessageProcessingMode.LocalChildNodeConcurrent:
                    processorTasks.AddRange(processorNodes.Select(node => node.StartProcessing()));
                    break;
            }

            logger.DebugFormat("[{0}] - Processing messages...", this);
            ProcessingTask = Task.WhenAll(processorTasks.ToArray());
            await ProcessingTask;
        }

        private IQueueProcessorNode<TRequestType> CreateProcessorNode()
        {
            return queueProcessorNodeFactory.Create(readQueueFactory.Create(), logicFactory);
        }
    }
}