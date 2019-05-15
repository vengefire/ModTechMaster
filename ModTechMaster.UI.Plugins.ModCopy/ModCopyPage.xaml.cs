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
        private readonly ILogger logger;
        private readonly IModService modService;
        private readonly ObservableCollection<MTMTreeViewItem> modCollectionData;

        public ModCopyPage(IModService modService, ILogger logger)
        {
            this.modService = modService;
            this.logger = logger;
            this.InitializeComponent();
            this.PluginCommands = new List<IPluginCommand> {new ValidateModsCommand(null)};
            if (null != this.modService.ModCollection)
            {
                var collectionNode = new ModCollectionNode(modService.ModCollection, null);
                this.modCollectionData = new ObservableCollection<MTMTreeViewItem> { collectionNode };
                this.tvModControl.ItemsSource = this.modCollectionData;
            }
        }

        public string ModuleName => @"Mod Copy";
        public Type PageType => typeof(ModCopyPage);
        public List<IPluginCommand> PluginCommands { get; }

        private async void TextBoxBase_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var tb = (TextBox)sender;
            var startLength = tb.Text.Length;

            await Task.Delay(300);
            if (startLength == tb.Text.Length)
            {
                this.logger.Info("Run Filter");
            }
        }
    }
}