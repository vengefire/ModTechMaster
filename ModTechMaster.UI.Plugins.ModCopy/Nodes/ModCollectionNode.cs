namespace ModTechMaster.UI.Plugins.ModCopy.Nodes
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    using ModTechMaster.Core.Enums.Mods;
    using ModTechMaster.Core.Interfaces.Models;

    public class ModCollectionNode : MtmTreeViewItem
    {
        public ModCollectionNode(IModCollection modCollection, MtmTreeViewItem parent)
            : base(parent, modCollection)
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
                                    this.OnPropertyChanged(nameof(this.SelectedMods));
                                    this.OnPropertyChanged(nameof(this.SelectedModSize));
                                    this.OnPropertyChanged(nameof(this.SelectedModObjectCount));
                                    this.OnPropertyChanged(nameof(this.SelectedMechCount));
                                    this.OnPropertyChanged(nameof(this.SelectedVehicleCount));
                                    this.OnPropertyChanged(nameof(this.SelectedTurretCount));
                                    this.OnPropertyChanged(nameof(this.SelectedWeaponCount));
                                    this.OnPropertyChanged(nameof(this.SelectedUpgradeCount));
                                    this.OnPropertyChanged(nameof(this.SelectedCoolingCount));
                                }
                            };
                    });
            this.IsExpanded = true;
        }

        public override string HumanReadableContent => this.ModCollection.Name;

        public IModCollection ModCollection { get; }

        public override string Name => this.ModCollection.Name;

        public override IReferenceableObjectProvider ReferenceableObjectProvider => this.ModCollection;

        public long SelectedCoolingCount =>
            this.SelectedMods.Sum(
                node => node.SelectedObjects.Count(
                    item => ((IObjectDefinition)item.Object).ObjectType == ObjectType.HeatSinkDef));

        public long SelectedMechCount =>
            this.SelectedMods.Sum(
                node => node.SelectedObjects.Count(
                    item => ((IObjectDefinition)item.Object).ObjectType == ObjectType.MechDef));

        public long SelectedModObjectCount => this.SelectedMods.Sum(node => node.SelectedObjects.Count);

        // Null checks are partials...
        public ObservableCollection<ModNode> SelectedMods =>
            new ObservableCollection<ModNode>(
                this.Children.Where(item => item.IsChecked == true || item.IsChecked == null).Cast<ModNode>());

        public double SelectedModSize => this.SelectedMods.Sum(node => node.Mod.SizeOnDisk);

        public long SelectedTurretCount =>
            this.SelectedMods.Sum(
                node => node.SelectedObjects.Count(
                    item => ((IObjectDefinition)item.Object).ObjectType == ObjectType.TurretDef));

        public long SelectedUpgradeCount =>
            this.SelectedMods.Sum(
                node => node.SelectedObjects.Count(
                    item => ((IObjectDefinition)item.Object).ObjectType == ObjectType.UpgradeDef));

        public long SelectedVehicleCount =>
            this.SelectedMods.Sum(
                node => node.SelectedObjects.Count(
                    item => ((IObjectDefinition)item.Object).ObjectType == ObjectType.VehicleDef));

        public long SelectedWeaponCount =>
            this.SelectedMods.Sum(
                node => node.SelectedObjects.Count(
                    item => ((IObjectDefinition)item.Object).ObjectType == ObjectType.WeaponDef));

        public void SelectMods(IEnumerable<string> settingsAlwaysIncludedMods)
        {
            this.Children.Where(item => item.IsChecked == false && settingsAlwaysIncludedMods.Contains(item.Name))
                .ToList().ForEach(item => item.IsChecked = true);
        }
    }
}