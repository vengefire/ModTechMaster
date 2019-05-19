namespace ModTechMaster.UI.Plugins.Home.Models
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using Annotations;
    using Core.Interfaces;
    using ModTechMaster.Core.Interfaces.Models;
    using ModTechMaster.Core.Interfaces.Services;

    public class HomeModel : INotifyPropertyChanged
    {
        private readonly IModService _modService;
        private readonly IMtmMainModel mainModel;
        private readonly ISettingsService settingsService;
        private bool _isBusy;

        public HomeModel(IModService modService, ISettingsService settingsService, IMtmMainModel mainModel)
        {
            this._modService = modService;
            this.settingsService = settingsService;
            this.mainModel = mainModel;
            this.HomeSettings = settingsService.ReadSettings<HomeSettings>("HomeSettings");
        }

        public IModCollection ModCollection { get; private set; }

        public HomeSettings HomeSettings { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public IModCollection LoadMods()
        {
            this.mainModel.IsBusy = true;
            this.ModCollection = this._modService.LoadCollectionFromPath(this.HomeSettings.ModDirectory, this.HomeSettings.ModCollectionName);
            this.OnPropertyChanged(nameof(HomeModel.ModCollection));
            this.mainModel.IsBusy = false;
            return this.ModCollection;
        }
    }
}