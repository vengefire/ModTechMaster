namespace ModTechMaster.UI.Plugins.Core.Interfaces
{
    using System;

    public interface IPluginModule
    {
        string ModuleName { get; }
        Type PageType { get; }
    }
}