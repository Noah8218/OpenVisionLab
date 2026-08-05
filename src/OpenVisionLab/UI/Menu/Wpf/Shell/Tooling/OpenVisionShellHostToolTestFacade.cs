using OpenVisionLab.Vision2D.Pipeline;
using OpenVisionLab.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static OpenVisionLab.DEFINE;

namespace OpenVisionLab
{
    internal sealed class OpenVisionShellHostToolTestFacade
    {
        private readonly OpenVisionShellPreviewViewModel viewModel;
        private readonly OpenVisionShellHostStatePresenter statePresenter;
        private readonly OpenVisionShellHostDocumentController documentController;
        private readonly OpenVisionShellHostToolWindowController toolWindowController;
        private readonly OpenVisionShellHostToolWindowLifecycleController toolWindowLifecycleController;
        private readonly OpenVisionFloatingToolWindowHost floatingToolWindowHost;
        private readonly OpenVisionDockedToolInspectorController dockedToolInspectorController;
        private readonly OpenVisionShellHostChromeController chromeController;
        private readonly OpenVisionShellHostRefreshCoordinator refreshCoordinator;
        private readonly OpenVisionShellHostToolTestFacadeBindings bindings;

        public OpenVisionShellHostToolTestFacade(
            OpenVisionShellPreviewViewModel viewModel,
            OpenVisionShellHostStatePresenter statePresenter,
            OpenVisionShellHostDocumentController documentController,
            OpenVisionShellHostToolWindowController toolWindowController,
            OpenVisionShellHostToolWindowLifecycleController toolWindowLifecycleController,
            OpenVisionFloatingToolWindowHost floatingToolWindowHost,
            OpenVisionDockedToolInspectorController dockedToolInspectorController,
            OpenVisionShellHostChromeController chromeController,
            OpenVisionShellHostRefreshCoordinator refreshCoordinator,
            OpenVisionShellHostToolTestFacadeBindings bindings)
        {
            this.viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
            this.statePresenter = statePresenter ?? throw new ArgumentNullException(nameof(statePresenter));
            this.documentController = documentController ?? throw new ArgumentNullException(nameof(documentController));
            this.toolWindowController = toolWindowController ?? throw new ArgumentNullException(nameof(toolWindowController));
            this.toolWindowLifecycleController = toolWindowLifecycleController ?? throw new ArgumentNullException(nameof(toolWindowLifecycleController));
            this.floatingToolWindowHost = floatingToolWindowHost ?? throw new ArgumentNullException(nameof(floatingToolWindowHost));
            this.dockedToolInspectorController = dockedToolInspectorController ?? throw new ArgumentNullException(nameof(dockedToolInspectorController));
            this.chromeController = chromeController ?? throw new ArgumentNullException(nameof(chromeController));
            this.refreshCoordinator = refreshCoordinator ?? throw new ArgumentNullException(nameof(refreshCoordinator));
            this.bindings = bindings ?? throw new ArgumentNullException(nameof(bindings));
        }

        public string ActiveToolFormTypeName => statePresenter.ActiveToolFormTypeName;

        public string ActiveWpfToolWindowTypeName => IsDockedToolInspectorVisible
            ? dockedToolInspectorController.ActiveContent?.GetType().Name ?? string.Empty
            : statePresenter.ActiveWpfToolWindowTypeName;

        public string ActiveWpfToolWindowTitle => IsDockedToolInspectorVisible
            ? dockedToolInspectorController.ActiveTitle
            : statePresenter.ActiveWpfToolWindowTitle;

        public string ActiveWpfToolWindowMinimizeToolTip => statePresenter.ActiveWpfToolWindowMinimizeToolTip;

        public string ActiveWpfToolWindowMaximizeRestoreToolTip => statePresenter.ActiveWpfToolWindowMaximizeRestoreToolTip;

        public string ActiveWpfToolWindowCloseToolTip => statePresenter.ActiveWpfToolWindowCloseToolTip;

        public bool IsActiveWpfToolWindowVisible => IsDockedToolInspectorVisible || statePresenter.IsActiveWpfToolWindowVisibleForTest;

        public bool IsDockedToolInspectorVisible => dockedToolInspectorController.IsVisible;

        public string ActivePendingToolTitle => statePresenter.ActivePendingToolTitle;

        public string ActivePendingToolStatusText => statePresenter.ActivePendingToolStatusText;

        public string ActiveNativeDocumentTypeName => statePresenter.ActiveNativeDocumentTypeName;

