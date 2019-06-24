namespace ModTechMaster.Data.Models.Mods.TypedObjectDefinitions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using ModTechMaster.Core.Constants;
    using ModTechMaster.Core.Enums.Mods;
    using ModTechMaster.Core.Interfaces.Models;
    using ModTechMaster.Core.Interfaces.Services;

    using Newtonsoft.Json.Linq;

    public class LanceDefObjectDefinition : ObjectDefinition
    {
        private readonly IFactionService factionService;

        public LanceDefObjectDefinition(
            ObjectType objectType,
            IObjectDefinitionDescription objectDescription,
            dynamic jsonObject,
            string filePath,
            IReferenceFinderService referenceFinderService,
            IFactionService factionService)
            : base(objectType, objectDescription, (JObject)jsonObject, filePath, referenceFinderService)
        {
            this.factionService = factionService;
        }

        public int Difficulty { get; set; }

        public List<LanceSlotDefinition> LanceSlots { get; } = new List<LanceSlotDefinition>();

        public List<string> LanceTags => this.Tags[Keywords.MyTags];

        public override void AddMetaData()
        {
            base.AddMetaData();
            this.Difficulty = Convert.ToInt32(this.JsonObject.Difficulty.ToString());

            var i = 0;
            foreach (var lanceUnit in this.JsonObject.LanceUnits)
            {
                var slot = new LanceSlotDefinition(
                    ObjectType.LanceSlotDef,
                    null,
                    lanceUnit,
                    this.SourceFilePath,
                    i++,
                    this.ReferenceFinderService,
                    this,
                    this.factionService);
                slot.AddMetaData();
                this.LanceSlots.Add(slot);
            }

            var slotMetaKeys = new List<string>
                                   {
                                       Keywords.MechDefId, Keywords.TurretId, Keywords.VehicleId, Keywords.PilotId
                                   };

            var slotMetas = this.LanceSlots.SelectMany(definition => definition.MetaData).ToList()
                .Where(pair => slotMetaKeys.Contains(pair.Key)).Select(pair => pair).ToList();
            slotMetas.ForEach(
                pair =>
                    {
                        if (!this.MetaData.ContainsKey(pair.Key))
                        {
                            this.MetaData.Add(pair.Key, new List<string>(new string[] { pair.Value.ToString() }));
                        }
                        else
                        {
                            this.MetaData[pair.Key].Add(pair.Value.ToString());
                        }
                    });
        }

        public override IValidationResult ValidateObject()
        {
            return ValidationResult.AggregateResults(this.LanceSlots.Select(definition => definition.ValidateObject()));
        }
    }
}