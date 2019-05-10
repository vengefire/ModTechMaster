namespace ModTechMaster.Logic.Processors
{
    using System.IO;
    using System.Linq;
    using Core.Enums.Mods;
    using Core.Interfaces.Models;
    using Core.Interfaces.Processors;
    using Data.Models.Mods;
    using Factories;
    using Newtonsoft.Json.Linq;

    public class ResourceManifestEntryProcessor : IManifestEntryProcessor
    {
        public IManifestEntry ProcessManifestEntry(IManifest manifest, ObjectType entryType, string path, dynamic jsonObject)
        {
            var manifestEntry = new ManifestEntry(manifest, entryType, path, jsonObject);
            var di = new DirectoryInfo(Path.Combine(manifest.Mod.SourceDirectoryPath, manifestEntry.Path));
            if (!di.Exists)
            {
                 // TODO: Add a warning here...
                return manifestEntry;
            }
            di.GetFiles("*.*").ToList().ForEach(
                                                fi =>
                                                {
                                                    var objectDefinition = ObjectDefinitionFactory.ObjectDefinitionFactorySingleton.Get(entryType, new ObjectDefinitionDescription(fi.Name, fi.Name, jsonObject), (JObject)jsonObject, fi.FullName);
                                                    manifestEntry.Objects.Add(objectDefinition);
                                                });
            return manifestEntry;
        }
    }
}