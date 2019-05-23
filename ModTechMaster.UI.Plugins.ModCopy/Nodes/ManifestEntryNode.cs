namespace ModTechMaster.UI.Plugins.ModCopy.Nodes
{
    using System.Collections.Generic;
    using System.Linq;

    using ModTechMaster.Core.Interfaces.Models;

    public class ManifestEntryNode : MtmTreeViewItem
    {
        public ManifestEntryNode(IMtmTreeViewItem parent, IManifestEntry manifestEntry)
            : base(parent, manifestEntry)
        {
            this.ManifestEntry = manifestEntry;
            this.ManifestEntry.Objects.ToList()
                .ForEach(definition => this.Children.Add(new ObjectDefinitionNode(this, definition)));
        }

        public ManifestEntryNode(
            IMtmTreeViewItem parent,
            IManifestEntry manifestEntry,
            List<HashSet<IObjectDefinition>> objectLists)
            : base(parent, manifestEntry)
        {
            this.ManifestEntry = manifestEntry;
            objectLists.ForEach(
                list => list.OrderBy(definition => definition.Name).ToList().ForEach(
                    definition => this.Children.Add(new ObjectDefinitionNode(this, definition))));
        }

        public override string HumanReadableContent => this.ManifestEntry.EntryType.ToString();

        public IManifestEntry ManifestEntry { get; }

        public override string Name => this.ManifestEntry.EntryType.ToString();
    }
}