namespace ModTechMaster.Data.Models.Mods
{
    using System.Collections.Generic;
    using System.Linq;

    using ModTechMaster.Core.Constants;
    using ModTechMaster.Core.Enums.Mods;
    using ModTechMaster.Core.Interfaces.Models;

    using Newtonsoft.Json.Linq;

    public class ObjectDefinition : JsonObjectSourcedFromFile, IObjectDefinition
    {
        public ObjectDefinition(
            ObjectType objectType,
            IObjectDefinitionDescription objectDescription,
            dynamic jsonObject,
            string filePath)
            : base(objectType, filePath, (JObject)jsonObject)
        {
            this.ObjectDescription = objectDescription;
            this.MetaData = new Dictionary<string, dynamic>();
            this.Tags = new Dictionary<string, List<string>>();
        }

        public string HumanReadableText => this.JsonString;

        public override string Id => this.MetaData.ContainsKey(Keywords.Id) ? this.MetaData[Keywords.Id] : this.Name;

        public Dictionary<string, dynamic> MetaData { get; }

        public override string Name =>
            this.MetaData.ContainsKey(Keywords.Name) ? this.MetaData[Keywords.Name] : this.SourceFileName;

        public IObjectDefinitionDescription ObjectDescription { get; }

        public Dictionary<string, List<string>> Tags { get; }

        public virtual void AddMetaData()
        {
            if (this.ObjectDescription?.Id != null)
            {
                this.MetaData.Add(Keywords.Id, this.ObjectDescription.Id);
            }
            else if (this.JsonObject?.Id != null)
            {
                this.MetaData.Add(Keywords.Id, this.JsonObject.Id);
            }
            else if (this.JsonObject?.identifier != null)
            {
                this.MetaData.Add(Keywords.Id, this.JsonObject.identifier);
            }
            else if (this.JsonObject?.ID != null)
            {
                this.MetaData.Add(Keywords.Id, this.JsonObject.ID);
            }

            if (this.ObjectDescription?.Name != null)
            {
                this.MetaData.Add(Keywords.Name, this.ObjectDescription.Name);
            }
            else if (this.JsonObject?.Name != null)
            {
                this.MetaData.Add(Keywords.Name, this.JsonObject.Name);
            }

            // If we didn't find a Name in our json store (which may not be there if we're a resource object) then add our default name (FileName)
            if (!this.MetaData.ContainsKey(Keywords.Name))
            {
                this.MetaData.Add(Keywords.Name, this.Name);
            }

            // Similarly if we didnt find an ID, use the name as ID.
            if (!this.MetaData.ContainsKey(Keywords.Id))
            {
                this.MetaData.Add(Keywords.Id, this.Name);
            }

            // Add tag data. Not all relationships are defined via tight IDs. Some are defined by loose tags.
            var jobject = this.JsonObject as JObject;
            var tags = jobject.Properties().FirstOrDefault(property => property.Name.Contains("Tags"))?.Value?.First;
            var tagList = new List<string>();
            if (tags != null)
            {
                foreach (var tagArray in tags.Children())
                {
                    foreach (var tag in tagArray)
                    {
                        tagList.Add(tag.ToString());
                    }
                }
            }

            this.Tags.Add(Keywords.Tags, tagList);
        }
    }
}