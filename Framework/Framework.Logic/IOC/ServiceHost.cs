namespace Framework.Logic.IOC
{
    using System;
    using Interfaces.Injection;

    public class ServiceHost : System.ServiceModel.ServiceHost
    {
        private readonly IContainer _container;

        public ServiceHost(IContainer container, Type serviceType, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
            this._container = container;
        }

        protected override void OnOpening()
        {
            this.Description.Behaviors.Add(this._container.GetInstance<ServiceBehavior>());

            //this.Description.Behaviors.Add(this._container.GetInstance<ErrorLoggingServiceBehaviour>());

            base.OnOpening();
        }
    }
}