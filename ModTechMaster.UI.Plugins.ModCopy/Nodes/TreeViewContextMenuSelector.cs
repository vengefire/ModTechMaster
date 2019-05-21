﻿namespace ModTechMaster.UI.Plugins.ModCopy.Nodes
{
    using System;
    using System.Windows.Controls;
    using Model;

    public static class TreeViewContextMenuSelector
    {
        public static ContextMenu GetContextMenu(object value)
        {
            var type = value.GetType();

            var contextMenu = new ContextMenu();

            if (type != typeof(ModCollectionNode))
            {
                contextMenu.Items.Add(new MenuItem {Header = "Expand All", Command = MtmTreeViewItem.ExpandAllCommand, CommandParameter = value});
            }

            contextMenu.Items.Add(new MenuItem { Header = "Collapse All", Command = MtmTreeViewItem.CollapseAllCommand, CommandParameter = value });

            if (type == typeof(ModNode))
            {
                contextMenu.Items.Add(
                    new MenuItem
                    {
                        Header = "Add to Imperative Mods List",
                        Command = ModCopyModel.AddModToImperativeListCommand,
                        CommandParameter = new Tuple<ModCopyPage, ModNode>(ModCopyPage.Self, value as ModNode)
                    });
                contextMenu.Items.Add(
                    new MenuItem
                    {
                        Header = "Remove from Imperative Mods List",
                        Command = ModCopyModel.RemoveModFromImperativeListCommand,
                        CommandParameter = new Tuple<ModCopyPage, ModNode>(ModCopyPage.Self, value as ModNode)
                    });
            }

            return contextMenu;
        }
    }
}