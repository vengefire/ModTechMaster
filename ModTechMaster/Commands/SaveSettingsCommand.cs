using System;
using System.Windows.Input;
using ModTechMaster.Core.Interfaces.Services;
using ModTechMaster.UI.Plugins.Core.Interfaces;

namespace ModTechMaster.UI.Commands
{
    public class SaveSettingsCommand : ICommand
    {
        private readonly ISettingsService settingsService;

        public SaveSettingsCommand(ISettingsService settingsService)
        {
            this.settingsService = settingsService;
        }

        public string Name => @"Save Settings";
        public IPluginCommandCategory Category { get; }

        public bool CanExecute(object parameter)
        {
            return true;
        }
            
        public void Execute(object parameter)
        {
            var plugin = parameter as IPluginControl;
            settingsService.SaveSettings(plugin.ModuleName, plugin.Settings);
        }

        public event EventHandler CanExecuteChanged;
    }
}