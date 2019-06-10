namespace ModTechMaster.UI.Plugins.ModCopy.Nodes
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;

    using ModTechMaster.Core.Enums.Mods;
    using ModTechMaster.Core.Interfaces.Models;
    using ModTechMaster.UI.Plugins.ModCopy.Nodes.SpecialisedNodes;

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

            // Hook up interdependent properties
            // When Pilots/Units are selected, notify lances
            // TODO: Formalize this into a modeled relationship so we can refactor properly
            // and reuse the concept.
            var lanceDependencyProperties = new List<ObjectType>
                                                {
                                                    ObjectType.MechDef,
                                                    ObjectType.TurretDef,
                                                    ObjectType.VehicleDef,
                                                    ObjectType.PilotDef
                                                };
            var sourceObjects = this.AllChildren.Where(
                item => item.Object is IObjectDefinition obj && lanceDependencyProperties.Contains(obj.ObjectType));
            var targetObjects = this.AllChildren.Where(item => item is LanceDefNode obj).Cast<LanceDefNode>()
                .SelectMany(node => node.LanceSlots);

            targetObjects.AsParallel().ForAll(
                target =>
                    {
                        target.LoadEligibleUnitsAndPilots();
                        sourceObjects.AsParallel().ForAll(
                            source =>
                                {
                                    var propertyName = nameof(LanceSlotModel.SelectedEligibleUnits);
                                    if (((IObjectDefinition)source.Object).ObjectType == ObjectType.PilotDef)
                                    {
                                        propertyName = nameof(LanceSlotModel.SelectedEligiblePilots);
                                    }

                                    source.PropertyChanged += (sender, args) =>
                                        {
                                            if (args.PropertyName == nameof(IMtmTreeViewItem.IsChecked))
                                            {
                                                target.OnPropertyChanged(propertyName);
                                                target.OnPropertyChanged(nameof(LanceSlotModel.ObjectStatus));
                                            }
                                        };
                                });
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

        public long SelectedJumpJetCount =>
            this.SelectedMods.Sum(
                node => node.SelectedObjects.Count(
                    item => ((IObjectDefinition)item.Object).ObjectType == ObjectType.JumpJetDef));

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

        public long TotalCoolingCount =>
            this.ModCollection.GetReferenceableObjects().Count(o => o.ObjectType == ObjectType.HeatSinkDef);

        public long TotalJumpJetCount =>
            this.ModCollection.GetReferenceableObjects().Count(o => o.ObjectType == ObjectType.JumpJetDef);

        public long TotalMechCount =>
            this.ModCollection.GetReferenceableObjects().Count(o => o.ObjectType == ObjectType.MechDef);

        public long TotalModObjectCount => this.ModCollection.GetReferenceableObjects().Count;

        public int TotalMods => this.ModCollection.Mods.Count;

        public double TotalModSize => this.ModCollection.Mods.Sum(mod => mod.SizeOnDisk);

        public long TotalTurretCount =>
            this.ModCollection.GetReferenceableObjects().Count(o => o.ObjectType == ObjectType.TurretDef);

        public long TotalUpgradeCount =>
            this.ModCollection.GetReferenceableObjects().Count(o => o.ObjectType == ObjectType.UpgradeDef);

        public long TotalVehicleCount =>
            this.ModCollection.GetReferenceableObjects().Count(o => o.ObjectType == ObjectType.VehicleDef);

        public long TotalWeaponCount =>
            this.ModCollection.GetReferenceableObjects().Count(o => o.ObjectType == ObjectType.WeaponDef);

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

        public void SelectMods(IEnumerable<string> settingsAlwaysIncludedMods)
        {
            this.Children.Where(item => item.IsChecked == false && settingsAlwaysIncludedMods.Contains(item.Name))
                .ToList().ForEach(item => item.IsChecked = true);
        }
    }
}