namespace ModTechMaster.Core.Interfaces.Models
{
    using System.Collections.Generic;

    public interface IMod : IJsonObjectBase, ISourcedFromFile, IReferenceableObject, IReferenceableObjectProvider
    {
        string Author { get; }

        HashSet<string> ConflictsWith { get; set; }

        string Contact { get; }

        HashSet<string> DependsOn { get; }

        string Description { get; }

        string Dll { get; }

        bool Enabled { get; }

        IManifest Manifest { get; }

        List<IResourceDefinition> ResourceFiles { get; }

        double SizeOnDisk { get; }

        string Version { get; }

        string Website { get; }

        bool IsBattleTech { get; }
    }
}