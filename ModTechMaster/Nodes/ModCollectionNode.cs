using System.Linq;
using ModTechMaster.Core.Interfaces.Models;

namespace ModTechMaster.Nodes
{
    public class ModCollectionNode : MTMTreeViewItem
    {
        public ModCollectionNode(IModCollection modCollection, MTMTreeViewItem parent) : base(parent)
        {
            ModCollection = modCollection;
            ModCollection.Mods.ToList().ForEach(mod => Children.Add(new ModNode(mod, this)));
        }

        public IModCollection ModCollection { get; }
        public override string Name => ModCollection.Name;
    }
}