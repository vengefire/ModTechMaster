using System.ComponentModel;
using System.Linq;
using System.Windows.Data;

namespace ModTechMaster.UI.Plugins.ModCopy
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Threading.Tasks;
    using System.Windows.Controls;
    using Castle.Core.Logging;
    using Commands;
    using Core.Interfaces;
    using ModTechMaster.Core.Interfaces.Services;
    using Nodes;

    /// <summary>
    ///     Interaction logic for ModCopyPage.xaml
    /// </summary>
    public partial class ModCopyPage : UserControl, IPluginControl
    {
        public string FilterText { get; set; }
        private readonly ILogger logger;
        private readonly IModService modService;
        private ObservableCollection<MTMTreeViewItem> modCollectionData;

        private bool Filter(object obj)
        {
            if (string.IsNullOrEmpty(FilterText))
                return true;
            return false;
        }

        public ModCopyPage(IModService modService, ILogger logger)
        {
            this.modService = modService;
            this.logger = logger;
            this.InitializeComponent();
            this.PluginCommands = new List<IPluginCommand> {new ValidateModsCommand(null)};
            this.modService.PropertyChanged += ModServiceOnPropertyChanged;
        }

        private void ModServiceOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ModCollection")
            {
                var collectionNode = new ModCollectionNode(modService.ModCollection, null);
                this.modCollectionData = new ObservableCollection<MTMTreeViewItem> {collectionNode};
                this.Dispatcher.Invoke(() => this.tvModControl.ItemsSource = this.modCollectionData);
            }
        }

        public string ModuleName => @"Mod Copy";
        public Type PageType => typeof(ModCopyPage);
        public List<IPluginCommand> PluginCommands { get; }

        private async void TextBoxBase_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var tb = (TextBox)sender;
            var startLength = tb.Text.Length;

            await Task.Delay(750);
            if (startLength == tb.Text.Length)
            {
                FilterText = tb.Text;
                var rootCollectionView = CollectionViewSource.GetDefaultView(modCollectionData);
                rootCollectionView.Filter = (node => ((IMTMTreeViewItem) node).Filter(FilterText));
            }
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            txtFilter.Clear();
        }
    }
}