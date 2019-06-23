namespace ModTechMaster.Data.Models.Mods
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using ModTechMaster.Core.Enums.Mods;
    using ModTechMaster.Core.Interfaces.Models;
    using ModTechMaster.Core.Interfaces.Services;
    using ModTechMaster.Data.Models.Mods.TypedObjectDefinitions;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public class ManifestEntry : JsonObjectBase, IManifestEntry
    {
        private readonly IReferenceFinderService referenceFinderService;

        public ManifestEntry(
            IManifest manifest,
            ObjectType entryType,
            string path,
            dynamic jsonObject,
            IReferenceFinderService referenceFinderService)
            : base((JObject)jsonObject, ObjectType.ManifestEntry)
        {
            this.referenceFinderService = referenceFinderService;
            this.Manifest = manifest;
            this.EntryType = entryType;
            this.Path = path;
            this.Objects = new HashSet<IObjectDefinition>();
            this.Resources = new HashSet<IResourceDefinition>();
        }

        public ObjectType EntryType { get; }

        public override string Id => this.Name;

        public IManifest Manifest { get; }

        public override string Name => $"{this.EntryType.ToString()} - {this.Path}";

        public HashSet<IObjectDefinition> Objects { get; }

        public string Path { get; }

        public HashSet<IResourceDefinition> Resources { get; }

        public List<IReferenceableObject> GetReferenceableObjects()
        {
            return this.Objects.Select(definition => definition as IReferenceableObject).ToList();
        }

        public override IValidationResult ValidateObject()
        {
            return ValidationResult.AggregateResults(this.Objects.Select(definition => definition.ValidateObject()));
        }
    }
}