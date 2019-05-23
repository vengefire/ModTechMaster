namespace ModTechMaster.UI.Plugins.Home.Commands
{
    using System;

    using ModTechMaster.Core.Interfaces.Services;
    using ModTechMaster.UI.Plugins.Core.Interfaces;
    using ModTechMaster.UI.Plugins.Home.Models;

    public class SaveSettingsCommand : IPluginCommand
    {
        private readonly HomeSettings settings;

        private readonly ISettingsService settingsService;

        public SaveSettingsCommand(ISettingsService settingsService, HomeSettings settings)
        {
            this.settingsService = settingsService;
            this.settings = settings;
        }

        public event EventHandler CanExecuteChanged;

        public IPluginCommandCategory Category { get; }

        public object CommandParameter => this.settings;

        public string Name => @"Save Settings";

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            this.settingsService.SaveSettings("HomeSettings", this.settings);
        }
    }
}