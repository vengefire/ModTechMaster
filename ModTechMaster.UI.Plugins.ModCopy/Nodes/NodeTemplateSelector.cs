namespace ModTechMaster.UI.Plugins.ModCopy.Nodes
{
    using System.Windows;
    using System.Windows.Controls;

    using ModTechMaster.UI.Plugins.ModCopy.Nodes.SpecialisedNodes;

    public class NodeTemplateSelector : DataTemplateSelector
    {
        public DataTemplate DefaultNodeTemplate { get; set; }

        public DataTemplate LanceDefNodeTemplate { get; set; }

        public DataTemplate ModCollectionNodeTemplate { get; set; }

        public DataTemplate ModNodeTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is ModCollectionNode)
            {
                return this.ModCollectionNodeTemplate;
            }

            if (item is LanceDefNode)
            {
                return this.LanceDefNodeTemplate;
            }

            if (item is ModNode)
            {
                return this.ModNodeTemplate;
            }

            if (item is MtmTreeViewItem)
            {
                return this.DefaultNodeTemplate;
            }

            return base.SelectTemplate(item, container);
        }
    }
}