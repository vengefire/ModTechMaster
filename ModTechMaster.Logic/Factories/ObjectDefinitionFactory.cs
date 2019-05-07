namespace ModTechMaster.Logic.Factories
{
    using Core.Enums.Mods;
    using Core.Interfaces.Factories;
    using Core.Interfaces.Models;
    using Data.Models.Mods;
    using Data.Models.Mods.TypedObjectDefinitions;
    using Newtonsoft.Json.Linq;

    public class ObjectDefinitionFactory : IObjectDefinitionFactory
    {
        private static IObjectDefinitionFactory objectDefinitionFactorySingleton;

        public static IObjectDefinitionFactory ObjectDefinitionFactorySingleton => ObjectDefinitionFactory.objectDefinitionFactorySingleton ?? (ObjectDefinitionFactory.objectDefinitionFactorySingleton = new ObjectDefinitionFactory());

        public IObjectDefinition Get(ManifestEntryType entryType, IObjectDefinitionDescription objectDescription, dynamic jsonObject, string filePath)
        {
            IObjectDefinition objectDefinition;
            switch (entryType)
            {
                case ManifestEntryType.Texture2D:
                case ManifestEntryType.Sprite:
                    objectDefinition = new ResourceObjectDefinition(objectDescription, (JObject)jsonObject, filePath);
                    break;
                case ManifestEntryType.AssetBundle:
                    objectDefinition = new AssetBundleObjectDefinition(objectDescription, (JObject)jsonObject, filePath);
                    break;
                case ManifestEntryType.Prefab:
                    objectDefinition = new PrefabObjectDefinition(objectDescription, (JObject)jsonObject, filePath);
                    break;
                default:
                    objectDefinition = new ObjectDefinition(objectDescription, (JObject)jsonObject, filePath);
                    break;
            }
            objectDefinition.AddMetaData();
            return objectDefinition;
        }
    }
}