using System.Windows.Input;
using ModTechMaster.Core.Interfaces.Services;

namespace ModTechMaster.UI.Commands
{
    public static class CommonCommands
    {
        static CommonCommands()
        {
            SettingsService = App.Container.GetInstance<ISettingsService>();
            SaveCurrentSettingsCommand = new SaveSettingsCommand(SettingsService);
            LoadCurrentSettingsCommand = new LoadSettingsCommand(SettingsService);
        }

        public static ISettingsService SettingsService { get; }
        public static ICommand SaveCurrentSettingsCommand { get; }
        public static ICommand LoadCurrentSettingsCommand { get; }
    }
}