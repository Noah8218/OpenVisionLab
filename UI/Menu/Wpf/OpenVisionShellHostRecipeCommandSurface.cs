using Lib.OpenCV;
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
using static OpenVisionLab.OpenVisionRecipeLlmIntent;

namespace OpenVisionLab
{
    public sealed partial class OpenVisionShellHostRecipeCommandSurface : ObservableObject
    {
        private readonly Func<string> currentRecipeProvider;
        private readonly Action<string> switchRecipe;
        private readonly Action refreshAfterSwitch;
        private readonly Func<string, bool> confirmDeleteRecipe;
        private readonly Func<string, string, bool> confirmDeletePipeline;
        private readonly Func<string> selectImportPipelineXmlPath;
        private readonly Func<string, string> selectExportPipelineXmlPath;
        private readonly Func<string, string> selectExportReviewBundlePath;
        private readonly Func<string, IReadOnlyList<string>> selectValidationSetImagePaths;
        private readonly Func<string, string> selectValidationSetFolderPath;
        private readonly Func<string, string> selectValidationSetReplacementImagePath;
        private readonly Func<string, bool> confirmDeleteValidationSet;
        private readonly Action openLlmXmlReview;
        private readonly Action openPipelineReview;
        private readonly Func<string, OpenVisionRecipeLayerCard> layerCardProvider;
        private readonly Func<string, bool> navigateLayer;
        private readonly Func<string, string, bool> loadImageIntoLayer;
        private readonly Func<OpenVisionRecipeRunEvidence, bool> openSelectedBatchRunEvidence;
        private readonly Action openPinArrayGapValidationRuns;
        private readonly Action<VISION_MENU> selectStepTool;
        private readonly Func<bool> commitSelectedStepEdit;
        private readonly IReadOnlyList<string> llmToolTemplateOptions = new[]
        {
            OpenVisionGuidedSetupCatalog.PinGapTemplate,
            OpenVisionGuidedSetupCatalog.PinArrayGapTemplate,
            OpenVisionGuidedSetupCatalog.DarkBandGapTemplate,
            OpenVisionGuidedSetupCatalog.HybridRelativeRoiGapTemplate,
            "Line Measurement",
            OpenVisionGuidedSetupCatalog.MatchingTemplate,
            OpenVisionGuidedSetupCatalog.FeatureMatchingTemplate,
            OpenVisionGuidedSetupCatalog.EdgeBasedMatchingTemplate,
            OpenVisionGuidedSetupCatalog.ReferenceDifferenceTemplate,
            OpenVisionGuidedSetupCatalog.ContourTemplate,
            OpenVisionGuidedSetupCatalog.BlobTemplate,
            OpenVisionGuidedSetupCatalog.MeanTemplate
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
        private IReadOnlyList<OpenVisionRecipeValidationSetOption> validationSetOptions = Array.Empty<OpenVisionRecipeValidationSetOption>();
        private IReadOnlyList<OpenVisionRecipeValidationSetImageRow> validationSetImageRows = Array.Empty<OpenVisionRecipeValidationSetImageRow>();
        private IReadOnlyList<OpenVisionRecipeDependencyReviewRow> llmXmlDraftDependencyRows = Array.Empty<OpenVisionRecipeDependencyReviewRow>();
        private readonly IReadOnlyList<OpenVisionRecipeValidationSuiteScopeOption> validationSuiteScopeOptions = OpenVisionRecipeValidationSuiteScopeOption.CreateDefaults();
        private OpenVisionRecipeValidationSuiteScopeOption selectedValidationSuiteScopeOption;
        private OpenVisionRecipeBatchRunOption selectedRecentBatchRunOption;
        private OpenVisionRecipeBatchRunOption selectedBenchmarkBaselineRunOption;
        private OpenVisionRecipeBatchSampleResultOption selectedRecentBatchSampleResultOption;
        private OpenVisionRecipeBatchRunComparisonRow selectedRecentBatchRunComparisonRow;
        private OpenVisionRecipeSampleMatrixRow selectedSampleMatrixRow;
        private OpenVisionRecipeValidationSetOption selectedValidationSetOption;
        private OpenVisionRecipeValidationSetOption pinArrayGapTrainValidationSetOption;
        private OpenVisionRecipeValidationSetOption pinArrayGapValidationValidationSetOption;
        private OpenVisionRecipeValidationSetOption pinArrayGapTestValidationSetOption;
        private OpenVisionRecipeValidationSetImageRow selectedValidationSetImageRow;
        private OpenVisionRecipePipelineStepPreview selectedPipelinePreviewStep;
        private readonly OpenVisionRecipeStepEditSessionViewModel selectedStepEditSession =
            new OpenVisionRecipeStepEditSessionViewModel();
        private readonly OpenVisionRecipeExecutionSessionViewModel executionSession =
            new OpenVisionRecipeExecutionSessionViewModel();
        private string selectedRecipeName = string.Empty;
        private string recipeFilterText = string.Empty;
        private string pipelineFilterText = string.Empty;
        private bool showRecentBatchNgOnly;
        private bool showRecentBatchReviewQueueOnly;
        private string editRecipeName = string.Empty;
        private string pipelineEditName = string.Empty;
        private string selectedLlmToolTemplate = "Template Matching";
        private string llmInspectionGoalText = string.Empty;
        private string llmDetectionPointText = string.Empty;
        private string pinGapIntentRoiText = OpenVisionRecipePinGapIntentSkill.DefaultRoiSamplesText;
        private string darkBandGapIntentRoiText = OpenVisionRecipeDarkBandGapIntentSkill.DefaultRoiText;
        private string hybridReferencePoseText = string.Empty;
        private string hybridRelativeRoiText = string.Empty;
        private string hybridScoreMarginText = OpenVisionRecipeHybridRelativeRoiIntentSkill.DefaultScoreMargin.ToString(CultureInfo.InvariantCulture);
        private string hybridAngleMinimumText = OpenVisionRecipeHybridRelativeRoiIntentSkill.DefaultAngleMinimum.ToString(CultureInfo.InvariantCulture);
        private string hybridAngleMaximumText = OpenVisionRecipeHybridRelativeRoiIntentSkill.DefaultAngleMaximum.ToString(CultureInfo.InvariantCulture);
        private string hybridScaleRatioMinimumText = OpenVisionRecipeHybridRelativeRoiIntentSkill.DefaultScaleRatioMinimum.ToString(CultureInfo.InvariantCulture);
        private string hybridScaleRatioMaximumText = OpenVisionRecipeHybridRelativeRoiIntentSkill.DefaultScaleRatioMaximum.ToString(CultureInfo.InvariantCulture);
        private string hybridMinimumValidPixelRatioText = OpenVisionRecipeHybridRelativeRoiIntentSkill.DefaultMinimumValidPixelRatio.ToString(CultureInfo.InvariantCulture);
        private string pinGapIntentDistanceMinText = "0.40";
        private string pinGapIntentDistanceMaxText = "0.55";
        private string pinGapIntentRangeMaxText = "0.06";
        private string pinGapIntentScaleText = "0.006";
        private readonly IReadOnlyList<string> pinArrayGapPolarityOptions = new[]
        {
            OpenVisionRecipePinArrayGapIntentSkill.SupportedPinPolarity,
            "Bright"
        };
        private readonly IReadOnlyList<string> pinArrayGapMeasurementOptions = new[]
        {
            OpenVisionRecipePinArrayGapIntentSkill.SupportedMeasurementDefinition,
            "Center-to-center pitch"
        };
        private string pinArrayGapRoiText = string.Empty;
        private string pinArrayGapPolarityText = OpenVisionRecipePinArrayGapIntentSkill.SupportedPinPolarity;
        private string pinArrayGapMeasurementText = OpenVisionRecipePinArrayGapIntentSkill.SupportedMeasurementDefinition;
        private string pinArrayGapRangeMaxText = string.Empty;
        private string pinArrayGapDarkThresholdText = OpenVisionRecipePinArrayGapIntentSkill.DefaultDarkThreshold.ToString(CultureInfo.InvariantCulture);
        private string pinArrayGapMinDarkCoverageRatioText = OpenVisionRecipePinArrayGapIntentSkill.DefaultMinimumDarkCoverageRatio.ToString(CultureInfo.InvariantCulture);
        private string pinArrayGapMinPinWidthText = OpenVisionRecipePinArrayGapIntentSkill.DefaultMinimumPinWidth.ToString(CultureInfo.InvariantCulture);
        private string pinArrayGapMaxPinBreakWidthText = OpenVisionRecipePinArrayGapIntentSkill.DefaultMaximumPinBreakWidth.ToString(CultureInfo.InvariantCulture);
        private string pinArrayGapMinGapWidthText = OpenVisionRecipePinArrayGapIntentSkill.DefaultMinimumGapWidth.ToString(CultureInfo.InvariantCulture);
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
        private string matchingIntentSearchRoiText = "0,0,572,420";
        private string matchingIntentScoreMinText = "0.60";
        private string matchingIntentExpectedCountText = "1";
        private string featureMatchingIntentScoreMinText = "0.85";
        private string featureMatchingIntentRansacReprojThresholdText = "4";
        private string featureMatchingIntentAcceptanceScoreMinText = "80";
        private string edgeBasedIntentScoreMinText = "0.70";
        private string edgeBasedIntentSearchCountText = "1";
        private string edgeBasedIntentCannyLowText = "30";
        private string edgeBasedIntentCannyHighText = "90";
        private string edgeBasedIntentAcceptanceScoreMinText = "70";
        private string referenceDifferencePath2 = string.Empty;
        private string referenceDifferencePath3 = string.Empty;
        private string referenceDifferencePath4 = string.Empty;
        private string referenceDifferenceThresholdText = "35";
        private string referenceDifferenceMinimumAreaText = "80";
        private string referenceDifferenceMaximumAreaText = "20000";
        private string meanIntentRoiText = string.Empty;
        private string meanIntentTypeText = "Mean";
        private string meanIntentMinimumText = "185";
        private string meanIntentMaximumText = "220";
        private string llmPromptText = string.Empty;
        private string llmXmlDraftText = string.Empty;
        private string llmReferenceImagePath = string.Empty;
        private string llmXmlDraftValidationReport = string.Empty;
        private string llmXmlDraftDependencyReport = string.Empty;
        private string llmXmlDraftReviewReport = string.Empty;
        private string llmXmlDraftDiffReport = string.Empty;
        private string llmPromptCopyStatusText = string.Empty;
        private string llmBrowserAssistStatusText = string.Empty;
        private string llmReviewBundleCopyStatusText = string.Empty;
        private string llmXmlDraftPasteStatusText = string.Empty;
        private string operatorHandoffReportStatusText = string.Empty;
        private string selectedRecentBatchRunReviewCopyStatusText = string.Empty;
        private string pinArrayGapValidationStatusText = string.Empty;
        private bool isPinArrayGapValidationIdentityFrozen;
        private string newValidationSetName = "Local_Validation_Set";
        private string validationSetPendingNotes = string.Empty;
        private string statusText = string.Empty;
        private OpenVisionRecipeValidationSetDocument validationSetDocument = OpenVisionRecipeValidationSetStorage.CreateEmpty();
        private readonly OpenVisionRecipePipelineExchangeUseCase pipelineExchangeUseCase = new OpenVisionRecipePipelineExchangeUseCase();
        private readonly OpenVisionRecipePipelineLifecycleUseCase pipelineLifecycleUseCase = new OpenVisionRecipePipelineLifecycleUseCase();
        private readonly OpenVisionRecipeWorkspaceUseCase recipeWorkspaceUseCase = new OpenVisionRecipeWorkspaceUseCase();
        private bool validationSetStorageReady = true;
        private OpenVisionRecipeReviewBundleInspection loadedReviewBundleInspection;
        private bool llmXmlDraftImportReady;
        private bool isGuidedSetupDraftStale;
        private bool isRefreshingOptions;
        private bool isSelectingRecipe;
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
            Func<string, string> selectExportReviewBundlePath = null,
            Func<string, OpenVisionRecipeLayerCard> layerCardProvider = null,
            Func<string, bool> navigateLayer = null,
            Func<string, string, bool> loadImageIntoLayer = null,
            Action<VISION_MENU> selectStepTool = null,
            Func<bool> commitSelectedStepEdit = null,
            Action openLlmXmlReview = null,
            Func<string, IReadOnlyList<string>> selectValidationSetImagePaths = null,
            Func<string, string> selectValidationSetFolderPath = null,
            Func<string, string> selectValidationSetReplacementImagePath = null,
            Func<string, bool> confirmDeleteValidationSet = null,
            Action openPipelineReview = null,
            Func<OpenVisionRecipeRunEvidence, bool> openSelectedBatchRunEvidence = null,
            Action openPinArrayGapValidationRuns = null)
        {
            this.currentRecipeProvider = currentRecipeProvider ?? throw new ArgumentNullException(nameof(currentRecipeProvider));
            this.switchRecipe = switchRecipe ?? throw new ArgumentNullException(nameof(switchRecipe));
            this.refreshAfterSwitch = refreshAfterSwitch ?? throw new ArgumentNullException(nameof(refreshAfterSwitch));
            this.confirmDeleteRecipe = confirmDeleteRecipe ?? (_ => true);
            this.confirmDeletePipeline = confirmDeletePipeline ?? ((_, _) => true);
            this.selectImportPipelineXmlPath = selectImportPipelineXmlPath ?? (() => string.Empty);
            this.selectExportPipelineXmlPath = selectExportPipelineXmlPath ?? (_ => string.Empty);
            this.selectExportReviewBundlePath = selectExportReviewBundlePath ?? (_ => string.Empty);
            this.selectValidationSetImagePaths = selectValidationSetImagePaths ?? (_ => Array.Empty<string>());
            this.selectValidationSetFolderPath = selectValidationSetFolderPath ?? (_ => string.Empty);
            this.selectValidationSetReplacementImagePath = selectValidationSetReplacementImagePath ?? (_ => string.Empty);
            this.confirmDeleteValidationSet = confirmDeleteValidationSet ?? (_ => true);
            this.openLlmXmlReview = openLlmXmlReview ?? (() => { });
            this.openPipelineReview = openPipelineReview ?? (() => { });
            this.layerCardProvider = layerCardProvider ?? OpenVisionRecipeLayerCard.CreateMissing;
            this.navigateLayer = navigateLayer ?? (_ => false);
            this.loadImageIntoLayer = loadImageIntoLayer ?? ((_, _) => false);
            this.openSelectedBatchRunEvidence = openSelectedBatchRunEvidence ?? (_ => false);
            this.openPinArrayGapValidationRuns = openPinArrayGapValidationRuns ?? (() => { });
            this.selectStepTool = selectStepTool;
            this.commitSelectedStepEdit = commitSelectedStepEdit ?? (() => true);
            selectedStepEditSession.PropertyChanged += OnSelectedStepEditSessionPropertyChanged;
            executionSession.PropertyChanged += OnExecutionSessionPropertyChanged;
            selectedValidationSuiteScopeOption = validationSuiteScopeOptions.FirstOrDefault();
            executionSession.SetStatus(OpenVisionRecipeText.Local(
                "Suite 범위를 선택한 뒤 명시적으로 Run suite를 실행하세요.",
                "Select a suite scope, then run the explicit suite."));
            SetLlmXmlDraftDependencyPlaceholder(LocalText(
                "XML 초안을 붙여넣거나 로드한 뒤 검증을 실행하세요.",
                "Paste or load an XML draft, then run validation."));
            LlmBrowserAssistStatusText = CreateLlmBrowserAssistReadyText();

            CreateRecipeCommand = new RelayCommand(CreateRecipe);
            CreateNamedRecipeCommand = new RelayCommand(CreateNamedRecipe, CanCreateNamedRecipe);
            DuplicateRecipeCommand = new RelayCommand(DuplicateSelectedRecipe, CanDuplicateSelectedRecipe);
            RenameRecipeCommand = new RelayCommand(RenameSelectedRecipe, CanRenameSelectedRecipe);
            DeleteRecipeCommand = new RelayCommand(DeleteSelectedRecipe, CanDeleteSelectedRecipe);
            ImportPipelineXmlCommand = new RelayCommand(ImportPipelineXml, CanUseSelectedRecipe);
            ExportPipelineXmlCommand = new RelayCommand(ExportActivePipelineXml, CanUseSelectedRecipe);
            ExportRecipeReviewBundleCommand = new RelayCommand(ExportActivePipelineReviewBundle, CanUseSelectedRecipe);
            DuplicateFromSampleCommand = new RelayCommand(DuplicatePipelineFromSample, CanDuplicatePipelineFromSample);
            ActivatePipelineCommand = new RelayCommand(ActivateSelectedPipeline, CanUseSelectedPipeline);
            DuplicatePipelineCommand = new RelayCommand(DuplicateSelectedPipeline, CanUseSelectedPipeline);
            RenamePipelineCommand = new RelayCommand(RenameSelectedPipeline, CanRenameSelectedPipeline);
            DeletePipelineCommand = new RelayCommand(DeleteSelectedPipeline, CanDeleteSelectedPipeline);
            LoadLlmXmlDraftCommand = new RelayCommand(LoadLlmXmlDraft, CanUseSelectedRecipe);
            ValidateLlmXmlDraftCommand = new RelayCommand(ValidateLlmXmlDraft, CanUseLlmXmlDraft);
            ImportLlmXmlDraftCommand = new RelayCommand(ImportLlmXmlDraft, CanImportLlmXmlDraft);
            CopyLlmPromptCommand = new RelayCommand(CopyLlmPrompt, CanCopyLlmPrompt);
            CopyLlmReviewBundleCommand = new RelayCommand(CopyLlmReviewBundle, CanCopyLlmReviewBundle);
            PasteLlmXmlDraftFromClipboardCommand = new RelayCommand(PasteLlmXmlDraftFromClipboard);
            UseSelectedSampleReferenceCommand = new RelayCommand(UseSelectedSampleReference, CanUseSelectedSampleReference);
            SuggestPinGapIntentRoiSamplesCommand = new RelayCommand(SuggestPinGapIntentRoiSamples, CanSuggestPinGapIntentRoiSamples);
            RunSelectedSampleCheckCommand = new RelayCommand(RunSelectedSampleCheck, CanRunSelectedSampleCheck);
            RunSelectedSamplePairCheckCommand = new RelayCommand(RunSelectedSamplePairCheck, CanRunSelectedSamplePairCheck);
            RunCatalogBenchmarkCommand = new RelayCommand(RunCatalogBenchmark, CanRunCatalogBenchmark);
            RunValidationSuiteCommand = new RelayCommand(RunValidationSuite, CanRunValidationSuite);
            StopValidationSuiteCommand = new RelayCommand(RequestValidationSuiteStop, CanStopValidationSuite);
            CreateValidationSetCommand = new RelayCommand(CreateValidationSet, CanCreateValidationSet);
            DeleteValidationSetCommand = new RelayCommand(DeleteValidationSet, CanDeleteValidationSet);
            AddValidationSetOkImagesCommand = new RelayCommand(
                () => AddValidationSetImages(OpenVisionRecipeValidationSetImage.ExpectedOk),
                CanAddValidationSetImages);
            AddValidationSetNgImagesCommand = new RelayCommand(
                () => AddValidationSetImages(OpenVisionRecipeValidationSetImage.ExpectedNg),
                CanAddValidationSetImages);
            AddValidationSetOkFolderCommand = new RelayCommand(
                () => AddValidationSetFolder(OpenVisionRecipeValidationSetImage.ExpectedOk),
                CanAddValidationSetImages);
            AddValidationSetNgFolderCommand = new RelayCommand(
                () => AddValidationSetFolder(OpenVisionRecipeValidationSetImage.ExpectedNg),
                CanAddValidationSetImages);
            RepairValidationSetImagePathCommand = new RelayCommand(
                RepairValidationSetImagePath,
                CanRepairValidationSetImagePath);
            RemoveValidationSetImageCommand = new RelayCommand(RemoveValidationSetImage, CanRemoveValidationSetImage);
            SelectPairSampleResultCommand = new RelayCommand<OpenVisionRecipePairSampleRunSummary>(
                SelectPairSampleResult,
                CanSelectPairSampleResult);
            BuildLlmPromptCommand = new RelayCommand(BuildLlmPrompt, CanUseSelectedRecipe);
            CreateLlmTemplateXmlDraftCommand = new RelayCommand(CreateLlmTemplateXmlDraft, CanUseSelectedRecipe);
            CreateGuidedSetupStarterXmlCommand = new RelayCommand(CreateGuidedSetupStarterXml, CanCreateGuidedSetupStarterXml);
            CreatePinGapIntentXmlDraftCommand = new RelayCommand(CreatePinGapIntentXmlDraft, CanUseSelectedRecipe);
            CreateBlobCountIntentXmlDraftCommand = new RelayCommand(CreateBlobCountIntentXmlDraft, CanUseSelectedRecipe);
            CreateContourCountIntentXmlDraftCommand = new RelayCommand(CreateContourCountIntentXmlDraft, CanUseSelectedRecipe);
            RefreshLlmDraftReviewCommand = new RelayCommand(RefreshLlmDraftReview, CanUseLlmXmlDraft);
            NavigateSelectedStepInputLayerCommand = new RelayCommand(NavigateSelectedStepInputLayer, CanNavigateSelectedStepInputLayer);
            NavigateSelectedStepOutputLayerCommand = new RelayCommand(NavigateSelectedStepOutputLayer, CanNavigateSelectedStepOutputLayer);
            FocusSelectedRunFailureStepCommand = new RelayCommand(FocusSelectedRunFailureStep, CanFocusSelectedRunFailureStep);
            LoadSelectedRunSampleImageToInputLayerCommand = new RelayCommand(LoadSelectedRunSampleImageToInputLayer, CanLoadSelectedRunSampleImageToInputLayer);
            OpenSelectedRecentBatchRunEvidenceCommand = new RelayCommand(OpenSelectedRecentBatchRunEvidence, CanOpenSelectedRecentBatchRunEvidence);
            FreezePinArrayGapValidationIdentityCommand = new RelayCommand(
                FreezePinArrayGapValidationIdentity,
                CanFreezePinArrayGapValidationIdentity);
            OpenPinArrayGapValidationRunsCommand = new RelayCommand(
                OpenPinArrayGapValidationRuns,
                CanOpenPinArrayGapValidationRuns);
            SelectPreviousPipelinePreviewStepCommand = new RelayCommand(SelectPreviousPipelinePreviewStep, CanSelectPreviousPipelinePreviewStep);
            SelectNextPipelinePreviewStepCommand = new RelayCommand(SelectNextPipelinePreviewStep, CanSelectNextPipelinePreviewStep);
            OpenSelectedStepToolCommand = new RelayCommand(OpenSelectedStepTool, CanOpenSelectedStepTool);
            LoadSelectedStepParametersCommand = new RelayCommand(LoadSelectedStepParameters, CanLoadSelectedStepParameters);
            ApplySelectedStepParametersCommand = new RelayCommand(ApplySelectedStepParameters, CanApplySelectedStepParameters);
            CopyOperatorHandoffReportCommand = new RelayCommand(CopyOperatorHandoffReport, CanCopyOperatorHandoffReport);
            CopySelectedRecentBatchRunReviewCommand = new RelayCommand(CopySelectedRecentBatchRunReview, CanCopySelectedRecentBatchRunReview);
            RunRecipeGuidedNextActionCommand = new RelayCommand(RunRecipeGuidedNextAction, CanRunRecipeGuidedNextAction);
            OpenPipelineReviewCommand = new RelayCommand(this.openPipelineReview, CanUseSelectedRecipe);
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

        public IReadOnlyList<OpenVisionRecipeValidationSuiteScopeOption> ValidationSuiteScopeOptions => validationSuiteScopeOptions;

        public OpenVisionRecipeValidationSuiteScopeOption SelectedValidationSuiteScopeOption
        {
            get => selectedValidationSuiteScopeOption;
            set
            {
                if (SetProperty(ref selectedValidationSuiteScopeOption, value ?? validationSuiteScopeOptions.FirstOrDefault()))
                {
                    OnPropertyChanged(nameof(IsLocalValidationSetSelected));
                    OnPropertyChanged(nameof(ValidationSuiteSummaryText));
                    NotifyValidationSetEvidenceChanged();
                    RefreshCommandState();
                }
            }
        }

        public bool IsLocalValidationSetSelected => string.Equals(
            SelectedValidationSuiteScopeOption?.Key,
            OpenVisionRecipeValidationSuiteScopeOption.LocalValidationSetKey,
            StringComparison.OrdinalIgnoreCase);

        public IReadOnlyList<OpenVisionRecipeValidationSetOption> ValidationSetOptions
        {
            get => validationSetOptions;
            private set => SetProperty(ref validationSetOptions, value ?? Array.Empty<OpenVisionRecipeValidationSetOption>());
        }

        public OpenVisionRecipeValidationSetOption SelectedValidationSetOption
        {
            get => selectedValidationSetOption;
            set
            {
                if (SetProperty(ref selectedValidationSetOption, value))
                {
                    RefreshValidationSetImageRows();
                    OnPropertyChanged(nameof(ValidationSetSelectionSummaryText));
                    OnPropertyChanged(nameof(ValidationSuiteSummaryText));
                    NotifyValidationSetEvidenceChanged();
                    RefreshCommandState();
                }
            }
        }

        public OpenVisionRecipeValidationSetOption PinArrayGapTrainValidationSetOption
        {
            get => pinArrayGapTrainValidationSetOption;
            set
            {
                if (SetProperty(ref pinArrayGapTrainValidationSetOption, value))
                {
                    RefreshPinArrayGapValidationIdentityState();
                    RefreshCommandState();
                }
            }
        }

        public OpenVisionRecipeValidationSetOption PinArrayGapValidationValidationSetOption
        {
            get => pinArrayGapValidationValidationSetOption;
            set
            {
                if (SetProperty(ref pinArrayGapValidationValidationSetOption, value))
                {
                    RefreshPinArrayGapValidationIdentityState();
                    RefreshCommandState();
                }
            }
        }

        public OpenVisionRecipeValidationSetOption PinArrayGapTestValidationSetOption
        {
            get => pinArrayGapTestValidationSetOption;
            set
            {
                if (SetProperty(ref pinArrayGapTestValidationSetOption, value))
                {
                    RefreshPinArrayGapValidationIdentityState();
                    RefreshCommandState();
                }
            }
        }

        public string PinArrayGapValidationStatusText
        {
            get => string.IsNullOrWhiteSpace(pinArrayGapValidationStatusText)
                ? LocalText(
                    "2단계 미고정 | 판정용 PinArrayGap 파이프라인과 서로 겹치지 않는 Train/Validation/Test 세트를 선택하세요.",
                    "PHASE 2 NOT FROZEN | Select a judged PinArrayGap pipeline and disjoint Train/Validation/Test sets.")
                : pinArrayGapValidationStatusText;
            private set => SetProperty(ref pinArrayGapValidationStatusText, value ?? string.Empty);
        }

        public bool IsPinArrayGapValidationIdentityFrozen
        {
            get => isPinArrayGapValidationIdentityFrozen;
            private set => SetProperty(ref isPinArrayGapValidationIdentityFrozen, value);
        }

        public IReadOnlyList<OpenVisionRecipeValidationSetImageRow> ValidationSetImageRows
        {
            get => validationSetImageRows;
            private set => SetProperty(ref validationSetImageRows, value ?? Array.Empty<OpenVisionRecipeValidationSetImageRow>());
        }

        public OpenVisionRecipeValidationSetImageRow SelectedValidationSetImageRow
        {
            get => selectedValidationSetImageRow;
            set
            {
                if (SetProperty(ref selectedValidationSetImageRow, value))
                {
                    RefreshCommandState();
                }
            }
        }

        public string NewValidationSetName
        {
            get => newValidationSetName;
            set
            {
                if (SetProperty(ref newValidationSetName, value ?? string.Empty))
                {
                    RefreshCommandState();
                }
            }
        }

        public string ValidationSetPendingNotes
        {
            get => validationSetPendingNotes;
            set => SetProperty(ref validationSetPendingNotes, value ?? string.Empty);
        }

        public IReadOnlyList<OpenVisionRecipeBatchRunOption> RecentBatchRunOptions
        {
            get => recentBatchRunOptions;
            private set => SetProperty(ref recentBatchRunOptions, value ?? Array.Empty<OpenVisionRecipeBatchRunOption>());
        }

        public IReadOnlyList<OpenVisionRecipeBatchSampleResultOption> FilteredRecentBatchRunSampleResults =>
            OpenVisionRecipeRunHistoryPresenter.BuildFilteredSampleResults(
                SelectedRecentBatchRunOption,
                ShowRecentBatchNgOnly,
                ShowRecentBatchReviewQueueOnly);

        public bool ShowRecentBatchNgOnly
        {
            get => showRecentBatchNgOnly;
            set
            {
                if (SetProperty(ref showRecentBatchNgOnly, value))
                {
                    if (value && showRecentBatchReviewQueueOnly)
                    {
                        showRecentBatchReviewQueueOnly = false;
                        OnPropertyChanged(nameof(ShowRecentBatchReviewQueueOnly));
                    }

                    SelectedRecentBatchSampleResultOption = OpenVisionRecipeRunHistoryPresenter.SelectDefaultBatchSampleResult(
                        SelectedRecentBatchRunOption,
                        ShowRecentBatchNgOnly,
                        ShowRecentBatchReviewQueueOnly);
                    OnPropertyChanged(nameof(FilteredRecentBatchRunSampleResults));
                    OnPropertyChanged(nameof(RecentBatchRunNgOnlyText));
                    OnPropertyChanged(nameof(RecentBatchRunNgFilterSummaryText));
                    OnPropertyChanged(nameof(RecentBatchRunReviewQueueSummaryText));
                    OnPropertyChanged(nameof(SelectedRecentBatchRunReviewText));
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public bool ShowRecentBatchReviewQueueOnly
        {
            get => showRecentBatchReviewQueueOnly;
            set
            {
                if (SetProperty(ref showRecentBatchReviewQueueOnly, value))
                {
                    if (value && showRecentBatchNgOnly)
                    {
                        showRecentBatchNgOnly = false;
                        OnPropertyChanged(nameof(ShowRecentBatchNgOnly));
                    }

                    SelectedRecentBatchSampleResultOption = OpenVisionRecipeRunHistoryPresenter.SelectDefaultBatchSampleResult(
                        SelectedRecentBatchRunOption,
                        ShowRecentBatchNgOnly,
                        ShowRecentBatchReviewQueueOnly);
                    OnPropertyChanged(nameof(FilteredRecentBatchRunSampleResults));
                    OnPropertyChanged(nameof(RecentBatchRunNgFilterSummaryText));
                    OnPropertyChanged(nameof(RecentBatchRunReviewQueueSummaryText));
                    OnPropertyChanged(nameof(SelectedRecentBatchRunReviewText));
                    CommandManager.InvalidateRequerySuggested();
                }
            }
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
                    OnPropertyChanged(nameof(OperatorDecisionEvidenceText));
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
                    SelectedRecentBatchSampleResultOption = OpenVisionRecipeRunHistoryPresenter.SelectDefaultBatchSampleResult(
                        value,
                        ShowRecentBatchNgOnly,
                        ShowRecentBatchReviewQueueOnly);
                    RefreshBenchmarkBaselineRunOptions();
                    RefreshRecentBatchRunComparison();
                    SelectedRecentBatchRunReviewCopyStatusText = string.Empty;
                    OnPropertyChanged(nameof(FilteredRecentBatchRunSampleResults));
                    OnPropertyChanged(nameof(RecentBatchRunNgFilterSummaryText));
                    OnPropertyChanged(nameof(RecentBatchRunReviewQueueSummaryText));
                    OnPropertyChanged(nameof(SelectedRecentBatchRunReviewText));
                    OnPropertyChanged(nameof(OperatorDecisionEvidenceText));
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
            get => selectedStepEditSession.EditObject;
        }

        public bool HasSelectedStepEditObject => SelectedStepEditObject != null;

        public string SelectedStepEditStatusText =>
            string.IsNullOrWhiteSpace(selectedStepEditSession.StatusText)
                ? LocalText("Step 파라미터를 불러온 뒤 PropertyGrid에서 검토하고 XML 반영을 누르세요.", "Load step parameters, review them in the PropertyGrid, then apply to XML.")
                : selectedStepEditSession.StatusText;

        public bool IsSelectedStepEditDirty => selectedStepEditSession.IsDirty;

        public IReadOnlyList<string> LlmToolTemplateOptions => llmToolTemplateOptions;

        public IReadOnlyList<string> PinArrayGapPolarityOptions => pinArrayGapPolarityOptions;

        public IReadOnlyList<string> PinArrayGapMeasurementOptions => pinArrayGapMeasurementOptions;

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
                    llmXmlDraftImportReady = false;
                    OnPropertyChanged(nameof(GuidedSetupReadinessText));
                    OnPropertyChanged(nameof(LlmResultChannelContractSummaryText));
                    RefreshPinArrayGapValidationIdentityState();
                    NotifyGuidedSetupIntentInputChanged();
                    RefreshCommandState();
                }
            }
        }

        internal bool SelectGuidedSetupForTool(VISION_MENU menu)
        {
            if (!OpenVisionGuidedSetupCatalog.TryResolveTemplate(menu, out string template))
            {
                return false;
            }

            SelectedLlmToolTemplate = template;
            return true;
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
                    OnPropertyChanged(nameof(PinGapIntentCalibrationReviewText));
                    OnPropertyChanged(nameof(PinGapIntentFeedbackText));
                    OnPropertyChanged(nameof(PinGapIntentLatestRunText));
                    NotifyGuidedSetupIntentInputChanged();
                    RefreshCommandState();
                }
            }
        }

