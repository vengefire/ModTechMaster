namespace ModTechMaster.UI.Plugins.ModCopy.Modals.MechSelector
{
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Forms;
    using System.Windows.Input;

    using ModTechMaster.UI.Core.Async;
    using ModTechMaster.UI.Core.WinForms.Extensions;
    using ModTechMaster.UI.Plugins.ModCopy.Model;

    using Xceed.Wpf.Toolkit;

    using WindowStartupLocation = Xceed.Wpf.Toolkit.WindowStartupLocation;

    /// <summary>
    ///     Interaction logic for MechSelectorWindow.xaml
    /// </summary>
    public partial class MechSelectorWindow : ChildWindow
    {
        private static readonly Regex Regex = new Regex("[^0-9.-]+"); // regex that matches disallowed text

        public MechSelectorWindow(ModCopyModel modCopyModel)
        {
            this.WindowStartupLocation = WindowStartupLocation.Center;
            Self = this;
            this.MechSelectorModel = new MechSelectorModel(modCopyModel);
            this.InitializeComponent();
            this.SelectAllCommand =
                new AwaitableDelegateCommand<bool>(b => Task.Run(() => this.MechSelectorModel.SelectAllMechs(b)));
            this.DataContext = this;
        }

        public static MechSelectorWindow Self { get; private set; }

        public MechSelectorModel MechSelectorModel { get; }

        public ICommand SelectAllCommand { get; }

        private static bool IsTextAllowed(string text)
        {
            return !Regex.IsMatch(text);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            using (var fileDialog = new OpenFileDialog())
            {
                var result = fileDialog.ShowDialog(this.GetIWin32Window());
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    this.MechSelectorModel.MechFilePath = fileDialog.FileName;
                }
            }
        }

        private void PreviewMaxProdYearInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }
    }
}