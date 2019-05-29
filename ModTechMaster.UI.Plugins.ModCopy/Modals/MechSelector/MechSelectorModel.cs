namespace ModTechMaster.UI.Plugins.ModCopy.Modals.MechSelector
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Windows.Data;
    using System.Windows.Forms;

    using Castle.Core.Internal;

    using ModTechMaster.Core.Enums.Mods;
    using ModTechMaster.Core.Interfaces.Models;
    using ModTechMaster.UI.Plugins.ModCopy.Annotations;
    using ModTechMaster.UI.Plugins.ModCopy.Model;

    public class MechSelectorModel : INotifyPropertyChanged
    {
        private readonly List<IReferenceableObject> mechs;

        private readonly ModCopyModel modCopyModel;

        private ObservableCollection<FilterOption> eraOptions = new ObservableCollection<FilterOption>();

        private long? maxProdYear;

        private ListCollectionView mechCollectionView;

        private string mechFilePath;

        private ObservableCollection<MechModel> mechModels = new ObservableCollection<MechModel>();

        private bool nonExtinctOnly;

        private ObservableCollection<FilterOption> rulesOptions = new ObservableCollection<FilterOption>();

        private ObservableCollection<FilterOption> techOptions = new ObservableCollection<FilterOption>();

        public MechSelectorModel(ModCopyModel modCopyModel)
        {
            this.modCopyModel = modCopyModel;
            this.mechs = this.modCopyModel.ModCollectionNode.ModCollection.GetReferenceableObjects()
                .Where(referenceableObject => referenceableObject.ObjectType == ObjectType.ChassisDef).ToList();

            // Hit it with a hammer for the moment...
            this.PropertyChanged += this.Filter;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<FilterOption> EraOptions
        {
            get => this.eraOptions;
            set
            {
                if (value == this.eraOptions)
                {
                    return;
                }

                this.eraOptions = value;
                this.OnPropertyChanged();
            }
        }

        public long? MaxProdYear
        {
            get => this.maxProdYear;
            set
            {
                if (value == this.maxProdYear)
                {
                    return;
                }

                this.maxProdYear = value;
                this.OnPropertyChanged();
            }
        }

        public ListCollectionView MechCollectionView =>
            this.mechCollectionView ?? (this.mechCollectionView = CollectionViewSource.GetDefaultView(this.MechModels) as ListCollectionView);

        public List<MechModel> DisplayedModels
        {
            get
            {
                var list = new List<MechModel>();
                foreach (var item in this.MechCollectionView)
                {
                    list.Add(item as MechModel);
                }

                return list;
            }
        }

        public string MechFilePath
        {
            get => this.mechFilePath;
            set
            {
                if (value == this.mechFilePath)
                {
                    return;
                }

                this.mechFilePath = value;
                this.OnPropertyChanged();
            }
        }

        public List<MechModel> SelectedModels => this.MechModels?.Where(model => model.Selected).ToList();

        public ObservableCollection<MechModel> MechModels
        {
            get => this.mechModels;
            set
            {
                if (this.mechModels != value)
                {
                    this.mechModels = value;
                    var collectionView = this.mechCollectionView = CollectionViewSource.GetDefaultView(this.MechModels) as ListCollectionView;

                    collectionView.Filter = o =>
                        {
                            var mech = o as MechModel;
                            if (mech.Designer != "Catalyst Game Labs" || mech.ResidentInCollection == false)
                            {
                                return false;
                            }

                            return true;
                        };
                    collectionView.SortDescriptions.Add(
                        new SortDescription(nameof(MechModel.Year), ListSortDirection.Ascending));
                    collectionView.SortDescriptions.Add(
                        new SortDescription(nameof(MechModel.Name), ListSortDirection.Ascending));
                    this.OnPropertyChanged(nameof(this.MechModels));

                    this.MechModels?.Select(model => model.Era).Distinct().OrderBy(s => s).ToList().ToList().ForEach(
                        s =>
                            {
                                var option = new FilterOption(s, false);
                                option.PropertyChanged += this.Filter;
                                this.EraOptions.Add(option);
                            });

                    this.OnPropertyChanged(nameof(this.EraOptions));
                    this.MechModels?.Select(model => model.RulesLevel).Distinct().OrderBy(s => s).ToList().ForEach(
                        s =>
                            {
                                var option = new FilterOption(s, false);
                                option.PropertyChanged += this.Filter;
                                this.RulesOptions.Add(option);
                            });

                    this.OnPropertyChanged(nameof(this.RulesOptions));
                    this.MechModels?.Select(model => model.TechnologyBase).Distinct().OrderBy(s => s).ToList().ForEach(
                        s =>
                            {
                                var option = new FilterOption(s, false);
                                option.PropertyChanged += this.Filter;
                                this.TechOptions.Add(option);
                            });
                    this.MechModels?.ToList().ForEach(model => model.PropertyChanged += (sender, args) => this.OnPropertyChanged(nameof(this.SelectedModels)));
                    this.OnPropertyChanged(nameof(this.DisplayedModels));
                }
            }
        }

        public bool NonExtinctOnly
        {
            get => this.nonExtinctOnly;
            set
            {
                if (value == this.nonExtinctOnly)
                {
                    return;
                }

                this.nonExtinctOnly = value;
                this.OnPropertyChanged();
            }
        }

        public ObservableCollection<FilterOption> RulesOptions
        {
            get => this.rulesOptions;
            set
            {
                if (Equals(value, this.rulesOptions))
                {
                    return;
                }

                this.rulesOptions = value;
                this.OnPropertyChanged();
            }
        }

        public ObservableCollection<FilterOption> TechOptions
        {
            get => this.techOptions;
            set
            {
                if (Equals(value, this.techOptions))
                {
                    return;
                }

                this.techOptions = value;
                this.OnPropertyChanged();
            }
        }

        public void Filter(object sender, PropertyChangedEventArgs e)
        {
            var filterProperties = new List<string>()
                                       {
                                           nameof(this.MaxProdYear),
                                           nameof(this.NonExtinctOnly),
                                       };
            if (sender is FilterOption || !filterProperties.Contains(e.PropertyName))
            {
                return;
            }

            if (this.MechModels == null)
            {
                return;
            }

            var eras = this.EraOptions.Where(option => option.Selected).Select(option => option.Name).ToList();
            var rules = this.RulesOptions.Where(option => option.Selected).Select(option => option.Name).ToList();
            var techs = this.TechOptions.Where(option => option.Selected).Select(option => option.Name).ToList();

            var collectionView = this.MechCollectionView;

            collectionView.Filter = o =>
                {
                    var mech = o as MechModel;

                    if (!mech.ResidentInCollection)
                    {
                        return false;
                    }

                    if (mech.Designer != "Catalyst Game Labs")
                    {
                        return false;
                    }

                    if (eras.Any() && !eras.Contains(mech.Era))
                    {
                        return false;
                    }

                    if (rules.Any() && !rules.Contains(mech.RulesLevel))
                    {
                        return false;
                    }

                    if (techs.Any() && !techs.Contains(mech.TechnologyBase))
                    {
                        return false;
                    }

                    if (this.NonExtinctOnly && mech.Extinct)
                    {
                        return false;
                    }

                    if (this.MaxProdYear != null && mech.Year > this.MaxProdYear)
                    {
                        return false;
                    }

                    return true;
                };
            this.OnPropertyChanged(nameof(this.DisplayedModels));
        }

        internal static bool CanProcessMechSelectionFile(MechSelectorModel mechSelectorModel)
        {
            return true;
        }

        internal static void ProcessMechSelectionFile(MechSelectorModel mechSelectorModel)
        {
            using (var fileDialog = new OpenFileDialog())
            {
                // var result = fileDialog.ShowDialog(this.GetIWin32Window());
                var result = fileDialog.ShowDialog();
                if (result == DialogResult.OK)
                {
                    mechSelectorModel.MechFilePath = fileDialog.FileName;
                }
            }

            if (!File.Exists(mechSelectorModel.MechFilePath))
            {
                throw new ArgumentException(
                    $"The file [{mechSelectorModel.MechFilePath}] does not exist.",
                    nameof(mechSelectorModel.MechFilePath));
            }

            var mechList = new List<MechModel>();

            using (var reader = new StreamReader(mechSelectorModel.MechFilePath))
            {
                // First line headers...
                var line = reader.ReadLine();
                while ((line = reader.ReadLine()) != null)
                {
                    if (string.IsNullOrEmpty(line))
                    {
                        continue;
                    }

                    var mech = MechModel.FromCsv(line);

                    var searchTerm1 = ($"{mech.BaseModel}".Replace(" ", string.Empty) + $"_{mech.Variant}").ToLower();
                    var searchTerm2 = mech.HeroName.IsNullOrEmpty()
                                          ? string.Empty
                                          : ($"{mech.HeroName}".Replace(" ", string.Empty) + $"_{mech.Variant}")
                                          .ToLower();
                    if (mechSelectorModel.mechs.Any(
                        referenceableObject =>
                            {
                                var test = referenceableObject.Id.ToLower();
                                return test.Contains(searchTerm1)
                                       || !searchTerm2.IsNullOrEmpty() && test.Contains(searchTerm2);
                            }))
                    {
                        mech.ResidentInCollection = true;
                    }

                    mechList.Add(mech);
                }
            }

            mechList.Sort((model, mechModel) => string.Compare(model.Name, mechModel.Name, StringComparison.Ordinal));
            mechSelectorModel.MechModels = new ObservableCollection<MechModel>(mechList);
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}