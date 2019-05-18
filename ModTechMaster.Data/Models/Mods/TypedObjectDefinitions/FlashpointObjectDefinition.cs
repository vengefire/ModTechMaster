namespace ModTechMaster.Data.Models.Mods.TypedObjectDefinitions
{
    using Core.Constants;
    using Core.Enums.Mods;
    using Core.Interfaces.Models;
    using Newtonsoft.Json.Linq;

    public class FlashpointObjectDefinition : ObjectDefinition
    {
        public FlashpointObjectDefinition(
            ObjectType objectType, IObjectDefinitionDescription objectDescription,
            dynamic jsonObject, string filePath) : base(objectType, objectDescription, (JObject)jsonObject, filePath)
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