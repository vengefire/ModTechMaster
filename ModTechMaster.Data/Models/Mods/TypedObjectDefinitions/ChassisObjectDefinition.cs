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

    public class ChassisObjectDefinition : ObjectDefinition
    {
        public ChassisObjectDefinition(
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
            this.MetaData.Add(Keywords.MovementCapDefId, this.JsonObject.MovementCapDefID.ToString());
            this.MetaData.Add(Keywords.PathingCapDefId, this.JsonObject.PathingCapDefID.ToString());
            this.MetaData.Add(Keywords.HardpointDataDefId, this.JsonObject.HardpointDataDefID.ToString());
            this.MetaData.Add(Keywords.PrefabId, this.JsonObject.PrefabIdentifier.ToString());

            var componentIdList = new Dictionary<string, HashSet<string>>
                                      {
                                          { Keywords.UpgradeDefId, new HashSet<string>() },
                                          { Keywords.WeaponDefId, new HashSet<string>() },
                                          { Keywords.HeatSinkDefId, new HashSet<string>() },
                                          { Keywords.AmmoBoxId, new HashSet<string>() },
                                          { Keywords.JumpJetDefId, new HashSet<string>() }
                                      };

            // this.MetaData.Add(Keywords.ComponentDefId, mechComponentIdList);
            if (this.JsonObject.FixedEquipment != null)
            {
                foreach (var item in this.JsonObject.FixedEquipment)
                {
                    switch (item.ComponentDefType.ToString().Trim())
                    {
                        case "Upgrade":
                            componentIdList[Keywords.UpgradeDefId].Add(item.ComponentDefID.ToString());
                            break;
                        case "Weapon":
                            componentIdList[Keywords.WeaponDefId].Add(item.ComponentDefID.ToString());
                            break;
                        case "HeatSink":
                            componentIdList[Keywords.HeatSinkDefId].Add(item.ComponentDefID.ToString());
                            break;
                        case "AmmunitionBox":
                            componentIdList[Keywords.AmmoBoxId].Add(item.ComponentDefID.ToString());
                            break;
                        case "JumpJet":
                            componentIdList[Keywords.JumpJetDefId].Add(item.ComponentDefID.ToString());
                            break;
                        default:
                            throw new InvalidProgramException();
                    }
                }
            }

            componentIdList.ToList().ForEach(pair => { this.MetaData.Add(pair.Key, pair.Value.ToList()); });
        }
    }
}