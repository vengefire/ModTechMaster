namespace ModTechMaster.UI.Plugins.ModCopy.Modals.MechSelector
{
    using System.Text.RegularExpressions;
    using System.Windows;
    using System.Windows.Input;

    using ModTechMaster.UI.Plugins.ModCopy.Model;

    using Xceed.Wpf.Toolkit;

    /// <summary>
    ///     Interaction logic for MechSelectorWindow.xaml
    /// </summary>
    public partial class MechSelectorWindow : ChildWindow
    {
        private static readonly Regex Regex = new Regex("[^0-9.-]+"); // regex that matches disallowed text

        public MechSelectorWindow(ModCopyModel modCopyModel)
        {
            this.MechSelectorModel = new MechSelectorModel(modCopyModel);
            this.InitializeComponent();
            this.DataContext = this;
        }

        public MechSelectorModel MechSelectorModel { get; }

        private static bool IsTextAllowed(string text)
        {
            return !Regex.IsMatch(text);
        }

        private void PreviewMaxProdYearInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }
    }
}