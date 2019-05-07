namespace ModTechMaster.Core.Interfaces.Services
{
    using Models;

    public interface IModService
    {
        IMod TryLoadFromPath(string path);
    }
}