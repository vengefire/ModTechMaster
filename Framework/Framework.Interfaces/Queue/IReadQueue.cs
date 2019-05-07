namespace Framework.Interfaces.Queue
{
    using System;
    using System.Messaging;
    using System.Threading.Tasks;

    public interface IReadQueue : IQueueBase
    {
        event Action<Message, object, string, string> QueueMessageHandlerEvent;

        object ReceiveMessage();

        object ReceiveMessageByCorrelationId(string correlationId, int timeoutInMilliseconds);

        Message PeekMessage(TimeSpan timeSpan);

        Task StartReading();
    }

    public interface IReadQueue<TRequestType> : IReadQueue
        where TRequestType : class
    {
        Task ReadTask { get; }

        new event Action<Message, TRequestType, string, string> QueueMessageHandlerEvent;

        new TRequestType ReceiveMessage();

        new TRequestType ReceiveMessageByCorrelationId(string correlationId, int timeoutInMilliseconds);
    }
}