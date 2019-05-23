namespace ModTechMaster.Data.Models.Mods.TypedObjectDefinitions
{
    using System.Collections.Generic;

    using ModTechMaster.Core.Constants;
    using ModTechMaster.Core.Enums.Mods;
    using ModTechMaster.Core.Interfaces.Models;

    using Newtonsoft.Json.Linq;

    public class SimGameConstantsObjectDefinition : ObjectDefinition
    {
        public SimGameConstantsObjectDefinition(
            ObjectType objectType,
            IObjectDefinitionDescription objectDescription,
            dynamic jsonObject,
            string filePath)
            : base(objectType, objectDescription, (JObject)jsonObject, filePath)
        {
            this.AddMetaData();
        }

        public override sealed void AddMetaData()
        {
            base.AddMetaData();
            var requiredAbilities = new List<string>();
            this.MetaData.Add(Keywords.AbilityDefId, requiredAbilities);

            if (this.JsonObject.Progression != null)
            {
                foreach (var skill in this.JsonObject.Progression)
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
            this.MetaData.Add(Keywords.MechDefId, requiredMechs);

            if (this.JsonObject?.Story?.StartingLance != null)
            {
                foreach (var mechDef in this.JsonObject.Story.StartingLance)
                {
                    requiredMechs.Add(mechDef.ToString());
                }
            }
        }
    }
}