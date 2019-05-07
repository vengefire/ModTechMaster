namespace ModTechMaster.Logic.Processors
{
    using System.IO;
    using Core.Enums.Mods;
    using Core.Interfaces.Models;
    using Core.Interfaces.Processors;
    using Data.Models.Mods;
    using Factories;
    using Newtonsoft.Json.Linq;

    public class AssetBundleManifestEntryProcessor : IManifestEntryProcessor
    {
        public IManifestEntry ProcessManifestEntry(IManifest manifest, ManifestEntryType entryType, string path, dynamic jsonObject)
        {
            var manifestEntry = new ManifestEntry(manifest, entryType, path, jsonObject);
            var fi = new FileInfo(path);

            var objectDefinition = ObjectDefinitionFactory.ObjectDefinitionFactorySingleton.Get(entryType, new ObjectDefinitionDescription(fi.Name, jsonObject), (JObject)jsonObject, path);
            manifestEntry.Objects.Add(objectDefinition);
            return manifestEntry;
        }
    }
}