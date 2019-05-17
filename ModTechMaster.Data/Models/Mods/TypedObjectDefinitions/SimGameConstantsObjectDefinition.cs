using System.Collections.Generic;
using ModTechMaster.Core.Constants;
using ModTechMaster.Core.Enums.Mods;
using ModTechMaster.Core.Interfaces.Models;
using Newtonsoft.Json.Linq;

namespace ModTechMaster.Data.Models.Mods.TypedObjectDefinitions
{
    public class SimGameConstantsObjectDefinition : ObjectDefinition
    {
        public SimGameConstantsObjectDefinition(ObjectType objectType, IObjectDefinitionDescription objectDescription,
            dynamic jsonObject, string filePath) : base(objectType, objectDescription, (JObject) jsonObject, filePath)
        {
            AddMetaData();
        }

        public sealed override void AddMetaData()
        {
            base.AddMetaData();
            var requiredAbilities = new List<string>();
            MetaData.Add(Keywords.AbilityDefId, requiredAbilities);

            if (JsonObject.Progression != null)
            {
                foreach (var skill in JsonObject.Progression)
                {
                    foreach (var level in skill)
                    {
                        foreach (var abilityList in level)
                        {
                            foreach (var ability in abilityList)
                            {
                                requiredAbilities.Add(ability.ToString());
                            }
                        }
                    }
                }
            }

            var requiredMechs = new List<string>();
            MetaData.Add(Keywords.MechDefId, requiredMechs);

            if (JsonObject?.Story?.StartingLance != null)
            {
                foreach (var mechDef in JsonObject.Story.StartingLance)
                {
                    requiredMechs.Add(mechDef.ToString());
                }
            }
        }
    }
}