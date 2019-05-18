namespace ModTechMaster.Logic.Processors
{
    using System.IO;
    using Core.Enums.Mods;
    using Core.Interfaces.Models;
    using Core.Interfaces.Processors;
    using Data.Models.Mods.TypedObjectDefinitions;

    internal class ItemCollectionObjectDefinitionProcessor : IObjectDefinitionProcessor
    {
        public IObjectDefinition ProcessObjectDefinition(IManifestEntry manifestEntry, DirectoryInfo di, FileInfo fi)
        {
            var csvText = File.ReadAllText(fi.FullName);
            var itemCollection = new ItemCollectionObjectDefinition(ObjectType.ItemCollectionDef, csvText, fi.FullName);
            itemCollection.AddMetaData();
            return itemCollection;
        }
    }
}