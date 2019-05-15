using System;
using System.ComponentModel;
using System.Windows;

namespace ModTechMaster.UI.Plugins.ModCopy.Nodes
{
    using System.Collections.ObjectModel;
    using Data.Enums;

    public interface IMTMTreeViewItem : INotifyPropertyChanged
    {
        IMTMTreeViewItem Parent { get; }
        ObservableCollection<IMTMTreeViewItem> Children { get; set; }
        bool IsSelected { get; }
        bool IsExpanded { get; }
        Visibility Visibility { get; }
        bool HasCheck { get; }
        bool? IsChecked { get; set; }
        SelectionStatus SelectionStatus { get; }
        ObjectStatus ObjectStatus { get; }
        string Name { get; }
        string HumanReadableContent { get; }
        bool Filter(string filterText);
    }
}