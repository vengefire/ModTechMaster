using System;
using System.Collections.Generic;
using ModTechMaster.UI.Plugins.Core.Interfaces;

namespace ModTechMaster.UI.Plugins.Home
{
    public class HomePlugin : IPlugin
    {
        public string Name => @"ModTechMaster - Home";
        public string Description => @"The basic home modules for Mod Tech Master.";
        public Type PageType => typeof(ModTechMasterHome);
        public List<IPluginControl> Modules { get; }
    }
}