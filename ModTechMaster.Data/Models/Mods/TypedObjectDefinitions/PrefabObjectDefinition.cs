﻿namespace ModTechMaster.Data.Models.Mods.TypedObjectDefinitions
{
    using Core.Constants;
    using Core.Interfaces.Models;
    using Newtonsoft.Json.Linq;

    public class PrefabObjectDefinition : ObjectDefinition
    {
        public PrefabObjectDefinition(IObjectDefinitionDescription objectDescription, dynamic jsonObject, string filePath) : base(objectDescription, (JObject)jsonObject, filePath)
        {
        }

        public override void AddMetaData()
        {
            base.AddMetaData();
            this.MetaData.Add(Keywords.AssetBundle, this.JsonObject.AssetBundleName);
        }
    }
}