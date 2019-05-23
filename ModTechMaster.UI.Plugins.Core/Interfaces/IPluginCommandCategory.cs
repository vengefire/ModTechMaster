namespace ModTechMaster.UI.Plugins.Core.Interfaces
{
    using System.Collections.Generic;

    public interface IPluginCommandCategory
    {
        List<IPluginCommand> Commands { get; }

        string Name { get; }
    }
}