namespace ModTechMaster.UI.Plugins.ModCopy.Nodes
{
    using System.Collections.Generic;
    using System.Linq;

    using ModTechMaster.Core.Enums.Mods;
    using ModTechMaster.Core.Interfaces.Models;
    using ModTechMaster.Data.Models.Mods.TypedObjectDefinitions;
    using ModTechMaster.UI.Plugins.ModCopy.Nodes.SpecialisedNodes;

    public class ManifestEntryNode : MtmTreeViewItem
    {
        public ManifestEntryNode(IMtmTreeViewItem parent, List<IManifestEntry> manifestEntries, ObjectType entryType)
            : base(parent, manifestEntries)
        {
            this.ManifestEntries = manifestEntries;
            this.EntryType = entryType;
            this.ManifestEntryLookupByObject = new Dictionary<IMtmTreeViewItem, IManifestEntry>();
            this.ManifestEntries.ForEach(
                entry =>
                    {
                        entry.Objects.ToList().ForEach(
                            definition =>
                                {
                                    var objectDefinitionNode = definition.ObjectType == ObjectType.LanceDef ? new LanceDefNode(this, definition as LanceDefObjectDefinition) : new ObjectDefinitionNode(this, definition);
                                    this.Children.Add(objectDefinitionNode);
                                    this.ManifestEntryLookupByObject.Add(objectDefinitionNode, entry);
                                });
                    });
        }

        public Dictionary<IMtmTreeViewItem, IManifestEntry> ManifestEntryLookupByObject { get; }

        public ManifestEntryNode(
            IMtmTreeViewItem parent,
            List<IManifestEntry> manifestEntries, 
            List<HashSet<IObjectDefinition>> objectLists)
            : base(parent, manifestEntries)
        {
            this.ManifestEntries = manifestEntries;
            objectLists.ForEach(
                list => list.OrderBy(definition => definition.Name).ToList().ForEach(
                    definition => this.Children.Add(new ObjectDefinitionNode(this, definition))));
        }

        public override string HumanReadableContent => this.ManifestEntries.First().EntryType.ToString();

        public List<IManifestEntry> ManifestEntries { get; }

        public ObjectType EntryType { get; }

        public override string Name => this.ManifestEntries.First().EntryType.ToString();
    }
}