namespace ModTechMaster.UI.Plugins.ModCopy.Floaters
{
    using System.Windows;

    using ModTechMaster.UI.Plugins.ModCopy.Model;

    /// <summary>
    ///     Interaction logic for ObjectSummaryWindow.xaml
    /// </summary>
    public partial class ObjectSummaryWindow : Window
    {
        public ObjectSummaryWindow(ModCopyModel modCopyModel)
        {
            this.ObjectSummaryViewModel = new ObjectSummaryViewModel(modCopyModel);
            this.InitializeComponent();
            this.DataContext = this.ObjectSummaryViewModel;
        }

        public ObjectSummaryViewModel ObjectSummaryViewModel { get; }
    }
}