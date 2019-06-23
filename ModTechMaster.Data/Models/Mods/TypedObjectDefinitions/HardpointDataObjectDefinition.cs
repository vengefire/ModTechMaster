namespace ModTechMaster.Data.Models.Mods.TypedObjectDefinitions
{
    using System.Collections.Generic;

    using ModTechMaster.Core.Constants;
    using ModTechMaster.Core.Enums.Mods;
    using ModTechMaster.Core.Interfaces.Models;
    using ModTechMaster.Core.Interfaces.Services;

    using Newtonsoft.Json.Linq;

    public class HardpointDataObjectDefinition : ObjectDefinition
    {
        public HardpointDataObjectDefinition(
            ObjectType objectType,
            IObjectDefinitionDescription objectDescription,
            dynamic jsonObject,
            string filePath,
            IReferenceFinderService referenceFinderService)
            : base(objectType, objectDescription, (JObject)jsonObject, filePath, referenceFinderService)
        {
        }

        public override void AddMetaData()
        {
            base.AddMetaData();
            var prefabWeapons = new HashSet<string>();
            foreach (var data in this.JsonObject.HardpointData)
            {
                this.RecurseWeapons(data.weapons, prefabWeapons);
            }

            this.MetaData.Add(Keywords.PrefabId, new List<string>(prefabWeapons));
        }

        private void RecurseWeapons(dynamic item, HashSet<string> set)
        {
            if (item is JArray)
            {
                foreach (var subItem in item)
                {
                    this.RecurseWeapons(subItem, set);
                }
            }
            else
            {
                set.Add(item.ToString());
            }
        }
    }
}