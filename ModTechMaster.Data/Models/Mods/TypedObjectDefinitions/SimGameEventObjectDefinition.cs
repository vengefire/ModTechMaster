namespace ModTechMaster.Data.Models.Mods.TypedObjectDefinitions
{
    using System.Collections.Generic;

    using ModTechMaster.Core.Constants;
    using ModTechMaster.Core.Enums.Mods;
    using ModTechMaster.Core.Interfaces.Models;

    using Newtonsoft.Json.Linq;

    public class SimGameEventObjectDefinition : ObjectDefinition
    {
        public SimGameEventObjectDefinition(
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
            var requiredTags = new List<string>();
            this.MetaData.Add(Keywords.RequiredTags, requiredTags);
            foreach (var req in this.JsonObject.Requirements.RequirementTags.items)
            {
                requiredTags.Add(req.ToString());
            }
        }
    }
}