using OpenVisionLab._1._Core;
using System;

namespace OpenVisionLab
{
    internal sealed class OpenVisionShellHostRecipeController
    {
        private readonly ApplicationRuntimeContext runtimeContext;
        private readonly IDisplayManager displayManager;
        private readonly OpenVisionShellHostDocumentController documentController;
        private readonly OpenVisionShellHostToolPrewarmController toolPrewarmController;
        private readonly OpenVisionShellHostToolWindowLifecycleController toolWindowLifecycleController;
        private readonly Action<string> setActiveDocumentText;
        private readonly Action refreshHostLayerRows;
        private readonly Action<string> refreshHostSelectedLayerDetail;
        private readonly Action refreshDirectRouteText;

        public OpenVisionShellHostRecipeController(
            ApplicationRuntimeContext runtimeContext,
            IDisplayManager displayManager,
            OpenVisionShellHostDocumentController documentController,
            OpenVisionShellHostToolPrewarmController toolPrewarmController,
            OpenVisionShellHostToolWindowLifecycleController toolWindowLifecycleController,
            Action<string> setActiveDocumentText,
            Action refreshHostLayerRows,
            Action<string> refreshHostSelectedLayerDetail,
            Action refreshDirectRouteText)
        {
            this.runtimeContext = runtimeContext ?? throw new ArgumentNullException(nameof(runtimeContext));
            this.displayManager = displayManager ?? throw new ArgumentNullException(nameof(displayManager));
            this.documentController = documentController ?? throw new ArgumentNullException(nameof(documentController));
            this.toolPrewarmController = toolPrewarmController ?? throw new ArgumentNullException(nameof(toolPrewarmController));
            this.toolWindowLifecycleController = toolWindowLifecycleController ?? throw new ArgumentNullException(nameof(toolWindowLifecycleController));
            this.setActiveDocumentText = setActiveDocumentText ?? throw new ArgumentNullException(nameof(setActiveDocumentText));
            this.refreshHostLayerRows = refreshHostLayerRows ?? throw new ArgumentNullException(nameof(refreshHostLayerRows));
            this.refreshHostSelectedLayerDetail = refreshHostSelectedLayerDetail ?? throw new ArgumentNullException(nameof(refreshHostSelectedLayerDetail));
            this.refreshDirectRouteText = refreshDirectRouteText ?? throw new ArgumentNullException(nameof(refreshDirectRouteText));
        }

        public void OnRecipeChanged(object sender, EventArgs e)
        {
            // Recipe changes reload repository-owned Property objects. Cached native tool views must be rebuilt
            // so PropertyGrid editors do not keep editing a previous recipe's model instance.
            toolPrewarmController.Cancel();
            OpenVisionNativeToolPropertySessionStore.SetRepositoryContext(() => runtimeContext.Global?.VisionTools);
            toolWindowLifecycleController.CloseActiveWpfToolWindow();
            documentController.CloseAllDocuments();
            setActiveDocumentText(string.Empty);
            refreshHostLayerRows();
            refreshHostSelectedLayerDetail(displayManager.SelectedItem);
            refreshDirectRouteText();
            toolPrewarmController.ScheduleNativePrewarmIfEnabled();
        }
    }
}
