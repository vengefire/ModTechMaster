namespace ModTechMaster.UI.Core.Utils
{
    using System.Windows.Controls;

    public class LayoutGroup : StackPanel
    {
        public LayoutGroup()
        {
            Grid.SetIsSharedSizeScope(this, true);
        }
    }
}