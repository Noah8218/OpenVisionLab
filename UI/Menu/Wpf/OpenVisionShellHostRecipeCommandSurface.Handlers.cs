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
    public sealed partial class OpenVisionShellHostRecipeCommandSurface
    {
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

            if (!TryLeaveSelectedStepEdit(
                OpenVisionRecipePendingEditTransitionKind.Recipe,
                normalized))
            {
                OnPropertyChanged(nameof(SelectedRecipeName));
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
                return;
            }

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
                    Notes = image.Notes,
                    VariantId = image.VariantId,
                    ExpectedMetricName = image.ExpectedMetricName,
                    ExpectedMetricMinimum = image.ExpectedMetricMinimum,
                    ExpectedMetricMaximum = image.ExpectedMetricMaximum
                })
                .ToList();
            string recipeName = NormalizeRecipeName(selectedRecipeName);
            string pipelineName = SelectedPipelineOption?.PipelineName ?? string.Empty;
            string pipelinePath = RecipeWorkspaceService.GetVisionPipelinePath(recipeName, pipelineName);

            executionSession.StartValidationSuite(
                true,
                LocalText("로컬 세트 실행 중: ", "Running local set: ") + setName);
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
                    if (executionSession.StopRequested)
                    {
                        break;
                    }

                    OpenVisionRecipeValidationSetImage image = images[index];
                    VisionPipelineSampleCatalogItem sample = CreateLocalValidationSample(setName, image, index);
                    VisionPipelineSampleCheckResult result =
                        await VisionPipelineSampleCheckService.RunSampleCheckWithReportSafeAsync(sample, pipelineXmlText, recipeName);
                    VisionPipelineBatchSampleRunResult storageResult = CreateBatchSampleRunResult(sample, result);
                    storageResult.VariantId = OpenVisionRecipeValidationSetStorage.GetVariantDisplayId(image);
                    storageResult.ExpectedMetricName = image.ExpectedMetricName ?? string.Empty;
                    storageResult.ExpectedMetricMinimum = image.ExpectedMetricMinimum ?? string.Empty;
                    storageResult.ExpectedMetricMaximum = image.ExpectedMetricMaximum ?? string.Empty;
                    storageResult.ExpectedText = "ExpectedActual: Expected "
                        + image.Expected
                        + " | Variant "
                        + storageResult.VariantId
                        + (string.IsNullOrWhiteSpace(image.ExpectedMetricName)
                            ? string.Empty
                            : " | " + OpenVisionRecipeValidationSetStorage.BuildExpectedMetricText(image));
                    VisionPipelineBatchOutcomeContract.Apply(
                        storageResult,
                        result?.ExecutionCompleted == true,
                        result?.ActualSuccess == true,
                        hasJudgment: true,
                        expectedSuccess: !image.IsExpectedNg,
                        judgmentCorrect: result?.Success == true);
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

                bool isPartial = executionSession.StopRequested && storageResults.Count < images.Count;
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
                executionSession.CompleteValidationSuite();
                RefreshCommandState();
            }
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
            StatusText = ValidationSuiteStatusText;
            RefreshCommandState();
        }

        private static bool IsExpectedOutcomeCorrect(VisionPipelineBatchSampleRunResult result)
        {
            return VisionPipelineBatchOutcomeContract.ResolveJudgmentCorrect(result);
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
                ExpectedMetricName = image.ExpectedMetricName ?? string.Empty,
                ExpectedMetricMinimum = image.ExpectedMetricMinimum ?? string.Empty,
                ExpectedMetricMaximum = image.ExpectedMetricMaximum ?? string.Empty,
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

            executionSession.StartValidationSuite(
                false,
                LocalText("Selected sample suite 실행 중: ", "Running selected-sample suite: ") + sampleOption.SampleName);
            executionSession.StartSampleCheck();
            LatestSampleRunSummary = OpenVisionRecipeSampleRunSummary.CreateRunning(sampleOption, recipeName, pipelineName);
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
                executionSession.CompleteSampleCheck();
                executionSession.CompleteValidationSuite();
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

            executionSession.StartSampleCheck();
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
                executionSession.CompleteSampleCheck();
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

            executionSession.StartPairCheck();
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
                executionSession.CompletePairCheck();
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

            executionSession.StartCatalogBenchmark();
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
                executionSession.CompleteCatalogBenchmark();
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
                && !executionSession.IsValidationSuiteRunning
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
                || !CanUseSelectedPipeline())
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

        private bool CanRunLocalValidationSet(
            OpenVisionRecipeValidationSetOption option)
        {
            if (!validationSetStorageReady
                || executionSession.IsValidationSuiteRunning
                || executionSession.IsCatalogBenchmarkRunning
                || executionSession.IsPairCheckRunning
                || executionSession.IsSampleCheckRunning
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
            VisionPipelineBatchSampleRunResult storageResult = new VisionPipelineBatchSampleRunResult
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
            VisionPipelineBatchOutcomeContract.Apply(
                storageResult,
                result?.ExecutionCompleted == true,
                result?.ActualSuccess == true,
                hasJudgment: false,
                expectedSuccess: true,
                judgmentCorrect: false);
            return storageResult;
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

            if (!TryLoadSelectedPipelineStep(out string recipeName, out string pipelineName, out VisionPipeline pipeline, out VisionPipelineStep step, out string message))
            {
                SetSelectedStepEditStatus(message);
                return false;
            }

            int selectedIndex = SelectedPipelinePreviewStep?.Index ?? 0;
            string pipelinePath = RecipeWorkspaceService.GetVisionPipelinePath(recipeName, pipelineName);
            if (!VisionPipelineStorage.TryLoadFromFile(
                pipelinePath,
                out VisionPipeline originalPipeline,
                out string originalLoadMessage))
            {
                SetSelectedStepEditStatus(
                    OpenVisionRecipeText.Local(
                        "기존 XML 백업을 읽지 못해 적용을 중단했습니다: ",
                        "Apply was stopped because the existing XML backup could not be read: ")
                    + originalLoadMessage);
                return false;
            }

            if (!VisionPipelineStepPropertyMapper.ApplyProperty(step, SelectedStepEditObject))
            {
                SetSelectedStepEditStatus(OpenVisionRecipeText.Local("이 Step 파라미터는 XML로 반영할 수 없습니다.", "This step property set cannot be applied to XML."));
                return false;
            }

            try
            {
                pipeline.Name = pipelineName;
                saveStepEditPipeline(recipeName, pipeline);
            }
            catch (Exception ex)
            {
                string saveFailure = OpenVisionRecipeText.Local("XML 저장 실패: ", "XML save failed: ")
                    + ex.GetBaseException().Message;
                TryRestorePipelineAfterFailedApply(recipeName, originalPipeline, out string restoreMessage);
                SetSelectedStepEditStatus(saveFailure + Environment.NewLine + restoreMessage);
                return false;
            }

            OpenVisionRecipeRoundTripValidationResult validation =
                validateStepEditRoundTrip(recipeName, pipeline)
                ?? new OpenVisionRecipeRoundTripValidationResult
                {
                    Succeeded = false,
                    Message = OpenVisionRecipeText.Local(
                        "왕복 검증 결과가 없습니다.",
                        "No round-trip validation result was returned.")
                };
            string validationMessage = validation.Message ?? string.Empty;
            if (!validation.Succeeded)
            {
                bool restored = TryRestorePipelineAfterFailedApply(
                    recipeName,
                    originalPipeline,
                    out string restoreMessage);
                SetSelectedStepEditStatus(
                    OpenVisionRecipeText.Local(
                        "XML 왕복 검증에 실패하여 전환을 중단했습니다: ",
                        "Transition was stopped because XML round-trip validation failed: ")
                    + validationMessage
                    + Environment.NewLine
                    + restoreMessage);
                StatusText = restored
                    ? OpenVisionRecipeText.Local(
                        "Step XML 적용 실패 — 기존 저장 상태 복원",
                        "Step XML apply failed — previous saved state restored")
                    : OpenVisionRecipeText.Local(
                        "Step XML 적용 실패 — 복원 오류 확인 필요",
                        "Step XML apply failed — review the restore error");
                return false;
            }

            selectedStepEditSession.MarkClean();
            UpdateSelectedRecipeSummary();
            SelectedPipelinePreviewStep = SelectedRecipeSummary?.PipelinePreviewSteps?
                .FirstOrDefault(stepPreview => stepPreview.Index == selectedIndex);
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
                    validationMessage,
                    IsSelectedRunLocalValidationSet()));
            StatusText = OpenVisionRecipeText.Local("Step XML 반영 완료", "Step XML apply complete");
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
            object property = VisionPipelineStepPropertyMapper.CreateProperty(
                step,
                new VisionPipelinePropertyContext(pipeline, selectedStepIndex));
            if (property == null)
            {
                ClearSelectedStepEdit();
                if (updateStatus)
                {
                    SetSelectedStepEditStatus(OpenVisionRecipeText.Local("지원하지 않는 Step 도구입니다: ", "Unsupported step tool: ") + step.ToolType);
                }

                return false;
            }

            selectedStepEditSession.Load(
                property,
                OpenVisionRecipeText.Local("불러옴: ", "Loaded: ")
                + pipelineName
                + " / Step "
                + (SelectedPipelinePreviewStep?.Index ?? 0).ToString(CultureInfo.InvariantCulture),
                updateStatus);

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

        private static bool IsSamePipelinePreviewStep(
            OpenVisionRecipePipelineStepPreview left,
            OpenVisionRecipePipelineStepPreview right)
        {
            if (ReferenceEquals(left, right))
            {
                return true;
            }

            return left != null
                && right != null
                && left.Index == right.Index
                && string.Equals(left.Name, right.Name, StringComparison.Ordinal)
                && string.Equals(left.ToolType, right.ToolType, StringComparison.OrdinalIgnoreCase)
                && string.Equals(left.InputLayer, right.InputLayer, StringComparison.OrdinalIgnoreCase)
                && string.Equals(left.OutputLayer, right.OutputLayer, StringComparison.OrdinalIgnoreCase);
        }

        private static bool TryRestorePipelineAfterFailedApply(
            string recipeName,
            VisionPipeline originalPipeline,
            out string message)
        {
            try
            {
                VisionPipelineStorage.Save(recipeName, originalPipeline);
                if (VisionPipelineStorage.TryValidateRoundTrip(
                    recipeName,
                    originalPipeline,
                    out string validationMessage))
                {
                    message = OpenVisionRecipeText.Local(
                        "기존 저장 상태를 복원했습니다. ",
                        "The previous saved state was restored. ")
                        + validationMessage;
                    return true;
                }

                message = OpenVisionRecipeText.Local(
                    "기존 XML을 다시 저장했지만 복원 검증에 실패했습니다: ",
                    "The previous XML was saved again, but restore validation failed: ")
                    + validationMessage;
                return false;
            }
            catch (Exception ex)
            {
                message = OpenVisionRecipeText.Local(
                    "기존 저장 상태 복원 실패: ",
                    "Failed to restore the previous saved state: ")
                    + ex.GetBaseException().Message;
                return false;
            }
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

        private void OnExecutionSessionPropertyChanged(
            object sender,
            System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e?.PropertyName)
            {
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

        private void SetSelectedRecipeName(string recipeName)
        {
            string normalized = NormalizeRecipeName(recipeName);
            bool changed = !string.Equals(selectedRecipeName, normalized, StringComparison.Ordinal);
            selectedRecipeName = normalized;
            if (changed)
            {
                SelectedPipelinePreviewStep = null;
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

    }
}
