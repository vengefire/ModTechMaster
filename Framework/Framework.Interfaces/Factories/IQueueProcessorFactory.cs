namespace Framework.Interfaces.Factories
{
    using System;
    using Queue;

    public interface IQueueProcessorFactory<TRequestType> : IDisposable
        where TRequestType : class
    {
        IQueueProcessor<TRequestType> Create();

        void Release(IQueueProcessor<TRequestType> queueProcessor);
    }
}