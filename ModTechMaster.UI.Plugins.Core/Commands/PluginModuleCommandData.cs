namespace ModTechMaster.UI.Plugins.Core.Commands
{
    using System.Windows.Controls;
    using Interfaces;

    public class PluginModuleCommandData
    {
        public PluginModuleCommandData(ContentControl containerFrame, IPluginModule module)
        {
            this.ContainerFrame = containerFrame;
            this.Module = module;
        }

        public ContentControl ContainerFrame { get; }
        public IPluginModule Module { get; }
    }
}