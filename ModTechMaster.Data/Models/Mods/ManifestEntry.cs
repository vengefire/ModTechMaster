using System;
using System.IO;
using ModTechMaster.Data.Models.Mods.TypedObjectDefinitions;
using Newtonsoft.Json;

namespace ModTechMaster.Data.Models.Mods
{
    using System.Collections.Generic;
    using System.Linq;
    using Core.Enums.Mods;
    using Core.Interfaces.Models;
    using Newtonsoft.Json.Linq;

    public class ManifestEntry : JsonObjectBase, IManifestEntry
    {
        public ManifestEntry(IManifest manifest, ObjectType entryType, string path, dynamic jsonObject) : base((JObject)jsonObject, ObjectType.ManifestEntry)
        {
            this.Manifest = manifest;
            this.EntryType = entryType;
            this.Path = path;
            this.Objects = new HashSet<IObjectDefinition>();
            this.Resources = new HashSet<IResourceDefinition>();
        }

        public HashSet<IResourceDefinition> Resources { get; }

        public IManifest Manifest { get; }

        public ObjectType EntryType { get; }

        public string Path { get; }

        public HashSet<IObjectDefinition> Objects { get; }

        public List<IReferenceableObject> GetReferenceableObjects()
        {
            return this.Objects.Select(definition => definition as IReferenceableObject).ToList();
        }

        public override string Name => $"{this.EntryType.ToString()} - {this.Path}";
        public override string Id => this.Name;

        public void ParseStreamingAssets()
        {
            void RecurseStreamingAssetsDirectory(string path)
            {
                DirectoryInfo di = new DirectoryInfo(path);
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
                                objectDefinition = new ObjectDefinition(ObjectType.StreamingAssetsData, description, jsonData, fi.FullName);
                                break;
                            case "pilot":
                                objectDefinition = new PilotObjectDefinition(ObjectType.PilotDef, description, jsonData, fi.FullName);
                                break;
                            case "simgameconstants":
                                objectDefinition = new SimGameConstantsObjectDefinition(ObjectType.SimGameConstants, ObjectDefinitionDescription.CreateDefault(jsonData.Description), jsonData, fi.FullName);
                                break;
                            default:
                                throw new InvalidProgramException($"Unknown streaming assets folder type detected [{hostDirectory}]");
                        }

                        Objects.Add(objectDefinition);
                    }
                    else
                    {
                        switch (hostDirectory.ToLower())
                        {
                            case "itemcollections":
                                var itemCollection = new ItemCollectionObjectDefinition(ObjectType.ItemCollectionDef, fileData, fi.FullName);
                                itemCollection.AddMetaData();
                                this.Objects.Add(itemCollection);
                                break;
                            default:
                                IResourceDefinition resourceDefinition;
                                // Handle resource file style definition...
                                switch (fi.Name)
                                {
                                    default:
                                        resourceDefinition = new ResourceDefinition(ObjectType.UnhandledResource, fi.FullName, fi.Name, fi.Name);
                                        break;
                                }

                                Resources.Add(resourceDefinition);
                                // TBD: Add Note here -- throw new InvalidProgramException();
                                break;
                        }
                    }
                }

                di.GetDirectories().ToList().ForEach(subdi => RecurseStreamingAssetsDirectory(subdi.FullName));
            }

            RecurseStreamingAssetsDirectory(Path);
        }
    }
}