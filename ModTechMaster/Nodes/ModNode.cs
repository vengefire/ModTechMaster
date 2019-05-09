using System.Collections.ObjectModel;
using ModTechMaster.Core.Interfaces.Models;

namespace ModTechMaster.Nodes
{
    public class ModNode : MTMTreeViewItem
    {
        public ModNode(IMod mod, MTMTreeViewItem parent) : base(parent)
        {
            Mod = mod;
            if (Mod.Manifest != null) Manifest = new ManifestNode(mod.Manifest, this);
        }

        public ManifestNode Manifest { get; }
        public IMod Mod { get; }

        public ObservableCollection<IMTMTreeViewItem> Contents =>
            new ObservableCollection<IMTMTreeViewItem>
            {
                Manifest
            };}
}