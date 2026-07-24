using OpenVisionLab._1._Core;
using System;

namespace OpenVisionLab
{
    internal sealed class OpenVisionShellHostStatePresenter
    {
        private readonly IDisplayManager displayManager;
        private readonly OpenVisionShellHostLayerListPresenter layerListPresenter;
        private readonly OpenVisionShellHostDocumentController documentController;
        private readonly OpenVisionFloatingToolWindowHost floatingToolWindowHost;
        private readonly OpenVisionNativeToolPrewarmService nativeToolPrewarmService;
        private readonly OpenVisionShellHostWorkspacePreviewController workspacePreviewController;
        private readonly OpenVisionLayerViewerWindowRegistry layerViewerWindows;

        public OpenVisionShellHostStatePresenter(
            IDisplayManager displayManager,
            OpenVisionShellHostLayerListPresenter layerListPresenter,
            OpenVisionShellHostDocumentController documentController,
            OpenVisionFloatingToolWindowHost floatingToolWindowHost,
            OpenVisionNativeToolPrewarmService nativeToolPrewarmService,
            OpenVisionShellHostWorkspacePreviewController workspacePreviewController,
            OpenVisionLayerViewerWindowRegistry layerViewerWindows)
        {
            this.displayManager = displayManager ?? throw new ArgumentNullException(nameof(displayManager));
            this.layerListPresenter = layerListPresenter ?? throw new ArgumentNullException(nameof(layerListPresenter));
            this.documentController = documentController ?? throw new ArgumentNullException(nameof(documentController));
            this.floatingToolWindowHost = floatingToolWindowHost ?? throw new ArgumentNullException(nameof(floatingToolWindowHost));
            this.nativeToolPrewarmService = nativeToolPrewarmService ?? throw new ArgumentNullException(nameof(nativeToolPrewarmService));
            this.workspacePreviewController = workspacePreviewController ?? throw new ArgumentNullException(nameof(workspacePreviewController));
            this.layerViewerWindows = layerViewerWindows ?? throw new ArgumentNullException(nameof(layerViewerWindows));
        }