        public string DarkBandGapIntentRoiText
        {
            get => darkBandGapIntentRoiText;
            set
            {
                if (SetProperty(ref darkBandGapIntentRoiText, value ?? string.Empty))
                {
                    NotifyGuidedSetupIntentInputChanged();
                    RefreshCommandState();
                }
            }
        }

        public string HybridReferencePoseText
        {
            get => hybridReferencePoseText;
            set
            {
                if (SetProperty(ref hybridReferencePoseText, value ?? string.Empty))
                {
                    NotifyHybridRelativeRoiIntentTextChanged();
                }
            }
        }

        public string HybridRelativeRoiText
        {
            get => hybridRelativeRoiText;
            set
            {
                if (SetProperty(ref hybridRelativeRoiText, value ?? string.Empty))
                {
                    NotifyHybridRelativeRoiIntentTextChanged();
                }
            }
        }

        public string HybridScoreMarginText
        {
            get => hybridScoreMarginText;
            set
            {
                if (SetProperty(ref hybridScoreMarginText, value ?? string.Empty))
                {
                    NotifyHybridRelativeRoiIntentTextChanged();
                }
            }
        }

        public string HybridAngleMinimumText
        {
            get => hybridAngleMinimumText;
            set
            {
                if (SetProperty(ref hybridAngleMinimumText, value ?? string.Empty))
                {
                    NotifyHybridRelativeRoiIntentTextChanged();
                }
            }
        }

