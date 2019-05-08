using System.Linq;

namespace ModTechMaster.Data.Models.Mods
{
    using System.Collections.Generic;
    using Core.Enums.Mods;
    using Core.Interfaces.Models;
    using Newtonsoft.Json.Linq;

    public class ManifestEntry : JsonObjectBase, IManifestEntry
    {
        public ManifestEntry(IManifest manifest, ObjectType entryType, string path, dynamic jsonObject) : base((JObject)jsonObject, ObjectType.ManifestEntry)
        {
            this.Manifest = manifest;
            this.EntryType = entryType;
            this.Path = path;
            this.Objects = new HashSet<IObjectDefinition>();
        }

        public IManifest Manifest { get; }

        public ObjectType EntryType { get; }

        public string Path { get; }

        public HashSet<IObjectDefinition> Objects { get; }
        public List<IReferenceableObject> GetReferenceableObjects()
        {
            return this.Objects.Select(definition => definition as IReferenceableObject).ToList();
        }
    }
}