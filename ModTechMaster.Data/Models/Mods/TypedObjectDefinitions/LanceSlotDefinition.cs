namespace ModTechMaster.Data.Models.Mods.TypedObjectDefinitions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Framework.Utils.Extensions.Json;

    using ModTechMaster.Core.Constants;
    using ModTechMaster.Core.Enums;
    using ModTechMaster.Core.Enums.Mods;
    using ModTechMaster.Core.Interfaces.Models;
    using ModTechMaster.Core.Interfaces.Services;

    using Newtonsoft.Json.Linq;

    public class LanceSlotDefinition : ObjectDefinition
    {
        private readonly IFactionService factionService;

        private readonly Dictionary<string, int> factionUnitDictionary = new Dictionary<string, int>();

        public LanceSlotDefinition(
            ObjectType objectType,
            IObjectDefinitionDescription objectDescription,
            dynamic jsonObject,
            string filePath,
            long lanceSlotNumber,
            IReferenceFinderService referenceFinderService,
            LanceDefObjectDefinition lanceDefObjectDefinition,
            IFactionService factionService)
            : base(objectType, objectDescription, jsonObject as JObject, filePath, referenceFinderService)
        {
            this.LanceSlotNumber = lanceSlotNumber;
            this.LanceDefObjectDefinition = lanceDefObjectDefinition;
            this.factionService = factionService;
            this.factionService.GetFactions().ForEach(faction => this.factionUnitDictionary.Add(faction, 0));
        }

        public List<string> EligibleFactions =>
            this.factionUnitDictionary.Where(pair => pair.Value > 0).Select(pair => pair.Key).ToList();

        public List<PilotObjectDefinition> EligiblePilots { get; set; }

        public List<ObjectDefinition> EligibleUnits { get; set; }

        public List<string> ExcludedPilotTags { get; } = new List<string>();

        public List<string> ExcludedUnitTags { get; } = new List<string>();

        public List<string> IneligibleFactions =>
            this.RestrictByFaction
                ? this.factionUnitDictionary.Where(pair => pair.Value <= 0).Select(pair => pair.Key).ToList()
                : new List<string>();

        public LanceDefObjectDefinition LanceDefObjectDefinition { get; }

        public long LanceSlotNumber { get; }

        public ObjectType LanceSlotObjectType { get; set; }

        public string PilotId { get; set; }

        public List<string> PilotTags { get; } = new List<string>();

        public bool RestrictByFaction { get; set; }

        public string UnitId { get; set; }

        public List<string> UnitTags { get; } = new List<string>();

        public override void AddMetaData()
        {
            // Hacky, need to fix this shit up. If only this was my day job.
            this.MetaData.Add(Keywords.Id, $"{this.Id} - Slot {this.LanceSlotNumber}");
            this.MetaData.Add(Keywords.Name, $"{this.Id} - Slot {this.LanceSlotNumber}");

            base.AddMetaData();

            var lanceUnit = this.JsonObject;

            var unitType = lanceUnit.unitType.ToString();
            var unitId = lanceUnit.unitId.ToString();
            var pilotId = lanceUnit.pilotId.ToString();

            var idKeyword = string.Empty;

            switch (unitType)
            {
                case "Mech":
                    this.LanceSlotObjectType = ObjectType.MechDef;
                    idKeyword = Keywords.MechDefId;
                    break;
                case "Turret":
                    this.LanceSlotObjectType = ObjectType.TurretDef;
                    idKeyword = Keywords.TurretId;
                    break;
                case "Vehicle":
                    this.LanceSlotObjectType = ObjectType.VehicleDef;
                    idKeyword = Keywords.VehicleId;
                    break;
                case "UNDEFINED":
                    throw new InvalidProgramException();
            }

            if (unitId != "Tagged")
            {
                this.MetaData.Add(idKeyword, unitId);
            }

            this.UnitId = unitId;
            this.PilotId = pilotId;
            if (this.PilotId != "Tagged")
            {
                this.MetaData.Add(Keywords.PilotId, this.UnitId);
            }

            bool CheckTagSet(dynamic val)
            {
                if (val is JValue jVal && !jVal.IsNullOrEmpty())
                {
                    return true;
                }
                else if (val is JToken jTok && !jTok.IsNullOrEmpty())
                {
                    return true;
                }

                return false;
            }

            if (CheckTagSet(lanceUnit.unitTagSet) && lanceUnit.unitTagSet?.items != null)
            {
                foreach (var tagItem in lanceUnit.unitTagSet.items)
                {
                    var tag = tagItem.ToString();
                    if (tag.Contains("{CUR_TEAM"))
                    {
                        this.RestrictByFaction = true;
                    }

                    this.UnitTags.Add(tag);
                }
            }

            if (CheckTagSet(lanceUnit.excludedUnitTagSet) && lanceUnit.excludedUnitTagSet?.items != null)
            {
                foreach (var tagItem in lanceUnit.excludedUnitTagSet.items)
                {
                    var tag = tagItem.ToString();
                    if (tag.Contains("{CUR_TEAM"))
                    {
                        continue;
                    }

                    this.ExcludedUnitTags.Add(tag);
                }
            }

            if (CheckTagSet(lanceUnit.pilotTagSet) && lanceUnit.pilotTagSet?.items != null)
            {
                foreach (var tagItem in lanceUnit.pilotTagSet.items)
                {
                    var tag = tagItem.ToString();
                    if (tag.Contains("{CUR_TEAM"))
                    {
                        continue;
                    }

                    this.PilotTags.Add(tag);
                }
            }

            if (CheckTagSet(lanceUnit.excludedPilotTagSet) && lanceUnit.excludedPilotTagSet?.items != null)
            {
                foreach (var tagItem in lanceUnit.excludedPilotTagSet.items)
                {
                    var tag = tagItem.ToString();
                    if (tag.Contains("{CUR_TEAM"))
                    {
                        continue;
                    }

                    this.ExcludedPilotTags.Add(tag);
                }
            }

            this.Tags.Add(Keywords.UnitTags, this.UnitTags);
            this.Tags.Add(Keywords.ExcludedUnitTags, this.ExcludedUnitTags);
            this.Tags.Add(Keywords.PilotTags, this.PilotTags);
            this.Tags.Add(Keywords.ExcludedPilotTags, this.ExcludedPilotTags);
        }

        public void LoadEligibleUnitsAndPilots(IReferenceableObjectProvider objectProvider)
        {
            this.EligibleUnits = this.GetEligibleUnits(objectProvider);
            this.EligiblePilots = this.GetEligiblePilots(objectProvider);
        }

        private List<PilotObjectDefinition> GetEligiblePilots(IReferenceableObjectProvider objectProvider)
        {
            var candidates = objectProvider.GetReferenceableObjects().Where(o => o.ObjectType == ObjectType.PilotDef && o.Tags.ContainsKey(Keywords.MyTags));

            var eligible = candidates.Where(
                o => !this.PilotTags.Any() || o.Tags.ContainsKey(Keywords.MyTags)
                     && !this.PilotTags.Except(o.Tags[Keywords.MyTags]).Any());
            var filteredEligible = eligible.Where(
                o => !o.Tags[Keywords.MyTags].Any(s => this.ExcludedPilotTags.Contains(s)));
            return filteredEligible.Cast<PilotObjectDefinition>().ToList();
        }

        private List<ObjectDefinition> GetEligibleUnits(IReferenceableObjectProvider objectProvider)
        {
            var validUnitObjectTypes =
                new List<ObjectType> { ObjectType.MechDef, ObjectType.TurretDef, ObjectType.VehicleDef };

            var candidates = objectProvider.GetReferenceableObjects()
                .Where(o => validUnitObjectTypes.Contains(o.ObjectType));

            var eligibleUnits = candidates.Where(
                o => !this.UnitTags.Except(new[] { "{CUR_TEAM.faction}" }).Except(o.Tags[Keywords.MyTags]).Any());

            var filteredEligibleUnits = eligibleUnits.Where(
                o => !o.Tags[Keywords.MyTags].Any(s => this.ExcludedUnitTags.Contains(s))).ToList();

            if (this.RestrictByFaction)
            {
                filteredEligibleUnits.AsParallel().ForAll(
                    // filteredEligibleUnits.ToList().ForEach(
                    unit =>
                        {
                            var unitTags = unit.Tags[Keywords.MyTags];
                            var factionIntersect = unitTags.Intersect(this.factionService.GetFactions()).ToList();
                            factionIntersect.ForEach(faction => this.factionUnitDictionary[faction] += 1);
                        });
            }

            return filteredEligibleUnits.Cast<ObjectDefinition>().ToList();
        }

        public override IValidationResult ValidateObject()
        {
            var result = ValidationResult.SuccessValidationResult();

            if (!this.EligiblePilots.Any())
            {
                result.Result = ValidationResultEnum.Failure;
                result.ValidationResultReasons.Add(new ValidationResultReason(this, $"No eligible pilots."));
            }

            if (!this.EligibleUnits.Any())
            {
                result.Result = ValidationResultEnum.Failure;
                result.ValidationResultReasons.Add(new ValidationResultReason(this, $"No eligible units."));

                if (this.RestrictByFaction)
                {
                    result.ValidationResultReasons.Add(new ValidationResultReason(this, $"Ineligible factions: [{string.Join(", ", this.IneligibleFactions)}] "));
                }
            }

            return result;
        }
    }
}