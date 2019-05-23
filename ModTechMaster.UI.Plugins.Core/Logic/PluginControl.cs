namespace ModTechMaster.UI.Plugins.Core.Logic
{
    using System;
    using System.Collections.Generic;

    using ModTechMaster.UI.Plugins.Core.Interfaces;

    public class PluginControl : IPluginControl
    {
        public PluginControl(string moduleName, Type pageType, List<IPluginCommand> commands)
        {
            this.ModuleName = moduleName;
            this.PageType = pageType;
            this.PluginCommands = commands;
        }

        public string ModuleName { get; }

        public Type PageType { get; }

        public List<IPluginCommand> PluginCommands { get; }

        public object Settings { get; set; }

        public virtual Type SettingsType { get; }
    }
}