namespace ModTechMaster.UI.Plugins.Home.Commands
{
    using System.Threading.Tasks;
    using Core.Interfaces;
    using Models;

    public class LoadModsCommand : AwaitableDelegateCommand<HomeModel>, IPluginCommand
    {
        private readonly HomeModel homeModel;

        public LoadModsCommand(HomeModel homeModel) : base(LoadModsCommand.Execute, LoadModsCommand.CanExecute)
        {
            this.homeModel = homeModel;
        }

        public string Name => @"Load Mods";
        public IPluginCommandCategory Category { get; }
        public object CommandParameter => this.homeModel;

        public static bool CanExecute(HomeModel homeModel)
        {
            return !string.IsNullOrEmpty(homeModel?.HomeSettings?.ModDirectory) &&
                !string.IsNullOrEmpty(homeModel?.HomeSettings?.ModCollectionName);
        }

        public static Task Execute(HomeModel homeModel)
        {
            return Task.Run(() => homeModel.LoadMods());
        }
    }
}