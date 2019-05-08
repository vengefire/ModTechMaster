namespace ModTechMaster.Logic.Processors
{
    using System.IO;
    using System.Linq;
    using Core.Enums.Mods;
    using Core.Interfaces.Models;
    using Core.Interfaces.Processors;
    using Data.Models.Mods;
    using Factories;

    public class ManifestEntryProcessor : IManifestEntryProcessor
    {
        public IManifestEntry ProcessManifestEntry(IManifest manifest, ObjectType entryType, string path, dynamic jsonObject)
        {
            var manifestEntry =
                new ManifestEntry(manifest, entryType, path, jsonObject);

            var objectDefinitionProcessor = ObjectDefinitionProcessorFactory.ObjectDefinitionProcessorFactorySingleton.Get(entryType);

            var di = new DirectoryInfo(Path.Combine(manifest.Mod.SourceDirectoryPath, manifestEntry.Path));
            di.GetFiles("*.*").ToList().ForEach(
                                                   fi =>
                                                   {
                                                       var objectDefinition = objectDefinitionProcessor.ProcessObjectDefinition(manifestEntry, di, fi);
                                                       manifestEntry.Objects.Add(objectDefinition);
                                                   });

            return manifestEntry;
        }
    }
}