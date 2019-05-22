namespace ModTechMaster.UI.Plugins.ModCopy.Nodes
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Windows;
    using Data.Enums;
    using ModTechMaster.Core.Interfaces.Models;

    public interface IMtmTreeViewItem : INotifyPropertyChanged
    {
        bool IsThreeState { get; set;  }
        IReferenceableObjectProvider ReferenceableObjectProvider { get; }
        IMtmTreeViewItem Parent { get; }
        IMtmTreeViewItem TopNode { get; }
        ObservableCollection<IMtmTreeViewItem> Children { get; set; }
        List<IObjectReference<IReferenceableObject>> ObjectReferences { get; }
        List<IObjectReference<IReferenceableObject>> Dependencies { get; }
        List<IObjectReference<IReferenceableObject>> Dependents { get; }
        bool IsSelected { get; set; }
        bool IsExpanded { get; set; }
        Visibility Visibility { get; set; }
        bool HasCheck { get; }
        bool? IsChecked { get; set; }
        SelectionStatus SelectionStatus { get; }
        ObjectStatus ObjectStatus { get; }
        string Name { get; }
        string HumanReadableContent { get; }
        bool Filter(string filterText);
        object Object { get; }
        void Sort();
        long SelectedObjectCount { get; }
    }
}