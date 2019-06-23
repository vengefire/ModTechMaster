namespace ModTechMaster.Core.Interfaces.Services
{
    using System.ComponentModel;

    using ModTechMaster.Core.Interfaces.Models;

    public interface IModService : INotifyPropertyChanged
    {
        IModCollection ModCollection { get; }

        IModCollection LoadCollectionFromPath(string battleTechPath, string modsPath, string name);

        IMod TryLoadFromPath(string path, bool isBattleTechData);
    }
}