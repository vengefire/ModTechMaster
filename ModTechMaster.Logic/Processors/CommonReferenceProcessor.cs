namespace ModTechMaster.Logic.Processors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;

    using Castle.Core.Internal;

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
            List<IReferenceableObject> baseReferences)
            where TType : class, IReferenceableObject
        {
            if (objectDefinition == null)
            {
                return new List<IObjectReference<TType>>();
            }

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
                            candidates.Where(o => o.ObjectType == relationship.DependentType),
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

                                    object objectKeyObject;
                                    if (!objectDefinition.MetaData.TryGetValue(
                                            relationship.DependencyKey,
                                            out objectKeyObject))
                                    {
                                        return;
                                    }

                                    var objectKey = objectKeyObject.ToString();
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
                        candidates.Where(o => o.ObjectType == relationship.DependencyType).ToList().ForEach(
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

                                    if (objectKeys == null || (relationship.HasMultipleDependencies && ((List<string>)objectKeys).Count == 0))
                                    {
                                        return;
                                    }

                                    if (relationship.HasMultipleDependencies && ((List<string>)objectKeys).All(s => s.IsNullOrEmpty()))
                                    {
                                        return;
                                    }

                                    object dependencyKeyObject; //= candidate.MetaData[relationship.DependencyKey].ToString();
                                    if (!candidate.MetaData.TryGetValue(relationship.DependencyKey, out dependencyKeyObject))
                                    {
                                        return;
                                    }

                                    var dependencyKey = dependencyKeyObject.ToString();

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

                        var ignoreList = new List<Tuple<ObjectType, ObjectType>>(new []
                                                                                     {
                                                                                         new Tuple<ObjectType, ObjectType>(ObjectType.ShopDef, ObjectType.WeaponDef),           // HBS Shopdefs contain refs to non-existent weapons
                                                                                         new Tuple<ObjectType, ObjectType>(ObjectType.HardpointDataDef, ObjectType.Prefab),     // Ball ache to identify prefabs, and unnecessary.
                                                                                         new Tuple<ObjectType, ObjectType>(ObjectType.TurretChassisDef, ObjectType.Prefab),     // Ball ache to identify prefabs, and unnecessary.
                                                                                         new Tuple<ObjectType, ObjectType>(ObjectType.ShopDef, ObjectType.MechDef),             // Stupid Templates.
                                                                                     });

                        var tempIgnoreList = new List<ObjectType>();
                        tempIgnoreList.AddRange(new []{ ObjectType.Prefab, ObjectType.AssetBundle, ObjectType.HardpointDataDef, ObjectType.MovementCapabilitiesDef });

                        if (tempIgnoreList.Contains(relationship.DependencyType))
                        {
                            return;
                        }

                        if (baseReferences != null && 
                            baseReferences.Contains(objectDefinition))
                        {
                            if (ignoreList.Any(
                                ignoreEntry =>
                                    objectDefinition.ObjectType == ignoreEntry.Item1
                                    && relationship.DependencyType == ignoreEntry.Item2))
                            {
                                return;
                            }
                        }

                        // Add an invalid entry for each specified dependency that was not matched...
                        List<string> keys = !objectDefinition.MetaData.ContainsKey(relationship.DependentKey) || objectDefinition.MetaData[relationship.DependentKey] == null
                                                ?
                                                new List<string>()
                                                : !relationship.HasMultipleDependencies
                                                    ? new List<string>(new string[] {objectDefinition.MetaData[relationship.DependentKey].ToString()})
                                                    : new List<string>((List<string>)objectDefinition.MetaData[relationship.DependentKey]);

                        keys.Where(s => !s.IsNullOrEmpty()).ToList().ForEach(
                            s =>
                                {
                                    if (!dependencyRecs.Any(reference => reference.Relationship == relationship && string.Equals(reference.ReferenceKey.ToString(), s, StringComparison.InvariantCultureIgnoreCase)))
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