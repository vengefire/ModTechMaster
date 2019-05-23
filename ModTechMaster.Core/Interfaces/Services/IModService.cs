namespace ModTechMaster.Core.Interfaces.Services
{
    using System.ComponentModel;

    using ModTechMaster.Core.Interfaces.Models;

    public interface IModService : INotifyPropertyChanged
    {
        IModCollection ModCollection { get; }

        IModCollection LoadCollectionFromPath(string path, string name);

        IMod TryLoadFromPath(string path);
    }
}