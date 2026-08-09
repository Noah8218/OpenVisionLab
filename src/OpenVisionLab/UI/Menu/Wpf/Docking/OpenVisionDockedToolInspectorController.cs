using System;
using System.Windows;
using System.Windows.Controls;

namespace OpenVisionLab
{
    internal sealed class OpenVisionDockedToolInspectorController
    {
        private const double DefaultWidth = 620D;
        private const double MinimumWidth = 560D;
        private const double MaximumWidth = 760D;
        private const double SplitterWidth = 6D;

        private readonly ContentControl contentHost;
        private readonly TextBlock titleText;
        private readonly FrameworkElement panel;
        private readonly FrameworkElement splitter;
        private readonly ColumnDefinition splitterColumn;
        private readonly ColumnDefinition inspectorColumn;
        private readonly Func<bool> closeFloatingWindow;
        private FrameworkElement content;
        private string title = string.Empty;
        private double floatingWidth = 900D;
        private double floatingHeight = 620D;
        private double inspectorWidth = DefaultWidth;

        public OpenVisionDockedToolInspectorController(
            ContentControl contentHost,
            TextBlock titleText,
            FrameworkElement panel,
            FrameworkElement splitter,
            ColumnDefinition splitterColumn,
            ColumnDefinition inspectorColumn,
            Func<bool> closeFloatingWindow)
        {
            this.contentHost = contentHost;
            this.titleText = titleText;
            this.panel = panel;
            this.splitter = splitter;
            this.splitterColumn = splitterColumn;
            this.inspectorColumn = inspectorColumn;
            this.closeFloatingWindow = closeFloatingWindow;
        }

        public FrameworkElement ActiveContent => content;

        public string ActiveTitle => title;

        public bool IsVisible => panel?.Visibility == Visibility.Visible && contentHost?.Content != null;

        public bool Show(FrameworkElement nextContent, string nextTitle, double nextFloatingWidth, double nextFloatingHeight)
        {
            if (nextContent == null || contentHost == null)
            {
                return false;
            }

            SaveInspectorWidth();
            closeFloatingWindow?.Invoke();
            ClearContent(resetMode: true);

            content = nextContent;
            title = string.IsNullOrWhiteSpace(nextTitle) ? "OpenVisionLab Tool" : nextTitle;
            floatingWidth = nextFloatingWidth > 0D ? nextFloatingWidth : 900D;
            floatingHeight = nextFloatingHeight > 0D ? nextFloatingHeight : 620D;

            if (titleText != null)
            {
                titleText.Text = title;
            }

            OpenVisionToolDockModeHelper.Apply(content, true);
            contentHost.Content = content;
            ShowChrome();
            return true;
        }

        public bool CloseByUser()
        {
            bool hadContent = content != null;
            ClearContent(resetMode: true);
            HideChrome();
            return hadContent;
        }

        public void CloseSilently()
        {
            ClearContent(resetMode: true);
            HideChrome();
        }

        public bool Float(Action<FrameworkElement, string, double, double> showFloatingWindow)
        {
            FrameworkElement currentContent = content;
            if (currentContent == null || showFloatingWindow == null)
            {
                return false;
            }

            string currentTitle = title;
            double width = floatingWidth;
            double height = floatingHeight;
            ClearContent(resetMode: false);
            HideChrome();
            showFloatingWindow(currentContent, currentTitle, width, height);
            return true;
        }

        public void SaveInspectorWidth()
        {
            if (panel == null
                || inspectorColumn == null
                || panel.Visibility != Visibility.Visible)
            {
                return;
            }

            double width = inspectorColumn.ActualWidth > 0D
                ? inspectorColumn.ActualWidth
                : inspectorColumn.Width.Value;
            if (width > 0D && !double.IsNaN(width) && !double.IsInfinity(width))
            {
                // The right inspector is an operator-adjusted work area. Keep the last practical
                // width when switching tools so PropertyGrid readability does not reset.
                inspectorWidth = ClampWidth(width);
            }
        }

        private void ShowChrome()
        {
            if (splitterColumn != null)
            {
                splitterColumn.Width = new GridLength(SplitterWidth);
            }

            if (inspectorColumn != null)
            {
                inspectorColumn.Width = new GridLength(ClampWidth(inspectorWidth));
                inspectorColumn.MinWidth = MinimumWidth;
            }

            if (splitter != null)
            {
                splitter.Visibility = Visibility.Visible;
            }

            if (panel != null)
            {
                panel.Visibility = Visibility.Visible;
            }
        }

        private void HideChrome()
        {
            SaveInspectorWidth();

            if (panel != null)
            {
                panel.Visibility = Visibility.Collapsed;
            }

            if (splitter != null)
            {
                splitter.Visibility = Visibility.Collapsed;
            }

            if (splitterColumn != null)
            {
                splitterColumn.Width = new GridLength(0D);
            }

            if (inspectorColumn != null)
            {
                inspectorColumn.Width = new GridLength(0D);
                inspectorColumn.MinWidth = 0D;
            }

            if (titleText != null)
            {
                titleText.Text = string.Empty;
            }
        }

        private void ClearContent(bool resetMode)
        {
            FrameworkElement previousContent = content;
            if (contentHost != null)
            {
                contentHost.Content = null;
            }

            content = null;
            title = string.Empty;

            if (resetMode && previousContent != null)
            {
                OpenVisionToolDockModeHelper.Apply(previousContent, false);
            }
        }

        private static double ClampWidth(double width)
        {
            if (double.IsNaN(width) || double.IsInfinity(width) || width <= 0D)
            {
                return DefaultWidth;
            }

            return Math.Max(
                MinimumWidth,
                Math.Min(MaximumWidth, width));
        }
    }
}
