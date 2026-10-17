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
        private readonly TextBlock workspaceLoadImageButtonText;
        private readonly TextBlock workspaceEmptySampleButtonText;
        private readonly TextBlock openSelectedLayerWindowButtonText;
        private readonly TextBlock dockSelectedLayerButtonText;
        private readonly Button floatDockedToolButton;
        private readonly Button closeDockedToolButton;
        private readonly Button floatDockedDocumentButton;
        private readonly Button closeDockedDocumentButton;

        public OpenVisionShellHostMenuPresenter(
            MenuItem workspaceLoadImageMenu,
            MenuItem workspaceOpenLayerWindowMenu,
            MenuItem workspaceFitImageMenu,
            MenuItem workspaceSaveImageMenu,
            MenuItem workspaceDockLayerMenu,
            MenuItem workspaceClearDockedLayersMenu,
            TextBlock workspaceEmptyTitleText,
            TextBlock workspaceEmptyDetailText,
            TextBlock workspaceLoadImageButtonText,
            TextBlock workspaceEmptySampleButtonText,
            TextBlock openSelectedLayerWindowButtonText,
            TextBlock dockSelectedLayerButtonText,
            Button floatDockedToolButton,
            Button closeDockedToolButton,
            Button floatDockedDocumentButton,
            Button closeDockedDocumentButton)
        {
            this.workspaceLoadImageMenu = workspaceLoadImageMenu;
            this.workspaceOpenLayerWindowMenu = workspaceOpenLayerWindowMenu;
            this.workspaceFitImageMenu = workspaceFitImageMenu;
            this.workspaceSaveImageMenu = workspaceSaveImageMenu;
            this.workspaceDockLayerMenu = workspaceDockLayerMenu;
            this.workspaceClearDockedLayersMenu = workspaceClearDockedLayersMenu;
            this.workspaceEmptyTitleText = workspaceEmptyTitleText;
            this.workspaceEmptyDetailText = workspaceEmptyDetailText;
            this.workspaceLoadImageButtonText = workspaceLoadImageButtonText;
            this.workspaceEmptySampleButtonText = workspaceEmptySampleButtonText;
            this.openSelectedLayerWindowButtonText = openSelectedLayerWindowButtonText;
            this.dockSelectedLayerButtonText = dockSelectedLayerButtonText;
            this.floatDockedToolButton = floatDockedToolButton;
            this.closeDockedToolButton = closeDockedToolButton;
            this.floatDockedDocumentButton = floatDockedDocumentButton;
            this.closeDockedDocumentButton = closeDockedDocumentButton;
        }

        public void ApplyLocalization()
        {
            SetHeader(workspaceLoadImageMenu, OpenVisionLanguageService.T("Shell.WorkspaceLoadImage"));
            SetHeader(workspaceOpenLayerWindowMenu, OpenVisionLanguageService.T("Shell.OpenLayerWindow"));
            SetHeader(workspaceFitImageMenu, TOrFallback("Shell.WorkspaceFitImage", "Fit Image"));
            SetHeader(workspaceSaveImageMenu, TOrFallback("Shell.WorkspaceSaveImage", "Save Image"));
            SetHeader(workspaceDockLayerMenu, OpenVisionLanguageService.T("Shell.DockLayer"));
            SetHeader(workspaceClearDockedLayersMenu, OpenVisionLanguageService.T("Shell.ClearDockedLayers"));

            SetText(workspaceEmptyTitleText, OpenVisionLanguageService.T("Workbench.Viewer.NoImage"));
            SetText(workspaceEmptyDetailText, OpenVisionLanguageService.T("Workbench.Viewer.EmptyDetail"));
            SetText(workspaceLoadImageButtonText, OpenVisionLanguageService.T("Workbench.Viewer.OpenImage"));
            SetText(workspaceEmptySampleButtonText, OpenVisionLanguageService.T("Workbench.Viewer.OpenSamples"));
            SetText(openSelectedLayerWindowButtonText, OpenVisionLanguageService.T("Shell.OpenLayerWindowShort"));
            SetText(dockSelectedLayerButtonText, OpenVisionLanguageService.T("Shell.DockLayerShort"));
            ApplyDockedToolHeaderLocalization();
        }

        public void ApplyDockedToolHeaderLocalization()
        {
            string floatText = TOrFallback("Shell.FloatDockedTool", "Float tool window");
            string closeText = TOrFallback("Shell.CloseDockedTool", "Close tool");

            SetButtonToolTipAndName(floatDockedToolButton, OpenVisionLanguageService.T("Workbench.Tool.Float"));
            SetButtonToolTipAndName(closeDockedToolButton, closeText);
            SetButtonToolTipAndName(floatDockedDocumentButton, floatText);
            SetButtonToolTipAndName(closeDockedDocumentButton, closeText);
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
