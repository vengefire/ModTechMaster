namespace ModTechMaster.UI
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls.Ribbon;
    using Plugins.Core.Interfaces;
    using Plugins.Core.Services;

    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<IPlugin> plugins;
        private PluginService pluginService;

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
            var tab = new RibbonTab {Header = @"Plugins"};
            foreach (var plugin in this.plugins)
            {
                var group = new RibbonGroup {Header = plugin.Name};

                foreach (var module in plugin.Modules)
                {
                    var moduleButton = new RibbonButton
                    {
                        Label = module.ModuleName, Command = new PluginModuleCommand(), CommandParameter = new PluginModuleCommandData(this.frmContent, module)
                    };
                    group.Items.Add(moduleButton);
                }

                tab.Items.Add(group);
            }

            this.rbnMain.Items.Add(tab);
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
    }
}