namespace ModTechMaster.Data.Models.Mods.TypedObjectDefinitions
{
    using System.Collections.Generic;

    using ModTechMaster.Core.Constants;
    using ModTechMaster.Core.Enums.Mods;
    using ModTechMaster.Core.Interfaces.Models;
    using ModTechMaster.Core.Interfaces.Services;

    using Newtonsoft.Json.Linq;

    public class FactionObjectDefinition : ObjectDefinition
    {
        public FactionObjectDefinition(
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
            var jobject = (JObject)this.JsonObject;


            this.MetaData.Add(Keywords.FactionId, this.JsonObject.Faction != null ? this.JsonObject.Faction : this.JsonObject.ID.ToString().Replace("faction_", string.Empty));

            if (this.JsonObject?.DefaultCombatLeaderCastDefId != null && this.JsonObject.DefaultCombatLeaderCastDefId.ToString() != "castDef_None")
            {
                this.MetaData.Add(Keywords.CombatLeaderCastDefId, this.JsonObject.DefaultCombatLeaderCastDefId);
            }

            this.MetaData.Add(Keywords.RepresentativeCastDefId, this.JsonObject.DefaultRepresentativeCastDefId);
            if (this.JsonObject.HeraldryDefId != null && this.JsonObject.HeraldryDefId.ToString() != "heraldrydef_random")
            {
                this.MetaData.Add(Keywords.HeraldryDefId, this.JsonObject.HeraldryDefId);
            }

            var enemies = new List<string>();
            foreach (var enemy in this.JsonObject.Enemies)
            {
                enemies.Add(enemy.ToString());
            }

            this.MetaData.Add(Keywords.EnemyFactionId, new List<string>(enemies));

            var allies = new List<string>();
            foreach (var ally in this.JsonObject.Allies)
            {
                allies.Add(ally.ToString());
            }

            this.MetaData.Add(Keywords.AlliedFactionId, new List<string>(allies));
        }
    }
}