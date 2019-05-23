namespace ModTechMaster.UI
{
    using System.Windows;

    using Framework.Interfaces.Injection;

    using ModTechMaster.UI.Init;

    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IContainer Container { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            var bootstrapper = new Bootstrap();
            Container = bootstrapper.RegisterContainer();
            var mainWindow = Container.GetInstance<Window>();
            mainWindow.Show();
        }
    }
}