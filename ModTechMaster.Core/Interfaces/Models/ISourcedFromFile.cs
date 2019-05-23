namespace ModTechMaster.Core.Interfaces.Models
{
    public interface ISourcedFromFile
    {
        string SourceDirectoryPath { get; }

        string SourceFileExtension { get; }

        string SourceFileName { get; }

        string SourceFilePath { get; }
    }
}