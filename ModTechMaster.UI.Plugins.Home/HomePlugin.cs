namespace ModTechMaster.UI.Plugins.Home
{
    using System;
    using System.Collections.Generic;

    using ModTechMaster.UI.Plugins.Core.Interfaces;

    public class HomePlugin : IPlugin
    {
        public string Description => @"The basic home modules for Mod Tech Master.";

        public List<IPluginControl> Modules { get; }

        public string Name => @"ModTechMaster - Home";

        public Type PageType => typeof(ModTechMasterHome);
    }
}