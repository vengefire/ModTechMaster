using System.IO;
using ModTechMaster.Core.Interfaces.Models;

namespace ModTechMaster.Data.Models.Mods
{
    public class SourcedFromFile : ISourcedFromFile
    {
        public SourcedFromFile(string sourceFilePath)
        {
            SourceFilePath = sourceFilePath;
        }

        public string SourceFilePath { get; }

        public string SourceDirectoryPath => new FileInfo(SourceFilePath).DirectoryName;
    }
}