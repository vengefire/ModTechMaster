namespace MTM
{
    using CommandLine;

    public class Options
    {
        [Option('m', "modsPath", Required = true, HelpText = "Mod Collection Path")]
        public string ModsDirectory { get; set; }

        [Option('b', "btPath", Required = false, HelpText = "BattleTech Path")]
        public string BattleTechDirectory { get; set; }

        [Option('v', "val-refs", Required = false, Default = false, HelpText = "Validate Mod References")]
        public bool ValidateRefs { get; set; }

        [Option('l', "val-lances", Required = false, Default = false, HelpText = "Validate Lance Eligibility")]
        public bool ValidateLances { get; set; }

        [Option('j', "val-json", Required = false, Default = false, HelpText = "Pre-validate Mod json files")]
        public bool ValidateJson { get; set; }
    }
}