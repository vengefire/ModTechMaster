namespace ModTechMaster.UI.Plugins.ModCopy
{
    using System;
    using System.Threading.Tasks;
    using System.Windows.Controls;
    using Core.Interfaces;

    /// <summary>
    ///     Interaction logic for ModCopyPage.xaml
    /// </summary>
    public partial class ModCopyPage : Page, IPluginModule
    {
        public ModCopyPage()
        {
            this.InitializeComponent();
        }

        private async void TextBoxBase_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var tb = (TextBox)sender;
            var startLength = tb.Text.Length;

            await Task.Delay(300);
            if (startLength == tb.Text.Length)
            {
                Console.WriteLine("Run Filter");
            }
        }

        public string ModuleName => @"ModTechMaster - Mod Copy Module";
        public string PageSource => @"ModCopyPage.xaml";
    }
}