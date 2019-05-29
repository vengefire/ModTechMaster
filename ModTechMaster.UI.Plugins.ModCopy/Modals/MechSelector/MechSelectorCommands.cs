namespace ModTechMaster.UI.Plugins.ModCopy.Modals.MechSelector
{
    using System.Threading.Tasks;
    using System.Windows.Input;

    using ModTechMaster.UI.Core.Async;

    public static class MechSelectorCommands
    {
        public static ICommand ProcessMechSelectionFileCommand { get; }
            = new AwaitableDelegateCommand<MechSelectorModel>(model => Task.Run(() => MechSelectorModel.ProcessMechSelectionFile(model)), MechSelectorModel.CanProcessMechSelectionFile);
    }
}