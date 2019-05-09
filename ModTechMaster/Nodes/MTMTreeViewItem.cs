using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ModTechMaster.Annotations;
using ModTechMaster.Enums;

namespace ModTechMaster.Nodes
{
    public class MTMTreeViewItem : IMTMTreeViewItem, INotifyPropertyChanged
    {
        private bool _isChecked;
        private bool _isExpanded;
        private bool _isSelected;

        public MTMTreeViewItem(IMTMTreeViewItem parent)
        {
            Parent = parent;
        }

        public IMTMTreeViewItem Parent { get; }
        public List<IMTMTreeViewItem> Children { get; } = new List<IMTMTreeViewItem>();

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
                    _isChecked = value;
                    OnPropertyChanged("IsChecked");
                }
            }
        }

        public SelectionStatus SelectionStatus { get; set; } = SelectionStatus.Unselected;
        public ObjectStatus ObjectStatus { get; set; } = ObjectStatus.Nominal;

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}