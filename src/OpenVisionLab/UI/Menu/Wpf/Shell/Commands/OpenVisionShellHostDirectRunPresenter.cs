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

        public bool UsesToolRouteText => displayState == DirectRunDisplayState.Pending || displayState == DirectRunDisplayState.Succeeded;

        public void SetWorkspaceEmpty()
        {
            displayState = DirectRunDisplayState.WorkspaceEmpty;
            SetBadgeText(T("Shell.WorkspaceStatus.EmptyBadge", "Start"));
            SetBadgeBrush("OpenVision.Workbench.StaleBrush", Brushes.DarkGoldenrod);
            SetTitleText(T("Shell.WorkspaceStatus.EmptyTitle", "Load an image"));
            SetRouteText(T("Shell.WorkspaceStatus.EmptyRoute", "Load image or open sample"));
            SetStatusText(T("Shell.WorkspaceStatus.EmptyStatus", "Select a tool after image is ready"));
            SetStatusBrush("OpenVision.Workbench.AccentBrush", Brushes.LightBlue);
        }

        public void SetImageReady()
        {
            displayState = DirectRunDisplayState.ImageReady;
            SetBadgeText(T("Shell.MainAction.ReadyBadge", "\uC900\uBE44"));
            SetBadgeBrush("OpenVision.Workbench.StaleBrush", Brushes.DarkGoldenrod);
            SetTitleText(T("Shell.MainAction.ImageReadyTitle", "\uC774\uBBF8\uC9C0 \uC900\uBE44\uB428"));
            SetRouteText(T("Shell.MainAction.ImageReadyRoute", "\uB3C4\uAD6C \uC120\uD0DD -> \uBBF8\uB9AC\uBCF4\uAE30 \uD655\uC778"));
            SetStatusText(T("Shell.MainAction.ImageReadyStatus", "\uD30C\uC774\uD504\uB77C\uC778 \uCD94\uAC00 \uAC00\uB2A5"));
            SetStatusBrush("OpenVision.Workbench.AccentBrush", Brushes.LightBlue);
        }

        public void SetSampleReady()
        {
            displayState = DirectRunDisplayState.SampleReady;
            SetBadgeText(T("Shell.MainAction.ReadyBadge", "Ready"));
            SetBadgeBrush("OpenVision.Workbench.OkBrush", Brushes.SeaGreen);
            SetTitleText(T("Shell.WorkspaceStatus.SampleTitle", "Sample pipeline ready"));
            SetRouteText(T("Shell.WorkspaceStatus.SampleRoute", "Open Pipeline Review or the first step"));
            SetStatusText(T("Shell.WorkspaceStatus.SampleStatus", "Preview runs manually"));
            SetStatusBrush("OpenVision.Workbench.OkBrush", Brushes.SeaGreen);
        }

        public void SetPending()
        {
            displayState = DirectRunDisplayState.Pending;
            SetBadgeText(OpenVisionLanguageService.T("Shell.DirectBadgeReady"));
            SetBadgeBrush("OpenVision.Workbench.StaleBrush", Brushes.DarkGoldenrod);
            SetTitleText(selectedTitleProvider?.Invoke());
            SetStatusText(OpenVisionLanguageService.T("Shell.DirectStatusReadyDetail"));
            SetStatusBrush("OpenVision.Workbench.AccentBrush", Brushes.LightBlue);
        }

        public void SetSucceeded(string tackTime)
        {
            displayState = DirectRunDisplayState.Succeeded;
            lastTackTime = tackTime ?? string.Empty;
            SetBadgeText(OpenVisionLanguageService.T("Shell.DirectBadgeOk"));
            SetBadgeBrush("OpenVision.Workbench.OkBrush", Brushes.SeaGreen);
            string elapsed = string.IsNullOrWhiteSpace(tackTime)
                ? OpenVisionLanguageService.T("Shell.DirectStatusOk")
                : string.Format(CultureInfo.CurrentCulture, OpenVisionLanguageService.T("Shell.DirectStatusOkFormat"), tackTime);
            SetTitleText(selectedTitleProvider?.Invoke());
            SetStatusText(elapsed);
            SetStatusBrush("OpenVision.Workbench.OkBrush", Brushes.SeaGreen);
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

        private void SetStatusBrush(string resourceKey, Brush fallback)
        {
            SetBrush(statusText, TextBlock.ForegroundProperty, resourceKey, fallback);
        }

        private void SetBadgeBrush(string resourceKey, Brush fallback)
        {
            SetBrush(badge, Border.BackgroundProperty, "OpenVision.Workbench.PanelAltBrush", Brushes.Transparent);
            SetBrush(badgeText, TextBlock.ForegroundProperty, resourceKey, fallback);
            SetBrush(panel, Border.BorderBrushProperty, resourceKey, fallback);
        }

        private void SetBrush(FrameworkElement element, DependencyProperty property, string resourceKey, Brush fallback)
        {
            if (element == null) { return; }
            if (resourceHost?.TryFindResource(resourceKey) is Brush) { element.SetResourceReference(property, resourceKey); }
            else { element.SetValue(property, fallback); }
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
