namespace ModTechMaster.Logic.Processors
{
    using ModTechMaster.Core.Enums.Mods;
    using ModTechMaster.Core.Interfaces.Models;
    using ModTechMaster.Core.Interfaces.Processors;
    using ModTechMaster.Data.Models.Mods;
    using ModTechMaster.Logic.Factories;

    using Newtonsoft.Json.Linq;

    public class PrefabManifestEntryProcessor : IManifestEntryProcessor
    {
        public IManifestEntry ProcessManifestEntry(
            IManifest manifest,
            ObjectType entryType,
            string path,
            dynamic jsonObject)
        {
            var manifestEntry = new ManifestEntry(manifest, entryType, path, jsonObject);

            var objectDefinition = ObjectDefinitionFactory.ObjectDefinitionFactorySingleton.Get(
                entryType,
                new ObjectDefinitionDescription(path, null, null, null, (JObject)jsonObject),
                (JObject)jsonObject,
                path);
            manifestEntry.Objects.Add(objectDefinition);
            return manifestEntry;
        }
    }
}