namespace ModTechMaster.UI.Plugins.ModCopy.Nodes
{
    using ModTechMaster.Core.Interfaces.Models;

    public class ObjectDefinitionNode : MTMTreeViewItem
    {
        public ObjectDefinitionNode(IMTMTreeViewItem parent, IObjectDefinition objectDefinition) : base(parent)
        {
            this.ObjectDefinition = objectDefinition;
        }

        public IObjectDefinition ObjectDefinition { get; }
        public override string Name => this.ObjectDefinition.Id;
        public override string HumanReadableContent => this.ObjectDefinition.HumanReadableText;
    }
}