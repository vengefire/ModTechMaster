using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Framework.Utils.Directory;
using ModTechMaster.Core.Enums;
using ModTechMaster.Core.Enums.Mods;
using ModTechMaster.Core.Interfaces.Models;
using ModTechMaster.Core.Interfaces.Services;
using ModTechMaster.Data.Models.Mods;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ModTechMaster.Logic.Services
{
    public class ModService : IModService
    {
        private readonly IMessageService _messageService;

        public ModService(IMessageService messageService)
        {
            _messageService = messageService;
        }

        public IMod TryLoadFromPath(string path)
        {
            if (!DirectoryUtils.Exists(path) || !File.Exists(ModFilePath(path))) return null;

            return LoadFromPath(path);
        }

        private IMod LoadFromPath(string path)
        {
            dynamic src = JsonConvert.DeserializeObject(File.ReadAllText(ModFilePath(path)));

            var mod = InitModFromJson(src, ModFilePath(path));
            ProcessModConfig(mod);

            return mod;
        }

        private void ProcessModConfig(Mod mod)
        {
            // Process Manifest
            if (mod.JsonObject.Manifest == null) return;

            var manifest = ProcessManifest(mod);
            mod.Manifest = manifest;
            // Process implicits like StreamingAssets folder...
        }

        private Manifest ProcessManifest(Mod mod)
        {
            var manifest = new Manifest(mod, mod.JsonObject.Manifest);
            foreach (var manifestEntrySrc in manifest.JsonObject)
            {
                ManifestEntry manifestEntry = ProcessManifestEntry(manifest, manifestEntrySrc);
                manifest.Entries.Add(manifestEntry);
            }

            return manifest;
        }

        private ManifestEntry ProcessManifestEntry(Manifest manifest, dynamic manifestEntrySrc)
        {
            ManifestEntryType entryType;
            if (!Enum.TryParse((string) manifestEntrySrc.Type, out entryType))
            {
                _messageService.PushMessage(
                    $"Failed to parse Manifest Entry Type [{manifestEntrySrc.Type.ToString()}].", MessageType.Error);
                return null;
            }

            var manifestEntry =
                new ManifestEntry(manifest, entryType, (string) manifestEntrySrc.Path, manifestEntrySrc);
            var di = new DirectoryInfo(Path.Combine(manifest.Mod.SourceDirectoryPath, manifestEntry.Path));
            di.GetFiles("*.json").ToList().ForEach(
                fi =>
                {
                    var objectDefinition = ProcessObjectDefinition(di, fi);
                    manifestEntry.Objects.Add(objectDefinition);
                });

            return manifestEntry;
        }

        private IObjectDefinition ProcessObjectDefinition(DirectoryInfo di, FileInfo fi)
        {
            dynamic json = JsonConvert.DeserializeObject(File.ReadAllText(fi.FullName));
            return new ObjectDefinition(ProcessObjectDescription(json.Description), json, fi.FullName);
        }

        private IObjectDefinitionDescription ProcessObjectDescription(dynamic description)
        {
            if (description == null) return null;

            string id = description.Id != null ? description.Id.ToString() : null;
            string name = description.Name != null ? description.Name.ToString() : null;
            string desc = description.Description != null ? description.Description.ToString() : null;
            string icon = description.Icon != null ? description.Icon.ToString() : null;
            return new ObjectDefinitionDescription(id, name, desc, icon, description);
        }

        private Mod InitModFromJson(dynamic src, string path)
        {
            var depends = src.DependsOn == null
                ? new HashSet<string>()
                : new HashSet<string>(((JArray) src.DependsOn).Select(token => token.ToString()));
            var conflicts = src.ConflictsWith == null
                ? new HashSet<string>()
                : new HashSet<string>(((JArray) src.ConflictsWith).Select(token => token.ToString()));
            var name = src.Name.ToString();
            var enabled = (bool?) src.Enabled;
            var version = src.Version?.ToString();
            var description = src.Description?.ToString();
            var author = src.Author?.ToString();
            var website = src.Website?.ToString();
            var contact = src.Contact?.ToString();
            return new Mod(name, enabled, version,
                description, author, website, contact,
                depends, conflicts, path, src);
        }

        private static string ModFilePath(string path)
        {
            return Path.Combine(path, @"mod.json");
        }
    }
}