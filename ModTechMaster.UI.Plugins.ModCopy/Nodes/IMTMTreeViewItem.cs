namespace ModTechMaster.UI.Plugins.ModCopy.Nodes
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Windows;

    using ModTechMaster.Core.Interfaces.Models;
    using ModTechMaster.UI.Data.Enums;

    public interface IMtmTreeViewItem : INotifyPropertyChanged
    {
        ObservableCollection<IMtmTreeViewItem> AllChildren { get; }

        ObservableCollection<IMtmTreeViewItem> Children { get; set; }

        List<IObjectReference<IReferenceableObject>> Dependencies { get; }

        List<IObjectReference<IReferenceableObject>> Dependents { get; }

        bool HasCheck { get; }

        string HumanReadableContent { get; }

        bool? IsChecked { get; set; }

        bool IsExpanded { get; set; }

        bool IsSelected { get; set; }

        bool IsThreeState { get; set; }

        string Name { get; }

        object Object { get; }

        List<IObjectReference<IReferenceableObject>> ObjectReferences { get; }

        ObjectStatus ObjectStatus { get; }

        IMtmTreeViewItem Parent { get; }

        IReferenceableObjectProvider ReferenceableObjectProvider { get; }

        ObservableCollection<IMtmTreeViewItem> SelectedObjects { get; }

        SelectionStatus SelectionStatus { get; }

        IMtmTreeViewItem TopNode { get; }

        Visibility Visibility { get; set; }

        bool Filter(string filterText);

        void IncestPropertyChanged(object sender, PropertyChangedEventArgs e);

        void SelectAbsentModDependencies();

        void Sort();
    }
}