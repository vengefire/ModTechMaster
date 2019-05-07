namespace ModTechMaster.Data.Models.Mods
{
    using System.Collections.Generic;
    using Core.Enums.Mods;
    using Core.Interfaces.Models;
    using Newtonsoft.Json.Linq;

    public class ManifestEntry : JsonObjectBase, IManifestEntry
    {
        public ManifestEntry(Manifest manifest, ManifestEntryType entryType, string path, dynamic jsonObject) : base((JObject)jsonObject)
        {
            this.Manifest = manifest;
            this.EntryType = entryType;
            this.Path = path;
            this.Objects = new HashSet<IObjectDefinition>();
        }

        public IManifest Manifest { get; }

        public ManifestEntryType EntryType { get; }

        public string Path { get; }

        public HashSet<IObjectDefinition> Objects { get; }
    }
}