namespace ModTechMaster.UI.Plugins.ModCopy.Nodes.SpecialisedNodes
{
    using System.Collections.Generic;
    using System.Linq;

    using ModTechMaster.Core.Enums;
    using ModTechMaster.Core.Interfaces.Models;
    using ModTechMaster.Data.Models.Mods.TypedObjectDefinitions;

    public class LanceDefNode : ObjectDefinitionNode
    {
        public LanceDefNode(IMtmTreeViewItem parent, LanceDefObjectDefinition objectDefinition)
            : base(parent, objectDefinition)
        {
            objectDefinition.LanceSlots.ForEach(
                definition =>
                    {
                        var slot = new LanceSlotModel(definition, this);
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
    }
}