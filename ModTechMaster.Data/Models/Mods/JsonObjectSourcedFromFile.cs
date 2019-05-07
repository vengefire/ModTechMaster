namespace ModTechMaster.Data.Models.Mods
{
    using System.IO;
    using Core.Interfaces.Models;
    using Newtonsoft.Json.Linq;

    public class JsonObjectSourcedFromFile : JsonObjectBase, ISourcedFromFile
    {
        public JsonObjectSourcedFromFile(string sourceFilePath, dynamic jsonObject) : base((JObject)jsonObject)
        {
            this.SourceFilePath = sourceFilePath;
        }

        public string SourceFilePath { get; }

        public string SourceDirectoryPath => new FileInfo(this.SourceFilePath).DirectoryName;
    }
}