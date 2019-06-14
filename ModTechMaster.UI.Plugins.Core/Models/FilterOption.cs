namespace ModTechMaster.UI.Plugins.Core.Models
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    using ModTechMaster.UI.Plugins.Core.Annotations;

    public class FilterOption : INotifyPropertyChanged
    {
        private string name;

        private bool selected;

        private object value;

        public FilterOption(string name, bool selected = false, object value = null)
        {
            this.name = name;
            this.selected = selected;
            this.value = value;
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

        public object Value
        {
            get => this.value;
            set
            {
                if (value == this.value)
                {
                    return;
                }

                this.value = value;
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