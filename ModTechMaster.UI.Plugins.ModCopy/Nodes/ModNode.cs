namespace ModTechMaster.UI.Plugins.ModCopy.Nodes
{
    using ModTechMaster.Core.Interfaces.Models;

    public sealed class ModNode : MtmTreeViewItem
    {
        public ModNode(IMod mod, MtmTreeViewItem parent) : base(parent, mod)
        {
            this.Mod = mod;
            if (this.Mod.Manifest != null)
            {
                this.Manifest = new ManifestNode(mod.Manifest, this);
                this.Children.Add(this.Manifest);
            }
        }

        private ManifestNode Manifest { get; }
        public IMod Mod { get; }
        public override string Name => this.Mod.Name;
        public override string HumanReadableContent => this.Mod.JsonString;
    }
}