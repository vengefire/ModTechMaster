namespace ModTechMaster.UI.Plugins.ModCopy.Nodes
{
    using System.Collections.Generic;
    using System.Linq;

    using ModTechMaster.Core.Interfaces.Models;
    using ModTechMaster.Core.Interfaces.Services;

    public sealed class ManifestNode : MtmTreeViewItem
    {
        private readonly List<IMtmTreeViewItem> children = new List<IMtmTreeViewItem>();

        public ManifestNode(IManifest modManifest, MtmTreeViewItem parent, IReferenceFinderService referenceFinderService)
            : base(parent, modManifest, referenceFinderService)
        {
            this.Manifest = modManifest;
            var groupedManifestEntries =
                this.Manifest.Entries.GroupBy(entry => entry.EntryType).OrderBy(entries => entries.Key);

            foreach (var groupedManifestEntry in groupedManifestEntries)
            {
                this.Children.Add(new ManifestEntryNode(this, groupedManifestEntry.Select(entry => entry).ToList(), groupedManifestEntry.Key, referenceFinderService));
            }
        }

        public override string HumanReadableContent => this.Manifest.JsonString;

        public override string Name => @"Mod Manifest";

        private IManifest Manifest { get; }
    }
}