namespace ModTechMaster.UI.Plugins.Core.Interfaces
{
    using Framework.Interfaces.Injection;

    public interface IMtmMainModel
    {
        bool IsBusy { get; set; }
        IContainer Container { get; }
        IPluginControl CurrentPluginControl { get; set; }
    }
}