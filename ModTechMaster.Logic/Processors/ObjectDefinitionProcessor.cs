namespace ModTechMaster.Logic.Processors
{
    using System.IO;
    using Core.Interfaces.Models;
    using Core.Interfaces.Processors;
    using Data.Models.Mods;
    using Factories;
    using Newtonsoft.Json;

    internal class FactionObjectDefinitionProcessor : IObjectDefinitionProcessor
    {
        public IObjectDefinition ProcessObjectDefinition(IManifestEntry manifestEntry, DirectoryInfo di, FileInfo fi)
        {
            dynamic json = JsonConvert.DeserializeObject(File.ReadAllText(fi.FullName));
            return ObjectDefinitionFactory.ObjectDefinitionFactorySingleton.Get(manifestEntry.EntryType, FactionObjectDefinitionProcessor.ProcessObjectDescription(json), json, fi.FullName);
        }

        private static IObjectDefinitionDescription ProcessObjectDescription(dynamic description)
        {
            if (description == null)
            {
                return null;
            }

            string id = description.ID != null ? description.ID.ToString() : null;
            string name = description.Name != null ? description.Name.ToString() : null;
            string desc = description.Description != null ? description.Description.ToString() : null;
            string icon = description.Icon != null ? description.Icon.ToString() : null;
            return new ObjectDefinitionDescription(id, name, desc, icon, description);
        }
    }
}