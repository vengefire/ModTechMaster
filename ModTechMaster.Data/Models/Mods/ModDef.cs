// ReSharper disable CollectionNeverUpdated.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace ModTechMaster.Data.Models.Mods
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public class ModDef
    {
        // adding and running code
        [JsonIgnore]
        public Assembly Assembly { get; set; }

        public string Author { get; set; }

        public string BattleTechVersion { get; set; }

        public string BattleTechVersionMax { get; set; }

        public string BattleTechVersionMin { get; set; }

        public HashSet<string> ConflictsWith { get; set; } = new HashSet<string>();

        public string Contact { get; set; }

        // custom resources types that will be passed into FinishedLoading method
        public HashSet<string> CustomResourceTypes { get; set; } = new HashSet<string>();

        // load order and requirements
        public HashSet<string> DependsOn { get; set; } = new HashSet<string>();

        // informational
        public string Description { get; set; }

        // this path will be set at runtime by ModTek
        [JsonIgnore]
        public string Directory { get; set; }

        public string DLL { get; set; }

        public string DLLEntryPoint { get; set; }

        [DefaultValue(false)]
        public bool EnableAssemblyVersionCheck { get; set; } = false;

        // this will abort loading by ModTek if set to false
        [DefaultValue(true)]
        public bool Enabled { get; set; } = true;

        [DefaultValue(false)]
        public bool IgnoreLoadFailure { get; set; }

        // changing implicit loading behavior
        [DefaultValue(true)]
        public bool LoadImplicitManifest { get; set; } = true;

        // manifest, for including any kind of things to add to the game's manifest
        public List<ModEntry> Manifest { get; set; } = new List<ModEntry>();

        [JsonProperty(Required = Required.Always)]
        public string Name { get; set; }

        public HashSet<string> OptionallyDependsOn { get; set; } = new HashSet<string>();

        public DateTime? PackagedOn { get; set; }

        // remove these entries by ID from the game
        public List<string> RemoveManifestEntries { get; set; } = new List<string>();

        // a settings file to be nice to our users and have a known place for settings
        // these will be different depending on the mod obviously
        public JObject Settings { get; set; } = new JObject();

        // versioning
        public string Version { get; set; }

        public string Website { get; set; }

        /// <summary>
        ///     Creates a ModDef from a path to a mod.json
        /// </summary>
        public static ModDef CreateFromPath(string path)
        {
            var modDef = JsonConvert.DeserializeObject<ModDef>(File.ReadAllText(path));
            modDef.Directory = Path.GetDirectoryName(path);
            return modDef;
        }

        /// <summary>
        ///     Checks if all dependencies are present in param loaded
        /// </summary>
        public bool AreDependenciesResolved(IEnumerable<string> loaded)
        {
            return this.DependsOn.Count == 0 || this.DependsOn.Intersect(loaded).Count() == this.DependsOn.Count;
        }

        /// <summary>
        ///     Checks against provided list of mods to see if any of them conflict
        /// </summary>
        public bool HasConflicts(IEnumerable<string> otherMods)
        {
            return this.ConflictsWith.Intersect(otherMods).Any();
        }

        /// <summary>
        ///     Checks to see if this ModDef should load, providing a reason if shouldn't load
        /// </summary>
        public bool ShouldTryLoad(List<string> alreadyTryLoadMods, out string reason)
        {
            if (!this.Enabled)
            {
                reason = "it is disabled";
                this.IgnoreLoadFailure = true;
                return false;
            }

            if (alreadyTryLoadMods.Contains(this.Name))
            {
                reason = $"ModTek already loaded with the same name. Skipping load from {this.Directory}.";
                return false;
            }

            // check game version vs. specific version or against min/max
            if (!string.IsNullOrEmpty(this.BattleTechVersion) && !VersionInfo.ProductVersion.StartsWith(this.BattleTechVersion))
            {
                reason = $"it specifies a game version and this isn't it ({this.BattleTechVersion} vs. game {VersionInfo.ProductVersion})";
                return false;
            }

            /*var btgVersion = new Version(VersionInfo.ProductVersion);
            if (!string.IsNullOrEmpty(this.BattleTechVersionMin) && btgVersion < new Version(this.BattleTechVersionMin))
            {
                reason = $"it doesn't match the min version set in the mod.json ({this.BattleTechVersionMin} vs. game {VersionInfo.ProductVersion})";
                return false;
            }

            if (!string.IsNullOrEmpty(this.BattleTechVersionMax) && btgVersion > new Version(this.BattleTechVersionMax))
            {
                reason = $"it doesn't match the max version set in the mod.json ({this.BattleTechVersionMax} vs. game {VersionInfo.ProductVersion})";
                return false;
            }*/

            reason = string.Empty;
            return true;
        }
    }
}
