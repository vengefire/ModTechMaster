using System.Collections.Generic;
using Castle.Facilities.TypedFactory;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Framework.Interfaces.Factories;
using Framework.Interfaces.Queue;
using Framework.Logic.Queue.Config;
using Framework.Logic.Queue.Config.MessageLogicHandler;

namespace Framework.Logic.Queue.AutoTypeRegisters
{
    public static class MessageLogicHandlerAutoRegister
    {
        public static void RegisterConfiguredMessageLogicHandlers(QueueConfigSectionHandler config,
            IWindsorContainer container)
        {
            var registrations = new List<IRegistration>();
            foreach (MessageLogicHandlerElement handler in config.MessageLogicHandlers)
            {
                var interfaceType =
                    typeof(IMessageProcessingLogic<>).MakeGenericType(config.MessageTypes[handler.MessageType].Type);
                registrations.Add(Component.For(interfaceType).ImplementedBy(handler.Type));
                registrations.Add(Component.For(typeof(IGenericFactory<>).MakeGenericType(interfaceType)).AsFactory());
            }

            container.Register(registrations.ToArray());
        }
    }
}