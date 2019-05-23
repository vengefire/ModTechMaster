namespace ModTechMaster.UI.Commands
{
    using System;
    using System.Windows.Input;

    using ModTechMaster.Core.Interfaces.Services;
    using ModTechMaster.UI.Plugins.Core.Interfaces;

    public class SaveSettingsCommand : ICommand
    {
        private readonly ISettingsService settingsService;

        public SaveSettingsCommand(ISettingsService settingsService)
        {
            this.settingsService = settingsService;
        }

        public event EventHandler CanExecuteChanged;

        public IPluginCommandCategory Category { get; }

        public string Name => @"Save Settings";

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var plugin = parameter as IPluginControl;
            this.settingsService.SaveSettings(plugin.ModuleName, plugin.Settings);
        }
    }
}