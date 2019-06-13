namespace MTMScrapeJKRarity
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Newtonsoft.Json;

    internal class Program
    {
        private static void Main(string[] args)
        {
            var targetFolder = args[0];
            var di = new DirectoryInfo(targetFolder);
            var objectAppearanceDate = new List<Tuple<string, string>>();
            di.EnumerateFileSystemInfos("*.json").ToList().ForEach(
                fi =>
                    {
                        dynamic json = JsonConvert.DeserializeObject(File.ReadAllText(fi.FullName));
                        string dateString = json.MinAppearanceDate.ToString();
                        var date = DateTime.Parse(dateString);
                        var year = date.Year;
                        objectAppearanceDate.Add(new Tuple<string, string>(fi.Name, year.ToString()));
                    });
            using (var writer = new StreamWriter("objectAppearanceDates.csv"))
            {
                objectAppearanceDate.ForEach(tuple => writer.WriteLine($"{tuple.Item1},{tuple.Item2}"));
            }
        }
    }
}