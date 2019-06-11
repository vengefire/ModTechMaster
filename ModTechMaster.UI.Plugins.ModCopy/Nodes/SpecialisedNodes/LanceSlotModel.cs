namespace ModTechMaster.UI.Plugins.ModCopy.Nodes.SpecialisedNodes
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using System.Windows.Input;

    using ModTechMaster.Core.Constants;
    using ModTechMaster.Core.Enums;
    using ModTechMaster.Core.Enums.Mods;
    using ModTechMaster.Core.Interfaces.Models;
    using ModTechMaster.Core.Interfaces.Services;
    using ModTechMaster.Data.Models.Mods.TypedObjectDefinitions;
    using ModTechMaster.UI.Plugins.ModCopy.Annotations;
    using ModTechMaster.UI.Plugins.ModCopy.Modals;

    public class LanceSlotModel : INotifyPropertyChanged
    {
        private readonly IFactionService factionService;

        private readonly Dictionary<string, int> factionUnitDictionary = new Dictionary<string, int>();

        private readonly LanceDefNode lanceNode;

        // TODO: Put in a mechanism to identify selected eligible factions given the distinct set from selected units intersecting with faction list.
        private ObjectStatus previousObjectStatus = ObjectStatus.Warning;

        private int previousSelectedPilotsCount = -1;

        private int previousSelectedUnitsCount = -1;

        private ICommand selectEligiblePilotsCommand;

        private ICommand selectEligibleUnitsCommand;

        public LanceSlotModel(
            LanceSlotDefinition lanceSlotDefinition,
            LanceDefNode lanceNode,
            IFactionService factionService)
        {
            this.lanceNode = lanceNode;
            this.factionService = factionService;
            this.LanceSlotDefinition = lanceSlotDefinition;

            this.factionService.GetFactions().ForEach(faction => this.factionUnitDictionary.Add(faction, 0));

            this.CheckObjectStatusChanged();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public List<string> EligibleFactions =>
            this.factionUnitDictionary.Where(pair => pair.Value > 0).Select(pair => pair.Key).ToList();

        public List<ObjectDefinitionNode> EligiblePilotNodes { get; private set; } = new List<ObjectDefinitionNode>();

        public List<IObjectDefinition> EligiblePilots =>
            this.EligiblePilotNodes.Select(node => node.ObjectDefinition).ToList();

        public List<ObjectDefinitionNode> EligibleUnitNodes { get; private set; } = new List<ObjectDefinitionNode>();

        public List<IObjectDefinition> EligibleUnits =>
            this.EligibleUnitNodes.Select(node => node.ObjectDefinition).ToList();

        public List<string> IneligibleFactions =>
            this.LanceSlotDefinition.RestrictByFaction
                ? this.factionUnitDictionary.Where(pair => pair.Value > 0).Select(pair => pair.Key).ToList()
                : new List<string>();

        public LanceSlotDefinition LanceSlotDefinition { get; }

        public ObjectStatus ObjectStatus =>
            this.SelectedEligibleUnits.Any() && this.SelectedEligiblePilots.Any() && !this.IneligibleFactions.Any()
                ? ObjectStatus.Nominal
                : ObjectStatus.Error;

        public List<IObjectDefinition> SelectedEligiblePilots =>
            this.EligiblePilotNodes.Where(node => node.IsChecked == true).Select(node => node.ObjectDefinition)
                .ToList();

        public List<IObjectDefinition> SelectedEligibleUnits =>
            this.EligibleUnitNodes.Where(node => node.IsChecked == true).Select(node => node.ObjectDefinition).ToList();

        public ICommand SelectEligiblePilotsCommand =>
            this.selectEligiblePilotsCommand
            ?? (this.selectEligiblePilotsCommand = new DelegateCommand(this.ShowEligiblePilotsPopup));

        public ICommand SelectEligibleUnitsCommand =>
            this.selectEligibleUnitsCommand
            ?? (this.selectEligibleUnitsCommand = new DelegateCommand(this.ShowEligibleUnitsPopup));

        public void CheckObjectStatusChanged()
        {
            if (this.ObjectStatus != this.previousObjectStatus)
            {
                this.previousObjectStatus = this.ObjectStatus;
                this.OnPropertyChanged(nameof(this.ObjectStatus));
            }
        }

        public void LoadEligibleUnitsAndPilots()
        {
            this.EligibleUnitNodes = this.GetEligibleUnits();
            this.EligiblePilotNodes = this.GetEligiblePilots();
        }

        [NotifyPropertyChangedInvocator]
        public virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void ShowEligiblePilotsPopup()
        {
            var window = new SimpleObjectSelectorWindow(this.EligiblePilots, this.SelectedEligiblePilots);
            ModCopyPage.Self.TabWindowContainer.Children.Add(window);
            window.Show();
            window.Closed += this.WindowOnClosed;
        }

        public void ShowEligibleUnitsPopup()
        {
            var window = new SimpleObjectSelectorWindow(this.EligibleUnits, this.SelectedEligibleUnits);
            ModCopyPage.Self.TabWindowContainer.Children.Add(window);
            window.Show();
            window.Closed += this.WindowOnClosed;
        }

        private List<ObjectDefinitionNode> GetEligiblePilots()
        {
            var collectionNode = this.lanceNode.TopNode as ModCollectionNode;

            var candidates = collectionNode.AllChildren
                .Where(o => o is ObjectDefinitionNode obj && obj.ObjectDefinition.ObjectType == ObjectType.PilotDef)
                .Cast<ObjectDefinitionNode>();

            var eligible = candidates.Where(
                o => !this.LanceSlotDefinition.PilotTags.Any() || o.ObjectDefinition.Tags.ContainsKey(Keywords.MyTags)
                     && !this.LanceSlotDefinition.PilotTags.Except(o.ObjectDefinition.Tags[Keywords.MyTags]).Any());
            var filteredEligible = eligible.Where(
                o => !o.ObjectDefinition.Tags[Keywords.MyTags]
                         .Any(s => this.LanceSlotDefinition.ExcludedPilotTags.Contains(s)));
            return filteredEligible.ToList();
        }

        private List<ObjectDefinitionNode> GetEligibleUnits()
        {
            var validUnitObjectTypes =
                new List<ObjectType> { ObjectType.MechDef, ObjectType.TurretDef, ObjectType.VehicleDef };

            var collectionNode = this.lanceNode.TopNode as ModCollectionNode;

            var candidates = collectionNode.AllChildren
                .Where(
                    o => o is ObjectDefinitionNode obj
                         && validUnitObjectTypes.Contains(obj.ObjectDefinition.ObjectType))
                .Cast<ObjectDefinitionNode>();

            var eligibleUnits = candidates.Where(
                o => !this.LanceSlotDefinition.UnitTags.Except(o.ObjectDefinition.Tags[Keywords.MyTags]).Any());
            var filteredEligibleUnits = eligibleUnits.Where(
                o => !o.ObjectDefinition.Tags[Keywords.MyTags]
                         .Any(s => this.LanceSlotDefinition.ExcludedUnitTags.Contains(s))).ToList();

            if (this.LanceSlotDefinition.RestrictByFaction)
            {
                filteredEligibleUnits.AsParallel().ForAll(
                    unit =>
                        {
                            var unitTags = unit.ObjectDefinition.Tags[Keywords.MyTags];
                            var factionIntersect = unitTags.Intersect(this.factionService.GetFactions()).ToList();
                            factionIntersect.ForEach(faction => this.factionUnitDictionary[faction] += 1);
                        });
            }

            return filteredEligibleUnits.ToList();
        }

        private void WindowOnClosed(object sender, EventArgs e)
        {
            Task.Run(
                () =>
                    {
                        try
                        {
                            ModCopyPage.Self.ModCopyModel.MainModel.IsBusy = true;
                            var selectWindow = sender as SimpleObjectSelectorWindow;
                            var selectedObjects = selectWindow.SelectedItems;
                            selectedObjects.ToList().ForEach(
                                definition =>
                                    {
                                        IMtmTreeViewItem node = null;
                                        if (MtmTreeViewItem.DictRefsToTreeViewItems.TryGetValue(definition, out node))
                                        {
                                            node.IsChecked = true;
                                        }
                                    });
                            ModCopyPage.Self.Dispatcher.Invoke(
                                () =>
                                    {
                                        if (this.SelectedEligibleUnits.Count != this.previousSelectedUnitsCount)
                                        {
                                            this.previousSelectedUnitsCount = this.SelectedEligibleUnits.Count;
                                            this.OnPropertyChanged(nameof(this.SelectedEligibleUnits));
                                            this.lanceNode.LanceSlots.Where(model => model != this).ToList().ForEach(
                                                model =>
                                                    {
                                                        model.OnPropertyChanged(nameof(this.SelectedEligibleUnits));
                                                        model.OnPropertyChanged(nameof(this.ObjectStatus));
                                                    });
                                        }

                                        if (this.SelectedEligiblePilots.Count != this.previousSelectedPilotsCount)
                                        {
                                            this.previousSelectedPilotsCount = this.SelectedEligiblePilots.Count;
                                            this.OnPropertyChanged(nameof(this.SelectedEligiblePilots));
                                            this.lanceNode.LanceSlots.Where(model => model != this).ToList().ForEach(
                                                model =>
                                                    {
                                                        model.OnPropertyChanged(nameof(this.SelectedEligiblePilots));
                                                        model.OnPropertyChanged(nameof(this.ObjectStatus));
                                                    });
                                        }

                                        this.OnPropertyChanged(nameof(this.ObjectStatus));
                                    });
                        }
                        finally
                        {
                            ModCopyPage.Self.ModCopyModel.MainModel.IsBusy = false;
                        }
                    }).ConfigureAwait(false);
        }
    }
}