        public string ActiveNativeStatusText => statePresenter.ActiveNativeStatusText;

        public string ActiveNativeResultReviewText => statePresenter.ActiveNativeResultReviewText;

        public string ActiveNativeRouteInputLayerName => statePresenter.ActiveNativeRouteInputLayerNameForTest;

        public string ActiveNativeRouteInputLayerBName => statePresenter.ActiveNativeRouteInputLayerBNameForTest;

        public string ActiveNativeRouteOutputLayerName => statePresenter.ActiveNativeRouteOutputLayerNameForTest;

        public bool IsNativeToolPrewarmCompleted => statePresenter.IsNativeToolPrewarmCompletedForTest;

        public int NativeToolPrewarmCreatedCount => statePresenter.NativeToolPrewarmCreatedCountForTest;

        public long NativeToolPrewarmElapsedMilliseconds => statePresenter.NativeToolPrewarmElapsedMillisecondsForTest;

        public int NativeToolDocumentCacheCount => statePresenter.NativeToolDocumentCacheCountForTest;

        public string LastToolOpenTimingText => toolWindowController.LastTiming?.ToPerfText() ?? string.Empty;

        public bool IsShellLoaded => bindings.IsShellLoaded();

        public int HostedDocumentCount => statePresenter.HostedDocumentCount;

        public bool IsNativeDocumentActive => statePresenter.IsNativeDocumentActive;

        public int NativePreviewRunCount => statePresenter.NativePreviewRunCount;

        public bool HasNativePreviewResult => statePresenter.HasNativePreviewResult;

        public int ActiveLineInputRoiOverlayCount => statePresenter.ActiveLineInputRoiOverlayCount;
        public bool ActiveLineSignalInspectorHasEvidence => statePresenter.ActiveLineSignalInspectorHasEvidence;
        public bool ActiveLineSignalInspectorOverlayVisible => statePresenter.ActiveLineSignalInspectorOverlayVisible;
        public string ActiveLineSignalInspectorEvidenceId => statePresenter.ActiveLineSignalInspectorEvidenceId;
        public string ActiveLineSignalInspectorSourceSha256 => statePresenter.ActiveLineSignalInspectorSourceSha256;
        public int ActiveLineSignalInspectorSeriesCount => statePresenter.ActiveLineSignalInspectorSeriesCount;
        public int ActiveLineSignalInspectorMarkerCount => statePresenter.ActiveLineSignalInspectorMarkerCount;

        public bool IsToolRailCompact => bindings.IsToolRailCompact();

        public double ToolRailWidth => bindings.ToolRailWidth();

        public bool IsToolRailNavigationVisible => bindings.IsToolRailNavigationVisible();

        public bool IsToolRailCompactLabelHidden => bindings.IsToolRailCompactLabelHidden();

        public string DirectResultBadgeText => bindings.DirectResultBadgeText();

        public string DirectResultTitleText => bindings.DirectResultTitleText();

        public string DirectResultStatusText => bindings.DirectResultStatusText();

        public string DirectResultRouteText => bindings.DirectResultRouteText();

        public int PipelineReviewStepCount => statePresenter.PipelineReviewStepCount;

        public string PipelineReviewRecipeContextName => statePresenter.PipelineReviewRecipeContextName;

        public string PipelineReviewRecipeContextPipelineName => statePresenter.PipelineReviewRecipeContextPipelineName;

        public string PipelineReviewSelectedStepName => statePresenter.PipelineReviewSelectedStepName;

        public string PipelineReviewSelectedStatusText => statePresenter.PipelineReviewSelectedStatusText;

        public string PipelineReviewFlowSummaryText => statePresenter.PipelineReviewFlowSummaryText;

        public string PipelineReviewParameterSummaryText => statePresenter.PipelineReviewParameterSummaryText;

        public string PipelineReviewValidationStatusText => statePresenter.PipelineReviewValidationStatusText;

        public string PipelineReviewValidationDetailText => statePresenter.PipelineReviewValidationDetailText;

        public string PipelineReviewResultSummaryText => statePresenter.PipelineReviewResultSummaryText;

        public string PipelineReviewResultDetailText => statePresenter.PipelineReviewResultDetailText;

        public string PipelineReviewRunLogText => statePresenter.PipelineReviewRunLogText;

