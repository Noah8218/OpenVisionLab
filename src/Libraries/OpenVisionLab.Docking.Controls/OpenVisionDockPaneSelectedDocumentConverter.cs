using AvalonDock.Layout;
using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace OpenVisionLab.Docking.Controls
{
    public sealed class OpenVisionDockPaneSelectedDocumentConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            LayoutContent selectedContent = ResolveLayoutContent(values?.ElementAtOrDefault(0));
            LayoutContent fallbackContent = selectedContent ?? ResolveFallbackContent(values?.ElementAtOrDefault(1));

            if (IsContentParameter(parameter))
            {
                return fallbackContent?.Content;
            }

            return fallbackContent;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        private static LayoutContent ResolveLayoutContent(object value)
        {
            return value == null || value == DependencyProperty.UnsetValue
                ? null
                : value as LayoutContent;
        }

        private static LayoutContent ResolveFallbackContent(object value)
        {
            if (value == null || value == DependencyProperty.UnsetValue)
            {
                return null;
            }

            if (value is LayoutContent layoutContent)
            {
                return layoutContent;
            }

            if (value is LayoutAnchorablePane pane)
            {
                return pane.Children
                    .OfType<LayoutAnchorable>()
                    .FirstOrDefault(document => document.IsSelected)
                    ?? pane.Children.OfType<LayoutAnchorable>().FirstOrDefault();
            }

            return null;
        }

        private static bool IsContentParameter(object parameter)
        {
            return parameter is string text
                && string.Equals(text, "Content", StringComparison.OrdinalIgnoreCase);
        }
    }
}
