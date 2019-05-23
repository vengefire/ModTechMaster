﻿namespace ModTechMaster.UI.Plugins.ModCopy.Nodes
{
    using System.IO;

    using ModTechMaster.Core.Interfaces.Models;

    public class ResourceNode : MtmTreeViewItem
    {
        public ResourceNode(IMtmTreeViewItem parent, IResourceDefinition resourceDefinition)
            : base(parent, resourceDefinition)
        {
        }

        public override string HumanReadableContent
        {
            get
            {
                var data = File.ReadAllText(this.ResourceDefinition.SourceFilePath);
                if (this.ResourceDefinition.SourceFileExtension.ToLower() == ".json")
                {
                    return JsonUtils.JsonPrettify(data);
                }

                return data;
            }
        }

        public override string Name => this.ResourceDefinition.Id;

        private IResourceDefinition ResourceDefinition => this.Object as IResourceDefinition;
    }
}