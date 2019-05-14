using System;
using ModTechMaster.UI.Plugins.Core.Interfaces;
using ModTechMaster.UI.Plugins.Home.Models;

namespace ModTechMaster.UI.Plugins.Home.Commands
{
    public class LoadModsCommand : IPluginCommand
    {
        private readonly HomeModel _homeModel;

        public LoadModsCommand(HomeModel homeModel)
        {
            _homeModel = homeModel;
        }

        public bool CanExecute(object parameter)
        {
            return !string.IsNullOrEmpty(_homeModel.ModDirectory) &&
                   !string.IsNullOrEmpty(_homeModel.ModCollectionName);
        }

        public void Execute(object parameter)
        {
            _homeModel.LoadMods();
        }

        public event EventHandler CanExecuteChanged;
        public string Name => @"Load Mods";
        public IPluginCommandCategory Category { get; }
    }
}