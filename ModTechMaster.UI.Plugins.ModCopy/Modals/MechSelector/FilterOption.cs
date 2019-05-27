namespace ModTechMaster.UI.Plugins.ModCopy.Modals.MechSelector
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    using ModTechMaster.UI.Plugins.ModCopy.Annotations;

    public class FilterOption : INotifyPropertyChanged
    {
        private string name;

        private bool selected;

        public FilterOption(string name, bool selected)
        {
            this.name = name;
            this.selected = selected;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string Name
        {
            get => this.name;
            set
            {
                if (value == this.name)
                {
                    return;
                }

                this.name = value;
                this.OnPropertyChanged();
            }
        }

        public bool Selected
        {
            get => this.selected;
            set
            {
                if (value == this.selected)
                {
                    return;
                }

                this.selected = value;
                this.OnPropertyChanged();
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}