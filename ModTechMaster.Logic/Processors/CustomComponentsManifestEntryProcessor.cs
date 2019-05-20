namespace ModTechMaster.Logic.Processors
{
    using System;
    using System.IO;
    using System.Linq;
    using Core.Enums.Mods;
    using Core.Interfaces.Models;
    using Core.Interfaces.Processors;
    using Data.Models.Mods;
    using Factories;
    using Newtonsoft.Json;

    public class CustomComponentsManifestEntryProcessor : IManifestEntryProcessor
    {
        public IManifestEntry ProcessManifestEntry(IManifest manifest, ObjectType entryType, string path, dynamic jsonObject)
        {
            var manifestEntry =
                new ManifestEntry(manifest, entryType, path, jsonObject);

            var di = new DirectoryInfo(Path.Combine(manifest.Mod.SourceDirectoryPath, manifestEntry.Path));
            var files = di.EnumerateFiles();

            if (files.Count() > 1)
            {
                throw new InvalidProgramException($"Encountered more than ONE CC settings files for a CC Manifest Entry at [{di.FullName}]");
            }

            var file = files.First();

            dynamic ccSettingsData = JsonConvert.DeserializeObject(File.ReadAllText(file.FullName));
            foreach (var ccSetting in ccSettingsData.Settings)
            {
                var objectDefinition = ObjectDefinitionFactory.ObjectDefinitionFactorySingleton.Get(entryType, null, ccSetting, file.FullName);
                manifestEntry.Objects.Add(objectDefinition);
            }

            return manifestEntry;
        }
    }
}