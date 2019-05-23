namespace ModTechMaster.Data.Models.Mods.TypedObjectDefinitions
{
    using System.Collections.Generic;

    using ModTechMaster.Core.Constants;
    using ModTechMaster.Core.Enums.Mods;
    using ModTechMaster.Core.Interfaces.Models;

    using Newtonsoft.Json.Linq;

    public class FactionObjectDefinition : ObjectDefinition
    {
        public FactionObjectDefinition(
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
            this.MetaData.Add(Keywords.FactionId, this.JsonObject.Faction);
            this.MetaData.Add(Keywords.CombatLeaderCastDefId, this.JsonObject.DefaultCombatLeaderCastDefId);
            this.MetaData.Add(Keywords.RepresentativeCastDefId, this.JsonObject.DefaultRepresentativeCastDefId);
            this.MetaData.Add(Keywords.HeraldryDefId, this.JsonObject.HeraldryDefId);

            var enemies = new List<string>();
            foreach (var enemy in this.JsonObject.Enemies)
            {
                enemies.Add(enemy as string);
            }

            this.MetaData.Add(Keywords.EnemyFactionId, new List<string>(enemies));

            var allies = new List<string>();
            foreach (var ally in this.JsonObject.Allies)
            {
                allies.Add(ally as string);
            }

            this.MetaData.Add(Keywords.AlliedFactionId, new List<string>(allies));
        }
    }
}