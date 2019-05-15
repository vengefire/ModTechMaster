namespace ModTechMaster.UI.Plugins.Home.Models
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using Annotations;
    using ModTechMaster.Core.Interfaces.Models;
    using ModTechMaster.Core.Interfaces.Services;

    public class HomeModel : INotifyPropertyChanged
    {
        private readonly IModService _modService;
        private readonly ISettingsService settingsService;

        public HomeModel(IModService modService, ISettingsService settingsService)
        {
            this._modService = modService;
            this.settingsService = settingsService;
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
            this.ModCollection = this._modService.LoadCollectionFromPath(this.HomeSettings.ModDirectory, this.HomeSettings.ModCollectionName);
            return this.ModCollection;
        }
    }
}