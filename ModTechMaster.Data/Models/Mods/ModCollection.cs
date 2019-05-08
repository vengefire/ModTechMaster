using System;
using System.Collections.Generic;
using System.Linq;
using ModTechMaster.Core.Interfaces.Models;

namespace ModTechMaster.Data.Models.Mods
{
    public class ModCollection : IModCollection
    {
        public ModCollection(string name)
        {
            Name = name;
        }

        public string Name { get; }
        public HashSet<IMod> Mods { get; } = new HashSet<IMod>();
        public void AddModToCollection(IMod mod)
        {
            if (mod == null)
            {
                // TODO : Add Warning here...
                return;
            }
            this.Mods.Add(mod);
        }

        public void RemoveModFromCollection(IMod mod)
        {
            throw new System.NotImplementedException();
        }

        public List<IReferenceableObject> GetReferenceableObjects()
        {
            var objects = new List<IReferenceableObject>();
            Mods.ToList().ForEach(mod => objects.AddRange(mod.GetReferenceableObjects()));
            return objects;
        }
    }
}