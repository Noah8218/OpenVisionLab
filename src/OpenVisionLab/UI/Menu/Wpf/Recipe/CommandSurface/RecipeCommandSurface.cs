using OpenVisionLab.Vision2D;
using OpenVisionLab.Vision2D.Pipeline;
using OpenVisionLab.Vision2D.Property;
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

using System.Text;
namespace OpenVisionLab
{
    public sealed class OpenVisionShellHostRecipeCommandSurface : ObservableObject
    {
        #region Fields

        private readonly Func<string> currentRecipeProvider;
        private readonly Action<string> switchRecipe;
        private readonly Action refreshAfterSwitch;
        private readonly Func<Task> waitForRecipeSwitchCompletion;
        private readonly Func<string, bool> confirmDeleteRecipe;
        private readonly Func<string, string, bool> confirmDeletePipeline;
        private readonly Func<string> selectImportPipelineXmlPath;
        private readonly Func<string> selectLocatorEvidencePacketPath;
        private readonly Func<string> selectLocatorEvidenceReviewDecisionPath;
        private readonly Action<string> copyTextToClipboard;
        private readonly Func<bool> clipboardContainsText;
        private readonly Func<string> readClipboardText;
        private readonly Func<Task> yieldToUi;
        private readonly Action flushUi;
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
        private readonly Action openPipelineXmlSteps;
        private readonly Action<VISION_MENU> selectStepTool;
        private readonly Func<bool> commitSelectedStepEdit;
        private readonly Func<bool> saveRecipe;
        private readonly OpenVisionRecipeStepEditApplyOwner stepEditApplyOwner;
        private readonly OpenVisionRecipeStepEditApplyProjectionOwner stepEditApplyProjectionOwner;
        private readonly OpenVisionRecipeWorkspaceLifecycleProjectionOwner workspaceLifecycleProjectionOwner;
        private readonly OpenVisionRecipeManagerSummaryProjectionOwner recipeManagerSummaryProjectionOwner;
        private readonly OpenVisionRecipePipelineOptionProjectionOwner recipePipelineOptionProjectionOwner;
        private readonly OpenVisionRecipePendingEditTransitionController pendingEditTransitionController;
        private readonly IReadOnlyList<string> llmToolTemplateOptions = new[]
        {
            OpenVisionGuidedSetupCatalog.PinGapTemplate,
            OpenVisionGuidedSetupCatalog.PinArrayGapTemplate,
            OpenVisionGuidedSetupCatalog.DarkBandGapTemplate,
            OpenVisionGuidedSetupCatalog.HybridRelativeRoiGapTemplate,
            OpenVisionGuidedSetupCatalog.LocatorRelativeBlobTemplate,
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
        private IReadOnlyList<OpenVisionRecipeDependencyReviewRow> llmXmlDraftDependencyRows = Array.Empty<OpenVisionRecipeDependencyReviewRow>();
        private readonly IReadOnlyList<OpenVisionRecipeValidationSuiteScopeOption> validationSuiteScopeOptions = OpenVisionRecipeValidationSuiteScopeOption.CreateDefaults();
        private OpenVisionRecipeValidationSuiteScopeOption selectedValidationSuiteScopeOption;
        private OpenVisionRecipeBatchRunOption selectedRecentBatchRunOption;
        private OpenVisionRecipeBatchRunOption selectedBenchmarkBaselineRunOption;
        private OpenVisionRecipeBatchSampleResultOption selectedRecentBatchSampleResultOption;
        private OpenVisionRecipeBatchRunComparisonRow selectedRecentBatchRunComparisonRow;
        private OpenVisionRecipeSampleMatrixRow selectedSampleMatrixRow;
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
        private string locatorEvidencePacketPath = string.Empty;
        private string locatorEvidencePacketStatusText = string.Empty;
        private string locatorEvidenceReviewText = string.Empty;
        private BitmapSource locatorEvidenceOverlayImage;
        private OpenVisionRecipeLocatorRelativeBlobEvidencePacket loadedLocatorEvidencePacket;
        private string loadedLocatorEvidencePacketPath = string.Empty;
        private bool locatorEvidenceCompilationReady;
        private OpenVisionRecipeLocatorRelativeBlobReviewDecision loadedLocatorEvidenceReviewDecision;
        private string loadedLocatorEvidenceReviewDecisionPath = string.Empty;
        private string locatorEvidenceReviewDecisionPath = string.Empty;
        private string locatorEvidenceReviewDecisionStatusText = string.Empty;
        private string locatorEvidenceVisualCorrespondence = OpenVisionRecipeLocatorRelativeBlobReviewDecision.NotReviewed;
        private string locatorEvidenceReviewer = string.Empty;
        private string locatorEvidenceReviewNotes = string.Empty;
        private string operatorHandoffReportStatusText = string.Empty;
        private string selectedRecentBatchRunReviewCopyStatusText = string.Empty;
        private string pinArrayGapValidationStatusText = string.Empty;
        private bool isPinArrayGapValidationIdentityFrozen;
        private string newValidationSetName = "Local_Validation_Set";
        private string validationSetPendingNotes = string.Empty;
        private string validationSetPendingVariantId = string.Empty;
        private string validationSetPendingMetricName = string.Empty;
        private string validationSetPendingMetricMinimum = string.Empty;
        private string validationSetPendingMetricMaximum = string.Empty;
        private string statusText = string.Empty;
        private readonly OpenVisionRecipeValidationSetDocumentOwner validationSetDocumentOwner =
            new OpenVisionRecipeValidationSetDocumentOwner();
        private readonly OpenVisionRecipeValidationEvidenceOwner validationEvidenceOwner =
            new OpenVisionRecipeValidationEvidenceOwner();
        private readonly OpenVisionRecipePinArrayGapValidationIdentityOwner pinArrayGapValidationIdentityOwner =
            new OpenVisionRecipePinArrayGapValidationIdentityOwner();
        private readonly OpenVisionRecipeValidationSetSelectionOwner validationSetSelectionOwner;
        private readonly OpenVisionRecipeStepEditLoader stepEditLoader = new OpenVisionRecipeStepEditLoader();
        private readonly OpenVisionRecipeStepPreviewNavigationOwner stepPreviewNavigationOwner =
            new OpenVisionRecipeStepPreviewNavigationOwner();
        private readonly OpenVisionRecipePipelineExchangeUseCase pipelineExchangeUseCase = new OpenVisionRecipePipelineExchangeUseCase();
        private readonly OpenVisionRecipePipelineExchangeProjectionOwner pipelineExchangeProjectionOwner = new OpenVisionRecipePipelineExchangeProjectionOwner();
        private readonly OpenVisionRecipeReviewBundleDryRunProjectionOwner reviewBundleDryRunProjectionOwner = new OpenVisionRecipeReviewBundleDryRunProjectionOwner();
        private readonly OpenVisionRecipeLlmDraftReviewOwner llmDraftReviewOwner = new OpenVisionRecipeLlmDraftReviewOwner();
        private readonly OpenVisionRecipeRunHistoryOrchestrationOwner runHistoryOrchestrationOwner = new OpenVisionRecipeRunHistoryOrchestrationOwner();
        private readonly OpenVisionRecipePipelineLifecycleUseCase pipelineLifecycleUseCase = new OpenVisionRecipePipelineLifecycleUseCase();
        private readonly OpenVisionRecipePipelineLifecycleProjectionOwner pipelineLifecycleProjectionOwner;
        private readonly OpenVisionRecipeWorkspaceUseCase recipeWorkspaceUseCase = new OpenVisionRecipeWorkspaceUseCase();
        private readonly OpenVisionRecipeQualifiedSnapshotController qualifiedSnapshotController;
        private readonly Func<string, string, string, bool> confirmQualifiedSnapshotLifecycle;
        private readonly Func<string, bool> openQualifiedSnapshotEvidence;
        private OpenVisionRecipeReviewBundleInspection loadedReviewBundleInspection;
        private bool llmXmlDraftImportReady;
        private bool isGuidedSetupDraftStale;
        private bool isRefreshingOptions;
        private bool isSelectingRecipe;
        private bool isSwitchingRecipe;
        private OpenVisionRecipePipelineOption selectedPipelineOption;
        private OpenVisionRecipeSampleOption selectedSampleOption;
        private OpenVisionRecipePairSampleRunSummary selectedPairSampleResult;
        private OpenVisionRecipeManagerSummary selectedRecipeSummary = OpenVisionRecipeManagerSummary.Empty;

        #endregion

        #region Constructors

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
            Action openPinArrayGapValidationRuns = null,
            Func<OpenVisionRecipePendingEditRequest, OpenVisionRecipePendingEditDecision> decidePendingEdit = null,
            Func<string, VisionPipeline, OpenVisionRecipeRoundTripValidationResult> validateStepEditRoundTrip = null,
            Action<string, VisionPipeline> saveStepEditPipeline = null,
            OpenVisionRecipeQualifiedSnapshotController qualifiedSnapshotController = null,
            Func<string, string, string, bool> confirmQualifiedSnapshotLifecycle = null,
            Func<string, bool> openQualifiedSnapshotEvidence = null,
            Action openPipelineXmlSteps = null,
            Func<bool> saveRecipe = null,
            Func<Task> waitForRecipeSwitchCompletion = null,
            Func<string> selectLocatorEvidencePacketPath = null,
            Func<string> selectLocatorEvidenceReviewDecisionPath = null,
            Action<string> copyTextToClipboard = null,
            Func<bool> clipboardContainsText = null,
            Func<string> readClipboardText = null,
            Func<Task> yieldToUi = null,
            Action flushUi = null)
        {
            this.currentRecipeProvider = currentRecipeProvider ?? throw new ArgumentNullException(nameof(currentRecipeProvider));
            this.switchRecipe = switchRecipe ?? throw new ArgumentNullException(nameof(switchRecipe));
            this.refreshAfterSwitch = refreshAfterSwitch ?? throw new ArgumentNullException(nameof(refreshAfterSwitch));
            validationSetSelectionOwner = new OpenVisionRecipeValidationSetSelectionOwner(validationSetDocumentOwner);
            this.waitForRecipeSwitchCompletion = waitForRecipeSwitchCompletion ?? (() => Task.CompletedTask);
            this.confirmDeleteRecipe = confirmDeleteRecipe ?? (_ => true);
            this.confirmDeletePipeline = confirmDeletePipeline ?? ((_, _) => true);
            this.selectImportPipelineXmlPath = selectImportPipelineXmlPath ?? (() => string.Empty);
            this.selectLocatorEvidencePacketPath = selectLocatorEvidencePacketPath ?? (() => string.Empty);
            this.selectLocatorEvidenceReviewDecisionPath = selectLocatorEvidenceReviewDecisionPath ?? (() => string.Empty);
            this.copyTextToClipboard = copyTextToClipboard ?? (_ => throw new InvalidOperationException("Clipboard callback is not configured."));
            this.clipboardContainsText = clipboardContainsText ?? (() => false);
            this.readClipboardText = readClipboardText ?? (() => string.Empty);
            this.yieldToUi = yieldToUi ?? (() => Task.CompletedTask);
            this.flushUi = flushUi ?? (() => { });
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
            this.openPipelineXmlSteps = openPipelineXmlSteps ?? (() => { });
            this.qualifiedSnapshotController =
                qualifiedSnapshotController
                ?? new OpenVisionRecipeQualifiedSnapshotController();
            this.confirmQualifiedSnapshotLifecycle =
                confirmQualifiedSnapshotLifecycle ?? ((_, _, _) => false);
            this.openQualifiedSnapshotEvidence =
                openQualifiedSnapshotEvidence ?? (_ => false);
            this.selectStepTool = selectStepTool;
            this.commitSelectedStepEdit = commitSelectedStepEdit ?? (() => true);
            this.saveRecipe = saveRecipe ?? (() => false);
            stepEditApplyOwner = new OpenVisionRecipeStepEditApplyOwner(
                validateStepEditRoundTrip,
                saveStepEditPipeline);
            stepEditApplyProjectionOwner = new OpenVisionRecipeStepEditApplyProjectionOwner();
            pipelineLifecycleProjectionOwner = new OpenVisionRecipePipelineLifecycleProjectionOwner();
            workspaceLifecycleProjectionOwner = new OpenVisionRecipeWorkspaceLifecycleProjectionOwner();
            recipeManagerSummaryProjectionOwner = new OpenVisionRecipeManagerSummaryProjectionOwner();
            recipePipelineOptionProjectionOwner = new OpenVisionRecipePipelineOptionProjectionOwner();
            pendingEditTransitionController = new OpenVisionRecipePendingEditTransitionController(
                decidePendingEdit ?? (_ => OpenVisionRecipePendingEditDecision.Cancel),
                TryApplySelectedStepParameters,
                ClearSelectedStepEdit);
            selectedStepEditSession.PropertyChanged += OnSelectedStepEditSessionPropertyChanged;
            executionSession.PropertyChanging += OnExecutionSessionPropertyChanging;
            executionSession.PropertyChanged += OnExecutionSessionPropertyChanged;
            executionSession.BatchRunSaved += OnExecutionBatchRunSaved;
            executionSession.CommandStateChanged += OnExecutionCommandStateChanged;
            selectedValidationSuiteScopeOption = validationSuiteScopeOptions.FirstOrDefault();
            executionSession.SetStatus(OpenVisionRecipeText.Local(
                "Suite 범위를 선택한 뒤 명시적으로 Run suite를 실행하세요.",
                "Select a suite scope, then run the explicit suite."));
            SetLlmXmlDraftDependencyPlaceholder(LocalText(
                "XML 초안을 붙여넣거나 로드한 뒤 검증을 실행하세요.",
                "Paste or load an XML draft, then run validation."));
            LocatorEvidencePacketStatusText = LocalText(
                "Evidence Packet을 선택하면 해시·Candidate·overlay를 검토합니다. 실행은 별도 명령입니다.",
                "Select an Evidence Packet to review its hashes, candidate, and overlay. Execution is a separate command.");
            LocatorEvidenceReviewText = LocalText(
                "대기 중: 해시 검증된 locator-relative-blob-v1 Evidence Packet을 로드하세요.",
                "Waiting: load a hash-verified locator-relative-blob-v1 Evidence Packet.");
            LocatorEvidenceReviewDecisionStatusText = LocalText(
                "대기 중: Evidence Packet을 로드한 뒤 review decision 파일을 선택하세요.",
                "Waiting: load an Evidence Packet, then select its review decision file.");
            LlmBrowserAssistStatusText = CreateLlmBrowserAssistReadyText();

            CreateRecipeCommand = new RelayCommand(CreateRecipe);
            CreateNamedRecipeCommand = new RelayCommand(CreateNamedRecipe, CanCreateNamedRecipe);
            DuplicateRecipeCommand = new RelayCommand(DuplicateSelectedRecipe, CanDuplicateSelectedRecipe);
            RenameRecipeCommand = new RelayCommand(RenameSelectedRecipe, CanRenameSelectedRecipe);
            DeleteRecipeCommand = new RelayCommand(DeleteSelectedRecipe, CanDeleteSelectedRecipe);
            SaveRecipeCommand = new RelayCommand(SaveSelectedRecipe, CanUseSelectedRecipe);
            ImportPipelineXmlCommand = new RelayCommand(ImportPipelineXml, CanUseSelectedRecipe);
            ExportPipelineXmlCommand = new RelayCommand(ExportActivePipelineXml, CanUseSelectedRecipe);
            ExportRecipeReviewBundleCommand = new RelayCommand(ExportActivePipelineReviewBundle, CanUseSelectedRecipe);
            DuplicateFromSampleCommand = new RelayCommand(DuplicatePipelineFromSample, CanDuplicatePipelineFromSample);
            ActivatePipelineCommand = new RelayCommand(ActivateSelectedPipeline, CanUseSelectedPipeline);
            DuplicatePipelineCommand = new RelayCommand(DuplicateSelectedPipeline, CanUseSelectedPipeline);
            RenamePipelineCommand = new RelayCommand(RenameSelectedPipeline, CanRenameSelectedPipeline);
            DeletePipelineCommand = new RelayCommand(DeleteSelectedPipeline, CanDeleteSelectedPipeline);
            LoadLlmXmlDraftCommand = new RelayCommand(LoadLlmXmlDraft, CanUseSelectedRecipe);
            LoadLocatorEvidencePacketCommand = new RelayCommand(LoadLocatorEvidencePacket, CanUseSelectedRecipe);
            LoadLocatorEvidenceReviewDecisionCommand = new RelayCommand(LoadLocatorEvidenceReviewDecision, CanUseSelectedRecipe);
            ApproveLocatorEvidenceReviewDecisionCommand = new RelayCommand(ApproveLocatorEvidenceReviewDecision, CanRecordLocatorEvidenceReviewDecision);
            RejectLocatorEvidenceReviewDecisionCommand = new RelayCommand(RejectLocatorEvidenceReviewDecision, CanRecordLocatorEvidenceReviewDecision);
            RequestLocatorEvidenceReplacementCommand = new RelayCommand(RequestLocatorEvidenceReplacement, CanRecordLocatorEvidenceReviewDecision);
            CompileLocatorEvidencePacketCommand = new RelayCommand(CompileLocatorEvidencePacket, CanCompileLocatorEvidencePacket);
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
            RerunCorrectedOutputCommand = new RelayCommand(
                RerunCorrectedOutput,
                CanRerunCorrectedOutput);
            CreateValidationSetFromSelectedPairCommand = new RelayCommand(
                CreateValidationSetFromSelectedPair,
                CanCreateValidationSetFromSelectedPair);
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
            ApplyValidationSetVariantContractCommand = new RelayCommand(
                ApplyValidationSetVariantContract,
                CanApplyValidationSetVariantContract);
            ResetValidationSetVariantContractCommand = new RelayCommand(
                ResetValidationSetVariantContract,
                CanApplyValidationSetVariantContract);
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
            PrepareSelectedRunFailureCorrectionCommand = new RelayCommand(
                PrepareSelectedRunFailureCorrection,
                CanPrepareSelectedRunFailureCorrection);
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
            ResetSelectedStepDisplayDefaultsCommand = new RelayCommand(
                ResetSelectedStepDisplayDefaults,
                CanResetSelectedStepDisplayDefaults);
            ApplySelectedStepParametersCommand = new RelayCommand(ApplySelectedStepParameters, CanApplySelectedStepParameters);
            CopyOperatorHandoffReportCommand = new RelayCommand(CopyOperatorHandoffReport, CanCopyOperatorHandoffReport);
            CopySelectedRecentBatchRunReviewCommand = new RelayCommand(CopySelectedRecentBatchRunReview, CanCopySelectedRecentBatchRunReview);
            RunRecipeGuidedNextActionCommand = new RelayCommand(RunRecipeGuidedNextAction, CanRunRecipeGuidedNextAction);
            OpenPipelineReviewCommand = new RelayCommand(this.openPipelineReview, CanUseSelectedRecipe);
            InitializeQualifiedSnapshotCommands();
            RefreshSampleOptions();
            RefreshOptions();
            RefreshQualifiedSnapshotOptions();
        }

        #endregion

        #region Binding Properties

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
                    NotifyQualifiedSnapshotContextChanged();
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
            get => validationSetSelectionOwner.Options;
        }

        public OpenVisionRecipeValidationSetOption SelectedValidationSetOption
        {
            get => validationSetSelectionOwner.Selected;
            set
            {
                if (validationSetSelectionOwner.SelectSet(value))
                {
                    RefreshValidationSetImageRows();
                    OnPropertyChanged(nameof(SelectedValidationSetOption));
                    OnPropertyChanged(nameof(ValidationSetSelectionSummaryText));
                    OnPropertyChanged(nameof(ValidationSuiteSummaryText));
                    NotifyValidationSetEvidenceChanged();
                    RefreshCommandState();
                }
            }
        }

        public OpenVisionRecipeValidationSetOption PinArrayGapTrainValidationSetOption
        {
            get => validationSetSelectionOwner.Train;
            set
            {
                if (validationSetSelectionOwner.SelectTrain(value))
                {
                    OnPropertyChanged(nameof(PinArrayGapTrainValidationSetOption));
                    RefreshPinArrayGapValidationIdentityState();
                    RefreshCommandState();
                }
            }
        }

        public OpenVisionRecipeValidationSetOption PinArrayGapValidationValidationSetOption
        {
            get => validationSetSelectionOwner.Validation;
            set
            {
                if (validationSetSelectionOwner.SelectValidation(value))
                {
                    OnPropertyChanged(nameof(PinArrayGapValidationValidationSetOption));
                    RefreshPinArrayGapValidationIdentityState();
                    RefreshCommandState();
                }
            }
        }

