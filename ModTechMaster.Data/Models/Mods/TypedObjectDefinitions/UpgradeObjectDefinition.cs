﻿namespace ModTechMaster.Data.Models.Mods.TypedObjectDefinitions
{
    using ModTechMaster.Core.Constants;
    using ModTechMaster.Core.Enums.Mods;
    using ModTechMaster.Core.Interfaces.Models;

    using Newtonsoft.Json.Linq;

    public class UpgradeObjectDefinition : ObjectDefinition
    {
        public UpgradeObjectDefinition(
            ObjectType objectType,
            IObjectDefinitionDescription objectDescription,
            dynamic jsonObject,
            string filePath)
            : base(objectType, objectDescription, (JObject)jsonObject, filePath)
        {
        }

        public override void AddMetaData()
        {
            base.AddMetaData();
            if (this.JsonObject.Custom.Lootable?.ItemID != null)
            {
                this.MetaData.Add(Keywords.LootableId, this.JsonObject.Custom.Lootable?.ItemID);
            }
        }
    }
}