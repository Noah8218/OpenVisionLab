using OpenVisionLab.Vision2D.Pipeline;
using OpenVisionLab.Core;
using OpenVisionLab.Docking.Controls;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using static OpenVisionLab.DEFINE;

namespace OpenVisionLab
{
    public sealed partial class OpenVisionShellHostView
    {
        internal OpenVisionTcpIntegrationController TcpIntegrationControllerForTest => tcpIntegrationController;

        internal void OpenTcpIntegrationForTest() =>
            tcpIntegrationController.Show(System.Windows.Window.GetWindow(this));

        internal Func<string, string, string, bool>
            QualifiedSnapshotLifecycleConfirmationForTest { get; set; }

        internal Func<string, bool>
            QualifiedSnapshotEvidenceOpenerForTest { get; set; }

        public string ActiveToolFormTypeName => toolTestFacade.ActiveToolFormTypeName;
        public string ActiveWpfToolWindowTypeName => toolTestFacade.ActiveWpfToolWindowTypeName;
        public string ActiveWpfToolWindowTitle => toolTestFacade.ActiveWpfToolWindowTitle;
        public string ActiveWpfToolWindowMinimizeToolTip => toolTestFacade.ActiveWpfToolWindowMinimizeToolTip;
        public string ActiveWpfToolWindowMaximizeRestoreToolTip => toolTestFacade.ActiveWpfToolWindowMaximizeRestoreToolTip;
        public string ActiveWpfToolWindowCloseToolTip => toolTestFacade.ActiveWpfToolWindowCloseToolTip;
        public bool IsActiveWpfToolWindowVisibleForTest => toolTestFacade.IsActiveWpfToolWindowVisible;
        public bool IsDockedToolInspectorVisibleForTest => toolTestFacade.IsDockedToolInspectorVisible;

