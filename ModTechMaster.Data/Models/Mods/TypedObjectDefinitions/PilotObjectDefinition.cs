﻿namespace ModTechMaster.Data.Models.Mods.TypedObjectDefinitions
{
    using System.Collections.Generic;

    using ModTechMaster.Core.Constants;
    using ModTechMaster.Core.Enums.Mods;
    using ModTechMaster.Core.Interfaces.Models;
    using ModTechMaster.Core.Interfaces.Services;

    using Newtonsoft.Json.Linq;

    public class PilotObjectDefinition : ObjectDefinition
    {
        public PilotObjectDefinition(
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

            if (this.JsonObject.abilityDefNames != null)
            {
                var abilityDefs = new List<string>();
                this.MetaData.Add(Keywords.AbilityDefId, abilityDefs);
                foreach (var item in this.JsonObject.abilityDefNames)
                {
                    abilityDefs.Add(item.ToString());
                }
            }
        }
    }
}