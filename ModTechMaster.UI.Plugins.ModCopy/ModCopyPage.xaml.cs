namespace ModTechMaster.UI.Plugins.ModCopy
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Input;
    using System.Windows.Media;
    using Annotations;
    using Castle.Core.Logging;
    using Commands;
    using Core.Interfaces;
    using Model;
    using ModTechMaster.Core.Interfaces.Services;
    using Nodes;

    // TBD : Refactor this logic into the model.

    /// <summary>
    ///     Interaction logic for ModCopyPage.xaml
    /// </summary>
    public partial class ModCopyPage : UserControl, IPluginControl, INotifyPropertyChanged
    {
        private static ModCopyPage self;
        private readonly ILogger logger;
        private readonly IModService modService;
        private ObservableCollection<MtmTreeViewItem> modCollectionData;

        public ModCopyPage(IModService modService, ILogger logger)
        {
            ModCopyPage.self = this;
            this.modService = modService;
            this.logger = logger;
            this.ModCopyModel = new ModCopyModel();
            this.InitializeComponent();
            this.tvModControl.SelectedItemChanged += this.ModCopyModel.OnSelectedItemChanged;
            this.PluginCommands = new List<IPluginCommand> {new ValidateModsCommand(null)};
            this.modService.PropertyChanged += this.ModServiceOnPropertyChanged;
            this.DataContext = this;
            this.ModCopyModel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "Settings")
                {
                    // Settings were reallocated, hook up watch event...
                    var model = sender as ModCopyModel;
                    var settings = model.Settings;
                    settings.PropertyChanged += (setingsSender, settingsArgs) =>
                    {
                        if (settingsArgs.PropertyName == "AlwaysIncludedMods")
                        {
                            this.ModCollectionNode.SelectMods(this.ModCopyModel.Settings.AlwaysIncludedMods);
                        }
                    };
                    // We also need to initialize our auto include mods...
                    this.ModCollectionNode.SelectMods(this.ModCopyModel.Settings.AlwaysIncludedMods);
                }
            };
        }

        public ModCollectionNode ModCollectionNode { get; private set; }

        public static ModCopyPage Self => ModCopyPage.self;

        public ModCopyModel ModCopyModel { get; }

        public string FilterText { get; set; }
        public Type PageType => typeof(ModCopyPage);

        public Type SettingsType => typeof(ModCopySettings);
        public string ModuleName => @"Mod Copy";
        public List<IPluginCommand> PluginCommands { get; }

        public object Settings { get => this.ModCopyModel.Settings; set => this.ModCopyModel.Settings = value as ModCopySettings; }

        private bool Filter(object obj)
        {
            if (string.IsNullOrEmpty(this.FilterText))
            {
                return true;
            }

            return false;
        }

        private void ModServiceOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ModCollection")
            {
                this.ModCollectionNode = new ModCollectionNode(this.modService.ModCollection, null);
                this.modCollectionData = new ObservableCollection<MtmTreeViewItem> {this.ModCollectionNode};
                this.Dispatcher.Invoke(() => this.tvModControl.ItemsSource = this.modCollectionData);
                this.OnPropertyChanged("ModCollectionNode");
            }
        }

        private async void TextBoxBase_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var tb = (TextBox)sender;
            var startLength = tb.Text.Length;

            await Task.Delay(750);
            if (startLength == tb.Text.Length)
            {
                this.FilterText = tb.Text;
                var rootCollectionView = CollectionViewSource.GetDefaultView(this.modCollectionData);
                rootCollectionView.Filter = node => ((IMtmTreeViewItem)node).Filter(this.FilterText);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.txtFilter.Clear();
        }

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

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}