        public string ActiveToolFormTypeName => string.Empty;
        public string ActiveWpfToolWindowTypeName => floatingToolWindowHost.ActiveWindow?.GetType().Name ?? string.Empty;
        public string ActiveWpfToolWindowTitle => floatingToolWindowHost.ActiveWindow?.Title ?? string.Empty;
        public string ActiveWpfToolWindowMinimizeToolTip => floatingToolWindowHost.ActiveWindow?.MinimizeToolTipText ?? string.Empty;
        public string ActiveWpfToolWindowMaximizeRestoreToolTip => floatingToolWindowHost.ActiveWindow?.MaximizeRestoreToolTipText ?? string.Empty;
        public string ActiveWpfToolWindowCloseToolTip => floatingToolWindowHost.ActiveWindow?.CloseToolTipText ?? string.Empty;
        public bool IsActiveWpfToolWindowVisibleForTest => floatingToolWindowHost.ActiveWindow?.IsVisible == true;
        public string ActivePendingToolTitle => documentController.ActivePendingToolViewModel?.Title ?? string.Empty;
        public string ActivePendingToolStatusText => documentController.ActivePendingToolViewModel?.StatusText ?? string.Empty;
        public string ActiveNativeDocumentTypeName => documentController.ActiveNativeDocument?.ActiveViewTypeName ?? documentController.ActivePipelineReviewDocument?.ActiveViewTypeName ?? string.Empty;
        public string ActiveNativeStatusText => documentController.ActiveNativeDocument?.LastStatusText ?? string.Empty;
        public string ActiveNativeResultReviewText => documentController.ActiveNativeDocument?.ResultReviewText ?? string.Empty;
        public string ActiveNativeRouteInputLayerNameForTest => documentController.ActiveNativeDocument?.RouteInputLayerName ?? string.Empty;
        public string ActiveNativeRouteInputLayerBNameForTest => documentController.ActiveNativeDocument?.RouteInputLayerBName ?? string.Empty;
        public string ActiveNativeRouteOutputLayerNameForTest => documentController.ActiveNativeDocument?.RouteOutputLayerName ?? string.Empty;
        public bool IsNativeToolPrewarmCompletedForTest => nativeToolPrewarmService.IsCompleted;
        public int NativeToolPrewarmCreatedCountForTest => nativeToolPrewarmService.CreatedCount;
        public long NativeToolPrewarmElapsedMillisecondsForTest => nativeToolPrewarmService.ElapsedMilliseconds;
        public int NativeToolDocumentCacheCountForTest => documentController.NativeToolDocumentCacheCount;
        public int HostedDocumentCount => documentController.HostedDocumentCount;
        public bool IsNativeDocumentActive => documentController.IsNativeDocumentActive;
        public int NativePreviewRunCount => documentController.ActiveNativeDocument?.PreviewRunCount ?? 0;
        public bool HasNativePreviewResult => documentController.ActiveNativeDocument?.HasPreviewResult ?? false;
        public int ActiveLineInputRoiOverlayCount => documentController.ActiveNativeDocument?.LineInputRoiOverlayCount ?? 0;
        public int LayerDocumentCount => displayManager.LayerCount;
        public bool HasMainLayer => displayManager.FindIndex("Main") >= 0;
        public int HostLayerRowCount => layerListPresenter.RowCount;
        public string ActiveHostLayerTitle => layerListPresenter.ActiveLayerTitle;
        public bool HasWorkspaceLayerPreview => workspacePreviewController.HasImage;
        public int WorkspaceTextureTileCount => workspacePreviewController.TextureTileCount;
        public int OpenLayerViewerWindowCount => layerViewerWindows.Count;
        public string OpenLayerViewerWindowTitles => layerViewerWindows.Titles;
        public int PipelineReviewStepCount => documentController.ActivePipelineReviewDocument?.StepCount ?? 0;
        public string PipelineReviewRecipeContextName => documentController.ActivePipelineReviewDocument?.RecipeContext?.Name ?? string.Empty;
        public string PipelineReviewRecipeContextPipelineName => documentController.ActivePipelineReviewDocument?.ActivePipelineName ?? string.Empty;
        public string PipelineReviewSelectedStepName => documentController.ActivePipelineReviewDocument?.SelectedStepName ?? string.Empty;
        public string PipelineReviewSelectedStatusText => documentController.ActivePipelineReviewDocument?.SelectedStatusText ?? string.Empty;
        public string PipelineReviewFlowSummaryText => documentController.ActivePipelineReviewDocument?.FlowSummaryText ?? string.Empty;
        public string PipelineReviewParameterSummaryText => documentController.ActivePipelineReviewDocument?.ParameterSummaryText ?? string.Empty;
        public string PipelineReviewValidationStatusText => documentController.ActivePipelineReviewDocument?.ValidationStatusText ?? string.Empty;
        public string PipelineReviewValidationDetailText => documentController.ActivePipelineReviewDocument?.ValidationDetailText ?? string.Empty;
        public string PipelineReviewResultSummaryText => documentController.ActivePipelineReviewDocument?.ResultSummaryText ?? string.Empty;
        public string PipelineReviewResultDetailText => documentController.ActivePipelineReviewDocument?.ResultDetailText ?? string.Empty;
        public string PipelineReviewRunLogText => documentController.ActivePipelineReviewDocument?.RunLogText ?? string.Empty;
        public string PipelineReviewExecutionState => documentController.ActivePipelineReviewDocument?.ReviewExecutionState ?? string.Empty;
        public string PipelineReviewProgressText => documentController.ActivePipelineReviewDocument?.ReviewProgressText ?? string.Empty;
        public string PipelineReviewGuideStageText => documentController.ActivePipelineReviewDocument?.GuideStageText ?? string.Empty;
        public string PipelineReviewGuideCurrentStepText => documentController.ActivePipelineReviewDocument?.GuideCurrentStepText ?? string.Empty;
        public string PipelineReviewGuideNextActionText => documentController.ActivePipelineReviewDocument?.GuideNextActionText ?? string.Empty;
        public string PipelineReviewGuideResultDecisionText => documentController.ActivePipelineReviewDocument?.GuideResultDecisionText ?? string.Empty;
        public string PipelineReviewGuideDetailText => documentController.ActivePipelineReviewDocument?.GuideDetailText ?? string.Empty;
        public string PipelineReviewGuidePairText => documentController.ActivePipelineReviewDocument?.GuidePairText ?? string.Empty;
        public string PipelineReviewGuidePairActionText => documentController.ActivePipelineReviewDocument?.GuidePairActionText ?? string.Empty;
        public string PipelineReviewGuidePairMetricText => documentController.ActivePipelineReviewDocument?.GuidePairMetricText ?? string.Empty;
        public string PipelineReviewGuideChecklistText => documentController.ActivePipelineReviewDocument?.GuideChecklistText ?? string.Empty;
        public string PipelineReviewGuideParameterFocusText => documentController.ActivePipelineReviewDocument?.GuideParameterFocusText ?? string.Empty;
        public string PipelineReviewGuideTriageFailureText => documentController.ActivePipelineReviewDocument?.GuideTriageFailureText ?? string.Empty;
        public string PipelineReviewGuideTriageAdjustmentText => documentController.ActivePipelineReviewDocument?.GuideTriageAdjustmentText ?? string.Empty;
        public string PipelineReviewGuideTriageRerunText => documentController.ActivePipelineReviewDocument?.GuideTriageRerunText ?? string.Empty;
        public bool CanOpenPipelineReviewPairSample => documentController.ActivePipelineReviewDocument?.CanOpenPairSample ?? false;
        public bool CanSelectPreviousPipelineReviewStep => documentController.ActivePipelineReviewDocument?.CanSelectPreviousStep ?? false;
        public bool CanSelectNextPipelineReviewStep => documentController.ActivePipelineReviewDocument?.CanSelectNextStep ?? false;
        public bool CanSelectFirstIssuePipelineReviewStep => documentController.ActivePipelineReviewDocument?.CanSelectFirstIssueStep ?? false;
        public bool HasPipelineReviewInputPreview => documentController.ActivePipelineReviewDocument?.HasInputPreview ?? false;
        public bool HasPipelineReviewOutputPreview => documentController.ActivePipelineReviewDocument?.HasOutputPreview ?? false;
        public int PipelineReviewObjectResultCount => documentController.ActivePipelineReviewDocument?.ObjectResultCount ?? 0;

        public bool IsPipelineReviewFixtureDesignerVisible => documentController.ActivePipelineReviewDocument?.IsFixtureDesignerVisible == true;

        public string PipelineReviewFixtureRelationshipText => documentController.ActivePipelineReviewDocument?.FixtureRelationshipText ?? string.Empty;

        public int PipelineReviewFixtureProducerStepNumber => documentController.ActivePipelineReviewDocument?.FixtureProducerStepNumber ?? 0;

        public int PipelineReviewFixtureMeasurementStepNumber => documentController.ActivePipelineReviewDocument?.FixtureMeasurementStepNumber ?? 0;
        public int PipelineReviewSelectedObjectResultNumber => documentController.ActivePipelineReviewDocument?.SelectedObjectResultNumber ?? 0;
        public bool HasPipelineReviewObjectHighlight => documentController.ActivePipelineReviewDocument?.HasObjectHighlight ?? false;
    }
}
