namespace ModTechMaster.UI.Plugins.ModCopy.Commands
{
    using ModTechMaster.UI.Plugins.Core.Interfaces;
    using ModTechMaster.UI.Plugins.ModCopy.Modals.Validators;
    using ModTechMaster.UI.Plugins.ModCopy.Model;

    public class ValidateLanceDefinitionsCommand : DelegateCommand<ModCopyModel>, IPluginCommand
    {
        private readonly ModCopyModel model;

        public ValidateLanceDefinitionsCommand(ModCopyModel model)
            : base(Execute, CanExecute)
        {
            this.model = model;
        }

        public IPluginCommandCategory Category { get; }

        public object CommandParameter => this.model;

        public string Name => "Validate Lance Definitions";

        public static bool CanExecute(ModCopyModel model)
        {
            return true;
        }

        public static void Execute(ModCopyModel model)
        {
            var window = new ValidateLanceDefsWindow(new ValidateLanceDefViewModel(model));
            ModCopyPage.Self.WindowContainer.Children.Add(window);
            window.Show();
        }
    }
}