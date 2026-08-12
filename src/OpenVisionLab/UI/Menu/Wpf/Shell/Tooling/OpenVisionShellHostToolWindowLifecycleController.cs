using System;
using System.Windows;
using static OpenVisionLab.DEFINE;

namespace OpenVisionLab
{
    internal sealed class OpenVisionShellHostToolWindowLifecycleController
    {
        private readonly OpenVisionShellHostDocumentController documentController;
        private readonly OpenVisionFloatingToolWindowHost floatingToolWindowHost;
        private readonly OpenVisionDockedToolInspectorController dockedToolInspectorController;
        private readonly OpenVisionDockedDocumentWorkspaceController dockedDocumentWorkspaceController;
        private readonly Action setDirectRunSucceeded;
        private readonly Action<string> setActiveDocumentText;
        private readonly Action refreshHostLayerRows;
        private readonly Action<bool> refreshNativePreviewRouteAfterLayerStateChanged;
        private readonly Action refreshLastNativeOutputWorkspacePreview;

        public OpenVisionShellHostToolWindowLifecycleController(
            OpenVisionShellHostDocumentController documentController,
            OpenVisionFloatingToolWindowHost floatingToolWindowHost,
            OpenVisionDockedToolInspectorController dockedToolInspectorController,
            OpenVisionDockedDocumentWorkspaceController dockedDocumentWorkspaceController,
            Action setDirectRunSucceeded,
            Action<string> setActiveDocumentText,
            Action refreshHostLayerRows,
            Action<bool> refreshNativePreviewRouteAfterLayerStateChanged,
            Action refreshLastNativeOutputWorkspacePreview)
        {
            this.documentController = documentController ?? throw new ArgumentNullException(nameof(documentController));
            this.floatingToolWindowHost = floatingToolWindowHost ?? throw new ArgumentNullException(nameof(floatingToolWindowHost));
            this.dockedToolInspectorController = dockedToolInspectorController ?? throw new ArgumentNullException(nameof(dockedToolInspectorController));
            this.dockedDocumentWorkspaceController = dockedDocumentWorkspaceController ?? throw new ArgumentNullException(nameof(dockedDocumentWorkspaceController));
            this.setDirectRunSucceeded = setDirectRunSucceeded ?? throw new ArgumentNullException(nameof(setDirectRunSucceeded));
            this.setActiveDocumentText = setActiveDocumentText ?? throw new ArgumentNullException(nameof(setActiveDocumentText));
            this.refreshHostLayerRows = refreshHostLayerRows ?? throw new ArgumentNullException(nameof(refreshHostLayerRows));
            this.refreshNativePreviewRouteAfterLayerStateChanged = refreshNativePreviewRouteAfterLayerStateChanged ?? throw new ArgumentNullException(nameof(refreshNativePreviewRouteAfterLayerStateChanged));
            this.refreshLastNativeOutputWorkspacePreview = refreshLastNativeOutputWorkspacePreview ?? throw new ArgumentNullException(nameof(refreshLastNativeOutputWorkspacePreview));
        }

        public bool IsDockedToolInspectorVisible => dockedToolInspectorController.IsVisible;

        public bool IsDockedDocumentWorkspaceVisible => dockedDocumentWorkspaceController.IsVisible;

        public bool ShouldShowPipelineReviewDocked =>
            dockedToolInspectorController.IsVisible
            || dockedDocumentWorkspaceController.ShouldRestoreDocked;

        public bool ShowDockedToolWindow(FrameworkElement content, string title, double floatingWidth, double floatingHeight)
        {
            return dockedToolInspectorController.Show(content, title, floatingWidth, floatingHeight);
        }

        public bool ShowDockedDocumentWorkspace(FrameworkElement content, string title, double floatingWidth, double floatingHeight)
        {
            return dockedDocumentWorkspaceController.Show(content, title, floatingWidth, floatingHeight);
        }

        public bool PrepareDockedDocumentWorkspace(FrameworkElement content, string title, double floatingWidth, double floatingHeight)
        {
            return dockedDocumentWorkspaceController.Prepare(content, title, floatingWidth, floatingHeight);
        }

        public void PrepareForToolSelection(VISION_MENU menu)
        {
            if (menu == VISION_MENU.Pipeline)
            {
                dockedToolInspectorController.CloseSilently();
                return;
            }

            dockedDocumentWorkspaceController.SuspendForReuse();
        }

        public void CloseActiveDocument()
        {
            CloseActiveWpfToolWindow();
            documentController.Dispose();
        }

        public bool CloseActiveWpfToolWindowByUser()
        {
            if (IsDockedDocumentWorkspaceVisible)
            {
                dockedDocumentWorkspaceController.CloseByUser();
                CloseVisibleDocumentsAndResetActiveText();
                return true;
            }

            if (IsDockedToolInspectorVisible)
            {
                CloseDockedToolByUser();
                return true;
            }

            return floatingToolWindowHost.CloseByUser();
        }

        public bool SuspendFloatingPipelineReviewForRecipeReturn()
        {
            if (IsDockedToolInspectorVisible || documentController.ActivePipelineReviewDocument == null)
            {
                return false;
            }

            bool hostSuspended = IsDockedDocumentWorkspaceVisible
                ? dockedDocumentWorkspaceController.SuspendForReuse()
                : floatingToolWindowHost.HideForReuse();
            if (!hostSuspended || !documentController.SuspendPipelineReviewForRecipeReturn())
            {
                return false;
            }

            setActiveDocumentText(string.Empty);
            refreshHostLayerRows();
            return true;
        }

        public void CloseActiveWpfToolWindow()
        {
            dockedDocumentWorkspaceController.CloseSilently();
            dockedToolInspectorController.CloseSilently();
            floatingToolWindowHost.CloseSilently();
        }

        public bool FloatDockedTool(Action<FrameworkElement, string, double, double> showFloatingWindow)
        {
            return dockedDocumentWorkspaceController.Float(showFloatingWindow)
                || dockedToolInspectorController.Float(showFloatingWindow);
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

            if (ReferenceEquals(documentController.ActivePipelineReviewDocument?.View, e.Content))
            {
                ShowDockedDocumentWorkspace(e.Content, e.Title, e.FloatingWidth, e.FloatingHeight);
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
            if (sender is not OpenVisionPipelineReviewDocument)
            {
                documentController.RefreshPipelineReviewInputLayerState();
            }
        }

        public void RefreshAfterNativeLayerStateChanged(bool hasPreviewResult)
        {
            refreshNativePreviewRouteAfterLayerStateChanged(hasPreviewResult);
            if (!IsDockedToolInspectorVisible && !IsDockedDocumentWorkspaceVisible)
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
