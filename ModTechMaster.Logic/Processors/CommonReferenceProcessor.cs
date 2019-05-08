using System.Collections.Generic;
using System.Linq;
using ModTechMaster.Core.Enums;
using ModTechMaster.Core.Enums.Mods;
using ModTechMaster.Core.Interfaces.Models;
using ModTechMaster.Data.Models;
using ModTechMaster.Logic.Managers;

namespace ModTechMaster.Logic.Processors
{
    public class CommonReferenceProcessor
    {
        public static List<IObjectReference<TType>> FindReferences<TType>(IReferenceableObjectProvider objectProvider,
            ObjectType objectType,
            IReferenceableObject objectDefinition, List<ObjectType> dependantTypesToIgnore)
            where TType : IReferenceableObject
        {
            var dependenciesRels = RelationshipManager.GetDependenciesRelationshipsForType(objectType);
            var dependantRels = RelationshipManager.GetDependantRelationShipsForType(objectType);

            var targetEntryTypes = new HashSet<ObjectType>();
            dependenciesRels.Select(ship => ship.DependencyType).Distinct().ToList()
                .ForEach(type => targetEntryTypes.Add(type));
            dependantRels.Select(ship => ship.DependentType).Distinct().ToList()
                .ForEach(type => targetEntryTypes.Add(type));
            dependantRels = dependantRels.Where(ship => !dependantTypesToIgnore.Contains(ship.DependentType)).ToList();

            var dependantRecs = objectProvider.GetReferenceableObjects()
                .Where(o => targetEntryTypes.Contains(o.ObjectType)).Select(definition =>
                {
                    var relationship = dependantRels.First(ship =>
                        definition.ObjectType == ship.DependentType &&
                        definition.MetaData.ContainsKey(ship.DependentKey) &&
                        objectDefinition.MetaData.ContainsKey(ship.DependencyKey) &&
                        (
                            !ship.HasMultipleDependencies &&
                            definition.MetaData[ship.DependentKey].ToString() ==
                            objectDefinition.MetaData[ship.DependencyKey].ToString() ||
                            ship.HasMultipleDependencies &&
                            ((List<string>) definition.MetaData[ship.DependentKey]).Exists(v =>
                                v == objectDefinition.MetaData[ship.DependencyKey].ToString())
                        )
                    );

                    return new ObjectReference<TType>((TType) definition, ObjectReferenceType.Dependent,
                        relationship, false);
                });

            var dependencyRecs = objectProvider.GetReferenceableObjects()
                .Where(o => targetEntryTypes.Contains(o.ObjectType)).Select(definition =>
                {
                    var relationship = dependenciesRels.First(ship =>
                        definition.ObjectType == ship.DependencyType &&
                        definition.MetaData.ContainsKey(ship.DependencyKey) &&
                        objectDefinition.MetaData.ContainsKey(ship.DependentKey) &&
                        (
                            !ship.HasMultipleDependencies &&
                            definition.MetaData[ship.DependencyKey].ToString() ==
                            objectDefinition.MetaData[ship.DependentKey].ToString() ||
                            ship.HasMultipleDependencies &&
                            ((List<string>) objectDefinition.MetaData[ship.DependentKey]).Exists(v =>
                                v == definition.MetaData[ship.DependencyKey].ToString())
                        )
                    );
                    return new ObjectReference<TType>((TType) definition, ObjectReferenceType.Dependent,
                        relationship, false);
                });
            var references = dependantRecs.ToList();
            references.AddRange(dependencyRecs.Except(dependantRecs));
            return references.Cast<IObjectReference<TType>>().ToList();
        }
    }
}