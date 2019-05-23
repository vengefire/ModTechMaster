namespace ModTechMaster.UI.Plugins.ModCopy.Modals.MechSelector
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    using ModTechMaster.UI.Plugins.ModCopy.Annotations;

    public class MechSelectorModel : INotifyPropertyChanged
    {
        private string mechFilePath;

        private List<MechModel> mechModels = new List<MechModel>();

        public event PropertyChangedEventHandler PropertyChanged;

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

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}