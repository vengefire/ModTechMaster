namespace ModTechMaster.Logic.Processors
{
    using System.Diagnostics;
    using System.IO;
    using System.Linq;

    using ModTechMaster.Core.Enums.Mods;
    using ModTechMaster.Core.Interfaces.Models;
    using ModTechMaster.Core.Interfaces.Processors;
    using ModTechMaster.Data.Models.Mods;
    using ModTechMaster.Logic.Factories;

    public class ManifestEntryProcessor : IManifestEntryProcessor
    {
        public IManifestEntry ProcessManifestEntry(
            IManifest manifest,
            ObjectType entryType,
            string path,
            dynamic jsonObject)
        {
            var manifestEntry = new ManifestEntry(manifest, entryType, path, jsonObject);

            var objectDefinitionProcessor =
                ObjectDefinitionProcessorFactory.ObjectDefinitionProcessorFactorySingleton.Get(entryType);

            var targetDirectory = Path.Combine(manifest.Mod.SourceDirectoryPath, manifestEntry.Path);
            if (!Directory.Exists(targetDirectory))
            {
                Debug.WriteLine($"Folder does not exist {targetDirectory}");
                return manifestEntry;
            }

            var di = new DirectoryInfo(targetDirectory);
            di.GetFiles("*.*").ToList().ForEach(
                fi =>
                    {
                        var objectDefinition = objectDefinitionProcessor.ProcessObjectDefinition(manifestEntry, di, fi);
                        if (objectDefinition != null)
                        {
                            manifestEntry.Objects.Add(objectDefinition);
                        }
                    });

            return manifestEntry;
        }
    }
}