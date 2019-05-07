namespace ModTechMaster.Data.Models.Mods
{
    using System.Collections.Generic;
    using Core.Interfaces.Models;
    using Newtonsoft.Json.Linq;

    public class Manifest : JsonObjectBase, IManifest
    {
        public Manifest(Mod mod, dynamic jsonObject) : base((JArray)jsonObject)
        {
            this.Mod = mod;
            this.Entries = new HashSet<IManifestEntry>();
        }

        public IMod Mod { get; }

        public HashSet<IManifestEntry> Entries { get; }
    }
}