namespace MTMFixer
{
    using System.Collections.Generic;

    internal class Program
    {
        internal static void Main(string[] args)
        {
            var modsFolder = args[0];
            var chassisGearFixer = new ChassisGearToMechFixer();
            chassisGearFixer.MoveFixedEquipmentFromChassisToMechDef(modsFolder, new List<string> { "cockpit" });
        }
    }
}