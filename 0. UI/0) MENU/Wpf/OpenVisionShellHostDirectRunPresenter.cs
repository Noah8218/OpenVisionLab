using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace OpenVisionLab
{
    internal sealed class OpenVisionShellHostDirectRunPresenter
    {
        private readonly FrameworkElement resourceHost;
        private readonly Border panel;
        private readonly Border badge;
        private readonly TextBlock badgeText;
        private readonly TextBlock titleText;
        private readonly TextBlock routeText;
        private readonly TextBlock statusText;
        private readonly Func<string> selectedTitleProvider;
        private DirectRunDisplayState displayState = DirectRunDisplayState.WorkspaceEmpty;
        private string lastTackTime = string.Empty;

        public OpenVisionShellHostDirectRunPresenter(
            FrameworkElement resourceHost,
            Border panel,
            Border badge,
            TextBlock badgeText,
            TextBlock titleText,
            TextBlock routeText,
            TextBlock statusText,
            Func<string> selectedTitleProvider)
        {
            this.resourceHost = resourceHost;
            this.panel = panel;
            this.badge = badge;
            this.badgeText = badgeText;
            this.titleText = titleText;
            this.routeText = routeText;
            this.statusText = statusText;
            this.selectedTitleProvider = selectedTitleProvider;
        }

        public void SetWorkspaceEmpty()
        {
            displayState = DirectRunDisplayState.WorkspaceEmpty;
            SetBadgeText(T("Shell.WorkspaceStatus.EmptyBadge", "Start"));
            Brush pendingBrush = FindBrush("ShellHost.WarnBrush", Brushes.DarkGoldenrod);
            SetBadgeBrush(pendingBrush);
            SetTitleText(T("Shell.WorkspaceStatus.EmptyTitle", "Load an image"));
            SetRouteText(T("Shell.WorkspaceStatus.EmptyRoute", "Load image or open sample"));
            SetStatusText(T("Shell.WorkspaceStatus.EmptyStatus", "Select a tool after image is ready"));
            SetStatusBrush(FindBrush("ShellHost.FieldFocusBrush", Brushes.LightBlue));
        }

        public void SetImageReady()
        {
            displayState = DirectRunDisplayState.ImageReady;
            SetBadgeText(T("Shell.MainAction.ReadyBadge", "\uC900\uBE44"));
            Brush pendingBrush = FindBrush("ShellHost.WarnBrush", Brushes.DarkGoldenrod);
            SetBadgeBrush(pendingBrush);
            SetTitleText(T("Shell.MainAction.ImageReadyTitle", "\uC774\uBBF8\uC9C0 \uC900\uBE44\uB428"));
            SetRouteText(T("Shell.MainAction.ImageReadyRoute", "\uB3C4\uAD6C \uC120\uD0DD -> \uBBF8\uB9AC\uBCF4\uAE30 \uD655\uC778"));
            SetStatusText(T("Shell.MainAction.ImageReadyStatus", "\uD30C\uC774\uD504\uB77C\uC778 \uCD94\uAC00 \uAC00\uB2A5"));
            SetStatusBrush(FindBrush("ShellHost.FieldFocusBrush", Brushes.LightBlue));
        }

        public void SetSampleReady()
        {
            displayState = DirectRunDisplayState.SampleReady;
            SetBadgeText(T("Shell.MainAction.ReadyBadge", "Ready"));
            Brush okBrush = FindBrush("ShellHost.OkBrush", Brushes.SeaGreen);
            SetBadgeBrush(okBrush);
            SetTitleText(T("Shell.WorkspaceStatus.SampleTitle", "Sample pipeline ready"));
            SetRouteText(T("Shell.WorkspaceStatus.SampleRoute", "Open Pipeline Review or the first step"));
            SetStatusText(T("Shell.WorkspaceStatus.SampleStatus", "Preview runs manually"));
            SetStatusBrush(okBrush);
        }

        public void SetPending()
        {
            displayState = DirectRunDisplayState.Pending;
            SetBadgeText(OpenVisionLanguageService.T("Shell.DirectBadgeReady"));
            Brush pendingBrush = FindBrush("ShellHost.WarnBrush", Brushes.DarkGoldenrod);
            SetBadgeBrush(pendingBrush);
            SetTitleText(selectedTitleProvider?.Invoke());
            SetStatusText(OpenVisionLanguageService.T("Shell.DirectStatusReadyDetail"));
            SetStatusBrush(FindBrush("ShellHost.FieldFocusBrush", Brushes.LightBlue));
        }

        public void SetSucceeded(string tackTime)
        {
            displayState = DirectRunDisplayState.Succeeded;
            lastTackTime = tackTime ?? string.Empty;
            SetBadgeText(OpenVisionLanguageService.T("Shell.DirectBadgeOk"));
            Brush okBrush = FindBrush("ShellHost.OkBrush", Brushes.SeaGreen);
            SetBadgeBrush(okBrush);
            string elapsed = string.IsNullOrWhiteSpace(tackTime)
                ? OpenVisionLanguageService.T("Shell.DirectStatusOk")
                : string.Format(CultureInfo.CurrentCulture, OpenVisionLanguageService.T("Shell.DirectStatusOkFormat"), tackTime);
            SetTitleText(selectedTitleProvider?.Invoke());
            SetStatusText(elapsed);
            SetStatusBrush(okBrush);
        }

        public void SetRouteText(string text)
        {
            if (routeText != null)
            {
                routeText.Text = text ?? string.Empty;
            }
        }

        public void ApplyLocalization()
        {
            switch (displayState)
            {
                case DirectRunDisplayState.ImageReady:
                    SetImageReady();
                    break;
                case DirectRunDisplayState.SampleReady:
                    SetSampleReady();
                    break;
                case DirectRunDisplayState.Pending:
                    SetPending();
                    break;
                case DirectRunDisplayState.Succeeded:
                    SetSucceeded(lastTackTime);
                    break;
                default:
                    SetWorkspaceEmpty();
                    break;
            }
        }

        private void SetTitleText(string text)
        {
            if (titleText != null)
            {
                titleText.Text = text ?? string.Empty;
            }
        }

        private void SetBadgeText(string text)
        {
            if (badgeText != null)
            {
                badgeText.Text = text ?? string.Empty;
            }
        }

        private void SetStatusText(string text)
        {
            if (statusText != null)
            {
                statusText.Text = text ?? string.Empty;
            }
        }

        private void SetStatusBrush(Brush brush)
        {
            if (statusText != null)
            {
                statusText.Foreground = brush;
            }
        }

        private void SetBadgeBrush(Brush brush)
        {
            if (badge != null)
            {
                badge.Background = brush;
            }

            if (panel != null)
            {
                panel.BorderBrush = brush;
            }
        }

        private Brush FindBrush(string resourceKey, Brush fallback)
        {
            return resourceHost?.FindResource(resourceKey) as Brush ?? fallback;
        }

        private static string T(string key, string fallbackText)
        {
            string value = OpenVisionLanguageService.T(key);
            return string.IsNullOrWhiteSpace(value) || string.Equals(value, key, StringComparison.Ordinal)
                ? fallbackText
                : value;
        }

        private enum DirectRunDisplayState
        {
            WorkspaceEmpty,
            ImageReady,
            SampleReady,
            Pending,
            Succeeded
        }
    }
}