        public string HybridAngleMaximumText
        {
            get => hybridAngleMaximumText;
            set
            {
                if (SetProperty(ref hybridAngleMaximumText, value ?? string.Empty))
                {
                    NotifyHybridRelativeRoiIntentTextChanged();
                }
            }
        }

        public string HybridScaleRatioMinimumText
        {
            get => hybridScaleRatioMinimumText;
            set
            {
                if (SetProperty(ref hybridScaleRatioMinimumText, value ?? string.Empty))
                {
                    NotifyHybridRelativeRoiIntentTextChanged();
                }
            }
        }

        public string HybridScaleRatioMaximumText
        {
            get => hybridScaleRatioMaximumText;
            set
            {
                if (SetProperty(ref hybridScaleRatioMaximumText, value ?? string.Empty))
                {
                    NotifyHybridRelativeRoiIntentTextChanged();
                }
            }
        }

        public string HybridMinimumValidPixelRatioText
        {
            get => hybridMinimumValidPixelRatioText;
            set
            {
                if (SetProperty(ref hybridMinimumValidPixelRatioText, value ?? string.Empty))
                {
                    NotifyHybridRelativeRoiIntentTextChanged();
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
                    OnPropertyChanged(nameof(PinGapIntentCalibrationReviewText));
                    OnPropertyChanged(nameof(PinGapIntentFeedbackText));
                    OnPropertyChanged(nameof(PinGapIntentLatestRunText));
                    NotifyGuidedSetupIntentInputChanged();
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
                    OnPropertyChanged(nameof(PinGapIntentCalibrationReviewText));
                    OnPropertyChanged(nameof(PinGapIntentFeedbackText));
                    OnPropertyChanged(nameof(PinGapIntentLatestRunText));
                    NotifyGuidedSetupIntentInputChanged();
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
                    OnPropertyChanged(nameof(PinGapIntentCalibrationReviewText));
                    OnPropertyChanged(nameof(PinGapIntentFeedbackText));
                    OnPropertyChanged(nameof(PinGapIntentLatestRunText));
                    NotifyGuidedSetupIntentInputChanged();
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
                    OnPropertyChanged(nameof(PinGapIntentDistanceMinLabelText));
                    OnPropertyChanged(nameof(PinGapIntentDistanceMaxLabelText));
                    OnPropertyChanged(nameof(PinGapIntentWorkflowText));
                    OnPropertyChanged(nameof(PinGapIntentCalibrationReviewText));
                    OnPropertyChanged(nameof(PinGapIntentFeedbackText));
                    OnPropertyChanged(nameof(PinGapIntentLatestRunText));
                    NotifyGuidedSetupIntentInputChanged();
                    RefreshCommandState();
                }
            }
        }

