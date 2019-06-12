namespace ModTechMaster.UI.Plugins.ModCopy.Nodes
{
    using System.Collections.Generic;
    using System.Linq;

    using ModTechMaster.Core.Enums.Mods;
    using ModTechMaster.Core.Interfaces.Models;
    using ModTechMaster.Core.Interfaces.Services;
    using ModTechMaster.Data.Models.Mods.TypedObjectDefinitions;
    using ModTechMaster.UI.Plugins.ModCopy.Nodes.SpecialisedNodes;

    public class ManifestEntryNode : MtmTreeViewItem
    {
        public ManifestEntryNode(IMtmTreeViewItem parent, List<IManifestEntry> manifestEntries, ObjectType entryType, IReferenceFinderService referenceFinderService)
            : base(parent, manifestEntries, referenceFinderService)
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
                                    var objectDefinitionNode = definition.ObjectType == ObjectType.LanceDef ? new LanceDefNode(this, definition as LanceDefObjectDefinition, referenceFinderService) : new ObjectDefinitionNode(this, definition, referenceFinderService);
                                    this.Children.Add(objectDefinitionNode);
                                    this.ManifestEntryLookupByObject.Add(objectDefinitionNode, entry);
                                });
                    });
        }

        public Dictionary<IMtmTreeViewItem, IManifestEntry> ManifestEntryLookupByObject { get; }

        public ManifestEntryNode(
            IMtmTreeViewItem parent,
            List<IManifestEntry> manifestEntries, 
            List<HashSet<IObjectDefinition>> objectLists,
            IReferenceFinderService referenceFinderService)
            : base(parent, manifestEntries, referenceFinderService)
        {
            this.ManifestEntries = manifestEntries;
            objectLists.ForEach(
                list => list.OrderBy(definition => definition.Name).ToList().ForEach(
                    definition => this.Children.Add(new ObjectDefinitionNode(this, definition, referenceFinderService))));
        }

        public override string HumanReadableContent => this.ManifestEntries.First().EntryType.ToString();

        public List<IManifestEntry> ManifestEntries { get; }

        public ObjectType EntryType { get; }

        public override string Name => this.ManifestEntries.First().EntryType.ToString();
    }
}