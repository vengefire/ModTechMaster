namespace ModTechMaster.UI
{
    using System.Windows.Controls;
    using Plugins.Core.Interfaces;

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