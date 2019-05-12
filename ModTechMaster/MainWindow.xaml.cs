namespace ModTechMaster.UI
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;
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
                foreach (var module in plugin.Modules)
                {
                    var moduleTab = new TabItem {Header = module.ModuleName};
                    moduleTab.Content = Activator.CreateInstance(module.PageType);
                    this.tabPages.Items.Add(moduleTab);
                }
            }
        }
    }
}