namespace ModTechMaster.Data.Models
{
    using ModTechMaster.Core.Enums.Mods;
    using ModTechMaster.Core.Interfaces.Models;

    public abstract class Relationship : IRelationship
    {
        public Relationship(string dependentKey, string dependencyKey, bool hasMultipleDependencies)
        {
            this.DependentKey = dependentKey;
            this.DependencyKey = dependencyKey;
            this.HasMultipleDependencies = hasMultipleDependencies;
        }

        public string DependencyKey { get; }

        public ObjectType DependencyType { get; set; }

        public string DependentKey { get; }

        public ObjectType DependentType { get; set; }

        public bool HasMultipleDependencies { get; }
    }
}