using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using Framework.Interfaces.Injection;

namespace Framework.Logic.IOC
{
    public class InstanceProvider : IInstanceProvider
    {
        private readonly IContainer container;

        private readonly Type serviceType;

        public InstanceProvider(IContainer container, Type serviceType)
        {
            this.container = container;
            this.serviceType = serviceType;
        }

        public object GetInstance(InstanceContext instanceContext)
        {
            return GetInstance(instanceContext, null);
        }

        public object GetInstance(InstanceContext instanceContext, Message message)
        {
            return container.GetInstance(serviceType);
        }

        public void ReleaseInstance(InstanceContext instanceContext, object instance)
        {
        }
    }
}