        public int PipelineReviewObjectResultCount => statePresenter.PipelineReviewObjectResultCount;
        public int PipelineReviewObjectMetricDistributionSeriesCount => statePresenter.PipelineReviewObjectMetricDistributionSeriesCount;
        public int PipelineReviewObjectMetricDistributionMarkerCount => statePresenter.PipelineReviewObjectMetricDistributionMarkerCount;
        public string PipelineReviewObjectMetricDistributionMetric => statePresenter.PipelineReviewObjectMetricDistributionMetric;
        public string PipelineReviewObjectMetricDistributionEvidenceId => statePresenter.PipelineReviewObjectMetricDistributionEvidenceId;
        public bool PipelineReviewMatcherDiagnosticTabVisible => statePresenter.PipelineReviewMatcherDiagnosticTabVisible;
        public string PipelineReviewMatcherDiagnosticState => statePresenter.PipelineReviewMatcherDiagnosticState;
        public string PipelineReviewMatcherDiagnosticEvidenceId => statePresenter.PipelineReviewMatcherDiagnosticEvidenceId;
        public int PipelineReviewMatcherDiagnosticRowCount => statePresenter.PipelineReviewMatcherDiagnosticRowCount;
        public int PipelineReviewMatcherDiagnosticModelPointCount => statePresenter.PipelineReviewMatcherDiagnosticModelPointCount;
        public bool PipelineReviewMatcherDiagnosticHasSelectedCandidate => statePresenter.PipelineReviewMatcherDiagnosticHasSelectedCandidate;
        public bool PipelineReviewMatcherDiagnosticHasAlternative => statePresenter.PipelineReviewMatcherDiagnosticHasAlternative;

        public bool IsPipelineReviewFixtureDesignerVisible => statePresenter.IsPipelineReviewFixtureDesignerVisible;

        public string PipelineReviewFixtureRelationshipText => statePresenter.PipelineReviewFixtureRelationshipText;

        public int PipelineReviewFixtureProducerStepNumber => statePresenter.PipelineReviewFixtureProducerStepNumber;

        public int PipelineReviewFixtureMeasurementStepNumber => statePresenter.PipelineReviewFixtureMeasurementStepNumber;

        public int PipelineReviewSelectedObjectResultNumber => statePresenter.PipelineReviewSelectedObjectResultNumber;

        public bool HasPipelineReviewObjectHighlight => statePresenter.HasPipelineReviewObjectHighlight;

        public string PipelineReviewExecutionState => statePresenter.PipelineReviewExecutionState;

        public string PipelineReviewProgressText => statePresenter.PipelineReviewProgressText;

        public string PipelineReviewGuideStageText => statePresenter.PipelineReviewGuideStageText;

        public string PipelineReviewGuideCurrentStepText => statePresenter.PipelineReviewGuideCurrentStepText;

        public string PipelineReviewGuideNextActionText => statePresenter.PipelineReviewGuideNextActionText;

        public string PipelineReviewGuideResultDecisionText => statePresenter.PipelineReviewGuideResultDecisionText;

        public string PipelineReviewGuideDetailText => statePresenter.PipelineReviewGuideDetailText;

        public string PipelineReviewGuidePairText => statePresenter.PipelineReviewGuidePairText;

        public string PipelineReviewGuidePairActionText => statePresenter.PipelineReviewGuidePairActionText;

        public string PipelineReviewGuidePairMetricText => statePresenter.PipelineReviewGuidePairMetricText;

        public string PipelineReviewGuideChecklistText => statePresenter.PipelineReviewGuideChecklistText;

        public string PipelineReviewGuideParameterFocusText => statePresenter.PipelineReviewGuideParameterFocusText;

        public string PipelineReviewGuideTriageFailureText => statePresenter.PipelineReviewGuideTriageFailureText;

        public string PipelineReviewGuideTriageAdjustmentText => statePresenter.PipelineReviewGuideTriageAdjustmentText;

        public string PipelineReviewGuideTriageRerunText => statePresenter.PipelineReviewGuideTriageRerunText;

        public bool CanOpenPipelineReviewPairSample => statePresenter.CanOpenPipelineReviewPairSample;

        public bool CanSelectPreviousPipelineReviewStep => statePresenter.CanSelectPreviousPipelineReviewStep;

        public bool CanSelectNextPipelineReviewStep => statePresenter.CanSelectNextPipelineReviewStep;

        public bool CanSelectFirstIssuePipelineReviewStep => statePresenter.CanSelectFirstIssuePipelineReviewStep;