        public OpenVisionRecipeValidationSetOption PinArrayGapTestValidationSetOption
        {
            get => validationSetSelectionOwner.Test;
            set
            {
                if (validationSetSelectionOwner.SelectTest(value))
                {
                    OnPropertyChanged(nameof(PinArrayGapTestValidationSetOption));
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
            get => validationSetSelectionOwner.ImageRows;
        }

        public OpenVisionRecipeValidationSetImageRow SelectedValidationSetImageRow
        {
            get => validationSetSelectionOwner.SelectedImage;
            set
            {
                if (validationSetSelectionOwner.SelectImage(value))
                {
                    OnPropertyChanged(nameof(SelectedValidationSetImageRow));
                    LoadSelectedValidationVariantContract();
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
                    if (!isRefreshingOptions)
                    {
                        OnPropertyChanged(nameof(CorrectedOutputRerunText));
                        OnPropertyChanged(nameof(CorrectedOutputRerunToolTipText));
                    }
                    OnPropertyChanged(nameof(CorrectedOutputReviewText));
                    NotifyQualifiedSnapshotContextChanged(includePreflight: !isRefreshingOptions);
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
                if (IsSamePipelinePreviewStep(selectedPipelinePreviewStep, value))
                {
                    if (SetProperty(ref selectedPipelinePreviewStep, value))
                    {
                        RefreshSelectedPipelineStepFlow();
                        OnPropertyChanged(nameof(OpenSelectedStepToolText));
                        OnPropertyChanged(nameof(FailureReviewText));
                        OnPropertyChanged(nameof(PipelineSelectedStepOperatorContextText));
                        OnPropertyChanged(nameof(CorrectedOutputReviewText));
                        CommandManager.InvalidateRequerySuggested();
                    }

                    return;
                }

                if (!TryLeaveSelectedStepEdit(
                    OpenVisionRecipePendingEditTransitionKind.Step,
                    value?.DisplayText))
                {
                    OnPropertyChanged(nameof(SelectedPipelinePreviewStep));
                    return;
                }

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

        public string ValidationSetPendingVariantId
        {
            get => validationSetPendingVariantId;
            set
            {
                if (SetProperty(ref validationSetPendingVariantId, value ?? string.Empty))
                {
                    RefreshCommandState();
                }
            }
        }

        public string ValidationSetPendingMetricName
        {
            get => validationSetPendingMetricName;
            set
            {
                if (SetProperty(ref validationSetPendingMetricName, value ?? string.Empty))
                {
                    RefreshCommandState();
                }
            }
        }

        public string ValidationSetPendingMetricMinimum
        {
            get => validationSetPendingMetricMinimum;
            set
            {
                if (SetProperty(ref validationSetPendingMetricMinimum, value ?? string.Empty))
                {
                    RefreshCommandState();
                }
            }
        }

        public string ValidationSetPendingMetricMaximum
        {
            get => validationSetPendingMetricMaximum;
            set
            {
                if (SetProperty(ref validationSetPendingMetricMaximum, value ?? string.Empty))
                {
                    RefreshCommandState();
                }
            }
        }

        public bool HasSelectedStepEditObject => SelectedStepEditObject != null;

        public bool HasSelectedOverlayMergeEditObject =>
            VisionPipelineOverlayMergePropertyAdapter.IsProperty(SelectedStepEditObject);

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
                    InvalidateLocatorEvidenceCompilation();
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

        public bool HasSelectedPipelinePersistenceStatus =>
            selectedPipelineOption?.HasPersistenceStatus == true;

        public bool HasSelectedPipelinePersistenceFailure =>
            selectedPipelineOption?.HasPersistenceFailure == true;

        public string SelectedPipelinePersistenceStatusText =>
            selectedPipelineOption?.PersistenceStatusText
            ?? string.Empty;

        public string SelectedPipelinePersistenceHelpText =>
            selectedPipelineOption?.PersistenceHelpText
            ?? string.Empty;

        private RecipeDataPersistenceState SelectedRecipeDataPersistenceState
        {
            get
            {
                if (string.IsNullOrWhiteSpace(
                    selectedRecipeName))
                {
                    return null;
                }

                RecipeDataStorage.TryGetPersistenceState(
                    selectedRecipeName,
                    out RecipeDataPersistenceState state);
                return state;
            }
        }

        public bool HasSelectedRecipePersistenceStatus =>
            HasSelectedPipelinePersistenceStatus
            || SelectedRecipeDataPersistenceState != null;

        public bool HasSelectedRecipePersistenceFailure =>
            HasSelectedPipelinePersistenceFailure
            || SelectedRecipeDataPersistenceState?.IsFailure == true;

        public string SelectedRecipePersistenceStatusText =>
            string.Join(
                Environment.NewLine,
                new[]
                {
                    SelectedPipelinePersistenceStatusText,
                    OpenVisionRecipePersistenceStatusPresenter
                        .CreateCompactText(
                            SelectedRecipeDataPersistenceState)
                }.Where(text =>
                    !string.IsNullOrWhiteSpace(text)));

        public string SelectedRecipePersistenceHelpText =>
            string.Join(
                Environment.NewLine
                + Environment.NewLine,
                new[]
                {
                    SelectedPipelinePersistenceHelpText,
                    OpenVisionRecipePersistenceStatusPresenter
                        .CreateHelpText(
                            SelectedRecipeDataPersistenceState)
                }.Where(text =>
                    !string.IsNullOrWhiteSpace(text)));

        public OpenVisionRecipeManagerSummary SelectedRecipeSummary
        {
            get => selectedRecipeSummary;
            private set
            {
                OpenVisionRecipePipelineStepPreview previousStep = selectedPipelinePreviewStep;
                if (SetProperty(ref selectedRecipeSummary, value ?? OpenVisionRecipeManagerSummary.Empty))
                {
                    OnPropertyChanged(nameof(HasCurrentRecipeSampleExecution));
                    OnPropertyChanged(nameof(RecipeOverviewLastResultValueText));
                    OnPropertyChanged(nameof(RecipeOverviewLastResultToolTipText));
                    NotifyOperatorReviewChanged(includeGuidedNextAction: !isRefreshingOptions);
                    OnPropertyChanged(nameof(RecipeGuidedSetupText));
                    RefreshSelectedPipelineStepFlow();
                    OpenVisionRecipePipelineStepPreview targetStep =
                        FindPipelinePreviewStep(SelectedRecentBatchSampleResultOption?.FailedStep)
                        ?? selectedRecipeSummary.PipelinePreviewSteps?.FirstOrDefault(step =>
                            previousStep != null
                            && step.Index == previousStep.Index
                            && string.Equals(step.Name, previousStep.Name, StringComparison.Ordinal)
                            && string.Equals(step.ToolType, previousStep.ToolType, StringComparison.OrdinalIgnoreCase));
                    SelectedPipelinePreviewStep = targetStep;
                }
            }
        }

        public OpenVisionRecipeSampleRunSummary LatestSampleRunSummary
        {
            get => executionSession.LatestSampleRunSummary;
            private set => executionSession.LatestSampleRunSummary = value;
        }

        public OpenVisionRecipePairRunSummary LatestPairRunSummary
        {
            get => executionSession.LatestPairRunSummary;
            private set => executionSession.LatestPairRunSummary = value;
        }

        public OpenVisionRecipeCatalogBenchmarkSummary LatestCatalogBenchmarkSummary
        {
            get => executionSession.LatestCatalogBenchmarkSummary;
            private set => executionSession.LatestCatalogBenchmarkSummary = value;
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

        #region Commands

        public ICommand CreateRecipeCommand { get; private set; }

        public ICommand CreateNamedRecipeCommand { get; private set; }

        public ICommand DuplicateRecipeCommand { get; private set; }

        public ICommand RenameRecipeCommand { get; private set; }

        public ICommand DeleteRecipeCommand { get; private set; }

        public ICommand SaveRecipeCommand { get; private set; }

        public ICommand ImportPipelineXmlCommand { get; private set; }

        public ICommand ExportPipelineXmlCommand { get; private set; }

        public ICommand ExportRecipeReviewBundleCommand { get; private set; }

        public ICommand DuplicateFromSampleCommand { get; private set; }

        public ICommand ActivatePipelineCommand { get; private set; }

        public ICommand DuplicatePipelineCommand { get; private set; }

        public ICommand RenamePipelineCommand { get; private set; }

        public ICommand DeletePipelineCommand { get; private set; }

        public ICommand LoadLlmXmlDraftCommand { get; private set; }

        public ICommand LoadLocatorEvidencePacketCommand { get; private set; }

        public ICommand LoadLocatorEvidenceReviewDecisionCommand { get; private set; }

        public ICommand ApproveLocatorEvidenceReviewDecisionCommand { get; private set; }

        public ICommand RejectLocatorEvidenceReviewDecisionCommand { get; private set; }

        public ICommand RequestLocatorEvidenceReplacementCommand { get; private set; }

        public ICommand CompileLocatorEvidencePacketCommand { get; private set; }

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

        public ICommand RerunCorrectedOutputCommand { get; private set; }

        public ICommand CreateValidationSetFromSelectedPairCommand { get; private set; }

        public ICommand CreateValidationSetCommand { get; private set; }

        public ICommand DeleteValidationSetCommand { get; private set; }

        public ICommand AddValidationSetOkImagesCommand { get; private set; }

        public ICommand AddValidationSetNgImagesCommand { get; private set; }

        public ICommand AddValidationSetOkFolderCommand { get; private set; }

        public ICommand AddValidationSetNgFolderCommand { get; private set; }

        public ICommand RepairValidationSetImagePathCommand { get; private set; }

        public ICommand RemoveValidationSetImageCommand { get; private set; }

        public ICommand ApplyValidationSetVariantContractCommand { get; private set; }

        public ICommand ResetValidationSetVariantContractCommand { get; private set; }

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

        public ICommand PrepareSelectedRunFailureCorrectionCommand { get; private set; }

        public ICommand OpenSelectedRecentBatchRunEvidenceCommand { get; private set; }

        public ICommand FreezePinArrayGapValidationIdentityCommand { get; private set; }

        public ICommand OpenPinArrayGapValidationRunsCommand { get; private set; }

        public ICommand SelectPreviousPipelinePreviewStepCommand { get; private set; }

        public ICommand SelectNextPipelinePreviewStepCommand { get; private set; }

        public ICommand OpenSelectedStepToolCommand { get; private set; }

        public ICommand LoadSelectedStepParametersCommand { get; private set; }

        public ICommand ResetSelectedStepDisplayDefaultsCommand { get; private set; }

        public ICommand ApplySelectedStepParametersCommand { get; private set; }

        public ICommand CopyOperatorHandoffReportCommand { get; private set; }

        public ICommand CopySelectedRecentBatchRunReviewCommand { get; private set; }

        public ICommand RunRecipeGuidedNextActionCommand { get; private set; }

        public ICommand OpenPipelineReviewCommand { get; private set; }

        #endregion

        public string NewRecipeButtonText => LocalText("새 레시피", "New recipe");

        public string RecipeSelectorToolTipText => LocalText("레시피 선택 / 전환", "Select or switch recipe");

        public string ManagerButtonText => LocalText("레시피 관리", "Manage recipes");

        public string SaveRecipeText => LocalText("레시피 저장", "Save recipe");

        public string SaveRecipeToolTipText => LocalText(
            "현재 레시피 설정을 저장합니다. 단축키: Ctrl+S",
            "Save the current recipe settings. Shortcut: Ctrl+S");

        public string RecipeSwitchingTitleText => LocalText(
            "레시피 불러오는 중",
            "Loading recipe");

        public bool IsSwitchingRecipe
        {
            get => isSwitchingRecipe;
            private set => SetProperty(ref isSwitchingRecipe, value);
        }

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

        public string RecipeLibrarySummaryText => recipeManagerSummaryProjectionOwner.ProjectLibrarySummary(
            RecipeLibraryText,
            RecipeOptions?.Count ?? 0,
            FilteredRecipeOptions?.Count ?? 0);

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

        public string PipelineListSummaryText => recipePipelineOptionProjectionOwner.ProjectListSummary(
            PipelineListText,
            PipelineOptions?.Count ?? 0,
            FilteredPipelineOptions?.Count ?? 0);

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

        public string ValidationSetVariantIdLabelText => LocalText("Variant", "Variant");

        public string ValidationSetMetricNameLabelText => LocalText("판정 Metric", "Expected metric");

        public string ValidationSetMetricMinimumLabelText => LocalText("최소", "Min");

        public string ValidationSetMetricMaximumLabelText => LocalText("최대", "Max");

        public string ApplyValidationSetVariantContractText => LocalText("선택 항목 적용", "Apply selected");

        public string ResetValidationSetVariantContractText => LocalText("기본값", "Reset");

        public string ValidationSetVariantContractToolTipText => LocalText(
            "선택한 이미지의 Variant와 기대 Metric 범위를 표시·편집합니다. 적용 또는 기본값 버튼은 Preview/Run을 실행하지 않습니다.",
            "Shows and edits the selected image Variant and expected metric range. Apply and Reset never Preview or Run.");

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
                validationSetDocumentOwner.StorageReady,
                SelectedValidationSetOption,
                ValidationSetImageRows);

        public string ValidationSetEvidenceText => LocalText("검증 근거", "Validation evidence");

        public string ValidationSetExpectedLabelText => LocalText("기대 OK/NG", "Expected OK/NG");

        public string ValidationSetAcceptanceLabelText => LocalText("판정 기준", "Acceptance gate");

        public string ValidationSetCalibrationLabelText => LocalText("보정 적용", "Calibration");

        public string ValidationSetNextActionLabelText => LocalText("다음 작업", "Next action");

        public string ValidationSetExpectedText =>
            OpenVisionRecipeValidationSetPresenter.BuildExpectedText(
                validationSetDocumentOwner.StorageReady,
                SelectedValidationSetOption);

        public string ValidationSetAcceptanceText => BuildValidationSetAcceptanceText();

        public string ValidationSetCalibrationText => BuildValidationSetCalibrationText();

        public string ValidationSetNextActionText =>
            OpenVisionRecipeValidationSetPresenter.BuildNextActionText(
                executionSession.IsValidationSuiteRunning,
                validationSetDocumentOwner.StorageReady,
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

        public string CreateValidationSetFromSelectedPairText =>
            LocalText("쌍을 검증 세트로", "Save pair as set");

        public string CreateValidationSetFromSelectedPairToolTipText =>
            LocalText(
                "선택한 카탈로그 OK/NG 쌍의 역할, 기대 지표, 이미지 SHA-256을 현재 레시피의 로컬 검증 세트로 저장합니다. Preview/Run은 실행하지 않습니다.",
                "Save the selected catalog OK/NG pair, expected metrics, and image SHA-256 values as a recipe-local validation set. Preview/Run is not executed.");

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

        public string PrepareSelectedRunFailureCorrectionText => LocalText("실패 수정 준비", "Prepare correction");

        public string PrepareSelectedRunFailureCorrectionToolTipText => LocalText(
            "실패 Step과 보존된 샘플을 불러와 PropertyGrid 수정을 준비합니다. Preview/Run은 실행하지 않습니다.",
            "Load the failed Step and retained sample for PropertyGrid correction. This does not run Preview/Run.");

        public string RerunFailurePairCheckText => LocalText("Good/Bad 재검사", "Rerun Good/Bad");

        public string CorrectedOutputRerunText => IsSelectedStepEditDirty
            ? LocalText("XML 반영 후 재검사", "Apply before rerun")
            : IsSelectedRunLocalValidationSet()
                ? LocalText("동일 세트 재검사", "Rerun same set")
                : RerunFailurePairCheckText;

        public string CorrectedOutputRerunToolTipText
        {
            get
            {
                if (IsSelectedStepEditDirty)
                {
                    return LocalText(
                        "저장하지 않은 Step 편집이 있습니다. XML 반영 또는 취소 후 재검사하세요.",
                        "A pending Step edit must be applied or discarded before rerunning.");
                }

                if (!IsSelectedRunLocalValidationSet())
                {
                    return LocalText(
                        "현재 카탈로그 Good/Bad 쌍을 명시적으로 재검사합니다.",
                        "Explicitly rerun the current catalog Good/Bad pair.");
                }

                return TryResolveSelectedRunValidationSet(
                        out OpenVisionRecipeValidationSetOption option,
                        out string reason)
                    ? LocalText(
                        "실패가 기록된 동일 검증 세트를 현재 수정된 파이프라인으로 명시적으로 재실행합니다: ",
                        "Explicitly rerun the same validation set with the corrected current pipeline: ")
                        + option.Name
                    : reason;
            }
        }

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

        public string ResetSelectedStepDisplayDefaultsText => LocalText("표시 기본값", "Display defaults");

        public string ApplySelectedStepParametersText => LocalText("XML 반영", "Apply to XML");

        public string CorrectedOutputReviewLabelText => LocalText("수정 출력 확인", "Corrected output review");

        public string CorrectedOutputReviewText =>
            string.IsNullOrWhiteSpace(selectedStepEditSession.CorrectedOutputReviewText)
                ? OpenVisionRecipePipelineStepReviewPresenter.BuildCorrectedOutputReviewText(
                    SelectedPipelinePreviewStep,
                    selectedStepEditSession.IsDirty,
                    SelectedStepEditObject,
                    IsSelectedRunLocalValidationSet())
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
            + LocalText(" / PIXELPERMM = mm/px", " / PIXELPERMM = mm/px")
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



        #endregion

        #region Run History Selection

        private void RefreshRecentBatchRunOptions()
        {
            string recipeName = NormalizeRecipeName(selectedRecipeName);
            string pipelineName = selectedPipelineOption?.PipelineName ?? string.Empty;
            string previousSummaryPath = SelectedRecentBatchRunOption?.SummaryPath ?? string.Empty;
            OpenVisionRecipeRunHistorySelection selection = runHistoryOrchestrationOwner.BuildRecentRunSelection(
                recipeName,
                pipelineName,
                previousSummaryPath);
            RecentBatchRunOptions = selection.Options;
            SelectedRecentBatchRunOption = selection.SelectedOption;
        }

        private void RefreshBenchmarkBaselineRunOptions()
        {
            string previousBaselinePath = selectedBenchmarkBaselineRunOption?.SummaryPath ?? string.Empty;
            OpenVisionRecipeRunHistorySelection selection = runHistoryOrchestrationOwner.BuildBaselineRunSelection(
                SelectedRecentBatchRunOption,
                RecentBatchRunOptions,
                previousBaselinePath);
            BenchmarkBaselineRunOptions = selection.Options;
            SelectedBenchmarkBaselineRunOption = selection.SelectedOption;
        }

        #endregion


        #region Selection, execution, guided setup, step edit and notifications

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

                SetSelectedRecipeName(current, refreshCommandState: false);
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

        public bool PrepareWorkspaceSampleContext(
            string sampleName,
            string pipelineName)
        {
            string requestedPipeline = NormalizePipelineName(pipelineName);
            if (string.IsNullOrWhiteSpace(sampleName)
                || string.IsNullOrWhiteSpace(requestedPipeline))
            {
                StatusText = OpenVisionRecipeText.Local(
                    "작업공간 샘플 문맥을 준비할 수 없습니다.",
                    "Cannot prepare the workspace sample context.");
                return false;
            }

            if (string.Equals(
                    selectedPipelineOption?.PipelineName,
                    requestedPipeline,
                    StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            if (TryLeaveSelectedStepEdit(
                    OpenVisionRecipePendingEditTransitionKind.Pipeline,
                    requestedPipeline))
            {
                return true;
            }

            StatusText = OpenVisionRecipeText.Local(
                    "보류 중인 Step 편집 전환이 취소되어 샘플 열기를 중단했습니다: ",
                    "Opening the sample was cancelled because the pending Step edit transition was cancelled: ")
                + sampleName;
            return false;
        }

        public bool SynchronizeWorkspaceSampleContext(
            string sampleName,
            string pipelineName)
        {
            string requestedPipeline = NormalizePipelineName(pipelineName);
            RefreshPipelineOptions(requestedPipeline);
            RefreshSampleOptions();

            OpenVisionRecipePipelineOption pipelineOption =
                PipelineOptions.FirstOrDefault(option => string.Equals(
                    option?.PipelineName,
                    requestedPipeline,
                    StringComparison.OrdinalIgnoreCase));
            OpenVisionRecipeSampleOption sampleOption =
                SampleOptions.FirstOrDefault(option => string.Equals(
                    option?.SampleName,
                    sampleName,
                    StringComparison.OrdinalIgnoreCase));
            if (pipelineOption == null || sampleOption == null)
            {
                StatusText = OpenVisionRecipeText.Local(
                        "작업공간 샘플과 Recipe Manager 문맥을 맞추지 못했습니다: ",
                        "Could not synchronize the workspace sample and Recipe Manager context: ")
                    + sampleName;
                RefreshCommandState();
                return false;
            }

            SelectedPipelineOption = pipelineOption;
            SelectedSampleOption = sampleOption;
            StatusText = OpenVisionRecipeText.Local(
                    "작업공간 샘플 문맥 동기화: ",
                    "Workspace sample context synchronized: ")
                + sampleName
                + " / "
                + requestedPipeline
                + OpenVisionRecipeText.Local(
                    ". Preview/Run은 실행되지 않았습니다.",
                    ". Preview/Run was not executed.");
            return true;
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
                if (!string.Equals(selectedRecipeName, requestedRecipe, StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
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
            if (!string.Equals(
                selectedPipelineOption?.PipelineName,
                requestedPipeline,
                StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

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
            OnPropertyChanged(nameof(SaveRecipeText));
            OnPropertyChanged(nameof(SaveRecipeToolTipText));
            OnPropertyChanged(nameof(RecipeSwitchingTitleText));
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
            OnPropertyChanged(nameof(PrepareSelectedRunFailureCorrectionText));
            OnPropertyChanged(nameof(PrepareSelectedRunFailureCorrectionToolTipText));
            OnPropertyChanged(nameof(RerunFailurePairCheckText));
            OnPropertyChanged(nameof(CorrectedOutputRerunText));
            OnPropertyChanged(nameof(CorrectedOutputRerunToolTipText));
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
            OnPropertyChanged(nameof(LocatorEvidencePacketLabelText));
            OnPropertyChanged(nameof(LocatorEvidenceLoadText));
            OnPropertyChanged(nameof(LocatorEvidenceCompileText));
            OnPropertyChanged(nameof(LocatorEvidenceReviewLabelText));
            OnPropertyChanged(nameof(LocatorEvidenceOverlayLabelText));
            OnPropertyChanged(nameof(LocatorEvidenceBoundaryText));
            OnPropertyChanged(nameof(LocatorEvidenceReviewDecisionLabelText));
            OnPropertyChanged(nameof(LocatorEvidenceReviewDecisionLoadText));
            OnPropertyChanged(nameof(LocatorEvidenceVisualCorrespondenceLabelText));
            OnPropertyChanged(nameof(LocatorEvidenceReviewerLabelText));
            OnPropertyChanged(nameof(LocatorEvidenceReviewNotesLabelText));
            OnPropertyChanged(nameof(LocatorEvidenceApproveText));
            OnPropertyChanged(nameof(LocatorEvidenceRejectText));
            OnPropertyChanged(nameof(LocatorEvidenceReplacementText));
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

        private async void SelectRecipe(string recipeName)
        {
            await SelectRecipeAsync(recipeName);
        }

        private async Task SelectRecipeAsync(string recipeName)
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

            if (!TryLeaveSelectedStepEdit(
                OpenVisionRecipePendingEditTransitionKind.Recipe,
                normalized))
            {
                OnPropertyChanged(nameof(SelectedRecipeName));
                return;
            }

            if (isSelectingRecipe)
            {
                OnPropertyChanged(nameof(SelectedRecipeName));
                return;
            }

            try
            {
                isSelectingRecipe = true;
                BeginRecipeSwitchingState(normalized);
                await yieldToUi();
                RecipeWorkspaceService.EnsureVisionWorkspace(normalized);
                switchRecipe(normalized);
                if (!string.Equals(
                    NormalizeRecipeName(currentRecipeProvider()),
                    normalized,
                    StringComparison.OrdinalIgnoreCase))
                {
                    StatusText = string.Format(
                        CultureInfo.CurrentCulture,
                        LocalText("레시피 로드 실패: {0}", "Recipe load failed: {0}"),
                        normalized);
                    OnPropertyChanged(nameof(SelectedRecipeName));
                    return;
                }

                RefreshAfterRecipeSwitchIfNeeded(normalized);
                await waitForRecipeSwitchCompletion();
                StatusText = string.Format(
                    CultureInfo.CurrentCulture,
                    LocalText("선택됨: {0}", "Selected: {0}"),
                    normalized);
            }
            finally
            {
                IsSwitchingRecipe = false;
                isSelectingRecipe = false;
            }
        }

        private void BeginRecipeSwitchingState(string recipeName)
        {
            IsSwitchingRecipe = true;
            StatusText = string.Format(
                CultureInfo.CurrentCulture,
                LocalText("레시피 변경 중: {0}", "Switching recipe: {0}"),
                recipeName);

            flushUi();
        }

        private void RefreshAfterRecipeSwitchIfNeeded(string recipeName)
        {
            if (string.Equals(
                    selectedRecipeName,
                    NormalizeRecipeName(recipeName),
                    StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            // The normal shell path refreshes from RecipeState.EventChangedRecipe.
            // Keep this fallback for isolated command-surface hosts without that event.
            RefreshOptions();
            refreshAfterSwitch();
        }

        private void SelectPipelineOption(OpenVisionRecipePipelineOption option)
        {
            if (option == null)
            {
                return;
            }

            if (!string.Equals(
                    selectedPipelineOption?.PipelineName,
                    option.PipelineName,
                    StringComparison.OrdinalIgnoreCase)
                && !TryLeaveSelectedStepEdit(
                    OpenVisionRecipePendingEditTransitionKind.Pipeline,
                    option.PipelineName))
            {
                OnPropertyChanged(nameof(SelectedPipelineOption));
                return;
            }

            if (!SetProperty(ref selectedPipelineOption, option, nameof(SelectedPipelineOption)))
            {
                PipelineEditName = option.PipelineName;
                NotifySelectedPipelinePersistenceStatusChanged();
                return;
            }

            NotifySelectedPipelinePersistenceStatusChanged();
            SelectedPipelinePreviewStep = null;
            PipelineEditName = option.PipelineName;
            LatestCatalogBenchmarkSummary = OpenVisionRecipeCatalogBenchmarkSummary.Empty;
            UpdateSelectedRecipeSummary();
            RefreshRecentBatchRunOptions();
            RefreshPinArrayGapValidationIdentityState();
            NotifyValidationSetEvidenceChanged();
            NotifyQualifiedSnapshotContextChanged();
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

        private void NotifyOperatorReviewChanged(bool includeGuidedNextAction = true)
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
            if (includeGuidedNextAction)
            {
                OnPropertyChanged(nameof(RecipeGuidedNextActionText));
            }
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
            return BuildValidationSetEvidence().AcceptanceText;
        }

        private string BuildValidationSetCalibrationText()
        {
            return BuildValidationSetEvidence().CalibrationText;
        }

        private OpenVisionRecipeValidationEvidence BuildValidationSetEvidence()
        {
            return validationEvidenceOwner.Build(
                NormalizeRecipeName(selectedRecipeName),
                SelectedPipelineOption?.PipelineName ?? string.Empty,
                SelectedPipelineOption != null);
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
                copyTextToClipboard(report);
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
                copyTextToClipboard(review);
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
            OpenVisionRecipeRunHistoryComparison comparison = runHistoryOrchestrationOwner.BuildComparison(
                SelectedRecentBatchRunOption,
                SelectedBenchmarkBaselineRunOption,
                RecentBatchRunOptions);
            RecentBatchRunComparisonRows = comparison.Rows;
            SelectedRecentBatchRunComparisonRow = comparison.SelectedRow;
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
                (int width, int height) = OpenVisionBitmapImagePreviewFactory.ReadPixelSize(imagePath);
                IReadOnlyList<OpenVisionRecipePinGapIntentSkill.RoiSample> samples =
                    OpenVisionRecipePinGapIntentSkill.CreateScaledRoiSamples(width, height);
                PinGapIntentRoiText = OpenVisionRecipePinGapIntentSkill.FormatRoiSamples(samples);
                StatusText = LocalText(
                    "핀 간격 ROI 샘플 제안: ",
                    "Suggested pin gap ROI samples: ")
                    + Path.GetFileName(imagePath)
                    + " ("
                    + width.ToString(CultureInfo.InvariantCulture)
                    + "x"
                    + height.ToString(CultureInfo.InvariantCulture)
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
            await RunValidationSuiteAsync();
        }

        private async Task RunValidationSuiteAsync()
        {
            if (!CanRunValidationSuite())
            {
                return;
            }

            await executionSession.RunValidationSuiteAsync(
                SelectedValidationSuiteScopeOption?.Key ?? OpenVisionRecipeValidationSuiteScopeOption.SelectedSampleKey,
                NormalizeRecipeName(selectedRecipeName),
                SelectedPipelineOption?.PipelineName ?? string.Empty,
                SelectedSampleOption,
                SelectedValidationSetOption);
        }

        private bool CanStopValidationSuite()
        {
            return executionSession.CanStop;
        }

        private void RequestValidationSuiteStop()
        {
            if (!CanStopValidationSuite())
            {
                return;
            }

            executionSession.RequestStop(LocalText(
                "현재 이미지 완료 후 중지하고 부분 결과를 저장합니다.",
                "Stopping after the current image and saving a partial result."));
        }

        private async void RunSelectedSampleCheck()
        {
            await RunSelectedSampleCheckAsync();
        }

        private async Task RunSelectedSampleCheckAsync()
        {
            if (!CanRunSelectedSampleCheck())
            {
                return;
            }

            await executionSession.RunSelectedSampleCheckAsync(
                NormalizeRecipeName(selectedRecipeName),
                SelectedPipelineOption?.PipelineName ?? string.Empty,
                SelectedSampleOption);
        }

        private async void RunSelectedSamplePairCheck()
        {
            await RunSelectedSamplePairCheckAsync();
        }

        private async Task RunSelectedSamplePairCheckAsync()
        {
            if (!CanRunSelectedSamplePairCheck())
            {
                return;
            }

            await executionSession.RunSelectedSamplePairCheckAsync(
                NormalizeRecipeName(selectedRecipeName),
                SelectedPipelineOption?.PipelineName ?? string.Empty,
                SelectedSampleOption);
        }

        private async void RunCatalogBenchmark()
        {
            await RunCatalogBenchmarkAsync();
        }

        private async Task RunCatalogBenchmarkAsync()
        {
            if (!CanRunCatalogBenchmark())
            {
                return;
            }

            await executionSession.RunCatalogBenchmarkAsync(
                NormalizeRecipeName(selectedRecipeName),
                SelectedPipelineOption?.PipelineName ?? string.Empty);
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
                copyTextToClipboard(prompt);
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
                copyTextToClipboard(bundle);
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
                if (!clipboardContainsText())
                {
                    LlmXmlDraftPasteStatusText = LocalText("클립보드에 붙여넣을 XML 텍스트가 없습니다.", "Clipboard does not contain XML text.");
                    return;
                }

                string xmlText = readClipboardText();
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
            else if (OpenVisionRecipeLlmIntent.IsLocatorRelativeBlobTemplate(SelectedLlmToolTemplate))
            {
                CreateLocatorRelativeBlobIntentXmlDraft();
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
                && !executionSession.IsValidationSuiteRunning
                && validationSetDocumentOwner.StorageReady
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
            OpenVisionRecipePinArrayGapValidationIdentityResult result =
                pinArrayGapValidationIdentityOwner.Freeze(
                    recipeName,
                    SelectedPipelineOption?.PipelineName ?? string.Empty,
                    PinArrayGapTrainValidationSetOption,
                    PinArrayGapValidationValidationSetOption,
                    PinArrayGapTestValidationSetOption);
            if (!result.Succeeded)
            {
                IsPinArrayGapValidationIdentityFrozen = false;
                PinArrayGapValidationStatusText = LocalText("2단계 검토 필요 | ", "PHASE 2 REVIEW | ") + result.Error;
                StatusText = PinArrayGapValidationStatusText;
                RefreshCommandState();
                return;
            }

            IsPinArrayGapValidationIdentityFrozen = true;
            PinArrayGapValidationStatusText = BuildPinArrayGapFrozenStatus(result.Record);
            StatusText = PinArrayGapValidationStatusText;
            RefreshCommandState();
        }

        private bool CanOpenPinArrayGapValidationRuns()
        {
            return OpenVisionRecipeLlmIntent.IsPinArrayGapTemplate(SelectedLlmToolTemplate)
                && validationSetDocumentOwner.StorageReady
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
            OpenVisionRecipePinArrayGapValidationIdentityResult result =
                pinArrayGapValidationIdentityOwner.Evaluate(
                    recipeName,
                    SelectedPipelineOption?.PipelineName ?? string.Empty,
                    PinArrayGapTrainValidationSetOption,
                    PinArrayGapValidationValidationSetOption,
                    PinArrayGapTestValidationSetOption);
            if (!result.Succeeded)
            {
                PinArrayGapValidationStatusText = result.Error.Contains("does not exist", StringComparison.OrdinalIgnoreCase)
                    ? LocalText(
                        "2단계 미고정 | 세 분할과 선택된 XML을 검토한 뒤 검증 기준을 고정하세요.",
                        "PHASE 2 NOT FROZEN | Review the three splits and selected XML, then freeze identity.")
                    : LocalText("2단계 검토 필요 | ", "PHASE 2 REVIEW | ") + result.Error;
                return;
            }

            if (!result.Matches)
            {
                PinArrayGapValidationStatusText = LocalText(
                    "2단계 변경됨 | XML 또는 세트 내용이 고정 기록과 다릅니다. 검토 후 다시 고정하세요.",
                    "PHASE 2 STALE | XML or set content differs from the frozen record. Review and freeze again.");
                return;
            }

            IsPinArrayGapValidationIdentityFrozen = true;
            PinArrayGapValidationStatusText = BuildPinArrayGapFrozenStatus(result.Record);
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
            InvalidateLocatorEvidenceCompilation();
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
                (int width, int height) = OpenVisionBitmapImagePreviewFactory.ReadPixelSize(sourceImagePath);
                if (!OpenVisionRecipePinArrayGapIntentSkill.TryValidateV1Inputs(
                        PinArrayGapMeasurementText,
                        PinArrayGapPolarityText,
                        OpenVisionRecipePinArrayGapIntentSkill.SupportedUnitMode,
                        rowRois,
                        width,
                        height,
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

        private void CreateLocatorRelativeBlobIntentXmlDraft()
        {
            if (!OpenVisionRecipeLocatorRelativeBlobIntentSkill.TryCreatePlan(
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
                    BlobCountIntentThresholdText,
                    BlobCountIntentMinAreaText,
                    BlobCountIntentMaxAreaText,
                    ResolveLocatorRelativeBlobExpectedCountText(),
                    out OpenVisionRecipeLocatorRelativeBlobIntentSkill.Plan plan,
                    out string message))
            {
                StatusText = LocalText(
                    "Evidence-constrained locator Blob 입력을 확인하세요: ",
                    "Check evidence-constrained locator Blob inputs: ") + message;
                return;
            }

            SelectedLlmToolTemplate = OpenVisionGuidedSetupCatalog.LocatorRelativeBlobTemplate;
            LlmPromptText = BuildLlmPromptText();
            LlmXmlDraftText = SerializePipelineToXmlText(
                OpenVisionRecipeLocatorRelativeBlobIntentSkill.CreateMeasurementPipeline(plan));
            ValidateLlmXmlDraftText(false);
            StatusText = plan.IsMeasurementOnly
                ? LocalText(
                    "Evidence-constrained locator Blob 측정 전용 XML 초안을 만들었습니다. Candidate ID 증거 패킷과 명시적 Run 전까지 판정하지 않습니다.",
                    "Created an evidence-constrained locator Blob measurement-only XML draft. It remains unjudged until a Candidate ID evidence packet and explicit Run.")
                : LocalText(
                    "Evidence-constrained locator Blob XML 초안을 만들었습니다. ResultCount gate는 작업자가 명시한 동일 개수에만 적용되며 Preview/Run은 실행하지 않았습니다.",
                    "Created an evidence-constrained locator Blob XML draft. The ResultCount gate uses only the operator-supplied exact count; Preview/Run was not executed.");
        }

        private string ResolveLocatorRelativeBlobExpectedCountText()
        {
            if (!OpenVisionRecipeBlobCountIntentSkill.TryParseNonNegativeInt(
                    BlobCountIntentMinCountText,
                    out int minimum)
                || !OpenVisionRecipeBlobCountIntentSkill.TryParseNonNegativeInt(
                    BlobCountIntentMaxCountText,
                    out int maximum)
                || minimum != maximum)
            {
                return string.Empty;
            }

            return minimum.ToString(CultureInfo.InvariantCulture);
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
                HybridMinimumValidPixelRatioText = HybridMinimumValidPixelRatioText,
                BlobCountThresholdText = BlobCountIntentThresholdText,
                BlobCountMinCountText = BlobCountIntentMinCountText,
                BlobCountMaxCountText = BlobCountIntentMaxCountText,
                BlobCountMinAreaText = BlobCountIntentMinAreaText,
                BlobCountMaxAreaText = BlobCountIntentMaxAreaText
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
                        : PinGapIntentRoiText,
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
                BlobCountIntentThresholdText,
                BlobCountIntentMinAreaText,
                BlobCountIntentMaxAreaText,
                OpenVisionRecipeLlmIntent.IsLocatorRelativeBlobTemplate(SelectedLlmToolTemplate)
                    ? ResolveLocatorRelativeBlobExpectedCountText()
                    : null);
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
            if (executionSession.IsValidationSuiteRunning
                || executionSession.IsCatalogBenchmarkRunning
                || executionSession.IsSampleCheckRunning
                || !CanUseSelectedPipeline()
                || HasSelectedRecipePersistenceFailure
                || SelectedSampleOption?.Sample == null)
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
            if (executionSession.IsValidationSuiteRunning
                || executionSession.IsCatalogBenchmarkRunning
                || executionSession.IsPairCheckRunning
                || executionSession.IsSampleCheckRunning
                || !CanUseSelectedPipeline()
                || HasSelectedRecipePersistenceFailure
                || SelectedSampleOption?.Sample == null)
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
            if (executionSession.IsValidationSuiteRunning
                || executionSession.IsCatalogBenchmarkRunning
                || executionSession.IsPairCheckRunning
                || executionSession.IsSampleCheckRunning
                || !CanUseSelectedPipeline()
                || HasSelectedRecipePersistenceFailure)
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

            return executionSession.HasCatalogBenchmarkSamples;
        }

        private bool CanRunValidationSuite()
        {
            if (executionSession.IsValidationSuiteRunning)
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
            return CanRunLocalValidationSet(SelectedValidationSetOption);
        }

        private void NotifySelectedPipelinePersistenceStatusChanged()
        {
            OnPropertyChanged(
                nameof(HasSelectedPipelinePersistenceStatus));
            OnPropertyChanged(
                nameof(HasSelectedPipelinePersistenceFailure));
            OnPropertyChanged(
                nameof(SelectedPipelinePersistenceStatusText));
            OnPropertyChanged(
                nameof(SelectedPipelinePersistenceHelpText));
            OnPropertyChanged(
                nameof(HasSelectedRecipePersistenceStatus));
            OnPropertyChanged(
                nameof(HasSelectedRecipePersistenceFailure));
            OnPropertyChanged(
                nameof(SelectedRecipePersistenceStatusText));
            OnPropertyChanged(
                nameof(SelectedRecipePersistenceHelpText));
        }

        private bool CanRunLocalValidationSet(
            OpenVisionRecipeValidationSetOption option)
        {
            if (!validationSetDocumentOwner.StorageReady
                || executionSession.IsValidationSuiteRunning
                || executionSession.IsCatalogBenchmarkRunning
                || executionSession.IsPairCheckRunning
                || executionSession.IsSampleCheckRunning
                || !CanUseSelectedPipeline()
                || HasSelectedRecipePersistenceFailure
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
            FilteredPipelineOptions = recipePipelineOptionProjectionOwner.Filter(
                PipelineOptions,
                PipelineFilterText);
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

            executionSession.HasCatalogBenchmarkSamples = options.Any(option =>
                option?.Sample?.CatalogSourceKind == VisionPipelineSampleCatalogSourceKind.Product);
            SampleOptions = options;
            SelectedSampleOption = options.FirstOrDefault(option =>
                    string.Equals(option.SampleName, previousSampleName, StringComparison.OrdinalIgnoreCase))
                ?? options.FirstOrDefault();
        }

        private void RefreshPipelineOptions(string preferredPipelineName = null, bool refreshCommandState = true)
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
                if (pipelineNames.Length == 0
                    && VisionPipelineStorage.TryGetPersistenceState(
                        recipeName,
                        activePipelineName,
                        out _))
                {
                    pipelineNames = new[] { activePipelineName };
                }
            }

            OpenVisionRecipePipelineOptionProjection projection =
                recipePipelineOptionProjectionOwner.Project(
                    new OpenVisionRecipePipelineOptionProjectionRequest
                    {
                        RecipeName = recipeName,
                        PipelineNames = pipelineNames,
                        ActivePipelineName = activePipelineName,
                        PreferredPipelineName = preferredPipelineName,
                        NormalizedPreferredPipelineName = NormalizePipelineName(preferredPipelineName),
                        PreviousSelectedPipelineName = selectedPipelineOption?.PipelineName
                    });
            PipelineOptions = projection.Options;
            OpenVisionRecipePipelineOption selectedOption = projection.SelectedOption;

            if (!EqualityComparer<OpenVisionRecipePipelineOption>.Default.Equals(selectedPipelineOption, selectedOption))
            {
                selectedPipelineOption = selectedOption;
                OnPropertyChanged(nameof(SelectedPipelineOption));
                NotifySelectedPipelinePersistenceStatusChanged();
            }

            PipelineEditName = selectedOption?.PipelineName ?? string.Empty;
            LatestCatalogBenchmarkSummary = OpenVisionRecipeCatalogBenchmarkSummary.Empty;
            RefreshRecentBatchRunOptions();
            UpdateSelectedRecipeSummary();
            RefreshPinArrayGapValidationIdentityState();
            if (refreshCommandState)
            {
                RefreshCommandState();
            }
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
            return stepPreviewNavigationOwner.Find(
                SelectedRecipeSummary?.PipelinePreviewSteps,
                failedStep);
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
            int pipelineCount =
                PipelineOptions?.Count
                ?? pipelineNames.Length;
            DateTime? lastWriteTime = RecipeWorkspaceService.GetRecipeLastWriteTime(recipeName);
            bool storedXmlLoaded = VisionPipelineStorage.TryLoadFromFile(
                pipelinePath,
                out VisionPipeline activePipeline,
                out string xmlMessage);
            bool xmlOk =
                storedXmlLoaded
                && !HasSelectedRecipePersistenceFailure;
            if (storedXmlLoaded
                && HasSelectedRecipePersistenceFailure)
            {
                xmlMessage =
                    SelectedRecipePersistenceStatusText;
            }
            SelectedRecipeSummary = recipeManagerSummaryProjectionOwner.Project(
                new OpenVisionRecipeManagerSummaryProjectionRequest
                {
                    RecipeName = recipeName,
                    ActivePipelineName = activePipelineName,
                    PreviewPipelineName = previewPipelineName,
                    PipelineCount = pipelineCount,
                    LastWriteTime = lastWriteTime,
                    XmlValid = xmlOk,
                    XmlMessage = xmlMessage,
                    PipelinePath = pipelinePath,
                    Pipeline = activePipeline,
                    LayerCardProvider = layerCardProvider
                });
            OnPropertyChanged(nameof(PipelineVariantComparisonReport));
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

        private void PrepareSelectedRunFailureCorrection()
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
                StatusText = LocalText("보존된 실패 샘플 이미지 경로를 찾을 수 없습니다.", "Could not find the retained failed sample image path.");
                return;
            }

            if (IsSamePipelinePreviewStep(SelectedPipelinePreviewStep, step)
                && selectedStepEditSession.IsDirty
                && !TryLeaveSelectedStepEdit(
                    OpenVisionRecipePendingEditTransitionKind.Step,
                    step.DisplayText))
            {
                StatusText = LocalText(
                    "현재 Step 편집 전환이 취소되어 실패 수정 준비를 중단했습니다.",
                    "Correction preparation stopped because the current Step edit transition was cancelled.");
                return;
            }

            SelectedPipelinePreviewStep = step;
            if (!IsSamePipelinePreviewStep(SelectedPipelinePreviewStep, step))
            {
                StatusText = LocalText(
                    "현재 Step 편집 전환이 취소되어 실패 수정 준비를 중단했습니다.",
                    "Correction preparation stopped because the current Step edit transition was cancelled.");
                return;
            }

            if (!LoadSelectedStepParametersForEdit(updateStatus: true))
            {
                return;
            }

            if (!loadImageIntoLayer(step.InputLayer, sampleImagePath))
            {
                StatusText = LocalText(
                    "실패 Step 파라미터는 불러왔지만 보존된 샘플을 입력 레이어에 로드하지 못했습니다: ",
                    "Loaded the failed Step parameters, but could not load the retained sample into the input layer: ")
                    + step.InputLayer;
                CommandManager.InvalidateRequerySuggested();
                return;
            }

            openPipelineXmlSteps();
            StatusText = LocalText(
                "실패 수정 준비 완료: 샘플과 Step 파라미터를 불러왔습니다. PropertyGrid에서 수정한 뒤 명시적으로 Preview/Run 하세요.",
                "Correction preparation complete: the sample and Step parameters are loaded. Edit in the PropertyGrid, then explicitly Preview/Run.");
            CommandManager.InvalidateRequerySuggested();
        }

        private bool CanPrepareSelectedRunFailureCorrection()
        {
            return CanLoadSelectedRunSampleImageToInputLayer();
        }

        private void RerunCorrectedOutput()
        {
            if (IsSelectedStepEditDirty)
            {
                StatusText = LocalText(
                    "재검사를 차단했습니다. 저장하지 않은 Step 편집을 XML 반영하거나 취소하세요.",
                    "Rerun blocked. Apply or discard the pending Step edit first.");
                return;
            }

            if (!IsSelectedRunLocalValidationSet())
            {
                RunSelectedSamplePairCheck();
                return;
            }

            if (!TryResolveSelectedRunValidationSet(
                    out OpenVisionRecipeValidationSetOption validationSet,
                    out string reason))
            {
                StatusText = reason;
                return;
            }

            OpenVisionRecipeValidationSuiteScopeOption localScope =
                ValidationSuiteScopeOptions.FirstOrDefault(option => string.Equals(
                    option?.Key,
                    OpenVisionRecipeValidationSuiteScopeOption.LocalValidationSetKey,
                    StringComparison.OrdinalIgnoreCase));
            if (localScope == null)
            {
                StatusText = LocalText(
                    "로컬 검증 세트 실행 범위를 찾을 수 없습니다.",
                    "The Local Validation Set suite scope is unavailable.");
                return;
            }

            SelectedValidationSuiteScopeOption = localScope;
            SelectedValidationSetOption = validationSet;
            if (!CanRunLocalValidationSet(validationSet))
            {
                StatusText = LocalText(
                    "동일 검증 세트를 재실행할 수 없습니다. 이미지 경로, 잠금된 파이프라인, 현재 실행 상태를 확인하세요: ",
                    "The same validation set cannot be rerun. Check image paths, locked pipeline identity, and current execution state: ")
                    + validationSet.Name;
                return;
            }

            StatusText = LocalText(
                "동일 검증 세트 재실행 시작: ",
                "Started rerunning the same validation set: ")
                + validationSet.Name;
            RunValidationSuite();
        }

        private bool CanRerunCorrectedOutput()
        {
            if (IsSelectedStepEditDirty)
            {
                return false;
            }

            if (!IsSelectedRunLocalValidationSet())
            {
                return CanRunSelectedSamplePairCheck();
            }

            return TryResolveSelectedRunValidationSet(
                    out OpenVisionRecipeValidationSetOption validationSet,
                    out _)
                && CanRunLocalValidationSet(validationSet);
        }

        private bool IsSelectedRunLocalValidationSet()
        {
            string suiteKind =
                SelectedRecentBatchRunOption?.RunSummary?.SuiteKind
                ?? string.Empty;
            return string.Equals(
                    suiteKind,
                    "LocalValidationSet",
                    StringComparison.OrdinalIgnoreCase)
                || string.Equals(
                    suiteKind,
                    "LocalValidationSetPartial",
                    StringComparison.OrdinalIgnoreCase);
        }

        private bool TryResolveSelectedRunValidationSet(
            out OpenVisionRecipeValidationSetOption validationSet,
            out string reason)
        {
            validationSet = null;
            reason = string.Empty;
            VisionPipelineBatchRunSummary summary =
                SelectedRecentBatchRunOption?.RunSummary;
            if (summary == null || !IsSelectedRunLocalValidationSet())
            {
                reason = LocalText(
                    "선택 실행은 로컬 검증 세트 이력이 아닙니다.",
                    "The selected run is not Local Validation Set history.");
                return false;
            }

            string recipeName = NormalizeRecipeName(selectedRecipeName);
            string pipelineName = SelectedPipelineOption?.PipelineName
                ?? string.Empty;
            if (!string.Equals(
                    summary.RecipeName,
                    recipeName,
                    StringComparison.Ordinal)
                || !string.Equals(
                    summary.PipelineName,
                    pipelineName,
                    StringComparison.Ordinal))
            {
                reason = LocalText(
                    "선택 이력의 레시피/파이프라인이 현재 편집 대상과 다릅니다.",
                    "The selected run recipe/pipeline differs from the current edit target.");
                return false;
            }

            validationSet = ValidationSetOptions.FirstOrDefault(option =>
                string.Equals(
                    option?.Name,
                    summary.SuiteName,
                    StringComparison.Ordinal));
            if (validationSet == null)
            {
                reason = LocalText(
                    "선택 이력의 원본 검증 세트를 찾을 수 없습니다: ",
                    "Could not find the source validation set for the selected run: ")
                    + summary.SuiteName;
                return false;
            }

            return true;
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

            path = SelectedRecentBatchSampleResultOption?.SampleImagePath;
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
            return stepPreviewNavigationOwner.GetByOffset(
                SelectedRecipeSummary?.PipelinePreviewSteps,
                SelectedPipelinePreviewStep,
                offset);
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
            TryApplySelectedStepParameters();
        }

        private void ResetSelectedStepDisplayDefaults()
        {
            if (!VisionPipelineOverlayMergePropertyAdapter.TryResetRenderingDefaults(
                SelectedStepEditObject))
            {
                SetSelectedStepEditStatus(OpenVisionRecipeText.Local(
                    "선택 Step에는 표시 기본값이 없습니다.",
                    "The selected Step has no display defaults."));
                return;
            }

            OnPropertyChanged(nameof(SelectedStepEditObject));
            MarkSelectedStepEditDirty();
            OnPropertyChanged(nameof(CorrectedOutputReviewText));
            SetSelectedStepEditStatus(OpenVisionRecipeText.Local(
                "표시 설정을 기존 호환 기본값으로 되돌렸습니다. XML 반영을 눌러 저장하세요.",
                "Display settings were reset to backward-compatible defaults. Apply to XML to save."));
        }

        private bool CanResetSelectedStepDisplayDefaults()
        {
            return VisionPipelineOverlayMergePropertyAdapter.IsProperty(
                SelectedStepEditObject);
        }

        private bool TryApplySelectedStepParameters()
        {
            if (SelectedStepEditObject == null && !LoadSelectedStepParametersForEdit(updateStatus: true))
            {
                return false;
            }

            if (!commitSelectedStepEdit())
            {
                SetSelectedStepEditStatus(OpenVisionRecipeText.Local("보류 중인 PropertyGrid 편집을 확정하지 못했습니다.", "Could not commit the pending PropertyGrid edit."));
                return false;
            }

            OpenVisionRecipeStepEditLoadResult selectedStepLoad = LoadSelectedStepEdit();
            if (!selectedStepLoad.Succeeded)
            {
                SetSelectedStepEditStatus(selectedStepLoad.Message);
                return false;
            }

            string recipeName = selectedStepLoad.RecipeName;
            string pipelineName = selectedStepLoad.PipelineName;
            VisionPipeline pipeline = selectedStepLoad.Pipeline;
            VisionPipelineStep step = selectedStepLoad.Step;

            OpenVisionRecipeStepEditApplyResult applyResult = stepEditApplyOwner.Apply(
                recipeName,
                pipelineName,
                pipeline,
                step,
                SelectedStepEditObject);
            if (!applyResult.Succeeded)
            {
                OpenVisionRecipeStepEditApplyProjection failureProjection =
                    stepEditApplyProjectionOwner.ProjectFailure(applyResult);
                SetSelectedStepEditStatus(failureProjection.SelectedStepEditStatusText);
                if (!string.IsNullOrWhiteSpace(failureProjection.ShellStatusText))
                {
                    StatusText = failureProjection.ShellStatusText;
                }

                return false;
            }

            string validationMessage = applyResult.ValidationMessage;
            int selectedIndex = SelectedPipelinePreviewStep?.Index ?? 0;
            selectedStepEditSession.MarkClean();
            RefreshPipelineOptions(pipelineName);
            SelectedPipelinePreviewStep = SelectedRecipeSummary?.PipelinePreviewSteps?
                .FirstOrDefault(stepPreview => stepPreview.Index == selectedIndex);
            LoadSelectedStepParametersForEdit(updateStatus: false);
            OpenVisionRecipeStepEditApplyProjection successProjection =
                stepEditApplyProjectionOwner.ProjectSuccess(
                    pipelineName,
                    selectedIndex,
                    SelectedPipelinePreviewStep,
                    validationMessage,
                    IsSelectedRunLocalValidationSet());
            SetSelectedStepEditStatus(successProjection.SelectedStepEditStatusText);
            SetCorrectedOutputReview(successProjection.CorrectedOutputReviewText);
            StatusText = successProjection.ShellStatusText;
            return true;
        }

        private bool CanApplySelectedStepParameters()
        {
            return SelectedStepEditObject != null;
        }

        public void MarkSelectedStepEditDirty()
        {
            selectedStepEditSession.MarkDirty(
                OpenVisionRecipeText.Local(
                    "편집됨: XML 반영 전입니다.",
                    "Edited: not yet applied to XML."));
        }

        internal void FailNextRecipeStepSaveForTest()
        {
            stepEditApplyOwner.FailNextSaveForTest();
        }

        internal void FailNextRecipeStepRoundTripValidationForTest()
        {
            stepEditApplyOwner.FailNextRoundTripValidationForTest();
        }

        private bool LoadSelectedStepParametersForEdit(bool updateStatus)
        {
            OpenVisionRecipeStepEditLoadResult selectedStepLoad = LoadSelectedStepEdit();
            if (!selectedStepLoad.Succeeded)
            {
                ClearSelectedStepEdit();
                if (updateStatus)
                {
                    SetSelectedStepEditStatus(selectedStepLoad.Message);
                }

                return false;
            }

            selectedStepEditSession.Load(
                selectedStepLoad.EditObject,
                OpenVisionRecipeText.Local("불러옴: ", "Loaded: ")
                + selectedStepLoad.PipelineName
                + " / Step "
                + (SelectedPipelinePreviewStep?.Index ?? 0).ToString(CultureInfo.InvariantCulture),
                updateStatus);

            return true;
        }

        private OpenVisionRecipeStepEditLoadResult LoadSelectedStepEdit()
        {
            return stepEditLoader.Load(
                NormalizeRecipeName(selectedRecipeName),
                selectedPipelineOption?.PipelineName,
                SelectedPipelinePreviewStep);
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
            selectedStepEditSession.SetStatus(value);
        }

        private void SetCorrectedOutputReview(string value)
        {
            selectedStepEditSession.SetCorrectedOutputReview(value);
        }

        private void ClearSelectedStepEdit()
        {
            selectedStepEditSession.Clear();
        }

        internal bool TryCloseRecipeManager()
        {
            return TryLeaveSelectedStepEdit(
                OpenVisionRecipePendingEditTransitionKind.RecipeManagerClose,
                OpenVisionRecipeText.Local("Recipe Manager 닫기", "Close Recipe Manager"));
        }

        private bool TryLeaveSelectedStepEdit(
            OpenVisionRecipePendingEditTransitionKind kind,
            string targetName)
        {
            return pendingEditTransitionController.TryLeave(
                selectedStepEditSession.IsDirty,
                new OpenVisionRecipePendingEditRequest
                {
                    Kind = kind,
                    RecipeName = NormalizeRecipeName(selectedRecipeName),
                    PipelineName = selectedPipelineOption?.PipelineName ?? string.Empty,
                    StepName = selectedPipelinePreviewStep?.DisplayText ?? string.Empty,
                    TargetName = targetName ?? string.Empty
                });
        }

        private bool IsSamePipelinePreviewStep(
            OpenVisionRecipePipelineStepPreview left,
            OpenVisionRecipePipelineStepPreview right)
        {
            return stepPreviewNavigationOwner.AreSame(left, right);
        }

        private void OnSelectedStepEditSessionPropertyChanged(
            object sender,
            System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e?.PropertyName)
            {
                case nameof(OpenVisionRecipeStepEditSessionViewModel.EditObject):
                    OnPropertyChanged(nameof(SelectedStepEditObject));
                    OnPropertyChanged(nameof(HasSelectedStepEditObject));
                    OnPropertyChanged(nameof(HasSelectedOverlayMergeEditObject));
                    RefreshCommandState();
                    break;
                case nameof(OpenVisionRecipeStepEditSessionViewModel.IsDirty):
                    OnPropertyChanged(nameof(IsSelectedStepEditDirty));
                    NotifyQualifiedSnapshotContextChanged();
                    RefreshCommandState();
                    break;
                case nameof(OpenVisionRecipeStepEditSessionViewModel.StatusText):
                    OnPropertyChanged(nameof(SelectedStepEditStatusText));
                    RefreshCommandState();
                    break;
                case nameof(OpenVisionRecipeStepEditSessionViewModel.CorrectedOutputReviewText):
                    OnPropertyChanged(nameof(CorrectedOutputReviewText));
                    break;
            }
        }

        private void OnExecutionSessionPropertyChanging(object sender, System.ComponentModel.PropertyChangingEventArgs e)
        {
            switch (e?.PropertyName)
            {
                case nameof(OpenVisionRecipeExecutionSessionViewModel.LatestSampleRunSummary):
                case nameof(OpenVisionRecipeExecutionSessionViewModel.LatestPairRunSummary):
                case nameof(OpenVisionRecipeExecutionSessionViewModel.LatestCatalogBenchmarkSummary):
                    OnPropertyChanging(e.PropertyName);
                    break;
            }
        }

        private void OnExecutionSessionPropertyChanged(
            object sender,
            System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e?.PropertyName)
            {
                case nameof(OpenVisionRecipeExecutionSessionViewModel.ExecutionStatusText):
                    StatusText = executionSession.ExecutionStatusText;
                    break;
                case nameof(OpenVisionRecipeExecutionSessionViewModel.LatestSampleRunSummary):
                    OnPropertyChanged(nameof(LatestSampleRunSummary));
                    OnPropertyChanged(nameof(HasCurrentRecipeSampleExecution));
                    OnPropertyChanged(nameof(RecipeOverviewLastResultValueText));
                    OnPropertyChanged(nameof(RecipeOverviewLastResultToolTipText));
                    NotifyOperatorReviewChanged();
                    OnPropertyChanged(nameof(RecipeGuidedSetupText));
                    OnPropertyChanged(nameof(PinGapIntentLatestRunText));
                    OnPropertyChanged(nameof(BlobCountIntentLatestRunText));
                    OnPropertyChanged(nameof(ContourCountIntentLatestRunText));
                    OnPropertyChanged(nameof(ValidationSuiteSummaryText));
                    break;
                case nameof(OpenVisionRecipeExecutionSessionViewModel.LatestPairRunSummary):
                    OnPropertyChanged(nameof(LatestPairRunSummary));
                    SelectedPairSampleResult = OpenVisionRecipeRunHistoryPresenter.SelectDefaultPairSampleResult(LatestPairRunSummary);
                    RefreshSampleMatrixRows();
                    NotifyOperatorReviewChanged();
                    OnPropertyChanged(nameof(FailureReviewText));
                    OnPropertyChanged(nameof(PipelineSelectedStepOperatorContextText));
                    OnPropertyChanged(nameof(RecipeGuidedSetupText));
                    OnPropertyChanged(nameof(ValidationSuiteSummaryText));
                    break;
                case nameof(OpenVisionRecipeExecutionSessionViewModel.LatestCatalogBenchmarkSummary):
                    OnPropertyChanged(nameof(LatestCatalogBenchmarkSummary));
                    OnPropertyChanged(nameof(CatalogBenchmarkSummaryText));
                    OnPropertyChanged(nameof(CatalogBenchmarkDetailText));
                    OnPropertyChanged(nameof(RecipeGuidedSetupText));
                    OnPropertyChanged(nameof(ValidationSuiteSummaryText));
                    NotifyOperatorReviewChanged();
                    break;
                case nameof(OpenVisionRecipeExecutionSessionViewModel.IsValidationSuiteRunning):
                    OnPropertyChanged(nameof(RunValidationSuiteText));
                    OnPropertyChanged(nameof(ValidationSuiteSummaryText));
                    OnPropertyChanged(nameof(ValidationSetNextActionText));
                    break;
                case nameof(OpenVisionRecipeExecutionSessionViewModel.IsLocalValidationSetRunning):
                    OnPropertyChanged(nameof(IsLocalValidationSetRunning));
                    break;
                case nameof(OpenVisionRecipeExecutionSessionViewModel.IsSampleCheckRunning):
                    OnPropertyChanged(nameof(RunSelectedSampleCheckText));
                    break;
                case nameof(OpenVisionRecipeExecutionSessionViewModel.IsPairCheckRunning):
                    OnPropertyChanged(nameof(RunSelectedSamplePairCheckText));
                    break;
                case nameof(OpenVisionRecipeExecutionSessionViewModel.IsCatalogBenchmarkRunning):
                    OnPropertyChanged(nameof(RunCatalogBenchmarkText));
                    OnPropertyChanged(nameof(RunCatalogBenchmarkShortText));
                    break;
                case nameof(OpenVisionRecipeExecutionSessionViewModel.StopRequested):
                    OnPropertyChanged(nameof(StopValidationSuiteText));
                    break;
                case nameof(OpenVisionRecipeExecutionSessionViewModel.StatusText):
                    OnPropertyChanged(nameof(ValidationSuiteStatusText));
                    OnPropertyChanged(nameof(ValidationSuiteSummaryText));
                    OnPropertyChanged(nameof(ValidationSetNextActionText));
                    break;
            }
        }

        private void OnExecutionBatchRunSaved(object sender, EventArgs e)
        {
            RefreshRecentBatchRunOptions();
        }

        private void OnExecutionCommandStateChanged(object sender, EventArgs e)
        {
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

        private void SetSelectedRecipeName(string recipeName, bool refreshCommandState = true)
        {
            string normalized = NormalizeRecipeName(recipeName);
            bool changed = !string.Equals(selectedRecipeName, normalized, StringComparison.Ordinal);
            selectedRecipeName = normalized;
            if (changed)
            {
                SelectedPipelinePreviewStep = null;
                llmXmlDraftImportReady = false;
                validationSetSelectionOwner.ClearPinnedSelections();
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
            RefreshPipelineOptions(preferredPipelineName, refreshCommandState);
            RefreshValidationSetOptions(refreshCommandState: refreshCommandState);
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
            OnPropertyChanged(nameof(CorrectedOutputRerunText));
            OnPropertyChanged(nameof(CorrectedOutputRerunToolTipText));
            OnPropertyChanged(nameof(QualifiedSnapshotPreflightText));
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



        #endregion


        #region LLM/XML draft and evidence review

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

        private void LoadLocatorEvidencePacket()
        {
            string path = selectLocatorEvidencePacketPath();
            if (string.IsNullOrWhiteSpace(path))
            {
                LocatorEvidencePacketStatusText = LocalText(
                    "Evidence Packet 로드가 취소되었습니다.",
                    "Evidence Packet load canceled.");
                return;
            }

            LoadLocatorEvidencePacketFromPath(path);
        }

        public bool LoadLocatorEvidencePacketFromPath(string path)
        {
            string normalizedPath;
            try
            {
                normalizedPath = string.IsNullOrWhiteSpace(path)
                    ? string.Empty
                    : Path.GetFullPath(path.Trim());
            }
            catch (Exception exception)
            {
                LocatorEvidencePacketPath = path ?? string.Empty;
                loadedLocatorEvidencePacket = null;
                loadedLocatorEvidencePacketPath = string.Empty;
                locatorEvidenceCompilationReady = false;
                LocatorEvidenceOverlayImage = null;
                ClearLoadedLocatorEvidenceReviewDecision();
                LocatorEvidencePacketStatusText = LocalText(
                    "Evidence Packet 경로가 잘못되었습니다: ",
                    "The Evidence Packet path is invalid: ") + exception.GetBaseException().Message;
                LocatorEvidenceReviewText = LocalText(
                    "유효한 .packet.json 경로를 선택하세요.",
                    "Select a valid .packet.json path.");
                OnPropertyChanged(nameof(IsLocatorEvidencePacketLoaded));
                OnPropertyChanged(nameof(IsLocatorEvidenceCompilationReady));
                RefreshCommandState();
                return false;
            }
            LocatorEvidencePacketPath = normalizedPath;
            loadedLocatorEvidencePacket = null;
            loadedLocatorEvidencePacketPath = string.Empty;
            locatorEvidenceCompilationReady = false;
            LocatorEvidenceOverlayImage = null;
            ClearLoadedLocatorEvidenceReviewDecision();

            if (!OpenVisionRecipeLocatorRelativeBlobEvidencePacket.TryLoad(
                    normalizedPath,
                    out OpenVisionRecipeLocatorRelativeBlobEvidencePacket packet,
                    out string message))
            {
                LocatorEvidencePacketStatusText = LocalText(
                    "Evidence Packet 검증 NG: ",
                    "Evidence Packet validation failed: ") + message;
                LocatorEvidenceReviewText = LocalText(
                    "해시·Candidate·좌표계 검증에 실패했습니다. Packet을 가져오지 않았고 실행하지 않았습니다.",
                    "Hash, candidate, or coordinate-frame validation failed. The packet was not applied and nothing was executed.");
                OnPropertyChanged(nameof(IsLocatorEvidencePacketLoaded));
                OnPropertyChanged(nameof(IsLocatorEvidenceCompilationReady));
                RefreshCommandState();
                return false;
            }

            try
            {
                BitmapSource overlay = OpenVisionBitmapImagePreviewFactory.CreateFromPath(packet.PreviewOverlayPath);
                loadedLocatorEvidencePacket = packet;
                loadedLocatorEvidencePacketPath = normalizedPath;
                LocatorEvidenceOverlayImage = overlay;
                LocatorEvidenceReviewText = BuildLocatorEvidenceReviewText(packet);
                LocatorEvidencePacketStatusText = LocalText(
                    "Evidence Packet 검증 OK. Compile을 눌러 현재 설정과 대조하세요. Preview/Run은 실행하지 않았습니다.",
                    "Evidence Packet verified. Select Compile to compare it with the current settings. Preview/Run was not executed.");
                LocatorEvidenceReviewDecisionStatusText = LocalText(
                    "Packet 검증 OK. review decision 파일을 로드해 후보를 명시적으로 검토하세요.",
                    "Packet verified. Load its review decision file for explicit candidate review.");
                OnPropertyChanged(nameof(IsLocatorEvidencePacketLoaded));
                OnPropertyChanged(nameof(IsLocatorEvidenceCompilationReady));
                RefreshCommandState();
                return true;
            }
            catch (Exception exception)
            {
                LocatorEvidencePacketStatusText = LocalText(
                    "Evidence Packet overlay를 표시할 수 없습니다: ",
                    "The Evidence Packet overlay could not be displayed: ") + exception.GetBaseException().Message;
                LocatorEvidenceReviewText = LocalText(
                    "검토용 overlay 디코딩에 실패했습니다. Packet을 적용하지 않았고 실행하지 않았습니다.",
                    "The review overlay could not be decoded. The packet was not applied and nothing was executed.");
                OnPropertyChanged(nameof(IsLocatorEvidencePacketLoaded));
                OnPropertyChanged(nameof(IsLocatorEvidenceCompilationReady));
                RefreshCommandState();
                return false;
            }
        }

        private void LoadLocatorEvidenceReviewDecision()
        {
            string path = selectLocatorEvidenceReviewDecisionPath();
            if (string.IsNullOrWhiteSpace(path))
            {
                LocatorEvidenceReviewDecisionStatusText = LocalText(
                    "review decision 로드가 취소되었습니다.",
                    "Review decision load canceled.");
                return;
            }

            LoadLocatorEvidenceReviewDecisionFromPath(path);
        }

        public bool LoadLocatorEvidenceReviewDecisionFromPath(string path)
        {
            if (loadedLocatorEvidencePacket == null
                || string.IsNullOrWhiteSpace(loadedLocatorEvidencePacketPath))
            {
                LocatorEvidenceReviewDecisionStatusText = LocalText(
                    "먼저 검증된 Evidence Packet을 로드하세요.",
                    "Load a verified Evidence Packet first.");
                return false;
            }

            string normalizedPath;
            try
            {
                normalizedPath = string.IsNullOrWhiteSpace(path)
                    ? string.Empty
                    : Path.GetFullPath(path.Trim());
            }
            catch (Exception exception)
            {
                ClearLoadedLocatorEvidenceReviewDecision();
                LocatorEvidenceReviewDecisionStatusText = LocalText(
                    "review decision 경로가 잘못되었습니다: ",
                    "The review decision path is invalid: ") + exception.GetBaseException().Message;
                RefreshCommandState();
                return false;
            }

            ClearLoadedLocatorEvidenceReviewDecision();
            LocatorEvidenceReviewDecisionPath = normalizedPath;
            if (!OpenVisionRecipeLocatorRelativeBlobReviewDecision.TryLoad(
                    normalizedPath,
                    out OpenVisionRecipeLocatorRelativeBlobReviewDecision decision,
                    out string message))
            {
                LocatorEvidenceReviewDecisionStatusText = LocalText(
                    "review decision 검증 NG: ",
                    "Review decision validation failed: ") + message;
                RefreshCommandState();
                return false;
            }

            if (!TryCreateCurrentLocatorRelativeBlobPlan(
                    out OpenVisionRecipeLocatorRelativeBlobIntentSkill.Plan plan,
                    out string planMessage))
            {
                LocatorEvidenceReviewDecisionStatusText = LocalText(
                    "현재 Guided Setup 설정과 review decision을 대조할 수 없습니다: ",
                    "The review decision cannot be compared with the current Guided Setup settings: ") + planMessage;
                RefreshCommandState();
                return false;
            }

            if (!decision.TryValidateAgainst(
                    loadedLocatorEvidencePacketPath,
                    loadedLocatorEvidencePacket,
                    plan,
                    out string currentMessage))
            {
                LocatorEvidenceReviewDecisionStatusText = LocalText(
                    "review decision이 현재 Packet 또는 설정과 일치하지 않습니다: ",
                    "The review decision does not match the current packet or settings: ") + currentMessage;
                RefreshCommandState();
                return false;
            }

            loadedLocatorEvidenceReviewDecision = decision;
            loadedLocatorEvidenceReviewDecisionPath = normalizedPath;
            LocatorEvidenceVisualCorrespondence = decision.VisualCorrespondence;
            LocatorEvidenceReviewer = decision.Reviewer;
            LocatorEvidenceReviewNotes = string.Equals(
                decision.Decision,
                OpenVisionRecipeLocatorRelativeBlobReviewDecision.Pending,
                StringComparison.Ordinal)
                ? string.Empty
                : decision.Notes;
            LocatorEvidenceReviewDecisionStatusText = BuildLocatorEvidenceReviewDecisionStatus(decision);
            OnPropertyChanged(nameof(IsLocatorEvidenceReviewDecisionLoaded));
            RefreshCommandState();
            return true;
        }

        private void ApproveLocatorEvidenceReviewDecision()
        {
            RecordLocatorEvidenceReviewDecision(OpenVisionRecipeLocatorRelativeBlobReviewDecision.Approved);
        }

        private void RejectLocatorEvidenceReviewDecision()
        {
            RecordLocatorEvidenceReviewDecision(OpenVisionRecipeLocatorRelativeBlobReviewDecision.Rejected);
        }

        private void RequestLocatorEvidenceReplacement()
        {
            RecordLocatorEvidenceReviewDecision(OpenVisionRecipeLocatorRelativeBlobReviewDecision.ReplacementRequested);
        }

        public bool RecordLocatorEvidenceReviewDecision(string decision)
        {
            if (loadedLocatorEvidencePacket == null
                || string.IsNullOrWhiteSpace(loadedLocatorEvidencePacketPath)
                || loadedLocatorEvidenceReviewDecision == null
                || string.IsNullOrWhiteSpace(loadedLocatorEvidenceReviewDecisionPath))
            {
                LocatorEvidenceReviewDecisionStatusText = LocalText(
                    "검토할 Packet과 review decision을 먼저 로드하세요.",
                    "Load the packet and review decision before recording a review.");
                return false;
            }

            if (string.Equals(
                    LocatorEvidenceVisualCorrespondence,
                    OpenVisionRecipeLocatorRelativeBlobReviewDecision.NotReviewed,
                    StringComparison.Ordinal))
            {
                LocatorEvidenceReviewDecisionStatusText = LocalText(
                    "Visual correspondence에서 PASS 또는 FAIL을 선택하세요.",
                    "Select PASS or FAIL for visual correspondence.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(LocatorEvidenceReviewer)
                || string.IsNullOrWhiteSpace(LocatorEvidenceReviewNotes))
            {
                LocatorEvidenceReviewDecisionStatusText = LocalText(
                    "Reviewer와 검토 Notes를 입력하세요.",
                    "Enter a reviewer and review notes.");
                return false;
            }

            if (!TryCreateCurrentLocatorRelativeBlobPlan(
                    out OpenVisionRecipeLocatorRelativeBlobIntentSkill.Plan plan,
                    out string planMessage))
            {
                LocatorEvidenceReviewDecisionStatusText = LocalText(
                    "현재 Guided Setup 설정이 검토 결정과 일치하지 않습니다: ",
                    "The current Guided Setup settings cannot be matched to the review decision: ") + planMessage;
                return false;
            }

            if (!loadedLocatorEvidenceReviewDecision.TryValidateAgainst(
                    loadedLocatorEvidencePacketPath,
                    loadedLocatorEvidencePacket,
                    plan,
                    out string beforeApplyMessage))
            {
                LocatorEvidenceReviewDecisionStatusText = LocalText(
                    "기존 review decision이 stale 상태입니다. 최신 Packet/결정을 다시 로드하세요: ",
                    "The existing review decision is stale. Reload the current packet/decision: ") + beforeApplyMessage;
                RefreshCommandState();
                return false;
            }

            if (!loadedLocatorEvidenceReviewDecision.TryApplyOperatorDecision(
                    decision,
                    LocatorEvidenceVisualCorrespondence,
                    LocatorEvidenceReviewer.Trim(),
                    LocatorEvidenceReviewNotes.Trim(),
                    DateTimeOffset.UtcNow,
                    out string applyMessage))
            {
                LocatorEvidenceReviewDecisionStatusText = LocalText(
                    "review decision을 기록할 수 없습니다: ",
                    "The review decision could not be recorded: ")
                    + applyMessage;
                RefreshCommandState();
                return false;
            }

            if (!loadedLocatorEvidenceReviewDecision.TryValidateAgainst(
                    loadedLocatorEvidencePacketPath,
                    loadedLocatorEvidencePacket,
                    plan,
                    out string afterApplyMessage))
            {
                LocatorEvidenceReviewDecisionStatusText = LocalText(
                    "review decision을 기록할 수 없습니다: ",
                    "The review decision could not be recorded: ")
                    + afterApplyMessage;
                RefreshCommandState();
                return false;
            }

            string savePath = ResolveLocatorEvidenceReviewDecisionSavePath(loadedLocatorEvidenceReviewDecisionPath);
            if (!loadedLocatorEvidenceReviewDecision.TrySave(savePath, out string saveMessage))
            {
                LocatorEvidenceReviewDecisionStatusText = LocalText(
                    "review decision 저장 NG. Recipe 승격은 계속 차단됩니다: ",
                    "Review decision save failed. Recipe promotion remains blocked: ") + saveMessage;
                RefreshCommandState();
                return false;
            }

            if (!OpenVisionRecipeLocatorRelativeBlobReviewDecision.TryLoad(
                    savePath,
                    out OpenVisionRecipeLocatorRelativeBlobReviewDecision persistedDecision,
                    out string reloadMessage))
            {
                LocatorEvidenceReviewDecisionStatusText = LocalText(
                    "저장된 review decision 재검증 NG. Recipe 승격은 차단됩니다: ",
                    "The saved review decision failed reload validation. Recipe promotion remains blocked: ")
                    + reloadMessage;
                RefreshCommandState();
                return false;
            }

            if (!persistedDecision.TryValidateAgainst(
                    loadedLocatorEvidencePacketPath,
                    loadedLocatorEvidencePacket,
                    plan,
                    out string persistedValidationMessage))
            {
                LocatorEvidenceReviewDecisionStatusText = LocalText(
                    "저장된 review decision 재검증 NG. Recipe 승격은 차단됩니다: ",
                    "The saved review decision failed reload validation. Recipe promotion remains blocked: ")
                    + persistedValidationMessage;
                RefreshCommandState();
                return false;
            }

            loadedLocatorEvidenceReviewDecision = persistedDecision;
            loadedLocatorEvidenceReviewDecisionPath = savePath;
            LocatorEvidenceReviewDecisionPath = savePath;
            LocatorEvidenceVisualCorrespondence = persistedDecision.VisualCorrespondence;
            LocatorEvidenceReviewer = persistedDecision.Reviewer;
            LocatorEvidenceReviewNotes = persistedDecision.Notes;
            LocatorEvidenceReviewDecisionStatusText = BuildLocatorEvidenceReviewDecisionStatus(persistedDecision)
                + LocalText(" 저장됨. Import는 현재 APPROVED일 때만 활성화됩니다.", " Saved. Import is enabled only for a current APPROVED decision.");
            OnPropertyChanged(nameof(IsLocatorEvidenceReviewDecisionLoaded));
            RefreshCommandState();
            return true;
        }

        private bool CanRecordLocatorEvidenceReviewDecision()
        {
            return CanUseSelectedRecipe()
                && loadedLocatorEvidencePacket != null
                && loadedLocatorEvidenceReviewDecision != null;
        }

        private bool TryValidateLocatorEvidenceReviewDecisionForPromotion(out string message)
        {
            message = string.Empty;
            if (!OpenVisionRecipeLlmIntent.IsLocatorRelativeBlobTemplate(SelectedLlmToolTemplate))
            {
                return true;
            }

            if (loadedLocatorEvidencePacket == null
                || string.IsNullOrWhiteSpace(loadedLocatorEvidencePacketPath))
            {
                message = LocalText(
                    "locator Recipe 승격에는 검증된 Evidence Packet이 필요합니다.",
                    "Locator Recipe promotion requires a verified Evidence Packet.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(loadedLocatorEvidenceReviewDecisionPath))
            {
                message = LocalText(
                    "locator Recipe 승격에는 현재 APPROVED review decision이 필요합니다.",
                    "Locator Recipe promotion requires a current APPROVED review decision.");
                return false;
            }

            if (!OpenVisionRecipeLocatorRelativeBlobReviewDecision.TryLoad(
                    loadedLocatorEvidenceReviewDecisionPath,
                    out OpenVisionRecipeLocatorRelativeBlobReviewDecision persistedDecision,
                    out string loadMessage))
            {
                message = loadMessage;
                return false;
            }

            if (!TryCreateCurrentLocatorRelativeBlobPlan(
                    out OpenVisionRecipeLocatorRelativeBlobIntentSkill.Plan plan,
                    out string planMessage))
            {
                message = planMessage;
                return false;
            }

            if (!persistedDecision.TryValidateAgainst(
                    loadedLocatorEvidencePacketPath,
                    loadedLocatorEvidencePacket,
                    plan,
                    out string validationMessage))
            {
                message = validationMessage;
                return false;
            }

            if (!string.Equals(
                    persistedDecision.Decision,
                    OpenVisionRecipeLocatorRelativeBlobReviewDecision.Approved,
                    StringComparison.Ordinal))
            {
                message = LocalText(
                    "현재 review decision이 APPROVED가 아니므로 Recipe를 승격하지 않습니다.",
                    "The current review decision is not APPROVED, so the Recipe will not be promoted.");
                return false;
            }

            return true;
        }

        private void ClearLoadedLocatorEvidenceReviewDecision()
        {
            loadedLocatorEvidenceReviewDecision = null;
            loadedLocatorEvidenceReviewDecisionPath = string.Empty;
            LocatorEvidenceReviewDecisionPath = string.Empty;
            LocatorEvidenceVisualCorrespondence = OpenVisionRecipeLocatorRelativeBlobReviewDecision.NotReviewed;
            LocatorEvidenceReviewer = string.Empty;
            LocatorEvidenceReviewNotes = string.Empty;
            LocatorEvidenceReviewDecisionStatusText = LocalText(
                "대기 중: Evidence Packet을 로드한 뒤 review decision 파일을 선택하세요.",
                "Waiting: load an Evidence Packet, then select its review decision file.");
            OnPropertyChanged(nameof(IsLocatorEvidenceReviewDecisionLoaded));
        }

        private string BuildLocatorEvidenceReviewDecisionStatus(
            OpenVisionRecipeLocatorRelativeBlobReviewDecision decision)
        {
            return LocalText("Review decision: ", "Review decision: ")
                + decision.Decision
                + " / visual="
                + decision.VisualCorrespondence
                + " / CandidateId="
                + decision.ReviewedCandidateId;
        }

        private static string ResolveLocatorEvidenceReviewDecisionSavePath(string path)
        {
            const string templateSuffix = ".template.json";
            if (!string.IsNullOrWhiteSpace(path)
                && path.EndsWith(templateSuffix, StringComparison.OrdinalIgnoreCase))
            {
                return path.Substring(0, path.Length - templateSuffix.Length) + ".json";
            }

            return path;
        }

        private void CompileLocatorEvidencePacket()
        {
            CompileLocatorEvidencePacketFromCurrentSettings();
        }

        private bool TryCreateCurrentLocatorRelativeBlobPlan(
            out OpenVisionRecipeLocatorRelativeBlobIntentSkill.Plan plan,
            out string message)
        {
            return OpenVisionRecipeLocatorRelativeBlobIntentSkill.TryCreatePlan(
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
                BlobCountIntentThresholdText,
                BlobCountIntentMinAreaText,
                BlobCountIntentMaxAreaText,
                ResolveLocatorRelativeBlobExpectedCountText(),
                out plan,
                out message);
        }

        public bool CompileLocatorEvidencePacketFromCurrentSettings()
        {
            locatorEvidenceCompilationReady = false;
            if (loadedLocatorEvidencePacket == null)
            {
                LocatorEvidencePacketStatusText = LocalText(
                    "먼저 검증된 Evidence Packet을 로드하세요.",
                    "Load a verified Evidence Packet first.");
                OnPropertyChanged(nameof(IsLocatorEvidenceCompilationReady));
                return false;
            }

            if (!OpenVisionRecipeLlmIntent.IsLocatorRelativeBlobTemplate(SelectedLlmToolTemplate))
            {
                LocatorEvidencePacketStatusText = LocalText(
                    "Locator-relative Blob 의도를 선택한 뒤 Compile하세요.",
                    "Select the Locator-relative Blob intent before compiling.");
                OnPropertyChanged(nameof(IsLocatorEvidenceCompilationReady));
                return false;
            }

            if (!TryCreateCurrentLocatorRelativeBlobPlan(
                    out OpenVisionRecipeLocatorRelativeBlobIntentSkill.Plan plan,
                    out string planMessage))
            {
                LocatorEvidencePacketStatusText = LocalText(
                    "현재 Guided Setup 설정이 Compile 불가합니다: ",
                    "The current Guided Setup settings cannot be compiled: ") + planMessage;
                LocatorEvidenceReviewText = BuildLocatorEvidenceReviewText(loadedLocatorEvidencePacket)
                    + Environment.NewLine
                    + Environment.NewLine
                    + LocalText("현재 설정 대조: NG - ", "Current settings comparison: NG - ")
                    + planMessage;
                OnPropertyChanged(nameof(IsLocatorEvidenceCompilationReady));
                RefreshCommandState();
                return false;
            }

            if (!OpenVisionRecipeLocatorRelativeBlobIntentSkill.TryCompile(
                    loadedLocatorEvidencePacket,
                    plan,
                    out VisionPipeline pipeline,
                    out string compileMessage))
            {
                LocatorEvidencePacketStatusText = LocalText(
                    "Evidence Packet Compile NG: ",
                    "Evidence Packet compile failed: ") + compileMessage;
                LocatorEvidenceReviewText = BuildLocatorEvidenceReviewText(loadedLocatorEvidencePacket)
                    + Environment.NewLine
                    + Environment.NewLine
                    + LocalText("Evidence compile: NG - ", "Evidence compile: NG - ")
                    + compileMessage;
                OnPropertyChanged(nameof(IsLocatorEvidenceCompilationReady));
                RefreshCommandState();
                return false;
            }

            LlmPromptText = BuildLlmPromptText();
            LlmXmlDraftText = SerializePipelineToXmlText(pipeline);
            bool xmlReady = ValidateLlmXmlDraftText(false);
            locatorEvidenceCompilationReady = xmlReady;
            LocatorEvidencePacketStatusText = xmlReady
                ? LocalText(
                    "Evidence Packet Compile OK. XML 초안만 준비했으며 Import와 명시적 Run은 별도입니다.",
                    "Evidence Packet compile OK. Only the XML draft was prepared; Import and explicit Run remain separate.")
                : LocalText(
                    "Evidence Packet은 OK지만 XML 검증이 NG입니다. Import/Preview/Run은 실행하지 않았습니다.",
                    "The Evidence Packet is valid, but XML validation failed. Import/Preview/Run were not executed.");
            LocatorEvidenceReviewText = BuildLocatorEvidenceReviewText(loadedLocatorEvidencePacket)
                + Environment.NewLine
                + Environment.NewLine
                + LocalText(
                    "현재 설정 대조: OK - CandidateId만 사용했으며 LLM 좌표는 적용하지 않았습니다.",
                    "Current settings comparison: OK - only CandidateId was used; no LLM-supplied coordinates were applied.")
                + Environment.NewLine
                + LocalText(
                    "다음 단계: XML 검증/가져오기 후 기존 명시적 Run 명령을 별도로 실행하세요.",
                    "Next: validate/import the XML, then execute the existing explicit Run command separately.");
            OnPropertyChanged(nameof(IsLocatorEvidenceCompilationReady));
            RefreshCommandState();
            return xmlReady;
        }

        private bool CanCompileLocatorEvidencePacket()
        {
            return CanUseSelectedRecipe()
                && loadedLocatorEvidencePacket != null
                && OpenVisionRecipeLlmIntent.IsLocatorRelativeBlobTemplate(SelectedLlmToolTemplate);
        }

        private void InvalidateLocatorEvidenceCompilation()
        {
            if (!locatorEvidenceCompilationReady)
            {
                return;
            }

            locatorEvidenceCompilationReady = false;
            if (loadedLocatorEvidencePacket != null)
            {
                LocatorEvidencePacketStatusText = LocalText(
                    "Guided Setup 또는 XML이 변경되었습니다. Evidence Packet을 다시 Compile하세요.",
                    "Guided Setup or XML changed. Compile the Evidence Packet again.");
                LocatorEvidenceReviewText = BuildLocatorEvidenceReviewText(loadedLocatorEvidencePacket)
                    + Environment.NewLine
                    + Environment.NewLine
                    + LocalText(
                        "상태: STALE - 현재 설정과의 Compile을 다시 수행해야 합니다.",
                        "State: STALE - compile it again against the current settings.");
            }

            OnPropertyChanged(nameof(IsLocatorEvidenceCompilationReady));
        }

        public string LocatorEvidencePacketPath
        {
            get => locatorEvidencePacketPath;
            set
            {
                string next = value ?? string.Empty;
                if (SetProperty(ref locatorEvidencePacketPath, next))
                {
                    if ((loadedLocatorEvidencePacket != null
                            || loadedLocatorEvidenceReviewDecision != null)
                        && !AreSamePath(next, loadedLocatorEvidencePacketPath))
                    {
                        loadedLocatorEvidencePacket = null;
                        loadedLocatorEvidencePacketPath = string.Empty;
                        locatorEvidenceCompilationReady = false;
                        LocatorEvidenceOverlayImage = null;
                        ClearLoadedLocatorEvidenceReviewDecision();
                        LocatorEvidencePacketStatusText = LocalText(
                            "Packet 경로가 변경되었습니다. 새 Packet을 로드하세요.",
                            "The packet path changed. Load the new packet.");
                        LocatorEvidenceReviewText = LocalText(
                            "대기 중: 새 Evidence Packet을 로드하세요.",
                            "Waiting: load the new Evidence Packet.");
                        OnPropertyChanged(nameof(IsLocatorEvidencePacketLoaded));
                        OnPropertyChanged(nameof(IsLocatorEvidenceCompilationReady));
                    }

                    RefreshCommandState();
                }
            }
        }

        public string LocatorEvidencePacketStatusText
        {
            get => locatorEvidencePacketStatusText;
            private set => SetProperty(ref locatorEvidencePacketStatusText, value ?? string.Empty);
        }

        public string LocatorEvidenceReviewDecisionPath
        {
            get => locatorEvidenceReviewDecisionPath;
            set
            {
                string next = value ?? string.Empty;
                if (!SetProperty(ref locatorEvidenceReviewDecisionPath, next))
                {
                    return;
                }

                if (loadedLocatorEvidenceReviewDecision != null
                    && !AreSamePath(next, loadedLocatorEvidenceReviewDecisionPath))
                {
                    loadedLocatorEvidenceReviewDecision = null;
                    loadedLocatorEvidenceReviewDecisionPath = string.Empty;
                    LocatorEvidenceVisualCorrespondence = OpenVisionRecipeLocatorRelativeBlobReviewDecision.NotReviewed;
                    LocatorEvidenceReviewer = string.Empty;
                    LocatorEvidenceReviewNotes = string.Empty;
                    LocatorEvidenceReviewDecisionStatusText = LocalText(
                        "review decision 경로가 변경되었습니다. 새 결정을 로드하세요.",
                        "The review decision path changed. Load the new decision.");
                    OnPropertyChanged(nameof(IsLocatorEvidenceReviewDecisionLoaded));
                }

                RefreshCommandState();
            }
        }

        public string LocatorEvidenceReviewDecisionStatusText
        {
            get => locatorEvidenceReviewDecisionStatusText;
            private set => SetProperty(ref locatorEvidenceReviewDecisionStatusText, value ?? string.Empty);
        }

        public string LocatorEvidenceVisualCorrespondence
        {
            get => locatorEvidenceVisualCorrespondence;
            set
            {
                if (SetProperty(ref locatorEvidenceVisualCorrespondence, value ?? string.Empty))
                {
                    RefreshCommandState();
                }
            }
        }

        public string LocatorEvidenceReviewer
        {
            get => locatorEvidenceReviewer;
            set
            {
                if (SetProperty(ref locatorEvidenceReviewer, value ?? string.Empty))
                {
                    RefreshCommandState();
                }
            }
        }

        public string LocatorEvidenceReviewNotes
        {
            get => locatorEvidenceReviewNotes;
            set
            {
                if (SetProperty(ref locatorEvidenceReviewNotes, value ?? string.Empty))
                {
                    RefreshCommandState();
                }
            }
        }

        public string LocatorEvidenceReviewText
        {
            get => locatorEvidenceReviewText;
            private set => SetProperty(ref locatorEvidenceReviewText, value ?? string.Empty);
        }

        public BitmapSource LocatorEvidenceOverlayImage
        {
            get => locatorEvidenceOverlayImage;
            private set => SetProperty(ref locatorEvidenceOverlayImage, value);
        }

        public bool IsLocatorEvidencePacketLoaded => loadedLocatorEvidencePacket != null;

        public bool IsLocatorEvidenceReviewDecisionLoaded => loadedLocatorEvidenceReviewDecision != null;

        public bool IsLocatorEvidenceCompilationReady => locatorEvidenceCompilationReady;

        public string LocatorEvidencePacketLabelText => LocalText("Evidence Packet", "Evidence Packet");

        public string LocatorEvidenceLoadText => LocalText("Packet 로드", "Load packet");

        public string LocatorEvidenceCompileText => LocalText("검증·Compile", "Validate / compile");

        public string LocatorEvidenceReviewDecisionLabelText => LocalText("검토 결정", "Review decision");

        public string LocatorEvidenceReviewDecisionLoadText => LocalText("결정 로드", "Load decision");

        public string LocatorEvidenceVisualCorrespondenceLabelText => LocalText("시각 대응", "Visual correspondence");

        public string LocatorEvidenceReviewerLabelText => LocalText("검토자", "Reviewer");

        public string LocatorEvidenceReviewNotesLabelText => LocalText("검토 Notes", "Review notes");

        public string LocatorEvidenceApproveText => LocalText("후보 승인", "Approve candidate");

        public string LocatorEvidenceRejectText => LocalText("후보 거부", "Reject candidate");

        public string LocatorEvidenceReplacementText => LocalText("교체 요청", "Request replacement");

        public string LocatorEvidenceReviewLabelText => LocalText("Candidate / 무결성 검토", "Candidate / integrity review");

        public string LocatorEvidenceOverlayLabelText => LocalText("현재 실행 overlay", "Current-run overlay");

        public string LocatorEvidenceBoundaryText => LocalText(
            "Load는 Packet 검토만 수행합니다. Compile은 현재 설정과 대조해 XML 초안만 준비합니다. 현재 Packet·Plan과 일치하는 APPROVED 검토 결정 전에는 Recipe Import가 차단되며, 승인도 qualification·Preview·Run·레이어·라우팅을 실행하지 않습니다.",
            "Load only reviews the packet. Compile compares the current settings and prepares XML only. Recipe Import remains blocked until a current Packet/Plan-matched APPROVED decision; approval does not qualify, Preview, Run, or mutate layers or routing.");

        private string BuildLocatorEvidenceReviewText(OpenVisionRecipeLocatorRelativeBlobEvidencePacket packet)
        {
            StringBuilder review = new StringBuilder();
            review.AppendLine("Schema: " + packet.SchemaVersion);
            review.AppendLine("Skill: " + packet.SkillId + " / " + packet.SkillVersion);
            review.AppendLine("Source: " + packet.SourceImagePath);
            review.AppendLine("Source SHA-256: " + packet.SourceImageSha256);
            review.AppendLine("Template: " + packet.LocatorTemplatePath);
            review.AppendLine("Template SHA-256: " + packet.LocatorTemplateSha256);
            review.AppendLine("Overlay: " + packet.PreviewOverlayPath);
            review.AppendLine("Overlay SHA-256: " + packet.PreviewOverlaySha256);
            review.AppendLine("Frame: " + packet.CoordinateFrame + " / "
                + packet.SourceImageWidth.ToString(CultureInfo.InvariantCulture) + "x"
                + packet.SourceImageHeight.ToString(CultureInfo.InvariantCulture));
            review.AppendLine("Selected CandidateId: " + packet.SelectedCandidateId);
            foreach (OpenVisionRecipeLocatorRelativeBlobEvidenceCandidate candidate in packet.Candidates)
            {
                review.AppendLine(
                    candidate.CandidateId
                    + " / native=" + candidate.NativeIndex.ToString(CultureInfo.InvariantCulture)
                    + " / accepted=" + candidate.Accepted.ToString(CultureInfo.InvariantCulture)
                    + " / center=(" + candidate.CenterX.ToString("0.###", CultureInfo.InvariantCulture)
                    + "," + candidate.CenterY.ToString("0.###", CultureInfo.InvariantCulture) + ")"
                    + " / score=" + candidate.Score.ToString("0.###", CultureInfo.InvariantCulture)
                    + " / margin=" + candidate.ScoreMargin.ToString("0.###", CultureInfo.InvariantCulture));
            }

            return review.ToString().TrimEnd();
        }

        private static bool AreSamePath(string left, string right)
        {
            try
            {
                return string.Equals(
                    Path.GetFullPath(left ?? string.Empty).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar),
                    Path.GetFullPath(right ?? string.Empty).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar),
                    StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return false;
            }
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
                OpenVisionRecipeReviewBundleDryRunProjection failureProjection = reviewBundleDryRunProjectionOwner.Project(
                    inspectionSucceeded: false,
                    xmlReady: false);
                StatusText = failureProjection.StatusText;
                openLlmXmlReview();
                return failureProjection.Succeeded;
            }

            LlmXmlDraftText = inspection.PipelineXml;
            loadedReviewBundleInspection = inspection;
            OnPropertyChanged(nameof(LlmDraftValidationText));
            OnPropertyChanged(nameof(LlmDependencyReportText));
            bool xmlReady = ValidateLlmXmlDraftText(false);
            OpenVisionRecipeReviewBundleDryRunProjection reviewProjection = reviewBundleDryRunProjectionOwner.Project(
                inspectionSucceeded: true,
                xmlReady: xmlReady);
            StatusText = reviewProjection.StatusText;
            openLlmXmlReview();
            return reviewProjection.Succeeded;
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
            if (!TryValidateLocatorEvidenceReviewDecisionForPromotion(out string reviewDecisionMessage))
            {
                llmXmlDraftImportReady = false;
                LocatorEvidenceReviewDecisionStatusText = LocalText(
                    "Recipe 승격 차단: ",
                    "Recipe promotion blocked: ") + reviewDecisionMessage;
                StatusText = LocalText(
                    "현재 APPROVED review decision 없이는 locator Recipe를 가져올 수 없습니다.",
                    "A locator Recipe cannot be imported without a current APPROVED review decision.");
                RefreshCommandState();
                return;
            }

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
            if (!TryLeaveSelectedStepEdit(
                OpenVisionRecipePendingEditTransitionKind.Pipeline,
                pipeline.Name))
            {
                return;
            }

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
            OpenVisionRecipeLlmDraftReview review = llmDraftReviewOwner.Build(
                NormalizeRecipeName(selectedRecipeName),
                pipeline);
            LlmXmlDraftReviewReport = review.ImportReviewText;
            LlmXmlDraftDiffReport = review.DiffReviewText;
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
            OpenVisionRecipeLlmDraftReview review = ok
                ? llmDraftReviewOwner.Build(NormalizeRecipeName(selectedRecipeName), pipeline)
                : null;
            LlmXmlDraftReviewReport = review?.ImportReviewText
                ?? LocalText("초안 검토 건너뜀: 검증 실패.", "Draft review skipped: validation failed.");
            LlmXmlDraftDiffReport = review?.DiffReviewText
                ?? LocalText("변경점 검토 건너뜀: 검증 실패.", "Diff review skipped: validation failed.");
            llmXmlDraftImportReady = ok;
            StatusText = ok ? LocalText("LLM XML 초안 검증 OK.", "LLM XML draft validation OK.") : LocalText("LLM XML 초안 검증 NG.", "LLM XML draft validation NG.");
            RefreshCommandState();
            return ok;
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
                            : null,
                        OpenVisionRecipeLlmIntent.IsLocatorRelativeBlobTemplate(SelectedLlmToolTemplate)
                            ? CreateLocatorRelativeBlobIntentValidationContext()
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
                    (int width, int height) = OpenVisionBitmapImagePreviewFactory.ReadPixelSize(sourceImagePath);
                    sourceWidth = width;
                    sourceHeight = height;
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

        private OpenVisionRecipeLocatorRelativeBlobIntentValidationContext CreateLocatorRelativeBlobIntentValidationContext()
        {
            return new OpenVisionRecipeLocatorRelativeBlobIntentValidationContext(
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
                BlobCountIntentThresholdText,
                BlobCountIntentMinAreaText,
                BlobCountIntentMaxAreaText,
                ResolveLocatorRelativeBlobExpectedCountText());
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

        private bool CanUseLlmXmlDraft()
        {
            return CanUseSelectedRecipe()
                && !string.IsNullOrWhiteSpace(LlmXmlDraftText);
        }

        private bool CanImportLlmXmlDraft()
        {
            if (!CanUseLlmXmlDraft() || !llmXmlDraftImportReady)
            {
                return false;
            }

            return !OpenVisionRecipeLlmIntent.IsLocatorRelativeBlobTemplate(SelectedLlmToolTemplate)
                || (loadedLocatorEvidencePacket != null
                    && loadedLocatorEvidenceReviewDecision != null
                    && string.Equals(
                        loadedLocatorEvidenceReviewDecision.Decision,
                        OpenVisionRecipeLocatorRelativeBlobReviewDecision.Approved,
                        StringComparison.Ordinal)
                    && !string.IsNullOrWhiteSpace(loadedLocatorEvidenceReviewDecisionPath));
        }


        #endregion


        #region Pipeline XML and review bundle exchange

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

            if (!TryLeaveSelectedStepEdit(
                OpenVisionRecipePendingEditTransitionKind.Pipeline,
                Path.GetFileNameWithoutExtension(path)))
            {
                return false;
            }

            OpenVisionRecipePipelineExchangeResult result = pipelineExchangeUseCase.Import(recipeName, path);
            OpenVisionRecipePipelineExchangeProjection projection = pipelineExchangeProjectionOwner.Project(
                OpenVisionRecipePipelineExchangeOperation.Import,
                result);
            if (!projection.Succeeded)
            {
                StatusText = projection.StatusText;
                UpdateSelectedRecipeSummary();
                return false;
            }

            RefreshPipelineOptions(projection.PipelineName);
            StatusText = projection.StatusText;
            RefreshOptions();
            refreshAfterSwitch();
            return true;
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
            OpenVisionRecipePipelineExchangeResult result = pipelineExchangeUseCase.Export(
                recipeName,
                activePipelineName,
                path);
            OpenVisionRecipePipelineExchangeProjection projection = pipelineExchangeProjectionOwner.Project(
                OpenVisionRecipePipelineExchangeOperation.Export,
                result);
            if (!projection.Succeeded)
            {
                StatusText = projection.StatusText;
                UpdateSelectedRecipeSummary();
                return false;
            }

            StatusText = projection.StatusText;
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
            OpenVisionRecipePipelineExchangeResult result = pipelineExchangeUseCase.ExportReviewBundle(
                recipeName,
                activePipelineName,
                path,
                BuildRecipeReviewReferences());
            OpenVisionRecipePipelineExchangeProjection projection = pipelineExchangeProjectionOwner.Project(
                OpenVisionRecipePipelineExchangeOperation.ExportReviewBundle,
                result);
            if (!projection.Succeeded)
            {
                StatusText = projection.StatusText;
                return false;
            }

            StatusText = projection.StatusText;
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


        #endregion


        #region Pipeline lifecycle

        private void ActivateSelectedPipeline()
        {
            OpenVisionRecipePipelineOption option = SelectedPipelineOption;
            if (!CanUseSelectedPipeline())
            {
                StatusText = LocalText("선택된 파이프라인이 없습니다.", "No pipeline selected.");
                return;
            }

            if (!TryLeaveSelectedStepEdit(
                OpenVisionRecipePendingEditTransitionKind.Pipeline,
                option.PipelineName))
            {
                return;
            }

            string recipeName = NormalizeRecipeName(selectedRecipeName);
            OpenVisionRecipePipelineLifecycleResult result = pipelineLifecycleUseCase.Activate(recipeName, option.PipelineName);
            OpenVisionRecipePipelineLifecycleProjection projection = pipelineLifecycleProjectionOwner.Project(
                OpenVisionRecipePipelineLifecycleOperation.Activate,
                result);
            StatusText = projection.StatusText;
            RefreshPipelineOptions(projection.PipelineName);
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
            if (!TryLeaveSelectedStepEdit(
                OpenVisionRecipePendingEditTransitionKind.Pipeline,
                requestedName))
            {
                return;
            }

            OpenVisionRecipePipelineLifecycleResult result = pipelineLifecycleUseCase.Duplicate(
                recipeName,
                option.PipelineName,
                requestedName);
            OpenVisionRecipePipelineLifecycleProjection projection = pipelineLifecycleProjectionOwner.Project(
                OpenVisionRecipePipelineLifecycleOperation.Duplicate,
                result);
            if (!projection.Succeeded)
            {
                StatusText = projection.StatusText;
                RefreshPipelineOptions(option.PipelineName);
                return;
            }

            StatusText = projection.StatusText;
            RefreshPipelineOptions(projection.PipelineName);
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
            if (!TryLeaveSelectedStepEdit(
                OpenVisionRecipePendingEditTransitionKind.Pipeline,
                targetName))
            {
                return;
            }

            bool wasActive = option.IsActive;
            OpenVisionRecipePipelineLifecycleResult result = pipelineLifecycleUseCase.Rename(
                recipeName,
                option.PipelineName,
                targetName);
            OpenVisionRecipePipelineLifecycleProjection projection = pipelineLifecycleProjectionOwner.Project(
                OpenVisionRecipePipelineLifecycleOperation.Rename,
                result);
            if (!projection.Succeeded)
            {
                StatusText = projection.StatusText;
                RefreshPipelineOptions(option.PipelineName);
                return;
            }

            StatusText = projection.StatusText;
            RefreshPipelineOptions(projection.PipelineName);
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

            if (!TryLeaveSelectedStepEdit(
                OpenVisionRecipePendingEditTransitionKind.Pipeline,
                LocalText("Pipeline 삭제: ", "Delete Pipeline: ") + option.PipelineName))
            {
                return;
            }

            bool wasActive = option.IsActive;
            OpenVisionRecipePipelineLifecycleResult result = pipelineLifecycleUseCase.Delete(recipeName, option.PipelineName);
            OpenVisionRecipePipelineLifecycleProjection projection = pipelineLifecycleProjectionOwner.Project(
                OpenVisionRecipePipelineLifecycleOperation.Delete,
                result);
            if (!projection.Succeeded)
            {
                StatusText = projection.StatusText;
                RefreshPipelineOptions(option.PipelineName);
                return;
            }

            StatusText = projection.StatusText;
            RefreshPipelineOptions(projection.PipelineName);
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

            if (!TryLeaveSelectedStepEdit(
                OpenVisionRecipePendingEditTransitionKind.Pipeline,
                sampleOption.SampleName))
            {
                return false;
            }

            OpenVisionRecipePipelineLifecycleResult result = pipelineLifecycleUseCase.DuplicateFromSample(
                recipeName,
                sampleOption.PipelinePath,
                sampleOption.SampleName);
            OpenVisionRecipePipelineLifecycleProjection projection = pipelineLifecycleProjectionOwner.Project(
                OpenVisionRecipePipelineLifecycleOperation.DuplicateFromSample,
                result);
            if (!projection.Succeeded)
            {
                StatusText = projection.StatusText;
                UpdateSelectedRecipeSummary();
                return false;
            }

            RefreshPipelineOptions(projection.PipelineName);
            StatusText = projection.StatusText;
            RefreshOptions();
            refreshAfterSwitch();
            return true;
        }


        #endregion


        #region Qualified snapshots

        private readonly IReadOnlyList<OpenVisionRecipeQualificationScopeOption>
            qualificationScopeOptions =
                OpenVisionRecipeQualificationScopeOption.CreateDefaults();
        private OpenVisionRecipeQualificationScopeOption
            selectedQualificationScopeOption;
        private IReadOnlyList<OpenVisionRecipeQualifiedSnapshotOption>
            qualifiedSnapshotOptions =
                Array.Empty<OpenVisionRecipeQualifiedSnapshotOption>();
        private OpenVisionRecipeQualifiedSnapshotOption
            selectedQualifiedSnapshotOption;
        private string qualificationNote = string.Empty;
        private string qualifiedSnapshotLifecycleReason = string.Empty;
        private string qualifiedSnapshotWorkingCopyName = "Qualified_Working_Copy";
        private string qualifiedSnapshotStatusText = string.Empty;

        public IReadOnlyList<OpenVisionRecipeQualificationScopeOption>
            QualificationScopeOptions => qualificationScopeOptions;

        public OpenVisionRecipeQualificationScopeOption
            SelectedQualificationScopeOption
        {
            get => selectedQualificationScopeOption;
            set
            {
                if (SetProperty(
                        ref selectedQualificationScopeOption,
                        value ?? qualificationScopeOptions.FirstOrDefault()))
                {
                    OnPropertyChanged(nameof(QualificationScopeClaimText));
                    NotifyQualifiedSnapshotContextChanged();
                }
            }
        }

        public IReadOnlyList<OpenVisionRecipeQualifiedSnapshotOption>
            QualifiedSnapshotOptions
        {
            get => qualifiedSnapshotOptions;
            private set => SetProperty(
                ref qualifiedSnapshotOptions,
                value ?? Array.Empty<OpenVisionRecipeQualifiedSnapshotOption>());
        }

        public OpenVisionRecipeQualifiedSnapshotOption
            SelectedQualifiedSnapshotOption
        {
            get => selectedQualifiedSnapshotOption;
            set
            {
                if (SetProperty(ref selectedQualifiedSnapshotOption, value))
                {
                    OnPropertyChanged(
                        nameof(SelectedQualifiedSnapshotDetailText));
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public string QualificationNote
        {
            get => qualificationNote;
            set
            {
                if (SetProperty(ref qualificationNote, value ?? string.Empty))
                {
                    NotifyQualifiedSnapshotContextChanged();
                }
            }
        }

        public string QualifiedSnapshotLifecycleReason
        {
            get => qualifiedSnapshotLifecycleReason;
            set
            {
                if (SetProperty(
                        ref qualifiedSnapshotLifecycleReason,
                        value ?? string.Empty))
                {
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public string QualifiedSnapshotWorkingCopyName
        {
            get => qualifiedSnapshotWorkingCopyName;
            set
            {
                if (SetProperty(
                        ref qualifiedSnapshotWorkingCopyName,
                        value ?? string.Empty))
                {
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public string QualifiedSnapshotStatusText
        {
            get => string.IsNullOrWhiteSpace(qualifiedSnapshotStatusText)
                ? LocalText(
                    "선택 실행의 불변 증거를 확인한 뒤 명시적으로 Snapshot을 생성하세요.",
                    "Review the selected run evidence, then explicitly create a Snapshot.")
                : qualifiedSnapshotStatusText;
            private set => SetProperty(
                ref qualifiedSnapshotStatusText,
                value ?? string.Empty);
        }

        public string QualificationScopeClaimText =>
            SelectedQualificationScopeOption?.ClaimText
            ?? LocalText(
                "자격 범위를 선택하세요.",
                "Select a qualification scope.");

        public string QualifiedSnapshotPreflightText
        {
            get
            {
                if (IsSelectedStepEditDirty)
                {
                    return LocalText(
                        "차단: 저장하지 않은 Step 편집을 먼저 적용하거나 취소하세요.",
                        "Blocked: apply or discard the pending Step edit first.");
                }

                VisionPipelineBatchRunSummary summary =
                    SelectedRecentBatchRunOption?.RunSummary;
                if (summary == null
                    || string.IsNullOrWhiteSpace(
                        SelectedRecentBatchRunOption?.SummaryPath))
                {
                    return LocalText(
                        "차단: 저장된 Local Validation Set 실행을 선택하세요.",
                        "Blocked: select a saved Local Validation Set run.");
                }

                if (!string.Equals(
                        summary.SuiteKind,
                        "LocalValidationSet",
                        StringComparison.OrdinalIgnoreCase))
                {
                    return LocalText(
                        "차단: 부분/일반 실행은 자격 증거가 아닙니다.",
                        "Blocked: partial or generic runs are not qualification evidence.");
                }

                if (SelectedValidationSetOption?.Set == null
                    || !string.Equals(
                        SelectedValidationSetOption.Set.Name,
                        summary.SuiteName,
                        StringComparison.Ordinal)
                    || SelectedValidationSetOption.Set.IsIdentityLocked
                        && !string.Equals(
                        SelectedValidationSetOption.Set.PipelineName,
                        SelectedPipelineOption?.PipelineName,
                        StringComparison.Ordinal))
                {
                    return LocalText(
                        "차단: 실행과 동일한 Validation Set/Pipeline을 선택하세요.",
                        "Blocked: select the Validation Set/Pipeline used by the run.");
                }

                if (string.IsNullOrWhiteSpace(QualificationNote))
                {
                    return LocalText(
                        "차단: 운영자 자격 메모를 입력하세요.",
                        "Blocked: enter an operator qualification note.");
                }

                string counts = summary.TotalCount.ToString(
                        CultureInfo.InvariantCulture)
                    + " rows | correct "
                    + summary.JudgmentCorrectCount.ToString(
                        CultureInfo.InvariantCulture)
                    + " | FA "
                    + summary.FalseAcceptCount.ToString(
                        CultureInfo.InvariantCulture)
                    + " | FR "
                    + summary.FalseRejectCount.ToString(
                        CultureInfo.InvariantCulture)
                    + " | errors "
                    + summary.ExecutionErrorCount.ToString(
                        CultureInfo.InvariantCulture);
                return LocalText(
                    "준비: 생성 시 모든 해시·보고서·도면·검토 큐를 정확히 재검증합니다. ",
                    "Ready: Create will exactly reverify all hashes, reports, drawings, and the review queue. ")
                    + counts;
            }
        }

        public string SelectedQualifiedSnapshotDetailText =>
            SelectedQualifiedSnapshotOption == null
                ? LocalText(
                    "Snapshot을 선택하면 무결성, runtime, 수명주기 상태가 표시됩니다.",
                    "Select a Snapshot to show integrity, runtime, and lifecycle state.")
                : SelectedQualifiedSnapshotOption.DetailText
                    + " | ID "
                    + SelectedQualifiedSnapshotOption.SnapshotId;

        public string QualifiedRecipeSnapshotText =>
            LocalText("자격 Recipe Snapshot", "Qualified Recipe Snapshot");

        public string QualificationScopeText =>
            LocalText("자격 범위", "Qualification scope");

        public string QualificationNoteText =>
            LocalText("운영자 메모", "Operator note");

        public string CreateQualifiedSnapshotText =>
            LocalText("Snapshot 생성", "Create Snapshot");

        public string VerifyQualifiedSnapshotText =>
            LocalText("무결성 확인", "Verify integrity");

        public string OpenQualifiedSnapshotEvidenceText =>
            LocalText("증거 열기", "Open evidence");

        public string CreateQualifiedSnapshotWorkingCopyText =>
            LocalText("작업 복사본", "Working copy");

        public string SupersedeQualifiedSnapshotText =>
            LocalText("대체 생성", "Supersede");

        public string RevokeQualifiedSnapshotText =>
            LocalText("폐기 기록", "Revoke");

        public string RefreshQualifiedSnapshotsText =>
            LocalText("새로고침", "Refresh");

        public string QualifiedSnapshotLifecycleReasonText =>
            LocalText("대체/폐기 사유", "Supersede/revoke reason");

        public string QualifiedSnapshotWorkingCopyNameText =>
            LocalText("작업 Recipe 이름", "Working Recipe name");

        public ICommand CreateQualifiedSnapshotCommand { get; private set; }
        public ICommand VerifyQualifiedSnapshotCommand { get; private set; }
        public ICommand OpenQualifiedSnapshotEvidenceCommand { get; private set; }
        public ICommand CreateQualifiedSnapshotWorkingCopyCommand { get; private set; }
        public ICommand SupersedeQualifiedSnapshotCommand { get; private set; }
        public ICommand RevokeQualifiedSnapshotCommand { get; private set; }
        public ICommand RefreshQualifiedSnapshotsCommand { get; private set; }

        private void InitializeQualifiedSnapshotCommands()
        {
            selectedQualificationScopeOption =
                qualificationScopeOptions.FirstOrDefault();
            CreateQualifiedSnapshotCommand = new RelayCommand(
                CreateQualifiedSnapshot,
                CanCreateQualifiedSnapshot);
            VerifyQualifiedSnapshotCommand = new RelayCommand(
                VerifyQualifiedSnapshot,
                CanUseSelectedQualifiedSnapshot);
            OpenQualifiedSnapshotEvidenceCommand = new RelayCommand(
                OpenQualifiedSnapshotEvidence,
                CanOpenQualifiedSnapshotEvidence);
            CreateQualifiedSnapshotWorkingCopyCommand = new RelayCommand(
                CreateQualifiedSnapshotWorkingCopy,
                CanCreateQualifiedSnapshotWorkingCopy);
            SupersedeQualifiedSnapshotCommand = new RelayCommand(
                SupersedeQualifiedSnapshot,
                CanSupersedeQualifiedSnapshot);
            RevokeQualifiedSnapshotCommand = new RelayCommand(
                RevokeQualifiedSnapshot,
                CanRevokeQualifiedSnapshot);
            RefreshQualifiedSnapshotsCommand = new RelayCommand(
                () => RefreshQualifiedSnapshotOptions());
        }

        private void CreateQualifiedSnapshot()
        {
            OpenVisionRecipeQualificationEvaluation evaluation =
                EvaluateQualifiedSnapshot();
            if (!evaluation.Success)
            {
                SetQualifiedSnapshotActionStatus(
                    false,
                    string.Join(" | ", evaluation.Errors));
                return;
            }

            OpenVisionRecipeQualifiedSnapshotActionResult result =
                qualifiedSnapshotController.Create(evaluation);
            SetQualifiedSnapshotActionStatus(result.Success, result.Message);
            RefreshQualifiedSnapshotOptions(result.SnapshotId);
        }

        private void SupersedeQualifiedSnapshot()
        {
            string predecessor =
                SelectedQualifiedSnapshotOption?.SnapshotId ?? string.Empty;
            string reason = QualifiedSnapshotLifecycleReason.Trim();
            if (!confirmQualifiedSnapshotLifecycle(
                    predecessor,
                    "Superseded",
                    reason))
            {
                SetQualifiedSnapshotActionStatus(
                    false,
                    LocalText(
                        "대체 작업을 취소했습니다.",
                        "Supersede was cancelled."));
                return;
            }

            OpenVisionRecipeQualificationEvaluation evaluation =
                qualifiedSnapshotController.Evaluate(
                    NormalizeRecipeName(selectedRecipeName),
                    SelectedPipelineOption?.PipelineName ?? string.Empty,
                    SelectedRecentBatchRunOption,
                    SelectedValidationSetOption,
                    SelectedQualificationScopeOption,
                    QualificationNote,
                    IsSelectedStepEditDirty,
                    predecessor,
                    reason);
            OpenVisionRecipeQualifiedSnapshotActionResult result =
                qualifiedSnapshotController.Supersede(
                    evaluation,
                    predecessor,
                    reason);
            SetQualifiedSnapshotActionStatus(result.Success, result.Message);
            RefreshQualifiedSnapshotOptions(
                result.Success ? result.SnapshotId : predecessor);
        }

        private void RevokeQualifiedSnapshot()
        {
            string snapshotId =
                SelectedQualifiedSnapshotOption?.SnapshotId ?? string.Empty;
            string reason = QualifiedSnapshotLifecycleReason.Trim();
            if (!confirmQualifiedSnapshotLifecycle(
                    snapshotId,
                    "Revoked",
                    reason))
            {
                SetQualifiedSnapshotActionStatus(
                    false,
                    LocalText(
                        "폐기 기록을 취소했습니다.",
                        "Revoke was cancelled."));
                return;
            }

            OpenVisionRecipeQualifiedSnapshotActionResult result =
                qualifiedSnapshotController.Revoke(snapshotId, reason);
            SetQualifiedSnapshotActionStatus(result.Success, result.Message);
            RefreshQualifiedSnapshotOptions(snapshotId);
        }

        private void VerifyQualifiedSnapshot()
        {
            string snapshotId =
                SelectedQualifiedSnapshotOption?.SnapshotId ?? string.Empty;
            OpenVisionRecipeQualifiedSnapshotActionResult result =
                qualifiedSnapshotController.Verify(
                    snapshotId,
                    NormalizeRecipeName(selectedRecipeName),
                    SelectedPipelineOption?.PipelineName ?? string.Empty,
                    GetCurrentQualifiedSnapshotPipelinePath(),
                    SelectedValidationSetOption?.Set,
                    null);
            SetQualifiedSnapshotActionStatus(result.Success, result.Message);
            RefreshQualifiedSnapshotOptions(snapshotId);
        }

        private void OpenQualifiedSnapshotEvidence()
        {
            string snapshotId =
                SelectedQualifiedSnapshotOption?.SnapshotId ?? string.Empty;
            if (!qualifiedSnapshotController.TryGetEvidenceDirectory(
                    snapshotId,
                    out string directory,
                    out string error))
            {
                SetQualifiedSnapshotActionStatus(false, error);
                return;
            }

            bool opened = openQualifiedSnapshotEvidence(directory);
            SetQualifiedSnapshotActionStatus(
                opened,
                opened
                    ? LocalText(
                        "Snapshot 증거 폴더를 열었습니다: ",
                        "Opened Snapshot evidence folder: ")
                        + directory
                    : LocalText(
                        "Snapshot 증거 폴더를 열지 못했습니다: ",
                        "Could not open Snapshot evidence folder: ")
                        + directory);
        }

        private void CreateQualifiedSnapshotWorkingCopy()
        {
            string snapshotId =
                SelectedQualifiedSnapshotOption?.SnapshotId ?? string.Empty;
            OpenVisionRecipeQualifiedSnapshotActionResult result =
                qualifiedSnapshotController.CreateWorkingCopy(
                    snapshotId,
                    QualifiedSnapshotWorkingCopyName);
            SetQualifiedSnapshotActionStatus(result.Success, result.Message);
            if (result.Success)
            {
                RefreshOptions();
            }
        }

        private OpenVisionRecipeQualificationEvaluation
            EvaluateQualifiedSnapshot()
        {
            return qualifiedSnapshotController.Evaluate(
                NormalizeRecipeName(selectedRecipeName),
                SelectedPipelineOption?.PipelineName ?? string.Empty,
                SelectedRecentBatchRunOption,
                SelectedValidationSetOption,
                SelectedQualificationScopeOption,
                QualificationNote,
                IsSelectedStepEditDirty);
        }

        private bool CanCreateQualifiedSnapshot()
        {
            VisionPipelineBatchRunSummary summary =
                SelectedRecentBatchRunOption?.RunSummary;
            return !IsSelectedStepEditDirty
                && !executionSession.IsValidationSuiteRunning
                && summary != null
                && string.Equals(
                    summary.SuiteKind,
                    "LocalValidationSet",
                    StringComparison.OrdinalIgnoreCase)
                && SelectedValidationSetOption?.Set != null
                && string.Equals(
                    SelectedValidationSetOption.Set.Name,
                    summary.SuiteName,
                    StringComparison.Ordinal)
                && (!SelectedValidationSetOption.Set.IsIdentityLocked
                    || string.Equals(
                        SelectedValidationSetOption.Set.PipelineName,
                        SelectedPipelineOption?.PipelineName,
                        StringComparison.Ordinal))
                && !string.IsNullOrWhiteSpace(QualificationNote)
                && File.Exists(SelectedRecentBatchRunOption.SummaryPath);
        }

        private bool CanUseSelectedQualifiedSnapshot()
        {
            return SelectedQualifiedSnapshotOption != null
                && !string.IsNullOrWhiteSpace(
                    SelectedQualifiedSnapshotOption.SnapshotId);
        }

        private bool CanOpenQualifiedSnapshotEvidence()
        {
            return CanUseSelectedQualifiedSnapshot()
                && SelectedQualifiedSnapshotOption.PayloadIntegrityValid;
        }

        private bool CanCreateQualifiedSnapshotWorkingCopy()
        {
            return CanOpenQualifiedSnapshotEvidence()
                && RecipeWorkspaceService.IsValidRecipeName(
                    QualifiedSnapshotWorkingCopyName)
                && !RecipeWorkspaceService.GetRecipeNames().Contains(
                    QualifiedSnapshotWorkingCopyName.Trim(),
                    StringComparer.OrdinalIgnoreCase);
        }

        private bool CanSupersedeQualifiedSnapshot()
        {
            return CanUseSelectedQualifiedSnapshot()
                && string.Equals(
                    SelectedQualifiedSnapshotOption.LifecycleState,
                    "Qualified",
                    StringComparison.Ordinal)
                && !string.IsNullOrWhiteSpace(
                    QualifiedSnapshotLifecycleReason)
                && CanCreateQualifiedSnapshot();
        }

        private bool CanRevokeQualifiedSnapshot()
        {
            return CanUseSelectedQualifiedSnapshot()
                && string.Equals(
                    SelectedQualifiedSnapshotOption.LifecycleState,
                    "Qualified",
                    StringComparison.Ordinal)
                && !string.IsNullOrWhiteSpace(
                    QualifiedSnapshotLifecycleReason);
        }

        private void RefreshQualifiedSnapshotOptions(
            string preferredSnapshotId = "")
        {
            string preferred = string.IsNullOrWhiteSpace(preferredSnapshotId)
                ? SelectedQualifiedSnapshotOption?.SnapshotId ?? string.Empty
                : preferredSnapshotId;
            IReadOnlyList<OpenVisionRecipeQualifiedSnapshotOption> options =
                qualifiedSnapshotController.List(
                    NormalizeRecipeName(selectedRecipeName),
                    SelectedPipelineOption?.PipelineName ?? string.Empty,
                    GetCurrentQualifiedSnapshotPipelinePath(),
                    SelectedValidationSetOption?.Set);
            QualifiedSnapshotOptions = options;
            SelectedQualifiedSnapshotOption = options.FirstOrDefault(option =>
                    string.Equals(
                        option.SnapshotId,
                        preferred,
                        StringComparison.OrdinalIgnoreCase))
                ?? options.FirstOrDefault();
            OnPropertyChanged(nameof(SelectedQualifiedSnapshotDetailText));
            CommandManager.InvalidateRequerySuggested();
        }

        private string GetCurrentQualifiedSnapshotPipelinePath()
        {
            if (SelectedPipelineOption == null
                || string.IsNullOrWhiteSpace(selectedRecipeName))
            {
                return string.Empty;
            }

            try
            {
                return RecipeWorkspaceService.GetVisionPipelinePath(
                    NormalizeRecipeName(selectedRecipeName),
                    SelectedPipelineOption.PipelineName);
            }
            catch (Exception exception) when (
                exception is ArgumentException
                || exception is IOException
                || exception is InvalidOperationException)
            {
                return string.Empty;
            }
        }

        private void NotifyQualifiedSnapshotContextChanged(bool includePreflight = true)
        {
            if (includePreflight)
            {
                OnPropertyChanged(nameof(QualifiedSnapshotPreflightText));
            }
            OnPropertyChanged(nameof(QualificationScopeClaimText));
            CommandManager.InvalidateRequerySuggested();
        }

        private void SetQualifiedSnapshotActionStatus(
            bool success,
            string message)
        {
            QualifiedSnapshotStatusText =
                (success ? "OK | " : "BLOCKED | ")
                + (message ?? string.Empty);
            StatusText = QualifiedSnapshotStatusText;
        }

        internal void DiscardSelectedStepEditForQualificationTest()
        {
            ClearSelectedStepEdit();
            NotifyQualifiedSnapshotContextChanged();
            RefreshCommandState();
        }

        internal void MarkSelectedStepEditDirtyForQualificationTest()
        {
            selectedStepEditSession.MarkDirty(
                "Qualification test pending Step edit.");
            NotifyQualifiedSnapshotContextChanged();
            RefreshCommandState();
        }


        #endregion


        #region Recipe workspace lifecycle

        private async void CreateRecipe()
        {
            if (!TryLeaveSelectedStepEdit(
                OpenVisionRecipePendingEditTransitionKind.Recipe,
                LocalText("새 Recipe", "New Recipe")))
            {
                return;
            }

            await CreateAndSwitchRecipeAsync(recipeWorkspaceUseCase.Create());
        }

        private async void CreateNamedRecipe()
        {
            string requestedName = EditRecipeName?.Trim();
            if (!TryLeaveSelectedStepEdit(
                OpenVisionRecipePendingEditTransitionKind.Recipe,
                requestedName))
            {
                return;
            }

            await CreateAndSwitchRecipeAsync(recipeWorkspaceUseCase.Create(requestedName));
        }

        private bool CanCreateNamedRecipe()
        {
            string requestedName = EditRecipeName?.Trim();
            return recipeWorkspaceUseCase.CanCreate(requestedName);
        }

        private void DuplicateSelectedRecipe()
        {
            string sourceName = NormalizeRecipeName(selectedRecipeName);
            string requestedName = NormalizeRecipeName(EditRecipeName);
            if (!TryLeaveSelectedStepEdit(
                OpenVisionRecipePendingEditTransitionKind.Recipe,
                requestedName))
            {
                return;
            }

            OpenVisionRecipeWorkspaceResult result = recipeWorkspaceUseCase.Duplicate(sourceName, requestedName);
            OpenVisionRecipeWorkspaceLifecycleProjection projection =
                workspaceLifecycleProjectionOwner.Project(
                    OpenVisionRecipeWorkspaceLifecycleOperation.Duplicate,
                    result);
            if (!result.Succeeded)
            {
                StatusText = projection.StatusText;
                return;
            }

            switchRecipe(result.RecipeName);
            StatusText = projection.StatusText;
            RefreshOptions();
            refreshAfterSwitch();
        }

        private bool CanDuplicateSelectedRecipe()
        {
            string selected = NormalizeRecipeName(selectedRecipeName);
            string requested = EditRecipeName?.Trim();
            return recipeWorkspaceUseCase.CanDuplicate(selected, requested, RecipeOptions);
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

            if (!TryLeaveSelectedStepEdit(
                OpenVisionRecipePendingEditTransitionKind.Recipe,
                newName))
            {
                return;
            }

            OpenVisionRecipeWorkspaceResult result = recipeWorkspaceUseCase.Rename(oldName, newName);
            OpenVisionRecipeWorkspaceLifecycleProjection projection =
                workspaceLifecycleProjectionOwner.Project(
                    OpenVisionRecipeWorkspaceLifecycleOperation.Rename,
                    result);
            if (!result.Succeeded)
            {
                StatusText = projection.StatusText;
                return;
            }

            switchRecipe(result.RecipeName);
            StatusText = projection.StatusText;
            RefreshOptions();
            refreshAfterSwitch();
        }

        private bool CanRenameSelectedRecipe()
        {
            string oldName = NormalizeRecipeName(selectedRecipeName);
            string newName = NormalizeRecipeName(EditRecipeName);
            return recipeWorkspaceUseCase.CanRename(oldName, newName, RecipeOptions);
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

            if (!TryLeaveSelectedStepEdit(
                OpenVisionRecipePendingEditTransitionKind.Recipe,
                LocalText("Recipe 삭제: ", "Delete Recipe: ") + deletedName))
            {
                return;
            }

            string fallback = RecipeOptions
                .FirstOrDefault(name => !string.Equals(name, deletedName, StringComparison.OrdinalIgnoreCase));
            fallback = NormalizeRecipeName(fallback);
            OpenVisionRecipeWorkspaceResult result = recipeWorkspaceUseCase.Delete(deletedName, fallback);
            OpenVisionRecipeWorkspaceLifecycleProjection projection =
                workspaceLifecycleProjectionOwner.Project(
                    OpenVisionRecipeWorkspaceLifecycleOperation.Delete,
                    result,
                    deletedName);
            if (!result.Succeeded)
            {
                StatusText = projection.StatusText;
                return;
            }

            switchRecipe(result.RecipeName);
            StatusText = projection.StatusText;
            RefreshOptions();
            refreshAfterSwitch();
        }

        private bool CanDeleteSelectedRecipe()
        {
            string selected = NormalizeRecipeName(selectedRecipeName);
            return recipeWorkspaceUseCase.CanDelete(selected, RecipeOptions);
        }

        private async Task CreateAndSwitchRecipeAsync(OpenVisionRecipeWorkspaceResult result)
        {
            OpenVisionRecipeWorkspaceLifecycleProjection projection =
                workspaceLifecycleProjectionOwner.Project(
                    OpenVisionRecipeWorkspaceLifecycleOperation.Create,
                    result);
            if (!projection.Succeeded)
            {
                return;
            }

            try
            {
                BeginRecipeSwitchingState(result.RecipeName);
                await yieldToUi();
                switchRecipe(result.RecipeName);
                RefreshAfterRecipeSwitchIfNeeded(result.RecipeName);
                await waitForRecipeSwitchCompletion();
                StatusText = projection.StatusText;
            }
            finally
            {
                IsSwitchingRecipe = false;
            }
        }

        private void SaveSelectedRecipe()
        {
            string recipeName = NormalizeRecipeName(selectedRecipeName);
            if (string.IsNullOrWhiteSpace(recipeName))
            {
                return;
            }

            if (selectedStepEditSession.IsDirty && !TryApplySelectedStepParameters())
            {
                return;
            }

            try
            {
                bool saved = saveRecipe();
                StatusText = saved
                    ? string.Format(
                        CultureInfo.CurrentCulture,
                        LocalText("레시피 저장 완료: {0}", "Recipe saved: {0}"),
                        recipeName)
                    : string.Format(
                        CultureInfo.CurrentCulture,
                        LocalText("레시피 저장 실패: {0}", "Recipe save failed: {0}"),
                        recipeName);
            }
            catch (Exception ex)
            {
                StatusText = string.Format(
                    CultureInfo.CurrentCulture,
                    LocalText("레시피 저장 실패: {0} / {1}", "Recipe save failed: {0} / {1}"),
                    recipeName,
                    ex.GetBaseException().Message);
            }
        }


        #endregion


        #region Validation sets

        private void CreateValidationSetFromSelectedPair()
        {
            CreateValidationSetFromSelectedPairCore();
        }

        internal bool CreateValidationSetFromSelectedPairForTest()
        {
            return CreateValidationSetFromSelectedPairCore();
        }

        private bool CreateValidationSetFromSelectedPairCore()
        {
            if (!CanCreateValidationSetFromSelectedPair())
            {
                return false;
            }

            OpenVisionRecipeCatalogPairValidationSetImportResult result =
                validationSetDocumentOwner.ImportCatalogPair(
                    SelectedSampleOption.Sample,
                    SampleOptions.Select(option => option?.Sample),
                    SelectedPipelineOption?.PipelineName);
            if (!result.Success)
            {
                ValidationSuiteStatusText = LocalText(
                    "카탈로그 쌍 가져오기 ERROR: ",
                    "Catalog pair import ERROR: ")
                    + result.Error;
                return false;
            }

            if (!TrySaveValidationSetDocument(LocalText(
                    "카탈로그 쌍을 검증 세트로 저장",
                    "Save catalog pair as validation set")))
            {
                return false;
            }

            SelectedValidationSuiteScopeOption = ValidationSuiteScopeOptions.FirstOrDefault(option =>
                string.Equals(
                    option.Key,
                    OpenVisionRecipeValidationSuiteScopeOption.LocalValidationSetKey,
                    StringComparison.OrdinalIgnoreCase));
            RefreshValidationSetOptions(result.SetName);
            NewValidationSetName = CreateUniqueValidationSetName();
            ValidationSuiteStatusText = string.Format(
                CultureInfo.CurrentCulture,
                result.Updated
                    ? LocalText(
                        "카탈로그 쌍 검증 세트를 갱신했습니다: {0} | OK {1} / NG {2} | 실행 안 함",
                        "Updated catalog pair validation set: {0} | OK {1} / NG {2} | not run")
                    : LocalText(
                        "카탈로그 쌍 검증 세트를 만들었습니다: {0} | OK {1} / NG {2} | 실행 안 함",
                        "Created catalog pair validation set: {0} | OK {1} / NG {2} | not run"),
                result.SetName,
                result.OkCount,
                result.NgCount);
            StatusText = ValidationSuiteStatusText;
            return true;
        }

        private bool CanCreateValidationSetFromSelectedPair()
        {
            return validationSetDocumentOwner.StorageReady
                && !executionSession.IsValidationSuiteRunning
                && !executionSession.IsSampleCheckRunning
                && !executionSession.IsPairCheckRunning
                && !executionSession.IsCatalogBenchmarkRunning
                && OpenVisionRecipeCatalogPairValidationSetService.CanImport(
                    SelectedSampleOption?.Sample,
                    SampleOptions.Select(option => option?.Sample));
        }

        private void CreateValidationSet()
        {
            if (!CanCreateValidationSet())
            {
                return;
            }

            string name = NewValidationSetName.Trim();
            if (!validationSetDocumentOwner.TryCreateSet(name))
            {
                return;
            }

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
            return validationSetDocumentOwner.StorageReady
                && !executionSession.IsValidationSuiteRunning
                && OpenVisionRecipeValidationSetStorage.IsValidSetName(name)
                && !validationSetDocumentOwner.ContainsSet(name);
        }

        private void DeleteValidationSet()
        {
            OpenVisionRecipeValidationSetOption option = SelectedValidationSetOption;
            if (!CanDeleteValidationSet() || option == null || !confirmDeleteValidationSet(option.Name))
            {
                return;
            }

            if (!validationSetDocumentOwner.TryDeleteSet(option.Name))
            {
                return;
            }

            if (!TrySaveValidationSetDocument(LocalText("검증 세트 삭제", "Delete validation set")))
            {
                return;
            }

            RefreshValidationSetOptions();
            ValidationSuiteStatusText = LocalText("로컬 검증 세트를 삭제했습니다: ", "Deleted local validation set: ") + option.Name;
        }

        private bool CanDeleteValidationSet()
        {
            return validationSetDocumentOwner.StorageReady
                && !executionSession.IsValidationSuiteRunning
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
                AddValidationSetImages(
                    expected,
                    paths,
                    ValidationSetPendingNotes,
                    ValidationSetPendingVariantId,
                    ValidationSetPendingMetricName,
                    ValidationSetPendingMetricMinimum,
                    ValidationSetPendingMetricMaximum);
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
                    AddValidationSetFolder(
                        expected,
                        folderPath,
                        ValidationSetPendingNotes,
                        ValidationSetPendingVariantId,
                        ValidationSetPendingMetricName,
                        ValidationSetPendingMetricMinimum,
                        ValidationSetPendingMetricMaximum);
                }
            }
            catch (Exception ex)
            {
                ValidationSuiteStatusText = LocalText("폴더 선택 ERROR: ", "Folder selection ERROR: ")
                    + ex.GetBaseException().Message;
            }
        }

        internal bool AddValidationSetFolderForTest(string expected, string folderPath, string notes = "")
        {
            return AddValidationSetFolder(expected, folderPath, notes, string.Empty, string.Empty, string.Empty, string.Empty);
        }

        internal bool AddValidationSetFolderForTest(
            string expected,
            string folderPath,
            string notes,
            string variantId,
            string metricName,
            string metricMinimum,
            string metricMaximum)
        {
            return AddValidationSetFolder(
                expected,
                folderPath,
                notes,
                variantId,
                metricName,
                metricMinimum,
                metricMaximum);
        }

        private bool AddValidationSetFolder(
            string expected,
            string folderPath,
            string notes,
            string variantId,
            string metricName,
            string metricMinimum,
            string metricMaximum)
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
                ValidationSuiteStatusText = OpenVisionRecipeValidationSetPresenter.BuildFolderImageRegistrationError(error);
                return false;
            }

            if (paths.Count == 0)
            {
                ValidationSuiteStatusText = OpenVisionRecipeValidationSetPresenter.BuildEmptyFolderImageRegistrationStatus();
                return false;
            }

            return AddValidationSetImages(
                expected,
                paths,
                notes,
                variantId,
                metricName,
                metricMinimum,
                metricMaximum);
        }

        internal bool AddValidationSetImagesForTest(string expected, IEnumerable<string> paths, string notes = "")
        {
            return AddValidationSetImages(expected, paths, notes, string.Empty, string.Empty, string.Empty, string.Empty);
        }

        internal bool AddValidationSetImagesForTest(
            string expected,
            IEnumerable<string> paths,
            string notes,
            string variantId,
            string metricName,
            string metricMinimum,
            string metricMaximum)
        {
            return AddValidationSetImages(
                expected,
                paths,
                notes,
                variantId,
                metricName,
                metricMinimum,
                metricMaximum);
        }

        private bool AddValidationSetImages(
            string expected,
            IEnumerable<string> paths,
            string notes,
            string variantId,
            string metricName,
            string metricMinimum,
            string metricMaximum)
        {
            OpenVisionRecipeValidationSetOption option = SelectedValidationSetOption;
            if (!CanAddValidationSetImages() || option?.Set == null)
            {
                return false;
            }

            if (!validationSetDocumentOwner.TryAddImages(
                    option.Name,
                    paths,
                    expected,
                    notes,
                    variantId,
                    metricName,
                    metricMinimum,
                    metricMaximum,
                    out int added,
                    out int updated,
                    out int skipped,
                    out string contractError))
            {
                if (!string.IsNullOrWhiteSpace(contractError))
                {
                    ValidationSuiteStatusText = LocalText("Variant 계약 ERROR: ", "Variant contract ERROR: ")
                        + contractError;
                    return false;
                }

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
            ValidationSuiteStatusText = OpenVisionRecipeValidationSetPresenter.BuildImageRegistrationStatus(
                expected,
                added,
                updated,
                skipped);
            return true;
        }

        private void LoadSelectedValidationVariantContract()
        {
            OpenVisionRecipeValidationSetImage image = SelectedValidationSetImageRow?.Image;
            validationSetPendingVariantId = image?.VariantId ?? string.Empty;
            validationSetPendingMetricName = image?.ExpectedMetricName ?? string.Empty;
            validationSetPendingMetricMinimum = image?.ExpectedMetricMinimum ?? string.Empty;
            validationSetPendingMetricMaximum = image?.ExpectedMetricMaximum ?? string.Empty;
            OnPropertyChanged(nameof(ValidationSetPendingVariantId));
            OnPropertyChanged(nameof(ValidationSetPendingMetricName));
            OnPropertyChanged(nameof(ValidationSetPendingMetricMinimum));
            OnPropertyChanged(nameof(ValidationSetPendingMetricMaximum));
        }

        private void ApplyValidationSetVariantContract()
        {
            OpenVisionRecipeValidationSetOption option = SelectedValidationSetOption;
            OpenVisionRecipeValidationSetImageRow row = SelectedValidationSetImageRow;
            if (!CanApplyValidationSetVariantContract()
                || option?.Set == null
                || row?.Image == null)
            {
                return;
            }

            if (!validationSetDocumentOwner.TryApplyVariantContract(
                    option.Name,
                    row.Path,
                    ValidationSetPendingVariantId,
                    ValidationSetPendingMetricName,
                    ValidationSetPendingMetricMinimum,
                    ValidationSetPendingMetricMaximum,
                    out string error))
            {
                ValidationSuiteStatusText = LocalText("Variant 계약 ERROR: ", "Variant contract ERROR: ") + error;
                return;
            }

            string setName = option.Name;
            string imagePath = row.Path;
            if (!TrySaveValidationSetDocument(LocalText("Validation Variant 적용", "Apply validation Variant")))
            {
                return;
            }

            RefreshValidationSetOptions(setName);
            SelectedValidationSetImageRow = ValidationSetImageRows.FirstOrDefault(item =>
                string.Equals(item.Path, imagePath, StringComparison.OrdinalIgnoreCase));
            ValidationSuiteStatusText = LocalText(
                "선택 이미지의 Variant 계약을 저장했습니다. Preview/Run은 실행되지 않았습니다.",
                "Saved the selected image Variant contract. Preview/Run was not executed.");
        }

        private void ResetValidationSetVariantContract()
        {
            ValidationSetPendingVariantId = string.Empty;
            ValidationSetPendingMetricName = string.Empty;
            ValidationSetPendingMetricMinimum = string.Empty;
            ValidationSetPendingMetricMaximum = string.Empty;
            ApplyValidationSetVariantContract();
        }

        private bool CanApplyValidationSetVariantContract()
        {
            return validationSetDocumentOwner.StorageReady
                && !executionSession.IsValidationSuiteRunning
                && SelectedValidationSetOption?.Set != null
                && !SelectedValidationSetOption.Set.IsIdentityLocked
                && SelectedValidationSetImageRow?.Image != null;
        }

        private bool CanAddValidationSetImages()
        {
            return validationSetDocumentOwner.StorageReady
                && !executionSession.IsValidationSuiteRunning
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
            if (!validationSetDocumentOwner.TryRepairMissingImagePath(
                    option.Name,
                    row.Path,
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
            return validationSetDocumentOwner.StorageReady
                && !executionSession.IsValidationSuiteRunning
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

            if (!validationSetDocumentOwner.TryRemoveImage(option.Name, row.Path))
            {
                return;
            }

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
            return validationSetDocumentOwner.StorageReady
                && !executionSession.IsValidationSuiteRunning
                && SelectedValidationSetOption?.Set != null
                && !SelectedValidationSetOption.Set.IsIdentityLocked
                && SelectedValidationSetImageRow?.Image != null;
        }

        private bool TrySaveValidationSetDocument(string operation)
        {
            if (validationSetDocumentOwner.TrySave(
                NormalizeRecipeName(selectedRecipeName),
                out string error))
            {
                return true;
            }

            RefreshValidationSetOptions();
            ValidationSuiteStatusText = OpenVisionRecipeValidationSetPresenter.BuildSaveErrorStatus(operation, error);
            return false;
        }

        private void RefreshValidationSetOptions(string preferredSetName = null, bool refreshCommandState = true)
        {
            string recipeName = NormalizeRecipeName(selectedRecipeName);
            string previousName = preferredSetName
                ?? SelectedValidationSetOption?.Name
                ?? string.Empty;
            string previousTrainName = PinArrayGapTrainValidationSetOption?.Name ?? string.Empty;
            string previousValidationName = PinArrayGapValidationValidationSetOption?.Name ?? string.Empty;
            string previousTestName = PinArrayGapTestValidationSetOption?.Name ?? string.Empty;
            string previousImagePath = SelectedValidationSetImageRow?.Path ?? string.Empty;
            bool storageReady = validationSetDocumentOwner.TryLoad(recipeName, out string error);

            if (!storageReady)
            {
                validationSetSelectionOwner.Clear();
                OnPropertyChanged(nameof(ValidationSetOptions));
                OnPropertyChanged(nameof(SelectedValidationSetOption));
                OnPropertyChanged(nameof(PinArrayGapTrainValidationSetOption));
                OnPropertyChanged(nameof(PinArrayGapValidationValidationSetOption));
                OnPropertyChanged(nameof(PinArrayGapTestValidationSetOption));
                OnPropertyChanged(nameof(ValidationSetImageRows));
                OnPropertyChanged(nameof(SelectedValidationSetImageRow));
                OnPropertyChanged(nameof(ValidationSetSelectionSummaryText));
                LoadSelectedValidationVariantContract();
                ValidationSuiteStatusText = LocalText("로컬 검증 세트 로드 ERROR: ", "Local validation set load ERROR: ") + error;
                RefreshPinArrayGapValidationIdentityState();
                NotifyValidationSetEvidenceChanged();
                if (refreshCommandState)
                {
                    RefreshCommandState();
                }
                return;
            }

            if (string.IsNullOrWhiteSpace(previousTrainName)
                && string.IsNullOrWhiteSpace(previousValidationName)
                && string.IsNullOrWhiteSpace(previousTestName)
                && pinArrayGapValidationIdentityOwner.TryGetFrozenSelectionNames(
                    recipeName,
                    out string frozenTrainName,
                    out string frozenValidationName,
                    out string frozenTestName))
            {
                previousTrainName = frozenTrainName;
                previousValidationName = frozenValidationName;
                previousTestName = frozenTestName;
            }

            validationSetSelectionOwner.Refresh(
                previousName,
                previousTrainName,
                previousValidationName,
                previousTestName,
                previousImagePath);
            OnPropertyChanged(nameof(ValidationSetOptions));
            OnPropertyChanged(nameof(SelectedValidationSetOption));
            OnPropertyChanged(nameof(PinArrayGapTrainValidationSetOption));
            OnPropertyChanged(nameof(PinArrayGapValidationValidationSetOption));
            OnPropertyChanged(nameof(PinArrayGapTestValidationSetOption));
            OnPropertyChanged(nameof(ValidationSetImageRows));
            OnPropertyChanged(nameof(SelectedValidationSetImageRow));
            LoadSelectedValidationVariantContract();
            RefreshPinArrayGapValidationIdentityState();
            NotifyValidationSetEvidenceChanged();

            OnPropertyChanged(nameof(ValidationSetSelectionSummaryText));
            if (refreshCommandState)
            {
                RefreshCommandState();
            }
        }

        private void RefreshValidationSetImageRows()
        {
            string previousPath = SelectedValidationSetImageRow?.Path ?? string.Empty;
            validationSetSelectionOwner.RefreshImageRows(previousPath);
            OnPropertyChanged(nameof(ValidationSetImageRows));
            OnPropertyChanged(nameof(SelectedValidationSetImageRow));
            LoadSelectedValidationVariantContract();
        }

        private string CreateUniqueValidationSetName()
        {
            return validationSetDocumentOwner.CreateUniqueSetName();
        }


        #endregion
}

}
