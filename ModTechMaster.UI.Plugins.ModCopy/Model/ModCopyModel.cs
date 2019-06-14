namespace ModTechMaster.UI.Plugins.ModCopy.Model
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Forms;
    using System.Windows.Input;

    using Castle.Core.Logging;

    using Framework.Utils.Directory;

    using ModTechMaster.Core.Enums;
    using ModTechMaster.Core.Enums.Mods;
    using ModTechMaster.Core.Interfaces.Models;
    using ModTechMaster.Core.Interfaces.Services;
    using ModTechMaster.Data.Models.Mods.TypedObjectDefinitions;
    using ModTechMaster.UI.Core.WinForms.Extensions;
    using ModTechMaster.UI.Plugins.Core.Interfaces;
    using ModTechMaster.UI.Plugins.Core.Logic;
    using ModTechMaster.UI.Plugins.ModCopy.Annotations;
    using ModTechMaster.UI.Plugins.ModCopy.Commands;
    using ModTechMaster.UI.Plugins.ModCopy.Floaters;
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

        public static readonly ICommand AddObjectsFromHostManifestCommand =
            new DelegateCommand<Tuple<ModCopyPage, ObjectDefinitionNode>>(
                parameters =>
                    {
                        var model = parameters.Item1;
                        var objectDefinitionNode = parameters.Item2;
                        var groupedManifestEntryNode = objectDefinitionNode.Parent as ManifestEntryNode;
                        var owningManifestNodeObjects = groupedManifestEntryNode.ManifestEntryLookupByObject
                            .Where(
                                pair => pair.Value
                                        == groupedManifestEntryNode.ManifestEntryLookupByObject[objectDefinitionNode])
                            .Select(pair => pair.Key);
                        owningManifestNodeObjects.ToList().ForEach(item => item.IsChecked = true);
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

        public ModCopyModel(
            IModService modService,
            ILogger logger,
            IMtmMainModel mainModel,
            IReferenceFinderService referenceFinderService,
            IMessageService messageService)
        {
            this.modService = modService;
            this.logger = logger;
            this.ReferenceFinderService = referenceFinderService;
            this.MessageService = messageService;
            this.MainModel = mainModel;
            this.modService.PropertyChanged += this.ModServiceOnPropertyChanged;
            ResetSelectionsCommand = new ResetSelectionsCommand(this);
            SelectMechsFromDataFileCommand = new SelectMechsFromDataFileCommand(this);
            BuildCustomCollectionCommand = new BuildCustomCollectionCommand(this);
            ValidateLanceDefinitionsCommand = new ValidateLanceDefinitionsCommand(this);
            SelectVeesFromDataFileCommand = new SelectVeesFromDataFileCommand(this);
            ViewObjectSummaryWindow = new DelegatePluginCommand(
                () =>
                    {
                        if (ObjectSummaryWindow == null)
                        {
                            ObjectSummaryWindow = new ObjectSummaryWindow(this);
                            ObjectSummaryWindow.Topmost = true;
                            ObjectSummaryWindow.Show();
                        }
                        else
                        {
                            ObjectSummaryWindow.Activate();
                        }
                    },
                () => true,
                this,
                @"Show Object Summary Window");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public static IPluginCommand BuildCustomCollectionCommand { get; private set; }

        // ToDo: Make this a list of floaties...
        public static ObjectSummaryWindow ObjectSummaryWindow { get; private set; }

        public static IPluginCommand ResetSelectionsCommand { get; private set; }

        public static IPluginCommand SelectMechsFromDataFileCommand { get; private set; }

        public static IPluginCommand SelectVeesFromDataFileCommand { get; private set; }

        public static IPluginCommand ValidateLanceDefinitionsCommand { get; private set; }

        public static IPluginCommand ViewObjectSummaryWindow { get; private set; }

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

        public IMessageService MessageService { get; }

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

        public bool ProcessRefsOnLoad { get; set; } = false;

        public IReferenceFinderService ReferenceFinderService { get; }

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
                            await this.SelectImperativeMods().ConfigureAwait(false);
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

        public static void SelectVeesFromDataFile(ModCopyModel modCopyModel)
        {
            using (var fileDialog = new OpenFileDialog())
            {
                var result = fileDialog.ShowDialog(ModCopyPage.Self.GetIWin32Window());
                if (result == DialogResult.OK)
                {
                    var vehicleList = File.ReadAllLines(fileDialog.FileName).Select(s => s.Split(',')[0]).ToList();
                    Task.Run(() => modCopyModel.SelectVehiclesBySourceFileName(vehicleList));
                }
            }
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
                                    var group = entryGroups.First(entryNode => entryNode.EntryType == entry.EntryType);
                                    var intersect = entry.Objects.Intersect(
                                        group.Children.Cast<ObjectDefinitionNode>()
                                            .Where(definitionNode => definitionNode.IsChecked != false)
                                            .Select(definitionNode => definitionNode.ObjectDefinition));
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

                                // Skip copying prefabs, they're hosted in the selected asset bundle.
                                var manifestEntryNodes = manifestNode.Children.Where(item => item is ManifestEntryNode)
                                    .Cast<ManifestEntryNode>().Where(
                                        entryNode =>
                                            entryNode.EntryType != ObjectType.Prefab
                                            && entryNode.Children.Any(item => item.IsChecked != false)).ToList();

                                var manifestSelectionData = manifestEntryNodes
                                    .SelectMany(
                                        entryNode => entryNode.Children.Where(item => item.IsChecked != false).Select(
                                            item => new
                                                        {
                                                            entryNode,
                                                            manifest = entryNode.ManifestEntryLookupByObject[item],
                                                            itemNode = item,
                                                            itemObject = item.Object as IObjectDefinition
                                                        })).GroupBy(
                                        arg => arg.entryNode,
                                        (entryNode, items) =>
                                            new
                                                {
                                                    entryNode,
                                                    manifestEntries = items.GroupBy(
                                                        arg => arg.manifest,
                                                        (entry, objects) => new { entry, objects }).ToList()
                                                }).ToList();

                                manifestSelectionData.ForEach(
                                    entryNode =>
                                        {
                                            foreach (var manifestEntry in entryNode.manifestEntries)
                                            {
                                                var objects = manifestEntry.objects.ToList();
                                                var itemCollectionObjects = objects.Where(
                                                    o => o.itemObject.ObjectType == ObjectType.ItemCollectionDef);

                                                var objectTypesToIgnore =
                                                    new List<ObjectType>
                                                        {
                                                            ObjectType.ItemCollectionDef, ObjectType.Prefab
                                                        };

                                                var fileObjectsToCopy = objects.Where(
                                                        o => !objectTypesToIgnore.Contains(o.itemObject.ObjectType))
                                                    .Select(arg => arg.itemObject).Cast<ISourcedFromFile>();

                                                // Copy non-item list objects...
                                                var files = fileObjectsToCopy.Select(file => file.SourceFileName)
                                                    .ToList();
                                                DirectoryUtils.DirectoryCopy(
                                                    Path.Combine(mod.SourceDirectoryPath, manifestEntry.entry.Path),
                                                    Path.Combine(modDestinationDirectory, manifestEntry.entry.Path),
                                                    true,
                                                    files);

                                                // Handle Item Lists
                                                foreach (var itemCollectionObject in itemCollectionObjects)
                                                {
                                                    var itemCollection =
                                                        itemCollectionObject.itemObject as
                                                            ItemCollectionObjectDefinition;

                                                    var selectedItemLines = new List<string>
                                                                                {
                                                                                    string.Join(
                                                                                        ",",
                                                                                        itemCollection.Id,
                                                                                        string.Empty,
                                                                                        string.Empty,
                                                                                        string.Empty)
                                                                                };

                                                    var selectedItemNodes =
                                                        itemCollectionObject.itemNode.Dependencies.Where(
                                                            reference =>
                                                                {
                                                                    var referencedNode =
                                                                        MtmTreeViewItem.DictRefsToTreeViewItems[
                                                                            reference.ReferenceObject];
                                                                    return referencedNode.IsChecked != false;
                                                                });

                                                    selectedItemNodes.ToList().ForEach(
                                                        reference =>
                                                            {
                                                                var selectedItem = reference.ReferenceObject;
                                                                var matchedItemCollectionLine =
                                                                    itemCollection.CsvData.First(
                                                                        list => list.Any(
                                                                            s => string.Equals(
                                                                                s,
                                                                                selectedItem.Id,
                                                                                StringComparison.OrdinalIgnoreCase)));
                                                                selectedItemLines.Add(
                                                                    string.Join(",", matchedItemCollectionLine));
                                                            });

                                                    var targetFile = Path.Combine(
                                                        modDestinationDirectory,
                                                        manifestEntry.entry.Path,
                                                        itemCollection.SourceFileName);

                                                    File.WriteAllText(
                                                        targetFile,
                                                        string.Join("\r\n", selectedItemLines) + "\r\n");
                                                }
                                            }
                                        });

                                var handledPaths = manifest.Entries.Select(
                                    entry =>
                                        {
                                            var path = entry.Path;
                                            var subIndex = entry.Path.IndexOfAny(new[] { '/', '\\' });

                                            if (subIndex == -1)
                                            {
                                                return path;
                                            }

                                            return path.Substring(0, subIndex);
                                        }).Distinct();

                                var assetDirectories = di.EnumerateDirectories().Select(info => info.Name).ToList()
                                    .Except(handledPaths);

                                foreach (var assetDirectory in assetDirectories)
                                {
                                    DirectoryUtils.DirectoryCopy(
                                        Path.Combine(mod.SourceDirectoryPath, assetDirectory),
                                        Path.Combine(modDestinationDirectory, assetDirectory),
                                        true,
                                        null);
                                }
                            }

                            File.WriteAllText(
                                Path.Combine(modDestinationDirectory, "mod.json"),
                                JsonConvert.SerializeObject(mod.JsonObject, Formatting.Indented));
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
                        try
                        {
                            this.MainModel.IsBusy = true;
                            this.ModCollectionNode.SelectMods(this.Settings.AlwaysIncludedMods);
                        }
                        catch (Exception ex)
                        {
                            this.MessageService.PushMessage(ex.Message, MessageType.Error);
                            this.MessageService.PushMessage(ex.StackTrace, MessageType.Error);
                        }
                        finally
                        {
                            this.MainModel.IsBusy = false;
                        }
                    }).ConfigureAwait(false);
        }

        private static string GetModDestinationPath(string targetCollectionDirectory, string modDirectoryName)
        {
            return Path.Combine(targetCollectionDirectory, modDirectoryName);
        }

        private static void MechSelectorWindowOnClosed(object sender, EventArgs e)
        {
            Task.Run(
                () =>
                    {
                        var selectorWindow = sender as MechSelectorWindow;
                        try
                        {
                            selectorWindow.MechSelectorModel.ModCopyModel.MainModel.IsBusy = true;
                            if (selectorWindow.SelectMechs)
                            {
                                var mechsObjectsToSelect =
                                    selectorWindow.MechSelectorModel.SelectedModels.SelectMany(
                                        model => model.ObjectDefinitions.Select(o => o));

                                foreach (var objectReference in mechsObjectsToSelect)
                                {
                                    IMtmTreeViewItem treeItem;
                                    if (MtmTreeViewItem.DictRefsToTreeViewItems.TryGetValue(
                                        objectReference,
                                        out treeItem))
                                    {
                                        treeItem.IsChecked = true;
                                    }
                                }

                                selectorWindow.MechSelectorModel.ModCopyModel.ModCollectionNode
                                    .SelectAbsentModDependencies();
                            }
                        }
                        finally
                        {
                            selectorWindow.MechSelectorModel.ModCopyModel.MainModel.IsBusy = false;
                        }
                    }).ConfigureAwait(false);
        }

        private string GetModDestinationPath(string modDirectoryName)
        {
            return GetModDestinationPath(this.settings.OutputDirectory, modDirectoryName);
        }

        private void ModServiceOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ModCollection")
            {
                this.MessageService.PushMessage("Mod Collection updated. Processing References...", MessageType.Info);
                this.ReferenceFinderService.ReferenceableObjectProvider = this.modService.ModCollection;
                if (this.ProcessRefsOnLoad)
                {
                    var elapsedTime = this.ReferenceFinderService.ProcessAllReferences();
                    this.MessageService.PushMessage($"References processed in [{elapsedTime}]ms.", MessageType.Info);
                    this.MessageService.PushMessage("Building Mod Collection views...", MessageType.Info);
                }
                else
                {
                    this.MessageService.PushMessage("Skipping pre-processing of relationships...", MessageType.Info);
                }

                var stopwatch = new Stopwatch();
                stopwatch.Start();
                this.ModCollectionNode = new ModCollectionNode(
                    this.modService.ModCollection,
                    null,
                    this.ReferenceFinderService);
                stopwatch.Stop();
                this.MessageService.PushMessage(
                    $"Building Mod Collection complete. Process took {stopwatch.ElapsedMilliseconds}ms",
                    MessageType.Info);
                this.modCollectionData = new ObservableCollection<MtmTreeViewItem> { this.ModCollectionNode };
                this.OnPropertyChanged("ModCollectionNode");
            }
        }

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void SelectVehiclesBySourceFileName(List<string> vehicleList)
        {
            try
            {
                this.MainModel.IsBusy = true;
                this.ModCollectionNode.AllChildren
                    .Where(
                        item => item is ObjectDefinitionNode obj
                                && obj.ObjectDefinition.ObjectType == ObjectType.VehicleDef
                                && vehicleList.Contains(obj.ObjectDefinition.SourceFileName)).ToList()
                    .ForEach(item => item.IsChecked = true);
            }
            catch (Exception ex)
            {
                this.MessageService.PushMessage(ex.Message, MessageType.Error);
            }
            finally
            {
                this.MainModel.IsBusy = false;
            }
        }
    }
}