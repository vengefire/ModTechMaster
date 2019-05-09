using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ModTechMaster.Logic.Factories;
using ModTechMaster.Logic.Services;
using ModTechMaster.Nodes;

namespace ModTechMaster
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string FilterText { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            var modService = new ModService(new MessageService(), new ManifestEntryProcessorFactory());
            var collectionData = modService.LoadCollectionFromPath(@"D:\source\repos\vf\ModTechMaster\TestData\In\Mods",
                "Test Collection");

            var collectionNode = new ModCollectionNode(collectionData, null);
            tvModControl.ItemsSource = new ObservableCollection<ModCollectionNode> {collectionNode};
        }

        private void BtnSelectAll_OnClick(object sender, RoutedEventArgs e)
        {
            throw new System.NotImplementedException();
        }

        private void BtnDeselectAll_OnClick(object sender, RoutedEventArgs e)
        {
            throw new System.NotImplementedException();
        }

        private async void TextBoxBase_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            int startLength = tb.Text.Length;

            await Task.Delay(300);
            if (startLength == tb.Text.Length)
                Console.WriteLine("Run Filter");
        }
    }
}