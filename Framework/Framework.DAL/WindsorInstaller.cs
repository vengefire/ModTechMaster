namespace Framework.Data
{
    using System.Diagnostics.CodeAnalysis;
    using Castle.Core;
    using Castle.Facilities.TypedFactory;
    using Castle.MicroKernel.Registration;
    using Castle.MicroKernel.SubSystems.Configuration;
    using Castle.Windsor;
    using Interfaces.Data.Services;
    using Interfaces.Factories;
    using MessageQueue;

    [SuppressMessage(
        "StyleCop.CSharp.ReadabilityRules",
        "SA1118:ParameterMustNotSpanMultipleLines",
        Justification = "Reviewed.")]
    public class WindsorInstaller_Defunct : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                               Component.For<IMessageQueueService>()
                                        .ImplementedBy<MessageQueueService>()
                                        .LifestyleTransient()
                                        .Interceptors(InterceptorReference.ForKey("LogAspect")).First
                                        .Interceptors(InterceptorReference.ForKey("EFDBEntityValidationExceptionAspect")).Anywhere,
                               Component.For<IMessageQueueServiceFactory>().AsFactory());
        }
    }
}