namespace ModTechMaster.Logic.Processors
{
    using System.IO;
    using System.Linq;

    using Castle.Core.Logging;

    using ModTechMaster.Core.Enums.Mods;
    using ModTechMaster.Core.Interfaces.Models;
    using ModTechMaster.Core.Interfaces.Processors;
    using ModTechMaster.Core.Interfaces.Services;
    using ModTechMaster.Data.Models.Mods;
    using ModTechMaster.Logic.Factories;

    using Newtonsoft.Json.Linq;

    public class ResourceManifestEntryProcessor : IManifestEntryProcessor
    {
        private readonly ILogger logger;

        public ResourceManifestEntryProcessor(ILogger logger)
        {
            this.logger = logger;
        }

        public IManifestEntry ProcessManifestEntry(
            IManifest manifest,
            ObjectType entryType,
            string path,
            dynamic jsonObject,
            IReferenceFinderService referenceFinderService)
        {
            var manifestEntry = new ManifestEntry(manifest, entryType, path, jsonObject, referenceFinderService);
            var di = new DirectoryInfo(Path.Combine(manifest.Mod.SourceDirectoryPath, manifestEntry.Path));
            if (!di.Exists)
            {
                return manifestEntry;
            }

            di.GetFiles("*.*").ToList().ForEach(
                fi =>
                    {
                        var identifier = fi.Name.Substring(0, fi.Name.Length - fi.Extension.Length);
                        var objectDefinition = ObjectDefinitionFactory.ObjectDefinitionFactorySingleton.Get(
                            entryType,
                            new ObjectDefinitionDescription(identifier, identifier, jsonObject),
                            (JObject)jsonObject,
                            fi.FullName,
                            referenceFinderService);
                        manifestEntry.Objects.Add(objectDefinition);
                    });
            return manifestEntry;
        }
    }
}