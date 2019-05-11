namespace ModTechMaster.UI.Plugins.Core.Interfaces
{
    using System.Collections.Generic;

    public interface IPluginCommandCategory
    {
        string Name { get; }
        List<IPluginCommand> Commands { get; }
    }
}