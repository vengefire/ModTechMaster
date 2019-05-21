namespace ModTechMaster.UI.Plugins.ModCopy.Nodes
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Input;
    using Data.Enums;
    using Logic.Processors;
    using ModTechMaster.Core.Enums;
    using ModTechMaster.Core.Enums.Mods;
    using ModTechMaster.Core.Interfaces.Models;

    //using Annotations;

    public abstract class MtmTreeViewItem : IMtmTreeViewItem
    {
        private static readonly Dictionary<IReferenceableObject, IMtmTreeViewItem> DictRefsToTreeViewItems = new Dictionary<IReferenceableObject, IMtmTreeViewItem>();

        private bool? isChecked = false;
        private bool isExpanded;
        private bool isSelected;

        private bool isThreeState;

        private List<IObjectReference<IReferenceableObject>> objectReferences;
        private Visibility visibility = Visibility.Visible;

        public MtmTreeViewItem(IMtmTreeViewItem parent, object o)
        {
            this.Parent = parent;
            this.Object = o;
            if (this.Object is IReferenceableObject referenceableObject)
            {
                MtmTreeViewItem.DictRefsToTreeViewItems.Add(referenceableObject, this);
            }
        }

        private static void SetExpandedState(IMtmTreeViewItem current, bool state)
        {
            if (!current.Children.Any())
            {
                return;
            }

            current.IsExpanded = state;
            current.Children.ToList().ForEach(item => SetExpandedState(item, state));
        }

        public static ICommand ExpandAllCommand { get; } = new DelegateCommand<IMtmTreeViewItem>(
            node => MtmTreeViewItem.SetExpandedState(node, true), node => node?.Children?.Count > 0);

        public static ICommand CollapseAllCommand { get; } = new DelegateCommand<IMtmTreeViewItem>(
            node => MtmTreeViewItem.SetExpandedState(node, false), node => node?.Children?.Count > 0);

        public IMtmTreeViewItem TopNode
        {
            get
            {
                if (this.Parent == null)
                {
                    return this;
                }

                return this.Parent.TopNode;
            }
        }

        public bool IsThreeState
        {
            get => this.isThreeState;
            set
            {
                if (this.isThreeState == value)
                {
                    return;
                }

                this.isThreeState = value;
                this.OnPropertyChanged(nameof(this.IsThreeState));
            }
        }


        public virtual IReferenceableObjectProvider ReferenceableObjectProvider => throw new NotImplementedException();

        public IMtmTreeViewItem Parent { get; }

        public virtual ObservableCollection<IMtmTreeViewItem> Children { get; set; } =
            new ObservableCollection<IMtmTreeViewItem>();

        public List<IObjectReference<IReferenceableObject>> ObjectReferences
        {
            get
            {
                if (this.objectReferences == null)
                {
                    this.objectReferences = CommonReferenceProcessor.FindReferences<IReferenceableObject>(
                        this.TopNode.ReferenceableObjectProvider, this.Object as IReferenceableObject, new List<ObjectType> {ObjectType.MechDef, ObjectType.Mod}); //TBD
                    this.objectReferences.Sort(
                        (reference, objectReference) => string.CompareOrdinal(reference.ReferenceObject.ObjectType.ToString(), objectReference.ReferenceObject.ObjectType.ToString()));

                    this.OnPropertyChanged();
                    if (this.Dependencies.Any())
                    {
                        this.OnPropertyChanged(nameof(this.Dependencies));
                    }

                    if (this.Dependents.Any())
                    {
                        this.OnPropertyChanged(nameof(this.Dependents));
                    }
                }

                return this.objectReferences;
            }
        }

        public List<IObjectReference<IReferenceableObject>> Dependencies => this.ObjectReferences.Where(reference => reference.ObjectReferenceType == ObjectReferenceType.Dependency).ToList();
        public List<IObjectReference<IReferenceableObject>> Dependents => this.ObjectReferences.Where(reference => reference.ObjectReferenceType == ObjectReferenceType.Dependent).ToList();

        public bool IsSelected
        {
            get => this.isSelected;
            set
            {
                if (value != this.IsSelected)
                {
                    this.isSelected = value;
                    this.OnPropertyChanged(nameof(this.IsSelected));
                }
            }
        }

        public bool IsExpanded
        {
            get => this.isExpanded;
            set
            {
                if (value != this.isExpanded)
                {
                    if (value)
                    {
                        var viewSource = CollectionViewSource.GetDefaultView(this);
                        viewSource?.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
                    }

                    this.isExpanded = value;
                    this.OnPropertyChanged(nameof(this.IsExpanded));
                }
            }
        }

        public Visibility Visibility
        {
            get => this.visibility;
            set
            {
                if (this.visibility != value)
                {
                    this.visibility = value;
                    this.OnPropertyChanged(nameof(MtmTreeViewItem.Visibility));
                }
            }
        }

        public bool HasCheck => false;

        public bool? IsChecked
        {
            get => this.isChecked;
            set
            {
                if (value != this.isChecked)
                {
                    MtmTreeViewItem.CheckNode(this, value);
                    this.PropertyChanged += this.OnPropertyChanged;
                    this.OnPropertyChanged(nameof(this.IsChecked));
                }
            }
        }

        public bool Filter(string filterText)
        {
            if (this.Children.Count > 0)
            {
                var childCollectionView = CollectionViewSource.GetDefaultView(this.Children);
                childCollectionView.Filter = child => ((IMtmTreeViewItem)child).Filter(filterText);
                return !childCollectionView.IsEmpty || this.Name.Contains(filterText) || this.HumanReadableContent.Contains(filterText);
            }

            return this.Name.Contains(filterText) || this.HumanReadableContent.Contains(filterText);
        }

        public void Sort()
        {
            if (this.Children.Count > 0)
            {
                var collectionView = CollectionViewSource.GetDefaultView(this);
                collectionView.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
                this.Children.ToList().ForEach(item => item.Sort());
            }
        }

        public object Object { get; }

        public SelectionStatus SelectionStatus { get; set; } = SelectionStatus.Unselected;
        public ObjectStatus ObjectStatus { get; set; } = ObjectStatus.Nominal;
        public abstract string Name { get; }
        public abstract string HumanReadableContent { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(this.IsChecked))
            {
                if (this.IsChecked == null)
                {
                    this.IsThreeState = true;
                }
                else
                {
                    this.IsThreeState = false;
                }
            }
        }

        public static void CheckNode(MtmTreeViewItem node, bool? value)
        {
            // If we're setting the node to an unspecified state, we don't want to roll that down to our children.
            node.isChecked = value;

            if (value == null)
            {
                return;
            }

            foreach (var child in node.Children) child.IsChecked = value;
            IEnumerable<IObjectReference<IReferenceableObject>> objects;
            objects = value == true
                ? node.ObjectReferences.Where(reference => reference.ObjectReferenceType == ObjectReferenceType.Dependency)
                : node.ObjectReferences.Where(reference => reference.ObjectReferenceType == ObjectReferenceType.Dependent);

            foreach (var objectReference in objects)
            {
                IMtmTreeViewItem treeItem;
                if (MtmTreeViewItem.DictRefsToTreeViewItems.TryGetValue(objectReference.ReferenceObject, out treeItem))
                {
                    treeItem.IsChecked = value;
                }
            }

            MtmTreeViewItem.CheckParentChildrenSelectedState(node);
        }

        private static void CheckParentChildrenSelectedState(IMtmTreeViewItem item)
        {
            if (item.Children.Count == 0)
            {
                var currentNode = item.Parent;
                while (currentNode != null)
                {
                    if (currentNode.Children.All(node => node.IsChecked == true))
                    {
                        currentNode.IsChecked = true;
                    }
                    else if (currentNode.Children.All(Nodes => Nodes.IsChecked == false))
                    {
                        currentNode.IsChecked = false;
                    }
                    else
                    {
                        currentNode.IsChecked = null;
                        // break;
                    }

                    currentNode = currentNode.Parent;
                }
            }
        }

        //[NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}