        public bool IsDockedDocumentWorkspaceVisibleForTest => toolTestFacade.IsDockedDocumentWorkspaceVisible;
        public string ActivePendingToolTitle => toolTestFacade.ActivePendingToolTitle;
        public string ActivePendingToolStatusText => toolTestFacade.ActivePendingToolStatusText;
        public string ActiveNativeDocumentTypeName => toolTestFacade.ActiveNativeDocumentTypeName;
        public string ActiveNativeStatusText => toolTestFacade.ActiveNativeStatusText;
        public string ActiveNativeResultReviewText => toolTestFacade.ActiveNativeResultReviewText;
        public string ActiveNativeRouteInputLayerNameForTest => toolTestFacade.ActiveNativeRouteInputLayerName;
        public string ActiveNativeRouteInputLayerBNameForTest => toolTestFacade.ActiveNativeRouteInputLayerBName;
        public string ActiveNativeRouteOutputLayerNameForTest => toolTestFacade.ActiveNativeRouteOutputLayerName;
        public bool IsNativeToolPrewarmCompletedForTest => toolTestFacade.IsNativeToolPrewarmCompleted;
        public int NativeToolPrewarmCreatedCountForTest => toolTestFacade.NativeToolPrewarmCreatedCount;
        public long NativeToolPrewarmElapsedMillisecondsForTest => toolTestFacade.NativeToolPrewarmElapsedMilliseconds;
        public int NativeToolDocumentCacheCountForTest => toolTestFacade.NativeToolDocumentCacheCount;
        public string LastToolOpenTimingTextForTest => toolTestFacade.LastToolOpenTimingText;
        public bool HasPipelineReviewDocumentForTest => toolTestFacade.HasPipelineReviewDocument;
        public bool IsShellLoadedForTest => toolTestFacade.IsShellLoaded;
        public int HostedDocumentCount => toolTestFacade.HostedDocumentCount;
        public bool IsNativeDocumentActive => toolTestFacade.IsNativeDocumentActive;
        public int NativePreviewRunCount => toolTestFacade.NativePreviewRunCount;
        public bool HasNativePreviewResult => toolTestFacade.HasNativePreviewResult;
        public VisionToolRepository VisionToolRepositoryForTest => runtimeContext.Global?.VisionTools;
        public int ActiveLineInputRoiOverlayCount => toolTestFacade.ActiveLineInputRoiOverlayCount;
        public bool ActiveLineSignalInspectorHasEvidenceForTest => toolTestFacade.ActiveLineSignalInspectorHasEvidence;
        public bool ActiveLineSignalInspectorOverlayVisibleForTest => toolTestFacade.ActiveLineSignalInspectorOverlayVisible;
        public bool ActiveLineSignalEvidenceCueVisibleForTest => toolTestFacade.ActiveLineSignalEvidenceCueVisible;
        public string ActiveLineSignalInspectorEvidenceIdForTest => toolTestFacade.ActiveLineSignalInspectorEvidenceId;
        public string ActiveLineSignalInspectorSourceSha256ForTest => toolTestFacade.ActiveLineSignalInspectorSourceSha256;
        public int ActiveLineSignalInspectorSeriesCountForTest => toolTestFacade.ActiveLineSignalInspectorSeriesCount;
        public int ActiveLineSignalInspectorMarkerCountForTest => toolTestFacade.ActiveLineSignalInspectorMarkerCount;
        public int LayerDocumentCount => layerTestFacade.LayerDocumentCount;
        public bool HasMainLayer => layerTestFacade.HasMainLayer;
        public int HostLayerRowCount => layerTestFacade.HostLayerRowCount;
        public string ActiveHostLayerTitle => layerTestFacade.ActiveHostLayerTitle;
        public string SelectedHostLayerTitle => layerTestFacade.SelectedHostLayerTitle;
        public string SelectedHostLayerMeta => layerTestFacade.SelectedHostLayerMeta;
        public bool HasSelectedHostLayerPreview => layerTestFacade.HasSelectedHostLayerPreview;
        public bool HasWorkspaceLayerPreview => layerTestFacade.HasWorkspaceLayerPreview;
        public bool IsSingleWorkspaceVisibleForTest => layerTestFacade.IsSingleWorkspaceVisible;
        public bool IsDockedWorkspaceVisibleForTest => dockingTestFacade.IsDockedWorkspaceVisible;
        public bool IsWorkspaceLayerDropEnabledForTest => layerTestFacade.IsWorkspaceLayerDropEnabled;
        public bool HasWorkspaceDropOverlayForTest => layerTestFacade.HasWorkspaceDropOverlay;
        public bool IsWorkspaceDropOverlayVisibleForTest => layerTestFacade.IsWorkspaceDropOverlayVisible;
        public bool IsWorkspaceDropOverlayHitTestSafeForTest => layerTestFacade.IsWorkspaceDropOverlayHitTestSafe;
        public bool HasDockingGuideOverlayForTest => dockingTestFacade.HasGuideOverlay;
        public bool IsDockingGuideOverlayVisibleForTest => dockingTestFacade.IsGuideOverlayVisible;
        public string ActiveDockingGuideZoneForTest => dockingTestFacade.ActiveGuideZone;
        public bool IsDockingGuideOverlayHitTestSafeForTest => dockingTestFacade.IsGuideOverlayHitTestSafe;
        public int DockingGuideZoneCountForTest => dockingTestFacade.GuideZoneCount;
        public int WorkspaceTextureTileCount => layerTestFacade.WorkspaceTextureTileCount;
        public bool IsWorkspaceEmptyPromptVisible => layerTestFacade.IsWorkspaceEmptyPromptVisible;
        public string WorkspaceCoordinatesTextForTest => layerTestFacade.WorkspaceCoordinatesText;
        public string WorkspacePixelTextForTest => layerTestFacade.WorkspacePixelText;
        public string WorkspaceEmptyTitle => layerTestFacade.WorkspaceEmptyTitle;
        public string WorkspaceEmptyDetail => layerTestFacade.WorkspaceEmptyDetail;
        public string WorkspaceLayerTitle => layerTestFacade.WorkspaceLayerTitle;
        public string WorkspaceLayerMeta => layerTestFacade.WorkspaceLayerMeta;
        public string WorkspaceLoadImageMenuText => layerTestFacade.WorkspaceLoadImageMenuText;
        public string WorkspaceLoadImageButtonText => layerTestFacade.WorkspaceLoadImageButtonText;
        public bool HasWorkspaceLoadImageMenu => layerTestFacade.HasWorkspaceLoadImageMenu;
        public bool IsWorkspaceLoadImageIntoLayerMenuVisibleForTest => layerTestFacade.IsWorkspaceLoadImageIntoLayerMenuVisible;
        public int OpenLayerViewerWindowCount => layerTestFacade.OpenLayerViewerWindowCount;
        public string OpenLayerViewerWindowTitles => layerTestFacade.OpenLayerViewerWindowTitles;
        public int DockedLayerCount => dockingTestFacade.LayerCount;
        public int DockedLayerTextureTileCount => dockingTestFacade.TextureTileCount;
        public int DockedLayerPaneCount => dockingTestFacade.PaneCount;
        public string DockedLayerRootOrientationForTest => dockingTestFacade.RootOrientationName;
        public int DockedLayerNestedLayoutPanelCountForTest => dockingTestFacade.NestedLayoutPanelCount;
        public bool AreDockedLayerViewersCompactSizeReadyForTest => dockingTestFacade.AreViewersCompactSizeReady;
        public bool IsToolRailCompactForTest => toolTestFacade.IsToolRailCompact;
        public double ToolRailWidthForTest => toolTestFacade.ToolRailWidth;
        public bool IsToolRailNavigationVisibleForTest => toolTestFacade.IsToolRailNavigationVisible;
        public bool IsToolRailCompactLabelHiddenForTest => toolTestFacade.IsToolRailCompactLabelHidden;
        public string ToolSearchTextForTest => viewModel.ToolSearchText;
        public int VisibleToolSearchItemCountForTest => viewModel.VisibleToolCount;
        public string VisibleToolSearchCommandIdsForTest => viewModel.VisibleToolCommandIds;
        public bool AreDockedLayersNativeFloatingDisabledForTest => dockingTestFacade.AreNativeFloatingDisabled;
        public bool AreDockedLayerViewersCompactForTest => dockingTestFacade.AreViewersCompact;
        public int DockedLayerTabHeaderCount => dockingTestFacade.TabHeaderCount;
        public bool AreDockedLayerTabHeadersGestureReadyForTest => dockingTestFacade.AreTabHeadersGestureReady;
        public bool AreDockedLayerTabHeadersReadableForTest => dockingTestFacade.AreTabHeadersReadable;
        public bool AreDockedLayerTabHeaderGripsReadyForTest => dockingTestFacade.AreTabHeaderGripsReady;
        public bool AreDockedLayersNativeFloatingEnabledForTest => dockingTestFacade.AreNativeFloatingEnabled;
        public string DockedLayerTabHeaderDiagnosticsForTest => dockingTestFacade.TabHeaderDiagnostics;
        public string DockedLayerTitles => dockingTestFacade.Titles;
        public OpenVisionDockingVisualSnapshot DockedLayerVisualSnapshotForTest => dockingTestFacade.CreateDockingVisualSnapshot();
        public string DirectResultBadgeText => toolTestFacade.DirectResultBadgeText;
        public string DirectResultTitleText => toolTestFacade.DirectResultTitleText;
        public string DirectResultStatusText => toolTestFacade.DirectResultStatusText;
        public string DirectResultRouteText => toolTestFacade.DirectResultRouteText;
        public string ActiveNativeRecipeContextNameForTest => documentController.ActiveNativeDocument?.RecipeContextName ?? string.Empty;
        public string ActiveNativeRecipeContextPipelineNameForTest => documentController.ActiveNativeDocument?.RecipeContextPipelineName ?? string.Empty;
        public int PipelineReviewStepCount => toolTestFacade.PipelineReviewStepCount;
        public string PipelineReviewRecipeContextNameForTest => toolTestFacade.PipelineReviewRecipeContextName;
        public string PipelineReviewRecipeContextPipelineNameForTest => toolTestFacade.PipelineReviewRecipeContextPipelineName;
        public string PipelineReviewSelectedStepName => toolTestFacade.PipelineReviewSelectedStepName;
        public string PipelineReviewSelectedStatusText => toolTestFacade.PipelineReviewSelectedStatusText;
        public string PipelineReviewFlowSummaryText => toolTestFacade.PipelineReviewFlowSummaryText;
        public string PipelineReviewParameterSummaryText => toolTestFacade.PipelineReviewParameterSummaryText;
        public string PipelineReviewValidationStatusText => toolTestFacade.PipelineReviewValidationStatusText;
        public string PipelineReviewValidationDetailText => toolTestFacade.PipelineReviewValidationDetailText;
        public string PipelineReviewResultSummaryText => toolTestFacade.PipelineReviewResultSummaryText;
        public string PipelineReviewResultDetailText => toolTestFacade.PipelineReviewResultDetailText;
        public string PipelineReviewRunLogText => toolTestFacade.PipelineReviewRunLogText;
        public string PipelineReviewExecutionState => toolTestFacade.PipelineReviewExecutionState;
        public string PipelineReviewProgressText => toolTestFacade.PipelineReviewProgressText;
        public string PipelineReviewGuideStageText => toolTestFacade.PipelineReviewGuideStageText;
        public string PipelineReviewGuideCurrentStepText => toolTestFacade.PipelineReviewGuideCurrentStepText;
        public string PipelineReviewGuideNextActionText => toolTestFacade.PipelineReviewGuideNextActionText;
        public string PipelineReviewGuideResultDecisionText => toolTestFacade.PipelineReviewGuideResultDecisionText;
        public string PipelineReviewGuideDetailText => toolTestFacade.PipelineReviewGuideDetailText;
        public string PipelineReviewGuidePairText => toolTestFacade.PipelineReviewGuidePairText;
        public string PipelineReviewGuidePairActionText => toolTestFacade.PipelineReviewGuidePairActionText;
        public string PipelineReviewGuidePairMetricText => toolTestFacade.PipelineReviewGuidePairMetricText;
        public string PipelineReviewGuideChecklistText => toolTestFacade.PipelineReviewGuideChecklistText;
        public string PipelineReviewGuideParameterFocusText => toolTestFacade.PipelineReviewGuideParameterFocusText;
        public string PipelineReviewGuideTriageFailureText => toolTestFacade.PipelineReviewGuideTriageFailureText;
        public string PipelineReviewGuideTriageAdjustmentText => toolTestFacade.PipelineReviewGuideTriageAdjustmentText;
        public string PipelineReviewGuideTriageRerunText => toolTestFacade.PipelineReviewGuideTriageRerunText;
        public bool CanOpenPipelineReviewPairSampleForTest => toolTestFacade.CanOpenPipelineReviewPairSample;
        public bool CanSelectPreviousPipelineReviewStepForTest => toolTestFacade.CanSelectPreviousPipelineReviewStep;
        public bool CanSelectNextPipelineReviewStepForTest => toolTestFacade.CanSelectNextPipelineReviewStep;
        public bool CanSelectFirstIssuePipelineReviewStepForTest => toolTestFacade.CanSelectFirstIssuePipelineReviewStep;
        public bool HasPipelineReviewInputPreview => toolTestFacade.HasPipelineReviewInputPreview;
        public bool HasPipelineReviewOutputPreview => toolTestFacade.HasPipelineReviewOutputPreview;
        public int PipelineReviewObjectResultCountForTest => toolTestFacade.PipelineReviewObjectResultCount;
        public int PipelineReviewObjectMetricDistributionSeriesCountForTest => toolTestFacade.PipelineReviewObjectMetricDistributionSeriesCount;
        public int PipelineReviewObjectMetricDistributionMarkerCountForTest => toolTestFacade.PipelineReviewObjectMetricDistributionMarkerCount;
        public string PipelineReviewObjectMetricDistributionMetricForTest => toolTestFacade.PipelineReviewObjectMetricDistributionMetric;
        public string PipelineReviewObjectMetricDistributionEvidenceIdForTest => toolTestFacade.PipelineReviewObjectMetricDistributionEvidenceId;
        public bool PipelineReviewMatcherDiagnosticTabVisibleForTest => toolTestFacade.PipelineReviewMatcherDiagnosticTabVisible;
        public string PipelineReviewMatcherDiagnosticStateForTest => toolTestFacade.PipelineReviewMatcherDiagnosticState;
        public string PipelineReviewMatcherDiagnosticEvidenceIdForTest => toolTestFacade.PipelineReviewMatcherDiagnosticEvidenceId;
        public int PipelineReviewMatcherDiagnosticRowCountForTest => toolTestFacade.PipelineReviewMatcherDiagnosticRowCount;
        public int PipelineReviewMatcherDiagnosticModelPointCountForTest => toolTestFacade.PipelineReviewMatcherDiagnosticModelPointCount;
        public bool PipelineReviewMatcherDiagnosticHasSelectedCandidateForTest => toolTestFacade.PipelineReviewMatcherDiagnosticHasSelectedCandidate;
        public bool PipelineReviewMatcherDiagnosticHasAlternativeForTest => toolTestFacade.PipelineReviewMatcherDiagnosticHasAlternative;
        public bool IsPipelineReviewFixtureDesignerVisibleForTest => toolTestFacade.IsPipelineReviewFixtureDesignerVisible;
        public string PipelineReviewFixtureRelationshipTextForTest => toolTestFacade.PipelineReviewFixtureRelationshipText;
        public int PipelineReviewFixtureProducerStepNumberForTest => toolTestFacade.PipelineReviewFixtureProducerStepNumber;
        public int PipelineReviewFixtureMeasurementStepNumberForTest => toolTestFacade.PipelineReviewFixtureMeasurementStepNumber;
        public int PipelineReviewSelectedObjectResultNumberForTest => toolTestFacade.PipelineReviewSelectedObjectResultNumber;
        public bool HasPipelineReviewObjectHighlightForTest => toolTestFacade.HasPipelineReviewObjectHighlight;

