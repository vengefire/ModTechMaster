namespace ModTechMaster.Logic.Processors
{
    using System.IO;

    using ModTechMaster.Core.Enums.Mods;
    using ModTechMaster.Core.Interfaces.Models;
    using ModTechMaster.Core.Interfaces.Processors;
    using ModTechMaster.Data.Models.Mods.TypedObjectDefinitions;

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