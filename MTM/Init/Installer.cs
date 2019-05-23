namespace MTM.Init
{
    using Castle.Core.Logging;
    using Castle.MicroKernel.Registration;
    using Castle.MicroKernel.SubSystems.Configuration;
    using Castle.Services.Logging.Log4netIntegration;
    using Castle.Windsor;

    using Framework.Interfaces.Logging;

    using ModTechMaster.Core.Interfaces.Factories;
    using ModTechMaster.Core.Interfaces.Services;
    using ModTechMaster.Logic.Factories;
    using ModTechMaster.Logic.Services;

    public class Installer : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<ILogger>().ImplementedBy<Log4netLogger>(),
                Component.For<IExceptionLogger>().ImplementedBy<ExceptionLogger>(),
                Component.For<IMessageService>().ImplementedBy<MessageService>(),
                Component.For<IModService>().ImplementedBy<ModService>(),
                Component.For<IReferenceFinderService>().ImplementedBy<ReferenceFinderService>(),
                Component.For<IManifestEntryProcessorFactory>().ImplementedBy<ManifestEntryProcessorFactory>());
        }
    }
}