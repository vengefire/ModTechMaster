using ModTechMaster.Core.Interfaces.Models;

namespace ModTechMaster.Nodes
{
    public sealed class ModNode : MTMTreeViewItem
    {
        public ModNode(IMod mod, MTMTreeViewItem parent) : base(parent)
        {
            Mod = mod;
            if (Mod.Manifest != null)
            {
                Manifest = new ManifestNode(mod.Manifest, this);
                Children.Add(Manifest);
            }
        }

        private ManifestNode Manifest { get; }
        public IMod Mod { get; }
        public override string Name => Mod.Name;
    }
}