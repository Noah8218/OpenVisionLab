using OpenVisionLab.Vision2D.Pipeline;
using OpenVisionLab.Core;
using OpenVisionLab.Docking.Controls;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using static OpenVisionLab.DEFINE;

namespace OpenVisionLab
{
    public sealed class OpenVisionShellHostViewTestSurface
    {
        private readonly OpenVisionShellHostViewTestSurfaceBindings _bindings;

        internal OpenVisionShellHostViewTestSurface(OpenVisionShellHostViewTestSurfaceBindings bindings)
        {
            _bindings = bindings ?? throw new ArgumentNullException(nameof(bindings));
        }
        internal OpenVisionTcpIntegrationController TcpIntegrationControllerForTest => _bindings.TcpIntegrationController;

        internal void OpenTcpIntegrationForTest() =>
            _bindings.TcpIntegrationController.Show(_bindings.GetOwnerWindow());

        internal Func<string, string, string, bool> QualifiedSnapshotLifecycleConfirmationForTest
        {
            get => _bindings.RecipeDialogAdapter.QualifiedSnapshotLifecycleConfirmationForTest;
            set => _bindings.RecipeDialogAdapter.QualifiedSnapshotLifecycleConfirmationForTest = value;
        }

        internal Func<string, bool> QualifiedSnapshotEvidenceOpenerForTest
        {
            get => _bindings.RecipeDialogAdapter.QualifiedSnapshotEvidenceOpenerForTest;
            set => _bindings.RecipeDialogAdapter.QualifiedSnapshotEvidenceOpenerForTest = value;
        }

        public string ActiveToolFormTypeName => _bindings.ToolTestFacade.ActiveToolFormTypeName;
        public string ActiveWpfToolWindowTypeName => _bindings.ToolTestFacade.ActiveWpfToolWindowTypeName;
        public string ActiveWpfToolWindowTitle => _bindings.ToolTestFacade.ActiveWpfToolWindowTitle;
        public string ActiveWpfToolWindowMinimizeToolTip => _bindings.ToolTestFacade.ActiveWpfToolWindowMinimizeToolTip;
        public string ActiveWpfToolWindowMaximizeRestoreToolTip => _bindings.ToolTestFacade.ActiveWpfToolWindowMaximizeRestoreToolTip;
        public string ActiveWpfToolWindowCloseToolTip => _bindings.ToolTestFacade.ActiveWpfToolWindowCloseToolTip;
        public bool IsActiveWpfToolWindowVisibleForTest => _bindings.ToolTestFacade.IsActiveWpfToolWindowVisible;
        public bool IsDockedToolInspectorVisibleForTest => _bindings.ToolTestFacade.IsDockedToolInspectorVisible;

