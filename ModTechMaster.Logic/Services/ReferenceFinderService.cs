namespace ModTechMaster.Logic.Services
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    using ModTechMaster.Core.Enums.Mods;
    using ModTechMaster.Core.Interfaces.Models;
    using ModTechMaster.Core.Interfaces.Services;
    using ModTechMaster.Logic.Managers;
    using ModTechMaster.Logic.Processors;

    public class ReferenceFinderService : IReferenceFinderService
    {
        private readonly ConcurrentDictionary<IReferenceableObject, List<IObjectReference<IReferenceableObject>>> objectReferencesDictionary =
                new ConcurrentDictionary<IReferenceableObject, List<IObjectReference<IReferenceableObject>>>();

        public IReferenceableObjectProvider ReferenceableObjectProvider { get; set; }

        public List<IObjectRelationship> GetDependencyRelationships(ObjectType objectType)
        {
            return RelationshipManager.GetDependenciesRelationshipsForType(objectType);
        }

        public List<IObjectRelationship> GetDependentRelationships(ObjectType objectType)
        {
            return RelationshipManager.GetDependentRelationShipsForType(objectType);
        }

        public List<IObjectReference<IReferenceableObject>> GetObjectReferences(
            IReferenceableObject referenceableObject)
        {
            if (referenceableObject == null)
            {
                return new List<IObjectReference<IReferenceableObject>>();
            }

            if (!this.objectReferencesDictionary.ContainsKey(referenceableObject))
            {
                var newReferences = CommonReferenceProcessor.FindReferences<IReferenceableObject>(
                    this.ReferenceableObjectProvider,
                    referenceableObject,
                    null);

                this.objectReferencesDictionary[referenceableObject] = newReferences;
            }

            return this.objectReferencesDictionary[referenceableObject];
        }

        public long ProcessAllReferences()
        {
            var allReferences = this.ReferenceableObjectProvider.GetReferenceableObjects();
            var sw = new Stopwatch();
            sw.Start();

            allReferences.AsParallel().ForAll(
                o =>
                    {
                        var references = this.GetObjectReferences(o);
                    });

            sw.Stop();
            return sw.ElapsedMilliseconds;
        }

        public long ProcessModCollectionReferences(IModCollection modCollection)
        {
            var allReferences = modCollection.GetReferenceableObjects();
            var sw = new Stopwatch();
            sw.Start();

            allReferences.AsParallel().ForAll(
            // allReferences.ToList().ForEach(
                o =>
                    {
                        // allReferences.ForEach(o =>
                        var references = this.GetObjectReferences(o);
                    });

            sw.Stop();
            return sw.ElapsedMilliseconds;
        }
    }
}