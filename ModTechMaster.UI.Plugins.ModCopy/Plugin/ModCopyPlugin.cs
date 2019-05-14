using System;

namespace ModTechMaster.UI.Plugins.ModCopy.Plugin
{
    using System.Collections.Generic;
    using Core.Interfaces;
    using Core.Logic;

    public class ModCopyPlugin : IPlugin
    {
        public ModCopyPlugin()
        {
            this.Modules = new List<IPluginControl>
            {
                new PluginControl(@"ModCopy", typeof(ModCopyPage), null)
            };
        }

        public string Name => "ModCopy";
        public string Description => "Processes a collection of mods and allows the selection of specific mods with cross reference validation.";
        public Type PageType => typeof(ModCopyPage);
        public List<IPluginControl> Modules { get; }
        public List<IPluginCommandCategory> PluginCommands { get; }
    }
}