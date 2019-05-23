namespace ModTechMaster.UI.Commands
{
    using System;
    using System.Windows.Input;

    using ModTechMaster.Core.Interfaces.Services;
    using ModTechMaster.UI.Plugins.Core.Interfaces;

    public class LoadSettingsCommand : ICommand
    {
        private readonly ISettingsService settingsService;

        public LoadSettingsCommand(ISettingsService settingsService)
        {
            this.settingsService = settingsService;
        }

        public event EventHandler CanExecuteChanged;

        public IPluginCommandCategory Category { get; }

        public string Name => @"Load Settings";

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var plugin = parameter as IPluginControl;
            plugin.Settings = this.settingsService.ReadSettings(plugin.ModuleName, plugin.SettingsType);
        }
    }
}