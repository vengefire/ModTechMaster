using ModTechMaster.Core.Interfaces.Models;

namespace ModTechMaster.Nodes
{
    public class ManifestNode : MTMTreeViewItem
    {
        public ManifestNode(IManifest modManifest, MTMTreeViewItem parent) : base(parent)
        {
            Manifest = modManifest;
        }

        public IManifest Manifest { get; }
    }
}