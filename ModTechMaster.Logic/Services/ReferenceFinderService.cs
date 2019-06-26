namespace ModTechMaster.Logic.Services
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    using Castle.Core.Logging;

    using Framework.Utils.Instrumentation;

    using ModTechMaster.Core.Enums.Mods;
    using ModTechMaster.Core.Interfaces.Models;
    using ModTechMaster.Core.Interfaces.Services;
    using ModTechMaster.Data.Models.Mods.TypedObjectDefinitions;
    using ModTechMaster.Logic.Managers;
    using ModTechMaster.Logic.Processors;

    public class ReferenceFinderService : IReferenceFinderService
    {
        private readonly ILogger logger;

        private readonly ConcurrentDictionary<IReferenceableObject, List<IObjectReference<IReferenceableObject>>> objectReferencesDictionary =
                new ConcurrentDictionary<IReferenceableObject, List<IObjectReference<IReferenceableObject>>>();

        public ReferenceFinderService(ILogger logger)
        {
            this.logger = logger;
        }

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
            IReferenceableObject referenceableObject,
            List<IReferenceableObject> baseReferences)
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
                    baseReferences);

                this.objectReferencesDictionary[referenceableObject] = newReferences;
            }

            return this.objectReferencesDictionary[referenceableObject];
        }

        public long ProcessAllReferences(List<IReferenceableObject> baseReferences)
        {
            var allReferences = this.ReferenceableObjectProvider.GetReferenceableObjects();
            var sw = new Stopwatch();
            sw.Start();

            allReferences.AsParallel().ForAll(
                o =>
                    {
                        var references = this.GetObjectReferences(o, baseReferences);
                    });

            sw.Stop();
            return sw.ElapsedMilliseconds;
        }

        public void ProcessModCollectionLanceSlotEligibility(IModCollection modCollection)
        {
            this.logger.Info("Processing Lance Slot Eligibility...");
            using (var scopedStopwatch = new ScopedStopwatch(this.logger))
            {
                var lanceSlots = modCollection.GetReferenceableObjects()
                    .Where(o => o.ObjectType == ObjectType.LanceDef).Cast<LanceDefObjectDefinition>()
                    .SelectMany(definition => definition.LanceSlots);
                lanceSlots
                    //.AsParallel().ForAll(
                    .ToList().ForEach(
                    o =>
                        {
                            this.logger.Debug($"Processing lance slot eligibility for Lance [{o.LanceDefObjectDefinition.Id}] - Slot [{o.LanceSlotNumber}]...");
                            o.LoadEligibleUnitsAndPilots(modCollection);
                        });
            }
        }

        public long ProcessModCollectionReferences(IModCollection modCollection)
        {
            var allReferences = modCollection.GetReferenceableObjects();
            var baseReferences = modCollection.Mods.FirstOrDefault(mod => mod.IsBattleTech)?.GetReferenceableObjects();
            var sw = new Stopwatch();
            sw.Start();

            allReferences.AsParallel().ForAll(
            //allReferences.ToList().ForEach(
                o =>
                    {
                        var references = this.GetObjectReferences(o, baseReferences);
                    });

            sw.Stop();

            this.ProcessModCollectionLanceSlotEligibility(modCollection);

            return sw.ElapsedMilliseconds;
        }
    }
}