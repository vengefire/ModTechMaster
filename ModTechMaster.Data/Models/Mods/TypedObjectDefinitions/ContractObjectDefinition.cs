using System.Collections.Generic;
using System.Linq;
using ModTechMaster.Core.Constants;
using ModTechMaster.Core.Enums.Mods;
using ModTechMaster.Core.Interfaces.Models;
using Newtonsoft.Json.Linq;

namespace ModTechMaster.Data.Models.Mods.TypedObjectDefinitions
{
    public class ContractObjectDefinition : ObjectDefinition
    {
        public ContractObjectDefinition(ObjectType objectType, IObjectDefinitionDescription objectDescription,
            dynamic jsonObject, string filePath) : base(objectType, objectDescription, (JObject) jsonObject, filePath)
        {
        }

        public override void AddMetaData()
        {
            base.AddMetaData();
            var castList = new HashSet<string>();
            foreach (var dialog in JsonObject.dialogueList)
            {
                foreach (var dialogueContent in dialog.dialogueContent)
                castList.Add(dialogueContent.SelectedCastDefId);
            }
            MetaData.Add(Keywords.CastDefId, new List<string>(castList));

            // castDef_TeamLeader_Current?
            // selectedLanceDefId
            // pilotDefId
            // lanceOverrideList

            MetaData.Add(Keywords.DialogBucketId, JsonObject.overrideAutoCompleteDialogueId);
        }
    }
}