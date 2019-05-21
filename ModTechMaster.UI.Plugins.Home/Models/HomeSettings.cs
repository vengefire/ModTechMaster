namespace ModTechMaster.UI.Plugins.Home.Models
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using Annotations;

    public class HomeSettings : INotifyPropertyChanged
    {
        private string modCollectionName;
        private string modDirectory;

        public string ModCollectionName
        {
            get => this.modCollectionName;
            set
            {
                if (this.modCollectionName != value)
                {
                    this.modCollectionName = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public string ModDirectory
        {
            get => this.modDirectory;
            set
            {
                if (this.modDirectory != value)
                {
                    this.modDirectory = value;
                    this.OnPropertyChanged();
                }
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