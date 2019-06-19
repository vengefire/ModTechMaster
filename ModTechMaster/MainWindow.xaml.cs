namespace ModTechMaster.UI
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;

    using Framework.Interfaces.Injection;

    using ModTechMaster.UI.Commands;
    using ModTechMaster.UI.Plugins.Core.Interfaces;
    using ModTechMaster.UI.Plugins.Core.Services;

    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IMtmMainModel mainModel;

        private List<IPlugin> plugins;

        private PluginService pluginService;

        public MainWindow(IMtmMainModel mainModel)
        {
            this.InitializeComponent();
            this.InitializePlugins();
            this.mainModel = mainModel;
            this.DataContext = mainModel;
        }

        private void ClearMessages_OnClick(object sender, RoutedEventArgs e)
        {
            this.mainModel.MessageService.ClearMessages();
        }

        private void InitializePlugins()
        {
            this.pluginService = new PluginService();
            this.plugins = this.pluginService.GetPlugins(".");
            foreach (var plugin in this.plugins)
            {
                var moduleTab = new TabItem { Header = plugin.Name };
                var modulePage = Container.Instance.GetInstance(plugin.PageType);
                moduleTab.Content = modulePage;
                this.tabPages.Items.Add(moduleTab);
            }
        }

        // TODO: this needs to go into a single activation spot
        private void TabPages_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var tabControl = sender as TabControl;

            // Every Tab Control in the entire app, I tell you... ffs.
            if (tabControl == null || tabControl.Name != "tabPages")
            {
                return;
            }

            // Remove non-common tool bars...
            var toolbars = this.toolbarTray.ToolBars.Where(bar => bar.Name != "tbCommon").ToList();
            toolbars.ForEach(bar => this.toolbarTray.ToolBars.Remove(bar));

            var tabItem = (sender as TabControl).SelectedItem as TabItem;
            if (!(tabItem?.Content is IPluginControl))
            {
                return;
            }

            var pluginModule = (IPluginControl)tabItem.Content;
            var pluginToolbar = new ToolBar();

            foreach (var command in pluginModule.PluginCommands)
            {
                pluginToolbar.Items.Add(
                    new Button
                        {
                            Content = command.Name, Command = command, CommandParameter = command.CommandParameter
                        });
            }

            this.toolbarTray.ToolBars.Add(pluginToolbar);

            // Init settings and commands...
            this.mainModel.CurrentPluginControl = pluginModule;
            if (pluginModule.Settings == null)
            {
                CommonCommands.LoadCurrentSettingsCommand.Execute(pluginModule);
            }
        }
    }
}