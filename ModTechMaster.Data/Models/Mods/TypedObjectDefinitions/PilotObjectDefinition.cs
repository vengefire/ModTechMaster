using System.Collections.Generic;
using ModTechMaster.Core.Constants;
using ModTechMaster.Core.Enums.Mods;
using ModTechMaster.Core.Interfaces.Models;
using Newtonsoft.Json.Linq;

namespace ModTechMaster.Data.Models.Mods.TypedObjectDefinitions
{
    public class PilotObjectDefinition : ObjectDefinition
    {
        public PilotObjectDefinition(ObjectType objectType, IObjectDefinitionDescription objectDescription,
            dynamic jsonObject, string filePath) : base(objectType, objectDescription, (JObject) jsonObject, filePath)
        {
        }

        public override void AddMetaData()
        {
            base.AddMetaData();
            var abilityDefs = new List<string>();
            MetaData.Add(Keywords.AbilityDefId, abilityDefs);
            foreach (var item in JsonObject.abilityDefNames) abilityDefs.Add(item.ToString());
        }
    }
}