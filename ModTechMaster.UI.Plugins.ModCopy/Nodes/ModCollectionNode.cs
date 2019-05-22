using ModTechMaster.UI.Data.Enums;

namespace ModTechMaster.UI.Plugins.ModCopy.Nodes
{
    using System.Collections.ObjectModel;
    using System.Linq;
    using ModTechMaster.Core.Interfaces.Models;

    public class ModCollectionNode : MtmTreeViewItem
    {
        public ModCollectionNode(IModCollection modCollection, MtmTreeViewItem parent) : base(parent, modCollection)
        {
            this.ModCollection = modCollection;
            this.ModCollection.Mods.OrderBy(mod => mod.Name).ToList().ForEach(
                mod =>
                {
                    var modNode = new ModNode(mod, this);
                    this.Children.Add(modNode);
                    modNode.PropertyChanged += (sender, args) =>
                    {
                        if (args.PropertyName == "IsChecked")
                        {
                            this.OnPropertyChanged(nameof(SelectedMods));
                            this.OnPropertyChanged(nameof(SelectedModSize));
                            this.OnPropertyChanged(nameof(SelectedModObjectCount));
                        }
                    };
                });
            this.IsExpanded = true;
        }

        public IModCollection ModCollection { get; }
        public override IReferenceableObjectProvider ReferenceableObjectProvider => this.ModCollection;
        public override string Name => this.ModCollection.Name;
        public override string HumanReadableContent => this.ModCollection.Name;

        // Null checks are partials...
        public ObservableCollection<ModNode> SelectedMods => new ObservableCollection<ModNode>(this.Children.Where(item => item.IsChecked == true || item.IsChecked == null).Cast<ModNode>());

        public void SelectMods(ObservableCollection<string> settingsAlwaysIncludedMods)
        {
            this.Children.Where(item => item.IsChecked == false && settingsAlwaysIncludedMods.Contains(item.Name)).ToList().ForEach(item => item.IsChecked = true);
            this.OnPropertyChanged("SelectedMods");
        }

        public double SelectedModSize => SelectedMods.Sum(node => node.Mod.SizeOnDisk);
        public long SelectedModObjectCount => SelectedMods.Sum(node => node.SelectedObjectCount);
    }
}