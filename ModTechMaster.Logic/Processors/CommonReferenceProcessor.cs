using System.Collections.Generic;
using ModTechMaster.Core.Enums.Mods;
using ModTechMaster.Core.Interfaces.Models;
using ModTechMaster.Data.Models.Mods;
using ModTechMaster.Logic.Managers;

namespace ModTechMaster.Logic.Processors
{
    public class CommonReferenceProcessor
    {
        public static List<ObjectDefinition> FindReferences(IModCollection modCollection, ManifestEntryType objectType,
            IObjectDefinition objectDefinition, List<ManifestEntryType> dependantTypesToIgnore)
        {
            var dependenciesRels = RelationshipManager.GetDependenciesRelationshipsForType(objectDefinition.ManifestEntry.Type);
            var dependantRels = ObjRels.GetDependantRelationShipsForType(objectDefinition.ManifestEntry.Type);

            var targetEntryTypes = new HashSet<ManifestEntryType>();
            dependenciesRels.Select(ship => ship.typeDependedUpon).Distinct().ToList()
                .ForEach(type => targetEntryTypes.Add(type));
            dependantRels.Select(ship => ship.dependingType).Distinct().ToList()
                .ForEach(type => targetEntryTypes.Add(type));
            dependantRels = dependantRels.Where(ship => !dependantTypesToIgnore.Contains(ship.dependingType)).ToList();

            var dependantRecs = battleTechModCollection.Mods.SelectMany(mod =>
                mod.Manifest.Entries.Where(entry => targetEntryTypes.Contains(entry.Type)).SelectMany(entry =>
                    entry.Objects.Where(definition =>
                        {
                            return dependantRels.Any(ship =>
                                definition.ManifestEntry.Type == ship.dependingType &&
                                definition.MetaData.ContainsKey(ship.dependantKey) &&
                                objectDefinition.MetaData.ContainsKey(ship.targetKey) &&
                                (
                                    !ship.dependantKeyInList && definition.MetaData[ship.dependantKey].ToString() ==
                                    objectDefinition.MetaData[ship.targetKey].ToString() ||
                                    ship.dependantKeyInList &&
                                    ((List<string>) definition.MetaData[ship.dependantKey]).Exists(v =>
                                        v == objectDefinition.MetaData[ship.targetKey].ToString())
                                )
                            );
                        })
                        .Select(definition => definition))).ToList();

            var dependencyRecs = battleTechModCollection.Mods.SelectMany(mod =>
                mod.Manifest.Entries.Where(entry => targetEntryTypes.Contains(entry.Type)).SelectMany(entry =>
                    entry.Objects.Where(definition =>
                        {
                            return dependenciesRels.Any(ship =>
                                definition.ManifestEntry.Type == ship.typeDependedUpon &&
                                definition.MetaData.ContainsKey(ship.targetKey) &&
                                objectDefinition.MetaData.ContainsKey(ship.dependantKey) &&
                                (
                                    !ship.dependantKeyInList && definition.MetaData[ship.targetKey].ToString() ==
                                    objectDefinition.MetaData[ship.dependantKey].ToString() ||
                                    ship.dependantKeyInList &&
                                    ((List<string>) objectDefinition.MetaData[ship.dependantKey]).Exists(v =>
                                        v == definition.MetaData[ship.targetKey].ToString())
                                )
                            );
                        })
                        .Select(definition => definition))).ToList();
            var references = dependantRecs;
            references.AddRange(dependencyRecs.Except(dependantRecs));
            return references;
        }
    }
}