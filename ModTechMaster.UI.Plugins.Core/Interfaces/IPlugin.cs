namespace ModTechMaster.UI.Plugins.Core.Interfaces
{
    using System.Collections.Generic;

    public interface IPlugin
    {
        string Name { get; }
        string Description { get; }
        List<IPluginModule> Modules { get; }
        List<IPluginCommandCategory> PluginCommands { get; }
    }
}