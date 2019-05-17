using System.Collections.Generic;
using ModTechMaster.Core.Constants;
using ModTechMaster.Core.Enums.Mods;
using ModTechMaster.Core.Interfaces.Models;
using Newtonsoft.Json.Linq;

namespace ModTechMaster.Data.Models.Mods.TypedObjectDefinitions
{
    public class MechObjectDefinition : ObjectDefinition
    {
        public MechObjectDefinition(ObjectType objectType, IObjectDefinitionDescription objectDescription,
            dynamic jsonObject, string filePath) : base(objectType, objectDescription, (JObject) jsonObject, filePath)
        {
        }

        public override void AddMetaData()
        {
            base.AddMetaData();
            var mechComponentIdList = new List<string>();
            MetaData.Add(Keywords.ComponentDefId, mechComponentIdList);
            foreach (var item in JsonObject.inventory) mechComponentIdList.Add(item.ComponentDefID.ToString());
            MetaData.Add(Keywords.ChassisId, JsonObject.ChassisID);
        }
    }
}