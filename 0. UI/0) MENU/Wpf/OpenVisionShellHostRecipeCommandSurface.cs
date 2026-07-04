using Lib.OpenCV.Pipeline;
using OpenVisionLab.Mvvm;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

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
        private readonly IReadOnlyList<string> llmToolTemplateOptions = new[]
        {
            "Threshold + Blob",
            "Template Matching",
            "Edge Based Matching",
            "Line Measurement",
            "Mean Intensity"
        };
        private IReadOnlyList<string> recipeOptions = Array.Empty<string>();
        private IReadOnlyList<string> filteredRecipeOptions = Array.Empty<string>();
        private IReadOnlyList<OpenVisionRecipePipelineOption> pipelineOptions = Array.Empty<OpenVisionRecipePipelineOption>();
        private IReadOnlyList<OpenVisionRecipeSampleOption> sampleOptions = Array.Empty<OpenVisionRecipeSampleOption>();
        private string selectedRecipeName = string.Empty;
        private string recipeFilterText = string.Empty;
        private string editRecipeName = string.Empty;
        private string pipelineEditName = string.Empty;
        private string selectedLlmToolTemplate = "Template Matching";
        private string llmInspectionGoalText = string.Empty;
        private string llmDetectionPointText = string.Empty;
        private string llmPromptText = string.Empty;
        private string llmXmlDraftText = string.Empty;
        private string llmReferenceImagePath = string.Empty;
        private string llmXmlDraftValidationReport = string.Empty;
        private string llmXmlDraftDependencyReport = string.Empty;
        private string llmXmlDraftReviewReport = string.Empty;
        private string statusText = string.Empty;
        private bool isRefreshingOptions;
        private bool isSelectingRecipe;
        private bool isSampleCheckRunning;
        private bool isPairCheckRunning;
        private OpenVisionRecipePipelineOption selectedPipelineOption;
        private OpenVisionRecipeSampleOption selectedSampleOption;
        private OpenVisionRecipeSampleRunSummary latestSampleRunSummary = OpenVisionRecipeSampleRunSummary.Empty;
        private OpenVisionRecipePairRunSummary latestPairRunSummary = OpenVisionRecipePairRunSummary.Empty;
        private OpenVisionRecipeManagerSummary selectedRecipeSummary = OpenVisionRecipeManagerSummary.Empty;

        internal OpenVisionShellHostRecipeCommandSurface(
            Func<string> currentRecipeProvider,
            Action<string> switchRecipe,
            Action refreshAfterSwitch,
            Func<string, bool> confirmDeleteRecipe = null,
            Func<string, string, bool> confirmDeletePipeline = null,
            Func<string> selectImportPipelineXmlPath = null,
            Func<string, string> selectExportPipelineXmlPath = null)
        {
            this.currentRecipeProvider = currentRecipeProvider ?? throw new ArgumentNullException(nameof(currentRecipeProvider));
            this.switchRecipe = switchRecipe ?? throw new ArgumentNullException(nameof(switchRecipe));
            this.refreshAfterSwitch = refreshAfterSwitch ?? throw new ArgumentNullException(nameof(refreshAfterSwitch));
            this.confirmDeleteRecipe = confirmDeleteRecipe ?? (_ => true);
            this.confirmDeletePipeline = confirmDeletePipeline ?? ((_, _) => true);
            this.selectImportPipelineXmlPath = selectImportPipelineXmlPath ?? (() => string.Empty);
            this.selectExportPipelineXmlPath = selectExportPipelineXmlPath ?? (_ => string.Empty);

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
            UseSelectedSampleReferenceCommand = new RelayCommand(UseSelectedSampleReference, CanUseSelectedSampleReference);
            RunSelectedSampleCheckCommand = new RelayCommand(RunSelectedSampleCheck, CanRunSelectedSampleCheck);
            RunSelectedSamplePairCheckCommand = new RelayCommand(RunSelectedSamplePairCheck, CanRunSelectedSamplePairCheck);
            BuildLlmPromptCommand = new RelayCommand(BuildLlmPrompt, CanUseSelectedRecipe);
            CreateLlmTemplateXmlDraftCommand = new RelayCommand(CreateLlmTemplateXmlDraft, CanUseSelectedRecipe);
            RefreshLlmDraftReviewCommand = new RelayCommand(RefreshLlmDraftReview, CanUseLlmXmlDraft);
            RefreshSampleOptions();
            RefreshOptions();
        }

        public IReadOnlyList<string> RecipeOptions
        {
            get => recipeOptions;
            private set => SetProperty(ref recipeOptions, value ?? Array.Empty<string>());
        }

        public IReadOnlyList<string> FilteredRecipeOptions
        {
            get => filteredRecipeOptions;
            private set => SetProperty(ref filteredRecipeOptions, value ?? Array.Empty<string>());
        }

        public IReadOnlyList<OpenVisionRecipePipelineOption> PipelineOptions
        {
            get => pipelineOptions;
            private set => SetProperty(ref pipelineOptions, value ?? Array.Empty<OpenVisionRecipePipelineOption>());
        }

        public IReadOnlyList<OpenVisionRecipeSampleOption> SampleOptions
        {
            get => sampleOptions;
            private set => SetProperty(ref sampleOptions, value ?? Array.Empty<OpenVisionRecipeSampleOption>());
        }

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

        public string LlmPromptText
        {
            get => llmPromptText;
            set => SetProperty(ref llmPromptText, value ?? string.Empty);
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
                    OnPropertyChanged(nameof(SelectedSampleAcceptanceSummaryText));
                    OnPropertyChanged(nameof(RunSelectedSampleCheckText));
                    OnPropertyChanged(nameof(RunSelectedSamplePairCheckText));
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
            private set => SetProperty(ref selectedRecipeSummary, value ?? OpenVisionRecipeManagerSummary.Empty);
        }

        public OpenVisionRecipeSampleRunSummary LatestSampleRunSummary
        {
            get => latestSampleRunSummary;
            private set => SetProperty(ref latestSampleRunSummary, value ?? OpenVisionRecipeSampleRunSummary.Empty);
        }

        public OpenVisionRecipePairRunSummary LatestPairRunSummary
        {
            get => latestPairRunSummary;
            private set => SetProperty(ref latestPairRunSummary, value ?? OpenVisionRecipePairRunSummary.Empty);
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

        public ICommand UseSelectedSampleReferenceCommand { get; }

        public ICommand RunSelectedSampleCheckCommand { get; }

        public ICommand RunSelectedSamplePairCheckCommand { get; }

        public ICommand BuildLlmPromptCommand { get; }

        public ICommand CreateLlmTemplateXmlDraftCommand { get; }

        public ICommand RefreshLlmDraftReviewCommand { get; }

        public string NewRecipeButtonText => LocalText("새 레시피", "New recipe");

        public string RecipeSelectorToolTipText => LocalText("레시피 선택 / 전환", "Select or switch recipe");

        public string ManagerButtonText => LocalText("레시피 관리", "Manage recipes");

        public string ManagerButtonShortText => LocalText("관리", "Manage");

        public string ManagerTitleText => LocalText("레시피 관리", "Recipe manager");

        public string RecipeListText => LocalText("레시피 목록", "Recipe list");

        public string RecipeFilterLabelText => LocalText("검색", "Search");

        public string EditRecipeNameLabelText => LocalText("선택/새 이름", "Selected/new name");

        public string CreateNamedRecipeText => LocalText("새로 만들기", "Create");

        public string DuplicateRecipeText => LocalText("복제", "Duplicate");

        public string RenameRecipeText => LocalText("이름 변경", "Rename");

        public string DeleteRecipeText => LocalText("삭제", "Delete");

        public string ImportPipelineXmlText => LocalText("XML 가져오기", "Import XML");

        public string ExportPipelineXmlText => LocalText("XML 내보내기", "Export XML");

        public string RecipeDetailText => LocalText("레시피 상세", "Recipe details");

        public string DuplicateFromSampleText => "Sample copy";

        public string PipelineListText => "Pipelines";

        public string PipelineNameText => "Pipeline name";

        public string ActivatePipelineText => "Active";

        public string DuplicatePipelineText => "Duplicate";

        public string RenamePipelineText => "Rename";

        public string DeletePipelineText => "Delete";

        public string SampleSourceText => "Sample source";

        public string SampleAcceptanceText => "Sample acceptance";

        public string SampleCheckResultText => "Sample check result";

        public string PairCheckResultText => "Good/Bad pair check";

        public string RunSelectedSampleCheckText => isSampleCheckRunning ? "Running..." : "Run check";

        public string RunSelectedSamplePairCheckText => isPairCheckRunning ? "Running..." : "Run pair";

        public string SelectedSampleAcceptanceSummaryText =>
            SelectedSampleOption?.AcceptanceSummaryText ?? "Select a sample to review expected metric gates.";

        public string LlmXmlValidationReportText => "LLM XML validation report";

        public string PipelinePreviewStepListText => "Pipeline preview step list";

        public string LlmAssistantText => "LLM assistant";

        public string LlmToolTemplateText => "Tool template";

        public string LlmInspectionGoalLabelText => "Inspection goal";

        public string LlmDetectionPointLabelText => "Detection points";

        public string BuildLlmPromptButtonText => "Build prompt";

        public string CreateLlmTemplateXmlText => "XML starter";

        public string RefreshLlmDraftReviewText => "Review";

        public string LlmPromptPreviewText => "Prompt preview";

        public string LlmXmlDraftLabelText => "LLM XML draft";

        public string LoadLlmXmlDraftText => "Load XML";

        public string ValidateLlmXmlDraftButtonText => "Validate";

        public string ImportLlmXmlDraftText => "Import";

        public string UseSelectedSampleReferenceText => "Use sample";

        public string LlmReferenceImageText => "Reference image";

        public string LlmDraftValidationText => "Draft validation";

        public string LlmDependencyReportText => "Dependency copy report";

        public string LlmDraftReviewReportText => "Draft import review";

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
            OnPropertyChanged(nameof(RecipeListText));
            OnPropertyChanged(nameof(RecipeFilterLabelText));
            OnPropertyChanged(nameof(EditRecipeNameLabelText));
            OnPropertyChanged(nameof(CreateNamedRecipeText));
            OnPropertyChanged(nameof(DuplicateRecipeText));
            OnPropertyChanged(nameof(RenameRecipeText));
            OnPropertyChanged(nameof(DeleteRecipeText));
            OnPropertyChanged(nameof(ImportPipelineXmlText));
            OnPropertyChanged(nameof(ExportPipelineXmlText));
            OnPropertyChanged(nameof(RecipeDetailText));
            OnPropertyChanged(nameof(DuplicateFromSampleText));
            OnPropertyChanged(nameof(PipelineListText));
            OnPropertyChanged(nameof(PipelineNameText));
            OnPropertyChanged(nameof(ActivatePipelineText));
            OnPropertyChanged(nameof(DuplicatePipelineText));
            OnPropertyChanged(nameof(RenamePipelineText));
            OnPropertyChanged(nameof(DeletePipelineText));
            OnPropertyChanged(nameof(SampleSourceText));
            OnPropertyChanged(nameof(SampleAcceptanceText));
            OnPropertyChanged(nameof(SampleCheckResultText));
            OnPropertyChanged(nameof(PairCheckResultText));
            OnPropertyChanged(nameof(RunSelectedSampleCheckText));
            OnPropertyChanged(nameof(RunSelectedSamplePairCheckText));
            OnPropertyChanged(nameof(SelectedSampleAcceptanceSummaryText));
            OnPropertyChanged(nameof(LlmXmlValidationReportText));
            OnPropertyChanged(nameof(PipelinePreviewStepListText));
            OnPropertyChanged(nameof(LlmAssistantText));
            OnPropertyChanged(nameof(LlmToolTemplateText));
            OnPropertyChanged(nameof(LlmInspectionGoalLabelText));
            OnPropertyChanged(nameof(LlmDetectionPointLabelText));
            OnPropertyChanged(nameof(BuildLlmPromptButtonText));
            OnPropertyChanged(nameof(CreateLlmTemplateXmlText));
            OnPropertyChanged(nameof(RefreshLlmDraftReviewText));
            OnPropertyChanged(nameof(LlmPromptPreviewText));
            OnPropertyChanged(nameof(LlmXmlDraftLabelText));
            OnPropertyChanged(nameof(LoadLlmXmlDraftText));
            OnPropertyChanged(nameof(ValidateLlmXmlDraftButtonText));
            OnPropertyChanged(nameof(ImportLlmXmlDraftText));
            OnPropertyChanged(nameof(UseSelectedSampleReferenceText));
            OnPropertyChanged(nameof(LlmReferenceImageText));
            OnPropertyChanged(nameof(LlmDraftValidationText));
            OnPropertyChanged(nameof(LlmDependencyReportText));
            OnPropertyChanged(nameof(LlmDraftReviewReportText));
            RefreshSampleOptions();
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
            UpdateSelectedRecipeSummary();
            RefreshCommandState();
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
                StatusText = "LLM XML draft load canceled.";
                return;
            }

            LoadLlmXmlDraftFromPath(path);
        }

        public bool LoadLlmXmlDraftFromPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
            {
                StatusText = "LLM XML draft file was not found.";
                return false;
            }

            LlmXmlDraftText = File.ReadAllText(path);
            StatusText = "Loaded LLM XML draft: " + Path.GetFileName(path);
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
                LlmXmlDraftReviewReport = "Draft review skipped: validation failed.";
                StatusText = "LLM XML draft is not importable.";
                return;
            }

            string recipeName = NormalizeRecipeName(selectedRecipeName);
            string basePipelineName = string.IsNullOrWhiteSpace(pipeline.Name)
                ? "LLM_Draft_" + DateTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture)
                : pipeline.Name.Trim();
            pipeline.Name = CreateUniquePipelineName(recipeName, basePipelineName);
            CopyReferenceImageForDraftImport(recipeName, pipeline.Name, ref dependencyReport);
            LlmXmlDraftReviewReport = BuildLlmDraftReviewReport(pipeline);
            VisionPipelineStorage.Save(recipeName, pipeline);
            VisionPipelineStorage.SaveActivePipelineName(recipeName, pipeline.Name);
            LlmXmlDraftValidationReport = validationReport;
            LlmXmlDraftDependencyReport = dependencyReport;
            StatusText = "Imported LLM XML draft: " + pipeline.Name;
            RefreshPipelineOptions(pipeline.Name);
            RefreshOptions();
            refreshAfterSwitch();
        }

        private void UseSelectedSampleReference()
        {
            if (SelectedSampleOption?.Sample == null || string.IsNullOrWhiteSpace(SelectedSampleOption.Sample.ImageFullPath))
            {
                StatusText = "No selected sample image is available.";
                return;
            }

            LlmReferenceImagePath = SelectedSampleOption.Sample.ImageFullPath;
            StatusText = "Reference image set from sample: " + SelectedSampleOption.Sample.SampleName;
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
            StatusText = "Running sample check: " + sampleOption.SampleName;
            RefreshCommandState();

            try
            {
                string pipelineXmlText = File.ReadAllText(pipelinePath);
                VisionPipelineSampleCheckResult result =
                    await VisionPipelineSampleCheckService.RunSampleCheckSafeAsync(sampleOption.Sample, pipelineXmlText);
                LatestSampleRunSummary = OpenVisionRecipeSampleRunSummary.FromResult(sampleOption, pipelineName, result);
                StatusText = "Sample check " + result.Status + ": " + sampleOption.SampleName;
            }
            catch (Exception ex)
            {
                VisionPipelineSampleCheckResult result = VisionPipelineSampleCheckService.CreateErrorResult(
                    ex.GetBaseException().Message);
                LatestSampleRunSummary = OpenVisionRecipeSampleRunSummary.FromResult(sampleOption, pipelineName, result);
                StatusText = "Sample check ERROR: " + result.Message;
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
            StatusText = "Running Good/Bad pair check: " + sampleOption.Sample.PairGroup;
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
                StatusText = LatestPairRunSummary.StatusText + ": " + sampleOption.Sample.PairGroup;
            }
            catch (Exception ex)
            {
                LatestPairRunSummary = OpenVisionRecipePairRunSummary.FromError(
                    sampleOption,
                    pipelineName,
                    ex.GetBaseException().Message);
                StatusText = "Pair check ERROR: " + ex.GetBaseException().Message;
            }
            finally
            {
                isPairCheckRunning = false;
                OnPropertyChanged(nameof(RunSelectedSamplePairCheckText));
                RefreshCommandState();
            }
        }

        private void BuildLlmPrompt()
        {
            LlmPromptText = BuildLlmPromptText();
            StatusText = "Built LLM prompt from current recipe context.";
        }

        private void CreateLlmTemplateXmlDraft()
        {
            VisionPipeline pipeline = CreateLlmTemplatePipeline();
            LlmPromptText = BuildLlmPromptText();
            LlmXmlDraftText = SerializePipelineToXmlText(pipeline);
            ValidateLlmXmlDraftText(false);
            StatusText = "Created XML starter from LLM template: " + SelectedLlmToolTemplate;
        }

        private void RefreshLlmDraftReview()
        {
            ValidateLlmXmlDraftText(false);
        }

        public void CreateLlmTemplateXmlDraftForTest()
        {
            CreateLlmTemplateXmlDraft();
        }

        private bool ValidateLlmXmlDraftText(bool copyDependencies)
        {
            bool ok = TryBuildLlmDraftPipeline(copyDependencies, out VisionPipeline pipeline, out string validationReport, out string dependencyReport);
            LlmXmlDraftValidationReport = validationReport;
            LlmXmlDraftDependencyReport = dependencyReport;
            LlmXmlDraftReviewReport = ok ? BuildLlmDraftReviewReport(pipeline) : "Draft review skipped: validation failed.";
            StatusText = ok ? "LLM XML draft validation OK." : "LLM XML draft validation NG.";
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
                "Never overwrite the input layer. Read from Main unless a previous step output is intentionally used.",
                "Do not run Preview/Run automatically. The user will validate and import the XML explicitly.",
                "Recipe: " + recipeName,
                "Current active pipeline: " + activePipelineName,
                "Preferred tool template: " + SelectedLlmToolTemplate,
                "Template guidance: " + ResolveTemplateGuidance(SelectedLlmToolTemplate),
                "Reference image: " + referenceImage,
                "Inspection goal: " + goal,
                "Detection points: " + detectionPoints,
                "Required response: return only a VisionPipeline XML document that can be loaded by OpenVisionLab."
            });
        }

        private VisionPipeline CreateLlmTemplatePipeline()
        {
            string template = SelectedLlmToolTemplate ?? string.Empty;
            string pipelineName = "LLM_Starter_" + SanitizePathSegment(template.Replace("+", "And").Replace(" ", string.Empty));
            VisionPipeline pipeline = new VisionPipeline { Name = pipelineName };

            if (template.IndexOf("Blob", StringComparison.OrdinalIgnoreCase) >= 0)
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

            if (template.IndexOf("Edge", StringComparison.OrdinalIgnoreCase) >= 0)
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

            if (template.IndexOf("Line", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                VisionPipelineStep step = CreateDraftStep("Line_Measure", "Line", "Main", "Line_Result");
                step.Parameters["CONTRAST"] = "20";
                step.Parameters["THICKNESS"] = "5";
                step.Parameters["SAMPLING_STEP"] = "1";
                pipeline.Steps.Add(step);
                return pipeline;
            }

            if (template.IndexOf("Mean", StringComparison.OrdinalIgnoreCase) >= 0)
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
            string value = template ?? string.Empty;
            if (value.IndexOf("Blob", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return "Use Threshold to isolate the foreground, then Blob to measure area/count/position.";
            }

            if (value.IndexOf("Edge", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return "Use EdgeBasedMatching when contour shape is more reliable than raw intensity.";
            }

            if (value.IndexOf("Line", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return "Use Line/LineDistance style outputs for edge or gap measurement points.";
            }

            if (value.IndexOf("Mean", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return "Use Mean when the judgment is based on brightness or region intensity.";
            }

            return "Use Matching when a stable template image and score threshold define the target.";
        }

        private string BuildLlmDraftReviewReport(VisionPipeline draftPipeline)
        {
            if (draftPipeline == null)
            {
                return "Draft import review: NG - pipeline is null.";
            }

            string recipeName = NormalizeRecipeName(selectedRecipeName);
            string activePipelineName = VisionPipelineStorage.LoadActivePipelineName(
                recipeName,
                VisionPipelineAppendService.DefaultPipelineName);
            VisionPipeline activePipeline = VisionPipelineStorage.Load(recipeName, activePipelineName);
            List<string> lines = new List<string>
            {
                "Draft import review: READY",
                "Import action: save as a new/unique pipeline, activate it, do not run Preview.",
                "Current active: " + FormatPipelineHeader(activePipeline),
                "Draft: " + FormatPipelineHeader(draftPipeline),
                "Step count delta: " + FormatSignedNumber((draftPipeline.Steps?.Count ?? 0) - (activePipeline.Steps?.Count ?? 0)),
                "Draft dependency paths: " + CountDependencyParameters(draftPipeline).ToString(CultureInfo.InvariantCulture)
            };

            int activeCount = activePipeline?.Steps?.Count ?? 0;
            int draftCount = draftPipeline?.Steps?.Count ?? 0;
            int compareCount = Math.Min(Math.Max(activeCount, draftCount), 6);
            for (int index = 0; index < compareCount; index++)
            {
                VisionPipelineStep activeStep = index < activeCount ? activePipeline.Steps[index] : null;
                VisionPipelineStep draftStep = index < draftCount ? draftPipeline.Steps[index] : null;
                lines.Add("Step " + (index + 1).ToString(CultureInfo.InvariantCulture) + ": " + FormatStepDiff(activeStep, draftStep));
            }

            if (Math.Max(activeCount, draftCount) > compareCount)
            {
                lines.Add("More steps omitted from review: " + (Math.Max(activeCount, draftCount) - compareCount).ToString(CultureInfo.InvariantCulture));
            }

            return string.Join(Environment.NewLine, lines);
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
                + " step(s)";
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
                return "New -> " + FormatStepBrief(draftStep);
            }

            if (draftStep == null)
            {
                return "Removed from draft -> " + FormatStepBrief(activeStep);
            }

            List<string> changes = new List<string>();
            if (!string.Equals(activeStep.ToolType, draftStep.ToolType, StringComparison.OrdinalIgnoreCase))
            {
                changes.Add("tool " + FormatValue(activeStep.ToolType) + " -> " + FormatValue(draftStep.ToolType));
            }

            string activeRoute = FormatRoute(activeStep);
            string draftRoute = FormatRoute(draftStep);
            if (!string.Equals(activeRoute, draftRoute, StringComparison.OrdinalIgnoreCase))
            {
                changes.Add("route " + activeRoute + " -> " + draftRoute);
            }

            if (!string.Equals(activeStep.Name, draftStep.Name, StringComparison.OrdinalIgnoreCase))
            {
                changes.Add("name " + FormatValue(activeStep.Name) + " -> " + FormatValue(draftStep.Name));
            }

            return changes.Count == 0
                ? "No structural change -> " + FormatStepBrief(draftStep)
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
            validationLines.Add("LLM draft validation: WAIT");

            string xmlText = LlmXmlDraftText ?? string.Empty;
            if (string.IsNullOrWhiteSpace(xmlText))
            {
                validationLines[0] = "LLM draft validation: NG";
                validationLines.Add("XML text is empty.");
                validationReport = string.Join(Environment.NewLine, validationLines);
                dependencyReport = "Dependency scan skipped.";
                return false;
            }

            if (!TryValidateXmlSyntax(xmlText, validationLines))
            {
                validationReport = string.Join(Environment.NewLine, validationLines);
                dependencyReport = "Dependency scan skipped.";
                return false;
            }

            if (!SerializeHelper.TryLoadFromXmlText(xmlText, out pipeline, out string deserializeMessage) || pipeline == null)
            {
                validationLines[0] = "LLM draft validation: NG";
                validationLines.Add("OpenVision pipeline deserialize: NG - " + deserializeMessage);
                validationReport = string.Join(Environment.NewLine, validationLines);
                dependencyReport = "Dependency scan skipped.";
                return false;
            }

            validationLines.Add("OpenVision pipeline deserialize: OK");
            validationLines.Add("Pipeline: " + (string.IsNullOrWhiteSpace(pipeline.Name) ? "-" : pipeline.Name));
            validationLines.Add("Steps: " + (pipeline.Steps?.Count ?? 0).ToString(CultureInfo.InvariantCulture));
            VisionPipelineValidationResult validation = VisionPipelineValidator.Validate(pipeline, new[] { "Main" });
            validationLines.Add(string.Format(
                CultureInfo.CurrentCulture,
                "Schema/routing: {0} / Errors: {1} / Warnings: {2}",
                validation.Success ? "OK" : "NG",
                validation.Errors.Count,
                validation.Warnings.Count));

            foreach (string error in validation.Errors.Take(4))
            {
                validationLines.Add("Error: " + error);
            }

            foreach (string warning in validation.Warnings.Take(4))
            {
                validationLines.Add("Warning: " + warning);
            }

            if (!string.IsNullOrWhiteSpace(LlmReferenceImagePath))
            {
                validationLines.Add(File.Exists(LlmReferenceImagePath)
                    ? "Reference image: OK - " + LlmReferenceImagePath
                    : "Reference image: missing - " + LlmReferenceImagePath);
            }

            dependencyReport = BuildDependencyReport(pipeline, NormalizeRecipeName(selectedRecipeName), copyDependencies);
            validationLines[0] = validation.Success ? "LLM draft validation: OK" : "LLM draft validation: NG";
            validationReport = string.Join(Environment.NewLine, validationLines);
            return validation.Success;
        }

        private static bool TryValidateXmlSyntax(string xmlText, ICollection<string> validationLines)
        {
            try
            {
                XDocument.Parse(xmlText, LoadOptions.SetLineInfo);
                validationLines.Add("XML syntax: OK");
                return true;
            }
            catch (XmlException ex)
            {
                validationLines.Clear();
                validationLines.Add("LLM draft validation: NG");
                validationLines.Add(string.Format(
                    CultureInfo.CurrentCulture,
                    "XML syntax: NG at line {0}, position {1}: {2}",
                    ex.LineNumber,
                    ex.LinePosition,
                    ex.Message));
                return false;
            }
        }

        private string BuildDependencyReport(VisionPipeline pipeline, string recipeName, bool copyDependencies)
        {
            if (pipeline?.Steps == null || pipeline.Steps.Count == 0)
            {
                return "Dependency scan skipped: pipeline has no steps.";
            }

            List<string> lines = new List<string>
            {
                copyDependencies ? "Dependency copy report" : "Dependency scan report"
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
                        lines.Add(string.Format(
                            CultureInfo.CurrentCulture,
                            "Missing: {0}.{1} -> {2}",
                            step.Name,
                            key,
                            value));
                        continue;
                    }

                    if (!copyDependencies)
                    {
                        lines.Add(string.Format(
                            CultureInfo.CurrentCulture,
                            "Found: {0}.{1} -> {2}",
                            step.Name,
                            key,
                            sourcePath));
                        continue;
                    }

                    string copiedPath = CopyDependencyToRecipe(recipeName, sourcePath);
                    step.Parameters[key] = copiedPath;
                    copied++;
                    lines.Add(string.Format(
                        CultureInfo.CurrentCulture,
                        "Copied: {0}.{1} -> {2}",
                        step.Name,
                        key,
                        copiedPath));
                }
            }

            if (found == 0)
            {
                lines.Add("No external image/template dependencies detected.");
            }

            lines.Add(string.Format(
                CultureInfo.CurrentCulture,
                "Summary: detected={0}, copied={1}, missing={2}",
                found,
                copied,
                missing));
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
                ? "Reference image copied: " + targetPath
                : dependencyReport + Environment.NewLine + "Reference image copied: " + targetPath;
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
                StatusText = "No pipeline selected.";
                return;
            }

            string recipeName = NormalizeRecipeName(selectedRecipeName);
            VisionPipelineStorage.SaveActivePipelineName(recipeName, option.PipelineName);
            StatusText = string.Format(CultureInfo.CurrentCulture, "Active pipeline: {0}", option.PipelineName);
            RefreshPipelineOptions(option.PipelineName);
            refreshAfterSwitch();
        }

        private void DuplicateSelectedPipeline()
        {
            OpenVisionRecipePipelineOption option = SelectedPipelineOption;
            if (!CanUseSelectedPipeline())
            {
                StatusText = "No pipeline selected.";
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
                StatusText = "Cannot rename this pipeline.";
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
                StatusText = "Cannot delete this pipeline.";
                return;
            }

            string recipeName = NormalizeRecipeName(selectedRecipeName);
            if (!confirmDeletePipeline(recipeName, option.PipelineName))
            {
                StatusText = "Pipeline delete canceled.";
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
                StatusText = "Select a sample pipeline first.";
                return;
            }

            DuplicatePipelineFromSampleOption(SelectedSampleOption);
        }

        public bool DuplicatePipelineFromSampleOption(OpenVisionRecipeSampleOption sampleOption)
        {
            string recipeName = NormalizeRecipeName(selectedRecipeName);
            if (!CanUseSelectedRecipe())
            {
                StatusText = "No recipe selected.";
                return false;
            }

            if (sampleOption == null || string.IsNullOrWhiteSpace(sampleOption.PipelinePath))
            {
                StatusText = "Sample pipeline is not available.";
                return false;
            }

            if (!VisionPipelineStorage.TryLoadFromFile(sampleOption.PipelinePath, out VisionPipeline pipeline, out string message))
            {
                StatusText = "Sample pipeline load failed: " + message;
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
                "Duplicated sample pipeline: {0}",
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
            if (isSampleCheckRunning || !CanUseSelectedPipeline() || SelectedSampleOption?.Sample == null)
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
            if (isPairCheckRunning || isSampleCheckRunning || !CanUseSelectedPipeline() || SelectedSampleOption?.Sample == null)
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
            UpdateSelectedRecipeSummary();
            RefreshCommandState();
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
            IReadOnlyList<OpenVisionRecipePipelineStepPreview> previewSteps = BuildPipelinePreviewSteps(activePipeline);
            string updatedText = lastWriteTime.HasValue
                ? lastWriteTime.Value.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.CurrentCulture)
                : "-";
            string detail = string.Join(
                Environment.NewLine,
                string.Format(CultureInfo.CurrentCulture, LocalText("활성 파이프라인: {0}", "Active pipeline: {0}"), activePipelineName),
                string.Format(CultureInfo.CurrentCulture, LocalText("파이프라인 수: {0}", "Pipelines: {0}"), pipelineNames.Length),
                string.Format(CultureInfo.CurrentCulture, LocalText("Step 수: {0}", "Steps: {0}"), stepCount),
                string.Format(CultureInfo.CurrentCulture, "XML: {0}", xmlOk ? "OK" : "NG - " + xmlMessage),
                string.Format(CultureInfo.CurrentCulture, LocalText("수정: {0}", "Updated: {0}"), updatedText),
                string.Format(CultureInfo.CurrentCulture, LocalText("경로: {0}", "Path: {0}"), pipelinePath));

            detail = string.Format(CultureInfo.CurrentCulture, "Selected pipeline: {0}", previewPipelineName)
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
                "LLM XML validation: " + (xmlOk ? "OK" : "NG"),
                "XML load: " + (xmlOk ? "OK" : xmlMessage),
                "Assumed source layer: Main"
            };

            if (!xmlOk || activePipeline == null)
            {
                lines.Add("Action: ask the LLM to output OpenVisionLab VisionPipeline XML with supported ToolType, InputLayer, OutputLayer, and Parameters.");
                return string.Join(Environment.NewLine, lines);
            }

            string filePipelineName = Path.GetFileNameWithoutExtension(pipelinePath) ?? string.Empty;
            lines.Add(string.Format(
                CultureInfo.CurrentCulture,
                "Pipeline: {0} / Steps: {1}",
                string.IsNullOrWhiteSpace(activePipeline.Name) ? "-" : activePipeline.Name,
                activePipeline.Steps.Count));

            if (!string.Equals(filePipelineName, activePipeline.Name ?? string.Empty, StringComparison.OrdinalIgnoreCase))
            {
                lines.Add(string.Format(
                    CultureInfo.CurrentCulture,
                    "Warning: XML Name '{0}' differs from file '{1}'.",
                    activePipeline.Name ?? string.Empty,
                    filePipelineName));
            }

            VisionPipelineValidationResult validation = VisionPipelineValidator.Validate(activePipeline, new[] { "Main" });
            lines.Add(string.Format(
                CultureInfo.CurrentCulture,
                "Schema/routing: {0} / Errors: {1} / Warnings: {2}",
                validation.Success ? "OK" : "NG",
                validation.Errors.Count,
                validation.Warnings.Count));

            foreach (string error in validation.Errors.Take(4))
            {
                lines.Add("Error: " + error);
            }

            if (validation.Errors.Count > 4)
            {
                lines.Add("Error: +" + (validation.Errors.Count - 4).ToString(CultureInfo.InvariantCulture) + " more");
            }

            foreach (string warning in validation.Warnings.Take(4))
            {
                lines.Add("Warning: " + warning);
            }

            if (validation.Warnings.Count > 4)
            {
                lines.Add("Warning: +" + (validation.Warnings.Count - 4).ToString(CultureInfo.InvariantCulture) + " more");
            }

            return string.Join(Environment.NewLine, lines);
        }

        private static IReadOnlyList<OpenVisionRecipePipelineStepPreview> BuildPipelinePreviewSteps(VisionPipeline pipeline)
        {
            if (pipeline?.Steps == null || pipeline.Steps.Count == 0)
            {
                return Array.Empty<OpenVisionRecipePipelineStepPreview>();
            }

            List<OpenVisionRecipePipelineStepPreview> steps = new List<OpenVisionRecipePipelineStepPreview>();
            for (int i = 0; i < pipeline.Steps.Count; i++)
            {
                VisionPipelineStep step = pipeline.Steps[i];
                steps.Add(new OpenVisionRecipePipelineStepPreview(i + 1, step));
            }

            return steps;
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
            return OpenVisionLanguageService.CurrentLanguage == OpenVisionLanguage.Korean ? korean : english;
        }
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

        public IReadOnlyList<OpenVisionRecipePipelineStepPreview> PipelinePreviewSteps { get; }

        public string HeaderText =>
            "Selected pipeline: " + (string.IsNullOrWhiteSpace(PreviewPipelineName) ? "-" : PreviewPipelineName);

        public string ActivePipelineDisplay =>
            "Active: " + (string.IsNullOrWhiteSpace(ActivePipelineName) ? "-" : ActivePipelineName);

        public string PipelineCountDisplay =>
            "Pipelines " + PipelineCount.ToString(CultureInfo.InvariantCulture);

        public string StepCountDisplay =>
            "Steps " + StepCount.ToString(CultureInfo.InvariantCulture);

        public string XmlStatusDisplay => XmlValid ? "XML OK" : "XML NG";

        public string PipelinePreviewStepListDisplay =>
            "Pipeline preview step list (" + PipelinePreviewSteps.Count.ToString(CultureInfo.InvariantCulture) + ")";

        public string OperatorReviewText
        {
            get
            {
                if (!XmlValid)
                {
                    return "XML needs review before run.";
                }

                if (StepCount <= 0)
                {
                    return "Add a pipeline step before review.";
                }

                if (!string.Equals(ActivePipelineName, PreviewPipelineName, StringComparison.OrdinalIgnoreCase))
                {
                    return "Activate this pipeline or choose the active pipeline.";
                }

                return "Review ready: run with a sample image, then check output layers.";
            }
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
            (IsActive ? "[ACTIVE] " : string.Empty)
            + PipelineName;

        public string DetailText =>
            StepCount.ToString(CultureInfo.InvariantCulture)
            + " step(s) | "
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
                ? "XML OK / Route OK"
                : "XML OK / Route NG " + validation.Errors.Count.ToString(CultureInfo.InvariantCulture);
            return new OpenVisionRecipePipelineOption(
                name,
                isActive,
                pipeline?.Steps?.Count ?? 0,
                true,
                validation.Success,
                status);
        }
    }

    public sealed class OpenVisionRecipePipelineStepPreview
    {
        internal OpenVisionRecipePipelineStepPreview(int index, VisionPipelineStep step)
        {
            Index = index;
            Name = string.IsNullOrWhiteSpace(step?.Name) ? "Step " + index.ToString(CultureInfo.InvariantCulture) : step.Name.Trim();
            ToolType = string.IsNullOrWhiteSpace(step?.ToolType) ? "-" : step.ToolType.Trim();
            InputLayer = string.IsNullOrWhiteSpace(step?.InputLayer) ? "-" : step.InputLayer.Trim();
            OutputLayer = string.IsNullOrWhiteSpace(step?.OutputLayer) ? "-" : step.OutputLayer.Trim();
            ParameterCount = step?.Parameters?.Count ?? 0;
            IsEnabled = step?.Enabled ?? false;
            AcceptanceText = ResolveAcceptanceText(step);
        }

        public int Index { get; }

        public string Name { get; }

        public string ToolType { get; }

        public string InputLayer { get; }

        public string OutputLayer { get; }

        public int ParameterCount { get; }

        public bool IsEnabled { get; }

        public string AcceptanceText { get; }

        public string DisplayText =>
            Index.ToString(CultureInfo.InvariantCulture) + ". "
            + (IsEnabled ? "[ON] " : "[OFF] ")
            + Shorten(Name, 42)
            + " / "
            + ToolType;

        public string DetailText =>
            Shorten(InputLayer, 32)
            + " -> "
            + Shorten(OutputLayer, 32)
            + " | Params "
            + ParameterCount.ToString(CultureInfo.InvariantCulture)
            + AcceptanceText;

        public string FullDetailText =>
            InputLayer
            + " -> "
            + OutputLayer
            + " | Params "
            + ParameterCount.ToString(CultureInfo.InvariantCulture)
            + AcceptanceText;

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
                ? " | Accept " + metric
                : " | Accept " + metric + " " + string.Join(" ", gates);
        }
    }

    public sealed class OpenVisionRecipeSampleRunSummary
    {
        public static OpenVisionRecipeSampleRunSummary Empty { get; } = new OpenVisionRecipeSampleRunSummary(
            "Not run yet.",
            "Select a sample and run an explicit check.",
            false);

        private OpenVisionRecipeSampleRunSummary(string statusText, string detailText, bool hasResult, string compactText = null)
        {
            StatusText = statusText ?? string.Empty;
            DetailText = detailText ?? string.Empty;
            HasResult = hasResult;
            CompactText = string.IsNullOrWhiteSpace(compactText) ? StatusText : compactText.Trim();
        }

        public string StatusText { get; }

        public string DetailText { get; }

        public bool HasResult { get; }

        public string CompactText { get; }

        public string DisplayText => StatusText + Environment.NewLine + DetailText;

        public static OpenVisionRecipeSampleRunSummary CreatePending(OpenVisionRecipeSampleOption sampleOption)
        {
            if (sampleOption == null)
            {
                return Empty;
            }

            return new OpenVisionRecipeSampleRunSummary(
                "Not run yet.",
                "Ready to run selected sample: " + sampleOption.SampleName,
                false,
                "Ready: " + sampleOption.SampleName);
        }

        public static OpenVisionRecipeSampleRunSummary CreateRunning(
            OpenVisionRecipeSampleOption sampleOption,
            string pipelineName)
        {
            return new OpenVisionRecipeSampleRunSummary(
                "Running sample check...",
                FormatSampleAndPipeline(sampleOption, pipelineName),
                false,
                "Running: " + (string.IsNullOrWhiteSpace(sampleOption?.SampleName) ? "-" : sampleOption.SampleName));
        }

        internal static OpenVisionRecipeSampleRunSummary FromResult(
            OpenVisionRecipeSampleOption sampleOption,
            string pipelineName,
            VisionPipelineSampleCheckResult result)
        {
            if (result == null)
            {
                return new OpenVisionRecipeSampleRunSummary(
                    "Sample check ERROR",
                    FormatSampleAndPipeline(sampleOption, pipelineName),
                    true,
                    "Sample check ERROR");
            }

            string status = string.IsNullOrWhiteSpace(result.Status) ? "-" : result.Status;
            string metric = string.IsNullOrWhiteSpace(result.MetricText) ? "-" : result.MetricText;
            List<string> lines = new List<string>
            {
                FormatSampleAndPipeline(sampleOption, pipelineName),
                "Metric: " + metric,
                "Action: " + (string.IsNullOrWhiteSpace(result.ActionSummaryText) ? "-" : result.ActionSummaryText)
            };

            if (!string.IsNullOrWhiteSpace(result.FailedStepText))
            {
                lines.Add("Failed step: " + result.FailedStepText);
            }

            if (!string.IsNullOrWhiteSpace(result.Message))
            {
                lines.Add("Message: " + result.Message);
            }

            string compact = "Sample check " + status + " | " + metric;
            if (!result.Success && !string.IsNullOrWhiteSpace(result.FailedStepText))
            {
                compact += " | " + result.FailedStepText;
            }

            return new OpenVisionRecipeSampleRunSummary(
                "Sample check " + status,
                string.Join(Environment.NewLine, lines),
                true,
                compact);
        }

        private static string FormatSampleAndPipeline(OpenVisionRecipeSampleOption sampleOption, string pipelineName)
        {
            string sample = string.IsNullOrWhiteSpace(sampleOption?.SampleName) ? "-" : sampleOption.SampleName;
            string pipeline = string.IsNullOrWhiteSpace(pipelineName) ? "-" : pipelineName;
            return "Sample: " + sample + " / Pipeline: " + pipeline;
        }
    }

    public sealed class OpenVisionRecipePairRunSummary
    {
        public static OpenVisionRecipePairRunSummary Empty { get; } = new OpenVisionRecipePairRunSummary(
            "Pair check not run.",
            "Select a Good/Bad sample pair and run an explicit pair check.",
            false);

        private OpenVisionRecipePairRunSummary(string statusText, string detailText, bool hasResult, string compactText = null)
        {
            StatusText = statusText ?? string.Empty;
            DetailText = detailText ?? string.Empty;
            HasResult = hasResult;
            CompactText = string.IsNullOrWhiteSpace(compactText) ? StatusText : compactText.Trim();
        }

        public string StatusText { get; }

        public string DetailText { get; }

        public bool HasResult { get; }

        public string CompactText { get; }

        public string DisplayText => StatusText + Environment.NewLine + DetailText;

        public static OpenVisionRecipePairRunSummary CreatePending(OpenVisionRecipeSampleOption sampleOption)
        {
            if (sampleOption?.Sample == null || string.IsNullOrWhiteSpace(sampleOption.Sample.PairGroup))
            {
                return Empty;
            }

            return new OpenVisionRecipePairRunSummary(
                "Pair check not run.",
                "Ready to run PairGroup: " + sampleOption.Sample.PairGroup,
                false,
                "Ready: " + sampleOption.Sample.PairGroup);
        }

        public static OpenVisionRecipePairRunSummary CreateRunning(
            OpenVisionRecipeSampleOption sampleOption,
            string pipelineName,
            int sampleCount)
        {
            string group = string.IsNullOrWhiteSpace(sampleOption?.Sample?.PairGroup) ? "-" : sampleOption.Sample.PairGroup.Trim();
            return new OpenVisionRecipePairRunSummary(
                "Running Good/Bad pair check...",
                "PairGroup: " + group + " / Pipeline: " + (string.IsNullOrWhiteSpace(pipelineName) ? "-" : pipelineName),
                false,
                "Running: " + group + " (" + sampleCount.ToString(CultureInfo.InvariantCulture) + " samples)");
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
            string compact = "Pair check " + (ok ? "OK" : "NG")
                + " | " + pass.ToString(CultureInfo.InvariantCulture)
                + "/" + total.ToString(CultureInfo.InvariantCulture)
                + " pass";

            if (resultList.Count > 0)
            {
                compact += " | " + string.Join(" | ", resultList.Select(result => result.CompactText));
            }

            List<string> lines = new List<string>
            {
                "PairGroup: " + group,
                "Pipeline: " + (string.IsNullOrWhiteSpace(pipelineName) ? "-" : pipelineName),
                "Pass: " + pass.ToString(CultureInfo.InvariantCulture) + "/" + total.ToString(CultureInfo.InvariantCulture)
            };
            lines.AddRange(resultList.Select(result => result.DisplayText));
            if (!string.IsNullOrWhiteSpace(summaryPath))
            {
                lines.Add("Saved summary: " + summaryPath);
            }

            return new OpenVisionRecipePairRunSummary(
                "Pair check " + (ok ? "OK" : "NG"),
                string.Join(Environment.NewLine, lines),
                true,
                compact);
        }

        internal static OpenVisionRecipePairRunSummary FromError(
            OpenVisionRecipeSampleOption sampleOption,
            string pipelineName,
            string message)
        {
            string group = string.IsNullOrWhiteSpace(sampleOption?.Sample?.PairGroup) ? "-" : sampleOption.Sample.PairGroup.Trim();
            return new OpenVisionRecipePairRunSummary(
                "Pair check ERROR",
                "PairGroup: " + group
                + Environment.NewLine
                + "Pipeline: " + (string.IsNullOrWhiteSpace(pipelineName) ? "-" : pipelineName)
                + Environment.NewLine
                + "Message: " + (message ?? string.Empty),
                true,
                "Pair check ERROR | " + group);
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
            string message)
        {
            Role = string.IsNullOrWhiteSpace(role) ? "Sample" : role.Trim();
            SampleName = sampleName ?? string.Empty;
            Status = status ?? string.Empty;
            Success = success;
            MetricText = metricText ?? string.Empty;
            Message = message ?? string.Empty;
        }

        public string Role { get; }

        public string SampleName { get; }

        public string Status { get; }

        public bool Success { get; }

        public string MetricText { get; }

        public string Message { get; }

        public string CompactText =>
            Role + " " + (string.IsNullOrWhiteSpace(Status) ? "-" : Status);

        public string DisplayText =>
            Role + ": "
            + (string.IsNullOrWhiteSpace(SampleName) ? "-" : SampleName)
            + " / "
            + (string.IsNullOrWhiteSpace(Status) ? "-" : Status)
            + " / "
            + (string.IsNullOrWhiteSpace(MetricText) ? "-" : MetricText)
            + (string.IsNullOrWhiteSpace(Message) ? string.Empty : " / " + Message);

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
                result?.Message);
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
                "Sample: " + sample.SampleName,
                "Mode: " + (string.IsNullOrWhiteSpace(sample.ValidationMode) ? "-" : sample.ValidationMode.Trim()),
                "Expected: " + (string.IsNullOrWhiteSpace(sample.ExpectedText) ? "-" : sample.ExpectedText)
            };

            if (sample.HasPair)
            {
                lines.Add("Pair: " + sample.PairText);
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
    }
}
