using System;
using System.Collections.Generic;

namespace ModTechMaster.UI.Plugins.Core.Interfaces
{
    public interface IPlugin
    {
        string Name { get; }
        string Description { get; }
        Type PageType { get; }
        List<IPluginControl> Modules { get; }
    }
}