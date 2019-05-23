namespace ModTechMaster.Data.Models.Mods.TypedObjectDefinitions
{
    using ModTechMaster.Core.Constants;
    using ModTechMaster.Core.Enums.Mods;
    using ModTechMaster.Core.Interfaces.Models;

    using Newtonsoft.Json.Linq;

    public class HeraldryObjectDefinition : ObjectDefinition
    {
        public HeraldryObjectDefinition(
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
            this.MetaData.Add(Keywords.TextureId, this.JsonObject.textureLogoID);
        }
    }
}