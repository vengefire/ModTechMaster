using ModTechMaster.Core.Interfaces.Services;
using ModTechMaster.Logic.Services;

namespace MTM
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            IModService modService = new ModService(new MessageService());
            var mod = modService.TryLoadFromPath(@"C:\BattleTech\Mods\RogueModuleTech");
        }
    }
}