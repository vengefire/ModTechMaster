using System;
using System.Collections.Generic;
using System.Linq;
using Castle.Facilities.TypedFactory;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Framework.Interfaces.Factories;
using Framework.Interfaces.Queue;
using Framework.Logic.Queue.Config;
using Framework.Logic.Queue.Config.QueueProcessor;

namespace Framework.Logic.Queue.AutoTypeRegisters
{
    public static class QueueProcessorAutoRegister
    {
        public static void RegisterConfiguredQueueProcessors(QueueConfigSectionHandler queueConfig,
            IWindsorContainer container)
        {
            var registrations = new List<IRegistration>();

            foreach (QueueProcessorElement config in queueConfig.QueueProcessors)
            {
                var parameters = new Dictionary<string, object>
                {
                    {
                        "messageProcessingMode", config.MessageProcessingMode
                    },
                    {
                        "numWorkers", config.NumWorkers
                    }
                };
                var queueType =
                    queueConfig.MessageTypes[queueConfig.MessageQueues[config.MessageQueue].MessageType].Type;
                registrations.AddRange(RegisterQueueProcessorAndNode(queueType, parameters));
            }

            container.Register(registrations.ToArray());
        }

        public static void RegisterQueueProcessorsAndNodes(IEnumerable<Type> queueTypes, IEnumerable<Type> logicTypes,
            IWindsorContainer container)
        {
            var registrations = new List<IRegistration>();

            var enumerable = queueTypes as Type[] ?? queueTypes.ToArray();

            registrations.AddRange(
                enumerable.Select(
                    type =>
                        Component.For(typeof(IQueueProcessor<>).MakeGenericType(type))
                            .ImplementedBy(typeof(QueueProcessor<>).MakeGenericType(type)))
                    .Cast<IRegistration>()
                    .ToArray());
            registrations.AddRange(
                enumerable.Select(
                    type =>
                        Component.For(typeof(IQueueProcessorNode<>).MakeGenericType(type))
                            .ImplementedBy(typeof(QueueProcessorNode<>).MakeGenericType(type)))
                    .Cast<IRegistration>()
                    .ToArray());
            registrations.AddRange(
                enumerable.Select(
                    type =>
                        Component.For(typeof(IWritableQueueProcessorNode<>).MakeGenericType(type))
                            .ImplementedBy(typeof(WritableQueueProcessorNode<>).MakeGenericType(type)))
                    .Cast<IRegistration>()
                    .ToArray());

            var types = logicTypes as Type[] ?? logicTypes.ToArray();
            registrations.AddRange(
                types.Select(
                    type =>
                        Component.For(
                            typeof(IMessageProcessingLogic<>).MakeGenericType(
                                type.GetInterfaces().First().GenericTypeArguments.First())).ImplementedBy(type))
                    .Cast<IRegistration>()
                    .ToArray());
            registrations.AddRange(
                types.Select(
                    type =>
                        Component.For(
                            typeof(IGenericFactory<>).MakeGenericType(
                                typeof(IMessageProcessingLogic<>).MakeGenericType(
                                    type.GetInterfaces().First().GenericTypeArguments.First()))).AsFactory())
                    .Cast<IRegistration>()
                    .ToArray());

            container.Register(registrations.ToArray());
        }

        private static IRegistration[] RegisterQueueProcessorAndNode(Type queueType,
            Dictionary<string, object> queueProcessorDependencies)
        {
            var registrations = new List<IRegistration>(3);
            registrations.Add(
                Component.For(typeof(IQueueProcessor<>).MakeGenericType(queueType))
                    .ImplementedBy(typeof(QueueProcessor<>).MakeGenericType(queueType))
                    .DependsOn(queueProcessorDependencies));
            registrations.Add(
                Component.For(typeof(IQueueProcessorNode<>).MakeGenericType(queueType))
                    .ImplementedBy(typeof(QueueProcessorNode<>).MakeGenericType(queueType)));
            registrations.Add(Component.For(typeof(IQueueProcessorNodeFactory<>).MakeGenericType(queueType)).AsFactory());
            registrations.Add(
                Component.For(typeof(IWritableQueueProcessorNode<>).MakeGenericType(queueType))
                    .ImplementedBy(typeof(WritableQueueProcessorNode<>).MakeGenericType(queueType)));
            return registrations.ToArray();
        }
    }
}