namespace ModTechMaster.UI.Plugins.ModCopy.Nodes
{
    using System.Collections.ObjectModel;
    using Data.Enums;

    public interface IMTMTreeViewItem
    {
        IMTMTreeViewItem Parent { get; }
        ObservableCollection<IMTMTreeViewItem> Children { get; }
        bool IsSelected { get; }
        bool IsExpanded { get; }
        bool HasCheck { get; }
        bool? IsChecked { get; set; }
        SelectionStatus SelectionStatus { get; }
        ObjectStatus ObjectStatus { get; }
        string Name { get; }
        string HumanReadableContent { get; }
    }
}