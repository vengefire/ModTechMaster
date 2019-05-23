namespace ModTechMaster.Core.Interfaces.Models
{
    public interface IRelationship
    {
        string DependencyKey { get; }

        string DependentKey { get; }

        bool HasMultipleDependencies { get; }
    }
}