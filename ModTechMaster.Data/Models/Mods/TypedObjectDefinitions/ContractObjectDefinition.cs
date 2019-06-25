namespace ModTechMaster.Data.Models.Mods.TypedObjectDefinitions
{
    using System;
    using System.Collections.Generic;

    using ModTechMaster.Core.Constants;
    using ModTechMaster.Core.Enums.Mods;
    using ModTechMaster.Core.Interfaces.Models;
    using ModTechMaster.Core.Interfaces.Services;

    using Newtonsoft.Json.Linq;

    public class ContractObjectDefinition : ObjectDefinition
    {
        public ContractObjectDefinition(
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

            var dynamicIdsList = new List<string>()
                                     {
                                         "UseLance",
                                         "mechDef_InheritLance",
                                         "pilotDef_InheritLance",
                                         "vehicleDef_InheritLance",
                                         "Tagged",
                                         "mechDef_None",
                                         "turretDef_InheritLance",
                                         "castDef_None"
                                     };

            var castList = new HashSet<string>();
            foreach (var dialog in this.JsonObject.dialogueList)
            {
                foreach (var dialogueContent in dialog.dialogueContent)
                {
                    if (!dynamicIdsList.Contains(dialogueContent.selectedCastDefId.ToString()))
                    {
                        castList.Add(dialogueContent.selectedCastDefId.ToString());
                    }
                }
            }

            this.MetaData.Add(Keywords.CastDefId, new List<string>(castList));

            // castDef_TeamLeader_Current?
            // selectedLanceDefId
            // pilotDefId
            // lanceOverrideList
            if (this.JsonObject.overrideAutoCompleteDialogueId != null)
            {
                this.MetaData.Add(Keywords.DialogBucketId, this.JsonObject.overrideAutoCompleteDialogueId);
            }

            var lanceDefs = new HashSet<string>();
            var heraldryDefs = new HashSet<string>();
            var mechDefs = new HashSet<string>();
            var pilotDefs = new HashSet<string>();
            var turretDefs = new HashSet<string>();
            var vehicleDefs = new HashSet<string>();
            foreach (var lanceOverride in this.JsonObject.targetTeam.lanceOverrideList)
            {
                if (lanceOverride.selectedLanceDefId != null)
                {
                    lanceDefs.Add(lanceOverride.selectedLanceDefId.ToString());
                }

                foreach (var unitSpawn in lanceOverride.unitSpawnPointOverrideList)
                {
                    if (unitSpawn.customHeraldryDefId != null)
                    {
                        heraldryDefs.Add(unitSpawn.customHeraldryDefId.ToString());
                    }

                    if (unitSpawn.unitType == "Mech" && !dynamicIdsList.Contains(unitSpawn.unitDefId.ToString()))
                    {
                        mechDefs.Add(unitSpawn.unitDefId.ToString());
                    }
                    else if (unitSpawn.unitType == "Turret" && !dynamicIdsList.Contains(unitSpawn.unitDefId.ToString()))
                    {
                        turretDefs.Add(unitSpawn.unitDefId.ToString());
                    }
                    else if (unitSpawn.unitType == "Vehicle" && !dynamicIdsList.Contains(unitSpawn.unitDefId.ToString()))
                    {
                        vehicleDefs.Add(unitSpawn.unitDefId.ToString());
                    }
                    /*else
                    {
                        throw new NotImplementedException();
                    }*/

                    if (!dynamicIdsList.Contains(unitSpawn.pilotDefId.ToString()))
                    {
                        pilotDefs.Add(unitSpawn.pilotDefId.ToString());
                    }
                }
            }

            this.MetaData.Add(Keywords.LanceDefId, new List<string>(lanceDefs));
            this.MetaData.Add(Keywords.HeraldryDefId, new List<string>(heraldryDefs));
            this.MetaData.Add(Keywords.MechDefId, new List<string>(mechDefs));
            this.MetaData.Add(Keywords.PilotId, new List<string>(pilotDefs));
            this.MetaData.Add(Keywords.TurretId, new List<string>(turretDefs));
            this.MetaData.Add(Keywords.VehicleId, new List<string>(vehicleDefs));
        }
    }
}