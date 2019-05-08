using ModTechMaster.Core.Enums.Mods;

namespace ModTechMaster.Core.Interfaces.Models
{
    public interface IObjectRelationship
    {
        ManifestEntryType DependentType { get; }
        ManifestEntryType DependencyType { get; }
        string DependentKey { get; }
        string DependencyKey { get; }
        bool HasMultipleDependencies { get; }
    }
}