namespace ModTechMaster.UI.Plugins.Core.Commands
{
    using System;
    using System.Windows.Input;

    public class PluginModuleCommand : ICommand
    {
        private object _moduleObject;
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var data = (PluginModuleCommandData)parameter;
            if (this._moduleObject == null)
            {
                this._moduleObject = Activator.CreateInstance(data.Module.PageType);
            }
            
            data.ContainerFrame.Content = this._moduleObject;
        }

        public event EventHandler CanExecuteChanged { add => CommandManager.RequerySuggested += value; remove => CommandManager.RequerySuggested -= value; }
    }
}