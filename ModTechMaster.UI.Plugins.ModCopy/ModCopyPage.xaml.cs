namespace ModTechMaster.UI.Plugins.ModCopy
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
    using Castle.Core.Logging;
    using Commands;
    using Core.Interfaces;
    using Model;
    using ModTechMaster.Core.Interfaces.Services;
    using Nodes;

    /// <summary>
    ///     Interaction logic for ModCopyPage.xaml
    /// </summary>
    public partial class ModCopyPage : UserControl, IPluginControl
    {
        public ModCopyPage(IModService modService, ILogger logger, IMtmMainModel mainModel)
        {
            ModCopyPage.Self = this;
            this.ModCopyModel = new ModCopyModel(modService, logger, mainModel);
            this.InitializeComponent();
            this.ModCopyModel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "ModCollectionNode")
                {
                    this.Dispatcher.Invoke(() => this.tvModControl.ItemsSource = this.ModCopyModel.ModCollectionData);
                }
            };
            this.tvModControl.SelectedItemChanged += this.ModCopyModel.OnSelectedItemChanged;
            this.PluginCommands = new List<IPluginCommand> {ModCopyModel.ResetSelectionsCommand};
            this.DataContext = this;
        }

        public static ModCopyPage Self { get; private set; }

        public ModCopyModel ModCopyModel { get; }

        public Type PageType => typeof(ModCopyPage);

        public Type SettingsType => typeof(ModCopySettings);
        public string ModuleName => @"Mod Copy";
        public List<IPluginCommand> PluginCommands { get; }

        public object Settings { get => this.ModCopyModel.Settings; set => this.ModCopyModel.Settings = value as ModCopySettings; }

        private void TvModControl_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var treeViewItem = ModCopyPage.VisualUpwardSearch(e.OriginalSource as DependencyObject);

            if (treeViewItem != null)
            {
                treeViewItem.Focus();
                e.Handled = true;
                var contextMenu = TreeViewContextMenuSelector.GetContextMenu(treeViewItem.DataContext);
                if (contextMenu.Items.Count == 0)
                {
                    return;
                }

                contextMenu.PlacementTarget = treeViewItem;
                contextMenu.IsOpen = true;
            }
        }

        private static TreeViewItem VisualUpwardSearch(DependencyObject source)
        {
            while (source != null &&
                !(source is TreeViewItem))
                source = VisualTreeHelper.GetParent(source);

            return source as TreeViewItem;
        }
    }
}