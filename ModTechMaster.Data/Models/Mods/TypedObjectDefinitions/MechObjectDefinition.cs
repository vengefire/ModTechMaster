﻿using System;
using System.Linq;

namespace ModTechMaster.Data.Models.Mods.TypedObjectDefinitions
{
    using System.Collections.Generic;
    using Core.Constants;
    using Core.Enums.Mods;
    using Core.Interfaces.Models;
    using Newtonsoft.Json.Linq;

    public class MechObjectDefinition : ObjectDefinition
    {
        public MechObjectDefinition(
            ObjectType objectType, IObjectDefinitionDescription objectDescription,
            dynamic jsonObject, string filePath) : base(objectType, objectDescription, (JObject)jsonObject, filePath)
        {
        }

        public override void AddMetaData()
        {
            base.AddMetaData();
            var componentIdList = new Dictionary<string, HashSet<string>>()
            {
                { Keywords.UpgradeDefId, new HashSet<string>()} ,
                { Keywords.WeaponDefId, new HashSet<string>()} ,
                { Keywords.HeatSinkDefId, new HashSet<string>()} ,
                { Keywords.AmmoBoxId, new HashSet<string>()} ,
                { Keywords.JumpJetDefId, new HashSet<string>()} ,
            };
            //this.MetaData.Add(Keywords.ComponentDefId, mechComponentIdList);
            foreach (var item in this.JsonObject.inventory)
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
            componentIdList.ToList().ForEach(pair =>
            {
                this.MetaData.Add(pair.Key, pair.Value.ToList());
            });
            this.MetaData.Add(Keywords.ChassisId, this.JsonObject.ChassisID);
        }
    }
}