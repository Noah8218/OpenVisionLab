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
    public sealed class OpenVisionShellHostRecipeCommandSurface : ObservableObject
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
        private object selectedStepEditObject;
        private string selectedStepEditStatusText = string.Empty;
        private string correctedOutputReviewText = string.Empty;
        private bool selectedStepEditDirty;
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
        private string validationSuiteStatusText = string.Empty;
        private string pinArrayGapValidationStatusText = string.Empty;
        private bool isPinArrayGapValidationIdentityFrozen;
        private string newValidationSetName = "Local_Validation_Set";
        private string validationSetPendingNotes = string.Empty;
        private string statusText = string.Empty;
        private OpenVisionRecipeValidationSetDocument validationSetDocument = OpenVisionRecipeValidationSetStorage.CreateEmpty();
        private bool validationSetStorageReady = true;
        private OpenVisionRecipeReviewBundleInspection loadedReviewBundleInspection;
        private bool llmXmlDraftImportReady;
        private bool isGuidedSetupDraftStale;
        private bool isRefreshingOptions;
        private bool isSelectingRecipe;
        private bool isSampleCheckRunning;
        private bool isPairCheckRunning;
        private bool isCatalogBenchmarkRunning;
        private bool isValidationSuiteRunning;
        private bool isLocalValidationSetRunning;
        private bool validationSuiteStopRequested;
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
            selectedValidationSuiteScopeOption = validationSuiteScopeOptions.FirstOrDefault();
            validationSuiteStatusText = OpenVisionRecipeText.Local(
                "Suite 범위를 선택한 뒤 명시적으로 Run suite를 실행하세요.",
                "Select a suite scope, then run the explicit suite.");
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

                    SelectedRecentBatchSampleResultOption = SelectDefaultBatchSampleResult(
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

                    SelectedRecentBatchSampleResultOption = SelectDefaultBatchSampleResult(
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
                    SelectedRecentBatchSampleResultOption = SelectDefaultBatchSampleResult(
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
                    SelectedPairSampleResult = SelectDefaultPairSampleResult(latestPairRunSummary);
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

        public ICommand CreateRecipeCommand { get; }

        public ICommand CreateNamedRecipeCommand { get; }

        public ICommand DuplicateRecipeCommand { get; }

        public ICommand RenameRecipeCommand { get; }

        public ICommand DeleteRecipeCommand { get; }

        public ICommand ImportPipelineXmlCommand { get; }

        public ICommand ExportPipelineXmlCommand { get; }

        public ICommand ExportRecipeReviewBundleCommand { get; }

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

        public ICommand SuggestPinGapIntentRoiSamplesCommand { get; }

        public ICommand RunSelectedSampleCheckCommand { get; }

        public ICommand RunSelectedSamplePairCheckCommand { get; }

        public ICommand RunCatalogBenchmarkCommand { get; }

        public ICommand RunValidationSuiteCommand { get; }

        public ICommand StopValidationSuiteCommand { get; }

        public ICommand CreateValidationSetCommand { get; }

        public ICommand DeleteValidationSetCommand { get; }

        public ICommand AddValidationSetOkImagesCommand { get; }

        public ICommand AddValidationSetNgImagesCommand { get; }

        public ICommand AddValidationSetOkFolderCommand { get; }

        public ICommand AddValidationSetNgFolderCommand { get; }

        public ICommand RepairValidationSetImagePathCommand { get; }

        public ICommand RemoveValidationSetImageCommand { get; }

        public ICommand SelectPairSampleResultCommand { get; }

        public ICommand BuildLlmPromptCommand { get; }

        public ICommand CreateLlmTemplateXmlDraftCommand { get; }

        public ICommand CreateGuidedSetupStarterXmlCommand { get; }

        public ICommand CreatePinGapIntentXmlDraftCommand { get; }

        public ICommand CreateBlobCountIntentXmlDraftCommand { get; }

        public ICommand CreateContourCountIntentXmlDraftCommand { get; }

        public ICommand RefreshLlmDraftReviewCommand { get; }

        public ICommand NavigateSelectedStepInputLayerCommand { get; }

        public ICommand NavigateSelectedStepOutputLayerCommand { get; }

        public ICommand FocusSelectedRunFailureStepCommand { get; }

        public ICommand LoadSelectedRunSampleImageToInputLayerCommand { get; }

        public ICommand OpenSelectedRecentBatchRunEvidenceCommand { get; }

        public ICommand FreezePinArrayGapValidationIdentityCommand { get; }

        public ICommand OpenPinArrayGapValidationRunsCommand { get; }

        public ICommand SelectPreviousPipelinePreviewStepCommand { get; }

        public ICommand SelectNextPipelinePreviewStepCommand { get; }

        public ICommand OpenSelectedStepToolCommand { get; }

        public ICommand LoadSelectedStepParametersCommand { get; }

        public ICommand ApplySelectedStepParametersCommand { get; }

        public ICommand CopyOperatorHandoffReportCommand { get; }

        public ICommand CopySelectedRecentBatchRunReviewCommand { get; }

        public ICommand RunRecipeGuidedNextActionCommand { get; }

        public ICommand OpenPipelineReviewCommand { get; }

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
            isCatalogBenchmarkRunning ? LocalText("실행 중...", "Running...") : LocalText("전체 샘플 검사", "Run catalog");

        public string RunCatalogBenchmarkShortText =>
            isCatalogBenchmarkRunning ? LocalText("실행 중", "Running") : LocalText("카탈로그", "Catalog");

        public string CatalogBenchmarkSummaryText =>
            LatestCatalogBenchmarkSummary?.CompactText
            ?? OpenVisionRecipeCatalogBenchmarkSummary.Empty.CompactText;

        public string CatalogBenchmarkDetailText =>
            LatestCatalogBenchmarkSummary?.DetailText
            ?? OpenVisionRecipeCatalogBenchmarkSummary.Empty.DetailText;

        public string ValidationSuiteText => "Validation Suite";

        public string ValidationSuiteScopeLabelText => LocalText("범위", "Scope");

        public string RunValidationSuiteText =>
            isValidationSuiteRunning
                ? LocalText("실행 중...", "Running...")
                : IsLocalValidationSetSelected
                    ? LocalText("목록 검증 실행", "Run image list")
                    : LocalText("Suite 실행", "Run suite");

        public string StopValidationSuiteText => validationSuiteStopRequested
            ? LocalText("중지 대기", "Stopping")
            : LocalText("실행 중지", "Stop");

        public bool IsLocalValidationSetRunning => isLocalValidationSetRunning;

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
                isValidationSuiteRunning,
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
            get => validationSuiteStatusText;
            private set
            {
                if (SetProperty(ref validationSuiteStatusText, value ?? string.Empty))
                {
                    OnPropertyChanged(nameof(ValidationSuiteSummaryText));
                    OnPropertyChanged(nameof(ValidationSetNextActionText));
                }
            }
        }

        public string RunSelectedSampleCheckText => isSampleCheckRunning ? LocalText("실행 중...", "Running...") : LocalText("검사 실행", "Run check");

        public string RunSelectedSamplePairCheckText => isPairCheckRunning ? LocalText("실행 중...", "Running...") : LocalText("쌍 검사", "Run pair");

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
            string.IsNullOrWhiteSpace(correctedOutputReviewText)
                ? OpenVisionRecipePipelineStepReviewPresenter.BuildCorrectedOutputReviewText(
                    SelectedPipelinePreviewStep,
                    selectedStepEditDirty,
                    SelectedStepEditObject)
                : correctedOutputReviewText;

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

        public void SelectLocalValidationSetScope()
        {
            OpenVisionRecipeValidationSuiteScopeOption option = validationSuiteScopeOptions.FirstOrDefault(candidate =>
                string.Equals(
                    candidate?.Key,
                    OpenVisionRecipeValidationSuiteScopeOption.LocalValidationSetKey,
                    StringComparison.OrdinalIgnoreCase));
            if (option != null)
            {
                SelectedValidationSuiteScopeOption = option;
            }
        }

        public bool FocusPipelineStepForEdit(string recipeName, string pipelineName, int stepNumber)
        {
            string requestedRecipe = NormalizeRecipeName(recipeName);
            string requestedPipeline = NormalizePipelineName(pipelineName);
            if (string.IsNullOrWhiteSpace(requestedRecipe)
                || string.IsNullOrWhiteSpace(requestedPipeline)
                || stepNumber <= 0)
            {
                StatusText = OpenVisionRecipeText.Local(
                    "Step 설정을 열 수 없습니다. 레시피, 파이프라인, Step 선택을 확인하세요.",
                    "Cannot open step settings. Check the recipe, pipeline, and step selection.");
                return false;
            }

            if (!string.Equals(selectedRecipeName, requestedRecipe, StringComparison.OrdinalIgnoreCase))
            {
                SelectRecipe(requestedRecipe);
            }

            RefreshPipelineOptions(requestedPipeline);
            OpenVisionRecipePipelineOption option = PipelineOptions.FirstOrDefault(candidate =>
                string.Equals(candidate.PipelineName, requestedPipeline, StringComparison.OrdinalIgnoreCase));
            if (option == null)
            {
                StatusText = OpenVisionRecipeText.Local("파이프라인을 찾을 수 없습니다: ", "Pipeline not found: ") + requestedPipeline;
                return false;
            }

            SelectedPipelineOption = option;
            OpenVisionRecipeSampleOption matchingSample = SampleOptions.FirstOrDefault(candidate =>
                string.Equals(
                    "Sample_" + SanitizePathSegment(candidate.SampleName),
                    requestedPipeline,
                    StringComparison.OrdinalIgnoreCase));
            if (matchingSample != null)
            {
                SelectedSampleOption = matchingSample;
            }

            OpenVisionRecipePipelineStepPreview step = SelectedRecipeSummary?.PipelinePreviewSteps?
                .FirstOrDefault(candidate => candidate.Index == stepNumber);
            if (step == null)
            {
                StatusText = OpenVisionRecipeText.Local("Step을 찾을 수 없습니다: ", "Step not found: ")
                    + stepNumber.ToString(CultureInfo.InvariantCulture);
                return false;
            }

            SelectedPipelinePreviewStep = step;
            bool propertyLoaded = LoadSelectedStepParametersForEdit(updateStatus: true);
            StatusText = propertyLoaded
                ? OpenVisionRecipeText.Local("Step 설정 편집 준비: ", "Step settings ready: ") + step.Name
                : OpenVisionRecipeText.Local("Step을 선택했습니다. XML/Step 정보를 확인하세요: ", "Step selected. Review its XML/Step details: ") + step.Name;
            return true;
        }

        public void RefreshLocalization()
        {
            OnPropertyChanged(nameof(NewRecipeButtonText));
            OnPropertyChanged(nameof(RecipeSelectorToolTipText));
            OnPropertyChanged(nameof(ManagerButtonText));
            OnPropertyChanged(nameof(ManagerButtonShortText));
            OnPropertyChanged(nameof(ManagerTitleText));
            OnPropertyChanged(nameof(RecipeOverviewTabText));
            OnPropertyChanged(nameof(RecipeAdvancedReviewText));
            OnPropertyChanged(nameof(RecipeReturnToSummaryText));
            OnPropertyChanged(nameof(RecipeTechnicalReviewText));
            OnPropertyChanged(nameof(RecipeOverviewTitleText));
            OnPropertyChanged(nameof(RecipeOverviewPipelineText));
            OnPropertyChanged(nameof(RecipeOverviewValidationText));
            OnPropertyChanged(nameof(RecipeOverviewSelectedSampleText));
            OnPropertyChanged(nameof(RecipeOverviewSelectedSampleContextText));
            OnPropertyChanged(nameof(RecipeOverviewLastResultText));
            OnPropertyChanged(nameof(RecipeOverviewLastResultValueText));
            OnPropertyChanged(nameof(RecipeOverviewLastResultToolTipText));
            OnPropertyChanged(nameof(OpenPipelineReviewText));
            OnPropertyChanged(nameof(OpenImageListValidationText));
            OnPropertyChanged(nameof(OpenImageListValidationToolTipText));
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
            OnPropertyChanged(nameof(ExportRecipeReviewBundleText));
            OnPropertyChanged(nameof(ExportRecipeReviewBundleToolTipText));
            OnPropertyChanged(nameof(RecipeDetailText));
            OnPropertyChanged(nameof(RecipePipelineTabText));
            OnPropertyChanged(nameof(RecipeGuidedSetupTabText));
            OnPropertyChanged(nameof(RecipeLlmXmlTabText));
            OnPropertyChanged(nameof(RecipeLlmBrowserAssistTabText));
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
            OnPropertyChanged(nameof(RecentBatchRunNgOnlyText));
            OnPropertyChanged(nameof(RecentBatchRunNgFilterSummaryText));
            OnPropertyChanged(nameof(RecentBatchRunReviewQueueOnlyText));
            OnPropertyChanged(nameof(RecentBatchRunReviewQueueSummaryText));
            OnPropertyChanged(nameof(RecentBatchRunComparisonText));
            OnPropertyChanged(nameof(BenchmarkBaselineRunText));
            OnPropertyChanged(nameof(RecentBatchRunComparisonSummaryText));
            OnPropertyChanged(nameof(SelectedRecentBatchRunComparisonReviewText));
            OnPropertyChanged(nameof(SelectedRecentBatchRunReviewLabelText));
            OnPropertyChanged(nameof(SelectedRecentBatchRunReviewText));
            OnPropertyChanged(nameof(CopySelectedRecentBatchRunReviewText));
            OnPropertyChanged(nameof(OpenSelectedRecentBatchRunEvidenceText));
            OnPropertyChanged(nameof(CatalogBenchmarkText));
            OnPropertyChanged(nameof(RunCatalogBenchmarkText));
            OnPropertyChanged(nameof(RunCatalogBenchmarkShortText));
            OnPropertyChanged(nameof(CatalogBenchmarkSummaryText));
            OnPropertyChanged(nameof(CatalogBenchmarkDetailText));
            OnPropertyChanged(nameof(RunValidationSuiteText));
            OnPropertyChanged(nameof(StopValidationSuiteText));
            OnPropertyChanged(nameof(ValidationSetText));
            OnPropertyChanged(nameof(ValidationSetSelectionLabelText));
            OnPropertyChanged(nameof(NewValidationSetNameLabelText));
            OnPropertyChanged(nameof(CreateValidationSetText));
            OnPropertyChanged(nameof(DeleteValidationSetText));
            OnPropertyChanged(nameof(ValidationSetPendingNotesLabelText));
            OnPropertyChanged(nameof(AddValidationSetOkImagesText));
            OnPropertyChanged(nameof(AddValidationSetNgImagesText));
            OnPropertyChanged(nameof(ValidationSetFolderBatchLabelText));
            OnPropertyChanged(nameof(AddValidationSetOkFolderText));
            OnPropertyChanged(nameof(AddValidationSetNgFolderText));
            OnPropertyChanged(nameof(AddValidationSetFolderToolTipText));
            OnPropertyChanged(nameof(RepairValidationSetImagePathText));
            OnPropertyChanged(nameof(RepairValidationSetImagePathToolTipText));
            OnPropertyChanged(nameof(RemoveValidationSetImageText));
            OnPropertyChanged(nameof(ValidationSetSelectionSummaryText));
            OnPropertyChanged(nameof(ValidationSetEvidenceText));
            OnPropertyChanged(nameof(ValidationSetExpectedLabelText));
            OnPropertyChanged(nameof(ValidationSetAcceptanceLabelText));
            OnPropertyChanged(nameof(ValidationSetCalibrationLabelText));
            OnPropertyChanged(nameof(ValidationSetNextActionLabelText));
            NotifyValidationSetEvidenceChanged();
            OnPropertyChanged(nameof(RunSelectedSampleCheckText));
            OnPropertyChanged(nameof(RunSelectedSamplePairCheckText));
            OnPropertyChanged(nameof(SelectedSampleAcceptanceSummaryText));
            OnPropertyChanged(nameof(OperatorReviewText));
            OnPropertyChanged(nameof(PipelineVariantComparisonText));
            OnPropertyChanged(nameof(PipelineVariantComparisonReport));
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
            OnPropertyChanged(nameof(GuidedSetupNoLlmText));
            OnPropertyChanged(nameof(GuidedSetupIntentLabelText));
            OnPropertyChanged(nameof(GuidedSetupSelectedSampleLabelText));
            OnPropertyChanged(nameof(GuidedSetupCreateStarterText));
            OnPropertyChanged(nameof(GuidedSetupDraftLabelText));
            OnPropertyChanged(nameof(GuidedSetupNextText));
            OnPropertyChanged(nameof(GuidedSetupActionBoundaryText));
            OnPropertyChanged(nameof(GuidedSetupSummaryText));
            OnPropertyChanged(nameof(GuidedSetupReadinessText));
            OnPropertyChanged(nameof(OpenLlmGuidedSetupText));
            OnPropertyChanged(nameof(IsGuidedSetupIntentInputReady));
            OnPropertyChanged(nameof(GuidedSetupIntentInputStatusText));
            OnPropertyChanged(nameof(MatchingIntentTemplatePathLabelText));
            OnPropertyChanged(nameof(MatchingIntentSearchRoiLabelText));
            OnPropertyChanged(nameof(MatchingIntentScoreMinLabelText));
            OnPropertyChanged(nameof(MatchingIntentExpectedCountLabelText));
            OnPropertyChanged(nameof(FeatureMatchingIntentTemplatePathLabelText));
            OnPropertyChanged(nameof(FeatureMatchingIntentScopeLabelText));
            OnPropertyChanged(nameof(FeatureMatchingIntentScopeValueText));
            OnPropertyChanged(nameof(FeatureMatchingIntentScoreMinLabelText));
            OnPropertyChanged(nameof(FeatureMatchingIntentRansacReprojThresholdLabelText));
            OnPropertyChanged(nameof(FeatureMatchingIntentAcceptanceScoreMinLabelText));
            OnPropertyChanged(nameof(EdgeBasedIntentTemplatePathLabelText));
            OnPropertyChanged(nameof(EdgeBasedIntentScopeLabelText));
            OnPropertyChanged(nameof(EdgeBasedIntentScopeValueText));
            OnPropertyChanged(nameof(EdgeBasedIntentScoreMinLabelText));
            OnPropertyChanged(nameof(EdgeBasedIntentSearchCountLabelText));
            OnPropertyChanged(nameof(EdgeBasedIntentCannyLowLabelText));
            OnPropertyChanged(nameof(EdgeBasedIntentCannyHighLabelText));
            OnPropertyChanged(nameof(EdgeBasedIntentAcceptanceScoreMinLabelText));
            OnPropertyChanged(nameof(ReferenceDifferencePath1LabelText));
            OnPropertyChanged(nameof(ReferenceDifferencePath2LabelText));
            OnPropertyChanged(nameof(ReferenceDifferencePath3LabelText));
            OnPropertyChanged(nameof(ReferenceDifferencePath4LabelText));
            OnPropertyChanged(nameof(ReferenceDifferenceThresholdLabelText));
            OnPropertyChanged(nameof(ReferenceDifferenceMinimumAreaLabelText));
            OnPropertyChanged(nameof(ReferenceDifferenceMaximumAreaLabelText));
            OnPropertyChanged(nameof(ReferenceDifferenceBoundaryText));
            OnPropertyChanged(nameof(MeanIntentRoiLabelText));
            OnPropertyChanged(nameof(MeanIntentTypeLabelText));
            OnPropertyChanged(nameof(MeanIntentMinimumLabelText));
            OnPropertyChanged(nameof(MeanIntentMaximumLabelText));
            OnPropertyChanged(nameof(LlmToolTemplateText));
            OnPropertyChanged(nameof(LlmInspectionGoalLabelText));
            OnPropertyChanged(nameof(LlmDetectionPointLabelText));
            OnPropertyChanged(nameof(LlmResultChannelContractSummaryText));
            OnPropertyChanged(nameof(PinGapIntentWorkflowText));
            OnPropertyChanged(nameof(DarkBandGapIntentSkillText));
            OnPropertyChanged(nameof(DarkBandGapIntentRoiLabelText));
            OnPropertyChanged(nameof(DarkBandGapIntentBoundaryText));
            OnPropertyChanged(nameof(HybridRelativeRoiIntentSkillText));
            OnPropertyChanged(nameof(HybridLocatorTemplateLabelText));
            OnPropertyChanged(nameof(HybridSearchRoiLabelText));
            OnPropertyChanged(nameof(HybridReferencePoseLabelText));
            OnPropertyChanged(nameof(HybridRelativeRoiLabelText));
            OnPropertyChanged(nameof(HybridScoreMinimumLabelText));
            OnPropertyChanged(nameof(HybridScoreMarginLabelText));
            OnPropertyChanged(nameof(HybridAngleRangeLabelText));
            OnPropertyChanged(nameof(HybridScaleRatioRangeLabelText));
            OnPropertyChanged(nameof(HybridMinimumValidPixelRatioLabelText));
            OnPropertyChanged(nameof(HybridRelativeRoiBoundaryText));
            OnPropertyChanged(nameof(PinGapIntentCalibrationReviewText));
            OnPropertyChanged(nameof(PinGapIntentFeedbackText));
            OnPropertyChanged(nameof(PinGapIntentLatestRunText));
            OnPropertyChanged(nameof(PinArrayGapRoiLabelText));
            OnPropertyChanged(nameof(PinArrayGapPolarityLabelText));
            OnPropertyChanged(nameof(PinArrayGapMeasurementLabelText));
            OnPropertyChanged(nameof(PinArrayGapRangeMaxLabelText));
            OnPropertyChanged(nameof(PinArrayGapDarkThresholdLabelText));
            OnPropertyChanged(nameof(PinArrayGapMinDarkCoverageRatioLabelText));
            OnPropertyChanged(nameof(PinArrayGapMinPinWidthLabelText));
            OnPropertyChanged(nameof(PinArrayGapMaxPinBreakWidthLabelText));
            OnPropertyChanged(nameof(PinArrayGapMinGapWidthLabelText));
            OnPropertyChanged(nameof(PinArrayGapIntentContractText));
            OnPropertyChanged(nameof(PinArrayGapValidationSetsLabelText));
            OnPropertyChanged(nameof(PinArrayGapTrainLabelText));
            OnPropertyChanged(nameof(PinArrayGapValidationLabelText));
            OnPropertyChanged(nameof(PinArrayGapTestLabelText));
            OnPropertyChanged(nameof(FreezePinArrayGapValidationIdentityText));
            OnPropertyChanged(nameof(OpenPinArrayGapValidationRunsText));
            OnPropertyChanged(nameof(PinArrayGapValidationBoundaryText));
            RefreshPinArrayGapValidationIdentityState();
            OnPropertyChanged(nameof(PinArrayGapValidationStatusText));
            OnPropertyChanged(nameof(BuildLlmPromptButtonText));
            OnPropertyChanged(nameof(OpenLlmBrowserAssistText));
            OnPropertyChanged(nameof(LlmBrowserAssistTitleText));
            OnPropertyChanged(nameof(LlmBrowserAssistBoundaryText));
            OnPropertyChanged(nameof(OpenLlmBrowserAssistChatGptText));
            OnPropertyChanged(nameof(OpenLlmBrowserAssistExternalText));
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
            RefreshValidationSetOptions();
            RefreshRecentBatchRunOptions();
            UpdateSelectedRecipeSummary();
        }

        internal void SetLlmBrowserAssistStatus(OpenVisionRecipeLlmBrowserAssistOpenResult result)
        {
            LlmBrowserAssistStatusText = result switch
            {
                OpenVisionRecipeLlmBrowserAssistOpenResult.EmbeddedChatGptOpened => LocalText(
                    "ChatGPT를 웹 보조 창에 열었습니다. 직접 로그인 후 프롬프트를 복사하세요.",
                    "ChatGPT opened in Web assist. Sign in yourself, then copy the prompt."),
                OpenVisionRecipeLlmBrowserAssistOpenResult.ExternalChatGptOpened => LocalText(
                    "기본 외부 브라우저에서 ChatGPT를 열었습니다.",
                    "ChatGPT opened in the default external browser."),
                OpenVisionRecipeLlmBrowserAssistOpenResult.EmbeddedBrowserUnavailable => LocalText(
                    "내장 브라우저를 열 수 없습니다. 외부 브라우저를 사용하세요.",
                    "The embedded browser is unavailable. Use the external browser."),
                _ => LocalText(
                    "ChatGPT를 열지 못했습니다. 외부 브라우저를 다시 시도하세요.",
                    "ChatGPT could not be opened. Try the external browser again.")
            };
        }

        private string CreateLlmBrowserAssistReadyText()
        {
            return LocalText(
                "자동 로그인·전송·XML 가져오기·Preview/Run은 수행하지 않습니다.",
                "No automatic sign-in, send, XML import, Preview, or Run is performed.");
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
            RefreshPinArrayGapValidationIdentityState();
            NotifyValidationSetEvidenceChanged();
            RefreshCommandState();
        }

        private OpenVisionRecipeEditValidationRequest CreateRecipeEditValidationRequest()
        {
            return new OpenVisionRecipeEditValidationRequest
            {
                SelectedRecipeName = NormalizeRecipeName(selectedRecipeName),
                RequestedRecipeName = EditRecipeName,
                RecipeNames = RecipeOptions
            };
        }

        private void NotifyOperatorReviewChanged()
        {
            OnPropertyChanged(nameof(OperatorRunReviewText));
            OnPropertyChanged(nameof(OperatorDecisionXmlCardText));
            OnPropertyChanged(nameof(OperatorDecisionSampleCardText));
            OnPropertyChanged(nameof(OperatorDecisionPairCardText));
            OnPropertyChanged(nameof(OperatorDecisionSummaryStatusText));
            OnPropertyChanged(nameof(OperatorDecisionNextActionText));
            OnPropertyChanged(nameof(OperatorDecisionEvidenceText));
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
            VisionPipelineSampleCatalogItem selectedSample = SelectedSampleOption?.Sample;
            IReadOnlyList<OpenVisionRecipeSampleMatrixRow> rows =
                OpenVisionRecipeSampleMatrixPresenter.BuildRows(selectedSample, LatestPairRunSummary);
            OpenVisionRecipeSampleMatrixRow previous = SelectedSampleMatrixRow;
            SampleMatrixRows = rows;
            SelectedSampleMatrixRow = OpenVisionRecipeSampleMatrixPresenter.SelectDefaultRow(rows, previous);
            OnPropertyChanged(nameof(SampleMatrixSummaryText));
            OnPropertyChanged(nameof(SelectedSampleMatrixReviewText));
        }




        private void NotifyValidationSetEvidenceChanged()
        {
            OnPropertyChanged(nameof(ValidationSetExpectedText));
            OnPropertyChanged(nameof(ValidationSetAcceptanceText));
            OnPropertyChanged(nameof(ValidationSetCalibrationText));
            OnPropertyChanged(nameof(ValidationSetNextActionText));
        }


        private string BuildValidationSetAcceptanceText()
        {
            if (!TryLoadSelectedPipelineForValidationEvidence(out VisionPipeline pipeline, out string error))
            {
                return error;
            }

            List<VisionPipelineStep> acceptanceSteps = GetEnabledAcceptanceSteps(pipeline);
            if (acceptanceSteps.Count == 0)
            {
                return LocalText(
                    "Metric 기준 없음: 파이프라인 OK/NG 결과를 기대 OK/NG와 비교합니다.",
                    "No metric gate: compare pipeline OK/NG against the expected OK/NG roles.");
            }

            List<string> gates = acceptanceSteps
                .Select(FormatValidationSetAcceptanceGate)
                .Where(text => !string.IsNullOrWhiteSpace(text))
                .ToList();
            string visible = string.Join(" | ", gates.Take(2));
            int remaining = Math.Max(0, gates.Count - 2);
            return LocalText("활성 기준: ", "Active gate: ")
                + visible
                + (remaining > 0 ? " +" + remaining.ToString(CultureInfo.InvariantCulture) : string.Empty);
        }

        private string BuildValidationSetCalibrationText()
        {
            if (!TryLoadSelectedPipelineForValidationEvidence(out VisionPipeline pipeline, out string error))
            {
                return error;
            }

            List<VisionPipelineStep> millimeterSteps = GetEnabledAcceptanceSteps(pipeline)
                .Where(step => (step.AcceptanceMetricName ?? string.Empty).IndexOf("Mm", StringComparison.OrdinalIgnoreCase) >= 0)
                .ToList();
            if (millimeterSteps.Count == 0)
            {
                return LocalText(
                    "해당 없음: 현재 판정 기준은 mm 물리 단위를 사용하지 않습니다.",
                    "Not required: the active acceptance gates do not use physical mm units.");
            }

            List<double> scales = new List<double>();
            bool missingScale = false;
            foreach (VisionPipelineStep step in millimeterSteps)
            {
                string value = step.Parameters?
                    .Where(parameter => string.Equals(parameter.Key, "PIXELPERMM", StringComparison.OrdinalIgnoreCase))
                    .Select(parameter => parameter.Value)
                    .FirstOrDefault();
                if (!TryParsePositiveDouble(value, out double scale))
                {
                    missingScale = true;
                    continue;
                }

                scales.Add(scale);
            }

            if (missingScale)
            {
                return LocalText(
                    "필수: mm 판정 기준에 PIXELPERMM이 없거나 0입니다. 물리 단위 판정을 실행하지 마십시오.",
                    "Required: an mm gate has no positive PIXELPERMM. Do not use it for a physical-unit decision.");
            }

            string scaleText = string.Join(
                ", ",
                scales
                    .Distinct()
                    .OrderBy(value => value)
                    .Select(value => value.ToString("0.######", CultureInfo.InvariantCulture)));
            return LocalText("적용됨: PIXELPERMM ", "Applied: PIXELPERMM ")
                + scaleText
                + LocalText(" mm/px. 현재 렌즈와 이미지의 보정값인지 확인하십시오.", " mm/px. Confirm this scale matches the current lens and image.");
        }


        private bool TryLoadSelectedPipelineForValidationEvidence(out VisionPipeline pipeline, out string error)
        {
            pipeline = null;
            error = string.Empty;
            OpenVisionRecipePipelineOption option = SelectedPipelineOption;
            if (option == null)
            {
                error = LocalText(
                    "판정 기준을 보려면 파이프라인을 선택하십시오.",
                    "Select a pipeline to review the acceptance gate.");
                return false;
            }

            string path = RecipeWorkspaceService.GetVisionPipelinePath(
                NormalizeRecipeName(selectedRecipeName),
                option.PipelineName);
            if (!VisionPipelineStorage.TryLoadFromFile(path, out pipeline, out string loadError) || pipeline == null)
            {
                error = LocalText("파이프라인 XML을 읽지 못했습니다: ", "Pipeline XML could not be read: ") + loadError;
                return false;
            }

            return true;
        }

        private static List<VisionPipelineStep> GetEnabledAcceptanceSteps(VisionPipeline pipeline)
        {
            return pipeline?.Steps?
                .Where(step => step != null && step.Enabled && step.UseAcceptance)
                .ToList()
                ?? new List<VisionPipelineStep>();
        }

        private string FormatValidationSetAcceptanceGate(VisionPipelineStep step)
        {
            string metric = step?.AcceptanceMetricName?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(metric))
            {
                return step?.ExpectedSuccess == false
                    ? LocalText("Step 상태 = NG", "Step status = NG")
                    : LocalText("Step 상태 = OK", "Step status = OK");
            }

            if (step.UseAcceptanceMetricMinimum && step.UseAcceptanceMetricMaximum)
            {
                return metric + " "
                    + step.AcceptanceMetricMinimum.ToString("0.######", CultureInfo.InvariantCulture)
                    + ".."
                    + step.AcceptanceMetricMaximum.ToString("0.######", CultureInfo.InvariantCulture);
            }

            if (step.UseAcceptanceMetricMinimum)
            {
                return metric + " >= " + step.AcceptanceMetricMinimum.ToString("0.######", CultureInfo.InvariantCulture);
            }

            if (step.UseAcceptanceMetricMaximum)
            {
                return metric + " <= " + step.AcceptanceMetricMaximum.ToString("0.######", CultureInfo.InvariantCulture);
            }

            return metric;
        }

        private static bool TryParsePositiveDouble(string value, out double result)
        {
            return (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out result)
                    || double.TryParse(value, NumberStyles.Float, CultureInfo.CurrentCulture, out result))
                && result > 0D;
        }



        private OpenVisionRecipeOperatorDecisionPresentation BuildOperatorDecisionPresentation()
        {
            OpenVisionRecipePairSampleRunSummary selectedRole = SelectedPairSampleResult;
            OpenVisionRecipeSampleMatrixRow selectedMatrix = SelectedSampleMatrixRow;
            OpenVisionRecipeBatchSampleResultOption selectedBatchSample = SelectedRecentBatchSampleResultOption;
            OpenVisionRecipeBatchRunComparisonRow selectedBatchComparison = SelectedRecentBatchRunComparisonRow;
            string evidenceFailedStepName = OpenVisionRecipeOperatorDecisionPresenter.ResolveEvidenceFailedStepName(
                selectedRole,
                selectedMatrix,
                selectedBatchSample,
                selectedBatchComparison);
            OpenVisionRecipePipelineStepPreview evidenceStep = !string.IsNullOrWhiteSpace(evidenceFailedStepName)
                ? FindPipelinePreviewStep(evidenceFailedStepName)
                : SelectedPipelinePreviewStep;
            OpenVisionRecipePipelineStepPreview handoffStep = selectedRole?.CanOpenFailedStep == true
                ? FindPipelinePreviewStep(selectedRole.FailedStepText)
                : SelectedPipelinePreviewStep;

            return OpenVisionRecipeOperatorDecisionPresenter.Build(
                new OpenVisionRecipeOperatorDecisionRequest(
                    SelectedRecipeSummary,
                    LatestSampleRunSummary,
                    LatestPairRunSummary,
                    LatestCatalogBenchmarkSummary,
                    RecentBatchRunComparisonRows,
                    RecentBatchRunComparisonSummaryText,
                    selectedMatrix,
                    selectedRole,
                    selectedBatchSample,
                    selectedBatchComparison,
                    SelectedSampleOption?.Sample?.ExpectedText,
                    evidenceStep,
                    handoffStep));
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

        private bool CanRunRecipeGuidedNextAction()
        {
            return OpenVisionRecipeGuidedWorkflowPresenter.ResolveNextAction(
                CreateRecipeGuidedWorkflowActionRequest()) != OpenVisionRecipeGuidedWorkflowAction.None;
        }

        private void RunRecipeGuidedNextAction()
        {
            switch (OpenVisionRecipeGuidedWorkflowPresenter.ResolveNextAction(
                CreateRecipeGuidedWorkflowActionRequest()))
            {
                case OpenVisionRecipeGuidedWorkflowAction.ValidateLlmXmlDraft:
                    ValidateLlmXmlDraft();
                    return;
                case OpenVisionRecipeGuidedWorkflowAction.DuplicatePipelineFromSample:
                    DuplicatePipelineFromSample();
                    return;
                case OpenVisionRecipeGuidedWorkflowAction.ActivateSelectedPipeline:
                    ActivateSelectedPipeline();
                    return;
                case OpenVisionRecipeGuidedWorkflowAction.RunSelectedSampleCheck:
                    RunSelectedSampleCheck();
                    return;
                case OpenVisionRecipeGuidedWorkflowAction.LoadSelectedStepParameters:
                    LoadSelectedStepParameters();
                    return;
                case OpenVisionRecipeGuidedWorkflowAction.RunSelectedSamplePairCheck:
                    RunSelectedSamplePairCheck();
                    return;
                case OpenVisionRecipeGuidedWorkflowAction.OpenSelectedStepTool:
                    OpenSelectedStepTool();
                    return;
                default:
                    StatusText = LocalText("현재 실행할 다음 가이드 작업이 없습니다.", "No guided next action is available.");
                    return;
            }
        }

        private OpenVisionRecipeGuidedWorkflowActionRequest CreateRecipeGuidedWorkflowActionRequest()
        {
            return new OpenVisionRecipeGuidedWorkflowActionRequest
            {
                Summary = SelectedRecipeSummary,
                Sample = LatestSampleRunSummary,
                Pair = LatestPairRunSummary,
                CanValidateLlmXmlDraft = CanUseLlmXmlDraft(),
                CanDuplicatePipelineFromSample = CanDuplicatePipelineFromSample(),
                CanActivateSelectedPipeline = CanUseSelectedPipeline(),
                CanRunSelectedSampleCheck = CanRunSelectedSampleCheck(),
                CanLoadSelectedStepParameters = CanLoadSelectedStepParameters(),
                CanRunSelectedSamplePairCheck = CanRunSelectedSamplePairCheck(),
                CanOpenSelectedStepTool = CanOpenSelectedStepTool()
            };
        }

        private void RefreshRecentBatchRunComparison()
        {
            OpenVisionRecipeBatchRunOption currentOption = SelectedRecentBatchRunOption;
            OpenVisionRecipeBatchRunOption baselineOption =
                OpenVisionRecipeRunHistoryPresenter.ResolveBaselineRunOption(
                    SelectedBenchmarkBaselineRunOption,
                    currentOption,
                    RecentBatchRunOptions);
            VisionPipelineBatchRunSummary currentSummary = null;
            VisionPipelineBatchRunSummary baselineSummary = null;
            if (currentOption != null
                && baselineOption != null
                && !string.IsNullOrWhiteSpace(currentOption.SummaryPath)
                && !string.IsNullOrWhiteSpace(baselineOption.SummaryPath))
            {
                currentSummary = VisionPipelineBatchRunSummaryStorage.Load(currentOption.SummaryPath);
                baselineSummary = VisionPipelineBatchRunSummaryStorage.Load(baselineOption.SummaryPath);
            }

            IReadOnlyList<OpenVisionRecipeBatchRunComparisonRow> rows =
                OpenVisionRecipeRunHistoryPresenter.BuildComparisonRows(
                    currentOption,
                    baselineOption,
                    currentSummary,
                    baselineSummary);
            RecentBatchRunComparisonRows = rows;
            SelectedRecentBatchRunComparisonRow =
                OpenVisionRecipeRunHistoryPresenter.SelectDefaultComparisonRow(rows);
            OnPropertyChanged(nameof(RecentBatchRunComparisonSummaryText));
            OnPropertyChanged(nameof(SelectedRecentBatchRunComparisonReviewText));
        }










        private OpenVisionRecipePipelineEditValidationRequest CreatePipelineEditValidationRequest()
        {
            return new OpenVisionRecipePipelineEditValidationRequest
            {
                SelectedRecipeName = NormalizeRecipeName(selectedRecipeName),
                RecipeNames = RecipeOptions,
                HasSelectedPipelineOption = SelectedPipelineOption != null,
                SelectedPipelineName = SelectedPipelineOption?.PipelineName,
                RequestedPipelineName = PipelineEditName,
                NormalizedPipelineName = NormalizePipelineName(PipelineEditName),
                PipelineNames = PipelineOptions
                    .Select(option => option.PipelineName)
                    .ToArray()
            };
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

            if (IsReviewBundlePath(path))
            {
                return LoadReviewBundleForDryRun(path);
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

            if (IsReviewBundlePath(path))
            {
                return LoadReviewBundleForDryRun(path);
            }

            LlmXmlDraftText = File.ReadAllText(path);
            StatusText = LocalText("LLM XML 초안 로드됨: ", "Loaded LLM XML draft: ") + Path.GetFileName(path);
            return ValidateLlmXmlDraftText(false);
        }

        private bool LoadReviewBundleForDryRun(string path)
        {
            if (!OpenVisionRecipeReviewBundleInspector.TryInspect(path, out OpenVisionRecipeReviewBundleInspection inspection))
            {
                ClearLoadedReviewBundleContext();
                LlmXmlDraftValidationReport = inspection.IntegrityReport;
                LlmXmlDraftDependencyReport = inspection.PathReport;
                SetLlmXmlDraftDependencyPlaceholder(LocalText(
                    "번들 무결성 오류를 해결한 뒤 다시 선택하세요.",
                    "Fix the bundle integrity issue, then select it again."));
                StatusText = LocalText("검토 번들 dry-run NG. 가져오지 않았습니다.", "Review bundle dry-run NG. Nothing was imported.");
                openLlmXmlReview();
                return false;
            }

            LlmXmlDraftText = inspection.PipelineXml;
            loadedReviewBundleInspection = inspection;
            OnPropertyChanged(nameof(LlmDraftValidationText));
            OnPropertyChanged(nameof(LlmDependencyReportText));
            bool xmlReady = ValidateLlmXmlDraftText(false);
            StatusText = xmlReady
                ? LocalText(
                    "검토 번들 dry-run OK. XML은 검토 화면에만 로드했으며 가져오기/Preview/Run은 실행하지 않았습니다.",
                    "Review bundle dry-run OK. XML was loaded for review only; import, Preview, and Run were not executed.")
                : LocalText(
                    "검토 번들 무결성은 OK지만 XML/의존성 검토는 NG입니다. 가져오기/Preview/Run은 실행하지 않았습니다.",
                    "Review bundle integrity is OK, but XML/dependency review is NG. Import, Preview, and Run were not executed.");
            openLlmXmlReview();
            return true;
        }

        private static bool IsReviewBundlePath(string path)
        {
            return !string.IsNullOrWhiteSpace(path)
                && (path.EndsWith(".review.zip", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(Path.GetExtension(path), ".zip", StringComparison.OrdinalIgnoreCase));
        }

        private void ClearLoadedReviewBundleContext()
        {
            if (loadedReviewBundleInspection == null)
            {
                return;
            }

            loadedReviewBundleInspection = null;
            OnPropertyChanged(nameof(LlmDraftValidationText));
            OnPropertyChanged(nameof(LlmDependencyReportText));
            LlmXmlDraftValidationReport = LocalText(
                "검토 번들에서 로드한 XML이 변경되었습니다. 다시 검증하세요.",
                "XML loaded from the review bundle changed. Validate it again.");
            LlmXmlDraftDependencyReport = LocalText(
                "번들 경로 증거 연결이 해제되었습니다.",
                "Bundle path evidence was detached.");
            SetLlmXmlDraftDependencyPlaceholder(LocalText(
                "변경된 XML을 다시 검증하세요.",
                "Validate the changed XML again."));
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
                llmXmlDraftImportReady = false;
                LlmXmlDraftValidationReport = validationReport;
                LlmXmlDraftDependencyReport = dependencyReport;
                LlmXmlDraftReviewReport = LocalText("초안 검토 건너뜀: 검증 실패.", "Draft review skipped: validation failed.");
                LlmXmlDraftDiffReport = LocalText("변경점 검토 건너뜀: 검증 실패.", "Diff review skipped: validation failed.");
                StatusText = LocalText("LLM XML 초안을 가져올 수 없습니다.", "LLM XML draft is not importable.");
                RefreshCommandState();
                return;
            }

            string recipeName = NormalizeRecipeName(selectedRecipeName);
            string basePipelineName = string.IsNullOrWhiteSpace(pipeline.Name)
                ? "LLM_Draft_" + DateTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture)
                : pipeline.Name.Trim();
            pipeline.Name = CreateUniquePipelineName(recipeName, basePipelineName);
            if (OpenVisionRecipeDependencyReviewService.TryCopyReferenceImageToRecipe(
                recipeName,
                pipeline.Name,
                LlmReferenceImagePath,
                out string copiedReferenceImagePath))
            {
                dependencyReport = string.IsNullOrWhiteSpace(dependencyReport)
                    ? LocalText("참조 이미지 복사됨: ", "Reference image copied: ") + copiedReferenceImagePath
                    : dependencyReport + Environment.NewLine + LocalText("참조 이미지 복사됨: ", "Reference image copied: ") + copiedReferenceImagePath;
            }
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

        private void SuggestPinGapIntentRoiSamples()
        {
            string imagePath = ResolvePinGapRoiSuggestionImagePath();
            if (string.IsNullOrWhiteSpace(imagePath))
            {
                StatusText = LocalText(
                    "핀 간격 ROI를 제안할 샘플/참조 이미지가 없습니다.",
                    "No sample/reference image is available for pin gap ROI suggestion.");
                return;
            }

            try
            {
                BitmapFrame frame = BitmapFrame.Create(
                    new Uri(imagePath, UriKind.Absolute),
                    BitmapCreateOptions.DelayCreation,
                    BitmapCacheOption.OnLoad);
                IReadOnlyList<OpenVisionRecipePinGapIntentSkill.RoiSample> samples =
                    OpenVisionRecipePinGapIntentSkill.CreateScaledRoiSamples(frame.PixelWidth, frame.PixelHeight);
                PinGapIntentRoiText = OpenVisionRecipePinGapIntentSkill.FormatRoiSamples(samples);
                StatusText = LocalText(
                    "핀 간격 ROI 샘플 제안: ",
                    "Suggested pin gap ROI samples: ")
                    + Path.GetFileName(imagePath)
                    + " ("
                    + frame.PixelWidth.ToString(CultureInfo.InvariantCulture)
                    + "x"
                    + frame.PixelHeight.ToString(CultureInfo.InvariantCulture)
                    + ")";
            }
            catch (Exception ex)
            {
                StatusText = LocalText(
                    "핀 간격 ROI 제안 실패: ",
                    "Pin gap ROI suggestion failed: ")
                    + ex.GetBaseException().Message;
            }
        }

        private async void RunValidationSuite()
        {
            if (!CanRunValidationSuite())
            {
                return;
            }

            string scope = SelectedValidationSuiteScopeOption?.Key ?? OpenVisionRecipeValidationSuiteScopeOption.SelectedSampleKey;
            if (string.Equals(scope, OpenVisionRecipeValidationSuiteScopeOption.LocalValidationSetKey, StringComparison.OrdinalIgnoreCase))
            {
                await RunLocalValidationSetAsync();
                return;
            }

            if (string.Equals(scope, OpenVisionRecipeValidationSuiteScopeOption.GoodBadPairKey, StringComparison.OrdinalIgnoreCase))
            {
                ValidationSuiteStatusText = LocalText("Good/Bad suite 실행 시작.", "Started Good/Bad suite.");
                RunSelectedSamplePairCheck();
                return;
            }

            if (string.Equals(scope, OpenVisionRecipeValidationSuiteScopeOption.CatalogKey, StringComparison.OrdinalIgnoreCase))
            {
                ValidationSuiteStatusText = LocalText("Catalog suite 실행 시작.", "Started catalog suite.");
                RunCatalogBenchmark();
                return;
            }

            await RunSelectedSampleValidationSuiteAsync();
        }

        private async Task RunLocalValidationSetAsync()
        {
            OpenVisionRecipeValidationSetOption option = SelectedValidationSetOption;
            if (option?.Set == null || !CanRunLocalValidationSet())
            {
                return;
            }

            string setName = option.Name;
            string setNotes = option.Set.Notes ?? string.Empty;
            List<OpenVisionRecipeValidationSetImage> images = option.Set.Images
                .Where(image => image != null)
                .Select(image => new OpenVisionRecipeValidationSetImage
                {
                    Expected = image.Expected,
                    Path = image.Path,
                    Notes = image.Notes
                })
                .ToList();
            string recipeName = NormalizeRecipeName(selectedRecipeName);
            string pipelineName = SelectedPipelineOption?.PipelineName ?? string.Empty;
            string pipelinePath = RecipeWorkspaceService.GetVisionPipelinePath(recipeName, pipelineName);

            isValidationSuiteRunning = true;
            isLocalValidationSetRunning = true;
            validationSuiteStopRequested = false;
            OnPropertyChanged(nameof(RunValidationSuiteText));
            OnPropertyChanged(nameof(StopValidationSuiteText));
            OnPropertyChanged(nameof(IsLocalValidationSetRunning));
            ValidationSuiteStatusText = LocalText("로컬 세트 실행 중: ", "Running local set: ") + setName;
            StatusText = ValidationSuiteStatusText;
            RefreshCommandState();

            DateTime startedAt = DateTime.Now;
            List<VisionPipelineBatchSampleRunResult> storageResults = new List<VisionPipelineBatchSampleRunResult>();
            try
            {
                string pipelineXmlText = File.ReadAllText(pipelinePath);
                if (!OpenVisionRecipeValidationSetStorage.TryValidateFrozenIdentity(
                        option.Set,
                        pipelineName,
                        pipelineXmlText,
                        out string identityError))
                {
                    throw new InvalidDataException(identityError);
                }

                for (int index = 0; index < images.Count; index++)
                {
                    if (validationSuiteStopRequested)
                    {
                        break;
                    }

                    OpenVisionRecipeValidationSetImage image = images[index];
                    VisionPipelineSampleCatalogItem sample = CreateLocalValidationSample(setName, image, index);
                    VisionPipelineSampleCheckResult result =
                        await VisionPipelineSampleCheckService.RunSampleCheckWithReportSafeAsync(sample, pipelineXmlText, recipeName);
                    VisionPipelineBatchSampleRunResult storageResult = CreateBatchSampleRunResult(sample, result);
                    storageResult.Success = image.IsExpectedNg ? !result.Success : result.Success;
                    storageResult.Status = storageResult.Success ? "OK" : "NG";
                    storageResult.ExpectedText = "ExpectedActual: Expected " + image.Expected;
                    if (!string.IsNullOrWhiteSpace(image.Notes))
                    {
                        storageResult.Message = string.IsNullOrWhiteSpace(storageResult.Message)
                            ? "Note: " + image.Notes
                            : storageResult.Message + " | Note: " + image.Notes;
                    }

                    storageResults.Add(storageResult);
                    ValidationSuiteStatusText = string.Format(
                        CultureInfo.CurrentCulture,
                        LocalText("로컬 세트 실행 중: {0} ({1}/{2})", "Running local set: {0} ({1}/{2})"),
                        setName,
                        index + 1,
                        images.Count);
                }

                bool isPartial = validationSuiteStopRequested && storageResults.Count < images.Count;
                string savedNotes = isPartial
                    ? AppendPartialValidationSetNote(setNotes, storageResults.Count, images.Count)
                    : setNotes;
                string summaryPath = VisionPipelineBatchRunSummaryStorage.Save(
                    recipeName,
                    pipelineName,
                    startedAt,
                    DateTime.Now,
                    storageResults,
                    setName,
                    isPartial ? "LocalValidationSetPartial" : "LocalValidationSet",
                    savedNotes);
                RefreshRecentBatchRunOptions();
                int correct = storageResults.Count(IsExpectedOutcomeCorrect);
                ValidationSuiteStatusText = string.Format(
                    CultureInfo.CurrentCulture,
                    isPartial
                        ? LocalText("목록 검증 중단·부분 저장: {0}/{1} 판정 일치 | {2}", "Image-list run stopped and partially saved: {0}/{1} judgments matched | {2}")
                        : LocalText("목록 검증 저장됨: {0}/{1} 판정 일치 | {2}", "Image-list run saved: {0}/{1} judgments matched | {2}"),
                    correct,
                    storageResults.Count,
                    summaryPath);
                StatusText = ValidationSuiteStatusText;
            }
            catch (Exception ex)
            {
                ValidationSuiteStatusText = LocalText("로컬 세트 ERROR: ", "Local set ERROR: ") + ex.GetBaseException().Message;
                StatusText = ValidationSuiteStatusText;
            }
            finally
            {
                isValidationSuiteRunning = false;
                isLocalValidationSetRunning = false;
                validationSuiteStopRequested = false;
                OnPropertyChanged(nameof(RunValidationSuiteText));
                OnPropertyChanged(nameof(StopValidationSuiteText));
                OnPropertyChanged(nameof(IsLocalValidationSetRunning));
                OnPropertyChanged(nameof(ValidationSuiteSummaryText));
                RefreshCommandState();
            }
        }

        private bool CanStopValidationSuite()
        {
            return isLocalValidationSetRunning && !validationSuiteStopRequested;
        }

        private void RequestValidationSuiteStop()
        {
            if (!CanStopValidationSuite())
            {
                return;
            }

            validationSuiteStopRequested = true;
            ValidationSuiteStatusText = LocalText(
                "현재 이미지 완료 후 중지하고 부분 결과를 저장합니다.",
                "Stopping after the current image and saving a partial result.");
            StatusText = ValidationSuiteStatusText;
            OnPropertyChanged(nameof(StopValidationSuiteText));
            RefreshCommandState();
        }

        private static bool IsExpectedOutcomeCorrect(VisionPipelineBatchSampleRunResult result)
        {
            return OpenVisionRecipeBatchSampleResultOption.TryResolveExpectedSuccess(result, out bool expectedSuccess)
                && expectedSuccess == result.Success;
        }

        private static string AppendPartialValidationSetNote(string notes, int completed, int total)
        {
            string partial = "Partial run: completed "
                + completed.ToString(CultureInfo.InvariantCulture)
                + "/"
                + total.ToString(CultureInfo.InvariantCulture)
                + ". This is not a full-set accuracy or timing baseline.";
            return string.IsNullOrWhiteSpace(notes) ? partial : notes.Trim() + " | " + partial;
        }

        private static VisionPipelineSampleCatalogItem CreateLocalValidationSample(
            string setName,
            OpenVisionRecipeValidationSetImage image,
            int index)
        {
            return new VisionPipelineSampleCatalogItem
            {
                SampleName = (index + 1).ToString("000", CultureInfo.InvariantCulture)
                    + " "
                    + Path.GetFileName(image.Path),
                ImagePath = image.Path,
                ImageFullPath = image.Path,
                ValidationMode = image.IsExpectedNg ? "ExpectedFailure" : "ExpectedSuccess",
                PairGroup = setName ?? string.Empty,
                PairRole = image.Expected ?? OpenVisionRecipeValidationSetImage.ExpectedOk,
                Notes = image.Notes ?? string.Empty,
                CatalogSourceKind = VisionPipelineSampleCatalogSourceKind.Unknown
            };
        }

        private async Task RunSelectedSampleValidationSuiteAsync()
        {
            OpenVisionRecipeSampleOption sampleOption = SelectedSampleOption;
            string recipeName = NormalizeRecipeName(selectedRecipeName);
            string pipelineName = SelectedPipelineOption?.PipelineName ?? string.Empty;
            string pipelinePath = RecipeWorkspaceService.GetVisionPipelinePath(recipeName, pipelineName);

            isValidationSuiteRunning = true;
            isSampleCheckRunning = true;
            OnPropertyChanged(nameof(RunValidationSuiteText));
            OnPropertyChanged(nameof(RunSelectedSampleCheckText));
            LatestSampleRunSummary = OpenVisionRecipeSampleRunSummary.CreateRunning(sampleOption, recipeName, pipelineName);
            ValidationSuiteStatusText = LocalText("Selected sample suite 실행 중: ", "Running selected-sample suite: ") + sampleOption.SampleName;
            StatusText = ValidationSuiteStatusText;
            RefreshCommandState();

            DateTime startedAt = DateTime.Now;
            try
            {
                string pipelineXmlText = File.ReadAllText(pipelinePath);
                VisionPipelineSampleCheckResult result =
                    await VisionPipelineSampleCheckService.RunSampleCheckWithReportSafeAsync(sampleOption.Sample, pipelineXmlText, recipeName);
                LatestSampleRunSummary = OpenVisionRecipeSampleRunSummary.FromResult(sampleOption, recipeName, pipelineName, result);

                string summaryPath = VisionPipelineBatchRunSummaryStorage.Save(
                    recipeName,
                    pipelineName,
                    startedAt,
                    DateTime.Now,
                    new[] { CreateBatchSampleRunResult(sampleOption.Sample, result) },
                    "Selected:" + sampleOption.SampleName,
                    "SelectedSample");
                RefreshRecentBatchRunOptions();
                ValidationSuiteStatusText = LocalText("Selected sample suite 저장됨: ", "Selected-sample suite saved: ") + summaryPath;
                StatusText = LocalText("샘플 검사 ", "Sample check ") + result.Status + ": " + sampleOption.SampleName;
            }
            catch (Exception ex)
            {
                VisionPipelineSampleCheckResult result = VisionPipelineSampleCheckService.CreateErrorResult(
                    ex.GetBaseException().Message);
                LatestSampleRunSummary = OpenVisionRecipeSampleRunSummary.FromResult(sampleOption, recipeName, pipelineName, result);
                ValidationSuiteStatusText = LocalText("Selected sample suite ERROR: ", "Selected-sample suite ERROR: ") + result.Message;
                StatusText = ValidationSuiteStatusText;
            }
            finally
            {
                isSampleCheckRunning = false;
                isValidationSuiteRunning = false;
                OnPropertyChanged(nameof(RunSelectedSampleCheckText));
                OnPropertyChanged(nameof(RunValidationSuiteText));
                OnPropertyChanged(nameof(ValidationSuiteSummaryText));
                RefreshCommandState();
            }
        }

        private async void RunSelectedSampleCheck()
        {
            if (!CanRunSelectedSampleCheck())
            {
                return;
            }

            OpenVisionRecipeSampleOption sampleOption = SelectedSampleOption;
            string recipeName = NormalizeRecipeName(selectedRecipeName);
            string pipelineName = SelectedPipelineOption?.PipelineName ?? string.Empty;
            string pipelinePath = RecipeWorkspaceService.GetVisionPipelinePath(
                recipeName,
                pipelineName);

            isSampleCheckRunning = true;
            OnPropertyChanged(nameof(RunSelectedSampleCheckText));
            LatestSampleRunSummary = OpenVisionRecipeSampleRunSummary.CreateRunning(sampleOption, recipeName, pipelineName);
            StatusText = LocalText("샘플 검사 실행 중: ", "Running sample check: ") + sampleOption.SampleName;
            RefreshCommandState();

            try
            {
                string pipelineXmlText = File.ReadAllText(pipelinePath);
                VisionPipelineSampleCheckResult result =
                    await VisionPipelineSampleCheckService.RunSampleCheckSafeAsync(sampleOption.Sample, pipelineXmlText);
                LatestSampleRunSummary = OpenVisionRecipeSampleRunSummary.FromResult(sampleOption, recipeName, pipelineName, result);
                StatusText = LocalText("샘플 검사 ", "Sample check ") + result.Status + ": " + sampleOption.SampleName;
            }
            catch (Exception ex)
            {
                VisionPipelineSampleCheckResult result = VisionPipelineSampleCheckService.CreateErrorResult(
                    ex.GetBaseException().Message);
                LatestSampleRunSummary = OpenVisionRecipeSampleRunSummary.FromResult(sampleOption, recipeName, pipelineName, result);
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
                        await VisionPipelineSampleCheckService.RunSampleCheckWithReportSafeAsync(sample, pipelineXmlText, recipeName);
                    pairResults.Add(OpenVisionRecipePairSampleRunSummary.FromResult(sample, result));
                    storageResults.Add(CreateBatchSampleRunResult(sample, result));
                }

                summaryPath = VisionPipelineBatchRunSummaryStorage.Save(
                    recipeName,
                    pipelineName,
                    startedAt,
                    DateTime.Now,
                    storageResults,
                    "Pair:" + (sampleOption.Sample.PairGroup ?? string.Empty),
                    "GoodBadPair");
                LatestPairRunSummary = OpenVisionRecipePairRunSummary.FromResults(
                    sampleOption,
                    pipelineName,
                    pairResults,
                    summaryPath);
                RefreshRecentBatchRunOptions();
                ValidationSuiteStatusText = LocalText("Good/Bad suite 저장됨: ", "Good/Bad suite saved: ") + summaryPath;
                StatusText = LatestPairRunSummary.StatusText + ": " + sampleOption.Sample.PairGroup;
            }
            catch (Exception ex)
            {
                LatestPairRunSummary = OpenVisionRecipePairRunSummary.FromError(
                    sampleOption,
                    pipelineName,
                    ex.GetBaseException().Message);
                ValidationSuiteStatusText = LocalText("Good/Bad suite ERROR: ", "Good/Bad suite ERROR: ") + ex.GetBaseException().Message;
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
                        await VisionPipelineSampleCheckService.RunSampleCheckWithReportSafeAsync(sample, pipelineXmlText, recipeName);

                    storageResults.Add(CreateBatchSampleRunResult(sample, result, FormatCatalogBenchmarkMessage(result)));

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
                    storageResults,
                    "Catalog",
                    "Catalog");
                LatestCatalogBenchmarkSummary = OpenVisionRecipeCatalogBenchmarkSummary.FromResults(
                    pipelineName,
                    storageResults,
                    summaryPath);
                RefreshRecentBatchRunOptions();
                ValidationSuiteStatusText = LocalText("Catalog suite 저장됨: ", "Catalog suite saved: ") + summaryPath;
                StatusText = LatestCatalogBenchmarkSummary.CompactText;
            }
            catch (Exception ex)
            {
                LatestCatalogBenchmarkSummary = OpenVisionRecipeCatalogBenchmarkSummary.FromError(
                    pipelineName,
                    ex.GetBaseException().Message);
                ValidationSuiteStatusText = LocalText("Catalog suite ERROR: ", "Catalog suite ERROR: ") + ex.GetBaseException().Message;
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
            return OpenVisionRecipeLlmReviewBundleBuilder.Build(new OpenVisionRecipeLlmReviewBundleRequest
            {
                RecipeName = recipeName,
                PipelineName = SelectedPipelineOption?.PipelineName ?? string.Empty,
                Template = SelectedLlmToolTemplate,
                SelectedStepOperatorContextText = PipelineSelectedStepOperatorContextText,
                FailureReviewText = FailureReviewText,
                ValidationReport = LlmXmlDraftValidationReport,
                DependencyReport = LlmXmlDraftDependencyReport,
                DraftReviewReport = LlmXmlDraftReviewReport,
                DiffReport = LlmXmlDraftDiffReport,
                XmlDraftText = LlmXmlDraftText
            });
        }

        internal string BuildLlmReviewBundleTextForTest()
        {
            return BuildLlmReviewBundleText();
        }

        private void CreateLlmTemplateXmlDraft()
        {
            VisionPipeline pipeline = CreateLlmTemplatePipeline();
            LlmPromptText = BuildLlmPromptText();
            LlmXmlDraftText = SerializePipelineToXmlText(pipeline);
            ValidateLlmXmlDraftText(false);
            StatusText = LocalText(
                "검사 설정 XML 초안을 만들었습니다. Preview/Run은 실행하지 않았습니다: ",
                "Created Guided setup draft XML. Preview/Run was not executed: ")
                + SelectedLlmToolTemplate;
        }

        private void CreateGuidedSetupStarterXml()
        {
            if (OpenVisionRecipeLlmIntent.IsHybridRelativeRoiGapTemplate(SelectedLlmToolTemplate))
            {
                CreateHybridRelativeRoiIntentXmlDraft();
            }
            else if (OpenVisionRecipeLlmIntent.IsPinArrayGapTemplate(SelectedLlmToolTemplate))
            {
                CreatePinArrayGapIntentXmlDraft();
            }
            else if (OpenVisionRecipeLlmIntent.IsDarkBandGapTemplate(SelectedLlmToolTemplate))
            {
                CreateDarkBandGapIntentXmlDraft();
            }
            else if (IsLineDistanceTemplate(SelectedLlmToolTemplate))
            {
                CreatePinGapIntentXmlDraft();
            }
            else if (IsBlobTemplate(SelectedLlmToolTemplate))
            {
                CreateBlobCountIntentXmlDraft();
            }
            else if (IsContourTemplate(SelectedLlmToolTemplate))
            {
                CreateContourCountIntentXmlDraft();
            }
            else if (IsEdgeBasedTemplate(SelectedLlmToolTemplate))
            {
                CreateEdgeBasedIntentXmlDraft();
            }
            else if (IsFeatureMatchingTemplate(SelectedLlmToolTemplate))
            {
                CreateFeatureMatchingIntentXmlDraft();
            }
            else if (IsMatchingTemplate(SelectedLlmToolTemplate))
            {
                CreateMatchingIntentXmlDraft();
            }
            else if (IsReferenceDifferenceTemplate(SelectedLlmToolTemplate))
            {
                CreateReferenceDifferenceIntentXmlDraft();
            }
            else if (IsMeanTemplate(SelectedLlmToolTemplate))
            {
                CreateMeanIntentXmlDraft();
            }
            else
            {
                CreateLlmTemplateXmlDraft();
            }

            if (llmXmlDraftImportReady && !string.IsNullOrWhiteSpace(LlmXmlDraftText))
            {
                SetGuidedSetupDraftStale(false);
            }
        }

        private bool CanCreateGuidedSetupStarterXml()
        {
            return CanUseSelectedRecipe() && IsGuidedSetupIntentInputReady;
        }

        private bool IsPinGapPixelOnly => string.IsNullOrWhiteSpace(PinGapIntentScaleText);

        private string PinGapIntentUnitText => IsPinGapPixelOnly ? "px" : "mm";

        private string PinGapIntentAverageMetricName => IsPinGapPixelOnly
            ? VisionPipelineKnownMetrics.DistancePxAvg
            : VisionPipelineKnownMetrics.DistanceMmAvg;

        private string PinGapIntentRangeMetricName => IsPinGapPixelOnly
            ? VisionPipelineKnownMetrics.DistancePxRange
            : VisionPipelineKnownMetrics.DistanceMmRange;

        private bool CanFreezePinArrayGapValidationIdentity()
        {
            return OpenVisionRecipeLlmIntent.IsPinArrayGapTemplate(SelectedLlmToolTemplate)
                && !isValidationSuiteRunning
                && validationSetStorageReady
                && CanUseSelectedPipeline()
                && PinArrayGapTrainValidationSetOption != null
                && PinArrayGapValidationValidationSetOption != null
                && PinArrayGapTestValidationSetOption != null;
        }

        private void FreezePinArrayGapValidationIdentity()
        {
            if (!CanFreezePinArrayGapValidationIdentity())
            {
                return;
            }

            string recipeName = NormalizeRecipeName(selectedRecipeName);
            if (!TryReadSelectedPipelineXml(out string pipelineXmlText, out string error)
                || !OpenVisionRecipePinArrayGapValidationRecordStorage.TrySave(
                    recipeName,
                    pipelineXmlText,
                    PinArrayGapTrainValidationSetOption,
                    PinArrayGapValidationValidationSetOption,
                    PinArrayGapTestValidationSetOption,
                    out OpenVisionRecipePinArrayGapValidationRecord record,
                    out error))
            {
                IsPinArrayGapValidationIdentityFrozen = false;
                PinArrayGapValidationStatusText = LocalText("2단계 검토 필요 | ", "PHASE 2 REVIEW | ") + error;
                StatusText = PinArrayGapValidationStatusText;
                RefreshCommandState();
                return;
            }

            IsPinArrayGapValidationIdentityFrozen = true;
            PinArrayGapValidationStatusText = BuildPinArrayGapFrozenStatus(record);
            StatusText = PinArrayGapValidationStatusText;
            RefreshCommandState();
        }

        private bool CanOpenPinArrayGapValidationRuns()
        {
            return OpenVisionRecipeLlmIntent.IsPinArrayGapTemplate(SelectedLlmToolTemplate)
                && validationSetStorageReady
                && CanUseSelectedPipeline()
                && PinArrayGapTrainValidationSetOption != null;
        }

        private void OpenPinArrayGapValidationRuns()
        {
            if (!CanOpenPinArrayGapValidationRuns())
            {
                return;
            }

            SelectedValidationSetOption = PinArrayGapTrainValidationSetOption;
            SelectLocalValidationSetScope();
            openPinArrayGapValidationRuns();
            StatusText = LocalText(
                "Train 세트를 선택했습니다. Validation Set 화면에서 Run suite를 명시적으로 실행하세요.",
                "Train set selected. Explicitly run the suite in the Validation Set screen.");
        }

        private void RefreshPinArrayGapValidationIdentityState()
        {
            IsPinArrayGapValidationIdentityFrozen = false;
            if (SelectedPipelineOption == null
                || PinArrayGapTrainValidationSetOption == null
                || PinArrayGapValidationValidationSetOption == null
                || PinArrayGapTestValidationSetOption == null)
            {
                PinArrayGapValidationStatusText = string.Empty;
                return;
            }

            string recipeName = NormalizeRecipeName(selectedRecipeName);
            if (!TryReadSelectedPipelineXml(out string pipelineXmlText, out string error))
            {
                PinArrayGapValidationStatusText = LocalText("2단계 검토 필요 | ", "PHASE 2 REVIEW | ") + error;
                return;
            }

            if (!OpenVisionRecipePinArrayGapValidationRecordStorage.TryLoad(
                    recipeName,
                    out OpenVisionRecipePinArrayGapValidationRecord record,
                    out error))
            {
                PinArrayGapValidationStatusText = error.Contains("does not exist", StringComparison.OrdinalIgnoreCase)
                    ? LocalText(
                        "2단계 미고정 | 세 분할과 선택된 XML을 검토한 뒤 검증 기준을 고정하세요.",
                        "PHASE 2 NOT FROZEN | Review the three splits and selected XML, then freeze identity.")
                    : LocalText("2단계 검토 필요 | ", "PHASE 2 REVIEW | ") + error;
                return;
            }

            if (!OpenVisionRecipePinArrayGapValidationRecordStorage.TryMatchesCurrent(
                    recipeName,
                    pipelineXmlText,
                    PinArrayGapTrainValidationSetOption,
                    PinArrayGapValidationValidationSetOption,
                    PinArrayGapTestValidationSetOption,
                    record,
                    out bool matches,
                    out error))
            {
                PinArrayGapValidationStatusText = LocalText("2단계 검토 필요 | ", "PHASE 2 REVIEW | ") + error;
                return;
            }

            if (!matches)
            {
                PinArrayGapValidationStatusText = LocalText(
                    "2단계 변경됨 | XML 또는 세트 내용이 고정 기록과 다릅니다. 검토 후 다시 고정하세요.",
                    "PHASE 2 STALE | XML or set content differs from the frozen record. Review and freeze again.");
                return;
            }

            IsPinArrayGapValidationIdentityFrozen = true;
            PinArrayGapValidationStatusText = BuildPinArrayGapFrozenStatus(record);
        }

        private bool TryReadSelectedPipelineXml(out string pipelineXmlText, out string error)
        {
            pipelineXmlText = string.Empty;
            string pipelineName = SelectedPipelineOption?.PipelineName ?? string.Empty;
            if (string.IsNullOrWhiteSpace(pipelineName))
            {
                error = "Select the imported PinArrayGap pipeline.";
                return false;
            }

            string path = RecipeWorkspaceService.GetVisionPipelinePath(
                NormalizeRecipeName(selectedRecipeName),
                pipelineName);
            if (!File.Exists(path))
            {
                error = "Selected pipeline XML was not found: " + path;
                return false;
            }

            try
            {
                pipelineXmlText = File.ReadAllText(path);
                error = string.Empty;
                return true;
            }
            catch (Exception ex)
            {
                error = ex.GetBaseException().Message;
                return false;
            }
        }

        private static string BuildPinArrayGapFrozenStatus(OpenVisionRecipePinArrayGapValidationRecord record)
        {
            if (record == null)
            {
                return string.Empty;
            }

            return "PHASE 2 FROZEN | "
                + record.PipelineName
                + " | Train "
                + record.Train.ImageCount.ToString(CultureInfo.InvariantCulture)
                + " / Validation "
                + record.Validation.ImageCount.ToString(CultureInfo.InvariantCulture)
                + " / Test "
                + record.Test.ImageCount.ToString(CultureInfo.InvariantCulture)
                + " | DistancePxRange <= "
                + record.DistancePxRangeMaximum.ToString("0.###", CultureInfo.InvariantCulture)
                + " px";
        }

        private void NotifyPinArrayGapIntentTextChanged()
        {
            OnPropertyChanged(nameof(PinArrayGapIntentContractText));
            NotifyGuidedSetupIntentInputChanged();
            RefreshCommandState();
        }

        private void NotifyHybridRelativeRoiIntentTextChanged()
        {
            OnPropertyChanged(nameof(HybridRelativeRoiBoundaryText));
            NotifyGuidedSetupIntentInputChanged();
            RefreshCommandState();
        }

        private void NotifyBlobCountIntentTextChanged()
        {
            OnPropertyChanged(nameof(BlobCountIntentWorkflowText));
            OnPropertyChanged(nameof(BlobCountIntentFeedbackText));
            OnPropertyChanged(nameof(BlobCountIntentLatestRunText));
            NotifyGuidedSetupIntentInputChanged();
            RefreshCommandState();
        }

        private void NotifyGuidedSetupIntentInputChanged()
        {
            if (!string.IsNullOrWhiteSpace(LlmXmlDraftText))
            {
                llmXmlDraftImportReady = false;
                SetGuidedSetupDraftStale(true);
            }

            OnPropertyChanged(nameof(IsGuidedSetupIntentInputReady));
            OnPropertyChanged(nameof(GuidedSetupIntentInputStatusText));
        }

        private void SetGuidedSetupDraftStale(bool value)
        {
            if (isGuidedSetupDraftStale == value)
            {
                return;
            }

            isGuidedSetupDraftStale = value;
            OnPropertyChanged(nameof(IsGuidedSetupDraftStale));
            OnPropertyChanged(nameof(GuidedSetupDraftLabelText));
        }

        private void NotifyContourCountIntentTextChanged()
        {
            OnPropertyChanged(nameof(ContourCountIntentWorkflowText));
            OnPropertyChanged(nameof(ContourCountIntentFeedbackText));
            OnPropertyChanged(nameof(ContourCountIntentLatestRunText));
            NotifyGuidedSetupIntentInputChanged();
            RefreshCommandState();
        }

        private void NotifyMatchingIntentTextChanged()
        {
            NotifyGuidedSetupIntentInputChanged();
            RefreshCommandState();
        }

        private void NotifyFeatureMatchingIntentTextChanged()
        {
            NotifyGuidedSetupIntentInputChanged();
            RefreshCommandState();
        }

        private void NotifyEdgeBasedIntentTextChanged()
        {
            NotifyGuidedSetupIntentInputChanged();
            RefreshCommandState();
        }

        private void NotifyReferenceDifferenceIntentTextChanged()
        {
            NotifyGuidedSetupIntentInputChanged();
            RefreshCommandState();
        }

        private void NotifyMeanIntentTextChanged()
        {
            NotifyGuidedSetupIntentInputChanged();
            RefreshCommandState();
        }

        private void CreateHybridRelativeRoiIntentXmlDraft()
        {
            if (!OpenVisionRecipeHybridRelativeRoiIntentSkill.TryValidateInputs(
                    LlmReferenceImagePath,
                    MatchingIntentSearchRoiText,
                    HybridRelativeRoiText,
                    HybridReferencePoseText,
                    MatchingIntentScoreMinText,
                    HybridScoreMarginText,
                    HybridAngleMinimumText,
                    HybridAngleMaximumText,
                    HybridScaleRatioMinimumText,
                    HybridScaleRatioMaximumText,
                    HybridMinimumValidPixelRatioText,
                    out OpenVisionRecipePinGapIntentSkill.RoiSample searchRoi,
                    out OpenVisionRecipePinGapIntentSkill.RoiSample measurementRoi,
                    out OpenVisionRecipeHybridRelativeRoiIntentSkill.ReferencePose referencePose,
                    out double scoreMinimum,
                    out double scoreMargin,
                    out double angleMinimum,
                    out double angleMaximum,
                    out double scaleRatioMinimum,
                    out double scaleRatioMaximum,
                    out double minimumValidPixelRatio,
                    out string message))
            {
                StatusText = LocalText("상대 ROI Gap 입력을 확인하세요: ", "Check locator-aligned Gap inputs: ") + message;
                return;
            }

            VisionPipeline pipeline = OpenVisionRecipeHybridRelativeRoiIntentSkill.CreateMeasurementPipeline(
                LlmReferenceImagePath,
                searchRoi,
                measurementRoi,
                referencePose,
                scoreMinimum,
                scoreMargin,
                angleMinimum,
                angleMaximum,
                scaleRatioMinimum,
                scaleRatioMaximum,
                minimumValidPixelRatio);
            SelectedLlmToolTemplate = OpenVisionGuidedSetupCatalog.HybridRelativeRoiGapTemplate;
            LlmPromptText = BuildLlmPromptText();
            LlmXmlDraftText = SerializePipelineToXmlText(pipeline);
            ValidateLlmXmlDraftText(false);
            StatusText = LocalText(
                "위치 보정 후 상대 ROI Gap 측정 XML 초안을 만들었습니다. 위치검출 gate만 포함하며 Gap 판정, Preview, Run은 실행하지 않았습니다.",
                "Created the locator-aligned relative-ROI Gap XML draft. It includes locator gates but no Gap judgement, Preview, or Run.");
        }

        private void CreatePinArrayGapIntentXmlDraft()
        {
            string sourceImagePath = ResolvePinGapRoiSuggestionImagePath();
            if (string.IsNullOrWhiteSpace(sourceImagePath))
            {
                StatusText = LocalText(
                    "Pin row edge-gap 스킬에 사용할 선택 샘플 또는 참조 이미지가 필요합니다.",
                    "Select a sample or reference image for the Pin row edge-gap skill.");
                return;
            }

            bool roiReady = OpenVisionRecipePinArrayGapIntentSkill.TryParseRowRois(
                PinArrayGapRoiText,
                out IReadOnlyList<OpenVisionRecipePinGapIntentSkill.RoiSample> rowRois,
                out string roiMessage);
            bool thresholdReady = int.TryParse(
                PinArrayGapDarkThresholdText,
                NumberStyles.Integer,
                CultureInfo.InvariantCulture,
                out int darkThreshold);
            bool coverageReady = double.TryParse(
                PinArrayGapMinDarkCoverageRatioText,
                NumberStyles.Float,
                CultureInfo.InvariantCulture,
                out double minimumDarkCoverageRatio);
            bool minimumPinWidthReady = int.TryParse(
                PinArrayGapMinPinWidthText,
                NumberStyles.Integer,
                CultureInfo.InvariantCulture,
                out int minimumPinWidth);
            bool maximumBreakWidthReady = int.TryParse(
                PinArrayGapMaxPinBreakWidthText,
                NumberStyles.Integer,
                CultureInfo.InvariantCulture,
                out int maximumPinBreakWidth);
            bool minimumGapWidthReady = int.TryParse(
                PinArrayGapMinGapWidthText,
                NumberStyles.Integer,
                CultureInfo.InvariantCulture,
                out int minimumGapWidth);
            string rangeText = (PinArrayGapRangeMaxText ?? string.Empty).Trim();
            bool measurementOnly = rangeText.Length == 0;
            bool rangeReady = measurementOnly
                || (double.TryParse(
                        rangeText,
                        NumberStyles.Float,
                        CultureInfo.InvariantCulture,
                        out double parsedRangeMaximum)
                    && !double.IsNaN(parsedRangeMaximum)
                    && !double.IsInfinity(parsedRangeMaximum)
                    && parsedRangeMaximum > 0D);

            if (!roiReady
                || !thresholdReady
                || !coverageReady
                || !minimumPinWidthReady
                || !maximumBreakWidthReady
                || !minimumGapWidthReady
                || !rangeReady)
            {
                StatusText = LocalText(
                    "Pin row edge-gap 입력을 확인하세요. 행 ROI는 x,y,w,h 형식이며 검출값은 유효한 수치, Range는 공란 또는 양수여야 합니다. ",
                    "Check Pin row edge-gap inputs. Row ROIs must be x,y,w,h, detection values must be valid numbers, and Range must be blank or positive. ")
                    + roiMessage;
                return;
            }

            try
            {
                BitmapFrame frame = BitmapFrame.Create(
                    new Uri(sourceImagePath, UriKind.Absolute),
                    BitmapCreateOptions.DelayCreation,
                    BitmapCacheOption.OnLoad);
                if (!OpenVisionRecipePinArrayGapIntentSkill.TryValidateV1Inputs(
                        PinArrayGapMeasurementText,
                        PinArrayGapPolarityText,
                        OpenVisionRecipePinArrayGapIntentSkill.SupportedUnitMode,
                        rowRois,
                        frame.PixelWidth,
                        frame.PixelHeight,
                        darkThreshold,
                        minimumDarkCoverageRatio,
                        minimumPinWidth,
                        maximumPinBreakWidth,
                        minimumGapWidth,
                        out string validationMessage))
                {
                    StatusText = validationMessage;
                    return;
                }

                double maximumDistancePxRange = measurementOnly
                    ? 0D
                    : double.Parse(rangeText, NumberStyles.Float, CultureInfo.InvariantCulture);
                VisionPipeline pipeline = measurementOnly
                    ? OpenVisionRecipePinArrayGapIntentSkill.CreateMeasurementPipeline(
                        rowRois,
                        darkThreshold,
                        minimumDarkCoverageRatio,
                        minimumPinWidth,
                        maximumPinBreakWidth,
                        minimumGapWidth)
                    : OpenVisionRecipePinArrayGapIntentSkill.CreateJudgedPipeline(
                        rowRois,
                        darkThreshold,
                        minimumDarkCoverageRatio,
                        minimumPinWidth,
                        maximumPinBreakWidth,
                        minimumGapWidth,
                        maximumDistancePxRange);

                SelectedLlmToolTemplate = OpenVisionGuidedSetupCatalog.PinArrayGapTemplate;
                LlmPromptText = BuildLlmPromptText();
                LlmXmlDraftText = SerializePipelineToXmlText(pipeline);
                ValidateLlmXmlDraftText(false);
                StatusText = measurementOnly
                    ? LocalText(
                        "Pin row edge-gap 측정 전용 XML 초안을 만들었습니다. 판정 기준은 없으며 Preview/Run은 실행하지 않았습니다.",
                        "Created a Pin row edge-gap measurement-only XML draft. It is not judged, and Preview/Run was not executed.")
                    : LocalText(
                        "모든 행에 DistancePxRange 최대 판정이 있는 Pin row edge-gap XML 초안을 만들었습니다. Validation Set과 Preview/Run은 실행하지 않았습니다.",
                        "Created a Pin row edge-gap XML draft with a DistancePxRange maximum gate on every row. Validation Set and Preview/Run were not executed.");
            }
            catch (Exception ex)
            {
                StatusText = LocalText(
                    "Pin row edge-gap XML 초안 생성 실패: ",
                    "Pin row edge-gap XML draft creation failed: ")
                    + ex.GetBaseException().Message;
            }
        }

        private void CreatePinGapIntentXmlDraft()
        {
            bool pixelOnly = IsPinGapPixelOnly;
            double mmPerPixel = 0;
            if (!OpenVisionRecipePinGapIntentSkill.TryParseRoiSamples(PinGapIntentRoiText, out IReadOnlyList<OpenVisionRecipePinGapIntentSkill.RoiSample> roiSamples, out string roiMessage)
                || !OpenVisionRecipePinGapIntentSkill.TryParsePositiveDouble(PinGapIntentDistanceMinText, out double minimumDistance)
                || !OpenVisionRecipePinGapIntentSkill.TryParsePositiveDouble(PinGapIntentDistanceMaxText, out double maximumDistance)
                || !OpenVisionRecipePinGapIntentSkill.TryParsePositiveDouble(PinGapIntentRangeMaxText, out double maximumRange)
                || (!pixelOnly && !OpenVisionRecipePinGapIntentSkill.TryParsePositiveDouble(PinGapIntentScaleText, out mmPerPixel)))
            {
                StatusText = LocalText(
                    "핀 간격 skill 입력을 확인하세요. ROI는 x,y,w,h이고 거리/Range는 양수여야 합니다. mm/px는 양수이거나 px-only 사용을 위해 비워 두세요. ",
                    "Check Pin gap skill inputs. ROI samples must be x,y,w,h groups separated by semicolons, distance/range must be positive, and mm/px must be positive or blank for px-only. ")
                    + roiMessage;
                return;
            }

            if (minimumDistance > maximumDistance)
            {
                StatusText = LocalText("핀 간격 Min은 Max보다 클 수 없습니다.", "Pin gap Min cannot be greater than Max.");
                return;
            }

            SelectedLlmToolTemplate = "Pin gap / edge distance (LineDistance)";
            VisionPipeline pipeline = pixelOnly
                ? OpenVisionRecipePinGapIntentSkill.CreatePixelPipeline(
                    roiSamples,
                    minimumDistance,
                    maximumDistance,
                    maximumRange)
                : OpenVisionRecipePinGapIntentSkill.CreatePipeline(
                    roiSamples,
                    minimumDistance,
                    maximumDistance,
                    maximumRange,
                    mmPerPixel);

            LlmPromptText = BuildLlmPromptText()
                + Environment.NewLine
                + Environment.NewLine
                + "[Pin gap skill inputs]"
                + Environment.NewLine
                + "ROI samples: " + OpenVisionRecipePinGapIntentSkill.FormatRoiSamples(roiSamples)
                + Environment.NewLine
                + "Default scope: whole visible pin array unless the user marked one specific pair or region."
                + Environment.NewLine
                + "Unit mode: " + (pixelOnly ? "PX-ONLY (no physical-unit claim)" : "MM-READY")
                + Environment.NewLine
                + "Nominal " + PinGapIntentAverageMetricName + ": " + minimumDistance.ToString("0.###", CultureInfo.InvariantCulture)
                + ".." + maximumDistance.ToString("0.###", CultureInfo.InvariantCulture) + " " + PinGapIntentUnitText
                + Environment.NewLine
                + "Consistency " + PinGapIntentRangeMetricName + " max: " + maximumRange.ToString("0.###", CultureInfo.InvariantCulture) + " " + PinGapIntentUnitText
                + Environment.NewLine
                + "Scale mm/px: " + (pixelOnly ? "not provided" : mmPerPixel.ToString("0.######", CultureInfo.InvariantCulture))
                + Environment.NewLine
                + "Generated contract: every ROI sample gets " + PinGapIntentAverageMetricName + " and " + PinGapIntentRangeMetricName + " gates, then a final OverlayMerge review. No Step runs until the user explicitly validates/imports/runs.";
            LlmXmlDraftText = SerializePipelineToXmlText(pipeline);
            ValidateLlmXmlDraftText(false);
            StatusText = LocalText(
                "핀 간격 skill XML 초안을 생성했습니다. Preview/Run은 실행하지 않았습니다.",
                "Created Pin gap skill XML draft. Preview/Run was not executed.");
        }

        private void CreateDarkBandGapIntentXmlDraft()
        {
            if (!OpenVisionRecipeDarkBandGapIntentSkill.TryParseCoarseRoi(
                    DarkBandGapIntentRoiText,
                    out OpenVisionRecipePinGapIntentSkill.RoiSample roi,
                    out string message))
            {
                StatusText = "Check dark-band Gap ROI: " + message;
                return;
            }

            SelectedLlmToolTemplate = OpenVisionGuidedSetupCatalog.DarkBandGapTemplate;
            VisionPipeline pipeline = OpenVisionRecipeDarkBandGapIntentSkill.CreateMeasurementPipeline(roi);
            LlmPromptText = BuildLlmPromptText();
            LlmXmlDraftText = SerializePipelineToXmlText(pipeline);
            ValidateLlmXmlDraftText(false);
            StatusText = LocalText(
                "검은 띠 Gap 측정 전용 XML 초안을 만들었습니다. 판정 기준은 없으며 Preview/Run은 실행하지 않았습니다.",
                "Created a dark-band Gap measurement-only XML draft. It is not judged, and Preview/Run was not executed.");
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

        private void CreateMatchingIntentXmlDraft()
        {
            string templatePath = (LlmReferenceImagePath ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(templatePath) || !File.Exists(templatePath))
            {
                StatusText = LocalText(
                    "초안 XML을 만들기 전에 사용할 템플릿 이미지를 선택하세요.",
                    "Select a template image before creating draft XML.");
                return;
            }

            if (!OpenVisionRecipeMatchingIntentSkill.TryParseRoi(MatchingIntentSearchRoiText, out int roiX, out int roiY, out int roiWidth, out int roiHeight, out string roiMessage)
                || !OpenVisionRecipeMatchingIntentSkill.TryParseScore(MatchingIntentScoreMinText, out double scoreMinimum)
                || !OpenVisionRecipeMatchingIntentSkill.TryParsePositiveInt(MatchingIntentExpectedCountText, out int expectedCount))
            {
                StatusText = LocalText(
                    "Check Matching inputs. Search ROI must be x,y,w,h, SCORE_MIN must be 0..1, and expected count must be positive. ",
                    "Check Matching inputs. Search ROI must be x,y,w,h, SCORE_MIN must be 0..1, and expected count must be positive. ")
                    + roiMessage;
                return;
            }

            SelectedLlmToolTemplate = "Template Matching";
            VisionPipeline pipeline = OpenVisionRecipeMatchingIntentSkill.CreatePipeline(
                templatePath,
                roiX,
                roiY,
                roiWidth,
                roiHeight,
                scoreMinimum,
                expectedCount);

            LlmPromptText = BuildLlmPromptText()
                + Environment.NewLine
                + Environment.NewLine
                + "[Matching target-presence skill inputs]"
                + Environment.NewLine
                + "Template path: " + templatePath
                + Environment.NewLine
                + "Search ROI: " + OpenVisionRecipeBlobCountIntentSkill.FormatRoi(roiX, roiY, roiWidth, roiHeight)
                + Environment.NewLine
                + "SCORE_MIN: " + scoreMinimum.ToString("0.###", CultureInfo.InvariantCulture)
                + Environment.NewLine
                + "Expected ResultCount: " + expectedCount.ToString(CultureInfo.InvariantCulture)
                + Environment.NewLine
                + "Generated contract: Matching filters candidates with SCORE_MIN and judges exact ResultCount. No Step runs until the user explicitly validates/imports/runs.";
            LlmXmlDraftText = SerializePipelineToXmlText(pipeline);
            ValidateLlmXmlDraftText(false);
            StatusText = LocalText(
                "Created Matching target-presence skill XML draft. Preview/Run was not executed.",
                "Created Matching target-presence skill XML draft. Preview/Run was not executed.");
        }

        private void CreateFeatureMatchingIntentXmlDraft()
        {
            string templatePath = (LlmReferenceImagePath ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(templatePath) || !File.Exists(templatePath))
            {
                StatusText = LocalText(
                    "초안 XML을 만들기 전에 사용할 Feature Matching 템플릿 이미지를 선택하세요.",
                    "Select a Feature Matching template image before creating draft XML.");
                return;
            }

            if (!OpenVisionRecipeFeatureMatchingIntentSkill.TryParseScore(FeatureMatchingIntentScoreMinText, out double scoreMinimum)
                || !OpenVisionRecipeFeatureMatchingIntentSkill.TryParsePositiveDouble(FeatureMatchingIntentRansacReprojThresholdText, out double ransacReprojectionThreshold)
                || !OpenVisionRecipeFeatureMatchingIntentSkill.TryParseAcceptanceScoreMinimum(FeatureMatchingIntentAcceptanceScoreMinText, out double acceptanceScoreMinimum))
            {
                StatusText = LocalText(
                    "Feature Matching 입력을 확인하세요. Ratio 기준은 0..1, RANSAC px는 양수, ScoreMax 최소는 0보다 크고 100 이하여야 합니다.",
                    "Check Feature Matching inputs. Ratio min must be 0..1, RANSAC px must be positive, and ScoreMax min must be greater than 0 and no more than 100.");
                return;
            }

            SelectedLlmToolTemplate = OpenVisionGuidedSetupCatalog.FeatureMatchingTemplate;
            VisionPipeline pipeline = OpenVisionRecipeFeatureMatchingIntentSkill.CreatePipeline(
                templatePath,
                scoreMinimum,
                ransacReprojectionThreshold,
                acceptanceScoreMinimum);

            LlmPromptText = BuildLlmPromptText()
                + Environment.NewLine
                + Environment.NewLine
                + "[Feature matching skill inputs]"
                + Environment.NewLine
                + "Feature template path: " + templatePath
                + Environment.NewLine
                + "Inspection scope: full image (USE_ROI=false)"
                + Environment.NewLine
                + "Ratio minimum: " + scoreMinimum.ToString("0.###", CultureInfo.InvariantCulture)
                + Environment.NewLine
                + "RANSAC reprojection threshold px: " + ransacReprojectionThreshold.ToString("0.###", CultureInfo.InvariantCulture)
                + Environment.NewLine
                + "Accepted ScoreMax: " + acceptanceScoreMinimum.ToString("0.###", CultureInfo.InvariantCulture) + "..100"
                + Environment.NewLine
                + "Generated contract: FeatureMatching uses ScoreMax as the acceptance gate. ResultCount is review evidence only. No Step runs until the user explicitly validates/imports/runs.";
            LlmXmlDraftText = SerializePipelineToXmlText(pipeline);
            ValidateLlmXmlDraftText(false);
            StatusText = LocalText(
                "Feature Matching skill XML 초안을 만들었습니다. Preview/Run은 실행하지 않았습니다.",
                "Created Feature Matching skill XML draft. Preview/Run was not executed.");
        }

        private void CreateEdgeBasedIntentXmlDraft()
        {
            string templatePath = (LlmReferenceImagePath ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(templatePath) || !File.Exists(templatePath))
            {
                StatusText = LocalText(
                    "초안 XML을 만들기 전에 사용할 Edge Based Matching 템플릿 이미지를 선택하세요.",
                    "Select an Edge Based Matching template image before creating draft XML.");
                return;
            }

            if (!OpenVisionRecipeEdgeBasedMatchingIntentSkill.TryParseScore(EdgeBasedIntentScoreMinText, out double scoreMinimum)
                || !OpenVisionRecipeEdgeBasedMatchingIntentSkill.TryParsePositiveInt(EdgeBasedIntentSearchCountText, out int searchCount)
                || !OpenVisionRecipeEdgeBasedMatchingIntentSkill.TryParseByte(EdgeBasedIntentCannyLowText, out int cannyLow)
                || !OpenVisionRecipeEdgeBasedMatchingIntentSkill.TryParseByte(EdgeBasedIntentCannyHighText, out int cannyHigh)
                || !OpenVisionRecipeEdgeBasedMatchingIntentSkill.TryParseAcceptanceScoreMinimum(EdgeBasedIntentAcceptanceScoreMinText, out double acceptanceScoreMinimum))
            {
                StatusText = LocalText(
                    "Edge Based Matching 입력을 확인하세요. 최소 점수는 0..1, 검색 개수는 양수, Canny 값은 0..255, ScoreMax 최소는 0보다 크고 100 이하여야 합니다.",
                    "Check Edge Based Matching inputs. Min score must be 0..1, search count must be positive, Canny values must be 0..255, and ScoreMax min must be greater than 0 and no more than 100.");
                return;
            }

            if (cannyLow > cannyHigh)
            {
                StatusText = LocalText(
                    "Canny low는 Canny high보다 클 수 없습니다.",
                    "Canny low cannot be greater than Canny high.");
                return;
            }

            SelectedLlmToolTemplate = OpenVisionGuidedSetupCatalog.EdgeBasedMatchingTemplate;
            VisionPipeline pipeline = OpenVisionRecipeEdgeBasedMatchingIntentSkill.CreatePipeline(
                templatePath,
                scoreMinimum,
                searchCount,
                cannyLow,
                cannyHigh,
                acceptanceScoreMinimum);

            LlmPromptText = BuildLlmPromptText()
                + Environment.NewLine
                + Environment.NewLine
                + "[Edge based matching skill inputs]"
                + Environment.NewLine
                + "Edge template path: " + templatePath
                + Environment.NewLine
                + "Inspection scope: full image (USE_ROI=false)"
                + Environment.NewLine
                + "Minimum score: " + scoreMinimum.ToString("0.###", CultureInfo.InvariantCulture)
                + Environment.NewLine
                + "Search count: " + searchCount.ToString(CultureInfo.InvariantCulture)
                + Environment.NewLine
                + "Canny low/high: " + cannyLow.ToString(CultureInfo.InvariantCulture) + "/" + cannyHigh.ToString(CultureInfo.InvariantCulture)
                + Environment.NewLine
                + "Accepted ScoreMax: " + acceptanceScoreMinimum.ToString("0.###", CultureInfo.InvariantCulture) + "..100"
                + Environment.NewLine
                + "Generated contract: EdgeBasedMatching uses ScoreMax as the acceptance gate. ResultCount is review evidence only. No Step runs until the user explicitly validates/imports/runs.";
            LlmXmlDraftText = SerializePipelineToXmlText(pipeline);
            ValidateLlmXmlDraftText(false);
            StatusText = LocalText(
                "Edge Based Matching skill XML 초안을 만들었습니다. Preview/Run은 실행하지 않았습니다.",
                "Created Edge Based Matching skill XML draft. Preview/Run was not executed.");
        }

        private void CreateReferenceDifferenceIntentXmlDraft()
        {
            if (!OpenVisionRecipeReferenceDifferenceIntentSkill.TryCollectReferencePaths(
                    LlmReferenceImagePath,
                    ReferenceDifferencePath2,
                    ReferenceDifferencePath3,
                    ReferenceDifferencePath4,
                    out IReadOnlyList<string> referencePaths)
                || !OpenVisionRecipeReferenceDifferenceIntentSkill.TryParseThreshold(
                    ReferenceDifferenceThresholdText,
                    out int differenceThreshold)
                || !OpenVisionRecipeReferenceDifferenceIntentSkill.TryParsePositiveArea(
                    ReferenceDifferenceMinimumAreaText,
                    out int minimumArea)
                || !OpenVisionRecipeReferenceDifferenceIntentSkill.TryParsePositiveArea(
                    ReferenceDifferenceMaximumAreaText,
                    out int maximumArea)
                || minimumArea > maximumArea)
            {
                StatusText = LocalText(
                    "Good 기준 이미지 1~4개와 차이 임계값 0..255, 양수인 최소/최대 결함 면적을 확인하세요.",
                    "Check 1-4 existing Good references, difference threshold 0..255, and positive min/max defect areas.");
                return;
            }

            SelectedLlmToolTemplate = OpenVisionGuidedSetupCatalog.ReferenceDifferenceTemplate;
            VisionPipeline pipeline = OpenVisionRecipeReferenceDifferenceIntentSkill.CreatePipeline(
                referencePaths,
                differenceThreshold,
                minimumArea,
                maximumArea);

            LlmPromptText = BuildLlmPromptText()
                + Environment.NewLine
                + Environment.NewLine
                + "[Golden-reference defect skill inputs]"
                + Environment.NewLine
                + "Approved Good references: " + string.Join(" | ", referencePaths)
                + Environment.NewLine
                + "Difference threshold: " + differenceThreshold.ToString(CultureInfo.InvariantCulture)
                + Environment.NewLine
                + "Defect area: " + minimumArea.ToString(CultureInfo.InvariantCulture)
                + ".." + maximumArea.ToString(CultureInfo.InvariantCulture)
                + Environment.NewLine
                + "Generated contract: ReferenceDifference registers against the approved references and accepts only ResultCount=0. References are never learned or replaced automatically. No Step runs until the user explicitly validates/imports/runs.";
            LlmXmlDraftText = SerializePipelineToXmlText(pipeline);
            ValidateLlmXmlDraftText(false);
            StatusText = LocalText(
                "Golden-reference 결함 검사 XML 초안을 만들었습니다. Preview/Run은 실행하지 않았습니다.",
                "Created Golden-reference defect XML draft. Preview/Run was not executed.");
        }

        private void CreateMeanIntentXmlDraft()
        {
            if (!OpenVisionRecipeMeanIntentSkill.TryParseOptionalRoi(MeanIntentRoiText, out bool useRoi, out int roiX, out int roiY, out int roiWidth, out int roiHeight, out string roiMessage)
                || !OpenVisionRecipeMeanIntentSkill.TryParseMeanType(MeanIntentTypeText, out MeanType meanType)
                || !OpenVisionRecipeMeanIntentSkill.TryParseByte(MeanIntentMinimumText, out int minimum)
                || !OpenVisionRecipeMeanIntentSkill.TryParseByte(MeanIntentMaximumText, out int maximum))
            {
                StatusText = LocalText(
                    "Check Mean inputs. ROI is optional but must be x,y,w,h when used, Mean type must be supported, and Min/Max GV must be 0..255. ",
                    "Check Mean inputs. ROI is optional but must be x,y,w,h when used, Mean type must be supported, and Min/Max GV must be 0..255. ")
                    + roiMessage;
                return;
            }

            if (minimum > maximum)
            {
                StatusText = LocalText("Mean Min GV cannot be greater than Max GV.", "Mean Min GV cannot be greater than Max GV.");
                return;
            }

            SelectedLlmToolTemplate = "Mean Intensity";
            VisionPipeline pipeline = OpenVisionRecipeMeanIntentSkill.CreatePipeline(
                useRoi,
                roiX,
                roiY,
                roiWidth,
                roiHeight,
                meanType,
                minimum,
                maximum);

            string scopeText = useRoi
                ? OpenVisionRecipeBlobCountIntentSkill.FormatRoi(roiX, roiY, roiWidth, roiHeight)
                : "full image";
            LlmPromptText = BuildLlmPromptText()
                + Environment.NewLine
                + Environment.NewLine
                + "[Mean brightness-drift skill inputs]"
                + Environment.NewLine
                + "Scope: " + scopeText
                + Environment.NewLine
                + "Mean type: " + meanType
                + Environment.NewLine
                + "Accepted MeanValueAvg GV: " + minimum.ToString(CultureInfo.InvariantCulture)
                + ".." + maximum.ToString(CultureInfo.InvariantCulture)
                + Environment.NewLine
                + "Generated contract: Mean judges MeanValueAvg inside the configured GV band. No Step runs until the user explicitly validates/imports/runs.";
            LlmXmlDraftText = SerializePipelineToXmlText(pipeline);
            ValidateLlmXmlDraftText(false);
            StatusText = LocalText(
                "Created Mean brightness-drift skill XML draft. Preview/Run was not executed.",
                "Created Mean brightness-drift skill XML draft. Preview/Run was not executed.");
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
            if (loadedReviewBundleInspection != null)
            {
                validationReport = loadedReviewBundleInspection.IntegrityReport
                    + Environment.NewLine
                    + Environment.NewLine
                    + validationReport;
                dependencyReport = loadedReviewBundleInspection.PathReport
                    + Environment.NewLine
                    + Environment.NewLine
                    + dependencyReport;
            }

            LlmXmlDraftValidationReport = validationReport;
            LlmXmlDraftDependencyReport = dependencyReport;
            LlmXmlDraftReviewReport = ok ? BuildLlmDraftReviewReport(pipeline) : LocalText("초안 검토 건너뜀: 검증 실패.", "Draft review skipped: validation failed.");
            LlmXmlDraftDiffReport = ok ? BuildLlmDraftDiffReport(pipeline) : LocalText("변경점 검토 건너뜀: 검증 실패.", "Diff review skipped: validation failed.");
            llmXmlDraftImportReady = ok;
            StatusText = ok ? LocalText("LLM XML 초안 검증 OK.", "LLM XML draft validation OK.") : LocalText("LLM XML 초안 검증 NG.", "LLM XML draft validation NG.");
            RefreshCommandState();
            return ok;
        }

        private string BuildLlmPromptText()
        {
            string recipeName = NormalizeRecipeName(selectedRecipeName);
            string activePipelineName = VisionPipelineStorage.LoadActivePipelineName(
                recipeName,
                VisionPipelineAppendService.DefaultPipelineName);
            return OpenVisionRecipeLlmPromptBuilder.Build(new OpenVisionRecipeLlmPromptRequest
            {
                RecipeName = recipeName,
                ActivePipelineName = activePipelineName,
                Template = SelectedLlmToolTemplate,
                InspectionGoal = LlmInspectionGoalText,
                DetectionPoints = LlmDetectionPointText,
                ReferenceImagePath = OpenVisionRecipeLlmIntent.IsPinArrayGapTemplate(SelectedLlmToolTemplate)
                    ? ResolvePinGapRoiSuggestionImagePath()
                    : LlmReferenceImagePath,
                PinGapAverageMetricName = PinGapIntentAverageMetricName,
                PinGapRangeMetricName = PinGapIntentRangeMetricName,
                PinGapRoiText = PinGapIntentRoiText,
                PinGapIsPixelOnly = IsPinGapPixelOnly,
                PinGapDistanceMinText = PinGapIntentDistanceMinText,
                PinGapDistanceMaxText = PinGapIntentDistanceMaxText,
                PinGapRangeMaxText = PinGapIntentRangeMaxText,
                PinGapUnitText = PinGapIntentUnitText,
                PinGapScaleText = PinGapIntentScaleText,
                PinArrayGapRoiText = PinArrayGapRoiText,
                PinArrayGapPolarityText = PinArrayGapPolarityText,
                PinArrayGapMeasurementText = PinArrayGapMeasurementText,
                PinArrayGapRangeMaxText = PinArrayGapRangeMaxText,
                PinArrayGapDarkThresholdText = PinArrayGapDarkThresholdText,
                PinArrayGapMinDarkCoverageRatioText = PinArrayGapMinDarkCoverageRatioText,
                PinArrayGapMinPinWidthText = PinArrayGapMinPinWidthText,
                PinArrayGapMaxPinBreakWidthText = PinArrayGapMaxPinBreakWidthText,
                PinArrayGapMinGapWidthText = PinArrayGapMinGapWidthText,
                DarkBandGapRoiText = DarkBandGapIntentRoiText,
                HybridReferencePoseText = HybridReferencePoseText,
                HybridRelativeRoiText = HybridRelativeRoiText,
                HybridSearchRoiText = MatchingIntentSearchRoiText,
                HybridScoreMinimumText = MatchingIntentScoreMinText,
                HybridScoreMarginText = HybridScoreMarginText,
                HybridAngleMinimumText = HybridAngleMinimumText,
                HybridAngleMaximumText = HybridAngleMaximumText,
                HybridScaleRatioMinimumText = HybridScaleRatioMinimumText,
                HybridScaleRatioMaximumText = HybridScaleRatioMaximumText,
                HybridMinimumValidPixelRatioText = HybridMinimumValidPixelRatioText
            });
        }

        private OpenVisionRecipeGuidedSetupReadinessInput CreateGuidedSetupReadinessInput()
        {
            return new OpenVisionRecipeGuidedSetupReadinessInput
            {
                Template = SelectedLlmToolTemplate,
                ReferenceImagePath = LlmReferenceImagePath,
                PinGapRoiText = PinGapIntentRoiText,
                DarkBandGapRoiText = DarkBandGapIntentRoiText,
                HybridReferencePoseText = HybridReferencePoseText,
                HybridRelativeRoiText = HybridRelativeRoiText,
                HybridSearchRoiText = MatchingIntentSearchRoiText,
                HybridScoreMinimumText = MatchingIntentScoreMinText,
                HybridScoreMarginText = HybridScoreMarginText,
                HybridAngleMinimumText = HybridAngleMinimumText,
                HybridAngleMaximumText = HybridAngleMaximumText,
                HybridScaleRatioMinimumText = HybridScaleRatioMinimumText,
                HybridScaleRatioMaximumText = HybridScaleRatioMaximumText,
                HybridMinimumValidPixelRatioText = HybridMinimumValidPixelRatioText,
                PinGapPixelOnly = IsPinGapPixelOnly,
                PinGapDistanceMinText = PinGapIntentDistanceMinText,
                PinGapDistanceMaxText = PinGapIntentDistanceMaxText,
                PinGapRangeMaxText = PinGapIntentRangeMaxText,
                PinGapScaleText = PinGapIntentScaleText,
                PinArrayGapRoiText = PinArrayGapRoiText,
                PinArrayGapSourceImagePath = ResolvePinGapRoiSuggestionImagePath(),
                PinArrayGapPolarityText = PinArrayGapPolarityText,
                PinArrayGapMeasurementText = PinArrayGapMeasurementText,
                PinArrayGapRangeMaxText = PinArrayGapRangeMaxText,
                PinArrayGapDarkThresholdText = PinArrayGapDarkThresholdText,
                PinArrayGapMinDarkCoverageRatioText = PinArrayGapMinDarkCoverageRatioText,
                PinArrayGapMinPinWidthText = PinArrayGapMinPinWidthText,
                PinArrayGapMaxPinBreakWidthText = PinArrayGapMaxPinBreakWidthText,
                PinArrayGapMinGapWidthText = PinArrayGapMinGapWidthText,
                BlobCountRoiText = BlobCountIntentRoiText,
                BlobCountThresholdText = BlobCountIntentThresholdText,
                BlobCountMinCountText = BlobCountIntentMinCountText,
                BlobCountMaxCountText = BlobCountIntentMaxCountText,
                BlobCountMinAreaText = BlobCountIntentMinAreaText,
                BlobCountMaxAreaText = BlobCountIntentMaxAreaText,
                ContourCountRoiText = ContourCountIntentRoiText,
                ContourCountThresholdText = ContourCountIntentThresholdText,
                ContourCountMinCountText = ContourCountIntentMinCountText,
                ContourCountMaxCountText = ContourCountIntentMaxCountText,
                ContourCountMinAreaText = ContourCountIntentMinAreaText,
                ContourCountMaxAreaText = ContourCountIntentMaxAreaText,
                MatchingSearchRoiText = MatchingIntentSearchRoiText,
                MatchingScoreMinText = MatchingIntentScoreMinText,
                MatchingExpectedCountText = MatchingIntentExpectedCountText,
                FeatureMatchingScoreMinText = FeatureMatchingIntentScoreMinText,
                FeatureMatchingRansacReprojThresholdText = FeatureMatchingIntentRansacReprojThresholdText,
                FeatureMatchingAcceptanceScoreMinText = FeatureMatchingIntentAcceptanceScoreMinText,
                EdgeBasedScoreMinText = EdgeBasedIntentScoreMinText,
                EdgeBasedSearchCountText = EdgeBasedIntentSearchCountText,
                EdgeBasedCannyLowText = EdgeBasedIntentCannyLowText,
                EdgeBasedCannyHighText = EdgeBasedIntentCannyHighText,
                EdgeBasedAcceptanceScoreMinText = EdgeBasedIntentAcceptanceScoreMinText,
                ReferenceDifferencePath2 = ReferenceDifferencePath2,
                ReferenceDifferencePath3 = ReferenceDifferencePath3,
                ReferenceDifferencePath4 = ReferenceDifferencePath4,
                ReferenceDifferenceThresholdText = ReferenceDifferenceThresholdText,
                ReferenceDifferenceMinimumAreaText = ReferenceDifferenceMinimumAreaText,
                ReferenceDifferenceMaximumAreaText = ReferenceDifferenceMaximumAreaText,
                MeanRoiText = MeanIntentRoiText,
                MeanTypeText = MeanIntentTypeText,
                MeanMinimumText = MeanIntentMinimumText,
                MeanMaximumText = MeanIntentMaximumText
            };
        }

        private VisionPipeline CreateLlmTemplatePipeline()
        {
            return OpenVisionRecipeLlmTemplateDraftBuilder.Create(
                SelectedLlmToolTemplate,
                LlmReferenceImagePath,
                OpenVisionRecipeLlmIntent.IsPinArrayGapTemplate(SelectedLlmToolTemplate)
                    ? PinArrayGapRoiText
                    : OpenVisionRecipeLlmIntent.IsDarkBandGapTemplate(SelectedLlmToolTemplate)
                        ? DarkBandGapIntentRoiText
                        : PinGapIntentRoiText);
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

        private string BuildLlmDraftReviewReport(VisionPipeline draftPipeline)
        {
            string recipeName = NormalizeRecipeName(selectedRecipeName);
            string activePipelineName = VisionPipelineStorage.LoadActivePipelineName(
                recipeName,
                VisionPipelineAppendService.DefaultPipelineName);
            VisionPipeline activePipeline = VisionPipelineStorage.Load(recipeName, activePipelineName);
            return OpenVisionRecipePipelineComparisonPresenter.BuildDraftImportReview(activePipeline, draftPipeline);
        }

        private string BuildLlmDraftDiffReport(VisionPipeline draftPipeline)
        {
            string recipeName = NormalizeRecipeName(selectedRecipeName);
            string activePipelineName = VisionPipelineStorage.LoadActivePipelineName(
                recipeName,
                VisionPipelineAppendService.DefaultPipelineName);
            VisionPipeline activePipeline = VisionPipelineStorage.Load(recipeName, activePipelineName);
            return OpenVisionRecipePipelineComparisonPresenter.BuildDraftDiffReview(activePipeline, draftPipeline);
        }

        private string BuildPipelineVariantComparisonReport()
        {
            if (!CanUseSelectedRecipe() || SelectedPipelineOption == null)
            {
                return OpenVisionRecipePipelineComparisonPresenter.BuildVariantComparison(
                    activePipeline: null,
                    selectedPipeline: null,
                    hasSelectedPipeline: false,
                    selectedIsActive: false);
            }

            string recipeName = NormalizeRecipeName(selectedRecipeName);
            string activePipelineName = VisionPipelineStorage.LoadActivePipelineName(
                recipeName,
                VisionPipelineAppendService.DefaultPipelineName);
            VisionPipeline activePipeline = VisionPipelineStorage.Load(recipeName, activePipelineName);
            VisionPipeline selectedPipeline = VisionPipelineStorage.Load(recipeName, SelectedPipelineOption.PipelineName);
            return OpenVisionRecipePipelineComparisonPresenter.BuildVariantComparison(
                activePipeline,
                selectedPipeline,
                hasSelectedPipeline: true,
                selectedIsActive: string.Equals(
                    activePipelineName,
                    SelectedPipelineOption.PipelineName,
                    StringComparison.OrdinalIgnoreCase));
        }


        private bool TryBuildLlmDraftPipeline(
            bool copyDependencies,
            out VisionPipeline pipeline,
            out string validationReport,
            out string dependencyReport)
        {
            OpenVisionRecipeLlmDraftValidationResult result =
                OpenVisionRecipeLlmDraftValidationService.Validate(
                    new OpenVisionRecipeLlmDraftValidationRequest(
                        LlmXmlDraftText,
                        NormalizeRecipeName(selectedRecipeName),
                        SelectedLlmToolTemplate,
                        LlmReferenceImagePath,
                        loadedReviewBundleInspection == null,
                        loadedReviewBundleInspection,
                        copyDependencies,
                        OpenVisionRecipeLlmIntent.IsPinArrayGapTemplate(SelectedLlmToolTemplate)
                            ? CreatePinArrayGapIntentValidationContext()
                            : null,
                        OpenVisionRecipeLlmIntent.IsDarkBandGapTemplate(SelectedLlmToolTemplate)
                            ? new OpenVisionRecipeDarkBandGapIntentValidationContext(DarkBandGapIntentRoiText)
                            : null,
                        OpenVisionRecipeLlmIntent.IsHybridRelativeRoiGapTemplate(SelectedLlmToolTemplate)
                            ? CreateHybridRelativeRoiIntentValidationContext()
                            : null));

            pipeline = result.Pipeline;
            validationReport = result.ValidationReport;
            dependencyReport = result.DependencyReport;
            LlmXmlDraftDependencyRows = result.DependencyRows;
            return result.Success;
        }

        private OpenVisionRecipePinArrayGapIntentValidationContext CreatePinArrayGapIntentValidationContext()
        {
            int sourceWidth = 0;
            int sourceHeight = 0;
            string sourceImagePath = ResolvePinGapRoiSuggestionImagePath();
            if (!string.IsNullOrWhiteSpace(sourceImagePath))
            {
                try
                {
                    BitmapFrame frame = BitmapFrame.Create(
                        new Uri(sourceImagePath, UriKind.Absolute),
                        BitmapCreateOptions.DelayCreation,
                        BitmapCacheOption.OnLoad);
                    sourceWidth = frame.PixelWidth;
                    sourceHeight = frame.PixelHeight;
                }
                catch
                {
                    sourceWidth = 0;
                    sourceHeight = 0;
                }
            }

            return new OpenVisionRecipePinArrayGapIntentValidationContext(
                PinArrayGapRoiText,
                PinArrayGapPolarityText,
                PinArrayGapMeasurementText,
                PinArrayGapRangeMaxText,
                PinArrayGapDarkThresholdText,
                PinArrayGapMinDarkCoverageRatioText,
                PinArrayGapMinPinWidthText,
                PinArrayGapMaxPinBreakWidthText,
                PinArrayGapMinGapWidthText,
                sourceWidth,
                sourceHeight);
        }

        private OpenVisionRecipeHybridRelativeRoiIntentValidationContext CreateHybridRelativeRoiIntentValidationContext()
        {
            return new OpenVisionRecipeHybridRelativeRoiIntentValidationContext(
                LlmReferenceImagePath,
                MatchingIntentSearchRoiText,
                HybridRelativeRoiText,
                HybridReferencePoseText,
                MatchingIntentScoreMinText,
                HybridScoreMarginText,
                HybridAngleMinimumText,
                HybridAngleMaximumText,
                HybridScaleRatioMinimumText,
                HybridScaleRatioMaximumText,
                HybridMinimumValidPixelRatioText);
        }

        private void SetLlmXmlDraftDependencyPlaceholder(string action)
        {
            LlmXmlDraftDependencyRows = new[]
            {
                new OpenVisionRecipeDependencyReviewRow(
                    LocalText("대기", "Waiting"),
                    "-",
                    "-",
                    "-",
                    action)
            };
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

        private void ExportActivePipelineReviewBundle()
        {
            string recipeName = NormalizeRecipeName(selectedRecipeName);
            string activePipelineName = VisionPipelineStorage.LoadActivePipelineName(
                recipeName,
                VisionPipelineAppendService.DefaultPipelineName);
            string suggestedFileName = SanitizePathSegment(activePipelineName) + ".review.zip";
            string path = selectExportReviewBundlePath(suggestedFileName);
            if (string.IsNullOrWhiteSpace(path))
            {
                StatusText = LocalText("검토 묶음 내보내기가 취소되었습니다.", "Review bundle export canceled.");
                return;
            }

            ExportActivePipelineReviewBundleToPath(path);
        }

        public bool ExportActivePipelineReviewBundleToPath(string path)
        {
            if (!CanUseSelectedRecipe())
            {
                StatusText = LocalText("선택된 레시피가 없습니다.", "No recipe selected.");
                return false;
            }

            string recipeName = NormalizeRecipeName(selectedRecipeName);
            string activePipelineName = VisionPipelineStorage.LoadActivePipelineName(
                recipeName,
                VisionPipelineAppendService.DefaultPipelineName);
            VisionPipeline pipeline = VisionPipelineStorage.Load(recipeName, activePipelineName);
            string pipelineXml = SerializePipelineToXmlText(pipeline);
            if (!OpenVisionRecipeReviewBundleExporter.TryExport(
                path,
                recipeName,
                activePipelineName,
                pipeline,
                pipelineXml,
                BuildRecipeReviewReferences(),
                out string message))
            {
                StatusText = LocalText("검토 묶음 내보내기 실패: ", "Review bundle export failed: ") + message;
                return false;
            }

            StatusText = string.Format(
                CultureInfo.CurrentCulture,
                LocalText("검토 묶음 내보내기 완료: {0}", "Exported review bundle: {0}"),
                Path.GetFileName(message));
            return true;
        }

        private IReadOnlyList<OpenVisionRecipeReviewReference> BuildRecipeReviewReferences()
        {
            List<OpenVisionRecipeReviewReference> references = new List<OpenVisionRecipeReviewReference>();
            VisionPipelineSampleCatalogItem sample = SelectedSampleOption?.Sample;
            if (sample != null)
            {
                references.Add(new OpenVisionRecipeReviewReference(
                    "SelectedSampleImage",
                    sample.SampleName,
                    sample.ImageFullPath,
                    sample.CatalogSourceId));
                references.Add(new OpenVisionRecipeReviewReference(
                    "SelectedSamplePipeline",
                    sample.SampleName,
                    sample.PipelineFullPath,
                    sample.CatalogSourceId));
                references.Add(new OpenVisionRecipeReviewReference(
                    "SelectedSampleReferenceImage",
                    sample.SampleName,
                    sample.ReferenceImageFullPath,
                    sample.CatalogSourceId));
            }

            if (!string.IsNullOrWhiteSpace(LlmReferenceImagePath))
            {
                references.Add(new OpenVisionRecipeReviewReference(
                    "LlmReferenceImage",
                    Path.GetFileName(LlmReferenceImagePath),
                    LlmReferenceImagePath,
                    "OperatorSelected"));
            }

            return references
                .Where(reference => !string.IsNullOrWhiteSpace(reference.Path))
                .ToList();
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

        private bool CanImportLlmXmlDraft()
        {
            return CanUseLlmXmlDraft() && llmXmlDraftImportReady;
        }

        private bool CanUseSelectedSampleReference()
        {
            return SelectedSampleOption?.Sample != null
                && !string.IsNullOrWhiteSpace(SelectedSampleOption.Sample.ImageFullPath)
                && File.Exists(SelectedSampleOption.Sample.ImageFullPath);
        }

        private bool CanSuggestPinGapIntentRoiSamples()
        {
            return !string.IsNullOrWhiteSpace(ResolvePinGapRoiSuggestionImagePath());
        }

        private string ResolvePinGapRoiSuggestionImagePath()
        {
            string selectedSamplePath = SelectedSampleOption?.Sample?.ImageFullPath ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(selectedSamplePath) && File.Exists(selectedSamplePath))
            {
                return selectedSamplePath;
            }

            string referenceImagePath = LlmReferenceImagePath;
            if (!string.IsNullOrWhiteSpace(referenceImagePath) && File.Exists(referenceImagePath))
            {
                return referenceImagePath;
            }

            return string.Empty;
        }

        private bool CanRunSelectedSampleCheck()
        {
            if (isValidationSuiteRunning || isCatalogBenchmarkRunning || isSampleCheckRunning || !CanUseSelectedPipeline() || SelectedSampleOption?.Sample == null)
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
            if (isValidationSuiteRunning || isCatalogBenchmarkRunning || isPairCheckRunning || isSampleCheckRunning || !CanUseSelectedPipeline() || SelectedSampleOption?.Sample == null)
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
            if (isValidationSuiteRunning || isCatalogBenchmarkRunning || isPairCheckRunning || isSampleCheckRunning || !CanUseSelectedPipeline())
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

        private bool CanRunValidationSuite()
        {
            if (isValidationSuiteRunning)
            {
                return false;
            }

            string scope = SelectedValidationSuiteScopeOption?.Key ?? OpenVisionRecipeValidationSuiteScopeOption.SelectedSampleKey;
            if (string.Equals(scope, OpenVisionRecipeValidationSuiteScopeOption.LocalValidationSetKey, StringComparison.OrdinalIgnoreCase))
            {
                return CanRunLocalValidationSet();
            }

            if (string.Equals(scope, OpenVisionRecipeValidationSuiteScopeOption.GoodBadPairKey, StringComparison.OrdinalIgnoreCase))
            {
                return CanRunSelectedSamplePairCheck();
            }

            if (string.Equals(scope, OpenVisionRecipeValidationSuiteScopeOption.CatalogKey, StringComparison.OrdinalIgnoreCase))
            {
                return CanRunCatalogBenchmark();
            }

            return CanRunSelectedSampleCheck();
        }

        private bool CanRunLocalValidationSet()
        {
            OpenVisionRecipeValidationSetOption option = SelectedValidationSetOption;
            if (!validationSetStorageReady
                || isValidationSuiteRunning
                || isCatalogBenchmarkRunning
                || isPairCheckRunning
                || isSampleCheckRunning
                || !CanUseSelectedPipeline()
                || option?.Set?.Images == null
                || option.Set.Images.Count == 0
                || option.Set.Images.Any(image => image == null || !image.Exists))
            {
                return false;
            }

            if (option.Set.IsIdentityLocked
                && !string.Equals(
                    option.Set.PipelineName,
                    SelectedPipelineOption.PipelineName,
                    StringComparison.Ordinal))
            {
                return false;
            }

            string pipelinePath = RecipeWorkspaceService.GetVisionPipelinePath(
                NormalizeRecipeName(selectedRecipeName),
                SelectedPipelineOption.PipelineName);
            return File.Exists(pipelinePath);
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

        private static VisionPipelineBatchSampleRunResult CreateBatchSampleRunResult(
            VisionPipelineSampleCatalogItem sample,
            VisionPipelineSampleCheckResult result,
            string messageOverride = null)
        {
            string sampleImagePath = sample?.ImageFullPath ?? string.Empty;
            return new VisionPipelineBatchSampleRunResult
            {
                SampleName = sample?.SampleName ?? string.Empty,
                Status = result?.Status ?? string.Empty,
                Success = result?.Success ?? false,
                TotalMilliseconds = result?.TotalMilliseconds ?? 0D,
                FailedStep = result?.FailedStepText ?? string.Empty,
                Message = messageOverride ?? result?.Message ?? string.Empty,
                ReportPath = sampleImagePath,
                SampleImagePath = sampleImagePath,
                PairGroup = sample?.PairGroup ?? string.Empty,
                PairRole = sample?.PairRole ?? string.Empty,
                ExpectedText = sample?.ExpectedText ?? string.Empty,
                MetricText = result?.MetricText ?? string.Empty,
                MetricReviewText = result?.MetricReviewText ?? string.Empty,
                FinalLayer = result?.FinalLayerText ?? string.Empty,
                OverlayCount = result?.OverlayCountText ?? string.Empty,
                ActionSummary = result?.ActionSummaryText ?? string.Empty,
                RunReportPath = result?.RunReportPath ?? string.Empty
            };
        }

        private void CreateValidationSet()
        {
            if (!CanCreateValidationSet())
            {
                return;
            }

            string name = NewValidationSetName.Trim();
            validationSetDocument.Sets.Add(new OpenVisionRecipeValidationSet { Name = name });
            if (!TrySaveValidationSetDocument(LocalText("검증 세트 만들기", "Create validation set")))
            {
                return;
            }

            RefreshValidationSetOptions(name);
            NewValidationSetName = CreateUniqueValidationSetName();
            ValidationSuiteStatusText = LocalText("로컬 검증 세트를 만들었습니다: ", "Created local validation set: ") + name;
        }

        private bool CanCreateValidationSet()
        {
            string name = NewValidationSetName?.Trim() ?? string.Empty;
            return validationSetStorageReady
                && !isValidationSuiteRunning
                && OpenVisionRecipeValidationSetStorage.IsValidSetName(name)
                && !validationSetDocument.Sets.Any(set =>
                    string.Equals(set?.Name, name, StringComparison.OrdinalIgnoreCase));
        }

        private void DeleteValidationSet()
        {
            OpenVisionRecipeValidationSetOption option = SelectedValidationSetOption;
            if (!CanDeleteValidationSet() || option == null || !confirmDeleteValidationSet(option.Name))
            {
                return;
            }

            validationSetDocument.Sets.RemoveAll(set =>
                string.Equals(set?.Name, option.Name, StringComparison.OrdinalIgnoreCase));
            if (!TrySaveValidationSetDocument(LocalText("검증 세트 삭제", "Delete validation set")))
            {
                return;
            }

            RefreshValidationSetOptions();
            ValidationSuiteStatusText = LocalText("로컬 검증 세트를 삭제했습니다: ", "Deleted local validation set: ") + option.Name;
        }

        private bool CanDeleteValidationSet()
        {
            return validationSetStorageReady
                && !isValidationSuiteRunning
                && SelectedValidationSetOption != null;
        }

        private void AddValidationSetImages(string expected)
        {
            if (!CanAddValidationSetImages())
            {
                return;
            }

            try
            {
                IReadOnlyList<string> paths = selectValidationSetImagePaths(expected) ?? Array.Empty<string>();
                AddValidationSetImages(expected, paths, ValidationSetPendingNotes);
            }
            catch (Exception ex)
            {
                ValidationSuiteStatusText = LocalText("이미지 선택 ERROR: ", "Image selection ERROR: ") + ex.GetBaseException().Message;
            }
        }

        private void AddValidationSetFolder(string expected)
        {
            if (!CanAddValidationSetImages())
            {
                return;
            }

            try
            {
                string folderPath = selectValidationSetFolderPath(expected) ?? string.Empty;
                if (!string.IsNullOrWhiteSpace(folderPath))
                {
                    AddValidationSetFolder(expected, folderPath, ValidationSetPendingNotes);
                }
            }
            catch (Exception ex)
            {
                ValidationSuiteStatusText = LocalText("폴더 선택 ERROR: ", "Folder selection ERROR: ")
                    + ex.GetBaseException().Message;
            }
        }

        internal bool AddValidationSetFolderForTest(
            string expected,
            string folderPath,
            string notes = "")
        {
            return AddValidationSetFolder(expected, folderPath, notes);
        }

        private bool AddValidationSetFolder(string expected, string folderPath, string notes)
        {
            if (!CanAddValidationSetImages())
            {
                return false;
            }

            if (!OpenVisionRecipeValidationSetStorage.TryGetTopLevelImagePaths(
                    folderPath,
                    out IReadOnlyList<string> paths,
                    out string error))
            {
                ValidationSuiteStatusText = LocalText("폴더 이미지 등록 ERROR: ", "Folder image registration ERROR: ")
                    + error;
                return false;
            }

            if (paths.Count == 0)
            {
                ValidationSuiteStatusText = LocalText(
                    "선택한 폴더의 바로 아래에서 지원 이미지 파일을 찾지 못했습니다.",
                    "No supported images were found directly in the selected folder.");
                return false;
            }

            return AddValidationSetImages(expected, paths, notes);
        }

        internal bool AddValidationSetImagesForTest(
            string expected,
            IEnumerable<string> paths,
            string notes = "")
        {
            return AddValidationSetImages(expected, paths, notes);
        }

        private bool AddValidationSetImages(string expected, IEnumerable<string> paths, string notes)
        {
            OpenVisionRecipeValidationSetOption option = SelectedValidationSetOption;
            if (!CanAddValidationSetImages() || option?.Set == null)
            {
                return false;
            }

            int added = OpenVisionRecipeValidationSetStorage.AddOrUpdateImages(
                option.Set,
                paths,
                expected,
                notes,
                out int updated,
                out int skipped);
            if (added == 0 && updated == 0)
            {
                if (skipped > 0)
                {
                    ValidationSuiteStatusText = LocalText("지원되는 기존 이미지가 선택되지 않았습니다.", "No supported existing images were selected.");
                }

                return false;
            }

            string setName = option.Name;
            if (!TrySaveValidationSetDocument(LocalText("검증 이미지 추가", "Add validation images")))
            {
                return false;
            }

            RefreshValidationSetOptions(setName);
            ValidationSuiteStatusText = string.Format(
                CultureInfo.CurrentCulture,
                LocalText("{0} 이미지: 추가 {1}, 갱신 {2}, 건너뜀 {3}", "{0} images: added {1}, updated {2}, skipped {3}"),
                expected,
                added,
                updated,
                skipped);
            return true;
        }

        private bool CanAddValidationSetImages()
        {
            return validationSetStorageReady
                && !isValidationSuiteRunning
                && SelectedValidationSetOption?.Set != null
                && !SelectedValidationSetOption.Set.IsIdentityLocked;
        }

        private void RepairValidationSetImagePath()
        {
            OpenVisionRecipeValidationSetImageRow row = SelectedValidationSetImageRow;
            if (!CanRepairValidationSetImagePath() || row == null)
            {
                return;
            }

            try
            {
                string replacementPath = selectValidationSetReplacementImagePath(row.Path) ?? string.Empty;
                if (!string.IsNullOrWhiteSpace(replacementPath))
                {
                    RepairValidationSetImagePath(replacementPath);
                }
            }
            catch (Exception ex)
            {
                ValidationSuiteStatusText = LocalText("경로 복구 ERROR: ", "Path repair ERROR: ")
                    + ex.GetBaseException().Message;
            }
        }

        internal bool RepairValidationSetImagePathForTest(string replacementPath)
        {
            return RepairValidationSetImagePath(replacementPath);
        }

        private bool RepairValidationSetImagePath(string replacementPath)
        {
            OpenVisionRecipeValidationSetOption option = SelectedValidationSetOption;
            OpenVisionRecipeValidationSetImageRow row = SelectedValidationSetImageRow;
            if (!CanRepairValidationSetImagePath() || option?.Set == null || row?.Image == null)
            {
                return false;
            }

            string missingFileName = row.FileName;
            if (!OpenVisionRecipeValidationSetStorage.TryRepairMissingImagePath(
                    option.Set,
                    row.Image,
                    replacementPath,
                    out string repairedPath,
                    out string error))
            {
                ValidationSuiteStatusText = LocalText("경로 복구 ERROR: ", "Path repair ERROR: ") + error;
                return false;
            }

            string setName = option.Name;
            if (!TrySaveValidationSetDocument(LocalText("검증 이미지 경로 복구", "Repair validation image path")))
            {
                return false;
            }

            RefreshValidationSetOptions(setName);
            ValidationSuiteStatusText = string.Format(
                CultureInfo.CurrentCulture,
                LocalText("누락 이미지 경로를 복구했습니다: {0} -> {1}", "Repaired missing image path: {0} -> {1}"),
                missingFileName,
                Path.GetFileName(repairedPath));
            return true;
        }

        private bool CanRepairValidationSetImagePath()
        {
            return validationSetStorageReady
                && !isValidationSuiteRunning
                && SelectedValidationSetOption?.Set != null
                && !SelectedValidationSetOption.Set.IsIdentityLocked
                && SelectedValidationSetImageRow?.Image != null
                && SelectedValidationSetImageRow.IsMissing;
        }

        private void RemoveValidationSetImage()
        {
            OpenVisionRecipeValidationSetOption option = SelectedValidationSetOption;
            OpenVisionRecipeValidationSetImageRow row = SelectedValidationSetImageRow;
            if (!CanRemoveValidationSetImage() || option?.Set == null || row?.Image == null)
            {
                return;
            }

            option.Set.Images.RemoveAll(image => ReferenceEquals(image, row.Image)
                || string.Equals(image?.Path, row.Path, StringComparison.OrdinalIgnoreCase));
            string setName = option.Name;
            if (!TrySaveValidationSetDocument(LocalText("검증 이미지 제거", "Remove validation image")))
            {
                return;
            }

            RefreshValidationSetOptions(setName);
            ValidationSuiteStatusText = LocalText("검증 세트에서 이미지를 제거했습니다: ", "Removed image from validation set: ") + row.FileName;
        }

        private bool CanRemoveValidationSetImage()
        {
            return validationSetStorageReady
                && !isValidationSuiteRunning
                && SelectedValidationSetOption?.Set != null
                && !SelectedValidationSetOption.Set.IsIdentityLocked
                && SelectedValidationSetImageRow?.Image != null;
        }

        private bool TrySaveValidationSetDocument(string operation)
        {
            if (OpenVisionRecipeValidationSetStorage.TrySave(
                NormalizeRecipeName(selectedRecipeName),
                validationSetDocument,
                out string error))
            {
                return true;
            }

            RefreshValidationSetOptions();
            ValidationSuiteStatusText = operation + " ERROR: " + error;
            return false;
        }

        private void RefreshValidationSetOptions(string preferredSetName = null)
        {
            string recipeName = NormalizeRecipeName(selectedRecipeName);
            string previousName = preferredSetName
                ?? SelectedValidationSetOption?.Name
                ?? string.Empty;
            string previousTrainName = PinArrayGapTrainValidationSetOption?.Name ?? string.Empty;
            string previousValidationName = PinArrayGapValidationValidationSetOption?.Name ?? string.Empty;
            string previousTestName = PinArrayGapTestValidationSetOption?.Name ?? string.Empty;
            validationSetStorageReady = OpenVisionRecipeValidationSetStorage.TryLoad(
                recipeName,
                out validationSetDocument,
                out string error);

            if (!validationSetStorageReady)
            {
                ValidationSetOptions = Array.Empty<OpenVisionRecipeValidationSetOption>();
                SelectedValidationSetOption = null;
                PinArrayGapTrainValidationSetOption = null;
                PinArrayGapValidationValidationSetOption = null;
                PinArrayGapTestValidationSetOption = null;
                RefreshValidationSetImageRows();
                ValidationSuiteStatusText = LocalText("로컬 검증 세트 로드 ERROR: ", "Local validation set load ERROR: ") + error;
                RefreshCommandState();
                return;
            }

            if (string.IsNullOrWhiteSpace(previousTrainName)
                && string.IsNullOrWhiteSpace(previousValidationName)
                && string.IsNullOrWhiteSpace(previousTestName)
                && OpenVisionRecipePinArrayGapValidationRecordStorage.TryLoad(
                    recipeName,
                    out OpenVisionRecipePinArrayGapValidationRecord frozenRecord,
                    out _))
            {
                previousTrainName = frozenRecord.Train?.SetName ?? string.Empty;
                previousValidationName = frozenRecord.Validation?.SetName ?? string.Empty;
                previousTestName = frozenRecord.Test?.SetName ?? string.Empty;
            }

            IReadOnlyList<OpenVisionRecipeValidationSetOption> options = validationSetDocument.Sets
                .Where(set => set != null)
                .OrderBy(set => set.Name, StringComparer.OrdinalIgnoreCase)
                .Select(set => new OpenVisionRecipeValidationSetOption(set))
                .ToList();
            ValidationSetOptions = options;
            OpenVisionRecipeValidationSetOption selected = options.FirstOrDefault(option =>
                    string.Equals(option.Name, previousName, StringComparison.OrdinalIgnoreCase))
                ?? options.FirstOrDefault();
            SelectedValidationSetOption = selected;
            PinArrayGapTrainValidationSetOption = FindValidationSetOption(options, previousTrainName);
            PinArrayGapValidationValidationSetOption = FindValidationSetOption(options, previousValidationName);
            PinArrayGapTestValidationSetOption = FindValidationSetOption(options, previousTestName);
            if (selected == null)
            {
                RefreshValidationSetImageRows();
            }

            OnPropertyChanged(nameof(ValidationSetSelectionSummaryText));
            OnPropertyChanged(nameof(ValidationSuiteSummaryText));
            RefreshCommandState();
        }

        private static OpenVisionRecipeValidationSetOption FindValidationSetOption(
            IEnumerable<OpenVisionRecipeValidationSetOption> options,
            string name)
        {
            return string.IsNullOrWhiteSpace(name)
                ? null
                : options?.FirstOrDefault(option =>
                    string.Equals(option?.Name, name, StringComparison.OrdinalIgnoreCase));
        }

        private void RefreshValidationSetImageRows()
        {
            string previousPath = SelectedValidationSetImageRow?.Path ?? string.Empty;
            List<OpenVisionRecipeValidationSetImageRow> rows = SelectedValidationSetOption?.Set?.Images?
                .Where(image => image != null)
                .Select(image => new OpenVisionRecipeValidationSetImageRow(image))
                .ToList()
                ?? new List<OpenVisionRecipeValidationSetImageRow>();
            ValidationSetImageRows = rows;
            SelectedValidationSetImageRow = rows.FirstOrDefault(row =>
                    string.Equals(row.Path, previousPath, StringComparison.OrdinalIgnoreCase))
                ?? rows.FirstOrDefault();
            NotifyValidationSetEvidenceChanged();
        }

        private string CreateUniqueValidationSetName()
        {
            const string baseName = "Local_Validation_Set";
            HashSet<string> names = validationSetDocument.Sets
                .Where(set => set != null && !string.IsNullOrWhiteSpace(set.Name))
                .Select(set => set.Name)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);
            if (!names.Contains(baseName))
            {
                return baseName;
            }

            int suffix = 2;
            while (names.Contains(baseName + "_" + suffix.ToString(CultureInfo.InvariantCulture)))
            {
                suffix++;
            }

            return baseName + "_" + suffix.ToString(CultureInfo.InvariantCulture);
        }

        private void RefreshSampleOptions()
        {
            string previousSampleName = SelectedSampleOption?.SampleName ?? string.Empty;
            IReadOnlyList<OpenVisionRecipeSampleOption> options = VisionPipelineSampleCatalogItem.LoadRunnable()
                .Where(sample => sample != null
                    && sample.CanOpen
                    && sample.CatalogSourceKind != VisionPipelineSampleCatalogSourceKind.LocalLegacy)
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
            RefreshPinArrayGapValidationIdentityState();
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
            OpenVisionRecipeBatchRunOption autoBaseline =
                OpenVisionRecipeRunHistoryPresenter.FindAutoBaselineRunOption(current, RecentBatchRunOptions);
            SelectedBenchmarkBaselineRunOption = options.FirstOrDefault(option =>
                    !string.IsNullOrWhiteSpace(option.SummaryPath)
                    && string.Equals(option.SummaryPath, previousBaselinePath, StringComparison.OrdinalIgnoreCase))
                ?? options.FirstOrDefault(option =>
                    !string.IsNullOrWhiteSpace(option.SummaryPath)
                    && string.Equals(option.SummaryPath, autoBaseline?.SummaryPath, StringComparison.OrdinalIgnoreCase))
                ?? options.FirstOrDefault();
        }

        private static OpenVisionRecipeBatchSampleResultOption SelectDefaultBatchSampleResult(
            OpenVisionRecipeBatchRunOption option,
            bool ngOnly = false,
            bool reviewQueueOnly = false)
        {
            IReadOnlyList<OpenVisionRecipeBatchSampleResultOption> results =
                option?.SampleResults ?? Array.Empty<OpenVisionRecipeBatchSampleResultOption>();
            if (reviewQueueOnly)
            {
                return results.FirstOrDefault(result => result?.IsInReviewQueue == true);
            }

            if (ngOnly)
            {
                return results.FirstOrDefault(result => result != null && !result.Success);
            }

            return results
                .FirstOrDefault(result => result != null && !result.Success && !string.IsNullOrWhiteSpace(result.FailedStep))
                ?? results.FirstOrDefault();
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
            string llmValidationReport = OpenVisionRecipeStoredPipelineValidationReportBuilder.Build(
                new OpenVisionRecipeStoredPipelineValidationReportRequest
                {
                    PipelinePath = pipelinePath,
                    XmlOk = xmlOk,
                    Pipeline = activePipeline,
                    XmlMessage = xmlMessage
                });
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
            OnPropertyChanged(nameof(PipelineVariantComparisonReport));
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

        private void OpenSelectedRecentBatchRunEvidence()
        {
            if (!OpenVisionRecipeRunEvidence.TryCreate(
                    SelectedRecentBatchSampleResultOption,
                    out OpenVisionRecipeRunEvidence evidence,
                    out string reason))
            {
                StatusText = reason;
                return;
            }

            if (openSelectedBatchRunEvidence(evidence))
            {
                StatusText = LocalText("저장된 원본/검출 도면을 열었습니다: ", "Opened the persisted source/detection drawing: ")
                    + evidence.SampleName;
            }
            else
            {
                StatusText = LocalText("저장된 검출 도면 창을 열지 못했습니다.", "Could not open the persisted detection drawing window.");
            }
        }

        private bool CanOpenSelectedRecentBatchRunEvidence()
        {
            return OpenVisionRecipeRunEvidence.TryCreate(
                SelectedRecentBatchSampleResultOption,
                out _,
                out _);
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
            SetCorrectedOutputReview(
                OpenVisionRecipePipelineStepReviewPresenter.BuildCorrectedOutputAppliedText(
                    SelectedPipelinePreviewStep,
                    pipelineName,
                    selectedIndex,
                    validationMessage));
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
            if (!TryLoadSelectedPipelineStep(out _, out string pipelineName, out VisionPipeline pipeline, out VisionPipelineStep step, out string message))
            {
                ClearSelectedStepEdit();
                if (updateStatus)
                {
                    SetSelectedStepEditStatus(message);
                }

                return false;
            }

            int selectedStepIndex = Math.Max(0, (pipeline?.Steps ?? new List<VisionPipelineStep>()).IndexOf(step));
            VisionPipelineStepPropertyMapper.SetGeometryFeatureContext((mode, sourceA) =>
                VisionPipelineStepPropertyMapper.GetCompatibleGeometryFeatureReferences(pipeline, selectedStepIndex, mode, sourceA));
            VisionPipelineStepPropertyMapper.SetPointFeatureContext(() =>
                VisionPipelineStepPropertyMapper.GetCompatiblePointFeatureReferences(pipeline, selectedStepIndex));
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
                llmXmlDraftImportReady = false;
                pinArrayGapTrainValidationSetOption = null;
                pinArrayGapValidationValidationSetOption = null;
                pinArrayGapTestValidationSetOption = null;
                PinArrayGapValidationStatusText = string.Empty;
                IsPinArrayGapValidationIdentityFrozen = false;
                OnPropertyChanged(nameof(SelectedRecipeName));
                OnPropertyChanged(nameof(PinArrayGapTrainValidationSetOption));
                OnPropertyChanged(nameof(PinArrayGapValidationValidationSetOption));
                OnPropertyChanged(nameof(PinArrayGapTestValidationSetOption));
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
            RefreshValidationSetOptions();
        }

        private void RefreshCommandState()
        {
            OnPropertyChanged(nameof(RecipeEditValidationText));
            OnPropertyChanged(nameof(PipelineEditValidationText));
            OnPropertyChanged(nameof(RecipeGuidedNextActionText));
            OnPropertyChanged(nameof(RunValidationSuiteText));
            OnPropertyChanged(nameof(StopValidationSuiteText));
            OnPropertyChanged(nameof(IsLocalValidationSetRunning));
            OnPropertyChanged(nameof(ValidationSuiteSummaryText));
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

}
