namespace ModTechMaster.UI
{
    using System;
    using System.Windows;

    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
            /*var modService = new ModService(new MessageService(), new ManifestEntryProcessorFactory());
            var collectionData = modService.LoadCollectionFromPath(
                                                                   @"C:\dev\repos\ModTechMaster\TestData\In\Mods",
                                                                   "Test Collection");

            if (null != collectionData)
            {
                var collectionNode = new ModCollectionNode(collectionData, null);
                //this.tvModControl.ItemsSource = new ObservableCollection<MTMTreeViewItem> {collectionNode};
            }*/
        }

        /*public string FilterText { get; set; }

        private async void TextBoxBase_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var tb = (TextBox)sender;
            var startLength = tb.Text.Length;

            await Task.Delay(300);
            if (startLength == tb.Text.Length)
            {
                Console.WriteLine("Run Filter");
            }
        }*/

        private void Home_Button_Click(object sender, RoutedEventArgs e)
        {
            this.frmContent.Source = null;
        }

        private void ModCopy_Button_Click(object sender, RoutedEventArgs e)
        {
            //this.frmContent.Navigate(new Uri("pack://application:,,,/ModTechMaster.UI.Plugins.ModCopy;ModCopyPage.xaml", UriKind.RelativeOrAbsolute));
            this.frmContent.Navigate(new Uri("/ModTechMaster.UI.Plugins.ModCopy;component/ModCopyPage.xaml", UriKind.RelativeOrAbsolute));
        }
    }
}