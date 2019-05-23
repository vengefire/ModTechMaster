namespace ModTechMaster.UI.Plugins.ModCopy.Nodes
{
    using ModTechMaster.Core.Interfaces.Models;

    public class ObjectDefinitionNode : MtmTreeViewItem
    {
        public ObjectDefinitionNode(IMtmTreeViewItem parent, IObjectDefinition objectDefinition)
            : base(parent, objectDefinition)
        {
            this.ObjectDefinition = objectDefinition;
        }

        public override string HumanReadableContent => this.ObjectDefinition.HumanReadableText;

        public override string Name => this.ObjectDefinition.Id;

        public IObjectDefinition ObjectDefinition { get; }
    }
}