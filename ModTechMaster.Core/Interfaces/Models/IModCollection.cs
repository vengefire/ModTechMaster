using System.Collections.Generic;
using System.ComponentModel;

namespace ModTechMaster.Core.Interfaces.Models
{
    public interface IModCollection : IReferenceableObjectProvider, INotifyPropertyChanged
    {
        string Name { get; set; }
        HashSet<IMod> Mods { get; }
        string Path { get; set;  }
        void AddModToCollection(IMod mod);
        void RemoveModFromCollection(IMod mod);
        int ObjectCount { get; }
    }
}