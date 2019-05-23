namespace ModTechMaster.UI.Plugins.ModCopy.Plugin
{
    using System;
    using System.Collections.Generic;

    using ModTechMaster.UI.Plugins.Core.Interfaces;
    using ModTechMaster.UI.Plugins.Core.Logic;

    public class ModCopyPlugin : IPlugin
    {
        public ModCopyPlugin()
        {
            this.Modules = new List<IPluginControl> { new PluginControl(@"ModCopy", typeof(ModCopyPage), null) };
        }

        public string Description =>
            "Processes a collection of mods and allows the selection of specific mods with cross reference validation.";

        public List<IPluginControl> Modules { get; }

        public string Name => "ModCopy";

        public Type PageType => typeof(ModCopyPage);

        public List<IPluginCommandCategory> PluginCommands { get; }
    }
}