        public void SelectToolForTest(VISION_MENU menu) => toolTestFacade.SelectTool(menu);

        public void ToggleToolRailForTest() => toolTestFacade.ToggleToolRail();

        public void SetToolSearchTextForTest(string text) => viewModel.ToolSearchText = text;

        public void ClearToolSearchForTest() => viewModel.ClearToolSearchCommand.Execute(null);

        public bool HasNativeToolDocumentCachedForTest(VISION_MENU menu) => toolTestFacade.HasNativeToolDocumentCached(menu);

        public void RunActiveNativePreviewForTest() => toolTestFacade.RunActiveNativePreview();

        public void CreateActiveNativeOutputLayerForTest() => toolTestFacade.CreateActiveNativeOutputLayer();

        public bool OpenLayerViewerForTest(string layerTitle) => layerTestFacade.OpenLayerViewer(layerTitle);

        public bool HasLayerForTest(string layerTitle) => layerTestFacade.HasLayer(layerTitle);

        public string HostLayerTabTextsForTest => layerTestFacade.HostLayerTabTexts;

        public bool AreHostLayerTabsReadableForTest => layerTestFacade.AreHostLayerTabsReadable;

        public Bitmap GetLayerImageCloneForTest(string layerTitle) => layerTestFacade.GetLayerImageClone(layerTitle);

