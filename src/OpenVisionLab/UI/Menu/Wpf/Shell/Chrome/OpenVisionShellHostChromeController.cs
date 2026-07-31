using MahApps.Metro.IconPacks;
using System;
using System.Windows;
using System.Windows.Controls;

namespace OpenVisionLab
{
    internal sealed class OpenVisionShellHostChromeController
    {
        private readonly OpenVisionShellHostMenuPresenter menuPresenter;
        private readonly OpenVisionShellHostToolRailPresenter toolRailPresenter;
        private readonly OpenVisionShellHostDirectRunPresenter directRunPresenter;
        private readonly TextBlock activeDocumentText;
        private readonly ColumnDefinition toolRailColumn;
        private readonly Border toolRailBorder;
        private readonly Button toggleToolRailButton;
        private readonly ScrollViewer toolRailScroll;
        private readonly StackPanel toolRailToggleContent;
        private readonly PackIconMaterial toolRailToggleIcon;
        private readonly TextBlock toolRailToggleText;
        private readonly Func<string> tackTimeProvider;
        private readonly Action refreshDirectRouteText;
        private readonly Action refreshLayerActionButtons;

        public OpenVisionShellHostChromeController(
            OpenVisionShellHostMenuPresenter menuPresenter,
            OpenVisionShellHostToolRailPresenter toolRailPresenter,
            OpenVisionShellHostDirectRunPresenter directRunPresenter,
            TextBlock activeDocumentText,
            ColumnDefinition toolRailColumn,
            Border toolRailBorder,
            Button toggleToolRailButton,
            ScrollViewer toolRailScroll,
            StackPanel toolRailToggleContent,
            PackIconMaterial toolRailToggleIcon,
            TextBlock toolRailToggleText,
            Func<string> tackTimeProvider,
            Action refreshDirectRouteText,
            Action refreshLayerActionButtons)
        {
            this.menuPresenter = menuPresenter ?? throw new ArgumentNullException(nameof(menuPresenter));
            this.toolRailPresenter = toolRailPresenter ?? throw new ArgumentNullException(nameof(toolRailPresenter));
            this.directRunPresenter = directRunPresenter ?? throw new ArgumentNullException(nameof(directRunPresenter));
            this.activeDocumentText = activeDocumentText;
            this.toolRailColumn = toolRailColumn;
            this.toolRailBorder = toolRailBorder;
            this.toggleToolRailButton = toggleToolRailButton;
            this.toolRailScroll = toolRailScroll;
            this.toolRailToggleContent = toolRailToggleContent;
            this.toolRailToggleIcon = toolRailToggleIcon;
            this.toolRailToggleText = toolRailToggleText;
            this.tackTimeProvider = tackTimeProvider ?? throw new ArgumentNullException(nameof(tackTimeProvider));
            this.refreshDirectRouteText = refreshDirectRouteText ?? throw new ArgumentNullException(nameof(refreshDirectRouteText));
            this.refreshLayerActionButtons = refreshLayerActionButtons ?? throw new ArgumentNullException(nameof(refreshLayerActionButtons));
        }

        public void ApplyLocalization(bool isToolRailCompact)
        {
            menuPresenter.ApplyLocalization();
            directRunPresenter.ApplyLocalization();
            ApplyToolRailCompactState(isToolRailCompact);
            refreshLayerActionButtons();
        }

        public void ApplyToolRailCompactState(bool isToolRailCompact)
        {
            toolRailPresenter.Apply(
                isToolRailCompact,
                toolRailColumn,
                toolRailBorder,
                toggleToolRailButton,
                toolRailScroll,
                toolRailToggleContent,
                toolRailToggleIcon,
                toolRailToggleText);
        }

        public void SetActiveDocumentText(string text)
        {
            if (activeDocumentText != null)
            {
                activeDocumentText.Text = text ?? string.Empty;
            }
        }

        public void SetDirectRunPending()
        {
            directRunPresenter.SetPending();
            RefreshDirectRouteText();
        }

        public void SetDirectRunSucceeded()
        {
            directRunPresenter.SetSucceeded(tackTimeProvider());
            RefreshDirectRouteText();
        }

        public void SetDirectRouteText(string text)
        {
            directRunPresenter.SetRouteText(text);
        }

        public void SetWorkspaceEmptyStatus()
        {
            directRunPresenter.SetWorkspaceEmpty();
        }

        public void SetWorkspaceImageReadyStatus()
        {
            directRunPresenter.SetImageReady();
        }

        public void SetWorkspaceSampleReadyStatus()
        {
            directRunPresenter.SetSampleReady();
        }

        public void RefreshDirectRouteText()
        {
            refreshDirectRouteText();
        }
    }
}
