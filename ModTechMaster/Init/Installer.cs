using System.Windows;
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

namespace ModTechMaster.UI.Init
{
    public class Installer : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<Window>().ImplementedBy<MainWindow>(),
                Component.For<ILogger>().ImplementedBy<Log4netLogger>(),
                Component.For<IExceptionLogger>().ImplementedBy<ExceptionLogger>(),
                Component.For<IMessageService>().ImplementedBy<MessageService>(),
                Component.For<ISettingsService>().ImplementedBy<SettingsService>(),
                Component.For<IModService>().ImplementedBy<ModService>(),
                Component.For<IReferenceFinderService>().ImplementedBy<ReferenceFinderService>(),
                Component.For<IManifestEntryProcessorFactory>().ImplementedBy<ManifestEntryProcessorFactory>()
            );
        }
    }
}