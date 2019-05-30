﻿namespace ModTechMaster.UI.Plugins.ModCopy.Modals.MechSelector
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
    using ModTechMaster.UI.Plugins.ModCopy.Annotations;
    using ModTechMaster.UI.Plugins.ModCopy.Model;

    public class MechSelectorModel : INotifyPropertyChanged
    {
        private readonly List<IReferenceableObject> mechs;

        private readonly ModCopyModel modCopyModel;

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
            this.modCopyModel = modCopyModel;
            var chassisTypes = new List<ObjectType> { ObjectType.MechDef, ObjectType.VehicleDef };
            this.mechs = this.modCopyModel.ModCollectionNode.ModCollection.GetReferenceableObjects()
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

                                this.OnPropertyChanged(nameof(this.UnfilteredMechs));
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

        public List<MechModel> UnfilteredMechs
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
            this.OnPropertyChanged(nameof(this.UnfilteredMechs));
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
                mechSelectorModel.modCopyModel.MainModel.IsBusy = true;

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

                        var searchTerm1 =
                            ($"{mech.BaseModel}".Replace(" ", string.Empty) + $"_{mech.Variant}").ToLower();
                        var searchTerm2 = mech.HeroName.IsNullOrEmpty()
                                              ? string.Empty
                                              : ($"{mech.HeroName}".Replace(" ", string.Empty) + $"_{mech.Variant}")
                                              .ToLower();

                        var objectDefinition = mechSelectorModel.mechs.FirstOrDefault(
                            referenceableObject =>
                                {
                                    var test = referenceableObject.Id.ToLower();
                                    return test.Contains(searchTerm1)
                                           || !searchTerm2.IsNullOrEmpty() && test.Contains(searchTerm2);
                                });

                        if (objectDefinition != null)
                        {
                            mech.ResidentInCollection = true;
                            mech.ObjectDefinition = objectDefinition;
                        }

                        mechList.Add(mech);
                    }
                }

                mechSelectorModel.MechModels = new ObservableCollection<MechModel>(mechList);
            }
            finally
            {
                mechSelectorModel.modCopyModel.MainModel.IsBusy = false;
            }
        }

        internal void SelectAllMechs(bool value, List<MechModel> unfilteredMechs)
        {
            this.modCopyModel.MainModel.IsBusy = true;
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

            this.modCopyModel.MainModel.IsBusy = false;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}