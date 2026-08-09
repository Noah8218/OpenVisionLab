using System;
using System.Windows;
using System.Windows.Controls;

namespace OpenVisionLab
{
    internal sealed class OpenVisionDockedDocumentWorkspaceController
    {
        private readonly ContentControl contentHost;
        private readonly TextBlock titleText;
        private readonly FrameworkElement panel;
        private readonly Action closeFloatingWindow;
        private FrameworkElement content;
        private string title = string.Empty;
        private double floatingWidth;
        private double floatingHeight;

        public OpenVisionDockedDocumentWorkspaceController(
            ContentControl contentHost,
            TextBlock titleText,
            FrameworkElement panel,
            Action closeFloatingWindow)
        {
            this.contentHost = contentHost ?? throw new ArgumentNullException(nameof(contentHost));
            this.titleText = titleText ?? throw new ArgumentNullException(nameof(titleText));
            this.panel = panel ?? throw new ArgumentNullException(nameof(panel));
            this.closeFloatingWindow = closeFloatingWindow ?? throw new ArgumentNullException(nameof(closeFloatingWindow));
        }

        public bool IsVisible => panel.Visibility == Visibility.Visible && content != null;

        public bool ShouldRestoreDocked { get; private set; } = true;

        public FrameworkElement ActiveContent => content;

        public string ActiveTitle => title;

        public bool Show(FrameworkElement nextContent, string nextTitle, double nextFloatingWidth, double nextFloatingHeight)
        {
            return Attach(nextContent, nextTitle, nextFloatingWidth, nextFloatingHeight, Visibility.Visible);
        }

        public bool Prepare(FrameworkElement nextContent, string nextTitle, double nextFloatingWidth, double nextFloatingHeight)
        {
            return Attach(nextContent, nextTitle, nextFloatingWidth, nextFloatingHeight, Visibility.Hidden);
        }

        public bool SuspendForReuse()
        {
            if (!IsVisible)
            {
                return false;
            }

            panel.Visibility = Visibility.Collapsed;
            titleText.Text = string.Empty;
            ShouldRestoreDocked = true;
            return true;
        }

        public bool Float(Action<FrameworkElement, string, double, double> showFloatingWindow)
        {
            if (!IsVisible || showFloatingWindow == null)
            {
                return false;
            }

            FrameworkElement currentContent = content;
            string currentTitle = title;
            double width = floatingWidth;
            double height = floatingHeight;
            ClearContent(keepDockedPreference: false);
            showFloatingWindow(currentContent, currentTitle, width, height);
            return true;
        }

        public bool CloseSilently()
        {
            if (!IsVisible && !ShouldRestoreDocked)
            {
                return false;
            }

            ClearContent(keepDockedPreference: false);
            return true;
        }

        public bool CloseByUser()
        {
            return CloseSilently();
        }

        private bool Attach(
            FrameworkElement nextContent,
            string nextTitle,
            double nextFloatingWidth,
            double nextFloatingHeight,
            Visibility visibility)
        {
            if (nextContent == null)
            {
                return false;
            }

            closeFloatingWindow();
            if (!ReferenceEquals(content, nextContent))
            {
                contentHost.Content = null;
            }

            content = nextContent;
            title = nextTitle ?? string.Empty;
            floatingWidth = nextFloatingWidth;
            floatingHeight = nextFloatingHeight;
            ShouldRestoreDocked = true;
            OpenVisionToolDockModeHelper.Apply(content, false);
            contentHost.Content = content;
            titleText.Text = title;
            panel.Visibility = visibility;
            return true;
        }

        private void ClearContent(bool keepDockedPreference)
        {
            contentHost.Content = null;
            panel.Visibility = Visibility.Collapsed;
            titleText.Text = string.Empty;
            content = null;
            title = string.Empty;
            floatingWidth = 0D;
            floatingHeight = 0D;
            ShouldRestoreDocked = keepDockedPreference;
        }
    }
}
