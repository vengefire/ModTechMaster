using System;
using System.Messaging;
using System.Transactions;
using Castle.Core.Logging;
using Framework.Interfaces.Data.Services;
using Framework.Interfaces.Environment;
using Framework.Interfaces.Queue;

namespace Framework.Logic.Queue
{
    public class WriteQueue<TRequestType> : QueueBase<TRequestType>, IWriteQueue<TRequestType>
        where TRequestType : class
    {
        public WriteQueue(
            string queueName,
            string messageQueueHostServerName,
            IMessageQueueService messageQueueService,
            ILogger logger,
            IEnvironment environment,
            bool isTransactional = true,
            bool auditActivity = true,
            bool defaultRecoverable = true,
            string multicastAddress = null)
            : base(
                logger, environment, queueName, messageQueueHostServerName, multicastAddress, QueueMode.Send,
                isTransactional, auditActivity, defaultRecoverable, messageQueueService)
        {
        }

        public string SendMessage(TRequestType message)
        {
            return SendMessage(message, null);
        }

        public string SendMessage(TRequestType message, string correlationId, TimeSpan timeToBeReceived)
        {
            return SendMessageImpl(message, correlationId, timeToBeReceived);
        }

        public string SendMessage(TRequestType message, string correlationId)
        {
            return SendMessage(message, correlationId, Message.InfiniteTimeout);
        }

        public string SendMessage(object message)
        {
            return SendMessage((TRequestType) message);
        }

        private string SendMessageImpl(object message, string correlationId, TimeSpan timeToBeReceived)
        {
            var transactionOptions = new TransactionOptions
            {
                IsolationLevel = IsolationLevel.ReadCommitted
            };
            var transactionScope = !IsTransactional
                ? null
                : new TransactionScope(TransactionScopeOption.Required, transactionOptions);
            using (transactionScope)
            {
                try
                {
                    var msg = new Message
                    {
                        Body = message,
                        TimeToBeReceived = timeToBeReceived
                    };

                    if (!string.IsNullOrEmpty(correlationId))
                    {
                        msg.CorrelationId = correlationId;
                    }

                    MsmqMessageQueue.Send(msg,
                        IsTransactional ? MessageQueueTransactionType.Automatic : MessageQueueTransactionType.None);
                    lock (Synch)
                    {
                        AddMessageAudit(msg);
                    }

                    Logger.DebugFormat("Sent message to [{0}]...", QueueName);
                    if (transactionScope != null)
                    {
                        transactionScope.Complete();
                    }

                    return msg.Id;
                }
                catch (Exception ex)
                {
                    Logger.ErrorFormat(ex, "Exception encountered by [{0}] sending Message [{1}].", this, message);
                    throw;
                }
            }
        }
    }
}