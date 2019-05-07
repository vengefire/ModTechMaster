using System;
using Framework.Interfaces.Injection;

namespace Framework.Logic.IOC
{
    public class ServiceHost : System.ServiceModel.ServiceHost
    {
        private readonly IContainer _container;

        public ServiceHost(IContainer container, Type serviceType, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
            _container = container;
        }

        protected override void OnOpening()
        {
            Description.Behaviors.Add(_container.GetInstance<ServiceBehavior>());
            //this.Description.Behaviors.Add(this._container.GetInstance<ErrorLoggingServiceBehaviour>());

            base.OnOpening();
        }
    }
}