        public bool ActivateHostLayerForTest(string layerTitle) => layerTestFacade.ActivateHostLayer(layerTitle);

        public bool SelectHostLayerRowForTest(string layerTitle) => layerTestFacade.SelectHostLayerRow(layerTitle);

        public bool RightClickHostLayerRowForTest(string layerTitle) => layerTestFacade.RightClickHostLayerRow(layerTitle);

        public bool DockLayerForTest(string layerTitle) => dockingTestFacade.DockLayerDocument(layerTitle);

        public bool ActivateDockedLayerForTest(string layerTitle)
        {
            if (!dockedLayerWorkspaceComposition.Commands.ActivateLayerDocument(layerTitle))
            {
                return false;
            }

            ActivateDockedLayer(layerTitle);
            return true;
        }

        public bool AddLayerImageForTest(string layerTitle, Bitmap image) => layerTestFacade.AddLayerImage(layerTitle, image);

        public string CreateLayerForTest() => layerManagementController.CreateLayer();

        public bool LoadImageIntoLayerForTest(string layerTitle, string path) =>
            layerManagementController.LoadImageIntoLayer(layerTitle, path);

        public bool SetLayerImageForTest(string layerTitle, Bitmap image) =>
            layerManagementController.SetLayerImage(layerTitle, image);