        public string PinArrayGapRoiText
        {
            get => pinArrayGapRoiText;
            set
            {
                if (SetProperty(ref pinArrayGapRoiText, value ?? string.Empty))
                {
                    NotifyPinArrayGapIntentTextChanged();
                }
            }
        }

        public string PinArrayGapPolarityText
        {
            get => pinArrayGapPolarityText;
            set
            {
                if (SetProperty(ref pinArrayGapPolarityText, value ?? string.Empty))
                {
                    NotifyPinArrayGapIntentTextChanged();
                }
            }
        }

        public string PinArrayGapMeasurementText
        {
            get => pinArrayGapMeasurementText;
            set
            {
                if (SetProperty(ref pinArrayGapMeasurementText, value ?? string.Empty))
                {
                    NotifyPinArrayGapIntentTextChanged();
                }
            }
        }

        public string PinArrayGapRangeMaxText
        {
            get => pinArrayGapRangeMaxText;
            set
            {
                if (SetProperty(ref pinArrayGapRangeMaxText, value ?? string.Empty))
                {
                    NotifyPinArrayGapIntentTextChanged();
                }
            }
        }

        public string PinArrayGapDarkThresholdText
        {
            get => pinArrayGapDarkThresholdText;
            set
            {
                if (SetProperty(ref pinArrayGapDarkThresholdText, value ?? string.Empty))
                {
                    NotifyPinArrayGapIntentTextChanged();
                }
            }
        }

        public string PinArrayGapMinDarkCoverageRatioText
        {
            get => pinArrayGapMinDarkCoverageRatioText;
            set
            {
                if (SetProperty(ref pinArrayGapMinDarkCoverageRatioText, value ?? string.Empty))
                {
                    NotifyPinArrayGapIntentTextChanged();
                }
            }
        }

        public string PinArrayGapMinPinWidthText
        {
            get => pinArrayGapMinPinWidthText;
            set
            {
                if (SetProperty(ref pinArrayGapMinPinWidthText, value ?? string.Empty))
                {
                    NotifyPinArrayGapIntentTextChanged();
                }
            }
        }

        public string PinArrayGapMaxPinBreakWidthText
        {
            get => pinArrayGapMaxPinBreakWidthText;
            set
            {
                if (SetProperty(ref pinArrayGapMaxPinBreakWidthText, value ?? string.Empty))
                {
                    NotifyPinArrayGapIntentTextChanged();
                }
            }
        }

