namespace ModTechMaster.Logic.Services
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using Core.Interfaces.Models;
    using Core.Interfaces.Services;
    using Processors;

    public class ReferenceFinderService : IReferenceFinderService
    {
        public long ProcessModCollectionReferences(IModCollection modCollection)
        {
            var dictRefs = new Dictionary<IReferenceableObject, List<IObjectReference<IReferenceableObject>>>();
            var allReferences = modCollection.GetReferenceableObjects();
            var sw = new Stopwatch();
            sw.Start();
            allReferences.ForEach(o => { dictRefs[o] = CommonReferenceProcessor.FindReferences<IReferenceableObject>(modCollection, o, null); });
            sw.Stop();
            return sw.ElapsedMilliseconds;
        }
    }
}