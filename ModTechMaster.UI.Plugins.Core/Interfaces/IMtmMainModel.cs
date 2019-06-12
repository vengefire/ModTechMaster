namespace ModTechMaster.UI.Plugins.Core.Interfaces
{
    using Framework.Interfaces.Injection;

    using ModTechMaster.Core.Interfaces.Services;

    public interface IMtmMainModel
    {
        IContainer Container { get; }

        IPluginControl CurrentPluginControl { get; set; }

        bool IsBusy { get; set; }

        IMessageService MessageService { get; }
    }
}