        public bool RenameLayerForTest(string oldLayerTitle, string newLayerTitle) =>
            layerManagementController.RenameLayer(oldLayerTitle, newLayerTitle);

        public bool DeleteLayerForTest(string layerTitle) => layerManagementController.DeleteLayer(layerTitle);

        public void ClearLayerImageHistoryForTest() => (displayManager as DisplayManagerService)?.ClearLayerImageHistory();

        public bool SplitDockedLayerForTest(string layerTitle) => dockingTestFacade.SplitLayerToNewPane(layerTitle);

        public bool ArrangeDockedLayerPanesForTest(string orientationName, params string[] layerTitles) =>
            dockingTestFacade.ArrangeLayerPanes(orientationName, layerTitles);

        public bool ArrangeDockedLayerGridForTest(params string[] layerTitles) => dockingTestFacade.ArrangeLayerGrid(layerTitles);

        public bool MoveDockedLayerToPrimaryPaneForTest(string layerTitle) => dockingTestFacade.MoveLayerToPrimaryPane(layerTitle);

        public bool DockLayerToGuideZoneForTest(string layerTitle, string zoneName) => dockingTestFacade.MoveLayerToGuideZone(layerTitle, zoneName);

        public void ClearDockedLayersForTest() => dockingTestFacade.ClearDockedLayerDocuments();

        public void ShowDockingGuideForTest(double xRatio = 0.5D, double yRatio = 0.5D) =>
            dockingTestFacade.ShowDockingGuide(xRatio, yRatio);

        public System.Windows.Point GetDockedWorkspaceScreenPointForTest(double x, double y) =>
            dockingTestFacade.GetWorkspaceScreenPoint(x, y);

        public bool ShowDockedLayerTabDragGuideForTest() => dockingTestFacade.ShowFirstDockedLayerTabDragGuide();

        public void HideDockingGuideForTest() => dockingTestFacade.HideDockingGuide();

        public void SaveDockingWorkspaceStateForTest() => dockingTestFacade.SaveLayerWorkspaceState();

        public bool RestoreDockingLayoutStateForTest() => dockingTestFacade.RestoreLayerWorkspaceState();

        public bool SaveWorkspaceImageToFileForTest(string path) => layerTestFacade.SaveWorkspaceImageToFile(path);

        public bool SaveDockedLayerImageToFileForTest(string layerTitle, string path) =>
            dockingTestFacade.SaveDockedLayerImageToFile(layerTitle, path);

        public Bitmap CloneDockedLayerImageForTest(string layerTitle) =>
            dockingTestFacade.CloneDockedLayerImage(layerTitle);

        public int GetDockedLayerImagePixelWidthForTest(string layerTitle) =>
            dockingTestFacade.GetLayerImagePixelWidth(layerTitle);

        public int GetDockedLayerImagePixelHeightForTest(string layerTitle) =>
            dockingTestFacade.GetLayerImagePixelHeight(layerTitle);

        public int GetDockedLayerTextureTileCountForTest(string layerTitle) =>
            dockingTestFacade.GetLayerTextureTileCount(layerTitle);

        public int LiveLayerViewerInstanceCountForTest => OpenVisionLayerViewerView.LiveInstanceCountForTest;

        public string LiveLayerViewerInstanceStatesForTest => OpenVisionLayerViewerView.LiveInstanceStatesForTest;

        public bool CloseActiveWpfToolWindowForTest() => toolTestFacade.CloseActiveWpfToolWindow();

        public bool DockActiveWpfToolWindowForTest() => toolTestFacade.DockActiveWpfToolWindow();

        public bool FloatDockedWpfToolWindowForTest() => toolTestFacade.FloatDockedWpfToolWindow();

        public string DockedToolTitleForTest => toolTestFacade.DockedToolTitle;

        public bool IsDockedToolFloatButtonVisibleForTest => toolTestFacade.IsDockedToolFloatButtonVisible;

