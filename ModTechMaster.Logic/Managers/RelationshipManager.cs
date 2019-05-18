using System.Collections.Generic;
using System.Linq;
using ModTechMaster.Core.Constants;
using ModTechMaster.Core.Enums.Mods;
using ModTechMaster.Core.Interfaces.Models;
using ModTechMaster.Data.Models;

namespace ModTechMaster.Logic.Managers
{
    public class RelationshipManager
    {
        private static readonly Dictionary<ObjectType, List<IObjectRelationship>> _ObjectRelationships =
            new Dictionary<ObjectType, List<IObjectRelationship>>
            {
                {
                    ObjectType.Mod, new List<IObjectRelationship>
                    {
                        new ObjectRelationship(ObjectType.Mod, ObjectType.Mod,
                            Keywords.DependsOn, Keywords.Name, true)
                    }
                },
                {
                    ObjectType.PilotDef, new List<IObjectRelationship>
                    {
                        new ObjectRelationship(ObjectType.PilotDef, ObjectType.AbilityDef,
                            Keywords.AbilityDefId, Keywords.Id, true)
                    }
                },
                {
                    ObjectType.AmmunitionBoxDef, new List<IObjectRelationship>
                    {
                        new ObjectRelationship(ObjectType.AmmunitionBoxDef, ObjectType.AmmunitionDef,
                            Keywords.AmmoId, Keywords.Id)
                    }
                },
                {
                    ObjectType.WeaponDef, new List<IObjectRelationship>
                    {
                        new ObjectRelationship(ObjectType.WeaponDef, ObjectType.AmmunitionDef,
                            Keywords.AmmoCategory, Keywords.Category)
                    }
                },
                {
                    ObjectType.MechDef, new List<IObjectRelationship>
                    {
                        new ObjectRelationship(ObjectType.MechDef, ObjectType.WeaponDef,
                            Keywords.ComponentDefId, Keywords.Id, true),
                        new ObjectRelationship(ObjectType.MechDef, ObjectType.UpgradeDef,
                            Keywords.ComponentDefId, Keywords.Id, true),
                        new ObjectRelationship(ObjectType.MechDef, ObjectType.HeatSinkDef,
                            Keywords.ComponentDefId, Keywords.Id, true),
                        new ObjectRelationship(ObjectType.MechDef, ObjectType.JumpJetDef,
                            Keywords.ComponentDefId, Keywords.Id, true),
                        new ObjectRelationship(ObjectType.MechDef, ObjectType.ChassisDef,
                            Keywords.ChassisId, Keywords.Id)
                    }
                },
                {
                    ObjectType.ChassisDef, new List<IObjectRelationship>
                    {
                        new ObjectRelationship(ObjectType.ChassisDef, ObjectType.MovementCapabilitiesDef,
                            Keywords.MovementCapDefId, Keywords.Id),
                        new ObjectRelationship(ObjectType.ChassisDef, ObjectType.PathingCapabilitiesDef,
                            Keywords.PathingCapDefId, Keywords.Id),
                        new ObjectRelationship(ObjectType.ChassisDef, ObjectType.HardpointDataDef,
                            Keywords.HardpointDataDefId, Keywords.Id),
                        new ObjectRelationship(ObjectType.ChassisDef, ObjectType.Prefab,
                            Keywords.PrefabId, Keywords.Id)
                    }
                },
                {
                    ObjectType.VehicleDef, new List<IObjectRelationship>
                    {
                        new ObjectRelationship(ObjectType.VehicleDef, ObjectType.WeaponDef,
                            Keywords.ComponentDefId, Keywords.Id, true),
                        new ObjectRelationship(ObjectType.VehicleDef, ObjectType.UpgradeDef,
                            Keywords.ComponentDefId, Keywords.Id, true),
                        new ObjectRelationship(ObjectType.VehicleDef, ObjectType.HeatSinkDef,
                            Keywords.ComponentDefId, Keywords.Id, true),
                        new ObjectRelationship(ObjectType.VehicleDef, ObjectType.JumpJetDef,
                            Keywords.ComponentDefId, Keywords.Id, true),
                        new ObjectRelationship(ObjectType.VehicleDef, ObjectType.ChassisDef,
                            Keywords.ChassisId, Keywords.Id)
                    }
                },
                {
                    ObjectType.VehicleChassisDef, new List<IObjectRelationship>
                    {
                        new ObjectRelationship(ObjectType.VehicleChassisDef,
                            ObjectType.MovementCapabilitiesDef, Keywords.MovementCapDefId, Keywords.Id),
                        new ObjectRelationship(ObjectType.VehicleChassisDef,
                            ObjectType.PathingCapabilitiesDef, Keywords.PathingCapDefId, Keywords.Id),
                        new ObjectRelationship(ObjectType.VehicleChassisDef, ObjectType.HardpointDataDef,
                            Keywords.HardpointDataDefId, Keywords.Id),
                        new ObjectRelationship(ObjectType.VehicleChassisDef, ObjectType.Prefab,
                            Keywords.PrefabId, Keywords.Id)
                    }
                },
                {
                    ObjectType.TurretDef, new List<IObjectRelationship>
                    {
                        new ObjectRelationship(ObjectType.TurretDef, ObjectType.WeaponDef,
                            Keywords.ComponentDefId, Keywords.Id, true),
                        new ObjectRelationship(ObjectType.TurretDef, ObjectType.UpgradeDef,
                            Keywords.ComponentDefId, Keywords.Id, true),
                        new ObjectRelationship(ObjectType.TurretDef, ObjectType.HeatSinkDef,
                            Keywords.ComponentDefId, Keywords.Id, true),
                        new ObjectRelationship(ObjectType.TurretDef, ObjectType.JumpJetDef,
                            Keywords.ComponentDefId, Keywords.Id, true),
                        new ObjectRelationship(ObjectType.TurretDef, ObjectType.ChassisDef,
                            Keywords.ChassisId, Keywords.Id)
                    }
                },
                {
                    ObjectType.TurretChassisDef, new List<IObjectRelationship>
                    {
                        new ObjectRelationship(ObjectType.TurretChassisDef, ObjectType.HardpointDataDef,
                            Keywords.HardpointDataDefId, Keywords.Id),
                        new ObjectRelationship(ObjectType.TurretChassisDef, ObjectType.Prefab,
                            Keywords.PrefabId, Keywords.Id)
                    }
                },
                {
                    ObjectType.Prefab, new List<IObjectRelationship>
                    {
                        new ObjectRelationship(ObjectType.Prefab, ObjectType.AssetBundle,
                            Keywords.AssetBundle, Keywords.Id)
                    }
                },
                {
                    ObjectType.CCDefaults, new List<IObjectRelationship>
                    {
                        new ObjectRelationship(ObjectType.CCDefaults, ObjectType.CCCategories,
                            Keywords.CategoryId, Keywords.Name),
                        new ObjectRelationship(ObjectType.CCDefaults, ObjectType.UpgradeDef,
                            Keywords.DefId, Keywords.Id),
                        new ObjectRelationship(ObjectType.CCDefaults, ObjectType.HeatSinkDef,
                            Keywords.DefId, Keywords.Id)
                    }
                },
                {
                    ObjectType.UpgradeDef, new List<IObjectRelationship>
                    {
                        new ObjectRelationship(ObjectType.UpgradeDef, ObjectType.UpgradeDef,
                            Keywords.LootableId, Keywords.Id)
                        //new ObjectRelationship(ObjectType.UpgradeDef, ObjectType.UpgradeDef, Keywords.defId, Keywords.Id),
                    }
                },
                {
                    ObjectType.StreamingAssetsData, new List<IObjectRelationship>
                    {
                        new ObjectRelationship(ObjectType.StreamingAssetsData, ObjectType.AbilityDef,
                            Keywords.AbilityDefId, Keywords.Id, true),
                        new ObjectRelationship(ObjectType.StreamingAssetsData, ObjectType.MechDef,
                            Keywords.MechDefId, Keywords.Id, true)
                        //new ObjectRelationship(ObjectType.UpgradeDef, ObjectType.UpgradeDef, Keywords.defId, Keywords.Id),
                    }
                },
                {
                    ObjectType.ContractOverride, new List<IObjectRelationship>
                    {
                        new ObjectRelationship(ObjectType.ContractOverride, ObjectType.CastDef,
                            Keywords.CastDefId, Keywords.Id, true),
                        new ObjectRelationship(ObjectType.ContractOverride, ObjectType.DialogBucketDef,
                            Keywords.DialogBucketId, Keywords.Id, false),
                        new ObjectRelationship(ObjectType.ContractOverride, ObjectType.LanceDef,
                            Keywords.LanceDefId, Keywords.Id, true),
                        new ObjectRelationship(ObjectType.ContractOverride, ObjectType.HeraldryDef,
                            Keywords.HeraldryDefId, Keywords.Id, true),
                        new ObjectRelationship(ObjectType.ContractOverride, ObjectType.MechDef,
                            Keywords.MechDefId, Keywords.Id, true),
                        new ObjectRelationship(ObjectType.ContractOverride, ObjectType.PilotDef,
                            Keywords.PilotId, Keywords.Id, true),
                        new ObjectRelationship(ObjectType.ContractOverride, ObjectType.TurretDef,
                            Keywords.TurretId, Keywords.Id, true),
                        new ObjectRelationship(ObjectType.ContractOverride, ObjectType.VehicleDef,
                            Keywords.VehicleId, Keywords.Id, true),
                    }
                },
                {
                    ObjectType.FactionDef, new List<IObjectRelationship>
                    {
                        new ObjectRelationship(ObjectType.FactionDef, ObjectType.CastDef,
                            Keywords.CombatLeaderCastDefId, Keywords.Id, false),
                        new ObjectRelationship(ObjectType.FactionDef, ObjectType.CastDef,
                            Keywords.RepresentativeCastDefId, Keywords.Id, false),
                        new ObjectRelationship(ObjectType.FactionDef, ObjectType.HeraldryDef,
                            Keywords.HeraldryDefId, Keywords.Id, false),
                        new ObjectRelationship(ObjectType.FactionDef, ObjectType.FactionDef,
                            Keywords.EnemyFactionId, Keywords.Faction, true),
                        new ObjectRelationship(ObjectType.FactionDef, ObjectType.FactionDef,
                            Keywords.AlliedFactionId, Keywords.Faction, true),
                    }
                },
                {
                    ObjectType.CastDef, new List<IObjectRelationship>
                    {
                        new ObjectRelationship(ObjectType.CastDef, ObjectType.FactionDef,
                            Keywords.Faction, Keywords.FactionId, false),
                    }
                },
                {
                    ObjectType.ConversationContent, new List<IObjectRelationship>
                    {
                        new ObjectRelationship(ObjectType.ConversationContent, ObjectType.DialogBucketDef,
                            Keywords.DialogId, Keywords.Id, true),
                    }
                },
                {
                    ObjectType.DialogBucketDef, new List<IObjectRelationship>
                    {
                        new ObjectRelationship(ObjectType.DialogBucketDef, ObjectType.CastDef,
                            Keywords.CastDefId, Keywords.Id, true),
                    }
                },
                {
                    ObjectType.FlashpointDef, new List<IObjectRelationship>
                    {
                        new ObjectRelationship(ObjectType.FlashpointDef, ObjectType.SimGameMilestoneSet,
                            Keywords.MilestoneSetId, Keywords.Id, false),
                        new ObjectRelationship(ObjectType.FlashpointDef, ObjectType.ItemCollectionDef,
                            Keywords.ItemCollectionId, Keywords.Id, false),
                        new ObjectRelationship(ObjectType.FlashpointDef, ObjectType.CastDef,
                        Keywords.CastDefId, Keywords.Id, false)
                    }
                },
                {
                    ObjectType.HeraldryDef, new List<IObjectRelationship>
                    {
                        new ObjectRelationship(ObjectType.HeraldryDef, ObjectType.Texture2D,
                            Keywords.TextureId, Keywords.Id, false)
                    }
                },
                {
                    ObjectType.ItemCollectionDef, new List<IObjectRelationship>
                    {
                        new ObjectRelationship(ObjectType.ItemCollectionDef, ObjectType.MechDef,
                            Keywords.MechDefId, Keywords.Id, true),
                        new ObjectRelationship(ObjectType.ItemCollectionDef, ObjectType.UpgradeDef,
                            Keywords.UpgradeDefId, Keywords.Id, true),
                        new ObjectRelationship(ObjectType.ItemCollectionDef, ObjectType.ItemCollectionDef,
                            Keywords.ItemCollectionId, Keywords.Id, true),
                        new ObjectRelationship(ObjectType.ItemCollectionDef, ObjectType.WeaponDef,
                            Keywords.WeaponDefId, Keywords.Id, true),
                        new ObjectRelationship(ObjectType.ItemCollectionDef, ObjectType.HeatSinkDef,
                            Keywords.HeatSinkDefId, Keywords.Id, true),
                        new ObjectRelationship(ObjectType.ItemCollectionDef, ObjectType.AmmunitionBoxDef,
                            Keywords.AmmoBoxId, Keywords.Id, true),
                        new ObjectRelationship(ObjectType.ItemCollectionDef, ObjectType.JumpJetDef,
                            Keywords.JumpJetDefId, Keywords.Id, true),
                    }
                },
                {
                    ObjectType.SimGameMilestoneSet, new List<IObjectRelationship>
                    {
                        new ObjectRelationship(ObjectType.SimGameMilestoneSet, ObjectType.SimGameMilestoneDef,
                            Keywords.MilestoneId, Keywords.Id, true),
                        new ObjectRelationship(ObjectType.SimGameMilestoneSet, ObjectType.ContractOverride,
                            Keywords.ContractId, Keywords.Id, true),
                        new ObjectRelationship(ObjectType.SimGameMilestoneSet, ObjectType.SimGameEventDef,
                            Keywords.EventId, Keywords.Id, true),
                        new ObjectRelationship(ObjectType.SimGameMilestoneSet, ObjectType.ItemCollectionDef,
                            Keywords.ItemCollectionId, Keywords.Id, true),
                    }
                },
                {
                    ObjectType.HardpointDataDef, new List<IObjectRelationship>
                    {
                        new ObjectRelationship(ObjectType.HardpointDataDef, ObjectType.Prefab,
                            Keywords.PrefabId, Keywords.Id, true)
                    }
                },
                {
                    ObjectType.ShopDef, new List<IObjectRelationship>
                    {
                        new ObjectRelationship(ObjectType.ShopDef, ObjectType.WeaponDef,
                            Keywords.WeaponDefId, Keywords.Id, true),
                        new ObjectRelationship(ObjectType.ShopDef, ObjectType.UpgradeDef,
                            Keywords.UpgradeDefId, Keywords.Id, true),
                        new ObjectRelationship(ObjectType.ShopDef, ObjectType.MechDef,
                            Keywords.MechDefId, Keywords.Id, true),
                        new ObjectRelationship(ObjectType.ShopDef, ObjectType.HeatSinkDef,
                            Keywords.HeatSinkDefId, Keywords.Id, true),
                        new ObjectRelationship(ObjectType.ShopDef, ObjectType.JumpJetDef,
                            Keywords.JumpJetDefId, Keywords.Id, true)
                    }
                },
            };

        public static List<IObjectRelationship> GetDependenciesRelationshipsForType(ObjectType objectType)
        {
            return _ObjectRelationships.ContainsKey(objectType)
                ? _ObjectRelationships[objectType]
                : new List<IObjectRelationship>();
        }

        public static List<IObjectRelationship> GetDependantRelationShipsForType(ObjectType objectType)
        {
            return _ObjectRelationships.Where(pair => pair.Value.Exists(ship => ship.DependentType == objectType)).SelectMany(pair => pair.Value).ToList();
        }
    }
}