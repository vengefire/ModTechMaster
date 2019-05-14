using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ModTechMaster.Core.Interfaces.Services;
using ModTechMaster.UI.Core.WinForms.Extensions;
using ModTechMaster.UI.Plugins.Core.Interfaces;
using ModTechMaster.UI.Plugins.Home.Commands;
using ModTechMaster.UI.Plugins.Home.Models;
using UserControl = System.Windows.Controls.UserControl;

namespace ModTechMaster.UI.Plugins.Home
{
    /// <summary>
    ///     Interaction logic for ModTechMasterHome.xaml
    /// </summary>
    public partial class ModTechMasterHome : UserControl, IPluginControl
    {
        private HomeModel Model { get; set; }

        public ModTechMasterHome(IModService modService )
        {
            InitializeComponent();

            Model = new HomeModel(modService)
            {
                ModDirectory = @"Some previously selected value from saved settings?",
                ModCollectionName = @"My mod collection"
            };

            PluginCommands = new List<IPluginCommand>()
            {
                new LoadModsCommand(Model)
            };

            this.DataContext = Model;
        }

        public string ModuleName => @"Home";
        public Type PageType => typeof(ModTechMasterHome);
        public List<IPluginCommand> PluginCommands { get; }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            using (var folderDialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                var result = folderDialog.ShowDialog(this.GetIWin32Window());
                if (result ==  DialogResult.OK)
                {
                    Model.ModDirectory = folderDialog.SelectedPath;
                }
            }
        }
    }
}