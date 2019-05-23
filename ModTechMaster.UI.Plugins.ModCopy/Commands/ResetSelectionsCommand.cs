namespace ModTechMaster.UI.Plugins.ModCopy.Commands
{
    using System.Linq;
    using System.Threading.Tasks;

    using ModTechMaster.UI.Core.Async;
    using ModTechMaster.UI.Plugins.Core.Interfaces;
    using ModTechMaster.UI.Plugins.ModCopy.Model;

    public class ResetSelectionsCommand : AwaitableDelegateCommand<ModCopyModel>, IPluginCommand
    {
        public ResetSelectionsCommand(object commandParameter)
            : base(MethodToRun)
        {
            this.CommandParameter = commandParameter;
        }

        public IPluginCommandCategory Category { get; }

        public object CommandParameter { get; }

        public string Name => "Reset Mod Selections";

        private static bool CanExecute(ModCopyModel model)
        {
            return model?.ModCollectionNode != null && model.ModCollectionNode.SelectedMods.Any();
        }

        private static async Task MethodToRun(ModCopyModel model)
        {
            await model.ResetModSelections();
        }
    }
}