namespace Framework.Logic.Queue.AutoTypeRegisters
{
    using System.Collections.Generic;
    using Castle.Facilities.TypedFactory;
    using Castle.MicroKernel.Registration;
    using Castle.Windsor;
    using Config;
    using Config.Queue;
    using Interfaces.Factories;
    using Interfaces.Queue;

    public static class MessageQueueAutoRegister
    {
        public static void RegisterConfiguredQueues(QueueConfigSectionHandler config, IWindsorContainer container)
        {
            var registrations = new List<IRegistration>();
            var untypedReadQueueInterface = typeof(IReadQueue<>);
            var untypedWriteQueueInterface = typeof(IWriteQueue<>);
            var untypedReadQueue = typeof(ReadQueue<>);
            var untypedWriteQueue = typeof(WriteQueue<>);

            foreach (MessageQueueElement queueConfig in config.MessageQueues)
            {
                var parameters = new Dictionary<string, object>
                                 {
                                     {"messageQueueHostServerName", queueConfig.MessageQueueHostServerName},
                                     {"auditActivity", queueConfig.AuditActivity},
                                     {"defaultRecoverable", queueConfig.DefaultRecoverable},
                                     {"isTransactional", queueConfig.IsTransactional},
                                     {"multicastAddress", queueConfig.MulticastAddress},
                                     {"queueName", queueConfig.Name}
                                 };

                var messageType = config.MessageTypes[queueConfig.MessageType].Type;

                if (queueConfig.Mode.Contains("R"))
                {
                    registrations.Add(
                                      Component.For(untypedReadQueueInterface.MakeGenericType(messageType))
                                               .ImplementedBy(untypedReadQueue.MakeGenericType(messageType))
                                               .DependsOn(parameters));
                }

                if (queueConfig.Mode.Contains("W"))
                {
                    registrations.Add(
                                      Component.For(untypedWriteQueueInterface.MakeGenericType(messageType))
                                               .ImplementedBy(untypedWriteQueue.MakeGenericType(messageType))
                                               .DependsOn(parameters));
                }

                registrations.Add(Component.For(typeof(IReadQueueFactory<>).MakeGenericType(messageType)).AsFactory());
                registrations.Add(Component.For(typeof(IWriteQueueFactory<>).MakeGenericType(messageType)).AsFactory());
            }

            container.Register(registrations.ToArray());
        }
    }
}