        public bool HasPipelineReviewInputPreview => statePresenter.HasPipelineReviewInputPreview;

        public bool HasPipelineReviewOutputPreview => statePresenter.HasPipelineReviewOutputPreview;

        public string DockedToolTitle => bindings.DockedToolTitleText();

        public bool IsDockedToolFloatButtonVisible => bindings.IsDockedToolFloatButtonVisible();

        public bool IsDockedToolCloseButtonVisible => bindings.IsDockedToolCloseButtonVisible();

        public double DockedToolFloatButtonWidth => bindings.DockedToolFloatButtonWidth();

        public double DockedToolCloseButtonWidth => bindings.DockedToolCloseButtonWidth();

        public string DockedToolFloatButtonToolTip => bindings.DockedToolFloatButtonToolTipText();

        public string DockedToolCloseButtonToolTip => bindings.DockedToolCloseButtonToolTipText();

        public double DockedToolInspectorWidth => bindings.DockedToolInspectorWidth();

        public void SelectTool(VISION_MENU menu)
        {
            OpenVisionShellNavItem item = viewModel.NavigationGroups
                .SelectMany(group => group.Items)
                .FirstOrDefault(command => command.Menu == menu);
            if (item == null)
            {
                throw new InvalidOperationException("WPF shell host command not found: " + menu);
            }

            viewModel.SelectToolCommand.Execute(item);
        }

        public void ToggleToolRail()
        {
            bindings.SetToolRailCompact(!bindings.IsToolRailCompact());
        }

        public bool HasNativeToolDocumentCached(VISION_MENU menu)
        {
            return documentController.NativeToolDocuments.Contains(menu);
        }

        public void RunActiveNativePreview()
        {
            documentController.ActiveNativeDocument?.RunPreview();
            toolWindowLifecycleController.RefreshAfterNativeLayerStateChanged(documentController.ActiveNativeDocument?.HasPreviewResult == true);
        }

        public void CreateActiveNativeOutputLayer()
        {
            documentController.ActiveNativeDocument?.CreateOutputLayerForTest();
            chromeController.RefreshDirectRouteText();
            refreshCoordinator.RefreshHostLayerRows();
            toolWindowLifecycleController.RefreshAfterNativeLayerStateChanged(documentController.ActiveNativeDocument?.HasPreviewResult == true);
        }

        public bool CloseActiveWpfToolWindow()
        {
            return toolWindowLifecycleController.CloseActiveWpfToolWindowByUser();
        }

        public bool DockActiveWpfToolWindow()
        {
            return floatingToolWindowHost.RequestDockForTest();
        }

        public bool FloatDockedWpfToolWindow()
        {
            return toolWindowLifecycleController.FloatDockedTool(toolWindowController.ShowWpfToolWindow);
        }

        public void SetDockedToolInspectorWidth(double width)
        {
            bindings.SetDockedToolInspectorWidth(width);
        }

        public bool LoadActiveNativePreviewImageFromFile(string path, VisionToolPreviewImageRole role)
        {
            bool loadedImage = documentController.ActiveNativeDocument?.LoadPreviewImageFromFileForTest(role, path) ?? false;
            if (!loadedImage)
            {
                return false;
            }

            string refreshedLayer = role switch
            {
                VisionToolPreviewImageRole.InputB => documentController.ActiveNativeDocument.RouteInputLayerBName,
                VisionToolPreviewImageRole.Output => documentController.ActiveNativeDocument.RouteOutputLayerName,
                _ => documentController.ActiveNativeDocument.RouteInputLayerName
            };
            refreshCoordinator.RefreshHostSelectedLayerDetail(refreshedLayer);
            refreshCoordinator.RefreshHostLayerRows();
            chromeController.RefreshDirectRouteText();
            return true;
        }

        public bool SaveActiveNativePreviewImageToFile(string path, VisionToolPreviewImageRole role)
        {
            return documentController.ActiveNativeDocument?.SavePreviewImageToFileForTest(role, path) ?? false;
        }

        public bool ConfigureActiveThresholdBasicInvert(bool invert)
        {
            ThresholdToolWpfView thresholdView = documentController.ActiveNativeDocument?.View as ThresholdToolWpfView;
            if (thresholdView == null)
            {
                return false;
            }

            thresholdView.ConfigureBasicInvertForTest(invert);
            chromeController.RefreshDirectRouteText();
            refreshCoordinator.RefreshHostLayerRows();
            return true;
        }

