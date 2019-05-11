namespace ModTechMaster.UI
{
    using System;
    using System.Windows.Input;

    public class PluginModuleCommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var data = (PluginModuleCommandData)parameter;
            var modulePage = Activator.CreateInstance(data.Module.PageType);
            data.ContainerFrame.Content = modulePage;
        }

        public event EventHandler CanExecuteChanged { add => CommandManager.RequerySuggested += value; remove => CommandManager.RequerySuggested -= value; }
    }
}