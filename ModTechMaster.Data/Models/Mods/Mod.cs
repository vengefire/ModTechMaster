namespace ModTechMaster.Data.Models.Mods
{
    using System.Collections.Generic;
    using Core.Interfaces.Models;
    using Newtonsoft.Json.Linq;

    public class Mod : JsonObjectSourcedFromFile, IMod
    {
        private readonly bool? enabled;

        public Mod(
            string name,
            bool? enabled,
            string version,
            string description,
            string author,
            string website,
            string contact,
            HashSet<string> dependsOn,
            HashSet<string> conflictsWith,
            string sourceFilePath,
            dynamic jsonObject) : base(sourceFilePath, (JObject)jsonObject)
        {
            this.Name = name;
            this.enabled = enabled;
            this.Version = version;
            this.Description = description;
            this.Author = author;
            this.Website = website;
            this.Contact = contact;
            this.DependsOn = dependsOn;
            this.ConflictsWith = conflictsWith;
        }

        public string Name { get; }

        public bool Enabled => this.enabled ?? true;

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