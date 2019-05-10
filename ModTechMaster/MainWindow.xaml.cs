namespace ModTechMaster
{
    using System;
    using System.Collections.ObjectModel;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using Logic.Factories;
    using Logic.Services;
    using Nodes;

    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
            var modService = new ModService(new MessageService(), new ManifestEntryProcessorFactory());
            var collectionData = modService.LoadCollectionFromPath(
                                                                   @"C:\dev\repos\ModTechMaster\TestData\In\Mods",
                                                                   "Test Collection");

            if (null != collectionData)
            {
                var collectionNode = new ModCollectionNode(collectionData, null);
                this.tvModControl.ItemsSource = new ObservableCollection<MTMTreeViewItem> {collectionNode};
            }
        }

        public string FilterText { get; set; }

        private async void TextBoxBase_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var tb = (TextBox)sender;
            var startLength = tb.Text.Length;

            await Task.Delay(300);
            if (startLength == tb.Text.Length)
            {
                Console.WriteLine("Run Filter");
            }
        }
    }
}