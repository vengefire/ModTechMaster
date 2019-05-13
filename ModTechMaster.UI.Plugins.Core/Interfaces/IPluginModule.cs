using System.Collections.Generic;
using System.Windows.Documents;

namespace ModTechMaster.UI.Plugins.Core.Interfaces
{
    using System;

    public interface IPluginModule
    {
        string ModuleName { get; }
        Type PageType { get; }
        List<IPluginCommand> Commands { get; }
    }
}