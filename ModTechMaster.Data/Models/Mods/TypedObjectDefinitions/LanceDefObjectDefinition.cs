namespace ModTechMaster.Data.Models.Mods.TypedObjectDefinitions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using ModTechMaster.Core.Constants;
    using ModTechMaster.Core.Enums.Mods;
    using ModTechMaster.Core.Interfaces.Models;

    using Newtonsoft.Json.Linq;

    public class LanceDefObjectDefinition : ObjectDefinition
    {
        public LanceDefObjectDefinition(
            ObjectType objectType,
            IObjectDefinitionDescription objectDescription,
            dynamic jsonObject,
            string filePath)
            : base(objectType, objectDescription, (JObject)jsonObject, filePath)
        {
        }

        public List<LanceSlotDefinition> LanceSlots { get; } = new List<LanceSlotDefinition>();

        public int Difficulty { get; set; }

        public List<string> LanceTags => this.Tags[Keywords.MyTags];

        public override void AddMetaData()
        {
            base.AddMetaData();
            this.Difficulty = Convert.ToInt32(this.JsonObject.Difficulty.ToString());

            var i = 0;
            foreach (var lanceUnit in this.JsonObject.LanceUnits)
            {
                var slot = new LanceSlotDefinition(ObjectType.LanceSlotDef, null, lanceUnit, this.SourceFilePath, i++);
                slot.AddMetaData();
                this.LanceSlots.Add(slot);
            }

            var slotMetaKeys = new List<string>
                                   {
                                       Keywords.MechDefId, Keywords.TurretId, Keywords.VehicleId, Keywords.PilotId
                                   };

            var slotMetas = this.LanceSlots.SelectMany(definition => definition.MetaData).ToList()
                .Where(pair => slotMetaKeys.Contains(pair.Key)).Select(pair => pair).ToList();
            slotMetas.ForEach(pair => { this.MetaData.Add(pair.Key, pair.Value); });
        }
    }
}