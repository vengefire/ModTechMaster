using System.Collections.ObjectModel;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using Framework.Interfaces.Injection;

namespace Framework.Logic.IOC
{
    public class ServiceBehavior : IServiceBehavior
    {
        private readonly IContainer _container;

        public ServiceBehavior(IContainer container)
        {
            _container = container;
        }

        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            foreach (var cdb in serviceHostBase.ChannelDispatchers)
            {
                var cd = cdb as ChannelDispatcher;
                if (cd != null)
                {
                    foreach (var ed in cd.Endpoints)
                    {
                        ed.DispatchRuntime.InstanceProvider = new InstanceProvider(_container,
                            serviceDescription.ServiceType);
                    }
                }
            }
        }

        public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase,
            Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters)
        {
        }

        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
        }
    }
}