namespace ModTechMaster.UI
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using Plugins.Core.Interfaces;
    using Plugins.Core.Services;

    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private PluginService pluginService;
        private List<IPlugin> plugins;

        public MainWindow()
        {
            this.InitializeComponent();
            this.InitializePlugins();
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

        private void InitializePlugins()
        {
            this.pluginService = new PluginService();
            this.plugins = this.pluginService.GetPlugins(".");
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