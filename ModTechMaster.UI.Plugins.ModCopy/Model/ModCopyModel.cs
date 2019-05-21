namespace ModTechMaster.UI.Plugins.ModCopy.Model
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Input;
    using Annotations;
    using Nodes;

    public sealed class ModCopyModel : INotifyPropertyChanged
    {
        public static readonly ICommand AddModToImperativeListCommand = new DelegateCommand<Tuple<ModCopyPage, ModNode>>(
            parameters =>
            {
                var model = parameters.Item1;
                var mod = parameters.Item2;
                var settings = model.Settings as ModCopySettings;
                Debug.Assert(settings != null, nameof(settings) + " != null");
                settings.AddImperativeMod(mod.Mod);
                mod.IsChecked = true;
            });

        public static readonly ICommand RemoveModFromImperativeListCommand = new DelegateCommand<Tuple<ModCopyPage, ModNode>>(
            parameters =>
            {
                var model = parameters.Item1;
                var mod = parameters.Item2;
                var settings = model.Settings as ModCopySettings;
                Debug.Assert(settings != null, nameof(settings) + " != null");
                settings.RemoveImperativeMod(mod.Mod);
                mod.IsChecked = false;
            });

        private ModCopySettings settings;

        private IMtmTreeViewItem currentSelectedItem;

        public IMtmTreeViewItem CurrentSelectedItem
        {
            get => this.currentSelectedItem;
            set
            {
                if (value == this.currentSelectedItem)
                {
                    return;
                }

                this.currentSelectedItem = value;
                this.OnPropertyChanged();
            }
        }

        public ModCopySettings Settings
        {
            get => this.settings;
            set
            {
                if (value == this.settings)
                {
                    return;
                }

                this.settings = value;
                this.OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            this.CurrentSelectedItem = e.NewValue as IMtmTreeViewItem;
        }

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}