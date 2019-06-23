namespace ModTechMaster.Data.Models.Mods
{
    using System.Collections.Generic;
    using System.Linq;

    using ModTechMaster.Core.Enums.Mods;
    using ModTechMaster.Core.Interfaces.Models;

    using Newtonsoft.Json.Linq;

    public class Manifest : JsonObjectBase, IManifest
    {
        public Manifest(Mod mod, dynamic jsonObject)
            : base((JArray)jsonObject, ObjectType.Manifest)
        {
            this.Mod = mod;
            this.Entries = new HashSet<IManifestEntry>();
        }

        public HashSet<IManifestEntry> Entries { get; }

        public override string Id => this.Name;

        public IMod Mod { get; }

        public override string Name => $"{this.Mod.Name}-Manifest";

        public List<IReferenceableObject> GetReferenceableObjects()
        {
            return this.Entries
                .SelectMany(entry => entry.Objects.Select(definition => definition as IReferenceableObject)).ToList();
        }

        public override IValidationResult ValidateObject()
        {
            return ValidationResult.AggregateResults(this.Entries.Select(entry => entry.ValidateObject()));
        }
    }
}