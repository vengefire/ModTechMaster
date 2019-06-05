namespace ModTechMaster.UI.Plugins.ModCopy
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;

    using Castle.Core.Logging;

    using ModTechMaster.Core.Interfaces.Services;
    using ModTechMaster.UI.Plugins.Core.Interfaces;
    using ModTechMaster.UI.Plugins.ModCopy.Model;
    using ModTechMaster.UI.Plugins.ModCopy.Nodes;

    /// <summary>
    ///     Interaction logic for ModCopyPage.xaml
    /// </summary>
    public partial class ModCopyPage : UserControl, IPluginControl
    {
        public ModCopyPage(IModService modService, ILogger logger, IMtmMainModel mainModel)
        {
            Self = this;
            this.ModCopyModel = new ModCopyModel(modService, logger, mainModel);
            this.InitializeComponent();
            this.ModCopyModel.PropertyChanged += (sender, args) =>
                {
                    if (args.PropertyName == "ModCollectionNode")
                    {
                        this.Dispatcher.Invoke(
                            () => this.tvModControl.ItemsSource = this.ModCopyModel.ModCollectionData);
                    }
                };
            this.tvModControl.SelectedItemChanged += this.ModCopyModel.OnSelectedItemChanged;
            this.PluginCommands = new List<IPluginCommand> { ModCopyModel.ResetSelectionsCommand, ModCopyModel.SelectMechsFromDataFileCommand, ModCopyModel.BuildCustomCollectionCommand };
            this.DataContext = this;
        }

        public static ModCopyPage Self { get; private set; }

        public ModCopyModel ModCopyModel { get; }

        public string ModuleName => @"Mod Copy";

        public Type PageType => typeof(ModCopyPage);

        public List<IPluginCommand> PluginCommands { get; }

        public object Settings
        {
            get => this.ModCopyModel.Settings;
            set => this.ModCopyModel.Settings = value as ModCopySettings;
        }

        public Type SettingsType => typeof(ModCopySettings);

        private static TreeViewItem VisualUpwardSearch(DependencyObject source)
        {
            while (source != null && !(source is TreeViewItem))
            {
                source = VisualTreeHelper.GetParent(source);
            }

            return source as TreeViewItem;
        }

        private void TvModControl_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var treeViewItem = VisualUpwardSearch(e.OriginalSource as DependencyObject);

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
    }
}