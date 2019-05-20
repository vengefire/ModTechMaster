namespace ModTechMaster.Core.Interfaces.Models
{
    public interface ISourcedFromFile
    {
        string SourceFilePath { get; }

        string SourceDirectoryPath { get; }
        string SourceFileName { get; }
        string SourceFileExtension { get; }
    }
}