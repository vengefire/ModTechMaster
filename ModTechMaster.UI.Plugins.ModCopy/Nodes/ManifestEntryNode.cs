namespace ModTechMaster.UI.Plugins.ModCopy.Nodes
{
    using System.Collections.Generic;
    using System.Linq;
    using ModTechMaster.Core.Interfaces.Models;

    public class ManifestEntryNode : MTMTreeViewItem
    {
        public ManifestEntryNode(IMTMTreeViewItem parent, IManifestEntry manifestEntry) : base(parent)
        {
            this.ManifestEntry = manifestEntry;
            this.ManifestEntry.Objects.ToList()
                .ForEach(definition => this.Children.Add(new ObjectDefinitionNode(this, definition)));
        }

        public ManifestEntryNode(IMTMTreeViewItem parent, IManifestEntry manifestEntry,
            List<HashSet<IObjectDefinition>> objectLists) : base(parent)
        {
            this.ManifestEntry = manifestEntry;
            objectLists
                .ForEach(list =>
                    list.ToList().ForEach(definition => this.Children.Add(new ObjectDefinitionNode(this, definition))));
        }

        public IManifestEntry ManifestEntry { get; }
        public override string Name => this.ManifestEntry.EntryType.ToString();
        public override string HumanReadableContent => this.ManifestEntry.EntryType.ToString();
    }
}