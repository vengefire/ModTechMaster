using System;
using Framework.Interfaces.Queue;

namespace Framework.Interfaces.Factories
{
    public interface IWriteQueueFactory<TRequestType> : IDisposable
        where TRequestType : class
    {
        IWriteQueue<TRequestType> Create();

        IWriteQueue<TRequestType> Create(IQueueBase cloneSource, int id);

        void Release(IWriteQueue<TRequestType> queue);
    }
}