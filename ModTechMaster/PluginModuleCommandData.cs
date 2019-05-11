namespace ModTechMaster.UI
{
    using System.Windows.Controls;
    using Plugins.Core.Interfaces;

    public class PluginModuleCommandData
    {
        public PluginModuleCommandData(Frame containerFrame, IPluginModule module)
        {
            this.ContainerFrame = containerFrame;
            this.Module = module;
        }

        public Frame ContainerFrame { get; }
        public IPluginModule Module { get; }
    }
}