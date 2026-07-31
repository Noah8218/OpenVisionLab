using AvalonDock.Controls;
using AvalonDock.Layout;
using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Media;

namespace OpenVisionLab.Docking.Controls
{
    public sealed partial class OpenVisionLayerDockingGestureController
    {
        public bool IsGestureSource(DependencyObject source)
        {
            return !string.IsNullOrWhiteSpace(ResolveLayerTitle(source));
        }

        private string ResolveLayerTitle(DependencyObject source)
        {
            DependencyObject current = source;
            while (current != null)
            {
                string typeName = current.GetType().Name;
                string automationId = current is FrameworkElement element
                    ? AutomationProperties.GetAutomationId(element)
                    : string.Empty;
                if (string.Equals(typeName, nameof(LayoutAnchorableTabItem), StringComparison.Ordinal)
                    || string.Equals(typeName, nameof(LayoutDocumentTabItem), StringComparison.Ordinal)
                    || string.Equals(typeName, nameof(AnchorablePaneTitle), StringComparison.Ordinal)
                    || string.Equals(typeName, "ToggleAnchorablePaneTitle", StringComparison.Ordinal)
                    || string.Equals(automationId, "DockedLayerTabHeader", StringComparison.Ordinal)
                    || string.Equals(automationId, "DockedLayerPaneHeader", StringComparison.Ordinal))
                {
                    string layerTitle = ResolveLayerTitleFromElement(current as FrameworkElement);
                    if (canOpenLayer(layerTitle))
                    {
                        return layerTitle;
                    }
                }

                current = VisualTreeHelper.GetParent(current);
            }

            return string.Empty;
        }

        private static string ResolveLayerTitleFromElement(FrameworkElement element)
        {
            DependencyObject current = element;
            while (current != null)
            {
                if (current is FrameworkElement frameworkElement)
                {
                    string title = ResolveLayerTitleFromObject(frameworkElement.DataContext);
                    if (!string.IsNullOrWhiteSpace(title))
                    {
                        return title;
                    }

                    title = ResolveLayerTitleFromObject(frameworkElement);
                    if (!string.IsNullOrWhiteSpace(title))
                    {
                        return title;
                    }
                }

                current = VisualTreeHelper.GetParent(current);
            }

            return string.Empty;
        }

        private static string ResolveLayerTitleFromObject(object value)
        {
            if (value == null)
            {
                return string.Empty;
            }

            if (value is LayoutAnchorable anchorable)
            {
                return !string.IsNullOrWhiteSpace(anchorable.ContentId)
                    ? anchorable.ContentId
                    : anchorable.Title;
            }

            object model = value.GetType().GetProperty("Model")?.GetValue(value);
            if (model is LayoutAnchorable modelAnchorable)
            {
                return !string.IsNullOrWhiteSpace(modelAnchorable.ContentId)
                    ? modelAnchorable.ContentId
                    : modelAnchorable.Title;
            }

            object contentId = value.GetType().GetProperty("ContentId")?.GetValue(value);
            if (contentId is string contentIdText && !string.IsNullOrWhiteSpace(contentIdText))
            {
                return contentIdText;
            }

            object title = value.GetType().GetProperty("Title")?.GetValue(value);
            return title as string ?? string.Empty;
        }
    }
}
