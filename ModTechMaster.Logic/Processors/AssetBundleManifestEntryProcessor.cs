namespace ModTechMaster.Logic.Processors
{
    using System;
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

    public class AssetBundleManifestEntryProcessor : IManifestEntryProcessor
    {
        private readonly ILogger logger;

        public AssetBundleManifestEntryProcessor(ILogger logger)
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

            var assetBundleDirectory = Path.Combine(manifest.Mod.SourceDirectoryPath, path);
            var di = new DirectoryInfo(assetBundleDirectory);
            if (!di.Exists)
            {
                throw new InvalidProgramException($"Expected asset bundle directory at [{di.FullName}].");
            }

            di.GetFiles("*.").ToList().ForEach(
                fi =>
                    {
                        var objectDefinition = ObjectDefinitionFactory.ObjectDefinitionFactorySingleton.Get(
                            entryType,
                            new ObjectDefinitionDescription(fi.Name, fi.Name, jsonObject),
                            (JObject)jsonObject,
                            fi.FullName,
                            referenceFinderService);
                        manifestEntry.Objects.Add(objectDefinition);
                    });
            return manifestEntry;
        }
    }
}