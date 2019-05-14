using System;
using System.Windows;
using System.Windows.Interop;
using IWin32Window = System.Windows.Forms.IWin32Window;

namespace ModTechMaster.UI.Core.WinForms
{
    public class Wpf32Window : IWin32Window
    {
        public Wpf32Window(Window wpfWindow)
        {
            Handle = new WindowInteropHelper(wpfWindow).Handle;
        }

        public IntPtr Handle { get; }
    }
}