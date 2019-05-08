using ModTechMaster.Core.Enums.Mods;

namespace ModTechMaster.Core.Interfaces.Models
{
    public interface IObject
    {
        ObjectType ObjectType { get; }
    }
}