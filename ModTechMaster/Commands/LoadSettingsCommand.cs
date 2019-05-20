using System;
using System.Windows.Input;
using ModTechMaster.Core.Interfaces.Services;
using ModTechMaster.UI.Plugins.Core.Interfaces;

namespace ModTechMaster.UI.Commands
{
    public class LoadSettingsCommand : ICommand
    {
        private readonly ISettingsService settingsService;

        public LoadSettingsCommand(ISettingsService settingsService)
        {
            this.settingsService = settingsService;
        }

        public string Name => @"Load Settings";
        public IPluginCommandCategory Category { get; }

        public bool CanExecute(object parameter)
        {
            return true;
        }
            
        public void Execute(object parameter)
        {
            var plugin = parameter as IPluginControl;
            plugin.Settings = settingsService.ReadSettings(plugin.ModuleName, plugin.SettingsType);
        }

        public event EventHandler CanExecuteChanged;
    }
}