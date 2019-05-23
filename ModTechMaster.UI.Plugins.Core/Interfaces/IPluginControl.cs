namespace ModTechMaster.UI.Plugins.Core.Interfaces
{
    using System;
    using System.Collections.Generic;

    public interface IPluginControl
    {
        string ModuleName { get; }

        List<IPluginCommand> PluginCommands { get; }

        object Settings { get; set; }

        Type SettingsType { get; }
    }
}