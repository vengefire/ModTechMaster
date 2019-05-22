namespace ModTechMaster.Logic.Services
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using Castle.Core.Logging;
    using Core.Enums;
    using Core.Enums.Mods;
    using Core.Interfaces.Factories;
    using Core.Interfaces.Models;
    using Core.Interfaces.Services;
    using Data.Annotations;
    using Data.Models.Mods;
    using Framework.Utils.Directory;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public class ModService : IModService
    {
        private readonly ILogger _logger;
        private readonly IManifestEntryProcessorFactory manifestEntryProcessorFactory;
        private readonly IMessageService messageService;

        public ModService(IMessageService messageService, IManifestEntryProcessorFactory manifestEntryProcessorFactory, ILogger logger)
        {
            this.messageService = messageService;
            this.manifestEntryProcessorFactory = manifestEntryProcessorFactory;
            this._logger = logger;
            this.ModCollection = new ModCollection("Unknown Mod Collection", string.Empty);
        }

        public IModCollection LoadCollectionFromPath(string path, string name)
        {
            if (!DirectoryUtils.Exists(path))
            {
                return null;
            }

            var di = new DirectoryInfo(path);
            this.ModCollection.Name = name;
            this.ModCollection.Path = path;

            this._logger.Info($"Processing mods from [{di.FullName}]");

            var result = Parallel.ForEach(
                di.GetDirectories(), sub =>
                {
                    this._logger.Debug(".");
                    var mod = this.TryLoadFromPath(sub.FullName);
                    this.ModCollection.AddModToCollection(mod);
                });
            this.ModCollection.Mods.Sort((mod, mod1) => string.Compare(mod.Name, mod1.Name, StringComparison.OrdinalIgnoreCase));

            this.OnPropertyChanged(nameof(ModService.ModCollection));

            return this.ModCollection;
        }

        public IModCollection ModCollection { get; }

        public IMod TryLoadFromPath(string path)
        {
            if (!DirectoryUtils.Exists(path) ||
                !File.Exists(ModService.ModFilePath(path)))
            {
                return null;
            }

            return this.LoadFromPath(path);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private static string ModFilePath(string path)
        {
            return Path.Combine(path, @"mod.json");
        }

        private IMod LoadFromPath(string path)
        {
            dynamic src = JsonConvert.DeserializeObject(File.ReadAllText(ModService.ModFilePath(path)));

            var mod = this.InitModFromJson(src, ModService.ModFilePath(path));
            this.ProcessModConfig(mod);

            return mod;
        }

        private void ProcessModConfig(Mod mod)
        {
            // Process Manifest
            var manifest = this.ProcessManifest(mod);

            // Process implicits like StreamingAssets folder...
            // Special handling for sim game constants...
            var streamingAssetsPath = @"StreamingAssets";
            var saPath = Path.Combine(mod.SourceDirectoryPath, streamingAssetsPath);
            if (Directory.Exists(saPath))
            {
                // Handle
                this.AddStreamingAssetsManifestEntry(saPath, mod, manifest);
            }

            if (manifest.Entries.Any())
            {
                mod.Manifest = manifest;
            }

            var di = new DirectoryInfo(mod.SourceDirectoryPath);
            foreach (var file in di.EnumerateFiles())
            {
                switch (file.Extension.ToLower())
                {
                    case ".dll":
                        mod.ResourceFiles.Add(new ResourceDefinition(ObjectType.Dll, file.FullName, file.Name, file.Name));
                        break;
                    default:
                        mod.ResourceFiles.Add(new ResourceDefinition(ObjectType.UnhandledResource, file.FullName, file.Name, file.Name));
                        break;
                }
            }
        }

        private void AddStreamingAssetsManifestEntry(string simPath, Mod mod, Manifest manifest)
        {
            var newEntry = new ManifestEntry(manifest, ObjectType.StreamingAssetsData, simPath, null);
            newEntry.ParseStreamingAssets();
            manifest.Entries.Add(newEntry);
        }

        private Manifest ProcessManifest(Mod mod)
        {
            var manifest = new Manifest(mod, mod.JsonObject.Manifest);
            if (manifest.JsonObject != null)
            {
                foreach (var manifestEntrySrc in manifest.JsonObject)
                {
                    ManifestEntry manifestEntry = this.ProcessManifestEntry(manifest, manifestEntrySrc);
                    if (manifestEntry == null)
                    {
                        throw new InvalidProgramException();
                    }

                    manifest.Entries.Add(manifestEntry);
                }
            }

            return manifest;
        }

        private ManifestEntry ProcessManifestEntry(Manifest manifest, dynamic manifestEntrySrc)
        {
            ObjectType entryType;
            if (!Enum.TryParse((string)manifestEntrySrc.Type, out entryType))
            {
                this.messageService.PushMessage($"Failed to parse Manifest Entry Type [{manifestEntrySrc.Type.ToString()}].", MessageType.Error);
                return null;
            }

            var manifestEntryProcessor = this.manifestEntryProcessorFactory.Get(entryType);
            return manifestEntryProcessor.ProcessManifestEntry(manifest, entryType, (string)manifestEntrySrc.Path, manifestEntrySrc);
        }

        private Mod InitModFromJson(dynamic src, string path)
        {
            var fi = new FileInfo(path);
            var di = new DirectoryInfo(fi.DirectoryName);
            var depends = src.DependsOn == null
                ? new HashSet<string>()
                : new HashSet<string>(((JArray)src.DependsOn).Select(token => token.ToString()));
            var conflicts = src.ConflictsWith == null
                ? new HashSet<string>()
                : new HashSet<string>(((JArray)src.ConflictsWith).Select(token => token.ToString()));
            var name = src.Name.ToString();
            var enabled = (bool?)src.Enabled;
            var version = src.Version?.ToString();
            var description = src.Description?.ToString();
            var author = src.Author?.ToString();
            var website = src.Website?.ToString();
            var contact = src.Contact?.ToString();
            var dll = src.DLL?.ToString();
            return new Mod(
                name,
                enabled,
                version,
                description,
                author,
                website,
                contact,
                depends,
                conflicts,
                path,
                src,
                Convert.ToDouble(di.EnumerateFiles("*", SearchOption.AllDirectories).Sum(info => info.Length)) / 1024,
                dll);
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}