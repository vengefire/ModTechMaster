using ModTechMaster.Core.Enums.Mods;
using ModTechMaster.Core.Interfaces.Models;

namespace ModTechMaster.Data.Models
{
    public class Relationship : IRelationship
    {
        public Relationship(string dependentKey, string dependencyKey, bool hasMultipleDependencies)
        {
            DependentKey = dependentKey;
            DependencyKey = dependencyKey;
            HasMultipleDependencies = hasMultipleDependencies;
        }

        public string DependentKey { get; }
        public string DependencyKey { get; }
        public bool HasMultipleDependencies { get; }
    }

    public class ObjectRelationship : Relationship, IObjectRelationship
    {
        public ObjectRelationship(ObjectType dependentType, ObjectType dependencyType,
            string dependentKey, string dependencyKey, bool hasMultipleDependencies = false) : base(dependentKey, dependencyKey, hasMultipleDependencies)
        {
            DependentType = dependentType;
            DependencyType = dependencyType;
        }

        public ObjectType DependentType { get; }
        public ObjectType DependencyType { get; }
    }
}