        public bool IsDockedDocumentWorkspaceVisibleForTest => _bindings.ToolTestFacade.IsDockedDocumentWorkspaceVisible;
        public string ActivePendingToolTitle => _bindings.ToolTestFacade.ActivePendingToolTitle;
        public string ActivePendingToolStatusText => _bindings.ToolTestFacade.ActivePendingToolStatusText;
        public string ActiveNativeDocumentTypeName => _bindings.ToolTestFacade.ActiveNativeDocumentTypeName;
        public string ActiveNativeStatusText => _bindings.ToolTestFacade.ActiveNativeStatusText;
        public string ActiveNativeResultReviewText => _bindings.ToolTestFacade.ActiveNativeResultReviewText;
        public string ActiveNativeRouteInputLayerNameForTest => _bindings.ToolTestFacade.ActiveNativeRouteInputLayerName;
        public string ActiveNativeRouteInputLayerBNameForTest => _bindings.ToolTestFacade.ActiveNativeRouteInputLayerBName;
        public string ActiveNativeRouteOutputLayerNameForTest => _bindings.ToolTestFacade.ActiveNativeRouteOutputLayerName;
        public bool IsNativeToolPrewarmCompletedForTest => _bindings.ToolTestFacade.IsNativeToolPrewarmCompleted;
        public int NativeToolPrewarmCreatedCountForTest => _bindings.ToolTestFacade.NativeToolPrewarmCreatedCount;
        public long NativeToolPrewarmElapsedMillisecondsForTest => _bindings.ToolTestFacade.NativeToolPrewarmElapsedMilliseconds;
        public int NativeToolDocumentCacheCountForTest => _bindings.ToolTestFacade.NativeToolDocumentCacheCount;
        public string LastToolOpenTimingTextForTest => _bindings.ToolTestFacade.LastToolOpenTimingText;
        public bool HasPipelineReviewDocumentForTest => _bindings.ToolTestFacade.HasPipelineReviewDocument;
        public bool IsShellLoadedForTest => _bindings.ToolTestFacade.IsShellLoaded;
        public int HostedDocumentCount => _bindings.ToolTestFacade.HostedDocumentCount;
        public bool IsNativeDocumentActive => _bindings.ToolTestFacade.IsNativeDocumentActive;
        public int NativePreviewRunCount => _bindings.ToolTestFacade.NativePreviewRunCount;
        public bool HasNativePreviewResult => _bindings.ToolTestFacade.HasNativePreviewResult;
        public VisionToolRepository VisionToolRepositoryForTest => _bindings.RuntimeContext.Global?.VisionTools;
        public int ActiveLineInputRoiOverlayCount => _bindings.ToolTestFacade.ActiveLineInputRoiOverlayCount;
        public bool ActiveLineSignalInspectorHasEvidenceForTest => _bindings.ToolTestFacade.ActiveLineSignalInspectorHasEvidence;
        public bool ActiveLineSignalInspectorOverlayVisibleForTest => _bindings.ToolTestFacade.ActiveLineSignalInspectorOverlayVisible;
        public bool ActiveLineSignalEvidenceCueVisibleForTest => _bindings.ToolTestFacade.ActiveLineSignalEvidenceCueVisible;
        public string ActiveLineSignalInspectorEvidenceIdForTest => _bindings.ToolTestFacade.ActiveLineSignalInspectorEvidenceId;
        public string ActiveLineSignalInspectorSourceSha256ForTest => _bindings.ToolTestFacade.ActiveLineSignalInspectorSourceSha256;
        public int ActiveLineSignalInspectorSeriesCountForTest => _bindings.ToolTestFacade.ActiveLineSignalInspectorSeriesCount;
        public int ActiveLineSignalInspectorMarkerCountForTest => _bindings.ToolTestFacade.ActiveLineSignalInspectorMarkerCount;
        public int LayerDocumentCount => _bindings.LayerTestFacade.LayerDocumentCount;
        public bool HasMainLayer => _bindings.LayerTestFacade.HasMainLayer;
        public int HostLayerRowCount => _bindings.LayerTestFacade.HostLayerRowCount;
        public string ActiveHostLayerTitle => _bindings.LayerTestFacade.ActiveHostLayerTitle;
        public string SelectedHostLayerTitle => _bindings.LayerTestFacade.SelectedHostLayerTitle;
        public string SelectedHostLayerMeta => _bindings.LayerTestFacade.SelectedHostLayerMeta;
        public bool HasSelectedHostLayerPreview => _bindings.LayerTestFacade.HasSelectedHostLayerPreview;
        public bool HasWorkspaceLayerPreview => _bindings.LayerTestFacade.HasWorkspaceLayerPreview;
        public bool IsSingleWorkspaceVisibleForTest => _bindings.LayerTestFacade.IsSingleWorkspaceVisible;
        public bool IsDockedWorkspaceVisibleForTest => _bindings.DockingTestFacade.IsDockedWorkspaceVisible;
        public bool IsWorkspaceLayerDropEnabledForTest => _bindings.LayerTestFacade.IsWorkspaceLayerDropEnabled;
        public bool HasWorkspaceDropOverlayForTest => _bindings.LayerTestFacade.HasWorkspaceDropOverlay;
        public bool IsWorkspaceDropOverlayVisibleForTest => _bindings.LayerTestFacade.IsWorkspaceDropOverlayVisible;
        public bool IsWorkspaceDropOverlayHitTestSafeForTest => _bindings.LayerTestFacade.IsWorkspaceDropOverlayHitTestSafe;
        public bool HasDockingGuideOverlayForTest => _bindings.DockingTestFacade.HasGuideOverlay;
        public bool IsDockingGuideOverlayVisibleForTest => _bindings.DockingTestFacade.IsGuideOverlayVisible;
        public string ActiveDockingGuideZoneForTest => _bindings.DockingTestFacade.ActiveGuideZone;
        public bool IsDockingGuideOverlayHitTestSafeForTest => _bindings.DockingTestFacade.IsGuideOverlayHitTestSafe;
        public int DockingGuideZoneCountForTest => _bindings.DockingTestFacade.GuideZoneCount;
        public int WorkspaceTextureTileCount => _bindings.LayerTestFacade.WorkspaceTextureTileCount;
        public bool IsWorkspaceEmptyPromptVisible => _bindings.LayerTestFacade.IsWorkspaceEmptyPromptVisible;
        public string WorkspaceCoordinatesTextForTest => _bindings.LayerTestFacade.WorkspaceCoordinatesText;
        public string WorkspacePixelTextForTest => _bindings.LayerTestFacade.WorkspacePixelText;
        public string WorkspaceEmptyTitle => _bindings.LayerTestFacade.WorkspaceEmptyTitle;
        public string WorkspaceEmptyDetail => _bindings.LayerTestFacade.WorkspaceEmptyDetail;
        public string WorkspaceLayerTitle => _bindings.LayerTestFacade.WorkspaceLayerTitle;
        public string WorkspaceLayerMeta => _bindings.LayerTestFacade.WorkspaceLayerMeta;
        public string WorkspaceLoadImageMenuText => _bindings.LayerTestFacade.WorkspaceLoadImageMenuText;
        public string WorkspaceLoadImageButtonText => _bindings.LayerTestFacade.WorkspaceLoadImageButtonText;
        public bool HasWorkspaceLoadImageMenu => _bindings.LayerTestFacade.HasWorkspaceLoadImageMenu;
        public bool IsWorkspaceLoadImageIntoLayerMenuVisibleForTest => _bindings.LayerTestFacade.IsWorkspaceLoadImageIntoLayerMenuVisible;
        public int OpenLayerViewerWindowCount => _bindings.LayerTestFacade.OpenLayerViewerWindowCount;
        public string OpenLayerViewerWindowTitles => _bindings.LayerTestFacade.OpenLayerViewerWindowTitles;
        public int DockedLayerCount => _bindings.DockingTestFacade.LayerCount;
        public int DockedLayerTextureTileCount => _bindings.DockingTestFacade.TextureTileCount;
        public int DockedLayerPaneCount => _bindings.DockingTestFacade.PaneCount;
        public string DockedLayerRootOrientationForTest => _bindings.DockingTestFacade.RootOrientationName;
        public int DockedLayerNestedLayoutPanelCountForTest => _bindings.DockingTestFacade.NestedLayoutPanelCount;
        public bool AreDockedLayerViewersCompactSizeReadyForTest => _bindings.DockingTestFacade.AreViewersCompactSizeReady;
        public bool IsToolRailCompactForTest => _bindings.ToolTestFacade.IsToolRailCompact;
        public double ToolRailWidthForTest => _bindings.ToolTestFacade.ToolRailWidth;
        public bool IsToolRailNavigationVisibleForTest => _bindings.ToolTestFacade.IsToolRailNavigationVisible;
        public bool IsToolRailCompactLabelHiddenForTest => _bindings.ToolTestFacade.IsToolRailCompactLabelHidden;
        public string ToolSearchTextForTest => _bindings.ViewModel.ToolSearchText;
        public int VisibleToolSearchItemCountForTest => _bindings.ViewModel.VisibleToolCount;
        public string VisibleToolSearchCommandIdsForTest => _bindings.ViewModel.VisibleToolCommandIds;
        public bool AreDockedLayersNativeFloatingDisabledForTest => _bindings.DockingTestFacade.AreNativeFloatingDisabled;
        public bool AreDockedLayerViewersCompactForTest => _bindings.DockingTestFacade.AreViewersCompact;
        public int DockedLayerTabHeaderCount => _bindings.DockingTestFacade.TabHeaderCount;
        public bool AreDockedLayerTabHeadersGestureReadyForTest => _bindings.DockingTestFacade.AreTabHeadersGestureReady;
        public bool AreDockedLayerTabHeadersReadableForTest => _bindings.DockingTestFacade.AreTabHeadersReadable;
        public bool AreDockedLayerTabHeaderGripsReadyForTest => _bindings.DockingTestFacade.AreTabHeaderGripsReady;
        public bool AreDockedLayersNativeFloatingEnabledForTest => _bindings.DockingTestFacade.AreNativeFloatingEnabled;
        public string DockedLayerTabHeaderDiagnosticsForTest => _bindings.DockingTestFacade.TabHeaderDiagnostics;
        public string DockedLayerTitles => _bindings.DockingTestFacade.Titles;
        public OpenVisionDockingVisualSnapshot DockedLayerVisualSnapshotForTest => _bindings.DockingTestFacade.CreateDockingVisualSnapshot();
        public string DirectResultBadgeText => _bindings.ToolTestFacade.DirectResultBadgeText;
        public string DirectResultTitleText => _bindings.ToolTestFacade.DirectResultTitleText;
        public string DirectResultStatusText => _bindings.ToolTestFacade.DirectResultStatusText;
        public string DirectResultRouteText => _bindings.ToolTestFacade.DirectResultRouteText;
        public string ActiveNativeRecipeContextNameForTest => _bindings.DocumentController.ActiveNativeDocument?.RecipeContextName ?? string.Empty;
        public string ActiveNativeRecipeContextPipelineNameForTest => _bindings.DocumentController.ActiveNativeDocument?.RecipeContextPipelineName ?? string.Empty;
        public int PipelineReviewStepCount => _bindings.ToolTestFacade.PipelineReviewStepCount;
        public string PipelineReviewRecipeContextNameForTest => _bindings.ToolTestFacade.PipelineReviewRecipeContextName;
        public string PipelineReviewRecipeContextPipelineNameForTest => _bindings.ToolTestFacade.PipelineReviewRecipeContextPipelineName;
        public string PipelineReviewSelectedStepName => _bindings.ToolTestFacade.PipelineReviewSelectedStepName;
        public string PipelineReviewSelectedStatusText => _bindings.ToolTestFacade.PipelineReviewSelectedStatusText;
        public string PipelineReviewFlowSummaryText => _bindings.ToolTestFacade.PipelineReviewFlowSummaryText;
        public string PipelineReviewParameterSummaryText => _bindings.ToolTestFacade.PipelineReviewParameterSummaryText;
        public string PipelineReviewValidationStatusText => _bindings.ToolTestFacade.PipelineReviewValidationStatusText;
        public string PipelineReviewValidationDetailText => _bindings.ToolTestFacade.PipelineReviewValidationDetailText;
        public string PipelineReviewResultSummaryText => _bindings.ToolTestFacade.PipelineReviewResultSummaryText;
        public string PipelineReviewResultDetailText => _bindings.ToolTestFacade.PipelineReviewResultDetailText;
        public string PipelineReviewRunLogText => _bindings.ToolTestFacade.PipelineReviewRunLogText;
        public string PipelineReviewExecutionState => _bindings.ToolTestFacade.PipelineReviewExecutionState;
        public string PipelineReviewProgressText => _bindings.ToolTestFacade.PipelineReviewProgressText;
        public string PipelineReviewGuideStageText => _bindings.ToolTestFacade.PipelineReviewGuideStageText;
        public string PipelineReviewGuideCurrentStepText => _bindings.ToolTestFacade.PipelineReviewGuideCurrentStepText;
        public string PipelineReviewGuideNextActionText => _bindings.ToolTestFacade.PipelineReviewGuideNextActionText;
        public string PipelineReviewGuideResultDecisionText => _bindings.ToolTestFacade.PipelineReviewGuideResultDecisionText;
        public string PipelineReviewGuideDetailText => _bindings.ToolTestFacade.PipelineReviewGuideDetailText;
        public string PipelineReviewGuidePairText => _bindings.ToolTestFacade.PipelineReviewGuidePairText;
        public string PipelineReviewGuidePairActionText => _bindings.ToolTestFacade.PipelineReviewGuidePairActionText;
        public string PipelineReviewGuidePairMetricText => _bindings.ToolTestFacade.PipelineReviewGuidePairMetricText;
        public string PipelineReviewGuideChecklistText => _bindings.ToolTestFacade.PipelineReviewGuideChecklistText;
        public string PipelineReviewGuideParameterFocusText => _bindings.ToolTestFacade.PipelineReviewGuideParameterFocusText;
        public string PipelineReviewGuideTriageFailureText => _bindings.ToolTestFacade.PipelineReviewGuideTriageFailureText;
        public string PipelineReviewGuideTriageAdjustmentText => _bindings.ToolTestFacade.PipelineReviewGuideTriageAdjustmentText;
        public string PipelineReviewGuideTriageRerunText => _bindings.ToolTestFacade.PipelineReviewGuideTriageRerunText;
        public bool CanOpenPipelineReviewPairSampleForTest => _bindings.ToolTestFacade.CanOpenPipelineReviewPairSample;
        public bool CanSelectPreviousPipelineReviewStepForTest => _bindings.ToolTestFacade.CanSelectPreviousPipelineReviewStep;
        public bool CanSelectNextPipelineReviewStepForTest => _bindings.ToolTestFacade.CanSelectNextPipelineReviewStep;
        public bool CanSelectFirstIssuePipelineReviewStepForTest => _bindings.ToolTestFacade.CanSelectFirstIssuePipelineReviewStep;
        public bool HasPipelineReviewInputPreview => _bindings.ToolTestFacade.HasPipelineReviewInputPreview;
        public bool HasPipelineReviewOutputPreview => _bindings.ToolTestFacade.HasPipelineReviewOutputPreview;
        public int PipelineReviewObjectResultCountForTest => _bindings.ToolTestFacade.PipelineReviewObjectResultCount;
        public int PipelineReviewObjectMetricDistributionSeriesCountForTest => _bindings.ToolTestFacade.PipelineReviewObjectMetricDistributionSeriesCount;
        public int PipelineReviewObjectMetricDistributionMarkerCountForTest => _bindings.ToolTestFacade.PipelineReviewObjectMetricDistributionMarkerCount;
        public string PipelineReviewObjectMetricDistributionMetricForTest => _bindings.ToolTestFacade.PipelineReviewObjectMetricDistributionMetric;
        public string PipelineReviewObjectMetricDistributionEvidenceIdForTest => _bindings.ToolTestFacade.PipelineReviewObjectMetricDistributionEvidenceId;
        public bool PipelineReviewMatcherDiagnosticTabVisibleForTest => _bindings.ToolTestFacade.PipelineReviewMatcherDiagnosticTabVisible;
        public string PipelineReviewMatcherDiagnosticStateForTest => _bindings.ToolTestFacade.PipelineReviewMatcherDiagnosticState;
        public string PipelineReviewMatcherDiagnosticEvidenceIdForTest => _bindings.ToolTestFacade.PipelineReviewMatcherDiagnosticEvidenceId;
        public int PipelineReviewMatcherDiagnosticRowCountForTest => _bindings.ToolTestFacade.PipelineReviewMatcherDiagnosticRowCount;
        public int PipelineReviewMatcherDiagnosticModelPointCountForTest => _bindings.ToolTestFacade.PipelineReviewMatcherDiagnosticModelPointCount;
        public bool PipelineReviewMatcherDiagnosticHasSelectedCandidateForTest => _bindings.ToolTestFacade.PipelineReviewMatcherDiagnosticHasSelectedCandidate;
        public bool PipelineReviewMatcherDiagnosticHasAlternativeForTest => _bindings.ToolTestFacade.PipelineReviewMatcherDiagnosticHasAlternative;
        public bool IsPipelineReviewFixtureDesignerVisibleForTest => _bindings.ToolTestFacade.IsPipelineReviewFixtureDesignerVisible;
        public string PipelineReviewFixtureRelationshipTextForTest => _bindings.ToolTestFacade.PipelineReviewFixtureRelationshipText;
        public int PipelineReviewFixtureProducerStepNumberForTest => _bindings.ToolTestFacade.PipelineReviewFixtureProducerStepNumber;
        public int PipelineReviewFixtureMeasurementStepNumberForTest => _bindings.ToolTestFacade.PipelineReviewFixtureMeasurementStepNumber;
        public int PipelineReviewSelectedObjectResultNumberForTest => _bindings.ToolTestFacade.PipelineReviewSelectedObjectResultNumber;
        public bool HasPipelineReviewObjectHighlightForTest => _bindings.ToolTestFacade.HasPipelineReviewObjectHighlight;

