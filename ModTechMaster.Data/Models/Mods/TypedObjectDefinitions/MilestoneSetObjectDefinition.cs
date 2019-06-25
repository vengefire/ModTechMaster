namespace ModTechMaster.Data.Models.Mods.TypedObjectDefinitions
{
    using System.Collections.Generic;

    using ModTechMaster.Core.Constants;
    using ModTechMaster.Core.Enums.Mods;
    using ModTechMaster.Core.Interfaces.Models;
    using ModTechMaster.Core.Interfaces.Services;

    using Newtonsoft.Json.Linq;

    public class MilestoneSetObjectDefinition : ObjectDefinition
    {
        public MilestoneSetObjectDefinition(
            ObjectType objectType,
            IObjectDefinitionDescription objectDescription,
            dynamic jsonObject,
            string filePath,
            IReferenceFinderService referenceFinderService)
            : base(objectType, objectDescription, (JObject)jsonObject, filePath, referenceFinderService)
        {
        }

        public override void AddMetaData()
        {
            base.AddMetaData();
            var milestones = new HashSet<string>();
            var contracts = new HashSet<string>();
            var events = new HashSet<string>();
            var itemCollections = new HashSet<string>();
            milestones.Add(this.JsonObject.StartingMilestoneID.ToString());
            foreach (var milestone in this.JsonObject.Milestones)
            {
                milestones.Add(milestone.Description.Id.ToString());
                foreach (var result in milestone.Results)
                {
                    if (result.Actions == null)
                    {
                        continue;
                    }

                    foreach (var action in result.Actions)
                    {
                        if (action.Type == "Flashpoint_SetNextMilestone")
                        {
                            milestones.Add(action.value.ToString());
                        }
                        else if (action.Type == "Flashpoint_AddContract" || action.Type == "Flashpoint_StartContract")
                        {
                            contracts.Add(action.additionalValues[2].ToString());
                        }
                        else if (action.Type == "ForceEvents")
                        {
                            foreach (var forceEvent in action.ForceEvents)
                            {
                                events.Add(forceEvent.EventID.ToString());
                            }
                        }
                        else if (action.Type == "Flashpoint_CompleteFlashpoint" && action.additionalValues != null)
                        {
                            itemCollections.Add(action.additionalValues[0].ToString());
                        }
                    }
                }
            }

            // this.MetaData.Add(Keywords.MilestoneId, new List<string>(milestones));
            this.MetaData.Add(Keywords.ContractId, new List<string>(contracts));
            this.MetaData.Add(Keywords.EventId, new List<string>(events));
            this.MetaData.Add(Keywords.ItemCollectionId, new List<string>(itemCollections));
        }
    }
}