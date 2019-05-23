namespace ModTechMaster.UI.Commands
{
    using System.Windows.Input;

    using ModTechMaster.Core.Interfaces.Services;

    public static class CommonCommands
    {
        static CommonCommands()
        {
            SettingsService = App.Container.GetInstance<ISettingsService>();
            SaveCurrentSettingsCommand = new SaveSettingsCommand(SettingsService);
            LoadCurrentSettingsCommand = new LoadSettingsCommand(SettingsService);
        }

        public static ICommand LoadCurrentSettingsCommand { get; }

        public static ICommand SaveCurrentSettingsCommand { get; }

        public static ISettingsService SettingsService { get; }
    }
}