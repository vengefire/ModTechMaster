using System.Collections.Generic;
using System.Linq;
using ModTechMaster.Core.Interfaces.Models;

namespace ModTechMaster.Nodes
{
    public sealed class ManifestNode : MTMTreeViewItem
    {
        private readonly List<IMTMTreeViewItem> _children = new List<IMTMTreeViewItem>();

        public ManifestNode(IManifest modManifest, MTMTreeViewItem parent) : base(parent)
        {
            Manifest = modManifest;
            var groupedManifestEntries = Manifest.Entries.GroupBy(entry => entry.EntryType);
            foreach (var groupedManifestEntry in groupedManifestEntries)
            {
                var firstEntry = groupedManifestEntry.First();
                var groupObjects = groupedManifestEntry.Select(entry => entry.Objects).ToList();
                Children.Add(new ManifestEntryNode(this, firstEntry, groupObjects));
            }
            //Manifest.Entries.ToList().ForEach(entry => _children.Add(new ManifestEntryNode(this, entry)));
        }

        private IManifest Manifest { get; }
    }
}