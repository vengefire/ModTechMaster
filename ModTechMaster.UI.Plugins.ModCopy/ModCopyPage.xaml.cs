using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Controls;
using ModTechMaster.Core.Interfaces.Services;
using ModTechMaster.UI.Plugins.Core.Interfaces;
using ModTechMaster.UI.Plugins.ModCopy.Nodes;

namespace ModTechMaster.UI.Plugins.ModCopy
{
    /// <summary>
    ///     Interaction logic for ModCopyPage.xaml
    /// </summary>
    public partial class ModCopyPage : UserControl, IPluginModule
    {
        private readonly IModService _modService;
        private readonly ObservableCollection<MTMTreeViewItem> modCollectionData;

        public ModCopyPage(IModService modService)
        {
            _modService = modService;
            InitializeComponent();
            var collectionData = _modService.LoadCollectionFromPath(
                @"D:\source\repos\vf\ModTechMaster\TestData\In\Mods",
                "Test Collection");

            if (null != collectionData)
            {
                var collectionNode = new ModCollectionNode(collectionData, null);
                modCollectionData = new ObservableCollection<MTMTreeViewItem> {collectionNode};
                tvModControl.ItemsSource = modCollectionData;
            }
        }

        public string ModuleName => @"ModTechMaster - Mod Copy Module";
        public Type PageType => typeof(ModCopyPage);

        private async void TextBoxBase_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var tb = (TextBox) sender;
            var startLength = tb.Text.Length;

            await Task.Delay(300);
            if (startLength == tb.Text.Length) Console.WriteLine("Run Filter");
        }
    }
}