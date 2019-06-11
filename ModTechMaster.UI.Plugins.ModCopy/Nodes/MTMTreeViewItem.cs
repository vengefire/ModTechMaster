namespace ModTechMaster.UI.Plugins.ModCopy.Nodes
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Input;

    using ModTechMaster.Core.Enums;
    using ModTechMaster.Core.Enums.Mods;
    using ModTechMaster.Core.Interfaces.Models;
    using ModTechMaster.Logic.Processors;
    using ModTechMaster.UI.Data.Enums;

    public abstract class MtmTreeViewItem : IMtmTreeViewItem
    {
        public static readonly Dictionary<IReferenceableObject, IMtmTreeViewItem> DictRefsToTreeViewItems =
            new Dictionary<IReferenceableObject, IMtmTreeViewItem>();

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
                DictRefsToTreeViewItems.Add(referenceableObject, this);
            }

            if (parent != null)
            {
                this.PropertyChanged += this.Parent.IncestPropertyChanged;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public static ICommand CollapseAllCommand { get; } = new DelegateCommand<IMtmTreeViewItem>(
            node => SetExpandedState(node, false),
            node => node?.Children?.Count > 0);

        public static ICommand ExpandAllCommand { get; } = new DelegateCommand<IMtmTreeViewItem>(
            node => SetExpandedState(node, true),
            node => node?.Children?.Count > 0);

        public ObservableCollection<IMtmTreeViewItem> AllChildren
        {
            get
            {
                var aggregatedChildren = new List<IMtmTreeViewItem>();
                if (this.Children.Any())
                {
                    aggregatedChildren = new List<IMtmTreeViewItem>(this.Children);
                    this.Children.ToList().ForEach(item => aggregatedChildren.AddRange(item.AllChildren));
                }

                return new ObservableCollection<IMtmTreeViewItem>(aggregatedChildren);
            }
        }

        public virtual ObservableCollection<IMtmTreeViewItem> Children { get; set; } =
            new ObservableCollection<IMtmTreeViewItem>();

        public List<IObjectReference<IReferenceableObject>> Dependencies =>
            this.ObjectReferences.Where(reference => reference.ObjectReferenceType == ObjectReferenceType.Dependency)
                .ToList();

        public List<IObjectReference<IReferenceableObject>> Dependents =>
            this.ObjectReferences.Where(reference => reference.ObjectReferenceType == ObjectReferenceType.Dependent)
                .ToList();

        public bool HasCheck => false;

        public abstract string HumanReadableContent { get; }

        public bool? IsChecked
        {
            get => this.isChecked;
            set
            {
                if (value != this.isChecked)
                {
                    // Are we executing concurrently?
                    if (!ModCopyPage.Self.ModCopyModel.MainModel.IsBusy)
                    {
                        Task.Run(
                            () =>
                                {
                                    ModCopyPage.Self.ModCopyModel.MainModel.IsBusy = true;
                                    CheckNode(this, value);

                                    this.SelectAbsentModDependencies();

                                    ModCopyPage.Self.ModCopyModel.MainModel.IsBusy = false;
                                    this.PropertyChanged += this.OnPropertyChanged;
                                    this.OnPropertyChanged();
                                    this.OnPropertyChanged("Selected");
                                    this.OnPropertyChanged("SelectionStatus");
                                    this.OnPropertyChanged("ObjectStatus");
                                });
                    }
                    else
                    {
                        CheckNode(this, value);
                        this.PropertyChanged += this.OnPropertyChanged;
                        this.OnPropertyChanged();
                        this.OnPropertyChanged("Selected");
                        this.OnPropertyChanged("SelectionStatus");
                        this.OnPropertyChanged("ObjectStatus");
                    }
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
                        if (this.Children.Any())
                        {
                            var viewSource = CollectionViewSource.GetDefaultView(this.Children);
                            viewSource?.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
                        }
                    }

                    this.isExpanded = value;
                    this.OnPropertyChanged(nameof(this.IsExpanded));
                }
            }
        }

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

        public abstract string Name { get; }

        public object Object { get; }

        public List<IObjectReference<IReferenceableObject>> ObjectReferences
        {
            get
            {
                if (this.objectReferences == null)
                {
                    this.objectReferences = CommonReferenceProcessor.FindReferences<IReferenceableObject>(
                        this.TopNode.ReferenceableObjectProvider,
                        this.Object as IReferenceableObject,
                        new List<ObjectType> { ObjectType.MechDef, ObjectType.Mod }); // TBD
                    this.objectReferences.Sort(
                        (reference, objectReference) => string.CompareOrdinal(
                            reference.ReferenceObject.ObjectType.ToString(),
                            objectReference.ReferenceObject.ObjectType.ToString()));

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

        public virtual ObjectStatus ObjectStatus =>
            this.Children.Any() ? this.Children.Where(item => item.IsChecked != false).All(item => item.ObjectStatus == ObjectStatus.Nominal) ? ObjectStatus.Nominal : ObjectStatus.Error :
            this.Object is IObjectDefinition obj ? obj.ObjectStatus : ObjectStatus.Nominal;

        public IMtmTreeViewItem Parent { get; }

        public virtual IReferenceableObjectProvider ReferenceableObjectProvider => throw new NotImplementedException();

        public ObservableCollection<IMtmTreeViewItem> SelectedObjects
        {
            get
            {
                var aggregatedObjects = new List<IMtmTreeViewItem>();
                if (this.Children.Any())
                {
                    aggregatedObjects = this.Children.Where(
                            item => item.SelectionStatus != SelectionStatus.Unselected && item is ObjectDefinitionNode)
                        .ToList();
                    this.Children.ToList().ForEach(item => aggregatedObjects.AddRange(item.SelectedObjects));
                }

                return new ObservableCollection<IMtmTreeViewItem>(aggregatedObjects);
            }
        }

        public SelectionStatus SelectionStatus =>
            this.IsChecked == null ? SelectionStatus.PartiallySelected :
            this.IsChecked.Value ? SelectionStatus.Selected : SelectionStatus.Unselected;

        public IMtmTreeViewItem HostingModNode
        {
            get
            {
                var curNode = this as IMtmTreeViewItem;
                while (curNode.Parent != null)
                {
                    if (curNode.Parent is ModNode)
                    {
                        return curNode.Parent;
                    }

                    curNode = curNode.Parent;
                }

                return this is ModNode ? this : null;
            }
        }

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

        public Visibility Visibility
        {
            get => this.visibility;
            set
            {
                if (this.visibility != value)
                {
                    this.visibility = value;
                    this.OnPropertyChanged(nameof(this.Visibility));
                }
            }
        }

        public static void CheckNode(MtmTreeViewItem node, bool? value)
        {
            // If we're setting the node to an unspecified state, we don't want to roll that down to our children.
            node.isChecked = value;

            // We don't go down the fucking list if we're setting partial. Cunt. Unless we're an Item Collection. Oh Fuck.
            var referenceableObject = node.Object is IReferenceableObject o ? o : null;
            var isItemCollection = referenceableObject != null
                                   && referenceableObject.ObjectType == ObjectType.ItemCollectionDef;

            if (value == null && !isItemCollection)
            {
                return;
            }

            if (!isItemCollection)
            {
                foreach (var child in node.Children)
                {
                    child.IsChecked = value;
                }
            }

            var objects = new List<IObjectReference<IReferenceableObject>>();

            // We check for not true so that we will select dependencies for partially selected mods.
            // Never select dependencies for mods. Handled elsewhere.
            if (!(node is ModNode))
            {
                objects = value == true
                              ? node.ObjectReferences.Where(
                                  reference => reference.ObjectReferenceType == ObjectReferenceType.Dependency).ToList()
                              : node.ObjectReferences.Where(
                                  reference => reference.ObjectReferenceType == ObjectReferenceType.Dependent).ToList();
            }

            // Add any item collections our object may belong to
            objects.AddRange(
                node.Dependents.Where(reference => reference.ReferenceObject.ObjectType == ObjectType.ItemCollectionDef)
                    .ToList());

            // Remove any dependency non-item collections if we're selecting an item collection
            if (isItemCollection)
            {
                objects.RemoveAll(reference => reference.ReferenceObject.ObjectType != ObjectType.ItemCollectionDef);
            }

            foreach (var objectReference in objects)
            {
                IMtmTreeViewItem treeItem;
                if (DictRefsToTreeViewItems.TryGetValue(objectReference.ReferenceObject, out treeItem))
                {
                    // If we're selecting an item collection, flag it as a partial.
                    treeItem.IsChecked = objectReference.ReferenceObject.ObjectType == ObjectType.ItemCollectionDef
                                             ? null
                                             : value;
                }
            }

            CheckParentChildrenSelectedState(node);
        }

        public bool Filter(string filterText)
        {
            if (this.Children.Count > 0)
            {
                var childCollectionView = CollectionViewSource.GetDefaultView(this.Children);
                childCollectionView.Filter = child => ((IMtmTreeViewItem)child).Filter(filterText);
                return !childCollectionView.IsEmpty || this.Name.Contains(filterText)
                                                    || this.HumanReadableContent.Contains(filterText);
            }

            return this.Name.Contains(filterText) || this.HumanReadableContent.Contains(filterText);
        }

        // Propagates property changed events up the stack...
        public virtual void IncestPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.OnPropertyChanged(e.PropertyName);
        }

        // [NotifyPropertyChangedInvocator]
        public virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void SelectAbsentModDependencies()
        {
            // Check if any partially selected mods are missing dependencies and select them.
            var lastCount = 0;
            var referencedDependencies = new List<IMtmTreeViewItem>();
            do
            {
                lastCount = 0;
                this.TopNode.Children.Where(item => item.IsChecked == null || referencedDependencies.Contains(item))
                    .SelectMany(item => item.Dependencies).Distinct().ToList().ForEach(
                        dependency =>
                            {
                                IMtmTreeViewItem modDependencyNode;
                                if (DictRefsToTreeViewItems.TryGetValue(
                                        dependency.ReferenceObject,
                                        out modDependencyNode) && modDependencyNode.IsChecked == false)
                                {
                                    modDependencyNode.IsChecked = true;
                                    lastCount += 1;
                                    referencedDependencies.Add(modDependencyNode);
                                }
                            });
            }
            while (lastCount != 0);
        }

        public void Sort()
        {
            if (this.Children.Count > 0)
            {
                var collectionView = CollectionViewSource.GetDefaultView(this.Children);
                collectionView.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
                this.Children.ToList().ForEach(item => item.Sort());
            }
        }

        private static void CheckParentChildrenSelectedState(IMtmTreeViewItem item)
        {
            if (item.Children.Count == 0)
            {
                bool? valueSet;
                var currentNode = item.Parent;

                while (currentNode != null)
                {
                    if (currentNode.Children.All(node => node.IsChecked == true))
                    {
                        currentNode.IsChecked = valueSet = true;
                    }
                    else if (currentNode.Children.All(node => node.IsChecked == false))
                    {
                        currentNode.IsChecked = valueSet = false;
                    }
                    else
                    {
                        currentNode.IsChecked = valueSet = null;
                    }

                    // Set siblings to mod node children resource objects...
                    if (currentNode.Parent is ModNode modNode)
                    {
                        modNode.Children.Where(viewItem => viewItem is ResourceNode).ToList()
                            .ForEach(viewItem => { viewItem.IsChecked = valueSet != false; });
                    }

                    currentNode = currentNode.Parent;
                }
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
    }
}