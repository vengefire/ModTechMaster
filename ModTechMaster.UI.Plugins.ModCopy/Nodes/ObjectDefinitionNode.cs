namespace ModTechMaster.UI.Plugins.ModCopy.Nodes
{
    using ModTechMaster.Core.Interfaces.Models;
    using ModTechMaster.Core.Interfaces.Services;

    public class ObjectDefinitionNode : MtmTreeViewItem
    {
        public ObjectDefinitionNode(IMtmTreeViewItem parent, IObjectDefinition objectDefinition, IReferenceFinderService referenceFinderService)
            : base(parent, objectDefinition, referenceFinderService)
        {
            this.ObjectDefinition = objectDefinition;
        }

        public override string HumanReadableContent => this.ObjectDefinition.HumanReadableText;

        // public override string Name => this.ObjectDefinition.Id;
        public override string Name => this.ObjectDefinition.Id;

        public IObjectDefinition ObjectDefinition { get; }
    }
}