        public void SelectToolForTest(VISION_MENU menu) => _bindings.ToolTestFacade.SelectTool(menu);

        public void ToggleToolRailForTest() => _bindings.ToolTestFacade.ToggleToolRail();

        public void SetToolSearchTextForTest(string text) => _bindings.ViewModel.ToolSearchText = text;

        public void ClearToolSearchForTest() => _bindings.ViewModel.ClearToolSearchCommand.Execute(null);

        public bool HasNativeToolDocumentCachedForTest(VISION_MENU menu) => _bindings.ToolTestFacade.HasNativeToolDocumentCached(menu);

        public void RunActiveNativePreviewForTest() => _bindings.ToolTestFacade.RunActiveNativePreview();

        public void CreateActiveNativeOutputLayerForTest() => _bindings.ToolTestFacade.CreateActiveNativeOutputLayer();

        public bool OpenLayerViewerForTest(string layerTitle) => _bindings.LayerTestFacade.OpenLayerViewer(layerTitle);

        public bool HasLayerForTest(string layerTitle) => _bindings.LayerTestFacade.HasLayer(layerTitle);

        public string HostLayerTabTextsForTest => _bindings.LayerTestFacade.HostLayerTabTexts;

        public bool AreHostLayerTabsReadableForTest => _bindings.LayerTestFacade.AreHostLayerTabsReadable;

