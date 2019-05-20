﻿using System.Collections.Generic;
using ModTechMaster.Core.Constants;
using ModTechMaster.Core.Enums.Mods;
using ModTechMaster.Core.Interfaces.Models;
using Newtonsoft.Json.Linq;

namespace ModTechMaster.Data.Models.Mods.TypedObjectDefinitions
{
    public class VehicleChassisObjectDefinition : ObjectDefinition
    {
        public VehicleChassisObjectDefinition(ObjectType objectType, IObjectDefinitionDescription objectDescription,
            dynamic jsonObject, string filePath) : base(objectType, objectDescription, (JObject) jsonObject, filePath)
        {
        }

        public override void AddMetaData()
        {
            base.AddMetaData();
            MetaData.Add(Keywords.MovementCapDefId, JsonObject.MovementCapDefID);
            MetaData.Add(Keywords.PathingCapDefId, JsonObject.PathingCapDefID);
            MetaData.Add(Keywords.HardpointDataDefId, JsonObject.HardpointDataDefID);
            MetaData.Add(Keywords.PrefabId, JsonObject.PrefabIdentifier);
        }
    }
}