namespace ModTechMaster.Core.Interfaces.Processors
{
    using System.IO;
    using Models;

    public interface IObjectDefinitionProcessor
    {
        IObjectDefinition ProcessObjectDefinition(IManifestEntry manifestEntry, DirectoryInfo di, FileInfo fi);
    }
}