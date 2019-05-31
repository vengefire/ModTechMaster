namespace ModTechMaster.UI.Plugins.ModCopy.Nodes
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
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
                                this.OnPropertyChanged(nameof(this.TotalMods));
                                this.OnPropertyChanged(nameof(this.TotalModSize));
                                this.OnPropertyChanged(nameof(this.TotalModObjectCount));
                                this.OnPropertyChanged(nameof(this.TotalMechCount));
                                this.OnPropertyChanged(nameof(this.TotalVehicleCount));
                                this.OnPropertyChanged(nameof(this.TotalTurretCount));
                                this.OnPropertyChanged(nameof(this.TotalWeaponCount));
                                this.OnPropertyChanged(nameof(this.TotalUpgradeCount));
                                this.OnPropertyChanged(nameof(this.TotalCoolingCount));
                            };
                    });
            this.IsExpanded = true;
        }

        public override void IncestPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.IncestPropertyChanged(sender, e);
            if (e.PropertyName == "Selected")
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

        public long TotalMechCount =>
            this.ModCollection.GetReferenceableObjects().Count(o => o.ObjectType == ObjectType.MechDef);

        public long SelectedModObjectCount => this.SelectedMods.Sum(node => node.SelectedObjects.Count);

        public long TotalModObjectCount => this.ModCollection.GetReferenceableObjects().Count;

        // Null checks are partials...
        public ObservableCollection<ModNode> SelectedMods =>
            new ObservableCollection<ModNode>(
                this.Children.Where(item => item.IsChecked == true || item.IsChecked == null).Cast<ModNode>());

        public int TotalMods => this.ModCollection.Mods.Count;

        public double SelectedModSize => this.SelectedMods.Sum(node => node.Mod.SizeOnDisk);

        public double TotalModSize => this.ModCollection.Mods.Sum(mod => mod.SizeOnDisk);

        public long SelectedTurretCount =>
            this.SelectedMods.Sum(
                node => node.SelectedObjects.Count(
                    item => ((IObjectDefinition)item.Object).ObjectType == ObjectType.TurretDef));

        public long TotalTurretCount =>
            this.ModCollection.GetReferenceableObjects().Count(o => o.ObjectType == ObjectType.TurretDef);

        public long SelectedUpgradeCount =>
            this.SelectedMods.Sum(
                node => node.SelectedObjects.Count(
                    item => ((IObjectDefinition)item.Object).ObjectType == ObjectType.UpgradeDef));

        public long TotalUpgradeCount =>
            this.ModCollection.GetReferenceableObjects().Count(o => o.ObjectType == ObjectType.UpgradeDef);

        public long SelectedVehicleCount =>
            this.SelectedMods.Sum(
                node => node.SelectedObjects.Count(
                    item => ((IObjectDefinition)item.Object).ObjectType == ObjectType.VehicleDef));

        public long TotalVehicleCount =>
            this.ModCollection.GetReferenceableObjects().Count(o => o.ObjectType == ObjectType.VehicleDef);

        public long SelectedWeaponCount =>
            this.SelectedMods.Sum(
                node => node.SelectedObjects.Count(
                    item => ((IObjectDefinition)item.Object).ObjectType == ObjectType.WeaponDef));

        public long TotalWeaponCount =>
            this.ModCollection.GetReferenceableObjects().Count(o => o.ObjectType == ObjectType.WeaponDef);

        public long TotalCoolingCount =>
            this.ModCollection.GetReferenceableObjects().Count(o => o.ObjectType == ObjectType.HeatSinkDef);

        public long SelectedJumpJetCount =>
            this.SelectedMods.Sum(
                node => node.SelectedObjects.Count(
                    item => ((IObjectDefinition)item.Object).ObjectType == ObjectType.JumpJetDef));

        public long TotalJumpJetCount =>
            this.ModCollection.GetReferenceableObjects().Count(o => o.ObjectType == ObjectType.JumpJetDef);

        public void SelectMods(IEnumerable<string> settingsAlwaysIncludedMods)
        {
            this.Children.Where(item => item.IsChecked == false && settingsAlwaysIncludedMods.Contains(item.Name))
                .ToList().ForEach(item => item.IsChecked = true);
        }
    }
}