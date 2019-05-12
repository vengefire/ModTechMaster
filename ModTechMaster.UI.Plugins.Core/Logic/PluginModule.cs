namespace ModTechMaster.UI.Plugins.Core.Logic
{
    using System;
    using Interfaces;

    public class PluginModule : IPluginModule
    {
        public PluginModule(string moduleName, Type pageType)
        {
            this.ModuleName = moduleName;
            this.PageType = pageType;
        }

        public string ModuleName { get; }
        public Type PageType { get; }
    }
}