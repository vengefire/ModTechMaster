namespace ModTechMaster.UI.Plugins.Home.Commands
{
    using System;
    using Core.Interfaces;
    using Models;

    public class LoadModsCommand : AwaitableDelegateCommand<HomeModel>, IPluginCommand
    {
        private readonly HomeModel homeModel;

        public LoadModsCommand(HomeModel homeModel) : base(Execute, CanExecute)
        {
            this.homeModel = homeModel;
        }

        public bool CanExecute(object parameter)
        {
            return !string.IsNullOrEmpty(this.homeModel.HomeSettings.ModDirectory) &&
                !string.IsNullOrEmpty(this.homeModel.HomeSettings.ModCollectionName);
        }

        public void Execute(object parameter)
        {
            this.homeModel.LoadMods();
        }

        public event EventHandler CanExecuteChanged;
        public string Name => @"Load Mods";
        public IPluginCommandCategory Category { get; }
    }
}