        public bool IsDockedToolCloseButtonVisibleForTest => toolTestFacade.IsDockedToolCloseButtonVisible;

        public double DockedToolFloatButtonWidthForTest => toolTestFacade.DockedToolFloatButtonWidth;

        public double DockedToolCloseButtonWidthForTest => toolTestFacade.DockedToolCloseButtonWidth;

        public string DockedToolFloatButtonToolTipForTest => toolTestFacade.DockedToolFloatButtonToolTip;

        public string DockedToolCloseButtonToolTipForTest => toolTestFacade.DockedToolCloseButtonToolTip;

        public double DockedToolInspectorWidthForTest => toolTestFacade.DockedToolInspectorWidth;

        public void SetDockedToolInspectorWidthForTest(double width) => toolTestFacade.SetDockedToolInspectorWidth(width);

        public void SetMainLayerImageForTest(Bitmap image) => layerTestFacade.SetMainLayerImage(image);

        public bool LoadMainImageFromFileForTest(string path) => layerTestFacade.LoadMainImageFromFile(path);

        public bool IsShellLogExpandedForTest => btnShellLogToggle?.IsChecked == true;

        public string ShellLogToggleTextForTest => txtShellLogToggle?.Text ?? string.Empty;

        public System.Windows.Point RecipeManagerPanelOffsetForTest =>
            new System.Windows.Point(recipeManagerPanelTransform?.X ?? 0D, recipeManagerPanelTransform?.Y ?? 0D);

        public bool MoveRecipeManagerPanelForTest(double deltaX, double deltaY)
        {
            System.Windows.Point before = RecipeManagerPanelOffsetForTest;
            SetRecipeManagerPanelOffset(before.X + deltaX, before.Y + deltaY);
            System.Windows.Point after = RecipeManagerPanelOffsetForTest;
            return Math.Abs(after.X - before.X) > 0.1D || Math.Abs(after.Y - before.Y) > 0.1D;
        }

        public void SetShellLogExpandedForTest(bool expanded)
        {
            if (btnShellLogToggle != null)
            {
                btnShellLogToggle.IsChecked = expanded;
            }

            SetShellLogExpanded(expanded);
        }

        public bool HasRunnableWorkspaceSampleForTest => commandController.HasRunnableSample();

        public void OpenFirstRunnableWorkspaceSampleForTest() => commandController.OpenFirstRunnableSample();

        public bool OpenWorkspaceSampleForTest(string sampleName) => commandController.OpenRunnableSampleByName(sampleName);

        public bool IsWorkspaceSampleWorkflowVisibleForTest => sampleWorkflowPresenter?.IsVisible == true;

        public bool IsWorkspaceMainActionVisibleForTest => mainActionPresenter?.IsVisible == true;

        public string WorkspaceMainActionTitleForTest => mainActionPresenter?.Title ?? string.Empty;

        public string WorkspaceMainActionDetailForTest => mainActionPresenter?.Detail ?? string.Empty;

        public string WorkspaceMainActionMetaForTest => mainActionPresenter?.Meta ?? string.Empty;

        public string WorkspaceSampleWorkflowTitleForTest => sampleWorkflowPresenter?.Title ?? string.Empty;

        public string WorkspaceSampleWorkflowMetaForTest => sampleWorkflowPresenter?.Meta ?? string.Empty;

        public string WorkspaceSampleWorkflowDetailForTest => sampleWorkflowPresenter?.Detail ?? string.Empty;

        public bool CanOpenSamplePipelineForTest => WorkspaceCommands?.OpenSamplePipelineCommand.CanExecute(null) == true;

        public bool CanOpenSampleFirstStepToolForTest => WorkspaceCommands?.OpenSampleFirstStepCommand.CanExecute(null) == true;

        public bool CanOpenSampleCounterpartForTest => WorkspaceCommands?.OpenSampleCounterpartCommand.CanExecute(null) == true;

        public bool CanOpenWorkspaceThresholdToolForTest => WorkspaceCommands?.OpenThresholdToolCommand.CanExecute(null) == true;

        public bool CanOpenWorkspaceMatchingToolForTest => WorkspaceCommands?.OpenMatchingToolCommand.CanExecute(null) == true;

        public bool CanOpenWorkspaceLineToolForTest => WorkspaceCommands?.OpenLineToolCommand.CanExecute(null) == true;

        public string WorkspaceSampleFirstStepMenuForTest => sampleWorkflowPresenter?.FirstStepMenu?.ToString() ?? string.Empty;

        public void OpenSamplePipelineForTest() => WorkspaceCommands?.OpenSamplePipelineCommand.Execute(null);

        public void OpenSampleFirstStepToolForTest() => WorkspaceCommands?.OpenSampleFirstStepCommand.Execute(null);

        public void OpenSampleCounterpartForTest() => WorkspaceCommands?.OpenSampleCounterpartCommand.Execute(null);

