namespace ModTechMaster.Logic.Processors
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;

    using ModTechMaster.Core.Enums.Mods;
    using ModTechMaster.Core.Interfaces.Models;
    using ModTechMaster.Core.Interfaces.Processors;
    using ModTechMaster.Core.Interfaces.Services;
    using ModTechMaster.Data.Models.Mods;
    using ModTechMaster.Logic.Factories;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    internal class 
        ObjectDefinitionProcessor : IObjectDefinitionProcessor
    {
        public IObjectDefinition ProcessObjectDefinition(
            IManifestEntry manifestEntry,
            DirectoryInfo di,
            FileInfo fi,
            IReferenceFinderService referenceFinderService,
            ObjectType? objectTypeOverride = null)
        {
            if (fi.Extension != ".json")
            {
                Debug.WriteLine($"File {fi.FullName} is not a JSON file.");
                return null;
            }

            dynamic json = JsonConvert.DeserializeObject(File.ReadAllText(fi.FullName));

            if (manifestEntry.EntryType == ObjectType.AdvancedJSONMerge)
            {
                var targetIds = new List<string>();
                if (json.TargetID != null)
                {
                    targetIds.Add(json.TargetID.ToString());
                }
                else
                {
                    foreach (var id in json.TargetIDs)
                    {
                        targetIds.Add(id.ToString());
                    }
                }

                foreach (var targetId in targetIds)
                {

                    if (referenceFinderService.ReferenceableObjectProvider.GetReferenceableObjects().FirstOrDefault(
                                o => o is ObjectDefinition objectDefinition && objectDefinition.Id == targetId) is
                            ObjectDefinition parsedObject)
                    {
                        foreach (var instruction in json.Instructions)
                        {
                            var path = instruction.JSONPath.ToString();
                            var action = instruction.Action.ToString();
                            string value = string.Empty;
                            if (action != "Remove")
                            {
                                value = instruction.Value.ToString();
                            }

                            var token = ((JObject)parsedObject.JsonObject).SelectToken(path);
                            token = value;
                        }

                        parsedObject.MetaData.Clear();
                        parsedObject.Tags.Clear();
                        parsedObject.AddMetaData();
                    }
                    else
                    {
                        // Schedule a post completion update...
                        int i = 666;
                    }
                }
            }
            else if ((manifestEntry.JsonObject?.ShouldMergeJSON ?? false) == true)
            {
                var parsedObject = referenceFinderService.ReferenceableObjectProvider.GetReferenceableObjects().FirstOrDefault(o => o is ISourcedFromFile file && file.SourceFileName == fi.Name) as ObjectDefinition;

                /*if (parsedObject == null)
                {
                    IObjectDefinition testObject = null;
                    try
                    {
                        testObject = ObjectDefinitionFactory.ObjectDefinitionFactorySingleton.Get(
                            objectTypeOverride ?? manifestEntry.EntryType,
                            ProcessObjectDescription(json.Description),
                            json,
                            fi.FullName,
                            referenceFinderService);

                        parsedObject = referenceFinderService
                                               .ReferenceableObjectProvider.GetReferenceableObjects().FirstOrDefault(
                                                   o => o is ObjectDefinition objectDef
                                                        && objectDef.Id == testObject.Id) as
                                           ObjectDefinition;
                    }
                    catch (Exception)
                    {
                        // Man Fuck You.
                    }

                    if (parsedObject == null)
                    {
                        return testObject;
                    }
                }*/

                if (parsedObject != null)
                {
                    ((JObject)parsedObject.JsonObject).Merge(json);
                    parsedObject.MetaData.Clear();
                    parsedObject.Tags.Clear();
                    parsedObject.AddMetaData();
                }
                else
                {
                    // Schedule a post completion update...
                    int i = 666;
                }
            }
            else
            {
                return ObjectDefinitionFactory.ObjectDefinitionFactorySingleton.Get(
                    objectTypeOverride ?? manifestEntry.EntryType,
                    ProcessObjectDescription(json.Description),
                    json,
                    fi.FullName,
                    referenceFinderService);
            }

            return null;
        }

        private static IObjectDefinitionDescription ProcessObjectDescription(dynamic description)
        {
            if (description == null)
            {
                return null;
            }

            string id = description.Id != null ? description.Id.ToString() : null;
            string name = description.Name != null ? description.Name.ToString() : null;
            string desc = description.Description != null ? description.Description.ToString() : null;
            string icon = description.Icon != null ? description.Icon.ToString() : null;
            return new ObjectDefinitionDescription(id, name, desc, icon, description);
        }
    }
}