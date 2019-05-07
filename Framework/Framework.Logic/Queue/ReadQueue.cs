namespace Framework.Logic.Queue
{
    using System;
    using System.Diagnostics;
    using System.Messaging;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Transactions;
    using Castle.Core.Logging;
    using Domain.Queue;
    using Interfaces.Async;
    using Interfaces.Data.Services;
    using Interfaces.Environment;
    using Interfaces.Queue;

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
                   logger,
                   environment,
                   queueName,
                   messageQueueHostServerName,
                   multicastAddress,
                   QueueMode.Recv,
                   isTransactional,
                   auditActivity,
                   defaultRecoverable,
                   messageQueueService)
        {
            this.cancellationToken = cancellationTokenProvider.CancellationToken;
        }

        public event Action<Message, TRequestType, string, string> QueueMessageHandlerEvent;

        event Action<Message, object, string, string> IReadQueue.QueueMessageHandlerEvent { add => this.QueueMessageHandlerEvent += value; remove => this.QueueMessageHandlerEvent -= value; }

        public Task ReadTask { get; private set; }

        public TRequestType ReceiveMessage()
        {
            var msg = this.MsmqMessageQueue.Receive(new TimeSpan(0, 0, 0, 10));
            if (null == msg)
            {
                return null;
            }

            return msg.Body as TRequestType;
        }

        public TRequestType ReceiveMessageByCorrelationId(string correlationId, int timeoutInMilliseconds)
        {
            var msg = this.MsmqMessageQueue.ReceiveByCorrelationId(
                                                                   correlationId,
                                                                   new TimeSpan(0, 0, 0, 0, timeoutInMilliseconds));
            msg.Formatter = new XmlMessageFormatter(new[] {typeof(TRequestType)});
            return msg.Body as TRequestType;
        }

        public Message PeekMessage(TimeSpan timeSpan)
        {
            try
            {
                return this.MsmqMessageQueue.Peek(timeSpan);
            }
            catch (Exception ex)
            {
                var msgEx = ex as MessageQueueException;
                if (null == msgEx ||
                    msgEx.MessageQueueErrorCode != MessageQueueErrorCode.IOTimeout)
                {
                    throw;
                }
            }

            return null;
        }

        public async Task StartReading()
        {
            this.ReadTask = new Task(this.Read, TaskCreationOptions.LongRunning);
            this.ReadTask.Start();
            this.Logger.InfoFormat("[{0}] has started reading...", this);
            await this.ReadTask;
        }

        object IReadQueue.ReceiveMessageByCorrelationId(string correlationId, int timeoutInMilliseconds)
        {
            return this.ReceiveMessageByCorrelationId(correlationId, timeoutInMilliseconds);
        }

        object IReadQueue.ReceiveMessage()
        {
            return this.ReceiveMessage();
        }

        public void Read()
        {
            var timer = new Stopwatch();
            while (!this.cancellationToken.IsCancellationRequested)
            {
                Message msg = null;
                timer.Reset();
                timer.Start();
                var transaction = this.IsTransactional
                    ? new TransactionScope(
                                           TransactionScopeOption.RequiresNew,
                                           new TransactionOptions {IsolationLevel = IsolationLevel.ReadCommitted},
                                           TransactionScopeAsyncFlowOption.Enabled)
                    : null;
                using (transaction)
                {
                    try
                    {
                        var handleTimer = new Stopwatch();
                        try
                        {
                            msg = this.MsmqMessageQueue.Receive(
                                                                new TimeSpan(0, 0, 0, 3),
                                                                this.IsTransactional
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
                        Task.WaitAll(this.OnMessageRead(msg));
                        handleTimer.Stop();

                        this.UpdateMessageAudit(msg, MessageStatus.Processed, timer.ElapsedMilliseconds);

                        if (null != transaction)
                        {
                            transaction.Complete();
                        }

                        timer.Stop();
                        this.Logger.DebugFormat(
                                                "Processed message from Queue [{0}] in [{1}] ms. Handling took [{2}]ms, overhead = [{3}]ms.",
                                                this.QueueName,
                                                timer.ElapsedMilliseconds,
                                                handleTimer.ElapsedMilliseconds,
                                                timer.ElapsedMilliseconds - handleTimer.ElapsedMilliseconds);
                    }
                    catch (Exception ex)
                    {
                        timer.Stop();
                        this.Logger.ErrorFormat(
                                                ex,
                                                "An exception occurred processing message [{0}] in Queue[{1}].",
                                                msg,
                                                this);
                        if (transaction != null)
                        {
                            transaction.Dispose();
                        }

                        using (var errorTransaction = new TransactionScope(
                                                                           TransactionScopeOption.RequiresNew,
                                                                           new TransactionOptions {IsolationLevel = IsolationLevel.ReadCommitted}))
                        {
                            // If transactional, then move the errored message to the SubQueue, else just read the message off the queue...
                            if (this.IsTransactional)
                            {
                                this.MoveToSubQueue("error", msg);
                            }

                            this.UpdateMessageAudit(msg, MessageStatus.Errored, timer.ElapsedMilliseconds);
                            if (this.IsTransactional)
                            {
                                this.AddMessageAudit(msg, "error", ex);
                            }

                            errorTransaction.Complete();
                        }
                    }
                }
            }
        }

        private async Task OnMessageRead(Message msg)
        {
            msg.Formatter = new XmlMessageFormatter(new[] {typeof(TRequestType)});

            if (this.QueueMessageHandlerEvent != null)
            {
                this.Logger.DebugFormat("Calling Queue Message Handler Event [{0}].", this.QueueMessageHandlerEvent);
                await Task.Run(() => this.QueueMessageHandlerEvent(msg, (TRequestType)msg.Body, msg.Id, msg.CorrelationId));
            }
        }
    }
}