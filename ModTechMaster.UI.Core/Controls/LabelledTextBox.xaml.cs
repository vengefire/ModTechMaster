namespace ModTechMaster.UI.Core.Controls
{
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    ///     Interaction logic for LabelledTextBox.xaml
    /// </summary>
    public partial class LabelledTextBox : UserControl
    {
        public static readonly DependencyProperty LabelProperty = DependencyProperty.Register(
            "Label",
            typeof(string),
            typeof(LabelledTextBox),
            new FrameworkPropertyMetadata("Unnamed Label"));

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text",
            typeof(string),
            typeof(LabelledTextBox),
            new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static readonly DependencyProperty ReadOnlyProperty = DependencyProperty.Register(
            "IsReadOnly",
            typeof(string),
            typeof(LabelledTextBox),
            new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public LabelledTextBox()
        {
            this.InitializeComponent();
            this.Root.DataContext = this;
        }

        public string Label
        {
            get => (string)this.GetValue(LabelProperty);
            set => this.SetValue(LabelProperty, value);
        }

        public string Text
        {
            get => (string)this.GetValue(TextProperty);
            set => this.SetValue(TextProperty, value);
        }

        public bool IsReadOnly
        {
            get => (bool)this.GetValue(ReadOnlyProperty);
            set => this.SetValue(ReadOnlyProperty, value);
        }
    }
}