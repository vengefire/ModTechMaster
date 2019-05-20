namespace ModTechMaster.Core.Interfaces.Models
{
    using System.Collections.Generic;

    public interface IMod : IJsonObjectBase, ISourcedFromFile, IReferenceableObject, IReferenceableObjectProvider
    {
        bool Enabled { get; }

        string Version { get; }

        string Description { get; }

        string Author { get; }

        string Website { get; }

        string Contact { get; }

        IManifest Manifest { get; }

        HashSet<string> DependsOn { get; }

        HashSet<string> ConflictsWith { get; set; }
        double SizeOnDisk { get; }
        string Dll { get; }
        List<IResourceDefinition> ResourceFiles { get; }
    }
}