namespace ModTechMaster.UI.Plugins.ModCopy.Model
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using Annotations;
    using ModTechMaster.Core.Enums.Mods;
    using ModTechMaster.Core.Interfaces.Models;

    public class ModCopySettings : INotifyPropertyChanged
    {
        private HashSet<IMod> alwaysIncludedMods = new HashSet<IMod>();
        private HashSet<ObjectType> dependentTypesToIgnore = new HashSet<ObjectType>();
        private string outputDirectory;
        private bool autoIncludeDependents;

        public HashSet<IMod> AlwaysIncludedMods
        {
            get => this.alwaysIncludedMods;
            set
            {
                if (Equals(value, this.alwaysIncludedMods)) return;
                this.alwaysIncludedMods = value;
                this.OnPropertyChanged();
            }
        }

        public HashSet<ObjectType> DependentTypesToIgnore
        {
            get => this.dependentTypesToIgnore;
            set
            {
                if (Equals(value, this.dependentTypesToIgnore)) return;
                this.dependentTypesToIgnore = value;
                this.OnPropertyChanged();
            }
        }

        public string OutputDirectory
        {
            get => this.outputDirectory;
            set
            {
                if (value == this.outputDirectory) return;
                this.outputDirectory = value;
                this.OnPropertyChanged();
            }
        }

        public bool AutoIncludeDependents
        {
            get => this.autoIncludeDependents;
            set
            {
                if (value == this.autoIncludeDependents) return;
                this.autoIncludeDependents = value;
                this.OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}