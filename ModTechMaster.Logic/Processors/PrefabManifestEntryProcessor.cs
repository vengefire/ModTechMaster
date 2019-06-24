namespace ModTechMaster.Logic.Processors
{
    using Castle.Core.Logging;

    using ModTechMaster.Core.Enums.Mods;
    using ModTechMaster.Core.Interfaces.Models;
    using ModTechMaster.Core.Interfaces.Processors;
    using ModTechMaster.Core.Interfaces.Services;
    using ModTechMaster.Data.Models.Mods;
    using ModTechMaster.Logic.Factories;

    using Newtonsoft.Json.Linq;

    public class PrefabManifestEntryProcessor : IManifestEntryProcessor
    {
        private readonly ILogger logger;

        public PrefabManifestEntryProcessor(ILogger logger)
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

            var objectDefinition = ObjectDefinitionFactory.ObjectDefinitionFactorySingleton.Get(
                entryType,
                new ObjectDefinitionDescription(null, null, null, null, (JObject)jsonObject),
                (JObject)jsonObject,
                path,
                referenceFinderService);
            manifestEntry.Objects.Add(objectDefinition);
            return manifestEntry;
        }
    }
}