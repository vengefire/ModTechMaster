namespace ModTechMaster.UI.Models
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using Annotations;
    using Plugins.Core.Interfaces;
    using IContainer = Framework.Interfaces.Injection.IContainer;

    public class MtmMainModel : INotifyPropertyChanged, IMtmMainModel
    {
        private bool isBusy;

        private IPluginControl _currentPluginControl;
        public IPluginControl CurrentPluginControl
        {
            get => _currentPluginControl;
            set
            {
                if (Equals(value, _currentPluginControl)) return;
                _currentPluginControl = value;
                OnPropertyChanged();
            }
        }

        public MtmMainModel(IContainer container)
        {
            this.Container = container;
        }

        public bool IsBusy
        {
            get => this.isBusy;
            set
            {
                if (this.isBusy != value)
                {
                    this.isBusy = value;
                    this.OnPropertyChanged(nameof(this.IsBusy));
                }

                ;
            }
        }

        public IContainer Container { get;  }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}