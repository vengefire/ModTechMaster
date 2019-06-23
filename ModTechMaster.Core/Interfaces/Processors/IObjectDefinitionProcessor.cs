namespace ModTechMaster.Core.Interfaces.Processors
{
    using System.IO;

    using ModTechMaster.Core.Enums.Mods;
    using ModTechMaster.Core.Interfaces.Models;
    using ModTechMaster.Core.Interfaces.Services;

    public interface IObjectDefinitionProcessor
    {
        IObjectDefinition ProcessObjectDefinition(
            IManifestEntry manifestEntry,
            DirectoryInfo di,
            FileInfo fi,
            IReferenceFinderService referenceFinderService,
            ObjectType? objectTypeOverride = null);
    }
}