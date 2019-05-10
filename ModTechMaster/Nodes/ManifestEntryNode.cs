using System.Collections.Generic;
using System.Linq;
using ModTechMaster.Core.Interfaces.Models;

namespace ModTechMaster.Nodes
{
    public class ManifestEntryNode : MTMTreeViewItem
    {
        public ManifestEntryNode(IMTMTreeViewItem parent, IManifestEntry manifestEntry) : base(parent)
        {
            ManifestEntry = manifestEntry;
            ManifestEntry.Objects.ToList()
                .ForEach(definition => Children.Add(new ObjectDefinitionNode(this, definition)));
        }

        public ManifestEntryNode(IMTMTreeViewItem parent, IManifestEntry manifestEntry,
            List<HashSet<IObjectDefinition>> objectLists) : base(parent)
        {
            ManifestEntry = manifestEntry;
            objectLists
                .ForEach(list =>
                    list.ToList().ForEach(definition => Children.Add(new ObjectDefinitionNode(this, definition))));
        }

        public IManifestEntry ManifestEntry { get; }
        public override string Name => ManifestEntry.EntryType.ToString();
    }
}