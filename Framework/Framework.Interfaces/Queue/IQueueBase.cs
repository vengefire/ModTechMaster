namespace Framework.Interfaces.Queue
{
    using System;
    using System.Messaging;

    public enum QueueMode
    {
        Recv,

        Send
    }

    public interface IQueueBase
    {
        Guid Identifier { get; }

        string MulticastAddress { get; }

        string MessageQueueHostServerName { get; }

        bool DefaultRecoverable { get; }

        string BaseQueueName { get; }

        string QueueName { get; }

        string QueuePath { get; }

        bool IsLocal { get; }

        bool IsMulticast { get; }

        bool IsTransactional { get; }

        bool AuditActivity { get; }

        QueueMode QueueMode { get; }

        void MoveToSubQueue(string subQueueName, Message message);
    }
}