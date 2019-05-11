namespace ModTechMaster.UI.Plugins.ModCopy.Nodes
{
    using System.Linq;
    using ModTechMaster.Core.Interfaces.Models;

    public class ModCollectionNode : MTMTreeViewItem
    {
        public ModCollectionNode(IModCollection modCollection, MTMTreeViewItem parent) : base(parent)
        {
            this.ModCollection = modCollection;
            this.ModCollection.Mods.ToList().ForEach(mod => this.Children.Add(new ModNode(mod, this)));
        }

        public IModCollection ModCollection { get; }
        public override string Name => this.ModCollection.Name;
        public override string HumanReadableContent => this.ModCollection.Name;
    }
}