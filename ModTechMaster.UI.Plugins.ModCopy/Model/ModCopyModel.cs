using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Castle.Core.Logging;
using ModTechMaster.Core.Interfaces.Services;
using ModTechMaster.UI.Plugins.ModCopy.Annotations;
using ModTechMaster.UI.Plugins.ModCopy.Nodes;

namespace ModTechMaster.UI.Plugins.ModCopy.Model
{
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

        private readonly ILogger logger;
        private readonly IModService modService;

        private IMtmTreeViewItem currentSelectedItem;
        private string filterText;
        private ObservableCollection<MtmTreeViewItem> modCollectionData;

        private ModCopySettings settings;

        public ModCopyModel(IModService modService, ILogger logger)
        {
            this.modService = modService;
            this.logger = logger;
            this.modService.PropertyChanged += ModServiceOnPropertyChanged;
        }

        public ObservableCollection<MtmTreeViewItem> ModCollectionData
        {
            get => modCollectionData;
            set
            {
                if (Equals(value, modCollectionData)) return;
                modCollectionData = value;
                OnPropertyChanged();
            }
        }

        public string FilterText
        {
            get => filterText;
            set
            {
                if (FilterText != value)
                {
                    filterText = value;
                    var rootCollectionView = CollectionViewSource.GetDefaultView(modCollectionData);
                    rootCollectionView.Filter = node => ((IMtmTreeViewItem) node).Filter(FilterText);
                    OnPropertyChanged();
                }
            }
        }

        public ModCollectionNode ModCollectionNode { get; private set; }

        public IMtmTreeViewItem CurrentSelectedItem
        {
            get => currentSelectedItem;
            set
            {
                if (value == currentSelectedItem) return;

                currentSelectedItem = value;
                OnPropertyChanged();
            }
        }

        public ModCopySettings Settings
        {
            get => settings;
            set
            {
                if (value == settings) return;

                settings = value;
                ModCollectionNode.SelectMods(Settings.AlwaysIncludedMods);
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            CurrentSelectedItem = e.NewValue as IMtmTreeViewItem;
        }

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void ModServiceOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ModCollection")
            {
                ModCollectionNode = new ModCollectionNode(modService.ModCollection, null);
                modCollectionData = new ObservableCollection<MtmTreeViewItem> {ModCollectionNode};
                OnPropertyChanged("ModCollectionNode");
            }
        }
    }
}