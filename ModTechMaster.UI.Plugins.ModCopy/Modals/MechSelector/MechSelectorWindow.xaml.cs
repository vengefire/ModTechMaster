namespace ModTechMaster.UI.Plugins.ModCopy.Modals.MechSelector
{
    using System.Windows;

    using ModTechMaster.UI.Plugins.ModCopy.Model;

    /// <summary>
    ///     Interaction logic for MechSelectorWindow.xaml
    /// </summary>
    public partial class MechSelectorWindow : Window
    {
        private readonly MechSelectorModel mechSelectorModel;

        public MechSelectorWindow(ModCopyModel modCopyModel)
        {
            this.mechSelectorModel = new MechSelectorModel(modCopyModel);
            this.InitializeComponent();
            this.DataContext = this;
        }

        public MechSelectorModel MechSelectorModel => this.mechSelectorModel;
    }
}