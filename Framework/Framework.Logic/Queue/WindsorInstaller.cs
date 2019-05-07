using System.Diagnostics.CodeAnalysis;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace Framework.Logic.Queue
{
    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines",
        Justification = "Reviewed.")]
    public class WindsorInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<QueueSubResolver>());
            container.Kernel.Resolver.AddSubResolver(container.Kernel.Resolve<QueueSubResolver>());
        }
    }
}