namespace ModTechMaster.UI.Plugins.Core.Interfaces
{
    using System;
    using System.Collections.Generic;

    public interface IPlugin
    {
        string Description { get; }

        List<IPluginControl> Modules { get; }

        string Name { get; }

        Type PageType { get; }
    }
}