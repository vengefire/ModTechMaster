using System;
using Framework.Interfaces.Queue;

namespace Framework.Interfaces.Factories
{
    public interface IQueueProcessorFactory<TRequestType> : IDisposable
        where TRequestType : class
    {
        IQueueProcessor<TRequestType> Create();

        void Release(IQueueProcessor<TRequestType> queueProcessor);
    }
}