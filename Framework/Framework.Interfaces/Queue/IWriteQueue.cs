namespace Framework.Interfaces.Queue
{
    using System;

    public interface IWriteQueue<TRequestType> : IQueueBase
        where TRequestType : class
    {
        string SendMessage(TRequestType message, string correlationId);

        string SendMessage(TRequestType message);

        string SendMessage(TRequestType message, string correlationId, TimeSpan timeToBeReceived);
    }
}