namespace ModTechMaster.UI.Plugins.ModCopy
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
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
    public partial class ModCopyPage : UserControl, IPluginControl
    {
        private readonly ILogger logger;
        private static ISettingsService settingsService;
        private readonly IModService modService;
        private ObservableCollection<MtmTreeViewItem> modCollectionData;

        public ModCopyPage(IModService modService, ILogger logger, ISettingsService settingsService)
        {
            this.modService = modService;
            this.logger = logger;
            ModCopyPage.settingsService = settingsService;
            this.ModCopyModel = new ModCopyModel(settingsService);
            this.InitializeComponent();
            this.tvModControl.SelectedItemChanged += this.ModCopyModel.OnSelectedItemChanged;
            this.PluginCommands = new List<IPluginCommand> {new ValidateModsCommand(null)};
            this.modService.PropertyChanged += this.ModServiceOnPropertyChanged;
            this.DataContext = this;
        }

        public ModCopyModel ModCopyModel { get; }

        public string FilterText { get; set; }

        public string ModuleName => @"Mod Copy";
        public Type PageType => typeof(ModCopyPage);
        public List<IPluginCommand> PluginCommands { get; }

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
                var collectionNode = new ModCollectionNode(this.modService.ModCollection, null);
                this.modCollectionData = new ObservableCollection<MtmTreeViewItem> {collectionNode};
                this.Dispatcher.Invoke(() => this.tvModControl.ItemsSource = this.modCollectionData);
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
    }
}