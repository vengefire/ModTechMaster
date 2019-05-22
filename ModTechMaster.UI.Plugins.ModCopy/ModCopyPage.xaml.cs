using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Castle.Core.Logging;
using ModTechMaster.Core.Interfaces.Services;
using ModTechMaster.UI.Plugins.Core.Interfaces;
using ModTechMaster.UI.Plugins.ModCopy.Commands;
using ModTechMaster.UI.Plugins.ModCopy.Model;
using ModTechMaster.UI.Plugins.ModCopy.Nodes;

namespace ModTechMaster.UI.Plugins.ModCopy
{
    /// <summary>
    ///     Interaction logic for ModCopyPage.xaml
    /// </summary>
    public partial class ModCopyPage : UserControl, IPluginControl
    {
        public ModCopyPage(IModService modService, ILogger logger)
        {
            Self = this;
            ModCopyModel = new ModCopyModel(modService, logger);
            InitializeComponent();
            ModCopyModel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "ModCollectionNode")
                {
                    Dispatcher.Invoke(() => this.tvModControl.ItemsSource = ModCopyModel.ModCollectionData);
                }
            };
            tvModControl.SelectedItemChanged += ModCopyModel.OnSelectedItemChanged;
            PluginCommands = new List<IPluginCommand> {new ValidateModsCommand(null)};
            DataContext = this;
        }

        public static ModCopyPage Self { get; private set; }

        public ModCopyModel ModCopyModel { get; }

        public Type PageType => typeof(ModCopyPage);

        public Type SettingsType => typeof(ModCopySettings);
        public string ModuleName => @"Mod Copy";
        public List<IPluginCommand> PluginCommands { get; }

        public object Settings
        {
            get => ModCopyModel.Settings;
            set => ModCopyModel.Settings = value as ModCopySettings;
        }

        private void TvModControl_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var treeViewItem = VisualUpwardSearch(e.OriginalSource as DependencyObject);

            if (treeViewItem != null)
            {
                treeViewItem.Focus();
                e.Handled = true;
                var contextMenu = TreeViewContextMenuSelector.GetContextMenu(treeViewItem.DataContext);
                if (contextMenu.Items.Count == 0) return;

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