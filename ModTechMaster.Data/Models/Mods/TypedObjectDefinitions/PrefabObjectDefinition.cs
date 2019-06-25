namespace ModTechMaster.Data.Models.Mods.TypedObjectDefinitions
{
    using ModTechMaster.Core.Constants;
    using ModTechMaster.Core.Enums.Mods;
    using ModTechMaster.Core.Interfaces.Models;
    using ModTechMaster.Core.Interfaces.Services;

    using Newtonsoft.Json.Linq;

    public class PrefabObjectDefinition : ObjectDefinition
    {
        public PrefabObjectDefinition(
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
            // We us the asset bundle name as the ID so chassis defs can link to all the required prefabs, which will link to the required asset bundle.
            var assetBundleName = this.JsonObject.AssetBundleName.ToString();
            this.MetaData.Add(Keywords.Id, this.SourceFileName);
            this.MetaData.Add(Keywords.Name, this.SourceFileName);
            base.AddMetaData();
            this.MetaData.Add(Keywords.AssetBundle, assetBundleName);
        }
    }
}