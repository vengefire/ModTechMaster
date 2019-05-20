namespace ModTechMaster.Logic.Processors
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
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
            if (objectDefinition == null)
            {
                return new List<IObjectReference<TType>>();
            }

            // dependantTypesToIgnore = dependantTypesToIgnore ?? new List<ObjectType>();

            var dependenciesRels = RelationshipManager.GetDependenciesRelationshipsForType(objectDefinition.ObjectType);
            var dependantRels = RelationshipManager.GetDependantRelationShipsForType(objectDefinition.ObjectType);

            if (!dependantRels.Any() &&
                !dependenciesRels.Any())
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

            // dependantRels = dependantRels.Where(ship => !dependantTypesToIgnore.Contains(ship.DependentType)).ToList();


            var candidates = objectProvider.GetReferenceableObjects();
            candidates = candidates.Where(o => targetObjectTypes.Contains(o.ObjectType)).ToList();
            var dependentRecs = new List<ObjectReference<TType>>();
            Parallel.ForEach(
                dependantRels, relationship =>
                {
                    Parallel.ForEach(
                        candidates, candidate =>
                        {
                            if (candidate.ObjectType != relationship.DependentType)
                            {
                                return;
                            }

                            if (!candidate.MetaData.ContainsKey(relationship.DependentKey) ||
                                !objectDefinition.MetaData.ContainsKey(relationship.DependencyKey))
                            {
                                return;
                            }

                            var objectKey = objectDefinition.MetaData[relationship.DependencyKey];

                            var dependentKeys = candidate.MetaData[relationship.DependentKey];
                            if ((relationship.HasMultipleDependencies && ((List<string>)dependentKeys).Contains(objectKey)) ||
                                dependentKeys.ToString() == objectKey.ToString())
                            {
                                lock (dependentRecs)
                                {
                                    dependentRecs.Add(new ObjectReference<TType>((TType)candidate, ObjectReferenceType.Dependent, relationship, false));
                                }
                            }
                        });
                });

            var dependencyRecs = new List<ObjectReference<TType>>();
            Parallel.ForEach(
                dependenciesRels, relationship =>
                {
                    Parallel.ForEach(
                        candidates, candidate =>
                        {
                            if (candidate.ObjectType != relationship.DependencyType)
                            {
                                return;
                            }

                            if (!candidate.MetaData.ContainsKey(relationship.DependencyKey) ||
                                !objectDefinition.MetaData.ContainsKey(relationship.DependentKey))
                            {
                                return;
                            }

                            var objectKeys = objectDefinition.MetaData[relationship.DependentKey];

                            var dependencyKey = candidate.MetaData[relationship.DependencyKey];
                            if ((relationship.HasMultipleDependencies && ((List<string>)objectKeys).Contains(dependencyKey)) ||
                                string.CompareOrdinal(objectKeys.ToString(), dependencyKey.ToString()) == 0)
                            {
                                lock (dependencyRecs)
                                {
                                    dependencyRecs.Add(new ObjectReference<TType>((TType)candidate, ObjectReferenceType.Dependency, relationship, false));
                                }
                            }
                        });
                });

            var references = dependentRecs;
            references.AddRange(dependencyRecs);
            return references.Cast<IObjectReference<TType>>().ToList();
        }
    }
}