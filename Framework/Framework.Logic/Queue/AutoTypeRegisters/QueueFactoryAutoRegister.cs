namespace Framework.Logic.Queue.AutoTypeRegisters
{
    using System;
    using System.Collections.Generic;
    using Castle.Facilities.TypedFactory;
    using Castle.MicroKernel.Registration;
    using Castle.Windsor;
    using Interfaces.Factories;

    public static class QueueFactoryAutoRegister
    {
        public static void RegisterQueueFactories(IEnumerable<Type> types, IWindsorContainer container)
        {
            var registrations = new List<IRegistration>();
            var readFactoryType = typeof(IReadQueueFactory<>);
            var writeFactoryType = typeof(IWriteQueueFactory<>);
            foreach (var type in types)
            {
                var targetedReadFactory = readFactoryType.MakeGenericType(type);
                var targetedWriteFactory = writeFactoryType.MakeGenericType(type);
                var readFactoryFactory = typeof(IGenericFactory<>).MakeGenericType(targetedReadFactory);
                var writeFactoryFactory = typeof(IGenericFactory<>).MakeGenericType(targetedWriteFactory);
                registrations.Add(Component.For(targetedReadFactory).AsFactory());
                registrations.Add(Component.For(targetedWriteFactory).AsFactory());
                registrations.Add(Component.For(readFactoryFactory).LifestyleSingleton().AsFactory());
                registrations.Add(Component.For(writeFactoryFactory).LifestyleSingleton().AsFactory());
            }

            container.Register(registrations.ToArray());
        }
    }
}