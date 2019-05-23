namespace ModTechMaster.Core.Interfaces.Services
{
    using ModTechMaster.Core.Interfaces.Models;

    public interface IReferenceFinderService
    {
        long ProcessModCollectionReferences(IModCollection modCollection);
    }
}