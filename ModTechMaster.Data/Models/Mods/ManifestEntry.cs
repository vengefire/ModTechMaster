namespace ModTechMaster.Data.Models.Mods
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using ModTechMaster.Core.Enums.Mods;
    using ModTechMaster.Core.Interfaces.Models;
    using ModTechMaster.Core.Interfaces.Services;
    using ModTechMaster.Data.Models.Mods.TypedObjectDefinitions;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public class ManifestEntry : JsonObjectBase, IManifestEntry
    {
        private readonly IReferenceFinderService referenceFinderService;

        public ManifestEntry(IManifest manifest, ObjectType entryType, string path, dynamic jsonObject, IReferenceFinderService referenceFinderService)
            : base((JObject)jsonObject, ObjectType.ManifestEntry)
        {
            this.referenceFinderService = referenceFinderService;
            this.Manifest = manifest;
            this.EntryType = entryType;
            this.Path = path;
            this.Objects = new HashSet<IObjectDefinition>();
            this.Resources = new HashSet<IResourceDefinition>();
        }

        public ObjectType EntryType { get; }

        public override string Id => this.Name;

        public IManifest Manifest { get; }

        public override string Name => $"{this.EntryType.ToString()} - {this.Path}";

        public HashSet<IObjectDefinition> Objects { get; }

        public string Path { get; }

        public HashSet<IResourceDefinition> Resources { get; }

        public List<IReferenceableObject> GetReferenceableObjects()
        {
            return this.Objects.Select(definition => definition as IReferenceableObject).ToList();
        }

        public void ParseStreamingAssets()
        {
            this.RecurseStreamingAssetsDirectory(this.Path);
        }

        public override IValidationResult ValidateObject()
        {
            return ValidationResult.AggregateResults(this.Objects.Select(definition => definition.ValidateObject()));
        }

        private void RecurseStreamingAssetsDirectory(string path)
        {
            var di = new DirectoryInfo(path);
            foreach (var fi in di.EnumerateFiles())
            {
                var fileName = fi.Name;
                var fileData = File.ReadAllText(fi.FullName);
                var hostDirectory = di.Name;

                if (fi.Extension == ".json")
                {
                    // Handle json object definition...
                    IObjectDefinition objectDefinition;
                    dynamic jsonData = JsonConvert.DeserializeObject(fileData);
                    var description = ObjectDefinitionDescription.CreateDefault(jsonData.Description);

                    // infer the object type from the current sub-directory.
                    switch (hostDirectory.ToLower())
                    {
                        case "constants":
                        case "milestones":
                        case "events":
                        case "cast":
                        case "behaviorvariables":
                        case "buildings":
                        case "hardpoints":
                        case "factions":
                        case "lifepathnode":
                        case "campaign":
                            objectDefinition = new ObjectDefinition(
                                ObjectType.StreamingAssetsData,
                                description,
                                jsonData,
                                fi.FullName, this.referenceFinderService);
                            break;
                        case "pilot":
                            objectDefinition = new PilotObjectDefinition(
                                ObjectType.PilotDef,
                                description,
                                jsonData,
                                fi.FullName,
                                this.referenceFinderService);
                            break;
                        case "simgameconstants":
                            objectDefinition = new SimGameConstantsObjectDefinition(
                                ObjectType.SimGameConstants,
                                ObjectDefinitionDescription.CreateDefault(jsonData.Description),
                                jsonData,
                                fi.FullName,
                                this.referenceFinderService);
                            break;
                        default:
                            throw new InvalidProgramException(
                                $"Unknown streaming assets folder type detected [{hostDirectory}]");
                    }

                    this.Objects.Add(objectDefinition);
                }
                else
                {
                    switch (hostDirectory.ToLower())
                    {
                        case "itemcollections":
                            var itemCollection = new ItemCollectionObjectDefinition(
                                ObjectType.ItemCollectionDef,
                                fileData,
                                fi.FullName);
                            itemCollection.AddMetaData();
                            this.Objects.Add(itemCollection);
                            break;
                        default:
                            IResourceDefinition resourceDefinition;

                            // Handle resource file style definition...
                            switch (fi.Name)
                            {
                                default:
                                    resourceDefinition = new ResourceDefinition(
                                        ObjectType.UnhandledResource,
                                        fi.FullName,
                                        fi.Name,
                                        fi.Name);
                                    break;
                            }

                            this.Resources.Add(resourceDefinition);

                            // TBD: Add Note here -- throw new InvalidProgramException();
                            break;
                    }
                }
            }

            di.GetDirectories().ToList().ForEach(subdi => this.RecurseStreamingAssetsDirectory(subdi.FullName));
        }
    }
}