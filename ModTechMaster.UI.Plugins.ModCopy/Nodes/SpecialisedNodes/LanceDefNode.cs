namespace ModTechMaster.UI.Plugins.ModCopy.Nodes.SpecialisedNodes
{
    using ModTechMaster.Core.Interfaces.Models;
    using ModTechMaster.Data.Models.Mods.TypedObjectDefinitions;

    public class LanceDefNode : ObjectDefinitionNode
    {
        public LanceDefNode(IMtmTreeViewItem parent, IObjectDefinition objectDefinition)
            : base(parent, objectDefinition)
        {
        }

        public LanceDefObjectDefinition LanceDefObject => this.ObjectDefinition as LanceDefObjectDefinition;
    }
}