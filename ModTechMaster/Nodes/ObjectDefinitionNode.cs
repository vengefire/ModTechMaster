using ModTechMaster.Core.Interfaces.Models;

namespace ModTechMaster.Nodes
{
    public class ObjectDefinitionNode : MTMTreeViewItem
    {
        public ObjectDefinitionNode(IMTMTreeViewItem parent, IObjectDefinition objectDefinition) : base(parent)
        {
            ObjectDefinition = objectDefinition;
        }

        public IObjectDefinition ObjectDefinition { get; }
        public override string Name => this.ObjectDefinition.Id;
        public override string HumanReadableContent => this.ObjectDefinition.JsonString;
    }
}