using System;
using System.Configuration;
using Castle.Core;
using Castle.MicroKernel;
using Castle.MicroKernel.Context;

namespace Framework.Logic.Queue
{
    public class QueueSubResolver : ISubDependencyResolver
    {
        public bool CanResolve(CreationContext context, ISubDependencyResolver contextHandlerResolver,
            ComponentModel model, DependencyModel dependency)
        {
            var canResolve = dependency.IsPrimitiveTypeDependency &&
                             dependency.DependencyKey == "messageQueueHostServerName";
            return canResolve;
        }

        public object Resolve(CreationContext context, ISubDependencyResolver contextHandlerResolver,
            ComponentModel model, DependencyModel dependency)
        {
            var host = ConfigurationManager.AppSettings["MessageQueueHostServerName"];
            if (null == host)
            {
                throw new InvalidProgramException("MessageQueueHostServerName missing from configuration store.");
            }

            return ConfigurationManager.AppSettings["MessageQueueHostServerName"];
        }
    }
}