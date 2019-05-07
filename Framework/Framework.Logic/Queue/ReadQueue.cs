using System;
using System.Diagnostics;
using System.Messaging;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Castle.Core.Logging;
using Framework.Domain.Queue;
using Framework.Interfaces.Async;
using Framework.Interfaces.Data.Services;
using Framework.Interfaces.Environment;
using Framework.Interfaces.Queue;

namespace Framework.Logic.Queue
{
    public class ReadQueue<TRequestType> : QueueBase<TRequestType>, IReadQueue<TRequestType>
        where TRequestType : class
    {
        private readonly CancellationToken cancellationToken;

        public ReadQueue(
            string queueName,
            string messageQueueHostServerName,
            IMessageQueueService messageQueueService,
            ILogger logger,
            IEnvironment environment,
            ICancellationTokenProvider cancellationTokenProvider,
            bool isTransactional = true,
            bool auditActivity = true,
            bool defaultRecoverable = true,
            string multicastAddress = null)
            : base(
                logger, environment, queueName, messageQueueHostServerName, multicastAddress, QueueMode.Recv,
                isTransactional, auditActivity, defaultRecoverable, messageQueueService)
        {
            cancellationToken = cancellationTokenProvider.CancellationToken;
        }

        public event Action<Message, TRequestType, string, string> QueueMessageHandlerEvent;

        event Action<Message, object, string, string> IReadQueue.QueueMessageHandlerEvent
        {
            add { QueueMessageHandlerEvent += value; }

            remove { QueueMessageHandlerEvent -= value; }
        }

        public Task ReadTask { get; private set; }

        public TRequestType ReceiveMessage()
        {
            var msg = MsmqMessageQueue.Receive(new TimeSpan(0, 0, 0, 10));
            if (null == msg)
            {
                return null;
            }

            return msg.Body as TRequestType;
        }

        public TRequestType ReceiveMessageByCorrelationId(string correlationId, int timeoutInMilliseconds)
        {
            var msg = MsmqMessageQueue.ReceiveByCorrelationId(correlationId,
                new TimeSpan(0, 0, 0, 0, timeoutInMilliseconds));
            msg.Formatter = new XmlMessageFormatter(
                new[]
                {
                    typeof(TRequestType)
                });
            return msg.Body as TRequestType;
        }

        public Message PeekMessage(TimeSpan timeSpan)
        {
            try
            {
                return MsmqMessageQueue.Peek(timeSpan);
            }
            catch (Exception ex)
            {
                var msgEx = ex as MessageQueueException;
                if (null == msgEx || msgEx.MessageQueueErrorCode != MessageQueueErrorCode.IOTimeout)
                {
                    throw;
                }
            }

            return null;
        }

        public async Task StartReading()
        {
            ReadTask = new Task(Read, TaskCreationOptions.LongRunning);
            ReadTask.Start();
            Logger.InfoFormat("[{0}] has started reading...", this);
            await ReadTask;
        }

        object IReadQueue.ReceiveMessageByCorrelationId(string correlationId, int timeoutInMilliseconds)
        {
            return ReceiveMessageByCorrelationId(correlationId, timeoutInMilliseconds);
        }

        object IReadQueue.ReceiveMessage()
        {
            return ReceiveMessage();
        }

        public void Read()
        {
            var timer = new Stopwatch();
            while (!cancellationToken.IsCancellationRequested)
            {
                Message msg = null;
                timer.Reset();
                timer.Start();
                var transaction = IsTransactional
                    ? new TransactionScope(
                        TransactionScopeOption.RequiresNew,
                        new TransactionOptions
                        {
                            IsolationLevel = IsolationLevel.ReadCommitted
                        },
                        TransactionScopeAsyncFlowOption.Enabled)
                    : null;
                using (transaction)
                {
                    try
                    {
                        var handleTimer = new Stopwatch();
                        try
                        {
                            msg = MsmqMessageQueue.Receive(new TimeSpan(0, 0, 0, 3),
                                IsTransactional
                                    ? MessageQueueTransactionType.Automatic
                                    : MessageQueueTransactionType.None);
                        }
                        catch (MessageQueueException ex)
                        {
                            if (ex.MessageQueueErrorCode == MessageQueueErrorCode.IOTimeout)
                            {
                                continue;
                            }
                        }

                        handleTimer.Start();
                        Task.WaitAll(OnMessageRead(msg));
                        handleTimer.Stop();

                        UpdateMessageAudit(msg, MessageStatus.Processed, timer.ElapsedMilliseconds);

                        if (null != transaction)
                        {
                            transaction.Complete();
                        }

                        timer.Stop();
                        Logger.DebugFormat(
                            "Processed message from Queue [{0}] in [{1}] ms. Handling took [{2}]ms, overhead = [{3}]ms.",
                            QueueName, timer.ElapsedMilliseconds, handleTimer.ElapsedMilliseconds,
                            timer.ElapsedMilliseconds - handleTimer.ElapsedMilliseconds);
                    }
                    catch (Exception ex)
                    {
                        timer.Stop();
                        Logger.ErrorFormat(ex, "An exception occurred processing message [{0}] in Queue[{1}].", msg,
                            this);
                        if (transaction != null)
                        {
                            transaction.Dispose();
                        }

                        using (var errorTransaction = new TransactionScope(
                            TransactionScopeOption.RequiresNew,
                            new TransactionOptions
                            {
                                IsolationLevel = IsolationLevel.ReadCommitted
                            }))
                        {
                            // If transactional, then move the errored message to the SubQueue, else just read the message off the queue...
                            if (IsTransactional)
                            {
                                MoveToSubQueue("error", msg);
                            }

                            UpdateMessageAudit(msg, MessageStatus.Errored, timer.ElapsedMilliseconds);
                            if (IsTransactional)
                            {
                                AddMessageAudit(msg, "error", ex);
                            }

                            errorTransaction.Complete();
                        }
                    }
                }
            }
        }

        private async Task OnMessageRead(Message msg)
        {
            msg.Formatter = new XmlMessageFormatter(
                new[]
                {
                    typeof(TRequestType)
                });

            if (QueueMessageHandlerEvent != null)
            {
                Logger.DebugFormat("Calling Queue Message Handler Event [{0}].", QueueMessageHandlerEvent);
                await Task.Run(() => QueueMessageHandlerEvent(msg, (TRequestType) msg.Body, msg.Id, msg.CorrelationId));
            }
        }
    }
}