        public Bitmap GetLayerImageCloneForTest(string layerTitle) => _bindings.LayerTestFacade.GetLayerImageClone(layerTitle);

        public bool ActivateHostLayerForTest(string layerTitle) => _bindings.LayerTestFacade.ActivateHostLayer(layerTitle);

        public bool SelectHostLayerRowForTest(string layerTitle) => _bindings.LayerTestFacade.SelectHostLayerRow(layerTitle);

        public bool RightClickHostLayerRowForTest(string layerTitle) => _bindings.LayerTestFacade.RightClickHostLayerRow(layerTitle);

        public bool DockLayerForTest(string layerTitle) => _bindings.DockingTestFacade.DockLayerDocument(layerTitle);

        public bool ActivateDockedLayerForTest(string layerTitle)
        {
            if (!_bindings.DockedLayerWorkspaceComposition.Commands.ActivateLayerDocument(layerTitle))
            {
                return false;
            }

            _bindings.ActivateDockedLayer(layerTitle);
            return true;
        }

        public bool AddLayerImageForTest(string layerTitle, Bitmap image) => _bindings.LayerTestFacade.AddLayerImage(layerTitle, image);

        public string CreateLayerForTest() => _bindings.LayerManagementController.CreateLayer();

        public bool LoadImageIntoLayerForTest(string layerTitle, string path) =>
            _bindings.LayerManagementController.LoadImageIntoLayer(layerTitle, path);

