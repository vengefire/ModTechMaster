namespace ModTechMaster.Core.Interfaces.Services
{
    using Models;

    public interface IReferenceFinderService
    {
        long ProcessModCollectionReferences(IModCollection modCollection);
    }
}