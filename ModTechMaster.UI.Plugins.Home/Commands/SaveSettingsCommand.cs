namespace ModTechMaster.UI.Plugins.Home.Commands
{
    using System;
    using Core.Interfaces;
    using Models;
    using ModTechMaster.Core.Interfaces.Services;

    public class SaveSettingsCommand : IPluginCommand
    {
        private readonly HomeSettings settings;
        private readonly ISettingsService settingsService;

        public SaveSettingsCommand(ISettingsService settingsService, HomeSettings settings)
        {
            this.settingsService = settingsService;
            this.settings = settings;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            this.settingsService.SaveSettings("HomeSettings", this.settings);
        }

        public event EventHandler CanExecuteChanged;
        public string Name => @"Save Settings";
        public IPluginCommandCategory Category { get; }
        public object CommandParameter => this.settings;
    }
}