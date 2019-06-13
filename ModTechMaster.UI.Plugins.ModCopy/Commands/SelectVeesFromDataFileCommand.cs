namespace ModTechMaster.UI.Plugins.ModCopy.Commands
{
    using ModTechMaster.UI.Plugins.Core.Interfaces;
    using ModTechMaster.UI.Plugins.ModCopy.Model;

    public class SelectVeesFromDataFileCommand : DelegateCommand<ModCopyModel>, IPluginCommand
    {
        private readonly ModCopyModel modCopyModel;

        public SelectVeesFromDataFileCommand(ModCopyModel modCopyModel)
            : base(ExecuteMethod, CanExecuteMethod)
        {
            this.modCopyModel = modCopyModel;
        }

        public IPluginCommandCategory Category { get; }

        public object CommandParameter => this.modCopyModel;

        public string Name => "Select Vee's from Data File";

        private static bool CanExecuteMethod(ModCopyModel arg)
        {
            return !arg?.MainModel?.IsBusy ?? false;
        }

        private static void ExecuteMethod(ModCopyModel obj)
        {
            ModCopyModel.SelectVeesFromDataFile(obj);
        }
    }
}