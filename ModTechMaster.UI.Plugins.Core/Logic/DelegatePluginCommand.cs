namespace ModTechMaster.UI.Plugins.Core.Logic
{
    using System;

    using ModTechMaster.UI.Plugins.Core.Interfaces;

    public class DelegatePluginCommand : DelegateCommand, IPluginCommand
    {
        public DelegatePluginCommand(Action executeMethod, object commandParameter, string name)
            : base(executeMethod)
        {
            this.CommandParameter = commandParameter;
            this.Name = name;
        }

        public DelegatePluginCommand(
            Action executeMethod,
            Func<bool> canExecuteMethod,
            object commandParameter,
            string name)
            : base(executeMethod, canExecuteMethod)
        {
            this.CommandParameter = commandParameter;
            this.Name = name;
        }

        public IPluginCommandCategory Category { get; }

        public object CommandParameter { get; }

        public string Name { get; }
    }
}