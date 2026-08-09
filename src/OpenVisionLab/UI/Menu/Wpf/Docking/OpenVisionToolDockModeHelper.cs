using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace OpenVisionLab
{
    internal static class OpenVisionToolDockModeHelper
    {
        private static readonly DependencyProperty OriginalMinWidthProperty =
            DependencyProperty.RegisterAttached(
                "OriginalMinWidth",
                typeof(double),
                typeof(OpenVisionToolDockModeHelper),
                new PropertyMetadata(double.NaN));

        private static readonly DependencyProperty OriginalMinHeightProperty =
            DependencyProperty.RegisterAttached(
                "OriginalMinHeight",
                typeof(double),
                typeof(OpenVisionToolDockModeHelper),
                new PropertyMetadata(double.NaN));

        public static void Apply(FrameworkElement root, bool isDocked)
        {
            if (root == null)
            {
                return;
            }

            ApplyDockedRootSize(root, isDocked);
            root.ApplyTemplate();
            ApplyToDescendants(
                root,
                isDocked,
                new HashSet<DependencyObject>(ReferenceEqualityComparer.Instance));
        }

        private static void ApplyDockedRootSize(FrameworkElement root, bool isDocked)
        {
            if (isDocked)
            {
                if (double.IsNaN((double)root.GetValue(OriginalMinWidthProperty)))
                {
                    root.SetValue(OriginalMinWidthProperty, root.MinWidth);
                    root.SetValue(OriginalMinHeightProperty, root.MinHeight);
                }

                root.MinWidth = 0D;
                root.MinHeight = 0D;
                return;
            }

            double originalMinWidth = (double)root.GetValue(OriginalMinWidthProperty);
            double originalMinHeight = (double)root.GetValue(OriginalMinHeightProperty);
            if (!double.IsNaN(originalMinWidth))
            {
                root.MinWidth = originalMinWidth;
                root.ClearValue(OriginalMinWidthProperty);
            }

            if (!double.IsNaN(originalMinHeight))
            {
                root.MinHeight = originalMinHeight;
                root.ClearValue(OriginalMinHeightProperty);
            }
        }

        private static bool ApplyToDescendants(
            DependencyObject element,
            bool isDocked,
            HashSet<DependencyObject> visited)
        {
            if (element == null || !visited.Add(element))
            {
                return false;
            }

            if (element is VisionToolSingleInputPropertyToolShell shell)
            {
                shell.IsDockedInspectorMode = isDocked;
                return true;
            }

            if (element is VisionToolDoubleInputCustomToolShell doubleInputShell)
            {
                doubleInputShell.IsDockedInspectorMode = isDocked;
                return true;
            }

            bool applied = false;
            if (element is ContentControl contentControl && contentControl.Content is DependencyObject content)
            {
                applied |= ApplyToDescendants(content, isDocked, visited);
            }

            int childCount;
            try
            {
                childCount = VisualTreeHelper.GetChildrenCount(element);
            }
            catch (InvalidOperationException)
            {
                childCount = 0;
            }

            for (int index = 0; index < childCount; index++)
            {
                applied |= ApplyToDescendants(VisualTreeHelper.GetChild(element, index), isDocked, visited);
            }

            foreach (object logicalChild in EnumerateLogicalChildren(element))
            {
                if (logicalChild is DependencyObject dependencyObject)
                {
                    applied |= ApplyToDescendants(dependencyObject, isDocked, visited);
                }
            }

            return applied;
        }

        private static IEnumerable EnumerateLogicalChildren(DependencyObject element)
        {
            try
            {
                return LogicalTreeHelper.GetChildren(element);
            }
            catch (InvalidOperationException)
            {
                return Array.Empty<object>();
            }
        }
    }
}
