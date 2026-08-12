using OpenVisionLab.Core;
using System;
using System.Threading.Tasks;

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

        public Task RecipePreparationTask { get; private set; } = Task.CompletedTask;

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
            OpenVisionNativeToolSettingsStore.ResetContext();
            toolWindowLifecycleController.CloseActiveWpfToolWindow();
            documentController.CloseAllDocuments();
            setActiveDocumentText(string.Empty);
            refreshHostLayerRows();
            refreshHostSelectedLayerDetail(displayManager.SelectedItem);
            refreshDirectRouteText();
            // The recipe is ready when its repository state, layers, routes, and command surface are rebound.
            // Do not include Pipeline Review construction in the Recipe loading task. Prepare it at background
            // priority after the switch so Recipe selection stays responsive and the later explicit open is fast.
            RecipePreparationTask = Task.CompletedTask;
            toolPrewarmController.SchedulePipelineReviewPrewarmAfterIdle();
            // Native documents are rebuilt on the next explicit Tool selection.
            // Restarting the whole prewarm queue here competes with Recipe Manager input.
        }
    }
}
