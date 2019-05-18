namespace ModTechMaster.Core.Interfaces.Models
{
    using System.Collections.Generic;

    public interface ICsvObjectBase : IObject
    {
        List<List<string>> CsvData { get; }

        string CsvString { get; }
    }
}