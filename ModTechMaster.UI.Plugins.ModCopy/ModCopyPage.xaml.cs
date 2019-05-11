﻿namespace ModTechMaster.UI.Plugins.ModCopy
{
    using System;
    using System.Collections.ObjectModel;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using Core.Interfaces;
    using Logic.Factories;
    using Logic.Services;
    using Nodes;

    /// <summary>
    ///     Interaction logic for ModCopyPage.xaml
    /// </summary>
    public partial class ModCopyPage : Page, IPluginModule
    {
        private ObservableCollection<MTMTreeViewItem> modCollectionData;

        public ModCopyPage()
        {
            this.InitializeComponent();
        }

        public string ModuleName => @"ModTechMaster - Mod Copy Module";
        public string PageSource => @"ModCopyPage.xaml";
        public Type PageType => typeof(ModCopyPage);

        private async void TextBoxBase_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var tb = (TextBox)sender;
            var startLength = tb.Text.Length;

            await Task.Delay(300);
            if (startLength == tb.Text.Length)
            {
                Console.WriteLine("Run Filter");
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var modService = new ModService(new MessageService(), new ManifestEntryProcessorFactory());
            var collectionData = modService.LoadCollectionFromPath(
                @"C:\dev\repos\ModTechMaster\TestData\In\Mods",
                "Test Collection");

            if (null != collectionData)
            {
                var collectionNode = new ModCollectionNode(collectionData, null);
                this.modCollectionData = new ObservableCollection<MTMTreeViewItem> {collectionNode};
                this.tvModControl.ItemsSource = this.modCollectionData;
            }
        }
    }
}