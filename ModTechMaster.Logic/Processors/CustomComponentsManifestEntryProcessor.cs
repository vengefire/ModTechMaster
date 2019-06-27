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

    using Newtonsoft.Json;

    public class CustomComponentsManifestEntryProcessor : IManifestEntryProcessor
    {
        private readonly ILogger logger;

        public CustomComponentsManifestEntryProcessor(ILogger logger)
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
            var files = di.EnumerateFiles();

            if (entryType == ObjectType.CCDefaults && files.Count() > 1)
            {
                throw new InvalidProgramException(
                    $"Encountered more than ONE CC settings files for a CC Manifest Entry at [{di.FullName}]");
            }

            files.ToList().ForEach(file =>
                {
                    dynamic ccSettingsData = JsonConvert.DeserializeObject(File.ReadAllText(file.FullName));
                    foreach (var ccSetting in ccSettingsData.Settings)
                    {
                        var objectDefinition = ObjectDefinitionFactory.ObjectDefinitionFactorySingleton.Get(
                            entryType,
                            null,
                            ccSetting,
                            file.FullName,
                            referenceFinderService);
                        manifestEntry.Objects.Add(objectDefinition);
                    }
                });

            return manifestEntry;
        }
    }
}