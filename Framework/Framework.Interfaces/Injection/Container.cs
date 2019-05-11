namespace Framework.Interfaces.Injection
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    ///     Proxy to IOC container.
    /// </summary>
    public class Container : IContainer
    {
        private static IContainer instance;

        private readonly IContainer proxy;

        private Container(IContainer target)
        {
            this.proxy = target;
        }

        public static IContainer Instance
        {
            get
            {
                if (!Container.InstanceRegistered())
                {
                    throw new InvalidOperationException("Container has not been registered.");
                }

                return Container.instance;
            }
        }

        public bool ConfigurationIsValid(out string details)
        {
            return this.proxy.ConfigurationIsValid(out details);
        }

        public bool ConfigurationIsValid(Type type, out string details)
        {
            return this.proxy.ConfigurationIsValid(type, out details);
        }

        public T GetInstance<T>()
        {
            return this.proxy.GetInstance<T>();
        }

        public T GetInstance<T>(IEnumerable<KeyValuePair<string,object>>args)
        {
            return this.proxy.GetInstance<T>(args);
        }

        public T GetInstance<T>(string name)
        {
            return this.proxy.GetInstance<T>(name);
        }

        public object GetInstance(Type type)
        {
            return this.proxy.GetInstance(type);
        }

        public IEnumerable<T> GetAllInstances<T>()
        {
            return this.proxy.GetAllInstances<T>();
        }

        public IEnumerable GetAllInstances(Type type)
        {
            return this.proxy.GetAllInstances(type);
        }

        public void Release(object instance)
        {
            this.proxy.Release(instance);
        }

        public static void RegisterContainer(IContainer container)
        {
            if (Container.InstanceRegistered())
            {
                throw new InvalidOperationException("Container has already been registered.");
            }

            Container.instance = new Container(container);
        }

        public static void DeregisterContainer()
        {
            if (!Container.InstanceRegistered())
            {
                throw new InvalidOperationException("Container has not been registered.");
            }

            Container.Instance.Release(Container.Instance);
            Container.instance = null;
        }

        private static bool InstanceRegistered()
        {
            return Container.instance != null;
        }
    }
}