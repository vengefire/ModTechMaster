namespace ModTechMaster.UI.Plugins.Core.Interfaces
{
    using System.Windows.Input;

    public interface IPluginCommand : ICommand
    {
        IPluginCommandCategory Category { get; }

        object CommandParameter { get; }

        string Name { get; }
    }
}