        public bool SetLayerImageForTest(string layerTitle, Bitmap image) =>
            _bindings.LayerManagementController.SetLayerImage(layerTitle, image);

        public bool RenameLayerForTest(string oldLayerTitle, string newLayerTitle) =>
            _bindings.LayerManagementController.RenameLayer(oldLayerTitle, newLayerTitle);

        public bool DeleteLayerForTest(string layerTitle) => _bindings.LayerManagementController.DeleteLayer(layerTitle);

        public void ClearLayerImageHistoryForTest() => (_bindings.DisplayManager as DisplayManagerService)?.ClearLayerImageHistory();

        public bool SplitDockedLayerForTest(string layerTitle) => _bindings.DockingTestFacade.SplitLayerToNewPane(layerTitle);

        public bool ArrangeDockedLayerPanesForTest(string orientationName, params string[] layerTitles) =>
            _bindings.DockingTestFacade.ArrangeLayerPanes(orientationName, layerTitles);

        public bool ArrangeDockedLayerGridForTest(params string[] layerTitles) => _bindings.DockingTestFacade.ArrangeLayerGrid(layerTitles);

        public bool MoveDockedLayerToPrimaryPaneForTest(string layerTitle) => _bindings.DockingTestFacade.MoveLayerToPrimaryPane(layerTitle);

        public bool DockLayerToGuideZoneForTest(string layerTitle, string zoneName) => _bindings.DockingTestFacade.MoveLayerToGuideZone(layerTitle, zoneName);

        public void ClearDockedLayersForTest() => _bindings.DockingTestFacade.ClearDockedLayerDocuments();

        public void ShowDockingGuideForTest(double xRatio = 0.5D, double yRatio = 0.5D) =>
            _bindings.DockingTestFacade.ShowDockingGuide(xRatio, yRatio);

        public System.Windows.Point GetDockedWorkspaceScreenPointForTest(double x, double y) =>
            _bindings.DockingTestFacade.GetWorkspaceScreenPoint(x, y);

        public bool ShowDockedLayerTabDragGuideForTest() => _bindings.DockingTestFacade.ShowFirstDockedLayerTabDragGuide();

        public void HideDockingGuideForTest() => _bindings.DockingTestFacade.HideDockingGuide();

        public void SaveDockingWorkspaceStateForTest() => _bindings.DockingTestFacade.SaveLayerWorkspaceState();

        public bool RestoreDockingLayoutStateForTest() => _bindings.DockingTestFacade.RestoreLayerWorkspaceState();

        public bool SaveWorkspaceImageToFileForTest(string path) => _bindings.LayerTestFacade.SaveWorkspaceImageToFile(path);

        public bool SaveDockedLayerImageToFileForTest(string layerTitle, string path) =>
            _bindings.DockingTestFacade.SaveDockedLayerImageToFile(layerTitle, path);

        public Bitmap CloneDockedLayerImageForTest(string layerTitle) =>
            _bindings.DockingTestFacade.CloneDockedLayerImage(layerTitle);

        public int GetDockedLayerImagePixelWidthForTest(string layerTitle) =>
            _bindings.DockingTestFacade.GetLayerImagePixelWidth(layerTitle);

        public int GetDockedLayerImagePixelHeightForTest(string layerTitle) =>
            _bindings.DockingTestFacade.GetLayerImagePixelHeight(layerTitle);

        public int GetDockedLayerTextureTileCountForTest(string layerTitle) =>
            _bindings.DockingTestFacade.GetLayerTextureTileCount(layerTitle);

        public int LiveLayerViewerInstanceCountForTest => OpenVisionLayerViewerView.LiveInstanceCountForTest;

        public string LiveLayerViewerInstanceStatesForTest => OpenVisionLayerViewerView.LiveInstanceStatesForTest;

        public bool CloseActiveWpfToolWindowForTest() => _bindings.ToolTestFacade.CloseActiveWpfToolWindow();

        public bool DockActiveWpfToolWindowForTest() => _bindings.ToolTestFacade.DockActiveWpfToolWindow();

        public bool FloatDockedWpfToolWindowForTest() => _bindings.ToolTestFacade.FloatDockedWpfToolWindow();

        public string DockedToolTitleForTest => _bindings.ToolTestFacade.DockedToolTitle;

        public bool IsDockedToolFloatButtonVisibleForTest => _bindings.ToolTestFacade.IsDockedToolFloatButtonVisible;

        public bool IsDockedToolCloseButtonVisibleForTest => _bindings.ToolTestFacade.IsDockedToolCloseButtonVisible;

        public double DockedToolFloatButtonWidthForTest => _bindings.ToolTestFacade.DockedToolFloatButtonWidth;

        public double DockedToolCloseButtonWidthForTest => _bindings.ToolTestFacade.DockedToolCloseButtonWidth;

        public string DockedToolFloatButtonToolTipForTest => _bindings.ToolTestFacade.DockedToolFloatButtonToolTip;

        public string DockedToolCloseButtonToolTipForTest => _bindings.ToolTestFacade.DockedToolCloseButtonToolTip;

        public double DockedToolInspectorWidthForTest => _bindings.ToolTestFacade.DockedToolInspectorWidth;

        public void SetDockedToolInspectorWidthForTest(double width) => _bindings.ToolTestFacade.SetDockedToolInspectorWidth(width);

        public void SetMainLayerImageForTest(Bitmap image) => _bindings.LayerTestFacade.SetMainLayerImage(image);

        public bool LoadMainImageFromFileForTest(string path) => _bindings.LayerTestFacade.LoadMainImageFromFile(path);

        public bool IsShellLogExpandedForTest => _bindings.ShellLogToggle?.IsChecked == true;

        public string ShellLogToggleTextForTest => _bindings.ShellLogToggleText?.Text ?? string.Empty;

