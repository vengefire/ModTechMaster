namespace ModTechMaster.UI.Plugins.ModCopy.Commands
{
    using ModTechMaster.UI.Plugins.Core.Interfaces;
    using ModTechMaster.UI.Plugins.ModCopy.Model;

    public class SelectMechsFromDataFileCommand : DelegateCommand<ModCopyModel>, IPluginCommand
    {
        private readonly ModCopyModel modCopyModel;

        public SelectMechsFromDataFileCommand(ModCopyModel modCopyModel)
            : base(ExecuteMethod, CanExecuteMethod)
        {
            this.modCopyModel = modCopyModel;
        }

        public IPluginCommandCategory Category { get; }

        public object CommandParameter => this.modCopyModel;

        public string Name => "Select 'Mechs from Data File";

        private static bool CanExecuteMethod(ModCopyModel arg)
        {
            return !arg?.MainModel?.IsBusy ?? false;
        }

        private static void ExecuteMethod(ModCopyModel obj)
        {
            ModCopyModel.SelectMechsFromDataFile(obj);
        }
    }
}