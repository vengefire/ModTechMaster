namespace ModTechMaster.Data.Models.Mods.TypedObjectDefinitions
{
    using Core.Constants;
    using Core.Enums.Mods;
    using Core.Interfaces.Models;
    using Newtonsoft.Json.Linq;

    public class CcCategoryObjectDefinition : ObjectDefinition
    {
        public CcCategoryObjectDefinition(
            ObjectType objectType, IObjectDefinitionDescription objectDescription,
            dynamic jsonObject, string filePath) : base(objectType, objectDescription, (JObject)jsonObject, filePath)
        {
        }

        public override void AddMetaData()
        {
            base.AddMetaData();
        }
    }
}