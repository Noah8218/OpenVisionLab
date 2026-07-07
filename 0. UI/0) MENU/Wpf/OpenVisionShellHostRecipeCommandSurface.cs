using Lib.OpenCV.Pipeline;
using Lib.OpenCV.Property;
using OpenVisionLab.Mvvm;
using OpenVisionLab.Vision._1._Tools.OpenCV;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using static OpenVisionLab.DEFINE;

namespace OpenVisionLab
{
    public sealed class OpenVisionShellHostRecipeCommandSurface : ObservableObject
    {
        private readonly Func<string> currentRecipeProvider;
        private readonly Action<string> switchRecipe;
        private readonly Action refreshAfterSwitch;
        private readonly Func<string, bool> confirmDeleteRecipe;
        private readonly Func<string, string, bool> confirmDeletePipeline;
        private readonly Func<string> selectImportPipelineXmlPath;
        private readonly Func<string, string> selectExportPipelineXmlPath;
        private readonly Func<string, OpenVisionRecipeLayerCard> layerCardProvider;
        private readonly Func<string, bool> navigateLayer;
        private readonly Func<string, string, bool> loadImageIntoLayer;
        private readonly Action<VISION_MENU> selectStepTool;
        private readonly Func<bool> commitSelectedStepEdit;
        private readonly IReadOnlyList<string> llmToolTemplateOptions = new[]
        {
            "Pin gap / edge distance (LineDistance)",
            "Line Measurement",
            "Template Matching",
            "Edge Based Matching",
            "Shape boundary (Contour)",
            "Threshold + Blob",
            "Mean Intensity"
        };
        private IReadOnlyList<string> recipeOptions = Array.Empty<string>();
        private IReadOnlyList<string> filteredRecipeOptions = Array.Empty<string>();
        private IReadOnlyList<OpenVisionRecipePipelineOption> pipelineOptions = Array.Empty<OpenVisionRecipePipelineOption>();
        private IReadOnlyList<OpenVisionRecipePipelineOption> filteredPipelineOptions = Array.Empty<OpenVisionRecipePipelineOption>();
        private IReadOnlyList<OpenVisionRecipeSampleOption> sampleOptions = Array.Empty<OpenVisionRecipeSampleOption>();
        private IReadOnlyList<OpenVisionRecipeBatchRunOption> recentBatchRunOptions = Array.Empty<OpenVisionRecipeBatchRunOption>();
        private IReadOnlyList<OpenVisionRecipeBatchRunOption> benchmarkBaselineRunOptions = Array.Empty<OpenVisionRecipeBatchRunOption>();
        private IReadOnlyList<OpenVisionRecipeBatchRunComparisonRow> recentBatchRunComparisonRows = Array.Empty<OpenVisionRecipeBatchRunComparisonRow>();
        private IReadOnlyList<OpenVisionRecipeSampleMatrixRow> sampleMatrixRows = Array.Empty<OpenVisionRecipeSampleMatrixRow>();
        private IReadOnlyList<OpenVisionRecipeDependencyReviewRow> llmXmlDraftDependencyRows = Array.Empty<OpenVisionRecipeDependencyReviewRow>();
        private OpenVisionRecipeBatchRunOption selectedRecentBatchRunOption;
        private OpenVisionRecipeBatchRunOption selectedBenchmarkBaselineRunOption;
        private OpenVisionRecipeBatchSampleResultOption selectedRecentBatchSampleResultOption;
        private OpenVisionRecipeBatchRunComparisonRow selectedRecentBatchRunComparisonRow;
        private OpenVisionRecipeSampleMatrixRow selectedSampleMatrixRow;
        private OpenVisionRecipePipelineStepPreview selectedPipelinePreviewStep;
        private object selectedStepEditObject;
        private string selectedStepEditStatusText = string.Empty;
        private string correctedOutputReviewText = string.Empty;
        private bool selectedStepEditDirty;
        private string selectedRecipeName = string.Empty;
        private string recipeFilterText = string.Empty;
        private string pipelineFilterText = string.Empty;
        private string editRecipeName = string.Empty;
        private string pipelineEditName = string.Empty;
        private string selectedLlmToolTemplate = "Template Matching";
        private string llmInspectionGoalText = string.Empty;
        private string llmDetectionPointText = string.Empty;
        private string pinGapIntentRoiText = string.Empty;
        private string pinGapIntentDistanceMinText = "0.40";
        private string pinGapIntentDistanceMaxText = "0.55";
        private string pinGapIntentRangeMaxText = "0.06";
        private string pinGapIntentScaleText = "0.006";
        private string blobCountIntentRoiText = "0,0,572,420";
        private string blobCountIntentThresholdText = "128";
        private string blobCountIntentMinCountText = "1";
        private string blobCountIntentMaxCountText = "99";
        private string blobCountIntentMinAreaText = "50";
        private string blobCountIntentMaxAreaText = "999999";
        private string contourCountIntentRoiText = "0,0,572,420";
        private string contourCountIntentThresholdText = "150";
        private string contourCountIntentMinCountText = "5";
        private string contourCountIntentMaxCountText = "5";
        private string contourCountIntentMinAreaText = "700";
        private string contourCountIntentMaxAreaText = "9000";
        private string llmPromptText = string.Empty;
        private string llmXmlDraftText = string.Empty;
        private string llmReferenceImagePath = string.Empty;
        private string llmXmlDraftValidationReport = string.Empty;
        private string llmXmlDraftDependencyReport = string.Empty;
        private string llmXmlDraftReviewReport = string.Empty;
        private string llmXmlDraftDiffReport = string.Empty;
        private string llmPromptCopyStatusText = string.Empty;
        private string llmReviewBundleCopyStatusText = string.Empty;
        private string llmXmlDraftPasteStatusText = string.Empty;
        private string operatorHandoffReportStatusText = string.Empty;
        private string selectedRecentBatchRunReviewCopyStatusText = string.Empty;
        private string statusText = string.Empty;
        private bool isRefreshingOptions;
        private bool isSelectingRecipe;
        private bool isSampleCheckRunning;
        private bool isPairCheckRunning;
        private bool isCatalogBenchmarkRunning;
        private OpenVisionRecipePipelineOption selectedPipelineOption;
        private OpenVisionRecipeSampleOption selectedSampleOption;
        private OpenVisionRecipeSampleRunSummary latestSampleRunSummary = OpenVisionRecipeSampleRunSummary.Empty;
        private OpenVisionRecipePairRunSummary latestPairRunSummary = OpenVisionRecipePairRunSummary.Empty;
        private OpenVisionRecipeCatalogBenchmarkSummary latestCatalogBenchmarkSummary = OpenVisionRecipeCatalogBenchmarkSummary.Empty;
        private OpenVisionRecipePairSampleRunSummary selectedPairSampleResult;
        private OpenVisionRecipeManagerSummary selectedRecipeSummary = OpenVisionRecipeManagerSummary.Empty;

        internal OpenVisionShellHostRecipeCommandSurface(
            Func<string> currentRecipeProvider,
            Action<string> switchRecipe,
            Action refreshAfterSwitch,
            Func<string, bool> confirmDeleteRecipe = null,
            Func<string, string, bool> confirmDeletePipeline = null,
            Func<string> selectImportPipelineXmlPath = null,
            Func<string, string> selectExportPipelineXmlPath = null,
            Func<string, OpenVisionRecipeLayerCard> layerCardProvider = null,
            Func<string, bool> navigateLayer = null,
            Func<string, string, bool> loadImageIntoLayer = null,
            Action<VISION_MENU> selectStepTool = null,
            Func<bool> commitSelectedStepEdit = null)
        {
            this.currentRecipeProvider = currentRecipeProvider ?? throw new ArgumentNullException(nameof(currentRecipeProvider));
            this.switchRecipe = switchRecipe ?? throw new ArgumentNullException(nameof(switchRecipe));
            this.refreshAfterSwitch = refreshAfterSwitch ?? throw new ArgumentNullException(nameof(refreshAfterSwitch));
            this.confirmDeleteRecipe = confirmDeleteRecipe ?? (_ => true);
            this.confirmDeletePipeline = confirmDeletePipeline ?? ((_, _) => true);
            this.selectImportPipelineXmlPath = selectImportPipelineXmlPath ?? (() => string.Empty);
            this.selectExportPipelineXmlPath = selectExportPipelineXmlPath ?? (_ => string.Empty);
            this.layerCardProvider = layerCardProvider ?? OpenVisionRecipeLayerCard.CreateMissing;
            this.navigateLayer = navigateLayer ?? (_ => false);
            this.loadImageIntoLayer = loadImageIntoLayer ?? ((_, _) => false);
            this.selectStepTool = selectStepTool;
            this.commitSelectedStepEdit = commitSelectedStepEdit ?? (() => true);

            CreateRecipeCommand = new RelayCommand(CreateRecipe);
            CreateNamedRecipeCommand = new RelayCommand(CreateNamedRecipe, CanCreateNamedRecipe);
            DuplicateRecipeCommand = new RelayCommand(DuplicateSelectedRecipe, CanDuplicateSelectedRecipe);
            RenameRecipeCommand = new RelayCommand(RenameSelectedRecipe, CanRenameSelectedRecipe);
            DeleteRecipeCommand = new RelayCommand(DeleteSelectedRecipe, CanDeleteSelectedRecipe);
            ImportPipelineXmlCommand = new RelayCommand(ImportPipelineXml, CanUseSelectedRecipe);
            ExportPipelineXmlCommand = new RelayCommand(ExportActivePipelineXml, CanUseSelectedRecipe);
            DuplicateFromSampleCommand = new RelayCommand(DuplicatePipelineFromSample, CanDuplicatePipelineFromSample);
            ActivatePipelineCommand = new RelayCommand(ActivateSelectedPipeline, CanUseSelectedPipeline);
            DuplicatePipelineCommand = new RelayCommand(DuplicateSelectedPipeline, CanUseSelectedPipeline);
            RenamePipelineCommand = new RelayCommand(RenameSelectedPipeline, CanRenameSelectedPipeline);
            DeletePipelineCommand = new RelayCommand(DeleteSelectedPipeline, CanDeleteSelectedPipeline);
            LoadLlmXmlDraftCommand = new RelayCommand(LoadLlmXmlDraft, CanUseSelectedRecipe);
            ValidateLlmXmlDraftCommand = new RelayCommand(ValidateLlmXmlDraft, CanUseLlmXmlDraft);
            ImportLlmXmlDraftCommand = new RelayCommand(ImportLlmXmlDraft, CanUseLlmXmlDraft);
            CopyLlmPromptCommand = new RelayCommand(CopyLlmPrompt, CanCopyLlmPrompt);
            CopyLlmReviewBundleCommand = new RelayCommand(CopyLlmReviewBundle, CanCopyLlmReviewBundle);
            PasteLlmXmlDraftFromClipboardCommand = new RelayCommand(PasteLlmXmlDraftFromClipboard);
            UseSelectedSampleReferenceCommand = new RelayCommand(UseSelectedSampleReference, CanUseSelectedSampleReference);
            RunSelectedSampleCheckCommand = new RelayCommand(RunSelectedSampleCheck, CanRunSelectedSampleCheck);
            RunSelectedSamplePairCheckCommand = new RelayCommand(RunSelectedSamplePairCheck, CanRunSelectedSamplePairCheck);
            RunCatalogBenchmarkCommand = new RelayCommand(RunCatalogBenchmark, CanRunCatalogBenchmark);
            SelectPairSampleResultCommand = new RelayCommand<OpenVisionRecipePairSampleRunSummary>(
                SelectPairSampleResult,
                CanSelectPairSampleResult);
            BuildLlmPromptCommand = new RelayCommand(BuildLlmPrompt, CanUseSelectedRecipe);
            CreateLlmTemplateXmlDraftCommand = new RelayCommand(CreateLlmTemplateXmlDraft, CanUseSelectedRecipe);
            CreatePinGapIntentXmlDraftCommand = new RelayCommand(CreatePinGapIntentXmlDraft, CanUseSelectedRecipe);
            CreateBlobCountIntentXmlDraftCommand = new RelayCommand(CreateBlobCountIntentXmlDraft, CanUseSelectedRecipe);
            CreateContourCountIntentXmlDraftCommand = new RelayCommand(CreateContourCountIntentXmlDraft, CanUseSelectedRecipe);
            RefreshLlmDraftReviewCommand = new RelayCommand(RefreshLlmDraftReview, CanUseLlmXmlDraft);
            NavigateSelectedStepInputLayerCommand = new RelayCommand(NavigateSelectedStepInputLayer, CanNavigateSelectedStepInputLayer);
            NavigateSelectedStepOutputLayerCommand = new RelayCommand(NavigateSelectedStepOutputLayer, CanNavigateSelectedStepOutputLayer);
            FocusSelectedRunFailureStepCommand = new RelayCommand(FocusSelectedRunFailureStep, CanFocusSelectedRunFailureStep);
            LoadSelectedRunSampleImageToInputLayerCommand = new RelayCommand(LoadSelectedRunSampleImageToInputLayer, CanLoadSelectedRunSampleImageToInputLayer);
            SelectPreviousPipelinePreviewStepCommand = new RelayCommand(SelectPreviousPipelinePreviewStep, CanSelectPreviousPipelinePreviewStep);
            SelectNextPipelinePreviewStepCommand = new RelayCommand(SelectNextPipelinePreviewStep, CanSelectNextPipelinePreviewStep);
            OpenSelectedStepToolCommand = new RelayCommand(OpenSelectedStepTool, CanOpenSelectedStepTool);
            LoadSelectedStepParametersCommand = new RelayCommand(LoadSelectedStepParameters, CanLoadSelectedStepParameters);
            ApplySelectedStepParametersCommand = new RelayCommand(ApplySelectedStepParameters, CanApplySelectedStepParameters);
            CopyOperatorHandoffReportCommand = new RelayCommand(CopyOperatorHandoffReport, CanCopyOperatorHandoffReport);
            CopySelectedRecentBatchRunReviewCommand = new RelayCommand(CopySelectedRecentBatchRunReview, CanCopySelectedRecentBatchRunReview);
            RunRecipeGuidedNextActionCommand = new RelayCommand(RunRecipeGuidedNextAction, CanRunRecipeGuidedNextAction);
            RefreshSampleOptions();
            RefreshOptions();
        }

        public IReadOnlyList<string> RecipeOptions
        {
            get => recipeOptions;
            private set
            {
                if (SetProperty(ref recipeOptions, value ?? Array.Empty<string>()))
                {
                    OnPropertyChanged(nameof(RecipeLibrarySummaryText));
                }
            }
        }

        public IReadOnlyList<string> FilteredRecipeOptions
        {
            get => filteredRecipeOptions;
            private set
            {
                if (SetProperty(ref filteredRecipeOptions, value ?? Array.Empty<string>()))
                {
                    OnPropertyChanged(nameof(RecipeLibrarySummaryText));
                }
            }
        }

        public IReadOnlyList<OpenVisionRecipePipelineOption> PipelineOptions
        {
            get => pipelineOptions;
            private set
            {
                if (SetProperty(ref pipelineOptions, value ?? Array.Empty<OpenVisionRecipePipelineOption>()))
                {
                    OnPropertyChanged(nameof(PipelineListSummaryText));
                    ApplyPipelineFilter();
                }
            }
        }

        public IReadOnlyList<OpenVisionRecipePipelineOption> FilteredPipelineOptions
        {
            get => filteredPipelineOptions;
            private set
            {
                if (SetProperty(ref filteredPipelineOptions, value ?? Array.Empty<OpenVisionRecipePipelineOption>()))
                {
                    OnPropertyChanged(nameof(PipelineListSummaryText));
                }
            }
        }

        public IReadOnlyList<OpenVisionRecipeSampleOption> SampleOptions
        {
            get => sampleOptions;
            private set => SetProperty(ref sampleOptions, value ?? Array.Empty<OpenVisionRecipeSampleOption>());
        }

        public IReadOnlyList<OpenVisionRecipeBatchRunOption> RecentBatchRunOptions
        {
            get => recentBatchRunOptions;
            private set => SetProperty(ref recentBatchRunOptions, value ?? Array.Empty<OpenVisionRecipeBatchRunOption>());
        }

        public IReadOnlyList<OpenVisionRecipeBatchRunOption> BenchmarkBaselineRunOptions
        {
            get => benchmarkBaselineRunOptions;
            private set => SetProperty(ref benchmarkBaselineRunOptions, value ?? Array.Empty<OpenVisionRecipeBatchRunOption>());
        }

        public IReadOnlyList<OpenVisionRecipeBatchRunComparisonRow> RecentBatchRunComparisonRows
        {
            get => recentBatchRunComparisonRows;
            private set
            {
                if (SetProperty(ref recentBatchRunComparisonRows, value ?? Array.Empty<OpenVisionRecipeBatchRunComparisonRow>()))
                {
                    OnPropertyChanged(nameof(RecentBatchRunComparisonSummaryText));
                }
            }
        }

        public IReadOnlyList<OpenVisionRecipeSampleMatrixRow> SampleMatrixRows
        {
            get => sampleMatrixRows;
            private set
            {
                if (SetProperty(ref sampleMatrixRows, value ?? Array.Empty<OpenVisionRecipeSampleMatrixRow>()))
                {
                    OnPropertyChanged(nameof(SampleMatrixSummaryText));
                }
            }
        }

        public OpenVisionRecipeSampleMatrixRow SelectedSampleMatrixRow
        {
            get => selectedSampleMatrixRow;
            set
            {
                if (SetProperty(ref selectedSampleMatrixRow, value))
                {
                    if (!string.IsNullOrWhiteSpace(value?.FailedStep))
                    {
                        SelectedPipelinePreviewStep = FindPipelinePreviewStep(value.FailedStep);
                    }

                    OnPropertyChanged(nameof(SelectedSampleMatrixReviewText));
                    OnPropertyChanged(nameof(FailureReviewText));
                    OnPropertyChanged(nameof(PipelineSelectedStepOperatorContextText));
                }
            }
        }

        public IReadOnlyList<OpenVisionRecipeDependencyReviewRow> LlmXmlDraftDependencyRows
        {
            get => llmXmlDraftDependencyRows;
            private set => SetProperty(ref llmXmlDraftDependencyRows, value ?? Array.Empty<OpenVisionRecipeDependencyReviewRow>());
        }

        public OpenVisionRecipeBatchRunOption SelectedRecentBatchRunOption
        {
            get => selectedRecentBatchRunOption;
            set
            {
                if (SetProperty(ref selectedRecentBatchRunOption, value))
                {
                    SelectedRecentBatchSampleResultOption = SelectDefaultBatchSampleResult(value);
                    RefreshBenchmarkBaselineRunOptions();
                    RefreshRecentBatchRunComparison();
                    SelectedRecentBatchRunReviewCopyStatusText = string.Empty;
                    OnPropertyChanged(nameof(SelectedRecentBatchRunReviewText));
                    OnPropertyChanged(nameof(FailureReviewText));
                    OnPropertyChanged(nameof(PipelineSelectedStepOperatorContextText));
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public OpenVisionRecipeBatchRunOption SelectedBenchmarkBaselineRunOption
        {
            get => selectedBenchmarkBaselineRunOption;
            set
            {
                if (SetProperty(ref selectedBenchmarkBaselineRunOption, value))
                {
                    RefreshRecentBatchRunComparison();
                    OnPropertyChanged(nameof(RecentBatchRunComparisonSummaryText));
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public OpenVisionRecipeBatchRunComparisonRow SelectedRecentBatchRunComparisonRow
        {
            get => selectedRecentBatchRunComparisonRow;
            set
            {
                if (SetProperty(ref selectedRecentBatchRunComparisonRow, value))
                {
                    OpenVisionRecipeBatchSampleResultOption matchingSample = SelectedRecentBatchRunOption?.SampleResults?
                        .FirstOrDefault(result => result != null
                            && string.Equals(result.SampleName, value?.SampleName, StringComparison.OrdinalIgnoreCase));
                    if (matchingSample != null && !ReferenceEquals(SelectedRecentBatchSampleResultOption, matchingSample))
                    {
                        SelectedRecentBatchSampleResultOption = matchingSample;
                    }

                    if (!string.IsNullOrWhiteSpace(value?.FailedStep))
                    {
                        SelectedPipelinePreviewStep = FindPipelinePreviewStep(value.FailedStep);
                    }

                    OnPropertyChanged(nameof(SelectedRecentBatchRunComparisonReviewText));
                    OnPropertyChanged(nameof(FailureReviewText));
                    OnPropertyChanged(nameof(PipelineSelectedStepOperatorContextText));
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public OpenVisionRecipeBatchSampleResultOption SelectedRecentBatchSampleResultOption
        {
            get => selectedRecentBatchSampleResultOption;
            set
            {
                if (SetProperty(ref selectedRecentBatchSampleResultOption, value))
                {
                    SelectedPipelinePreviewStep = FindPipelinePreviewStep(value?.FailedStep);
                    SelectedRecentBatchRunReviewCopyStatusText = string.Empty;
                    OnPropertyChanged(nameof(SelectedRecentBatchRunReviewText));
                    OnPropertyChanged(nameof(FailureReviewText));
                    OnPropertyChanged(nameof(PipelineSelectedStepOperatorContextText));
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public OpenVisionRecipePipelineStepPreview SelectedPipelinePreviewStep
        {
            get => selectedPipelinePreviewStep;
            set
            {
                if (SetProperty(ref selectedPipelinePreviewStep, value))
                {
                    ClearSelectedStepEdit();
                    RefreshSelectedPipelineStepFlow();
                    OnPropertyChanged(nameof(OpenSelectedStepToolText));
                    OnPropertyChanged(nameof(FailureReviewText));
                    OnPropertyChanged(nameof(PipelineSelectedStepOperatorContextText));
                    OnPropertyChanged(nameof(CorrectedOutputReviewText));
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public object SelectedStepEditObject
        {
            get => selectedStepEditObject;
            private set
            {
                if (SetProperty(ref selectedStepEditObject, value))
                {
                    OnPropertyChanged(nameof(HasSelectedStepEditObject));
                    RefreshCommandState();
                }
            }
        }

        public bool HasSelectedStepEditObject => SelectedStepEditObject != null;

        public string SelectedStepEditStatusText =>
            string.IsNullOrWhiteSpace(selectedStepEditStatusText)
                ? LocalText("Step 파라미터를 불러온 뒤 PropertyGrid에서 검토하고 XML 반영을 누르세요.", "Load step parameters, review them in the PropertyGrid, then apply to XML.")
                : selectedStepEditStatusText;

        public bool IsSelectedStepEditDirty => selectedStepEditDirty;

        public IReadOnlyList<string> LlmToolTemplateOptions => llmToolTemplateOptions;

        public string SelectedRecipeName
        {
            get => selectedRecipeName;
            set => SelectRecipe(value);
        }

        public string RecipeFilterText
        {
            get => recipeFilterText;
            set
            {
                if (!SetProperty(ref recipeFilterText, value ?? string.Empty))
                {
                    return;
                }

                ApplyRecipeFilter();
            }
        }

        public string PipelineFilterText
        {
            get => pipelineFilterText;
            set
            {
                if (!SetProperty(ref pipelineFilterText, value ?? string.Empty))
                {
                    return;
                }

                ApplyPipelineFilter();
            }
        }

        public string EditRecipeName
        {
            get => editRecipeName;
            set
            {
                if (SetProperty(ref editRecipeName, value ?? string.Empty))
                {
                    RefreshCommandState();
                }
            }
        }

        public string PipelineEditName
        {
            get => pipelineEditName;
            set
            {
                if (SetProperty(ref pipelineEditName, value ?? string.Empty))
                {
                    RefreshCommandState();
                }
            }
        }

        public string SelectedLlmToolTemplate
        {
            get => selectedLlmToolTemplate;
            set
            {
                if (SetProperty(ref selectedLlmToolTemplate, string.IsNullOrWhiteSpace(value) ? llmToolTemplateOptions[0] : value))
                {
                    OnPropertyChanged(nameof(LlmResultChannelContractSummaryText));
                    RefreshCommandState();
                }
            }
        }

        public string LlmInspectionGoalText
        {
            get => llmInspectionGoalText;
            set
            {
                if (SetProperty(ref llmInspectionGoalText, value ?? string.Empty))
                {
                    RefreshCommandState();
                }
            }
        }

        public string LlmDetectionPointText
        {
            get => llmDetectionPointText;
            set
            {
                if (SetProperty(ref llmDetectionPointText, value ?? string.Empty))
                {
                    RefreshCommandState();
                }
            }
        }

        public string PinGapIntentRoiText
        {
            get => pinGapIntentRoiText;
            set
            {
                if (SetProperty(ref pinGapIntentRoiText, value ?? string.Empty))
                {
                    OnPropertyChanged(nameof(PinGapIntentWorkflowText));
                    OnPropertyChanged(nameof(PinGapIntentFeedbackText));
                    OnPropertyChanged(nameof(PinGapIntentLatestRunText));
                    RefreshCommandState();
                }
            }
        }

        public string PinGapIntentDistanceMinText
        {
            get => pinGapIntentDistanceMinText;
            set
            {
                if (SetProperty(ref pinGapIntentDistanceMinText, value ?? string.Empty))
                {
                    OnPropertyChanged(nameof(PinGapIntentWorkflowText));
                    OnPropertyChanged(nameof(PinGapIntentFeedbackText));
                    OnPropertyChanged(nameof(PinGapIntentLatestRunText));
                    RefreshCommandState();
                }
            }
        }

        public string PinGapIntentDistanceMaxText
        {
            get => pinGapIntentDistanceMaxText;
            set
            {
                if (SetProperty(ref pinGapIntentDistanceMaxText, value ?? string.Empty))
                {
                    OnPropertyChanged(nameof(PinGapIntentWorkflowText));
                    OnPropertyChanged(nameof(PinGapIntentFeedbackText));
                    OnPropertyChanged(nameof(PinGapIntentLatestRunText));
                    RefreshCommandState();
                }
            }
        }

        public string PinGapIntentRangeMaxText
        {
            get => pinGapIntentRangeMaxText;
            set
            {
                if (SetProperty(ref pinGapIntentRangeMaxText, value ?? string.Empty))
                {
                    OnPropertyChanged(nameof(PinGapIntentWorkflowText));
                    OnPropertyChanged(nameof(PinGapIntentFeedbackText));
                    OnPropertyChanged(nameof(PinGapIntentLatestRunText));
                    RefreshCommandState();
                }
            }
        }

        public string PinGapIntentScaleText
        {
            get => pinGapIntentScaleText;
            set
            {
                if (SetProperty(ref pinGapIntentScaleText, value ?? string.Empty))
                {
                    OnPropertyChanged(nameof(PinGapIntentWorkflowText));
                    OnPropertyChanged(nameof(PinGapIntentFeedbackText));
                    OnPropertyChanged(nameof(PinGapIntentLatestRunText));
                    RefreshCommandState();
                }
            }
        }

        public string BlobCountIntentRoiText
        {
            get => blobCountIntentRoiText;
            set
            {
                if (SetProperty(ref blobCountIntentRoiText, value ?? string.Empty))
                {
                    NotifyBlobCountIntentTextChanged();
                }
            }
        }

        public string BlobCountIntentThresholdText
        {
            get => blobCountIntentThresholdText;
            set
            {
                if (SetProperty(ref blobCountIntentThresholdText, value ?? string.Empty))
                {
                    NotifyBlobCountIntentTextChanged();
                }
            }
        }

        public string BlobCountIntentMinCountText
        {
            get => blobCountIntentMinCountText;
            set
            {
                if (SetProperty(ref blobCountIntentMinCountText, value ?? string.Empty))
                {
                    NotifyBlobCountIntentTextChanged();
                }
            }
        }

        public string BlobCountIntentMaxCountText
        {
            get => blobCountIntentMaxCountText;
            set
            {
                if (SetProperty(ref blobCountIntentMaxCountText, value ?? string.Empty))
                {
                    NotifyBlobCountIntentTextChanged();
                }
            }
        }

        public string BlobCountIntentMinAreaText
        {
            get => blobCountIntentMinAreaText;
            set
            {
                if (SetProperty(ref blobCountIntentMinAreaText, value ?? string.Empty))
                {
                    NotifyBlobCountIntentTextChanged();
                }
            }
        }

        public string BlobCountIntentMaxAreaText
        {
            get => blobCountIntentMaxAreaText;
            set
            {
                if (SetProperty(ref blobCountIntentMaxAreaText, value ?? string.Empty))
                {
                    NotifyBlobCountIntentTextChanged();
                }
            }
        }

        public string ContourCountIntentRoiText
        {
            get => contourCountIntentRoiText;
            set
            {
                if (SetProperty(ref contourCountIntentRoiText, value ?? string.Empty))
                {
                    NotifyContourCountIntentTextChanged();
                }
            }
        }

        public string ContourCountIntentThresholdText
        {
            get => contourCountIntentThresholdText;
            set
            {
                if (SetProperty(ref contourCountIntentThresholdText, value ?? string.Empty))
                {
                    NotifyContourCountIntentTextChanged();
                }
            }
        }

        public string ContourCountIntentMinCountText
        {
            get => contourCountIntentMinCountText;
            set
            {
                if (SetProperty(ref contourCountIntentMinCountText, value ?? string.Empty))
                {
                    NotifyContourCountIntentTextChanged();
                }
            }
        }

        public string ContourCountIntentMaxCountText
        {
            get => contourCountIntentMaxCountText;
            set
            {
                if (SetProperty(ref contourCountIntentMaxCountText, value ?? string.Empty))
                {
                    NotifyContourCountIntentTextChanged();
                }
            }
        }

        public string ContourCountIntentMinAreaText
        {
            get => contourCountIntentMinAreaText;
            set
            {
                if (SetProperty(ref contourCountIntentMinAreaText, value ?? string.Empty))
                {
                    NotifyContourCountIntentTextChanged();
                }
            }
        }

        public string ContourCountIntentMaxAreaText
        {
            get => contourCountIntentMaxAreaText;
            set
            {
                if (SetProperty(ref contourCountIntentMaxAreaText, value ?? string.Empty))
                {
                    NotifyContourCountIntentTextChanged();
                }
            }
        }

        public string LlmPromptText
        {
            get => llmPromptText;
            set
            {
                if (SetProperty(ref llmPromptText, value ?? string.Empty))
                {
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public string LlmXmlDraftText
        {
            get => llmXmlDraftText;
            set
            {
                if (SetProperty(ref llmXmlDraftText, value ?? string.Empty))
                {
                    RefreshCommandState();
                }
            }
        }

        public string LlmReferenceImagePath
        {
            get => llmReferenceImagePath;
            set => SetProperty(ref llmReferenceImagePath, value ?? string.Empty);
        }

        public string LlmXmlDraftValidationReport
        {
            get => llmXmlDraftValidationReport;
            private set => SetProperty(ref llmXmlDraftValidationReport, value ?? string.Empty);
        }

        public string LlmXmlDraftDependencyReport
        {
            get => llmXmlDraftDependencyReport;
            private set => SetProperty(ref llmXmlDraftDependencyReport, value ?? string.Empty);
        }

        public string LlmXmlDraftReviewReport
        {
            get => llmXmlDraftReviewReport;
            private set => SetProperty(ref llmXmlDraftReviewReport, value ?? string.Empty);
        }

        public string LlmXmlDraftDiffReport
        {
            get => llmXmlDraftDiffReport;
            private set => SetProperty(ref llmXmlDraftDiffReport, value ?? string.Empty);
        }

        public string StatusText
        {
            get => statusText;
            private set => SetProperty(ref statusText, value ?? string.Empty);
        }

        public OpenVisionRecipeSampleOption SelectedSampleOption
        {
            get => selectedSampleOption;
            set
            {
                if (SetProperty(ref selectedSampleOption, value))
                {
                    LatestSampleRunSummary = OpenVisionRecipeSampleRunSummary.CreatePending(value);
                    LatestPairRunSummary = OpenVisionRecipePairRunSummary.CreatePending(value);
                    RefreshSampleMatrixRows();
                    OnPropertyChanged(nameof(SelectedSampleAcceptanceSummaryText));
                    OnPropertyChanged(nameof(RunSelectedSampleCheckText));
                    OnPropertyChanged(nameof(RunSelectedSamplePairCheckText));
                    OnPropertyChanged(nameof(RecipeGuidedSetupText));
                    RefreshCommandState();
                }
            }
        }

        public OpenVisionRecipePipelineOption SelectedPipelineOption
        {
            get => selectedPipelineOption;
            set => SelectPipelineOption(value);
        }

        public OpenVisionRecipeManagerSummary SelectedRecipeSummary
        {
            get => selectedRecipeSummary;
            private set
            {
                if (SetProperty(ref selectedRecipeSummary, value ?? OpenVisionRecipeManagerSummary.Empty))
                {
                    NotifyOperatorReviewChanged();
                    OnPropertyChanged(nameof(RecipeGuidedSetupText));
                    RefreshSelectedPipelineStepFlow();
                    SelectedPipelinePreviewStep = FindPipelinePreviewStep(SelectedRecentBatchSampleResultOption?.FailedStep);
                }
            }
        }

        public OpenVisionRecipeSampleRunSummary LatestSampleRunSummary
        {
            get => latestSampleRunSummary;
            private set
            {
                if (SetProperty(ref latestSampleRunSummary, value ?? OpenVisionRecipeSampleRunSummary.Empty))
                {
                    NotifyOperatorReviewChanged();
                    OnPropertyChanged(nameof(RecipeGuidedSetupText));
                    OnPropertyChanged(nameof(PinGapIntentLatestRunText));
                    OnPropertyChanged(nameof(BlobCountIntentLatestRunText));
                    OnPropertyChanged(nameof(ContourCountIntentLatestRunText));
                }
            }
        }

        public OpenVisionRecipePairRunSummary LatestPairRunSummary
        {
            get => latestPairRunSummary;
            private set
            {
                if (SetProperty(ref latestPairRunSummary, value ?? OpenVisionRecipePairRunSummary.Empty))
                {
                    SelectedPairSampleResult = SelectDefaultPairSampleResult(latestPairRunSummary);
                    RefreshSampleMatrixRows();
                    NotifyOperatorReviewChanged();
                    OnPropertyChanged(nameof(FailureReviewText));
                    OnPropertyChanged(nameof(PipelineSelectedStepOperatorContextText));
                    OnPropertyChanged(nameof(RecipeGuidedSetupText));
                }
            }
        }

        public OpenVisionRecipeCatalogBenchmarkSummary LatestCatalogBenchmarkSummary
        {
            get => latestCatalogBenchmarkSummary;
            private set
            {
                if (SetProperty(ref latestCatalogBenchmarkSummary, value ?? OpenVisionRecipeCatalogBenchmarkSummary.Empty))
                {
                    OnPropertyChanged(nameof(CatalogBenchmarkSummaryText));
                    OnPropertyChanged(nameof(CatalogBenchmarkDetailText));
                    OnPropertyChanged(nameof(RecipeGuidedSetupText));
                    NotifyOperatorReviewChanged();
                }
            }
        }

        public OpenVisionRecipePairSampleRunSummary SelectedPairSampleResult
        {
            get => selectedPairSampleResult;
            private set
            {
                if (SetProperty(ref selectedPairSampleResult, value))
                {
                    NotifyOperatorReviewChanged();
                    OnPropertyChanged(nameof(FailureReviewText));
                    OnPropertyChanged(nameof(PipelineSelectedStepOperatorContextText));
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public ICommand CreateRecipeCommand { get; }

        public ICommand CreateNamedRecipeCommand { get; }

        public ICommand DuplicateRecipeCommand { get; }

        public ICommand RenameRecipeCommand { get; }

        public ICommand DeleteRecipeCommand { get; }

        public ICommand ImportPipelineXmlCommand { get; }

        public ICommand ExportPipelineXmlCommand { get; }

        public ICommand DuplicateFromSampleCommand { get; }

        public ICommand ActivatePipelineCommand { get; }

        public ICommand DuplicatePipelineCommand { get; }

        public ICommand RenamePipelineCommand { get; }

        public ICommand DeletePipelineCommand { get; }

        public ICommand LoadLlmXmlDraftCommand { get; }

        public ICommand ValidateLlmXmlDraftCommand { get; }

        public ICommand ImportLlmXmlDraftCommand { get; }

        public ICommand CopyLlmPromptCommand { get; }

        public ICommand CopyLlmReviewBundleCommand { get; }

        public ICommand PasteLlmXmlDraftFromClipboardCommand { get; }

        public ICommand UseSelectedSampleReferenceCommand { get; }

        public ICommand RunSelectedSampleCheckCommand { get; }

        public ICommand RunSelectedSamplePairCheckCommand { get; }

        public ICommand RunCatalogBenchmarkCommand { get; }

        public ICommand SelectPairSampleResultCommand { get; }

        public ICommand BuildLlmPromptCommand { get; }

        public ICommand CreateLlmTemplateXmlDraftCommand { get; }

        public ICommand CreatePinGapIntentXmlDraftCommand { get; }

        public ICommand CreateBlobCountIntentXmlDraftCommand { get; }

        public ICommand CreateContourCountIntentXmlDraftCommand { get; }

        public ICommand RefreshLlmDraftReviewCommand { get; }

        public ICommand NavigateSelectedStepInputLayerCommand { get; }

        public ICommand NavigateSelectedStepOutputLayerCommand { get; }

        public ICommand FocusSelectedRunFailureStepCommand { get; }

        public ICommand LoadSelectedRunSampleImageToInputLayerCommand { get; }

        public ICommand SelectPreviousPipelinePreviewStepCommand { get; }

        public ICommand SelectNextPipelinePreviewStepCommand { get; }

        public ICommand OpenSelectedStepToolCommand { get; }

        public ICommand LoadSelectedStepParametersCommand { get; }

        public ICommand ApplySelectedStepParametersCommand { get; }

        public ICommand CopyOperatorHandoffReportCommand { get; }

        public ICommand CopySelectedRecentBatchRunReviewCommand { get; }

        public ICommand RunRecipeGuidedNextActionCommand { get; }

        public string NewRecipeButtonText => LocalText("새 레시피", "New recipe");

        public string RecipeSelectorToolTipText => LocalText("레시피 선택 / 전환", "Select or switch recipe");

        public string ManagerButtonText => LocalText("레시피 관리", "Manage recipes");

        public string ManagerButtonShortText => LocalText("관리", "Manage");

        public string ManagerTitleText => LocalText("레시피 관리", "Recipe manager");

        public string ManagerWorkbenchText => LocalText("워크벤치", "Workbench");

        public string RecipeListText => LocalText("레시피 목록", "Recipe list");

        public string RecipeLibraryText => LocalText("레시피 라이브러리", "Recipe library");

        public string RecipeLibrarySummaryText
        {
            get
            {
                int total = RecipeOptions?.Count ?? 0;
                int visible = FilteredRecipeOptions?.Count ?? 0;
                if (total <= 0)
                {
                    return RecipeLibraryText;
                }

                return visible == total
                    ? string.Format(CultureInfo.CurrentCulture, "{0} ({1})", RecipeLibraryText, total)
                    : string.Format(CultureInfo.CurrentCulture, "{0} ({1}/{2})", RecipeLibraryText, visible, total);
            }
        }

        public string ReviewWorkspaceText => LocalText("검토 작업면", "Review workspace");

        public string RecipeGuidedSetupText => BuildRecipeGuidedSetupText();

        public string RecipeGuidedNextActionText => BuildRecipeGuidedNextActionText();

        public string RecipeFilterLabelText => LocalText("검색", "Search");

        public string EditRecipeNameLabelText => LocalText("선택/새 이름", "Selected/new name");

        public string CreateNamedRecipeText => LocalText("새로 만들기", "Create");

        public string DuplicateRecipeText => LocalText("복제", "Duplicate");

        public string RenameRecipeText => LocalText("이름 변경", "Rename");

        public string DeleteRecipeText => LocalText("삭제", "Delete");

        public string ImportPipelineXmlText => LocalText("XML 가져오기", "Import XML");

        public string ExportPipelineXmlText => LocalText("XML 내보내기", "Export XML");

        public string RecipeDetailText => LocalText("레시피 상세", "Recipe details");

        public string RecipePipelineTabText => LocalText("파이프라인", "Pipeline");

        public string RecipeLlmXmlTabText => LocalText("LLM XML", "LLM XML");

        public string RecipePreviewTabText => LocalText("미리보기", "Preview");

        public string DuplicateFromSampleText => LocalText("샘플 복제", "Sample copy");

        public string PipelineListText => LocalText("파이프라인", "Pipelines");

        public string PipelineListSummaryText
        {
            get
            {
                int total = PipelineOptions?.Count ?? 0;
                int visible = FilteredPipelineOptions?.Count ?? 0;
                if (total <= 0)
                {
                    return PipelineListText;
                }

                return visible == total
                    ? string.Format(CultureInfo.CurrentCulture, "{0} ({1})", PipelineListText, total)
                    : string.Format(CultureInfo.CurrentCulture, "{0} ({1}/{2})", PipelineListText, visible, total);
            }
        }

        public string PipelineFilterLabelText => LocalText("검색", "Search");

        public string PipelineNameText => LocalText("파이프라인 이름", "Pipeline name");

        public string ActivatePipelineText => LocalText("활성화", "Active");

        public string DuplicatePipelineText => LocalText("복제", "Duplicate");

        public string RenamePipelineText => LocalText("이름 변경", "Rename");

        public string DeletePipelineText => LocalText("삭제", "Delete");

        public string SampleSourceText => LocalText("샘플 소스", "Sample source");

        public string SampleAcceptanceText => LocalText("샘플 판정 기준", "Sample acceptance");

        public string SampleCheckResultText => LocalText("샘플 검사 결과", "Sample check result");

        public string PairCheckResultText => LocalText("Good/Bad 쌍 검사", "Good/Bad pair check");

        public string SampleMatrixText => LocalText("샘플 매트릭스", "Sample matrix");

        public string SampleMatrixSummaryText => BuildSampleMatrixSummaryText();

        public string SelectedSampleMatrixReviewText =>
            SelectedSampleMatrixRow?.ReviewText
            ?? LocalText("샘플 매트릭스 행을 선택하면 기대 기준, 현재 결과, 다음 조치가 표시됩니다.", "Select a sample matrix row to see its expected gate, current result, and next action.");

        public string RecentBatchRunsText => LocalText("최근 쌍 검사 이력", "Recent pair check runs");

        public string RecentBatchRunSampleResultsText => LocalText("선택 이력 샘플 결과", "Selected run sample results");

        public string RecentBatchRunComparisonText => LocalText("Benchmark 회귀 비교", "Benchmark regression diff");

        public string BenchmarkBaselineRunText => LocalText("기준 실행", "Baseline run");

        public string RecentBatchRunComparisonSummaryText => BuildRecentBatchRunComparisonSummaryText();

        public string SelectedRecentBatchRunComparisonReviewText =>
            SelectedRecentBatchRunComparisonRow?.ReviewText
            ?? LocalText("비교 행을 선택하면 이전 실행 대비 변화와 다음 조치가 표시됩니다.", "Select a diff row to see the change from the previous run and next action.");

        public string SelectedRecentBatchRunReviewLabelText => LocalText("선택 이력 판독", "Selected run review");

        public string SelectedRecentBatchRunReviewText => BuildSelectedRecentBatchRunReviewText();

        public string CopySelectedRecentBatchRunReviewText => LocalText("판독 복사", "Copy review");

        public string CatalogBenchmarkText => LocalText("카탈로그 벤치마크", "Catalog benchmark");

        public string RunCatalogBenchmarkText =>
            isCatalogBenchmarkRunning ? LocalText("실행 중...", "Running...") : LocalText("전체 샘플 검사", "Run catalog");

        public string RunCatalogBenchmarkShortText =>
            isCatalogBenchmarkRunning ? LocalText("실행 중", "Running") : LocalText("카탈로그", "Catalog");

        public string CatalogBenchmarkSummaryText =>
            LatestCatalogBenchmarkSummary?.CompactText
            ?? OpenVisionRecipeCatalogBenchmarkSummary.Empty.CompactText;

        public string CatalogBenchmarkDetailText =>
            LatestCatalogBenchmarkSummary?.DetailText
            ?? OpenVisionRecipeCatalogBenchmarkSummary.Empty.DetailText;

        public string SelectedRecentBatchRunReviewCopyStatusText
        {
            get => selectedRecentBatchRunReviewCopyStatusText;
            private set => SetProperty(ref selectedRecentBatchRunReviewCopyStatusText, value ?? string.Empty);
        }

        public string RunSelectedSampleCheckText => isSampleCheckRunning ? LocalText("실행 중...", "Running...") : LocalText("검사 실행", "Run check");

        public string RunSelectedSamplePairCheckText => isPairCheckRunning ? LocalText("실행 중...", "Running...") : LocalText("쌍 검사", "Run pair");

        public string SelectedSampleAcceptanceSummaryText =>
            SelectedSampleOption?.AcceptanceSummaryText ?? LocalText("기대 지표 기준을 확인할 샘플을 선택하세요.", "Select a sample to review expected metric gates.");

        public string OperatorReviewText => LocalText("작업자 검토", "Operator review");

        public string PipelineReviewTabText => LocalText("검토", "Review");

        public string PipelineReportTabText => LocalText("리포트", "Report");

        public string PipelineRunHistoryTabText => LocalText("이력", "Runs");

        public string PipelineXmlStepTabText => LocalText("XML/Step", "XML/Steps");

        public string OperatorRunReviewLabelText => LocalText("실행 판정 요약", "Run review summary");

        public string OperatorRunReviewText => BuildOperatorRunReviewText() + BuildSelectedPairRoleRunReviewSuffix();

        public string OperatorDecisionBoardText => LocalText("작업자 판정 보드", "Operator decision board");

        public string OperatorValidationChecklistText => LocalText("검증 체크리스트", "Validation checklist");

        public IReadOnlyList<OpenVisionRecipeOperatorValidationRow> OperatorValidationChecklistRows => BuildOperatorValidationChecklistRows();

        public string OperatorResultChannelsText => LocalText("판정 출력 정의", "Judgement outputs");

        public IReadOnlyList<OpenVisionRecipeOperatorResultChannelRow> OperatorResultChannelRows => BuildOperatorResultChannelRows();

        public IReadOnlyList<OpenVisionRecipeOperatorResultChannelRow> OperatorResultChannelBoardRows => BuildOperatorResultChannelRows();

        public string OperatorDecisionXmlCardText => BuildOperatorDecisionXmlCardText();

        public string OperatorDecisionSampleCardText => BuildOperatorDecisionSampleCardText();

        public string OperatorDecisionPairCardText => BuildOperatorDecisionPairCardText();

        public string OperatorDecisionNextActionText => BuildOperatorDecisionNextActionText();

        public string OperatorHandoffReportText => BuildOperatorHandoffReportText();

        public string CopyOperatorHandoffReportText => LocalText("리포트 복사", "Copy report");

        public string OperatorHandoffReportStatusText
        {
            get => operatorHandoffReportStatusText;
            private set => SetProperty(ref operatorHandoffReportStatusText, value ?? string.Empty);
        }

        public string FailureReviewLabelText => LocalText("실패 Step 재검사 / 비교", "Failed step rerun / comparison");

        public string FailureReviewText => BuildFailureReviewText();

        public string ViewFailureInputLayerText => LocalText("입력 보기", "View input");

        public string ViewFailureOutputLayerText => LocalText("출력 보기", "View output");

        public string FocusSelectedRunFailureStepText => LocalText("실패 Step", "Failed step");

        public string LoadSelectedRunSampleImageToInputLayerText => LocalText("샘플->입력", "Sample -> input");

        public string RerunFailurePairCheckText => LocalText("Good/Bad 재검사", "Rerun Good/Bad");

        public string LoadFailureStepParametersText => LocalText("파라미터 검토", "Review parameters");

        private string BuildSelectedPairRoleRunReviewSuffix()
        {
            if (SelectedPairSampleResult == null)
            {
                return string.Empty;
            }

            return Environment.NewLine
                + OpenVisionRecipeText.Local("역할 리뷰: ", "Role review: ") + SelectedPairSampleResult.Role + " / " + SelectedPairSampleResult.ResultText
                + Environment.NewLine
                + OpenVisionRecipeText.Local("역할 다음: ", "Role next: ") + SelectedPairSampleResult.NextActionText;
        }

        public string LlmXmlValidationReportText => LocalText("LLM XML 검증 보고서", "LLM XML validation report");

        public string PipelinePreviewStepListText => LocalText("파이프라인 미리보기 단계 목록", "Pipeline preview step list");

        public string PipelineStepComparisonText => LocalText("Step 비교표", "Step comparison");

        public string PipelineSelectedStepDetailText => LocalText("선택 Step 상세", "Selected step detail");

        public string PipelineSelectedStepRouteText => LocalText("입출력 레이어", "Input/output layers");

        public string PipelineSelectedStepInputLayerText => LocalText("입력 레이어", "Input layer");

        public string PipelineSelectedStepOutputLayerText => LocalText("결과 레이어", "Output layer");

        public string PipelineSelectedStepAcceptanceText => LocalText("판정 기준", "Acceptance gate");

        public string PipelineSelectedStepParametersText => LocalText("전체 파라미터", "Full parameters");

        public string PipelineSelectedStepRoiTemplateText => LocalText("ROI / 템플릿", "ROI / template");

        public string PipelineSelectedStepPropertyGridText => LocalText("Step PropertyGrid 검토", "Step PropertyGrid review");

        public string PipelineStepFlowText => LocalText("Step 흐름 포커스", "Step flow focus");

        public string PipelineSelectedStepOperatorContextText => BuildPipelineSelectedStepOperatorContextText();

        public string PipelineStepFlowReviewText => BuildPipelineStepFlowReviewText();

        public string BranchOutputComparisonText => BuildBranchOutputComparisonText();

        public IReadOnlyList<OpenVisionRecipeBranchOutputComparisonRow> BranchOutputComparisonRows => BuildBranchOutputComparisonRows();

        public string PreviousPipelineStepText => BuildPipelineStepSlotText(GetPipelinePreviewStepByOffset(-1), LocalText("이전 Step 없음", "No previous step"));

        public string CurrentPipelineStepText => BuildPipelineStepSlotText(SelectedPipelinePreviewStep, LocalText("선택 Step 없음", "No selected step"));

        public string NextPipelineStepText => BuildPipelineStepSlotText(GetPipelinePreviewStepByOffset(1), LocalText("다음 Step 없음", "No next step"));

        public string SelectPreviousPipelineStepText => LocalText("이전", "Previous");

        public string SelectNextPipelineStepText => LocalText("다음", "Next");

        public string LoadSelectedStepParametersText => LocalText("파라미터 불러오기", "Load parameters");

        public string ApplySelectedStepParametersText => LocalText("XML 반영", "Apply to XML");

        public string CorrectedOutputReviewLabelText => LocalText("수정 출력 확인", "Corrected output review");

        public string CorrectedOutputReviewText =>
            string.IsNullOrWhiteSpace(correctedOutputReviewText)
                ? BuildCorrectedOutputReviewText()
                : correctedOutputReviewText;

        public string OpenSelectedStepToolText =>
            SelectedPipelinePreviewStep?.EditorActionText
            ?? LocalText("도구 열기", "Open tool");

        public string LlmAssistantText => LocalText("LLM 어시스턴트", "LLM assistant");

        public string LlmToolTemplateText => LocalText("검사 의도", "Inspection intent");

        public string LlmInspectionGoalLabelText => LocalText("검사 목표", "Inspection goal");

        public string LlmDetectionPointLabelText => LocalText("검출 포인트", "Detection points");

        public string PinGapIntentSkillText => LocalText("핀 간격 skill", "Pin gap skill");

        public string PinGapIntentRoiLabelText => LocalText("ROI", "ROI");

        public string PinGapIntentDistanceMinLabelText => LocalText("Min mm", "Min mm");

        public string PinGapIntentDistanceMaxLabelText => LocalText("Max mm", "Max mm");

        public string PinGapIntentRangeMaxLabelText => LocalText("Range", "Range");

        public string PinGapIntentScaleLabelText => LocalText("mm/px", "mm/px");

        public string CreatePinGapIntentXmlText => LocalText("핀 간격 XML", "Pin gap XML");

        public string PinGapIntentWorkflowText =>
            LocalText("판정: DistanceMmAvg ", "Gates: DistanceMmAvg ")
            + PinGapIntentDistanceMinText
            + ".."
            + PinGapIntentDistanceMaxText
            + LocalText(" mm, DistanceMmRange <= ", " mm, DistanceMmRange <= ")
            + PinGapIntentRangeMaxText
            + LocalText(
                " mm / 다음: Pin gap XML -> 검증 -> 가져오기 -> 샘플 실행으로 ROI/scale 튜닝",
                " mm / Next: Pin gap XML -> Validate -> Import -> run sample to tune ROI/scale");

        public string PinGapIntentFeedbackText =>
            LocalText(
                "Feedback: Avg NG는 mm/px 또는 Min/Max spec을 조정합니다. Range NG/긴 선/허공 검출은 ROI를 실제 핀 간격만 포함하게 줄이고 edge contrast/sampling을 조정합니다.",
                "Feedback: Avg NG means tune mm/px or Min/Max spec. Range NG/long line/empty-space hit means narrow ROI to the real pin gap and tune edge contrast/sampling.");

        public string PinGapIntentLatestRunText => BuildPinGapIntentLatestRunText();

        public string BlobCountIntentSkillText => LocalText("Blob count skill", "Blob count skill");

        public string BlobCountIntentRoiLabelText => LocalText("ROI", "ROI");

        public string BlobCountIntentThresholdLabelText => LocalText("Threshold", "Threshold");

        public string BlobCountIntentMinCountLabelText => LocalText("Min count", "Min count");

        public string BlobCountIntentMaxCountLabelText => LocalText("Max count", "Max count");

        public string BlobCountIntentMinAreaLabelText => LocalText("Min area", "Min area");

        public string BlobCountIntentMaxAreaLabelText => LocalText("Max area", "Max area");

        public string CreateBlobCountIntentXmlText => LocalText("Blob count XML", "Blob count XML");

        public string BlobCountIntentWorkflowText =>
            LocalText("Gates: ResultCount ", "Gates: ResultCount ")
            + BlobCountIntentMinCountText
            + ".."
            + BlobCountIntentMaxCountText
            + LocalText(" / area ", " / area ")
            + BlobCountIntentMinAreaText
            + ".."
            + BlobCountIntentMaxAreaText
            + LocalText(" / Next: Blob count XML -> Validate -> Import -> run sample to tune threshold/ROI/area", " / Next: Blob count XML -> Validate -> Import -> run sample to tune threshold/ROI/area");

        public string BlobCountIntentFeedbackText =>
            LocalText(
                "Feedback: Count NG means tune threshold, ROI, or area limits. Noise means raise Min area; missing targets means lower threshold or widen ROI.",
                "Feedback: Count NG means tune threshold, ROI, or area limits. Noise means raise Min area; missing targets means lower threshold or widen ROI.");

        public string BlobCountIntentLatestRunText => BuildBlobCountIntentLatestRunText();

        public string ContourCountIntentSkillText => LocalText("Contour count/size skill", "Contour count/size skill");

        public string ContourCountIntentRoiLabelText => LocalText("ROI", "ROI");

        public string ContourCountIntentThresholdLabelText => LocalText("Threshold", "Threshold");

        public string ContourCountIntentMinCountLabelText => LocalText("Min count", "Min count");

        public string ContourCountIntentMaxCountLabelText => LocalText("Max count", "Max count");

        public string ContourCountIntentMinAreaLabelText => LocalText("Min area", "Min area");

        public string ContourCountIntentMaxAreaLabelText => LocalText("Max area", "Max area");

        public string CreateContourCountIntentXmlText => LocalText("Contour XML", "Contour XML");

        public string ContourCountIntentWorkflowText =>
            LocalText("Gates: ResultCount ", "Gates: ResultCount ")
            + ContourCountIntentMinCountText
            + ".."
            + ContourCountIntentMaxCountText
            + LocalText(", AreaMax <= ", ", AreaMax <= ")
            + ContourCountIntentMaxAreaText
            + LocalText(" / Review overlay -> Next: Contour XML -> Validate -> Import -> run sample to tune threshold/ROI/area", " / Review overlay -> Next: Contour XML -> Validate -> Import -> run sample to tune threshold/ROI/area");

        public string ContourCountIntentFeedbackText =>
            LocalText(
                "Feedback: Count NG means tune threshold, ROI, or area limits. AreaMax NG means split/limit oversized shapes before accepting the recipe.",
                "Feedback: Count NG means tune threshold, ROI, or area limits. AreaMax NG means split/limit oversized shapes before accepting the recipe.");

        public string ContourCountIntentLatestRunText => BuildContourCountIntentLatestRunText();

        public string LlmResultChannelContractSummaryText =>
            LocalText("선택 의도는 도구군을 고정합니다: ", "Selected intent locks tool family: ")
            + ResolveIntentSummary(SelectedLlmToolTemplate)
            + LocalText(
                " / 출력 채널은 XML 검증과 명시적 샘플 실행에서 파생됩니다.",
                " / Result channels are derived from XML validation and explicit sample runs.");

        public string BuildLlmPromptButtonText => LocalText("프롬프트 생성", "Build prompt");

        public string CopyLlmPromptText => LocalText("프롬프트 복사", "Copy prompt");

        public string LlmPromptCopyStatusText
        {
            get => llmPromptCopyStatusText;
            private set => SetProperty(ref llmPromptCopyStatusText, value ?? string.Empty);
        }

        public string CreateLlmTemplateXmlText => LocalText("XML 시작안", "XML starter");

        public string RefreshLlmDraftReviewText => LocalText("검토", "Review");

        public string LlmPromptPreviewText => LocalText("프롬프트 미리보기", "Prompt preview");

        public string LlmXmlDraftLabelText => LocalText("LLM XML 초안", "LLM XML draft");

        public string CopyLlmReviewBundleText => LocalText("검토 복사", "Copy review");

        public string PasteLlmXmlDraftText => LocalText("XML 붙여넣기", "Paste XML");

        public string LlmReviewBundleCopyStatusText
        {
            get => llmReviewBundleCopyStatusText;
            private set => SetProperty(ref llmReviewBundleCopyStatusText, value ?? string.Empty);
        }

        public string LlmXmlDraftPasteStatusText
        {
            get => llmXmlDraftPasteStatusText;
            private set => SetProperty(ref llmXmlDraftPasteStatusText, value ?? string.Empty);
        }

        public string LoadLlmXmlDraftText => LocalText("XML 로드", "Load XML");

        public string ValidateLlmXmlDraftButtonText => LocalText("검증", "Validate");

        public string ImportLlmXmlDraftText => LocalText("가져오기", "Import");

        public string UseSelectedSampleReferenceText => LocalText("샘플 사용", "Use sample");

        public string LlmReferenceImageText => LocalText("참조 이미지", "Reference image");

        public string LlmDraftValidationText => LocalText("초안 검증", "Draft validation");

        public string LlmDependencyReportText => LocalText("의존 파일 복사 보고서", "Dependency copy report");

        public string LlmDependencyPathRowsText => LocalText("경로 검토", "Path review");

        public string LlmDraftReviewReportText => LocalText("초안 가져오기 검토", "Draft import review");

        public string LlmDraftDiffReportText => LocalText("LLM XML 변경점", "LLM XML diff review");

        public string RecipeEditValidationText => BuildRecipeEditValidationText();

        public string PipelineEditValidationText => BuildPipelineEditValidationText();

        public void RefreshOptions()
        {
            if (isRefreshingOptions)
            {
                return;
            }

            try
            {
                isRefreshingOptions = true;
                string current = NormalizeRecipeName(currentRecipeProvider());
                IReadOnlyList<string> names = RecipeWorkspaceService.GetRecipeNames()
                    .Append(current)
                    .Where(name => !string.IsNullOrWhiteSpace(name))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .OrderBy(name => name, StringComparer.OrdinalIgnoreCase)
                    .ToList();

                SetSelectedRecipeName(current);
                RecipeOptions = names;
                ApplyRecipeFilter();
            }
            finally
            {
                isRefreshingOptions = false;
            }

            RefreshCommandState();
        }

        public void RefreshLocalization()
        {
            OnPropertyChanged(nameof(NewRecipeButtonText));
            OnPropertyChanged(nameof(RecipeSelectorToolTipText));
            OnPropertyChanged(nameof(ManagerButtonText));
            OnPropertyChanged(nameof(ManagerButtonShortText));
            OnPropertyChanged(nameof(ManagerTitleText));
            OnPropertyChanged(nameof(ManagerWorkbenchText));
            OnPropertyChanged(nameof(RecipeListText));
            OnPropertyChanged(nameof(RecipeLibraryText));
            OnPropertyChanged(nameof(RecipeLibrarySummaryText));
            OnPropertyChanged(nameof(ReviewWorkspaceText));
            OnPropertyChanged(nameof(RecipeGuidedSetupText));
            OnPropertyChanged(nameof(RecipeGuidedNextActionText));
            OnPropertyChanged(nameof(RecipeFilterLabelText));
            OnPropertyChanged(nameof(EditRecipeNameLabelText));
            OnPropertyChanged(nameof(CreateNamedRecipeText));
            OnPropertyChanged(nameof(DuplicateRecipeText));
            OnPropertyChanged(nameof(RenameRecipeText));
            OnPropertyChanged(nameof(DeleteRecipeText));
            OnPropertyChanged(nameof(ImportPipelineXmlText));
            OnPropertyChanged(nameof(ExportPipelineXmlText));
            OnPropertyChanged(nameof(RecipeDetailText));
            OnPropertyChanged(nameof(RecipePipelineTabText));
            OnPropertyChanged(nameof(RecipeLlmXmlTabText));
            OnPropertyChanged(nameof(RecipePreviewTabText));
            OnPropertyChanged(nameof(DuplicateFromSampleText));
            OnPropertyChanged(nameof(PipelineListText));
            OnPropertyChanged(nameof(PipelineListSummaryText));
            OnPropertyChanged(nameof(PipelineFilterLabelText));
            OnPropertyChanged(nameof(PipelineNameText));
            OnPropertyChanged(nameof(ActivatePipelineText));
            OnPropertyChanged(nameof(DuplicatePipelineText));
            OnPropertyChanged(nameof(RenamePipelineText));
            OnPropertyChanged(nameof(DeletePipelineText));
            OnPropertyChanged(nameof(SampleSourceText));
            OnPropertyChanged(nameof(SampleAcceptanceText));
            OnPropertyChanged(nameof(SampleCheckResultText));
            OnPropertyChanged(nameof(PairCheckResultText));
            OnPropertyChanged(nameof(SampleMatrixText));
            OnPropertyChanged(nameof(SampleMatrixSummaryText));
            OnPropertyChanged(nameof(SelectedSampleMatrixReviewText));
            OnPropertyChanged(nameof(RecentBatchRunsText));
            OnPropertyChanged(nameof(RecentBatchRunSampleResultsText));
            OnPropertyChanged(nameof(RecentBatchRunComparisonText));
            OnPropertyChanged(nameof(BenchmarkBaselineRunText));
            OnPropertyChanged(nameof(RecentBatchRunComparisonSummaryText));
            OnPropertyChanged(nameof(SelectedRecentBatchRunComparisonReviewText));
            OnPropertyChanged(nameof(SelectedRecentBatchRunReviewLabelText));
            OnPropertyChanged(nameof(SelectedRecentBatchRunReviewText));
            OnPropertyChanged(nameof(CopySelectedRecentBatchRunReviewText));
            OnPropertyChanged(nameof(CatalogBenchmarkText));
            OnPropertyChanged(nameof(RunCatalogBenchmarkText));
            OnPropertyChanged(nameof(RunCatalogBenchmarkShortText));
            OnPropertyChanged(nameof(CatalogBenchmarkSummaryText));
            OnPropertyChanged(nameof(CatalogBenchmarkDetailText));
            OnPropertyChanged(nameof(RunSelectedSampleCheckText));
            OnPropertyChanged(nameof(RunSelectedSamplePairCheckText));
            OnPropertyChanged(nameof(SelectedSampleAcceptanceSummaryText));
            OnPropertyChanged(nameof(OperatorReviewText));
            OnPropertyChanged(nameof(PipelineReviewTabText));
            OnPropertyChanged(nameof(PipelineReportTabText));
            OnPropertyChanged(nameof(PipelineRunHistoryTabText));
            OnPropertyChanged(nameof(PipelineXmlStepTabText));
            OnPropertyChanged(nameof(OperatorRunReviewLabelText));
            NotifyOperatorReviewChanged();
            OnPropertyChanged(nameof(OperatorDecisionBoardText));
            OnPropertyChanged(nameof(CopyOperatorHandoffReportText));
            OnPropertyChanged(nameof(FailureReviewLabelText));
            OnPropertyChanged(nameof(FailureReviewText));
            OnPropertyChanged(nameof(ViewFailureInputLayerText));
            OnPropertyChanged(nameof(ViewFailureOutputLayerText));
            OnPropertyChanged(nameof(FocusSelectedRunFailureStepText));
            OnPropertyChanged(nameof(LoadSelectedRunSampleImageToInputLayerText));
            OnPropertyChanged(nameof(RerunFailurePairCheckText));
            OnPropertyChanged(nameof(LoadFailureStepParametersText));
            OnPropertyChanged(nameof(LlmXmlValidationReportText));
            OnPropertyChanged(nameof(PipelinePreviewStepListText));
            OnPropertyChanged(nameof(PipelineStepComparisonText));
            OnPropertyChanged(nameof(PipelineSelectedStepDetailText));
            OnPropertyChanged(nameof(PipelineSelectedStepOperatorContextText));
            OnPropertyChanged(nameof(PipelineSelectedStepRouteText));
            OnPropertyChanged(nameof(PipelineSelectedStepInputLayerText));
            OnPropertyChanged(nameof(PipelineSelectedStepOutputLayerText));
            OnPropertyChanged(nameof(PipelineSelectedStepAcceptanceText));
            OnPropertyChanged(nameof(PipelineSelectedStepParametersText));
            OnPropertyChanged(nameof(PipelineSelectedStepRoiTemplateText));
            OnPropertyChanged(nameof(PipelineSelectedStepPropertyGridText));
            OnPropertyChanged(nameof(PipelineStepFlowText));
            OnPropertyChanged(nameof(PipelineStepFlowReviewText));
            OnPropertyChanged(nameof(BranchOutputComparisonText));
            OnPropertyChanged(nameof(BranchOutputComparisonRows));
            OnPropertyChanged(nameof(PreviousPipelineStepText));
            OnPropertyChanged(nameof(CurrentPipelineStepText));
            OnPropertyChanged(nameof(NextPipelineStepText));
            OnPropertyChanged(nameof(SelectPreviousPipelineStepText));
            OnPropertyChanged(nameof(SelectNextPipelineStepText));
            OnPropertyChanged(nameof(LoadSelectedStepParametersText));
            OnPropertyChanged(nameof(ApplySelectedStepParametersText));
            OnPropertyChanged(nameof(CorrectedOutputReviewLabelText));
            OnPropertyChanged(nameof(CorrectedOutputReviewText));
            OnPropertyChanged(nameof(SelectedStepEditStatusText));
            OnPropertyChanged(nameof(OpenSelectedStepToolText));
            OnPropertyChanged(nameof(LlmAssistantText));
            OnPropertyChanged(nameof(LlmToolTemplateText));
            OnPropertyChanged(nameof(LlmInspectionGoalLabelText));
            OnPropertyChanged(nameof(LlmDetectionPointLabelText));
            OnPropertyChanged(nameof(LlmResultChannelContractSummaryText));
            OnPropertyChanged(nameof(PinGapIntentWorkflowText));
            OnPropertyChanged(nameof(PinGapIntentFeedbackText));
            OnPropertyChanged(nameof(PinGapIntentLatestRunText));
            OnPropertyChanged(nameof(BuildLlmPromptButtonText));
            OnPropertyChanged(nameof(CopyLlmPromptText));
            OnPropertyChanged(nameof(CreateLlmTemplateXmlText));
            OnPropertyChanged(nameof(RefreshLlmDraftReviewText));
            OnPropertyChanged(nameof(LlmPromptPreviewText));
            OnPropertyChanged(nameof(LlmXmlDraftLabelText));
            OnPropertyChanged(nameof(CopyLlmReviewBundleText));
            OnPropertyChanged(nameof(PasteLlmXmlDraftText));
            OnPropertyChanged(nameof(LoadLlmXmlDraftText));
            OnPropertyChanged(nameof(ValidateLlmXmlDraftButtonText));
            OnPropertyChanged(nameof(ImportLlmXmlDraftText));
            OnPropertyChanged(nameof(UseSelectedSampleReferenceText));
            OnPropertyChanged(nameof(LlmReferenceImageText));
            OnPropertyChanged(nameof(LlmDraftValidationText));
            OnPropertyChanged(nameof(LlmDependencyReportText));
            OnPropertyChanged(nameof(LlmDependencyPathRowsText));
            OnPropertyChanged(nameof(LlmDraftReviewReportText));
            OnPropertyChanged(nameof(LlmDraftDiffReportText));
            OnPropertyChanged(nameof(RecipeEditValidationText));
            OnPropertyChanged(nameof(PipelineEditValidationText));
            RefreshSampleOptions();
            RefreshRecentBatchRunOptions();
            UpdateSelectedRecipeSummary();
        }

        private void SelectRecipe(string recipeName)
        {
            if (string.IsNullOrWhiteSpace(recipeName))
            {
                return;
            }

            if (isRefreshingOptions)
            {
                return;
            }

            string normalized = NormalizeRecipeName(recipeName);
            if (string.Equals(selectedRecipeName, normalized, StringComparison.Ordinal))
            {
                SetSelectedRecipeName(normalized);
                return;
            }

            if (isSelectingRecipe)
            {
                SetSelectedRecipeName(normalized);
                return;
            }

            try
            {
                isSelectingRecipe = true;
                RecipeWorkspaceService.EnsureVisionWorkspace(normalized);
                switchRecipe(normalized);
                StatusText = string.Format(
                    CultureInfo.CurrentCulture,
                    LocalText("선택됨: {0}", "Selected: {0}"),
                    normalized);
                RefreshOptions();
                refreshAfterSwitch();
            }
            finally
            {
                isSelectingRecipe = false;
            }
        }

        private void SelectPipelineOption(OpenVisionRecipePipelineOption option)
        {
            if (option == null)
            {
                return;
            }

            if (!SetProperty(ref selectedPipelineOption, option, nameof(SelectedPipelineOption)))
            {
                PipelineEditName = option.PipelineName;
                return;
            }

            PipelineEditName = option.PipelineName;
            LatestCatalogBenchmarkSummary = OpenVisionRecipeCatalogBenchmarkSummary.Empty;
            UpdateSelectedRecipeSummary();
            RefreshRecentBatchRunOptions();
            RefreshCommandState();
        }

        private string BuildRecipeEditValidationText()
        {
            string selected = NormalizeRecipeName(selectedRecipeName);
            string requested = EditRecipeName?.Trim() ?? string.Empty;
            bool hasSelectedRecipe = !string.IsNullOrWhiteSpace(selected)
                && RecipeOptions.Any(name => string.Equals(name, selected, StringComparison.OrdinalIgnoreCase));

            if (string.IsNullOrWhiteSpace(requested))
            {
                return hasSelectedRecipe
                    ? LocalText("빈 이름은 새 레시피 생성 시 자동 이름을 사용합니다. XML 가져오기/내보내기는 선택된 레시피에 적용됩니다.", "Blank name uses an automatic name for create. XML import/export applies to the selected recipe.")
                    : LocalText("레시피를 선택하거나 새 이름을 입력하세요.", "Select a recipe or type a new name.");
            }

            if (!RecipeWorkspaceService.IsValidRecipeName(requested))
            {
                return LocalText("이름에 사용할 수 없는 문자가 있습니다.", "The name contains invalid characters.");
            }

            bool matchesSelected = string.Equals(selected, requested, StringComparison.OrdinalIgnoreCase);
            bool duplicateName = RecipeOptions.Any(name => string.Equals(name, requested, StringComparison.OrdinalIgnoreCase));

            if (!hasSelectedRecipe)
            {
                return LocalText("선택된 레시피가 없어 가져오기/내보내기/복제/이름 변경은 사용할 수 없습니다.", "No recipe is selected, so import/export/duplicate/rename are unavailable.");
            }

            if (matchesSelected)
            {
                return RecipeOptions.Count > 1
                    ? LocalText("현재 선택된 레시피입니다. 다른 이름을 입력하면 이름 변경이 활성화됩니다.", "This is the selected recipe. Type a different name to enable rename.")
                    : LocalText("현재 유일한 레시피입니다. 마지막 레시피는 삭제할 수 없습니다.", "This is the only recipe. The last recipe cannot be deleted.");
            }

            if (duplicateName)
            {
                return LocalText("이미 같은 이름의 레시피가 있습니다.", "A recipe with this name already exists.");
            }

            return LocalText("사용 가능한 이름입니다. 새로 만들기, 복제, 이름 변경에 사용할 수 있습니다.", "This name is available for create, duplicate, and rename.");
        }

        private void NotifyOperatorReviewChanged()
        {
            OnPropertyChanged(nameof(OperatorRunReviewText));
            OnPropertyChanged(nameof(OperatorDecisionXmlCardText));
            OnPropertyChanged(nameof(OperatorDecisionSampleCardText));
            OnPropertyChanged(nameof(OperatorDecisionPairCardText));
            OnPropertyChanged(nameof(OperatorDecisionNextActionText));
            OnPropertyChanged(nameof(OperatorValidationChecklistText));
            OnPropertyChanged(nameof(OperatorValidationChecklistRows));
            OnPropertyChanged(nameof(OperatorResultChannelsText));
            OnPropertyChanged(nameof(OperatorResultChannelRows));
            OnPropertyChanged(nameof(OperatorResultChannelBoardRows));
            OnPropertyChanged(nameof(OperatorHandoffReportText));
            OnPropertyChanged(nameof(RecipeGuidedNextActionText));
            CommandManager.InvalidateRequerySuggested();
        }

        private void RefreshSampleMatrixRows()
        {
            IReadOnlyList<OpenVisionRecipeSampleMatrixRow> rows = BuildSampleMatrixRows();
            OpenVisionRecipeSampleMatrixRow previous = SelectedSampleMatrixRow;
            SampleMatrixRows = rows;
            SelectedSampleMatrixRow = SelectDefaultSampleMatrixRow(rows, previous);
            OnPropertyChanged(nameof(SampleMatrixSummaryText));
            OnPropertyChanged(nameof(SelectedSampleMatrixReviewText));
        }

        private IReadOnlyList<OpenVisionRecipeSampleMatrixRow> BuildSampleMatrixRows()
        {
            VisionPipelineSampleCatalogItem selectedSample = SelectedSampleOption?.Sample;
            if (selectedSample == null)
            {
                return new[] { OpenVisionRecipeSampleMatrixRow.CreateEmpty() };
            }

            List<VisionPipelineSampleCatalogItem> samples = VisionPipelineSampleCheckService.GetPairSamples(selectedSample);
            if (samples.Count == 0)
            {
                samples.Add(selectedSample);
            }

            Dictionary<string, OpenVisionRecipePairSampleRunSummary> resultsBySample =
                (LatestPairRunSummary?.SampleResults ?? Array.Empty<OpenVisionRecipePairSampleRunSummary>())
                .Where(result => result != null && !string.IsNullOrWhiteSpace(result.SampleName))
                .GroupBy(result => result.SampleName.Trim(), StringComparer.OrdinalIgnoreCase)
                .ToDictionary(group => group.Key, group => group.First(), StringComparer.OrdinalIgnoreCase);

            return samples
                .Select(sample =>
                {
                    resultsBySample.TryGetValue(sample.SampleName ?? string.Empty, out OpenVisionRecipePairSampleRunSummary result);
                    return OpenVisionRecipeSampleMatrixRow.Create(sample, result);
                })
                .ToList();
        }

        private static OpenVisionRecipeSampleMatrixRow SelectDefaultSampleMatrixRow(
            IReadOnlyList<OpenVisionRecipeSampleMatrixRow> rows,
            OpenVisionRecipeSampleMatrixRow previous)
        {
            if (rows == null || rows.Count == 0)
            {
                return null;
            }

            if (!string.IsNullOrWhiteSpace(previous?.SampleName))
            {
                OpenVisionRecipeSampleMatrixRow sameSample = rows.FirstOrDefault(row =>
                    string.Equals(row.SampleName, previous.SampleName, StringComparison.OrdinalIgnoreCase));
                if (sameSample != null)
                {
                    return sameSample;
                }
            }

            return rows.FirstOrDefault(row => row.HasResult && !row.Success)
                ?? rows.FirstOrDefault(row => !row.HasResult)
                ?? rows[0];
        }

        private string BuildSampleMatrixSummaryText()
        {
            IReadOnlyList<OpenVisionRecipeSampleMatrixRow> rows = SampleMatrixRows ?? Array.Empty<OpenVisionRecipeSampleMatrixRow>();
            int runnableRows = rows.Count(row => !row.IsPlaceholder);
            if (runnableRows == 0)
            {
                return LocalText("샘플을 선택하면 Good/Bad 매트릭스가 표시됩니다.", "Select a sample to show the Good/Bad matrix.");
            }

            int completed = rows.Count(row => !row.IsPlaceholder && row.HasResult);
            int pass = rows.Count(row => !row.IsPlaceholder && row.HasResult && row.Success);
            int fail = rows.Count(row => !row.IsPlaceholder && row.HasResult && !row.Success);
            string group = string.IsNullOrWhiteSpace(SelectedSampleOption?.Sample?.PairGroup)
                ? "-"
                : SelectedSampleOption.Sample.PairGroup.Trim();

            return "PairGroup " + group
                + " | "
                + LocalText("행 ", "Rows ")
                + runnableRows.ToString(CultureInfo.InvariantCulture)
                + " | "
                + LocalText("실행 ", "Run ")
                + completed.ToString(CultureInfo.InvariantCulture)
                + "/"
                + runnableRows.ToString(CultureInfo.InvariantCulture)
                + " | OK "
                + pass.ToString(CultureInfo.InvariantCulture)
                + " / NG "
                + fail.ToString(CultureInfo.InvariantCulture);
        }

        private string BuildOperatorRunReviewText()
        {
            OpenVisionRecipeManagerSummary summary = SelectedRecipeSummary ?? OpenVisionRecipeManagerSummary.Empty;
            OpenVisionRecipeSampleRunSummary sample = LatestSampleRunSummary ?? OpenVisionRecipeSampleRunSummary.Empty;
            OpenVisionRecipePairRunSummary pair = LatestPairRunSummary ?? OpenVisionRecipePairRunSummary.Empty;
            return string.Join(
                Environment.NewLine,
                OpenVisionRecipeText.Local("XML/단계: ", "XML/Steps: ") + summary.XmlStatusDisplay + " / " + summary.StepCount.ToString(CultureInfo.InvariantCulture),
                OpenVisionRecipeText.Local("샘플: ", "Sample: ") + sample.CompactText,
                OpenVisionRecipeText.Local("쌍 검사: ", "Pair: ") + pair.CompactText,
                OpenVisionRecipeText.Local("다음: ", "Next: ") + BuildOperatorRunReviewNextAction(summary, sample, pair));
        }

        private string BuildOperatorDecisionXmlCardText()
        {
            OpenVisionRecipeManagerSummary summary = SelectedRecipeSummary ?? OpenVisionRecipeManagerSummary.Empty;
            string state = summary.XmlValid && summary.StepCount > 0
                ? LocalText("준비", "Ready")
                : LocalText("조치 필요", "Needs action");
            return LocalText("XML/Step", "XML/Steps")
                + Environment.NewLine
                + state
                + " | "
                + summary.XmlStatusDisplay
                + " | "
                + summary.StepCount.ToString(CultureInfo.InvariantCulture)
                + " Step";
        }

        private string BuildOperatorDecisionSampleCardText()
        {
            OpenVisionRecipeSampleRunSummary sample = LatestSampleRunSummary ?? OpenVisionRecipeSampleRunSummary.Empty;
            string state = !sample.HasResult
                ? LocalText("미실행", "Not run")
                : (sample.Succeeded ? "OK" : "NG");
            return LocalText("선택 샘플", "Selected sample")
                + Environment.NewLine
                + state
                + " | "
                + sample.CompactText;
        }

        private string BuildOperatorDecisionPairCardText()
        {
            OpenVisionRecipePairRunSummary pair = LatestPairRunSummary ?? OpenVisionRecipePairRunSummary.Empty;
            string state = !pair.HasResult
                ? LocalText("미실행", "Not run")
                : (pair.Succeeded ? "OK" : "NG");
            return "Good/Bad"
                + Environment.NewLine
                + state
                + " | "
                + pair.CompactText;
        }

        private string BuildOperatorDecisionNextActionText()
        {
            OpenVisionRecipeManagerSummary summary = SelectedRecipeSummary ?? OpenVisionRecipeManagerSummary.Empty;
            OpenVisionRecipeSampleRunSummary sample = LatestSampleRunSummary ?? OpenVisionRecipeSampleRunSummary.Empty;
            OpenVisionRecipePairRunSummary pair = LatestPairRunSummary ?? OpenVisionRecipePairRunSummary.Empty;
            return LocalText("다음 작업: ", "Next action: ") + BuildOperatorRunReviewNextAction(summary, sample, pair);
        }

        private IReadOnlyList<OpenVisionRecipeOperatorValidationRow> BuildOperatorValidationChecklistRows()
        {
            OpenVisionRecipeManagerSummary summary = SelectedRecipeSummary ?? OpenVisionRecipeManagerSummary.Empty;
            OpenVisionRecipeSampleRunSummary sample = LatestSampleRunSummary ?? OpenVisionRecipeSampleRunSummary.Empty;
            OpenVisionRecipePairRunSummary pair = LatestPairRunSummary ?? OpenVisionRecipePairRunSummary.Empty;
            OpenVisionRecipeCatalogBenchmarkSummary catalog = LatestCatalogBenchmarkSummary ?? OpenVisionRecipeCatalogBenchmarkSummary.Empty;
            List<OpenVisionRecipeOperatorValidationRow> rows = new List<OpenVisionRecipeOperatorValidationRow>();

            rows.Add(OpenVisionRecipeOperatorValidationRow.Create(
                LocalText("XML/Step", "XML/Steps"),
                summary.XmlValid && summary.StepCount > 0 ? "OK" : "NG",
                summary.XmlStatusDisplay + " / " + summary.StepCount.ToString(CultureInfo.InvariantCulture) + " Step",
                summary.XmlValid && summary.StepCount > 0
                    ? LocalText("샘플 검증으로 진행", "Proceed to sample validation")
                    : LocalText("LLM XML 검증 보고서와 Step 구성을 먼저 수정", "Fix the LLM XML validation report and step structure first")));

            rows.Add(OpenVisionRecipeOperatorValidationRow.Create(
                LocalText("선택 샘플", "Selected sample"),
                !sample.HasResult ? "WAIT" : (sample.Succeeded ? "OK" : "NG"),
                sample.CompactText,
                !sample.HasResult
                    ? LocalText("검사 실행", "Run check")
                    : (sample.Succeeded
                        ? LocalText("Good/Bad 쌍 검증 진행", "Proceed to Good/Bad pair validation")
                        : LocalText("실패 Step 입력/출력과 파라미터 확인", "Review failed step input/output and parameters"))));

            rows.Add(OpenVisionRecipeOperatorValidationRow.Create(
                "Good/Bad",
                !pair.HasResult ? "WAIT" : (pair.Succeeded ? "OK" : "NG"),
                pair.CompactText,
                !pair.HasResult
                    ? LocalText("쌍 검사 실행", "Run pair check")
                    : (pair.Succeeded
                        ? LocalText("카탈로그 또는 이력 비교로 확장", "Expand to catalog or run-history comparison")
                        : LocalText("NG 역할을 선택하고 실패 Step 조정", "Select the NG role and tune the failed step"))));

            rows.Add(OpenVisionRecipeOperatorValidationRow.Create(
                LocalText("카탈로그", "Catalog"),
                !catalog.HasResult ? "WAIT" : (catalog.Succeeded ? "OK" : "NG"),
                catalog.CompactText,
                !catalog.HasResult
                    ? LocalText("전체 샘플 검사 실행", "Run catalog benchmark")
                    : (catalog.Succeeded
                        ? LocalText("결과 고정 가능", "Ready to keep result")
                        : LocalText("실패 샘플 우선 재검토", "Review failing samples first"))));

            IReadOnlyList<OpenVisionRecipeBatchRunComparisonRow> comparisonRows =
                RecentBatchRunComparisonRows ?? Array.Empty<OpenVisionRecipeBatchRunComparisonRow>();
            int comparable = comparisonRows.Count(row => row != null && row.IsComparable);
            int regression = comparisonRows.Count(row => row != null && row.IsRegression);
            string benchmarkState = comparable == 0 ? "WAIT" : (regression == 0 ? "OK" : "NG");
            rows.Add(OpenVisionRecipeOperatorValidationRow.Create(
                LocalText("회귀 비교", "Regression diff"),
                benchmarkState,
                RecentBatchRunComparisonSummaryText,
                comparable == 0
                    ? LocalText("이전 benchmark 기준 실행 확보", "Create or select a baseline benchmark run")
                    : (regression == 0
                        ? LocalText("회귀 없음. Still NG만 추적", "No regression. Track remaining Still NG rows")
                        : LocalText("REGRESSION 행부터 확인", "Start with REGRESSION rows"))));

            return rows;
        }

        private IReadOnlyList<OpenVisionRecipeOperatorResultChannelRow> BuildOperatorResultChannelRows()
        {
            OpenVisionRecipeManagerSummary summary = SelectedRecipeSummary ?? OpenVisionRecipeManagerSummary.Empty;
            OpenVisionRecipeSampleRunSummary sample = LatestSampleRunSummary ?? OpenVisionRecipeSampleRunSummary.Empty;
            OpenVisionRecipePairRunSummary pair = LatestPairRunSummary ?? OpenVisionRecipePairRunSummary.Empty;
            IReadOnlyList<OpenVisionRecipeBatchRunComparisonRow> comparisonRows =
                RecentBatchRunComparisonRows ?? Array.Empty<OpenVisionRecipeBatchRunComparisonRow>();

            string finalStatus;
            string finalSource;
            if (!summary.XmlValid || summary.StepCount <= 0)
            {
                finalStatus = "NG";
                finalSource = "XML/Step";
            }
            else if (pair.HasResult)
            {
                finalStatus = pair.Succeeded ? "OK" : "NG";
                finalSource = "Good/Bad";
            }
            else if (sample.HasResult)
            {
                finalStatus = sample.Succeeded ? "OK" : "NG";
                finalSource = LocalText("선택 샘플", "Selected sample");
            }
            else
            {
                finalStatus = "WAIT";
                finalSource = "XML/Step";
            }

            string failedStep = SelectedPairSampleResult?.FailedStepText;
            if (string.IsNullOrWhiteSpace(failedStep))
            {
                failedStep = SelectedRecentBatchRunComparisonRow?.FailedStep;
            }

            if (string.IsNullOrWhiteSpace(failedStep))
            {
                failedStep = SelectedRecentBatchSampleResultOption?.FailedStep;
            }

            string evidence = pair.HasResult
                ? pair.CompactText
                : (sample.HasResult ? sample.CompactText : summary.XmlStatusDisplay);
            int comparable = comparisonRows.Count(row => row != null && row.IsComparable);
            int regression = comparisonRows.Count(row => row != null && row.IsRegression);
            string benchmark = comparable <= 0
                ? "WAIT"
                : (regression == 0 ? "OK" : "NG");

            return new[]
            {
                OpenVisionRecipeOperatorResultChannelRow.Create(
                    "Inspection.Status",
                    finalStatus,
                    finalSource,
                    LocalText("최종 OK/NG 판정", "Final OK/NG judgement")),
                OpenVisionRecipeOperatorResultChannelRow.Create(
                    "Inspection.FailedStep",
                    string.IsNullOrWhiteSpace(failedStep) ? "-" : failedStep,
                    string.IsNullOrWhiteSpace(failedStep) ? LocalText("실패 없음", "No failure") : LocalText("실패 추적", "Failure trace"),
                    LocalText("실패 원인 추적", "Failure triage")),
                OpenVisionRecipeOperatorResultChannelRow.Create(
                    "Inspection.Evidence",
                    evidence,
                    finalSource,
                    LocalText("리포트/LLM 재검토 근거", "Report/LLM review evidence")),
                OpenVisionRecipeOperatorResultChannelRow.Create(
                    "Inspection.Benchmark",
                    benchmark,
                    LocalText("이력 비교", "Run history"),
                    comparable <= 0
                        ? LocalText("기준 실행 필요", "Needs baseline run")
                        : LocalText("회귀 비교 결과", "Regression diff result")),
                OpenVisionRecipeOperatorResultChannelRow.Create(
                    "Inspection.NextAction",
                    BuildOperatorRunReviewNextAction(summary, sample, pair),
                    LocalText("작업자 검토", "Operator review"),
                    LocalText("다음 작업 지시", "Next action instruction"))
            };
        }

        private string BuildOperatorHandoffReportText()
        {
            OpenVisionRecipeManagerSummary summary = SelectedRecipeSummary ?? OpenVisionRecipeManagerSummary.Empty;
            OpenVisionRecipeSampleRunSummary sample = LatestSampleRunSummary ?? OpenVisionRecipeSampleRunSummary.Empty;
            OpenVisionRecipePairRunSummary pair = LatestPairRunSummary ?? OpenVisionRecipePairRunSummary.Empty;
            OpenVisionRecipePipelineStepPreview failedStep = SelectedPairSampleResult?.CanOpenFailedStep == true
                ? FindPipelinePreviewStep(SelectedPairSampleResult.FailedStepText)
                : SelectedPipelinePreviewStep;

            List<string> lines = new List<string>
            {
                LocalText("OpenVisionLab 작업자 리포트", "OpenVisionLab operator report"),
                LocalText("레시피: ", "Recipe: ") + (string.IsNullOrWhiteSpace(summary.RecipeName) ? "-" : summary.RecipeName),
                LocalText("파이프라인: ", "Pipeline: ") + (string.IsNullOrWhiteSpace(summary.PreviewPipelineName) ? "-" : summary.PreviewPipelineName),
                LocalText("활성 파이프라인: ", "Active pipeline: ") + (string.IsNullOrWhiteSpace(summary.ActivePipelineName) ? "-" : summary.ActivePipelineName),
                LocalText("XML/Step: ", "XML/Steps: ") + summary.XmlStatusDisplay + " / " + summary.StepCount.ToString(CultureInfo.InvariantCulture),
                LocalText("샘플: ", "Sample: ") + sample.CompactText,
                "Good/Bad: " + pair.CompactText,
                LocalText("다음 작업: ", "Next action: ") + BuildOperatorRunReviewNextAction(summary, sample, pair)
            };

            lines.Add(LocalText("검증 체크리스트:", "Validation checklist:"));
            foreach (OpenVisionRecipeOperatorValidationRow row in OperatorValidationChecklistRows)
            {
                lines.Add("- " + row.ItemText + ": " + row.StateText + " | " + row.EvidenceText + " | " + row.NextActionText);
            }

            lines.Add(LocalText("판정 출력 정의:", "Judgement outputs:"));
            foreach (OpenVisionRecipeOperatorResultChannelRow row in OperatorResultChannelRows)
            {
                lines.Add("- " + row.ChannelText + ": " + row.ValueText + " | " + row.SourceText + " | " + row.UseText);
            }

            if (SelectedPairSampleResult != null)
            {
                lines.Add(LocalText("선택 역할: ", "Selected role: ")
                    + SelectedPairSampleResult.Role
                    + " / "
                    + SelectedPairSampleResult.ResultText
                    + " / "
                    + SelectedPairSampleResult.SampleName);
            }

            if (failedStep != null)
            {
                lines.Add(LocalText("검토 Step: ", "Review step: ") + failedStep.DisplayText);
                lines.Add(LocalText("입출력: ", "Route: ") + failedStep.InputLayer + " -> " + failedStep.OutputLayer);
            }

            if (!string.IsNullOrWhiteSpace(summary.LlmXmlValidationReport))
            {
                lines.Add(LocalText("LLM XML: ", "LLM XML: ") + FirstReportLine(summary.LlmXmlValidationReport));
            }

            return string.Join(Environment.NewLine, lines);
        }

        private static string FirstReportLine(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return "-";
            }

            return text
                .Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries)
                .Select(line => line.Trim())
                .FirstOrDefault(line => line.Length > 0) ?? "-";
        }

        private bool CanCopyOperatorHandoffReport()
        {
            return !string.IsNullOrWhiteSpace(OperatorHandoffReportText);
        }

        private void CopyOperatorHandoffReport()
        {
            string report = OperatorHandoffReportText;
            if (string.IsNullOrWhiteSpace(report))
            {
                OperatorHandoffReportStatusText = LocalText("복사할 리포트가 없습니다.", "No report to copy.");
                return;
            }

            try
            {
                System.Windows.Clipboard.SetText(report);
                OperatorHandoffReportStatusText = LocalText("리포트가 클립보드에 복사되었습니다.", "Report copied to clipboard.");
            }
            catch (Exception ex)
            {
                OperatorHandoffReportStatusText = LocalText("클립보드 복사 실패: ", "Clipboard copy failed: ") + ex.Message;
            }
        }

        private bool CanCopySelectedRecentBatchRunReview()
        {
            return SelectedRecentBatchRunOption != null
                && !string.IsNullOrWhiteSpace(SelectedRecentBatchRunOption.SummaryPath)
                && !string.IsNullOrWhiteSpace(SelectedRecentBatchRunReviewText);
        }

        private void CopySelectedRecentBatchRunReview()
        {
            string review = SelectedRecentBatchRunReviewText;
            if (string.IsNullOrWhiteSpace(review))
            {
                SelectedRecentBatchRunReviewCopyStatusText = LocalText("복사할 이력 판독이 없습니다.", "No run review to copy.");
                return;
            }

            try
            {
                System.Windows.Clipboard.SetText(review);
                SelectedRecentBatchRunReviewCopyStatusText = LocalText("이력 판독이 클립보드에 복사되었습니다.", "Run review copied to clipboard.");
            }
            catch (Exception ex)
            {
                SelectedRecentBatchRunReviewCopyStatusText = LocalText("이력 판독 복사 실패: ", "Run review copy failed: ") + ex.Message;
            }
        }

        private string BuildRecipeGuidedSetupText()
        {
            OpenVisionRecipeManagerSummary summary = SelectedRecipeSummary ?? OpenVisionRecipeManagerSummary.Empty;
            OpenVisionRecipeSampleRunSummary sample = LatestSampleRunSummary ?? OpenVisionRecipeSampleRunSummary.Empty;
            OpenVisionRecipePairRunSummary pair = LatestPairRunSummary ?? OpenVisionRecipePairRunSummary.Empty;
            string sampleState = SelectedSampleOption?.Sample == null
                ? LocalText("샘플 선택", "Select sample")
                : LocalText("샘플 준비", "Sample ready");
            string xmlState = summary.XmlValid
                ? LocalText("XML OK", "XML OK")
                : LocalText("XML 검증", "Validate XML");
            string stepState = summary.StepCount > 0
                ? LocalText("Step ", "Steps ") + summary.StepCount.ToString(CultureInfo.InvariantCulture)
                : LocalText("Step 없음", "No steps");
            string sampleRunState = sample.HasResult
                ? (sample.Succeeded ? LocalText("샘플 OK", "Sample OK") : LocalText("샘플 NG", "Sample NG"))
                : LocalText("샘플 실행", "Run sample");
            string pairRunState = pair.HasResult
                ? (pair.Succeeded ? LocalText("Good/Bad OK", "Good/Bad OK") : LocalText("Good/Bad NG", "Good/Bad NG"))
                : LocalText("Good/Bad 실행", "Run Good/Bad");
            string next = BuildOperatorRunReviewNextAction(summary, sample, pair);
            return LocalText("가이드", "Guide")
                + ": 1 " + sampleState
                + " -> 2 " + xmlState
                + " -> 3 " + stepState
                + " -> 4 " + sampleRunState
                + " -> 5 " + pairRunState
                + " | " + LocalText("다음: ", "Next: ") + next;
        }

        private bool CanRunRecipeGuidedNextAction()
        {
            return ResolveRecipeGuidedNextAction() != null;
        }

        private string BuildRecipeGuidedNextActionText()
        {
            OpenVisionRecipeManagerSummary summary = SelectedRecipeSummary ?? OpenVisionRecipeManagerSummary.Empty;
            OpenVisionRecipeSampleRunSummary sample = LatestSampleRunSummary ?? OpenVisionRecipeSampleRunSummary.Empty;
            OpenVisionRecipePairRunSummary pair = LatestPairRunSummary ?? OpenVisionRecipePairRunSummary.Empty;

            if (!summary.XmlValid && CanUseLlmXmlDraft())
            {
                return LocalText("XML 검증", "Validate XML");
            }

            if (summary.StepCount <= 0 && CanDuplicatePipelineFromSample())
            {
                return LocalText("샘플 복제", "Duplicate sample");
            }

            if (!string.Equals(summary.ActivePipelineName, summary.PreviewPipelineName, StringComparison.OrdinalIgnoreCase)
                && CanUseSelectedPipeline())
            {
                return LocalText("활성화", "Activate");
            }

            if (!sample.HasResult && CanRunSelectedSampleCheck())
            {
                return LocalText("검사 실행", "Run check");
            }

            if (sample.HasResult && !sample.Succeeded && CanLoadSelectedStepParameters())
            {
                return LocalText("파라미터 열기", "Load params");
            }

            if (!pair.HasResult && CanRunSelectedSamplePairCheck())
            {
                return LocalText("Good/Bad 실행", "Run Good/Bad");
            }

            if (pair.HasResult && !pair.Succeeded && CanOpenSelectedStepTool())
            {
                return LocalText("도구 열기", "Open tool");
            }

            return LocalText("완료", "Complete");
        }

        private void RunRecipeGuidedNextAction()
        {
            Action action = ResolveRecipeGuidedNextAction();
            if (action == null)
            {
                StatusText = LocalText("현재 실행할 다음 가이드 작업이 없습니다.", "No guided next action is available.");
                return;
            }

            action();
        }

        private Action ResolveRecipeGuidedNextAction()
        {
            OpenVisionRecipeManagerSummary summary = SelectedRecipeSummary ?? OpenVisionRecipeManagerSummary.Empty;
            OpenVisionRecipeSampleRunSummary sample = LatestSampleRunSummary ?? OpenVisionRecipeSampleRunSummary.Empty;
            OpenVisionRecipePairRunSummary pair = LatestPairRunSummary ?? OpenVisionRecipePairRunSummary.Empty;

            if (!summary.XmlValid && CanUseLlmXmlDraft())
            {
                return ValidateLlmXmlDraft;
            }

            if (summary.StepCount <= 0 && CanDuplicatePipelineFromSample())
            {
                return DuplicatePipelineFromSample;
            }

            if (!string.Equals(summary.ActivePipelineName, summary.PreviewPipelineName, StringComparison.OrdinalIgnoreCase)
                && CanUseSelectedPipeline())
            {
                return ActivateSelectedPipeline;
            }

            if (!sample.HasResult && CanRunSelectedSampleCheck())
            {
                return RunSelectedSampleCheck;
            }

            if (sample.HasResult && !sample.Succeeded && CanLoadSelectedStepParameters())
            {
                return LoadSelectedStepParameters;
            }

            if (!pair.HasResult && CanRunSelectedSamplePairCheck())
            {
                return RunSelectedSamplePairCheck;
            }

            if (pair.HasResult && !pair.Succeeded && CanOpenSelectedStepTool())
            {
                return OpenSelectedStepTool;
            }

            return null;
        }

        private string BuildPipelineSelectedStepOperatorContextText()
        {
            OpenVisionRecipePipelineStepPreview step = SelectedPipelinePreviewStep;
            if (step == null)
            {
                return LocalText(
                    "Step을 선택하면 선택 이유, 입력/출력 경로, 다음 검토 순서가 여기에 표시됩니다.",
                    "Select a step to see why it is under review, its input/output route, and the next review action.");
            }

            List<string> lines = new List<string>
            {
                LocalText("선택 Step: ", "Selected step: ") + step.DisplayText,
                LocalText("경로: ", "Route: ") + step.InputLayer + " -> " + step.OutputLayer
            };

            if (SelectedPairSampleResult?.CanOpenFailedStep == true)
            {
                lines.Add(
                    LocalText("Good/Bad 실패 연결: ", "Good/Bad failure link: ")
                    + SelectedPairSampleResult.Role
                    + " / "
                    + SelectedPairSampleResult.SampleName
                    + " / "
                    + SelectedPairSampleResult.ResultText);
            }
            else if (!string.IsNullOrWhiteSpace(SelectedRecentBatchSampleResultOption?.FailedStep))
            {
                lines.Add(
                    LocalText("실행 이력 실패 연결: ", "Run-history failure link: ")
                    + SelectedRecentBatchSampleResultOption.DisplayText);
            }
            else if (!string.IsNullOrWhiteSpace(SelectedRecentBatchRunComparisonRow?.FailedStep))
            {
                lines.Add(
                    LocalText("비교 이력 실패 연결: ", "Comparison failure link: ")
                    + SelectedRecentBatchRunComparisonRow.DisplayText);
            }
            else
            {
                lines.Add(LocalText("실패 연결: 없음", "Failure link: none"));
            }

            lines.Add(LocalText(
                "다음: 출력 보기 -> 입력과 비교 -> PropertyGrid 검토 -> Good/Bad 명시 재검사",
                "Next: view output -> compare input -> review PropertyGrid -> explicitly rerun Good/Bad."));

            return string.Join(Environment.NewLine, lines);
        }

        private string BuildFailureReviewText()
        {
            OpenVisionRecipePipelineStepPreview step = SelectedPipelinePreviewStep;
            if (step == null)
            {
                return LocalText(
                    "Good/Bad 역할 또는 실행 이력에서 실패 Step을 선택하면 입력/출력 레이어 비교와 재검사 경로가 여기에 표시됩니다.",
                    "Select a failed step from Good/Bad roles or run history to see input/output comparison and rerun actions here.");
            }

            List<string> lines = new List<string>
            {
                LocalText("선택 Step: ", "Selected step: ") + step.DisplayText,
                LocalText("비교: ", "Compare: ") + step.InputLayer + " -> " + step.OutputLayer,
                LocalText("다음: 출력 보기로 결과 레이어를 확인하고, 입력 보기로 원본 기준을 확인한 뒤 Good/Bad 재검사를 실행하세요.",
                    "Next: view the output layer, compare it against the input layer, then rerun Good/Bad.")
            };

            if (SelectedPairSampleResult?.CanOpenFailedStep == true)
            {
                lines.Insert(
                    1,
                    LocalText("역할 실패: ", "Role failure: ")
                    + SelectedPairSampleResult.Role
                    + " / "
                    + SelectedPairSampleResult.SampleName);
            }
            else if (!string.IsNullOrWhiteSpace(SelectedRecentBatchSampleResultOption?.FailedStep))
            {
                lines.Insert(
                    1,
                    LocalText("이력 실패: ", "History failure: ")
                    + SelectedRecentBatchSampleResultOption.DisplayText);
            }

            return string.Join(Environment.NewLine, lines);
        }

        private string BuildCorrectedOutputReviewText()
        {
            OpenVisionRecipePipelineStepPreview step = SelectedPipelinePreviewStep;
            if (step == null)
            {
                return LocalText(
                    "Step을 선택하면 XML 수정 후 출력 확인 순서가 표시됩니다.",
                    "Select a step to see the XML edit and corrected-output check sequence.");
            }

            if (selectedStepEditDirty)
            {
                return LocalText(
                    "편집됨: XML 반영을 누른 뒤 출력 보기 또는 Good/Bad 재검사로 수정 결과를 확인하세요.",
                    "Edited: apply to XML, then use View output or Rerun Good/Bad to check the correction.");
            }

            if (SelectedStepEditObject == null)
            {
                return LocalText(
                    "파라미터 불러오기 -> PropertyGrid 검토 -> XML 반영 -> 출력 보기/Good-Bad 재검사 순서로 확인하세요.",
                    "Load parameters -> review in PropertyGrid -> apply to XML -> view output or rerun Good/Bad.");
            }

            return LocalText(
                "PropertyGrid 값을 검토 중입니다. 변경 후 XML 반영을 눌러야 corrected output 검토가 시작됩니다.",
                "Reviewing PropertyGrid values. Apply to XML after edits to start corrected-output review.");
        }

        private string BuildCorrectedOutputAppliedText(string pipelineName, int selectedIndex, string validationMessage)
        {
            OpenVisionRecipePipelineStepPreview step = SelectedPipelinePreviewStep;
            string route = step == null
                ? "-"
                : step.InputLayer + " -> " + step.OutputLayer;

            return LocalText(
                    "XML 반영 완료: ",
                    "Applied to XML: ")
                + pipelineName
                + " / Step "
                + selectedIndex.ToString(CultureInfo.InvariantCulture)
                + Environment.NewLine
                + LocalText("확인 경로: ", "Check route: ")
                + route
                + Environment.NewLine
                + LocalText(
                    "다음: 출력 보기로 corrected output 레이어를 확인하고, Good/Bad 재검사를 명시 실행해 판정 기준을 다시 확인하세요.",
                    "Next: view the corrected output layer, then explicitly rerun Good/Bad to recheck acceptance gates.")
                + Environment.NewLine
                + LocalText("검증: ", "Validation: ")
                + validationMessage;
        }

        private string BuildSelectedRecentBatchRunReviewText()
        {
            OpenVisionRecipeBatchRunOption run = SelectedRecentBatchRunOption;
            OpenVisionRecipeBatchSampleResultOption sample = SelectedRecentBatchSampleResultOption;
            if (run == null || string.IsNullOrWhiteSpace(run.SummaryPath))
            {
                return LocalText("저장된 쌍 검사 이력을 선택하면 샘플별 결과와 실패 Step이 여기에 표시됩니다.", "Select a saved pair check run to review sample results and failed steps here.");
            }

            List<string> lines = new List<string>
            {
                LocalText("이력: ", "Run: ") + run.DisplayText,
                LocalText("요약: ", "Summary: ") + run.DetailText,
                LocalText("샘플: ", "Sample: ") + (sample?.DisplayText ?? "-"),
                LocalText("결과: ", "Result: ") + (sample?.DetailText ?? "-")
            };

            if (!string.IsNullOrWhiteSpace(sample?.FailedStep))
            {
                OpenVisionRecipePipelineStepPreview linkedStep = FindPipelinePreviewStep(sample.FailedStep);
                lines.Add(LocalText("연결 Step: ", "Linked step: ") + (linkedStep?.DisplayText ?? sample.FailedStep.Trim()));
                lines.Add(LocalText("다음: 실패 Step을 선택했습니다. 미리보기 목록에서 입력/출력과 기준을 확인하세요.", "Next: Failed step is selected. Review input/output and gates in the preview step list."));
            }
            else if (sample?.Success == true)
            {
                lines.Add(LocalText("다음: 이 샘플은 통과했습니다. NG 샘플을 선택하면 실패 Step이 연결됩니다.", "Next: This sample passed. Select an NG sample to link its failed step."));
            }

            return string.Join(Environment.NewLine, lines);
        }

        private void RefreshRecentBatchRunComparison()
        {
            IReadOnlyList<OpenVisionRecipeBatchRunComparisonRow> rows = BuildRecentBatchRunComparisonRows();
            RecentBatchRunComparisonRows = rows;
            SelectedRecentBatchRunComparisonRow = SelectDefaultBatchComparisonRow(rows);
            OnPropertyChanged(nameof(RecentBatchRunComparisonSummaryText));
            OnPropertyChanged(nameof(SelectedRecentBatchRunComparisonReviewText));
        }

        private IReadOnlyList<OpenVisionRecipeBatchRunComparisonRow> BuildRecentBatchRunComparisonRows()
        {
            OpenVisionRecipeBatchRunOption currentOption = SelectedRecentBatchRunOption;
            OpenVisionRecipeBatchRunOption baselineOption = FindBaselineBatchRunOption(currentOption);
            if (currentOption == null || string.IsNullOrWhiteSpace(currentOption.SummaryPath))
            {
                return new[] { OpenVisionRecipeBatchRunComparisonRow.CreateEmpty() };
            }

            if (baselineOption == null || string.IsNullOrWhiteSpace(baselineOption.SummaryPath))
            {
                return new[] { OpenVisionRecipeBatchRunComparisonRow.CreateNoBaseline(currentOption.DisplayText) };
            }

            VisionPipelineBatchRunSummary current = VisionPipelineBatchRunSummaryStorage.Load(currentOption.SummaryPath);
            VisionPipelineBatchRunSummary baseline = VisionPipelineBatchRunSummaryStorage.Load(baselineOption.SummaryPath);
            if (current?.Results == null || baseline?.Results == null)
            {
                return new[] { OpenVisionRecipeBatchRunComparisonRow.CreateNoBaseline(currentOption.DisplayText) };
            }

            Dictionary<string, VisionPipelineBatchSampleRunResult> currentBySample = BuildBatchResultMap(current.Results);
            Dictionary<string, VisionPipelineBatchSampleRunResult> baselineBySample = BuildBatchResultMap(baseline.Results);
            List<string> sampleNames = currentBySample.Keys
                .Union(baselineBySample.Keys, StringComparer.OrdinalIgnoreCase)
                .OrderBy(name => name, StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (sampleNames.Count == 0)
            {
                return new[] { OpenVisionRecipeBatchRunComparisonRow.CreateEmpty() };
            }

            return sampleNames
                .Select(sampleName =>
                {
                    currentBySample.TryGetValue(sampleName, out VisionPipelineBatchSampleRunResult currentResult);
                    baselineBySample.TryGetValue(sampleName, out VisionPipelineBatchSampleRunResult baselineResult);
                    return OpenVisionRecipeBatchRunComparisonRow.Create(sampleName, baselineResult, currentResult);
                })
                .ToList();
        }

        private OpenVisionRecipeBatchRunOption FindBaselineBatchRunOption(OpenVisionRecipeBatchRunOption current)
        {
            if (selectedBenchmarkBaselineRunOption != null
                && !string.IsNullOrWhiteSpace(selectedBenchmarkBaselineRunOption.SummaryPath)
                && !string.Equals(selectedBenchmarkBaselineRunOption.SummaryPath, current?.SummaryPath, StringComparison.OrdinalIgnoreCase))
            {
                return selectedBenchmarkBaselineRunOption;
            }

            return FindAutoBaselineBatchRunOption(current);
        }

        private OpenVisionRecipeBatchRunOption FindAutoBaselineBatchRunOption(OpenVisionRecipeBatchRunOption current)
        {
            if (current == null || string.IsNullOrWhiteSpace(current.SummaryPath))
            {
                return null;
            }

            List<OpenVisionRecipeBatchRunOption> runs = (RecentBatchRunOptions ?? Array.Empty<OpenVisionRecipeBatchRunOption>())
                .Where(option => option != null && !string.IsNullOrWhiteSpace(option.SummaryPath))
                .ToList();
            int currentIndex = runs.FindIndex(option =>
                string.Equals(option.SummaryPath, current.SummaryPath, StringComparison.OrdinalIgnoreCase));
            if (currentIndex < 0)
            {
                return null;
            }

            return runs.Skip(currentIndex + 1).FirstOrDefault()
                ?? runs.Take(currentIndex).LastOrDefault();
        }

        private static Dictionary<string, VisionPipelineBatchSampleRunResult> BuildBatchResultMap(
            IEnumerable<VisionPipelineBatchSampleRunResult> results)
        {
            return (results ?? Enumerable.Empty<VisionPipelineBatchSampleRunResult>())
                .Where(result => result != null && !string.IsNullOrWhiteSpace(result.SampleName))
                .GroupBy(result => result.SampleName.Trim(), StringComparer.OrdinalIgnoreCase)
                .ToDictionary(group => group.Key, group => group.First(), StringComparer.OrdinalIgnoreCase);
        }

        private static OpenVisionRecipeBatchRunComparisonRow SelectDefaultBatchComparisonRow(
            IReadOnlyList<OpenVisionRecipeBatchRunComparisonRow> rows)
        {
            return rows?.FirstOrDefault(row => row != null && row.IsRegression)
                ?? rows?.FirstOrDefault(row => row != null && row.IsStillFailing)
                ?? rows?.FirstOrDefault(row => row != null && row.IsRecovered)
                ?? rows?.FirstOrDefault();
        }

        private string BuildRecentBatchRunComparisonSummaryText()
        {
            IReadOnlyList<OpenVisionRecipeBatchRunComparisonRow> rows =
                RecentBatchRunComparisonRows ?? Array.Empty<OpenVisionRecipeBatchRunComparisonRow>();
            int comparable = rows.Count(row => row != null && row.IsComparable);
            if (comparable == 0)
            {
                return LocalText("비교할 이전 benchmark 실행이 없습니다.", "No previous benchmark run is available for comparison.");
            }

            int regression = rows.Count(row => row.IsRegression);
            int recovered = rows.Count(row => row.IsRecovered);
            int stillNg = rows.Count(row => row.IsStillFailing);
            string baseline = SelectedBenchmarkBaselineRunOption?.DisplayText;
            string prefix = string.IsNullOrWhiteSpace(baseline)
                ? string.Empty
                : LocalText("기준 ", "Baseline ") + baseline + " | ";
            return prefix + "Compared "
                + comparable.ToString(CultureInfo.InvariantCulture)
                + " | Regression "
                + regression.ToString(CultureInfo.InvariantCulture)
                + " | Recovered "
                + recovered.ToString(CultureInfo.InvariantCulture)
                + " | Still NG "
                + stillNg.ToString(CultureInfo.InvariantCulture);
        }

        private static string BuildOperatorRunReviewNextAction(
            OpenVisionRecipeManagerSummary summary,
            OpenVisionRecipeSampleRunSummary sample,
            OpenVisionRecipePairRunSummary pair)
        {
            if (summary == null || !summary.XmlValid)
            {
                return OpenVisionRecipeText.Local("LLM XML 검증 보고서를 먼저 수정하세요.", "Fix the LLM XML validation report first.");
            }

            if (summary.StepCount <= 0)
            {
                return OpenVisionRecipeText.Local("파이프라인 단계를 추가하거나 샘플에서 복제하세요.", "Add pipeline steps or duplicate from a sample.");
            }

            if (!string.Equals(summary.ActivePipelineName, summary.PreviewPipelineName, StringComparison.OrdinalIgnoreCase))
            {
                return OpenVisionRecipeText.Local("검토할 파이프라인을 활성화하거나 활성 파이프라인을 선택하세요.", "Activate the reviewed pipeline or select the active pipeline.");
            }

            if (sample == null || !sample.HasResult)
            {
                return OpenVisionRecipeText.Local("검사 실행으로 선택 샘플의 출력 레이어를 확인하세요.", "Run check to inspect the selected sample output layer.");
            }

            if (!sample.Succeeded)
            {
                return OpenVisionRecipeText.Local("샘플 실패 단계의 입력/출력 레이어와 파라미터를 조정하세요.", "Tune the failed sample step input/output layer and parameters.");
            }

            if (pair == null || !pair.HasResult)
            {
                return OpenVisionRecipeText.Local("Good/Bad 쌍 검사로 판정 기준을 확인하세요.", "Run Good/Bad pair check to verify acceptance gates.");
            }

            if (!pair.Succeeded)
            {
                return OpenVisionRecipeText.Local("Good/Bad가 모두 기준과 맞을 때까지 활성 파이프라인을 조정하세요.", "Tune the active pipeline until Good and Bad both match expectations.");
            }

            return OpenVisionRecipeText.Local("검토 완료: XML, 샘플 검사, 쌍 검사가 모두 통과했습니다.", "Review complete: XML, sample check, and pair check passed.");
        }

        private string BuildPipelineEditValidationText()
        {
            if (!CanUseSelectedRecipe())
            {
                return LocalText("레시피를 먼저 선택하세요.", "Select a recipe first.");
            }

            OpenVisionRecipePipelineOption option = SelectedPipelineOption;
            if (option == null)
            {
                return LocalText("파이프라인을 선택하세요.", "Select a pipeline.");
            }

            string requested = PipelineEditName?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(requested))
            {
                return LocalText("파이프라인 이름은 비워둘 수 없습니다.", "Pipeline name cannot be blank.");
            }

            string normalized = NormalizePipelineName(requested);
            if (!RecipeWorkspaceService.IsValidRecipeName(normalized))
            {
                return LocalText("파이프라인 이름에 사용할 수 없는 문자가 있습니다.", "The pipeline name contains invalid characters.");
            }

            if (!string.Equals(requested, normalized, StringComparison.Ordinal))
            {
                return string.Format(
                    CultureInfo.CurrentCulture,
                    LocalText("사용할 수 없는 문자는 '_'로 저장됩니다: {0}", "Invalid characters will be saved as '_': {0}"),
                    normalized);
            }

            bool matchesSelected = string.Equals(option.PipelineName, normalized, StringComparison.OrdinalIgnoreCase);
            bool duplicateName = PipelineOptions.Any(item => string.Equals(item.PipelineName, normalized, StringComparison.OrdinalIgnoreCase));

            if (matchesSelected)
            {
                return PipelineOptions.Count > 1
                    ? LocalText("현재 선택된 파이프라인입니다. 다른 이름을 입력하면 이름 변경이 활성화됩니다.", "This is the selected pipeline. Type a different name to enable rename.")
                    : LocalText("현재 유일한 파이프라인입니다. 마지막 파이프라인은 삭제할 수 없습니다.", "This is the only pipeline. The last pipeline cannot be deleted.");
            }

            if (duplicateName)
            {
                return LocalText("이미 같은 이름의 파이프라인이 있습니다.", "A pipeline with this name already exists.");
            }

            return LocalText("사용 가능한 파이프라인 이름입니다. 복제 또는 이름 변경에 사용할 수 있습니다.", "This pipeline name is available for duplicate or rename.");
        }

        private void CreateRecipe()
        {
            CreateAndSwitchRecipe(CreateUniqueRecipeName());
        }

        private void CreateNamedRecipe()
        {
            string requestedName = EditRecipeName?.Trim();
            string recipeName = string.IsNullOrWhiteSpace(requestedName)
                ? CreateUniqueRecipeName()
                : CreateUniqueRecipeName(requestedName);
            CreateAndSwitchRecipe(recipeName);
        }

        private bool CanCreateNamedRecipe()
        {
            string requestedName = EditRecipeName?.Trim();
            return string.IsNullOrWhiteSpace(requestedName)
                || RecipeWorkspaceService.IsValidRecipeName(requestedName);
        }

        private void DuplicateSelectedRecipe()
        {
            string sourceName = NormalizeRecipeName(selectedRecipeName);
            string requestedName = NormalizeRecipeName(EditRecipeName);
            string baseName = string.Equals(sourceName, requestedName, StringComparison.OrdinalIgnoreCase)
                ? sourceName + "_Copy"
                : requestedName;
            string targetName = CreateUniqueRecipeName(baseName);
            if (!RecipeWorkspaceService.DuplicateVisionWorkspace(sourceName, targetName))
            {
                StatusText = LocalText("레시피 복제에 실패했습니다.", "Duplicate failed.");
                return;
            }

            switchRecipe(targetName);
            StatusText = string.Format(
                CultureInfo.CurrentCulture,
                LocalText("복제됨: {0}", "Duplicated: {0}"),
                targetName);
            RefreshOptions();
            refreshAfterSwitch();
        }

        private bool CanDuplicateSelectedRecipe()
        {
            string selected = NormalizeRecipeName(selectedRecipeName);
            string requested = EditRecipeName?.Trim();
            return !string.IsNullOrWhiteSpace(selected)
                && RecipeOptions.Any(name => string.Equals(name, selected, StringComparison.OrdinalIgnoreCase))
                && (string.IsNullOrWhiteSpace(requested)
                    || RecipeWorkspaceService.IsValidRecipeName(requested));
        }

        private void RenameSelectedRecipe()
        {
            string oldName = NormalizeRecipeName(selectedRecipeName);
            string newName = NormalizeRecipeName(EditRecipeName);
            if (!CanRenameSelectedRecipe())
            {
                StatusText = LocalText("이름을 변경할 수 없습니다.", "Cannot rename this recipe.");
                return;
            }

            if (!RecipeWorkspaceService.RenameVisionWorkspace(oldName, newName))
            {
                StatusText = LocalText("이름 변경에 실패했습니다.", "Rename failed.");
                return;
            }

            switchRecipe(newName);
            StatusText = string.Format(
                CultureInfo.CurrentCulture,
                LocalText("이름 변경됨: {0}", "Renamed: {0}"),
                newName);
            RefreshOptions();
            refreshAfterSwitch();
        }

        private bool CanRenameSelectedRecipe()
        {
            string oldName = NormalizeRecipeName(selectedRecipeName);
            string newName = NormalizeRecipeName(EditRecipeName);
            return !string.IsNullOrWhiteSpace(oldName)
                && RecipeWorkspaceService.IsValidRecipeName(newName)
                && !string.Equals(oldName, newName, StringComparison.OrdinalIgnoreCase)
                && !RecipeOptions.Any(name => string.Equals(name, newName, StringComparison.OrdinalIgnoreCase));
        }

        private void DeleteSelectedRecipe()
        {
            string deletedName = NormalizeRecipeName(selectedRecipeName);
            if (!CanDeleteSelectedRecipe())
            {
                StatusText = LocalText("삭제할 수 없습니다.", "Cannot delete this recipe.");
                return;
            }

            if (!confirmDeleteRecipe(deletedName))
            {
                StatusText = LocalText("삭제가 취소되었습니다.", "Delete canceled.");
                return;
            }

            if (!RecipeWorkspaceService.DeleteVisionWorkspace(deletedName))
            {
                StatusText = LocalText("삭제에 실패했습니다.", "Delete failed.");
                return;
            }

            string fallback = RecipeOptions
                .FirstOrDefault(name => !string.Equals(name, deletedName, StringComparison.OrdinalIgnoreCase));
            fallback = NormalizeRecipeName(fallback);
            RecipeWorkspaceService.EnsureVisionWorkspace(fallback);
            switchRecipe(fallback);
            StatusText = string.Format(
                CultureInfo.CurrentCulture,
                LocalText("삭제됨: {0}", "Deleted: {0}"),
                deletedName);
            RefreshOptions();
            refreshAfterSwitch();
        }

        private bool CanDeleteSelectedRecipe()
        {
            string selected = NormalizeRecipeName(selectedRecipeName);
            return !string.IsNullOrWhiteSpace(selected)
                && RecipeOptions.Count > 1
                && RecipeOptions.Any(name => string.Equals(name, selected, StringComparison.OrdinalIgnoreCase));
        }

        private void ImportPipelineXml()
        {
            string path = selectImportPipelineXmlPath();
            if (string.IsNullOrWhiteSpace(path))
            {
                StatusText = LocalText("XML 가져오기가 취소되었습니다.", "Import canceled.");
                return;
            }

            ImportPipelineXmlFromPath(path);
        }

        public bool ImportPipelineXmlFromPath(string path)
        {
            string recipeName = NormalizeRecipeName(selectedRecipeName);
            if (!CanUseSelectedRecipe())
            {
                StatusText = LocalText("선택된 레시피가 없습니다.", "No recipe selected.");
                return false;
            }

            if (!VisionPipelineStorage.TryLoadFromFile(path, out VisionPipeline pipeline, out string message))
            {
                StatusText = message;
                UpdateSelectedRecipeSummary();
                return false;
            }

            string basePipelineName = string.IsNullOrWhiteSpace(pipeline.Name)
                ? Path.GetFileNameWithoutExtension(path)
                : pipeline.Name.Trim();
            pipeline.Name = CreateUniquePipelineName(recipeName, basePipelineName);
            VisionPipelineStorage.Save(recipeName, pipeline);
            VisionPipelineStorage.SaveActivePipelineName(recipeName, pipeline.Name);
            RefreshPipelineOptions(pipeline.Name);
            StatusText = string.Format(
                CultureInfo.CurrentCulture,
                LocalText("XML 가져오기 완료: {0}", "Imported XML: {0}"),
                pipeline.Name);
            RefreshOptions();
            refreshAfterSwitch();
            return true;
        }

        private void LoadLlmXmlDraft()
        {
            string path = selectImportPipelineXmlPath();
            if (string.IsNullOrWhiteSpace(path))
            {
                StatusText = LocalText("LLM XML 초안 로드가 취소되었습니다.", "LLM XML draft load canceled.");
                return;
            }

            LoadLlmXmlDraftFromPath(path);
        }

        public bool LoadLlmXmlDraftFromPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
            {
                StatusText = LocalText("LLM XML 초안 파일을 찾을 수 없습니다.", "LLM XML draft file was not found.");
                return false;
            }

            LlmXmlDraftText = File.ReadAllText(path);
            StatusText = LocalText("LLM XML 초안 로드됨: ", "Loaded LLM XML draft: ") + Path.GetFileName(path);
            return ValidateLlmXmlDraftText(false);
        }

        private void ValidateLlmXmlDraft()
        {
            ValidateLlmXmlDraftText(false);
        }

        public bool ValidateLlmXmlDraftTextForTest()
        {
            return ValidateLlmXmlDraftText(false);
        }

        private void ImportLlmXmlDraft()
        {
            if (!TryBuildLlmDraftPipeline(copyDependencies: true, out VisionPipeline pipeline, out string validationReport, out string dependencyReport))
            {
                LlmXmlDraftValidationReport = validationReport;
                LlmXmlDraftDependencyReport = dependencyReport;
                LlmXmlDraftReviewReport = LocalText("초안 검토 건너뜀: 검증 실패.", "Draft review skipped: validation failed.");
                LlmXmlDraftDiffReport = LocalText("변경점 검토 건너뜀: 검증 실패.", "Diff review skipped: validation failed.");
                StatusText = LocalText("LLM XML 초안을 가져올 수 없습니다.", "LLM XML draft is not importable.");
                return;
            }

            string recipeName = NormalizeRecipeName(selectedRecipeName);
            string basePipelineName = string.IsNullOrWhiteSpace(pipeline.Name)
                ? "LLM_Draft_" + DateTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture)
                : pipeline.Name.Trim();
            pipeline.Name = CreateUniquePipelineName(recipeName, basePipelineName);
            CopyReferenceImageForDraftImport(recipeName, pipeline.Name, ref dependencyReport);
            LlmXmlDraftReviewReport = BuildLlmDraftReviewReport(pipeline);
            LlmXmlDraftDiffReport = BuildLlmDraftDiffReport(pipeline);
            VisionPipelineStorage.Save(recipeName, pipeline);
            VisionPipelineStorage.SaveActivePipelineName(recipeName, pipeline.Name);
            LlmXmlDraftValidationReport = validationReport;
            LlmXmlDraftDependencyReport = dependencyReport;
            StatusText = LocalText("LLM XML 초안 가져오기 완료: ", "Imported LLM XML draft: ") + pipeline.Name;
            RefreshPipelineOptions(pipeline.Name);
            RefreshOptions();
            refreshAfterSwitch();
        }

        private void UseSelectedSampleReference()
        {
            if (SelectedSampleOption?.Sample == null || string.IsNullOrWhiteSpace(SelectedSampleOption.Sample.ImageFullPath))
            {
                StatusText = LocalText("선택된 샘플 이미지를 사용할 수 없습니다.", "No selected sample image is available.");
                return;
            }

            LlmReferenceImagePath = SelectedSampleOption.Sample.ImageFullPath;
            StatusText = LocalText("참조 이미지가 샘플에서 설정됨: ", "Reference image set from sample: ") + SelectedSampleOption.Sample.SampleName;
        }

        private async void RunSelectedSampleCheck()
        {
            if (!CanRunSelectedSampleCheck())
            {
                return;
            }

            OpenVisionRecipeSampleOption sampleOption = SelectedSampleOption;
            string pipelineName = SelectedPipelineOption?.PipelineName ?? string.Empty;
            string pipelinePath = RecipeWorkspaceService.GetVisionPipelinePath(
                NormalizeRecipeName(selectedRecipeName),
                pipelineName);

            isSampleCheckRunning = true;
            OnPropertyChanged(nameof(RunSelectedSampleCheckText));
            LatestSampleRunSummary = OpenVisionRecipeSampleRunSummary.CreateRunning(sampleOption, pipelineName);
            StatusText = LocalText("샘플 검사 실행 중: ", "Running sample check: ") + sampleOption.SampleName;
            RefreshCommandState();

            try
            {
                string pipelineXmlText = File.ReadAllText(pipelinePath);
                VisionPipelineSampleCheckResult result =
                    await VisionPipelineSampleCheckService.RunSampleCheckSafeAsync(sampleOption.Sample, pipelineXmlText);
                LatestSampleRunSummary = OpenVisionRecipeSampleRunSummary.FromResult(sampleOption, pipelineName, result);
                StatusText = LocalText("샘플 검사 ", "Sample check ") + result.Status + ": " + sampleOption.SampleName;
            }
            catch (Exception ex)
            {
                VisionPipelineSampleCheckResult result = VisionPipelineSampleCheckService.CreateErrorResult(
                    ex.GetBaseException().Message);
                LatestSampleRunSummary = OpenVisionRecipeSampleRunSummary.FromResult(sampleOption, pipelineName, result);
                StatusText = LocalText("샘플 검사 ERROR: ", "Sample check ERROR: ") + result.Message;
            }
            finally
            {
                isSampleCheckRunning = false;
                OnPropertyChanged(nameof(RunSelectedSampleCheckText));
                RefreshCommandState();
            }
        }

        private async void RunSelectedSamplePairCheck()
        {
            if (!CanRunSelectedSamplePairCheck())
            {
                return;
            }

            OpenVisionRecipeSampleOption sampleOption = SelectedSampleOption;
            string recipeName = NormalizeRecipeName(selectedRecipeName);
            string pipelineName = SelectedPipelineOption?.PipelineName ?? string.Empty;
            string pipelinePath = RecipeWorkspaceService.GetVisionPipelinePath(recipeName, pipelineName);
            List<VisionPipelineSampleCatalogItem> pairSamples = VisionPipelineSampleCheckService.GetPairSamples(sampleOption.Sample);

            isPairCheckRunning = true;
            OnPropertyChanged(nameof(RunSelectedSamplePairCheckText));
            LatestPairRunSummary = OpenVisionRecipePairRunSummary.CreateRunning(sampleOption, pipelineName, pairSamples.Count);
            StatusText = LocalText("Good/Bad 쌍 검사 실행 중: ", "Running Good/Bad pair check: ") + sampleOption.Sample.PairGroup;
            RefreshCommandState();

            DateTime startedAt = DateTime.Now;
            List<OpenVisionRecipePairSampleRunSummary> pairResults = new List<OpenVisionRecipePairSampleRunSummary>();
            List<VisionPipelineBatchSampleRunResult> storageResults = new List<VisionPipelineBatchSampleRunResult>();
            string summaryPath = string.Empty;
            try
            {
                string pipelineXmlText = File.ReadAllText(pipelinePath);
                foreach (VisionPipelineSampleCatalogItem sample in pairSamples)
                {
                    VisionPipelineSampleCheckResult result =
                        await VisionPipelineSampleCheckService.RunSampleCheckSafeAsync(sample, pipelineXmlText);
                    pairResults.Add(OpenVisionRecipePairSampleRunSummary.FromResult(sample, result));
                    storageResults.Add(new VisionPipelineBatchSampleRunResult
                    {
                        SampleName = sample?.SampleName ?? string.Empty,
                        Status = result?.Status ?? string.Empty,
                        Success = result?.Success ?? false,
                        TotalMilliseconds = result?.TotalMilliseconds ?? 0D,
                        FailedStep = result?.FailedStepText ?? string.Empty,
                        Message = result?.Message ?? string.Empty
                    });
                }

                summaryPath = VisionPipelineBatchRunSummaryStorage.Save(
                    recipeName,
                    pipelineName,
                    startedAt,
                    DateTime.Now,
                    storageResults);
                LatestPairRunSummary = OpenVisionRecipePairRunSummary.FromResults(
                    sampleOption,
                    pipelineName,
                    pairResults,
                    summaryPath);
                RefreshRecentBatchRunOptions();
                StatusText = LatestPairRunSummary.StatusText + ": " + sampleOption.Sample.PairGroup;
            }
            catch (Exception ex)
            {
                LatestPairRunSummary = OpenVisionRecipePairRunSummary.FromError(
                    sampleOption,
                    pipelineName,
                    ex.GetBaseException().Message);
                StatusText = LocalText("쌍 검사 ERROR: ", "Pair check ERROR: ") + ex.GetBaseException().Message;
            }
            finally
            {
                isPairCheckRunning = false;
                OnPropertyChanged(nameof(RunSelectedSamplePairCheckText));
                RefreshCommandState();
            }
        }

        private async void RunCatalogBenchmark()
        {
            if (!CanRunCatalogBenchmark())
            {
                return;
            }

            string recipeName = NormalizeRecipeName(selectedRecipeName);
            string pipelineName = SelectedPipelineOption?.PipelineName ?? string.Empty;
            string pipelinePath = RecipeWorkspaceService.GetVisionPipelinePath(recipeName, pipelineName);
            List<VisionPipelineSampleCatalogItem> samples = BuildCatalogBenchmarkSamples();

            isCatalogBenchmarkRunning = true;
            OnPropertyChanged(nameof(RunCatalogBenchmarkText));
            OnPropertyChanged(nameof(RunCatalogBenchmarkShortText));
            LatestCatalogBenchmarkSummary = OpenVisionRecipeCatalogBenchmarkSummary.CreateRunning(pipelineName, samples.Count);
            StatusText = LocalText("카탈로그 벤치마크 실행 중: ", "Running catalog benchmark: ") + pipelineName;
            RefreshCommandState();

            DateTime startedAt = DateTime.Now;
            List<VisionPipelineBatchSampleRunResult> storageResults = new List<VisionPipelineBatchSampleRunResult>();
            try
            {
                string pipelineXmlText = File.ReadAllText(pipelinePath);
                for (int index = 0; index < samples.Count; index++)
                {
                    VisionPipelineSampleCatalogItem sample = samples[index];
                    VisionPipelineSampleCheckResult result =
                        await VisionPipelineSampleCheckService.RunSampleCheckSafeAsync(sample, pipelineXmlText);

                    storageResults.Add(new VisionPipelineBatchSampleRunResult
                    {
                        SampleName = sample?.SampleName ?? string.Empty,
                        Status = result?.Status ?? string.Empty,
                        Success = result?.Success ?? false,
                        TotalMilliseconds = result?.TotalMilliseconds ?? 0D,
                        FailedStep = result?.FailedStepText ?? string.Empty,
                        Message = FormatCatalogBenchmarkMessage(result),
                        ReportPath = sample?.ImageFullPath ?? string.Empty
                    });

                    if ((index + 1) == samples.Count || (index + 1) % 10 == 0)
                    {
                        LatestCatalogBenchmarkSummary = OpenVisionRecipeCatalogBenchmarkSummary.CreateProgress(
                            pipelineName,
                            index + 1,
                            samples.Count,
                            storageResults);
                    }
                }

                string summaryPath = VisionPipelineBatchRunSummaryStorage.Save(
                    recipeName,
                    pipelineName,
                    startedAt,
                    DateTime.Now,
                    storageResults);
                LatestCatalogBenchmarkSummary = OpenVisionRecipeCatalogBenchmarkSummary.FromResults(
                    pipelineName,
                    storageResults,
                    summaryPath);
                RefreshRecentBatchRunOptions();
                StatusText = LatestCatalogBenchmarkSummary.CompactText;
            }
            catch (Exception ex)
            {
                LatestCatalogBenchmarkSummary = OpenVisionRecipeCatalogBenchmarkSummary.FromError(
                    pipelineName,
                    ex.GetBaseException().Message);
                StatusText = LocalText("카탈로그 벤치마크 ERROR: ", "Catalog benchmark ERROR: ") + ex.GetBaseException().Message;
            }
            finally
            {
                isCatalogBenchmarkRunning = false;
                OnPropertyChanged(nameof(RunCatalogBenchmarkText));
                OnPropertyChanged(nameof(RunCatalogBenchmarkShortText));
                RefreshCommandState();
            }
        }

        private void BuildLlmPrompt()
        {
            LlmPromptText = BuildLlmPromptText();
            StatusText = LocalText("현재 레시피 컨텍스트에서 LLM 프롬프트를 생성했습니다.", "Built LLM prompt from current recipe context.");
        }

        private bool CanCopyLlmPrompt()
        {
            return !string.IsNullOrWhiteSpace(LlmPromptText);
        }

        private void CopyLlmPrompt()
        {
            string prompt = LlmPromptText;
            if (string.IsNullOrWhiteSpace(prompt))
            {
                LlmPromptCopyStatusText = LocalText("복사할 프롬프트가 없습니다.", "No prompt to copy.");
                return;
            }

            try
            {
                System.Windows.Clipboard.SetText(prompt);
                LlmPromptCopyStatusText = LocalText("프롬프트가 클립보드에 복사되었습니다.", "Prompt copied to clipboard.");
            }
            catch (Exception ex)
            {
                LlmPromptCopyStatusText = LocalText("프롬프트 복사 실패: ", "Prompt copy failed: ") + ex.Message;
            }
        }

        private bool CanCopyLlmReviewBundle()
        {
            return !string.IsNullOrWhiteSpace(LlmXmlDraftText)
                || !string.IsNullOrWhiteSpace(LlmXmlDraftValidationReport)
                || !string.IsNullOrWhiteSpace(LlmXmlDraftDependencyReport)
                || !string.IsNullOrWhiteSpace(LlmXmlDraftDiffReport);
        }

        private void CopyLlmReviewBundle()
        {
            string bundle = BuildLlmReviewBundleText();
            if (string.IsNullOrWhiteSpace(bundle))
            {
                LlmReviewBundleCopyStatusText = LocalText("복사할 검토 묶음이 없습니다.", "No review bundle to copy.");
                return;
            }

            try
            {
                System.Windows.Clipboard.SetText(bundle);
                LlmReviewBundleCopyStatusText = LocalText("LLM 검토 묶음이 클립보드에 복사되었습니다.", "LLM review bundle copied to clipboard.");
            }
            catch (Exception ex)
            {
                LlmReviewBundleCopyStatusText = LocalText("LLM 검토 묶음 복사 실패: ", "LLM review bundle copy failed: ") + ex.Message;
            }
        }

        private void PasteLlmXmlDraftFromClipboard()
        {
            try
            {
                if (!System.Windows.Clipboard.ContainsText())
                {
                    LlmXmlDraftPasteStatusText = LocalText("클립보드에 붙여넣을 XML 텍스트가 없습니다.", "Clipboard does not contain XML text.");
                    return;
                }

                string xmlText = System.Windows.Clipboard.GetText();
                if (string.IsNullOrWhiteSpace(xmlText))
                {
                    LlmXmlDraftPasteStatusText = LocalText("클립보드 XML 텍스트가 비어 있습니다.", "Clipboard XML text is empty.");
                    return;
                }

                LlmXmlDraftText = xmlText;
                LlmXmlDraftPasteStatusText = LocalText(
                    "클립보드 XML을 초안에 붙여넣었습니다. 검증을 눌러 확인하세요.",
                    "Pasted clipboard XML into the draft. Click Validate to check it.");
                StatusText = LocalText("LLM XML 초안을 클립보드에서 붙여넣었습니다.", "LLM XML draft pasted from clipboard.");
            }
            catch (Exception ex)
            {
                LlmXmlDraftPasteStatusText = LocalText("XML 붙여넣기 실패: ", "Paste XML failed: ") + ex.Message;
            }
        }

        private string BuildLlmReviewBundleText()
        {
            if (!CanCopyLlmReviewBundle())
            {
                return string.Empty;
            }

            string recipeName = NormalizeRecipeName(selectedRecipeName);
            string pipelineName = SelectedPipelineOption?.PipelineName ?? string.Empty;
            return string.Join(Environment.NewLine, new[]
            {
                "OpenVisionLab LLM XML review bundle",
                "Instruction: revise the VisionPipeline XML using this feedback. Return only a VisionPipeline XML document.",
                "Recipe: " + recipeName,
                "Pipeline: " + (string.IsNullOrWhiteSpace(pipelineName) ? "-" : pipelineName),
                "Inspection intent: " + SelectedLlmToolTemplate,
                "Intent contract: " + BuildLlmIntentContractText(SelectedLlmToolTemplate),
                "",
                "[Correction rules]",
                "- Use only OpenVisionLab VisionPipeline XML and return only XML.",
                "- Use InputLayer=\"Main\" or the exact OutputLayer of a previous enabled step; do not invent layers.",
                "- Use supported OpenVisionLab ToolType names and PropertyGrid-compatible parameter values.",
                "- Do not switch to another tool family unless the selected intent contract explicitly allows it.",
                "- Replace missing template/image dependency paths with existing files, or remove those dependency parameters until a real file is selected.",
                "- Do not add camera, lighting, PLC, I/O, account, Preview, or Run instructions.",
                "",
                "[Result channel contract]",
                BuildLlmResultChannelContractText(),
                "",
                "[Selected step operator context]",
                string.IsNullOrWhiteSpace(PipelineSelectedStepOperatorContextText) ? "-" : PipelineSelectedStepOperatorContextText,
                "",
                "[Failure review]",
                string.IsNullOrWhiteSpace(FailureReviewText) ? "-" : FailureReviewText,
                "",
                "[Validation report]",
                string.IsNullOrWhiteSpace(LlmXmlDraftValidationReport) ? "-" : LlmXmlDraftValidationReport,
                "",
                "[Dependency report]",
                string.IsNullOrWhiteSpace(LlmXmlDraftDependencyReport) ? "-" : LlmXmlDraftDependencyReport,
                "",
                "[Draft import review]",
                string.IsNullOrWhiteSpace(LlmXmlDraftReviewReport) ? "-" : LlmXmlDraftReviewReport,
                "",
                "[Diff review]",
                string.IsNullOrWhiteSpace(LlmXmlDraftDiffReport) ? "-" : LlmXmlDraftDiffReport,
                "",
                "[Current XML draft]",
                string.IsNullOrWhiteSpace(LlmXmlDraftText) ? "-" : LlmXmlDraftText
            });
        }

        private void CreateLlmTemplateXmlDraft()
        {
            VisionPipeline pipeline = CreateLlmTemplatePipeline();
            LlmPromptText = BuildLlmPromptText();
            LlmXmlDraftText = SerializePipelineToXmlText(pipeline);
            ValidateLlmXmlDraftText(false);
            StatusText = LocalText("LLM 템플릿에서 XML 시작안을 생성했습니다: ", "Created XML starter from LLM template: ") + SelectedLlmToolTemplate;
        }

        private string BuildPinGapIntentLatestRunText()
        {
            OpenVisionRecipeSampleRunSummary sample = LatestSampleRunSummary ?? OpenVisionRecipeSampleRunSummary.Empty;
            if (!sample.HasResult)
            {
                return LocalText(
                    "최근 샘플: 아직 실행 결과가 없습니다. Pin gap XML을 가져온 뒤 샘플 검사를 실행하면 DistanceMmAvg/DistanceMmRange가 여기에 표시됩니다.",
                    "Latest sample: no run result yet. Import Pin gap XML and run the sample check to show DistanceMmAvg/DistanceMmRange here.");
            }

            string metrics = sample.DistanceMetricText;
            if (string.IsNullOrWhiteSpace(metrics))
            {
                return LocalText(
                    "최근 샘플: DistanceMmAvg/DistanceMmRange가 아직 없습니다. Pin gap XML을 가져온 뒤 샘플 검사를 실행하세요.",
                    "Latest sample: no DistanceMmAvg/DistanceMmRange yet. Import Pin gap XML and run the sample check.");
            }

            return LocalText("최근 샘플: ", "Latest sample: ")
                + metrics
                + " / "
                + ResolvePinGapMetricAdvice(metrics);
        }

        private string ResolvePinGapMetricAdvice(string metrics)
        {
            bool hasAverage = OpenVisionRecipePinGapIntentSkill.TryExtractMetricValue(metrics, VisionPipelineKnownMetrics.DistanceMmAvg, out double average);
            bool hasRange = OpenVisionRecipePinGapIntentSkill.TryExtractMetricValue(metrics, VisionPipelineKnownMetrics.DistanceMmRange, out double range);

            if (hasRange
                && OpenVisionRecipePinGapIntentSkill.TryParsePositiveDouble(PinGapIntentRangeMaxText, out double rangeMax)
                && range > rangeMax)
            {
                return LocalText(
                    "판정: Range NG -> ROI를 핀 간격만 남기고 줄인 뒤 edge contrast/sampling을 먼저 조정",
                    "Decision: Range NG -> narrow ROI to the pin gap first, then tune edge contrast/sampling");
            }

            bool hasMinimum = OpenVisionRecipePinGapIntentSkill.TryParsePositiveDouble(PinGapIntentDistanceMinText, out double minimum);
            bool hasMaximum = OpenVisionRecipePinGapIntentSkill.TryParsePositiveDouble(PinGapIntentDistanceMaxText, out double maximum);
            if (hasAverage
                && ((hasMinimum && average < minimum) || (hasMaximum && average > maximum)))
            {
                return LocalText(
                    "판정: Avg NG -> mm/px 또는 Min/Max spec을 조정",
                    "Decision: Avg NG -> tune mm/px or Min/Max spec");
            }

            if (hasAverage && !hasRange)
            {
                return LocalText(
                    "판정: Avg만 있음 -> Range gate가 있는 Pin gap XML로 샘플을 다시 실행",
                    "Decision: Avg only -> rerun with Pin gap XML that includes the Range gate");
            }

            if (hasAverage || hasRange)
            {
                return LocalText(
                    "판정: 현재 입력 기준에서는 Distance gate가 OK",
                    "Decision: Distance gates are OK against the current fields");
            }

            return LocalText(
                "판정: Distance metric 없음 -> LineDistance/Pin gap XML로 샘플을 다시 실행",
                "Decision: no distance metric -> rerun with LineDistance/Pin gap XML");
        }

        private string BuildBlobCountIntentLatestRunText()
        {
            OpenVisionRecipeSampleRunSummary sample = LatestSampleRunSummary ?? OpenVisionRecipeSampleRunSummary.Empty;
            if (!sample.HasResult)
            {
                return LocalText(
                    "Latest sample: no run result yet. Import Blob count XML and run the sample check to show ResultCount here.",
                    "Latest sample: no run result yet. Import Blob count XML and run the sample check to show ResultCount here.");
            }

            string metrics = sample.DisplayText;
            if (!OpenVisionRecipeBlobCountIntentSkill.TryExtractMetricValue(metrics, VisionPipelineKnownMetrics.ResultCount, out double count))
            {
                return LocalText(
                    "Latest sample: no ResultCount yet. Import Blob count XML and run the sample check.",
                    "Latest sample: no ResultCount yet. Import Blob count XML and run the sample check.");
            }

            return LocalText("Latest sample: ResultCount=", "Latest sample: ResultCount=")
                + count.ToString("0.###", CultureInfo.InvariantCulture)
                + " / "
                + ResolveBlobCountMetricAdvice(count);
        }

        private string ResolveBlobCountMetricAdvice(double count)
        {
            bool hasMinimum = OpenVisionRecipeBlobCountIntentSkill.TryParseNonNegativeInt(BlobCountIntentMinCountText, out int minimum);
            bool hasMaximum = OpenVisionRecipeBlobCountIntentSkill.TryParseNonNegativeInt(BlobCountIntentMaxCountText, out int maximum);
            if (hasMinimum && hasMaximum && minimum <= maximum && (count < minimum || count > maximum))
            {
                return LocalText(
                    "Decision: Count NG -> tune threshold, ROI, or area limits",
                    "Decision: Count NG -> tune threshold, ROI, or area limits");
            }

            if (hasMinimum && hasMaximum && minimum > maximum)
            {
                return LocalText(
                    "Decision: count field range is invalid",
                    "Decision: count field range is invalid");
            }

            return LocalText(
                "Decision: ResultCount gate is OK against the current fields",
                "Decision: ResultCount gate is OK against the current fields");
        }

        private void NotifyBlobCountIntentTextChanged()
        {
            OnPropertyChanged(nameof(BlobCountIntentWorkflowText));
            OnPropertyChanged(nameof(BlobCountIntentFeedbackText));
            OnPropertyChanged(nameof(BlobCountIntentLatestRunText));
            RefreshCommandState();
        }

        private string BuildContourCountIntentLatestRunText()
        {
            OpenVisionRecipeSampleRunSummary sample = LatestSampleRunSummary ?? OpenVisionRecipeSampleRunSummary.Empty;
            if (!sample.HasResult)
            {
                return LocalText(
                    "Latest sample: no run result yet. Import Contour XML and run the sample check to show ResultCount/AreaMax here.",
                    "Latest sample: no run result yet. Import Contour XML and run the sample check to show ResultCount/AreaMax here.");
            }

            string metrics = sample.DisplayText;
            bool hasCount = OpenVisionRecipeContourCountIntentSkill.TryExtractMetricValue(metrics, VisionPipelineKnownMetrics.ResultCount, out double count);
            bool hasAreaMax = OpenVisionRecipeContourCountIntentSkill.TryExtractMetricValue(metrics, VisionPipelineKnownMetrics.AreaMax, out double areaMax);
            if (!hasCount && !hasAreaMax)
            {
                return LocalText(
                    "Latest sample: no ResultCount/AreaMax yet. Import Contour XML and run the sample check.",
                    "Latest sample: no ResultCount/AreaMax yet. Import Contour XML and run the sample check.");
            }

            List<string> parts = new List<string>();
            if (hasCount)
            {
                parts.Add(VisionPipelineKnownMetrics.ResultCount + "=" + count.ToString("0.###", CultureInfo.InvariantCulture));
            }

            if (hasAreaMax)
            {
                parts.Add(VisionPipelineKnownMetrics.AreaMax + "=" + areaMax.ToString("0.###", CultureInfo.InvariantCulture));
            }

            return LocalText("Latest sample: ", "Latest sample: ")
                + string.Join(", ", parts)
                + " / "
                + ResolveContourCountMetricAdvice(count, hasCount, areaMax, hasAreaMax);
        }

        private string ResolveContourCountMetricAdvice(double count, bool hasCount, double areaMax, bool hasAreaMax)
        {
            bool hasMinimum = OpenVisionRecipeContourCountIntentSkill.TryParseNonNegativeInt(ContourCountIntentMinCountText, out int minimum);
            bool hasMaximum = OpenVisionRecipeContourCountIntentSkill.TryParseNonNegativeInt(ContourCountIntentMaxCountText, out int maximum);
            if (hasCount && hasMinimum && hasMaximum && minimum <= maximum && (count < minimum || count > maximum))
            {
                return LocalText(
                    "Decision: Count NG -> tune threshold, ROI, or area limits",
                    "Decision: Count NG -> tune threshold, ROI, or area limits");
            }

            if (hasAreaMax
                && OpenVisionRecipeContourCountIntentSkill.TryParsePositiveInt(ContourCountIntentMaxAreaText, out int maxArea)
                && areaMax > maxArea)
            {
                return LocalText(
                    "Decision: AreaMax NG -> reduce oversized contour before accepting",
                    "Decision: AreaMax NG -> reduce oversized contour before accepting");
            }

            if (hasMinimum && hasMaximum && minimum > maximum)
            {
                return LocalText(
                    "Decision: count field range is invalid",
                    "Decision: count field range is invalid");
            }

            return LocalText(
                "Decision: Contour gates are OK against the current fields",
                "Decision: Contour gates are OK against the current fields");
        }

        private void NotifyContourCountIntentTextChanged()
        {
            OnPropertyChanged(nameof(ContourCountIntentWorkflowText));
            OnPropertyChanged(nameof(ContourCountIntentFeedbackText));
            OnPropertyChanged(nameof(ContourCountIntentLatestRunText));
            RefreshCommandState();
        }

        private void CreatePinGapIntentXmlDraft()
        {
            if (!OpenVisionRecipePinGapIntentSkill.TryParseRoi(PinGapIntentRoiText, out int roiX, out int roiY, out int roiWidth, out int roiHeight, out string roiMessage)
                || !OpenVisionRecipePinGapIntentSkill.TryParsePositiveDouble(PinGapIntentDistanceMinText, out double minDistanceMm)
                || !OpenVisionRecipePinGapIntentSkill.TryParsePositiveDouble(PinGapIntentDistanceMaxText, out double maxDistanceMm)
                || !OpenVisionRecipePinGapIntentSkill.TryParsePositiveDouble(PinGapIntentRangeMaxText, out double maxRangeMm)
                || !OpenVisionRecipePinGapIntentSkill.TryParsePositiveDouble(PinGapIntentScaleText, out double mmPerPixel))
            {
                StatusText = LocalText(
                    "핀 간격 skill 입력을 확인하세요. ROI는 x,y,w,h이고 거리/Range/mm/px는 양수여야 합니다. ",
                    "Check Pin gap skill inputs. ROI must be x,y,w,h and distance/range/mm-per-pixel values must be positive. ")
                    + roiMessage;
                return;
            }

            if (minDistanceMm > maxDistanceMm)
            {
                StatusText = LocalText("핀 간격 Min mm은 Max mm보다 클 수 없습니다.", "Pin gap Min mm cannot be greater than Max mm.");
                return;
            }

            SelectedLlmToolTemplate = "Pin gap / edge distance (LineDistance)";
            VisionPipeline pipeline = OpenVisionRecipePinGapIntentSkill.CreatePipeline(
                roiX,
                roiY,
                roiWidth,
                roiHeight,
                minDistanceMm,
                maxDistanceMm,
                maxRangeMm,
                mmPerPixel);

            LlmPromptText = BuildLlmPromptText()
                + Environment.NewLine
                + Environment.NewLine
                + "[Pin gap skill inputs]"
                + Environment.NewLine
                + "ROI: " + OpenVisionRecipePinGapIntentSkill.FormatRoi(roiX, roiY, roiWidth, roiHeight)
                + Environment.NewLine
                + "Nominal distance mm: " + minDistanceMm.ToString("0.###", CultureInfo.InvariantCulture)
                + ".." + maxDistanceMm.ToString("0.###", CultureInfo.InvariantCulture)
                + Environment.NewLine
                + "Consistency range max mm: " + maxRangeMm.ToString("0.###", CultureInfo.InvariantCulture)
                + Environment.NewLine
                + "Scale mm/px: " + mmPerPixel.ToString("0.######", CultureInfo.InvariantCulture)
                + Environment.NewLine
                + "Generated contract: Step 1 judges DistanceMmAvg, Step 2 judges DistanceMmRange. Neither Step runs until the user explicitly validates/imports/runs.";
            LlmXmlDraftText = SerializePipelineToXmlText(pipeline);
            ValidateLlmXmlDraftText(false);
            StatusText = LocalText(
                "핀 간격 skill XML 초안을 생성했습니다. Preview/Run은 실행하지 않았습니다.",
                "Created Pin gap skill XML draft. Preview/Run was not executed.");
        }

        private void CreateBlobCountIntentXmlDraft()
        {
            if (!OpenVisionRecipeBlobCountIntentSkill.TryParseRoi(BlobCountIntentRoiText, out int roiX, out int roiY, out int roiWidth, out int roiHeight, out string roiMessage)
                || !OpenVisionRecipeBlobCountIntentSkill.TryParseByte(BlobCountIntentThresholdText, out int threshold)
                || !OpenVisionRecipeBlobCountIntentSkill.TryParseNonNegativeInt(BlobCountIntentMinCountText, out int minCount)
                || !OpenVisionRecipeBlobCountIntentSkill.TryParseNonNegativeInt(BlobCountIntentMaxCountText, out int maxCount)
                || !OpenVisionRecipeBlobCountIntentSkill.TryParsePositiveInt(BlobCountIntentMinAreaText, out int minArea)
                || !OpenVisionRecipeBlobCountIntentSkill.TryParsePositiveInt(BlobCountIntentMaxAreaText, out int maxArea))
            {
                StatusText = LocalText(
                    "Check Blob count skill inputs. ROI must be x,y,w,h, threshold must be 0..255, count must be 0 or greater, and area values must be positive. ",
                    "Check Blob count skill inputs. ROI must be x,y,w,h, threshold must be 0..255, count must be 0 or greater, and area values must be positive. ")
                    + roiMessage;
                return;
            }

            if (minCount > maxCount)
            {
                StatusText = LocalText("Blob count Min count cannot be greater than Max count.", "Blob count Min count cannot be greater than Max count.");
                return;
            }

            if (minArea > maxArea)
            {
                StatusText = LocalText("Blob count Min area cannot be greater than Max area.", "Blob count Min area cannot be greater than Max area.");
                return;
            }

            SelectedLlmToolTemplate = "Threshold + Blob";
            VisionPipeline pipeline = OpenVisionRecipeBlobCountIntentSkill.CreatePipeline(
                roiX,
                roiY,
                roiWidth,
                roiHeight,
                threshold,
                minCount,
                maxCount,
                minArea,
                maxArea);

            LlmPromptText = BuildLlmPromptText()
                + Environment.NewLine
                + Environment.NewLine
                + "[Blob count skill inputs]"
                + Environment.NewLine
                + "ROI: " + OpenVisionRecipeBlobCountIntentSkill.FormatRoi(roiX, roiY, roiWidth, roiHeight)
                + Environment.NewLine
                + "Threshold: " + threshold.ToString(CultureInfo.InvariantCulture)
                + Environment.NewLine
                + "Expected ResultCount: " + minCount.ToString(CultureInfo.InvariantCulture)
                + ".." + maxCount.ToString(CultureInfo.InvariantCulture)
                + Environment.NewLine
                + "Blob area px: " + minArea.ToString(CultureInfo.InvariantCulture)
                + ".." + maxArea.ToString(CultureInfo.InvariantCulture)
                + Environment.NewLine
                + "Generated contract: Step 1 creates a binary layer, Step 2 judges ResultCount. Neither Step runs until the user explicitly validates/imports/runs.";
            LlmXmlDraftText = SerializePipelineToXmlText(pipeline);
            ValidateLlmXmlDraftText(false);
            StatusText = LocalText(
                "Created Blob count skill XML draft. Preview/Run was not executed.",
                "Created Blob count skill XML draft. Preview/Run was not executed.");
        }

        private void CreateContourCountIntentXmlDraft()
        {
            if (!OpenVisionRecipeContourCountIntentSkill.TryParseRoi(ContourCountIntentRoiText, out int roiX, out int roiY, out int roiWidth, out int roiHeight, out string roiMessage)
                || !OpenVisionRecipeContourCountIntentSkill.TryParseByte(ContourCountIntentThresholdText, out int threshold)
                || !OpenVisionRecipeContourCountIntentSkill.TryParseNonNegativeInt(ContourCountIntentMinCountText, out int minCount)
                || !OpenVisionRecipeContourCountIntentSkill.TryParseNonNegativeInt(ContourCountIntentMaxCountText, out int maxCount)
                || !OpenVisionRecipeContourCountIntentSkill.TryParsePositiveInt(ContourCountIntentMinAreaText, out int minArea)
                || !OpenVisionRecipeContourCountIntentSkill.TryParsePositiveInt(ContourCountIntentMaxAreaText, out int maxArea))
            {
                StatusText = LocalText(
                    "Check Contour skill inputs. ROI must be x,y,w,h, threshold must be 0..255, count must be 0 or greater, and area values must be positive. ",
                    "Check Contour skill inputs. ROI must be x,y,w,h, threshold must be 0..255, count must be 0 or greater, and area values must be positive. ")
                    + roiMessage;
                return;
            }

            if (minCount > maxCount)
            {
                StatusText = LocalText("Contour Min count cannot be greater than Max count.", "Contour Min count cannot be greater than Max count.");
                return;
            }

            if (minArea > maxArea)
            {
                StatusText = LocalText("Contour Min area cannot be greater than Max area.", "Contour Min area cannot be greater than Max area.");
                return;
            }

            SelectedLlmToolTemplate = "Shape boundary (Contour)";
            VisionPipeline pipeline = OpenVisionRecipeContourCountIntentSkill.CreatePipeline(
                roiX,
                roiY,
                roiWidth,
                roiHeight,
                threshold,
                minCount,
                maxCount,
                minArea,
                maxArea);

            LlmPromptText = BuildLlmPromptText()
                + Environment.NewLine
                + Environment.NewLine
                + "[Contour count/size skill inputs]"
                + Environment.NewLine
                + "ROI: " + OpenVisionRecipeContourCountIntentSkill.FormatRoi(roiX, roiY, roiWidth, roiHeight)
                + Environment.NewLine
                + "Threshold: " + threshold.ToString(CultureInfo.InvariantCulture)
                + Environment.NewLine
                + "Expected ResultCount: " + minCount.ToString(CultureInfo.InvariantCulture)
                + ".." + maxCount.ToString(CultureInfo.InvariantCulture)
                + Environment.NewLine
                + "Contour area px: " + minArea.ToString(CultureInfo.InvariantCulture)
                + ".." + maxArea.ToString(CultureInfo.InvariantCulture)
                + Environment.NewLine
                + "Generated contract: Step 1 creates a binary layer, Step 2 judges ResultCount, Step 3 judges AreaMax. No Step runs until the user explicitly validates/imports/runs.";
            LlmXmlDraftText = SerializePipelineToXmlText(pipeline);
            ValidateLlmXmlDraftText(false);
            StatusText = LocalText(
                "Created Contour count/size skill XML draft. Preview/Run was not executed.",
                "Created Contour count/size skill XML draft. Preview/Run was not executed.");
        }

        private void RefreshLlmDraftReview()
        {
            ValidateLlmXmlDraftText(false);
        }

        public void CreateLlmTemplateXmlDraftForTest()
        {
            CreateLlmTemplateXmlDraft();
        }

        public void RefreshRecentBatchRunOptionsForTest()
        {
            RefreshRecentBatchRunOptions();
        }

        public void SetPairRunSummaryForTest(IReadOnlyList<OpenVisionRecipePairSampleRunSummary> results)
        {
            LatestPairRunSummary = OpenVisionRecipePairRunSummary.FromResults(
                SelectedSampleOption,
                SelectedPipelineOption?.PipelineName ?? string.Empty,
                results ?? Array.Empty<OpenVisionRecipePairSampleRunSummary>(),
                string.Empty);
        }

        public void SetCatalogBenchmarkSummaryForTest(IReadOnlyList<VisionPipelineBatchSampleRunResult> results)
        {
            LatestCatalogBenchmarkSummary = OpenVisionRecipeCatalogBenchmarkSummary.FromResults(
                SelectedPipelineOption?.PipelineName ?? string.Empty,
                results ?? Array.Empty<VisionPipelineBatchSampleRunResult>(),
                string.Empty);
        }

        private bool ValidateLlmXmlDraftText(bool copyDependencies)
        {
            bool ok = TryBuildLlmDraftPipeline(copyDependencies, out VisionPipeline pipeline, out string validationReport, out string dependencyReport);
            LlmXmlDraftValidationReport = validationReport;
            LlmXmlDraftDependencyReport = dependencyReport;
            LlmXmlDraftReviewReport = ok ? BuildLlmDraftReviewReport(pipeline) : LocalText("초안 검토 건너뜀: 검증 실패.", "Draft review skipped: validation failed.");
            LlmXmlDraftDiffReport = ok ? BuildLlmDraftDiffReport(pipeline) : LocalText("변경점 검토 건너뜀: 검증 실패.", "Diff review skipped: validation failed.");
            StatusText = ok ? LocalText("LLM XML 초안 검증 OK.", "LLM XML draft validation OK.") : LocalText("LLM XML 초안 검증 NG.", "LLM XML draft validation NG.");
            return ok;
        }

        private string BuildLlmPromptText()
        {
            string recipeName = NormalizeRecipeName(selectedRecipeName);
            string activePipelineName = VisionPipelineStorage.LoadActivePipelineName(recipeName, VisionPipelineAppendService.DefaultPipelineName);
            string goal = string.IsNullOrWhiteSpace(LlmInspectionGoalText)
                ? "Describe the inspection target and OK/NG criteria."
                : LlmInspectionGoalText.Trim();
            string detectionPoints = string.IsNullOrWhiteSpace(LlmDetectionPointText)
                ? "List the target ROIs, features, expected pass/fail thresholds, and required output layers."
                : LlmDetectionPointText.Trim();
            string referenceImage = string.IsNullOrWhiteSpace(LlmReferenceImagePath)
                ? "No reference image path is selected in OpenVisionLab."
                : LlmReferenceImagePath.Trim();

            return string.Join(Environment.NewLine, new[]
            {
                "Create an OpenVisionLab VisionPipeline XML draft.",
                "Product identity: OpenCvSharp4 rule-based vision workbench; no camera, lighting, PLC, or I/O setup.",
                "Use only OpenVisionLab pipeline tools and parameters. Keep algorithm tool parameters compatible with PropertyGrid-backed tools.",
                "Selected inspection intent: " + SelectedLlmToolTemplate,
                "Intent contract: " + BuildLlmIntentContractText(SelectedLlmToolTemplate),
                "Hard rule: do not switch to another tool family unless the selected intent contract explicitly allows it.",
                "Never overwrite the input layer. Read from Main unless a previous step output is intentionally used.",
                "Use score and weight parameters such as SCORE_MIN, GREEDINESS, and HYBRID_VERIFY_IMAGE_WEIGHT as 0..1 decimals, not percentages.",
                "Use positive numeric values for MAGNIFIATION, RANSAC_REPROJ_THRESHOLD, and COARSE_ANGLE_STEP.",
                "Keep FIND_ANGLE_MIN less than or equal to FIND_ANGLE_MAX.",
                "Use only existing template/image dependency paths. If no real file is available, omit dependency path parameters and explain the missing file outside the XML request.",
                "Do not run Preview/Run automatically. The user will validate and import the XML explicitly.",
                "Recipe: " + recipeName,
                "Current active pipeline: " + activePipelineName,
                "Preferred tool template: " + SelectedLlmToolTemplate,
                "Template guidance: " + ResolveTemplateGuidance(SelectedLlmToolTemplate),
                "Reference image: " + referenceImage,
                "Inspection goal: " + goal,
                "Detection points: " + detectionPoints,
                "",
                "[Result channel contract]",
                BuildLlmResultChannelContractText(),
                "Required response: return only a VisionPipeline XML document that can be loaded by OpenVisionLab."
            });
        }

        private static string BuildLlmResultChannelContractText()
        {
            return string.Join(Environment.NewLine, new[]
            {
                "- Inspection.Status: final OK/NG is derived by OpenVisionLab from XML validation plus explicit sample/Good/Bad checks after import.",
                "- Inspection.FailedStep: every enabled step must have a clear Name, InputLayer, OutputLayer, and ToolType so failures can point to the exact step.",
                "- Inspection.Evidence: create explicit output layers and measurable parameters such as SCORE_MIN, MIN/MAX, THRESHOLD, AREA, DISTANCE, or MEAN when the tool supports them.",
                "- Inspection.Benchmark: keep deterministic parameters and dependency paths so catalog and run-history comparison can be repeated.",
                "- Inspection.NextAction: do not add custom Inspection.* XML elements; these are logical review channels mapped by OpenVisionLab after import."
            });
        }

        private static string BuildLlmIntentContractText(string template)
        {
            if (IsLineDistanceTemplate(template))
            {
                return "Use ToolType=LineDistance only for edge-to-edge or pin-to-pin distance. Primary value metrics: DistancePxAvg; use DistanceMmAvg only when PIXELPERMM is known. Quality metrics: DistancePxRange/DistanceMmRange and DistancePxMax/DistanceMmMax must be checked so one long outlier line cannot pass through the average. If both nominal distance and consistency must be judged, duplicate the same LineDistance parameters into a second validation Step with a separate OutputLayer. Required parameters: USE_ROI/CvROI, LeftPRJ_DIR, RightPRJ_DIR, PRJ_PORALITY, CONTRAST, THICKNESS, SAMPLING_STEP, POINT_RANGE. Do not use Blob or Contour to measure distance. ROI must cover a narrow measurement band across the two edges, not the full object or empty background.";
            }

            if (IsContourTemplate(template))
            {
                return "Use ToolType=Contour only for boundary, chip, scratch, shape, or region outline checks. Primary metrics: ResultCount, AreaAvg, BoundsWidthAvg, BoundsHeightAvg. Do not use Contour for pin-to-pin gap measurement.";
            }

            if (IsBlobTemplate(template))
            {
                return "Use Threshold followed by Blob for connected object count, area, position, or foreground presence checks. Primary metrics: ResultCount and AreaAvg.";
            }

            if (IsEdgeBasedTemplate(template))
            {
                return "Use ToolType=EdgeBasedMatching for template-like shape matching when edge geometry is more stable than intensity. Primary metrics: ScoreMax and ResultCount.";
            }

            if (IsMeanTemplate(template))
            {
                return "Use ToolType=Mean for region brightness or intensity band judgment. Primary metric: MeanValueAvg.";
            }

            return "Use ToolType=Matching for template position or presence checks with a real template path. Primary metrics: ScoreMax and ResultCount.";
        }

        private static string ResolveIntentSummary(string template)
        {
            if (IsLineDistanceTemplate(template))
            {
                return "LineDistance / DistancePxAvg + DistancePxRange";
            }

            if (IsContourTemplate(template))
            {
                return "Contour / ResultCount, AreaAvg, bounds";
            }

            if (IsBlobTemplate(template))
            {
                return "Threshold + Blob / ResultCount, AreaAvg";
            }

            if (IsEdgeBasedTemplate(template))
            {
                return "EdgeBasedMatching / ScoreMax";
            }

            if (IsMeanTemplate(template))
            {
                return "Mean / MeanValueAvg";
            }

            return "Matching / ScoreMax";
        }

        private static bool IsLineDistanceTemplate(string template)
        {
            string value = template ?? string.Empty;
            return value.IndexOf("LineDistance", StringComparison.OrdinalIgnoreCase) >= 0
                || value.IndexOf("gap", StringComparison.OrdinalIgnoreCase) >= 0
                || value.IndexOf("distance", StringComparison.OrdinalIgnoreCase) >= 0
                || string.Equals(value, "Line Measurement", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsContourTemplate(string template)
        {
            string value = template ?? string.Empty;
            return value.IndexOf("Contour", StringComparison.OrdinalIgnoreCase) >= 0
                || value.IndexOf("boundary", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static bool IsBlobTemplate(string template)
        {
            string value = template ?? string.Empty;
            return value.IndexOf("Blob", StringComparison.OrdinalIgnoreCase) >= 0
                || value.IndexOf("count", StringComparison.OrdinalIgnoreCase) >= 0
                || value.IndexOf("area", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static bool IsEdgeBasedTemplate(string template)
        {
            string value = template ?? string.Empty;
            return value.IndexOf("Edge Based", StringComparison.OrdinalIgnoreCase) >= 0
                || value.IndexOf("EdgeBased", StringComparison.OrdinalIgnoreCase) >= 0
                || value.IndexOf("edge-shape", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static bool IsMeanTemplate(string template)
        {
            string value = template ?? string.Empty;
            return value.IndexOf("Mean", StringComparison.OrdinalIgnoreCase) >= 0
                || value.IndexOf("brightness", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private VisionPipeline CreateLlmTemplatePipeline()
        {
            string template = SelectedLlmToolTemplate ?? string.Empty;
            string pipelineName = "LLM_Starter_" + SanitizePathSegment(template.Replace("+", "And").Replace(" ", string.Empty));
            VisionPipeline pipeline = new VisionPipeline { Name = pipelineName };

            if (IsLineDistanceTemplate(template))
            {
                VisionPipelineStep step = CreateDraftStep("LineDistance_Measure", "LineDistance", "Main", "LineDistance_Result");
                step.Parameters["Name"] = "LineDistance_Measure";
                step.Parameters["PIXELPERMM"] = "1";
                step.Parameters["USE_ROI"] = "False";
                step.Parameters["CvROI"] = "0,0,0,0";
                step.Parameters["LeftPRJ_DIR"] = "X_LTOR";
                step.Parameters["RightPRJ_DIR"] = "X_RTOL";
                step.Parameters["PRJ_PORALITY"] = "WTOB";
                step.Parameters["CONTRAST"] = "18";
                step.Parameters["THICKNESS"] = "2";
                step.Parameters["SAMPLING_STEP"] = "8";
                step.Parameters["POINT_RANGE"] = "8";
                step.Parameters["VER_PRJ_DIR"] = "X_RTOL";
                step.Parameters["USE_MANUAL_ANGLE"] = "False";
                step.UseAcceptance = true;
                step.ExpectedSuccess = true;
                step.AcceptanceMetricName = "DistancePxRange";
                step.UseAcceptanceMetricMaximum = true;
                step.AcceptanceMetricMaximum = 8;
                pipeline.Steps.Add(step);
                return pipeline;
            }

            if (IsBlobTemplate(template))
            {
                VisionPipelineStep threshold = CreateDraftStep("Threshold_Precheck", "Threshold", "Main", "Threshold_Preview");
                threshold.Parameters["Threshold"] = "128";
                threshold.Parameters["MaxValue"] = "255";
                pipeline.Steps.Add(threshold);

                VisionPipelineStep blob = CreateDraftStep("Blob_Inspect", "Blob", "Threshold_Preview", "Blob_Result");
                blob.Parameters["MIN_AREA"] = "50";
                blob.Parameters["MAX_AREA"] = "999999";
                pipeline.Steps.Add(blob);
                return pipeline;
            }

            if (IsContourTemplate(template))
            {
                VisionPipelineStep step = CreateDraftStep("Contour_Inspect", "Contour", "Main", "Contour_Result");
                step.Parameters["USE_THRESHOLD"] = "True";
                step.Parameters["THRESHOLD"] = "128";
                step.Parameters["MIN_AREA"] = "50";
                step.Parameters["MAX_AREA"] = "999999";
                step.Parameters["USE_DRAW_IMAGE"] = "True";
                step.UseAcceptance = true;
                step.ExpectedSuccess = true;
                step.AcceptanceMetricName = "ResultCount";
                pipeline.Steps.Add(step);
                return pipeline;
            }

            if (IsEdgeBasedTemplate(template))
            {
                VisionPipelineStep step = CreateDraftStep("Edge_Match", "EdgeBasedMatching", "Main", "EdgeMatching_Result");
                step.Parameters["SCORE_MIN"] = "0.75";
                step.Parameters["NUM_MATCH"] = "1";
                step.Parameters["CANNY_LOW"] = "30";
                step.Parameters["CANNY_HIGH"] = "90";
                AddReferenceTemplateParameters(step);
                pipeline.Steps.Add(step);
                return pipeline;
            }

            if (IsMeanTemplate(template))
            {
                VisionPipelineStep step = CreateDraftStep("Mean_Check", "Mean", "Main", "Mean_Result");
                step.Parameters["MEAN_MIN"] = "0";
                step.Parameters["MEAN_MAX"] = "255";
                pipeline.Steps.Add(step);
                return pipeline;
            }

            VisionPipelineStep matching = CreateDraftStep("Template_Match", "Matching", "Main", "Matching_Result");
            matching.Parameters["SCORE_MIN"] = "0.6";
            matching.Parameters["NUM_MATCH"] = "1";
            matching.Parameters["MAGNIFIATION"] = "1";
            matching.Parameters["USE_FIND_ANGLE"] = "True";
            matching.Parameters["FIND_ANGLE_MIN"] = "-10";
            matching.Parameters["FIND_ANGLE_MAX"] = "10";
            AddReferenceTemplateParameters(matching);
            pipeline.Steps.Add(matching);
            return pipeline;
        }

        private void AddReferenceTemplateParameters(VisionPipelineStep step)
        {
            if (step == null || string.IsNullOrWhiteSpace(LlmReferenceImagePath))
            {
                return;
            }

            step.Parameters["TemplatePath"] = LlmReferenceImagePath.Trim();
            step.Parameters["PATTERN_PATH"] = LlmReferenceImagePath.Trim();
        }

        private static VisionPipelineStep CreateDraftStep(string name, string toolType, string inputLayer, string outputLayer)
        {
            return new VisionPipelineStep
            {
                Name = name,
                ToolType = toolType,
                InputLayer = inputLayer,
                OutputLayer = outputLayer
            };
        }

        private static string SerializePipelineToXmlText(VisionPipeline pipeline)
        {
            using (StringWriter writer = new StringWriter(CultureInfo.InvariantCulture))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(VisionPipeline));
                serializer.Serialize(writer, pipeline);
                return writer.ToString();
            }
        }

        private static string ResolveTemplateGuidance(string template)
        {
            if (IsLineDistanceTemplate(template))
            {
                return "Use LineDistance for pin-to-pin, edge-to-edge, gap, pitch, width, or clearance measurement. Keep the ROI to the measurement band. Do not judge DistancePxAvg/DistanceMmAvg alone; also constrain DistancePxRange/DistanceMmRange or DistancePxMax/DistanceMmMax to reject outlier distance lines.";
            }

            if (IsBlobTemplate(template))
            {
                return "Use Threshold to isolate the foreground, then Blob to measure area/count/position.";
            }

            if (IsContourTemplate(template))
            {
                return "Use Contour only for boundary, chip, scratch, shape, or region outline checks; do not use it for pin-to-pin gap measurement.";
            }

            if (IsEdgeBasedTemplate(template))
            {
                return "Use EdgeBasedMatching when contour shape is more reliable than raw intensity.";
            }

            if (IsMeanTemplate(template))
            {
                return "Use Mean when the judgment is based on brightness or region intensity.";
            }

            return "Use Matching when a stable template image and score threshold define the target.";
        }

        private string BuildLlmDraftReviewReport(VisionPipeline draftPipeline)
        {
            if (draftPipeline == null)
            {
                return LocalText("초안 가져오기 검토: NG - 파이프라인이 없습니다.", "Draft import review: NG - pipeline is null.");
            }

            string recipeName = NormalizeRecipeName(selectedRecipeName);
            string activePipelineName = VisionPipelineStorage.LoadActivePipelineName(
                recipeName,
                VisionPipelineAppendService.DefaultPipelineName);
            VisionPipeline activePipeline = VisionPipelineStorage.Load(recipeName, activePipelineName);
            List<string> lines = new List<string>
            {
                LocalText("초안 가져오기 검토: 준비됨", "Draft import review: READY"),
                LocalText("가져오기 동작: 새 고유 파이프라인으로 저장하고 활성화하며 Preview는 실행하지 않습니다.", "Import action: save as a new/unique pipeline, activate it, do not run Preview."),
                LocalText("현재 활성: ", "Current active: ") + FormatPipelineHeader(activePipeline),
                LocalText("초안: ", "Draft: ") + FormatPipelineHeader(draftPipeline),
                LocalText("단계 수 변화: ", "Step count delta: ") + FormatSignedNumber((draftPipeline.Steps?.Count ?? 0) - (activePipeline.Steps?.Count ?? 0)),
                LocalText("초안 의존 경로 수: ", "Draft dependency paths: ") + CountDependencyParameters(draftPipeline).ToString(CultureInfo.InvariantCulture)
            };

            int activeCount = activePipeline?.Steps?.Count ?? 0;
            int draftCount = draftPipeline?.Steps?.Count ?? 0;
            int compareCount = Math.Min(Math.Max(activeCount, draftCount), 6);
            for (int index = 0; index < compareCount; index++)
            {
                VisionPipelineStep activeStep = index < activeCount ? activePipeline.Steps[index] : null;
                VisionPipelineStep draftStep = index < draftCount ? draftPipeline.Steps[index] : null;
                lines.Add(LocalText("단계 ", "Step ") + (index + 1).ToString(CultureInfo.InvariantCulture) + ": " + FormatStepDiff(activeStep, draftStep));
            }

            if (Math.Max(activeCount, draftCount) > compareCount)
            {
                lines.Add(LocalText("검토에서 생략된 추가 단계: ", "More steps omitted from review: ")
                    + (Math.Max(activeCount, draftCount) - compareCount).ToString(CultureInfo.InvariantCulture));
            }

            return string.Join(Environment.NewLine, lines);
        }

        private string BuildLlmDraftDiffReport(VisionPipeline draftPipeline)
        {
            if (draftPipeline == null)
            {
                return LocalText("LLM XML 변경점: NG - 파이프라인이 없습니다.", "LLM XML diff review: NG - pipeline is null.");
            }

            string recipeName = NormalizeRecipeName(selectedRecipeName);
            string activePipelineName = VisionPipelineStorage.LoadActivePipelineName(
                recipeName,
                VisionPipelineAppendService.DefaultPipelineName);
            VisionPipeline activePipeline = VisionPipelineStorage.Load(recipeName, activePipelineName);
            IReadOnlyList<VisionPipelineStep> activeSteps = activePipeline?.Steps != null
                ? (IReadOnlyList<VisionPipelineStep>)activePipeline.Steps
                : Array.Empty<VisionPipelineStep>();
            IReadOnlyList<VisionPipelineStep> draftSteps = draftPipeline?.Steps != null
                ? (IReadOnlyList<VisionPipelineStep>)draftPipeline.Steps
                : Array.Empty<VisionPipelineStep>();

            List<string> added = new List<string>();
            List<string> removed = new List<string>();
            List<string> changed = new List<string>();
            int compareCount = Math.Max(activeSteps.Count, draftSteps.Count);
            for (int index = 0; index < compareCount; index++)
            {
                VisionPipelineStep activeStep = index < activeSteps.Count ? activeSteps[index] : null;
                VisionPipelineStep draftStep = index < draftSteps.Count ? draftSteps[index] : null;
                string label = (index + 1).ToString(CultureInfo.InvariantCulture);
                if (activeStep == null && draftStep != null)
                {
                    added.Add(label + ". " + FormatStepBrief(draftStep));
                    continue;
                }

                if (activeStep != null && draftStep == null)
                {
                    removed.Add(label + ". " + FormatStepBrief(activeStep));
                    continue;
                }

                string stepDiff = FormatDetailedStepDiff(activeStep, draftStep);
                if (!string.IsNullOrWhiteSpace(stepDiff))
                {
                    changed.Add(label + ". " + stepDiff);
                }
            }

            List<string> lines = new List<string>
            {
                LocalText("LLM XML 변경점: 준비됨", "LLM XML diff review: READY"),
                LocalText("비교 기준: ", "Baseline: ") + FormatPipelineHeader(activePipeline),
                LocalText("초안: ", "Draft: ") + FormatPipelineHeader(draftPipeline),
                LocalText("단계 수 변화: ", "Step count delta: ") + FormatSignedNumber(draftSteps.Count - activeSteps.Count),
                LocalText("의존 경로 수 변화: ", "Dependency path delta: ") + FormatSignedNumber(CountDependencyParameters(draftPipeline) - CountDependencyParameters(activePipeline)),
                string.Format(
                    CultureInfo.CurrentCulture,
                    LocalText("변경 요약: 추가 {0}, 삭제 {1}, 수정 {2}", "Change summary: added {0}, removed {1}, changed {2}"),
                    added.Count,
                    removed.Count,
                    changed.Count)
            };

            AddLimitedDiffLines(lines, LocalText("추가 단계", "Added steps"), added);
            AddLimitedDiffLines(lines, LocalText("삭제 예정 단계", "Removed steps"), removed);
            AddLimitedDiffLines(lines, LocalText("수정 단계", "Changed steps"), changed);
            if (added.Count == 0 && removed.Count == 0 && changed.Count == 0)
            {
                lines.Add(LocalText("구조/파라미터 변경 없음.", "No step structure or parameter changes detected."));
            }

            return string.Join(Environment.NewLine, lines);
        }

        private static void AddLimitedDiffLines(ICollection<string> lines, string title, IReadOnlyList<string> items)
        {
            if (items == null || items.Count == 0)
            {
                return;
            }

            lines.Add(title + ":");
            foreach (string item in items.Take(4))
            {
                lines.Add("  - " + item);
            }

            if (items.Count > 4)
            {
                lines.Add("  - ... +" + (items.Count - 4).ToString(CultureInfo.InvariantCulture));
            }
        }

        private static string FormatDetailedStepDiff(VisionPipelineStep activeStep, VisionPipelineStep draftStep)
        {
            if (activeStep == null || draftStep == null)
            {
                return string.Empty;
            }

            List<string> parts = new List<string>();
            string structureDiff = FormatStepDiff(activeStep, draftStep);
            if (!structureDiff.StartsWith(OpenVisionRecipeText.Local("구조 변경 없음", "No structural change"), StringComparison.Ordinal))
            {
                parts.Add(structureDiff);
            }

            string parameterDiff = FormatParameterDiff(activeStep.Parameters, draftStep.Parameters);
            if (!string.IsNullOrWhiteSpace(parameterDiff))
            {
                parts.Add(parameterDiff);
            }

            return string.Join("; ", parts);
        }

        private static string FormatParameterDiff(IDictionary<string, string> activeParameters, IDictionary<string, string> draftParameters)
        {
            IDictionary<string, string> active = activeParameters ?? new Dictionary<string, string>();
            IDictionary<string, string> draft = draftParameters ?? new Dictionary<string, string>();
            List<string> changedKeys = active.Keys
                .Concat(draft.Keys)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(item => item, StringComparer.OrdinalIgnoreCase)
                .Where(key =>
                {
                    active.TryGetValue(key, out string activeValue);
                    draft.TryGetValue(key, out string draftValue);
                    return !string.Equals(activeValue ?? string.Empty, draftValue ?? string.Empty, StringComparison.Ordinal);
                })
                .ToList();

            if (changedKeys.Count == 0)
            {
                return string.Empty;
            }

            List<string> details = changedKeys.Take(4)
                .Select(key =>
                {
                    active.TryGetValue(key, out string activeValue);
                    draft.TryGetValue(key, out string draftValue);
                    return key + " " + FormatValue(activeValue) + " -> " + FormatValue(draftValue);
                })
                .ToList();
            if (changedKeys.Count > details.Count)
            {
                details.Add("... +" + (changedKeys.Count - details.Count).ToString(CultureInfo.InvariantCulture));
            }

            return OpenVisionRecipeText.Local("파라미터 변경: ", "Parameter changes: ") + string.Join(", ", details);
        }

        private static string FormatPipelineHeader(VisionPipeline pipeline)
        {
            if (pipeline == null)
            {
                return "- / 0 step(s)";
            }

            return (string.IsNullOrWhiteSpace(pipeline.Name) ? "-" : pipeline.Name)
                + " / "
                + (pipeline.Steps?.Count ?? 0).ToString(CultureInfo.InvariantCulture)
                + " "
                + OpenVisionRecipeText.Local("단계", "step(s)");
        }

        private static string FormatSignedNumber(int value)
        {
            return value > 0
                ? "+" + value.ToString(CultureInfo.InvariantCulture)
                : value.ToString(CultureInfo.InvariantCulture);
        }

        private static string FormatStepDiff(VisionPipelineStep activeStep, VisionPipelineStep draftStep)
        {
            if (activeStep == null && draftStep == null)
            {
                return "-";
            }

            if (activeStep == null)
            {
                return OpenVisionRecipeText.Local("새 단계", "New") + " -> " + FormatStepBrief(draftStep);
            }

            if (draftStep == null)
            {
                return OpenVisionRecipeText.Local("초안에서 제거됨", "Removed from draft") + " -> " + FormatStepBrief(activeStep);
            }

            List<string> changes = new List<string>();
            if (!string.Equals(activeStep.ToolType, draftStep.ToolType, StringComparison.OrdinalIgnoreCase))
            {
                changes.Add(OpenVisionRecipeText.Local("도구 ", "tool ") + FormatValue(activeStep.ToolType) + " -> " + FormatValue(draftStep.ToolType));
            }

            string activeRoute = FormatRoute(activeStep);
            string draftRoute = FormatRoute(draftStep);
            if (!string.Equals(activeRoute, draftRoute, StringComparison.OrdinalIgnoreCase))
            {
                changes.Add(OpenVisionRecipeText.Local("경로 ", "route ") + activeRoute + " -> " + draftRoute);
            }

            if (!string.Equals(activeStep.Name, draftStep.Name, StringComparison.OrdinalIgnoreCase))
            {
                changes.Add(OpenVisionRecipeText.Local("이름 ", "name ") + FormatValue(activeStep.Name) + " -> " + FormatValue(draftStep.Name));
            }

            return changes.Count == 0
                ? OpenVisionRecipeText.Local("구조 변경 없음", "No structural change") + " -> " + FormatStepBrief(draftStep)
                : string.Join("; ", changes);
        }

        private static string FormatStepBrief(VisionPipelineStep step)
        {
            if (step == null)
            {
                return "-";
            }

            return FormatValue(step.Name) + " / " + FormatValue(step.ToolType) + " / " + FormatRoute(step);
        }

        private static string FormatRoute(VisionPipelineStep step)
        {
            if (step == null)
            {
                return "-";
            }

            return FormatValue(step.InputLayer) + " -> " + FormatValue(step.OutputLayer);
        }

        private static string FormatValue(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? "-" : value.Trim();
        }

        private static int CountDependencyParameters(VisionPipeline pipeline)
        {
            if (pipeline?.Steps == null)
            {
                return 0;
            }

            int count = 0;
            foreach (VisionPipelineStep step in pipeline.Steps)
            {
                if (step?.Parameters == null)
                {
                    continue;
                }

                foreach (KeyValuePair<string, string> parameter in step.Parameters)
                {
                    if (LooksLikeDependencyPath(parameter.Key, parameter.Value))
                    {
                        count++;
                    }
                }
            }

            return count;
        }

        private bool TryBuildLlmDraftPipeline(
            bool copyDependencies,
            out VisionPipeline pipeline,
            out string validationReport,
            out string dependencyReport)
        {
            pipeline = null;
            List<string> validationLines = new List<string>();
            validationLines.Add(OpenVisionRecipeText.Local("LLM 초안 검증: 대기", "LLM draft validation: WAIT"));

            string xmlText = LlmXmlDraftText ?? string.Empty;
            if (string.IsNullOrWhiteSpace(xmlText))
            {
                validationLines[0] = OpenVisionRecipeText.Local("LLM 초안 검증: NG", "LLM draft validation: NG");
                validationLines.Add(OpenVisionRecipeText.Local("XML 텍스트가 비어 있습니다.", "XML text is empty."));
                validationLines.Add(OpenVisionRecipeText.Local("다음: 검증 전에 VisionPipeline XML 초안을 붙여넣거나 로드하세요.", "Next: Paste or load a VisionPipeline XML draft before validation."));
                validationReport = string.Join(Environment.NewLine, validationLines);
                dependencyReport = OpenVisionRecipeText.Local("의존 파일 스캔 건너뜀.", "Dependency scan skipped.");
                LlmXmlDraftDependencyRows = Array.Empty<OpenVisionRecipeDependencyReviewRow>();
                return false;
            }

            if (!TryValidateXmlSyntax(xmlText, validationLines))
            {
                validationReport = string.Join(Environment.NewLine, validationLines);
                dependencyReport = OpenVisionRecipeText.Local("의존 파일 스캔 건너뜀.", "Dependency scan skipped.");
                LlmXmlDraftDependencyRows = Array.Empty<OpenVisionRecipeDependencyReviewRow>();
                return false;
            }

            if (!SerializeHelper.TryLoadFromXmlText(xmlText, out pipeline, out string deserializeMessage) || pipeline == null)
            {
                validationLines[0] = OpenVisionRecipeText.Local("LLM 초안 검증: NG", "LLM draft validation: NG");
                validationLines.Add(OpenVisionRecipeText.Local("OpenVision 파이프라인 역직렬화: NG - ", "OpenVision pipeline deserialize: NG - ") + deserializeMessage);
                validationLines.Add(OpenVisionRecipeText.Local("다음: OpenVisionLab VisionPipeline 스키마에 맞는 XML을 LLM에 다시 생성하게 하세요.", "Next: Ask the LLM to regenerate XML that matches the OpenVisionLab VisionPipeline schema."));
                validationReport = string.Join(Environment.NewLine, validationLines);
                dependencyReport = OpenVisionRecipeText.Local("의존 파일 스캔 건너뜀.", "Dependency scan skipped.");
                LlmXmlDraftDependencyRows = Array.Empty<OpenVisionRecipeDependencyReviewRow>();
                return false;
            }

            validationLines.Add(OpenVisionRecipeText.Local("OpenVision 파이프라인 역직렬화: OK", "OpenVision pipeline deserialize: OK"));
            validationLines.Add(OpenVisionRecipeText.Local("파이프라인: ", "Pipeline: ") + (string.IsNullOrWhiteSpace(pipeline.Name) ? "-" : pipeline.Name));
            validationLines.Add(OpenVisionRecipeText.Local("단계: ", "Steps: ") + (pipeline.Steps?.Count ?? 0).ToString(CultureInfo.InvariantCulture));
            VisionPipelineValidationResult validation = VisionPipelineValidator.Validate(pipeline, new[] { "Main" });
            validationLines.Add(string.Format(
                CultureInfo.CurrentCulture,
                OpenVisionRecipeText.Local("스키마/경로: {0} / 오류: {1} / 경고: {2}", "Schema/routing: {0} / Errors: {1} / Warnings: {2}"),
                validation.Success ? "OK" : "NG",
                validation.Errors.Count,
                validation.Warnings.Count));

            foreach (string error in validation.Errors.Take(4))
            {
                validationLines.Add(OpenVisionRecipeText.Local("오류: ", "Error: ") + error);
            }

            foreach (string warning in validation.Warnings.Take(4))
            {
                validationLines.Add(OpenVisionRecipeText.Local("경고: ", "Warning: ") + warning);
            }

            if (!validation.Success)
            {
                validationLines.Add(OpenVisionRecipeText.Local("다음: 나열된 경로/레이어/도구 오류를 수정한 뒤 가져오기 전에 다시 검증하세요.", "Next: Fix the listed route/layer/tool errors, then validate again before import."));
            }

            bool resultChannelsReady = AppendLlmResultChannelValidation(pipeline, xmlText, validationLines);
            bool intentContractReady = AppendLlmIntentContractValidation(pipeline, validationLines);

            if (!string.IsNullOrWhiteSpace(LlmReferenceImagePath))
            {
                validationLines.Add(File.Exists(LlmReferenceImagePath)
                    ? OpenVisionRecipeText.Local("참조 이미지: OK - ", "Reference image: OK - ") + LlmReferenceImagePath
                    : OpenVisionRecipeText.Local("참조 이미지: 없음 - ", "Reference image: missing - ") + LlmReferenceImagePath);
                if (!File.Exists(LlmReferenceImagePath))
                {
                    validationLines.Add(OpenVisionRecipeText.Local("다음: 존재하는 참조 이미지를 선택하거나 선택된 샘플 이미지를 사용하세요.", "Next: Choose an existing reference image or use the selected sample image."));
                }
            }

            dependencyReport = BuildDependencyReport(pipeline, NormalizeRecipeName(selectedRecipeName), copyDependencies, out int missingDependencyCount);
            if (missingDependencyCount > 0)
            {
                validationLines.Add(string.Format(
                    CultureInfo.CurrentCulture,
                    LocalText("오류: LLM XML 의존 파일 경로 {0}개가 존재하지 않습니다.", "Error: {0} LLM XML dependency file path(s) are missing."),
                    missingDependencyCount));
                validationLines.Add(LocalText(
                    "다음: 가져오기 전에 누락 파일을 첨부하거나 XML 경로를 존재하는 샘플/템플릿 경로로 바꾸세요.",
                    "Next: Attach the missing file(s) or replace XML paths with existing sample/template paths before import."));
            }

            bool success = validation.Success && missingDependencyCount == 0 && resultChannelsReady && intentContractReady;
            validationLines[0] = success
                ? OpenVisionRecipeText.Local("LLM 초안 검증: OK", "LLM draft validation: OK")
                : OpenVisionRecipeText.Local("LLM 초안 검증: NG", "LLM draft validation: NG");
            validationReport = string.Join(Environment.NewLine, validationLines);
            return success;
        }

        private static bool AppendLlmResultChannelValidation(
            VisionPipeline pipeline,
            string xmlText,
            ICollection<string> validationLines)
        {
            List<VisionPipelineStep> enabledSteps = (pipeline?.Steps ?? new List<VisionPipelineStep>())
                .Where(step => step != null && step.Enabled)
                .ToList();
            validationLines.Add(OpenVisionRecipeText.Local("판정 출력 채널: Inspection.Status / Inspection.FailedStep / Inspection.Evidence / Inspection.Benchmark / Inspection.NextAction", "Result channels: Inspection.Status / Inspection.FailedStep / Inspection.Evidence / Inspection.Benchmark / Inspection.NextAction"));

            if (enabledSteps.Count == 0)
            {
                validationLines.Add(OpenVisionRecipeText.Local("오류: Inspection.Status를 만들 수 없습니다. 사용 중인 Step이 없습니다.", "Error: Inspection.Status cannot be derived because there are no enabled steps."));
                return false;
            }

            bool hasOutputLayer = enabledSteps.Any(step => !string.IsNullOrWhiteSpace(step.OutputLayer));
            if (!hasOutputLayer)
            {
                validationLines.Add(OpenVisionRecipeText.Local("오류: Inspection.Evidence를 만들 수 없습니다. 사용 중인 Step의 OutputLayer가 없습니다.", "Error: Inspection.Evidence cannot be derived because enabled steps have no OutputLayer."));
                return false;
            }

            bool hasSeparateOutput = enabledSteps.Any(step =>
                !string.IsNullOrWhiteSpace(step.OutputLayer)
                && !string.Equals(step.InputLayer, step.OutputLayer, StringComparison.OrdinalIgnoreCase));
            if (!hasSeparateOutput)
            {
                validationLines.Add(OpenVisionRecipeText.Local("경고: 모든 출력이 입력과 같습니다. 입력 보존과 Evidence 추적을 위해 별도 OutputLayer를 권장합니다.", "Warning: all outputs match their inputs. Prefer separate OutputLayer values for input preservation and evidence tracing."));
            }

            bool hasGateParameter = enabledSteps.Any(HasJudgementParameter);
            validationLines.Add(hasGateParameter
                ? OpenVisionRecipeText.Local("Inspection.Evidence: OK - 판정 파라미터가 있습니다.", "Inspection.Evidence: OK - judgement parameters are present.")
                : OpenVisionRecipeText.Local("경고: 판정 파라미터가 명확하지 않습니다. SCORE_MIN, MIN/MAX, THRESHOLD, AREA, DISTANCE, MEAN 계열 값을 추가하세요.", "Warning: judgement parameters are not explicit. Add SCORE_MIN, MIN/MAX, THRESHOLD, AREA, DISTANCE, or MEAN style values."));

            if ((xmlText ?? string.Empty).IndexOf("Inspection.", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                validationLines.Add(OpenVisionRecipeText.Local("오류: Inspection.* 이름은 XML 노드가 아니라 OpenVisionLab 리뷰 채널입니다. 사용자 정의 XML 노드나 파라미터를 제거하세요.", "Error: Inspection.* names are review channels, not XML nodes. Remove custom XML nodes or parameters."));
                return false;
            }

            validationLines.Add(OpenVisionRecipeText.Local("Inspection.Status: OK - XML 검증과 명시적 샘플/Good-Bad 실행 결과에서 파생됩니다.", "Inspection.Status: OK - derived from XML validation and explicit sample/Good-Bad runs."));
            validationLines.Add(OpenVisionRecipeText.Local("Inspection.FailedStep: OK - Step 이름과 경로로 실패 위치를 추적할 수 있습니다.", "Inspection.FailedStep: OK - failures can be traced through step names and routes."));
            validationLines.Add(OpenVisionRecipeText.Local("Inspection.Benchmark: WAIT - 가져오기 후 카탈로그/이력 비교 실행이 필요합니다.", "Inspection.Benchmark: WAIT - run catalog/history comparison after import."));
            validationLines.Add(OpenVisionRecipeText.Local("Inspection.NextAction: OK - 검증 리포트와 작업자 리포트에 다음 조치가 표시됩니다.", "Inspection.NextAction: OK - validation and operator reports expose the next action."));
            return true;
        }

        private bool AppendLlmIntentContractValidation(VisionPipeline pipeline, ICollection<string> validationLines)
        {
            string template = SelectedLlmToolTemplate ?? string.Empty;
            if (IsLineDistanceTemplate(template))
            {
                return AppendRequiredLlmIntentToolValidation(
                    pipeline,
                    validationLines,
                    "LineDistance",
                    "Pin gap / edge distance",
                    "Use ToolType=LineDistance for pin-to-pin, edge-to-edge, gap, pitch, width, or clearance measurement. Do not substitute Contour or Blob.");
            }

            if (IsContourTemplate(template))
            {
                return AppendRequiredLlmIntentToolValidation(
                    pipeline,
                    validationLines,
                    "Contour",
                    "Shape boundary",
                    "Use ToolType=Contour for boundary, chip, scratch, shape, or region outline checks.");
            }

            if (IsBlobTemplate(template))
            {
                return AppendRequiredLlmIntentToolValidation(
                    pipeline,
                    validationLines,
                    "Blob",
                    "Threshold + Blob",
                    "Use ToolType=Blob after Threshold for connected-object count, area, position, or foreground presence checks.");
            }

            validationLines.Add("Intent contract: SKIP - selected intent has no strict tool-family gate.");
            return true;
        }

        private static bool AppendRequiredLlmIntentToolValidation(
            VisionPipeline pipeline,
            ICollection<string> validationLines,
            string requiredToolType,
            string intentName,
            string nextAction)
        {
            List<string> enabledToolTypes = (pipeline?.Steps ?? new List<VisionPipelineStep>())
                .Where(step => step != null && step.Enabled)
                .Select(step => step.ToolType ?? string.Empty)
                .Where(toolType => !string.IsNullOrWhiteSpace(toolType))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(toolType => toolType, StringComparer.OrdinalIgnoreCase)
                .ToList();

            bool hasRequiredTool = enabledToolTypes.Any(toolType =>
                string.Equals(toolType, requiredToolType, StringComparison.OrdinalIgnoreCase)
                || IsAcceptedToolAlias(requiredToolType, toolType));

            if (hasRequiredTool)
            {
                validationLines.Add("Intent contract: OK - " + intentName + " uses ToolType=" + requiredToolType + ".");
                return true;
            }

            validationLines.Add("Error: Intent contract mismatch. Selected intent '" + intentName + "' requires ToolType=" + requiredToolType + ".");
            validationLines.Add("Draft enabled ToolTypes: " + (enabledToolTypes.Count == 0 ? "-" : string.Join(", ", enabledToolTypes)));
            validationLines.Add("Next: " + nextAction);
            return false;
        }

        private static bool IsAcceptedToolAlias(string requiredToolType, string actualToolType)
        {
            if (string.Equals(requiredToolType, "LineDistance", StringComparison.OrdinalIgnoreCase))
            {
                return string.Equals(actualToolType, "LineDistanceGauge", StringComparison.OrdinalIgnoreCase);
            }

            return false;
        }

        private static bool HasJudgementParameter(VisionPipelineStep step)
        {
            if (step?.Parameters == null || step.Parameters.Count == 0)
            {
                return false;
            }

            return step.Parameters.Keys.Any(key =>
            {
                string value = key ?? string.Empty;
                return value.IndexOf("SCORE", StringComparison.OrdinalIgnoreCase) >= 0
                    || value.IndexOf("THRESH", StringComparison.OrdinalIgnoreCase) >= 0
                    || value.IndexOf("MIN", StringComparison.OrdinalIgnoreCase) >= 0
                    || value.IndexOf("MAX", StringComparison.OrdinalIgnoreCase) >= 0
                    || value.IndexOf("AREA", StringComparison.OrdinalIgnoreCase) >= 0
                    || value.IndexOf("DISTANCE", StringComparison.OrdinalIgnoreCase) >= 0
                    || value.IndexOf("MEAN", StringComparison.OrdinalIgnoreCase) >= 0
                    || value.IndexOf("RATIO", StringComparison.OrdinalIgnoreCase) >= 0
                    || value.IndexOf("CONTRAST", StringComparison.OrdinalIgnoreCase) >= 0;
            });
        }

        private static bool TryValidateXmlSyntax(string xmlText, ICollection<string> validationLines)
        {
            try
            {
                XDocument.Parse(xmlText, LoadOptions.SetLineInfo);
                validationLines.Add(OpenVisionRecipeText.Local("XML 구문: OK", "XML syntax: OK"));
                return true;
            }
            catch (XmlException ex)
            {
                validationLines.Clear();
                validationLines.Add(OpenVisionRecipeText.Local("LLM 초안 검증: NG", "LLM draft validation: NG"));
                validationLines.Add(string.Format(
                    CultureInfo.CurrentCulture,
                    OpenVisionRecipeText.Local("XML 구문: NG, 줄 {0}, 위치 {1}: {2}", "XML syntax: NG at line {0}, position {1}: {2}"),
                    ex.LineNumber,
                    ex.LinePosition,
                    ex.Message));
                validationLines.Add(OpenVisionRecipeText.Local("다음: 보고된 줄/위치의 잘못된 XML을 수정한 뒤 다시 검증하세요.", "Next: Fix malformed XML at the reported line/position, then validate again."));
                return false;
            }
        }

        private string BuildDependencyReport(VisionPipeline pipeline, string recipeName, bool copyDependencies, out int missingDependencyCount)
        {
            missingDependencyCount = 0;
            if (pipeline?.Steps == null || pipeline.Steps.Count == 0)
            {
                LlmXmlDraftDependencyRows = Array.Empty<OpenVisionRecipeDependencyReviewRow>();
                return LocalText("의존 파일 스캔 건너뜀: 파이프라인 단계가 없습니다.", "Dependency scan skipped: pipeline has no steps.");
            }

            List<OpenVisionRecipeDependencyReviewRow> rows = new List<OpenVisionRecipeDependencyReviewRow>();
            List<string> lines = new List<string>
            {
                copyDependencies
                    ? LocalText("의존 파일 복사 보고서", "Dependency copy report")
                    : LocalText("의존 파일 스캔 보고서", "Dependency scan report")
            };
            int found = 0;
            int copied = 0;
            int missing = 0;
            foreach (VisionPipelineStep step in pipeline.Steps)
            {
                if (step?.Parameters == null)
                {
                    continue;
                }

                foreach (string key in step.Parameters.Keys.ToList())
                {
                    string value = step.Parameters[key];
                    if (!LooksLikeDependencyPath(key, value))
                    {
                        continue;
                    }

                    found++;
                    string sourcePath = ResolveDependencySourcePath(value);
                    if (string.IsNullOrWhiteSpace(sourcePath) || !File.Exists(sourcePath))
                    {
                        missing++;
                        rows.Add(new OpenVisionRecipeDependencyReviewRow(
                            LocalText("누락", "Missing"),
                            step.Name,
                            key,
                            value,
                            LocalText("파일 연결 또는 XML 경로 수정", "Attach file or fix XML path")));
                        lines.Add(string.Format(
                            CultureInfo.CurrentCulture,
                            LocalText("누락: {0}.{1} -> {2}", "Missing: {0}.{1} -> {2}"),
                            step.Name,
                            key,
                            value));
                        lines.Add(LocalText(
                            "조치: XML 가져오기 전에 누락 파일을 연결하거나 존재하는 샘플/템플릿 경로로 바꾸세요.",
                            "Action: attach the missing file or replace the XML path with an existing sample/template path before import."));
                        continue;
                    }

                    if (!copyDependencies)
                    {
                        rows.Add(new OpenVisionRecipeDependencyReviewRow(
                            LocalText("확인", "Found"),
                            step.Name,
                            key,
                            sourcePath,
                            LocalText("가져오기 시 레시피로 복사", "Copy into recipe on import")));
                        lines.Add(string.Format(
                            CultureInfo.CurrentCulture,
                            LocalText("찾음: {0}.{1} -> {2}", "Found: {0}.{1} -> {2}"),
                            step.Name,
                            key,
                            sourcePath));
                        lines.Add(LocalText("가져오기 시 레시피 폴더로 복사 예정: ", "Ready to copy into the recipe on import: ") + sourcePath);
                        continue;
                    }

                    string copiedPath = CopyDependencyToRecipe(recipeName, sourcePath);
                    step.Parameters[key] = copiedPath;
                    copied++;
                    rows.Add(new OpenVisionRecipeDependencyReviewRow(
                        LocalText("복사됨", "Copied"),
                        step.Name,
                        key,
                        copiedPath,
                        LocalText("XML 경로를 복사본으로 갱신", "XML path updated to copied file")));
                    lines.Add(string.Format(
                        CultureInfo.CurrentCulture,
                        LocalText("복사됨: {0}.{1} -> {2}", "Copied: {0}.{1} -> {2}"),
                        step.Name,
                        key,
                        copiedPath));
                    lines.Add(LocalText("원본: ", "Source: ") + sourcePath);
                }
            }

            if (found == 0)
            {
                rows.Add(new OpenVisionRecipeDependencyReviewRow(
                    "None",
                    "-",
                    "-",
                    "-",
                    "No external dependency paths"));
                lines.Add(LocalText("외부 이미지/템플릿 의존 파일이 없습니다.", "No external image/template dependencies detected."));
            }

            lines.Add(string.Format(
                CultureInfo.CurrentCulture,
                LocalText("요약: 감지={0}, 복사={1}, 누락={2}", "Summary: detected={0}, copied={1}, missing={2}"),
                found,
                copied,
                missing));
            missingDependencyCount = missing;
            LlmXmlDraftDependencyRows = rows;
            return string.Join(Environment.NewLine, lines);
        }

        private void CopyReferenceImageForDraftImport(string recipeName, string pipelineName, ref string dependencyReport)
        {
            if (string.IsNullOrWhiteSpace(LlmReferenceImagePath) || !File.Exists(LlmReferenceImagePath))
            {
                return;
            }

            string imageDirectory = RecipeWorkspaceService.GetVisionPipelineImageDirectory(recipeName, pipelineName);
            string targetPath = CreateUniqueFilePath(imageDirectory, "Reference_" + Path.GetFileName(LlmReferenceImagePath));
            File.Copy(LlmReferenceImagePath, targetPath, overwrite: false);
            dependencyReport = string.IsNullOrWhiteSpace(dependencyReport)
                ? LocalText("참조 이미지 복사됨: ", "Reference image copied: ") + targetPath
                : dependencyReport + Environment.NewLine + LocalText("참조 이미지 복사됨: ", "Reference image copied: ") + targetPath;
        }

        private static bool LooksLikeDependencyPath(string key, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            string normalizedKey = key ?? string.Empty;
            bool keyLooksPath = normalizedKey.IndexOf("path", StringComparison.OrdinalIgnoreCase) >= 0
                || normalizedKey.IndexOf("template", StringComparison.OrdinalIgnoreCase) >= 0
                || normalizedKey.IndexOf("pattern", StringComparison.OrdinalIgnoreCase) >= 0;
            if (!keyLooksPath)
            {
                return false;
            }

            return IsSupportedDependencyExtension(Path.GetExtension(value.Trim()));
        }

        private static bool IsSupportedDependencyExtension(string extension)
        {
            return string.Equals(extension, ".bmp", StringComparison.OrdinalIgnoreCase)
                || string.Equals(extension, ".png", StringComparison.OrdinalIgnoreCase)
                || string.Equals(extension, ".jpg", StringComparison.OrdinalIgnoreCase)
                || string.Equals(extension, ".jpeg", StringComparison.OrdinalIgnoreCase)
                || string.Equals(extension, ".tif", StringComparison.OrdinalIgnoreCase)
                || string.Equals(extension, ".tiff", StringComparison.OrdinalIgnoreCase);
        }

        private static string ResolveDependencySourcePath(string value)
        {
            string candidate = (value ?? string.Empty).Trim().Trim('"');
            if (string.IsNullOrWhiteSpace(candidate))
            {
                return string.Empty;
            }

            if (Path.IsPathRooted(candidate))
            {
                return Path.GetFullPath(candidate);
            }

            return Path.GetFullPath(Path.Combine(AppPathService.StartupPath, candidate));
        }

        private static string CopyDependencyToRecipe(string recipeName, string sourcePath)
        {
            string templateDirectory = RecipeWorkspaceService.GetTemplateDirectory(recipeName);
            string targetPath = CreateUniqueFilePath(templateDirectory, Path.GetFileName(sourcePath));
            File.Copy(sourcePath, targetPath, overwrite: false);
            return targetPath;
        }

        private static string CreateUniqueFilePath(string directory, string fileName)
        {
            Directory.CreateDirectory(directory);
            string safeName = string.IsNullOrWhiteSpace(fileName) ? "Dependency.bin" : fileName;
            string candidate = Path.Combine(directory, safeName);
            if (!File.Exists(candidate))
            {
                return candidate;
            }

            string name = Path.GetFileNameWithoutExtension(safeName);
            string extension = Path.GetExtension(safeName);
            for (int index = 2; ; index++)
            {
                candidate = Path.Combine(directory, name + "_" + index.ToString(CultureInfo.InvariantCulture) + extension);
                if (!File.Exists(candidate))
                {
                    return candidate;
                }
            }
        }

        private void ExportActivePipelineXml()
        {
            string recipeName = NormalizeRecipeName(selectedRecipeName);
            string activePipelineName = VisionPipelineStorage.LoadActivePipelineName(
                recipeName,
                VisionPipelineAppendService.DefaultPipelineName);
            string suggestedFileName = SanitizePathSegment(activePipelineName) + ".xml";
            string path = selectExportPipelineXmlPath(suggestedFileName);
            if (string.IsNullOrWhiteSpace(path))
            {
                StatusText = LocalText("XML 내보내기가 취소되었습니다.", "Export canceled.");
                return;
            }

            ExportActivePipelineXmlToPath(path);
        }

        public bool ExportActivePipelineXmlToPath(string path)
        {
            string recipeName = NormalizeRecipeName(selectedRecipeName);
            if (!CanUseSelectedRecipe())
            {
                StatusText = LocalText("선택된 레시피가 없습니다.", "No recipe selected.");
                return false;
            }

            string activePipelineName = VisionPipelineStorage.LoadActivePipelineName(
                recipeName,
                VisionPipelineAppendService.DefaultPipelineName);
            VisionPipeline pipeline = VisionPipelineStorage.Load(recipeName, activePipelineName);
            if (!VisionPipelineStorage.TrySaveToFile(path, pipeline, out string message))
            {
                StatusText = message;
                UpdateSelectedRecipeSummary();
                return false;
            }

            StatusText = string.Format(
                CultureInfo.CurrentCulture,
                LocalText("XML 내보내기 완료: {0}", "Exported XML: {0}"),
                Path.GetFileName(path));
            UpdateSelectedRecipeSummary();
            return true;
        }

        private void ActivateSelectedPipeline()
        {
            OpenVisionRecipePipelineOption option = SelectedPipelineOption;
            if (!CanUseSelectedPipeline())
            {
                StatusText = LocalText("선택된 파이프라인이 없습니다.", "No pipeline selected.");
                return;
            }

            string recipeName = NormalizeRecipeName(selectedRecipeName);
            VisionPipelineStorage.SaveActivePipelineName(recipeName, option.PipelineName);
            StatusText = string.Format(CultureInfo.CurrentCulture, LocalText("활성 파이프라인: {0}", "Active pipeline: {0}"), option.PipelineName);
            RefreshPipelineOptions(option.PipelineName);
            refreshAfterSwitch();
        }

        private void DuplicateSelectedPipeline()
        {
            OpenVisionRecipePipelineOption option = SelectedPipelineOption;
            if (!CanUseSelectedPipeline())
            {
                StatusText = LocalText("선택된 파이프라인이 없습니다.", "No pipeline selected.");
                return;
            }

            string recipeName = NormalizeRecipeName(selectedRecipeName);
            string requestedName = NormalizePipelineName(PipelineEditName);
            string baseName = string.Equals(option.PipelineName, requestedName, StringComparison.OrdinalIgnoreCase)
                ? option.PipelineName + "_Copy"
                : requestedName;
            string targetName = CreateUniquePipelineName(recipeName, baseName);
            if (!VisionPipelineStorage.TryDuplicatePipeline(recipeName, option.PipelineName, targetName, out string message))
            {
                StatusText = message;
                RefreshPipelineOptions(option.PipelineName);
                return;
            }

            VisionPipelineStorage.SaveActivePipelineName(recipeName, targetName);
            StatusText = message;
            RefreshPipelineOptions(targetName);
            refreshAfterSwitch();
        }

        private void RenameSelectedPipeline()
        {
            OpenVisionRecipePipelineOption option = SelectedPipelineOption;
            if (!CanRenameSelectedPipeline())
            {
                StatusText = LocalText("이 파이프라인 이름은 변경할 수 없습니다.", "Cannot rename this pipeline.");
                return;
            }

            string recipeName = NormalizeRecipeName(selectedRecipeName);
            string targetName = NormalizePipelineName(PipelineEditName);
            bool wasActive = option.IsActive;
            if (!VisionPipelineStorage.TryRenamePipeline(recipeName, option.PipelineName, targetName, out string message))
            {
                StatusText = message;
                RefreshPipelineOptions(option.PipelineName);
                return;
            }

            StatusText = message;
            RefreshPipelineOptions(targetName);
            if (wasActive)
            {
                refreshAfterSwitch();
            }
        }

        private void DeleteSelectedPipeline()
        {
            OpenVisionRecipePipelineOption option = SelectedPipelineOption;
            if (!CanDeleteSelectedPipeline())
            {
                StatusText = LocalText("이 파이프라인은 삭제할 수 없습니다.", "Cannot delete this pipeline.");
                return;
            }

            string recipeName = NormalizeRecipeName(selectedRecipeName);
            if (!confirmDeletePipeline(recipeName, option.PipelineName))
            {
                StatusText = LocalText("파이프라인 삭제가 취소되었습니다.", "Pipeline delete canceled.");
                return;
            }

            bool wasActive = option.IsActive;
            if (!VisionPipelineStorage.TryDeletePipeline(recipeName, option.PipelineName, out string fallbackPipelineName, out string message))
            {
                StatusText = message;
                RefreshPipelineOptions(option.PipelineName);
                return;
            }

            StatusText = message;
            RefreshPipelineOptions(fallbackPipelineName);
            if (wasActive)
            {
                refreshAfterSwitch();
            }
        }

        private void DuplicatePipelineFromSample()
        {
            if (SelectedSampleOption == null)
            {
                StatusText = LocalText("먼저 샘플 파이프라인을 선택하세요.", "Select a sample pipeline first.");
                return;
            }

            DuplicatePipelineFromSampleOption(SelectedSampleOption);
        }

        public bool DuplicatePipelineFromSampleOption(OpenVisionRecipeSampleOption sampleOption)
        {
            string recipeName = NormalizeRecipeName(selectedRecipeName);
            if (!CanUseSelectedRecipe())
            {
                StatusText = LocalText("선택된 레시피가 없습니다.", "No recipe selected.");
                return false;
            }

            if (sampleOption == null || string.IsNullOrWhiteSpace(sampleOption.PipelinePath))
            {
                StatusText = LocalText("샘플 파이프라인을 사용할 수 없습니다.", "Sample pipeline is not available.");
                return false;
            }

            if (!VisionPipelineStorage.TryLoadFromFile(sampleOption.PipelinePath, out VisionPipeline pipeline, out string message))
            {
                StatusText = LocalText("샘플 파이프라인 로드 실패: ", "Sample pipeline load failed: ") + message;
                UpdateSelectedRecipeSummary();
                return false;
            }

            string basePipelineName = string.IsNullOrWhiteSpace(sampleOption.SampleName)
                ? pipeline.Name
                : "Sample_" + sampleOption.SampleName;
            pipeline.Name = CreateUniquePipelineName(recipeName, basePipelineName);
            VisionPipelineStorage.Save(recipeName, pipeline);
            VisionPipelineStorage.SaveActivePipelineName(recipeName, pipeline.Name);
            RefreshPipelineOptions(pipeline.Name);
            StatusText = string.Format(
                CultureInfo.CurrentCulture,
                LocalText("샘플 파이프라인 복제됨: {0}", "Duplicated sample pipeline: {0}"),
                pipeline.Name);
            RefreshOptions();
            refreshAfterSwitch();
            return true;
        }

        private bool CanUseSelectedRecipe()
        {
            string selected = NormalizeRecipeName(selectedRecipeName);
            return !string.IsNullOrWhiteSpace(selected)
                && RecipeOptions.Any(name => string.Equals(name, selected, StringComparison.OrdinalIgnoreCase));
        }

        private bool CanUseSelectedPipeline()
        {
            return CanUseSelectedRecipe()
                && SelectedPipelineOption != null
                && PipelineOptions.Any(option => string.Equals(
                    option.PipelineName,
                    SelectedPipelineOption.PipelineName,
                    StringComparison.OrdinalIgnoreCase));
        }

        private bool CanRenameSelectedPipeline()
        {
            string newName = NormalizePipelineName(PipelineEditName);
            return CanUseSelectedPipeline()
                && RecipeWorkspaceService.IsValidRecipeName(newName)
                && !string.Equals(SelectedPipelineOption.PipelineName, newName, StringComparison.OrdinalIgnoreCase)
                && !PipelineOptions.Any(option => string.Equals(option.PipelineName, newName, StringComparison.OrdinalIgnoreCase));
        }

        private bool CanDeleteSelectedPipeline()
        {
            return CanUseSelectedPipeline()
                && PipelineOptions.Count > 1;
        }

        private bool CanDuplicatePipelineFromSample()
        {
            return CanUseSelectedRecipe()
                && SelectedSampleOption != null
                && !string.IsNullOrWhiteSpace(SelectedSampleOption.PipelinePath)
                && File.Exists(SelectedSampleOption.PipelinePath);
        }

        private bool CanUseLlmXmlDraft()
        {
            return CanUseSelectedRecipe()
                && !string.IsNullOrWhiteSpace(LlmXmlDraftText);
        }

        private bool CanUseSelectedSampleReference()
        {
            return SelectedSampleOption?.Sample != null
                && !string.IsNullOrWhiteSpace(SelectedSampleOption.Sample.ImageFullPath)
                && File.Exists(SelectedSampleOption.Sample.ImageFullPath);
        }

        private bool CanRunSelectedSampleCheck()
        {
            if (isCatalogBenchmarkRunning || isSampleCheckRunning || !CanUseSelectedPipeline() || SelectedSampleOption?.Sample == null)
            {
                return false;
            }

            string pipelinePath = RecipeWorkspaceService.GetVisionPipelinePath(
                NormalizeRecipeName(selectedRecipeName),
                SelectedPipelineOption.PipelineName);
            return !string.IsNullOrWhiteSpace(SelectedSampleOption.Sample.ImageFullPath)
                && File.Exists(SelectedSampleOption.Sample.ImageFullPath)
                && !string.IsNullOrWhiteSpace(pipelinePath)
                && File.Exists(pipelinePath);
        }

        private bool CanRunSelectedSamplePairCheck()
        {
            if (isCatalogBenchmarkRunning || isPairCheckRunning || isSampleCheckRunning || !CanUseSelectedPipeline() || SelectedSampleOption?.Sample == null)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(SelectedSampleOption.Sample.PairGroup))
            {
                return false;
            }

            string pipelinePath = RecipeWorkspaceService.GetVisionPipelinePath(
                NormalizeRecipeName(selectedRecipeName),
                SelectedPipelineOption.PipelineName);
            if (string.IsNullOrWhiteSpace(pipelinePath) || !File.Exists(pipelinePath))
            {
                return false;
            }

            return VisionPipelineSampleCheckService.GetPairSamples(SelectedSampleOption.Sample).Count >= 2;
        }

        private bool CanRunCatalogBenchmark()
        {
            if (isCatalogBenchmarkRunning || isPairCheckRunning || isSampleCheckRunning || !CanUseSelectedPipeline())
            {
                return false;
            }

            string pipelinePath = RecipeWorkspaceService.GetVisionPipelinePath(
                NormalizeRecipeName(selectedRecipeName),
                SelectedPipelineOption.PipelineName);
            if (string.IsNullOrWhiteSpace(pipelinePath) || !File.Exists(pipelinePath))
            {
                return false;
            }

            return BuildCatalogBenchmarkSamples().Count > 0;
        }

        private void CreateAndSwitchRecipe(string recipeName)
        {
            RecipeWorkspaceService.EnsureVisionWorkspace(recipeName);
            VisionPipelineStorage.SaveActivePipelineName(recipeName, VisionPipelineAppendService.DefaultPipelineName);
            VisionPipelineStorage.Load(recipeName, VisionPipelineAppendService.DefaultPipelineName);

            switchRecipe(recipeName);
            StatusText = string.Format(
                CultureInfo.CurrentCulture,
                LocalText("생성됨: {0}", "Created: {0}"),
                recipeName);
            RefreshOptions();
            refreshAfterSwitch();
        }

        private void ApplyRecipeFilter()
        {
            string filter = (RecipeFilterText ?? string.Empty).Trim();
            IEnumerable<string> source = RecipeOptions;
            if (!string.IsNullOrWhiteSpace(filter))
            {
                source = source.Where(name => name.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0);
            }

            FilteredRecipeOptions = source.ToList();
        }

        private void ApplyPipelineFilter()
        {
            string filter = (PipelineFilterText ?? string.Empty).Trim();
            IEnumerable<OpenVisionRecipePipelineOption> source = PipelineOptions;
            if (!string.IsNullOrWhiteSpace(filter))
            {
                source = source.Where(option =>
                    option != null
                    && ((option.PipelineName?.IndexOf(filter, StringComparison.OrdinalIgnoreCase) ?? -1) >= 0
                        || (option.DisplayText?.IndexOf(filter, StringComparison.OrdinalIgnoreCase) ?? -1) >= 0
                        || (option.DetailText?.IndexOf(filter, StringComparison.OrdinalIgnoreCase) ?? -1) >= 0));
            }

            FilteredPipelineOptions = source.ToList();
        }

        private static List<VisionPipelineSampleCatalogItem> BuildCatalogBenchmarkSamples()
        {
            return VisionPipelineSampleCatalogItem.LoadRunnable(VisionPipelineSampleCatalogSourceKind.Product)
                .Where(sample => sample != null
                    && sample.CanOpen
                    && !string.IsNullOrWhiteSpace(sample.ImageFullPath)
                    && File.Exists(sample.ImageFullPath))
                .OrderBy(sample => string.IsNullOrWhiteSpace(sample.PairGroup) ? "~" : sample.PairGroup.Trim(), StringComparer.OrdinalIgnoreCase)
                .ThenBy(sample => sample.SampleName, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        private static string FormatCatalogBenchmarkMessage(VisionPipelineSampleCheckResult result)
        {
            if (result == null)
            {
                return string.Empty;
            }

            List<string> parts = new List<string>();
            if (!string.IsNullOrWhiteSpace(result.Message))
            {
                parts.Add(result.Message.Trim());
            }

            if (!string.IsNullOrWhiteSpace(result.MetricText))
            {
                parts.Add(result.MetricText.Trim());
            }

            return string.Join(" | ", parts);
        }

        private void RefreshSampleOptions()
        {
            string previousSampleName = SelectedSampleOption?.SampleName ?? string.Empty;
            IReadOnlyList<OpenVisionRecipeSampleOption> options = VisionPipelineSampleCatalogItem.LoadRunnable()
                .Where(sample => sample != null && sample.CanOpen)
                .OrderBy(sample => sample.CatalogSourceKind == VisionPipelineSampleCatalogSourceKind.Product ? 0 : 1)
                .ThenBy(sample => sample.SampleName, StringComparer.OrdinalIgnoreCase)
                .Select(sample => new OpenVisionRecipeSampleOption(sample))
                .ToList();

            SampleOptions = options;
            SelectedSampleOption = options.FirstOrDefault(option =>
                    string.Equals(option.SampleName, previousSampleName, StringComparison.OrdinalIgnoreCase))
                ?? options.FirstOrDefault();
        }

        private void RefreshPipelineOptions(string preferredPipelineName = null)
        {
            string recipeName = NormalizeRecipeName(selectedRecipeName);
            string activePipelineName = VisionPipelineStorage.LoadActivePipelineName(
                recipeName,
                VisionPipelineAppendService.DefaultPipelineName);
            string[] pipelineNames = RecipeWorkspaceService.GetVisionPipelineNames(recipeName);
            if (pipelineNames.Length == 0)
            {
                VisionPipelineStorage.Load(recipeName, activePipelineName);
                pipelineNames = RecipeWorkspaceService.GetVisionPipelineNames(recipeName);
            }

            IReadOnlyList<OpenVisionRecipePipelineOption> options = pipelineNames
                .Select(name => OpenVisionRecipePipelineOption.Create(recipeName, name, activePipelineName))
                .OrderBy(option => option.IsActive ? 0 : 1)
                .ThenBy(option => option.PipelineName, StringComparer.OrdinalIgnoreCase)
                .ToList();

            PipelineOptions = options;
            string selectedName = NormalizePipelineName(preferredPipelineName);
            if (string.IsNullOrWhiteSpace(preferredPipelineName)
                && selectedPipelineOption != null
                && options.Any(option => string.Equals(option.PipelineName, selectedPipelineOption.PipelineName, StringComparison.OrdinalIgnoreCase)))
            {
                selectedName = selectedPipelineOption.PipelineName;
            }
            else if (string.IsNullOrWhiteSpace(preferredPipelineName))
            {
                selectedName = activePipelineName;
            }

            OpenVisionRecipePipelineOption selectedOption = options.FirstOrDefault(option =>
                    string.Equals(option.PipelineName, selectedName, StringComparison.OrdinalIgnoreCase))
                ?? options.FirstOrDefault(option => option.IsActive)
                ?? options.FirstOrDefault();

            if (!EqualityComparer<OpenVisionRecipePipelineOption>.Default.Equals(selectedPipelineOption, selectedOption))
            {
                selectedPipelineOption = selectedOption;
                OnPropertyChanged(nameof(SelectedPipelineOption));
            }

            PipelineEditName = selectedOption?.PipelineName ?? string.Empty;
            LatestCatalogBenchmarkSummary = OpenVisionRecipeCatalogBenchmarkSummary.Empty;
            RefreshRecentBatchRunOptions();
            UpdateSelectedRecipeSummary();
            RefreshCommandState();
        }

        private void RefreshRecentBatchRunOptions()
        {
            string recipeName = NormalizeRecipeName(selectedRecipeName);
            string pipelineName = selectedPipelineOption?.PipelineName ?? string.Empty;
            string previousSummaryPath = SelectedRecentBatchRunOption?.SummaryPath ?? string.Empty;
            List<OpenVisionRecipeBatchRunOption> options = VisionPipelineBatchRunSummaryStorage
                .List(recipeName, pipelineName)
                .Take(3)
                .Select(OpenVisionRecipeBatchRunOption.Create)
                .ToList();

            if (options.Count == 0)
            {
                options.Add(OpenVisionRecipeBatchRunOption.CreateEmpty());
            }

            RecentBatchRunOptions = options;
            SelectedRecentBatchRunOption = options.FirstOrDefault(option =>
                    string.Equals(option.SummaryPath, previousSummaryPath, StringComparison.OrdinalIgnoreCase))
                ?? options.FirstOrDefault();
        }

        private void RefreshBenchmarkBaselineRunOptions()
        {
            OpenVisionRecipeBatchRunOption current = SelectedRecentBatchRunOption;
            string previousBaselinePath = selectedBenchmarkBaselineRunOption?.SummaryPath ?? string.Empty;
            List<OpenVisionRecipeBatchRunOption> options = (RecentBatchRunOptions ?? Array.Empty<OpenVisionRecipeBatchRunOption>())
                .Where(option => option != null
                    && !string.IsNullOrWhiteSpace(option.SummaryPath)
                    && !string.Equals(option.SummaryPath, current?.SummaryPath, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (options.Count == 0)
            {
                options.Add(OpenVisionRecipeBatchRunOption.CreateEmpty());
            }

            BenchmarkBaselineRunOptions = options;
            OpenVisionRecipeBatchRunOption autoBaseline = FindAutoBaselineBatchRunOption(current);
            SelectedBenchmarkBaselineRunOption = options.FirstOrDefault(option =>
                    !string.IsNullOrWhiteSpace(option.SummaryPath)
                    && string.Equals(option.SummaryPath, previousBaselinePath, StringComparison.OrdinalIgnoreCase))
                ?? options.FirstOrDefault(option =>
                    !string.IsNullOrWhiteSpace(option.SummaryPath)
                    && string.Equals(option.SummaryPath, autoBaseline?.SummaryPath, StringComparison.OrdinalIgnoreCase))
                ?? options.FirstOrDefault();
        }

        private static OpenVisionRecipeBatchSampleResultOption SelectDefaultBatchSampleResult(OpenVisionRecipeBatchRunOption option)
        {
            return option?.SampleResults?
                .FirstOrDefault(result => result != null && !result.Success && !string.IsNullOrWhiteSpace(result.FailedStep))
                ?? option?.SampleResults?.FirstOrDefault();
        }

        private static OpenVisionRecipePairSampleRunSummary SelectDefaultPairSampleResult(OpenVisionRecipePairRunSummary summary)
        {
            return summary?.SampleResults?
                .FirstOrDefault(result => result != null && !result.Success)
                ?? summary?.SampleResults?.FirstOrDefault();
        }

        private bool CanSelectPairSampleResult(OpenVisionRecipePairSampleRunSummary result)
        {
            return result != null
                && LatestPairRunSummary?.SampleResults?.Contains(result) == true;
        }

        private void SelectPairSampleResult(OpenVisionRecipePairSampleRunSummary result)
        {
            if (result == null)
            {
                return;
            }

            SelectedPairSampleResult = result;
            OpenVisionRecipePipelineStepPreview step = FindPipelinePreviewStep(result.FailedStepText);
            if (step != null)
            {
                SelectedPipelinePreviewStep = step;
                StatusText = LocalText("역할 실패 Step 선택: ", "Selected role failed step: ") + step.DisplayText;
            }
            else
            {
                StatusText = result.Success
                    ? LocalText("역할 검사 OK: ", "Role check OK: ") + result.Role
                    : LocalText("역할 실패 Step을 찾을 수 없습니다: ", "Could not find failed step for role: ") + result.Role;
            }
        }

        private OpenVisionRecipePipelineStepPreview FindPipelinePreviewStep(string failedStep)
        {
            if (string.IsNullOrWhiteSpace(failedStep))
            {
                return null;
            }

            string needle = NormalizeStepMatchText(failedStep);
            if (string.IsNullOrWhiteSpace(needle))
            {
                return null;
            }

            return SelectedRecipeSummary?.PipelinePreviewSteps?
                .FirstOrDefault(step => StepMatches(step, needle));
        }

        private static bool StepMatches(OpenVisionRecipePipelineStepPreview step, string needle)
        {
            if (step == null)
            {
                return false;
            }

            if (TryExtractStepIndex(needle, out int stepIndex)
                && step.Index == stepIndex)
            {
                return true;
            }

            string[] candidates =
            {
                step.Name,
                step.ToolType,
                step.OutputLayer,
                step.DisplayText,
                step.DetailText,
                step.FullDetailText
            };

            return candidates
                .Select(NormalizeStepMatchText)
                .Any(candidate => !string.IsNullOrWhiteSpace(candidate)
                    && (candidate.Contains(needle) || needle.Contains(candidate)));
        }

        private static bool TryExtractStepIndex(string value, out int stepIndex)
        {
            stepIndex = 0;
            string digits = new string((value ?? string.Empty)
                .SkipWhile(ch => !char.IsDigit(ch))
                .TakeWhile(char.IsDigit)
                .ToArray());
            return !string.IsNullOrWhiteSpace(digits)
                && int.TryParse(digits, NumberStyles.Integer, CultureInfo.InvariantCulture, out stepIndex);
        }

        private static string NormalizeStepMatchText(string value)
        {
            return new string((value ?? string.Empty)
                .Trim()
                .ToLowerInvariant()
                .Where(ch => !char.IsWhiteSpace(ch))
                .ToArray());
        }

        private void UpdateSelectedRecipeSummary()
        {
            string recipeName = NormalizeRecipeName(selectedRecipeName);
            string[] pipelineNames = RecipeWorkspaceService.GetVisionPipelineNames(recipeName);
            string activePipelineName = VisionPipelineStorage.LoadActivePipelineName(
                recipeName,
                VisionPipelineAppendService.DefaultPipelineName);
            string previewPipelineName = selectedPipelineOption?.PipelineName ?? activePipelineName;
            string pipelinePath = RecipeWorkspaceService.GetVisionPipelinePath(recipeName, previewPipelineName);
            DateTime? lastWriteTime = RecipeWorkspaceService.GetRecipeLastWriteTime(recipeName);
            bool xmlOk = VisionPipelineStorage.TryLoadFromFile(pipelinePath, out VisionPipeline activePipeline, out string xmlMessage);
            int stepCount = activePipeline?.Steps?.Count ?? 0;
            string llmValidationReport = BuildLlmXmlValidationReport(pipelinePath, xmlOk, activePipeline, xmlMessage);
            IReadOnlyList<OpenVisionRecipePipelineStepPreview> previewSteps = BuildPipelinePreviewSteps(activePipeline, layerCardProvider);
            string updatedText = lastWriteTime.HasValue
                ? lastWriteTime.Value.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.CurrentCulture)
                : "-";
            string detail = string.Join(
                Environment.NewLine,
                string.Format(CultureInfo.CurrentCulture, LocalText("활성 파이프라인: {0}", "Active pipeline: {0}"), activePipelineName),
                string.Format(CultureInfo.CurrentCulture, LocalText("파이프라인 수: {0}", "Pipelines: {0}"), pipelineNames.Length),
                string.Format(CultureInfo.CurrentCulture, LocalText("Step 수: {0}", "Steps: {0}"), stepCount),
                string.Format(CultureInfo.CurrentCulture, LocalText("XML: {0}", "XML: {0}"), xmlOk ? "OK" : "NG - " + xmlMessage),
                string.Format(CultureInfo.CurrentCulture, LocalText("수정: {0}", "Updated: {0}"), updatedText),
                string.Format(CultureInfo.CurrentCulture, LocalText("경로: {0}", "Path: {0}"), pipelinePath));

            detail = string.Format(CultureInfo.CurrentCulture, LocalText("선택 파이프라인: {0}", "Selected pipeline: {0}"), previewPipelineName)
                + Environment.NewLine
                + detail;

            SelectedRecipeSummary = new OpenVisionRecipeManagerSummary(
                recipeName,
                activePipelineName,
                previewPipelineName,
                pipelineNames.Length,
                stepCount,
                xmlOk,
                detail,
                llmValidationReport,
                previewSteps);
        }

        private static string BuildLlmXmlValidationReport(
            string pipelinePath,
            bool xmlOk,
            VisionPipeline activePipeline,
            string xmlMessage)
        {
            List<string> lines = new List<string>
            {
                OpenVisionRecipeText.Local("LLM XML 검증: ", "LLM XML validation: ") + (xmlOk ? "OK" : "NG"),
                OpenVisionRecipeText.Local("XML 로드: ", "XML load: ") + (xmlOk ? "OK" : xmlMessage),
                OpenVisionRecipeText.Local("가정한 소스 레이어: Main", "Assumed source layer: Main")
            };

            if (!xmlOk || activePipeline == null)
            {
                lines.Add(OpenVisionRecipeText.Local("조치: LLM에 지원되는 ToolType, InputLayer, OutputLayer, Parameters를 포함한 OpenVisionLab VisionPipeline XML을 다시 출력하게 하세요.", "Action: ask the LLM to output OpenVisionLab VisionPipeline XML with supported ToolType, InputLayer, OutputLayer, and Parameters."));
                return string.Join(Environment.NewLine, lines);
            }

            string filePipelineName = Path.GetFileNameWithoutExtension(pipelinePath) ?? string.Empty;
            lines.Add(string.Format(
                CultureInfo.CurrentCulture,
                OpenVisionRecipeText.Local("파이프라인: {0} / 단계: {1}", "Pipeline: {0} / Steps: {1}"),
                string.IsNullOrWhiteSpace(activePipeline.Name) ? "-" : activePipeline.Name,
                activePipeline.Steps.Count));

            if (!string.Equals(filePipelineName, activePipeline.Name ?? string.Empty, StringComparison.OrdinalIgnoreCase))
            {
                lines.Add(string.Format(
                    CultureInfo.CurrentCulture,
                    OpenVisionRecipeText.Local("경고: XML 이름 '{0}'이 파일 '{1}'과 다릅니다.", "Warning: XML Name '{0}' differs from file '{1}'."),
                    activePipeline.Name ?? string.Empty,
                    filePipelineName));
            }

            VisionPipelineValidationResult validation = VisionPipelineValidator.Validate(activePipeline, new[] { "Main" });
            lines.Add(string.Format(
                CultureInfo.CurrentCulture,
                OpenVisionRecipeText.Local("스키마/경로: {0} / 오류: {1} / 경고: {2}", "Schema/routing: {0} / Errors: {1} / Warnings: {2}"),
                validation.Success ? "OK" : "NG",
                validation.Errors.Count,
                validation.Warnings.Count));

            foreach (string error in validation.Errors.Take(4))
            {
                lines.Add(OpenVisionRecipeText.Local("오류: ", "Error: ") + error);
            }

            if (validation.Errors.Count > 4)
            {
                lines.Add(OpenVisionRecipeText.Local("오류: +", "Error: +") + (validation.Errors.Count - 4).ToString(CultureInfo.InvariantCulture) + OpenVisionRecipeText.Local("개 더 있음", " more"));
            }

            foreach (string warning in validation.Warnings.Take(4))
            {
                lines.Add(OpenVisionRecipeText.Local("경고: ", "Warning: ") + warning);
            }

            if (validation.Warnings.Count > 4)
            {
                lines.Add(OpenVisionRecipeText.Local("경고: +", "Warning: +") + (validation.Warnings.Count - 4).ToString(CultureInfo.InvariantCulture) + OpenVisionRecipeText.Local("개 더 있음", " more"));
            }

            return string.Join(Environment.NewLine, lines);
        }

        private static IReadOnlyList<OpenVisionRecipePipelineStepPreview> BuildPipelinePreviewSteps(
            VisionPipeline pipeline,
            Func<string, OpenVisionRecipeLayerCard> layerCardProvider)
        {
            if (pipeline?.Steps == null || pipeline.Steps.Count == 0)
            {
                return Array.Empty<OpenVisionRecipePipelineStepPreview>();
            }

            List<OpenVisionRecipePipelineStepPreview> steps = new List<OpenVisionRecipePipelineStepPreview>();
            for (int i = 0; i < pipeline.Steps.Count; i++)
            {
                VisionPipelineStep step = pipeline.Steps[i];
                steps.Add(new OpenVisionRecipePipelineStepPreview(i + 1, step, layerCardProvider));
            }

            return steps;
        }

        private void NavigateSelectedStepInputLayer()
        {
            NavigateStepLayer(SelectedPipelinePreviewStep?.InputLayerCard);
        }

        private bool CanNavigateSelectedStepInputLayer()
        {
            return CanNavigateStepLayer(SelectedPipelinePreviewStep?.InputLayerCard);
        }

        private void NavigateSelectedStepOutputLayer()
        {
            NavigateStepLayer(SelectedPipelinePreviewStep?.OutputLayerCard);
        }

        private bool CanNavigateSelectedStepOutputLayer()
        {
            return CanNavigateStepLayer(SelectedPipelinePreviewStep?.OutputLayerCard);
        }

        private void FocusSelectedRunFailureStep()
        {
            OpenVisionRecipePipelineStepPreview step = ResolveSelectedRunFailureStep();
            if (step == null)
            {
                StatusText = LocalText("연결된 실패 Step이 없습니다.", "No linked failed step.");
                return;
            }

            SelectedPipelinePreviewStep = step;
            StatusText = LocalText("실패 Step 선택: ", "Failed step selected: ") + step.DisplayText;
        }

        private bool CanFocusSelectedRunFailureStep()
        {
            return ResolveSelectedRunFailureStep() != null;
        }

        private void LoadSelectedRunSampleImageToInputLayer()
        {
            OpenVisionRecipePipelineStepPreview step = ResolveSelectedRunFailureStep();
            string sampleImagePath = ResolveSelectedRunSampleImagePath();
            if (step == null)
            {
                StatusText = LocalText("연결된 실패 Step이 없습니다.", "No linked failed step.");
                return;
            }

            if (string.IsNullOrWhiteSpace(sampleImagePath) || !File.Exists(sampleImagePath))
            {
                StatusText = LocalText("샘플 이미지 경로를 찾을 수 없습니다.", "Could not find the sample image path.");
                return;
            }

            if (loadImageIntoLayer(step.InputLayer, sampleImagePath))
            {
                SelectedPipelinePreviewStep = step;
                StatusText = LocalText("샘플 이미지를 입력 레이어에 로드: ", "Sample image loaded to input layer: ") + step.InputLayer;
            }
            else
            {
                StatusText = LocalText("입력 레이어에 샘플 이미지를 로드하지 못했습니다: ", "Could not load sample image to input layer: ") + step.InputLayer;
            }

            CommandManager.InvalidateRequerySuggested();
        }

        private bool CanLoadSelectedRunSampleImageToInputLayer()
        {
            OpenVisionRecipePipelineStepPreview step = ResolveSelectedRunFailureStep();
            return step != null
                && !string.IsNullOrWhiteSpace(step.InputLayer)
                && File.Exists(ResolveSelectedRunSampleImagePath());
        }

        private OpenVisionRecipePipelineStepPreview ResolveSelectedRunFailureStep()
        {
            string failedStep = SelectedRecentBatchRunComparisonRow?.FailedStep;
            if (string.IsNullOrWhiteSpace(failedStep))
            {
                failedStep = SelectedRecentBatchSampleResultOption?.FailedStep;
            }

            return FindPipelinePreviewStep(failedStep);
        }

        private string ResolveSelectedRunSampleImagePath()
        {
            string path = SelectedRecentBatchRunComparisonRow?.SampleImagePath;
            if (File.Exists(path))
            {
                return path;
            }

            path = SelectedRecentBatchSampleResultOption?.ReportPath;
            if (File.Exists(path))
            {
                return path;
            }

            string sampleName = SelectedRecentBatchSampleResultOption?.SampleName;
            if (string.IsNullOrWhiteSpace(sampleName))
            {
                sampleName = SelectedRecentBatchRunComparisonRow?.SampleName;
            }

            if (string.IsNullOrWhiteSpace(sampleName))
            {
                return string.Empty;
            }

            VisionPipelineSampleCatalogItem sample = sampleOptions?
                .Select(option => option?.Sample)
                .FirstOrDefault(item => item != null
                    && string.Equals(item.SampleName, sampleName, StringComparison.OrdinalIgnoreCase));
            if (sample == null)
            {
                sample = VisionPipelineSampleCatalogItem.LoadRunnable()
                    .FirstOrDefault(item => item != null
                        && string.Equals(item.SampleName, sampleName, StringComparison.OrdinalIgnoreCase));
            }

            return File.Exists(sample?.ImageFullPath) ? sample.ImageFullPath : string.Empty;
        }

        private void SelectPreviousPipelinePreviewStep()
        {
            SelectPipelinePreviewStepByOffset(-1);
        }

        private bool CanSelectPreviousPipelinePreviewStep()
        {
            return GetPipelinePreviewStepByOffset(-1) != null;
        }

        private void SelectNextPipelinePreviewStep()
        {
            SelectPipelinePreviewStepByOffset(1);
        }

        private bool CanSelectNextPipelinePreviewStep()
        {
            return GetPipelinePreviewStepByOffset(1) != null;
        }

        private void SelectPipelinePreviewStepByOffset(int offset)
        {
            OpenVisionRecipePipelineStepPreview target = GetPipelinePreviewStepByOffset(offset);
            if (target == null)
            {
                return;
            }

            SelectedPipelinePreviewStep = target;
            StatusText = LocalText("선택 Step 이동: ", "Selected step: ")
                + target.Index.ToString(CultureInfo.InvariantCulture)
                + "/"
                + (SelectedRecipeSummary?.PipelinePreviewSteps?.Count ?? 0).ToString(CultureInfo.InvariantCulture)
                + " "
                + target.Name;
        }

        private OpenVisionRecipePipelineStepPreview GetPipelinePreviewStepByOffset(int offset)
        {
            IReadOnlyList<OpenVisionRecipePipelineStepPreview> steps = SelectedRecipeSummary?.PipelinePreviewSteps;
            OpenVisionRecipePipelineStepPreview selected = SelectedPipelinePreviewStep;
            if (steps == null || steps.Count == 0 || selected == null)
            {
                return null;
            }

            int selectedPosition = -1;
            for (int i = 0; i < steps.Count; i++)
            {
                OpenVisionRecipePipelineStepPreview candidate = steps[i];
                if (ReferenceEquals(candidate, selected) || candidate.Index == selected.Index)
                {
                    selectedPosition = i;
                    break;
                }
            }

            if (selectedPosition < 0)
            {
                return null;
            }

            int targetPosition = selectedPosition + offset;
            return targetPosition >= 0 && targetPosition < steps.Count
                ? steps[targetPosition]
                : null;
        }

        private string BuildPipelineStepFlowReviewText()
        {
            IReadOnlyList<OpenVisionRecipePipelineStepPreview> steps = SelectedRecipeSummary?.PipelinePreviewSteps;
            OpenVisionRecipePipelineStepPreview selected = SelectedPipelinePreviewStep;
            if (steps == null || steps.Count == 0)
            {
                return LocalText("선택한 파이프라인에 검토할 Step이 없습니다.", "The selected pipeline has no steps to review.");
            }

            if (selected == null)
            {
                return LocalText("Step을 선택하면 입력/출력 흐름과 앞뒤 Step을 여기에서 확인할 수 있습니다.", "Select a step to review its input/output flow and neighboring steps here.");
            }

            string position = selected.Index.ToString(CultureInfo.InvariantCulture)
                + "/"
                + steps.Count.ToString(CultureInfo.InvariantCulture);
            string flow = selected.InputLayer + " -> " + selected.OutputLayer;
            OpenVisionRecipePipelineStepPreview previous = GetPipelinePreviewStepByOffset(-1);
            OpenVisionRecipePipelineStepPreview next = GetPipelinePreviewStepByOffset(1);
            return LocalText("현재 Step ", "Current step ")
                + position
                + " | "
                + selected.ToolType
                + " | "
                + flow
                + " | "
                + LocalText("이전: ", "Previous: ")
                + (previous == null ? "-" : previous.OutputLayer)
                + " | "
                + LocalText("다음: ", "Next: ")
                + (next == null ? "-" : next.InputLayer);
        }

        private string BuildBranchOutputComparisonText()
        {
            IReadOnlyList<OpenVisionRecipePipelineStepPreview> steps = SelectedRecipeSummary?.PipelinePreviewSteps
                ?? Array.Empty<OpenVisionRecipePipelineStepPreview>();
            OpenVisionRecipePipelineStepPreview selected = SelectedPipelinePreviewStep;
            if (selected == null || steps.Count == 0)
            {
                return LocalText(
                    "Step을 선택하면 같은 입력의 출력 후보와 downstream 소비 Step을 비교합니다.",
                    "Select a step to compare same-input output candidates and downstream consumers.");
            }

            int sameInputBranches = steps.Count(step =>
                step != null
                && step.Index != selected.Index
                && string.Equals(step.InputLayer, selected.InputLayer, StringComparison.OrdinalIgnoreCase));
            int outputConsumers = steps.Count(step =>
                step != null
                && step.Index != selected.Index
                && string.Equals(step.InputLayer, selected.OutputLayer, StringComparison.OrdinalIgnoreCase));
            int upstreamProducers = steps.Count(step =>
                step != null
                && step.Index != selected.Index
                && string.Equals(step.OutputLayer, selected.InputLayer, StringComparison.OrdinalIgnoreCase));

            return string.Format(
                CultureInfo.CurrentCulture,
                LocalText(
                    "분기/출력 비교: 같은 입력 후보 {0}, 출력 소비 Step {1}, 입력 생성 Step {2}",
                    "Branch/output comparison: same-input candidates {0}, output consumers {1}, input producers {2}"),
                sameInputBranches,
                outputConsumers,
                upstreamProducers);
        }

        private IReadOnlyList<OpenVisionRecipeBranchOutputComparisonRow> BuildBranchOutputComparisonRows()
        {
            IReadOnlyList<OpenVisionRecipePipelineStepPreview> steps = SelectedRecipeSummary?.PipelinePreviewSteps
                ?? Array.Empty<OpenVisionRecipePipelineStepPreview>();
            OpenVisionRecipePipelineStepPreview selected = SelectedPipelinePreviewStep;
            if (selected == null || steps.Count == 0)
            {
                return new[]
                {
                    new OpenVisionRecipeBranchOutputComparisonRow(
                        LocalText("대기", "Waiting"),
                        "-",
                        "-",
                        LocalText("Step 선택 필요", "Select a step"))
                };
            }

            List<OpenVisionRecipeBranchOutputComparisonRow> rows = new List<OpenVisionRecipeBranchOutputComparisonRow>
            {
                CreateBranchOutputRow(
                    LocalText("선택", "Selected"),
                    selected,
                    LocalText("수정 대상 출력", "Correction target output"))
            };

            foreach (OpenVisionRecipePipelineStepPreview producer in steps
                .Where(step => step != null
                    && step.Index != selected.Index
                    && string.Equals(step.OutputLayer, selected.InputLayer, StringComparison.OrdinalIgnoreCase))
                .OrderBy(step => step.Index))
            {
                rows.Add(CreateBranchOutputRow(
                    LocalText("입력 생성", "Input producer"),
                    producer,
                    LocalText("선택 Step 입력을 만듦", "Feeds selected input")));
            }

            foreach (OpenVisionRecipePipelineStepPreview branch in steps
                .Where(step => step != null
                    && step.Index != selected.Index
                    && string.Equals(step.InputLayer, selected.InputLayer, StringComparison.OrdinalIgnoreCase))
                .OrderBy(step => step.Index))
            {
                rows.Add(CreateBranchOutputRow(
                    LocalText("같은 입력", "Same input"),
                    branch,
                    LocalText("대체 출력 후보", "Alternative output candidate")));
            }

            foreach (OpenVisionRecipePipelineStepPreview consumer in steps
                .Where(step => step != null
                    && step.Index != selected.Index
                    && string.Equals(step.InputLayer, selected.OutputLayer, StringComparison.OrdinalIgnoreCase))
                .OrderBy(step => step.Index))
            {
                rows.Add(CreateBranchOutputRow(
                    LocalText("출력 소비", "Output consumer"),
                    consumer,
                    LocalText("선택 출력 이후 영향", "Affected after selected output")));
            }

            if (rows.Count == 1)
            {
                rows.Add(new OpenVisionRecipeBranchOutputComparisonRow(
                    LocalText("단일 경로", "Single path"),
                    "-",
                    selected.OutputLayer,
                    LocalText("분기/소비 Step 없음", "No branch or consumer step")));
            }

            return rows;
        }

        private static OpenVisionRecipeBranchOutputComparisonRow CreateBranchOutputRow(
            string status,
            OpenVisionRecipePipelineStepPreview step,
            string action)
        {
            return new OpenVisionRecipeBranchOutputComparisonRow(
                status,
                step == null ? "-" : step.Index.ToString(CultureInfo.InvariantCulture) + ". " + step.Name,
                step == null ? "-" : step.InputLayer + " -> " + step.OutputLayer,
                action);
        }

        private static string BuildPipelineStepSlotText(OpenVisionRecipePipelineStepPreview step, string emptyText)
        {
            if (step == null)
            {
                return emptyText ?? string.Empty;
            }

            return step.Index.ToString(CultureInfo.InvariantCulture)
                + ". "
                + step.Name
                + " / "
                + step.ToolType
                + " | "
                + step.InputLayer
                + " -> "
                + step.OutputLayer;
        }

        private void RefreshSelectedPipelineStepFlow()
        {
            OnPropertyChanged(nameof(PipelineStepFlowReviewText));
            OnPropertyChanged(nameof(BranchOutputComparisonText));
            OnPropertyChanged(nameof(BranchOutputComparisonRows));
            OnPropertyChanged(nameof(PreviousPipelineStepText));
            OnPropertyChanged(nameof(CurrentPipelineStepText));
            OnPropertyChanged(nameof(NextPipelineStepText));
            CommandManager.InvalidateRequerySuggested();
        }

        private void OpenSelectedStepTool()
        {
            if (!TryResolveSelectedStepMenu(out VISION_MENU menu))
            {
                StatusText = OpenVisionRecipeText.Local("선택 Step에 연결할 도구가 없습니다.", "No tool is available for the selected step.");
                return;
            }

            if (!LoadSelectedStepParametersForEdit(updateStatus: true))
            {
                return;
            }

            SeedNativeToolSession(SelectedStepEditObject);
            selectStepTool?.Invoke(menu);
            StatusText = OpenVisionRecipeText.Local("Step 파라미터를 도구에 불러왔습니다: ", "Loaded step parameters into tool: ") + SelectedPipelinePreviewStep.ToolType;
        }

        private bool CanOpenSelectedStepTool()
        {
            return selectStepTool != null && TryResolveSelectedStepMenu(out _);
        }

        private void LoadSelectedStepParameters()
        {
            LoadSelectedStepParametersForEdit(updateStatus: true);
        }

        private bool CanLoadSelectedStepParameters()
        {
            return SelectedPipelinePreviewStep != null;
        }

        private void ApplySelectedStepParameters()
        {
            if (SelectedStepEditObject == null && !LoadSelectedStepParametersForEdit(updateStatus: true))
            {
                return;
            }

            if (!commitSelectedStepEdit())
            {
                SetSelectedStepEditStatus(OpenVisionRecipeText.Local("보류 중인 PropertyGrid 편집을 확정하지 못했습니다.", "Could not commit the pending PropertyGrid edit."));
                return;
            }

            if (!TryLoadSelectedPipelineStep(out string recipeName, out string pipelineName, out VisionPipeline pipeline, out VisionPipelineStep step, out string message))
            {
                SetSelectedStepEditStatus(message);
                return;
            }

            int selectedIndex = SelectedPipelinePreviewStep?.Index ?? 0;
            if (!VisionPipelineStepPropertyMapper.ApplyProperty(step, SelectedStepEditObject))
            {
                SetSelectedStepEditStatus(OpenVisionRecipeText.Local("이 Step 파라미터는 XML로 반영할 수 없습니다.", "This step property set cannot be applied to XML."));
                return;
            }

            try
            {
                pipeline.Name = pipelineName;
                VisionPipelineStorage.Save(recipeName, pipeline);
            }
            catch (Exception ex)
            {
                SetSelectedStepEditStatus(OpenVisionRecipeText.Local("XML 저장 실패: ", "XML save failed: ") + ex.GetBaseException().Message);
                return;
            }

            string validationMessage;
            bool roundTripOk = VisionPipelineStorage.TryValidateRoundTrip(recipeName, pipeline, out validationMessage);
            UpdateSelectedRecipeSummary();
            SelectedPipelinePreviewStep = SelectedRecipeSummary?.PipelinePreviewSteps?
                .FirstOrDefault(stepPreview => stepPreview.Index == selectedIndex);
            selectedStepEditDirty = false;
            OnPropertyChanged(nameof(IsSelectedStepEditDirty));
            LoadSelectedStepParametersForEdit(updateStatus: false);
            SetSelectedStepEditStatus(
                OpenVisionRecipeText.Local("XML 반영 완료: ", "Applied to XML: ")
                + pipelineName
                + " / Step "
                + selectedIndex.ToString(CultureInfo.InvariantCulture)
                + " / "
                + validationMessage);
            SetCorrectedOutputReview(BuildCorrectedOutputAppliedText(pipelineName, selectedIndex, validationMessage));
            StatusText = roundTripOk
                ? OpenVisionRecipeText.Local("Step XML 반영 완료", "Step XML apply complete")
                : OpenVisionRecipeText.Local("Step XML 반영 완료, 검증 경고 확인 필요", "Step XML applied; review validation warning");
        }

        private bool CanApplySelectedStepParameters()
        {
            return SelectedStepEditObject != null;
        }

        public void MarkSelectedStepEditDirty()
        {
            if (SelectedStepEditObject == null)
            {
                return;
            }

            selectedStepEditDirty = true;
            OnPropertyChanged(nameof(IsSelectedStepEditDirty));
            SetSelectedStepEditStatus(OpenVisionRecipeText.Local("편집됨: XML 반영 전입니다.", "Edited: not yet applied to XML."));
            SetCorrectedOutputReview(string.Empty);
        }

        private bool LoadSelectedStepParametersForEdit(bool updateStatus)
        {
            if (!TryLoadSelectedPipelineStep(out _, out string pipelineName, out _, out VisionPipelineStep step, out string message))
            {
                ClearSelectedStepEdit();
                if (updateStatus)
                {
                    SetSelectedStepEditStatus(message);
                }

                return false;
            }

            object property = VisionPipelineStepPropertyMapper.CreateProperty(step);
            if (property == null)
            {
                ClearSelectedStepEdit();
                if (updateStatus)
                {
                    SetSelectedStepEditStatus(OpenVisionRecipeText.Local("지원하지 않는 Step 도구입니다: ", "Unsupported step tool: ") + step.ToolType);
                }

                return false;
            }

            selectedStepEditDirty = false;
            OnPropertyChanged(nameof(IsSelectedStepEditDirty));
            SelectedStepEditObject = property;
            if (updateStatus)
            {
                SetSelectedStepEditStatus(
                    OpenVisionRecipeText.Local("불러옴: ", "Loaded: ")
                    + pipelineName
                    + " / Step "
                    + (SelectedPipelinePreviewStep?.Index ?? 0).ToString(CultureInfo.InvariantCulture));
            }

            return true;
        }

        private bool TryLoadSelectedPipelineStep(
            out string recipeName,
            out string pipelineName,
            out VisionPipeline pipeline,
            out VisionPipelineStep step,
            out string message)
        {
            recipeName = NormalizeRecipeName(selectedRecipeName);
            pipelineName = selectedPipelineOption?.PipelineName
                ?? VisionPipelineStorage.LoadActivePipelineName(recipeName, VisionPipelineAppendService.DefaultPipelineName);
            pipeline = null;
            step = null;
            message = string.Empty;

            OpenVisionRecipePipelineStepPreview preview = SelectedPipelinePreviewStep;
            if (preview == null)
            {
                message = OpenVisionRecipeText.Local("선택된 Step이 없습니다.", "No step is selected.");
                return false;
            }

            string path = RecipeWorkspaceService.GetVisionPipelinePath(recipeName, pipelineName);
            if (!VisionPipelineStorage.TryLoadFromFile(path, out pipeline, out message))
            {
                return false;
            }

            int index = preview.Index - 1;
            if (pipeline?.Steps != null && index >= 0 && index < pipeline.Steps.Count)
            {
                step = pipeline.Steps[index];
                return true;
            }

            step = pipeline?.Steps?.FirstOrDefault(candidate =>
                string.Equals(candidate.Name, preview.Name, StringComparison.OrdinalIgnoreCase)
                && string.Equals(candidate.ToolType, preview.ToolType, StringComparison.OrdinalIgnoreCase)
                && string.Equals(candidate.OutputLayer, preview.OutputLayer, StringComparison.OrdinalIgnoreCase));
            if (step != null)
            {
                return true;
            }

            message = OpenVisionRecipeText.Local("선택 Step을 XML에서 다시 찾지 못했습니다.", "Could not find the selected step in XML.");
            return false;
        }

        private static void SeedNativeToolSession(object property)
        {
            switch (property)
            {
                case BlobProperty blob:
                    OpenVisionNativeToolPropertySessionStore.SetRepositoryProperty("Blob_1", repository => repository.Blobs, blob);
                    break;
                case ContourProperty contour:
                    OpenVisionNativeToolPropertySessionStore.SetRepositoryProperty("Contour_1", repository => repository.Contours, contour);
                    break;
                case MatchingProperty matching:
                    OpenVisionNativeToolPropertySessionStore.SetRepositoryProperty("Matching_1", repository => repository.Matchings, matching);
                    break;
                case EdgeBasedMatchingProperty edgeBasedMatching:
                    OpenVisionNativeToolPropertySessionStore.SetRepositoryProperty("EdgeBasedMatching_1", repository => repository.EdgeBasedMatchings, edgeBasedMatching);
                    break;
                case FeatureMatchingProperty featureMatching:
                    OpenVisionNativeToolPropertySessionStore.SetRepositoryProperty("Feature_1", repository => repository.Features, featureMatching);
                    break;
                case LineGaugeProperty line:
                    OpenVisionNativeToolPropertySessionStore.SetRepositoryProperty("Line(L)_1", repository => repository.Lines_L, line);
                    break;
                default:
                    if (VisionPipelineStepPropertyMapper.TryCreateLineGaugePair(property, out LineGaugeProperty left, out LineGaugeProperty right))
                    {
                        OpenVisionNativeToolPropertySessionStore.SetRepositoryProperty("Line(L)_1", repository => repository.Lines_L, left);
                        OpenVisionNativeToolPropertySessionStore.SetRepositoryProperty("Line(R)_1", repository => repository.Lines_R, right);
                    }

                    break;
            }
        }

        private void SetSelectedStepEditStatus(string value)
        {
            selectedStepEditStatusText = value ?? string.Empty;
            OnPropertyChanged(nameof(SelectedStepEditStatusText));
            RefreshCommandState();
        }

        private void SetCorrectedOutputReview(string value)
        {
            correctedOutputReviewText = value ?? string.Empty;
            OnPropertyChanged(nameof(CorrectedOutputReviewText));
        }

        private void ClearSelectedStepEdit()
        {
            selectedStepEditObject = null;
            selectedStepEditDirty = false;
            selectedStepEditStatusText = string.Empty;
            correctedOutputReviewText = string.Empty;
            OnPropertyChanged(nameof(SelectedStepEditObject));
            OnPropertyChanged(nameof(HasSelectedStepEditObject));
            OnPropertyChanged(nameof(IsSelectedStepEditDirty));
            OnPropertyChanged(nameof(SelectedStepEditStatusText));
            OnPropertyChanged(nameof(CorrectedOutputReviewText));
            RefreshCommandState();
        }

        private bool TryResolveSelectedStepMenu(out VISION_MENU menu)
        {
            return TryResolveStepToolMenu(SelectedPipelinePreviewStep?.ToolType, out menu);
        }

        private static bool TryResolveStepToolMenu(string toolType, out VISION_MENU menu)
        {
            menu = VISION_MENU.Pipeline;
            string normalized = (toolType ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(normalized))
            {
                return false;
            }

            switch (normalized.ToLowerInvariant())
            {
                case "threshold":
                    menu = VISION_MENU.Threshold;
                    return true;
                case "filter":
                    menu = VISION_MENU.Filter;
                    return true;
                case "morphology":
                    menu = VISION_MENU.Morphology;
                    return true;
                case "arithmetic":
                    menu = VISION_MENU.Arithmetic;
                    return true;
                case "blob":
                    menu = VISION_MENU.Blob;
                    return true;
                case "contour":
                    menu = VISION_MENU.Contour;
                    return true;
                case "line":
                case "linegauge":
                case "linedistance":
                case "lineintersection":
                    menu = VISION_MENU.Line;
                    return true;
                case "matching":
                case "templatematching":
                    menu = VISION_MENU.Matching;
                    return true;
                case "edgebasedmatching":
                case "edgebased":
                case "edge":
                    menu = VISION_MENU.EdgeBasedMatching;
                    return true;
                case "featurematching":
                case "feature":
                    menu = VISION_MENU.FeatureMatching;
                    return true;
                case "mean":
                    menu = VISION_MENU.Mean;
                    return true;
                default:
                    return false;
            }
        }

        private void NavigateStepLayer(OpenVisionRecipeLayerCard card)
        {
            if (!CanNavigateStepLayer(card))
            {
                return;
            }

            bool moved = navigateLayer(card.LayerName);
            StatusText = moved
                ? OpenVisionRecipeText.Local("레이어 이동: ", "Layer selected: ") + card.LayerName
                : OpenVisionRecipeText.Local("레이어 없음: ", "Layer unavailable: ") + card.LayerName;
        }

        private static bool CanNavigateStepLayer(OpenVisionRecipeLayerCard card)
        {
            return card != null
                && !string.IsNullOrWhiteSpace(card.LayerName)
                && !string.Equals(card.LayerName, "-", StringComparison.Ordinal);
        }

        private string CreateUniqueRecipeName(string requestedBaseName = null)
        {
            string baseName = string.IsNullOrWhiteSpace(requestedBaseName)
                ? "Recipe_" + DateTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture)
                : requestedBaseName.Trim();
            string candidate = baseName;
            int index = 2;
            HashSet<string> existing = RecipeWorkspaceService.GetRecipeNames().ToHashSet(StringComparer.OrdinalIgnoreCase);
            while (existing.Contains(candidate))
            {
                candidate = baseName + "_" + index.ToString(CultureInfo.InvariantCulture);
                index++;
            }

            return candidate;
        }

        private static string CreateUniquePipelineName(string recipeName, string requestedBaseName)
        {
            string baseName = SanitizePathSegment(string.IsNullOrWhiteSpace(requestedBaseName)
                ? VisionPipelineAppendService.DefaultPipelineName
                : requestedBaseName.Trim());
            string candidate = baseName;
            int index = 2;
            HashSet<string> existing = RecipeWorkspaceService.GetVisionPipelineNames(recipeName).ToHashSet(StringComparer.OrdinalIgnoreCase);
            while (existing.Contains(candidate))
            {
                candidate = baseName + "_" + index.ToString(CultureInfo.InvariantCulture);
                index++;
            }

            return candidate;
        }

        private static string SanitizePathSegment(string value)
        {
            char[] invalidChars = Path.GetInvalidFileNameChars();
            string sanitized = new string((value ?? string.Empty)
                .Select(ch => invalidChars.Contains(ch) ? '_' : ch)
                .ToArray());
            return string.IsNullOrWhiteSpace(sanitized) ? "Item" : sanitized;
        }

        private void SetSelectedRecipeName(string recipeName)
        {
            string normalized = NormalizeRecipeName(recipeName);
            bool changed = !string.Equals(selectedRecipeName, normalized, StringComparison.Ordinal);
            selectedRecipeName = normalized;
            if (changed)
            {
                OnPropertyChanged(nameof(SelectedRecipeName));
            }

            if (!string.Equals(editRecipeName, normalized, StringComparison.Ordinal))
            {
                editRecipeName = normalized;
                OnPropertyChanged(nameof(EditRecipeName));
            }

            string preferredPipelineName = changed
                ? VisionPipelineStorage.LoadActivePipelineName(normalized, VisionPipelineAppendService.DefaultPipelineName)
                : selectedPipelineOption?.PipelineName;
            RefreshPipelineOptions(preferredPipelineName);
        }

        private void RefreshCommandState()
        {
            OnPropertyChanged(nameof(RecipeEditValidationText));
            OnPropertyChanged(nameof(PipelineEditValidationText));
            OnPropertyChanged(nameof(RecipeGuidedNextActionText));
            CommandManager.InvalidateRequerySuggested();
        }

        private static string NormalizeRecipeName(string recipeName)
        {
            return string.IsNullOrWhiteSpace(recipeName) ? "Default" : recipeName.Trim();
        }

        private static string NormalizePipelineName(string pipelineName)
        {
            return SanitizePathSegment(string.IsNullOrWhiteSpace(pipelineName) ? "Pipeline" : pipelineName.Trim());
        }

        private static string LocalText(string korean, string english)
        {
            return OpenVisionRecipeText.Local(korean, english);
        }
    }

    internal static class OpenVisionRecipeText
    {
        public static string Local(string korean, string english)
        {
            return OpenVisionLanguageService.CurrentLanguage == OpenVisionLanguage.Korean ? korean : english;
        }
    }

    public sealed class OpenVisionRecipeDependencyReviewRow
    {
        public OpenVisionRecipeDependencyReviewRow(
            string status,
            string stepName,
            string parameterName,
            string path,
            string action)
        {
            Status = string.IsNullOrWhiteSpace(status) ? "-" : status;
            StepName = string.IsNullOrWhiteSpace(stepName) ? "-" : stepName;
            ParameterName = string.IsNullOrWhiteSpace(parameterName) ? "-" : parameterName;
            Path = string.IsNullOrWhiteSpace(path) ? "-" : path;
            Action = string.IsNullOrWhiteSpace(action) ? "-" : action;
        }

        public string Status { get; }

        public string StepName { get; }

        public string ParameterName { get; }

        public string Path { get; }

        public string Action { get; }
    }

    public sealed class OpenVisionRecipeBranchOutputComparisonRow
    {
        public OpenVisionRecipeBranchOutputComparisonRow(
            string status,
            string stepName,
            string route,
            string action)
        {
            Status = string.IsNullOrWhiteSpace(status) ? "-" : status;
            StepName = string.IsNullOrWhiteSpace(stepName) ? "-" : stepName;
            Route = string.IsNullOrWhiteSpace(route) ? "-" : route;
            Action = string.IsNullOrWhiteSpace(action) ? "-" : action;
        }

        public string Status { get; }

        public string StepName { get; }

        public string Route { get; }

        public string Action { get; }
    }

    public sealed class OpenVisionRecipeManagerSummary
    {
        public static OpenVisionRecipeManagerSummary Empty { get; } = new OpenVisionRecipeManagerSummary(
            string.Empty,
            string.Empty,
            string.Empty,
            0,
            0,
            false,
            string.Empty,
            string.Empty,
            Array.Empty<OpenVisionRecipePipelineStepPreview>());

        public OpenVisionRecipeManagerSummary(
            string recipeName,
            string activePipelineName,
            string previewPipelineName,
            int pipelineCount,
            int stepCount,
            bool xmlValid,
            string detailText,
            string llmXmlValidationReport,
            IReadOnlyList<OpenVisionRecipePipelineStepPreview> pipelinePreviewSteps)
        {
            RecipeName = recipeName ?? string.Empty;
            ActivePipelineName = activePipelineName ?? string.Empty;
            PreviewPipelineName = previewPipelineName ?? string.Empty;
            PipelineCount = pipelineCount;
            StepCount = stepCount;
            XmlValid = xmlValid;
            DetailText = detailText ?? string.Empty;
            LlmXmlValidationReport = llmXmlValidationReport ?? string.Empty;
            LlmXmlValidationIssues = OpenVisionRecipeValidationIssue.CreateRows(LlmXmlValidationReport, XmlValid);
            PipelinePreviewSteps = pipelinePreviewSteps ?? Array.Empty<OpenVisionRecipePipelineStepPreview>();
        }

        public string RecipeName { get; }

        public string ActivePipelineName { get; }

        public string PreviewPipelineName { get; }

        public int PipelineCount { get; }

        public int StepCount { get; }

        public bool XmlValid { get; }

        public string DetailText { get; }

        public string LlmXmlValidationReport { get; }

        public IReadOnlyList<OpenVisionRecipeValidationIssue> LlmXmlValidationIssues { get; }

        public IReadOnlyList<OpenVisionRecipePipelineStepPreview> PipelinePreviewSteps { get; }

        public string HeaderText =>
            OpenVisionRecipeText.Local("선택 파이프라인: ", "Selected pipeline: ")
            + (string.IsNullOrWhiteSpace(PreviewPipelineName) ? "-" : PreviewPipelineName);

        public string ActivePipelineDisplay =>
            OpenVisionRecipeText.Local("활성: ", "Active: ")
            + (string.IsNullOrWhiteSpace(ActivePipelineName) ? "-" : ActivePipelineName);

        public string PipelineCountDisplay =>
            OpenVisionRecipeText.Local("파이프라인 ", "Pipelines ")
            + PipelineCount.ToString(CultureInfo.InvariantCulture);

        public string StepCountDisplay =>
            OpenVisionRecipeText.Local("단계 ", "Steps ")
            + StepCount.ToString(CultureInfo.InvariantCulture);

        public string XmlStatusDisplay => XmlValid ? "XML OK" : "XML NG";

        public string PipelinePreviewStepListDisplay =>
            OpenVisionRecipeText.Local("파이프라인 미리보기 단계 목록 (", "Pipeline preview step list (")
            + PipelinePreviewSteps.Count.ToString(CultureInfo.InvariantCulture)
            + ")";

        public string OperatorReviewText
        {
            get
            {
                if (!XmlValid)
                {
                    return OpenVisionRecipeText.Local("실행 전 XML 검토가 필요합니다.", "XML needs review before run.");
                }

                if (StepCount <= 0)
                {
                    return OpenVisionRecipeText.Local("검토 전에 파이프라인 단계를 추가하세요.", "Add a pipeline step before review.");
                }

                if (!string.Equals(ActivePipelineName, PreviewPipelineName, StringComparison.OrdinalIgnoreCase))
                {
                    return OpenVisionRecipeText.Local("이 파이프라인을 활성화하거나 활성 파이프라인을 선택하세요.", "Activate this pipeline or choose the active pipeline.");
                }

                return OpenVisionRecipeText.Local("검토 준비됨: 샘플 이미지로 실행한 뒤 출력 레이어를 확인하세요.", "Review ready: run with a sample image, then check output layers.");
            }
        }

        public string OperatorReviewChecklistText
        {
            get
            {
                if (!XmlValid)
                {
                    return string.Join(
                        Environment.NewLine,
                        OpenVisionRecipeText.Local("1. LLM XML 검증 보고서의 오류를 확인하세요.", "1. Review errors in the LLM XML validation report."),
                        OpenVisionRecipeText.Local("2. XML 경로, 레이어, ToolType, Parameters를 수정하세요.", "2. Fix XML paths, layers, ToolType, and Parameters."),
                        OpenVisionRecipeText.Local("3. XML OK 후 샘플 검사로 넘어가세요.", "3. Continue to sample check after XML OK."));
                }

                if (StepCount <= 0)
                {
                    return string.Join(
                        Environment.NewLine,
                        OpenVisionRecipeText.Local("1. 파이프라인 단계를 추가하거나 샘플에서 복제하세요.", "1. Add pipeline steps or duplicate from a sample."),
                        OpenVisionRecipeText.Local("2. 입력/출력 레이어 경로를 확인하세요.", "2. Check input/output layer routes."),
                        OpenVisionRecipeText.Local("3. 샘플 검사로 출력 레이어를 확인하세요.", "3. Run sample check to verify output layers."));
                }

                if (!string.Equals(ActivePipelineName, PreviewPipelineName, StringComparison.OrdinalIgnoreCase))
                {
                    return string.Join(
                        Environment.NewLine,
                        OpenVisionRecipeText.Local("1. 선택 파이프라인을 활성화하거나 활성 파이프라인을 선택하세요.", "1. Activate the selected pipeline or choose the active pipeline."),
                        OpenVisionRecipeText.Local("2. 활성 파이프라인 기준으로 샘플 검사를 실행하세요.", "2. Run sample check against the active pipeline."),
                        OpenVisionRecipeText.Local("3. 결과 레이어와 판정 기준을 비교하세요.", "3. Compare result layers and acceptance gates."));
                }

                return string.Join(
                    Environment.NewLine,
                    OpenVisionRecipeText.Local("1. 검사 실행으로 선택 샘플의 출력 레이어를 확인하세요.", "1. Run check to inspect selected sample output layers."),
                    OpenVisionRecipeText.Local("2. 쌍 검사로 Good/Bad 분리와 지표 기준을 확인하세요.", "2. Run pair check to verify Good/Bad separation and metric gates."),
                    OpenVisionRecipeText.Local("3. 실패 시 단계 목록에서 해당 출력/판정 기준을 조정하세요.", "3. On failure, tune the matching output or acceptance gate in the step list."));
            }
        }
    }

    public sealed class OpenVisionRecipeValidationIssue
    {
        private OpenVisionRecipeValidationIssue(
            string severity,
            string location,
            string explanation,
            string action)
        {
            Severity = severity ?? string.Empty;
            Location = location ?? string.Empty;
            Explanation = explanation ?? string.Empty;
            Action = action ?? string.Empty;
        }

        public string Severity { get; }

        public string Location { get; }

        public string Explanation { get; }

        public string Action { get; }

        internal static IReadOnlyList<OpenVisionRecipeValidationIssue> CreateRows(string report, bool xmlValid)
        {
            List<OpenVisionRecipeValidationIssue> rows = new List<OpenVisionRecipeValidationIssue>();
            foreach (string line in SplitLines(report))
            {
                string trimmed = line.Trim();
                if (TryStripPrefix(trimmed, "오류: ", "Error: ", out string error))
                {
                    rows.Add(new OpenVisionRecipeValidationIssue(
                        OpenVisionRecipeText.Local("오류", "Error"),
                        OpenVisionRecipeText.Local("XML/경로", "XML/route"),
                        error,
                        OpenVisionRecipeText.Local("ToolType, Layer, Parameter를 수정한 뒤 다시 검증하세요.", "Fix ToolType, Layer, and Parameters, then validate again.")));
                    continue;
                }

                if (TryStripPrefix(trimmed, "경고: ", "Warning: ", out string warning))
                {
                    rows.Add(new OpenVisionRecipeValidationIssue(
                        OpenVisionRecipeText.Local("경고", "Warning"),
                        OpenVisionRecipeText.Local("XML/레시피", "XML/recipe"),
                        warning,
                        OpenVisionRecipeText.Local("이름, 분기, 의존 경로가 의도와 맞는지 확인하세요.", "Check that names, branches, and dependency paths match the intent.")));
                    continue;
                }

                if (trimmed.IndexOf("XML load:", StringComparison.OrdinalIgnoreCase) >= 0
                    && trimmed.IndexOf("OK", StringComparison.OrdinalIgnoreCase) < 0)
                {
                    rows.Add(new OpenVisionRecipeValidationIssue(
                        OpenVisionRecipeText.Local("오류", "Error"),
                        OpenVisionRecipeText.Local("XML 로드", "XML load"),
                        trimmed,
                        OpenVisionRecipeText.Local("VisionPipeline XML 형식으로 다시 생성하거나 가져오세요.", "Regenerate or import a valid VisionPipeline XML.")));
                }
            }

            if (rows.Count == 0)
            {
                rows.Add(new OpenVisionRecipeValidationIssue(
                    xmlValid ? "OK" : OpenVisionRecipeText.Local("검토", "Review"),
                    OpenVisionRecipeText.Local("검증 요약", "Validation summary"),
                    xmlValid
                        ? OpenVisionRecipeText.Local("LLM XML 구조와 경로 검증이 통과했습니다.", "LLM XML structure and route validation passed.")
                        : OpenVisionRecipeText.Local("검증 리포트를 확인해야 합니다.", "Review the validation report."),
                    xmlValid
                        ? OpenVisionRecipeText.Local("샘플 검사 또는 Pipeline Review로 진행하세요.", "Continue to sample check or Pipeline Review.")
                        : OpenVisionRecipeText.Local("오류/경고 라인을 확인한 뒤 다시 검증하세요.", "Review error/warning lines, then validate again.")));
            }

            return rows.Take(6).ToArray();
        }

        private static IEnumerable<string> SplitLines(string report)
        {
            return (report ?? string.Empty)
                .Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
        }

        private static bool TryStripPrefix(string value, string koreanPrefix, string englishPrefix, out string stripped)
        {
            stripped = string.Empty;
            if (value.StartsWith(koreanPrefix, StringComparison.Ordinal))
            {
                stripped = value.Substring(koreanPrefix.Length).Trim();
                return true;
            }

            if (value.StartsWith(englishPrefix, StringComparison.OrdinalIgnoreCase))
            {
                stripped = value.Substring(englishPrefix.Length).Trim();
                return true;
            }

            return false;
        }
    }

    public sealed class OpenVisionRecipePipelineOption
    {
        private OpenVisionRecipePipelineOption(
            string pipelineName,
            bool isActive,
            int stepCount,
            bool xmlValid,
            bool routeValid,
            string statusText)
        {
            PipelineName = pipelineName ?? string.Empty;
            IsActive = isActive;
            StepCount = stepCount;
            XmlValid = xmlValid;
            RouteValid = routeValid;
            StatusText = statusText ?? string.Empty;
        }

        public string PipelineName { get; }

        public bool IsActive { get; }

        public int StepCount { get; }

        public bool XmlValid { get; }

        public bool RouteValid { get; }

        public string StatusText { get; }

        public string DisplayText =>
            (IsActive ? OpenVisionRecipeText.Local("[활성] ", "[ACTIVE] ") : string.Empty)
            + PipelineName;

        public string DetailText =>
            StepCount.ToString(CultureInfo.InvariantCulture)
            + OpenVisionRecipeText.Local(" 단계 | ", " step(s) | ")
            + StatusText;

        internal static OpenVisionRecipePipelineOption Create(
            string recipeName,
            string pipelineName,
            string activePipelineName)
        {
            string name = string.IsNullOrWhiteSpace(pipelineName) ? "Pipeline" : pipelineName.Trim();
            string path = RecipeWorkspaceService.GetVisionPipelinePath(recipeName, name);
            bool isActive = string.Equals(name, activePipelineName, StringComparison.OrdinalIgnoreCase);
            if (!VisionPipelineStorage.TryLoadFromFile(path, out VisionPipeline pipeline, out string message))
            {
                return new OpenVisionRecipePipelineOption(name, isActive, 0, false, false, "XML NG - " + message);
            }

            VisionPipelineValidationResult validation = VisionPipelineValidator.Validate(pipeline, new[] { "Main" });
            string status = validation.Success
                ? OpenVisionRecipeText.Local("XML OK / 경로 OK", "XML OK / Route OK")
                : OpenVisionRecipeText.Local("XML OK / 경로 NG ", "XML OK / Route NG ") + validation.Errors.Count.ToString(CultureInfo.InvariantCulture);
            return new OpenVisionRecipePipelineOption(
                name,
                isActive,
                pipeline?.Steps?.Count ?? 0,
                true,
                validation.Success,
                status);
        }
    }

    public sealed class OpenVisionRecipeLayerCard
    {
        public OpenVisionRecipeLayerCard(string layerName, string statusText, BitmapImage thumbnail, bool canNavigate)
        {
            LayerName = string.IsNullOrWhiteSpace(layerName) ? "-" : layerName.Trim();
            StatusText = string.IsNullOrWhiteSpace(statusText) ? "-" : statusText.Trim();
            Thumbnail = thumbnail;
            CanNavigate = canNavigate;
        }

        public string LayerName { get; }

        public string StatusText { get; }

        public BitmapImage Thumbnail { get; }

        public bool CanNavigate { get; }

        public bool HasThumbnail => Thumbnail != null;

        public static OpenVisionRecipeLayerCard CreateMissing(string layerName)
        {
            return new OpenVisionRecipeLayerCard(
                layerName,
                OpenVisionRecipeText.Local("레이어 없음", "Layer missing"),
                null,
                false);
        }
    }

    public sealed class OpenVisionRecipePipelineStepPreview
    {
        internal OpenVisionRecipePipelineStepPreview(
            int index,
            VisionPipelineStep step,
            Func<string, OpenVisionRecipeLayerCard> layerCardProvider)
        {
            Index = index;
            Name = string.IsNullOrWhiteSpace(step?.Name) ? "Step " + index.ToString(CultureInfo.InvariantCulture) : step.Name.Trim();
            ToolType = string.IsNullOrWhiteSpace(step?.ToolType) ? "-" : step.ToolType.Trim();
            InputLayer = string.IsNullOrWhiteSpace(step?.InputLayer) ? "-" : step.InputLayer.Trim();
            OutputLayer = string.IsNullOrWhiteSpace(step?.OutputLayer) ? "-" : step.OutputLayer.Trim();
            InputLayerCard = CreateLayerCard(layerCardProvider, InputLayer);
            OutputLayerCard = CreateLayerCard(layerCardProvider, OutputLayer);
            ParameterCount = step?.Parameters?.Count ?? 0;
            IsEnabled = step?.Enabled ?? false;
            AcceptanceText = ResolveAcceptanceText(step);
            RouteText = OpenVisionRecipeText.Local("레이어: ", "Layers: ") + Shorten(InputLayer, 34) + " -> " + Shorten(OutputLayer, 34);
            LayerRouteText = InputLayer + " -> " + OutputLayer;
            TableRouteText = Shorten(InputLayer, 8) + " -> " + Shorten(OutputLayer, 12);
            AcceptanceDetailText = ResolveAcceptanceDetailText(step);
            TableAcceptanceText = ResolveTableAcceptanceText(step);
            ParameterPreviewText = BuildParameterPreviewText(step);
            FullParameterText = BuildFullParameterText(step);
            RoiMetadataText = BuildRoiMetadataText(step);
            TemplateMetadataText = BuildTemplateMetadataText(step);
            EditorActionText = BuildEditorActionText(ToolType);
        }

        public int Index { get; }

        public string Name { get; }

        public string ToolType { get; }

        public string InputLayer { get; }

        public string OutputLayer { get; }

        public OpenVisionRecipeLayerCard InputLayerCard { get; }

        public OpenVisionRecipeLayerCard OutputLayerCard { get; }

        public int ParameterCount { get; }

        public bool IsEnabled { get; }

        public string AcceptanceText { get; }

        public string RouteText { get; }

        public string LayerRouteText { get; }

        public string TableRouteText { get; }

        public string AcceptanceDetailText { get; }

        public string TableAcceptanceText { get; }

        public string ParameterPreviewText { get; }

        public string FullParameterText { get; }

        public string RoiMetadataText { get; }

        public string TemplateMetadataText { get; }

        public string EditorActionText { get; }

        public string DisplayText =>
            Index.ToString(CultureInfo.InvariantCulture) + ". "
            + (IsEnabled ? OpenVisionRecipeText.Local("[사용] ", "[ON] ") : OpenVisionRecipeText.Local("[중지] ", "[OFF] "))
            + Shorten(Name, 42)
            + " / "
            + ToolType;

        public string DetailText =>
            Shorten(InputLayer, 32)
            + " -> "
            + Shorten(OutputLayer, 32)
            + OpenVisionRecipeText.Local(" | 파라미터 ", " | Params ")
            + ParameterCount.ToString(CultureInfo.InvariantCulture)
            + AcceptanceText;

        public string FullDetailText =>
            InputLayer
            + " -> "
            + OutputLayer
            + OpenVisionRecipeText.Local(" | 파라미터 ", " | Params ")
            + ParameterCount.ToString(CultureInfo.InvariantCulture)
            + AcceptanceText;

        private static OpenVisionRecipeLayerCard CreateLayerCard(
            Func<string, OpenVisionRecipeLayerCard> layerCardProvider,
            string layerName)
        {
            if (layerCardProvider == null)
            {
                return OpenVisionRecipeLayerCard.CreateMissing(layerName);
            }

            return layerCardProvider(layerName) ?? OpenVisionRecipeLayerCard.CreateMissing(layerName);
        }

        private static string Shorten(string value, int maxLength)
        {
            if (string.IsNullOrWhiteSpace(value) || value.Length <= maxLength)
            {
                return string.IsNullOrWhiteSpace(value) ? "-" : value;
            }

            return value.Substring(0, Math.Max(1, maxLength - 3)) + "...";
        }

        private static string ResolveAcceptanceText(VisionPipelineStep step)
        {
            if (step == null || !step.UseAcceptance)
            {
                return string.Empty;
            }

            string metric = string.IsNullOrWhiteSpace(step.AcceptanceMetricName) ? "result" : step.AcceptanceMetricName.Trim();
            List<string> gates = new List<string>();
            if (step.UseAcceptanceMetricMinimum)
            {
                gates.Add(">=" + step.AcceptanceMetricMinimum.ToString("0.###", CultureInfo.InvariantCulture));
            }

            if (step.UseAcceptanceMetricMaximum)
            {
                gates.Add("<=" + step.AcceptanceMetricMaximum.ToString("0.###", CultureInfo.InvariantCulture));
            }

            return gates.Count == 0
                ? OpenVisionRecipeText.Local(" | 판정 ", " | Accept ") + metric
                : OpenVisionRecipeText.Local(" | 판정 ", " | Accept ") + metric + " " + string.Join(" ", gates);
        }

        private static string ResolveTableAcceptanceText(VisionPipelineStep step)
        {
            if (step == null || !step.UseAcceptance)
            {
                return "-";
            }

            string metric = string.IsNullOrWhiteSpace(step.AcceptanceMetricName) ? "result" : step.AcceptanceMetricName.Trim();
            List<string> gates = new List<string>();
            if (step.UseAcceptanceMetricMinimum)
            {
                gates.Add(">=" + step.AcceptanceMetricMinimum.ToString("0.###", CultureInfo.InvariantCulture));
            }

            if (step.UseAcceptanceMetricMaximum)
            {
                gates.Add("<=" + step.AcceptanceMetricMaximum.ToString("0.###", CultureInfo.InvariantCulture));
            }

            return gates.Count == 0
                ? Shorten(metric, 12)
                : Shorten(metric, 12) + Environment.NewLine + string.Join(" ", gates);
        }

        private static string ResolveAcceptanceDetailText(VisionPipelineStep step)
        {
            if (step == null || !step.UseAcceptance)
            {
                return OpenVisionRecipeText.Local("판정 기준 없음", "No acceptance gate");
            }

            string metric = string.IsNullOrWhiteSpace(step.AcceptanceMetricName) ? "result" : step.AcceptanceMetricName.Trim();
            List<string> gates = new List<string>();
            if (step.UseAcceptanceMetricMinimum)
            {
                gates.Add(">=" + step.AcceptanceMetricMinimum.ToString("0.###", CultureInfo.InvariantCulture));
            }

            if (step.UseAcceptanceMetricMaximum)
            {
                gates.Add("<=" + step.AcceptanceMetricMaximum.ToString("0.###", CultureInfo.InvariantCulture));
            }

            return gates.Count == 0
                ? metric
                : metric + Environment.NewLine + string.Join(" ", gates);
        }

        private static string BuildParameterPreviewText(VisionPipelineStep step)
        {
            if (step?.Parameters == null || step.Parameters.Count == 0)
            {
                return OpenVisionRecipeText.Local("파라미터: 없음", "Params: none");
            }

            List<string> pairs = step.Parameters
                .OrderBy(parameter => parameter.Key, StringComparer.OrdinalIgnoreCase)
                .Take(4)
                .Select(parameter => parameter.Key + "=" + Shorten(parameter.Value, 18))
                .ToList();
            int remaining = Math.Max(0, step.Parameters.Count - pairs.Count);
            string suffix = remaining > 0 ? " +" + remaining.ToString(CultureInfo.InvariantCulture) : string.Empty;
            return OpenVisionRecipeText.Local("파라미터: ", "Params: ") + string.Join(", ", pairs) + suffix;
        }

        private static string BuildFullParameterText(VisionPipelineStep step)
        {
            if (step?.Parameters == null || step.Parameters.Count == 0)
            {
                return OpenVisionRecipeText.Local("파라미터 없음", "No parameters");
            }

            return string.Join(
                Environment.NewLine,
                step.Parameters
                    .OrderBy(parameter => parameter.Key, StringComparer.OrdinalIgnoreCase)
                    .Select(parameter => parameter.Key + " = " + (parameter.Value ?? string.Empty)));
        }

        private static string BuildRoiMetadataText(VisionPipelineStep step)
        {
            IDictionary<string, string> parameters = step?.Parameters;
            if (parameters == null || parameters.Count == 0)
            {
                return OpenVisionRecipeText.Local("ROI: 파라미터 없음", "ROI: no parameters");
            }

            bool useRoi = GetBoolParameter(parameters, "USE_ROI");
            bool useMultiRoi = GetBoolParameter(parameters, "USE_MULTI_ROI");
            string roi = GetParameter(parameters, useMultiRoi ? "CvROIS" : "CvROI");
            if (string.IsNullOrWhiteSpace(roi))
            {
                roi = string.Join(
                    " | ",
                    parameters
                        .Where(parameter => parameter.Key?.IndexOf("ROI", StringComparison.OrdinalIgnoreCase) >= 0)
                        .Where(parameter => !string.IsNullOrWhiteSpace(parameter.Key)
                            && !parameter.Key.StartsWith("USE_", StringComparison.OrdinalIgnoreCase))
                        .OrderBy(parameter => parameter.Key, StringComparer.OrdinalIgnoreCase)
                        .Take(3)
                        .Select(parameter => parameter.Key + "=" + parameter.Value));
            }

            if (string.IsNullOrWhiteSpace(roi))
            {
                return useRoi
                    ? OpenVisionRecipeText.Local("ROI: 켜짐, 영역 값 없음", "ROI: enabled, no region value")
                    : OpenVisionRecipeText.Local("ROI: 전체 이미지", "ROI: full image");
            }

            string prefix = useMultiRoi
                ? OpenVisionRecipeText.Local("ROI: 다중 ", "ROI: multi ")
                : OpenVisionRecipeText.Local("ROI: ", "ROI: ");
            return prefix + Shorten(roi, 72);
        }

        private static string BuildTemplateMetadataText(VisionPipelineStep step)
        {
            IDictionary<string, string> parameters = step?.Parameters;
            if (parameters == null || parameters.Count == 0)
            {
                return OpenVisionRecipeText.Local("Template: 파라미터 없음", "Template: no parameters");
            }

            string templatePath = GetFirstParameter(parameters, "TemplatePath", "PATTERN_PATH", "PatternPath");
            if (string.IsNullOrWhiteSpace(templatePath))
            {
                return OpenVisionRecipeText.Local("Template: 없음", "Template: none");
            }

            List<string> parts = new List<string>
            {
                "Template: " + Shorten(Path.GetFileName(templatePath.Trim()), 42)
            };
            string score = GetParameter(parameters, "SCORE_MIN");
            if (!string.IsNullOrWhiteSpace(score))
            {
                parts.Add("score >= " + score.Trim());
            }

            string count = GetParameter(parameters, "NUM_MATCH");
            if (!string.IsNullOrWhiteSpace(count))
            {
                parts.Add("count " + count.Trim());
            }

            return string.Join(" | ", parts);
        }

        private static string BuildEditorActionText(string toolType)
        {
            string name = string.IsNullOrWhiteSpace(toolType) ? "Tool" : toolType.Trim();
            return OpenVisionRecipeText.Local("도구 열기: ", "Open tool: ") + name;
        }

        private static bool GetBoolParameter(IDictionary<string, string> parameters, string key)
        {
            string value = GetParameter(parameters, key);
            return bool.TryParse(value, out bool parsed) && parsed;
        }

        private static string GetFirstParameter(IDictionary<string, string> parameters, params string[] keys)
        {
            foreach (string key in keys ?? Array.Empty<string>())
            {
                string value = GetParameter(parameters, key);
                if (!string.IsNullOrWhiteSpace(value))
                {
                    return value;
                }
            }

            return string.Empty;
        }

        private static string GetParameter(IDictionary<string, string> parameters, string key)
        {
            if (parameters == null || string.IsNullOrWhiteSpace(key))
            {
                return string.Empty;
            }

            return parameters.TryGetValue(key, out string value) ? value ?? string.Empty : string.Empty;
        }
    }

    public sealed class OpenVisionRecipeOperatorValidationRow
    {
        private OpenVisionRecipeOperatorValidationRow(
            string itemText,
            string stateText,
            string evidenceText,
            string nextActionText)
        {
            ItemText = string.IsNullOrWhiteSpace(itemText) ? "-" : itemText.Trim();
            StateText = string.IsNullOrWhiteSpace(stateText) ? "WAIT" : stateText.Trim().ToUpperInvariant();
            EvidenceText = string.IsNullOrWhiteSpace(evidenceText) ? "-" : evidenceText.Trim();
            NextActionText = string.IsNullOrWhiteSpace(nextActionText) ? "-" : nextActionText.Trim();
        }

        public string ItemText { get; }

        public string StateText { get; }

        public string EvidenceText { get; }

        public string NextActionText { get; }

        public bool IsOk => string.Equals(StateText, "OK", StringComparison.OrdinalIgnoreCase);

        public bool IsNg => string.Equals(StateText, "NG", StringComparison.OrdinalIgnoreCase);

        public bool IsWait => !IsOk && !IsNg;

        public string DisplayText => ItemText + " | " + StateText + " | " + EvidenceText;

        public static OpenVisionRecipeOperatorValidationRow Create(
            string itemText,
            string stateText,
            string evidenceText,
            string nextActionText)
        {
            return new OpenVisionRecipeOperatorValidationRow(itemText, stateText, evidenceText, nextActionText);
        }
    }

    public sealed class OpenVisionRecipeOperatorResultChannelRow
    {
        private OpenVisionRecipeOperatorResultChannelRow(
            string channelText,
            string valueText,
            string sourceText,
            string useText)
        {
            ChannelText = string.IsNullOrWhiteSpace(channelText) ? "-" : channelText.Trim();
            ValueText = string.IsNullOrWhiteSpace(valueText) ? "-" : valueText.Trim();
            SourceText = string.IsNullOrWhiteSpace(sourceText) ? "-" : sourceText.Trim();
            UseText = string.IsNullOrWhiteSpace(useText) ? "-" : useText.Trim();
        }

        public string ChannelText { get; }

        public string ValueText { get; }

        public string SourceText { get; }

        public string UseText { get; }

        public bool IsOk => string.Equals(ValueText, "OK", StringComparison.OrdinalIgnoreCase);

        public bool IsNg => string.Equals(ValueText, "NG", StringComparison.OrdinalIgnoreCase);

        public bool IsWait => string.Equals(ValueText, "WAIT", StringComparison.OrdinalIgnoreCase);

        public string DisplayText => ChannelText + " | " + ValueText + " | " + SourceText;

        public static OpenVisionRecipeOperatorResultChannelRow Create(
            string channelText,
            string valueText,
            string sourceText,
            string useText)
        {
            return new OpenVisionRecipeOperatorResultChannelRow(channelText, valueText, sourceText, useText);
        }
    }

    public sealed class OpenVisionRecipeSampleRunSummary
    {
        public static OpenVisionRecipeSampleRunSummary Empty { get; } = new OpenVisionRecipeSampleRunSummary(
            OpenVisionRecipeText.Local("아직 실행 안 됨.", "Not run yet."),
            OpenVisionRecipeText.Local("샘플을 선택한 뒤 명시적으로 검사를 실행하세요.", "Select a sample and run an explicit check."),
            false);

        private OpenVisionRecipeSampleRunSummary(
            string statusText,
            string detailText,
            bool hasResult,
            string compactText = null,
            bool succeeded = false,
            string distanceMetricText = null)
        {
            StatusText = statusText ?? string.Empty;
            DetailText = detailText ?? string.Empty;
            HasResult = hasResult;
            Succeeded = succeeded;
            CompactText = string.IsNullOrWhiteSpace(compactText) ? StatusText : compactText.Trim();
            DistanceMetricText = string.IsNullOrWhiteSpace(distanceMetricText) ? string.Empty : distanceMetricText.Trim();
        }

        public string StatusText { get; }

        public string DetailText { get; }

        public bool HasResult { get; }

        public bool Succeeded { get; }

        public string CompactText { get; }

        public string DistanceMetricText { get; }

        public string DisplayText => StatusText + Environment.NewLine + DetailText;

        public static OpenVisionRecipeSampleRunSummary CreatePending(OpenVisionRecipeSampleOption sampleOption)
        {
            if (sampleOption == null)
            {
                return Empty;
            }

            return new OpenVisionRecipeSampleRunSummary(
                OpenVisionRecipeText.Local("아직 실행 안 됨.", "Not run yet."),
                OpenVisionRecipeText.Local("선택 샘플 실행 준비: ", "Ready to run selected sample: ") + sampleOption.SampleName,
                false,
                OpenVisionRecipeText.Local("준비: ", "Ready: ") + sampleOption.SampleName);
        }

        public static OpenVisionRecipeSampleRunSummary CreateRunning(
            OpenVisionRecipeSampleOption sampleOption,
            string pipelineName)
        {
            return new OpenVisionRecipeSampleRunSummary(
                OpenVisionRecipeText.Local("샘플 검사 실행 중...", "Running sample check..."),
                FormatSampleAndPipeline(sampleOption, pipelineName),
                false,
                OpenVisionRecipeText.Local("실행 중: ", "Running: ") + (string.IsNullOrWhiteSpace(sampleOption?.SampleName) ? "-" : sampleOption.SampleName));
        }

        internal static OpenVisionRecipeSampleRunSummary FromResult(
            OpenVisionRecipeSampleOption sampleOption,
            string pipelineName,
            VisionPipelineSampleCheckResult result)
        {
            if (result == null)
            {
                return new OpenVisionRecipeSampleRunSummary(
                    OpenVisionRecipeText.Local("샘플 검사 ERROR", "Sample check ERROR"),
                    FormatSampleAndPipeline(sampleOption, pipelineName),
                    true,
                    OpenVisionRecipeText.Local("샘플 검사 ERROR", "Sample check ERROR"));
            }

            string status = string.IsNullOrWhiteSpace(result.Status) ? "-" : result.Status;
            string metric = string.IsNullOrWhiteSpace(result.MetricText) ? "-" : result.MetricText;
            List<string> lines = new List<string>
            {
                FormatSampleAndPipeline(sampleOption, pipelineName),
                OpenVisionRecipeText.Local("지표: ", "Metric: ") + metric,
                OpenVisionRecipeText.Local("동작: ", "Action: ") + (string.IsNullOrWhiteSpace(result.ActionSummaryText) ? "-" : result.ActionSummaryText),
                OpenVisionRecipeText.Local("다음: ", "Next: ") + BuildSampleNextAction(result)
            };

            if (!string.IsNullOrWhiteSpace(result.FailedStepText))
            {
                lines.Add(OpenVisionRecipeText.Local("실패 단계: ", "Failed step: ") + result.FailedStepText);
            }

            if (!string.IsNullOrWhiteSpace(result.Message))
            {
                lines.Add(OpenVisionRecipeText.Local("메시지: ", "Message: ") + result.Message);
            }

            string compact = OpenVisionRecipeText.Local("샘플 검사 ", "Sample check ") + status + " | " + metric;
            if (!result.Success && !string.IsNullOrWhiteSpace(result.FailedStepText))
            {
                compact += " | " + result.FailedStepText;
            }

            if (!result.Success)
            {
                compact += OpenVisionRecipeText.Local(" | 다음: ", " | Next: ") + BuildSampleNextAction(result);
            }

            return new OpenVisionRecipeSampleRunSummary(
                OpenVisionRecipeText.Local("샘플 검사 ", "Sample check ") + status,
                string.Join(Environment.NewLine, lines),
                true,
                compact,
                result.Success,
                result.DistanceMetricText);
        }

        private static string BuildSampleNextAction(VisionPipelineSampleCheckResult result)
        {
            if (result?.Success == true)
            {
                return OpenVisionRecipeText.Local("추가 조치가 필요 없습니다.", "No action needed.");
            }

            string message = result?.Message ?? string.Empty;
            if (message.IndexOf("missing", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return OpenVisionRecipeText.Local("기대 지표 이름과 해당 지표를 생성해야 하는 출력 단계를 확인하세요.", "Check the expected metric name and the output step that should produce it.");
            }

            if (!string.IsNullOrWhiteSpace(result?.FailedStepText))
            {
                return OpenVisionRecipeText.Local("실패 단계를 열어 입력/출력 레이어를 확인한 뒤 해당 도구 파라미터를 조정하세요.", "Open the failed step, review input/output layers, then tune that tool parameter.");
            }

            if (string.Equals(result?.Status, "ERROR", StringComparison.OrdinalIgnoreCase))
            {
                return OpenVisionRecipeText.Local("XML, 샘플 이미지 경로, 참조 템플릿 파일을 검증하세요.", "Validate XML, sample image path, and referenced template files.");
            }

            return OpenVisionRecipeText.Local("지표 기준과 실제값을 비교한 뒤 임계값/ROI/템플릿 파라미터를 조정하세요.", "Compare metric gate versus actual value, then tune threshold/ROI/template parameters.");
        }

        private static string FormatSampleAndPipeline(OpenVisionRecipeSampleOption sampleOption, string pipelineName)
        {
            string sample = string.IsNullOrWhiteSpace(sampleOption?.SampleName) ? "-" : sampleOption.SampleName;
            string pipeline = string.IsNullOrWhiteSpace(pipelineName) ? "-" : pipelineName;
            return OpenVisionRecipeText.Local("샘플: ", "Sample: ") + sample
                + " / "
                + OpenVisionRecipeText.Local("파이프라인: ", "Pipeline: ") + pipeline;
        }
    }

    public sealed class OpenVisionRecipeSampleMatrixRow
    {
        private OpenVisionRecipeSampleMatrixRow(
            string role,
            string sampleName,
            string expectedText,
            string resultText,
            string metricText,
            string failedStep,
            string nextActionText,
            bool hasResult,
            bool success,
            bool isPlaceholder)
        {
            Role = string.IsNullOrWhiteSpace(role) ? "-" : role.Trim();
            SampleName = sampleName ?? string.Empty;
            ExpectedText = string.IsNullOrWhiteSpace(expectedText) ? "-" : expectedText.Trim();
            ResultText = string.IsNullOrWhiteSpace(resultText) ? "WAIT" : resultText.Trim();
            MetricText = string.IsNullOrWhiteSpace(metricText) ? "-" : metricText.Trim();
            FailedStep = failedStep ?? string.Empty;
            NextActionText = string.IsNullOrWhiteSpace(nextActionText) ? "-" : nextActionText.Trim();
            HasResult = hasResult;
            Success = success;
            IsPlaceholder = isPlaceholder;
        }

        public string Role { get; }

        public string SampleName { get; }

        public string ExpectedText { get; }

        public string ResultText { get; }

        public string MetricText { get; }

        public string FailedStep { get; }

        public string NextActionText { get; }

        public bool HasResult { get; }

        public bool Success { get; }

        public bool IsPlaceholder { get; }

        public string FailedStepDisplayText =>
            string.IsNullOrWhiteSpace(FailedStep) ? "-" : FailedStep.Trim();

        public string ResultBadgeText =>
            !HasResult ? "WAIT" : (Success ? "OK" : "NG");

        public string DisplayText =>
            Role + " | " + (string.IsNullOrWhiteSpace(SampleName) ? "-" : SampleName)
            + " | " + ResultBadgeText;

        public string ReviewText =>
            OpenVisionRecipeText.Local("샘플: ", "Sample: ") + (string.IsNullOrWhiteSpace(SampleName) ? "-" : SampleName)
            + Environment.NewLine
            + OpenVisionRecipeText.Local("역할/결과: ", "Role/result: ") + Role + " / " + ResultBadgeText
            + Environment.NewLine
            + OpenVisionRecipeText.Local("기대 기준: ", "Expected gate: ") + ExpectedText
            + Environment.NewLine
            + OpenVisionRecipeText.Local("현재 지표: ", "Current metric: ") + MetricText
            + Environment.NewLine
            + OpenVisionRecipeText.Local("실패 Step: ", "Failed step: ") + FailedStepDisplayText
            + Environment.NewLine
            + OpenVisionRecipeText.Local("다음: ", "Next: ") + NextActionText;

        internal static OpenVisionRecipeSampleMatrixRow Create(
            VisionPipelineSampleCatalogItem sample,
            OpenVisionRecipePairSampleRunSummary result)
        {
            if (sample == null)
            {
                return CreateEmpty();
            }

            bool hasResult = result != null;
            string role = string.IsNullOrWhiteSpace(sample.PairRole) ? "Sample" : sample.PairRole.Trim();
            string expected = sample.ExpectsFailure
                ? OpenVisionRecipeText.Local("통제된 NG/no-result 기대", "Expected controlled NG/no-result")
                : sample.ExpectedText;
            string metric = hasResult ? result.MetricText : sample.ExpectedText;
            string next = hasResult
                ? result.NextActionText
                : OpenVisionRecipeText.Local("명시적으로 Good/Bad 쌍 검사를 실행하세요.", "Run the explicit Good/Bad pair check.");

            return new OpenVisionRecipeSampleMatrixRow(
                role,
                sample.SampleName,
                expected,
                hasResult ? result.ResultText : "WAIT",
                metric,
                result?.FailedStepText,
                next,
                hasResult,
                result?.Success ?? false,
                false);
        }

        public static OpenVisionRecipeSampleMatrixRow CreateEmpty()
        {
            return new OpenVisionRecipeSampleMatrixRow(
                "-",
                OpenVisionRecipeText.Local("샘플 없음", "No sample"),
                "-",
                "WAIT",
                "-",
                string.Empty,
                OpenVisionRecipeText.Local("샘플을 선택하세요.", "Select a sample."),
                false,
                false,
                true);
        }
    }

    public sealed class OpenVisionRecipeCatalogBenchmarkSummary
    {
        public static OpenVisionRecipeCatalogBenchmarkSummary Empty { get; } = new OpenVisionRecipeCatalogBenchmarkSummary(
            OpenVisionRecipeText.Local("카탈로그 벤치마크 미실행", "Catalog benchmark not run."),
            OpenVisionRecipeText.Local("현재 파이프라인을 Product sample catalog 전체에 대해 명시적으로 실행하면 결과가 여기에 표시됩니다.", "Run the current pipeline against the full Product sample catalog to show the result here."),
            false,
            false,
            string.Empty);

        private OpenVisionRecipeCatalogBenchmarkSummary(
            string compactText,
            string detailText,
            bool hasResult,
            bool succeeded,
            string summaryPath)
        {
            CompactText = compactText ?? string.Empty;
            DetailText = detailText ?? string.Empty;
            HasResult = hasResult;
            Succeeded = succeeded;
            SummaryPath = summaryPath ?? string.Empty;
        }

        public string CompactText { get; }

        public string DetailText { get; }

        public bool HasResult { get; }

        public bool Succeeded { get; }

        public string SummaryPath { get; }

        public static OpenVisionRecipeCatalogBenchmarkSummary CreateRunning(string pipelineName, int total)
        {
            string pipeline = string.IsNullOrWhiteSpace(pipelineName) ? "-" : pipelineName.Trim();
            return new OpenVisionRecipeCatalogBenchmarkSummary(
                OpenVisionRecipeText.Local("카탈로그 벤치마크 실행 중", "Catalog benchmark running"),
                OpenVisionRecipeText.Local("파이프라인: ", "Pipeline: ") + pipeline
                + Environment.NewLine
                + OpenVisionRecipeText.Local("대상 Product 샘플: ", "Target Product samples: ") + total.ToString(CultureInfo.InvariantCulture),
                false,
                false,
                string.Empty);
        }

        public static OpenVisionRecipeCatalogBenchmarkSummary CreateProgress(
            string pipelineName,
            int completed,
            int total,
            IReadOnlyList<VisionPipelineBatchSampleRunResult> results)
        {
            List<VisionPipelineBatchSampleRunResult> resultList =
                (results ?? Array.Empty<VisionPipelineBatchSampleRunResult>()).Where(result => result != null).ToList();
            int pass = resultList.Count(result => result.Success);
            int fail = resultList.Count(result => !result.Success);
            string compact = OpenVisionRecipeText.Local("진행: ", "Progress: ")
                + completed.ToString(CultureInfo.InvariantCulture)
                + "/"
                + total.ToString(CultureInfo.InvariantCulture)
                + " | OK "
                + pass.ToString(CultureInfo.InvariantCulture)
                + " / NG "
                + fail.ToString(CultureInfo.InvariantCulture);

            return new OpenVisionRecipeCatalogBenchmarkSummary(
                compact,
                OpenVisionRecipeText.Local("파이프라인: ", "Pipeline: ") + (string.IsNullOrWhiteSpace(pipelineName) ? "-" : pipelineName.Trim())
                + Environment.NewLine
                + compact
                + FormatFailurePreview(resultList),
                false,
                false,
                string.Empty);
        }

        public static OpenVisionRecipeCatalogBenchmarkSummary FromResults(
            string pipelineName,
            IReadOnlyList<VisionPipelineBatchSampleRunResult> results,
            string summaryPath)
        {
            List<VisionPipelineBatchSampleRunResult> resultList =
                (results ?? Array.Empty<VisionPipelineBatchSampleRunResult>()).Where(result => result != null).ToList();
            int total = resultList.Count;
            int pass = resultList.Count(result => result.Success);
            int fail = resultList.Count(result => !result.Success);
            bool ok = total > 0 && fail == 0;
            string compact = "Catalog "
                + (ok ? "OK" : "NG")
                + " | "
                + pass.ToString(CultureInfo.InvariantCulture)
                + "/"
                + total.ToString(CultureInfo.InvariantCulture)
                + OpenVisionRecipeText.Local(" 통과", " pass");

            List<string> lines = new List<string>
            {
                OpenVisionRecipeText.Local("파이프라인: ", "Pipeline: ") + (string.IsNullOrWhiteSpace(pipelineName) ? "-" : pipelineName.Trim()),
                OpenVisionRecipeText.Local("Product 샘플: ", "Product samples: ") + total.ToString(CultureInfo.InvariantCulture),
                OpenVisionRecipeText.Local("통과/실패: ", "Pass/fail: ") + pass.ToString(CultureInfo.InvariantCulture) + "/" + fail.ToString(CultureInfo.InvariantCulture),
                OpenVisionRecipeText.Local("다음: ", "Next: ") + (ok
                    ? OpenVisionRecipeText.Local("대량 샘플에서 회귀가 발견되지 않았습니다. Run History에서 summary.tsv를 보관하거나 비교하세요.", "No regression was found across the catalog. Keep or compare the summary.tsv from Run History.")
                    : OpenVisionRecipeText.Local("Run History에서 NG 샘플을 선택하고 실패 Step, 입력/출력 레이어, PropertyGrid 파라미터를 확인하세요.", "Select NG samples in Run History and review failed steps, input/output layers, and PropertyGrid parameters."))
            };
            string failurePreview = FormatFailurePreview(resultList);
            if (!string.IsNullOrWhiteSpace(failurePreview))
            {
                lines.Add(failurePreview.Trim());
            }

            if (!string.IsNullOrWhiteSpace(summaryPath))
            {
                lines.Add(OpenVisionRecipeText.Local("저장된 요약: ", "Saved summary: ") + summaryPath);
            }

            return new OpenVisionRecipeCatalogBenchmarkSummary(
                compact,
                string.Join(Environment.NewLine, lines),
                true,
                ok,
                summaryPath);
        }

        public static OpenVisionRecipeCatalogBenchmarkSummary FromError(string pipelineName, string message)
        {
            return new OpenVisionRecipeCatalogBenchmarkSummary(
                OpenVisionRecipeText.Local("카탈로그 벤치마크 ERROR", "Catalog benchmark ERROR"),
                OpenVisionRecipeText.Local("파이프라인: ", "Pipeline: ") + (string.IsNullOrWhiteSpace(pipelineName) ? "-" : pipelineName.Trim())
                + Environment.NewLine
                + OpenVisionRecipeText.Local("메시지: ", "Message: ") + (message ?? string.Empty),
                true,
                false,
                string.Empty);
        }

        private static string FormatFailurePreview(IReadOnlyList<VisionPipelineBatchSampleRunResult> results)
        {
            List<VisionPipelineBatchSampleRunResult> failures = (results ?? Array.Empty<VisionPipelineBatchSampleRunResult>())
                .Where(result => result != null && !result.Success)
                .Take(5)
                .ToList();
            if (failures.Count == 0)
            {
                return string.Empty;
            }

            return Environment.NewLine
                + OpenVisionRecipeText.Local("주요 실패: ", "Top failures: ")
                + string.Join(", ", failures.Select(FormatFailure));
        }

        private static string FormatFailure(VisionPipelineBatchSampleRunResult result)
        {
            string sample = string.IsNullOrWhiteSpace(result.SampleName) ? "-" : result.SampleName.Trim();
            string step = string.IsNullOrWhiteSpace(result.FailedStep) ? string.Empty : " @ " + result.FailedStep.Trim();
            return sample + step;
        }
    }

    public sealed class OpenVisionRecipePairRunSummary
    {
        public static OpenVisionRecipePairRunSummary Empty { get; } = new OpenVisionRecipePairRunSummary(
            OpenVisionRecipeText.Local("쌍 검사 미실행.", "Pair check not run."),
            OpenVisionRecipeText.Local("Good/Bad 샘플 쌍을 선택한 뒤 명시적으로 쌍 검사를 실행하세요.", "Select a Good/Bad sample pair and run an explicit pair check."),
            false);

        private OpenVisionRecipePairRunSummary(
            string statusText,
            string detailText,
            bool hasResult,
            string compactText = null,
            bool succeeded = false,
            IReadOnlyList<OpenVisionRecipePairSampleRunSummary> sampleResults = null)
        {
            StatusText = statusText ?? string.Empty;
            DetailText = detailText ?? string.Empty;
            HasResult = hasResult;
            Succeeded = succeeded;
            CompactText = string.IsNullOrWhiteSpace(compactText) ? StatusText : compactText.Trim();
            SampleResults = sampleResults ?? Array.Empty<OpenVisionRecipePairSampleRunSummary>();
        }

        public string StatusText { get; }

        public string DetailText { get; }

        public bool HasResult { get; }

        public bool Succeeded { get; }

        public string CompactText { get; }

        public IReadOnlyList<OpenVisionRecipePairSampleRunSummary> SampleResults { get; }

        public string DisplayText => StatusText + Environment.NewLine + DetailText;

        public static OpenVisionRecipePairRunSummary CreatePending(OpenVisionRecipeSampleOption sampleOption)
        {
            if (sampleOption?.Sample == null || string.IsNullOrWhiteSpace(sampleOption.Sample.PairGroup))
            {
                return Empty;
            }

            return new OpenVisionRecipePairRunSummary(
                OpenVisionRecipeText.Local("쌍 검사 미실행.", "Pair check not run."),
                OpenVisionRecipeText.Local("PairGroup 실행 준비: ", "Ready to run PairGroup: ") + sampleOption.Sample.PairGroup,
                false,
                OpenVisionRecipeText.Local("준비: ", "Ready: ") + sampleOption.Sample.PairGroup);
        }

        public static OpenVisionRecipePairRunSummary CreateRunning(
            OpenVisionRecipeSampleOption sampleOption,
            string pipelineName,
            int sampleCount)
        {
            string group = string.IsNullOrWhiteSpace(sampleOption?.Sample?.PairGroup) ? "-" : sampleOption.Sample.PairGroup.Trim();
            return new OpenVisionRecipePairRunSummary(
                OpenVisionRecipeText.Local("Good/Bad 쌍 검사 실행 중...", "Running Good/Bad pair check..."),
                "PairGroup: " + group + " / " + OpenVisionRecipeText.Local("파이프라인: ", "Pipeline: ") + (string.IsNullOrWhiteSpace(pipelineName) ? "-" : pipelineName),
                false,
                OpenVisionRecipeText.Local("실행 중: ", "Running: ") + group + " (" + sampleCount.ToString(CultureInfo.InvariantCulture) + OpenVisionRecipeText.Local("개 샘플", " samples") + ")");
        }

        internal static OpenVisionRecipePairRunSummary FromResults(
            OpenVisionRecipeSampleOption sampleOption,
            string pipelineName,
            IReadOnlyList<OpenVisionRecipePairSampleRunSummary> results,
            string summaryPath)
        {
            List<OpenVisionRecipePairSampleRunSummary> resultList = (results ?? Array.Empty<OpenVisionRecipePairSampleRunSummary>()).ToList();
            int total = resultList.Count;
            int pass = resultList.Count(result => result.Success);
            bool ok = total > 0 && pass == total;
            string group = string.IsNullOrWhiteSpace(sampleOption?.Sample?.PairGroup) ? "-" : sampleOption.Sample.PairGroup.Trim();
            string compact = OpenVisionRecipeText.Local("쌍 검사 ", "Pair check ") + (ok ? "OK" : "NG")
                + " | " + pass.ToString(CultureInfo.InvariantCulture)
                + "/" + total.ToString(CultureInfo.InvariantCulture)
                + OpenVisionRecipeText.Local(" 통과", " pass");

            if (resultList.Count > 0)
            {
                compact += " | " + string.Join(" | ", resultList.Select(result => result.CompactText));
            }

            List<string> lines = new List<string>
            {
                "PairGroup: " + group,
                OpenVisionRecipeText.Local("파이프라인: ", "Pipeline: ") + (string.IsNullOrWhiteSpace(pipelineName) ? "-" : pipelineName),
                OpenVisionRecipeText.Local("통과: ", "Pass: ") + pass.ToString(CultureInfo.InvariantCulture) + "/" + total.ToString(CultureInfo.InvariantCulture),
                OpenVisionRecipeText.Local("다음: ", "Next: ") + (ok
                    ? OpenVisionRecipeText.Local("추가 조치가 필요 없습니다.", "No action needed.")
                    : OpenVisionRecipeText.Local("아래 실패 샘플 역할을 열고 Good/Bad가 모두 기대와 맞을 때까지 활성 파이프라인을 조정하세요.", "Open the failed sample role below and tune the active pipeline until Good and Bad both match expectations."))
            };
            lines.AddRange(resultList.Select(result => result.DisplayText));
            if (!string.IsNullOrWhiteSpace(summaryPath))
            {
                lines.Add(OpenVisionRecipeText.Local("저장된 요약: ", "Saved summary: ") + summaryPath);
            }

            return new OpenVisionRecipePairRunSummary(
                OpenVisionRecipeText.Local("쌍 검사 ", "Pair check ") + (ok ? "OK" : "NG"),
                string.Join(Environment.NewLine, lines),
                true,
                compact,
                ok,
                resultList);
        }

        internal static OpenVisionRecipePairRunSummary FromError(
            OpenVisionRecipeSampleOption sampleOption,
            string pipelineName,
            string message)
        {
            string group = string.IsNullOrWhiteSpace(sampleOption?.Sample?.PairGroup) ? "-" : sampleOption.Sample.PairGroup.Trim();
            return new OpenVisionRecipePairRunSummary(
                OpenVisionRecipeText.Local("쌍 검사 ERROR", "Pair check ERROR"),
                "PairGroup: " + group
                + Environment.NewLine
                + OpenVisionRecipeText.Local("파이프라인: ", "Pipeline: ") + (string.IsNullOrWhiteSpace(pipelineName) ? "-" : pipelineName)
                + Environment.NewLine
                + OpenVisionRecipeText.Local("메시지: ", "Message: ") + (message ?? string.Empty),
                true,
                OpenVisionRecipeText.Local("쌍 검사 ERROR | ", "Pair check ERROR | ") + group);
        }
    }

    public sealed class OpenVisionRecipePairSampleRunSummary
    {
        private OpenVisionRecipePairSampleRunSummary(
            string role,
            string sampleName,
            string status,
            bool success,
            string metricText,
            string message,
            string failedStepText)
        {
            Role = string.IsNullOrWhiteSpace(role) ? "Sample" : role.Trim();
            SampleName = sampleName ?? string.Empty;
            Status = status ?? string.Empty;
            Success = success;
            MetricText = metricText ?? string.Empty;
            Message = message ?? string.Empty;
            FailedStepText = failedStepText ?? string.Empty;
        }

        public string Role { get; }

        public string SampleName { get; }

        public string Status { get; }

        public bool Success { get; }

        public string MetricText { get; }

        public string Message { get; }

        public string FailedStepText { get; }

        public bool CanOpenFailedStep =>
            !Success && !string.IsNullOrWhiteSpace(FailedStepText) && FailedStepText.Trim() != "-";

        public string CompactText =>
            Role + " " + (string.IsNullOrWhiteSpace(Status) ? "-" : Status);

        public string ResultText =>
            Success ? "OK" : "NG";

        public string ActionText =>
            Success
                ? OpenVisionRecipeText.Local("기대 결과와 일치", "Matches expected result")
                : OpenVisionRecipeText.Local("실패 Step과 판정 기준 확인", "Review failed step and gate");

        public string DisplayText =>
            Role + ": "
            + (string.IsNullOrWhiteSpace(SampleName) ? "-" : SampleName)
            + " / "
            + (string.IsNullOrWhiteSpace(Status) ? "-" : Status)
            + " / "
            + (string.IsNullOrWhiteSpace(MetricText) ? "-" : MetricText)
            + (string.IsNullOrWhiteSpace(Message) ? string.Empty : " / " + Message);

        public string OpenFailedStepText =>
            CanOpenFailedStep
                ? OpenVisionRecipeText.Local("Step 보기", "View step")
                : OpenVisionRecipeText.Local("검토", "Review");

        public string NextActionText
        {
            get
            {
                if (Success)
                {
                    return OpenVisionRecipeText.Local("예상 결과와 일치합니다. 반대 역할도 OK인지 확인하세요.", "Matches the expected result. Confirm the counterpart role is also OK.");
                }

                if (CanOpenFailedStep)
                {
                    return OpenVisionRecipeText.Local("실패 Step을 선택한 뒤 입력/출력 레이어, 판정 기준, PropertyGrid 파라미터를 조정하세요.", "Select the failed step, then tune input/output layers, gates, and PropertyGrid parameters.");
                }

                return OpenVisionRecipeText.Local("실패 Step 기록이 없습니다. 실행 로그와 XML 경로를 먼저 확인하세요.", "No failed step was recorded. Check the run log and XML route first.");
            }
        }

        public string ReviewText =>
            Role + " / " + ResultText
            + Environment.NewLine
            + OpenVisionRecipeText.Local("샘플: ", "Sample: ") + (string.IsNullOrWhiteSpace(SampleName) ? "-" : SampleName)
            + Environment.NewLine
            + OpenVisionRecipeText.Local("지표: ", "Metric: ") + (string.IsNullOrWhiteSpace(MetricText) ? "-" : MetricText)
            + Environment.NewLine
            + OpenVisionRecipeText.Local("실패 Step: ", "Failed step: ") + (string.IsNullOrWhiteSpace(FailedStepText) ? "-" : FailedStepText)
            + Environment.NewLine
            + OpenVisionRecipeText.Local("다음: ", "Next: ") + NextActionText;

        internal static OpenVisionRecipePairSampleRunSummary FromResult(
            VisionPipelineSampleCatalogItem sample,
            VisionPipelineSampleCheckResult result)
        {
            return new OpenVisionRecipePairSampleRunSummary(
                sample?.PairRole,
                sample?.SampleName,
                result?.Status,
                result?.Success ?? false,
                result?.MetricText,
                result?.Message,
                result?.FailedStepText);
        }

        internal static OpenVisionRecipePairSampleRunSummary CreateForTest(
            string role,
            string sampleName,
            string status,
            bool success,
            string metricText,
            string message,
            string failedStepText)
        {
            return new OpenVisionRecipePairSampleRunSummary(
                role,
                sampleName,
                status,
                success,
                metricText,
                message,
                failedStepText);
        }
    }

    public sealed class OpenVisionRecipeBatchRunOption
    {
        private OpenVisionRecipeBatchRunOption(
            string displayText,
            string detailText,
            string summaryPath,
            IReadOnlyList<OpenVisionRecipeBatchSampleResultOption> sampleResults)
        {
            DisplayText = displayText ?? string.Empty;
            DetailText = detailText ?? string.Empty;
            SummaryPath = summaryPath ?? string.Empty;
            SampleResults = sampleResults ?? Array.Empty<OpenVisionRecipeBatchSampleResultOption>();
        }

        public string DisplayText { get; }

        public string DetailText { get; }

        public string SummaryPath { get; }

        public IReadOnlyList<OpenVisionRecipeBatchSampleResultOption> SampleResults { get; }

        internal static OpenVisionRecipeBatchRunOption Create(
            VisionPipelineBatchRunSummaryStorage.BatchRunSummaryInfo summary)
        {
            if (summary == null)
            {
                return CreateEmpty();
            }

            VisionPipelineBatchRunSummary runSummary = VisionPipelineBatchRunSummaryStorage.Load(summary.SummaryPath);
            string status = summary.FailCount == 0 && summary.TotalCount > 0 ? "OK" : "NG";
            string display = summary.StartedAt.ToString("MM-dd HH:mm:ss", CultureInfo.CurrentCulture)
                + " | " + status
                + " | " + summary.PassCount.ToString(CultureInfo.InvariantCulture)
                + "/" + summary.TotalCount.ToString(CultureInfo.InvariantCulture);
            string detail = FormatBatchRunDetail(summary, runSummary)
                + " | "
                + OpenVisionRecipeText.Local("요약: ", "Summary: ")
                + summary.SummaryPath;
            IReadOnlyList<OpenVisionRecipeBatchSampleResultOption> sampleResults = BuildSampleResults(runSummary);
            return new OpenVisionRecipeBatchRunOption(display, detail, summary.SummaryPath, sampleResults);
        }

        private static string FormatBatchRunDetail(
            VisionPipelineBatchRunSummaryStorage.BatchRunSummaryInfo summary,
            VisionPipelineBatchRunSummary runSummary)
        {
            if (summary.FailCount <= 0)
            {
                return OpenVisionRecipeText.Local("모든 샘플 통과", "All samples passed");
            }

            List<VisionPipelineBatchSampleRunResult> failures = runSummary?.Results?
                .Where(result => result != null && !result.Success)
                .Take(2)
                .ToList() ?? new List<VisionPipelineBatchSampleRunResult>();

            if (failures.Count == 0)
            {
                return OpenVisionRecipeText.Local("실패: ", "Fail: ")
                    + summary.FailCount.ToString(CultureInfo.InvariantCulture);
            }

            string failedSamples = string.Join(", ", failures.Select(FormatFailure));
            int remaining = Math.Max(0, summary.FailCount - failures.Count);
            if (remaining > 0)
            {
                failedSamples += " +" + remaining.ToString(CultureInfo.InvariantCulture);
            }

            return OpenVisionRecipeText.Local("실패 샘플: ", "Failed: ") + failedSamples;
        }

        private static string FormatFailure(VisionPipelineBatchSampleRunResult result)
        {
            string sample = string.IsNullOrWhiteSpace(result.SampleName) ? "-" : result.SampleName.Trim();
            string step = string.IsNullOrWhiteSpace(result.FailedStep) ? string.Empty : " @ " + result.FailedStep.Trim();
            return sample + step;
        }

        private static IReadOnlyList<OpenVisionRecipeBatchSampleResultOption> BuildSampleResults(VisionPipelineBatchRunSummary runSummary)
        {
            List<OpenVisionRecipeBatchSampleResultOption> results = runSummary?.Results?
                .Where(result => result != null)
                .Select(OpenVisionRecipeBatchSampleResultOption.Create)
                .ToList() ?? new List<OpenVisionRecipeBatchSampleResultOption>();

            if (results.Count == 0)
            {
                results.Add(OpenVisionRecipeBatchSampleResultOption.CreateEmpty());
            }

            return results;
        }

        public static OpenVisionRecipeBatchRunOption CreateEmpty()
        {
            return new OpenVisionRecipeBatchRunOption(
                OpenVisionRecipeText.Local("저장된 쌍 검사 이력이 없습니다.", "No saved pair check runs."),
                OpenVisionRecipeText.Local("쌍 검사를 실행하면 최근 3건이 여기에 표시됩니다.", "Run a pair check to show the latest three runs here."),
                string.Empty,
                new[] { OpenVisionRecipeBatchSampleResultOption.CreateEmpty() });
        }
    }

    public sealed class OpenVisionRecipeBatchRunComparisonRow
    {
        private OpenVisionRecipeBatchRunComparisonRow(
            string sampleName,
            string stateText,
            string previousText,
            string currentText,
            string failedStep,
            string sampleImagePath,
            string reviewText,
            bool isComparable,
            bool isRegression,
            bool isRecovered,
            bool isStillFailing)
        {
            SampleName = sampleName ?? string.Empty;
            StateText = string.IsNullOrWhiteSpace(stateText) ? "-" : stateText.Trim();
            PreviousText = string.IsNullOrWhiteSpace(previousText) ? "-" : previousText.Trim();
            CurrentText = string.IsNullOrWhiteSpace(currentText) ? "-" : currentText.Trim();
            FailedStep = failedStep ?? string.Empty;
            SampleImagePath = sampleImagePath ?? string.Empty;
            ReviewText = string.IsNullOrWhiteSpace(reviewText) ? "-" : reviewText.Trim();
            IsComparable = isComparable;
            IsRegression = isRegression;
            IsRecovered = isRecovered;
            IsStillFailing = isStillFailing;
        }

        public string SampleName { get; }

        public string StateText { get; }

        public string PreviousText { get; }

        public string CurrentText { get; }

        public string FailedStep { get; }

        public string SampleImagePath { get; }

        public string ReviewText { get; }

        public bool IsComparable { get; }

        public bool IsRegression { get; }

        public bool IsRecovered { get; }

        public bool IsStillFailing { get; }

        public string DisplayText => StateText + " | " + (string.IsNullOrWhiteSpace(SampleName) ? "-" : SampleName);

        public string DetailText => PreviousText + " -> " + CurrentText;

        public static OpenVisionRecipeBatchRunComparisonRow Create(
            string sampleName,
            VisionPipelineBatchSampleRunResult previous,
            VisionPipelineBatchSampleRunResult current)
        {
            if (previous == null && current == null)
            {
                return CreateEmpty();
            }

            bool previousExists = previous != null;
            bool currentExists = current != null;
            bool previousSuccess = previous?.Success ?? false;
            bool currentSuccess = current?.Success ?? false;
            string state;
            bool regression = false;
            bool recovered = false;
            bool stillFailing = false;

            if (!previousExists)
            {
                state = currentSuccess ? "NEW OK" : "NEW NG";
                regression = !currentSuccess;
            }
            else if (!currentExists)
            {
                state = "MISSING";
            }
            else if (previousSuccess && !currentSuccess)
            {
                state = "REGRESSION";
                regression = true;
            }
            else if (!previousSuccess && currentSuccess)
            {
                state = "RECOVERED";
                recovered = true;
            }
            else if (!previousSuccess && !currentSuccess)
            {
                state = "STILL NG";
                stillFailing = true;
            }
            else
            {
                state = "OK";
            }

            string failedStep = !string.IsNullOrWhiteSpace(current?.FailedStep)
                ? current.FailedStep
                : previous?.FailedStep ?? string.Empty;
            string sampleImagePath = !string.IsNullOrWhiteSpace(current?.ReportPath)
                ? current.ReportPath
                : previous?.ReportPath ?? string.Empty;
            string review = BuildReviewText(sampleName, state, previous, current, failedStep);

            return new OpenVisionRecipeBatchRunComparisonRow(
                sampleName,
                state,
                FormatResult(previous),
                FormatResult(current),
                failedStep,
                sampleImagePath,
                review,
                previousExists && currentExists,
                regression,
                recovered,
                stillFailing);
        }

        public static OpenVisionRecipeBatchRunComparisonRow CreateEmpty()
        {
            return new OpenVisionRecipeBatchRunComparisonRow(
                OpenVisionRecipeText.Local("비교 결과 없음", "No comparison results"),
                "-",
                "-",
                "-",
                string.Empty,
                string.Empty,
                OpenVisionRecipeText.Local("비교할 benchmark 결과가 없습니다.", "No benchmark comparison result is available."),
                false,
                false,
                false,
                false);
        }

        public static OpenVisionRecipeBatchRunComparisonRow CreateNoBaseline(string currentRun)
        {
            return new OpenVisionRecipeBatchRunComparisonRow(
                OpenVisionRecipeText.Local("기준 이력 없음", "No baseline run"),
                "NO BASELINE",
                "-",
                string.IsNullOrWhiteSpace(currentRun) ? "-" : currentRun,
                string.Empty,
                string.Empty,
                OpenVisionRecipeText.Local("이전 benchmark 실행이 하나 더 있어야 회귀 비교가 가능합니다.", "Run at least one earlier benchmark to enable regression comparison."),
                false,
                false,
                false,
                false);
        }

        private static string FormatResult(VisionPipelineBatchSampleRunResult result)
        {
            if (result == null)
            {
                return "-";
            }

            string status = result.Success ? "OK" : "NG";
            if (!string.IsNullOrWhiteSpace(result.FailedStep))
            {
                status += " @ " + result.FailedStep.Trim();
            }

            return status;
        }

        private static string BuildReviewText(
            string sampleName,
            string state,
            VisionPipelineBatchSampleRunResult previous,
            VisionPipelineBatchSampleRunResult current,
            string failedStep)
        {
            string next;
            if (string.Equals(state, "REGRESSION", StringComparison.OrdinalIgnoreCase)
                || string.Equals(state, "NEW NG", StringComparison.OrdinalIgnoreCase))
            {
                next = OpenVisionRecipeText.Local("신규 실패입니다. 실패 Step과 현재 XML 파라미터를 먼저 확인하세요.", "New failure. Review the failed step and current XML parameters first.");
            }
            else if (string.Equals(state, "STILL NG", StringComparison.OrdinalIgnoreCase))
            {
                next = OpenVisionRecipeText.Local("지속 실패입니다. 기준/현재 실패 Step이 같은지 확인하고 파라미터 조정을 이어가세요.", "Persistent failure. Check whether the failed step is unchanged and continue parameter tuning.");
            }
            else if (string.Equals(state, "RECOVERED", StringComparison.OrdinalIgnoreCase))
            {
                next = OpenVisionRecipeText.Local("복구된 샘플입니다. 변경한 파라미터를 유지하고 다른 NG만 확인하세요.", "Recovered sample. Keep the change and focus on remaining NG samples.");
            }
            else
            {
                next = OpenVisionRecipeText.Local("회귀 없음. 다른 Regression/Still NG 항목을 우선 확인하세요.", "No regression. Prioritize Regression or Still NG rows.");
            }

            return OpenVisionRecipeText.Local("샘플: ", "Sample: ") + (string.IsNullOrWhiteSpace(sampleName) ? "-" : sampleName)
                + Environment.NewLine
                + "Diff: " + state
                + Environment.NewLine
                + OpenVisionRecipeText.Local("이전: ", "Previous: ") + FormatResult(previous)
                + Environment.NewLine
                + OpenVisionRecipeText.Local("현재: ", "Current: ") + FormatResult(current)
                + Environment.NewLine
                + OpenVisionRecipeText.Local("실패 Step: ", "Failed step: ") + (string.IsNullOrWhiteSpace(failedStep) ? "-" : failedStep)
                + Environment.NewLine
                + OpenVisionRecipeText.Local("다음: ", "Next: ") + next;
        }
    }

    public sealed class OpenVisionRecipeBatchSampleResultOption
    {
        private OpenVisionRecipeBatchSampleResultOption(
            string displayText,
            string detailText,
            string reviewText,
            bool success,
            string failedStep,
            string sampleName,
            string reportPath)
        {
            DisplayText = displayText ?? string.Empty;
            DetailText = detailText ?? string.Empty;
            ReviewText = reviewText ?? string.Empty;
            Success = success;
            FailedStep = failedStep ?? string.Empty;
            SampleName = sampleName ?? string.Empty;
            ReportPath = reportPath ?? string.Empty;
        }

        public string DisplayText { get; }

        public string DetailText { get; }

        public string ReviewText { get; }

        public bool Success { get; }

        public string FailedStep { get; }

        public string SampleName { get; }

        public string ReportPath { get; }

        internal static OpenVisionRecipeBatchSampleResultOption Create(VisionPipelineBatchSampleRunResult result)
        {
            if (result == null)
            {
                return CreateEmpty();
            }

            string status = result.Success ? "OK" : "NG";
            string display = status
                + " | "
                + (string.IsNullOrWhiteSpace(result.SampleName) ? "-" : result.SampleName.Trim())
                + " | "
                + result.TotalMilliseconds.ToString("0.0", CultureInfo.InvariantCulture)
                + " ms";
            string detail = string.IsNullOrWhiteSpace(result.FailedStep)
                ? OpenVisionRecipeText.Local("실패 Step 없음", "No failed step")
                : OpenVisionRecipeText.Local("실패 Step: ", "Failed step: ") + result.FailedStep.Trim();
            if (!string.IsNullOrWhiteSpace(result.Message))
            {
                detail += " | " + result.Message.Trim();
            }

            string review = result.Success
                ? OpenVisionRecipeText.Local("판독: 통과. NG 샘플을 선택하면 실패 Step을 연결합니다.", "Review: Passed. Select an NG sample to link the failed step.")
                : string.IsNullOrWhiteSpace(result.FailedStep)
                    ? OpenVisionRecipeText.Local("판독: 실패했지만 실패 Step이 기록되지 않았습니다. 실행 로그와 XML 경로를 확인하세요.", "Review: Failed, but no failed step was recorded. Check the run log and XML route.")
                    : OpenVisionRecipeText.Local("판독: 실패 Step을 선택했습니다. 입력/출력 레이어와 파라미터를 XML/Step 탭에서 확인하세요.", "Review: Failed step selected. Check input/output layers and parameters in XML/Steps.");

            return new OpenVisionRecipeBatchSampleResultOption(
                display,
                detail,
                review,
                result.Success,
                result.FailedStep,
                result.SampleName,
                result.ReportPath);
        }

        public static OpenVisionRecipeBatchSampleResultOption CreateEmpty()
        {
            return new OpenVisionRecipeBatchSampleResultOption(
                OpenVisionRecipeText.Local("샘플 결과 없음", "No sample results."),
                OpenVisionRecipeText.Local("쌍 검사 이력을 선택하세요.", "Select a pair check run."),
                OpenVisionRecipeText.Local("판독: 저장된 이력을 선택하세요.", "Review: Select a saved run."),
                true,
                string.Empty,
                string.Empty,
                string.Empty);
        }
    }

    public sealed class OpenVisionRecipeSampleOption
    {
        internal OpenVisionRecipeSampleOption(VisionPipelineSampleCatalogItem sample)
        {
            Sample = sample;
            SampleName = sample?.SampleName ?? string.Empty;
            PipelinePath = sample?.PipelineFullPath ?? string.Empty;
            DisplayText = FormatDisplayText(sample);
            DetailText = sample?.RecipeGuideText ?? string.Empty;
            AcceptanceSummaryText = FormatAcceptanceSummary(sample);
        }

        internal VisionPipelineSampleCatalogItem Sample { get; }

        public string SampleName { get; }

        public string PipelinePath { get; }

        public string DisplayText { get; }

        public string DetailText { get; }

        public string AcceptanceSummaryText { get; }

        private static string FormatDisplayText(VisionPipelineSampleCatalogItem sample)
        {
            if (sample == null)
            {
                return "-";
            }

            string source = string.IsNullOrWhiteSpace(sample.CatalogSourceId) ? "sample" : sample.CatalogSourceId;
            return "[" + source + "] " + sample.SampleName;
        }

        private static string FormatAcceptanceSummary(VisionPipelineSampleCatalogItem sample)
        {
            if (sample == null)
            {
                return "-";
            }

            List<string> lines = new List<string>
            {
                OpenVisionRecipeText.Local("샘플: ", "Sample: ") + Shorten(sample.SampleName, 48),
                OpenVisionRecipeText.Local("모드: ", "Mode: ") + (string.IsNullOrWhiteSpace(sample.ValidationMode) ? "-" : sample.ValidationMode.Trim()),
                OpenVisionRecipeText.Local("기대값: ", "Expected: ") + (string.IsNullOrWhiteSpace(sample.ExpectedText) ? "-" : sample.ExpectedText)
            };

            if (sample.HasPair)
            {
                lines.Add(OpenVisionRecipeText.Local("쌍: ", "Pair: ") + sample.PairText);
            }

            string checkGuide = sample.CheckGuideText;
            if (!string.IsNullOrWhiteSpace(checkGuide) && checkGuide != "-")
            {
                lines.Add(checkGuide);
            }

            string fixGuide = sample.FixGuideText;
            if (!string.IsNullOrWhiteSpace(fixGuide) && fixGuide != "-")
            {
                lines.Add(fixGuide);
            }

            return string.Join(Environment.NewLine, lines);
        }

        private static string Shorten(string value, int maxLength)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return "-";
            }

            string text = value.Trim();
            if (text.Length <= maxLength)
            {
                return text;
            }

            return text.Substring(0, Math.Max(1, maxLength - 3)) + "...";
        }
    }
}
