namespace ModTechMaster.UI.Plugins.ModCopy.Nodes.SpecialisedNodes
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    using ModTechMaster.Core.Enums;
    using ModTechMaster.Core.Interfaces.Models;
    using ModTechMaster.Core.Interfaces.Services;
    using ModTechMaster.Data.Models.Mods.TypedObjectDefinitions;
    using ModTechMaster.Logic.Services;

    public class LanceDefNode : ObjectDefinitionNode
    {
        private static IFactionService factionService = new FactionService();

        public LanceDefNode(IMtmTreeViewItem parent, LanceDefObjectDefinition objectDefinition)
            : base(parent, objectDefinition)
        {
            objectDefinition.LanceSlots.ForEach(
                definition =>
                    {
                        var slot = new LanceSlotModel(definition, this, LanceDefNode.factionService);
                        this.LanceSlots.Add(slot);
                        slot.PropertyChanged += (sender, args) =>
                            {
                                if (sender is LanceSlotModel && args.PropertyName == nameof(LanceSlotModel.ObjectStatus))
                                {
                                    this.OnPropertyChanged(nameof(this.ObjectStatus));
                                }
                            };
                    });
        }

        public List<IObjectDefinition> EligiblePilots { get; } = new List<IObjectDefinition>();

        public List<IObjectDefinition> EligibleUnits { get; } = new List<IObjectDefinition>();

        public LanceDefObjectDefinition LanceDefObjectDefinition => this.ObjectDefinition as LanceDefObjectDefinition;

        public List<LanceSlotModel> LanceSlots { get; } = new List<LanceSlotModel>();

        public override ObjectStatus ObjectStatus =>
            this.LanceSlots.All(model => model.ObjectStatus == ObjectStatus.Nominal)
                ? ObjectStatus.Nominal
                : ObjectStatus.Error;

        public ObservableCollection<LanceSlotModel> InvalidLanceSlots =>
            new ObservableCollection<LanceSlotModel>(this.LanceSlots.Where(model => model.ObjectStatus != ObjectStatus.Nominal));
    }
}