﻿namespace ModTechMaster.Logic.Factories
{
    using System;

    using ModTechMaster.Core.Enums.Mods;
    using ModTechMaster.Core.Interfaces.Factories;
    using ModTechMaster.Core.Interfaces.Models;
    using ModTechMaster.Core.Interfaces.Services;
    using ModTechMaster.Data.Models.Mods;
    using ModTechMaster.Data.Models.Mods.TypedObjectDefinitions;
    using ModTechMaster.Logic.Services;

    using Newtonsoft.Json.Linq;

    public class ObjectDefinitionFactory : IObjectDefinitionFactory
    {
        private static IObjectDefinitionFactory objectDefinitionFactorySingleton;

        public static IObjectDefinitionFactory ObjectDefinitionFactorySingleton =>
            objectDefinitionFactorySingleton ?? (objectDefinitionFactorySingleton = new ObjectDefinitionFactory());

        private static IFactionService factionService = new FactionService();

        public IObjectDefinition Get(
            ObjectType entryType,
            IObjectDefinitionDescription objectDescription,
            dynamic jsonObject,
            string filePath,
            IReferenceFinderService referenceFinderService)
        {
            IObjectDefinition objectDefinition;
            switch (entryType)
            {
                case ObjectType.Texture2D:
                case ObjectType.Sprite:
                    objectDefinition = new ResourceObjectDefinition(
                        entryType,
                        objectDescription,
                        (JObject)jsonObject,
                        filePath,
                        referenceFinderService);
                    break;
                case ObjectType.AssetBundle:
                    objectDefinition = new AssetBundleObjectDefinition(
                        entryType,
                        objectDescription,
                        (JObject)jsonObject,
                        filePath,
                        referenceFinderService);
                    break;
                case ObjectType.Prefab:
                    objectDefinition = new PrefabObjectDefinition(
                        entryType,
                        objectDescription,
                        (JObject)jsonObject,
                        filePath,
                        referenceFinderService);
                    break;
                case ObjectType.AmmunitionBoxDef:
                    objectDefinition = new AmmunitionBoxObjectDefinition(
                        entryType,
                        objectDescription,
                        (JObject)jsonObject,
                        filePath,
                        referenceFinderService);
                    break;
                case ObjectType.AmmunitionDef:
                    objectDefinition = new AmmunitionObjectDefinition(
                        entryType,
                        objectDescription,
                        (JObject)jsonObject,
                        filePath,
                        referenceFinderService);
                    break;
                case ObjectType.CCDefaults:
                    objectDefinition = new CcDefaultsObjectDefinition(
                        entryType,
                        objectDescription,
                        (JObject)jsonObject,
                        filePath,
                        referenceFinderService);
                    break;
                case ObjectType.CCCategories:
                    objectDefinition = new CcCategoryObjectDefinition(
                        entryType,
                        objectDescription,
                        (JObject)jsonObject,
                        filePath,
                        referenceFinderService);
                    break;
                case ObjectType.ChassisDef:
                    objectDefinition = new ChassisObjectDefinition(
                        entryType,
                        objectDescription,
                        (JObject)jsonObject,
                        filePath,
                        referenceFinderService);
                    break;
                case ObjectType.MechDef:
                    objectDefinition = new MechObjectDefinition(
                        entryType,
                        objectDescription,
                        (JObject)jsonObject,
                        filePath,
                        referenceFinderService);
                    break;
                case ObjectType.PilotDef:
                    objectDefinition = new PilotObjectDefinition(
                        entryType,
                        objectDescription,
                        (JObject)jsonObject,
                        filePath,
                        referenceFinderService);
                    break;
                case ObjectType.SimGameEventDef:
                    objectDefinition = new SimGameEventObjectDefinition(
                        entryType,
                        objectDescription,
                        (JObject)jsonObject,
                        filePath,
                        referenceFinderService);
                    break;
                case ObjectType.StreamingAssetsData:
                    objectDefinition = new SimGameConstantsObjectDefinition(
                        entryType,
                        objectDescription,
                        (JObject)jsonObject,
                        filePath,
                        referenceFinderService);
                    break;
                case ObjectType.TurretChassisDef:
                    objectDefinition = new TurretChassisObjectDefinition(
                        entryType,
                        objectDescription,
                        (JObject)jsonObject,
                        filePath,
                        referenceFinderService);
                    break;
                case ObjectType.TurretDef:
                    objectDefinition = new TurretObjectDefinition(
                        entryType,
                        objectDescription,
                        (JObject)jsonObject,
                        filePath,
                        referenceFinderService);
                    break;
                case ObjectType.UpgradeDef:
                    objectDefinition = new UpgradeObjectDefinition(
                        entryType,
                        objectDescription,
                        (JObject)jsonObject,
                        filePath,
                        referenceFinderService);
                    break;
                case ObjectType.VehicleChassisDef:
                    objectDefinition = new VehicleChassisObjectDefinition(
                        entryType,
                        objectDescription,
                        (JObject)jsonObject,
                        filePath,
                        referenceFinderService);
                    break;
                case ObjectType.VehicleDef:
                    objectDefinition = new VehicleObjectDefinition(
                        entryType,
                        objectDescription,
                        (JObject)jsonObject,
                        filePath,
                        referenceFinderService);
                    break;
                case ObjectType.WeaponDef:
                    objectDefinition = new WeaponObjectDefinition(
                        entryType,
                        objectDescription,
                        (JObject)jsonObject,
                        filePath,
                        referenceFinderService);
                    break;
                case ObjectType.ContractOverride:
                    objectDefinition = new ContractObjectDefinition(
                        entryType,
                        objectDescription,
                        (JObject)jsonObject,
                        filePath,
                        referenceFinderService);
                    break;
                case ObjectType.FactionDef:
                    objectDefinition = new FactionObjectDefinition(
                        entryType,
                        objectDescription,
                        (JObject)jsonObject,
                        filePath,
                        referenceFinderService);
                    break;
                case ObjectType.CastDef:
                    objectDefinition = new CastObjectDefinition(
                        entryType,
                        objectDescription,
                        (JObject)jsonObject,
                        filePath,
                        referenceFinderService);
                    break;
                case ObjectType.ConversationContent:
                    objectDefinition = new DialogObjectDefinition(
                        entryType,
                        objectDescription,
                        (JObject)jsonObject,
                        filePath,
                        referenceFinderService);

                    // objectDefinition = new ConversationObjectDefinition(entryType, objectDescription, (JObject)jsonObject, filePath);
                    break;
                case ObjectType.DialogBucketDef:
                    objectDefinition = new ConversationObjectDefinition(
                        entryType,
                        objectDescription,
                        (JObject)jsonObject,
                        filePath,
                        referenceFinderService);

                    // objectDefinition = new DialogObjectDefinition(entryType, objectDescription, (JObject)jsonObject, filePath);
                    break;
                case ObjectType.FlashpointDef:
                    objectDefinition = new FlashpointObjectDefinition(
                        entryType,
                        objectDescription,
                        (JObject)jsonObject,
                        filePath,
                        referenceFinderService);
                    break;
                case ObjectType.HeraldryDef:
                    objectDefinition = new HeraldryObjectDefinition(
                        entryType,
                        objectDescription,
                        (JObject)jsonObject,
                        filePath,
                        referenceFinderService);
                    break;
                case ObjectType.SimGameMilestoneSet:
                    objectDefinition = new MilestoneSetObjectDefinition(
                        entryType,
                        objectDescription,
                        (JObject)jsonObject,
                        filePath,
                        referenceFinderService);
                    break;
                case ObjectType.HardpointDataDef:
                    objectDefinition = new HardpointDataObjectDefinition(
                        entryType,
                        objectDescription,
                        (JObject)jsonObject,
                        filePath,
                        referenceFinderService);
                    break;
                case ObjectType.ShopDef:
                    objectDefinition = new ShopDefObjectDefinition(
                        entryType,
                        objectDescription,
                        (JObject)jsonObject,
                        filePath,
                        referenceFinderService);
                    break;
                case ObjectType.LanceDef:
                    objectDefinition = new LanceDefObjectDefinition(
                        entryType,
                        objectDescription,
                        (JObject)jsonObject,
                        filePath,
                        referenceFinderService,
                        factionService);
                    break;
                case ObjectType.AbilityDef:
                case ObjectType.MovementCapabilitiesDef:
                case ObjectType.ApplicationConstants:
                case ObjectType.CCTagRestrictions:
                case ObjectType.SimGameConversations:
                case ObjectType.HeatSinkDef:
                case ObjectType.JumpJetDef:
                case ObjectType.BaseDescriptionDef:
                case ObjectType.StarSystemDef:
                case ObjectType.MEBonusDescriptions:
                case ObjectType.MECriticalEffects:
                case ObjectType.ShipModuleUpgrade:
                case ObjectType.PathingCapabilitiesDef:
                case ObjectType.DesignMaskDef:
                case ObjectType.BackgroundDef:
                case ObjectType.SimGameMilestoneDef:
                case ObjectType.DebugSettings:
                case ObjectType.SimGameStatDescDef:
                case ObjectType.GameTip:
                case ObjectType.SoundBank:
                    objectDefinition = new ObjectDefinition(
                        entryType,
                        objectDescription,
                        (JObject)jsonObject,
                        filePath,
                        referenceFinderService);
                    break;
                default:
                    throw new InvalidProgramException();

                // objectDefinition = new ObjectDefinition(entryType, objectDescription, (JObject)jsonObject, filePath);
                // break;
            }

            objectDefinition.AddMetaData();
            return objectDefinition;
        }
    }
}