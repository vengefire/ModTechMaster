namespace ModTechMaster.UI.Plugins.Home.Commands
{
    using System.Threading.Tasks;

    using ModTechMaster.UI.Core.Async;
    using ModTechMaster.UI.Plugins.Core.Interfaces;
    using ModTechMaster.UI.Plugins.Home.Models;

    public class LoadModsCommand : AwaitableDelegateCommand<HomeModel>, IPluginCommand
    {
        private readonly HomeModel homeModel;

        public LoadModsCommand(HomeModel homeModel)
            : base(Execute, CanExecute)
        {
            this.homeModel = homeModel;
        }

        public IPluginCommandCategory Category { get; }

        public object CommandParameter => this.homeModel;

        public string Name => @"Load Mods";

        public static bool CanExecute(HomeModel homeModel)
        {
            return !string.IsNullOrEmpty(homeModel?.HomeSettings?.ModDirectory)
                   && !string.IsNullOrEmpty(homeModel?.HomeSettings?.ModCollectionName);
        }

        public static Task Execute(HomeModel homeModel)
        {
            return Task.Run(() => homeModel.LoadMods());
        }
    }
}