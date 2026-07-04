using System;
using System.Windows;

namespace OpenVisionLab
{
    internal sealed class OpenVisionShellHostToolWindowLifecycleController
    {
        private readonly OpenVisionShellHostDocumentController documentController;
        private readonly OpenVisionFloatingToolWindowHost floatingToolWindowHost;
        private readonly OpenVisionDockedToolInspectorController dockedToolInspectorController;
        private readonly Action setDirectRunSucceeded;
        private readonly Action<string> setActiveDocumentText;
        private readonly Action refreshHostLayerRows;
        private readonly Action<bool> refreshNativePreviewRouteAfterLayerStateChanged;
        private readonly Action refreshLastNativeOutputWorkspacePreview;

        public OpenVisionShellHostToolWindowLifecycleController(
            OpenVisionShellHostDocumentController documentController,
            OpenVisionFloatingToolWindowHost floatingToolWindowHost,
            OpenVisionDockedToolInspectorController dockedToolInspectorController,
            Action setDirectRunSucceeded,
            Action<string> setActiveDocumentText,
            Action refreshHostLayerRows,
            Action<bool> refreshNativePreviewRouteAfterLayerStateChanged,
            Action refreshLastNativeOutputWorkspacePreview)
        {
            this.documentController = documentController ?? throw new ArgumentNullException(nameof(documentController));
            this.floatingToolWindowHost = floatingToolWindowHost ?? throw new ArgumentNullException(nameof(floatingToolWindowHost));
            this.dockedToolInspectorController = dockedToolInspectorController ?? throw new ArgumentNullException(nameof(dockedToolInspectorController));
            this.setDirectRunSucceeded = setDirectRunSucceeded ?? throw new ArgumentNullException(nameof(setDirectRunSucceeded));
            this.setActiveDocumentText = setActiveDocumentText ?? throw new ArgumentNullException(nameof(setActiveDocumentText));
            this.refreshHostLayerRows = refreshHostLayerRows ?? throw new ArgumentNullException(nameof(refreshHostLayerRows));
            this.refreshNativePreviewRouteAfterLayerStateChanged = refreshNativePreviewRouteAfterLayerStateChanged ?? throw new ArgumentNullException(nameof(refreshNativePreviewRouteAfterLayerStateChanged));
            this.refreshLastNativeOutputWorkspacePreview = refreshLastNativeOutputWorkspacePreview ?? throw new ArgumentNullException(nameof(refreshLastNativeOutputWorkspacePreview));
        }

        public bool IsDockedToolInspectorVisible => dockedToolInspectorController.IsVisible;

        public bool ShowDockedToolWindow(FrameworkElement content, string title, double floatingWidth, double floatingHeight)
        {
            return dockedToolInspectorController.Show(content, title, floatingWidth, floatingHeight);
        }

        public void CloseActiveDocument()
        {
            CloseActiveWpfToolWindow();
            documentController.Dispose();
        }

        public bool CloseActiveWpfToolWindowByUser()
        {
            if (IsDockedToolInspectorVisible)
            {
                CloseDockedToolByUser();
                return true;
            }

            return floatingToolWindowHost.CloseByUser();
        }

        public void CloseActiveWpfToolWindow()
        {
            dockedToolInspectorController.CloseSilently();
            floatingToolWindowHost.CloseSilently();
        }

        public bool FloatDockedTool(Action<FrameworkElement, string, double, double> showFloatingWindow)
        {
            return dockedToolInspectorController.Float(showFloatingWindow);
        }

        public void OnFloatingToolWindowClosedByUser(object sender, EventArgs e)
        {
            CloseVisibleDocumentsAndResetActiveText();
        }

        public void OnFloatingToolWindowDockRequested(object sender, OpenVisionFloatingToolDockRequestedEventArgs e)
        {
            if (e == null)
            {
                return;
            }

            ShowDockedToolWindow(e.Content, e.Title, e.FloatingWidth, e.FloatingHeight);
        }

        public void OnNativeDocumentLayerStateChanged(object sender, EventArgs e)
        {
            bool hasPreviewResult = documentController.ActiveNativeDocument?.HasPreviewResult == true;
            bool showOutputWorkspacePreview =
                e is OpenVisionNativeToolLayerStateChangedEventArgs nativeArgs
                && nativeArgs.ShowOutputWorkspacePreview;
            if (hasPreviewResult && showOutputWorkspacePreview)
            {
                setDirectRunSucceeded();
            }

            RefreshAfterNativeLayerStateChanged(hasPreviewResult && showOutputWorkspacePreview);
        }

        public void RefreshAfterNativeLayerStateChanged(bool hasPreviewResult)
        {
            refreshNativePreviewRouteAfterLayerStateChanged(hasPreviewResult);
            if (!IsDockedToolInspectorVisible)
            {
                floatingToolWindowHost.BringActiveWindowAboveOwnerAirspace();
            }
        }

        private void CloseDockedToolByUser()
        {
            dockedToolInspectorController.CloseByUser();
            CloseVisibleDocumentsAndResetActiveText();
        }

        private void CloseVisibleDocumentsAndResetActiveText()
        {
            bool restoreNativeOutputWorkspacePreview = documentController.ActiveNativeDocument?.HasPreviewResult == true;
            documentController.CloseVisibleDocuments();
            setActiveDocumentText(string.Empty);
            refreshHostLayerRows();
            if (restoreNativeOutputWorkspacePreview)
            {
                refreshLastNativeOutputWorkspacePreview();
            }
        }
    }
}
