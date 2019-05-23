namespace ModTechMaster.UI.Core.WinForms.Extensions
{
    using System;
    using System.Windows;
    using System.Windows.Interop;
    using System.Windows.Media;

    using IWin32Window = System.Windows.Forms.IWin32Window;

    public static class WpfExtensions
    {
        public static IWin32Window GetIWin32Window(this Visual visual)
        {
            var source = PresentationSource.FromVisual(visual) as HwndSource;
            IWin32Window win = new OldWindow(source.Handle);
            return win;
        }

        private class OldWindow : IWin32Window
        {
            private readonly IntPtr handle;

            public OldWindow(IntPtr handle)
            {
                this.handle = handle;
            }

            IntPtr IWin32Window.Handle => this.handle;
        }
    }
}