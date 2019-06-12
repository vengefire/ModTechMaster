namespace ModTechMaster.Logic.Services
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    using ModTechMaster.Core.Interfaces.Models;
    using ModTechMaster.Core.Interfaces.Services;
    using ModTechMaster.Logic.Processors;

    public class ReferenceFinderService : IReferenceFinderService
    {
        private readonly ConcurrentDictionary<IReferenceableObject, List<IObjectReference<IReferenceableObject>>> objectReferencesDictionary =
                new ConcurrentDictionary<IReferenceableObject, List<IObjectReference<IReferenceableObject>>>();

        public IReferenceableObjectProvider ReferenceableObjectProvider { get; set; }

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

            /*var dependencyRels = RelationshipManager.GetDependenciesRelationshipsForType(referenceableObject.ObjectType);
            var dependantRels = RelationshipManager.GetDependantRelationShipsForType(referenceableObject.ObjectType);
            var allRels = new List<IObjectRelationship>(dependencyRels);
            allRels.AddRange(dependantRels);

            var cntRels = allRels.Count;

            if (!this.objectReferencesDictionary.ContainsKey(referenceableObject) || this.objectReferencesDictionary[referenceableObject].Count != cntRels)
            {
                var newReferences = CommonReferenceProcessor.FindReferences<IReferenceableObject>(this.referenceableObjectProvider, referenceableObject, null);

                if (!this.objectReferencesDictionary.ContainsKey(referenceableObject))
                {
                    lock (this.objectReferencesDictionary)
                    {
                        this.objectReferencesDictionary.Add(
                            referenceableObject,
                            new List<IObjectReference<IReferenceableObject>>());
                    }
                }

                var references = this.objectReferencesDictionary[referenceableObject];

                var nonExistingReferences = allRels
                    .Except(references.Select(reference => reference.Relationship))
                    .ToList()
                    .Select(relationship => new ObjectReference<IReferenceableObject>(null, dependantRels.Contains(relationship) ? ObjectReferenceType.Dependent : ObjectReferenceType.Dependency, relationship, false, false))
                    .ToList();

                references.AddRange(newReferences);
                references.AddRange(nonExistingReferences);

                this.objectReferencesDictionary[referenceableObject] = references;

                var reverseReferences = newReferences.Select(
                    reference => new ObjectReference<IReferenceableObject>(
                        reference.ReferenceObject,
                        reference.ObjectReferenceType == ObjectReferenceType.Dependency ? ObjectReferenceType.Dependent : ObjectReferenceType.Dependency,
                        new Relationship(
                            reference.Relationship.DependencyKey,
                            reference.Relationship.DependentKey,
                            !reference.Relationship.HasMultipleDependencies),
                        false,
                        true)).ToList();

                reverseReferences.ForEach(
                    reverseReference =>
                        {
                            if (!this.objectReferencesDictionary.ContainsKey(reverseReference.ReferenceObject))
                            {
                                lock (this.objectReferencesDictionary)
                                {
                                    this.objectReferencesDictionary.Add(
                                        reverseReference.ReferenceObject,
                                        new List<IObjectReference<IReferenceableObject>>());
                                }
                            }

                            var reverseReferencesList = this.objectReferencesDictionary[reverseReference.ReferenceObject];
                            reverseReferencesList.Add(reverseReference);
                        });
            }*/
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