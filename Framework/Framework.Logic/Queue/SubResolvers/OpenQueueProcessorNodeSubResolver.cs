namespace Framework.Logic.Queue.SubResolvers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Castle.Core;
    using Castle.MicroKernel;
    using Castle.MicroKernel.Context;
    using Interfaces.Queue;

    public class OpenQueueProcessorNodeSubResolver : ISubDependencyResolver
    {
        private readonly IKernel kernel;

        private readonly IEnumerable<Type> queueProcessorNodeTypes;

        public OpenQueueProcessorNodeSubResolver(IKernel kernel, IEnumerable<Type> queueProcessorNodeTypes)
        {
            this.kernel = kernel;
            this.queueProcessorNodeTypes = queueProcessorNodeTypes;
        }

        public bool CanResolve(
            CreationContext context,
            ISubDependencyResolver contextHandlerResolver,
            ComponentModel model,
            DependencyModel dependency)
        {
            var canResolve = dependency.TargetItemType != null &&
                             dependency.TargetItemType == typeof(IQueueProcessorNode<>);
            return canResolve;
        }

        public object Resolve(
            CreationContext context,
            ISubDependencyResolver contextHandlerResolver,
            ComponentModel model,
            DependencyModel dependency)
        {
            return this.queueProcessorNodeTypes.Select(type => this.kernel.Resolve(typeof(IQueueProcessorNode<>).MakeGenericType(type)));
        }
    }
}