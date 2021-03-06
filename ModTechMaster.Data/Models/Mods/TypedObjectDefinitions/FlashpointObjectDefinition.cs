﻿namespace ModTechMaster.Data.Models.Mods.TypedObjectDefinitions
{
    using ModTechMaster.Core.Constants;
    using ModTechMaster.Core.Enums.Mods;
    using ModTechMaster.Core.Interfaces.Models;
    using ModTechMaster.Core.Interfaces.Services;

    using Newtonsoft.Json.Linq;

    public class FlashpointObjectDefinition : ObjectDefinition
    {
        public FlashpointObjectDefinition(
            ObjectType objectType,
            IObjectDefinitionDescription objectDescription,
            dynamic jsonObject,
            string filePath,
            IReferenceFinderService referenceFinderService)
            : base(objectType, objectDescription, (JObject)jsonObject, filePath, referenceFinderService)
        {
        }

        public override void AddMetaData()
        {
            base.AddMetaData();
            this.MetaData.Add(Keywords.MilestoneSetId, this.JsonObject.MilestoneSetID);
            this.MetaData.Add(Keywords.ItemCollectionId, this.JsonObject.RewardCollectionID);
            this.MetaData.Add(Keywords.CastDefId, this.JsonObject.FlashpointDescriberCastDefId);
        }
    }
}