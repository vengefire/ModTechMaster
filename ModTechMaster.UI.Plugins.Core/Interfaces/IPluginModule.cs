namespace ModTechMaster.UI.Plugins.Core.Interfaces
{
    using System;

    public interface IPluginModule
    {
        string ModuleName { get; }
        string PageSource { get; }
        Type PageType { get; }
    }
}