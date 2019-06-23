namespace ModTechMaster.Data.Models.Mods.TypedObjectDefinitions
{
    using System;

    using ModTechMaster.Core.Constants;
    using ModTechMaster.Core.Enums.Mods;
    using ModTechMaster.Core.Interfaces.Models;
    using ModTechMaster.Core.Interfaces.Services;

    using Newtonsoft.Json.Linq;

    public class CcDefaultsObjectDefinition : ObjectDefinition
    {
        public CcDefaultsObjectDefinition(
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
            this.MetaData.Add(Keywords.CategoryId, this.JsonObject.CategoryID);
            switch (this.JsonObject.Type.ToString())
            {
                case "Upgrade":
                    this.MetaData.Add(Keywords.UpgradeDefId, this.JsonObject.DefID);
                    break;
                case "HeatSink":
                    this.MetaData.Add(Keywords.HeatSinkDefId, this.JsonObject.DefID);
                    break;
                default:
                    throw new InvalidProgramException(
                        $"Unknown CC default type encountered [{this.JsonObject.Type.ToString()}");
            }
        }
    }
}