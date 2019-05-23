namespace ModTechMaster.Logic.Processors
{
    using System;
    using System.IO;
    using System.Linq;

    using ModTechMaster.Core.Enums.Mods;
    using ModTechMaster.Core.Interfaces.Models;
    using ModTechMaster.Core.Interfaces.Processors;
    using ModTechMaster.Data.Models.Mods;
    using ModTechMaster.Logic.Factories;

    using Newtonsoft.Json.Linq;

    public class AssetBundleManifestEntryProcessor : IManifestEntryProcessor
    {
        public IManifestEntry ProcessManifestEntry(
            IManifest manifest,
            ObjectType entryType,
            string path,
            dynamic jsonObject)
        {
            var manifestEntry = new ManifestEntry(manifest, entryType, path, jsonObject);

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
                            fi.FullName);
                        manifestEntry.Objects.Add(objectDefinition);
                    });
            return manifestEntry;
        }
    }
}