using System.Collections.Generic;

namespace ModTechMaster.Core.Interfaces.Models
{
    public interface IModCollection : IReferenceableObjectProvider
    {
        string Name { get; }
        HashSet<IMod> Mods { get; }
        void AddModToCollection(IMod mod);
        void RemoveModFromCollection(IMod mod);
    }
}