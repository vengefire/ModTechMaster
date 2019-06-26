namespace ModTechMaster.Data.Models.Mods
{
    using System;
    using System.ComponentModel;

    using BattleTech;

    using Newtonsoft.Json;

    public class ModEntry
    {
        [JsonIgnore]
        private VersionManifestEntry versionManifestEntry;

        [JsonConstructor]
        public ModEntry(string path, bool shouldMergeJSON = false)
        {
            this.Path = path;
            this.ShouldMergeJSON = shouldMergeJSON;
        }

        public ModEntry(ModEntry parent, string path, string id)
        {
            this.Path = path;
            this.Id = id;

            this.Type = parent.Type;
            this.AssetBundleName = parent.AssetBundleName;
            this.AssetBundlePersistent = parent.AssetBundlePersistent;
            this.ShouldMergeJSON = parent.ShouldMergeJSON;
            this.ShouldAppendText = parent.ShouldAppendText;
            this.AddToAddendum = parent.AddToAddendum;
            this.AddToDB = parent.AddToDB;
        }

        public string AddToAddendum { get; set; }

        [DefaultValue(true)]
        public bool AddToDB { get; set; } = true;

        public string AssetBundleName { get; set; }

        public bool? AssetBundlePersistent { get; set; }

        public string Id { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string Path { get; set; }

        [DefaultValue(false)]
        public bool ShouldAppendText { get; set; }

        [DefaultValue(false)]
        public bool ShouldMergeJSON { get; set; }

        public string Type { get; set; }

        public VersionManifestEntry GetVersionManifestEntry()
        {
            return this.versionManifestEntry ?? (this.versionManifestEntry = new VersionManifestEntry(this.Id, this.Path, this.Type, DateTime.Now, "1", this.AssetBundleName, this.AssetBundlePersistent));
        }
    }
}
