using OpenVisionLab.Vision2D.Pipeline;
using OpenVisionLab.Core;
using OpenVisionLab.Pipeline.Controls;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace OpenVisionLab
{
    internal sealed partial class OpenVisionPipelineReviewDocument : IDisposable
    {
        private const int StepTimeoutMilliseconds = 60000;
        private readonly IDisplayManager displayManager;
        private readonly OpenVisionRecipeContext recipeContext;
        private readonly OpenVisionPipelineReviewView view;
        private readonly OpenVisionPipelineReviewExecutionController executionController;
        private VisionPipeline pipeline;
        private VisionPipelineValidationResult validationResult;
        private OpenVisionWorkspaceSamplePairDecisionGuide activeSamplePairGuide = OpenVisionWorkspaceSamplePairDecisionGuide.Empty;
        private VisionPipelineSampleCatalogItem activeCatalogSample;
        private VisionPipelineSampleCatalogItem activePairCounterpartSample;
        private string activePipelineName = string.Empty;
        private DateTime activePipelineLastWriteUtc;
        private int selectedIndex;
        private PipelineFlowPreviewMode selectedMode = PipelineFlowPreviewMode.Overlay;
        private string reviewExecutionState = T("PipelineReview.Execution.NotRun", "Not run");
        private int fixtureProducerIndex = -1;
        private int fixtureMeasurementIndex = -1;
        private bool disposed;

        public OpenVisionPipelineReviewDocument(IDisplayManager displayManager, string recipeName)
            : this(
                displayManager,
                new OpenVisionRecipeContext(
                    id: recipeName,
                    name: recipeName,
                    pipelineName: VisionPipelineAppendService.DefaultPipelineName,
                    sourcePath: string.Empty,
                    isDirty: false,
                    activeLayerName: "Main",
                    lastReviewState: string.Empty))
        {
        }

        public OpenVisionPipelineReviewDocument(IDisplayManager displayManager, OpenVisionRecipeContext recipeContext)
        {
            this.displayManager = displayManager ?? throw new ArgumentNullException(nameof(displayManager));
            this.recipeContext = recipeContext ?? new OpenVisionRecipeContext(
                id: "Default",
                name: "Default",
                pipelineName: VisionPipelineAppendService.DefaultPipelineName,
                sourcePath: string.Empty,
                isDirty: false,
                activeLayerName: "Main",
                lastReviewState: string.Empty);
            view = new OpenVisionPipelineReviewView();
            executionController = new OpenVisionPipelineReviewExecutionController(displayManager, InvokeOnViewDispatcher);
            executionController.StepUpdated += OnReviewStepExecutionUpdated;
            view.StepSelected += OnStepSelected;
            view.RunReviewRequested += OnRunReviewRequested;
            view.PreviousStepRequested += OnPreviousStepRequested;
            view.NextStepRequested += OnNextStepRequested;
            view.FirstIssueStepRequested += OnFirstIssueStepRequested;
            view.OpenPairSampleRequested += OnOpenPairSampleRequested;
            view.UseSelectedMatchingPoseRequested += OnUseSelectedMatchingPoseRequested;
            view.ReturnToRecipeRequested += OnReturnToRecipeRequested;
            view.OpenSelectedToolLearnRequested += OnOpenSelectedToolLearnRequested;
            view.EditSelectedStepRequested += OnEditSelectedStepRequested;
            view.EditFixtureProducerRequested += OnEditFixtureProducerRequested;
            view.EditFixtureMeasurementRequested += OnEditFixtureMeasurementRequested;
            view.FixtureConsumerSelected += OnFixtureConsumerSelected;
            view.ScaleCalibrationRequested += OnScaleCalibrationRequested;
            view.ScaleCalibrationApplyRequested += OnScaleCalibrationApplyRequested;
            OpenVisionLanguageService.LanguageChanged += OnLanguageChanged;
            RefreshLayerState();
        }

        public FrameworkElement View => view;
        public string ActiveViewTypeName => view.GetType().Name;
        public OpenVisionRecipeContext RecipeContext => recipeContext;
        public string ActivePipelineName => string.IsNullOrWhiteSpace(activePipelineName) ? ResolveActivePipelineName() : activePipelineName;
        public int StepCount => pipeline?.Steps?.Count ?? 0;
        public string SelectedStepName => view.SelectedStepText;
        public string SelectedToolType => view.SelectedToolText;
        public int SelectedStepNumber => selectedIndex < 0 ? 0 : selectedIndex + 1;
        public string SelectedStatusText => view.SelectedStatusText;
        public string RecipeContextText => view.RecipeContextText;
        public string ReviewProgressText => view.ReviewProgressText;
        public string FlowSummaryText => view.FlowSummaryText;
        public string ParameterSummaryText => view.ParameterSummaryText;
        public string ValidationStatusText => view.ValidationStatusText;
        public string ValidationDetailText => view.ValidationDetailText;
        public string ResultSummaryText => view.ResultSummaryText;
        public string ResultDetailText => view.ResultDetailText;
        public string RunLogText => view.RunLogText;
        public string ReadinessSummaryText => view.ReadinessSummaryText;
        public string ReviewExecutionState => reviewExecutionState;
        public string GuideStageText => view.ReviewGuideStageText;
        public string GuideCurrentStepText => view.ReviewGuideCurrentStepText;
        public string GuideNextActionText => view.ReviewGuideNextActionText;
        public string GuideResultDecisionText => view.ReviewGuideResultDecisionText;
        public string GuideDetailText => view.ReviewGuideDetailText;
        public string GuidePairText => view.ReviewGuidePairText;
        public string GuidePairActionText => view.ReviewGuidePairActionText;
        public string GuidePairMetricText => view.ReviewGuidePairMetricText;
        public string GuideChecklistText => view.ReviewGuideChecklistText;
        public string GuideParameterFocusText => view.ReviewGuideParameterFocusText;
        public string GuideTriageFailureText => view.ReviewGuideTriageFailureText;
        public string GuideTriageAdjustmentText => view.ReviewGuideTriageAdjustmentText;
        public string GuideTriageRerunText => view.ReviewGuideTriageRerunText;
        public bool CanOpenPairSample => view.CanOpenReviewGuidePairAction;
        public bool CanSelectPreviousStep => view.CanSelectPreviousStep;
        public bool CanSelectNextStep => view.CanSelectNextStep;
        public bool CanSelectFirstIssueStep => view.CanSelectFirstIssueStep;
        public bool HasInputPreview => view.HasInputPreview;
        public bool HasOutputPreview => view.HasOutputPreview;
        internal System.Windows.Media.Imaging.BitmapImage OutputPreviewImageForTest => view.OutputPreviewImageForTest;
        public int ObjectResultCount => view.ObjectResultCount;
        public int SelectedObjectResultNumber => view.SelectedObjectResultNumber;
        public bool HasObjectHighlight => view.HasObjectHighlight;
        public int ObjectMetricDistributionSeriesCount => view.ObjectMetricDistributionSeriesCountForTest;
        public int ObjectMetricDistributionMarkerCount => view.ObjectMetricDistributionMarkerCountForTest;
        public string ObjectMetricDistributionMetric => view.ObjectMetricDistributionMetricForTest;
        public string ObjectMetricDistributionEvidenceId => view.ObjectMetricDistributionEvidenceIdForTest;
        public bool MatcherDiagnosticTabVisible => view.MatcherDiagnosticTabVisibleForTest;
        public string MatcherDiagnosticState => view.MatcherDiagnosticStateForTest;
        public string MatcherDiagnosticEvidenceId => view.MatcherDiagnosticEvidenceIdForTest;
        public int MatcherDiagnosticRowCount => view.MatcherDiagnosticRowCountForTest;
        public int MatcherDiagnosticModelPointCount => view.MatcherDiagnosticModelPointCountForTest;
        public bool MatcherDiagnosticHasSelectedCandidate => view.MatcherDiagnosticHasSelectedCandidateForTest;
        public bool MatcherDiagnosticHasAlternative => view.MatcherDiagnosticHasAlternativeForTest;
        public bool IsFixtureDesignerVisible => view.IsFixtureDesignerVisible;
        public string FixtureRelationshipText => view.FixtureRelationshipText;
        public string ScaleCalibrationStatusText => view.ScaleCalibrationStatusText;
        public string ScaleCalibrationResultText => view.ScaleCalibrationResultText;
        public int FixtureProducerStepNumber => fixtureProducerIndex < 0 ? 0 : fixtureProducerIndex + 1;
        public int FixtureMeasurementStepNumber => fixtureMeasurementIndex < 0 ? 0 : fixtureMeasurementIndex + 1;

        public event EventHandler LayerStateChanged = delegate { };
        public event EventHandler<OpenVisionPipelineReviewSampleOpenRequestedEventArgs> OpenWorkspaceSampleRequested = delegate { };
        public event EventHandler ReturnToRecipeRequested = delegate { };
        public event EventHandler OpenSelectedToolLearnRequested = delegate { };
        public event EventHandler EditSelectedStepRequested = delegate { };

        public void RefreshLayerState()
        {
            activePipelineName = ResolveActivePipelineName();
            pipeline = VisionPipelineStorage.Load(recipeContext.Name, activePipelineName);
            activePipelineLastWriteUtc = GetPipelineLastWriteUtc(activePipelineName);
            RefreshActiveSamplePairGuide(activePipelineName);
            validationResult = VisionPipelineValidator.Validate(pipeline, GetLayerNames());
            executionController.Reset();
            int stepCount = pipeline?.Steps?.Count ?? 0;
            view.SetRecipeContext(recipeContext.Name);
            view.SetPipelineHeader(activePipelineName, stepCount);
            view.SetReviewProgress(FormatReviewProgressText());
            view.SetValidation(FormatValidationStatus(validationResult), FormatValidationDetails(validationResult));
            RefreshReadiness();
            view.SetResultSummary(
                T("PipelineReview.RunRequired", "Run review required"),
                T("PipelineReview.RunRequiredDetail", "Click Run Review to refresh step results."));

            if (stepCount == 0)
            {
                selectedIndex = -1;
                view.SetEmptyState(activePipelineName);
                view.SetValidation(FormatValidationStatus(validationResult), FormatValidationDetails(validationResult));
                view.SetScaleCalibrationState(
                    Array.Empty<VisionPipelineGeometryFeatureResult>(),
                    Array.Empty<VisionPipelineScaleTargetOption>(),
                    null,
                    null,
                    T("PipelineReview.ScaleCalibration.NoSteps", "Add measurement Steps before teaching scale."));
                LayerStateChanged(this, EventArgs.Empty);
                return;
            }

            int preservedIndex = selectedIndex >= 0 ? selectedIndex : view.SelectedFlowIndex;
            view.SetSteps(CreateFlowItems(pipeline.Steps));
            if (preservedIndex >= 0 && preservedIndex < stepCount)
            {
                selectedIndex = preservedIndex;
            }
            else if (selectedIndex < 0 || selectedIndex >= stepCount)
            {
                selectedIndex = 0;
            }

            SelectStep(selectedIndex, selectedMode);
            LayerStateChanged(this, EventArgs.Empty);
        }

        public void RefreshInputLayerState()
        {
            if (pipeline?.Steps == null || pipeline.Steps.Count == 0)
            {
                RefreshLayerState();
                return;
            }

            executionController.Reset();
            reviewExecutionState = T("PipelineReview.Execution.NotRun", "Not run");
            validationResult = VisionPipelineValidator.Validate(pipeline, GetLayerNames());
            view.SetReviewProgress(FormatReviewProgressText());
            view.SetValidation(FormatValidationStatus(validationResult), FormatValidationDetails(validationResult));
            RefreshReadiness();
            view.SetSteps(CreateFlowItems(pipeline.Steps));
            SelectStep(
                selectedIndex >= 0 && selectedIndex < pipeline.Steps.Count ? selectedIndex : 0,
                selectedMode);
        }

        public void RefreshIfPipelineChanged()
        {
            string resolvedPipelineName = ResolveActivePipelineName();
            if (!string.Equals(activePipelineName, resolvedPipelineName, StringComparison.Ordinal)
                || activePipelineLastWriteUtc != GetPipelineLastWriteUtc(resolvedPipelineName))
            {
                RefreshLayerState();
            }
        }

        private DateTime GetPipelineLastWriteUtc(string pipelineName)
        {
            string path = RecipeWorkspaceService.GetVisionPipelinePath(recipeContext.Name, pipelineName);
            return File.Exists(path) ? File.GetLastWriteTimeUtc(path) : DateTime.MinValue;
        }

        public void SelectStepForTest(int index, PipelineFlowPreviewMode mode)
        {
            SelectStep(index, mode);
        }

        public Task RunReviewForTestAsync()
        {
            return RunReviewAsync();
        }

        public void SelectObjectResultForTest(int index)
        {
            view.SelectObjectResultForTest(index);
        }

        public void SelectObjectResultFromImageForTest(int index)
        {
            view.SelectObjectResultFromImageForTest(index);
        }

        internal bool TeachScaleForTest(
            string pointAIdentity,
            string pointBIdentity,
            double knownDistance,
            VisionScaleCalibrationUnit unit)
        {
            return view.RequestScaleCalibrationForTest(pointAIdentity, pointBIdentity, knownDistance, unit);
        }

        internal bool ApplyScaleForTest(int stepIndex)
        {
            return view.RequestScaleCalibrationApplyForTest(stepIndex);
        }

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            disposed = true;
            view.StepSelected -= OnStepSelected;
            view.RunReviewRequested -= OnRunReviewRequested;
            view.PreviousStepRequested -= OnPreviousStepRequested;
            view.NextStepRequested -= OnNextStepRequested;
            view.FirstIssueStepRequested -= OnFirstIssueStepRequested;
            view.OpenPairSampleRequested -= OnOpenPairSampleRequested;
            view.UseSelectedMatchingPoseRequested -= OnUseSelectedMatchingPoseRequested;
            view.ReturnToRecipeRequested -= OnReturnToRecipeRequested;
            view.OpenSelectedToolLearnRequested -= OnOpenSelectedToolLearnRequested;
            view.EditSelectedStepRequested -= OnEditSelectedStepRequested;
            view.EditFixtureProducerRequested -= OnEditFixtureProducerRequested;
            view.EditFixtureMeasurementRequested -= OnEditFixtureMeasurementRequested;
            view.FixtureConsumerSelected -= OnFixtureConsumerSelected;
            view.ScaleCalibrationRequested -= OnScaleCalibrationRequested;
            view.ScaleCalibrationApplyRequested -= OnScaleCalibrationApplyRequested;
            OpenVisionLanguageService.LanguageChanged -= OnLanguageChanged;
            OpenWorkspaceSampleRequested = delegate { };
            ReturnToRecipeRequested = delegate { };
            OpenSelectedToolLearnRequested = delegate { };
            EditSelectedStepRequested = delegate { };
            executionController.StepUpdated -= OnReviewStepExecutionUpdated;
            executionController.Dispose();
        }


        private string ResolveActivePipelineName()
        {
            return VisionPipelineStorage.LoadActivePipelineName(
                recipeContext.Name,
                string.IsNullOrWhiteSpace(recipeContext.PipelineName)
                    ? VisionPipelineAppendService.DefaultPipelineName
                    : recipeContext.PipelineName);
        }

        private void RefreshActiveSamplePairGuide(string pipelineName)
        {
            activeSamplePairGuide = OpenVisionWorkspaceSamplePairDecisionGuide.Empty;
            activeCatalogSample = null;
            activePairCounterpartSample = null;
            VisionPipelineSampleCatalogItem sample = ResolveCatalogSampleForPipeline(pipelineName);
            if (sample == null || string.IsNullOrWhiteSpace(sample.PairGroup))
            {
                return;
            }

            activeCatalogSample = sample;
            string pairGroup = sample.PairGroup.Trim();
            List<VisionPipelineSampleCatalogItem> pairSamples = VisionPipelineSampleCatalogItem
                .LoadRunnable(sample.CatalogSourceKind)
                .Where(item => item != null
                    && item.CanOpen
                    && string.Equals(item.PairGroup?.Trim(), pairGroup, StringComparison.OrdinalIgnoreCase))
                .OrderBy(item => IsOkSampleReference(item) ? 0 : 1)
                .ThenBy(item => item.SampleName, StringComparer.OrdinalIgnoreCase)
                .ToList();
            activeSamplePairGuide = OpenVisionWorkspaceSamplePairDecisionGuidePresenter.Create(sample, pairSamples);
            activePairCounterpartSample = ResolvePairCounterpartSample(sample, pairSamples);
        }

        private static VisionPipelineSampleCatalogItem ResolveCatalogSampleForPipeline(string pipelineName)
        {
            if (string.IsNullOrWhiteSpace(pipelineName)
                || !pipelineName.StartsWith("Sample_", StringComparison.Ordinal))
            {
                return null;
            }

            return VisionPipelineSampleCatalogItem
                .LoadRunnable()
                .FirstOrDefault(item => string.Equals(
                    CreateSamplePipelineName(item.SampleName),
                    pipelineName,
                    StringComparison.OrdinalIgnoreCase));
        }

        private static string CreateSamplePipelineName(string sampleName)
        {
            string rawName = string.IsNullOrWhiteSpace(sampleName) ? "Sample" : sampleName.Trim();
            char[] invalidChars = Path.GetInvalidFileNameChars();
            string safeName = new string(rawName.Select(ch => invalidChars.Contains(ch) ? '_' : ch).ToArray());
            return "Sample_" + (string.IsNullOrWhiteSpace(safeName) ? "Pipeline" : safeName);
        }

        private static bool IsOkSampleReference(VisionPipelineSampleCatalogItem item)
        {
            return item != null
                && !item.ExpectsFailure
                && string.Equals(item.PairRole?.Trim(), "Good", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsNgSampleReference(VisionPipelineSampleCatalogItem item)
        {
            return item != null
                && (item.ExpectsFailure
                    || string.Equals(item.PairRole?.Trim(), "Bad", StringComparison.OrdinalIgnoreCase));
        }

        private static VisionPipelineSampleCatalogItem ResolvePairCounterpartSample(
            VisionPipelineSampleCatalogItem selectedSample,
            IEnumerable<VisionPipelineSampleCatalogItem> pairSamples)
        {
            if (selectedSample == null || pairSamples == null)
            {
                return null;
            }

            bool selectedIsOk = IsOkSampleReference(selectedSample);
            bool selectedIsNg = IsNgSampleReference(selectedSample);
            return pairSamples
                .Where(item => item != null && !IsSameSample(item, selectedSample))
                .Where(item =>
                    selectedIsOk
                        ? IsNgSampleReference(item)
                        : selectedIsNg
                            ? IsOkSampleReference(item)
                            : true)
                .OrderBy(item => IsOkSampleReference(item) ? 0 : 1)
                .ThenBy(item => item.SampleName, StringComparer.OrdinalIgnoreCase)
                .FirstOrDefault();
        }

        private static bool IsSameSample(VisionPipelineSampleCatalogItem left, VisionPipelineSampleCatalogItem right)
        {
            return left != null
                && right != null
                && string.Equals(left.SampleName?.Trim(), right.SampleName?.Trim(), StringComparison.OrdinalIgnoreCase);
        }

        private void SelectStep(int index, PipelineFlowPreviewMode mode)
        {
            if (pipeline?.Steps == null || index < 0 || index >= pipeline.Steps.Count)
            {
                return;
            }

            selectedIndex = index;
            selectedMode = mode;
            view.SelectStep(index, mode);
            view.SetNavigationState(index, pipeline.Steps.Count);
            view.SetIssueNavigationState(FindFirstIssueStepIndex() >= 0);

            VisionPipelineStep step = pipeline.Steps[index];
            view.SetSelectedToolLearnState(OpenVisionLearnTopicCatalog.TryResolveForToolType(step.ToolType, out _));
            Bitmap inputImage = ResolveLayerPreviewImage(step.InputLayer);
            Bitmap outputImage = ResolveStepOutputPreviewImage(step.OutputLayer);
            executionController.TryGetSummary(step, out VisionPipelineStepResultSummary summary);
            OpenVisionPipelineReviewFlowProjection flow =
                OpenVisionPipelineReviewFlowPresenter.CreateStepProjection(
                    pipeline.Steps,
                    index,
                    inputImage != null,
                    outputImage != null,
                    summary);

            view.SetSelectedStep(
                FormatStepName(index, step),
                SafeText(step.ToolType, "Tool"),
                flow.StatusText,
                step.InputLayer,
                inputImage,
                step.OutputLayer,
                outputImage,
                flow.FlowSummaryText,
                FormatParameters(step),
                OpenVisionPipelineReviewResultPresenter.FormatRunLog(
                    step,
                    inputImage,
                    outputImage,
                    mode,
                    flow.StatusText,
                    FormatValidationStatus(validationResult),
                    summary));
            view.SetResultSummary(
                OpenVisionPipelineReviewResultPresenter.FormatResultSummary(summary),
                OpenVisionPipelineReviewResultPresenter.FormatResultDetails(step, summary));
            view.SetObjectResults(
                IsObjectResultTool(step),
                step,
                summary?.ObjectResults,
                inputImage,
                outputImage);
            view.SetInstanceResults(
                IsMultiMatchMeanTool(step),
                summary?.InstanceResults);
            view.SetGeometryResults(IsGeometryResultTool(step), summary?.GeometryFeatures);
            view.SetCircleEvidence(
                IsCircleGaugeTool(step),
                summary?.CircleEvidence,
                inputImage,
                outputImage);
            view.SetMatcherDiagnostics(
                IsEdgeBasedMatchingTool(step),
                summary?.EdgeBasedMatchingDiagnostics,
                summary?.Metrics,
                inputImage);
            view.SetReviewGuide(OpenVisionPipelineReviewGuidePresenter.CreateSelected(
                index + 1,
                pipeline.Steps.Count,
                step,
                flow.StatusText,
                inputImage != null,
                outputImage != null,
                summary,
                validationResult,
                flow.ExpectedInputLayer,
                flow.IsBranch,
                flow.InputWillBeProduced,
                activeSamplePairGuide));
            view.SetReviewGuidePairAction(
                OpenVisionPipelineReviewResultPresenter.ResolvePairActionText(activePairCounterpartSample),
                activePairCounterpartSample?.CanOpen == true);
            view.SetReviewGuidePairMetric(OpenVisionPipelineReviewResultPresenter.ResolvePairMetricComparisonText(
                step,
                summary,
                activeCatalogSample,
                activePairCounterpartSample,
                activeSamplePairGuide));
            UpdateFixtureTeachState(step, summary);
            UpdateFixtureDesignerState();
            UpdateScaleCalibrationState();
        }

        private void UpdateScaleCalibrationState(string statusOverride = null)
        {
            List<VisionPipelineGeometryFeatureResult> points = executionController
                .GetCurrentGeometryFeatures()
                .Where(item => item.Kind == VisionPipelineGeometryKind.Point)
                .GroupBy(
                    item => item.Identity + "|" + item.CoordinateLayer + "|" + item.ImageWidth + "x" + item.ImageHeight,
                    StringComparer.OrdinalIgnoreCase)
                .Select(group => group.First())
                .ToList();
            IReadOnlyList<VisionPipelineScaleTargetOption> targets = VisionPipelineScaleCalibrationStorage.GetCompatibleTargets(pipeline);
            VisionPipelineScaleCalibrationStorage.TryLoad(
                recipeContext.Name,
                activePipelineName,
                out VisionPipelineScaleCalibrationRecord record,
                out _);

            string coordinateLayer = record?.CoordinateLayer
                ?? points.FirstOrDefault()?.CoordinateLayer
                ?? string.Empty;
            Bitmap coordinateImage = ResolveLayerPreviewImage(coordinateLayer);
            string status = statusOverride;
            string sourceError = string.Empty;
            if (string.IsNullOrWhiteSpace(status))
            {
                if (record != null
                    && VisionPipelineScaleCalibrationStorage.TryValidateCurrentSource(
                        record,
                        coordinateLayer,
                        coordinateImage,
                        out sourceError))
                {
                    status = "Saved evidence matches the current image. Select one compatible Step to apply; Apply never runs the pipeline.";
                }
                else if (record != null)
                {
                    status = "Saved evidence is not applicable to the current image: " + sourceError;
                }
                else if (points.Count < 2)
                {
                    status = "Run Review explicitly and produce at least two typed Point results in one coordinate layer.";
                }
                else
                {
                    status = "Select two same-run points, enter the certified real distance, then calculate and save evidence.";
                }
            }

            view.SetScaleCalibrationState(points, targets, record, coordinateImage, status);
        }

        private static bool IsObjectResultTool(VisionPipelineStep step)
        {
            string toolType = (step?.ToolType ?? string.Empty).Trim();
            if (toolType.EndsWith("Tool", StringComparison.OrdinalIgnoreCase))
            {
                toolType = toolType.Substring(0, toolType.Length - 4);
            }

            toolType = toolType.Replace(" ", string.Empty).Replace("_", string.Empty);
            return string.Equals(toolType, "Blob", StringComparison.OrdinalIgnoreCase)
                || string.Equals(toolType, "Contour", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsMultiMatchMeanTool(VisionPipelineStep step)
        {
            return VisionPipelineMultiMatchMeanService.IsMultiMatchMean(
                step?.ToolType);
        }

        private static bool IsGeometryResultTool(VisionPipelineStep step)
        {
            string toolType = (step?.ToolType ?? string.Empty)
                .Replace(" ", string.Empty)
                .Replace("_", string.Empty)
                .Trim();
            return string.Equals(toolType, "Line", StringComparison.OrdinalIgnoreCase)
                || string.Equals(toolType, "LineGauge", StringComparison.OrdinalIgnoreCase)
                || string.Equals(toolType, "CircleGauge", StringComparison.OrdinalIgnoreCase)
                || string.Equals(toolType, "GeometryMeasure", StringComparison.OrdinalIgnoreCase)
                || string.Equals(toolType, "GeometricMeasurement", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsCircleGaugeTool(VisionPipelineStep step)
        {
            string toolType = (step?.ToolType ?? string.Empty)
                .Replace(" ", string.Empty)
                .Replace("_", string.Empty)
                .Trim();
            if (toolType.EndsWith("Tool", StringComparison.OrdinalIgnoreCase))
            {
                toolType = toolType.Substring(0, toolType.Length - 4);
            }

            return string.Equals(toolType, "CircleGauge", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsEdgeBasedMatchingTool(VisionPipelineStep step)
        {
            string toolType = (step?.ToolType ?? string.Empty)
                .Replace(" ", string.Empty)
                .Replace("_", string.Empty)
                .Trim();
            if (toolType.EndsWith("Tool", StringComparison.OrdinalIgnoreCase))
            {
                toolType = toolType.Substring(0, toolType.Length - 4);
            }

            return string.Equals(toolType, "EdgeBasedMatching", StringComparison.OrdinalIgnoreCase)
                || string.Equals(toolType, "EdgeBasedTemplateMatching", StringComparison.OrdinalIgnoreCase)
                || string.Equals(toolType, "EdgeTemplateMatching", StringComparison.OrdinalIgnoreCase);
        }

        private void UpdateFixtureTeachState(VisionPipelineStep step, VisionPipelineStepResultSummary summary)
        {
            if (!VisionPipelineFixtureFrameService.IsProducer(step))
            {
                view.SetFixtureTeachState(false, false, string.Empty);
                return;
            }

            if (OpenVisionPipelineReviewFixturePresenter.TryGetReviewedFixturePose(step, summary, out double x, out double y, out double angle, out double scale)
                && TryGetReferenceImageSize(step, out int referenceWidth, out int referenceHeight))
            {
                view.SetFixtureTeachState(
                    true,
                    true,
                    TF(
                        "PipelineReview.FixtureTeach.ReadyWithDimensionsFormat",
                        "X {0} / Y {1} / {2} deg / scale {3} / reference {4} x {5}. Confirm the reference image.",
                        OpenVisionPipelineReviewFixturePresenter.FormatPoseValue(x),
                        OpenVisionPipelineReviewFixturePresenter.FormatPoseValue(y),
                        OpenVisionPipelineReviewFixturePresenter.FormatPoseValue(angle),
                        OpenVisionPipelineReviewFixturePresenter.FormatPoseValue(scale),
                        referenceWidth,
                        referenceHeight));
                return;
            }

            view.SetFixtureTeachState(
                true,
                false,
                T(
                    "PipelineReview.FixtureTeach.Waiting",
                    "Run Review and verify one Matching result."));
        }

        private void UpdateFixtureDesignerState()
        {
            using OpenVisionPipelineReviewFixtureState state =
                OpenVisionPipelineReviewFixturePresenter.Create(
                    pipeline,
                    step =>
                    {
                        executionController.TryGetSummary(
                            step,
                            out VisionPipelineStepResultSummary summary);
                        return summary;
                    },
                    ResolveLayerPreviewImage,
                    fixtureMeasurementIndex);

            fixtureProducerIndex = state.ProducerIndex;
            fixtureMeasurementIndex = state.MeasurementIndex;
            view.SetFixtureDesignerState(
                state.IsVisible,
                state.RelationshipText,
                state.TemplateText,
                state.ReferenceText,
                state.CurrentText,
                state.QualityText,
                state.SourceText,
                state.SourcePreview,
                state.NormalizedText,
                state.NormalizedPreview,
                state.TemplatePreview,
                state.CanTeachReference,
                state.CanEditProducer,
                state.CanEditMeasurement,
                state.Consumers,
                state.MeasurementIndex);
        }

        private void SaveSelectedMatchingPoseAsReference()
        {
            VisionPipelineStep step = fixtureProducerIndex >= 0
                ? pipeline?.Steps?.ElementAtOrDefault(fixtureProducerIndex)
                : GetSelectedStepOrDefault();
            if (step == null)
            {
                return;
            }

            executionController.TryGetSummary(step, out VisionPipelineStepResultSummary summary);
            if (!OpenVisionPipelineReviewFixturePresenter.TryGetReviewedFixturePose(step, summary, out double x, out double y, out double angle, out double scale)
                || !TryGetReferenceImageSize(step, out int referenceWidth, out int referenceHeight))
            {
                UpdateFixtureTeachState(step, summary);
                return;
            }

            Dictionary<string, string> parameters = step.Parameters;
            string[] keys =
            {
                VisionPipelineFixtureFrameService.ReferenceXParameter,
                VisionPipelineFixtureFrameService.ReferenceYParameter,
                VisionPipelineFixtureFrameService.ReferenceAngleParameter,
                VisionPipelineFixtureFrameService.ReferenceScaleParameter,
                VisionPipelineFixtureFrameService.ReferenceImageWidthParameter,
                VisionPipelineFixtureFrameService.ReferenceImageHeightParameter
            };
            Dictionary<string, string> previousValues = keys
                .Where(parameters.ContainsKey)
                .ToDictionary(key => key, key => parameters[key], StringComparer.OrdinalIgnoreCase);

            parameters[VisionPipelineFixtureFrameService.ReferenceXParameter] = OpenVisionPipelineReviewFixturePresenter.FormatPoseValue(x);
            parameters[VisionPipelineFixtureFrameService.ReferenceYParameter] = OpenVisionPipelineReviewFixturePresenter.FormatPoseValue(y);
            parameters[VisionPipelineFixtureFrameService.ReferenceAngleParameter] = OpenVisionPipelineReviewFixturePresenter.FormatPoseValue(angle);
            parameters[VisionPipelineFixtureFrameService.ReferenceScaleParameter] = OpenVisionPipelineReviewFixturePresenter.FormatPoseValue(scale);
            parameters[VisionPipelineFixtureFrameService.ReferenceImageWidthParameter] = referenceWidth.ToString(CultureInfo.InvariantCulture);
            parameters[VisionPipelineFixtureFrameService.ReferenceImageHeightParameter] = referenceHeight.ToString(CultureInfo.InvariantCulture);

            try
            {
                VisionPipelineStorage.Save(recipeContext.Name, pipeline);
            }
            catch (Exception ex)
            {
                foreach (string key in keys)
                {
                    if (previousValues.TryGetValue(key, out string previousValue))
                    {
                        parameters[key] = previousValue;
                    }
                    else
                    {
                        parameters.Remove(key);
                    }
                }

                view.SetFixtureTeachState(
                    true,
                    true,
                    TF(
                        "PipelineReview.FixtureTeach.SaveFailedFormat",
                        "Could not save reference: {0}",
                        ex.GetBaseException().Message));
                return;
            }

            executionController.Reset();
            reviewExecutionState = T(
                "PipelineReview.Execution.ReferenceChanged",
                "Reference changed / run review required");
            validationResult = VisionPipelineValidator.Validate(pipeline, GetLayerNames());
            view.SetSteps(CreateFlowItems(pipeline.Steps));
            view.SetReviewProgress(FormatReviewProgressText());
            view.SetValidation(FormatValidationStatus(validationResult), FormatValidationDetails(validationResult));
            RefreshReadiness();
            SelectStep(selectedIndex, selectedMode);
            view.SetResultSummary(
                T("PipelineReview.FixtureTeach.RunRequired", "Reference saved"),
                T(
                    "PipelineReview.FixtureTeach.RunRequiredDetail",
                    "The reference changed. Consumer ROI and routing were preserved; click Run Review to refresh every result."));
            view.SetFixtureTeachState(
                true,
                false,
                TF(
                    "PipelineReview.FixtureTeach.SavedWithDimensionsFormat",
                    "Saved X {0} / Y {1} / {2} deg / scale {3} / reference {4} x {5}. ROI kept; run review again.",
                    OpenVisionPipelineReviewFixturePresenter.FormatPoseValue(x),
                    OpenVisionPipelineReviewFixturePresenter.FormatPoseValue(y),
                    OpenVisionPipelineReviewFixturePresenter.FormatPoseValue(angle),
                    OpenVisionPipelineReviewFixturePresenter.FormatPoseValue(scale),
                    referenceWidth,
                    referenceHeight));
        }

        private bool TryGetReferenceImageSize(VisionPipelineStep step, out int width, out int height)
        {
            Bitmap image = ResolveLayerPreviewImage(step?.InputLayer);
            width = image?.Width ?? 0;
            height = image?.Height ?? 0;
            return width > 0 && height > 0;
        }

        private IReadOnlyList<PipelineFlowStepItem> CreateFlowItems(
            IReadOnlyList<VisionPipelineStep> steps)
        {
            return OpenVisionPipelineReviewFlowPresenter.CreateItems(
                steps,
                layerName => ResolveLayerPreviewImage(layerName) != null,
                step => executionController.TryGetSummary(
                    step,
                    out VisionPipelineStepResultSummary summary)
                        ? summary
                        : null);
        }

        private async Task RunReviewAsync()
        {
            if (executionController.IsRunning || pipeline?.Steps == null || pipeline.Steps.Count == 0)
            {
                reviewExecutionState = executionController.IsRunning
                    ? T("PipelineReview.Execution.AlreadyRunning", "Already running")
                    : T("PipelineReview.Execution.NoSteps", "No steps");
                return;
            }

            if (validationResult?.Errors.Count > 0)
            {
                reviewExecutionState = T("PipelineReview.Execution.ValidationErrors", "Validation errors");
                view.SetResultSummary(
                    T("PipelineReview.ValidationError", "Validation error"),
                    T("PipelineReview.FixValidationErrors", "Fix validation errors before running review."));
                view.SetReviewGuide(OpenVisionPipelineReviewGuidePresenter.CreateValidationError(
                    GetSelectedDisplayIndex(),
                    pipeline.Steps.Count,
                    GetSelectedStepOrDefault()));
                return;
            }

            reviewExecutionState = T("PipelineReview.Execution.Started", "Started");
            view.SetRunReviewBusy(true);
            view.SetReviewProgress(T("PipelineReview.Progress.Running", "Running..."));
            view.SetResultSummary(
                T("PipelineReview.RunningSummary", "Running"),
                T("PipelineReview.RunningDetail", "Pipeline review execution in progress."));
            view.SetReviewGuide(OpenVisionPipelineReviewGuidePresenter.CreateRunning(
                GetSelectedDisplayIndex(),
                pipeline.Steps.Count,
                GetSelectedStepOrDefault()));

            try
            {
                executionController.Reset();
                UpdateFixtureTeachState(GetSelectedStepOrDefault(), null);
                view.SetIssueNavigationState(false);
                OpenVisionPipelineReviewExecutionResult runResult = await executionController.RunAsync(
                    pipeline,
                    StepTimeoutMilliseconds);

                await view.Dispatcher.InvokeAsync(() => ApplyReviewRunResult(runResult));
            }
            catch (Exception ex)
            {
                await view.Dispatcher.InvokeAsync(() =>
                {
                    reviewExecutionState = TF("PipelineReview.Execution.FailedFormat", "Failed: {0}", ex.GetBaseException().Message);
                    view.SetResultSummary(T("PipelineReview.RunFailed", "Run failed"), ex.GetBaseException().Message);
                    view.SetValidation(FormatValidationStatus(validationResult), FormatValidationDetails(validationResult));
                });
            }
            finally
            {
                await view.Dispatcher.InvokeAsync(() =>
                {
                    view.SetRunReviewBusy(false);
                    view.SetReviewProgress(FormatReviewProgressText());
                });
            }
        }

        private void OnReviewStepExecutionUpdated(
            object sender,
            OpenVisionPipelineReviewStepUpdatedEventArgs e)
        {
            if (!view.Dispatcher.CheckAccess())
            {
                view.Dispatcher.Invoke(() => OnReviewStepExecutionUpdated(sender, e));
                return;
            }

            VisionPipelineStep updatedStep = e?.Step;
            if (updatedStep != null)
            {
                view.SetSteps(CreateFlowItems(pipeline.Steps));
                view.SetReviewProgress(FormatReviewProgressText());
                view.SetIssueNavigationState(FindFirstIssueStepIndex() >= 0);
                if (ReferenceEquals(updatedStep, pipeline.Steps.ElementAtOrDefault(selectedIndex)))
                {
                    SelectStep(selectedIndex, selectedMode);
                }
            }
        }

        private void ApplyReviewRunResult(OpenVisionPipelineReviewExecutionResult runResult)
        {
            reviewExecutionState = TF("PipelineReview.Execution.CompletedFormat", "Completed / {0} step results", runResult?.StepResultCount ?? 0);
            view.SetSteps(CreateFlowItems(pipeline.Steps));
            view.SetReviewProgress(FormatReviewProgressText());
            SelectStep(selectedIndex < 0 ? 0 : selectedIndex, selectedMode);
        }

        private int FindFirstIssueStepIndex()
        {
            if (pipeline?.Steps == null)
            {
                return -1;
            }

            for (int i = 0; i < pipeline.Steps.Count; i++)
            {
                VisionPipelineStep step = pipeline.Steps[i];
                if (step == null || step.Enabled == false)
                {
                    continue;
                }

                if (executionController.TryGetSummary(step, out VisionPipelineStepResultSummary summary)
                    && summary?.Success == false)
                {
                    return i;
                }
            }

            return -1;
        }

        private Bitmap ResolveLayerPreviewImage(string layerName)
        {
            if (string.IsNullOrWhiteSpace(layerName))
            {
                return null;
            }

            Bitmap displayImage = displayManager.GetLayerImage(layerName);
            if (displayImage != null && !DisplayManagerImageExtensions.IsPlaceholderBitmap(displayImage))
            {
                return displayImage;
            }

            return executionController.ResolveCachedOutput(layerName);
        }

        private Bitmap ResolveStepOutputPreviewImage(string layerName)
        {
            Bitmap reviewImage = executionController.ResolveCachedOutput(layerName);
            return reviewImage ?? ResolveLayerPreviewImage(layerName);
        }

        private void RefreshReadiness()
        {
            view.SetReadiness(OpenVisionPipelineReviewReadinessPresenter.Create(
                pipeline,
                validationResult,
                layerName => ResolveLayerPreviewImage(layerName) != null,
                activeSamplePairGuide?.HasGuide == true,
                activePairCounterpartSample?.CanOpen == true));
        }

        private string FormatReviewProgressText()
        {
            if (pipeline?.Steps == null || pipeline.Steps.Count == 0)
            {
                return T("PipelineReview.Progress.NoSteps", "No steps");
            }

            int okCount = 0;
            int ngCount = 0;
            int skippedCount = 0;
            foreach (VisionPipelineStep step in pipeline.Steps)
            {
                if (step?.Enabled == false)
                {
                    skippedCount++;
                    continue;
                }

                if (!executionController.TryGetSummary(step, out VisionPipelineStepResultSummary summary))
                {
                    continue;
                }

                if (summary.Success && !summary.IsAcceptanceNg)
                {
                    okCount++;
                }
                else
                {
                    ngCount++;
                }
            }

            int reviewableCount = pipeline.Steps.Count(step => step?.Enabled != false);
            int waitCount = Math.Max(0, reviewableCount - okCount - ngCount);
            if (okCount == 0 && ngCount == 0 && waitCount == reviewableCount && !executionController.IsRunning)
            {
                return T("PipelineReview.Progress.NotRun", "Not run");
            }

            string progress = TF("PipelineReview.Progress.CountsFormat", "OK {0} / NG {1} / WAIT {2}", okCount, ngCount, waitCount);
            if (skippedCount > 0)
            {
                progress = string.Format(
                    CultureInfo.CurrentCulture,
                    "{0} / {1}",
                    progress,
                    TF("PipelineReview.Progress.OffFormat", "OFF {0}", skippedCount));
            }

            return executionController.IsRunning
                ? string.Format(CultureInfo.CurrentCulture, "{0} / {1}", T("PipelineReview.Progress.Running", "Running..."), progress)
                : progress;
        }

        private int GetSelectedDisplayIndex()
        {
            return selectedIndex < 0 ? 0 : selectedIndex + 1;
        }

        private VisionPipelineStep GetSelectedStepOrDefault()
        {
            return pipeline?.Steps?.ElementAtOrDefault(selectedIndex);
        }

        private static string FormatParameters(VisionPipelineStep step)
        {
            if (step?.Parameters == null || step.Parameters.Count == 0)
            {
                return "-";
            }

            return string.Join(
                Environment.NewLine,
                step.Parameters
                    .OrderBy(parameter => parameter.Key, StringComparer.OrdinalIgnoreCase)
                    .Take(12)
                    .Select(parameter => string.Format(CultureInfo.CurrentCulture, "{0}: {1}", parameter.Key, parameter.Value)));
        }

        private List<string> GetLayerNames()
        {
            return displayManager.GetLayerInfos()
                .Select(layer => layer.Title)
                .Where(title => !string.IsNullOrWhiteSpace(title))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        private static string FormatValidationStatus(VisionPipelineValidationResult result)
        {
            if (result == null)
            {
                return "NOT RUN";
            }

            if (result.Errors.Count > 0)
            {
                return string.Format(CultureInfo.CurrentCulture, "ERROR: {0}", result.Errors.Count);
            }

            if (result.Warnings.Count > 0)
            {
                return string.Format(CultureInfo.CurrentCulture, "REVIEW: {0}", result.Warnings.Count);
            }

            return "OK";
        }

        private static string FormatValidationDetails(VisionPipelineValidationResult result)
        {
            if (result == null)
            {
                return "-";
            }

            List<string> lines = new List<string>();
            if (result.Errors.Count > 0)
            {
                lines.AddRange(result.Errors.Take(4).Select(message => T("PipelineReview.Validation.ErrorPrefix", "Error") + ": " + FormatValidationIssue(message)));
            }

            if (result.Warnings.Count > 0)
            {
                lines.AddRange(result.Warnings.Take(5).Select(FormatValidationIssue));
            }

            return lines.Count == 0
                ? T("PipelineReview.Validation.Valid", "Pipeline structure looks valid.")
                : string.Join(Environment.NewLine, lines);
        }

        private static string FormatValidationIssue(string message)
        {
            string text = SafeText(message, "-");
            if (text.IndexOf("Review branch input", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                string input = ExtractQuotedValueAfter(text, "reads '");
                string output = ExtractQuotedValueAfter(text, "outputs '");
                if (!string.IsNullOrWhiteSpace(input) && !string.IsNullOrWhiteSpace(output))
                {
                    return TF("PipelineReview.Validation.BranchInputFormat", "Review branch input: {0} -> {1}.", input, output);
                }
            }

            int keepUntil = text.IndexOf(" Keep this only", StringComparison.OrdinalIgnoreCase);
            if (keepUntil > 0)
            {
                text = text.Substring(0, keepUntil).TrimEnd();
            }

            const int MaxLength = 150;
            return text.Length <= MaxLength ? text : text.Substring(0, MaxLength - 3) + "...";
        }

        private static string ExtractQuotedValueAfter(string text, string marker)
        {
            int start = text.IndexOf(marker, StringComparison.OrdinalIgnoreCase);
            if (start < 0)
            {
                return string.Empty;
            }

            start += marker.Length;
            int end = text.IndexOf("'", start, StringComparison.Ordinal);
            return end <= start ? string.Empty : text.Substring(start, end - start);
        }

        private static string FormatStepName(int index, VisionPipelineStep step)
        {
            string name = SafeText(step?.Name, step?.ToolType);
            string ordinal = (index + 1).ToString("00", CultureInfo.CurrentCulture);
            if (name.StartsWith(ordinal + " ", StringComparison.OrdinalIgnoreCase))
            {
                name = name.Substring(ordinal.Length).TrimStart();
            }

            return string.Format(CultureInfo.CurrentCulture, "{0:00}  {1}", index + 1, name);
        }

        private static string SafeText(string value, string fallback)
        {
            return string.IsNullOrWhiteSpace(value) ? fallback : value.Trim();
        }

        private static string T(string key, string fallbackText)
        {
            string value = OpenVisionLanguageService.T(key);
            return string.IsNullOrWhiteSpace(value) || string.Equals(value, key, StringComparison.Ordinal)
                ? fallbackText ?? string.Empty
                : value;
        }

        private static string TF(string key, string fallbackFormat, params object[] args)
        {
            return string.Format(CultureInfo.CurrentCulture, T(key, fallbackFormat), args);
        }

    }

    internal sealed class OpenVisionPipelineReviewSampleOpenRequestedEventArgs : EventArgs
    {
        public OpenVisionPipelineReviewSampleOpenRequestedEventArgs(string sampleName)
        {
            SampleName = sampleName ?? string.Empty;
        }

        public string SampleName { get; }
    }
}
