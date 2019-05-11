namespace ModTechMaster.Plugins.ModCopy
{
    using System.Collections.Generic;
    using UI.Plugins.Core.Interfaces;

    public class ModCopyPlugin : IPlugin
    {
        public string Name => "ModCopy";
        public string Description => "Processes a collection of mods and allows the selection of specific mods with cross reference validation.";
        public List<IPluginCommand> PluginCommands { get; }
    }
}