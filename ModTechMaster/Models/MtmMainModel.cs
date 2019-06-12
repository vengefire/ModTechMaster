namespace ModTechMaster.UI.Models
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    using ModTechMaster.Annotations;
    using ModTechMaster.Core.Interfaces.Services;
    using ModTechMaster.UI.Plugins.Core.Interfaces;

    using IContainer = Framework.Interfaces.Injection.IContainer;

    public class MtmMainModel : INotifyPropertyChanged, IMtmMainModel
    {
        private IPluginControl currentPluginControl;

        private bool isBusy;

        public MtmMainModel(IContainer container, IMessageService messageService)
        {
            this.Container = container;
            this.MessageService = messageService;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public IContainer Container { get; }

        public IMessageService MessageService { get; }

        public IPluginControl CurrentPluginControl
        {
            get => this.currentPluginControl;
            set
            {
                if (value == this.currentPluginControl)
                {
                    return;
                }

                this.currentPluginControl = value;
                this.OnPropertyChanged();
            }
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
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}