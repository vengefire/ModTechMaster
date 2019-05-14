using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Controls;
using Castle.Core.Logging;
using ModTechMaster.Core.Interfaces.Services;
using ModTechMaster.UI.Plugins.Core.Interfaces;
using ModTechMaster.UI.Plugins.ModCopy.Commands;
using ModTechMaster.UI.Plugins.ModCopy.Nodes;

namespace ModTechMaster.UI.Plugins.ModCopy
{
    /// <summary>
    ///     Interaction logic for ModCopyPage.xaml
    /// </summary>
    public partial class ModCopyPage : UserControl, IPluginControl
    {
        private readonly IModService _modService;
        private readonly ILogger _logger;
        private readonly ObservableCollection<MTMTreeViewItem> modCollectionData;

        public ModCopyPage(IModService modService, ILogger logger)
        {
            _modService = modService;
            _logger = logger;

            InitializeComponent();

            PluginCommands = new List<IPluginCommand>()
            {
                new LoadModsCommand(null),
                new ValidateModsCommand(null),
            };

            /*var collectionData = _modService.LoadCollectionFromPath(
                @"D:\source\repos\vf\ModTechMaster\TestData\In\Mods",
                "Test Collection");

            if (null != collectionData)
            {
                var collectionNode = new ModCollectionNode(collectionData, null);
                modCollectionData = new ObservableCollection<MTMTreeViewItem> {collectionNode};
                tvModControl.ItemsSource = modCollectionData;
            }*/
        }

        public string ModuleName => @"Mod Copy";
        public Type PageType => typeof(ModCopyPage);
        public List<IPluginCommand> PluginCommands { get; }

        private async void TextBoxBase_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var tb = (TextBox) sender;
            var startLength = tb.Text.Length;

            await Task.Delay(300);
            if (startLength == tb.Text.Length) _logger.Info("Run Filter");
        }
    }
}