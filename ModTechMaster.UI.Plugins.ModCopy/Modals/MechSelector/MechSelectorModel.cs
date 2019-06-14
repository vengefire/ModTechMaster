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

    using Castle.Core.Internal;

    using ModTechMaster.Core.Enums.Mods;
    using ModTechMaster.Core.Interfaces.Models;
    using ModTechMaster.UI.Plugins.Core.Models;
    using ModTechMaster.UI.Plugins.ModCopy.Annotations;
    using ModTechMaster.UI.Plugins.ModCopy.Model;

    public class MechSelectorModel : INotifyPropertyChanged
    {
        public readonly ModCopyModel ModCopyModel;

        private readonly List<IReferenceableObject> mechs;

        private ObservableCollection<FilterOption> chassisOptions = new ObservableCollection<FilterOption>();

        private ObservableCollection<FilterOption> eraOptions = new ObservableCollection<FilterOption>();

        private long? maxProdYear;

        private string mechFilePath;

        private ObservableCollection<MechModel> mechModels;

        private bool nonExtinctOnly;

        private bool residentInCollectionOnly = true;

        private ObservableCollection<FilterOption> rulesOptions = new ObservableCollection<FilterOption>();

        private ObservableCollection<FilterOption> techOptions = new ObservableCollection<FilterOption>();

        public MechSelectorModel(ModCopyModel modCopyModel)
        {
            this.ModCopyModel = modCopyModel;
            var chassisTypes = new List<ObjectType>
                                   {
                                       ObjectType.MechDef

                                       // ObjectType.VehicleDef
                                   };
            this.mechs = this.ModCopyModel.ModCollectionNode.ModCollection.GetReferenceableObjects()
                .Where(referenceableObject => chassisTypes.Contains(referenceableObject.ObjectType)).ToList();

            // Hit it with a hammer for the moment...
            this.PropertyChanged += this.Filter;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<FilterOption> ChassisOptions
        {
            get => this.chassisOptions;
            set
            {
                if (value == this.chassisOptions)
                {
                    return;
                }

                this.chassisOptions = value;
                this.OnPropertyChanged();
            }
        }

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

        public ObservableCollection<MechModel> MechModels
        {
            get => this.mechModels;
            set
            {
                if (this.mechModels != value)
                {
                    this.mechModels = value;

                    var eras = new List<FilterOption>();
                    this.MechModels.Select(model => model.Era).Distinct().OrderBy(s => s).ToList().ForEach(
                        s =>
                            {
                                var option = new FilterOption(s, false);
                                option.PropertyChanged += this.Filter;
                                eras.Add(option);
                            });

                    var rules = new List<FilterOption>();
                    this.MechModels.Select(model => model.RulesLevel).Distinct().OrderBy(s => s).ToList().ForEach(
                        s =>
                            {
                                var option = new FilterOption(s, false);
                                option.PropertyChanged += this.Filter;
                                rules.Add(option);
                            });

                    var techs = new List<FilterOption>();
                    this.MechModels.Select(model => model.TechnologyBase).Distinct().OrderBy(s => s).ToList().ForEach(
                        s =>
                            {
                                var option = new FilterOption(s, false);
                                option.PropertyChanged += this.Filter;
                                techs.Add(option);
                            });

                    var chassis = new List<FilterOption>();
                    this.MechModels.Select(model => model.Chassis).Distinct().OrderBy(s => s).ToList().ForEach(
                        s =>
                            {
                                var option = new FilterOption(s, false);
                                option.PropertyChanged += this.Filter;
                                chassis.Add(option);
                            });

                    // Listen for changes to our models...
                    this.MechModels.ToList().ForEach(
                        model => model.PropertyChanged +=
                                     (sender, args) => this.OnPropertyChanged(nameof(this.SelectedModels)));

                    MechSelectorWindow.Self.Dispatcher.Invoke(
                        () =>
                            {
                                this.EraOptions = new ObservableCollection<FilterOption>(eras);
                                this.RulesOptions = new ObservableCollection<FilterOption>(rules);
                                this.TechOptions = new ObservableCollection<FilterOption>(techs);
                                this.ChassisOptions = new ObservableCollection<FilterOption>(chassis);
                            });

                    MechSelectorWindow.Self.Dispatcher.Invoke(
                        () =>
                            {
                                var collectionView = CollectionViewSource.GetDefaultView(this.MechModels);

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

                                this.OnPropertyChanged(nameof(this.FilteredMechs));
                                this.OnPropertyChanged(nameof(this.MechModels));
                            });
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

        public bool ResidentInCollectionOnly
        {
            get => this.residentInCollectionOnly;
            set
            {
                if (value == this.residentInCollectionOnly)
                {
                    return;
                }

                this.residentInCollectionOnly = value;
                this.OnPropertyChanged();
            }
        }

        public ObservableCollection<FilterOption> RulesOptions
        {
            get => this.rulesOptions;
            set
            {
                if (value == this.rulesOptions)
                {
                    return;
                }

                this.rulesOptions = value;
                this.OnPropertyChanged();
            }
        }

        public List<MechModel> SelectedModels => this.MechModels?.Where(model => model.Selected).ToList();

        public ObservableCollection<FilterOption> TechOptions
        {
            get => this.techOptions;
            set
            {
                if (value == this.techOptions)
                {
                    return;
                }

                this.techOptions = value;
                this.OnPropertyChanged();
            }
        }

        public List<MechModel> FilteredMechs
        {
            get
            {
                var view = CollectionViewSource.GetDefaultView(this.MechModels) as ListCollectionView;
                if (view == null)
                {
                    return null;
                }

                return this.MechModels.Where(model => view.PassesFilter(model)).ToList();
            }
        }

        public void Filter(object sender, PropertyChangedEventArgs e)
        {
            var filterProperties = new List<string>
                                       {
                                           nameof(this.MaxProdYear),
                                           nameof(this.NonExtinctOnly),
                                           nameof(this.ResidentInCollectionOnly)
                                       };

            if (!(sender is FilterOption || filterProperties.Contains(e.PropertyName)))
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
            var chassis = this.ChassisOptions.Where(option => option.Selected).Select(option => option.Name).ToList();

            var collectionView = CollectionViewSource.GetDefaultView(this.MechModels);

            collectionView.Filter = o =>
                {
                    var mech = o as MechModel;

                    if (this.ResidentInCollectionOnly && !mech.ResidentInCollection)
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

                    if (chassis.Any() && !chassis.Contains(mech.Chassis))
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
            this.OnPropertyChanged(nameof(this.FilteredMechs));
        }

        internal static bool CanProcessMechSelectionFile(MechSelectorModel mechSelectorModel)
        {
            return mechSelectorModel?.MechFilePath.IsNullOrEmpty() == false
                   && File.Exists(mechSelectorModel.MechFilePath);
        }

        internal static void ProcessMechSelectionFile(MechSelectorModel mechSelectorModel)
        {
            try
            {
                mechSelectorModel.ModCopyModel.MainModel.IsBusy = true;

                if (!File.Exists(mechSelectorModel.MechFilePath))
                {
                    throw new ArgumentException(
                        $@"The file [{mechSelectorModel.MechFilePath}] does not exist.",
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

                        var filteredMechs = mechSelectorModel.mechs.Where(
                            o => o.Id.StartsWith(
                                     $"mechdef_{mech.PrimitiveBaseModel.Replace(" ", string.Empty)}",
                                     StringComparison.OrdinalIgnoreCase) || !mech.PossibleClanName.IsNullOrEmpty()
                                 && o.Id.StartsWith(
                                     $"mechdef_{mech.PossibleClanName.Replace(" ", string.Empty)}",
                                     StringComparison.OrdinalIgnoreCase));

                        var objectDefinitions = filteredMechs.Where(
                            referenceableObject =>
                                {
                                    var testParts = referenceableObject.Id.ToLower().Split('_');
                                    var baseModel = testParts[1];

                                    // var potentialVariantIds = new List<string>();
                                    var variantId = testParts.Length >= 3 ? testParts[2] : "N/A";
                                    var hadToSplitBaseModel = false; // FFS RT

                                    var alternativeVariantId = string.Empty;

                                    if (testParts.Length == 2)
                                    {
                                        // Handle RT bad classification 1
                                        var subParts = testParts[1].Split('-');
                                        if (subParts.Length == 2)
                                        {
                                            baseModel = subParts[0];
                                            variantId = subParts[1];
                                            hadToSplitBaseModel = true;
                                        }
                                    }

                                    if (testParts.Length == 3)
                                    {
                                        // Handle RT bad classification 5
                                        var subParts = testParts[2].Split('-');
                                        if (subParts.Length > 1 && subParts[1].Length == 1)
                                        {
                                            alternativeVariantId =
                                                subParts
                                                    [1]; // Have to use an alternative variantId here because it's ambiguous one way or the other.
                                        }
                                    }

                                    if (testParts.Length > 3)
                                    {
                                        if (testParts[3].Length == 1)
                                        {
                                            // Handle RT bad classification 2
                                            variantId = testParts[3];
                                        }
                                        else
                                        {
                                            // Handle RT bad classification 3
                                            var concatenatedVariant = testParts[2];
                                            for (var i = 3; i < testParts.Length; i++)
                                            {
                                                concatenatedVariant += $"-{testParts[i]}";
                                            }

                                            alternativeVariantId = concatenatedVariant;
                                        }
                                    }

                                    // Handle RT bad classification 4
                                    var heroName = string.Empty;
                                    if (testParts.Length >= 4)
                                    {
                                        var potentialHeroName = string.Empty;
                                        for (var i = 3; i < testParts.Length; i++)
                                        {
                                            potentialHeroName += "_" + testParts[i];
                                        }

                                        if (potentialHeroName.Any(char.IsDigit) && !potentialHeroName.Contains("_"))
                                        {
                                            heroName = string.Empty;
                                        }
                                        else
                                        {
                                            heroName = potentialHeroName;
                                        }
                                    }

                                    var matchedOnClanName = false;
                                    var isCorrectBase = baseModel == mech.BaseModel.ToLower()
                                                        || baseModel == mech.BaseModel.Replace(" ", string.Empty)
                                                            .ToLower() || baseModel
                                                        == mech.PrimitiveBaseModel.ToLower();

                                    // Fuck IT ROGUE TECH, USE THE OFFICIAL CLAN NAME, NOT THE FKN IS NAME
                                    if (!isCorrectBase && baseModel == mech.PossibleClanName.ToLower())
                                    {
                                        isCorrectBase = true;
                                        matchedOnClanName = true;
                                    }

                                    var isCorrectVariant =
                                        variantId == mech.Variant.ToLower()
                                        || hadToSplitBaseModel && mech.Variant.Split('-').Length > 1
                                                               && mech.Variant.Split('-')[1].ToLower() == variantId
                                        || // JFC RT
                                        alternativeVariantId == mech.Variant.ToLower() || // JFC RT
                                        variantId == "clan" && mech.Variant.ToLower() == "c" || // JFC RT
                                        variantId == "p" && mech.Variant.ToLower() == "prime"; // JFC RT AGAIN

                                    var matches = 0;

                                    var testValue = referenceableObject.Id.ToLower();
                                    if (isCorrectBase)
                                    {
                                        matches += 1;
                                    }

                                    if (isCorrectVariant)
                                    {
                                        matches += 1;
                                    }

                                    var hasHeroName =
                                        !matchedOnClanName
                                        && (!mech.HeroName.IsNullOrEmpty() || !heroName.IsNullOrEmpty());
                                    if (!mech.HeroName.IsNullOrEmpty() && !heroName.IsNullOrEmpty()
                                                                       && testValue.Contains(mech.HeroName.ToLower()))
                                    {
                                        matches += 1;
                                    }

                                    if (!hasHeroName && matches >= 2 || hasHeroName && matches >= 3)
                                    {
                                        return true;
                                    }

                                    return false;
                                }).ToList();

                        if (objectDefinitions.Any())
                        {
                            mech.ResidentInCollection = true;
                            mech.ObjectDefinitions = objectDefinitions;
                        }

                        mechList.Add(mech);
                    }
                }

                mechSelectorModel.MechModels = new ObservableCollection<MechModel>(mechList);
            }
            finally
            {
                mechSelectorModel.ModCopyModel.MainModel.IsBusy = false;
            }
        }

        internal void SelectAllMechs(bool value, List<MechModel> unfilteredMechs)
        {
            this.ModCopyModel.MainModel.IsBusy = true;
            if (!value)
            {
                this.MechModels.ToList().ForEach(model => model.Selected = false);
            }
            else
            {
                this.MechModels.AsParallel().ForAll(
                    model =>
                        {
                            var isUnfiltered = unfilteredMechs.Contains(model);

                            if (!model.Selected && isUnfiltered)
                            {
                                model.Selected = true;
                            }
                            else if (model.Selected && !isUnfiltered)
                            {
                                model.Selected = false;
                            }
                        });
            }

            this.ModCopyModel.MainModel.IsBusy = false;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}