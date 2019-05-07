namespace Framework.Logic.Queue
{
    using System;
    using System.IO;
    using System.Messaging;
    using System.Transactions;
    using Castle.Core.Internal;
    using Castle.Core.Logging;
    using Domain.Queue;
    using Interfaces.Data.Services;
    using Interfaces.Environment;
    using Interfaces.Queue;
    using Utils.MSMQ;

    public class QueueBase<TRequestType> : IQueueBase, IDisposable
        where TRequestType : class
    {
        protected static readonly object Synch = new object();

        protected readonly ILogger Logger;

        protected readonly IMessageQueueService MessageQueueService;

        protected readonly MessageQueue MsmqMessageQueue;

        public QueueBase(
            ILogger logger,
            IEnvironment environment,
            string queueName,
            string messageQueueHostServerName,
            string multicastAddress,
            QueueMode queueMode,
            bool isTransactional,
            bool auditActivity,
            bool defaultRecoverable,
            IMessageQueueService messageQueueService)
        {
            this.Identifier = Guid.NewGuid();
            this.MessageQueueHostServerName = messageQueueHostServerName;
            this.BaseQueueName = queueName;
            this.DefaultRecoverable = defaultRecoverable;
            this.MessageQueueService = messageQueueService;
            this.AuditActivity = auditActivity;
            this.QueueMode = queueMode;
            this.MulticastAddress = multicastAddress;
            this.QueueName = string.Format("{0}_{1}", queueName, environment.DevStream);
            this.IsTransactional = isTransactional;

            if (!this.MulticastAddress.IsNullOrEmpty())
            {
                if (this.IsTransactional)
                {
                    throw new InvalidProgramException(
                                                      string.Format(
                                                                    "Multicast Queue [{0}] cannot be Transactional.",
                                                                    this.QueueName));
                }

                this.IsMulticast = true;
                this.IsTransactional = false;
            }

            if (messageQueueHostServerName.Equals(
                                                  Environment.MachineName,
                                                  StringComparison.InvariantCultureIgnoreCase))
            {
                this.IsLocal = true;
            }

            if (this.IsMulticast &&
                this.QueueMode == QueueMode.Send)
            {
                this.QueuePath = string.Format(@"FormatName:MULTICAST={0}", this.MulticastAddress);
            }
            else
            {
                this.QueuePath = string.Format(
                                               @"FormatName:Direct=OS:{0}\private$\{1}",
                                               messageQueueHostServerName,
                                               this.QueueName);
            }

            this.Logger = logger;

            if (!(this.IsMulticast && this.QueueMode == QueueMode.Send) &&
                this.IsLocal)
            {
                lock (QueueBase<TRequestType>.Synch)
                {
                    if (!MessageQueue.Exists(this.GetLocalPrivateName()))
                    {
                        this.Logger.InfoFormat(
                                               "Creating Queue [{0}], Transactional = [{1}], Multicast = [{2}], Multicast Address = [{3}].",
                                               this.GetLocalPrivateName(),
                                               this.IsTransactional,
                                               this.IsMulticast,
                                               this.MulticastAddress);
                        this.MsmqMessageQueue = MessageQueue.Create(this.GetLocalPrivateName(), this.IsTransactional);
                        this.MsmqMessageQueue.SetPermissions("Everyone", MessageQueueAccessRights.FullControl);
                        this.MsmqMessageQueue.SetPermissions("ANONYMOUS LOGON", MessageQueueAccessRights.FullControl);
                    }
                }
            }

            this.MsmqMessageQueue = new MessageQueue(this.QueuePath);

            if (this.IsMulticast &&
                QueueMode.Recv == this.QueueMode)
            {
                this.MsmqMessageQueue.MulticastAddress = this.MulticastAddress;
            }

            this.MsmqMessageQueue.DefaultPropertiesToSend.Recoverable = defaultRecoverable;
            this.MsmqMessageQueue.DefaultPropertiesToSend.UseAuthentication = false;
            this.MsmqMessageQueue.DefaultPropertiesToSend.UseDeadLetterQueue = true;
            this.MsmqMessageQueue.MessageReadPropertyFilter.CorrelationId = true;
        }

        public string HostServerName { get; private set; }

        public virtual void Dispose()
        {
        }

        public Guid Identifier { get; }

        public bool DefaultRecoverable { get; }

        public string BaseQueueName { get; }

        public string MessageQueueHostServerName { get; }

        public string MulticastAddress { get; }

        public string QueueName { get; }

        public string QueuePath { get; }

        public bool IsLocal { get; }

        public bool IsMulticast { get; }

        public bool IsTransactional { get; }

        public bool AuditActivity { get; }

        public QueueMode QueueMode { get; }

        public void MoveToSubQueue(string subQueueName, Message message)
        {
            this.MsmqMessageQueue.MoveToSubQueue(subQueueName, message);
        }

        protected string GetMessageXml(Message message)
        {
            var xmlMessageFormatter = new XmlMessageFormatter(new[] {typeof(TRequestType)});
            message.Formatter = xmlMessageFormatter;
            var sr = new StreamReader(message.BodyStream);
            message.BodyStream.Seek(0, SeekOrigin.Begin);
            var xml = sr.ReadToEnd();

            return xml;
        }

        protected void AddMessageAudit(Message msg, string subQueueName = null, Exception ex = null)
        {
            if (!this.AuditActivity)
            {
                return;
            }

            using (var transaction = new TransactionScope(
                                                          TransactionScopeOption.Required,
                                                          new TransactionOptions {IsolationLevel = IsolationLevel.ReadCommitted}))
            {
                var messageAudit = new MessageAudit
                                   {
                                       MessageId = msg.Id,
                                       MessageStatus = MessageStatus.AwaitingProcessing,
                                       CorrelationId = msg.CorrelationId,
                                       QueueName = null == subQueueName ? this.QueueName : string.Format("{0};{1}", this.QueueName, subQueueName),
                                       QueuePath = null == subQueueName ? this.QueuePath : string.Format("{0};{1}", this.QueuePath, subQueueName),
                                       MessageContent = this.GetMessageXml(msg)
                                   };
                var newId = this.MessageQueueService.CreateMessageAudit(messageAudit);
                if (null != ex)
                {
                    var processingError = new MessageProcessingError {MessageAuditId = newId, Error = ex.Message, StackTrace = ex.StackTrace, TmStamp = DateTime.Now};
                    this.MessageQueueService.CreateMessageAuditException(processingError);
                }

                transaction.Complete();
            }
        }

        protected void UpdateMessageAudit(Message msg, MessageStatus messageStatus, long elapedTimeInMilliseconds)
        {
            if (!this.AuditActivity)
            {
                return;
            }

            this.MessageQueueService.UpdateMessageProcessedStats(msg.Id, messageStatus, elapedTimeInMilliseconds);
        }

        private string GetLocalPrivateName()
        {
            return string.Format(@".\private$\{0}", this.QueueName);
        }
    }
}