using System;
using System.IO;
using System.Messaging;
using System.Transactions;
using Castle.Core.Internal;
using Castle.Core.Logging;
using Framework.Domain.Queue;
using Framework.Interfaces.Data.Services;
using Framework.Interfaces.Environment;
using Framework.Interfaces.Queue;
using Framework.Utils.MSMQ;

namespace Framework.Logic.Queue
{
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
            Identifier = Guid.NewGuid();
            MessageQueueHostServerName = messageQueueHostServerName;
            BaseQueueName = queueName;
            DefaultRecoverable = defaultRecoverable;
            MessageQueueService = messageQueueService;
            AuditActivity = auditActivity;
            QueueMode = queueMode;
            MulticastAddress = multicastAddress;
            QueueName = string.Format("{0}_{1}", queueName, environment.DevStream);
            IsTransactional = isTransactional;

            if (!MulticastAddress.IsNullOrEmpty())
            {
                if (IsTransactional)
                {
                    throw new InvalidProgramException(string.Format("Multicast Queue [{0}] cannot be Transactional.",
                        QueueName));
                }

                IsMulticast = true;
                IsTransactional = false;
            }

            if (messageQueueHostServerName.Equals(System.Environment.MachineName,
                StringComparison.InvariantCultureIgnoreCase))
            {
                IsLocal = true;
            }

            if (IsMulticast && QueueMode == QueueMode.Send)
            {
                QueuePath = string.Format(@"FormatName:MULTICAST={0}", MulticastAddress);
            }
            else
            {
                QueuePath = string.Format(@"FormatName:Direct=OS:{0}\private$\{1}", messageQueueHostServerName,
                    QueueName);
            }

            Logger = logger;

            if (!(IsMulticast && QueueMode == QueueMode.Send) && IsLocal)
            {
                lock (Synch)
                {
                    if (!MessageQueue.Exists(GetLocalPrivateName()))
                    {
                        Logger.InfoFormat(
                            "Creating Queue [{0}], Transactional = [{1}], Multicast = [{2}], Multicast Address = [{3}].",
                            GetLocalPrivateName(), IsTransactional, IsMulticast, MulticastAddress);
                        MsmqMessageQueue = MessageQueue.Create(GetLocalPrivateName(), IsTransactional);
                        MsmqMessageQueue.SetPermissions("Everyone", MessageQueueAccessRights.FullControl);
                        MsmqMessageQueue.SetPermissions("ANONYMOUS LOGON", MessageQueueAccessRights.FullControl);
                    }
                }
            }

            MsmqMessageQueue = new MessageQueue(QueuePath);

            if (IsMulticast && QueueMode.Recv == QueueMode)
            {
                MsmqMessageQueue.MulticastAddress = MulticastAddress;
            }

            MsmqMessageQueue.DefaultPropertiesToSend.Recoverable = defaultRecoverable;
            MsmqMessageQueue.DefaultPropertiesToSend.UseAuthentication = false;
            MsmqMessageQueue.DefaultPropertiesToSend.UseDeadLetterQueue = true;
            MsmqMessageQueue.MessageReadPropertyFilter.CorrelationId = true;
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
            MsmqMessageQueue.MoveToSubQueue(subQueueName, message);
        }

        protected string GetMessageXml(Message message)
        {
            var xmlMessageFormatter = new XmlMessageFormatter(
                new[]
                {
                    typeof(TRequestType)
                });
            message.Formatter = xmlMessageFormatter;
            var sr = new StreamReader(message.BodyStream);
            message.BodyStream.Seek(0, SeekOrigin.Begin);
            var xml = sr.ReadToEnd();

            return xml;
        }

        protected void AddMessageAudit(Message msg, string subQueueName = null, Exception ex = null)
        {
            if (!AuditActivity)
            {
                return;
            }

            using (var transaction = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions
                {
                    IsolationLevel = IsolationLevel.ReadCommitted
                }))
            {
                var messageAudit = new MessageAudit
                {
                    MessageId = msg.Id,
                    MessageStatus = MessageStatus.AwaitingProcessing,
                    CorrelationId = msg.CorrelationId,
                    QueueName = null == subQueueName ? QueueName : string.Format("{0};{1}", QueueName, subQueueName),
                    QueuePath = null == subQueueName ? QueuePath : string.Format("{0};{1}", QueuePath, subQueueName),
                    MessageContent = GetMessageXml(msg)
                };
                var newId = MessageQueueService.CreateMessageAudit(messageAudit);
                if (null != ex)
                {
                    var processingError = new MessageProcessingError
                    {
                        MessageAuditId = newId,
                        Error = ex.Message,
                        StackTrace = ex.StackTrace,
                        TmStamp = DateTime.Now
                    };
                    MessageQueueService.CreateMessageAuditException(processingError);
                }

                transaction.Complete();
            }
        }

        protected void UpdateMessageAudit(Message msg, MessageStatus messageStatus, long elapedTimeInMilliseconds)
        {
            if (!AuditActivity)
            {
                return;
            }

            MessageQueueService.UpdateMessageProcessedStats(msg.Id, messageStatus, elapedTimeInMilliseconds);
        }

        private string GetLocalPrivateName()
        {
            return string.Format(@".\private$\{0}", QueueName);
        }
    }
}