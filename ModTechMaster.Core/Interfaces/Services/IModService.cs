namespace ModTechMaster.Core.Interfaces.Services
{
    using Models;

    public interface IModService
    {
        IMod TryLoadFromPath(string path);
        IModCollection LoadCollectionFromPath(string path, string name);
        IModCollection ModCollection { get; }
    }
}