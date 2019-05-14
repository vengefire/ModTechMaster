using System.Collections.Generic;

namespace ModTechMaster.UI.Plugins.Core.Logic
{
    using System;
    using Interfaces;

    public class PluginControl : IPluginControl
    {
        public PluginControl(string moduleName, Type pageType, List<IPluginCommand> commands)
        {
            this.ModuleName = moduleName;
            this.PageType = pageType;
            PluginCommands = commands;
        }

        public string ModuleName { get; }
        public Type PageType { get; }
        public List<IPluginCommand> PluginCommands { get; }
    }
}