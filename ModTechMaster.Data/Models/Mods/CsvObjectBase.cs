namespace ModTechMaster.Data.Models.Mods
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Core.Constants;
    using Core.Enums.Mods;
    using Core.Interfaces.Models;

    public abstract class CsvObjectBase : ICsvObjectBase, IObjectDefinition
    {
        public CsvObjectBase(ObjectType objectType, string sourceFilePath, string csvString)
        {
            this.ObjectType = objectType;
            this.SourceFilePath = sourceFilePath;

            this.CsvString = csvString;
            this.CsvData = new List<List<string>>();
            this.CsvString.Split('\n').ToList().ForEach(
                s =>
                {
                    if (!string.IsNullOrEmpty(s.Trim()))
                    {
                        var lineData = new List<string>();
                        s.Split(',').ToList().ForEach(s1 => lineData.Add(s1));
                        if (lineData.Count >= 4)
                        {
                            this.CsvData.Add(lineData);
                        }
                    }
                });
        }

        public ObjectType ObjectType { get; }
        public abstract string Name { get; }
        public abstract string Id { get; }
        public List<List<string>> CsvData { get; }
        public string CsvString { get; }
        public Dictionary<string, object> MetaData { get; } = new Dictionary<string, object>();

        public virtual void AddMetaData()
        {
            this.MetaData.Add(Keywords.Id, this.CsvData[0][0]);
            this.MetaData.Add(Keywords.Name, this.CsvData[0][0]);
        }

        public string SourceFilePath { get; }

        public string SourceDirectoryPath => new FileInfo(this.SourceFilePath).DirectoryName;
        public string SourceFileName => new FileInfo(this.SourceFilePath).Name;
        public string SourceFileExtension => new FileInfo(this.SourceFilePath).Extension;

        public IObjectDefinitionDescription ObjectDescription => null;
        public string HumanReadableText => this.CsvString;
    }
}