namespace ModTechMaster.Data.Models.Mods
{
    using System.IO;

    using ModTechMaster.Core.Interfaces.Models;

    public class SourcedFromFile : ISourcedFromFile
    {
        public SourcedFromFile(string sourceFilePath)
        {
            this.SourceFilePath = sourceFilePath;
        }

        public string SourceDirectoryPath => new FileInfo(this.SourceFilePath).DirectoryName;

        public string SourceFileExtension => new FileInfo(this.SourceFilePath).Extension;

        public string SourceFileName => new FileInfo(this.SourceFilePath).Name;

        public string SourceFilePath { get; }

        public string SourceFileNameWithoutExtension
        {
            get
            {
                var fi = new FileInfo(this.SourceFilePath);
                return fi.Name.Substring(0, fi.Name.Length - fi.Extension.Length);
            }
        }
    }
}