namespace ModTechMaster.Data.Models
{
    using ModTechMaster.Core.Enums.Mods;
    using ModTechMaster.Core.Interfaces.Models;

    public class ObjectRelationship : Relationship, IObjectRelationship
    {
        public ObjectRelationship(
            ObjectType dependentType,
            ObjectType dependencyType,
            string dependentKey,
            string dependencyKey,
            bool hasMultipleDependencies = false)
            : base(dependentKey, dependencyKey, hasMultipleDependencies)
        {
            this.DependentType = dependentType;
            this.DependencyType = dependencyType;
        }

        public ObjectType DependencyType { get; }

        public ObjectType DependentType { get; }
    }
}