namespace ModTechMaster.Data.Models
{
    using ModTechMaster.Core.Interfaces.Models;

    public class Relationship : IRelationship
    {
        public Relationship(string dependentKey, string dependencyKey, bool hasMultipleDependencies)
        {
            this.DependentKey = dependentKey;
            this.DependencyKey = dependencyKey;
            this.HasMultipleDependencies = hasMultipleDependencies;
        }

        public string DependencyKey { get; }

        public string DependentKey { get; }

        public bool HasMultipleDependencies { get; }
    }
}