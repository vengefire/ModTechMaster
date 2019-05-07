using ModTechMaster.Core.Interfaces.Models;

namespace ModTechMaster.Core.Interfaces.Services
{
    public interface IModService
    {
        IMod TryLoadFromPath(string path);
    }
}