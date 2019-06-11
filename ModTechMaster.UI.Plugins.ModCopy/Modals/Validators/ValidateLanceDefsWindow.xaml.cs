namespace ModTechMaster.UI.Plugins.ModCopy.Modals.Validators
{
    using Xceed.Wpf.Toolkit;

    /// <summary>
    ///     Interaction logic for ValidateLanceDefsWindow.xaml
    /// </summary>
    public partial class ValidateLanceDefsWindow : ChildWindow
    {
        private readonly ValidateLanceDefViewModel validateLanceDefViewModel;

        public ValidateLanceDefsWindow(ValidateLanceDefViewModel validateLanceDefViewModel)
        {
            this.validateLanceDefViewModel = validateLanceDefViewModel;
            this.InitializeComponent();
            this.DataContext = validateLanceDefViewModel;
        }
    }
}