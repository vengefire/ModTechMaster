using System.Collections.Generic;
using ModTechMaster.Core.Enums.Mods;
using ModTechMaster.Core.Interfaces.Models;
using Newtonsoft.Json.Linq;

namespace ModTechMaster.Data.Models.Mods
{
    public class ManifestEntry : JsonObjectBase, IManifestEntry
    {
        public ManifestEntry(Manifest manifest, ManifestEntryType entryType, string path, dynamic jsonObject) : base((JObject)jsonObject)
        {
            Manifest = manifest;
            EntryType = entryType;
            Path = path;
            Objects = new HashSet<IObjectDefinition>();
        }

        public IManifest Manifest { get; }
        public ManifestEntryType EntryType { get; }
        public string Path { get; }
        public HashSet<IObjectDefinition> Objects { get; private set; }
    }
}