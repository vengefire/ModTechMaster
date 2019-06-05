namespace ModTechMaster.UI.Plugins.ModCopy.Model
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Input;

    using Castle.Core.Logging;

    using Framework.Utils.Directory;

    using ModTechMaster.Core.Interfaces.Models;
    using ModTechMaster.Core.Interfaces.Services;
    using ModTechMaster.UI.Plugins.Core.Interfaces;
    using ModTechMaster.UI.Plugins.ModCopy.Annotations;
    using ModTechMaster.UI.Plugins.ModCopy.Commands;
    using ModTechMaster.UI.Plugins.ModCopy.Modals.MechSelector;
    using ModTechMaster.UI.Plugins.ModCopy.Nodes;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

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

        public ModCopyModel(IModService modService, ILogger logger, IMtmMainModel mainModel)
        {
            this.modService = modService;
            this.logger = logger;
            this.MainModel = mainModel;
            this.modService.PropertyChanged += this.ModServiceOnPropertyChanged;
            ResetSelectionsCommand = new ResetSelectionsCommand(this);
            SelectMechsFromDataFileCommand = new SelectMechsFromDataFileCommand(this);
            BuildCustomCollectionCommand = new BuildCustomCollectionCommand(this);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public static IPluginCommand BuildCustomCollectionCommand { get; private set; }

        public static IPluginCommand ResetSelectionsCommand { get; private set; }

        public static IPluginCommand SelectMechsFromDataFileCommand { get; private set; }

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

        public IMtmMainModel MainModel { get; }

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

        public ModCollectionNode ModCollectionNode { get; private set; }

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

        public static void SelectMechsFromDataFile(ModCopyModel modCopyModel)
        {
            var mechSelectorWindow = new MechSelectorWindow(modCopyModel);
            mechSelectorWindow.Closed += MechSelectorWindowOnClosed;
            ModCopyPage.Self.WindowContainer.Children.Add(mechSelectorWindow);
            mechSelectorWindow.Show();
        }

        public void BuildCustomCollection()
        {
            this.MainModel.IsBusy = true;
            try
            {
                if (Directory.Exists(this.settings.OutputDirectory))
                {
                    Directory.Delete(this.settings.OutputDirectory, true);
                }

                // Copy all fully selected mod packages...
                this.ModCollectionNode.SelectedMods.Where(node => node.IsChecked == true).ToList().ForEach(
                    node =>
                        {
                            var di = new DirectoryInfo(node.Mod.SourceDirectoryPath);
                            DirectoryUtils.DirectoryCopy(di.FullName, this.GetModDestinationPath(di.Name), true, null);
                        });

                // Copy partially selected mod packages...
                this.ModCollectionNode.SelectedMods.Where(node => !node.IsChecked.HasValue).ToList().ForEach(
                    node =>
                        {
                            var mod = node.Mod;
                            var di = new DirectoryInfo(mod.SourceDirectoryPath);
                            var modDestinationDirectory = this.GetModDestinationPath(di.Name);
                            DirectoryUtils.EnsureExists(modDestinationDirectory);

                            // Copy Resource Files. Don't copy the mod.json file. This will be written after unselected items have been removed from child manifests.
                            mod.ResourceFiles
                                .Where(definition => definition.SourceFileName.ToLowerInvariant() != "mod.json")
                                .ToList().ForEach(
                                    definition =>
                                        {
                                            var sourceFilePath = definition.SourceFilePath;
                                            var destinationFilePath = Path.Combine(
                                                modDestinationDirectory,
                                                definition.SourceFileName);
                                            File.Copy(sourceFilePath, destinationFilePath);
                                        });

                            if (node.Children.FirstOrDefault(item => item is ManifestNode) is ManifestNode manifestNode)
                            {
                                var manifest = manifestNode.Object as IManifest;
                                var entryGroups = manifestNode.Children.Where(item => item is ManifestEntryNode)
                                    .Cast<ManifestEntryNode>().ToList();

                                // We need to check whether each entry instance has any selected items in the UI group.
                                foreach (var entry in manifest.Entries)
                                {
                                    var group = entryGroups.First(entryNode => entryNode.ManifestEntry.EntryType == entry.EntryType);
                                    var intersect = entry.Objects.Intersect(group.Children.Cast<ObjectDefinitionNode>().Where(definitionNode => definitionNode.IsChecked == true).Select(definitionNode => definitionNode.ObjectDefinition));
                                    if (!intersect.Any())
                                    {
                                        if (entry.JsonObject == null)
                                        {
                                            this.logger.Warn($@"Encountered null entry {entry.Id}");
                                            continue;
                                        }

                                        var jsonObject = (JObject)entry.JsonObject;
                                        var parent = jsonObject.Parent;
                                        if (parent != null)
                                        {
                                            jsonObject.Remove();
                                        }
                                        else
                                        {
                                            this.logger.Warn($@"Encountered parentless entry {entry.JsonString}");
                                        }
                                    }
                                }

                                var usedManifestEntries = manifestNode.Children.Where(item => item is ManifestEntryNode)
                                    .Cast<ManifestEntryNode>().Where(
                                        entryNode => entryNode.Children.Any(item => item.IsChecked == true)).ToList();

                                usedManifestEntries.ForEach(
                                    entryNode =>
                                        {
                                            var objects = entryNode.Children.Where(item => item.IsChecked == true)
                                                .Select(item => item.Object)
                                                .Cast<ISourcedFromFile>();
                                            var files = objects.Select(file => file.SourceFileName).ToList();
                                            DirectoryUtils.DirectoryCopy(
                                                Path.Combine(mod.SourceDirectoryPath, entryNode.ManifestEntry.Path),
                                                Path.Combine(modDestinationDirectory, entryNode.ManifestEntry.Path),
                                                true,
                                                files);
                                        });
                            }

                            File.WriteAllText(Path.Combine(modDestinationDirectory, "mod.json"), JsonConvert.SerializeObject(mod.JsonObject, Formatting.Indented));
                        });
            }
            finally
            {
                this.MainModel.IsBusy = false;
            }
        }

        public void OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            this.CurrentSelectedItem = e.NewValue as IMtmTreeViewItem;
        }

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

        private static string GetModDestinationPath(string targetCollectionDirectory, string modDirectoryName)
        {
            return Path.Combine(targetCollectionDirectory, modDirectoryName);
        }

        private static void MechSelectorWindowOnClosed(object sender, EventArgs e)
        {
            var selectorWindow = sender as MechSelectorWindow;
            if (selectorWindow.SelectMechs)
            {
                var mechsObjectsToSelect =
                    selectorWindow.MechSelectorModel.SelectedModels.SelectMany(
                        model => model.ObjectDefinitions.Select(o => o));
                foreach (var objectReference in mechsObjectsToSelect)
                {
                    IMtmTreeViewItem treeItem;
                    if (MtmTreeViewItem.DictRefsToTreeViewItems.TryGetValue(objectReference, out treeItem))
                    {
                        treeItem.IsChecked = true;
                    }
                }
            }
        }

        private string GetModDestinationPath(string modDirectoryName)
        {
            return GetModDestinationPath(this.settings.OutputDirectory, modDirectoryName);
        }

        private void ModServiceOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ModCollection")
            {
                this.ModCollectionNode = new ModCollectionNode(this.modService.ModCollection, null);
                this.modCollectionData = new ObservableCollection<MtmTreeViewItem> { this.ModCollectionNode };
                this.OnPropertyChanged("ModCollectionNode");
            }
        }

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}