namespace ModTechMaster.Core.Interfaces.Models
{
    using ModTechMaster.Core.Enums.Mods;

    public interface IRelationship
    {
        string DependencyKey { get; }

        ObjectType DependencyType { get; set; }

        string DependentKey { get; }

        ObjectType DependentType { get; set; }

        bool HasMultipleDependencies { get; }
    }
}