namespace ModTechMaster.UI.Plugins.ModCopy.Modals.Validators
{
    using System.Collections.ObjectModel;
    using System.Linq;

    using ModTechMaster.Core.Enums;
    using ModTechMaster.UI.Plugins.ModCopy.Model;
    using ModTechMaster.UI.Plugins.ModCopy.Nodes.SpecialisedNodes;

    public class ValidateLanceDefViewModel
    {
        private readonly ModCopyModel modCopyModel;

        public ValidateLanceDefViewModel(ModCopyModel modCopyModel)
        {
            this.modCopyModel = modCopyModel;
        }

        public ObservableCollection<LanceDefNode> InvalidLanceDefs =>
            new ObservableCollection<LanceDefNode>(
                this.modCopyModel.ModCollectionNode.AllChildren
                    .Where(item => item is LanceDefNode node && node.ObjectStatus != ObjectStatus.Nominal)
                    .Cast<LanceDefNode>());
    }
}