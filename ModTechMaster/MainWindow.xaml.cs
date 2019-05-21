using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using ModTechMaster.Annotations;
using ModTechMaster.UI.Commands;
using ModTechMaster.UI.Plugins.Core.Interfaces;
using ModTechMaster.UI.Plugins.Core.Services;
using Container = Framework.Interfaces.Injection.Container;

namespace ModTechMaster.UI
{
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
            InitializeComponent();
            InitializePlugins();
            this.mainModel = mainModel;
            DataContext = mainModel;
        }

        private void InitializePlugins()
        {
            pluginService = new PluginService();
            plugins = pluginService.GetPlugins(".");
            foreach (var plugin in plugins)
            {
                var moduleTab = new TabItem {Header = plugin.Name};
                var modulePage = Container.Instance.GetInstance(plugin.PageType);
                moduleTab.Content = modulePage;
                tabPages.Items.Add(moduleTab);
            }
        }

        private void TabPages_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var tabControl = sender as TabControl;
            // Every Tab Control in the entire app, I tell you... ffs.
            if (tabControl == null || tabControl.Name != "tabPages") return;

            // Remove non-common tool bars...
            var toolbars = toolbarTray.ToolBars.Where(bar => bar.Name != "tbCommon").ToList();
            toolbars.ForEach(bar => toolbarTray.ToolBars.Remove(bar));

            var tabItem = (sender as TabControl).SelectedItem as TabItem;
            if (!(tabItem?.Content is IPluginControl)) return;


            var pluginModule = (IPluginControl) tabItem.Content;
            var pluginToolbar = new ToolBar();

            foreach (var command in pluginModule.PluginCommands)
                pluginToolbar.Items.Add(new Button
                    {Content = command.Name, Command = command, CommandParameter = command.CommandParameter});
            toolbarTray.ToolBars.Add(pluginToolbar);

            // Init settings and commands...
            this.mainModel.CurrentPluginControl = pluginModule;
            CommonCommands.LoadCurrentSettingsCommand.Execute(pluginModule);
        }
    }
}