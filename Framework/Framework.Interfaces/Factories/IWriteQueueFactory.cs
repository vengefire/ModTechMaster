namespace Framework.Interfaces.Factories
{
    using System;
    using Queue;

    public interface IWriteQueueFactory<TRequestType> : IDisposable
        where TRequestType : class
    {
        IWriteQueue<TRequestType> Create();

        IWriteQueue<TRequestType> Create(IQueueBase cloneSource, int id);

        void Release(IWriteQueue<TRequestType> queue);
    }
}