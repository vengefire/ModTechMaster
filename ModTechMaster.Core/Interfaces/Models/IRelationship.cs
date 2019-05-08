namespace ModTechMaster.Core.Interfaces.Models
{
    public interface IRelationship
    {
        string DependentKey { get; }
        string DependencyKey { get; }
        bool HasMultipleDependencies { get; }
    }
}