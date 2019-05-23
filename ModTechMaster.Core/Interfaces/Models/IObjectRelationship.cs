namespace ModTechMaster.Core.Interfaces.Models
{
    using ModTechMaster.Core.Enums.Mods;

    public interface IObjectRelationship : IRelationship
    {
        ObjectType DependencyType { get; }

        ObjectType DependentType { get; }
    }
}