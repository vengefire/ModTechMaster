using System.Collections.Generic;
using ModTechMaster.Core.Constants;
using ModTechMaster.Core.Enums.Mods;
using ModTechMaster.Core.Interfaces.Models;
using Newtonsoft.Json.Linq;

namespace ModTechMaster.Data.Models.Mods.TypedObjectDefinitions
{
    public class TurretChassisObjectDefinition : ObjectDefinition
    {
        public TurretChassisObjectDefinition(ObjectType objectType, IObjectDefinitionDescription objectDescription,
            dynamic jsonObject, string filePath) : base(objectType, objectDescription, (JObject) jsonObject, filePath)
        {
        }

        public override void AddMetaData()
        {
            base.AddMetaData();
            MetaData.Add(Keywords.HardpointDataDefId, JsonObject.HardpointDataDefID);
            MetaData.Add(Keywords.PrefabId, JsonObject.PrefabIdentifier);
        }
    }
}