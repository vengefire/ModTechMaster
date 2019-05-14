using System.Collections.Generic;

namespace ModTechMaster.UI.Plugins.Core.Interfaces
{
    public interface IPluginControl
    {
        List<IPluginCommand> PluginCommands { get; }
    }
}