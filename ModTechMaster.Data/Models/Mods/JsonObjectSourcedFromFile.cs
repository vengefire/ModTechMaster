namespace ModTechMaster.Data.Models.Mods
{
    using System.IO;

    using ModTechMaster.Core.Enums.Mods;
    using ModTechMaster.Core.Interfaces.Models;

    using Newtonsoft.Json.Linq;

    public abstract class JsonObjectSourcedFromFile : JsonObjectBase, ISourcedFromFile
    {
        public JsonObjectSourcedFromFile(ObjectType objectType, string sourceFilePath, dynamic jsonObject)
            : base((JObject)jsonObject, objectType)
        {
            this.SourceFilePath = sourceFilePath;
        }

        public string SourceDirectoryPath => new FileInfo(this.SourceFilePath).DirectoryName;

        public string SourceFileExtension => new FileInfo(this.SourceFilePath).Extension;

        public string SourceFileName => new FileInfo(this.SourceFilePath).Name;

        public string SourceFilePath { get; }
    }
}