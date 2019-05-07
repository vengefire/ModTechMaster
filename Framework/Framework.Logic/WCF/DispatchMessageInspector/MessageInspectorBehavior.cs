using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace Framework.Logic.WCF.DispatchMessageInspector
{
    public class MessageInspectorBehavior : IEndpointBehavior
    {
        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
            var messageInspector = new MessageInspector();
            endpointDispatcher.DispatchRuntime.MessageInspectors.Add(messageInspector);
        }

        public void Validate(ServiceEndpoint endpoint)
        {
        }
    }
}