        public string PinArrayGapMinGapWidthText
        {
            get => pinArrayGapMinGapWidthText;
            set
            {
                if (SetProperty(ref pinArrayGapMinGapWidthText, value ?? string.Empty))
                {
                    NotifyPinArrayGapIntentTextChanged();
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

        public string MatchingIntentSearchRoiText
        {
            get => matchingIntentSearchRoiText;
            set
            {
                if (SetProperty(ref matchingIntentSearchRoiText, value ?? string.Empty))
                {
                    NotifyMatchingIntentTextChanged();
                }
            }
        }

        public string MatchingIntentScoreMinText
        {
            get => matchingIntentScoreMinText;
            set
            {
                if (SetProperty(ref matchingIntentScoreMinText, value ?? string.Empty))
                {
                    NotifyMatchingIntentTextChanged();
                }
            }
        }

        public string MatchingIntentExpectedCountText
        {
            get => matchingIntentExpectedCountText;
            set
            {
                if (SetProperty(ref matchingIntentExpectedCountText, value ?? string.Empty))
                {
                    NotifyMatchingIntentTextChanged();
                }
            }
        }

        public string FeatureMatchingIntentScoreMinText
        {
            get => featureMatchingIntentScoreMinText;
            set
            {
                if (SetProperty(ref featureMatchingIntentScoreMinText, value ?? string.Empty))
                {
                    NotifyFeatureMatchingIntentTextChanged();
                }
            }
        }

        public string FeatureMatchingIntentRansacReprojThresholdText
        {
            get => featureMatchingIntentRansacReprojThresholdText;
            set
            {
                if (SetProperty(ref featureMatchingIntentRansacReprojThresholdText, value ?? string.Empty))
                {
                    NotifyFeatureMatchingIntentTextChanged();
                }
            }
        }

        public string FeatureMatchingIntentAcceptanceScoreMinText
        {
            get => featureMatchingIntentAcceptanceScoreMinText;
            set
            {
                if (SetProperty(ref featureMatchingIntentAcceptanceScoreMinText, value ?? string.Empty))
                {
                    NotifyFeatureMatchingIntentTextChanged();
                }
            }
        }

        public string EdgeBasedIntentScoreMinText
        {
            get => edgeBasedIntentScoreMinText;
            set
            {
                if (SetProperty(ref edgeBasedIntentScoreMinText, value ?? string.Empty))
                {
                    NotifyEdgeBasedIntentTextChanged();
                }
            }
        }

        public string EdgeBasedIntentSearchCountText
        {
            get => edgeBasedIntentSearchCountText;
            set
            {
                if (SetProperty(ref edgeBasedIntentSearchCountText, value ?? string.Empty))
                {
                    NotifyEdgeBasedIntentTextChanged();
                }
            }
        }

        public string EdgeBasedIntentCannyLowText
        {
            get => edgeBasedIntentCannyLowText;
            set
            {
                if (SetProperty(ref edgeBasedIntentCannyLowText, value ?? string.Empty))
                {
                    NotifyEdgeBasedIntentTextChanged();
                }
            }
        }

        public string EdgeBasedIntentCannyHighText
        {
            get => edgeBasedIntentCannyHighText;
            set
            {
                if (SetProperty(ref edgeBasedIntentCannyHighText, value ?? string.Empty))
                {
                    NotifyEdgeBasedIntentTextChanged();
                }
            }
        }

        public string EdgeBasedIntentAcceptanceScoreMinText
        {
            get => edgeBasedIntentAcceptanceScoreMinText;
            set
            {
                if (SetProperty(ref edgeBasedIntentAcceptanceScoreMinText, value ?? string.Empty))
                {
                    NotifyEdgeBasedIntentTextChanged();
                }
            }
        }

        public string ReferenceDifferencePath2
        {
            get => referenceDifferencePath2;
            set
            {
                if (SetProperty(ref referenceDifferencePath2, value ?? string.Empty))
                {
                    NotifyReferenceDifferenceIntentTextChanged();
                }
            }
        }

        public string ReferenceDifferencePath3
        {
            get => referenceDifferencePath3;
            set
            {
                if (SetProperty(ref referenceDifferencePath3, value ?? string.Empty))
                {
                    NotifyReferenceDifferenceIntentTextChanged();
                }
            }
        }

        public string ReferenceDifferencePath4
        {
            get => referenceDifferencePath4;
            set
            {
                if (SetProperty(ref referenceDifferencePath4, value ?? string.Empty))
                {
                    NotifyReferenceDifferenceIntentTextChanged();
                }
            }
        }

        public string ReferenceDifferenceThresholdText
        {
            get => referenceDifferenceThresholdText;
            set
            {
                if (SetProperty(ref referenceDifferenceThresholdText, value ?? string.Empty))
                {
                    NotifyReferenceDifferenceIntentTextChanged();
                }
            }
        }

        public string ReferenceDifferenceMinimumAreaText
        {
            get => referenceDifferenceMinimumAreaText;
            set
            {
                if (SetProperty(ref referenceDifferenceMinimumAreaText, value ?? string.Empty))
                {
                    NotifyReferenceDifferenceIntentTextChanged();
                }
            }
        }

        public string ReferenceDifferenceMaximumAreaText
        {
            get => referenceDifferenceMaximumAreaText;
            set
            {
                if (SetProperty(ref referenceDifferenceMaximumAreaText, value ?? string.Empty))
                {
                    NotifyReferenceDifferenceIntentTextChanged();
                }
            }
        }

        public IReadOnlyList<string> MeanIntentTypeOptions => OpenVisionRecipeMeanIntentSkill.MeanTypeOptions;

        public string MeanIntentRoiText
        {
            get => meanIntentRoiText;
            set
            {
                if (SetProperty(ref meanIntentRoiText, value ?? string.Empty))
                {
                    NotifyMeanIntentTextChanged();
                }
            }
        }

        public string MeanIntentTypeText
        {
            get => meanIntentTypeText;
            set
            {
                if (SetProperty(ref meanIntentTypeText, value ?? string.Empty))
                {
                    NotifyMeanIntentTextChanged();
                }
            }
        }

        public string MeanIntentMinimumText
        {
            get => meanIntentMinimumText;
            set
            {
                if (SetProperty(ref meanIntentMinimumText, value ?? string.Empty))
                {
                    NotifyMeanIntentTextChanged();
                }
            }
        }

        public string MeanIntentMaximumText
        {
            get => meanIntentMaximumText;
            set
            {
                if (SetProperty(ref meanIntentMaximumText, value ?? string.Empty))
                {
                    NotifyMeanIntentTextChanged();
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
                    llmXmlDraftImportReady = false;
                    ClearLoadedReviewBundleContext();
                    RefreshCommandState();
                }
            }
        }

        public string LlmReferenceImagePath
        {
            get => llmReferenceImagePath;
            set
            {
                if (SetProperty(ref llmReferenceImagePath, value ?? string.Empty))
                {
                    NotifyGuidedSetupIntentInputChanged();
                    RefreshCommandState();
                }
            }
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
                    OnPropertyChanged(nameof(ValidationSuiteSummaryText));
                    OnPropertyChanged(nameof(RecipeGuidedSetupText));
                    NotifyGuidedSetupIntentInputChanged();
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
                    OnPropertyChanged(nameof(HasCurrentRecipeSampleExecution));
                    OnPropertyChanged(nameof(RecipeOverviewLastResultValueText));
                    OnPropertyChanged(nameof(RecipeOverviewLastResultToolTipText));
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
                    OnPropertyChanged(nameof(HasCurrentRecipeSampleExecution));
                    OnPropertyChanged(nameof(RecipeOverviewLastResultValueText));
                    OnPropertyChanged(nameof(RecipeOverviewLastResultToolTipText));
                    NotifyOperatorReviewChanged();
                    OnPropertyChanged(nameof(RecipeGuidedSetupText));
                    OnPropertyChanged(nameof(PinGapIntentLatestRunText));
                    OnPropertyChanged(nameof(BlobCountIntentLatestRunText));
                    OnPropertyChanged(nameof(ContourCountIntentLatestRunText));
                    OnPropertyChanged(nameof(ValidationSuiteSummaryText));
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
                    SelectedPairSampleResult = OpenVisionRecipeRunHistoryPresenter.SelectDefaultPairSampleResult(latestPairRunSummary);
                    RefreshSampleMatrixRows();
                    NotifyOperatorReviewChanged();
                    OnPropertyChanged(nameof(FailureReviewText));
                    OnPropertyChanged(nameof(PipelineSelectedStepOperatorContextText));
                    OnPropertyChanged(nameof(RecipeGuidedSetupText));
                    OnPropertyChanged(nameof(ValidationSuiteSummaryText));
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
                    OnPropertyChanged(nameof(ValidationSuiteSummaryText));
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

        public ICommand CreateRecipeCommand { get; private set; }

        public ICommand CreateNamedRecipeCommand { get; private set; }

        public ICommand DuplicateRecipeCommand { get; private set; }

        public ICommand RenameRecipeCommand { get; private set; }

        public ICommand DeleteRecipeCommand { get; private set; }

        public ICommand ImportPipelineXmlCommand { get; private set; }

        public ICommand ExportPipelineXmlCommand { get; private set; }

        public ICommand ExportRecipeReviewBundleCommand { get; private set; }

        public ICommand DuplicateFromSampleCommand { get; private set; }

        public ICommand ActivatePipelineCommand { get; private set; }

        public ICommand DuplicatePipelineCommand { get; private set; }

        public ICommand RenamePipelineCommand { get; private set; }

        public ICommand DeletePipelineCommand { get; private set; }

        public ICommand LoadLlmXmlDraftCommand { get; private set; }

        public ICommand ValidateLlmXmlDraftCommand { get; private set; }

        public ICommand ImportLlmXmlDraftCommand { get; private set; }

        public ICommand CopyLlmPromptCommand { get; private set; }

        public ICommand CopyLlmReviewBundleCommand { get; private set; }

        public ICommand PasteLlmXmlDraftFromClipboardCommand { get; private set; }

        public ICommand UseSelectedSampleReferenceCommand { get; private set; }

        public ICommand SuggestPinGapIntentRoiSamplesCommand { get; private set; }

        public ICommand RunSelectedSampleCheckCommand { get; private set; }

        public ICommand RunSelectedSamplePairCheckCommand { get; private set; }

        public ICommand RunCatalogBenchmarkCommand { get; private set; }

        public ICommand RunValidationSuiteCommand { get; private set; }

        public ICommand StopValidationSuiteCommand { get; private set; }

        public ICommand CreateValidationSetCommand { get; private set; }

        public ICommand DeleteValidationSetCommand { get; private set; }

        public ICommand AddValidationSetOkImagesCommand { get; private set; }

        public ICommand AddValidationSetNgImagesCommand { get; private set; }

        public ICommand AddValidationSetOkFolderCommand { get; private set; }

        public ICommand AddValidationSetNgFolderCommand { get; private set; }

        public ICommand RepairValidationSetImagePathCommand { get; private set; }

        public ICommand RemoveValidationSetImageCommand { get; private set; }

        public ICommand SelectPairSampleResultCommand { get; private set; }

        public ICommand BuildLlmPromptCommand { get; private set; }

        public ICommand CreateLlmTemplateXmlDraftCommand { get; private set; }

        public ICommand CreateGuidedSetupStarterXmlCommand { get; private set; }

        public ICommand CreatePinGapIntentXmlDraftCommand { get; private set; }

        public ICommand CreateBlobCountIntentXmlDraftCommand { get; private set; }

        public ICommand CreateContourCountIntentXmlDraftCommand { get; private set; }

        public ICommand RefreshLlmDraftReviewCommand { get; private set; }

        public ICommand NavigateSelectedStepInputLayerCommand { get; private set; }

        public ICommand NavigateSelectedStepOutputLayerCommand { get; private set; }

        public ICommand FocusSelectedRunFailureStepCommand { get; private set; }

        public ICommand LoadSelectedRunSampleImageToInputLayerCommand { get; private set; }

        public ICommand OpenSelectedRecentBatchRunEvidenceCommand { get; private set; }

        public ICommand FreezePinArrayGapValidationIdentityCommand { get; private set; }

        public ICommand OpenPinArrayGapValidationRunsCommand { get; private set; }

        public ICommand SelectPreviousPipelinePreviewStepCommand { get; private set; }

        public ICommand SelectNextPipelinePreviewStepCommand { get; private set; }

        public ICommand OpenSelectedStepToolCommand { get; private set; }

        public ICommand LoadSelectedStepParametersCommand { get; private set; }

        public ICommand ApplySelectedStepParametersCommand { get; private set; }

        public ICommand CopyOperatorHandoffReportCommand { get; private set; }

        public ICommand CopySelectedRecentBatchRunReviewCommand { get; private set; }

        public ICommand RunRecipeGuidedNextActionCommand { get; private set; }

        public ICommand OpenPipelineReviewCommand { get; private set; }

        public string NewRecipeButtonText => LocalText("새 레시피", "New recipe");

        public string RecipeSelectorToolTipText => LocalText("레시피 선택 / 전환", "Select or switch recipe");

        public string ManagerButtonText => LocalText("레시피 관리", "Manage recipes");

        public string ManagerButtonShortText => LocalText("관리", "Manage");

        public string ManagerTitleText => LocalText("레시피 관리", "Recipe manager");

        public string RecipeOverviewTabText => LocalText("요약", "Summary");

        public string RecipeAdvancedReviewText => LocalText("고급 검토", "Advanced review");

        public string RecipeReturnToSummaryText => LocalText("요약으로 돌아가기", "Back to summary");

        public string RecipeTechnicalReviewText => LocalText("선택 레시피 기술 검토", "Selected recipe technical review");

        public string RecipeOverviewTitleText => LocalText("선택한 레시피", "Selected recipe");

        public string RecipeOverviewPipelineText => LocalText("파이프라인 구성", "Pipeline");

        public string RecipeOverviewValidationText => LocalText("검증 상태", "Validation");

        public string RecipeOverviewSelectedSampleText => LocalText("현재 작업 샘플", "Current work sample");

        public string RecipeOverviewSelectedSampleContextText => LocalText(
            "샘플 검사 실행 후 이 레시피의 결과로 표시됩니다.",
            "Run a sample check to show its result for this recipe.");

        public string RecipeOverviewLastResultText => LocalText("현재 레시피 검사 결과", "Current recipe check result");

        public bool HasCurrentRecipeSampleExecution =>
            LatestSampleRunSummary?.IsForRecipePipeline(
                SelectedRecipeSummary?.RecipeName,
                SelectedRecipeSummary?.PreviewPipelineName) == true;

        public string RecipeOverviewLastResultValueText => HasCurrentRecipeSampleExecution
            ? LatestSampleRunSummary.CompactText
            : LocalText("아직 검사하지 않음", "Not checked yet");

        public string RecipeOverviewLastResultToolTipText => HasCurrentRecipeSampleExecution
            ? LatestSampleRunSummary.DisplayText
            : LocalText(
                "현재 선택한 레시피와 파이프라인으로 샘플 검사를 실행하면 결과가 표시됩니다.",
                "Run a sample check with the selected recipe and pipeline to show its result here.");

        public string OpenPipelineReviewText => LocalText("다음: 파이프라인 열기", "Next: Open Pipeline");

        public string OpenImageListValidationText => LocalText("이미지 목록 검증", "Image list validation");

        public string OpenImageListValidationToolTipText => LocalText(
            "저장한 파이프라인으로 OK/NG 이미지 목록을 순차 검증하는 화면을 엽니다. 열기만 하며 실행하지 않습니다.",
            "Opens sequential OK/NG image-list validation for the saved Pipeline. Opening does not run it.");

        public string ManagerWorkbenchText => LocalText("라이브러리", "Library");

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

        public string ReviewWorkspaceText => LocalText("선택 레시피", "Selected recipe");

        public string RecipeGuidedSetupText =>
            OpenVisionRecipeGuidedWorkflowPresenter.BuildSetupText(
                SelectedRecipeSummary,
                LatestSampleRunSummary,
                LatestPairRunSummary,
                SelectedSampleOption?.Sample != null);

        public string RecipeGuidedNextActionText =>
            OpenVisionRecipeGuidedWorkflowPresenter.BuildNextActionText(CreateRecipeGuidedWorkflowActionRequest());

        public string RecipeFilterLabelText => LocalText("검색", "Search");

        public string EditRecipeNameLabelText => LocalText("선택/새 이름", "Selected/new name");

        public string CreateNamedRecipeText => LocalText("새로 만들기", "Create");

        public string DuplicateRecipeText => LocalText("복제", "Duplicate");

        public string RenameRecipeText => LocalText("이름 변경", "Rename");

        public string DeleteRecipeText => LocalText("삭제", "Delete");

        public string ImportPipelineXmlText => LocalText("XML 가져오기", "Import XML");

        public string ExportPipelineXmlText => LocalText("XML 내보내기", "Export XML");

        public string ExportRecipeReviewBundleText => LocalText("검토 묶음", "Review bundle");

        public string ExportRecipeReviewBundleToolTipText => LocalText(
            "XML과 검토 manifest만 내보냅니다. 참조 파일 복사, Import, Preview, Run은 실행하지 않습니다.",
            "Exports XML and a review manifest only. It does not copy referenced files or run Import, Preview, or Run.");

        public string RecipeDetailText => LocalText("레시피 상세", "Recipe details");

        public string RecipePipelineTabText => LocalText("파이프라인 검토", "Pipeline review");

        public string RecipeGuidedSetupTabText => LocalText("검사 만들기", "Build inspection");

        public string RecipeLlmXmlTabText => LocalText("LLM XML", "LLM XML");

        public string RecipeLlmBrowserAssistTabText => LocalText("웹 보조", "Web assist");

        public string RecipePreviewTabText => LocalText("단계 미리보기", "Step preview");

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

        public string SampleMatrixSummaryText =>
            OpenVisionRecipeSampleMatrixPresenter.BuildSummaryText(
                SampleMatrixRows,
                SelectedSampleOption?.Sample);

        public string SelectedSampleMatrixReviewText =>
            SelectedSampleMatrixRow?.ReviewText
            ?? LocalText("샘플 매트릭스 행을 선택하면 기대 기준, 현재 결과, 다음 조치가 표시됩니다.", "Select a sample matrix row to see its expected gate, current result, and next action.");

        public string RecentBatchRunsText => LocalText("최근 쌍 검사 이력", "Recent pair check runs");

        public string RecentBatchRunSampleResultsText => LocalText("선택 이력 샘플 결과", "Selected run sample results");

        public string RecentBatchRunNgOnlyText => SelectedRecentBatchRunOption?.IsJudgmentSuite == true
            ? LocalText("오판만 보기", "Misclassified only")
            : LocalText("NG만 보기", "NG only");

        public string RecentBatchRunNgFilterSummaryText =>
            OpenVisionRecipeRunHistoryPresenter.BuildNgFilterSummaryText(
                SelectedRecentBatchRunOption,
                ShowRecentBatchNgOnly);

        public string RecentBatchRunReviewQueueOnlyText => LocalText("검토 큐만", "Review queue");

        public string RecentBatchRunReviewQueueSummaryText =>
            OpenVisionRecipeRunHistoryPresenter.BuildReviewQueueSummaryText(SelectedRecentBatchRunOption);

        public string RecentBatchRunComparisonText => LocalText("Benchmark 회귀 비교", "Benchmark regression diff");

        public string RecentBatchRunStepTimingText => LocalText("Step 병목", "Step bottlenecks");

        public string BenchmarkBaselineRunText => LocalText("기준 실행", "Baseline run");

        public string RecentBatchRunComparisonSummaryText =>
            OpenVisionRecipeRunHistoryPresenter.BuildComparisonSummaryText(
                SelectedRecentBatchRunOption,
                SelectedBenchmarkBaselineRunOption,
                OpenVisionRecipeRunHistoryPresenter.ResolveBaselineRunOption(
                    SelectedBenchmarkBaselineRunOption,
                    SelectedRecentBatchRunOption,
                    RecentBatchRunOptions),
                RecentBatchRunComparisonRows);

        public string SelectedRecentBatchRunComparisonReviewText =>
            SelectedRecentBatchRunComparisonRow?.ReviewText
            ?? LocalText("비교 행을 선택하면 이전 실행 대비 변화와 다음 조치가 표시됩니다.", "Select a diff row to see the change from the previous run and next action.");

        public string SelectedRecentBatchRunReviewLabelText => LocalText("선택 이력 판독", "Selected run review");

        public string SelectedRecentBatchRunReviewText =>
            OpenVisionRecipeRunReviewPresenter.BuildSelectedBatchRunReviewText(
                SelectedRecentBatchRunOption,
                SelectedRecentBatchSampleResultOption,
                FindPipelinePreviewStep(SelectedRecentBatchSampleResultOption?.FailedStep));

        public string CopySelectedRecentBatchRunReviewText => LocalText("판독 복사", "Copy review");

        public string OpenSelectedRecentBatchRunEvidenceText => LocalText("도면 보기", "View drawing");

        public string CatalogBenchmarkText => LocalText("카탈로그 벤치마크", "Catalog benchmark");

        public string RunCatalogBenchmarkText =>
            executionSession.IsCatalogBenchmarkRunning ? LocalText("실행 중...", "Running...") : LocalText("전체 샘플 검사", "Run catalog");

        public string RunCatalogBenchmarkShortText =>
            executionSession.IsCatalogBenchmarkRunning ? LocalText("실행 중", "Running") : LocalText("카탈로그", "Catalog");

        public string CatalogBenchmarkSummaryText =>
            LatestCatalogBenchmarkSummary?.CompactText
            ?? OpenVisionRecipeCatalogBenchmarkSummary.Empty.CompactText;

        public string CatalogBenchmarkDetailText =>
            LatestCatalogBenchmarkSummary?.DetailText
            ?? OpenVisionRecipeCatalogBenchmarkSummary.Empty.DetailText;

        public string ValidationSuiteText => "Validation Suite";

        public string ValidationSuiteScopeLabelText => LocalText("범위", "Scope");

        public string RunValidationSuiteText =>
            executionSession.IsValidationSuiteRunning
                ? LocalText("실행 중...", "Running...")
                : IsLocalValidationSetSelected
                    ? LocalText("목록 검증 실행", "Run image list")
                    : LocalText("Suite 실행", "Run suite");

        public string StopValidationSuiteText => executionSession.StopRequested
            ? LocalText("중지 대기", "Stopping")
            : LocalText("실행 중지", "Stop");

        public bool IsLocalValidationSetRunning => executionSession.IsLocalValidationSetRunning;

        public string ValidationSuiteSummaryText =>
            OpenVisionRecipeValidationSetPresenter.BuildValidationSuiteSummaryText(
                selectedRecipeName,
                SelectedPipelineOption?.PipelineName,
                SelectedValidationSuiteScopeOption?.DisplayText,
                IsLocalValidationSetSelected,
                ValidationSetSelectionSummaryText,
                SelectedSampleOption?.SampleName,
                LatestSampleRunSummary?.CompactText,
                LatestPairRunSummary?.CompactText,
                LatestCatalogBenchmarkSummary?.CompactText);

        public string ValidationSetText => LocalText("로컬 검증 세트", "Local validation set");

        public string ValidationSetSelectionLabelText => LocalText("세트", "Set");

        public string NewValidationSetNameLabelText => LocalText("새 이름", "New name");

        public string CreateValidationSetText => LocalText("만들기", "Create");

        public string DeleteValidationSetText => LocalText("세트 삭제", "Delete set");

        public string ValidationSetPendingNotesLabelText => LocalText("추가 파일 메모", "New image notes");

        public string AddValidationSetOkImagesText => LocalText("OK 이미지 추가", "Add OK images");

        public string AddValidationSetNgImagesText => LocalText("NG 이미지 추가", "Add NG images");

        public string ValidationSetFolderBatchLabelText => LocalText("폴더 일괄", "Folder batch");

        public string AddValidationSetOkFolderText => LocalText("OK 폴더 불러오기", "Load OK folder");

        public string AddValidationSetNgFolderText => LocalText("NG 폴더 불러오기", "Load NG folder");

        public string AddValidationSetFolderToolTipText => LocalText(
            "선택한 폴더의 바로 아래 지원 이미지 파일만 추가합니다. 하위 폴더는 포함하지 않습니다.",
            "Adds supported images directly in the selected folder. Subfolders are excluded.");

        public string RepairValidationSetImagePathText => LocalText("누락 경로 복구", "Repair missing");

        public string RepairValidationSetImagePathToolTipText => LocalText(
            "선택한 누락 이미지 1건을 사용자가 지정한 새 이미지 파일로 연결합니다.",
            "Connects the selected missing image to one replacement image chosen by the operator.");

        public string RemoveValidationSetImageText => LocalText("선택 제거", "Remove selected");

        public string ValidationSetSelectionSummaryText =>
            OpenVisionRecipeValidationSetPresenter.BuildSelectionSummaryText(
                validationSetStorageReady,
                SelectedValidationSetOption,
                ValidationSetImageRows);

        public string ValidationSetEvidenceText => LocalText("검증 근거", "Validation evidence");

        public string ValidationSetExpectedLabelText => LocalText("기대 OK/NG", "Expected OK/NG");

        public string ValidationSetAcceptanceLabelText => LocalText("판정 기준", "Acceptance gate");

        public string ValidationSetCalibrationLabelText => LocalText("보정 적용", "Calibration");

        public string ValidationSetNextActionLabelText => LocalText("다음 작업", "Next action");

        public string ValidationSetExpectedText =>
            OpenVisionRecipeValidationSetPresenter.BuildExpectedText(
                validationSetStorageReady,
                SelectedValidationSetOption);

        public string ValidationSetAcceptanceText => BuildValidationSetAcceptanceText();

        public string ValidationSetCalibrationText => BuildValidationSetCalibrationText();

        public string ValidationSetNextActionText =>
            OpenVisionRecipeValidationSetPresenter.BuildNextActionText(
                executionSession.IsValidationSuiteRunning,
                validationSetStorageReady,
                SelectedValidationSetOption,
                SelectedPipelineOption != null);

        public string SelectedRecentBatchRunReviewCopyStatusText
        {
            get => selectedRecentBatchRunReviewCopyStatusText;
            private set => SetProperty(ref selectedRecentBatchRunReviewCopyStatusText, value ?? string.Empty);
        }

        public string ValidationSuiteStatusText
        {
            get => executionSession.StatusText;
            private set => executionSession.SetStatus(value);
        }

        public string RunSelectedSampleCheckText => executionSession.IsSampleCheckRunning ? LocalText("실행 중...", "Running...") : LocalText("검사 실행", "Run check");

        public string RunSelectedSamplePairCheckText => executionSession.IsPairCheckRunning ? LocalText("실행 중...", "Running...") : LocalText("쌍 검사", "Run pair");

        public string SelectedSampleAcceptanceSummaryText =>
            SelectedSampleOption?.AcceptanceSummaryText ?? LocalText("기대 지표 기준을 확인할 샘플을 선택하세요.", "Select a sample to review expected metric gates.");

        public string OperatorReviewText => LocalText("작업자 검토", "Operator review");

        public string PipelineVariantComparisonText => LocalText("파이프라인 변형 비교", "Pipeline variant comparison");

        public string PipelineVariantComparisonReport => BuildPipelineVariantComparisonReport();

        public string PipelineReviewTabText => LocalText("검토", "Review");

        public string PipelineReportTabText => LocalText("리포트", "Report");

        public string PipelineRunHistoryTabText => LocalText("이력", "Runs");

        public string PipelineXmlStepTabText => LocalText("XML/Step", "XML/Steps");

        public string OperatorRunReviewLabelText => LocalText("실행 판정 요약", "Run review summary");

        public string OperatorRunReviewText =>
            OpenVisionRecipeRunReviewPresenter.BuildOperatorRunReviewText(
                SelectedRecipeSummary,
                LatestSampleRunSummary,
                LatestPairRunSummary)
            + OpenVisionRecipeRunReviewPresenter.BuildSelectedPairRoleSuffix(SelectedPairSampleResult);

        public string OperatorDecisionBoardText => LocalText("작업자 판정 보드", "Operator decision board");

        public string OperatorValidationChecklistText => LocalText("검증 체크리스트", "Validation checklist");

        public IReadOnlyList<OpenVisionRecipeOperatorValidationRow> OperatorValidationChecklistRows => BuildOperatorDecisionPresentation().ValidationRows;

        public string OperatorResultChannelsText => LocalText("판정 출력 정의", "Judgement outputs");

        public IReadOnlyList<OpenVisionRecipeOperatorResultChannelRow> OperatorResultChannelRows => BuildOperatorDecisionPresentation().ResultChannels;

        public IReadOnlyList<OpenVisionRecipeOperatorResultChannelRow> OperatorResultChannelBoardRows => BuildOperatorDecisionPresentation().ResultChannels;

        public string OperatorDecisionXmlCardText => BuildOperatorDecisionPresentation().XmlCardText;

        public string OperatorDecisionSampleCardText => BuildOperatorDecisionPresentation().SampleCardText;

        public string OperatorDecisionPairCardText => BuildOperatorDecisionPresentation().PairCardText;

        public string OperatorDecisionSummaryStatusText => BuildOperatorDecisionPresentation().SummaryStatusText;

        public string OperatorDecisionNextActionText => BuildOperatorDecisionPresentation().NextActionText;

        public string OperatorDecisionEvidenceText => BuildOperatorDecisionPresentation().EvidenceText;

        public string OperatorHandoffReportText => BuildOperatorDecisionPresentation().HandoffReportText;

        public string CopyOperatorHandoffReportText => LocalText("리포트 복사", "Copy report");

        public string OperatorHandoffReportStatusText
        {
            get => operatorHandoffReportStatusText;
            private set => SetProperty(ref operatorHandoffReportStatusText, value ?? string.Empty);
        }

        public string FailureReviewLabelText => LocalText("실패 Step 재검사 / 비교", "Failed step rerun / comparison");

        public string FailureReviewText => OpenVisionRecipePipelineStepReviewPresenter.BuildFailureReviewText(
            SelectedPipelinePreviewStep,
            SelectedPairSampleResult,
            SelectedRecentBatchSampleResultOption);

        public string ViewFailureInputLayerText => LocalText("입력 보기", "View input");

        public string ViewFailureOutputLayerText => LocalText("출력 보기", "View output");

        public string FocusSelectedRunFailureStepText => LocalText("실패 Step", "Failed step");

        public string LoadSelectedRunSampleImageToInputLayerText => LocalText("샘플->입력", "Sample -> input");

        public string RerunFailurePairCheckText => LocalText("Good/Bad 재검사", "Rerun Good/Bad");

        public string LoadFailureStepParametersText => LocalText("파라미터 검토", "Review parameters");

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

        public string PipelineSelectedStepOperatorContextText => OpenVisionRecipePipelineStepReviewPresenter.BuildOperatorContext(
            SelectedPipelinePreviewStep,
            SelectedPairSampleResult,
            SelectedRecentBatchSampleResultOption,
            SelectedRecentBatchRunComparisonRow);

        public string PipelineStepFlowReviewText => OpenVisionRecipePipelineStepReviewPresenter.BuildStepFlowReview(
            SelectedRecipeSummary?.PipelinePreviewSteps ?? Array.Empty<OpenVisionRecipePipelineStepPreview>(),
            SelectedPipelinePreviewStep,
            GetPipelinePreviewStepByOffset(-1),
            GetPipelinePreviewStepByOffset(1));

        public string BranchOutputComparisonText => OpenVisionRecipePipelineStepReviewPresenter.BuildBranchOutputComparisonText(
            SelectedRecipeSummary?.PipelinePreviewSteps ?? Array.Empty<OpenVisionRecipePipelineStepPreview>(),
            SelectedPipelinePreviewStep);

        public IReadOnlyList<OpenVisionRecipeBranchOutputComparisonRow> BranchOutputComparisonRows =>
            OpenVisionRecipePipelineStepReviewPresenter.BuildBranchOutputComparisonRows(
                SelectedRecipeSummary?.PipelinePreviewSteps ?? Array.Empty<OpenVisionRecipePipelineStepPreview>(),
                SelectedPipelinePreviewStep);

        public string PreviousPipelineStepText => OpenVisionRecipePipelineStepReviewPresenter.BuildStepSlotText(
            GetPipelinePreviewStepByOffset(-1),
            LocalText("이전 Step 없음", "No previous step"));

        public string CurrentPipelineStepText => OpenVisionRecipePipelineStepReviewPresenter.BuildStepSlotText(
            SelectedPipelinePreviewStep,
            LocalText("선택 Step 없음", "No selected step"));

        public string NextPipelineStepText => OpenVisionRecipePipelineStepReviewPresenter.BuildStepSlotText(
            GetPipelinePreviewStepByOffset(1),
            LocalText("다음 Step 없음", "No next step"));

        public string SelectPreviousPipelineStepText => LocalText("이전", "Previous");

        public string SelectNextPipelineStepText => LocalText("다음", "Next");

        public string LoadSelectedStepParametersText => LocalText("파라미터 불러오기", "Load parameters");

        public string ApplySelectedStepParametersText => LocalText("XML 반영", "Apply to XML");

        public string CorrectedOutputReviewLabelText => LocalText("수정 출력 확인", "Corrected output review");

        public string CorrectedOutputReviewText =>
            string.IsNullOrWhiteSpace(selectedStepEditSession.CorrectedOutputReviewText)
                ? OpenVisionRecipePipelineStepReviewPresenter.BuildCorrectedOutputReviewText(
                    SelectedPipelinePreviewStep,
                    selectedStepEditSession.IsDirty,
                    SelectedStepEditObject)
                : selectedStepEditSession.CorrectedOutputReviewText;

        public string OpenSelectedStepToolText =>
            SelectedPipelinePreviewStep?.EditorActionText
            ?? LocalText("도구 열기", "Open tool");

        public string LlmAssistantText => LocalText("검사 만들기 / LLM 보조", "Guided setup / LLM assistant");

        public string GuidedSetupNoLlmText => LocalText(
            "LLM 없이도 검사 의도에서 XML 초안을 만들 수 있습니다. LLM은 선택 사항입니다.",
            "Create a draft XML from an inspection intent without an LLM. LLM assistance is optional.");

        public string GuidedSetupIntentLabelText => LocalText("검사 의도", "Inspection intent");

        public string GuidedSetupSelectedSampleLabelText => LocalText("선택 샘플", "Selected sample");

        public string GuidedSetupCreateStarterText => LocalText("초안 XML 만들기", "Create draft XML");

        public bool IsGuidedSetupDraftStale => isGuidedSetupDraftStale;

        public string GuidedSetupDraftLabelText => LocalText(
            "생성된 XML 초안 (LLM 선택 사항)",
            "Generated draft XML (LLM optional)")
            + (IsGuidedSetupDraftStale
                ? LocalText(
                    " · 설정이 변경되었습니다. XML 초안을 다시 만들어 주세요.",
                    " · Settings changed. Create the draft XML again.")
                : string.Empty);

        public string GuidedSetupNextText => LocalText(
            "다음: 생성 결과를 확인한 뒤 LLM XML 탭에서 XML을 검증하고 가져오세요.",
            "Next: review the generated draft, then validate and import the XML from the LLM XML tab.");

        public string GuidedSetupActionBoundaryText => LocalText(
            "초안 XML 만들기는 현재 검사 설정을 바탕으로 XML 초안을 생성합니다. 생성 후 XML을 검증하고 가져오세요.",
            "Create draft XML generates an XML draft from the current inspection settings. Validate and import it after creation.");

        private const string GuidedSetupStarterXmlNoAutoRunContract =
            "Starter XML creation only updates the draft; it does not create layers, import a recipe, Preview, or Run.";

        public string GuidedSetupSummaryText => LocalText(
            "검사 의도와 필요한 값을 입력한 뒤 XML 초안 또는 프롬프트를 만드세요. XML을 검증한 뒤 가져옵니다.",
            "Choose an inspection intent and required values, then create a draft XML or prompt. Validate the XML before importing it.");

        public string GuidedSetupReadinessText =>
            OpenVisionRecipeGuidedSetupReadinessPresenter.BuildReadinessText(SelectedLlmToolTemplate);

        public string OpenLlmGuidedSetupText => LocalText("검사 설정", "Set up inspection");

        public bool IsGuidedSetupIntentInputReady =>
            OpenVisionRecipeGuidedSetupReadinessPresenter.Evaluate(CreateGuidedSetupReadinessInput()).IsReady;

        public string GuidedSetupIntentInputStatusText =>
            OpenVisionRecipeGuidedSetupReadinessPresenter.Evaluate(CreateGuidedSetupReadinessInput()).Text;

        public string LlmToolTemplateText => LocalText("검사 의도", "Inspection intent");

        public string LlmInspectionGoalLabelText => LocalText("검사 목표", "Inspection goal");

        public string LlmDetectionPointLabelText => LocalText("검출 포인트", "Detection points");

        public string PinGapIntentSkillText => LocalText("핀 간격 skill", "Pin gap skill");

        public string PinGapIntentRoiLabelText => LocalText("ROI 샘플", "ROI samples");

        public string PinGapIntentDistanceMinLabelText => "Min " + PinGapIntentUnitText;

        public string PinGapIntentDistanceMaxLabelText => "Max " + PinGapIntentUnitText;

        public string PinGapIntentRangeMaxLabelText => LocalText("Range", "Range");

        public string PinGapIntentScaleLabelText => LocalText("mm/px", "mm/px");

        public string CreatePinGapIntentXmlText => LocalText("초안 XML 만들기", "Create draft XML");

        public string SuggestPinGapIntentRoiSamplesText => LocalText("샘플 ROI", "Sample ROI");

        public string DarkBandGapIntentSkillText => LocalText("검은 띠 Gap 측정", "Dark-band Gap measurement");

        public string DarkBandGapIntentRoiLabelText => "Coarse ROI";

        public string DarkBandGapIntentBoundaryText => LocalText(
            "PX 측정 전용 · 파란색=상단, 자홍색=하단, 빨간색=Gap · 공차/mm 판정 없음",
            "PX measurement only · blue=upper, magenta=lower, red=Gap · no tolerance/mm judgement");

        public string HybridRelativeRoiIntentSkillText => LocalText(
            "위치 보정 후 상대 ROI Gap 측정",
            "Locator-aligned relative-ROI Gap measurement");

        public string HybridLocatorTemplateLabelText => LocalText("Locator 템플릿", "Locator template");

        public string HybridSearchRoiLabelText => LocalText("검색 ROI", "Search ROI");

        public string HybridReferencePoseLabelText => LocalText("기준 자세", "Reference pose");

        public string HybridRelativeRoiLabelText => LocalText("검사 ROI", "Measurement ROI");

        public string HybridScoreMinimumLabelText => "SCORE_MIN";

        public string HybridScoreMarginLabelText => LocalText("점수 차", "Score margin");

        public string HybridAngleRangeLabelText => LocalText("각도 범위", "Angle range");

        public string HybridScaleRatioRangeLabelText => LocalText("배율 비율", "Scale ratio");

        public string HybridMinimumValidPixelRatioLabelText => LocalText("최소 유효 비율", "Min valid ratio");

        public string HybridRelativeRoiBoundaryText => LocalText(
            "Matching이 위치·각도·배율을 찾고 NormalizeImage가 기준 좌표로 보정한 뒤, 고정 검사 ROI에서 px Gap만 측정합니다. 위치검출 실패는 NG 판정이 아니라 검사 불가로 차단됩니다.",
            "Matching finds position/angle/scale, NormalizeImage restores reference coordinates, then the fixed measurement ROI reports px Gap only. Locator failure is inspection-unavailable, not an NG part judgement.");

        public string PinGapIntentWorkflowText =>
            LocalText("판정: ", "Gates: ")
            + PinGapIntentAverageMetricName
            + " "
            + PinGapIntentDistanceMinText
            + ".."
            + PinGapIntentDistanceMaxText
            + " "
            + PinGapIntentUnitText
            + ", "
            + PinGapIntentRangeMetricName
            + " <= "
            + PinGapIntentRangeMaxText
            + " "
            + PinGapIntentUnitText
            + LocalText(
                " / 기본: 전체 핀 배열 샘플 / 다음: Pin gap XML -> 검증 -> 가져오기 -> 샘플 실행",
                " / Default: whole pin-array samples / Next: Pin gap XML -> Validate -> Import -> run sample");

        public string PinGapIntentCalibrationReviewText =>
            OpenVisionRecipeIntentFeedbackPresenter.BuildPinGapCalibrationReviewText(
                IsPinGapPixelOnly,
                PinGapIntentDistanceMinText,
                PinGapIntentDistanceMaxText,
                PinGapIntentRangeMaxText,
                PinGapIntentScaleText);

        public string PinGapIntentFeedbackText =>
            LocalText(
                "Feedback: 표시 영역이 없으면 전체 핀 배열을 좌/중/우 샘플로 봅니다. 특정 두 핀만 보려면 ROI 샘플을 하나로 줄이세요. Avg NG는 기준값"
                    + (IsPinGapPixelOnly ? string.Empty : "/mm/px")
                    + ", Range NG/긴 선은 ROI/contrast/sampling을 조정합니다.",
                "Feedback: without a marked region, inspect whole-array left/center/right samples. Use one ROI only for a marked pair. Avg NG tunes the gate"
                    + (IsPinGapPixelOnly ? string.Empty : "/mm-per-pixel scale")
                    + "; Range NG/long lines tune ROI/contrast/sampling.");

        public string PinGapIntentLatestRunText =>
            OpenVisionRecipeIntentFeedbackPresenter.BuildPinGapLatestRunText(
                LatestSampleRunSummary,
                IsPinGapPixelOnly,
                PinGapIntentDistanceMinText,
                PinGapIntentDistanceMaxText,
                PinGapIntentRangeMaxText);

        public string PinArrayGapRoiLabelText => LocalText("행 ROI", "Row ROI(s)");

        public string PinArrayGapPolarityLabelText => LocalText("핀 극성", "Pin polarity");

        public string PinArrayGapMeasurementLabelText => LocalText("측정 정의", "Measurement");

        public string PinArrayGapRangeMaxLabelText => LocalText("Range 최대 px", "Range max px");

        public string PinArrayGapDarkThresholdLabelText => "DarkThreshold";

        public string PinArrayGapMinDarkCoverageRatioLabelText => "Min dark ratio";

        public string PinArrayGapMinPinWidthLabelText => "Min pin width";

        public string PinArrayGapMaxPinBreakWidthLabelText => "Max break width";

        public string PinArrayGapMinGapWidthLabelText => "Min gap width";

        public string PinArrayGapIntentContractText => LocalText(
            "각 ROI에는 어두운 세로 핀 한 행만 포함해야 합니다. Range를 비우면 측정 전용(판정 아님), 양수를 입력하면 모든 행을 DistancePxRange 최대값으로 판정합니다. v1은 edge-to-edge와 px만 지원합니다.",
            "Each ROI must contain one row of dark, roughly vertical pins. Blank Range is measurement only (not judged); a positive value judges every row with a DistancePxRange maximum. v1 supports edge-to-edge and px only.");

        public string PinArrayGapValidationSetsLabelText => LocalText("2단계 세트", "Phase 2 sets");

        public string PinArrayGapTrainLabelText => "Train";

        public string PinArrayGapValidationLabelText => "Validation";

        public string PinArrayGapTestLabelText => "Test";

        public string FreezePinArrayGapValidationIdentityText => LocalText("검증 기준 고정", "Freeze identity");

        public string OpenPinArrayGapValidationRunsText => LocalText("명시적 실행·증거 열기", "Open explicit runs");

        public string PinArrayGapValidationBoundaryText => LocalText(
            "이 버튼은 XML·세트 해시만 고정합니다. 실행은 기존 Validation Set 화면에서 사용자가 명시적으로 시작합니다.",
            "Freeze records XML/set hashes only. Runs remain explicit in the existing Validation Set screen.");

        public string BlobCountIntentSkillText => LocalText("Blob count skill", "Blob count skill");

        public string BlobCountIntentRoiLabelText => LocalText("ROI", "ROI");

        public string BlobCountIntentThresholdLabelText => LocalText("Threshold", "Threshold");

        public string BlobCountIntentMinCountLabelText => LocalText("Min count", "Min count");

        public string BlobCountIntentMaxCountLabelText => LocalText("Max count", "Max count");

        public string BlobCountIntentMinAreaLabelText => LocalText("Min area", "Min area");

        public string BlobCountIntentMaxAreaLabelText => LocalText("Max area", "Max area");

        public string CreateBlobCountIntentXmlText => LocalText("초안 XML 만들기", "Create draft XML");

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

        public string BlobCountIntentLatestRunText =>
            OpenVisionRecipeIntentFeedbackPresenter.BuildBlobCountLatestRunText(
                LatestSampleRunSummary,
                BlobCountIntentMinCountText,
                BlobCountIntentMaxCountText);

        public string ContourCountIntentSkillText => LocalText("Contour count/size skill", "Contour count/size skill");

        public string ContourCountIntentRoiLabelText => LocalText("ROI", "ROI");

        public string ContourCountIntentThresholdLabelText => LocalText("Threshold", "Threshold");

        public string ContourCountIntentMinCountLabelText => LocalText("Min count", "Min count");

        public string ContourCountIntentMaxCountLabelText => LocalText("Max count", "Max count");

        public string ContourCountIntentMinAreaLabelText => LocalText("Min area", "Min area");

        public string ContourCountIntentMaxAreaLabelText => LocalText("Max area", "Max area");

        public string CreateContourCountIntentXmlText => LocalText("초안 XML 만들기", "Create draft XML");

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

        public string ContourCountIntentLatestRunText =>
            OpenVisionRecipeIntentFeedbackPresenter.BuildContourCountLatestRunText(
                LatestSampleRunSummary,
                ContourCountIntentMinCountText,
                ContourCountIntentMaxCountText,
                ContourCountIntentMaxAreaText);

        public string MatchingIntentTemplatePathLabelText => LocalText("Template path", "Template path");

        public string MatchingIntentSearchRoiLabelText => LocalText("Search ROI", "Search ROI");

        public string MatchingIntentScoreMinLabelText => LocalText("Min score", "Min score");

        public string MatchingIntentExpectedCountLabelText => LocalText("Expected count", "Expected count");

        public string FeatureMatchingIntentTemplatePathLabelText => LocalText("특징 템플릿", "Feature template");

        public string FeatureMatchingIntentScopeLabelText => LocalText("검사 범위", "Inspection scope");

        public string FeatureMatchingIntentScopeValueText => LocalText("전체 이미지", "Full image");

        public string FeatureMatchingIntentScoreMinLabelText => LocalText("Ratio 기준", "Ratio min");

        public string FeatureMatchingIntentRansacReprojThresholdLabelText => LocalText("RANSAC px", "RANSAC px");

        public string FeatureMatchingIntentAcceptanceScoreMinLabelText => LocalText("ScoreMax 최소", "ScoreMax min");

        public string EdgeBasedIntentTemplatePathLabelText => LocalText("에지 템플릿", "Edge template");

        public string EdgeBasedIntentScopeLabelText => LocalText("검사 범위", "Inspection scope");

        public string EdgeBasedIntentScopeValueText => LocalText("전체 이미지", "Full image");

        public string EdgeBasedIntentScoreMinLabelText => LocalText("최소 점수", "Min score");

        public string EdgeBasedIntentSearchCountLabelText => LocalText("검색 개수", "Search count");

        public string EdgeBasedIntentCannyLowLabelText => "Canny low";

        public string EdgeBasedIntentCannyHighLabelText => "Canny high";

        public string EdgeBasedIntentAcceptanceScoreMinLabelText => LocalText("ScoreMax 최소", "ScoreMax min");

        public string ReferenceDifferencePath1LabelText => LocalText("Good 기준 1", "Good reference 1");

        public string ReferenceDifferencePath2LabelText => LocalText("Good 기준 2", "Good reference 2");

        public string ReferenceDifferencePath3LabelText => LocalText("Good 기준 3", "Good reference 3");

        public string ReferenceDifferencePath4LabelText => LocalText("Good 기준 4", "Good reference 4");

        public string ReferenceDifferenceThresholdLabelText => LocalText("차이 임계값", "Difference threshold");

        public string ReferenceDifferenceMinimumAreaLabelText => LocalText("최소 결함 면적", "Min defect area");

        public string ReferenceDifferenceMaximumAreaLabelText => LocalText("최대 결함 면적", "Max defect area");

        public string ReferenceDifferenceBoundaryText => LocalText(
            "기준 이미지는 작업자가 승인해 직접 지정합니다. 초안 생성은 기준을 학습·교체하거나 Preview/Run을 실행하지 않습니다.",
            "The operator explicitly approves each reference. Draft creation does not learn or replace references, Preview, or Run.");

        public string MeanIntentRoiLabelText => LocalText("ROI (optional)", "ROI (optional)");

        public string MeanIntentTypeLabelText => LocalText("Mean type", "Mean type");

        public string MeanIntentMinimumLabelText => LocalText("Min GV", "Min GV");

        public string MeanIntentMaximumLabelText => LocalText("Max GV", "Max GV");

        public string LlmResultChannelContractSummaryText =>
            LocalText("선택 의도는 도구군을 고정합니다: ", "Selected intent locks tool family: ")
            + ResolveIntentSummary(SelectedLlmToolTemplate)
            + LocalText(
                " / 출력 채널은 XML 검증과 명시적 샘플 실행에서 파생됩니다.",
                " / Result channels are derived from XML validation and explicit sample runs.");

        public string BuildLlmPromptButtonText => LocalText("프롬프트 생성", "Build prompt");

        public string OpenLlmBrowserAssistText => LocalText("웹 보조", "Web assist");

        public string LlmBrowserAssistTitleText => LocalText("ChatGPT 웹으로 XML 작성", "Author XML in ChatGPT web");

        public string LlmBrowserAssistBoundaryText => LocalText(
            "API 키·계정·대화 내용은 OpenVisionLab이 관리하지 않습니다. 열기 뒤 직접 로그인·복사·붙여넣기·전송하고, XML은 명시적으로 검증/가져오기 하세요.",
            "OpenVisionLab does not manage API keys, accounts, or chats. After opening, sign in, copy, paste, and send yourself; validate/import XML explicitly.");

        public string OpenLlmBrowserAssistChatGptText => LocalText("ChatGPT 열기", "Open ChatGPT");

        public string OpenLlmBrowserAssistExternalText => LocalText("외부 브라우저", "External browser");

        public string LlmBrowserAssistStatusText
        {
            get => llmBrowserAssistStatusText;
            private set => SetProperty(ref llmBrowserAssistStatusText, value ?? string.Empty);
        }

        public string CopyLlmPromptText => LocalText("프롬프트 복사", "Copy prompt");

        public string LlmPromptCopyStatusText
        {
            get => llmPromptCopyStatusText;
            private set => SetProperty(ref llmPromptCopyStatusText, value ?? string.Empty);
        }

        public string CreateLlmTemplateXmlText => LocalText("초안 XML 만들기", "Create draft XML");

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

        public string LoadLlmXmlDraftText => LocalText("XML/번들 로드", "Load XML/bundle");

        public string ValidateLlmXmlDraftButtonText => LocalText("검증", "Validate");

        public string ImportLlmXmlDraftText => LocalText("가져오기", "Import");

        public string UseSelectedSampleReferenceText => LocalText("샘플 사용", "Use sample");

        public string LlmReferenceImageText => LocalText("참조 이미지", "Reference image");

        public string LlmDraftValidationText => loadedReviewBundleInspection == null
            ? LocalText("초안 검증", "Draft validation")
            : LocalText("검토 번들 / XML 검증", "Review bundle / XML validation");

        public string LlmDependencyReportText => loadedReviewBundleInspection == null
            ? LocalText("의존 파일 복사 보고서", "Dependency copy report")
            : LocalText("번들 의존성 / 재배치 검토", "Bundle dependency / relocation review");

        public string LlmDependencyPathRowsText => LocalText("경로 검토", "Path review");

        public string LlmDraftReviewReportText => LocalText("초안 가져오기 검토", "Draft import review");

        public string LlmDraftDiffReportText => LocalText("LLM XML 변경점", "LLM XML diff review");

        public string RecipeEditValidationText =>
            OpenVisionRecipeLifecycleValidationPresenter.BuildRecipeEditValidationText(
                CreateRecipeEditValidationRequest());

        public string PipelineEditValidationText =>
            OpenVisionRecipeLifecycleValidationPresenter.BuildPipelineEditValidationText(
                CreatePipelineEditValidationRequest());


    }

}