        public void OpenWorkspaceThresholdToolForTest() => WorkspaceCommands?.OpenThresholdToolCommand.Execute(null);

        public void OpenWorkspaceMatchingToolForTest() => WorkspaceCommands?.OpenMatchingToolCommand.Execute(null);

        public void OpenWorkspaceLineToolForTest() => WorkspaceCommands?.OpenLineToolCommand.Execute(null);

        public string ActivePipelineNameForTest =>
            VisionPipelineStorage.LoadActivePipelineName(ResolveRecipeName(), VisionPipelineAppendService.DefaultPipelineName);

        public int ActivePipelineStepCountForTest =>
            VisionPipelineStorage.Load(ResolveRecipeName(), ActivePipelineNameForTest)?.Steps?.Count ?? 0;

        public string ActiveRecipeContextNameForTest => recipeContextStore.Current.Name;

        public string ActiveRecipeContextPipelineNameForTest => recipeContextStore.Current.PipelineName;

        public string ActiveRecipeContextDisplayTextForTest => recipeContextStore.Current.DisplayText;

        public string ActiveRecipeContextSourcePathForTest => recipeContextStore.Current.SourcePath;

        public string ActiveRecipeContextLayerNameForTest => recipeContextStore.Current.ActiveLayerName;

        public string SelectedLanguageDisplayNameForTest => viewModel.SelectedLanguageOption?.DisplayName ?? string.Empty;

        public void SelectLanguageForTest(OpenVisionLanguage language)
        {
            viewModel.SelectedLanguageOption = viewModel.LanguageOptions.FirstOrDefault(option => option.Language == language);
        }

        public string SelectedRecipeNameForTest => RecipeCommands?.SelectedRecipeName ?? string.Empty;

        public IReadOnlyList<string> RecipeOptionsForTest => RecipeCommands?.RecipeOptions ?? Array.Empty<string>();

        public string RecipeManagerSelectedPipelineNameForTest =>
            RecipeCommands?.SelectedPipelineOption?.PipelineName
            ?? string.Empty;

        public string RecipeManagerSelectedSampleNameForTest =>
            RecipeCommands?.SelectedSampleOption?.SampleName
            ?? string.Empty;

        public bool IsRecipeManagerOpenForTest => btnHostRecipeManager?.IsChecked == true;

        public bool IsShellBusyOverlayVisibleForTest => busyPresenter.IsVisible;

        public string ShellBusyTitleForTest => busyPresenter.Title;

        public void ShowPipelineLoadingForTest() => busyPresenter.ShowPipelineLoading();

        public void HideShellBusyForTest() => busyPresenter.Hide();

        public void QueuePendingRecipeEditDecisionForTest(
            OpenVisionRecipePendingEditDecision decision)
        {
            pendingRecipeEditDecisionsForTest.Enqueue(decision);
        }

        public void FailNextRecipeStepEditCommitForTest()
        {
            failNextRecipeStepEditCommitForTest = true;
        }

        public void FailNextRecipeStepSaveForTest()
        {
            failNextRecipeStepSaveForTest = true;
        }

        public void FailNextRecipeStepRoundTripValidationForTest()
        {
            failNextRecipeStepRoundTripValidationForTest = true;
        }

        public void SetRecipeManagerOpenForTest(bool isOpen)
        {
            if (btnHostRecipeManager != null)
            {
                btnHostRecipeManager.IsChecked = isOpen;
            }
        }

        public void CreateRecipeForTest() => RecipeCommands?.CreateRecipeCommand.Execute(null);

        public void SelectRecipeForTest(string recipeName)
        {
            if (RecipeCommands != null)
            {
                RecipeCommands.SelectedRecipeName = recipeName;
            }
        }

        public void SwitchRecipeContextForTest(string recipeName)
        {
            runtimeContext.Global.Recipe.Name = recipeName;
            RefreshRecipeContext();
        }

        public bool UpdateWorkspacePointerAtCenterForTest() => layerTestFacade.UpdateWorkspacePointerAtCenter();

        public string GetWorkspacePointerCoordinateForTest(double xRatio, double yRatio) =>
            layerTestFacade.GetWorkspacePointerCoordinate(xRatio, yRatio);

        public void ZoomWorkspaceAtForTest(double xRatio, double yRatio, double factor) =>
            layerTestFacade.ZoomWorkspaceAt(xRatio, yRatio, factor);

        public void PanWorkspaceByForTest(double surfaceDeltaX, double surfaceDeltaY) =>
            layerTestFacade.PanWorkspaceBy(surfaceDeltaX, surfaceDeltaY);

        public bool LoadActiveNativePreviewImageFromFileForTest(
            string path,
            VisionToolPreviewImageRole role = VisionToolPreviewImageRole.Input) =>
            toolTestFacade.LoadActiveNativePreviewImageFromFile(path, role);

        public bool SaveActiveNativePreviewImageToFileForTest(
            string path,
            VisionToolPreviewImageRole role = VisionToolPreviewImageRole.Input) =>
            toolTestFacade.SaveActiveNativePreviewImageToFile(path, role);

