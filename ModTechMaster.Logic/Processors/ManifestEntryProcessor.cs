﻿namespace ModTechMaster.Logic.Processors
{
    using System.Diagnostics;
    using System.IO;
    using System.Linq;

    using Castle.Core.Logging;

    using ModTechMaster.Core.Enums.Mods;
    using ModTechMaster.Core.Interfaces.Models;
    using ModTechMaster.Core.Interfaces.Processors;
    using ModTechMaster.Core.Interfaces.Services;
    using ModTechMaster.Data.Models.Mods;
    using ModTechMaster.Logic.Factories;

    public class ManifestEntryProcessor : IManifestEntryProcessor
    {
        private readonly ILogger logger;

        public ManifestEntryProcessor(ILogger logger)
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

            var objectDefinitionProcessor =
                ObjectDefinitionProcessorFactory.ObjectDefinitionProcessorFactorySingleton.Get(entryType);

            var targetDirectory = Path.Combine(manifest.Mod.SourceDirectoryPath, manifestEntry.Path);
            if (!Directory.Exists(targetDirectory))
            {
                Debug.WriteLine($"Folder does not exist {targetDirectory}");
                return manifestEntry;
            }

            var di = new DirectoryInfo(targetDirectory);
            di.GetFiles("*.*", SearchOption.AllDirectories).ToList().ForEach(
                fi =>
                    {
                        var objectDefinition = objectDefinitionProcessor.ProcessObjectDefinition(
                            manifestEntry,
                            di,
                            fi,
                            referenceFinderService);
                        if (objectDefinition != null)
                        {
                            manifestEntry.Objects.Add(objectDefinition);
                        }
                    });

            return manifestEntry;
        }
    }
}