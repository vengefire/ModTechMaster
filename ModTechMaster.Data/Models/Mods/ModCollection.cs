namespace ModTechMaster.Data.Models.Mods
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using Annotations;
    using Core.Interfaces.Models;

    public class ModCollection : IModCollection
    {
        public ModCollection(string name, string path)
        {
            this.Name = name;
            this.Path = path;
        }

        public string Path { get; set; }

        public string Name { get; set; }
        public List<IMod> Mods { get; } = new List<IMod>();

        public void AddModToCollection(IMod mod)
        {
            if (mod == null)
            {
                return;
            }

            lock(this.Mods)
            {
                this.Mods.Add(mod);
            }

            this.OnPropertyChanged(nameof(ModCollection.Mods));
            this.OnPropertyChanged(nameof(ModCollection.ObjectCount));
        }

        public void RemoveModFromCollection(IMod mod)
        {
            throw new NotImplementedException();
        }

        public int ObjectCount =>
            this.Mods.Sum(
                mod =>
                    mod.Manifest?.Entries.Sum(entry => entry.Objects.Count) ?? 0);

        public List<IReferenceableObject> GetReferenceableObjects()
        {
            var objects = new List<IReferenceableObject>();
            this.Mods.ToList().ForEach(mod => objects.AddRange(mod.GetReferenceableObjects()));
            return objects;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}