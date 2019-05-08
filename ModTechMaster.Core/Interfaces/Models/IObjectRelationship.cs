using ModTechMaster.Core.Enums.Mods;

namespace ModTechMaster.Core.Interfaces.Models
{
    public interface IObjectRelationship : IRelationship
    {
        ObjectType DependentType { get; }
        ObjectType DependencyType { get; }
    }
}