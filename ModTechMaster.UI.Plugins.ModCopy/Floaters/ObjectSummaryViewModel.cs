namespace ModTechMaster.UI.Plugins.ModCopy.Floaters
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Windows.Data;

    using ModTechMaster.Core.Enums.Mods;
    using ModTechMaster.Logic.Annotations;
    using ModTechMaster.UI.Plugins.Core.Models;
    using ModTechMaster.UI.Plugins.ModCopy.Model;
    using ModTechMaster.UI.Plugins.ModCopy.Nodes;

    public class ObjectSummaryViewModel : INotifyPropertyChanged
    {
        private readonly List<ObjectType> filterableObjectTypes = new List<ObjectType>
                                                                      {
                                                                          ObjectType.MechDef,
                                                                          ObjectType.VehicleDef,
                                                                          ObjectType.TurretDef,
                                                                          ObjectType.WeaponDef,
                                                                          ObjectType.UpgradeDef,
                                                                          ObjectType.HeatSinkDef,
                                                                          ObjectType.JumpJetDef
                                                                      };

        private ObservableCollection<FilterOption> selectedObjectTypeFilters = new ObservableCollection<FilterOption>();

        private bool selectedOnly;

        private ObservableCollection<ObjectDefinitionNode> objectNodes;

        public ObjectSummaryViewModel(ModCopyModel modCopyModel)
        {
            this.ModCopyModel = modCopyModel;

            this.FilterableObjectTypeOptions = new ObservableCollection<FilterOption>(
                this.filterableObjectTypes.Select(type => new FilterOption(type.ToString(), false, type)));

            this.ObjectNodes = new ObservableCollection<ObjectDefinitionNode>(
                this.ModCopyModel.ModCollectionNode.AllChildren.Where(
                        item => item is ObjectDefinitionNode obj
                                && this.filterableObjectTypes.Contains(obj.ObjectDefinition.ObjectType))
                    .Cast<ObjectDefinitionNode>());

            var objectNodesView = CollectionViewSource.GetDefaultView(this.ObjectNodes);
            objectNodesView.Filter += this.Filter;
            this.selectedObjectTypeFilters.CollectionChanged += (sender, args) => this.RefreshObjectView();
        }

        private void RefreshObjectView()
        {
            CollectionViewSource.GetDefaultView(this.ObjectNodes).Refresh();
        }

        public ObservableCollection<ObjectDefinitionNode> ObjectNodes
        {
            get => this.objectNodes;
            set
            {
                if (value == this.objectNodes)
                {
                    return;
                }
                this.objectNodes = value;
                this.OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<FilterOption> FilterableObjectTypeOptions { get; }

        public ModCopyModel ModCopyModel { get; }

        public ObservableCollection<FilterOption> SelectedObjectTypeFilters
        {
            get => this.selectedObjectTypeFilters;
            set
            {
                if (value == this.selectedObjectTypeFilters)
                {
                    return;
                }

                this.selectedObjectTypeFilters = value;
                this.OnPropertyChanged();
                this.RefreshObjectView();
            }
        }

        public bool SelectedOnly
        {
            get => this.selectedOnly;
            set
            {
                if (value == this.selectedOnly)
                {
                    return;
                }

                this.selectedOnly = value;
                this.OnPropertyChanged();
                this.RefreshObjectView();
            }
        }

        public bool Filter(object item)
        {
            var selectedObjectTypes = this.selectedObjectTypeFilters?.Select(option => option.Value).Cast<ObjectType>();
            var node = item as ObjectDefinitionNode;
            if (this.SelectedOnly && node.IsChecked != true)
            {
                return false;
            }

            if (this.SelectedObjectTypeFilters != null && this.SelectedObjectTypeFilters.Any()
                                                       && !selectedObjectTypes.Contains(
                                                           node.ObjectDefinition.ObjectType))
            {
                return false;
            }

            return true;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}