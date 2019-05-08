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
                        new ObjectRelationship(ObjectType.ChassisDef, ObjectType.HardPointDataDef,
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
                        new ObjectRelationship(ObjectType.VehicleChassisDef, ObjectType.HardPointDataDef,
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
                        new ObjectRelationship(ObjectType.TurretChassisDef, ObjectType.HardPointDataDef,
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
                    ObjectType.StreamingData, new List<IObjectRelationship>
                    {
                        new ObjectRelationship(ObjectType.StreamingData, ObjectType.AbilityDef,
                            Keywords.AbilityDefId, Keywords.Id, true),
                        new ObjectRelationship(ObjectType.StreamingData, ObjectType.MechDef,
                            Keywords.MechDefId, Keywords.Id, true)
                        //new ObjectRelationship(ObjectType.UpgradeDef, ObjectType.UpgradeDef, Keywords.defId, Keywords.Id),
                    }
                }
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