        public System.Windows.Point RecipeManagerPanelOffsetForTest =>
            new System.Windows.Point(_bindings.RecipeManagerPanelTransform?.X ?? 0D, _bindings.RecipeManagerPanelTransform?.Y ?? 0D);

        public bool MoveRecipeManagerPanelForTest(double deltaX, double deltaY)
        {
            System.Windows.Point before = RecipeManagerPanelOffsetForTest;
            _bindings.SetRecipeManagerPanelOffset(before.X + deltaX, before.Y + deltaY);
            System.Windows.Point after = RecipeManagerPanelOffsetForTest;
            return Math.Abs(after.X - before.X) > 0.1D || Math.Abs(after.Y - before.Y) > 0.1D;
        }

        public void SetShellLogExpandedForTest(bool expanded)
        {
            if (_bindings.ShellLogToggle != null)
            {
                _bindings.ShellLogToggle.IsChecked = expanded;
            }

            _bindings.SetShellLogExpanded(expanded);
        }

        public bool HasRunnableWorkspaceSampleForTest => _bindings.CommandController.HasRunnableSample();

        public void OpenFirstRunnableWorkspaceSampleForTest() => _bindings.CommandController.OpenFirstRunnableSample();

        public bool OpenWorkspaceSampleForTest(string sampleName) => _bindings.CommandController.OpenRunnableSampleByName(sampleName);

        public bool IsWorkspaceSampleWorkflowVisibleForTest => _bindings.SampleWorkflowPresenter?.IsVisible == true;

        public bool IsWorkspaceMainActionVisibleForTest => _bindings.MainActionPresenter?.IsVisible == true;

        public string WorkspaceMainActionTitleForTest => _bindings.MainActionPresenter?.Title ?? string.Empty;

        public string WorkspaceMainActionDetailForTest => _bindings.MainActionPresenter?.Detail ?? string.Empty;

        public string WorkspaceMainActionMetaForTest => _bindings.MainActionPresenter?.Meta ?? string.Empty;

        public string WorkspaceSampleWorkflowTitleForTest => _bindings.SampleWorkflowPresenter?.Title ?? string.Empty;

        public string WorkspaceSampleWorkflowMetaForTest => _bindings.SampleWorkflowPresenter?.Meta ?? string.Empty;

        public string WorkspaceSampleWorkflowDetailForTest => _bindings.SampleWorkflowPresenter?.Detail ?? string.Empty;

        public bool CanOpenSamplePipelineForTest => _bindings.WorkspaceCommands?.OpenSamplePipelineCommand.CanExecute(null) == true;

        public bool CanOpenSampleFirstStepToolForTest => _bindings.WorkspaceCommands?.OpenSampleFirstStepCommand.CanExecute(null) == true;

        public bool CanOpenSampleCounterpartForTest => _bindings.WorkspaceCommands?.OpenSampleCounterpartCommand.CanExecute(null) == true;

        public bool CanOpenWorkspaceThresholdToolForTest => _bindings.WorkspaceCommands?.OpenThresholdToolCommand.CanExecute(null) == true;

        public bool CanOpenWorkspaceMatchingToolForTest => _bindings.WorkspaceCommands?.OpenMatchingToolCommand.CanExecute(null) == true;

        public bool CanOpenWorkspaceLineToolForTest => _bindings.WorkspaceCommands?.OpenLineToolCommand.CanExecute(null) == true;

        public string WorkspaceSampleFirstStepMenuForTest => _bindings.SampleWorkflowPresenter?.FirstStepMenu?.ToString() ?? string.Empty;

        public void OpenSamplePipelineForTest() => _bindings.WorkspaceCommands?.OpenSamplePipelineCommand.Execute(null);

        public void OpenSampleFirstStepToolForTest() => _bindings.WorkspaceCommands?.OpenSampleFirstStepCommand.Execute(null);

        public void OpenSampleCounterpartForTest() => _bindings.WorkspaceCommands?.OpenSampleCounterpartCommand.Execute(null);

        public void OpenWorkspaceThresholdToolForTest() => _bindings.WorkspaceCommands?.OpenThresholdToolCommand.Execute(null);

        public void OpenWorkspaceMatchingToolForTest() => _bindings.WorkspaceCommands?.OpenMatchingToolCommand.Execute(null);

        public void OpenWorkspaceLineToolForTest() => _bindings.WorkspaceCommands?.OpenLineToolCommand.Execute(null);

        public string ActivePipelineNameForTest =>
            VisionPipelineStorage.LoadActivePipelineName(_bindings.ResolveRecipeName(), VisionPipelineAppendService.DefaultPipelineName);

        public int ActivePipelineStepCountForTest =>
            VisionPipelineStorage.Load(_bindings.ResolveRecipeName(), ActivePipelineNameForTest)?.Steps?.Count ?? 0;

        public string ActiveRecipeContextNameForTest => _bindings.RecipeContextStore.Current.Name;

        public string ActiveRecipeContextPipelineNameForTest => _bindings.RecipeContextStore.Current.PipelineName;

        public string ActiveRecipeContextDisplayTextForTest => _bindings.RecipeContextStore.Current.DisplayText;

        public string ActiveRecipeContextSourcePathForTest => _bindings.RecipeContextStore.Current.SourcePath;

        public string ActiveRecipeContextLayerNameForTest => _bindings.RecipeContextStore.Current.ActiveLayerName;

        public string SelectedLanguageDisplayNameForTest => _bindings.ViewModel.SelectedLanguageOption?.DisplayName ?? string.Empty;

        public void SelectLanguageForTest(OpenVisionLanguage language)
        {
            _bindings.ViewModel.SelectedLanguageOption = _bindings.ViewModel.LanguageOptions.FirstOrDefault(option => option.Language == language);
        }

        public string SelectedRecipeNameForTest => _bindings.RecipeCommands?.SelectedRecipeName ?? string.Empty;

