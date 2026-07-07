using System;
using System.Windows.Automation;
using System.Windows.Controls;

namespace OpenVisionLab
{
    internal sealed class OpenVisionShellHostMenuPresenter
    {
        private readonly MenuItem workspaceLoadImageMenu;
        private readonly MenuItem workspaceOpenLayerWindowMenu;
        private readonly MenuItem workspaceFitImageMenu;
        private readonly MenuItem workspaceSaveImageMenu;
        private readonly MenuItem workspaceDockLayerMenu;
        private readonly MenuItem workspaceClearDockedLayersMenu;
        private readonly TextBlock workspaceEmptyTitleText;
        private readonly TextBlock workspaceEmptyDetailText;
        private readonly TextBlock workspaceEmptyStepLoadTitleText;
        private readonly TextBlock workspaceEmptyStepLoadDetailText;
        private readonly TextBlock workspaceEmptyStepSelectTitleText;
        private readonly TextBlock workspaceEmptyStepSelectDetailText;
        private readonly TextBlock workspaceEmptyStepPreviewTitleText;
        private readonly TextBlock workspaceEmptyStepPreviewDetailText;
        private readonly TextBlock workspaceLoadImageButtonText;
        private readonly TextBlock workspaceEmptySampleButtonText;
        private readonly TextBlock workspaceEmptyGuideButtonText;
        private readonly TextBlock workspaceEmptyLogHintText;
        private readonly TextBlock openSelectedLayerWindowButtonText;
        private readonly TextBlock dockSelectedLayerButtonText;
        private readonly Button floatDockedToolButton;
        private readonly Button closeDockedToolButton;

        public OpenVisionShellHostMenuPresenter(
            MenuItem workspaceLoadImageMenu,
            MenuItem workspaceOpenLayerWindowMenu,
            MenuItem workspaceFitImageMenu,
            MenuItem workspaceSaveImageMenu,
            MenuItem workspaceDockLayerMenu,
            MenuItem workspaceClearDockedLayersMenu,
            TextBlock workspaceEmptyTitleText,
            TextBlock workspaceEmptyDetailText,
            TextBlock workspaceEmptyStepLoadTitleText,
            TextBlock workspaceEmptyStepLoadDetailText,
            TextBlock workspaceEmptyStepSelectTitleText,
            TextBlock workspaceEmptyStepSelectDetailText,
            TextBlock workspaceEmptyStepPreviewTitleText,
            TextBlock workspaceEmptyStepPreviewDetailText,
            TextBlock workspaceLoadImageButtonText,
            TextBlock workspaceEmptySampleButtonText,
            TextBlock workspaceEmptyGuideButtonText,
            TextBlock workspaceEmptyLogHintText,
            TextBlock openSelectedLayerWindowButtonText,
            TextBlock dockSelectedLayerButtonText,
            Button floatDockedToolButton,
            Button closeDockedToolButton)
        {
            this.workspaceLoadImageMenu = workspaceLoadImageMenu;
            this.workspaceOpenLayerWindowMenu = workspaceOpenLayerWindowMenu;
            this.workspaceFitImageMenu = workspaceFitImageMenu;
            this.workspaceSaveImageMenu = workspaceSaveImageMenu;
            this.workspaceDockLayerMenu = workspaceDockLayerMenu;
            this.workspaceClearDockedLayersMenu = workspaceClearDockedLayersMenu;
            this.workspaceEmptyTitleText = workspaceEmptyTitleText;
            this.workspaceEmptyDetailText = workspaceEmptyDetailText;
            this.workspaceEmptyStepLoadTitleText = workspaceEmptyStepLoadTitleText;
            this.workspaceEmptyStepLoadDetailText = workspaceEmptyStepLoadDetailText;
            this.workspaceEmptyStepSelectTitleText = workspaceEmptyStepSelectTitleText;
            this.workspaceEmptyStepSelectDetailText = workspaceEmptyStepSelectDetailText;
            this.workspaceEmptyStepPreviewTitleText = workspaceEmptyStepPreviewTitleText;
            this.workspaceEmptyStepPreviewDetailText = workspaceEmptyStepPreviewDetailText;
            this.workspaceLoadImageButtonText = workspaceLoadImageButtonText;
            this.workspaceEmptySampleButtonText = workspaceEmptySampleButtonText;
            this.workspaceEmptyGuideButtonText = workspaceEmptyGuideButtonText;
            this.workspaceEmptyLogHintText = workspaceEmptyLogHintText;
            this.openSelectedLayerWindowButtonText = openSelectedLayerWindowButtonText;
            this.dockSelectedLayerButtonText = dockSelectedLayerButtonText;
            this.floatDockedToolButton = floatDockedToolButton;
            this.closeDockedToolButton = closeDockedToolButton;
        }

