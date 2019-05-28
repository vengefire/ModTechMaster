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
    using System.Windows.Input;

    using ModTechMaster.Core.Enums.Mods;
    using ModTechMaster.Core.Interfaces.Models;
    using ModTechMaster.UI.Plugins.ModCopy.Annotations;
    using ModTechMaster.UI.Plugins.ModCopy.Model;

    public class MechSelectorModel : INotifyPropertyChanged
    {
        private readonly List<IReferenceableObject> mechs;

        private readonly ModCopyModel modCopyModel;

        private ObservableCollection<FilterOption> eraOptions = new ObservableCollection<FilterOption>();

        private long maxProdYear;

        private string mechFilePath;

        private ObservableCollection<MechModel> mechModels;

        private bool nonExtinctOnly;

        private ObservableCollection<FilterOption> rulesOptions = new ObservableCollection<FilterOption>();

        private ObservableCollection<FilterOption> techOptions = new ObservableCollection<FilterOption>();

        public MechSelectorModel(ModCopyModel modCopyModel)
        {
            this.modCopyModel = modCopyModel;
            this.mechs = this.modCopyModel.ModCollectionNode.ModCollection.GetReferenceableObjects()
                .Where(referenceableObject => referenceableObject.ObjectType == ObjectType.ChassisDef).ToList();
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

        public long MaxProdYear
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
                    var collectionView = CollectionViewSource.GetDefaultView(this.mechModels);
                    collectionView.Filter = o =>
                        {
                            var mech = o as MechModel;
                            if (mech.Designer != "Catalyst Game Labs")
                            {
                                return false;
                            }

                            var searchTerm1 = ($"{mech.BaseModel}".Replace(" ", string.Empty) + $"_{mech.Variant}")
                                .ToLower();
                            var searchTerm2 = ($"{mech.HeroName}".Replace(" ", string.Empty) + $"_{mech.Variant}")
                                .ToLower();
                            if (this.mechs.Any(
                                referenceableObject =>
                                    {
                                        var test = referenceableObject.Id.ToLower();
                                        return test.Contains(searchTerm1) || test.Contains(searchTerm2);
                                    }))
                            {
                                return true;
                            }

                            return false;
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

                    this.OnPropertyChanged(nameof(this.TechOptions));
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
                var view = CollectionViewSource.GetDefaultView(this.mechModels);

                // Do the filter...
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
            var eras = this.EraOptions.Where(option => option.Selected).Select(option => option.Name).ToList();
            var rules = this.RulesOptions.Where(option => option.Selected).Select(option => option.Name).ToList();
            var techs = this.TechOptions.Where(option => option.Selected).Select(option => option.Name).ToList();

            var collectionView = CollectionViewSource.GetDefaultView(this.MechModels);

            collectionView.Filter = o =>
                {
                    var mech = o as MechModel;
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

                    return true;
                };
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

                    mechList.Add(MechModel.FromCsv(line));
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