using System;
using Framework.Interfaces.Data.Services;

namespace Framework.Interfaces.Factories
{
    public interface IMessageQueueServiceFactory : IDisposable
    {
        IMessageQueueService Create();

        void Release(IMessageQueueService messageQueueDal);
    }
}