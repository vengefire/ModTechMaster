namespace ModTechMaster.Logic.Processors
{
    using System.Collections.Generic;
    using System.IO;

    using Castle.Core.Logging;

    using ModTechMaster.Core.Enums.Mods;
    using ModTechMaster.Core.Interfaces.Models;
    using ModTechMaster.Core.Interfaces.Services;
    using ModTechMaster.Data.Models.Mods;
    using ModTechMaster.Data.Models.Mods.TypedObjectDefinitions;
    using ModTechMaster.Logic.Factories;

    using Newtonsoft.Json;

    public class StreamingAssetProcessor
    {
        private static readonly Dictionary<string, ObjectType> streamingAssetsDirectoryToObjectTypes =
            new Dictionary<string, ObjectType>
                {
                    { "traits", ObjectType.AbilityDef },
                    { "abilities", ObjectType.AbilityDef },
                    { "ammunition", ObjectType.AmmunitionDef },
                    { "ammunitionbox", ObjectType.AmmunitionBoxDef },
                    { "assetbundles", ObjectType.AssetBundle },
                    { "backgrounds", ObjectType.BackgroundDef },
                    { "cast", ObjectType.CastDef },
                    { "chassis", ObjectType.ChassisDef },
                    { "factions", ObjectType.FactionDef },
                    { "hardpoints", ObjectType.HardpointDataDef },
                    { "heatsinks", ObjectType.HeatSinkDef },
                    { "heraldry", ObjectType.HeraldryDef },
                    { "itemcollections", ObjectType.ItemCollectionDef },
                    { "jumpjets", ObjectType.JumpJetDef },
                    { "lance", ObjectType.LanceDef },
                    { "mech", ObjectType.MechDef },
                    { "movement", ObjectType.MovementCapabilitiesDef },
                    { "pilot", ObjectType.PilotDef },
                    { "shipupgrades", ObjectType.ShipModuleUpgrade },
                    { "shops", ObjectType.ShopDef },
                    { "starsystem", ObjectType.StarSystemDef },
                    { "turretchassis", ObjectType.TurretChassisDef },
                    { "turrets", ObjectType.TurretDef },
                    { "upgrades", ObjectType.UpgradeDef },
                    { "actuators", ObjectType.UpgradeDef },
                    { "cockpitmods", ObjectType.UpgradeDef },
                    { "general", ObjectType.UpgradeDef },
                    { "gyros", ObjectType.UpgradeDef },
                    { "targettrackingsystem", ObjectType.UpgradeDef },
                    { "vehicle", ObjectType.VehicleDef },
                    { "vehiclechassis", ObjectType.VehicleChassisDef },
                    { "weapon", ObjectType.WeaponDef },
                    { "pathing", ObjectType.PathingCapabilitiesDef },
                };

        public static object ProcessFile(
            IManifestEntry manifestEntry,
            DirectoryInfo di,
            string filename,
            string fileData,
            string hostDirectory,
            IReferenceFinderService referenceFinderService,
            ILogger logger)
        {
            var fi = new FileInfo(filename);
            if (fi.Extension == ".json")
            {
                // Handle json object definition...
                IObjectDefinition objectDefinition = null;
                dynamic jsonData = JsonConvert.DeserializeObject(
                    fileData,
                    new JsonSerializerSettings()
                        {
                            Error = (sender, args) =>
                                                                                       {
                                                                                           logger.Warn($"Error deserializing [{fi.FullName}]", args.ErrorContext.Error);
                                                                                           args.ErrorContext.Handled = true;
                                                                                       }
                                                                               });

                if (jsonData == null)
                {
                    return null;
                }

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
                        referenceFinderService,
                        objectType);
                }

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

                    /*throw new InvalidProgramException(
                                                    $"Unknown streaming assets folder type detected [{hostDirectory}]");*/
                }

                return objectDefinition;
            }

            if (di.FullName.ToLower().Contains("emblems"))
            {
                hostDirectory = "emblems";
            }

            if (di.FullName.ToLower().Contains("sprites"))
            {
                hostDirectory = "sprites";
            }

            var identifier = fi.Name.Substring(0, fi.Name.Length - fi.Extension.Length);
            switch (hostDirectory.ToLower())
            {
                case "assetbundles":
                    var assetBundleDefinition = ObjectDefinitionFactory.ObjectDefinitionFactorySingleton.Get(
                        ObjectType.AssetBundle,
                        new ObjectDefinitionDescription(fi.Name, fi.Name, null),
                        null,
                        fi.FullName,
                        referenceFinderService);
                    return assetBundleDefinition;
                    break;

                case "mechportraits":
                    var objectDefinition = ObjectDefinitionFactory.ObjectDefinitionFactorySingleton.Get(
                        ObjectType.Sprite,
                        new ObjectDefinitionDescription(identifier, identifier, null),
                        null,
                        fi.FullName,
                        referenceFinderService);
                    return objectDefinition;
                    break;

                case "itemcollections":
                    var itemCollection = new ItemCollectionObjectDefinition(
                        ObjectType.ItemCollectionDef,
                        fileData,
                        fi.FullName);
                    itemCollection.AddMetaData();
                    return itemCollection;
                    break;

                case "emblems":
                    var texIdentifier = fi.Name.Substring(0, fi.Name.Length - fi.Extension.Length);
                    var resourceObjectDefinition = ObjectDefinitionFactory.ObjectDefinitionFactorySingleton.Get(
                        ObjectType.Texture2D,
                        new ObjectDefinitionDescription(texIdentifier, texIdentifier, null),
                        null,
                        fi.FullName,
                        referenceFinderService);
                    return resourceObjectDefinition;
                    break;

                case "sprites":
                    var spriteIdentifier = fi.Name.Substring(0, fi.Name.Length - fi.Extension.Length);
                    var spriteObject = ObjectDefinitionFactory.ObjectDefinitionFactorySingleton.Get(
                        ObjectType.Sprite,
                        new ObjectDefinitionDescription(spriteIdentifier, spriteIdentifier, null),
                        null,
                        fi.FullName,
                        referenceFinderService);
                    return spriteObject;
                    break;

                default:
                    if (manifestEntry.Manifest.Mod.IsBattleTech)
                    {
                        return null;
                    }

                    IResourceDefinition resourceDefinition;

                    // Handle resource file style definition...
                    switch (fi.Name)
                    {
                        default:
                            resourceDefinition = new ResourceDefinition(
                                ObjectType.UnhandledResource,
                                fi.FullName,
                                fi.Name,
                                identifier);
                            resourceDefinition.AddMetaData();
                            break;
                    }

                    return resourceDefinition;

                    // this.Resources.Add(resourceDefinition);

                    // TBD: Add Note here -- throw new InvalidProgramException();
                    break;
            }
        }
    }
}