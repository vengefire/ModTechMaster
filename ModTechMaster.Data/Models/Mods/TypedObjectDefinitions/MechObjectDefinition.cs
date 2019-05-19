namespace ModTechMaster.Data.Models.Mods.TypedObjectDefinitions
{
    using System.Collections.Generic;
    using Core.Constants;
    using Core.Enums.Mods;
    using Core.Interfaces.Models;
    using Newtonsoft.Json.Linq;

    public class MechObjectDefinition : ObjectDefinition
    {
        public MechObjectDefinition(
            ObjectType objectType, IObjectDefinitionDescription objectDescription,
            dynamic jsonObject, string filePath) : base(objectType, objectDescription, (JObject)jsonObject, filePath)
        {
        }

        public override void AddMetaData()
        {
            base.AddMetaData();
            var mechComponentIdList = new List<string>();
            this.MetaData.Add(Keywords.ComponentDefId, mechComponentIdList);
            foreach (var item in this.JsonObject.inventory) mechComponentIdList.Add(item.ComponentDefID.ToString());
            this.MetaData.Add(Keywords.ChassisId, this.JsonObject.ChassisID);
        }
    }
}