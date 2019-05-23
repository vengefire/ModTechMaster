namespace ModTechMaster.UI.Plugins.ModCopy.Model
{
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Windows.Data;

    using ModTechMaster.Core.Enums.Mods;
    using ModTechMaster.Core.Interfaces.Models;
    using ModTechMaster.UI.Plugins.ModCopy.Annotations;

    public class ModCopySettings : INotifyPropertyChanged
    {
        private bool autoIncludeDependents;

        private ObservableCollection<ObjectType> dependentTypesToIgnore = new ObservableCollection<ObjectType>();

        private string outputDirectory;

        public ModCopySettings()
        {
            var view = CollectionViewSource.GetDefaultView(this.AlwaysIncludedMods);
            view.SortDescriptions.Add(new SortDescription(string.Empty, ListSortDirection.Ascending));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<string> AlwaysIncludedMods { get; } = new ObservableCollection<string>();

        public bool AutoIncludeDependents
        {
            get => this.autoIncludeDependents;
            set
            {
                if (value == this.autoIncludeDependents)
                {
                    return;
                }

                this.autoIncludeDependents = value;
                this.OnPropertyChanged();
            }
        }

        public ObservableCollection<ObjectType> DependentTypesToIgnore
        {
            get => this.dependentTypesToIgnore;
            set
            {
                if (value == this.dependentTypesToIgnore)
                {
                    return;
                }

                this.dependentTypesToIgnore = value;
                this.OnPropertyChanged();
            }
        }

        public string OutputDirectory
        {
            get => this.outputDirectory;
            set
            {
                if (value == this.outputDirectory)
                {
                    return;
                }

                this.outputDirectory = value;
                this.OnPropertyChanged();
            }
        }

        public void AddImperativeMod(IMod mod)
        {
            if (!this.AlwaysIncludedMods.Contains(mod.Name))
            {
                this.AlwaysIncludedMods.Add(mod.Name);
                this.OnPropertyChanged(nameof(this.AlwaysIncludedMods));
            }
        }

        public void RemoveImperativeMod(IMod mod)
        {
            if (this.AlwaysIncludedMods.Contains(mod.Name))
            {
                this.AlwaysIncludedMods.Remove(mod.Name);
                this.OnPropertyChanged(nameof(this.AlwaysIncludedMods));
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}