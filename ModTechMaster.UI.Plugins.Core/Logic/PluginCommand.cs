namespace ModTechMaster.UI.Plugins.Core.Logic
{
    using System;

    using ModTechMaster.UI.Plugins.Core.Interfaces;

    public abstract class PluginCommandBase : IPluginCommand
    {
        protected PluginCommandBase(object commandParameter, string name)
        {
            this.CommandParameter = commandParameter;
            this.Name = name;
        }

        public event EventHandler CanExecuteChanged;

        public IPluginCommandCategory Category { get; }

        public object CommandParameter { get; }

        public string Name { get; }

        public bool CanExecute(object parameter)
        {
            throw new NotImplementedException();
        }

        public void Execute(object parameter)
        {
            throw new NotImplementedException();
        }
    }
}