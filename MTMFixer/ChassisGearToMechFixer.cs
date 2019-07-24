namespace MTMFixer
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    internal class ChassisGearToMechFixer
    {
        public void MoveFixedEquipmentFromChassisToMechDef(string modCollectionPath, List<string> typesToMove)
        {
            var chassisDefinitionFiles = Directory.GetFiles(modCollectionPath, "chassisdef*.json", SearchOption.AllDirectories).Select(
                s =>
                {
                    var jsonObject = JsonHelper.DeserializeJson(s);
                    if (jsonObject.ContainsKey("FixedEquipment"))
                    {
                        var fi = new FileInfo(s);
                        return new { Id = fi.Name.Remove(fi.Name.Length - fi.Extension.Length), FilePath = s, ChassisDefinitionObject = jsonObject };
                    }

                    return null;
                }).Where(arg => arg != null).ToList();
            var chassisFilesById = chassisDefinitionFiles.GroupBy(s => s.Id, s => s, (s, enumerable) => new { Id = s, data = enumerable });

            var mechDefinitionFiles = Directory.GetFiles(modCollectionPath, "mechdef*.json", SearchOption.AllDirectories).Select(
                s =>
                {
                    var jsonObject = JsonHelper.DeserializeJson(s);
                    if (jsonObject.ContainsKey("ChassisID"))
                    {
                        var linkedChassisId = jsonObject["ChassisID"].ToString();
                        if (chassisFilesById.Any(arg => arg.Id == linkedChassisId))
                        {
                            var fi = new FileInfo(s);
                            return new { Id = fi.Name.Remove(fi.Name.Length - fi.Extension.Length), FilePath = s, ChassisId = linkedChassisId, mechDefinitionObject = jsonObject };
                        }
                    }

                    return null;
                }).Where(arg => arg != null).ToList();
            var mechFilesById = mechDefinitionFiles.GroupBy(arg => arg.Id, arg => arg, (s, enumerable) => new { Id = s, data = enumerable });

            /*
            chassisDefinitionFiles.ForEach(
                chassisDefinitionFile =>
                {
                    var chassisObject = chassisDefinitionFile.jsonObject;
                    if (chassisObject.ContainsKey("FixedEquipment"))
                    {
                        var chassisId = string.Empty;
                        var fixedEquipmentToMove = new List<JToken>();
                        var fixedEquipmentArray = (JArray)chassisObject["FixedEquipment"];
                        foreach (var componentDef in fixedEquipmentArray)
                        {
                            var componentDefId = componentDef["ComponentDefID"].ToString();
                            if (typesToMove.Any(typeToMove => componentDefId.ToLower().Contains(typeToMove)))
                            {
                                chassisId = chassisObject["Description"]["Id"].ToString();
                                fixedEquipmentToMove.Add(componentDef);
                                Console.WriteLine($"[{chassisId}] - Found [{componentDefId}].");
                            }
                        }

                        if (!fixedEquipmentToMove.Any())
                        {
                            return;
                        }

                        Console.WriteLine($"Finding Mech Definitions that use chassisId [{chassisId}]");
                        var linkedMechDefs = new List<Tuple<string, JObject>>();
                        mechDefinitionFiles.ForEach(
                            mechDefinitionFile =>
                            {
                                var mechDef = JsonHelper.DeserializeJson(mechDefinitionFile);
                                if (mechDef.ContainsKey("ChassisID"))
                                {
                                    var linkedChassisId = mechDef["ChassisID"].ToString().ToLower();
                                    if (linkedChassisId == chassisId.ToLower())
                                    {
                                        Console.WriteLine($"Found linked mech definition, File = [{mechDefinitionFile}], ID = [{mechDef["Description"]["Id"]}], UIName == [{mechDef["Description"]["UIName"]}]");
                                        linkedMechDefs.Add(new Tuple<string, JObject>(mechDefinitionFile, mechDef));
                                    }
                                }
                            });

                        Console.WriteLine($"Moving fixed equipment from chassis [{chassisId}] to [{linkedMechDefs.Count}] mechDefs...");

                        fixedEquipmentToMove.ForEach(
                            componentDef =>
                            {
                                linkedMechDefs.ForEach(
                                    tuple =>
                                    {
                                        var mechDef = tuple.Item2;
                                        Console.WriteLine($"Moving component [{componentDef["ComponentDefID"]}] to [{mechDef["Description"]["Id"]}]");
                                        var inventory = (JArray)mechDef["inventory"];
                                        inventory.Add(componentDef);
                                    });
                                Console.WriteLine($"Removing component [{componentDef["ComponentDefId"]}] from chassis [{chassisId}]...");
                                var fixedEquipment = (JArray)chassisObject["FixedEquipment"];
                                fixedEquipment.Remove(componentDef);
                            });

                        Console.WriteLine("Persisting changes...");
                    }
                });*/
        }
    }
}