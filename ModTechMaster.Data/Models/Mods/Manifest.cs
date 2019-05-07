using System.Collections.Generic;
using ModTechMaster.Core.Interfaces.Models;
using Newtonsoft.Json.Linq;

namespace ModTechMaster.Data.Models.Mods
{
    public class Manifest : JsonObjectBase, IManifest
    {
        public Manifest(Mod mod, dynamic jsonObject) : base((JArray)jsonObject)
        {
            Mod = mod;
            Entries = new HashSet<IManifestEntry>();
        }

        public IMod Mod { get; private set; }
        public HashSet<IManifestEntry> Entries { get; private set; }
    }
}
