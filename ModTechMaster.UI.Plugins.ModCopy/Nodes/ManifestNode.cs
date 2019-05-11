namespace ModTechMaster.UI.Plugins.ModCopy.Nodes
{
    using System.Collections.Generic;
    using System.Linq;
    using ModTechMaster.Core.Interfaces.Models;

    public sealed class ManifestNode : MTMTreeViewItem
    {
        private readonly List<IMTMTreeViewItem> _children = new List<IMTMTreeViewItem>();

        public ManifestNode(IManifest modManifest, MTMTreeViewItem parent) : base(parent)
        {
            this.Manifest = modManifest;
            var groupedManifestEntries = this.Manifest.Entries.GroupBy(entry => entry.EntryType);
            foreach (var groupedManifestEntry in groupedManifestEntries)
            {
                var firstEntry = groupedManifestEntry.First();
                var groupObjects = groupedManifestEntry.Select(entry => entry.Objects).ToList();
                this.Children.Add(new ManifestEntryNode(this, firstEntry, groupObjects));
            }
        }

        private IManifest Manifest { get; }
        public override string Name => @"Mod Manifest";
        public override string HumanReadableContent => this.Manifest.JsonString;
    }
}