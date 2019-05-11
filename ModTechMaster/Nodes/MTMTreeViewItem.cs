namespace ModTechMaster.Nodes
{
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using Annotations;
    using Enums;

    public abstract class MTMTreeViewItem : IMTMTreeViewItem, INotifyPropertyChanged
    {
        private bool? _isChecked = false;
        private bool _isExpanded;
        private bool _isSelected;

        public MTMTreeViewItem(IMTMTreeViewItem parent)
        {
            this.Parent = parent;
        }

        public IMTMTreeViewItem Parent { get; }

        public virtual ObservableCollection<IMTMTreeViewItem> Children { get; } =
            new ObservableCollection<IMTMTreeViewItem>();

        public bool IsSelected
        {
            get => this._isSelected;
            set
            {
                if (value != this.IsSelected)
                {
                    this._isSelected = value;
                    this.OnPropertyChanged("IsSelected");
                }
            }
        }

        public bool IsExpanded
        {
            get => this._isExpanded;
            set
            {
                if (value != this._isExpanded)
                {
                    this._isExpanded = value;
                    this.OnPropertyChanged("IsExpanded");
                }
            }
        }

        public bool HasCheck => false;

        public bool? IsChecked
        {
            get => this?._isChecked;
            set
            {
                if (value != this._isChecked)
                {
                    MTMTreeViewItem.CheckNode(this, value);
                    this.OnPropertyChanged("IsChecked");
                }
            }
        }

        public SelectionStatus SelectionStatus { get; set; } = SelectionStatus.Unselected;
        public ObjectStatus ObjectStatus { get; set; } = ObjectStatus.Nominal;
        public abstract string Name { get; }
        public abstract string HumanReadableContent { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        public static void CheckNode(MTMTreeViewItem node, bool? value)
        {
            node._isChecked = value;
            foreach (var child in node.Children) child.IsChecked = value;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}