        public VisionPipelineStep AddActiveNativePipelineStep()
        {
            VisionPipelineStep step = documentController.ActiveNativeDocument?.AddPipelineStep();
            chromeController.RefreshDirectRouteText();
            refreshCoordinator.RefreshHostLayerRows();
            return step;
        }

        public void SetActiveLineRoi(int x, int y, int width, int height)
        {
            documentController.ActiveNativeDocument?.SetLineRoiForTest(new OpenCvSharp.Rect(x, y, width, height));
            chromeController.RefreshDirectRouteText();
            refreshCoordinator.RefreshHostLayerRows();
        }

        public void SetActiveLineSetting(string setting)
        {
            documentController.ActiveNativeDocument?.SetLineSettingForTest(setting);
            chromeController.RefreshDirectRouteText();
            refreshCoordinator.RefreshHostLayerRows();
        }

        public void SetActiveSelectedLineRoi(int x, int y, int width, int height)
        {
            documentController.ActiveNativeDocument?.SetSelectedLineRoiForTest(new OpenCvSharp.Rect(x, y, width, height));
            chromeController.RefreshDirectRouteText();
            refreshCoordinator.RefreshHostLayerRows();
        }

        public void ConfigureActiveSelectedLine(string projectionDirection, string polarity, string verticalDirection)
        {
            documentController.ActiveNativeDocument?.ConfigureSelectedLineForTest(projectionDirection, polarity, verticalDirection);
            chromeController.RefreshDirectRouteText();
            refreshCoordinator.RefreshHostLayerRows();
        }

        public void ConfigureActiveSelectedLineDraw(bool showVerticalLine, bool showEdge, bool showContour, bool showFitLine)
        {
            documentController.ActiveNativeDocument?.ConfigureSelectedLineDrawForTest(showVerticalLine, showEdge, showContour, showFitLine);
            chromeController.RefreshDirectRouteText();
            refreshCoordinator.RefreshHostLayerRows();
        }

        public void ConfigureActiveSelectedLineThreshold(double threshold, bool invert)
        {
            documentController.ActiveNativeDocument?.ConfigureSelectedLineThresholdForTest(threshold, invert);
            chromeController.RefreshDirectRouteText();
            refreshCoordinator.RefreshHostLayerRows();
        }

        public void ConfigureActiveSelectedLineMeasureTuning(
            bool useThreshold,
            bool useAdaptiveThreshold,
            double contrast,
            double thickness,
            double samplingStep,
            int pointRange,
            bool useManualAngle,
            double manualAngleValue)
        {
            documentController.ActiveNativeDocument?.ConfigureSelectedLineMeasureTuningForTest(
                useThreshold,
                useAdaptiveThreshold,
                contrast,
                thickness,
                samplingStep,
                pointRange,
                useManualAngle,
                manualAngleValue);
            chromeController.RefreshDirectRouteText();
            refreshCoordinator.RefreshHostLayerRows();
        }

        public void SetActiveLinePurpose(string purpose)
        {
            documentController.ActiveNativeDocument?.SetLinePurposeForTest(purpose);
            chromeController.RefreshDirectRouteText();
            refreshCoordinator.RefreshHostLayerRows();
        }

        public string GetActiveLineSignalInspectorAttribute(string name)
        {
            return documentController.ActiveNativeDocument?.GetLineSignalInspectorAttributeForTest(name) ?? string.Empty;
        }

        public bool ExerciseActiveLineSignalInspectorNavigation()
        {
            return documentController.ActiveNativeDocument?.ExerciseLineSignalInspectorNavigationForTest() == true;
        }

        public void ExportActiveLineSignalEvidence(string path)
        {
            documentController.ActiveNativeDocument?.ExportLineSignalEvidenceForTest(path);
        }

        public void CloseActiveLineSignalInspector()
        {
            documentController.ActiveNativeDocument?.CloseLineSignalInspectorForTest();
        }

        public void OpenActiveLineSignalInspector()
        {
            documentController.ActiveNativeDocument?.OpenLineSignalInspectorForTest();
        }

        public void SetActiveMatchingTemplatePath(string path)
        {
            documentController.ActiveNativeDocument?.SetMatchingTemplatePathForTest(path);
            chromeController.RefreshDirectRouteText();
            refreshCoordinator.RefreshHostLayerRows();
        }

        public void ConfigureActiveMatching(Action<MatchingProperty> configure)
        {
            documentController.ActiveNativeDocument?.ConfigureMatchingForTest(configure);
            chromeController.RefreshDirectRouteText();
            refreshCoordinator.RefreshHostLayerRows();
        }

