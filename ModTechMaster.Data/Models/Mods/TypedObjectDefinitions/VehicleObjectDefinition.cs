using System.Collections.Generic;
using ModTechMaster.Core.Constants;
using ModTechMaster.Core.Enums.Mods;
using ModTechMaster.Core.Interfaces.Models;
using Newtonsoft.Json.Linq;

namespace ModTechMaster.Data.Models.Mods.TypedObjectDefinitions
{
    public class VehicleObjectDefinition : ObjectDefinition
    {
        public VehicleObjectDefinition(ObjectType objectType, IObjectDefinitionDescription objectDescription,
            dynamic jsonObject, string filePath) : base(objectType, objectDescription, (JObject) jsonObject, filePath)
        {
        }

        public override void AddMetaData()
        {
            base.AddMetaData();
            var vehicleComponentIdList = new List<string>();
            MetaData.Add(Keywords.ComponentDefId, vehicleComponentIdList);
            foreach (var item in JsonObject.inventory) vehicleComponentIdList.Add(item.ComponentDefID.ToString());
            MetaData.Add(Keywords.ChassisId, JsonObject.ChassisID);
        }
    }
}