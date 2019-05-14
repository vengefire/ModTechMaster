using System.ComponentModel;
using System.Runtime.CompilerServices;
using ModTechMaster.Core.Interfaces.Models;
using ModTechMaster.Core.Interfaces.Services;
using ModTechMaster.UI.Plugins.Home.Annotations;

namespace ModTechMaster.UI.Plugins.Home.Models
{
    public class HomeModel : INotifyPropertyChanged
    {
        private readonly IModService _modService;
        private string _modDirectory;
        private string _modCollectionName;
        private IModCollection _modCollection;

        public string ModCollectionName
        {
            get => _modCollectionName;
            set {
                if (_modCollectionName != value)
                {
                    _modCollectionName = value;
                    OnPropertyChanged(nameof(ModCollectionName));
                }
            }  
        }

        public HomeModel(IModService modService)
        {
            _modService = modService;
        }

        public string ModDirectory
        {
            get => _modDirectory;
            set
            {
                if (_modDirectory != value)
                {
                    _modDirectory = value;
                    OnPropertyChanged(nameof(ModDirectory));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public IModCollection LoadMods()
        {
            this._modCollection = _modService.LoadCollectionFromPath(ModDirectory, ModCollectionName);
            return _modCollection;
        }
    }
}