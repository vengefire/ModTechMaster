namespace Framework.Logic.Queue.AutoTypeRegisters
{
    using System;
    using System.Collections.Generic;
    using Castle.Facilities.TypedFactory;
    using Castle.MicroKernel.Registration;
    using Castle.Windsor;
    using Interfaces.Factories;

    public static class GenericFactoryAutoRegister
    {
        public static void RegisterGenericFactories(IEnumerable<Type> types, IWindsorContainer container)
        {
            var registrations = new List<IRegistration>();
            var factoryType = typeof(IGenericFactory<>);
            foreach (var type in types)
            {
                var genericizedType = factoryType.MakeGenericType(type);
                registrations.Add(Component.For(genericizedType).AsFactory());
                var factoryFactoryType = factoryType.MakeGenericType(genericizedType);
                registrations.Add(Component.For(factoryFactoryType).LifestyleSingleton().AsFactory());
            }

            container.Register(registrations.ToArray());
        }
    }
}