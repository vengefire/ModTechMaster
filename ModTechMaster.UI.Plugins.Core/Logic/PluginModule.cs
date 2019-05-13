using System.Collections.Generic;

namespace ModTechMaster.UI.Plugins.Core.Logic
{
    using System;
    using Interfaces;

    public class PluginModule : IPluginModule
    {
        public PluginModule(string moduleName, Type pageType, List<IPluginCommand> commands)
        {
            this.ModuleName = moduleName;
            this.PageType = pageType;
            Commands = commands;
        }

        public string ModuleName { get; }
        public Type PageType { get; }
        public List<IPluginCommand> Commands { get; }
    }
}