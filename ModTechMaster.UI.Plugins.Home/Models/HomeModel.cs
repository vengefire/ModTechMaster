namespace ModTechMaster.UI.Plugins.Home.Models
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    using ModTechMaster.Core.Interfaces.Models;
    using ModTechMaster.Core.Interfaces.Services;
    using ModTechMaster.UI.Plugins.Core.Interfaces;
    using ModTechMaster.UI.Plugins.Home.Annotations;

    public class HomeModel : INotifyPropertyChanged
    {
        private readonly IModService modService;

        private readonly IMtmMainModel mainModel;

        public HomeModel(IModService modService, IMtmMainModel mainModel)
        {
            this.modService = modService;
            this.mainModel = mainModel;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public HomeSettings HomeSettings { get; set; }

        public IModCollection ModCollection { get; private set; }

        public IModCollection LoadMods()
        {
            this.mainModel.IsBusy = true;
            this.ModCollection = this.modService.LoadCollectionFromPath(
                this.HomeSettings.ModDirectory,
                this.HomeSettings.ModCollectionName);
            this.OnPropertyChanged(nameof(this.ModCollection));
            this.mainModel.IsBusy = false;
            return this.ModCollection;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}