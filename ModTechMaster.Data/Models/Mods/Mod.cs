using System.Collections.Generic;
using ModTechMaster.Core.Interfaces.Models;
using Newtonsoft.Json.Linq;

namespace ModTechMaster.Data.Models.Mods
{
    public class Mod : JsonObjectSourcedFromFile, IMod
    {
        private readonly bool? _enabled;

        public Mod(string name, bool? enabled, string version, string description, string author, string website,
            string contact, HashSet<string> dependsOn, HashSet<string> conflictsWith, string sourceFilePath, dynamic jsonObject) : base(sourceFilePath, (JObject)jsonObject)
        {
            Name = name;
            _enabled = enabled;
            Version = version;
            Description = description;
            Author = author;
            Website = website;
            Contact = contact;
            DependsOn = dependsOn;
            ConflictsWith = conflictsWith;
        }

        public string Name { get; }

        public bool Enabled => _enabled ?? true;

        public string Version { get; }
        public string Description { get; }
        public string Author { get; }
        public string Website { get; }
        public string Contact { get; }

        public IManifest Manifest { get; set; }

        public HashSet<string> DependsOn { get; }
        public HashSet<string> ConflictsWith { get; set; }
    }
}