using OpenVisionLab.Core;
using OpenVisionLab.Pipeline.Controls;
using OpenVisionLab.Vision2D.Pipeline;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace OpenVisionLab
{
    internal sealed class OpenVisionPipelineReviewDocument : IDisposable
    {
        #region Core

        private const int StepTimeoutMilliseconds = 60000;
        private readonly IDisplayManager displayManager;
        private readonly OpenVisionRecipeContext recipeContext;
        private readonly OpenVisionPipelineReviewView view;
        private readonly OpenVisionPipelineReviewExecutionController executionController;
        private readonly OpenVisionPipelineReviewLayerImageOwner layerImageOwner;
        private readonly OpenVisionPipelineReviewDocumentRevisionGate revisionGate = new OpenVisionPipelineReviewDocumentRevisionGate();
        private readonly OpenVisionPipelineReviewResultStatusProjectionOwner resultStatusProjectionOwner = new OpenVisionPipelineReviewResultStatusProjectionOwner();
        private readonly OpenVisionPipelineReviewGuideResultProjectionOwner guideResultProjectionOwner = new OpenVisionPipelineReviewGuideResultProjectionOwner();
        private readonly OpenVisionPipelineReviewDomainEvidenceProjectionOwner domainEvidenceProjectionOwner = new OpenVisionPipelineReviewDomainEvidenceProjectionOwner();
        private VisionPipeline pipeline;
        private VisionPipelineValidationResult validationResult;
        private OpenVisionWorkspaceSamplePairDecisionGuide activeSamplePairGuide = OpenVisionWorkspaceSamplePairDecisionGuide.Empty;
        private VisionPipelineSampleCatalogItem activeCatalogSample;
        private VisionPipelineSampleCatalogItem activePairCounterpartSample;
        private string activePipelineName = string.Empty;
        private DateTime activePipelineLastWriteUtc;
        private int selectedIndex;
        private PipelineFlowPreviewMode selectedMode = PipelineFlowPreviewMode.Overlay;
        private string reviewExecutionState = string.Empty;
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
            layerImageOwner = new OpenVisionPipelineReviewLayerImageOwner(
                displayManager,
                (stepIndex, layerName) => executionController.AcquireCachedOutputSnapshot(stepIndex, layerName),
                layerName => executionController.AcquireCachedOutputSnapshot(layerName));
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
            revisionGate.InvalidateRecipe();
            activePipelineName = ResolveActivePipelineName();
            pipeline = VisionPipelineStorage.Load(recipeContext.Name, activePipelineName);
            activePipelineLastWriteUtc = GetPipelineLastWriteUtc(activePipelineName);
            RefreshActiveSamplePairGuide(activePipelineName);
            validationResult = VisionPipelineValidator.Validate(pipeline, GetLayerNames());
            executionController.Reset();
            ApplyReviewResultStatus(
                executionController.IsStopping
                    ? OpenVisionPipelineReviewResultStatusKind.Draining
                    : OpenVisionPipelineReviewResultStatusKind.NotRun);
            int stepCount = pipeline?.Steps?.Count ?? 0;
            view.SetRecipeContext(recipeContext.Name);
            view.SetPipelineHeader(activePipelineName, stepCount);
            view.SetReviewProgress(ProjectReviewProgressText());
            view.SetValidation(FormatValidationStatus(validationResult), FormatValidationDetails(validationResult));
            RefreshReadiness();
            ApplyReviewResultStatus(OpenVisionPipelineReviewResultStatusKind.RunRequired);

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

            revisionGate.InvalidateInput();
            executionController.Reset();
            ApplyReviewResultStatus(
                executionController.IsStopping
                    ? OpenVisionPipelineReviewResultStatusKind.Draining
                    : OpenVisionPipelineReviewResultStatusKind.NotRun);
            validationResult = VisionPipelineValidator.Validate(pipeline, GetLayerNames());
            view.SetReviewProgress(ProjectReviewProgressText());
            view.SetValidation(FormatValidationStatus(validationResult), FormatValidationDetails(validationResult));
            RefreshReadiness();
            view.SetSteps(CreateFlowItems(pipeline.Steps));
            SelectStep(
                selectedIndex >= 0 && selectedIndex < pipeline.Steps.Count ? selectedIndex : 0,
                selectedMode);
        }

        public bool RefreshIfPipelineChanged()
        {
            string resolvedPipelineName = ResolveActivePipelineName();
            if (!string.Equals(activePipelineName, resolvedPipelineName, StringComparison.Ordinal)
                || activePipelineLastWriteUtc != GetPipelineLastWriteUtc(resolvedPipelineName))
            {
                RefreshLayerState();
                return true;
            }

            return false;
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
            revisionGate.Dispose();
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
            using Bitmap inputImage = layerImageOwner.AcquirePreview(step.InputLayer);
            using Bitmap outputImage = layerImageOwner.AcquireOutputPreview(index, step.OutputLayer);
            executionController.TryGetSummary(step, out VisionPipelineStepResultSummary summary);
            OpenVisionPipelineReviewFlowProjection flow =
                OpenVisionPipelineReviewFlowPresenter.CreateStepProjection(
                    pipeline.Steps,
                    index,
                    inputImage != null,
                    outputImage != null,
                    summary);
            OpenVisionPipelineReviewGuideResultProjection guideResultProjection =
                guideResultProjectionOwner.ProjectSelected(new OpenVisionPipelineReviewGuideResultProjectionRequest
                {
                    DisplayIndex = index + 1,
                    StepCount = pipeline.Steps.Count,
                    Step = step,
                    StatusText = flow.StatusText,
                    HasInputImage = inputImage != null,
                    HasOutputImage = outputImage != null,
                    Summary = summary,
                    ValidationResult = validationResult,
                    ExpectedInputLayer = flow.ExpectedInputLayer,
                    IsBranch = flow.IsBranch,
                    InputWillBeProduced = flow.InputWillBeProduced,
                    SamplePairGuide = activeSamplePairGuide,
                    ActiveCatalogSample = activeCatalogSample,
                    ActivePairCounterpartSample = activePairCounterpartSample,
                    InputImage = inputImage,
                    OutputImage = outputImage,
                    PreviewMode = mode,
                    ValidationStatusText = FormatValidationStatus(validationResult)
                });

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
                guideResultProjection.RunLogText);
            view.SetResultSummary(
                guideResultProjection.ResultSummaryText,
                guideResultProjection.ResultDetailText);
            OpenVisionPipelineReviewDomainEvidenceProjection domainEvidenceProjection =
                domainEvidenceProjectionOwner.Project(step, summary);
            view.SetObjectResults(
                domainEvidenceProjection.SupportsObjectResults,
                step,
                domainEvidenceProjection.ObjectResults,
                inputImage,
                outputImage);
            view.SetInstanceResults(
                domainEvidenceProjection.SupportsInstanceResults,
                domainEvidenceProjection.InstanceResults);
            view.SetGeometryResults(
                domainEvidenceProjection.SupportsGeometryResults,
                domainEvidenceProjection.GeometryResults);
            view.SetCircleEvidence(
                domainEvidenceProjection.SupportsCircleEvidence,
                domainEvidenceProjection.CircleEvidence,
                inputImage,
                outputImage);
            view.SetMatcherDiagnostics(
                domainEvidenceProjection.SupportsMatcherDiagnostics,
                domainEvidenceProjection.MatcherDiagnostics,
                domainEvidenceProjection.Metrics,
                inputImage);
            view.SetReviewGuide(guideResultProjection.GuideState);
            view.SetReviewGuidePairAction(
                guideResultProjection.PairActionText,
                guideResultProjection.CanOpenPairAction);
            view.SetReviewGuidePairMetric(guideResultProjection.PairMetricText);
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
            using Bitmap coordinateImage = layerImageOwner.AcquirePreview(coordinateLayer);
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
                    layerImageOwner.AcquirePreview,
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
            validationResult = VisionPipelineValidator.Validate(pipeline, GetLayerNames());
            view.SetSteps(CreateFlowItems(pipeline.Steps));
            view.SetReviewProgress(ProjectReviewProgressText());
            view.SetValidation(FormatValidationStatus(validationResult), FormatValidationDetails(validationResult));
            RefreshReadiness();
            SelectStep(selectedIndex, selectedMode);
            ApplyReviewResultStatus(OpenVisionPipelineReviewResultStatusKind.ReferenceChanged);
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
            using Bitmap image = layerImageOwner.AcquirePreview(step?.InputLayer);
            width = image?.Width ?? 0;
            height = image?.Height ?? 0;
            return width > 0 && height > 0;
        }

        private IReadOnlyList<PipelineFlowStepItem> CreateFlowItems(
            IReadOnlyList<VisionPipelineStep> steps)
        {
            return OpenVisionPipelineReviewFlowPresenter.CreateItems(
                steps,
                layerName => layerImageOwner.HasPreview(layerName),
                step => executionController.TryGetSummary(
                    step,
                    out VisionPipelineStepResultSummary summary)
                        ? summary
                        : null);
        }

        private async Task RunReviewAsync()
        {
            if (disposed)
            {
                return;
            }

            if (executionController.IsRunning)
            {
                ApplyReviewResultStatus(
                    OpenVisionPipelineReviewResultStatusKind.AlreadyRunning);
                return;
            }

            if (TryGetPersistenceExecutionBlock(
                    out string persistenceStatus,
                    out string persistenceDetails))
            {
                ApplyReviewResultStatus(OpenVisionPipelineReviewResultStatusKind.ValidationErrors);
                view.SetValidation(persistenceStatus, persistenceDetails);
                view.SetReviewGuide(guideResultProjectionOwner.ProjectValidationErrorGuide(
                    GetSelectedDisplayIndex(),
                    pipeline?.Steps?.Count ?? 0,
                    GetSelectedStepOrDefault()));
                return;
            }

            if (pipeline?.Steps == null || pipeline.Steps.Count == 0)
            {
                ApplyReviewResultStatus(
                    executionController.IsStopping
                        ? OpenVisionPipelineReviewResultStatusKind.Draining
                        : OpenVisionPipelineReviewResultStatusKind.NoSteps);
                return;
            }

            if (validationResult?.Errors.Count > 0)
            {
                ApplyReviewResultStatus(OpenVisionPipelineReviewResultStatusKind.ValidationErrors);
                view.SetReviewGuide(guideResultProjectionOwner.ProjectValidationErrorGuide(
                    GetSelectedDisplayIndex(),
                    pipeline.Steps.Count,
                    GetSelectedStepOrDefault()));
                return;
            }

            OpenVisionPipelineReviewDocumentRevision runRevision = revisionGate.BeginRun();
            ApplyReviewResultStatus(OpenVisionPipelineReviewResultStatusKind.Started);
            view.SetRunReviewBusy(true);
            view.SetReviewProgress(resultStatusProjectionOwner.ProjectRunningProgressText());
            view.SetReviewGuide(guideResultProjectionOwner.ProjectRunningGuide(
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
                    StepTimeoutMilliseconds,
                    runRevision.InputRevision,
                    runRevision.RecipeRevision);

                if (runResult.WasSuperseded)
                {
                    InvokeOnViewDispatcher(() =>
                    {
                        if (revisionGate.IsCurrent(runRevision))
                        {
                            ApplyReviewResultStatus(OpenVisionPipelineReviewResultStatusKind.Superseded);
                        }
                    });
                }
                else
                {
                    InvokeOnViewDispatcher(() => ApplyReviewRunResult(runRevision, runResult));
                }
            }
            catch (Exception ex)
            {
                InvokeOnViewDispatcher(() =>
                {
                    if (!revisionGate.IsCurrent(runRevision))
                    {
                        return;
                    }

                    ApplyReviewResultStatus(
                        OpenVisionPipelineReviewResultStatusKind.Failed,
                        errorMessage: ex.GetBaseException().Message);
                    view.SetValidation(FormatValidationStatus(validationResult), FormatValidationDetails(validationResult));
                });
            }
            finally
            {
                InvokeOnViewDispatcher(ApplyRunReviewIdleState);
            }
        }

        private bool TryGetPersistenceExecutionBlock(
            out string status,
            out string details)
        {
            List<string> compactStatuses = new List<string>();
            List<string> helpTexts = new List<string>();

            if (VisionPipelineStorage.TryGetPersistenceState(
                    recipeContext.Name,
                    activePipelineName,
                    out VisionPipelinePersistenceState pipelineState)
                && pipelineState?.IsFailure == true)
            {
                compactStatuses.Add(
                    OpenVisionRecipePersistenceStatusPresenter
                        .CreateCompactText(pipelineState));
                helpTexts.Add(
                    OpenVisionRecipePersistenceStatusPresenter
                        .CreateHelpText(pipelineState));
            }

            if (RecipeDataStorage.TryGetPersistenceState(
                    recipeContext.Name,
                    out RecipeDataPersistenceState recipeDataState)
                && recipeDataState?.IsFailure == true)
            {
                compactStatuses.Add(
                    OpenVisionRecipePersistenceStatusPresenter
                        .CreateCompactText(recipeDataState));
                helpTexts.Add(
                    OpenVisionRecipePersistenceStatusPresenter
                        .CreateHelpText(recipeDataState));
            }

            if (compactStatuses.Count == 0)
            {
                status = string.Empty;
                details = string.Empty;
                return false;
            }

            status = OpenVisionRecipeText.Local(
                "차단: 저장 복구를 먼저 완료하세요.",
                "Blocked: complete persistence recovery before running.")
                + Environment.NewLine
                + string.Join(Environment.NewLine, compactStatuses);
            details = string.Join(
                Environment.NewLine + Environment.NewLine,
                helpTexts.Where(text => !string.IsNullOrWhiteSpace(text)));
            return true;
        }

        private void OnReviewStepExecutionUpdated(
            object sender,
            OpenVisionPipelineReviewStepUpdatedEventArgs e)
        {
            if (disposed || view == null)
            {
                return;
            }

            if (!view.Dispatcher.CheckAccess())
            {
                InvokeOnViewDispatcher(() => OnReviewStepExecutionUpdated(sender, e));
                return;
            }

            if (disposed
                || pipeline?.Steps == null
                || e == null
                || !revisionGate.IsCurrentRevision(e.InputRevision, e.RecipeRevision))
            {
                return;
            }

            VisionPipelineStep updatedStep = e.Step;
            if (updatedStep != null)
            {
                view.SetSteps(CreateFlowItems(pipeline.Steps));
                view.SetReviewProgress(ProjectReviewProgressText());
                view.SetIssueNavigationState(FindFirstIssueStepIndex() >= 0);
                if (ReferenceEquals(updatedStep, pipeline.Steps.ElementAtOrDefault(selectedIndex)))
                {
                    SelectStep(selectedIndex, selectedMode);
                }
            }
        }

        private void ApplyReviewRunResult(
            OpenVisionPipelineReviewDocumentRevision revision,
            OpenVisionPipelineReviewExecutionResult runResult)
        {
            if (!revisionGate.IsCurrent(revision))
            {
                return;
            }

            ApplyReviewResultStatus(
                OpenVisionPipelineReviewResultStatusKind.Completed,
                stepResultCount: runResult?.StepResultCount ?? 0);
            view.SetSteps(CreateFlowItems(pipeline.Steps));
            view.SetReviewProgress(ProjectReviewProgressText());
            SelectStep(selectedIndex < 0 ? 0 : selectedIndex, selectedMode);
        }

        private void ApplyRunReviewIdleState()
        {
            if (disposed || executionController.IsRunning || executionController.IsStopping)
            {
                return;
            }

            view.SetRunReviewBusy(false);
            view.SetReviewProgress(ProjectReviewProgressText());
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
                    && summary?.Executed == true
                    && summary.Success == false)
                {
                    return i;
                }
            }

            return -1;
        }

        private void RefreshReadiness()
        {
            view.SetReadiness(OpenVisionPipelineReviewReadinessPresenter.Create(
                pipeline,
                validationResult,
                layerName => layerImageOwner.HasPreview(layerName),
                activeSamplePairGuide?.HasGuide == true,
                activePairCounterpartSample?.CanOpen == true));
        }

        private void ApplyReviewResultStatus(
            OpenVisionPipelineReviewResultStatusKind kind,
            int stepResultCount = 0,
            string errorMessage = null)
        {
            OpenVisionPipelineReviewResultStatusProjection projection = resultStatusProjectionOwner.Project(
                kind,
                stepResultCount,
                errorMessage);
            reviewExecutionState = projection.ExecutionStateText;
            if (!string.IsNullOrWhiteSpace(projection.ResultSummaryText))
            {
                view.SetResultSummary(
                    projection.ResultSummaryText,
                    projection.ResultDetailText);
            }
        }

        private string ProjectReviewProgressText()
        {
            return resultStatusProjectionOwner.ProjectProgress(
                pipeline?.Steps,
                ResolveReviewSummary,
                executionController.IsRunning,
                executionController.IsStopping);
        }

        private VisionPipelineStepResultSummary ResolveReviewSummary(VisionPipelineStep step)
        {
            return executionController.TryGetSummary(
                    step,
                    out VisionPipelineStepResultSummary summary)
                ? summary
                : null;
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

        #endregion

        #region Event Handlers

        private void OnStepSelected(object sender, PipelineFlowStepSelectedEventArgs e)
        {
            SelectStep(e.Index, e.Mode);
        }

        private async void OnRunReviewRequested(object sender, EventArgs e)
        {
            if (disposed)
            {
                return;
            }

            await RunReviewAsync();
        }

        private void InvokeOnViewDispatcher(Action action)
        {
            if (action == null || disposed)
            {
                return;
            }

            Dispatcher dispatcher = view.Dispatcher;
            if (dispatcher.HasShutdownStarted || dispatcher.HasShutdownFinished)
            {
                return;
            }

            if (dispatcher.CheckAccess())
            {
                if (!disposed)
                {
                    action();
                }

                return;
            }

            try
            {
                dispatcher.Invoke(action);
            }
            catch (InvalidOperationException)
                when (dispatcher.HasShutdownStarted || dispatcher.HasShutdownFinished || disposed)
            {
            }
            catch (OperationCanceledException)
                when (dispatcher.HasShutdownStarted || dispatcher.HasShutdownFinished || disposed)
            {
            }
        }

        private void OnPreviousStepRequested(object sender, EventArgs e)
        {
            if (selectedIndex > 0)
            {
                SelectStep(selectedIndex - 1, selectedMode);
            }
        }

        private void OnNextStepRequested(object sender, EventArgs e)
        {
            int stepCount = pipeline?.Steps?.Count ?? 0;
            if (selectedIndex >= 0 && selectedIndex < stepCount - 1)
            {
                SelectStep(selectedIndex + 1, selectedMode);
            }
        }

        private void OnFirstIssueStepRequested(object sender, EventArgs e)
        {
            SelectFirstIssueStep();
        }

        private void SelectFirstIssueStep()
        {
            int issueIndex = FindFirstIssueStepIndex();
            if (issueIndex >= 0)
            {
                SelectStep(issueIndex, selectedMode);
            }
        }

        private void OnOpenPairSampleRequested(object sender, EventArgs e)
        {
            RequestOpenPairSample();
        }

        private void OnUseSelectedMatchingPoseRequested(object sender, EventArgs e)
        {
            SaveSelectedMatchingPoseAsReference();
        }

        private void OnReturnToRecipeRequested(object sender, EventArgs e)
        {
            ReturnToRecipeRequested(this, EventArgs.Empty);
        }

        private void OnOpenSelectedToolLearnRequested(object sender, EventArgs e)
        {
            if (OpenVisionLearnTopicCatalog.TryResolveForToolType(SelectedToolType, out _))
            {
                OpenSelectedToolLearnRequested(this, EventArgs.Empty);
            }
        }

        private void OnEditSelectedStepRequested(object sender, EventArgs e)
        {
            EditSelectedStepRequested(this, EventArgs.Empty);
        }

        private void OnEditFixtureProducerRequested(object sender, EventArgs e)
        {
            RequestStepEdit(fixtureProducerIndex);
        }

        private void OnEditFixtureMeasurementRequested(object sender, EventArgs e)
        {
            RequestStepEdit(fixtureMeasurementIndex);
        }

        private void OnFixtureConsumerSelected(
            object sender,
            OpenVisionPipelineReviewFixtureConsumerSelectedEventArgs e)
        {
            if (e == null
                || e.StepIndex == fixtureMeasurementIndex
                || pipeline?.Steps?.ElementAtOrDefault(e.StepIndex) == null)
            {
                return;
            }

            fixtureMeasurementIndex = e.StepIndex;
            UpdateFixtureDesignerState();
        }

        private void OnScaleCalibrationRequested(object sender, VisionScaleCalibrationRequestedEventArgs e)
        {
            IReadOnlyList<VisionPipelineGeometryFeatureResult> points = executionController
                .GetCurrentGeometryFeatures()
                .Where(item => item.Kind == VisionPipelineGeometryKind.Point)
                .ToList();
            VisionPipelineGeometryFeatureResult pointA = points.FirstOrDefault(item =>
                string.Equals(item.Identity, e?.PointAIdentity, StringComparison.OrdinalIgnoreCase));
            VisionPipelineGeometryFeatureResult pointB = points.FirstOrDefault(item =>
                string.Equals(item.Identity, e?.PointBIdentity, StringComparison.OrdinalIgnoreCase));
            using Bitmap coordinateImage = layerImageOwner.AcquirePreview(pointA?.CoordinateLayer);

            if (!VisionPipelineScaleCalibrationStorage.TryCalculate(
                    activePipelineName,
                    pointA,
                    pointB,
                    e?.KnownDistance ?? 0D,
                    e?.Unit ?? VisionScaleCalibrationUnit.Millimeter,
                    coordinateImage,
                    out VisionPipelineScaleCalibrationRecord record,
                    out string error)
                || !VisionPipelineScaleCalibrationStorage.TrySave(
                    recipeContext.Name,
                    record,
                    out string evidencePath,
                    out error))
            {
                view.SetScaleCalibrationStatus("Scale evidence was not saved: " + error);
                return;
            }

            UpdateScaleCalibrationState(
                "Saved exact two-point evidence: " + evidencePath + ". Apply remains explicit; no Preview/Run occurred.");
        }

        private void OnScaleCalibrationApplyRequested(object sender, VisionScaleCalibrationApplyRequestedEventArgs e)
        {
            if (pipeline?.Steps == null || e == null || e.StepIndex < 0 || e.StepIndex >= pipeline.Steps.Count)
            {
                view.SetScaleCalibrationStatus("Select one compatible target Step.");
                return;
            }

            if (!VisionPipelineScaleCalibrationStorage.TryLoad(
                    recipeContext.Name,
                    activePipelineName,
                    out VisionPipelineScaleCalibrationRecord record,
                    out string error))
            {
                view.SetScaleCalibrationStatus("Scale was not applied: " + error);
                return;
            }

            VisionPipelineStep target = pipeline.Steps[e.StepIndex];
            using Bitmap coordinateImage = layerImageOwner.AcquirePreview(record.CoordinateLayer);
            if (!VisionPipelineScaleCalibrationStorage.TryApply(record, coordinateImage, target, out error))
            {
                view.SetScaleCalibrationStatus("Scale was not applied: " + error);
                return;
            }

            try
            {
                VisionPipelineStorage.Save(recipeContext.Name, pipeline);
                if (!VisionPipelineStorage.TryValidateRoundTrip(recipeContext.Name, pipeline, out string roundTripMessage))
                {
                    view.SetScaleCalibrationStatus("Scale pipeline save did not verify: " + roundTripMessage);
                    return;
                }

                if (!VisionPipelineScaleCalibrationStorage.TrySave(
                        recipeContext.Name,
                        record,
                        out string evidencePath,
                        out error))
                {
                    view.SetScaleCalibrationStatus("Scale was applied, but its applied-Step audit did not save: " + error);
                    return;
                }

                validationResult = VisionPipelineValidator.Validate(pipeline, GetLayerNames());
                view.SetValidation(FormatValidationStatus(validationResult), FormatValidationDetails(validationResult));
                SelectStep(selectedIndex, selectedMode);
                view.SetScaleCalibrationStatus(
                    $"Applied {record.MillimetersPerPixel:0.############} mm/px to '{target.Name}' only. Pipeline and {evidencePath} round-tripped; no Preview/Run occurred.");
            }
            catch (Exception ex)
            {
                view.SetScaleCalibrationStatus("Scale apply failed: " + ex.GetBaseException().Message);
            }
        }

        private void RequestStepEdit(int index)
        {
            if (pipeline?.Steps == null || index < 0 || index >= pipeline.Steps.Count)
            {
                return;
            }

            SelectStep(index, PipelineFlowPreviewMode.Overlay);
            EditSelectedStepRequested(this, EventArgs.Empty);
        }

        public bool OpenPairSampleForTest()
        {
            return RequestOpenPairSample();
        }

        private bool RequestOpenPairSample()
        {
            if (activePairCounterpartSample?.CanOpen != true
                || string.IsNullOrWhiteSpace(activePairCounterpartSample.SampleName))
            {
                return false;
            }

            OpenWorkspaceSampleRequested(
                this,
                new OpenVisionPipelineReviewSampleOpenRequestedEventArgs(activePairCounterpartSample.SampleName));
            return true;
        }

        private void OnLanguageChanged(object sender, EventArgs e)
        {
            if (disposed)
            {
                return;
            }

            if (!view.Dispatcher.CheckAccess())
            {
                view.Dispatcher.Invoke(RefreshLocalizedDisplay);
                return;
            }

            RefreshLocalizedDisplay();
        }

        private void RefreshLocalizedDisplay()
        {
            activePipelineName = ResolveActivePipelineName();
            int stepCount = pipeline?.Steps?.Count ?? 0;
            RefreshActiveSamplePairGuide(activePipelineName);
            validationResult = VisionPipelineValidator.Validate(pipeline, GetLayerNames());
            view.SetRecipeContext(recipeContext.Name);
            view.SetPipelineHeader(activePipelineName, stepCount);
            view.SetReviewProgress(ProjectReviewProgressText());
            view.SetValidation(FormatValidationStatus(validationResult), FormatValidationDetails(validationResult));
            RefreshReadiness();

            if (stepCount == 0)
            {
                selectedIndex = -1;
                view.SetEmptyState(activePipelineName);
                view.SetValidation(FormatValidationStatus(validationResult), FormatValidationDetails(validationResult));
                return;
            }

            int preservedIndex = view.SelectedFlowIndex >= 0 ? view.SelectedFlowIndex : selectedIndex;
            PipelineFlowPreviewMode preservedMode = selectedMode;
            view.SetSteps(CreateFlowItems(pipeline.Steps));
            selectedMode = preservedMode;
            if (preservedIndex >= 0 && preservedIndex < stepCount)
            {
                selectedIndex = preservedIndex;
            }
            else if (selectedIndex < 0 || selectedIndex >= stepCount)
            {
                selectedIndex = 0;
            }

            SelectStep(selectedIndex, selectedMode);
        }

        #endregion
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
