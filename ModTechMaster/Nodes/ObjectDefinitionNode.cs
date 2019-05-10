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
    }
}