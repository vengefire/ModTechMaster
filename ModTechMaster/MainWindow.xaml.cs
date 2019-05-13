using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Framework.Interfaces.Injection;
using ModTechMaster.UI.Plugins.Core.Interfaces;
using ModTechMaster.UI.Plugins.Core.Services;

namespace ModTechMaster.UI
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<IPlugin> plugins;
        private PluginService pluginService;

        public MainWindow()
        {
            InitializeComponent();
            InitializePlugins();
        }

        private void InitializePlugins()
        {
            pluginService = new PluginService();
            plugins = pluginService.GetPlugins(".");
            foreach (var plugin in plugins)
            foreach (var module in plugin.Modules)
            {
                var moduleTab = new TabItem {Header = module.ModuleName};
                var modulePage = Container.Instance.GetInstance(module.PageType);
                moduleTab.Content = modulePage;
                tabPages.Items.Add(moduleTab);
            }
        }
    }
}