        public IReadOnlyList<string> RecipeOptionsForTest => _bindings.RecipeCommands?.RecipeOptions ?? Array.Empty<string>();

        public string RecipeManagerSelectedPipelineNameForTest =>
            _bindings.RecipeCommands?.SelectedPipelineOption?.PipelineName
            ?? string.Empty;

        public string RecipeManagerSelectedSampleNameForTest =>
            _bindings.RecipeCommands?.SelectedSampleOption?.SampleName
            ?? string.Empty;

        public bool IsRecipeManagerOpenForTest => _bindings.RecipeManagerToggle?.IsChecked == true;

        public bool IsShellBusyOverlayVisibleForTest => _bindings.BusyPresenter.IsVisible;

        public string ShellBusyTitleForTest => _bindings.BusyPresenter.Title;

        public void ShowPipelineLoadingForTest() => _bindings.BusyPresenter.ShowPipelineLoading();

        public void HideShellBusyForTest() => _bindings.BusyPresenter.Hide();

        public void QueuePendingRecipeEditDecisionForTest(
            OpenVisionRecipePendingEditDecision decision)
        {
            _bindings.RecipeDialogAdapter.QueuePendingRecipeEditDecisionForTest(decision);
        }

        public void FailNextRecipeStepEditCommitForTest()
        {
            _bindings.SetFailNextRecipeStepEditCommitForTest(true);
        }

        public void FailNextRecipeStepSaveForTest()
        {
            _bindings.RecipeCommands?.FailNextRecipeStepSaveForTest();
        }

        public void FailNextRecipeStepRoundTripValidationForTest()
        {
            _bindings.RecipeCommands?.FailNextRecipeStepRoundTripValidationForTest();
        }

        public void SetRecipeManagerOpenForTest(bool isOpen)
        {
            if (_bindings.RecipeManagerToggle != null)
            {
                _bindings.RecipeManagerToggle.IsChecked = isOpen;
            }
        }

        public void CreateRecipeForTest() => _bindings.RecipeCommands?.CreateRecipeCommand.Execute(null);

        public void SelectRecipeForTest(string recipeName)
        {
            if (_bindings.RecipeCommands != null)
            {
                _bindings.RecipeCommands.SelectedRecipeName = recipeName;
            }
        }

        public void SwitchRecipeContextForTest(string recipeName)
        {
            _bindings.RuntimeContext.Global.Recipe.Name = recipeName;
            _bindings.RefreshRecipeContext();
        }

        public bool UpdateWorkspacePointerAtCenterForTest() => _bindings.LayerTestFacade.UpdateWorkspacePointerAtCenter();

        public string GetWorkspacePointerCoordinateForTest(double xRatio, double yRatio) =>
            _bindings.LayerTestFacade.GetWorkspacePointerCoordinate(xRatio, yRatio);

        public void ZoomWorkspaceAtForTest(double xRatio, double yRatio, double factor) =>
            _bindings.LayerTestFacade.ZoomWorkspaceAt(xRatio, yRatio, factor);

        public void PanWorkspaceByForTest(double surfaceDeltaX, double surfaceDeltaY) =>
            _bindings.LayerTestFacade.PanWorkspaceBy(surfaceDeltaX, surfaceDeltaY);

        public bool LoadActiveNativePreviewImageFromFileForTest(
            string path,
            VisionToolPreviewImageRole role = VisionToolPreviewImageRole.Input) =>
            _bindings.ToolTestFacade.LoadActiveNativePreviewImageFromFile(path, role);

        public bool SaveActiveNativePreviewImageToFileForTest(
            string path,
            VisionToolPreviewImageRole role = VisionToolPreviewImageRole.Input) =>
            _bindings.ToolTestFacade.SaveActiveNativePreviewImageToFile(path, role);

        public bool ConfigureActiveThresholdBasicInvertForTest(bool invert) =>
            _bindings.ToolTestFacade.ConfigureActiveThresholdBasicInvert(invert);

        public VisionPipelineStep AddActiveNativePipelineStepForTest() => _bindings.ToolTestFacade.AddActiveNativePipelineStep();

        public void SetActiveLineRoiForTest(int x, int y, int width, int height) =>
            _bindings.ToolTestFacade.SetActiveLineRoi(x, y, width, height);

        public void SetActiveLineSettingForTest(string setting) => _bindings.ToolTestFacade.SetActiveLineSetting(setting);

        public void SetActiveSelectedLineRoiForTest(int x, int y, int width, int height) =>
            _bindings.ToolTestFacade.SetActiveSelectedLineRoi(x, y, width, height);

        public void ConfigureActiveSelectedLineForTest(string projectionDirection, string polarity, string verticalDirection = null) =>
            _bindings.ToolTestFacade.ConfigureActiveSelectedLine(projectionDirection, polarity, verticalDirection);

        public void ConfigureActiveSelectedLineDrawForTest(bool showVerticalLine, bool showEdge, bool showContour, bool showFitLine) =>
            _bindings.ToolTestFacade.ConfigureActiveSelectedLineDraw(showVerticalLine, showEdge, showContour, showFitLine);

        public void ConfigureActiveSelectedLineThresholdForTest(double threshold, bool invert) =>
            _bindings.ToolTestFacade.ConfigureActiveSelectedLineThreshold(threshold, invert);

        public void ConfigureActiveSelectedLineMeasureTuningForTest(
            bool useThreshold,
            bool useAdaptiveThreshold,
            double contrast,
            double thickness,
            double samplingStep,
            int pointRange,
            bool useManualAngle,
            double manualAngleValue) =>
            _bindings.ToolTestFacade.ConfigureActiveSelectedLineMeasureTuning(
                useThreshold,
                useAdaptiveThreshold,
                contrast,
                thickness,
                samplingStep,
                pointRange,
                useManualAngle,
                manualAngleValue);

        public void SetActiveLinePurposeForTest(string purpose) => _bindings.ToolTestFacade.SetActiveLinePurpose(purpose);

