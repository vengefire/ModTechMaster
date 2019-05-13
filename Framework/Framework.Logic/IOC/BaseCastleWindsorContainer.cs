using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Castle.Core;
using Castle.MicroKernel;
using Castle.MicroKernel.Handlers;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.Windsor.Diagnostics;
using Framework.Interfaces.Injection;
using Framework.Interfaces.Logging;

namespace Framework.Logic.IOC
{
    /// <summary>
    ///     Castle Windsor container which implements common functionality.
    /// </summary>
    /// <remarks>
    ///     The default lifestyle is set as transient.
    /// </remarks>
    public abstract class BaseCastleWindsorContainer : IContainer, IDisposable
    {
        private readonly WindsorContainer container;

        protected BaseCastleWindsorContainer(Action<WindsorContainer> initializer)
        {
            Debug.Assert(initializer != null, "Initializer is required");

            container = new WindsorContainer();
            container.Kernel.ComponentModelCreated += ComponentModelCreated;

            initializer(container);

            container.Register(Component.For<IContainer>().Instance(this));
        }

        public bool ConfigurationIsValid(out string details)
        {
            var host = (IDiagnosticsHost) container.Kernel.GetSubSystem(SubSystemConstants.DiagnosticsKey);
            var diagnostics = host.GetDiagnostic<IPotentiallyMisconfiguredComponentsDiagnostic>();

            var handlers = diagnostics.Inspect();

            if (handlers.Any())
            {
                var message = new StringBuilder();
                var inspector = new DependencyInspector(message);

                foreach (IExposeDependencyInfo handler in handlers) handler.ObtainDependencyDetails(inspector);

                details = message.ToString();
                return false;
            }

            details = string.Empty;
            return true;
        }

        public bool ConfigurationIsValid(Type type, out string details)
        {
            if (!container.Kernel.HasComponent(type))
            {
                var sb = new StringBuilder();
                sb.AppendLine();
                sb.AppendFormat("'{0}' not registered.", type.FullName);
                sb.AppendLine();

                details = sb.ToString();
                return false;
            }

            var host = (IDiagnosticsHost) container.Kernel.GetSubSystem(SubSystemConstants.DiagnosticsKey);
            var diagnostics = host.GetDiagnostic<IPotentiallyMisconfiguredComponentsDiagnostic>();

            var handlers = diagnostics.Inspect().Where(x => x.Supports(type)).ToArray();
            if (handlers.Any())
            {
                var message = new StringBuilder();
                var inspector = new DependencyInspector(message);

                foreach (IExposeDependencyInfo handler in handlers) handler.ObtainDependencyDetails(inspector);

                details = message.ToString();
                return false;
            }

            details = string.Empty;
            return true;
        }

        public T GetInstance<T>()
        {
            return TransientResolve<T>();
        }

        public T GetInstance<T>(IEnumerable<KeyValuePair<string, object>> args)
        {
            return container.Resolve<T>(args);
        }

        public T GetInstance<T>(string name)
        {
            return container.Resolve<T>(name);
        }

        public object GetInstance(Type type)
        {
            return TransientResolve(type);
        }

        public IEnumerable<T> GetAllInstances<T>()
        {
            return container.ResolveAll<T>();
        }

        public IEnumerable GetAllInstances(Type type)
        {
            return container.ResolveAll(type);
        }

        public void Release(object instance)
        {
            container.Release(instance);
        }

        public void Dispose()
        {
            container.Dispose();
        }

        private void ComponentModelCreated(ComponentModel model)
        {
            if (model.LifestyleType == LifestyleType.Undefined) model.LifestyleType = LifestyleType.Transient;
        }

        private T TransientResolve<T>()
        {
            return (T) TransientResolve(typeof(T));
        }

        private object TransientResolve(Type type)
        {
            if (type.IsClass &&
                !container.Kernel.HasComponent(type))
            {
                var exceptionLogger = container.Resolve<IExceptionLogger>();
                exceptionLogger.Log(
                    new ComponentNotFoundException(
                        type,
                        string.Format(
                            "No component for supporting the service {0} was found. Silently registering type as transient.",
                            type.FullName)));

                container.Kernel.Register(Component.For(type).ImplementedBy(type).LifestyleTransient());
            }

            return container.Resolve(type);
        }
    }
}