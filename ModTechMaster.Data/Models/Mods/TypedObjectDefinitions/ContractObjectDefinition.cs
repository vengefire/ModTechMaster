using System.Collections.Generic;
using System.Linq;
using ModTechMaster.Core.Constants;
using ModTechMaster.Core.Enums.Mods;
using ModTechMaster.Core.Interfaces.Models;
using Newtonsoft.Json.Linq;

namespace ModTechMaster.Data.Models.Mods.TypedObjectDefinitions
{
    using System;

    public class ContractObjectDefinition : ObjectDefinition
    {
        public ContractObjectDefinition(ObjectType objectType, IObjectDefinitionDescription objectDescription,
            dynamic jsonObject, string filePath) : base(objectType, objectDescription, (JObject) jsonObject, filePath)
        {
        }

        public override void AddMetaData()
        {
            base.AddMetaData();
            var castList = new HashSet<string>();
            foreach (var dialog in JsonObject.dialogueList)
            {
                foreach (var dialogueContent in dialog.dialogueContent)
                {
                    castList.Add(dialogueContent.SelectedCastDefId);
                }
            }

            this.MetaData.Add(Keywords.CastDefId, new List<string>(castList));

            // castDef_TeamLeader_Current?
            // selectedLanceDefId
            // pilotDefId
            // lanceOverrideList
            if (this.JsonObject.overrideAutoCompleteDialogueId != null)
            {
                this.MetaData.Add(Keywords.DialogBucketId, JsonObject.overrideAutoCompleteDialogueId);
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

                    if (unitSpawn.unitType == "Mech")
                    {
                        mechDefs.Add(unitSpawn.unitDefId.ToString());
                    }
                    else if (unitSpawn.unitType == "Turret")
                    {
                        turretDefs.Add(unitSpawn.unitDefId.ToString());
                    }
                    else if (unitSpawn.unitType == "Vehicle")
                    {
                        vehicleDefs.Add(unitSpawn.unitDefId.ToString());
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }

                    pilotDefs.Add(unitSpawn.pilotDefId.ToString());
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