        public string GetActiveLineSignalInspectorAttributeForTest(string name) =>
            _bindings.ToolTestFacade.GetActiveLineSignalInspectorAttribute(name);

        public bool ExerciseActiveLineSignalInspectorNavigationForTest() =>
            _bindings.ToolTestFacade.ExerciseActiveLineSignalInspectorNavigation();

        public void ExportActiveLineSignalEvidenceForTest(string path) =>
            _bindings.ToolTestFacade.ExportActiveLineSignalEvidence(path);

        public void CloseActiveLineSignalInspectorForTest() =>
            _bindings.ToolTestFacade.CloseActiveLineSignalInspector();

        public void OpenActiveLineSignalInspectorForTest() =>
            _bindings.ToolTestFacade.OpenActiveLineSignalInspector();

        public void SetActiveMatchingTemplatePathForTest(string path) => _bindings.ToolTestFacade.SetActiveMatchingTemplatePath(path);

        public void ConfigureActiveMatchingForTest(Action<MatchingProperty> configure) =>
            _bindings.ToolTestFacade.ConfigureActiveMatching(configure);

        public void ConfigureActiveAffineTransformForTest(Action<AffineTransformProperty> configure) =>
            _bindings.ToolTestFacade.ConfigureActiveAffineTransform(configure);

        public void SetActiveEdgeBasedMatchingTemplatePathForTest(string path) =>
            _bindings.ToolTestFacade.SetActiveEdgeBasedMatchingTemplatePath(path);

        public void ConfigureActiveEdgeBasedMatchingForTest(Action<EdgeBasedMatchingProperty> configure) =>
            _bindings.ToolTestFacade.ConfigureActiveEdgeBasedMatching(configure);

        public void SetActiveAutoMPointRepresentativeImagesForTest(IEnumerable<string> paths) =>
            _bindings.ToolTestFacade.SetActiveAutoMPointRepresentativeImages(paths);

        public void SetActiveFeatureMatchingTemplatePathForTest(string path) =>
            _bindings.ToolTestFacade.SetActiveFeatureMatchingTemplatePath(path);

        public bool RunActiveToolFormForTest() => _bindings.ToolTestFacade.RunActiveToolForm();

        public void SelectPipelineReviewStepForTest(int index, OpenVisionLab.Pipeline.Controls.PipelineFlowPreviewMode mode) =>
            _bindings.ToolTestFacade.SelectPipelineReviewStep(index, mode);

        public Task RunPipelineReviewForTestAsync() => _bindings.ToolTestFacade.RunPipelineReviewAsync();

        public bool OpenPipelineReviewPairSampleForTest() => _bindings.ToolTestFacade.OpenPipelineReviewPairSample();

        public void SelectPipelineReviewObjectResultForTest(int index) =>
            _bindings.ToolTestFacade.SelectPipelineReviewObjectResult(index);

        public void SelectPipelineReviewObjectResultFromImageForTest(int index) =>
            _bindings.ToolTestFacade.SelectPipelineReviewObjectResultFromImage(index);
    }

    internal sealed class OpenVisionShellHostViewTestSurfaceBindings
    {
        internal ApplicationRuntimeContext RuntimeContext { get; set; }
        internal IDisplayManager DisplayManager { get; set; }
        internal OpenVisionShellPreviewViewModel ViewModel { get; set; }
        internal OpenVisionShellHostDocumentController DocumentController { get; set; }
        internal ShellDockedLayerWorkspaceComposition DockedLayerWorkspaceComposition { get; set; }
        internal OpenVisionShellHostLayerManagementController LayerManagementController { get; set; }
        internal ShellDockingTestFacade DockingTestFacade { get; set; }
        internal ShellLayerTestFacade LayerTestFacade { get; set; }
        internal ShellToolTestFacade ToolTestFacade { get; set; }
        internal OpenVisionTcpIntegrationController TcpIntegrationController { get; set; }
        internal OpenVisionShellHostCommandController CommandController { get; set; }
        internal OpenVisionShellHostSampleWorkflowPresenter SampleWorkflowPresenter { get; set; }
        internal OpenVisionShellHostMainActionPresenter MainActionPresenter { get; set; }
        internal OpenVisionShellHostBusyPresenter BusyPresenter { get; set; }
        internal OpenVisionRecipeContextStore RecipeContextStore { get; set; }
        internal RecipeDialogAdapter RecipeDialogAdapter { get; set; }
        internal ToggleButton ShellLogToggle { get; set; }
        internal TextBlock ShellLogToggleText { get; set; }
        internal ToggleButton RecipeManagerToggle { get; set; }
        internal TranslateTransform RecipeManagerPanelTransform { get; set; }
        internal Func<OpenVisionShellHostRecipeCommandSurface> RecipeCommandsProvider { get; set; }
        internal Func<OpenVisionShellHostWorkspaceCommandSurface> WorkspaceCommandsProvider { get; set; }
        internal Func<OpenVisionShellHostLayerCommandSurface> LayerCommandsProvider { get; set; }
        internal Func<System.Windows.Window> GetOwnerWindow { get; set; }
        internal Action<string> ActivateDockedLayer { get; set; }
        internal Action RefreshRecipeContext { get; set; }
        internal Func<string> ResolveRecipeName { get; set; }
        internal Action<double, double> SetRecipeManagerPanelOffset { get; set; }
        internal Action<bool> SetShellLogExpanded { get; set; }
        internal Func<bool> GetFailNextRecipeStepEditCommitForTest { get; set; }
        internal Action<bool> SetFailNextRecipeStepEditCommitForTest { get; set; }

        internal OpenVisionShellHostRecipeCommandSurface RecipeCommands => RecipeCommandsProvider?.Invoke();
        internal OpenVisionShellHostWorkspaceCommandSurface WorkspaceCommands => WorkspaceCommandsProvider?.Invoke();
        internal OpenVisionShellHostLayerCommandSurface LayerCommands => LayerCommandsProvider?.Invoke();
    }
}
