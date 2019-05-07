namespace Framework.Logic.Queue.SubResolvers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Castle.Core;
    using Castle.MicroKernel;
    using Castle.MicroKernel.Context;
    using Config;
    using Config.QueueProcessor;
    using Interfaces.Queue;

    public class OpenQueueProcessorArraySubResolver : ISubDependencyResolver
    {
        private readonly IKernel kernel;

        private readonly IEnumerable<Type> queueProcessorTypes;

        public OpenQueueProcessorArraySubResolver(IKernel kernel, IEnumerable<Type> queueProcessorTypes)
        {
            this.kernel = kernel;
            this.queueProcessorTypes = queueProcessorTypes;
        }

        public OpenQueueProcessorArraySubResolver(IKernel kernel, QueueConfigSectionHandler queueConfig)
        {
            this.kernel = kernel;
            var messageTypes = new List<Type>(queueConfig.QueueProcessors.Count);
            messageTypes.AddRange(
                                  from QueueProcessorElement processor in queueConfig.QueueProcessors
                                  select queueConfig.MessageTypes[queueConfig.MessageQueues[processor.MessageQueue].MessageType].Type);
            this.queueProcessorTypes = messageTypes;
        }

        public bool CanResolve(
            CreationContext context,
            ISubDependencyResolver contextHandlerResolver,
            ComponentModel model,
            DependencyModel dependency)
        {
            var canResolve = dependency.TargetItemType != null && dependency.TargetItemType.IsArray &&
                             dependency.TargetItemType.GetElementType() == typeof(IQueueProcessor);
            return canResolve;
        }

        public object Resolve(
            CreationContext context,
            ISubDependencyResolver contextHandlerResolver,
            ComponentModel model,
            DependencyModel dependency)
        {
            return this.queueProcessorTypes.Select(type => this.kernel.Resolve(typeof(IQueueProcessor<>).MakeGenericType(type)) as IQueueProcessor)
                       .ToArray();
        }
    }
}