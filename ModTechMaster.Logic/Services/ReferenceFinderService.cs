using System;
using System.Collections.Generic;
using ModTechMaster.Core.Enums.Mods;
using ModTechMaster.Core.Interfaces.Models;
using ModTechMaster.Logic.Processors;

namespace ModTechMaster.Logic.Services
{
    public class ReferenceFinderService
    {
        public void ProcessModCollectionReferences(IModCollection modCollection)
        {
            var modReferences = new Dictionary<IMod, List<IObjectReference<IMod>>>();
            foreach (var mod in modCollection.Mods)
            {
                var references =
                    CommonReferenceProcessor.FindReferences<IMod>(modCollection, ObjectType.Mod, mod,
                        new List<ObjectType>());
                modReferences.Add(mod, references);
            }

            throw new NotImplementedException();
        }
    }
}