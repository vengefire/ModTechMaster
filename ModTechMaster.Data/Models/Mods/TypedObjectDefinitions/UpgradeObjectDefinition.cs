using System.Collections.Generic;
using ModTechMaster.Core.Constants;
using ModTechMaster.Core.Enums.Mods;
using ModTechMaster.Core.Interfaces.Models;
using Newtonsoft.Json.Linq;

namespace ModTechMaster.Data.Models.Mods.TypedObjectDefinitions
{
    public class UpgradeObjectDefinition : ObjectDefinition
    {
        public UpgradeObjectDefinition(ObjectType objectType, IObjectDefinitionDescription objectDescription,
            dynamic jsonObject, string filePath) : base(objectType, objectDescription, (JObject) jsonObject, filePath)
        {
        }

        public override void AddMetaData()
        {
            base.AddMetaData();
            if (JsonObject.Custom.Lootable?.ItemID != null)
            {
                MetaData.Add(Keywords.LootableId, JsonObject.Custom.Lootable?.ItemID);
            }
        }
    }
}