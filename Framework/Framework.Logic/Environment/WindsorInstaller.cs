namespace Framework.Logic.Environment
{
    using System.Diagnostics.CodeAnalysis;
    using Castle.MicroKernel.Registration;
    using Castle.MicroKernel.SubSystems.Configuration;
    using Castle.Windsor;
    using Interfaces.Environment;

    [SuppressMessage(
        "StyleCop.CSharp.ReadabilityRules",
        "SA1118:ParameterMustNotSpanMultipleLines",
        Justification = "Reviewed.")]
    public class WindsorInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                               Component.For<IEnvironment>()
                                        .ImplementedBy<EnvironmentImpl>());
        }
    }
}