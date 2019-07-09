namespace ModTechMaster.UI.Plugins.Home.Models
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    using ModTechMaster.Core.Enums;
    using ModTechMaster.Core.Interfaces.Models;
    using ModTechMaster.Core.Interfaces.Services;
    using ModTechMaster.UI.Plugins.Core.Interfaces;
    using ModTechMaster.UI.Plugins.Home.Annotations;

    public class HomeModel : INotifyPropertyChanged
    {
        private readonly IMtmMainModel mainModel;

        private readonly IModService modService;

        public HomeModel(IModService modService, IMtmMainModel mainModel, IMessageService messageService)
        {
            this.MessageService = messageService;
            this.modService = modService;
            this.mainModel = mainModel;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public HomeSettings HomeSettings { get; set; }

        public IMessageService MessageService { get; }

        public IModCollection ModCollection { get; private set; }

        public IModCollection LoadMods()
        {
            try
            {
                this.MessageService.PushMessage(
                    $"Started loading mods from [{this.HomeSettings.ModDirectory}]...",
                    MessageType.Info);
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                this.mainModel.IsBusy = true;
                var modCollection = this.modService.LoadCollectionFromPath(
                    //@"C:\Games\Steam\steamapps\common\BATTLETECH",
                    @"",
                    this.HomeSettings.ModDirectory,
                    this.HomeSettings.ModCollectionName);
                stopwatch.Stop();
                this.MessageService.PushMessage(
                    $"Loading mods from disk took {stopwatch.ElapsedMilliseconds}ms.",
                    MessageType.Info);
                this.ModCollection = modCollection;
                this.OnPropertyChanged(nameof(this.ModCollection));
                return this.ModCollection;
            }
            catch (Exception ex)
            {
                this.MessageService.PushMessage(ex.Message, MessageType.Error);
                return null;
            }
            finally
            {
                this.mainModel.IsBusy = false;
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}