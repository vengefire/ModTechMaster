namespace ModTechMaster.UI.Plugins.Core.Interfaces
{
    using System.Windows.Input;

    public interface IPluginCommand : ICommand
    {
        string Name { get; }
        IPluginCommandCategory Category { get; }
        object CommandParameter { get; }
    }
}