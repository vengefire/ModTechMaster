namespace ModTechMaster.UI.Plugins.ModCopy.Nodes
{
    using System.Linq;

    using ModTechMaster.Core.Interfaces.Models;
    using ModTechMaster.Core.Interfaces.Services;

    public sealed class ModNode : MtmTreeViewItem
    {
        public ModNode(IMod mod, MtmTreeViewItem parent, IReferenceFinderService referenceFinderService)
            : base(parent, mod, referenceFinderService)
        {
            this.Mod = mod;
            if (this.Mod.Manifest != null)
            {
                this.Manifest = new ManifestNode(mod.Manifest, this, referenceFinderService);
                this.Children.Add(this.Manifest);
            }

            this.Mod.ResourceFiles.ToList()
                .ForEach(definition => this.Children.Add(new ResourceNode(this, definition, referenceFinderService)));
        }

        public override string HumanReadableContent => this.Mod.JsonString;

        public IMod Mod { get; }

        public override string Name => this.Mod.Name;

        private ManifestNode Manifest { get; }
    }
}