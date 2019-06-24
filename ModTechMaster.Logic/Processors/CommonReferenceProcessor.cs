namespace ModTechMaster.Logic.Processors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;

    using ModTechMaster.Core.Enums;
    using ModTechMaster.Core.Enums.Mods;
    using ModTechMaster.Core.Interfaces.Models;
    using ModTechMaster.Data.Models;
    using ModTechMaster.Logic.Managers;

    public class CommonReferenceProcessor
    {
        public static List<IObjectReference<TType>> FindReferences<TType>(
            IReferenceableObjectProvider objectProvider,
            IReferenceableObject objectDefinition,
            List<ObjectType> dependantTypesToIgnore)
            where TType : class, IReferenceableObject
        {
            if (objectDefinition == null)
            {
                return new List<IObjectReference<TType>>();
            }

            // dependantTypesToIgnore = dependantTypesToIgnore ?? new List<ObjectType>();
            var dependenciesRels = RelationshipManager.GetDependenciesRelationshipsForType(objectDefinition.ObjectType);
            var dependantRels = RelationshipManager.GetDependentRelationShipsForType(objectDefinition.ObjectType);

            if (!dependantRels.Any() && !dependenciesRels.Any())
            {
                return new List<IObjectReference<TType>>();
            }

            var targetObjectTypes = new HashSet<ObjectType>();
            dependantRels.Select(ship => ship.DependentType).Distinct().ToList()
                .ForEach(type => targetObjectTypes.Add(type));

            var candidates = objectProvider.GetReferenceableObjects().Where(o => targetObjectTypes.Contains(o.ObjectType)).ToList();
            var dependentRecs = new List<ObjectReference<TType>>();
            Parallel.ForEach(
                dependantRels,
                relationship =>
                    {
                        Parallel.ForEach(
                            candidates,
                            candidate =>
                                {
                                    if (candidate.ObjectType != relationship.DependentType)
                                    {
                                        return;
                                    }

                                    if (!candidate.MetaData.ContainsKey(relationship.DependentKey)
                                        || !objectDefinition.MetaData.ContainsKey(relationship.DependencyKey))
                                    {
                                        return;
                                    }

                                    var objectKey = objectDefinition.MetaData[relationship.DependencyKey].ToString();
                                    var dependentKeys = candidate.MetaData[relationship.DependentKey];

                                    if (dependentKeys == null)
                                    {
                                        return;
                                    }

                                    if ((relationship.HasMultipleDependencies
                                        && ((List<string>)dependentKeys).Any(s => string.Equals(s, objectKey, StringComparison.OrdinalIgnoreCase)))
                                        || string.Compare(dependentKeys.ToString(), objectKey, StringComparison.InvariantCultureIgnoreCase) == 0)
                                    {
                                        lock (dependentRecs)
                                        {
                                            dependentRecs.Add(
                                                new ObjectReference<TType>(
                                                    (TType)candidate,
                                                    ObjectReferenceType.Dependent,
                                                    relationship,
                                                    false,
                                                    true,
                                                    objectKey));
                                        }
                                    }
                                });
                    });

            targetObjectTypes.Clear();
            dependenciesRels.Select(ship => ship.DependencyType).Distinct().ToList()
                .ForEach(type => targetObjectTypes.Add(type));
            candidates = objectProvider.GetReferenceableObjects().Where(o => targetObjectTypes.Contains(o.ObjectType)).ToList();

            var dependencyRecs = new List<ObjectReference<TType>>();

            dependenciesRels.ToList().ForEach(
            // dependenciesRels.AsParallel().ForAll(
                relationship =>
                    {

                        candidates.ToList().ForEach(
                        // candidates.AsParallel().ForAll(
                            candidate =>
                                {
                                    if (candidate.ObjectType != relationship.DependencyType)
                                    {
                                        return;
                                    }

                                    if (!candidate.MetaData.ContainsKey(relationship.DependencyKey)
                                        || !objectDefinition.MetaData.ContainsKey(relationship.DependentKey))
                                    {
                                        return;
                                    }

                                    var objectKeys = objectDefinition.MetaData[relationship.DependentKey];

                                    if (objectKeys == null)
                                    {
                                        return;
                                    }

                                    var dependencyKey = candidate.MetaData[relationship.DependencyKey].ToString();
                                    if ((relationship.HasMultipleDependencies && ((List<string>)objectKeys).Any(s => s.Equals(dependencyKey, StringComparison.OrdinalIgnoreCase))) 
                                        || string.Equals(dependencyKey, objectKeys.ToString(), StringComparison.OrdinalIgnoreCase))
                                    {
                                        lock (dependencyRecs)
                                        {
                                            dependencyRecs.Add(
                                                new ObjectReference<TType>(
                                                    (TType)candidate,
                                                    ObjectReferenceType.Dependency,
                                                    relationship,
                                                    false,
                                                    true,
                                                    dependencyKey));
                                        }
                                    }
                                });

                        // Add an invalid entry for each specified dependency that was not matched...
                        List<string> keys = !objectDefinition.MetaData.ContainsKey(relationship.DependentKey) || objectDefinition.MetaData[relationship.DependentKey] == null
                                                ?
                                                new List<string>()
                                                : !relationship.HasMultipleDependencies
                                                    ? new List<string>(new string[] {objectDefinition.MetaData[relationship.DependentKey].ToString()})
                                                    : new List<string>((List<string>)objectDefinition.MetaData[relationship.DependentKey]);

                        keys.ForEach(
                            s =>
                                {
                                    if (!dependencyRecs.Any(reference => reference.Relationship == relationship && reference.ReferenceKey == s))
                                    {
                                        lock (dependencyRecs)
                                        {
                                            dependencyRecs.Add(
                                                new ObjectReference<TType>(
                                                    null,
                                                    ObjectReferenceType.Dependency,
                                                    relationship,
                                                    false,
                                                    false,
                                                    s));
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