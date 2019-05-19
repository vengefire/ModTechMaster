namespace ModTechMaster.UI.Plugins.Home
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Forms;
    using Commands;
    using Core.Interfaces;
    using Models;
    using ModTechMaster.Core.Interfaces.Services;
    using UI.Core.WinForms.Extensions;
    using UserControl = System.Windows.Controls.UserControl;

    /// <summary>
    ///     Interaction logic for ModTechMasterHome.xaml
    /// </summary>
    public partial class ModTechMasterHome : UserControl, IPluginControl
    {
        public ModTechMasterHome(IModService modService, ISettingsService settingsService, IMtmMainModel mainModel)
        {
            this.InitializeComponent();
            this.Model = new HomeModel(modService, settingsService, mainModel);
            this.PluginCommands = new List<IPluginCommand>
            {
                new LoadModsCommand(this.Model),
                new SaveSettingsCommand(settingsService, this.Model.HomeSettings)
            };
            this.DataContext = this.Model;
        }

        public HomeModel Model { get; set; }

        public string ModuleName => @"Home";
        public Type PageType => typeof(ModTechMasterHome);
        public List<IPluginCommand> PluginCommands { get; }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            using (var folderDialog = new FolderBrowserDialog())
            {
                var result = folderDialog.ShowDialog(this.GetIWin32Window());
                if (result == DialogResult.OK)
                {
                    this.Model.HomeSettings.ModDirectory = folderDialog.SelectedPath;
                }
            }
        }
    }
}