namespace ModTechMaster.Core.Interfaces.Models
{
    using System.Collections.Generic;
    using System.ComponentModel;

    public interface IModCollection : IReferenceableObjectProvider, INotifyPropertyChanged
    {
        List<IMod> Mods { get; }

        string Name { get; set; }

        int ObjectCount { get; }

        string Path { get; set; }

        void AddModToCollection(IMod mod);

        void RemoveModFromCollection(IMod mod);

        IValidationResult ValidateMods();

        IValidationResult ValidateLances();
    }
}