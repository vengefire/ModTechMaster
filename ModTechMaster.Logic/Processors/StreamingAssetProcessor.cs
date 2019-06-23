﻿namespace ModTechMaster.Logic.Processors
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using ModTechMaster.Core.Enums.Mods;
    using ModTechMaster.Core.Interfaces.Models;
    using ModTechMaster.Core.Interfaces.Services;
    using ModTechMaster.Data.Models.Mods;
    using ModTechMaster.Data.Models.Mods.TypedObjectDefinitions;
    using ModTechMaster.Logic.Factories;

    using Newtonsoft.Json;

    public class StreamingAssetProcessor
    {
        private static Dictionary<string, ObjectType> streamingAssetsDirectoryToObjectTypes =
            new Dictionary<string, ObjectType>
                {
                    { "abilities", ObjectType.AbilityDef },
                    { "ammunition", ObjectType.AmmunitionDef },
                    { "ammunitionBox", ObjectType.AmmunitionBoxDef },
                    { "assetbundles", ObjectType.AssetBundle },
                    { "backgrounds", ObjectType.BackgroundDef },
                    { "cast", ObjectType.CastDef },
                    { "chassis", ObjectType.ChassisDef },
                    { "factions", ObjectType.FactionDef },
                    { "hardpoints", ObjectType.HardpointDataDef },
                    { "heatsinks", ObjectType.HeatSinkDef },
                    { "heraldry", ObjectType.HeraldryDef },
                    { "itemCollections", ObjectType.ItemCollectionDef },
                    { "jumpjets", ObjectType.JumpJetDef },
                    { "lance", ObjectType.LanceDef },
                    { "mech", ObjectType.MechDef },
                    { "movement", ObjectType.MovementCapabilitiesDef },
                    { "pilot", ObjectType.PilotDef },
                    { "shipUpgrades", ObjectType.ShipModuleUpgrade },
                    { "shops", ObjectType.ShopDef },
                    { "starsystem", ObjectType.StarSystemDef },
                    { "turretChassis", ObjectType.TurretChassisDef },
                    { "turrets", ObjectType.TurretDef },
                    { "upgrades", ObjectType.UpgradeDef },
                    { "vehicle", ObjectType.VehicleDef },
                    { "vehicleChassis", ObjectType.VehicleChassisDef },
                    { "weapon", ObjectType.WeaponDef }
                };

        public static object ProcessFile(IManifestEntry manifestEntry, DirectoryInfo di, string filename, string fileData, string hostDirectory, IReferenceFinderService referenceFinderService)
        {
            var fi = new FileInfo(filename);
            if (fi.Extension == ".json")
            {
                // Handle json object definition...
                IObjectDefinition objectDefinition;
                dynamic jsonData = JsonConvert.DeserializeObject(fileData);
                var description = ObjectDefinitionDescription.CreateDefault(jsonData.Description);

                if (streamingAssetsDirectoryToObjectTypes.ContainsKey(hostDirectory.ToLower()))
                {
                    var objectType = streamingAssetsDirectoryToObjectTypes[hostDirectory.ToLower()];
                    var objectDefinitionProcessor =
                        ObjectDefinitionProcessorFactory.ObjectDefinitionProcessorFactorySingleton.Get(objectType);

                    return objectDefinitionProcessor.ProcessObjectDefinition(
                        manifestEntry,
                        di,
                        fi,
                        referenceFinderService);
                }
                else
                {
                    // infer the object type from the current sub-directory.
                    switch (hostDirectory.ToLower())
                    {
                        case "abilities":
                        case "constants":
                        case "milestones":
                        case "events":
                        case "cast":
                        case "behaviorvariables":
                        case "buildings":
                        case "hardpoints":
                        case "factions":
                        case "lifepathnode":
                        case "campaign":
                            objectDefinition = new ObjectDefinition(
                                ObjectType.StreamingAssetsData,
                                description,
                                jsonData,
                                fi.FullName,
                                referenceFinderService);
                            break;
                        case "pilot":
                            objectDefinition = new PilotObjectDefinition(
                                ObjectType.PilotDef,
                                description,
                                jsonData,
                                fi.FullName,
                                referenceFinderService);
                            break;
                        case "simgameconstants":
                            objectDefinition = new SimGameConstantsObjectDefinition(
                                ObjectType.SimGameConstants,
                                ObjectDefinitionDescription.CreateDefault(jsonData.Description),
                                jsonData,
                                fi.FullName,
                                referenceFinderService);
                            break;
                        default:
                            throw new InvalidProgramException(
                                $"Unknown streaming assets folder type detected [{hostDirectory}]");
                    }
                }

                return objectDefinition;
            }
            else
            {
                switch (hostDirectory.ToLower())
                {
                    case "itemcollections":
                        var itemCollection = new ItemCollectionObjectDefinition(
                            ObjectType.ItemCollectionDef,
                            fileData,
                            fi.FullName);
                        itemCollection.AddMetaData();
                        return itemCollection;
                        break;

                    default:
                        IResourceDefinition resourceDefinition;

                        // Handle resource file style definition...
                        switch (fi.Name)
                        {
                            default:
                                resourceDefinition = new ResourceDefinition(
                                    ObjectType.UnhandledResource,
                                    fi.FullName,
                                    fi.Name,
                                    fi.Name);
                                break;
                        }

                        return resourceDefinition;
                        //this.Resources.Add(resourceDefinition);

                        // TBD: Add Note here -- throw new InvalidProgramException();
                        break;
                }
            }
        }
    }
}