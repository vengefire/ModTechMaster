using System.IO;
using ModTechMaster.Core.Interfaces.Models;
using Newtonsoft.Json.Linq;

namespace ModTechMaster.Data.Models.Mods
{
    public class JsonObjectSourcedFromFile : JsonObjectBase, ISourcedFromFile
    {
        public JsonObjectSourcedFromFile(string sourceFilePath, dynamic jsonObject) : base((JObject)jsonObject)
        {
            SourceFilePath = sourceFilePath;
        }

        public string SourceFilePath { get; }
        public string SourceDirectoryPath => new FileInfo(SourceFilePath).DirectoryName;
    }
}