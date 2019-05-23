namespace ModTechMaster.UI.Plugins.ModCopy.Modals.MechSelector
{
    using System;

    public class MechModel
    {
        public int BattleValue { get; set; }

        public string Chassis { get; set; }

        public int Cost { get; set; }

        public int DbId { get; set; }

        public string Designer { get; set; }

        public string Era { get; set; }

        public string Name { get; set; }

        public string Rating { get; set; }

        public string RulesLevel { get; set; }

        public string TechnologyBase { get; set; }

        public int Tonnage { get; set; }

        public int Year { get; set; }

        public static MechModel FromCsv(string csvData, char separator = ',')
        {
            var parts = csvData.Split(separator);
            if (parts.Length != 12)
            {
                throw new ArgumentException("Insufficient columns in CSV data to construct new Mech.", nameof(csvData));
            }

            return new MechModel
                       {
                           DbId = int.Parse(parts[0]),
                           Name = parts[1],
                           Era = parts[2],
                           Year = int.Parse(parts[3]),
                           TechnologyBase = parts[4],
                           Chassis = parts[5],
                           RulesLevel = parts[6],
                           Tonnage = int.Parse(parts[7]),
                           Designer = parts[8],
                           BattleValue = int.Parse(parts[9]),
                           Cost = int.Parse(parts[10]),
                           Rating = parts[11]
                       };
        }
    }
}