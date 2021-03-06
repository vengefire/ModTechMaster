﻿namespace ModTechMaster.UI.Core.WinForms.Extensions
{
    using System.Windows;
    using System.Windows.Forms;

    public static class CommonDialogExtensions
    {
        public static DialogResult ShowDialog(this CommonDialog dialog, Window parent)
        {
            return dialog.ShowDialog(new Wpf32Window(parent));
        }
    }
}