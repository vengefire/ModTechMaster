namespace ModTechMaster.Data.Models.Mods
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;

    using ModTechMaster.Core.Interfaces.Models;
    using ModTechMaster.Data.Annotations;

    public class ModCollection : IModCollection
    {
        public ModCollection(string name, string path)
        {
            this.Name = name;
            this.Path = path;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public List<IMod> Mods { get; } = new List<IMod>();

        public string Name { get; set; }

        public int ObjectCount => this.Mods.Sum(mod => mod.Manifest?.Entries.Sum(entry => entry.Objects.Count) ?? 0);

        public string Path { get; set; }

        public void AddModToCollection(IMod mod)
        {
            if (mod == null)
            {
                return;
            }

            lock (this.Mods)
            {
                this.Mods.Add(mod);
            }

            this.OnPropertyChanged(nameof(this.Mods));
            this.OnPropertyChanged(nameof(this.ObjectCount));
        }

        public List<IReferenceableObject> GetReferenceableObjects()
        {
            var objects = new List<IReferenceableObject>();
            this.Mods.ToList().ForEach(
                mod =>
                    {
                        objects.AddRange(mod.GetReferenceableObjects());
                        objects.AddRange(mod.ResourceFiles);
                    });
            return objects;
        }

        public void RemoveModFromCollection(IMod mod)
        {
            throw new NotImplementedException();
        }

        public IValidationResult ValidateMods()
        {
            return ValidationResult.AggregateResults(this.Mods.Select(mod => mod.ValidateObject()));
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}