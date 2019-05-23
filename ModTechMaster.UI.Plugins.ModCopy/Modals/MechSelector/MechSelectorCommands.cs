namespace ModTechMaster.UI.Plugins.ModCopy.Modals.MechSelector
{
    using System.Windows.Input;

    public static class MechSelectorCommands
    {
        public static ICommand ProcessMechSelectionFileCommand { get; }
            = new DelegateCommand<MechSelectorModel>(MechSelectorModel.ProcessMechSelectionFile, MechSelectorModel.CanProcessMechSelectionFile);
    }
}