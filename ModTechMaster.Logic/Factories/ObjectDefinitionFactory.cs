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

        public IObjectDefinition Get(ObjectType entryType, IObjectDefinitionDescription objectDescription, dynamic jsonObject, string filePath)
        {
            IObjectDefinition objectDefinition;
            switch (entryType)
            {
                case ObjectType.Texture2D:
                case ObjectType.Sprite:
                    objectDefinition = new ResourceObjectDefinition(entryType, objectDescription, (JObject)jsonObject, filePath);
                    break;
                case ObjectType.AssetBundle:
                    objectDefinition = new AssetBundleObjectDefinition(entryType, objectDescription, (JObject)jsonObject, filePath);
                    break;
                case ObjectType.Prefab:
                    objectDefinition = new PrefabObjectDefinition(entryType, objectDescription, (JObject)jsonObject, filePath);
                    break;
                default:
                    objectDefinition = new ObjectDefinition(entryType, objectDescription, (JObject)jsonObject, filePath);
                    break;
            }
            objectDefinition.AddMetaData();
            return objectDefinition;
        }
    }
}