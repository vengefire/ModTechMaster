namespace Framework.Interfaces.Factories
{
    using System;
    using Queue;

    public interface IQueueProcessorNodeFactory<TRequestType> : IDisposable
        where TRequestType : class
    {
        IQueueProcessorNode<TRequestType> Create();

        IQueueProcessorNode<TRequestType> Create(
            IReadQueue<TRequestType> requestQueue,
            IGenericFactory<IMessageProcessingLogic<TRequestType>> logicFactory);

        void Release(IQueueProcessorNode<TRequestType> instance);
    }
}