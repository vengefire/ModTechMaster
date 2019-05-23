namespace ModTechMaster.Core.Interfaces.Processors
{
    using System.IO;

    using ModTechMaster.Core.Interfaces.Models;

    public interface IObjectDefinitionProcessor
    {
        IObjectDefinition ProcessObjectDefinition(IManifestEntry manifestEntry, DirectoryInfo di, FileInfo fi);
    }
}