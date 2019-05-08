using System;
using System.Collections.Generic;
using ModTechMaster.Core.Constants;
using ModTechMaster.Core.Enums.Mods;
using ModTechMaster.Core.Interfaces.Models;
using ModTechMaster.Data.Models;
using ModTechMaster.Data.Models.Mods;

namespace ModTechMaster.Logic.Managers
{
    public class RelationshipManager
    {
        public List<ObjectReference<Mod>> GetModReferences(IModCollection modCollection)
        {

        }

        private static List<IRelationship> _modRelationShips = new List<IRelationship>
        {
            new Relationship(Keywords.DependsOn, Keywords.Name, true)
        };

        private static Dictionary<ManifestEntryType, List<ObjectRelationship>> _ObjectRelationships =
            new Dictionary<ManifestEntryType, List<ObjectRelationship>>
            {
                {
                    ManifestEntryType.PilotDef, new List<ObjectRelationship>
                    {
                        new ObjectRelationship(ManifestEntryType.PilotDef, ManifestEntryType.AbilityDef,
                            Keywords.AbilityDefId, Keywords.Id, true)
                    }
                },
                {
                    ManifestEntryType.AmmunitionBoxDef, new List<ObjectRelationship>
                    {
                        new ObjectRelationship(ManifestEntryType.AmmunitionBoxDef, ManifestEntryType.AmmunitionDef,
                            Keywords.AmmoId, Keywords.Id)
                    }
                },
                {
                    ManifestEntryType.WeaponDef, new List<ObjectRelationship>
                    {
                        new ObjectRelationship(ManifestEntryType.WeaponDef, ManifestEntryType.AmmunitionDef,
                            Keywords.AmmoCategory, Keywords.Category)
                    }
                },
                {
                    ManifestEntryType.MechDef, new List<ObjectRelationship>
                    {
                        new ObjectRelationship(ManifestEntryType.MechDef, ManifestEntryType.WeaponDef,
                            Keywords.ComponentDefId, Keywords.Id, true),
                        new ObjectRelationship(ManifestEntryType.MechDef, ManifestEntryType.UpgradeDef,
                            Keywords.ComponentDefId, Keywords.Id, true),
                        new ObjectRelationship(ManifestEntryType.MechDef, ManifestEntryType.HeatSinkDef,
                            Keywords.ComponentDefId, Keywords.Id, true),
                        new ObjectRelationship(ManifestEntryType.MechDef, ManifestEntryType.JumpJetDef,
                            Keywords.ComponentDefId, Keywords.Id, true),
                        new ObjectRelationship(ManifestEntryType.MechDef, ManifestEntryType.ChassisDef,
                            Keywords.ChassisId, Keywords.Id)
                    }
                },
                {
                    ManifestEntryType.ChassisDef, new List<ObjectRelationship>
                    {
                        new ObjectRelationship(ManifestEntryType.ChassisDef, ManifestEntryType.MovementCapabilitiesDef,
                            Keywords.MovementCapDefId, Keywords.Id),
                        new ObjectRelationship(ManifestEntryType.ChassisDef, ManifestEntryType.PathingCapabilitiesDef,
                            Keywords.PathingCapDefId, Keywords.Id),
                        new ObjectRelationship(ManifestEntryType.ChassisDef, ManifestEntryType.HardPointDataDef,
                            Keywords.HardpointDataDefId, Keywords.Id),
                        new ObjectRelationship(ManifestEntryType.ChassisDef, ManifestEntryType.Prefab,
                            Keywords.PrefabId, Keywords.Id)
                    }
                },
                {
                    ManifestEntryType.VehicleDef, new List<ObjectRelationship>
                    {
                        new ObjectRelationship(ManifestEntryType.VehicleDef, ManifestEntryType.WeaponDef,
                            Keywords.ComponentDefId, Keywords.Id, true),
                        new ObjectRelationship(ManifestEntryType.VehicleDef, ManifestEntryType.UpgradeDef,
                            Keywords.ComponentDefId, Keywords.Id, true),
                        new ObjectRelationship(ManifestEntryType.VehicleDef, ManifestEntryType.HeatSinkDef,
                            Keywords.ComponentDefId, Keywords.Id, true),
                        new ObjectRelationship(ManifestEntryType.VehicleDef, ManifestEntryType.JumpJetDef,
                            Keywords.ComponentDefId, Keywords.Id, true),
                        new ObjectRelationship(ManifestEntryType.VehicleDef, ManifestEntryType.ChassisDef,
                            Keywords.ChassisId, Keywords.Id)
                    }
                },
                {
                    ManifestEntryType.VehicleChassisDef, new List<ObjectRelationship>
                    {
                        new ObjectRelationship(ManifestEntryType.VehicleChassisDef,
                            ManifestEntryType.MovementCapabilitiesDef, Keywords.MovementCapDefId, Keywords.Id),
                        new ObjectRelationship(ManifestEntryType.VehicleChassisDef,
                            ManifestEntryType.PathingCapabilitiesDef, Keywords.PathingCapDefId, Keywords.Id),
                        new ObjectRelationship(ManifestEntryType.VehicleChassisDef, ManifestEntryType.HardPointDataDef,
                            Keywords.HardpointDataDefId, Keywords.Id),
                        new ObjectRelationship(ManifestEntryType.VehicleChassisDef, ManifestEntryType.Prefab,
                            Keywords.PrefabId, Keywords.Id)
                    }
                },
                {
                    ManifestEntryType.TurretDef, new List<ObjectRelationship>
                    {
                        new ObjectRelationship(ManifestEntryType.TurretDef, ManifestEntryType.WeaponDef,
                            Keywords.ComponentDefId, Keywords.Id, true),
                        new ObjectRelationship(ManifestEntryType.TurretDef, ManifestEntryType.UpgradeDef,
                            Keywords.ComponentDefId, Keywords.Id, true),
                        new ObjectRelationship(ManifestEntryType.TurretDef, ManifestEntryType.HeatSinkDef,
                            Keywords.ComponentDefId, Keywords.Id, true),
                        new ObjectRelationship(ManifestEntryType.TurretDef, ManifestEntryType.JumpJetDef,
                            Keywords.ComponentDefId, Keywords.Id, true),
                        new ObjectRelationship(ManifestEntryType.TurretDef, ManifestEntryType.ChassisDef,
                            Keywords.ChassisId, Keywords.Id)
                    }
                },
                {
                    ManifestEntryType.TurretChassisDef, new List<ObjectRelationship>
                    {
                        new ObjectRelationship(ManifestEntryType.TurretChassisDef, ManifestEntryType.HardPointDataDef,
                            Keywords.HardpointDataDefId, Keywords.Id),
                        new ObjectRelationship(ManifestEntryType.TurretChassisDef, ManifestEntryType.Prefab,
                            Keywords.PrefabId, Keywords.Id)
                    }
                },
                {
                    ManifestEntryType.Prefab, new List<ObjectRelationship>
                    {
                        new ObjectRelationship(ManifestEntryType.Prefab, ManifestEntryType.AssetBundle,
                            Keywords.AssetBundle, Keywords.Id)
                    }
                },
                {
                    ManifestEntryType.CCDefaults, new List<ObjectRelationship>
                    {
                        new ObjectRelationship(ManifestEntryType.CCDefaults, ManifestEntryType.CCCategories,
                            Keywords.CategoryId, Keywords.Name),
                        new ObjectRelationship(ManifestEntryType.CCDefaults, ManifestEntryType.UpgradeDef,
                            Keywords.DefId, Keywords.Id),
                        new ObjectRelationship(ManifestEntryType.CCDefaults, ManifestEntryType.HeatSinkDef,
                            Keywords.DefId, Keywords.Id)
                    }
                },
                {
                    ManifestEntryType.UpgradeDef, new List<ObjectRelationship>
                    {
                        new ObjectRelationship(ManifestEntryType.UpgradeDef, ManifestEntryType.UpgradeDef,
                            Keywords.LootableId, Keywords.Id)
                        //new ObjectRelationship(ManifestEntryType.UpgradeDef, ManifestEntryType.UpgradeDef, Keywords.defId, Keywords.Id),
                    }
                },
                {
                    ManifestEntryType.StreamingData, new List<ObjectRelationship>
                    {
                        new ObjectRelationship(ManifestEntryType.StreamingData, ManifestEntryType.AbilityDef,
                            Keywords.AbilityDefId, Keywords.Id, true),
                        new ObjectRelationship(ManifestEntryType.StreamingData, ManifestEntryType.MechDef,
                            Keywords.MechDefId, Keywords.Id, true)
                        //new ObjectRelationship(ManifestEntryType.UpgradeDef, ManifestEntryType.UpgradeDef, Keywords.defId, Keywords.Id),
                    }
                }
            };
    }
}