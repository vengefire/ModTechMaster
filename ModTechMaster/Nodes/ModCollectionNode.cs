using System.Collections.ObjectModel;
using System.Linq;
using ModTechMaster.Core.Interfaces.Models;

namespace ModTechMaster.Nodes
{
    public class ModCollectionNode : MTMTreeViewItem
    {
        public ModCollectionNode(IModCollection modCollection, MTMTreeViewItem parent) : base(parent)
        {
            ModCollection = modCollection;
            ModCollection.Mods.ToList().ForEach(mod => Mods.Add(new ModNode(mod, this)));
        }

        public ObservableCollection<ModNode> Mods { get; } = new ObservableCollection<ModNode>();
        public IModCollection ModCollection { get; }
    }
}