namespace ModTechMaster.Logic.Processors
{
    using System.Collections.Generic;
    using System.Linq;
    using Core.Enums;
    using Core.Enums.Mods;
    using Core.Interfaces.Models;
    using Data.Models;
    using Managers;

    public class CommonReferenceProcessor
    {
        public static List<IObjectReference<TType>> FindReferences<TType>(
            IReferenceableObjectProvider objectProvider,
            IReferenceableObject objectDefinition, List<ObjectType> dependantTypesToIgnore)
            where TType : IReferenceableObject
        {
            dependantTypesToIgnore = dependantTypesToIgnore ?? new List<ObjectType>();

            var dependenciesRels = RelationshipManager.GetDependenciesRelationshipsForType(objectDefinition.ObjectType);
            var dependantRels = RelationshipManager.GetDependantRelationShipsForType(objectDefinition.ObjectType);

            if (!dependantRels.Any() && !dependenciesRels.Any())
            {
                return new List<IObjectReference<TType>>();
            }

            var targetObjectTypes = new HashSet<ObjectType>();
            dependenciesRels
                .Select(ship => ship.DependencyType).Distinct().ToList()
                .ForEach(type => targetObjectTypes.Add(type));
            dependantRels
                .Select(ship => ship.DependentType).Distinct().ToList()
                .ForEach(type => targetObjectTypes.Add(type));

            dependantRels = dependantRels.Where(ship => !dependantTypesToIgnore.Contains(ship.DependentType)).ToList();


            var candidates = objectProvider.GetReferenceableObjects();
            candidates = candidates.Where(o => targetObjectTypes.Contains(o.ObjectType)).ToList();
            var dependentRecs = new List<ObjectReference<TType>>();
            foreach (var relationship in dependantRels)
            {
                foreach (var candidate in candidates)
                {
                    if (candidate.ObjectType != relationship.DependentType)
                    {
                        continue;
                    }

                    if (!candidate.MetaData.ContainsKey(relationship.DependentKey) ||
                        !objectDefinition.MetaData.ContainsKey(relationship.DependencyKey))
                    {
                        continue;
                    }

                    var objectKey = objectDefinition.MetaData[relationship.DependencyKey];

                    var dependentKeys = candidate.MetaData[relationship.DependentKey];
                    if ((relationship.HasMultipleDependencies && ((List<string>)dependentKeys).Contains(objectKey)) || 
                        dependentKeys == objectKey)
                    {
                        dependentRecs.Add(new ObjectReference<TType>((TType)candidate, ObjectReferenceType.Dependent, relationship, false));
                    }
                }
            }

            var dependencyRecs = new List<ObjectReference<TType>>();
            foreach (var relationship in dependenciesRels)
            {
                foreach (var candidate in candidates)
                {
                    if (candidate.ObjectType != relationship.DependencyType)
                    {
                        continue;
                    }

                    if (!candidate.MetaData.ContainsKey(relationship.DependencyKey) ||
                        !objectDefinition.MetaData.ContainsKey(relationship.DependentKey))
                    {
                        continue;
                    }

                    var objectKeys = objectDefinition.MetaData[relationship.DependentKey];

                    var dependencyKey = candidate.MetaData[relationship.DependencyKey];
                    if ((relationship.HasMultipleDependencies && ((List<string>)objectKeys).Contains(dependencyKey)) || 
                        objectKeys == dependencyKey)
                    {
                        dependencyRecs.Add(new ObjectReference<TType>((TType)candidate, ObjectReferenceType.Dependent, relationship, false));
                    }
                }
            }

            var references = dependentRecs;
            references.AddRange(dependencyRecs.Except(dependentRecs));
            return references.Cast<IObjectReference<TType>>().ToList();
        }
    }
}