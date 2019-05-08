using ModTechMaster.Core.Enums.Mods;

namespace ModTechMaster.Data.Models.Mods.TypedObjectDefinitions
{
    using Core.Constants;
    using Core.Interfaces.Models;
    using Newtonsoft.Json.Linq;

    public class AssetBundleObjectDefinition : ObjectDefinition
    {
        public AssetBundleObjectDefinition(ObjectType objectType, IObjectDefinitionDescription objectDescription, dynamic jsonObject, string filePath) : base(objectType, objectDescription, (JObject)jsonObject, filePath)
        {
        }

        public override void AddMetaData()
        {
            base.AddMetaData();
        }
    }
}