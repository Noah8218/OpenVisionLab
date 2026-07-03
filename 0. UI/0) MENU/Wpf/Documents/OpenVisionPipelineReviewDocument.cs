using Lib.Common;
using Lib.OpenCV.Pipeline;
using OpenCvSharp;
using OpenVisionLab._1._Core;
using OpenVisionLab.Pipeline.Controls;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
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
        private readonly Dictionary<VisionPipelineStep, VisionPipelineStepResultSummary> stepResultSummaries = new Dictionary<VisionPipelineStep, VisionPipelineStepResultSummary>();
        private readonly Dictionary<string, Bitmap> reviewLayerImages = new Dictionary<string, Bitmap>(StringComparer.OrdinalIgnoreCase);
        private VisionPipeline pipeline;
        private VisionPipelineValidationResult validationResult;
        private OpenVisionWorkspaceSamplePairDecisionGuide activeSamplePairGuide = OpenVisionWorkspaceSamplePairDecisionGuide.Empty;
        private VisionPipelineSampleCatalogItem activeCatalogSample;
        private VisionPipelineSampleCatalogItem activePairCounterpartSample;
        private string activePipelineName = string.Empty;
        private int selectedIndex;
        private PipelineFlowPreviewMode selectedMode = PipelineFlowPreviewMode.Overlay;
        private bool isRunningReview;
        private string reviewExecutionState = T("PipelineReview.Execution.NotRun", "Not run");
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
            view.StepSelected += OnStepSelected;
            view.RunReviewRequested += OnRunReviewRequested;
            view.PreviousStepRequested += OnPreviousStepRequested;
            view.NextStepRequested += OnNextStepRequested;
            view.OpenPairSampleRequested += OnOpenPairSampleRequested;
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
        public string SelectedStatusText => view.SelectedStatusText;
        public string FlowSummaryText => view.FlowSummaryText;
        public string ParameterSummaryText => view.ParameterSummaryText;
        public string ValidationStatusText => view.ValidationStatusText;
        public string ValidationDetailText => view.ValidationDetailText;
        public string ResultSummaryText => view.ResultSummaryText;
        public string ResultDetailText => view.ResultDetailText;
        public string RunLogText => view.RunLogText;
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
        public bool CanOpenPairSample => view.CanOpenReviewGuidePairAction;
        public bool CanSelectPreviousStep => view.CanSelectPreviousStep;
        public bool CanSelectNextStep => view.CanSelectNextStep;
        public bool HasInputPreview => view.HasInputPreview;
        public bool HasOutputPreview => view.HasOutputPreview;

        public event EventHandler LayerStateChanged = delegate { };
        public event EventHandler<OpenVisionPipelineReviewSampleOpenRequestedEventArgs> OpenWorkspaceSampleRequested = delegate { };

        public void RefreshLayerState()
        {
            activePipelineName = ResolveActivePipelineName();
            pipeline = VisionPipelineStorage.Load(recipeContext.Name, activePipelineName);
            RefreshActiveSamplePairGuide(activePipelineName);
            validationResult = VisionPipelineValidator.Validate(pipeline, GetLayerNames());
            ClearReviewRunCache();
            int stepCount = pipeline?.Steps?.Count ?? 0;
            view.SetPipelineHeader(activePipelineName, stepCount);
            view.SetValidation(FormatValidationStatus(validationResult), FormatValidationDetails(validationResult));
            view.SetResultSummary(
                T("PipelineReview.RunRequired", "Run review required"),
                T("PipelineReview.RunRequiredDetail", "Click Run Review to refresh step results."));

            if (stepCount == 0)
            {
                selectedIndex = -1;
                view.SetEmptyState(activePipelineName);
                view.SetValidation(FormatValidationStatus(validationResult), FormatValidationDetails(validationResult));
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
            view.OpenPairSampleRequested -= OnOpenPairSampleRequested;
            OpenVisionLanguageService.LanguageChanged -= OnLanguageChanged;
            OpenWorkspaceSampleRequested = delegate { };
            ClearReviewRunCache();
        }

        private void OnStepSelected(object sender, PipelineFlowStepSelectedEventArgs e)
        {
            SelectStep(e.Index, e.Mode);
        }

        private async void OnRunReviewRequested(object sender, EventArgs e)
        {
            await RunReviewAsync();
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

        private void OnOpenPairSampleRequested(object sender, EventArgs e)
        {
            RequestOpenPairSample();
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
            view.SetPipelineHeader(activePipelineName, stepCount);
            view.SetValidation(FormatValidationStatus(validationResult), FormatValidationDetails(validationResult));

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

        private static string ResolvePairActionText(VisionPipelineSampleCatalogItem counterpartSample)
        {
            if (counterpartSample == null)
            {
                return string.Empty;
            }

            string role = IsOkSampleReference(counterpartSample)
                ? LocalText("OK 기준", "OK reference")
                : IsNgSampleReference(counterpartSample)
                    ? LocalText("NG 기준", "NG reference")
                    : LocalText("반대 기준", "opposite reference");
            return string.Format(
                CultureInfo.CurrentCulture,
                LocalText("{0} 열기", "Open {0}"),
                role);
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

            VisionPipelineStep step = pipeline.Steps[index];
            Bitmap inputImage = ResolveLayerPreviewImage(step.InputLayer);
            Bitmap outputImage = ResolveLayerPreviewImage(step.OutputLayer);
            stepResultSummaries.TryGetValue(step, out VisionPipelineStepResultSummary summary);
            string expectedInput = ResolveExpectedInputLayer(index);
            bool isBranch = IsBranch(step, expectedInput);
            string statusText = ResolveStatusText(step, outputImage, summary);

            view.SetSelectedStep(
                FormatStepName(index, step),
                SafeText(step.ToolType, "Tool"),
                statusText,
                step.InputLayer,
                inputImage,
                step.OutputLayer,
                outputImage,
                ResolveFlowSummary(step, isBranch, expectedInput),
                FormatParameters(step),
                FormatRunLog(step, inputImage, outputImage, mode, statusText, FormatValidationStatus(validationResult), summary));
            view.SetResultSummary(FormatResultSummary(summary), FormatResultDetails(step, summary));
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
                activeSamplePairGuide));
            view.SetReviewGuidePairAction(
                ResolvePairActionText(activePairCounterpartSample),
                activePairCounterpartSample?.CanOpen == true);
            view.SetReviewGuidePairMetric(ResolvePairMetricComparisonText(step, summary));
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
                stepResultSummaries.TryGetValue(step, out VisionPipelineStepResultSummary summary);
                bool isBranch = IsBranch(step, previousEnabledOutput);
                string statusText = ResolveStatusText(step, outputImage, summary);
                items.Add(new PipelineFlowStepItem
                {
                    Index = i,
                    Name = step.Name,
                    ToolType = step.ToolType,
                    InputLayer = step.InputLayer,
                    OutputLayer = step.OutputLayer,
                    ExpectedInputLayer = previousEnabledOutput,
                    FlowStateText = ResolveFlowSummary(step, isBranch, previousEnabledOutput),
                    IsBranch = isBranch,
                    IsEnabled = step.Enabled,
                    HasInputImage = inputImage != null,
                    HasOutputImage = outputImage != null,
                    Status = ResolveFlowStatus(step, outputImage, summary),
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

        private static PipelineFlowStepStatus ResolveFlowStatus(VisionPipelineStep step, Bitmap outputImage, VisionPipelineStepResultSummary summary)
        {
            if (step != null && !step.Enabled)
            {
                return PipelineFlowStepStatus.Skipped;
            }

            if (summary != null)
            {
                return summary.Success ? PipelineFlowStepStatus.Loaded : PipelineFlowStepStatus.Waiting;
            }

            return outputImage == null ? PipelineFlowStepStatus.Waiting : PipelineFlowStepStatus.Loaded;
        }

        private static string ResolveStatusText(VisionPipelineStep step, Bitmap outputImage, VisionPipelineStepResultSummary summary)
        {
            if (step != null && !step.Enabled)
            {
                return "OFF";
            }

            if (summary != null)
            {
                return SafeText(summary.Status, "DONE");
            }

            return outputImage == null ? "WAIT" : "READY";
        }

        private async Task RunReviewAsync()
        {
            if (isRunningReview || pipeline?.Steps == null || pipeline.Steps.Count == 0)
            {
                reviewExecutionState = isRunningReview
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

            isRunningReview = true;
            reviewExecutionState = T("PipelineReview.Execution.Started", "Started");
            view.SetRunReviewBusy(true);
            view.SetResultSummary(
                T("PipelineReview.RunningSummary", "Running"),
                T("PipelineReview.RunningDetail", "Pipeline review execution in progress."));
            view.SetReviewGuide(OpenVisionPipelineReviewGuidePresenter.CreateRunning(
                GetSelectedDisplayIndex(),
                pipeline.Steps.Count,
                GetSelectedStepOrDefault()));

            try
            {
                ClearReviewRunCache();
                using VisionPipelineContext context = CreateReviewContextFromDisplayLayers();
                VisionPipelineRunResult runResult = await VisionPipelineExecutionService.RunAsync(
                    pipeline,
                    context,
                    StepTimeoutMilliseconds,
                    CancellationToken.None,
                    OnReviewStepExecutionUpdated);

                await view.Dispatcher.InvokeAsync(() => ApplyReviewRunResult(runResult));
                DisposeRunResultImages(runResult);
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
                isRunningReview = false;
                await view.Dispatcher.InvokeAsync(() => view.SetRunReviewBusy(false));
            }
        }

        private void OnReviewStepExecutionUpdated(VisionPipelineStepExecutionUpdate update)
        {
            if (!view.Dispatcher.CheckAccess())
            {
                view.Dispatcher.Invoke(() => OnReviewStepExecutionUpdated(update));
                return;
            }

            if (update?.StepResult != null && update.Step != null)
            {
                stepResultSummaries[update.Step] = VisionPipelineResultSummaryService.CreateStepSummary(GetStepDisplayIndex(update.Step), update.StepResult);
                CacheReviewOutput(update.StepResult);
            }

            if (update?.Step != null)
            {
                view.SetSteps(CreateFlowItems(pipeline.Steps));
                if (ReferenceEquals(update.Step, pipeline.Steps.ElementAtOrDefault(selectedIndex)))
                {
                    SelectStep(selectedIndex, selectedMode);
                }
            }
        }

        private void ApplyReviewRunResult(VisionPipelineRunResult runResult)
        {
            reviewExecutionState = TF("PipelineReview.Execution.CompletedFormat", "Completed / {0} step results", runResult?.StepResults?.Count ?? 0);
            CacheMissingReviewOutputs(runResult);
            List<VisionPipelineStepResultSummary> summaries = VisionPipelineResultSummaryService.CreateStepSummaries(runResult);
            foreach (VisionPipelineStepResultSummary summary in summaries)
            {
                VisionPipelineStep step = pipeline.Steps.FirstOrDefault(candidate => string.Equals(candidate?.Name, summary.Name, StringComparison.Ordinal)
                    && string.Equals(candidate?.OutputLayer, summary.OutputLayer, StringComparison.OrdinalIgnoreCase));
                if (step != null)
                {
                    stepResultSummaries[step] = summary;
                }
            }

            view.SetSteps(CreateFlowItems(pipeline.Steps));
            SelectStep(selectedIndex < 0 ? 0 : selectedIndex, selectedMode);
        }

        private VisionPipelineContext CreateReviewContextFromDisplayLayers()
        {
            VisionPipelineContext context = new VisionPipelineContext();
            for (int i = 0; i < displayManager.LayerCount; i++)
            {
                string title = displayManager.GetLayerTitle(i);
                Bitmap image = displayManager.GetLayerImage(i);
                if (string.IsNullOrWhiteSpace(title) || image == null || DisplayManagerImageExtensions.IsPlaceholderBitmap(image))
                {
                    continue;
                }

                using Mat mat = BitmapImageConverter.ToMat(image);
                context.SetLayer(title, mat);
            }

            return context;
        }

        private void CacheMissingReviewOutputs(VisionPipelineRunResult runResult)
        {
            foreach (VisionPipelineStepResult stepResult in runResult?.StepResults ?? Enumerable.Empty<VisionPipelineStepResult>())
            {
                CacheReviewOutput(stepResult, onlyIfMissing: true);
            }
        }

        private void CacheReviewOutput(VisionPipelineStepResult stepResult, bool onlyIfMissing = false)
        {
            string outputLayer = stepResult?.Step?.OutputLayer;
            if (string.IsNullOrWhiteSpace(outputLayer)
                || stepResult.ToolResult?.ResultImage == null
                || stepResult.ToolResult.ResultImage.Empty())
            {
                return;
            }

            if (onlyIfMissing && reviewLayerImages.ContainsKey(outputLayer))
            {
                return;
            }

            Bitmap bitmap = BitmapImageConverter.ToBitmap(stepResult.ToolResult.ResultImage);
            ReplaceReviewLayerImage(outputLayer, bitmap);
        }

        private static void DisposeRunResultImages(VisionPipelineRunResult runResult)
        {
            foreach (VisionPipelineStepResult stepResult in runResult?.StepResults ?? Enumerable.Empty<VisionPipelineStepResult>())
            {
                stepResult?.ToolResult?.ResultImage?.Dispose();
            }
        }

        private void ReplaceReviewLayerImage(string layerName, Bitmap image)
        {
            if (string.IsNullOrWhiteSpace(layerName) || image == null)
            {
                image?.Dispose();
                return;
            }

            if (reviewLayerImages.TryGetValue(layerName, out Bitmap existing))
            {
                existing?.Dispose();
            }

            reviewLayerImages[layerName] = image;
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

            return reviewLayerImages.TryGetValue(layerName, out Bitmap cached) ? cached : null;
        }

        private void ClearReviewRunCache()
        {
            stepResultSummaries.Clear();
            foreach (Bitmap image in reviewLayerImages.Values)
            {
                image?.Dispose();
            }

            reviewLayerImages.Clear();
        }

        private int GetStepDisplayIndex(VisionPipelineStep step)
        {
            int index = pipeline?.Steps?.IndexOf(step) ?? -1;
            return index < 0 ? 0 : index + 1;
        }

        private int GetSelectedDisplayIndex()
        {
            return selectedIndex < 0 ? 0 : selectedIndex + 1;
        }

        private VisionPipelineStep GetSelectedStepOrDefault()
        {
            return pipeline?.Steps?.ElementAtOrDefault(selectedIndex);
        }

        private static string ResolveFlowSummary(VisionPipelineStep step, bool isBranch, string expectedInputLayer)
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

        private static string FormatRunLog(
            VisionPipelineStep step,
            Bitmap inputImage,
            Bitmap outputImage,
            PipelineFlowPreviewMode mode,
            string statusText,
            string validationStatus,
            VisionPipelineStepResultSummary summary)
        {
            List<string> lines = new List<string>
            {
                TF("PipelineReview.RunLog.ReviewStateFormat", "Review state: {0}", SafeText(statusText, "WAIT")),
                TF("PipelineReview.RunLog.ValidationFormat", "Validation: {0}", SafeText(validationStatus, "NOT RUN")),
                TF("PipelineReview.RunLog.ResultFormat", "Result: {0}", FormatResultSummary(summary)),
                TF("PipelineReview.RunLog.PreviewModeFormat", "Preview mode: {0}", mode),
                TF("PipelineReview.RunLog.InputImageFormat", "Input image: {0}", FormatImageState(step?.InputLayer, inputImage)),
                TF("PipelineReview.RunLog.OutputImageFormat", "Output image: {0}", FormatImageState(step?.OutputLayer, outputImage))
            };

            return string.Join(Environment.NewLine, lines);
        }

        private static string FormatResultSummary(VisionPipelineStepResultSummary summary)
        {
            if (summary == null)
            {
                return T("PipelineReview.RunRequired", "Run review required");
            }

            string status = SafeText(summary.Status, summary.Success ? "OK" : "NG");
            if (summary.ElapsedMilliseconds > 0)
            {
                status += string.Format(CultureInfo.CurrentCulture, " / {0:0.0} ms", summary.ElapsedMilliseconds);
            }

            if (summary.ErrorCode > 0)
            {
                return string.Format(CultureInfo.CurrentCulture, "{0} / Error {1}:{2}", status, summary.ErrorCode, summary.ErrorName);
            }

            return status;
        }

        private static string FormatResultDetails(VisionPipelineStep step, VisionPipelineStepResultSummary summary)
        {
            if (summary == null)
            {
                return T("PipelineReview.NoRunResultForStep", "No run result for selected step.");
            }

            List<string> parts = new List<string>();
            if (summary.HasResultImage)
            {
                parts.Add(T("PipelineReview.Result.ImageLabel", "Image") + " " + summary.ResultImageSizeText.Replace(" ", string.Empty));
            }

            string metricText = FormatPrimaryMetricText(step, summary);
            if (!string.IsNullOrWhiteSpace(metricText))
            {
                parts.Add(metricText);
            }

            if (summary.OverlayCount > 0)
            {
                parts.Add(TF("PipelineReview.Result.OverlaysFormat", "Overlays {0}", summary.OverlayCount));
            }

            if (summary.IsAcceptanceNg)
            {
                string localizedAcceptanceMessage = OpenVisionPipelineReviewGuidePresenter.FormatAcceptanceMetricNgReason(step, summary);
                if (string.IsNullOrWhiteSpace(localizedAcceptanceMessage))
                {
                    localizedAcceptanceMessage = summary.AcceptanceMessage;
                }

                if (!string.IsNullOrWhiteSpace(localizedAcceptanceMessage))
                {
                    parts.Add(Truncate(localizedAcceptanceMessage, 80));
                }
            }
            else if (!summary.Success && !string.IsNullOrWhiteSpace(summary.Message))
            {
                parts.Add(Truncate(summary.Message, 80));
            }

            return parts.Count == 0 ? SafeText(summary.Message, "-") : string.Join(" / ", parts);
        }

        private string ResolvePairMetricComparisonText(VisionPipelineStep step, VisionPipelineStepResultSummary summary)
        {
            if (summary?.Metrics == null
                || summary.Metrics.Count == 0
                || activeCatalogSample == null
                || activeSamplePairGuide == null
                || string.IsNullOrWhiteSpace(activeSamplePairGuide.PairReviewText))
            {
                return string.Empty;
            }

            string metricName = ResolvePairComparisonMetricName(step, summary.Metrics);
            if (string.IsNullOrWhiteSpace(metricName)
                || !TryGetMetricValue(summary.Metrics, metricName, out double actualValue))
            {
                return string.Empty;
            }

            VisionPipelineSampleExpectedMetric selectedMetric = FindExpectedMetric(activeCatalogSample, metricName);
            VisionPipelineSampleExpectedMetric counterpartMetric = FindExpectedMetric(activePairCounterpartSample, metricName);
            if (selectedMetric == null && counterpartMetric == null)
            {
                return string.Empty;
            }

            string selectedRole = FormatSampleReferenceRole(activeCatalogSample);
            string counterpartRole = FormatSampleReferenceRole(activePairCounterpartSample);
            string selectedRange = FormatExpectedMetricRange(selectedMetric);
            string counterpartRange = FormatExpectedMetricRange(counterpartMetric);
            string selectedJudgment = FormatExpectedMetricJudgment(actualValue, selectedMetric, selectedRole);
            string counterpartJudgment = FormatExpectedMetricJudgment(actualValue, counterpartMetric, counterpartRole);

            return string.Format(
                CultureInfo.CurrentCulture,
                LocalText(
                    "\ud604\uc7ac \uce21\uc815: {0} {1} / {2} \uae30\uc900 {3} ({4}) / \ubc18\ub300 {5} \uae30\uc900 {6} ({7}) / \ub2e4\uc74c: \ubc18\ub300 \uc0d8\ud50c\ub3c4 \uac19\uc740 Pipeline\uc73c\ub85c \uc2e4\ud589\ud574 \uae30\uc900 \uc548/\ubc16 \uac08\ub9bc\uc744 \ud655\uc778",
                    "Measured: {0} {1} / {2} target {3} ({4}) / opposite {5} target {6} ({7}) / Next: run the opposite sample with the same Pipeline and confirm the metric splits inside/outside target."),
                FormatMetricName(metricName),
                FormatMetricValue(actualValue),
                selectedRole,
                selectedRange,
                selectedJudgment,
                counterpartRole,
                counterpartRange,
                counterpartJudgment);
        }

        private string ResolvePairComparisonMetricName(VisionPipelineStep step, IDictionary<string, double> metrics)
        {
            if (metrics == null || metrics.Count == 0)
            {
                return string.Empty;
            }

            if (!string.IsNullOrWhiteSpace(step?.AcceptanceMetricName)
                && TryResolveMetricKey(metrics, step.AcceptanceMetricName, out string acceptanceMetricName))
            {
                return acceptanceMetricName;
            }

            foreach (string expectedMetricName in EnumerateExpectedMetricNames(activeCatalogSample, activePairCounterpartSample))
            {
                if (TryResolveMetricKey(metrics, expectedMetricName, out string actualMetricName))
                {
                    return actualMetricName;
                }
            }

            KeyValuePair<string, double> metric = OrderResultMetrics(step, metrics).FirstOrDefault();
            return metric.Key ?? string.Empty;
        }

        private static IEnumerable<string> EnumerateExpectedMetricNames(params VisionPipelineSampleCatalogItem[] samples)
        {
            HashSet<string> emitted = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (VisionPipelineSampleCatalogItem sample in samples)
            {
                if (sample?.ExpectedMetrics == null)
                {
                    continue;
                }

                foreach (VisionPipelineSampleExpectedMetric metric in sample.ExpectedMetrics)
                {
                    string name = metric?.Name?.Trim();
                    if (!string.IsNullOrWhiteSpace(name) && emitted.Add(name))
                    {
                        yield return name;
                    }
                }
            }
        }

        private static VisionPipelineSampleExpectedMetric FindExpectedMetric(
            VisionPipelineSampleCatalogItem sample,
            string metricName)
        {
            if (sample?.ExpectedMetrics == null || string.IsNullOrWhiteSpace(metricName))
            {
                return null;
            }

            return sample.ExpectedMetrics.FirstOrDefault(metric =>
                metric != null
                && string.Equals(metric.Name?.Trim(), metricName.Trim(), StringComparison.OrdinalIgnoreCase));
        }

        private static bool TryResolveMetricKey(IDictionary<string, double> metrics, string metricName, out string actualMetricName)
        {
            actualMetricName = string.Empty;
            if (metrics == null || string.IsNullOrWhiteSpace(metricName))
            {
                return false;
            }

            foreach (string key in metrics.Keys)
            {
                if (string.Equals(key, metricName, StringComparison.OrdinalIgnoreCase))
                {
                    actualMetricName = key;
                    return true;
                }
            }

            return false;
        }

        private static bool TryGetMetricValue(IDictionary<string, double> metrics, string metricName, out double value)
        {
            value = 0D;
            if (metrics == null || string.IsNullOrWhiteSpace(metricName))
            {
                return false;
            }

            foreach (KeyValuePair<string, double> metric in metrics)
            {
                if (string.Equals(metric.Key, metricName, StringComparison.OrdinalIgnoreCase))
                {
                    value = metric.Value;
                    return true;
                }
            }

            return false;
        }

        private static string FormatExpectedMetricRange(VisionPipelineSampleExpectedMetric metric)
        {
            if (metric == null)
            {
                return "-";
            }

            string minimum = metric.Minimum?.Trim() ?? string.Empty;
            string maximum = metric.Maximum?.Trim() ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(minimum) && !string.IsNullOrWhiteSpace(maximum))
            {
                return string.Equals(minimum, maximum, StringComparison.OrdinalIgnoreCase)
                    ? minimum
                    : minimum + "~" + maximum;
            }

            if (!string.IsNullOrWhiteSpace(minimum))
            {
                return ">= " + minimum;
            }

            if (!string.IsNullOrWhiteSpace(maximum))
            {
                return "<= " + maximum;
            }

            return "-";
        }

        private static string FormatExpectedMetricJudgment(
            double actualValue,
            VisionPipelineSampleExpectedMetric metric,
            string roleText)
        {
            bool? isInside = IsInsideExpectedMetricRange(actualValue, metric);
            if (!isInside.HasValue)
            {
                return LocalText("\uae30\uc900 \ud655\uc778 \ubd88\uac00", "target unavailable");
            }

            string role = string.IsNullOrWhiteSpace(roleText)
                ? LocalText("\ud604\uc7ac", "current")
                : roleText.Trim();
            return isInside.Value
                ? string.Format(CultureInfo.CurrentCulture, LocalText("{0} \uae30\uc900 \uc548", "inside {0} target"), role)
                : string.Format(CultureInfo.CurrentCulture, LocalText("{0} \uae30\uc900 \ubc16", "outside {0} target"), role);
        }

        private static bool? IsInsideExpectedMetricRange(double actualValue, VisionPipelineSampleExpectedMetric metric)
        {
            if (metric == null)
            {
                return null;
            }

            bool hasMinimum = TryParseMetricLimit(metric.Minimum, out double minimum);
            bool hasMaximum = TryParseMetricLimit(metric.Maximum, out double maximum);
            if (!hasMinimum && !hasMaximum)
            {
                return null;
            }

            if (hasMinimum && actualValue < minimum)
            {
                return false;
            }

            if (hasMaximum && actualValue > maximum)
            {
                return false;
            }

            return true;
        }

        private static bool TryParseMetricLimit(string text, out double value)
        {
            value = 0D;
            string normalized = text?.Trim() ?? string.Empty;
            if (normalized.Length == 0)
            {
                return false;
            }

            return double.TryParse(normalized, NumberStyles.Float, CultureInfo.InvariantCulture, out value)
                || double.TryParse(normalized, NumberStyles.Float, CultureInfo.CurrentCulture, out value);
        }

        private static string FormatSampleReferenceRole(VisionPipelineSampleCatalogItem sample)
        {
            if (IsOkSampleReference(sample))
            {
                return "OK";
            }

            if (IsNgSampleReference(sample))
            {
                return "NG";
            }

            return LocalText("\uae30\uc900", "Reference");
        }

        private static string FormatPrimaryMetricText(VisionPipelineStep step, VisionPipelineStepResultSummary summary)
        {
            if (summary?.Metrics == null || summary.Metrics.Count == 0)
            {
                return string.Empty;
            }

            KeyValuePair<string, double> metric = OrderResultMetrics(step, summary.Metrics).FirstOrDefault();
            return string.IsNullOrWhiteSpace(metric.Key)
                ? string.Empty
                : string.Format(CultureInfo.CurrentCulture, "{0} {1}", FormatMetricName(metric.Key), FormatMetricValue(metric.Value));
        }

        private static IEnumerable<KeyValuePair<string, double>> OrderResultMetrics(VisionPipelineStep step, IDictionary<string, double> metrics)
        {
            if (metrics == null)
            {
                yield break;
            }

            HashSet<string> emitted = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (string metricName in VisionPipelineKnownMetrics.GetMetricNamesForTool(step?.ToolType))
            {
                if (IsReviewDetailMetric(metricName)
                    && metrics.TryGetValue(metricName, out double value)
                    && emitted.Add(metricName))
                {
                    yield return new KeyValuePair<string, double>(metricName, value);
                }
            }

            foreach (KeyValuePair<string, double> metric in VisionPipelineKnownMetrics.OrderMetrics(metrics))
            {
                if (IsReviewDetailMetric(metric.Key) && emitted.Add(metric.Key))
                {
                    yield return metric;
                }
            }
        }

        private static bool IsReviewDetailMetric(string metricName)
        {
            return !string.Equals(metricName, VisionPipelineKnownMetrics.SourceImageWidth, StringComparison.OrdinalIgnoreCase)
                && !string.Equals(metricName, VisionPipelineKnownMetrics.SourceImageHeight, StringComparison.OrdinalIgnoreCase)
                && !string.Equals(metricName, VisionPipelineKnownMetrics.SourceImageChannels, StringComparison.OrdinalIgnoreCase)
                && !string.Equals(metricName, VisionPipelineKnownMetrics.ResultImageWidth, StringComparison.OrdinalIgnoreCase)
                && !string.Equals(metricName, VisionPipelineKnownMetrics.ResultImageHeight, StringComparison.OrdinalIgnoreCase)
                && !string.Equals(metricName, VisionPipelineKnownMetrics.ResultImageChannels, StringComparison.OrdinalIgnoreCase);
        }

        private static string FormatMetricName(string metricName)
        {
            string name = SafeText(metricName, string.Empty);
            if (string.IsNullOrWhiteSpace(name))
            {
                return "-";
            }

            return T(
                "PipelineReview.Metric." + name,
                VisionPipelineKnownMetrics.GetDisplayName(name));
        }

        private static string FormatMetricValue(double value)
        {
            return Math.Abs(value - Math.Round(value)) < 0.000001
                ? Math.Round(value).ToString("0", CultureInfo.InvariantCulture)
                : value.ToString("0.###", CultureInfo.InvariantCulture);
        }

        private static string Truncate(string value, int maxLength)
        {
            string text = SafeText(value, string.Empty);
            return text.Length <= maxLength ? text : text.Substring(0, Math.Max(0, maxLength - 3)) + "...";
        }

        private static string FormatImageState(string layerName, Bitmap image)
        {
            string title = SafeText(layerName, "-");
            return image == null
                ? title + " / " + T("PipelineReview.ImageMissing", "missing")
                : string.Format(CultureInfo.CurrentCulture, "{0} / {1}x{2}", title, image.Width, image.Height);
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

        private static string LocalText(string korean, string english)
        {
            return OpenVisionLanguageService.CurrentLanguage == OpenVisionLanguage.English
                ? english ?? korean ?? string.Empty
                : korean ?? english ?? string.Empty;
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
