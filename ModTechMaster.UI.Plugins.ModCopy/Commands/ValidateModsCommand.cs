﻿using System;
using ModTechMaster.UI.Plugins.Core.Interfaces;

namespace ModTechMaster.UI.Plugins.ModCopy.Commands
{
    public class ValidateModsCommand : IPluginCommand
    {
        public ValidateModsCommand(IPluginCommandCategory category)
        {
            Category = category;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            throw new NotImplementedException();
        }

        public event EventHandler CanExecuteChanged;
        public string Name => @"Validate Mods";
        public IPluginCommandCategory Category { get; }
        public object CommandParameter => null;
    }
}