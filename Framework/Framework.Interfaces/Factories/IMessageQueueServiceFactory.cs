namespace Framework.Interfaces.Factories
{
    using System;
    using Data.Services;

    public interface IMessageQueueServiceFactory : IDisposable
    {
        IMessageQueueService Create();

        void Release(IMessageQueueService messageQueueDal);
    }
}