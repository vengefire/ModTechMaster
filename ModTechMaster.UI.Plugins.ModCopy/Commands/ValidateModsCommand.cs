namespace ModTechMaster.UI.Plugins.ModCopy.Commands
{
    using System;

    using ModTechMaster.UI.Plugins.Core.Interfaces;

    public class ValidateModsCommand : IPluginCommand
    {
        public ValidateModsCommand(IPluginCommandCategory category)
        {
            this.Category = category;
        }

        public event EventHandler CanExecuteChanged;

        public IPluginCommandCategory Category { get; }

        public object CommandParameter => null;

        public string Name => @"Validate Mods";

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            throw new NotImplementedException();
        }
    }
}