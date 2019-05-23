namespace ModTechMaster.Data.Models.Mods
{
    using System.Collections.Generic;

    using ModTechMaster.Core.Constants;
    using ModTechMaster.Core.Enums.Mods;
    using ModTechMaster.Core.Interfaces.Models;

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
            dynamic jsonObject,
            double sizeOnDisk,
            string dll)
            : base(ObjectType.Mod, sourceFilePath, (JObject)jsonObject)
        {
            this.Name = name;
            this.Id = this.GetId;
            this.enabled = enabled;
            this.Version = version;
            this.Description = description;
            this.Author = author;
            this.Website = website;
            this.Contact = contact;
            this.DependsOn = dependsOn;
            this.ConflictsWith = conflictsWith;
            this.SizeOnDisk = sizeOnDisk;
            this.Dll = dll;
            this.AddMetaData();
        }

        public string Author { get; }

        public HashSet<string> ConflictsWith { get; set; }

        public string Contact { get; }

        public HashSet<string> DependsOn { get; }

        public string Description { get; }

        public string Dll { get; }

        public bool Enabled => this.enabled ?? true;

        public string GetId => this.Name;

        public override string Id { get; }

        public IManifest Manifest { get; set; }

        public Dictionary<string, object> MetaData { get; } = new Dictionary<string, object>();

        public override string Name { get; }

        public List<IResourceDefinition> ResourceFiles { get; } = new List<IResourceDefinition>();

        public double SizeOnDisk { get; }

        public string Version { get; }

        public string Website { get; }

        public void AddMetaData()
        {
            this.MetaData.Add(Keywords.Id, this.GetId);
            this.MetaData.Add(Keywords.Name, this.Name);
            this.MetaData.Add(Keywords.DependsOn, new List<string>(this.DependsOn));
        }

        public List<IReferenceableObject> GetReferenceableObjects()
        {
            var objects = new List<IReferenceableObject>();
            if (this.Manifest != null)
            {
                objects.AddRange(this.Manifest.GetReferenceableObjects());
            }

            objects.Add(this);
            return objects;
        }
    }
}