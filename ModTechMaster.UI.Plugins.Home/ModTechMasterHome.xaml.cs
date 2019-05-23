namespace ModTechMaster.UI.Plugins.Home
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Forms;

    using ModTechMaster.Core.Interfaces.Services;
    using ModTechMaster.UI.Core.WinForms.Extensions;
    using ModTechMaster.UI.Plugins.Core.Interfaces;
    using ModTechMaster.UI.Plugins.Home.Commands;
    using ModTechMaster.UI.Plugins.Home.Models;

    using UserControl = System.Windows.Controls.UserControl;

    /// <summary>
    ///     Interaction logic for ModTechMasterHome.xaml
    /// </summary>
    public partial class ModTechMasterHome : UserControl, IPluginControl
    {
        public ModTechMasterHome(IModService modService, IMtmMainModel mainModel)
        {
            this.InitializeComponent();
            this.Model = new HomeModel(modService, mainModel);
            this.PluginCommands = new List<IPluginCommand> { new LoadModsCommand(this.Model) };
            this.DataContext = this.Model;
        }

        public HomeModel Model { get; set; }

        public string ModuleName => @"Home";

        public Type PageType => typeof(ModTechMasterHome);

        public List<IPluginCommand> PluginCommands { get; }

        public object Settings
        {
            get => this.Model.HomeSettings;
            set => this.Model.HomeSettings = value as HomeSettings;
        }

        public Type SettingsType => typeof(HomeSettings);

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