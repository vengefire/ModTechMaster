using ModTechMaster.Core.Constants;
using ModTechMaster.Core.Enums.Mods;
using ModTechMaster.Core.Interfaces.Models;
using Newtonsoft.Json.Linq;

namespace ModTechMaster.Data.Models.Mods.TypedObjectDefinitions
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;

    public class ShopDefObjectDefinition : ObjectDefinition
    {
        public ShopDefObjectDefinition(ObjectType objectType, IObjectDefinitionDescription objectDescription,
            dynamic jsonObject, string filePath) : base(objectType, objectDescription, (JObject) jsonObject, filePath)
        {
        }

        public override void AddMetaData()
        {
            base.AddMetaData();
            var weapons = new HashSet<string>();
            var upgrades = new HashSet<string>();
            var mechs = new HashSet<string>();
            var heatsinks = new HashSet<string>();
            var jumpjets = new HashSet<string>();

            foreach (var item in this.JsonObject.Inventory)
            {
                if (item.Type == "Weapon")
                {
                    weapons.Add(item.ID.ToString());
                }
                else if (item.Type == "Upgrade")
                {
                    upgrades.Add(item.ID.ToString());
                }
                else if (item.Type == "Mech" || item.Type == "MechPart")
                {
                    mechs.Add(item.ID.ToString());
                }
                else if (item.Type == "HeatSink")
                {
                    heatsinks.Add(item.ID.ToString());
                }
                else if (item.Type == "JumpJet")
                {
                    jumpjets.Add(item.ID.ToString());
                }
                else
                {
                    throw new NotImplementedException();
                }
            }

            foreach (var special in this.JsonObject.Specials)
            {
                if (special.Type == "Weapon")
                {
                    weapons.Add(special.ID.ToString());
                }
                else if (special.Type == "Upgrade")
                {
                    upgrades.Add(special.ID.ToString());
                }
                else if (special.Type == "Mech" || special.Type == "MechPart")
                {
                    mechs.Add(special.ID.ToString());
                }
                else if (special.Type == "HeatSink")
                {
                    heatsinks.Add(special.ID.ToString());
                }
                else if (special.Type == "JumpJet")
                {
                    jumpjets.Add(special.ID.ToString());
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
            this.MetaData.Add(Keywords.WeaponDefId, weapons);
            this.MetaData.Add(Keywords.UpgradeDefId, upgrades);
            this.MetaData.Add(Keywords.MechDefId, mechs);
            this.MetaData.Add(Keywords.HeatSinkDefId, heatsinks);
            this.MetaData.Add(Keywords.JumpJetDefId, jumpjets);
        }
    }
}