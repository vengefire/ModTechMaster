namespace ModTechMaster.UI.Plugins.ModCopy.Plugin
{
    using System.Collections.Generic;
    using Core.Interfaces;

    public class ModCopyPlugin : IPlugin
    {
        public ModCopyPlugin()
        {
            this.Modules = new List<IPluginModule>
            {
                new ModCopyPage()
            };
        }

        public string Name => "ModCopy";
        public string Description => "Processes a collection of mods and allows the selection of specific mods with cross reference validation.";
        public List<IPluginModule> Modules { get; }
        public List<IPluginCommandCategory> PluginCommands { get; }
    }
}