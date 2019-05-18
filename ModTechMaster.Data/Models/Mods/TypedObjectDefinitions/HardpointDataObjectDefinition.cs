using ModTechMaster.Core.Constants;
using ModTechMaster.Core.Enums.Mods;
using ModTechMaster.Core.Interfaces.Models;
using Newtonsoft.Json.Linq;

namespace ModTechMaster.Data.Models.Mods.TypedObjectDefinitions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class HardpointDataObjectDefinition : ObjectDefinition
    {
        public HardpointDataObjectDefinition(ObjectType objectType, IObjectDefinitionDescription objectDescription,
            dynamic jsonObject, string filePath) : base(objectType, objectDescription, (JObject) jsonObject, filePath)
        {
        }

        public override void AddMetaData()
        {
            base.AddMetaData();
            var prefabWeapons = new HashSet<string>();
            foreach (var data in this.JsonObject.HardpointData)
            {
                void RecurseWeapons(dynamic item, HashSet<string> set)
                {
                    if (item is JArray)
                    {
                        foreach (var subItem in item)
                        {
                            RecurseWeapons(subItem, set);
                        }
                    }
                    else
                    {
                        set.Add(item.ToString());
                    }
                }
                RecurseWeapons(data.weapons, prefabWeapons);
            }
            this.MetaData.Add(Keywords.PrefabId, prefabWeapons);
        }
    }
}