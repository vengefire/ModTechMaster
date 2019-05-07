namespace ModTechMaster.Logic.Processors
{
    using Core.Enums.Mods;
    using Core.Interfaces.Models;
    using Core.Interfaces.Processors;
    using Data.Models.Mods;
    using Factories;
    using Newtonsoft.Json.Linq;

    public class PrefabManifestEntryProcessor : IManifestEntryProcessor
    {
        public IManifestEntry ProcessManifestEntry(IManifest manifest, ManifestEntryType entryType, string path, dynamic jsonObject)
        {
            var manifestEntry =
                new ManifestEntry(manifest, entryType, path, jsonObject);

            var objectDefinition = ObjectDefinitionFactory.ObjectDefinitionFactorySingleton.Get(entryType, new ObjectDefinitionDescription(path, null, null, null, (JObject)jsonObject), (JObject)jsonObject, path);
            manifestEntry.Objects.Add(objectDefinition);
            return manifestEntry;
        }
    }
}