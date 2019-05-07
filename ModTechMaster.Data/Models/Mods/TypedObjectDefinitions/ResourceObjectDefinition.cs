namespace ModTechMaster.Data.Models.Mods.TypedObjectDefinitions
{
    using Core.Constants;
    using Core.Interfaces.Models;
    using Newtonsoft.Json.Linq;

    public class ResourceObjectDefinition : ObjectDefinition
    {
        public ResourceObjectDefinition(IObjectDefinitionDescription objectDescription, dynamic jsonObject, string filePath) : base(objectDescription, (JObject)jsonObject, filePath)
        {
        }

        public override void AddMetaData()
        {
            base.AddMetaData();
        }
    }
}