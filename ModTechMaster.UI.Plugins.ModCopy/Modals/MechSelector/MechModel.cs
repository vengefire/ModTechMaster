namespace ModTechMaster.UI.Plugins.ModCopy.Modals.MechSelector
{
    using System;

    public class MechModel
    {
        public long BattleValue { get; set; }

        public string Chassis { get; set; }

        public long Cost { get; set; }

        public long DbId { get; set; }

        public string Designer { get; set; }

        public string Era { get; set; }

        public bool Extinct { get; set; }

        public bool Selected { get; set; }

        public string Name { get; set; }

        public string Rating { get; set; }

        public string RulesLevel { get; set; }

        public string TechnologyBase { get; set; }

        public long Tonnage { get; set; }

        public long Year { get; set; }

        public static MechModel FromCsv(string csvData, char separator = ',')
        {
            var parts = csvData.Split(separator);
            if (parts.Length != 12)
            {
                throw new ArgumentException("Insufficient columns in CSV data to construct new Mech.", nameof(csvData));
            }

            return new MechModel
                       {
                           DbId = long.Parse(parts[0]),
                           Name = parts[1],
                           Era = parts[2],
                           Year = long.Parse(parts[3]),
                           TechnologyBase = parts[4],
                           Chassis = parts[5],
                           RulesLevel = parts[6],
                           Tonnage = long.Parse(parts[7]),
                           Designer = parts[8],
                           BattleValue = long.Parse(parts[9]),
                           Cost = long.Parse(parts[10]),
                           Rating = parts[11],
                           Extinct = parts[6].Contains("Extinct"),
                           Selected = false
                       };
        }
    }
}