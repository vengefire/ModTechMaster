namespace ModTechMaster.Data.Models.Mods.TypedObjectDefinitions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Core.Constants;
    using Core.Enums.Mods;

    public class ItemCollectionObjectDefinition : CsvObjectBase
    {
        public ItemCollectionObjectDefinition(ObjectType objectType, string csvText, string filePath) : base(objectType, filePath, csvText)
        {
        }

        public override string Name => this.CsvData[0][0]; // Default to first entry of first line...
        public override void AddMetaData()
        {
            base.AddMetaData();

            var mechDefs = new HashSet<string>();
            var itemCollections = new HashSet<string>();
            var upgrades = new HashSet<string>();
            var weapons = new HashSet<string>();
            var heatsinks = new HashSet<string>();
            var ammoBoxes = new HashSet<string>();
            var jumpJets = new HashSet<string>();
            foreach (var line in this.CsvData.Skip(1))
            {
                var id = line[0];
                var type = line[1];
                // What does 2 and 3 do?
                switch (type)
                {
                    case "MechPart":
                    case "Mech":
                        mechDefs.Add(id);
                        break;
                    case "Reference":
                        itemCollections.Add(id);
                        break;
                    case "Upgrade":
                        upgrades.Add(id);
                        break;
                    case "Weapon":
                        weapons.Add(id);
                        break;
                    case "HeatSink":
                        heatsinks.Add(id);
                        break;
                    case "AmmunitionBox":
                        ammoBoxes.Add(id);
                        break;
                    case "JumpJet":
                        jumpJets.Add(id);
                        break;
                    default:
                        throw new InvalidProgramException();
                }
            }
            this.MetaData.Add(Keywords.MechDefId, mechDefs);
            this.MetaData.Add(Keywords.ItemCollectionId, itemCollections);
            this.MetaData.Add(Keywords.UpgradeDefId, upgrades);
            this.MetaData.Add(Keywords.WeaponDefId, weapons);
            this.MetaData.Add(Keywords.HeatSinkDefId, heatsinks);
            this.MetaData.Add(Keywords.AmmoBoxId, ammoBoxes);
            this.MetaData.Add(Keywords.JumpJetDefId, jumpJets);
        }

        public override string Id => this.Name;
    }
}