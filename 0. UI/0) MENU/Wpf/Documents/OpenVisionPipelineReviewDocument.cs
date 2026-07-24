using Lib.OpenCV.Pipeline;
using OpenVisionLab._1._Core;
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
    internal sealed class OpenVisionPipelineReviewDocument : IDisposable
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
        private int selectedIndex;
        private PipelineFlowPreviewMode selectedMode = PipelineFlowPreviewMode.Overlay;
        private string reviewExecutionState = T("PipelineReview.Execution.NotRun", "Not run");
        private int fixtureProducerIndex = -1;
        private int fixtureNormalizeIndex = -1;
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
        public int ObjectResultCount => view.ObjectResultCount;
        public int SelectedObjectResultNumber => view.SelectedObjectResultNumber;
        public bool HasObjectHighlight => view.HasObjectHighlight;
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

        private void OnStepSelected(object sender, PipelineFlowStepSelectedEventArgs e)
        {
            SelectStep(e.Index, e.Mode);
        }

        private async void OnRunReviewRequested(object sender, EventArgs e)
        {
            await RunReviewAsync();
        }

        private void InvokeOnViewDispatcher(Action action)
        {
            if (action == null)
            {
                return;
            }

            if (view.Dispatcher.CheckAccess())
            {
                action();
                return;
            }

            view.Dispatcher.Invoke(action);
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
            Bitmap coordinateImage = ResolveLayerPreviewImage(pointA?.CoordinateLayer);

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
            Bitmap coordinateImage = ResolveLayerPreviewImage(record.CoordinateLayer);
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
            view.SetReviewProgress(FormatReviewProgressText());
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
            Bitmap outputImage = ResolveLayerPreviewImage(step.OutputLayer);
            executionController.TryGetSummary(step, out VisionPipelineStepResultSummary summary);
            string expectedInput = ResolveExpectedInputLayer(index);
            bool isBranch = IsBranch(step, expectedInput);
            bool inputWillBeProduced = HasEnabledProducerBefore(pipeline.Steps, index, step.InputLayer);
            bool isInputMissing = IsInputMissing(step, inputImage, inputWillBeProduced);
            string statusText = ResolveStatusText(step, outputImage, summary, isInputMissing);

            view.SetSelectedStep(
                FormatStepName(index, step),
                SafeText(step.ToolType, "Tool"),
                statusText,
                step.InputLayer,
                inputImage,
                step.OutputLayer,
                outputImage,
                ResolveFlowSummary(step, isBranch, expectedInput, isInputMissing),
                FormatParameters(step),
                OpenVisionPipelineReviewResultPresenter.FormatRunLog(step, inputImage, outputImage, mode, statusText, FormatValidationStatus(validationResult), summary));
            view.SetResultSummary(
                OpenVisionPipelineReviewResultPresenter.FormatResultSummary(summary),
                OpenVisionPipelineReviewResultPresenter.FormatResultDetails(step, summary));
            view.SetObjectResults(IsObjectResultTool(step), summary?.ObjectResults);
            view.SetGeometryResults(IsGeometryResultTool(step), summary?.GeometryFeatures);
            view.SetReviewGuide(OpenVisionPipelineReviewGuidePresenter.CreateSelected(
                index + 1,
                pipeline.Steps.Count,
                step,
                statusText,
                inputImage != null,
                outputImage != null,
                summary,
                validationResult,
                expectedInput,
                isBranch,
                inputWillBeProduced,
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

        private void UpdateFixtureTeachState(VisionPipelineStep step, VisionPipelineStepResultSummary summary)
        {
            if (!VisionPipelineFixtureFrameService.IsProducer(step))
            {
                view.SetFixtureTeachState(false, false, string.Empty);
                return;
            }

            if (TryGetReviewedFixturePose(step, summary, out double x, out double y, out double angle, out double scale)
                && TryGetReferenceImageSize(step, out int referenceWidth, out int referenceHeight))
            {
                view.SetFixtureTeachState(
                    true,
                    true,
                    TF(
                        "PipelineReview.FixtureTeach.ReadyWithDimensionsFormat",
                        "X {0} / Y {1} / {2} deg / scale {3} / reference {4} x {5}. Confirm the reference image.",
                        FormatPoseValue(x),
                        FormatPoseValue(y),
                        FormatPoseValue(angle),
                        FormatPoseValue(scale),
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
            fixtureProducerIndex = -1;
            fixtureNormalizeIndex = -1;
            fixtureMeasurementIndex = -1;

            if (!TryResolveFixtureChain(
                    out fixtureProducerIndex,
                    out fixtureNormalizeIndex,
                    out fixtureMeasurementIndex,
                    out string frameName))
            {
                view.SetFixtureDesignerState(
                    false,
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    null,
                    string.Empty,
                    null,
                    null,
                    false,
                    false,
                    false);
                return;
            }

            VisionPipelineStep producer = pipeline.Steps[fixtureProducerIndex];
            VisionPipelineStep normalize = pipeline.Steps[fixtureNormalizeIndex];
            VisionPipelineStep measurement = pipeline.Steps[fixtureMeasurementIndex];
            executionController.TryGetSummary(producer, out VisionPipelineStepResultSummary producerSummary);
            executionController.TryGetSummary(normalize, out VisionPipelineStepResultSummary normalizeSummary);

            bool hasPose = TryGetReviewedFixturePose(
                producer,
                producerSummary,
                out double currentX,
                out double currentY,
                out double currentAngle,
                out double currentScale);
            double referenceX = 0d;
            double referenceY = 0d;
            double referenceAngle = 0d;
            double referenceScale = 0d;
            bool hasReference = TryGetParameterDouble(producer, VisionPipelineFixtureFrameService.ReferenceXParameter, out referenceX)
                && TryGetParameterDouble(producer, VisionPipelineFixtureFrameService.ReferenceYParameter, out referenceY)
                && TryGetParameterDouble(producer, VisionPipelineFixtureFrameService.ReferenceAngleParameter, out referenceAngle)
                && TryGetParameterDouble(producer, VisionPipelineFixtureFrameService.ReferenceScaleParameter, out referenceScale)
                && referenceScale > 0d;
            int referenceWidth = GetParameterInt(producer, VisionPipelineFixtureFrameService.ReferenceImageWidthParameter);
            int referenceHeight = GetParameterInt(producer, VisionPipelineFixtureFrameService.ReferenceImageHeightParameter);
            bool hasRoi = TryGetStepRoi(measurement, out System.Drawing.RectangleF referenceRoi);

            string templateValue = GetTemplateValue(producer);

            string searchRoi = GetParameterBool(producer, "USE_ROI")
                ? GetParameter(producer, "CvROI")
                : T("PipelineReview.FixtureDesigner.FullImage", "full image");
            string relationshipText = TF(
                "PipelineReview.FixtureDesigner.RelationshipFormat",
                "{0}: {1:00} {2} -> {3:00} {4} -> {5:00} {6} / ROI {7}",
                frameName,
                fixtureProducerIndex + 1,
                SafeText(producer.ToolType, "Matching"),
                fixtureNormalizeIndex + 1,
                "NormalizeImage",
                fixtureMeasurementIndex + 1,
                SafeText(measurement.ToolType, "Tool"),
                hasRoi ? FormatRoi(referenceRoi) : "-");
            string templateText = TF(
                "PipelineReview.FixtureDesigner.TemplateFormat",
                "Template: {0} / search ROI: {1}",
                string.IsNullOrWhiteSpace(templateValue) ? "-" : Path.GetFileName(templateValue),
                string.IsNullOrWhiteSpace(searchRoi) ? "-" : searchRoi);
            string referenceText = hasReference
                ? TF(
                    "PipelineReview.FixtureDesigner.ReferenceCompactFormat",
                    "Ref ({0},{1}) / {2} deg / {3}x / {4}x{5}",
                    FormatPoseValue(referenceX),
                    FormatPoseValue(referenceY),
                    FormatPoseValue(referenceAngle),
                    FormatPoseValue(referenceScale),
                    referenceWidth,
                    referenceHeight)
                : T("PipelineReview.FixtureDesigner.ReferenceMissing", "Reference pose or image size is incomplete.");
            string currentText = hasPose
                ? TF(
                    "PipelineReview.FixtureDesigner.CurrentCompactFormat",
                    "Now ({0},{1}) / {2} deg / {3}x",
                    FormatPoseValue(currentX),
                    FormatPoseValue(currentY),
                    FormatPoseValue(currentAngle),
                    FormatPoseValue(currentScale))
                : T("PipelineReview.FixtureDesigner.CurrentWaiting", "Current pose: Run Review required");
            string qualityText = FormatFixtureQuality(producerSummary, normalizeSummary);

            Bitmap templatePreview = TryLoadTemplatePreview(templateValue);
            Bitmap sourcePreview = null;
            Bitmap normalizedPreview = null;
            string sourceText = SafeText(producer.InputLayer, "-");
            string normalizedText = SafeText(normalize.OutputLayer, "-");
            try
            {
                Bitmap source = ResolveLayerPreviewImage(producer.InputLayer);
                Bitmap normalized = normalizeSummary?.Success == true
                    ? ResolveLayerPreviewImage(normalize.OutputLayer)
                    : null;
                if (source != null && hasPose && hasReference && hasRoi)
                {
                    System.Drawing.PointF[] sourcePolygon = TransformReferenceRoi(
                        referenceRoi,
                        referenceX,
                        referenceY,
                        currentX,
                        currentY,
                        VisionPipelineFixtureFrameService.NormalizeAngle(currentAngle - referenceAngle),
                        currentScale / referenceScale);
                    sourcePreview = DrawRoiOverlay(source, sourcePolygon, "Relative ROI on source", System.Drawing.Color.Magenta);
                    sourceText = TF(
                        "PipelineReview.FixtureDesigner.SourceLayerFormat",
                        "{0} / transformed from ROI {1}",
                        SafeText(producer.InputLayer, "-"),
                        FormatRoi(referenceRoi));
                }
                else if (source != null)
                {
                    sourcePreview = new Bitmap(source);
                    sourceText = TF(
                        "PipelineReview.FixtureDesigner.SourceWaitingFormat",
                        "{0} / Run Review for transformed ROI",
                        SafeText(producer.InputLayer, "-"));
                }

                if (normalized != null && hasRoi)
                {
                    normalizedPreview = DrawRoiOverlay(
                        normalized,
                        RectanglePoints(referenceRoi),
                        "Reference ROI",
                        System.Drawing.Color.LimeGreen);
                    normalizedText = TF(
                        "PipelineReview.FixtureDesigner.NormalizedLayerFormat",
                        "{0} / ROI {1}",
                        SafeText(normalize.OutputLayer, "-"),
                        FormatRoi(referenceRoi));
                }

                view.SetFixtureDesignerState(
                    true,
                    relationshipText,
                    templateText,
                    referenceText,
                    currentText,
                    qualityText,
                    sourceText,
                    sourcePreview,
                    normalizedText,
                    normalizedPreview,
                    templatePreview,
                    hasPose && referenceWidth > 0 && referenceHeight > 0,
                    true,
                    true);
            }
            finally
            {
                sourcePreview?.Dispose();
                normalizedPreview?.Dispose();
                templatePreview?.Dispose();
            }
        }

        private bool TryResolveFixtureChain(
            out int producerIndex,
            out int normalizeIndex,
            out int measurementIndex,
            out string frameName)
        {
            producerIndex = -1;
            normalizeIndex = -1;
            measurementIndex = -1;
            frameName = string.Empty;
            IReadOnlyList<VisionPipelineStep> steps = pipeline?.Steps;
            if (steps == null)
            {
                return false;
            }

            for (int index = 0; index < steps.Count; index++)
            {
                VisionPipelineStep producer = steps[index];
                if (producer?.Enabled != true || !VisionPipelineFixtureFrameService.IsProducer(producer))
                {
                    continue;
                }

                string candidateFrame = GetParameter(producer, VisionPipelineFixtureFrameService.FrameNameParameter);
                int candidateNormalize = Enumerable.Range(index + 1, steps.Count - index - 1)
                    .FirstOrDefault(candidate =>
                        steps[candidate]?.Enabled == true
                        && VisionPipelineFixtureFrameService.IsNormalizeImageConsumer(steps[candidate])
                        && string.Equals(
                            GetParameter(steps[candidate], VisionPipelineFixtureFrameService.FrameNameParameter),
                            candidateFrame,
                            StringComparison.OrdinalIgnoreCase));
                if (candidateNormalize <= index)
                {
                    continue;
                }

                HashSet<string> reachableLayers = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                {
                    SafeText(steps[candidateNormalize].OutputLayer, string.Empty)
                };
                for (int candidate = candidateNormalize + 1; candidate < steps.Count; candidate++)
                {
                    VisionPipelineStep downstream = steps[candidate];
                    if (downstream?.Enabled != true || !reachableLayers.Contains(SafeText(downstream.InputLayer, string.Empty)))
                    {
                        continue;
                    }

                    if (GetParameterBool(downstream, "USE_ROI") && TryGetStepRoi(downstream, out _))
                    {
                        producerIndex = index;
                        normalizeIndex = candidateNormalize;
                        measurementIndex = candidate;
                        frameName = string.IsNullOrWhiteSpace(candidateFrame) ? "Fixture" : candidateFrame;
                        return true;
                    }

                    if (!string.IsNullOrWhiteSpace(downstream.OutputLayer))
                    {
                        reachableLayers.Add(downstream.OutputLayer.Trim());
                    }
                }
            }

            return false;
        }

        private string FormatFixtureQuality(
            VisionPipelineStepResultSummary producerSummary,
            VisionPipelineStepResultSummary normalizeSummary)
        {
            VisionPipelineStepResultSummary scoreSummary = producerSummary;
            if (!TryGetMetric(scoreSummary, VisionPipelineKnownMetrics.ScoreMargin, out _))
            {
                VisionPipelineStep producer = pipeline?.Steps?.ElementAtOrDefault(fixtureProducerIndex);
                string producerTemplate = GetTemplateValue(producer);
                for (int index = fixtureProducerIndex - 1; index >= 0; index--)
                {
                    VisionPipelineStep candidate = pipeline.Steps[index];
                    string toolType = VisionPipelineNormalizer.NormalizeToolType(candidate?.ToolType);
                    if ((toolType != "matching" && toolType != "templatematching")
                        || !string.Equals(candidate.InputLayer, producer?.InputLayer, StringComparison.OrdinalIgnoreCase)
                        || !string.Equals(GetTemplateValue(candidate), producerTemplate, StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    executionController.TryGetSummary(candidate, out VisionPipelineStepResultSummary candidateSummary);
                    if (TryGetMetric(candidateSummary, VisionPipelineKnownMetrics.ScoreMargin, out _))
                    {
                        scoreSummary = candidateSummary;
                        break;
                    }
                }
            }

            string score = TryGetMetric(producerSummary, VisionPipelineKnownMetrics.ScoreMax, out double scoreValue)
                ? FormatPoseValue(scoreValue)
                : "-";
            string margin = TryGetMetric(scoreSummary, VisionPipelineKnownMetrics.ScoreMargin, out double marginValue)
                ? FormatPoseValue(marginValue)
                : "-";
            string valid = TryGetMetric(normalizeSummary, VisionPipelineKnownMetrics.FixtureValidPixelRatio, out double validValue)
                ? validValue.ToString("P1", CultureInfo.CurrentCulture)
                : "-";
            return TF(
                "PipelineReview.FixtureDesigner.QualityCompactFormat",
                "Score {0} / margin {1} / valid {2}",
                score,
                margin,
                valid);
        }

        private static bool TryGetMetric(VisionPipelineStepResultSummary summary, string name, out double value)
        {
            value = 0d;
            return summary?.Metrics != null
                && summary.Metrics.TryGetValue(name, out value)
                && IsFinite(value);
        }

        private static Bitmap TryLoadTemplatePreview(string templateValue)
        {
            if (string.IsNullOrWhiteSpace(templateValue))
            {
                return null;
            }

            try
            {
                string path = VisionPipelineAppToolFactory.ResolveTemplatePath(templateValue);
                return File.Exists(path) ? new Bitmap(path) : null;
            }
            catch
            {
                return null;
            }
        }

        private static Bitmap DrawRoiOverlay(
            Bitmap source,
            System.Drawing.PointF[] points,
            string label,
            System.Drawing.Color color)
        {
            if (source == null || points == null || points.Length < 4)
            {
                return null;
            }

            Bitmap result = new Bitmap(source);
            using System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(result);
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            using System.Drawing.Pen shadow = new System.Drawing.Pen(System.Drawing.Color.Black, 6f);
            using System.Drawing.Pen pen = new System.Drawing.Pen(color, 3f);
            graphics.DrawPolygon(shadow, points);
            graphics.DrawPolygon(pen, points);
            using System.Drawing.Font font = new System.Drawing.Font("Segoe UI", 11f, System.Drawing.FontStyle.Bold);
            System.Drawing.SizeF size = graphics.MeasureString(label, font);
            float labelX = Math.Max(0f, Math.Min(points.Min(point => point.X), result.Width - size.Width - 8f));
            float labelY = Math.Max(0f, Math.Min(points.Min(point => point.Y) - size.Height - 4f, result.Height - size.Height - 4f));
            using System.Drawing.SolidBrush background = new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(210, 16, 32, 39));
            using System.Drawing.SolidBrush foreground = new System.Drawing.SolidBrush(System.Drawing.Color.White);
            graphics.FillRectangle(background, labelX, labelY, size.Width + 6f, size.Height + 2f);
            graphics.DrawString(label, font, foreground, labelX + 3f, labelY + 1f);
            return result;
        }

        private static System.Drawing.PointF[] RectanglePoints(System.Drawing.RectangleF roi)
        {
            return new[]
            {
                new System.Drawing.PointF(roi.Left, roi.Top),
                new System.Drawing.PointF(roi.Right, roi.Top),
                new System.Drawing.PointF(roi.Right, roi.Bottom),
                new System.Drawing.PointF(roi.Left, roi.Bottom)
            };
        }

        private static System.Drawing.PointF[] TransformReferenceRoi(
            System.Drawing.RectangleF roi,
            double referenceX,
            double referenceY,
            double currentX,
            double currentY,
            double angleDelta,
            double scaleRatio)
        {
            double radians = angleDelta * Math.PI / 180d;
            double cosine = Math.Cos(radians);
            double sine = Math.Sin(radians);
            return RectanglePoints(roi)
                .Select(point =>
                {
                    double x = point.X - referenceX;
                    double y = point.Y - referenceY;
                    return new System.Drawing.PointF(
                        (float)(currentX + scaleRatio * ((cosine * x) + (sine * y))),
                        (float)(currentY + scaleRatio * ((-sine * x) + (cosine * y))));
                })
                .ToArray();
        }

        private static bool TryGetStepRoi(VisionPipelineStep step, out System.Drawing.RectangleF roi)
        {
            roi = default;
            string[] parts = GetParameter(step, "CvROI").Split(',');
            if (parts.Length != 4
                || !float.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out float x)
                || !float.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out float y)
                || !float.TryParse(parts[2], NumberStyles.Float, CultureInfo.InvariantCulture, out float width)
                || !float.TryParse(parts[3], NumberStyles.Float, CultureInfo.InvariantCulture, out float height)
                || width <= 0f
                || height <= 0f)
            {
                return false;
            }

            roi = new System.Drawing.RectangleF(x, y, width, height);
            return true;
        }

        private static string FormatRoi(System.Drawing.RectangleF roi)
        {
            return string.Join(",", new[]
            {
                roi.X.ToString("0.###", CultureInfo.InvariantCulture),
                roi.Y.ToString("0.###", CultureInfo.InvariantCulture),
                roi.Width.ToString("0.###", CultureInfo.InvariantCulture),
                roi.Height.ToString("0.###", CultureInfo.InvariantCulture)
            });
        }

        private static string GetParameter(VisionPipelineStep step, string key)
        {
            if (step?.Parameters == null || string.IsNullOrWhiteSpace(key))
            {
                return string.Empty;
            }

            return step.Parameters
                .FirstOrDefault(parameter => string.Equals(parameter.Key, key, StringComparison.OrdinalIgnoreCase))
                .Value
                ?.Trim()
                ?? string.Empty;
        }

        private static string GetTemplateValue(VisionPipelineStep step)
        {
            string value = GetParameter(step, "TemplatePath");
            return string.IsNullOrWhiteSpace(value) ? GetParameter(step, "PATTERN_PATH") : value;
        }

        private static bool GetParameterBool(VisionPipelineStep step, string key)
        {
            return bool.TryParse(GetParameter(step, key), out bool value) && value;
        }

        private static bool TryGetParameterDouble(VisionPipelineStep step, string key, out double value)
        {
            return double.TryParse(GetParameter(step, key), NumberStyles.Float, CultureInfo.InvariantCulture, out value)
                && IsFinite(value);
        }

        private static int GetParameterInt(VisionPipelineStep step, string key)
        {
            return int.TryParse(GetParameter(step, key), NumberStyles.Integer, CultureInfo.InvariantCulture, out int value)
                ? value
                : 0;
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
            if (!TryGetReviewedFixturePose(step, summary, out double x, out double y, out double angle, out double scale)
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

            parameters[VisionPipelineFixtureFrameService.ReferenceXParameter] = FormatPoseValue(x);
            parameters[VisionPipelineFixtureFrameService.ReferenceYParameter] = FormatPoseValue(y);
            parameters[VisionPipelineFixtureFrameService.ReferenceAngleParameter] = FormatPoseValue(angle);
            parameters[VisionPipelineFixtureFrameService.ReferenceScaleParameter] = FormatPoseValue(scale);
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
                    FormatPoseValue(x),
                    FormatPoseValue(y),
                    FormatPoseValue(angle),
                    FormatPoseValue(scale),
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

        private static bool TryGetReviewedFixturePose(
            VisionPipelineStep step,
            VisionPipelineStepResultSummary summary,
            out double x,
            out double y,
            out double angle,
            out double scale)
        {
            x = 0d;
            y = 0d;
            angle = 0d;
            scale = 0d;
            if (!VisionPipelineFixtureFrameService.IsProducer(step)
                || summary?.Success != true
                || summary.Metrics == null)
            {
                return false;
            }

            string toolType = VisionPipelineNormalizer.NormalizeToolType(step.ToolType);
            return (toolType == "matching" || toolType == "templatematching")
                && summary.Metrics.TryGetValue(VisionPipelineKnownMetrics.FixtureCenterX, out x)
                && summary.Metrics.TryGetValue(VisionPipelineKnownMetrics.FixtureCenterY, out y)
                && summary.Metrics.TryGetValue(VisionPipelineKnownMetrics.FixtureAngle, out angle)
                && summary.Metrics.TryGetValue(VisionPipelineKnownMetrics.FixtureScale, out scale)
                && IsFinite(x)
                && IsFinite(y)
                && IsFinite(angle)
                && IsFinite(scale)
                && scale > 0d;
        }

        private static bool IsFinite(double value)
        {
            return !double.IsNaN(value) && !double.IsInfinity(value);
        }

        private static string FormatPoseValue(double value)
        {
            return value.ToString("0.###", CultureInfo.InvariantCulture);
        }

        private List<PipelineFlowStepItem> CreateFlowItems(IReadOnlyList<VisionPipelineStep> steps)
        {
            List<PipelineFlowStepItem> items = new List<PipelineFlowStepItem>();
            string previousEnabledOutput = null;
            for (int i = 0; i < (steps?.Count ?? 0); i++)
            {
                VisionPipelineStep step = steps[i];
                if (step == null)
                {
                    continue;
                }

                Bitmap inputImage = ResolveLayerPreviewImage(step.InputLayer);
                Bitmap outputImage = ResolveLayerPreviewImage(step.OutputLayer);
                executionController.TryGetSummary(step, out VisionPipelineStepResultSummary summary);
                bool isBranch = IsBranch(step, previousEnabledOutput);
                bool inputWillBeProduced = HasEnabledProducerBefore(steps, i, step.InputLayer);
                bool isInputMissing = IsInputMissing(step, inputImage, inputWillBeProduced);
                string statusText = ResolveStatusText(step, outputImage, summary, isInputMissing);
                items.Add(new PipelineFlowStepItem
                {
                    Index = i,
                    Name = step.Name,
                    ToolType = step.ToolType,
                    InputLayer = step.InputLayer,
                    OutputLayer = step.OutputLayer,
                    ExpectedInputLayer = previousEnabledOutput,
                    FlowStateText = ResolveFlowSummary(step, isBranch, previousEnabledOutput, isInputMissing),
                    IsBranch = isBranch,
                    IsEnabled = step.Enabled,
                    HasInputImage = inputImage != null,
                    IsInputMissing = isInputMissing,
                    HasOutputImage = outputImage != null,
                    Status = ResolveFlowStatus(step, outputImage, summary, isInputMissing),
                    StatusText = statusText
                });

                if (step.Enabled && !string.IsNullOrWhiteSpace(step.OutputLayer))
                {
                    previousEnabledOutput = step.OutputLayer.Trim();
                }
            }

            return items;
        }

        private string ResolveExpectedInputLayer(int index)
        {
            string previousEnabledOutput = null;
            for (int i = 0; i < index && i < (pipeline?.Steps?.Count ?? 0); i++)
            {
                VisionPipelineStep previous = pipeline.Steps[i];
                if (previous?.Enabled == true && !string.IsNullOrWhiteSpace(previous.OutputLayer))
                {
                    previousEnabledOutput = previous.OutputLayer.Trim();
                }
            }

            return previousEnabledOutput;
        }

        private static bool IsBranch(VisionPipelineStep step, string expectedInputLayer)
        {
            if (step == null || !step.Enabled || string.IsNullOrWhiteSpace(expectedInputLayer))
            {
                return false;
            }

            return !string.Equals(SafeText(step.InputLayer, string.Empty), expectedInputLayer, StringComparison.OrdinalIgnoreCase);
        }

        private static PipelineFlowStepStatus ResolveFlowStatus(
            VisionPipelineStep step,
            Bitmap outputImage,
            VisionPipelineStepResultSummary summary,
            bool isInputMissing)
        {
            if (step != null && !step.Enabled)
            {
                return PipelineFlowStepStatus.Skipped;
            }

            if (summary != null)
            {
                return summary.Success && !summary.IsAcceptanceNg
                    ? PipelineFlowStepStatus.Passed
                    : PipelineFlowStepStatus.Failed;
            }

            if (isInputMissing)
            {
                return PipelineFlowStepStatus.MissingInput;
            }

            return outputImage == null ? PipelineFlowStepStatus.Waiting : PipelineFlowStepStatus.Loaded;
        }

        private static string ResolveStatusText(
            VisionPipelineStep step,
            Bitmap outputImage,
            VisionPipelineStepResultSummary summary,
            bool isInputMissing)
        {
            if (step != null && !step.Enabled)
            {
                return "OFF";
            }

            if (summary != null)
            {
                return SafeText(summary.Status, "DONE");
            }

            if (isInputMissing)
            {
                return T("PipelineReview.Status.InputMissing", "Input missing");
            }

            return outputImage == null ? "WAIT" : "READY";
        }

        private static bool HasEnabledProducerBefore(
            IReadOnlyList<VisionPipelineStep> steps,
            int stepIndex,
            string inputLayer)
        {
            if (steps == null || stepIndex <= 0 || string.IsNullOrWhiteSpace(inputLayer))
            {
                return false;
            }

            string normalizedInput = inputLayer.Trim();
            for (int index = 0; index < stepIndex && index < steps.Count; index++)
            {
                VisionPipelineStep candidate = steps[index];
                if (candidate?.Enabled == true
                    && string.Equals(candidate.OutputLayer?.Trim(), normalizedInput, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsInputMissing(VisionPipelineStep step, Bitmap inputImage, bool inputWillBeProduced)
        {
            return step?.Enabled == true && inputImage == null && !inputWillBeProduced;
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

        private static string ResolveFlowSummary(
            VisionPipelineStep step,
            bool isBranch,
            string expectedInputLayer,
            bool isInputMissing)
        {
            if (step == null)
            {
                return "-";
            }

            if (!step.Enabled)
            {
                return T("PipelineReview.Flow.DisabledStep", "Disabled step");
            }

            string inputLayer = SafeText(step.InputLayer, T("PipelineReview.Flow.UnknownInput", "Input?"));
            if (isInputMissing)
            {
                return TF("PipelineReview.Flow.MissingInputFormat", "Missing input: {0}", inputLayer);
            }

            if (string.IsNullOrWhiteSpace(expectedInputLayer))
            {
                return TF("PipelineReview.Flow.SourceImageFormat", "Source image: {0}", inputLayer);
            }

            if (isBranch)
            {
                return TF("PipelineReview.Flow.BranchInputFormat", "Branch input: {0} instead of previous output {1}", inputLayer, expectedInputLayer);
            }

            return TF("PipelineReview.Flow.PreviousOutputFormat", "Previous output: {0}", expectedInputLayer);
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
