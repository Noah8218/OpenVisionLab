using System.Windows.Input;
using OpenVisionLab.Mvvm;

namespace OpenVisionLab
{
    public sealed partial class OpenVisionShellHostRecipeCommandSurface
    {
        private void InitializeRecipeCommands()
        {
            InitializeRecipeManagementCommands();
            InitializePipelineCommands();
            InitializeLlmDraftCommands();
            InitializeValidationCommands();
            InitializeReviewCommands();
            InitializeSampleSelectionCommands();
            InitializeStepNavigationCommands();
            InitializeRunHistoryCommands();
            InitializeGeneralCommands();
        }

        private void InitializeRecipeManagementCommands()
        {
            CreateRecipeCommand = new RelayCommand(CreateRecipe);
            CreateNamedRecipeCommand = new RelayCommand(CreateNamedRecipe, CanCreateNamedRecipe);
            DuplicateRecipeCommand = new RelayCommand(DuplicateSelectedRecipe, CanDuplicateSelectedRecipe);
            RenameRecipeCommand = new RelayCommand(RenameSelectedRecipe, CanRenameSelectedRecipe);
            DeleteRecipeCommand = new RelayCommand(DeleteSelectedRecipe, CanDeleteSelectedRecipe);
        }

        private void InitializePipelineCommands()
        {
            ImportPipelineXmlCommand = new RelayCommand(ImportPipelineXml, CanUseSelectedRecipe);
            ExportPipelineXmlCommand = new RelayCommand(ExportActivePipelineXml, CanUseSelectedRecipe);
            ExportRecipeReviewBundleCommand = new RelayCommand(ExportActivePipelineReviewBundle, CanUseSelectedRecipe);
            DuplicateFromSampleCommand = new RelayCommand(DuplicatePipelineFromSample, CanDuplicatePipelineFromSample);
            ActivatePipelineCommand = new RelayCommand(ActivateSelectedPipeline, CanUseSelectedPipeline);
            DuplicatePipelineCommand = new RelayCommand(DuplicateSelectedPipeline, CanUseSelectedPipeline);
            RenamePipelineCommand = new RelayCommand(RenameSelectedPipeline, CanRenameSelectedPipeline);
            DeletePipelineCommand = new RelayCommand(DeleteSelectedPipeline, CanDeleteSelectedPipeline);
        }

        private void InitializeLlmDraftCommands()
        {
            LoadLlmXmlDraftCommand = new RelayCommand(LoadLlmXmlDraft, CanUseSelectedRecipe);
            ValidateLlmXmlDraftCommand = new RelayCommand(ValidateLlmXmlDraft, CanUseLlmXmlDraft);
            ImportLlmXmlDraftCommand = new RelayCommand(ImportLlmXmlDraft, CanImportLlmXmlDraft);
            CopyLlmPromptCommand = new RelayCommand(CopyLlmPrompt, CanCopyLlmPrompt);
            CopyLlmReviewBundleCommand = new RelayCommand(CopyLlmReviewBundle, CanCopyLlmReviewBundle);
            PasteLlmXmlDraftFromClipboardCommand = new RelayCommand(PasteLlmXmlDraftFromClipboard);
            CreateLlmTemplateXmlDraftCommand = new RelayCommand(CreateLlmTemplateXmlDraft, CanUseSelectedRecipe);
            CreateGuidedSetupStarterXmlCommand = new RelayCommand(CreateGuidedSetupStarterXml, CanCreateGuidedSetupStarterXml);
            CreatePinGapIntentXmlDraftCommand = new RelayCommand(CreatePinGapIntentXmlDraft, CanUseSelectedRecipe);
            CreateBlobCountIntentXmlDraftCommand = new RelayCommand(CreateBlobCountIntentXmlDraft, CanUseSelectedRecipe);
            CreateContourCountIntentXmlDraftCommand = new RelayCommand(CreateContourCountIntentXmlDraft, CanUseSelectedRecipe);
            RefreshLlmDraftReviewCommand = new RelayCommand(RefreshLlmDraftReview, CanUseLlmXmlDraft);
            BuildLlmPromptCommand = new RelayCommand(BuildLlmPrompt, CanUseSelectedRecipe);
        }

        private void InitializeValidationCommands()
        {
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
        }

        private void InitializeReviewCommands()
        {
            UseSelectedSampleReferenceCommand = new RelayCommand(UseSelectedSampleReference, CanUseSelectedSampleReference);
            SuggestPinGapIntentRoiSamplesCommand = new RelayCommand(SuggestPinGapIntentRoiSamples, CanSuggestPinGapIntentRoiSamples);
            RunRecipeGuidedNextActionCommand = new RelayCommand(RunRecipeGuidedNextAction, CanRunRecipeGuidedNextAction);
            FreezePinArrayGapValidationIdentityCommand = new RelayCommand(
                FreezePinArrayGapValidationIdentity,
                CanFreezePinArrayGapValidationIdentity);
            OpenPinArrayGapValidationRunsCommand = new RelayCommand(
                OpenPinArrayGapValidationRuns,
                CanOpenPinArrayGapValidationRuns);
            SelectPairSampleResultCommand = new RelayCommand<OpenVisionRecipePairSampleRunSummary>(
                SelectPairSampleResult,
                CanSelectPairSampleResult);
            CopyOperatorHandoffReportCommand = new RelayCommand(CopyOperatorHandoffReport, CanCopyOperatorHandoffReport);
            CopySelectedRecentBatchRunReviewCommand = new RelayCommand(CopySelectedRecentBatchRunReview, CanCopySelectedRecentBatchRunReview);
        }

        private void InitializeSampleSelectionCommands()
        {
            LoadSelectedRunSampleImageToInputLayerCommand = new RelayCommand(LoadSelectedRunSampleImageToInputLayer, CanLoadSelectedRunSampleImageToInputLayer);
            OpenSelectedRecentBatchRunEvidenceCommand = new RelayCommand(OpenSelectedRecentBatchRunEvidence, CanOpenSelectedRecentBatchRunEvidence);
        }

        private void InitializeStepNavigationCommands()
        {
            NavigateSelectedStepInputLayerCommand = new RelayCommand(NavigateSelectedStepInputLayer, CanNavigateSelectedStepInputLayer);
            NavigateSelectedStepOutputLayerCommand = new RelayCommand(NavigateSelectedStepOutputLayer, CanNavigateSelectedStepOutputLayer);
            FocusSelectedRunFailureStepCommand = new RelayCommand(FocusSelectedRunFailureStep, CanFocusSelectedRunFailureStep);
            SelectPreviousPipelinePreviewStepCommand = new RelayCommand(SelectPreviousPipelinePreviewStep, CanSelectPreviousPipelinePreviewStep);
            SelectNextPipelinePreviewStepCommand = new RelayCommand(SelectNextPipelinePreviewStep, CanSelectNextPipelinePreviewStep);
        }

        private void InitializeRunHistoryCommands()
        {
            OpenSelectedStepToolCommand = new RelayCommand(OpenSelectedStepTool, CanOpenSelectedStepTool);
            LoadSelectedStepParametersCommand = new RelayCommand(LoadSelectedStepParameters, CanLoadSelectedStepParameters);
            ApplySelectedStepParametersCommand = new RelayCommand(ApplySelectedStepParameters, CanApplySelectedStepParameters);
        }

        private void InitializeGeneralCommands()
        {
            OpenPipelineReviewCommand = new RelayCommand(this.openPipelineReview, CanUseSelectedRecipe);
        }
    }
}
