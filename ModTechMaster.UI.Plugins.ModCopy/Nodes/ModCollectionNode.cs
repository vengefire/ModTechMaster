namespace ModTechMaster.UI.Plugins.ModCopy.Nodes
{
    using System.Linq;
    using ModTechMaster.Core.Interfaces.Models;

    public class ModCollectionNode : MtmTreeViewItem
    {
        public ModCollectionNode(IModCollection modCollection, MtmTreeViewItem parent) : base(parent, modCollection)
        {
            this.ModCollection = modCollection;
            this.ModCollection.Mods.OrderBy(mod => mod.Name).ToList().ForEach(mod => this.Children.Add(new ModNode(mod, this)));
        }

        public IModCollection ModCollection { get; }
        public override IReferenceableObjectProvider ReferenceableObjectProvider => this.ModCollection;
        public override string Name => this.ModCollection.Name;
        public override string HumanReadableContent => this.ModCollection.Name;
    }
}