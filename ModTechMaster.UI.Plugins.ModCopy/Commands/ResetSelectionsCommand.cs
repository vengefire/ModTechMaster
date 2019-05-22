namespace ModTechMaster.UI.Plugins.ModCopy.Commands
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.Interfaces;
    using Model;

    public class ResetSelectionsCommand : AwaitableDelegateCommand<ModCopyModel>, IPluginCommand
    {
        public ResetSelectionsCommand(object commandParameter) : base(ResetSelectionsCommand.MethodToRun)
        {
            this.CommandParameter = commandParameter;
        }

        static async Task MethodToRun(ModCopyModel model)
        {
            await model.ResetModSelections();
        }

        static bool CanExecute(ModCopyModel model)
        {
            return model?.ModCollectionNode != null && model.ModCollectionNode.SelectedMods.Any();
        }

        public string Name => "Reset Mod Selections";
        public IPluginCommandCategory Category { get; }
        public object CommandParameter { get; }
    }
}