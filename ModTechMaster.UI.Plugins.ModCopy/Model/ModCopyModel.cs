namespace ModTechMaster.UI.Plugins.ModCopy.Model
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using Annotations;
    using ModTechMaster.Core.Interfaces.Services;
    using Nodes;

    public class ModCopyModel : INotifyPropertyChanged
    {
        private static ISettingsService settingsService;

        public static ModCopySettings settings;
        private IMtmTreeViewItem currentSelectedItem;

        public ModCopyModel(ISettingsService settingsService)
        {
            ModCopyModel.settingsService = settingsService;
        }

        public IMtmTreeViewItem CurrentSelectedItem
        {
            get => this.currentSelectedItem;
            set
            {
                if (object.Equals(value, this.currentSelectedItem))
                {
                    return;
                }

                this.currentSelectedItem = value;
                this.OnPropertyChanged();
            }
        }

        public static ModCopySettings Settings => ModCopyModel.settings ?? (ModCopyModel.settings = ModCopyModel.settingsService.ReadSettings<ModCopySettings>(nameof(ModCopyModel)));
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            this.CurrentSelectedItem = e.NewValue as IMtmTreeViewItem;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}