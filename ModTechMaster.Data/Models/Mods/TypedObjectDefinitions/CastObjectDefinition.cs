namespace ModTechMaster.Data.Models.Mods.TypedObjectDefinitions
{
    using System.Collections.Generic;

    using ModTechMaster.Core.Constants;
    using ModTechMaster.Core.Enums.Mods;
    using ModTechMaster.Core.Interfaces.Models;
    using ModTechMaster.Core.Interfaces.Services;

    using Newtonsoft.Json.Linq;

    public class CastObjectDefinition : ObjectDefinition
    {
        public CastObjectDefinition(
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
            base.AddMetaData();
            var dynamicFactions = new List<string>()
                                      {
                                          "Faction_Neutral",
                                          "Faction_Employer",
                                          "Faction_EmployersAlly",
                                          "Faction_Hostile",
                                          "Faction_TargetsAlly",
                                          "Player1sMercUnit",
                                          "Faction_Target"
                                      };

            if (this.JsonObject?.faction != null && !dynamicFactions.Contains(this.JsonObject?.faction.ToString()))
            {
                this.MetaData.Add(Keywords.FactionId, this.JsonObject.faction);
            }
            this.MetaData.Add(Keywords.PortraitPath, this.JsonObject.defaultEmotePortrait.portraitAssetPath);
        }
    }
}