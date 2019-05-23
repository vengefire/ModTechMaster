﻿namespace ModTechMaster.Logic.Processors
{
    using System.IO;
    using System.Linq;

    using ModTechMaster.Core.Enums.Mods;
    using ModTechMaster.Core.Interfaces.Models;
    using ModTechMaster.Core.Interfaces.Processors;
    using ModTechMaster.Data.Models.Mods;
    using ModTechMaster.Logic.Factories;

    using Newtonsoft.Json.Linq;

    public class ResourceManifestEntryProcessor : IManifestEntryProcessor
    {
        public IManifestEntry ProcessManifestEntry(
            IManifest manifest,
            ObjectType entryType,
            string path,
            dynamic jsonObject)
        {
            var manifestEntry = new ManifestEntry(manifest, entryType, path, jsonObject);
            var di = new DirectoryInfo(Path.Combine(manifest.Mod.SourceDirectoryPath, manifestEntry.Path));
            if (!di.Exists)
            {
                return manifestEntry;
            }

            di.GetFiles("*.*").ToList().ForEach(
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