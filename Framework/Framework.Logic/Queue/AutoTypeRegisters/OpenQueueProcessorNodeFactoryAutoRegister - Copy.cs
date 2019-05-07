namespace Framework.Logic.Queue.AutoTypeRegisters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Castle.Facilities.TypedFactory;
    using Castle.MicroKernel.Registration;
    using Castle.Windsor;
    using Interfaces.Factories;

    public static class QueueProcessorNodeFactoryAutoRegister
    {
        public static void RegisterQueueProcessorNodeFactories(IEnumerable<Type> types, IWindsorContainer container)
        {
            container.Register(
                               types.Select(type => Component.For(typeof(IQueueProcessorNodeFactory<>).MakeGenericType(type)).AsFactory())
                                    .Cast<IRegistration>()
                                    .ToArray());
        }
    }
}