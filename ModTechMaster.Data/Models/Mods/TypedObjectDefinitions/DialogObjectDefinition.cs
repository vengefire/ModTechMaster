namespace ModTechMaster.Data.Models.Mods.TypedObjectDefinitions
{
    using System.Collections.Generic;

    using ModTechMaster.Core.Constants;
    using ModTechMaster.Core.Enums.Mods;
    using ModTechMaster.Core.Interfaces.Models;

    using Newtonsoft.Json.Linq;

    public class DialogObjectDefinition : ObjectDefinition
    {
        public DialogObjectDefinition(
            ObjectType objectType,
            IObjectDefinitionDescription objectDescription,
            dynamic jsonObject,
            string filePath)
            : base(objectType, objectDescription, (JObject)jsonObject, filePath)
        {
        }

        public override void AddMetaData()
        {
            base.AddMetaData();
            var castDefs = new HashSet<string>();
            foreach (var content in this.JsonObject.contents)
            {
                castDefs.Add(content.selectedCastDefId.ToString());
            }

            this.MetaData.Add(Keywords.CastDefId, new List<string>(castDefs));
        }
    }
}