        public void ApplyLocalization()
        {
            SetHeader(workspaceLoadImageMenu, OpenVisionLanguageService.T("Shell.WorkspaceLoadImage"));
            SetHeader(workspaceOpenLayerWindowMenu, OpenVisionLanguageService.T("Shell.OpenLayerWindow"));
            SetHeader(workspaceFitImageMenu, TOrFallback("Shell.WorkspaceFitImage", "Fit Image"));
            SetHeader(workspaceSaveImageMenu, TOrFallback("Shell.WorkspaceSaveImage", "Save Image"));
            SetHeader(workspaceDockLayerMenu, OpenVisionLanguageService.T("Shell.DockLayer"));
            SetHeader(workspaceClearDockedLayersMenu, OpenVisionLanguageService.T("Shell.ClearDockedLayers"));

            SetText(workspaceEmptyTitleText, OpenVisionLanguageService.T("Shell.WorkspaceEmptyTitle"));
            SetText(workspaceEmptyDetailText, OpenVisionLanguageService.T("Shell.WorkspaceEmptyDetail"));
            SetText(workspaceEmptyStepLoadTitleText, TOrFallback("Shell.WorkspaceEmptyStepLoadTitle", "1. Load image"));
            SetText(workspaceEmptyStepLoadDetailText, TOrFallback("Shell.WorkspaceEmptyStepLoadDetail", "Open the image to inspect into the Main layer."));
            SetText(workspaceEmptyStepSelectTitleText, TOrFallback("Shell.WorkspaceEmptyStepSelectTitle", "2. Select tool"));
            SetText(workspaceEmptyStepSelectDetailText, TOrFallback("Shell.WorkspaceEmptyStepSelectDetail", "Choose Threshold, Matching, Line, or another tool from the left list."));
            SetText(workspaceEmptyStepPreviewTitleText, TOrFallback("Shell.WorkspaceEmptyStepPreviewTitle", "3. Check preview"));
            SetText(workspaceEmptyStepPreviewDetailText, TOrFallback("Shell.WorkspaceEmptyStepPreviewDetail", "Check the result, then add the verified step to the pipeline."));
            SetText(workspaceLoadImageButtonText, OpenVisionLanguageService.T("Shell.WorkspaceLoadImageButton"));
            SetText(workspaceEmptySampleButtonText, TOrFallback("Shell.WorkspaceEmptySampleButton", "Open Sample"));
            SetText(workspaceEmptyGuideButtonText, TOrFallback("Shell.WorkspaceEmptyGuideButton", "Open Learn"));
            SetText(workspaceEmptyLogHintText, TOrFallback("Shell.WorkspaceEmptyLogHint", "After loading an image, use this area for zoom, pan, pixel values, and the Run Log for state tracking."));
            SetText(openSelectedLayerWindowButtonText, OpenVisionLanguageService.T("Shell.OpenLayerWindowShort"));
            SetText(dockSelectedLayerButtonText, OpenVisionLanguageService.T("Shell.DockLayerShort"));
            ApplyDockedToolHeaderLocalization();
        }

        public void ApplyDockedToolHeaderLocalization()
        {
            string floatText = TOrFallback("Shell.FloatDockedTool", "Float tool window");
            string closeText = TOrFallback("Shell.CloseDockedTool", "Close tool");

            SetButtonToolTipAndName(floatDockedToolButton, floatText);
            SetButtonToolTipAndName(closeDockedToolButton, closeText);
        }

        private static void SetHeader(MenuItem menuItem, string header)
        {
            if (menuItem != null)
            {
                menuItem.Header = header;
            }
        }

        private static void SetText(TextBlock textBlock, string text)
        {
            if (textBlock != null)
            {
                textBlock.Text = text ?? string.Empty;
            }
        }

        private static void SetButtonToolTipAndName(Button button, string text)
        {
            if (button == null)
            {
                return;
            }

            button.ToolTip = text;
            AutomationProperties.SetName(button, text);
        }

        private static string TOrFallback(string key, string fallback)
        {
            string text = OpenVisionLanguageService.T(key);
            return string.IsNullOrWhiteSpace(text) || string.Equals(text, key, StringComparison.Ordinal)
                ? fallback
                : text;
        }
    }
}
