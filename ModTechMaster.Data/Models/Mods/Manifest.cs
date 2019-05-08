using System.Linq;

namespace ModTechMaster.Data.Models.Mods
{
    using System.Collections.Generic;
    using Core.Interfaces.Models;
    using Newtonsoft.Json.Linq;

    public class Manifest : JsonObjectBase, IManifest
    {
        public Manifest(Mod mod, dynamic jsonObject) : base((JArray)jsonObject, Core.Enums.Mods.ObjectType.Manifest)
        {
            this.Mod = mod;
            this.Entries = new HashSet<IManifestEntry>();
        }

        public IMod Mod { get; }

        public HashSet<IManifestEntry> Entries { get; }
        public List<IReferenceableObject> GetReferenceableObjects()
        {
            return Entries.SelectMany(entry => entry.GetReferenceableObjects()).ToList();
        }
    }
}