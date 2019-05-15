using System.ComponentModel;

namespace ModTechMaster.Core.Interfaces.Services
{
    using Models;

    public interface IModService : INotifyPropertyChanged
    {
        IMod TryLoadFromPath(string path);
        IModCollection LoadCollectionFromPath(string path, string name);
        IModCollection ModCollection { get; }
    }
}