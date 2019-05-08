using System.Collections.Generic;
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
            this.Mods.Add(mod);
        }

        public void RemoveModFromCollection(IMod mod)
        {
            throw new System.NotImplementedException();
        }
    }
}