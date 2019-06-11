namespace ModTechMaster.Logic.Services
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading.Tasks;

    using ModTechMaster.Core.Interfaces.Models;
    using ModTechMaster.Core.Interfaces.Services;
    using ModTechMaster.Logic.Processors;

    public class ReferenceFinderService : IReferenceFinderService
    {
        public long ProcessModCollectionReferences(IModCollection modCollection)
        {
            var dictRefs = new Dictionary<IReferenceableObject, List<IObjectReference<IReferenceableObject>>>();
            var allReferences = modCollection.GetReferenceableObjects();
            var sw = new Stopwatch();
            sw.Start();
            Parallel.ForEach(
                allReferences,
                o =>
                    {
                        dictRefs[o] =
                            CommonReferenceProcessor.FindReferences<IReferenceableObject>(modCollection, o, null);
                    });

            sw.Stop();
            return sw.ElapsedMilliseconds;
        }
    }
}