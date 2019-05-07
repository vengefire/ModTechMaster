namespace Framework.Windsor.Installer
{
    using System.Diagnostics.CodeAnalysis;
    using Castle.DynamicProxy;
    using Castle.MicroKernel.Registration;
    using Castle.MicroKernel.SubSystems.Configuration;
    using Castle.Windsor;
    using Interceptors;

    [SuppressMessage(
        "StyleCop.CSharp.ReadabilityRules",
        "SA1118:ParameterMustNotSpanMultipleLines",
        Justification = "Reviewed.")]
    public class AspectsInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                               Component.For<IInterceptor>()
                                        .ImplementedBy<LogAspect>()
                                        .Named("LogAspect")
                                        .LifeStyle.Transient.DependsOn(Dependency.OnAppSettingsValue("SkipLog", "SkipServiceInstrumentLog")),
                               Component.For<IInterceptor>()
                                        .ImplementedBy<LogAspect>()
                                        .Named("LogEntryAspect")
                                        .LifeStyle.Transient.DependsOn(Parameter.ForKey("EntryLog").Eq("true")),
                               Component.For<IInterceptor>()
                                        .ImplementedBy<EFDBEntityValidationExceptionAspect>()
                                        .Named("EFDBEntityValidationExceptionAspect")
                                        .LifeStyle.Transient);
        }
    }
}