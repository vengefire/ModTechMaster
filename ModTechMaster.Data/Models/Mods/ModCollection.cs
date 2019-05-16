using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using ModTechMaster.Core.Interfaces.Models;
using ModTechMaster.Data.Annotations;

namespace ModTechMaster.Data.Models.Mods
{
    public class ModCollection : IModCollection
    {
        public ModCollection(string name, string path)
        {
            Name = name;
            Path = path;
        }

        public string Path { get; set; }

        public string Name { get; set;  }
        public HashSet<IMod> Mods { get; } = new HashSet<IMod>();

        public void AddModToCollection(IMod mod)
        {
            if (mod == null) return;
            Mods.Add(mod);
            OnPropertyChanged(nameof(Mods));
            OnPropertyChanged(nameof(ObjectCount));
        }

        public void RemoveModFromCollection(IMod mod)
        {
            throw new NotImplementedException();
        }

        public int ObjectCount => Mods.Sum(mod =>
            mod.Manifest?.Entries.Sum(entry => entry.Objects.Count) ?? 0);

        public List<IReferenceableObject> GetReferenceableObjects()
        {
            var objects = new List<IReferenceableObject>();
            Mods.ToList().ForEach(mod => objects.AddRange(mod.GetReferenceableObjects()));
            return objects;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}