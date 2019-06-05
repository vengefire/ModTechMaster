namespace ModTechMaster.Data.Models.Mods
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using ModTechMaster.Core.Constants;
    using ModTechMaster.Core.Enums.Mods;
    using ModTechMaster.Core.Interfaces.Models;

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
                            s = s.Trim().Trim(new[] { '\r', '\n' });
                            var lineData = new List<string>();
                            s.Split(',').ToList().ForEach(s1 => lineData.Add(s1));
                            if (lineData.Count >= 4)
                            {
                                this.CsvData.Add(lineData);
                            }
                        }
                    });
        }

        public List<List<string>> CsvData { get; }

        public string CsvString { get; }

        public string HumanReadableText => this.CsvString;

        public abstract string Id { get; }

        public Dictionary<string, object> MetaData { get; } = new Dictionary<string, object>();

        public abstract string Name { get; }

        public IObjectDefinitionDescription ObjectDescription => null;

        public ObjectType ObjectType { get; }

        public string SourceDirectoryPath => new FileInfo(this.SourceFilePath).DirectoryName;

        public string SourceFileExtension => new FileInfo(this.SourceFilePath).Extension;

        public string SourceFileName => new FileInfo(this.SourceFilePath).Name;

        public string SourceFilePath { get; }

        public string SourceFileNameWithoutExtension
        {
            get
            {
                var fi = new FileInfo(this.SourceFilePath);
                return fi.Name.Substring(0, fi.Name.Length - fi.Extension.Length);
            }
        }

        public virtual void AddMetaData()
        {
            this.MetaData.Add(Keywords.Id, this.CsvData[0][0]);
            this.MetaData.Add(Keywords.Name, this.CsvData[0][0]);
        }
    }
}