        public void ConfigureActiveAffineTransform(Action<AffineTransformProperty> configure)
        {
            documentController.ActiveNativeDocument?.ConfigureAffineTransformForTest(configure);
            chromeController.RefreshDirectRouteText();
            refreshCoordinator.RefreshHostLayerRows();
        }

        public void SetActiveEdgeBasedMatchingTemplatePath(string path)
        {
            documentController.ActiveNativeDocument?.SetEdgeBasedMatchingTemplatePathForTest(path);
            chromeController.RefreshDirectRouteText();
            refreshCoordinator.RefreshHostLayerRows();
        }

        public void ConfigureActiveEdgeBasedMatching(Action<EdgeBasedMatchingProperty> configure)
        {
            documentController.ActiveNativeDocument?.ConfigureEdgeBasedMatchingForTest(configure);
            chromeController.RefreshDirectRouteText();
            refreshCoordinator.RefreshHostLayerRows();
        }

        public void SetActiveAutoMPointRepresentativeImages(IEnumerable<string> paths)
        {
            documentController.ActiveNativeDocument?.SetAutoMPointRepresentativeImagesForTest(paths);
            chromeController.RefreshDirectRouteText();
            refreshCoordinator.RefreshHostLayerRows();
        }

        public void SetActiveFeatureMatchingTemplatePath(string path)
        {
            documentController.ActiveNativeDocument?.SetFeatureMatchingTemplatePathForTest(path);
            chromeController.RefreshDirectRouteText();
            refreshCoordinator.RefreshHostLayerRows();
        }

        public bool RunActiveToolForm()
        {
            return false;
        }

        public void SelectPipelineReviewStep(int index, OpenVisionLab.Pipeline.Controls.PipelineFlowPreviewMode mode)
        {
            documentController.ActivePipelineReviewDocument?.SelectStepForTest(index, mode);
        }

        public Task RunPipelineReviewAsync()
        {
            return documentController.ActivePipelineReviewDocument?.RunReviewForTestAsync() ?? Task.CompletedTask;
        }

        public bool OpenPipelineReviewPairSample()
        {
            return documentController.ActivePipelineReviewDocument?.OpenPairSampleForTest() == true;
        }

        public void SelectPipelineReviewObjectResult(int index)
        {
            documentController.ActivePipelineReviewDocument?.SelectObjectResultForTest(index);
        }

        public void SelectPipelineReviewObjectResultFromImage(int index)
        {
            documentController.ActivePipelineReviewDocument?.SelectObjectResultFromImageForTest(index);
        }
    }

    internal sealed class OpenVisionShellHostToolTestFacadeBindings
    {
        public Func<bool> IsShellLoaded { get; set; } = False;

        public Func<bool> IsToolRailCompact { get; set; } = False;

        public Action<bool> SetToolRailCompact { get; set; } = _ => { };

        public Func<double> ToolRailWidth { get; set; } = Zero;

        public Func<bool> IsToolRailNavigationVisible { get; set; } = False;

        public Func<bool> IsToolRailCompactLabelHidden { get; set; } = False;

        public Func<string> DirectResultBadgeText { get; set; } = EmptyText;

        public Func<string> DirectResultTitleText { get; set; } = EmptyText;

        public Func<string> DirectResultStatusText { get; set; } = EmptyText;

        public Func<string> DirectResultRouteText { get; set; } = EmptyText;

        public Func<string> DockedToolTitleText { get; set; } = EmptyText;

        public Func<bool> IsDockedToolFloatButtonVisible { get; set; } = False;

        public Func<bool> IsDockedToolCloseButtonVisible { get; set; } = False;

        public Func<double> DockedToolFloatButtonWidth { get; set; } = Zero;

        public Func<double> DockedToolCloseButtonWidth { get; set; } = Zero;

        public Func<string> DockedToolFloatButtonToolTipText { get; set; } = EmptyText;

        public Func<string> DockedToolCloseButtonToolTipText { get; set; } = EmptyText;

        public Func<double> DockedToolInspectorWidth { get; set; } = Zero;

        public Action<double> SetDockedToolInspectorWidth { get; set; } = _ => { };

        private static string EmptyText()
        {
            return string.Empty;
        }

        private static bool False()
        {
            return false;
        }

        private static double Zero()
        {
            return 0D;
        }
    }
}