        public bool ConfigureActiveThresholdBasicInvertForTest(bool invert) =>
            toolTestFacade.ConfigureActiveThresholdBasicInvert(invert);

        public VisionPipelineStep AddActiveNativePipelineStepForTest() => toolTestFacade.AddActiveNativePipelineStep();

        public void SetActiveLineRoiForTest(int x, int y, int width, int height) =>
            toolTestFacade.SetActiveLineRoi(x, y, width, height);

        public void SetActiveLineSettingForTest(string setting) => toolTestFacade.SetActiveLineSetting(setting);

        public void SetActiveSelectedLineRoiForTest(int x, int y, int width, int height) =>
            toolTestFacade.SetActiveSelectedLineRoi(x, y, width, height);

        public void ConfigureActiveSelectedLineForTest(string projectionDirection, string polarity, string verticalDirection = null) =>
            toolTestFacade.ConfigureActiveSelectedLine(projectionDirection, polarity, verticalDirection);

        public void ConfigureActiveSelectedLineDrawForTest(bool showVerticalLine, bool showEdge, bool showContour, bool showFitLine) =>
            toolTestFacade.ConfigureActiveSelectedLineDraw(showVerticalLine, showEdge, showContour, showFitLine);

        public void ConfigureActiveSelectedLineThresholdForTest(double threshold, bool invert) =>
            toolTestFacade.ConfigureActiveSelectedLineThreshold(threshold, invert);

        public void ConfigureActiveSelectedLineMeasureTuningForTest(
            bool useThreshold,
            bool useAdaptiveThreshold,
            double contrast,
            double thickness,
            double samplingStep,
            int pointRange,
            bool useManualAngle,
            double manualAngleValue) =>
            toolTestFacade.ConfigureActiveSelectedLineMeasureTuning(
                useThreshold,
                useAdaptiveThreshold,
                contrast,
                thickness,
                samplingStep,
                pointRange,
                useManualAngle,
                manualAngleValue);

        public void SetActiveLinePurposeForTest(string purpose) => toolTestFacade.SetActiveLinePurpose(purpose);

        public string GetActiveLineSignalInspectorAttributeForTest(string name) =>
            toolTestFacade.GetActiveLineSignalInspectorAttribute(name);

        public bool ExerciseActiveLineSignalInspectorNavigationForTest() =>
            toolTestFacade.ExerciseActiveLineSignalInspectorNavigation();

        public void ExportActiveLineSignalEvidenceForTest(string path) =>
            toolTestFacade.ExportActiveLineSignalEvidence(path);

        public void CloseActiveLineSignalInspectorForTest() =>
            toolTestFacade.CloseActiveLineSignalInspector();

        public void OpenActiveLineSignalInspectorForTest() =>
            toolTestFacade.OpenActiveLineSignalInspector();

        public void SetActiveMatchingTemplatePathForTest(string path) => toolTestFacade.SetActiveMatchingTemplatePath(path);

        public void ConfigureActiveMatchingForTest(Action<MatchingProperty> configure) =>
            toolTestFacade.ConfigureActiveMatching(configure);

        public void ConfigureActiveAffineTransformForTest(Action<AffineTransformProperty> configure) =>
            toolTestFacade.ConfigureActiveAffineTransform(configure);

        public void SetActiveEdgeBasedMatchingTemplatePathForTest(string path) =>
            toolTestFacade.SetActiveEdgeBasedMatchingTemplatePath(path);

        public void ConfigureActiveEdgeBasedMatchingForTest(Action<EdgeBasedMatchingProperty> configure) =>
            toolTestFacade.ConfigureActiveEdgeBasedMatching(configure);

        public void SetActiveAutoMPointRepresentativeImagesForTest(IEnumerable<string> paths) =>
            toolTestFacade.SetActiveAutoMPointRepresentativeImages(paths);

        public void SetActiveFeatureMatchingTemplatePathForTest(string path) =>
            toolTestFacade.SetActiveFeatureMatchingTemplatePath(path);

        public bool RunActiveToolFormForTest() => toolTestFacade.RunActiveToolForm();

        public void SelectPipelineReviewStepForTest(int index, OpenVisionLab.Pipeline.Controls.PipelineFlowPreviewMode mode) =>
            toolTestFacade.SelectPipelineReviewStep(index, mode);

        public Task RunPipelineReviewForTestAsync() => toolTestFacade.RunPipelineReviewAsync();

        public bool OpenPipelineReviewPairSampleForTest() => toolTestFacade.OpenPipelineReviewPairSample();

        public void SelectPipelineReviewObjectResultForTest(int index) =>
            toolTestFacade.SelectPipelineReviewObjectResult(index);

        public void SelectPipelineReviewObjectResultFromImageForTest(int index) =>
            toolTestFacade.SelectPipelineReviewObjectResultFromImage(index);
    }
}
