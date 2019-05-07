using System;
using System.Collections.Generic;
using System.Linq;
using Castle.Facilities.TypedFactory;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Framework.Interfaces.Factories;

namespace Framework.Logic.Queue.AutoTypeRegisters
{
    public static class QueueProcessorNodeFactoryAutoRegister
    {
        public static void RegisterQueueProcessorNodeFactories(IEnumerable<Type> types, IWindsorContainer container)
        {
            container.Register(
                types.Select(
                    type => Component.For(typeof(IQueueProcessorNodeFactory<>).MakeGenericType(type)).AsFactory())
                    .Cast<IRegistration>()
                    .ToArray());
        }
    }
}