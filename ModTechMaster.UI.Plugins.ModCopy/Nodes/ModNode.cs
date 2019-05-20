namespace ModTechMaster.UI.Plugins.ModCopy.Nodes
{
    using System.Linq;
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

            this.Mod.ResourceFiles.ToList().ForEach(definition => this.Children.Add(new ResourceNode(this, definition)));
        }

        private ManifestNode Manifest { get; }
        public IMod Mod { get; }
        public override string Name => this.Mod.Name;
        public override string HumanReadableContent => this.Mod.JsonString;
    }
}