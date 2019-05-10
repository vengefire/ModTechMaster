using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ModTechMaster.Annotations;
using ModTechMaster.Enums;

namespace ModTechMaster.Nodes
{
    public abstract class MTMTreeViewItem : IMTMTreeViewItem, INotifyPropertyChanged
    {
        private bool _isChecked;
        private bool _isExpanded;
        private bool _isSelected;

        public MTMTreeViewItem(IMTMTreeViewItem parent)
        {
            Parent = parent;
        }

        public IMTMTreeViewItem Parent { get; }

        public virtual ObservableCollection<IMTMTreeViewItem> Children { get; } =
            new ObservableCollection<IMTMTreeViewItem>();

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (value != IsSelected)
                {
                    _isSelected = value;
                    OnPropertyChanged("IsSelected");
                }
            }
        }

        public bool IsExpanded
        {
            get => _isExpanded;
            set
            {
                if (value != _isExpanded)
                {
                    _isExpanded = value;
                    OnPropertyChanged("IsExpanded");
                }
            }
        }

        public bool HasCheck => false;

        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                if (value != _isChecked)
                {
                    CheckNode(this, value);
                    OnPropertyChanged("IsChecked");
                }
            }
        }

        public SelectionStatus SelectionStatus { get; set; } = SelectionStatus.Unselected;
        public ObjectStatus ObjectStatus { get; set; } = ObjectStatus.Nominal;
        public abstract string Name { get; }
        public abstract string HumanReadableContent { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        public static void CheckNode(MTMTreeViewItem node, bool value)
        {
            node._isChecked = value;
            foreach (var child in node.Children) child.IsChecked = value;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}