namespace ModTechMaster.UI
{
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;
    using Framework.Interfaces.Injection;
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
        }

        private void InitializePlugins()
        {
            this.pluginService = new PluginService();
            this.plugins = this.pluginService.GetPlugins(".");
            foreach (var plugin in this.plugins)
            {
                var moduleTab = new TabItem {Header = plugin.Name};
                var modulePage = Container.Instance.GetInstance(plugin.PageType);
                moduleTab.Content = modulePage;
                this.tabPages.Items.Add(moduleTab);
            }
        }

        private void TabPages_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(sender is TabControl))
            {
                return;
            }

            this.toolbarTray.ToolBars.Clear();

            var tabItem = (sender as TabControl).SelectedItem as TabItem;
            if (!(tabItem?.Content is IPluginControl))
            {
                return;
            }

            var pluginModule = (IPluginControl)tabItem.Content;
            var pluginToolbar = new ToolBar();

            foreach (var command in pluginModule.PluginCommands)
                pluginToolbar.Items.Add(new Button {Content = command.Name, Command = command, CommandParameter = command.CommandParameter});
            this.toolbarTray.ToolBars.Add(pluginToolbar);
        }
    }
}