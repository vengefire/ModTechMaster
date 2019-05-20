using ModTechMaster.Core.Enums.Mods;

namespace ModTechMaster.Data.Models.Mods
{
    using System.IO;
    using Core.Interfaces.Models;
    using Newtonsoft.Json.Linq;

    public abstract class JsonObjectSourcedFromFile : JsonObjectBase, ISourcedFromFile
    {
        public JsonObjectSourcedFromFile(ObjectType objectType, string sourceFilePath, dynamic jsonObject) : base((JObject)jsonObject, objectType)
        {
            this.SourceFilePath = sourceFilePath;
        }

        public string SourceFilePath { get; }

        public string SourceDirectoryPath => new FileInfo(this.SourceFilePath).DirectoryName;
        public string SourceFileName => new FileInfo(this.SourceFilePath).Name;
        public string SourceFileExtension => new FileInfo(this.SourceFilePath).Extension;
    }
}