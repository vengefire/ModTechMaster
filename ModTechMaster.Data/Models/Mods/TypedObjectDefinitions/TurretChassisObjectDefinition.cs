namespace ModTechMaster.Data.Models.Mods.TypedObjectDefinitions
{
    using ModTechMaster.Core.Constants;
    using ModTechMaster.Core.Enums.Mods;
    using ModTechMaster.Core.Interfaces.Models;

    using Newtonsoft.Json.Linq;

    public class TurretChassisObjectDefinition : ObjectDefinition
    {
        public TurretChassisObjectDefinition(
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
            this.MetaData.Add(Keywords.HardpointDataDefId, this.JsonObject.HardpointDataDefID);
            this.MetaData.Add(Keywords.PrefabId, this.JsonObject.PrefabIdentifier);
        }
    }
}