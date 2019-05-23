namespace ModTechMaster.UI.Plugins.Core.Interfaces
{
    using Framework.Interfaces.Injection;

    public interface IMtmMainModel
    {
        IContainer Container { get; }

        IPluginControl CurrentPluginControl { get; set; }

        bool IsBusy { get; set; }
    }
}