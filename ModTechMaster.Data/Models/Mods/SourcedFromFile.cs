namespace ModTechMaster.Data.Models.Mods
{
    using System.IO;
    using Core.Interfaces.Models;

    public class SourcedFromFile : ISourcedFromFile
    {
        public SourcedFromFile(string sourceFilePath)
        {
            this.SourceFilePath = sourceFilePath;
        }

        public string SourceFilePath { get; }

        public string SourceDirectoryPath => new FileInfo(this.SourceFilePath).DirectoryName;
        public string SourceFileName => new FileInfo(this.SourceFilePath).Name;
        public string SourceFileExtension => new FileInfo(this.SourceFilePath).Extension;
    }
}