namespace ModTechMaster.UI.Plugins.ModCopy.Modals
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Windows;

    using ModTechMaster.Core.Interfaces.Models;

    using Xceed.Wpf.Toolkit;

    /// <summary>
    ///     Interaction logic for SimpleObjectSelectorWindow.xaml
    /// </summary>
    public partial class SimpleObjectSelectorWindow : ChildWindow
    {
        public SimpleObjectSelectorWindow(List<IObjectDefinition> objects, List<IObjectDefinition> selectedObjects)
        {
            this.Objects = new ObservableCollection<IObjectDefinition>(objects);
            this.SelectedItems = new ObservableCollection<IObjectDefinition>(selectedObjects);
            this.InitializeComponent();
            this.DataContext = this;
        }

        public ObservableCollection<IObjectDefinition> Objects { get; }

        public ObservableCollection<IObjectDefinition> SelectedItems { get; set; } = new ObservableCollection<IObjectDefinition>();

        private void ButtonOkOnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ButtonCancelOnClick(object sender, RoutedEventArgs e)
        {
            this.SelectedItems.Clear();
            this.Close();
        }
    }
}