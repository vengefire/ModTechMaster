namespace ModTechMaster.Data.Models.Mods
{
    using System.Collections.Generic;
    using System.Linq;
    using Core.Enums.Mods;
    using Core.Interfaces.Models;
    using Newtonsoft.Json.Linq;

    public class Manifest : JsonObjectBase, IManifest
    {
        public Manifest(Mod mod, dynamic jsonObject) : base((JArray)jsonObject, ObjectType.Manifest)
        {
            this.Mod = mod;
            this.Entries = new HashSet<IManifestEntry>();
        }

        public IMod Mod { get; }

        public HashSet<IManifestEntry> Entries { get; }

        public List<IReferenceableObject> GetReferenceableObjects()
        {
            return this.Entries.SelectMany(entry => entry.Objects.Select(definition => definition as IReferenceableObject)).ToList();
        }
    }
}