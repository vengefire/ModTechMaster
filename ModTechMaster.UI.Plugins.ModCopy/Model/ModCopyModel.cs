namespace ModTechMaster.UI.Plugins.ModCopy.Model
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Input;
    using Annotations;
    using Castle.Core.Logging;
    using Commands;
    using Core.Interfaces;
    using ModTechMaster.Core.Interfaces.Services;
    using Nodes;

    public sealed class ModCopyModel : INotifyPropertyChanged
    {
        public static readonly ICommand AddModToImperativeListCommand =
            new DelegateCommand<Tuple<ModCopyPage, ModNode>>(
                parameters =>
                {
                    var model = parameters.Item1;
                    var mod = parameters.Item2;
                    var settings = model.Settings as ModCopySettings;
                    Debug.Assert(settings != null, nameof(settings) + " != null");
                    settings.AddImperativeMod(mod.Mod);
                    mod.IsChecked = true;
                });

        public static readonly ICommand RemoveModFromImperativeListCommand =
            new DelegateCommand<Tuple<ModCopyPage, ModNode>>(
                parameters =>
                {
                    var model = parameters.Item1;
                    var mod = parameters.Item2;
                    var settings = model.Settings as ModCopySettings;
                    Debug.Assert(settings != null, nameof(settings) + " != null");
                    settings.RemoveImperativeMod(mod.Mod);
                    mod.IsChecked = false;
                });

        public static IPluginCommand ResetSelectionsCommand;

        private readonly ILogger logger;
        public IMtmMainModel MainModel { get; }
        private readonly IModService modService;

        private IMtmTreeViewItem currentSelectedItem;
        private string filterText;
        private ObservableCollection<MtmTreeViewItem> modCollectionData;

        private ModCopySettings settings;

        public ModCopyModel(IModService modService, ILogger logger, IMtmMainModel mainModel)
        {
            this.modService = modService;
            this.logger = logger;
            this.MainModel = mainModel;
            this.modService.PropertyChanged += this.ModServiceOnPropertyChanged;
            ModCopyModel.ResetSelectionsCommand = new ResetSelectionsCommand(this);
        }

        public ObservableCollection<MtmTreeViewItem> ModCollectionData
        {
            get => this.modCollectionData;
            set
            {
                if (value == this.modCollectionData)
                {
                    return;
                }

                this.modCollectionData = value;
                this.OnPropertyChanged();
            }
        }

        public string FilterText
        {
            get => this.filterText;
            set
            {
                if (this.FilterText != value)
                {
                    this.filterText = value;
                    var rootCollectionView = CollectionViewSource.GetDefaultView(this.modCollectionData);
                    rootCollectionView.Filter = node => ((IMtmTreeViewItem)node).Filter(this.FilterText);
                    this.OnPropertyChanged();
                }
            }
        }

        public ModCollectionNode ModCollectionNode { get; private set; }

        public IMtmTreeViewItem CurrentSelectedItem
        {
            get => this.currentSelectedItem;
            set
            {
                if (value == this.currentSelectedItem)
                {
                    return;
                }

                this.currentSelectedItem = value;
                this.OnPropertyChanged();
            }
        }

        public ModCopySettings Settings
        {
            get => this.settings;
            set
            {
                if (value == this.settings)
                {
                    return;
                }

                this.settings = value;
                Task.Run(
                    async () =>
                    {
                        await this.SelectImperativeMods();
                        this.OnPropertyChanged();
                    });
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public async Task ResetModSelections()
        {
            var query = this.ModCollectionNode.AllChildren.Where(item => item.IsChecked != false).AsParallel();
            await Task.Run(
                async () =>
                {
                    this.MainModel.IsBusy = true;
                    query.ForAll(item => item.IsChecked = false);
                    await this.SelectImperativeMods();
                    this.MainModel.IsBusy = false;
                });
        }

        public async Task SelectImperativeMods()
        {
            await Task.Run(
                () =>
                {
                    this.MainModel.IsBusy = true;
                    this.ModCollectionNode.SelectMods(this.Settings.AlwaysIncludedMods);
                    this.MainModel.IsBusy = false;
                });
        }

        public void OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            this.CurrentSelectedItem = e.NewValue as IMtmTreeViewItem;
        }

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void ModServiceOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ModCollection")
            {
                this.ModCollectionNode = new ModCollectionNode(this.modService.ModCollection, null);
                this.modCollectionData = new ObservableCollection<MtmTreeViewItem> {this.ModCollectionNode};
                this.OnPropertyChanged("ModCollectionNode");
            }
        }
    }
}