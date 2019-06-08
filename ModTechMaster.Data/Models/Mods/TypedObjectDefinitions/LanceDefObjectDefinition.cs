namespace ModTechMaster.Data.Models.Mods.TypedObjectDefinitions
{
    using System;
    using System.Collections.Generic;

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

        public override void AddMetaData()
        {
            base.AddMetaData();

            var unitTagSet = new List<string>();
            var excludeUnitTagSet = new List<string>();
            var pilotTagSet = new List<string>();
            var excludedPilotTagSet = new List<string>();

            var mechIds = new List<string>();
            var turretIds = new List<string>();
            var vehicleIds = new List<string>();
            var pilotIds = new List<string>();

            foreach (var lanceUnit in this.JsonObject.LanceUnits)
            {
                var unitType = lanceUnit.unitType.ToString();
                var unitId = lanceUnit.unitId.ToString();
                var pilotId = lanceUnit.pilotId.ToString();

                if (unitId != "Tagged")
                {
                    switch (unitType)
                    {
                        case "Mech":
                            mechIds.Add(unitId);
                            break;
                        case "Turret":
                            turretIds.Add(unitId);
                            break;
                        case "Vehicle":
                            vehicleIds.Add(unitId);
                            break;
                        case "UNDEFINED":
                            throw new InvalidProgramException();
                            vehicleIds.Add(unitId);
                            break;
                    }
                }

                if (pilotId != "Tagged")
                {
                    pilotIds.Add(pilotId);
                }

                if (lanceUnit.unitTagSet?.items != null)
                {
                    foreach (var tagItem in lanceUnit.unitTagSet.items)
                    {
                        unitTagSet.Add(tagItem.ToString());
                    }
                }

                if (lanceUnit.excludedUnitTagSet?.items != null)
                {
                    foreach (var tagItem in lanceUnit.excludedUnitTagSet.items)
                    {
                        excludeUnitTagSet.Add(tagItem.ToString());
                    }
                }

                if (lanceUnit.pilotTagSet?.items != null)
                {
                    foreach (var tagItem in lanceUnit.pilotTagSet.items)
                    {
                        pilotTagSet.Add(tagItem.ToString());
                    }
                }

                if (lanceUnit.excludedPilotTagSet?.items != null)
                {
                    foreach (var tagItem in lanceUnit.excludedPilotTagSet.items)
                    {
                        excludedPilotTagSet.Add(tagItem.ToString());
                    }
                }
            }

            this.Tags.Add(Keywords.UnitTags, unitTagSet);
            this.Tags.Add(Keywords.ExcludedUnitTags, excludeUnitTagSet);
            this.Tags.Add(Keywords.PilotTags, pilotTagSet);
            this.Tags.Add(Keywords.ExcludedPilotTags, excludedPilotTagSet);

            this.MetaData.Add(Keywords.MechDefId, mechIds);
            this.MetaData.Add(Keywords.TurretId, turretIds);
            this.MetaData.Add(Keywords.VehicleId, vehicleIds);
            this.MetaData.Add(Keywords.PilotId, pilotIds);
        }
    }
}