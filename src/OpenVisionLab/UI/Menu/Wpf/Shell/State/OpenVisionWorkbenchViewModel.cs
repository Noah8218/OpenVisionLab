using MahApps.Metro.IconPacks;
using OpenVisionLab.Mvvm;
using OpenVisionLab.Vision2D.Pipeline;
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace OpenVisionLab
{
    public sealed class OpenVisionWorkbenchViewModel : ObservableObject, IDisposable
    {
        private static readonly (string Key, string TextKey)[] shortcutDefinitions =
        {
            ("F5", "Workbench.Command.RunPipelineCurrent"),
            ("Ctrl+F5", "Workbench.Command.RunToStep"),
            ("F8", "Workbench.Shortcuts.RunToolOrStep"),
            ("F6", "Workbench.Command.BatchAll"),
            ("Shift+F5", "Workbench.Shortcuts.Stop"),
            ("Ctrl+S", "Workbench.Menu.SaveRecipe"),
            ("Ctrl+Shift+S", "Workbench.Menu.SaveAs"),
            ("Ctrl+O", "Workbench.Menu.RecipeManager"),
            ("Ctrl+N", "Workbench.Menu.NewRecipe"),
            ("Ctrl+I", "Workbench.Menu.OpenImage"),
            ("Ctrl+A", "Workbench.Shortcuts.SelectImages"),
            ("Ctrl+Enter", "Workbench.Shortcuts.AddTool"),
            ("PgUp / PgDn", "Workbench.Shortcuts.PreviousNext"),
            ("1 · 2 · 4", "Workbench.Shortcuts.Split"),
            ("F2", "Workbench.Shortcuts.Rename"),
            ("Ctrl+D", "Workbench.Shortcuts.Duplicate"),
            ("Ctrl+E", "Workbench.Shortcuts.Enable"),
            ("Del", "Workbench.Shortcuts.Delete"),
            ("Alt+↑ / ↓", "Workbench.Shortcuts.Move"),
            ("Ctrl+Z / Ctrl+Y", "Workbench.Shortcuts.UndoRedo"),
            ("Esc", "Workbench.Shortcuts.Escape"),
            ("Ctrl+1 / 2 / 3", "Workbench.Shortcuts.Panels"),
            ("F1", "Workbench.Menu.Guide"),
            ("Ctrl+/", "Workbench.Shortcuts.Show"),
        };
        private readonly Func<SystemStateConfig, bool> savePreferences;
        private SystemStateConfig preferenceSnapshot;
        private string[] recentRecipeNames = Array.Empty<string>();
        private bool recentRecipeSaveFailed;
        private bool isShortcutsOpen;
        private bool isImageEditOpen;
        private bool isLayerManagerOpen;
        private bool isMainLlmOpen;
        private bool isMainLlmSaving;
        private int mainLlmStage = 1;
        private string mainLlmRecipe = string.Empty, mainLlmPipeline = string.Empty, mainLlmName = string.Empty, mainLlmError = string.Empty;
        private byte[] mainLlmWorking = Array.Empty<byte>();
        private object mainLlmEditor, mainLlmImage;
        private bool mainLlmIncludePipeline = true, mainLlmIncludeImage = true, mainLlmOpenSaved;
        private bool isMainHistoryOpen;
        private IReadOnlyList<OpenVisionRecipeBatchRunOption> mainHistoryRows = Array.Empty<OpenVisionRecipeBatchRunOption>();
        private string mainHistoryError = string.Empty;
        private bool isMainValidationOpen;
        private bool isMainValidationSave;
        private string mainValidationName = string.Empty;
        private string mainValidationError = string.Empty;
        private string mainValidationRecipe = string.Empty;
        private IReadOnlyList<OpenVisionRecipeValidationSetOption> mainValidationSets = Array.Empty<OpenVisionRecipeValidationSetOption>();
        private string mainNoticeKey = string.Empty;
        private string mainNoticeArgument = string.Empty;
        private bool isMainRecipeOpen;
        private bool isMainRecipeSaveAs;
        private bool isMainPipelineCreate;
        private bool isMainPipelineRename;
        private string mainPipelineName = string.Empty;
        private string mainPipelineRecipe = string.Empty;
        private bool isMainRecipeBusy;
        private bool mainRecipeCopyCurrent;
        private string mainRecipeName = string.Empty;
        private string mainRecipeDescription = string.Empty;
        private string mainRecipeFirstPipeline = string.Empty;
        private string mainRecipeErrorKey = string.Empty;
        private string mainRecipeErrorDetail = string.Empty;
        private double viewportWidth = 1600D;
        private double viewportHeight = 900D;
        private double viewerToolbarWidth;
        private double leftWidth;
        private double rightWidth;
        private double bottomHeight;
        private bool leftCollapsed;
        private bool rightCollapsed;
        private bool bottomCollapsed;
        private bool toolsCollapsed;
        private bool toolPanelWide;
        private bool hasPreferences;
        private bool autoRunEnabled = true;
        private string themeName = "Dark";
        private int selectedBottomTab;
        private string layoutStatusKey = string.Empty;
        private int mainNImageIndex, mainNImagePlanned, mainNImagePrefix;
        private string mainActionKey = "Workbench.StatusBar.Ready";
        private string mainActionKind = string.Empty;
        private string mainActionArgumentKey = string.Empty;
        private string mainActionArgument = string.Empty;
        private bool mainStopRequested;
        private int mainBatchIndex;
        private int mainBatchPlanned;
        private string mainBatchImageName = "";
        private int mainBatchOk;
        private int mainBatchNg;
        private int mainBatchError;
        private int mainImportAdded;
        private int mainImportFailed;
        private int mainImportNestedExcluded;
        private string mainActionResultKind = string.Empty;
        private string mainActionElapsed = string.Empty;
        private bool mainSingleRunning;
        private bool disposed;

        public OpenVisionWorkbenchViewModel(SystemStateConfig preferences, Func<SystemStateConfig, bool> savePreferences,
            OpenVisionShellPreviewViewModel shell = null, OpenVisionShellHostRecipeCommandSurface recipes = null,
            OpenVisionShellHostChromeCommandSurface chrome = null, OpenVisionShellHostWorkspaceCommandSurface workspace = null,
            OpenVisionShellHostLayerCommandSurface layers = null)
        {
            this.savePreferences = savePreferences ?? throw new ArgumentNullException(nameof(savePreferences));
            preferenceSnapshot = preferences ?? new SystemStateConfig();
            Shell = shell;
            Recipes = recipes;
            Chrome = chrome;
            Workspace = workspace;
            Layers = layers;
            Restore(preferenceSnapshot);
            ToggleLeftPanelCommand = new RelayCommand(() => IsLeftCollapsed = !IsLeftCollapsed);
            ToggleRightPanelCommand = new RelayCommand(() => IsRightCollapsed = !IsRightCollapsed);
            ToggleBottomPanelCommand = new RelayCommand(() => IsBottomCollapsed = !IsBottomCollapsed);
            ToggleToolPaletteCommand = new RelayCommand(() => IsToolPaletteCollapsed = !IsToolPaletteCollapsed);
            ToggleToolPanelWideCommand = new RelayCommand(() => { toolPanelWide = !toolPanelWide; RaiseLayout(); CommitLayout(); });
            ResetLayoutCommand = new RelayCommand(ResetLayout);
            CommitLayoutCommand = new RelayCommand(CommitLayout);
            UpdateViewportCommand = new RelayCommand<Size>(size => UpdateViewport(size.Width + 2D, size.Height + 42D));
            UpdateViewerToolbarSizeCommand = new RelayCommand<Size>(size => UpdateViewerToolbarSize(size.Width));
            SelectDarkThemeCommand = new RelayCommand(() => SetTheme("Dark"), () => !disposed);
            SelectLightThemeCommand = new RelayCommand(() => SetTheme("Light"), () => !disposed);
            OpenLogCommand = new RelayCommand(() => { IsBottomCollapsed = false; SelectedBottomTab = 2; });
            OpenLayerManagerCommand = new RelayCommand(() =>
            {
                if (!OpenLayerManagerCommand.CanExecute(null)) { return; }
                IsLayerManagerOpen = true; LayerManagerOpenRequested(this, EventArgs.Empty);
            }, () => !disposed && IsMainInteractive && Pipeline?.IsRunning != true && Layers != null);
            CloseLayerManagerCommand = new RelayCommand(() => IsLayerManagerOpen = false, () => !disposed && isLayerManagerOpen);
            OpenRecipeManagerCommand = new RelayCommand(() => RecipeManagerOpenRequested(this, EventArgs.Empty));
            FocusToolPaletteCommand = new RelayCommand(() => { IsLeftCollapsed = false; IsToolPaletteCollapsed = false; ToolPaletteFocusRequested(this, EventArgs.Empty); });
            OpenMainLearnCommand = new RelayCommand(OpenMainLearn, () => !disposed && IsMainInteractive && Chrome?.OpenLearnCommand.CanExecute(null) == true);
            OpenShortcutsCommand = new RelayCommand(() => IsShortcutsOpen = true, () => !disposed && IsMainInteractive);
            OpenMainNewPipelineCommand = new RelayCommand(() => OpenMainPipeline(false), () => !disposed && IsMainInteractive && Recipes?.CanCreateMainPipeline == true);
            OpenMainRenamePipelineCommand = new RelayCommand(() => OpenMainPipeline(true), () => !disposed && IsMainInteractive && Recipes?.CanRenameMainPipeline == true);
            DuplicateMainPipelineCommand = new RelayCommand(DuplicateMainPipeline, () => !disposed && IsMainInteractive && Recipes?.CanRenameMainPipeline == true);
            ExportMainPipelineXmlCommand = new RelayCommand(ExportMainPipelineXml, () => !disposed && IsMainInteractive && Recipes?.CanRenameMainPipeline == true);
            OpenMainNewRecipeCommand = new RelayCommand(() => OpenMainRecipe(false), () => CanOpenMainRecipe(false));
            OpenMainSaveAsCommand = new RelayCommand(() => OpenMainRecipe(true), () => CanOpenMainRecipe(true));
            CloseMainWindowCommand = new RelayCommand<CancelEventArgs>(args =>
            {
                if (isMainRecipeBusy) { args.Cancel = true; }
                else if (Pipeline?.CloseCommand.CanExecute(args) == true) { Pipeline.CloseCommand.Execute(args); }
            });
            CloseMainRecipeCommand = new RelayCommand(() => IsMainRecipeOpen = false, () => !disposed && isMainRecipeOpen && !isMainRecipeBusy);
            SubmitMainRecipeCommand = new RelayCommand(() => MainRecipeSubmission = SubmitMainRecipeAsync(), () => !disposed && isMainRecipeOpen && !isMainRecipeBusy);
            OpenMainLlmCommand = new RelayCommand(OpenMainLlm, () => !disposed && IsMainInteractive && Pipeline?.IsRunning != true && !string.IsNullOrWhiteSpace(Recipes?.MainRecipeName));
            CloseMainLlmCommand = new RelayCommand(() => IsMainLlmOpen = false, () => !disposed && isMainLlmOpen && !isMainLlmSaving);
            SelectMainLlmStageCommand = new RelayCommand<string>(stage => { if (int.TryParse(stage, out int value)) { MainLlmStage = Math.Clamp(value, 1, 3); } }, _ => !disposed && isMainLlmOpen && !isMainLlmSaving);
            NextMainLlmStageCommand = new RelayCommand(() => MainLlmStage = Math.Min(3, mainLlmStage + 1), () => !disposed && isMainLlmOpen && !isMainLlmSaving && mainLlmStage < 3);
            BuildMainLlmPromptCommand = new RelayCommand(BuildMainLlmPrompt, () => !disposed && isMainLlmOpen && MainLlmStillCurrent());
            ValidateMainLlmCommand = new RelayCommand(ValidateMainLlm, () => !disposed && isMainLlmOpen && MainLlmStillCurrent() && Recipes.ValidateLlmXmlDraftCommand.CanExecute(null));
            SaveMainLlmCommand = new RelayCommand(SaveMainLlm, () => !disposed && isMainLlmOpen && !isMainLlmSaving && mainLlmStage == 3 && MainLlmStillCurrent() && Recipes.IsLlmXmlDraftReady);
            OpenMainHistoryCommand = new RelayCommand(OpenMainHistory, () => !disposed && IsMainInteractive && Pipeline?.IsRunning != true && !string.IsNullOrWhiteSpace(Recipes?.MainRecipeName));
            CloseMainHistoryCommand = new RelayCommand(() => IsMainHistoryOpen = false, () => !disposed && isMainHistoryOpen);
            OpenMainValidationSaveCommand = new RelayCommand(() => OpenMainValidation(true), () => CanOpenMainValidation(true));
            OpenMainValidationLoadCommand = new RelayCommand(() => OpenMainValidation(false), () => CanOpenMainValidation(false));
            CloseMainValidationCommand = new RelayCommand(() => IsMainValidationOpen = false, () => !disposed && isMainValidationOpen);
            SaveMainValidationCommand = new RelayCommand(SaveMainValidation, () => !disposed && isMainValidationOpen && isMainValidationSave && MainValidationStillCurrent());
            LoadMainValidationCommand = new RelayCommand<OpenVisionRecipeValidationSetOption>(LoadMainValidation, option => !disposed && isMainValidationOpen && !isMainValidationSave && mainValidationSets.Contains(option) && MainValidationStillCurrent());
            CloseShortcutsCommand = new RelayCommand(() => IsShortcutsOpen = false, () => !disposed && isShortcutsOpen);
            ShowProductInfoCommand = new RelayCommand(() => ShowMainNotice(string.Empty, string.Empty), () => !disposed);
            UnavailableCommand = new RelayCommand(() => { }, () => false);
            OpenVisionLanguageService.LanguageChanged += OnLanguageChanged;
            if (Recipes != null) { Recipes.PropertyChanged += OnRecipeChanged; Recipes.MainRecipeOpened += OnMainRecipeOpened; }
            if (Shell != null) { Shell.PropertyChanged += OnMainInputChanged; }
            if (Pipeline != null) { mainSingleRunning = Pipeline.IsSingleRunning; Pipeline.PropertyChanged += OnMainPipelineChanged; }
        }

        public event EventHandler LayerManagerOpenRequested = delegate { };

        public event EventHandler RecipeManagerOpenRequested = delegate { };
        public event EventHandler ToolPaletteFocusRequested = delegate { };
        public event EventHandler ProductInfoRequested = delegate { };

        public OpenVisionShellPreviewViewModel Shell { get; }
        public OpenVisionShellHostRecipeCommandSurface Recipes { get; }
        public IReadOnlyList<KeyValuePair<string, string>> RecentRecipes => recentRecipeNames
            .Where(name => Recipes?.RecipeOptions.Contains(name, StringComparer.OrdinalIgnoreCase) == true)
            .Select(name => new KeyValuePair<string, string>(name, name + (string.Equals(name, Recipes.MainRecipeName, StringComparison.OrdinalIgnoreCase) ? "  (" + this["Workbench.Recipe.InUse"] + ")" : string.Empty))).ToArray();
        public OpenVisionShellHostChromeCommandSurface Chrome { get; }
        public OpenVisionShellHostWorkspaceCommandSurface Workspace { get; }
        public OpenVisionShellHostLayerCommandSurface Layers { get; private set; }

        internal void AttachLayers(OpenVisionShellHostLayerCommandSurface layers)
        {
            Layers = layers;
            OnPropertyChanged(nameof(Layers));
            CommandManager.InvalidateRequerySuggested();
        }
        public OpenVisionWorkbenchPipelineViewModel Pipeline => Recipes?.WorkbenchPipeline;
        public OpenVisionWorkbenchToolViewModel Tool { get; private set; }
        public OpenVisionWorkbenchViewerViewModel Viewer { get; private set; }
        public OpenVisionWorkbenchImagesViewModel Images { get; private set; }
        public OpenVisionWorkbenchResultsViewModel Results { get; private set; }
        internal void AttachResults(OpenVisionWorkbenchResultsViewModel results)
        {
            Results = results ?? throw new ArgumentNullException(nameof(results));
            OnPropertyChanged(nameof(Results));
        }
        internal void AttachImages(OpenVisionWorkbenchImagesViewModel images)
        {
            if (images == null) { throw new ArgumentNullException(nameof(images)); }
            if (Images != null) { Images.ImportStarted -= OnImportStarted; Images.ImportCompleted -= OnImportCompleted; Images.BatchProgressed -= OnBatchProgressed; Images.BatchFinished -= OnBatchFinished; }
            Images = images;
            Images.ImportStarted += OnImportStarted;
            Images.ImportCompleted += OnImportCompleted;
            Images.BatchProgressed += OnBatchProgressed;
            Images.BatchFinished += OnBatchFinished;
            OnPropertyChanged(nameof(Images));
        }
        internal void AttachViewer(OpenVisionWorkbenchViewerViewModel viewer)
        {
            if (Viewer != null) { Viewer.LayoutChanged -= OnViewerLayoutChanged; }
            Viewer = viewer ?? throw new ArgumentNullException(nameof(viewer));
            Viewer.LayoutChanged += OnViewerLayoutChanged;
            RaiseLayout();
            OnPropertyChanged(nameof(Viewer));
        }
        internal void AttachToolEditor(OpenVisionWorkbenchToolViewModel tool)
        {
            if (Tool != null) { Tool.PropertyChanged -= OnMainToolChanged; }
            Tool = tool;
            tool.NImagePaths = () => Images?.Images.Select(image => image.Path).ToArray() ?? Array.Empty<string>();
            tool.ConfigureAutoRunPreference(() => IsAutoRunEnabled, value => IsAutoRunEnabled = value);
            tool.PropertyChanged += OnMainToolChanged;
            OnPropertyChanged(nameof(Tool));
        }
        public bool IsShortcutsOpen
        {
            get => isShortcutsOpen;
            private set { if (SetProperty(ref isShortcutsOpen, value)) { OnPropertyChanged(nameof(IsMainInteractive)); CommandManager.InvalidateRequerySuggested(); } }
        }
        public bool IsMainInteractive => !isImageEditOpen && !isShortcutsOpen && !isMainRecipeOpen && !isMainRecipeBusy && !isMainValidationOpen && !isMainHistoryOpen && !isMainLlmOpen && !isLayerManagerOpen;
        public bool IsMainValidationOpen
        {
            get => isMainValidationOpen;
            private set { if (SetProperty(ref isMainValidationOpen, value)) { OnPropertyChanged(nameof(IsMainInteractive)); CommandManager.InvalidateRequerySuggested(); } }
        }
        public bool IsMainValidationSave => isMainValidationSave;
        public bool IsMainValidationLoad => !isMainValidationSave;
        public bool HasMainValidationSets => mainValidationSets.Count > 0;
        public bool HasNoMainValidationSets => mainValidationSets.Count == 0;
        public IReadOnlyList<OpenVisionRecipeValidationSetOption> MainValidationSets => mainValidationSets;
        public string MainValidationName { get => mainValidationName; set { if (SetProperty(ref mainValidationName, value ?? string.Empty)) { MainValidationError = string.Empty; } } }
        public string MainValidationError { get => mainValidationError; private set { if (SetProperty(ref mainValidationError, value ?? string.Empty)) { OnPropertyChanged(nameof(HasMainValidationError)); } } }
        public bool HasMainValidationError => !string.IsNullOrEmpty(mainValidationError);
        public string MainValidationTitle => this[isMainValidationSave ? "Workbench.ValidationSet.SaveTitle" : "Workbench.ValidationSet.LoadTitle"];
        public string MainValidationSummary
        {
            get
            {
                int good = Images?.Images.Count(item => item.Label == "good") ?? 0, bad = Images?.Images.Count(item => item.Label == "bad") ?? 0;
                int excluded = (Images?.Count ?? 0) - good - bad;
                return string.Format(CultureInfo.CurrentCulture, this["Workbench.ValidationSet.SaveBodyFormat"], good + bad, good, bad)
                    + (excluded > 0 ? string.Format(CultureInfo.CurrentCulture, this["Workbench.ValidationSet.SaveExcludedFormat"], excluded) : string.Empty);
            }
        }
        public string MainActionKind => mainActionKind;
        public PackIconMaterialKind MainActionIcon => mainActionKind switch
        {
            "Running" => PackIconMaterialKind.TimerOutline,
            "Ok" => PackIconMaterialKind.CheckCircle,
            "Ng" => PackIconMaterialKind.CloseCircle,
            "Error" => PackIconMaterialKind.AlertCircle,
            "Warning" => PackIconMaterialKind.AlertOutline,
            _ => PackIconMaterialKind.InformationOutline
        };
        public string MainActionText => mainActionKey is "Workbench.Action.NImageProgressFormat" or "Workbench.Action.NImageCompletedFormat" or "Workbench.Action.NImageStoppedFormat"
            ? string.Format(CultureInfo.CurrentCulture, this[mainActionKey], mainNImageIndex, mainNImagePlanned, mainActionArgument, mainNImagePrefix)
            : mainActionKey == "Workbench.Batch.Progress"
            ? string.Format(CultureInfo.CurrentCulture, this[mainActionKey], mainBatchIndex, mainBatchPlanned, mainBatchImageName)
            : mainActionKey is "Workbench.Action.BatchCompletedFormat" or "Workbench.Action.BatchStoppedFormat"
            ? string.Format(CultureInfo.CurrentCulture, this[mainActionKey], mainBatchOk, mainBatchNg, mainBatchError)
            : mainActionKey is "Workbench.Action.ImagesAddedFormat" or "Workbench.Action.SampleImagesOpenedFormat"
            ? string.Format(CultureInfo.CurrentCulture, this[mainActionKey], mainImportAdded)
                + (mainImportFailed > 0 ? string.Format(CultureInfo.CurrentCulture, this["Workbench.Action.ImagesFailedSuffixFormat"], mainImportFailed) : "")
                + (mainImportNestedExcluded > 0 ? string.Format(CultureInfo.CurrentCulture, this["Workbench.Action.ImagesNestedExcludedSuffixFormat"], mainImportNestedExcluded) : "")
            : mainActionKey is "Workbench.Action.PipelineRunCompletedFormat" or "Workbench.Action.PrefixRunCompletedFormat"
            ? string.Format(CultureInfo.CurrentCulture, this[mainActionKey], this["Workbench.Status." + mainActionResultKind], mainActionElapsed)
            : mainActionKey == "Workbench.Action.RunCompletedFormat"
            ? string.Format(CultureInfo.CurrentCulture, this[mainActionKey], this["Workbench.Status." + mainActionKind], mainActionElapsed)
            : mainActionKey is "Workbench.Tool.PresetAppliedFormat" or "Workbench.Action.ToolTestOpenedFormat" or "Workbench.Action.ToolTestOpenedRunOnlyFormat" ? string.Format(CultureInfo.CurrentCulture, this[mainActionKey], this[mainActionArgumentKey])
            : mainActionKey is "Workbench.Action.PrefixRunStartedFormat" or "Workbench.Action.StepRunStartedFormat" or "Workbench.Action.ToolRunStartedFormat" or "Workbench.Action.InputChangedFormat" or "Workbench.Action.OutputChangedFormat" or "Workbench.Action.ImagesOpeningFormat"
                ? string.Format(CultureInfo.CurrentCulture, this[mainActionKey], string.IsNullOrEmpty(mainActionArgumentKey) ? mainActionArgument : this[mainActionArgumentKey])
            : mainActionKey is "Workbench.Action.StepRunCompletedFormat" or "Workbench.Action.ToolRunCompletedFormat"
                ? string.Format(CultureInfo.CurrentCulture, this[mainActionKey], string.IsNullOrEmpty(mainActionArgumentKey) ? mainActionArgument : this[mainActionArgumentKey], this["Workbench.Status." + mainActionResultKind])
            : mainActionKey is "Workbench.Action.RecipeOpenedFormat" or "Workbench.Action.PipelineSwitchedFormat" ? string.Format(CultureInfo.CurrentCulture, this[mainActionKey], mainActionArgument) : this[mainActionKey];
        public bool MainActionHasPrefixTarget => mainActionKey == "Workbench.Action.PrefixRunStartedFormat";
        public string MainActionPrefixBefore => this["Workbench.Action.PrefixRunStartedFormat"].Split("{0}")[0];
        public string MainActionPrefixTarget => mainActionArgument;
        public string MainActionPrefixAfter => this["Workbench.Action.PrefixRunStartedFormat"].Split("{0}")[1];
        public string MainNoticeText => string.IsNullOrEmpty(mainNoticeKey) ? ProductInfoText : string.Format(CultureInfo.CurrentCulture, this[mainNoticeKey], mainNoticeArgument);
        internal void SetImageEditOpen(bool value) { isImageEditOpen = value; Pipeline?.SetImageEditPending(value); OnPropertyChanged(nameof(IsMainInteractive)); CommandManager.InvalidateRequerySuggested(); }

        public ICommand CloseLayerManagerCommand { get; }
        public bool IsLayerManagerOpen
        {
            get => isLayerManagerOpen;
            private set { if (SetProperty(ref isLayerManagerOpen, value)) { OnPropertyChanged(nameof(IsMainInteractive)); CommandManager.InvalidateRequerySuggested(); } }
        }

        public ICommand OpenMainLlmCommand { get; }
        public ICommand CloseMainLlmCommand { get; }
        public ICommand SelectMainLlmStageCommand { get; }
        public ICommand NextMainLlmStageCommand { get; }
        public ICommand BuildMainLlmPromptCommand { get; }
        public ICommand ValidateMainLlmCommand { get; }
        public ICommand SaveMainLlmCommand { get; }
        public bool IsMainLlmOpen
        {
            get => isMainLlmOpen;
            private set { if (SetProperty(ref isMainLlmOpen, value)) { if (!value) { mainLlmWorking = Array.Empty<byte>(); mainLlmEditor = null; mainLlmImage = null; } OnPropertyChanged(nameof(IsMainInteractive)); CommandManager.InvalidateRequerySuggested(); } }
        }
        public int MainLlmStage
        {
            get => mainLlmStage;
            private set { if (SetProperty(ref mainLlmStage, value)) { OnPropertyChanged(nameof(IsMainLlmRequest)); OnPropertyChanged(nameof(IsMainLlmResponse)); OnPropertyChanged(nameof(IsMainLlmReview)); OnPropertyChanged(nameof(IsMainLlmNext)); CommandManager.InvalidateRequerySuggested(); } }
        }
        public bool IsMainLlmRequest => mainLlmStage == 1;
        public bool IsMainLlmResponse => mainLlmStage == 2;
        public bool IsMainLlmReview => mainLlmStage == 3;
        public bool IsMainLlmNext => mainLlmStage < 3;
        public bool MainLlmIncludePipeline { get => mainLlmIncludePipeline; set => SetProperty(ref mainLlmIncludePipeline, value); }
        public bool MainLlmIncludeImage { get => mainLlmIncludeImage; set => SetProperty(ref mainLlmIncludeImage, value); }
        public bool MainLlmOpenSaved { get => mainLlmOpenSaved; set => SetProperty(ref mainLlmOpenSaved, value); }
        public string MainLlmName { get => mainLlmName; set => SetProperty(ref mainLlmName, value ?? string.Empty); }
        public string MainLlmError { get => mainLlmError; private set => SetProperty(ref mainLlmError, value ?? string.Empty); }
        public string MainLlmContext => mainLlmRecipe + " / " + mainLlmPipeline;

        public ICommand OpenMainHistoryCommand { get; }
        public ICommand CloseMainHistoryCommand { get; }
        public bool IsMainHistoryOpen
        {
            get => isMainHistoryOpen;
            private set { if (SetProperty(ref isMainHistoryOpen, value)) { OnPropertyChanged(nameof(IsMainInteractive)); CommandManager.InvalidateRequerySuggested(); } }
        }
        public IReadOnlyList<OpenVisionRecipeBatchRunOption> MainHistoryRows => mainHistoryRows;
        public bool HasNoMainHistory => mainHistoryRows.Count == 0;
        public string MainHistoryError => mainHistoryError;
        public double MainHistoryMaximumWidth => Math.Max(400D, viewportWidth - 80D);

        public ICommand OpenMainValidationSaveCommand { get; }
        public ICommand OpenMainValidationLoadCommand { get; }
        public ICommand CloseMainValidationCommand { get; }
        public ICommand SaveMainValidationCommand { get; }
        public ICommand LoadMainValidationCommand { get; }

        public bool IsMainRecipeOpen
        {
            get => isMainRecipeOpen;
            private set { if (SetProperty(ref isMainRecipeOpen, value)) { OnPropertyChanged(nameof(IsMainInteractive)); CommandManager.InvalidateRequerySuggested(); } }
        }
        public bool IsMainRecipeSaveAs => isMainRecipeSaveAs;
        public bool IsMainRecipeNew => !isMainRecipeSaveAs && !IsMainPipelineForm;
        public bool IsMainRecipeDescriptionVisible => !IsMainPipelineForm;
        public bool IsMainPipelineCreate => isMainPipelineCreate;
        public bool IsMainPipelineRename => isMainPipelineRename;
        public bool IsMainPipelineForm => isMainPipelineCreate || isMainPipelineRename;
        public string MainRecipeNameLabel => this[IsMainPipelineForm ? "Workbench.Pipeline.Name" : "Workbench.Recipe.Name"];
        public bool IsMainRecipeBusy => isMainRecipeBusy;
        public double MainRecipeMaximumHeight => Math.Max(200D, viewportHeight - 60D);
        public string MainRecipeTitle => this[isMainPipelineRename ? "Workbench.Pipeline.RenameTitle" : isMainPipelineCreate ? "Workbench.Pipeline.NewTitle" : isMainRecipeSaveAs ? "Workbench.Recipe.SaveAsTitle" : "Workbench.Recipe.NewTitle"];
        public string MainRecipeConfirmText => this[IsMainPipelineForm ? "Common.OK" : isMainRecipeSaveAs ? "Workbench.Recipe.Save" : "Workbench.Recipe.Create"];
        public string MainRecipeName { get => mainRecipeName; set { if (SetProperty(ref mainRecipeName, value ?? string.Empty)) { SetMainRecipeError(string.Empty); } } }
        public string MainRecipeDescription { get => mainRecipeDescription; set { if (SetProperty(ref mainRecipeDescription, value ?? string.Empty)) { SetMainRecipeError(string.Empty); } } }
        public string MainRecipeFirstPipeline { get => mainRecipeFirstPipeline; set { if (SetProperty(ref mainRecipeFirstPipeline, value ?? string.Empty)) { SetMainRecipeError(string.Empty); } } }
        public bool MainRecipeCopyCurrent { get => mainRecipeCopyCurrent; set { if (SetProperty(ref mainRecipeCopyCurrent, value)) { OnPropertyChanged(nameof(MainRecipeStartEmpty)); } } }
        public bool MainRecipeStartEmpty { get => !mainRecipeCopyCurrent; set { if (value) { MainRecipeCopyCurrent = false; } } }
        public string MainRecipeError => (string.IsNullOrEmpty(mainRecipeErrorKey) ? string.Empty : this[mainRecipeErrorKey]) + mainRecipeErrorDetail;
        public bool HasMainRecipeError => !string.IsNullOrEmpty(mainRecipeErrorKey) || !string.IsNullOrEmpty(mainRecipeErrorDetail);
        internal Task MainRecipeSubmission { get; private set; } = Task.CompletedTask;
        public double ShortcutsMaximumHeight => Math.Max(200D, viewportHeight - 60D);
        public IReadOnlyList<KeyValuePair<string, string>> ShortcutRows => Array.ConvertAll(shortcutDefinitions, row => new KeyValuePair<string, string>(row.Key, this[row.TextKey]));
        public string ShortcutsCloseText => this["Common.Close"] + " (Esc)";
        public string ProductInfoText => "OpenVisionLab " + (typeof(OpenVisionWorkbenchViewModel).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion.Split('+')[0]
            ?? typeof(OpenVisionWorkbenchViewModel).Assembly.GetName().Version?.ToString() ?? string.Empty) + " · " + this["Workbench.Product.Identity"];
        public double ProductInfoHorizontalOffset => (LeftPanelWidth.Value - RightPanelWidth.Value) / 2D;
        public double ProductInfoMaximumWidth => Math.Min(420D, Math.Max(260D, viewportWidth - LeftPanelWidth.Value - RightPanelWidth.Value - 24D));

        public bool IsAutoRunEnabled
        {
            get => autoRunEnabled;
            set { if (SetProperty(ref autoRunEnabled, value)) { Tool?.NotifyAutoRunPreferenceChanged(); CommitLayout(); } }
        }
        public string ThemeName => themeName;
        public bool IsDarkTheme => themeName == "Dark";
        public bool IsLightTheme => themeName == "Light";
        public string this[string key] => OpenVisionLanguageService.T(key);
        public bool IsRecipeDirty => Recipes?.IsSelectedStepEditDirty == true || Recipes?.IsWorkbenchPipelineDirty == true;
        public bool HasRecipeSaveStatus => Recipes?.SelectedPipelineOption != null && !Recipes.IsSwitchingRecipe && string.Equals(Recipes.MainRecipeName, Recipes.SelectedRecipeName, StringComparison.OrdinalIgnoreCase);
        public string RecipeSaveState => Recipes?.HasRecipeSaveFailure == true || Recipes?.HasSelectedRecipePersistenceFailure == true || Recipes?.SelectedPipelineOption?.XmlValid == false ? "Error" : IsRecipeDirty ? "Unsaved" : "Saved";
        public long SavedRecipeRevision => Recipes?.SavedRevision ?? 0;
        public string RecipeSaveStatusText => this["Workbench.Recipe." + RecipeSaveState] + " · r" + SavedRecipeRevision;
        public string RecipeSaveStatusToolTip => RecipeSaveState == "Error"
            ? (Recipes?.HasRecipeSaveFailure == true ? Recipes.StatusText : Recipes?.HasSelectedRecipePersistenceFailure == true ? Recipes.SelectedRecipePersistenceHelpText : Recipes?.SelectedPipelineOption?.StatusText)
            : this["Workbench.Recipe." + RecipeSaveState + "Help"] + " · r" + SavedRecipeRevision;
        public string DocumentTitle => (Recipes?.MainRecipeName ?? string.Empty)
            + (Recipes?.IsSelectedStepEditDirty == true || Recipes?.IsWorkbenchPipelineDirty == true ? " •" : string.Empty)
            + " — " + (Recipes?.SelectedPipelineOption?.PipelineName ?? string.Empty);
        public string LayoutStatusText => recentRecipeSaveFailed ? this["Workbench.Recipe.RecentSaveFailed"] : string.IsNullOrEmpty(layoutStatusKey) ? string.Empty : this[layoutStatusKey];
        public bool HasLayoutStatus => recentRecipeSaveFailed || !string.IsNullOrEmpty(layoutStatusKey);
        public bool IsCompact => viewportWidth <= 1360D;
        public bool ShowLongCommandLabels => viewportWidth > 1560D;
        public bool ShowCommandLabels => !IsCompact;
        public bool ShowViewerFitCaption => ShowCommandLabels && viewerToolbarWidth > 860D;
        public bool ShowViewerZoomControls => !IsCompact && viewerToolbarWidth > 680D;
        public bool IsLeftExpanded => !leftCollapsed;
        public bool IsRightExpanded => !rightCollapsed;
        public bool IsBottomExpanded => !bottomCollapsed;
        public bool IsToolPanelWide => toolPanelWide;
        public PackIconMaterialKind BottomPanelChevron => bottomCollapsed ? PackIconMaterialKind.ChevronUp : PackIconMaterialKind.ChevronDown;
        public string BottomPanelToggleText => this[bottomCollapsed ? "Workbench.Bottom.Expand" : "Workbench.Bottom.Collapse"] + " (Ctrl+3)";
        public bool IsLogPanelVisible => !bottomCollapsed && selectedBottomTab == 2;
        public bool IsToolPaletteExpanded => leftCollapsed || !toolsCollapsed;
        public bool IsToolPaletteSearchVisible => !leftCollapsed && !toolsCollapsed;
        public PackIconMaterialKind ToolPaletteChevron => toolsCollapsed ? PackIconMaterialKind.ChevronDown : PackIconMaterialKind.ChevronUp;
        public double LeftPanelMinimum => leftCollapsed ? 48D : 248D;
        public double LeftPanelMaximum => leftCollapsed ? 48D : Math.Min(900D, Math.Max(248D, viewportWidth - 2D - 10D - (rightCollapsed ? 0D : RightWidthFloor) - 480D));
        public double RightPanelMinimum => rightCollapsed ? 0D : RightWidthFloor;
        public double RightPanelMaximum => rightCollapsed ? 0D : Math.Min(900D, Math.Max(RightWidthFloor, viewportWidth - 2D - 10D - LeftPanelWidth.Value - 480D));
        public double BottomPanelMinimum => bottomCollapsed ? 36D : 200D;
        public double ViewerMinimumHeight => Viewer?.IsSplit4 == true ? 246D : 120D;
        public double BottomPanelMaximum => bottomCollapsed ? 36D : Math.Max(200D, viewportHeight - 241D - ViewerMinimumHeight);
        public double ToolPaletteMaximum => leftCollapsed ? viewportHeight : Math.Max(36D, (viewportHeight - 116D) * 0.42D);

        public GridLength LeftPanelWidth
        {
            get => new GridLength(leftCollapsed ? 48D : Math.Clamp(leftWidth > 0D ? leftWidth : DefaultLeftWidth, 248D, LeftPanelMaximum));
            set { if (!leftCollapsed && value.IsAbsolute && double.IsFinite(value.Value)) { leftWidth = Math.Clamp(value.Value, 248D, 900D); RaiseLayout(); } }
        }

        public GridLength RightPanelWidth
        {
            get => new GridLength(rightCollapsed ? 0D : Math.Clamp(EffectiveRightWidth, RightWidthFloor, RightPanelMaximum));
            set { if (!rightCollapsed && value.IsAbsolute && double.IsFinite(value.Value)) { rightWidth = Math.Clamp(value.Value, 480D, 900D); toolPanelWide = false; RaiseLayout(); } }
        }

        public GridLength BottomPanelHeight
        {
            get => new GridLength(bottomCollapsed ? 36D : Math.Clamp(bottomHeight > 0D ? bottomHeight : viewportHeight <= 820D ? 200D : 256D, 200D, BottomPanelMaximum));
            set { if (!bottomCollapsed && value.IsAbsolute && double.IsFinite(value.Value)) { bottomHeight = Math.Clamp(value.Value, 200D, 1200D); RaiseLayout(); } }
        }

        public bool IsLeftCollapsed
        {
            get => leftCollapsed;
            set { if (SetProperty(ref leftCollapsed, value)) { RaiseLayout(); CommitLayout(); } }
        }

        public bool IsRightCollapsed
        {
            get => rightCollapsed;
            set { if (SetProperty(ref rightCollapsed, value)) { RaiseLayout(); CommitLayout(); } }
        }

        public bool IsBottomCollapsed
        {
            get => bottomCollapsed;
            set { if (SetProperty(ref bottomCollapsed, value)) { RaiseLayout(); CommitLayout(); } }
        }

        public bool IsToolPaletteCollapsed
        {
            get => toolsCollapsed;
            set { if (SetProperty(ref toolsCollapsed, value)) { RaiseLayout(); CommitLayout(); } }
        }

        public int SelectedBottomTab
        {
            get => selectedBottomTab;
            set { if (value >= 0 && value <= 2 && SetProperty(ref selectedBottomTab, value)) { OnPropertyChanged(nameof(IsLogPanelVisible)); CommitLayout(); } }
        }

        public ICommand ToggleLeftPanelCommand { get; }
        public ICommand ToggleRightPanelCommand { get; }
        public ICommand ToggleBottomPanelCommand { get; }
        public ICommand ToggleToolPaletteCommand { get; }
        public ICommand ToggleToolPanelWideCommand { get; }
        public ICommand ResetLayoutCommand { get; }
        public ICommand CommitLayoutCommand { get; }
        public ICommand UpdateViewportCommand { get; }
        public ICommand UpdateViewerToolbarSizeCommand { get; }
        public ICommand SelectDarkThemeCommand { get; }
        public ICommand SelectLightThemeCommand { get; }
        public ICommand OpenLogCommand { get; }
        public ICommand OpenLayerManagerCommand { get; }
        public ICommand OpenRecipeManagerCommand { get; }
        public ICommand FocusToolPaletteCommand { get; }
        public ICommand OpenMainLearnCommand { get; }
        public ICommand OpenShortcutsCommand { get; }
        public ICommand CloseShortcutsCommand { get; }
        public ICommand ShowProductInfoCommand { get; }
        public ICommand CloseMainWindowCommand { get; }
        public ICommand OpenMainNewRecipeCommand { get; }
        public ICommand OpenMainNewPipelineCommand { get; }
        public ICommand OpenMainRenamePipelineCommand { get; }
        public ICommand DuplicateMainPipelineCommand { get; }
        public ICommand ExportMainPipelineXmlCommand { get; }
        public ICommand OpenMainSaveAsCommand { get; }
        public ICommand CloseMainRecipeCommand { get; }
        public ICommand SubmitMainRecipeCommand { get; }
        public ICommand UnavailableCommand { get; }

        private double DefaultLeftWidth => viewportWidth <= 1360D ? 248D : viewportWidth <= 1560D ? 264D : 296D;
        private double RightWidthFloor => 480D;
        private double DefaultRightWidth => viewportWidth <= 1360D ? 480D : viewportWidth <= 1560D ? 488D : 520D;
        private double EffectiveRightWidth => toolPanelWide ? 720D : rightWidth > 0D ? rightWidth : DefaultRightWidth;

        public void UpdateViewport(double width, double height)
        {
            if (!double.IsFinite(width) || !double.IsFinite(height) || width <= 0D || height <= 0D) { return; }
            viewportWidth = width;
            viewportHeight = height;
            if (!hasPreferences) { toolsCollapsed = height < 900D; }
            RaiseLayout();
        }

        private void UpdateViewerToolbarSize(double width)
        {
            if (disposed) { return; }
            viewerToolbarWidth = Math.Max(0D, width - 20D);
            OnPropertyChanged(nameof(ShowViewerFitCaption));
            OnPropertyChanged(nameof(ShowViewerZoomControls));
        }

        private void OpenMainLlm()
        {
            if (!OpenMainLlmCommand.CanExecute(null)) { return; }
            mainLlmRecipe = Recipes.MainRecipeName; mainLlmPipeline = Recipes.SelectedPipelineOption?.PipelineName ?? string.Empty;
            mainLlmWorking = VisionPipelineExecutionPlan.SerializePipeline(Pipeline.CopyWorkingPipeline());
            mainLlmEditor = Recipes.SelectedStepEditObject; mainLlmImage = Images?.CurrentImage;
            MainLlmStage = 1; MainLlmOpenSaved = false; MainLlmError = string.Empty;
            OnPropertyChanged(nameof(MainLlmContext)); IsMainLlmOpen = true;
        }

        private bool MainLlmStillCurrent() => Pipeline?.IsRunning != true
            && string.Equals(mainLlmRecipe, Recipes?.MainRecipeName, StringComparison.OrdinalIgnoreCase)
            && string.Equals(mainLlmPipeline, Recipes?.SelectedPipelineOption?.PipelineName, StringComparison.OrdinalIgnoreCase)
            && ReferenceEquals(mainLlmEditor, Recipes?.SelectedStepEditObject) && ReferenceEquals(mainLlmImage, Images?.CurrentImage)
            && mainLlmWorking.SequenceEqual(VisionPipelineExecutionPlan.SerializePipeline(Pipeline.CopyWorkingPipeline()));

        private void BuildMainLlmPrompt()
        {
            if (!MainLlmStillCurrent()) { MainLlmError = this["Workbench.Llm.ContextChanged"]; return; }
            string prompt = Recipes.BuildMainLlmPrompt(MainLlmIncludePipeline);
            if (MainLlmIncludeImage)
            {
                prompt += Environment.NewLine + "Current image dimensions: " + (Shell?.HasWorkspaceInputImage == true ? Shell.WorkspaceInputSizeText : "No current image")
                    + Environment.NewLine + "Brightness statistics: not recorded. Do not invent values. No image pixels are included.";
            }
            Recipes.LlmPromptText = prompt; MainLlmError = string.Empty;
        }

        private void ValidateMainLlm()
        {
            if (!MainLlmStillCurrent()) { MainLlmError = this["Workbench.Llm.ContextChanged"]; return; }
            Recipes.ValidateLlmXmlDraftCommand.Execute(null);
            MainLlmError = string.Empty;
            if (Recipes.IsLlmXmlDraftReady) { MainLlmName = Recipes.ValidatedLlmDraftName; }
        }

        private void SaveMainLlm()
        {
            if (!isMainLlmOpen || isMainLlmSaving) { return; }
            if (mainLlmStage != 3 || !Recipes.IsLlmXmlDraftReady) { MainLlmError = this["Workbench.Llm.ValidateBeforeSave"]; return; }
            if (!MainLlmStillCurrent()) { MainLlmError = this["Workbench.Llm.ContextChanged"]; return; }
            isMainLlmSaving = true; CommandManager.InvalidateRequerySuggested();
            try
            {
                if (!Recipes.TrySaveMainLlmDraft(MainLlmName, MainLlmOpenSaved, out string error)) { MainLlmError = error; return; }
                IsMainLlmOpen = false; Recipes.LlmXmlDraftText = string.Empty; Recipes.LlmPromptText = string.Empty;
            }
            catch (Exception ex) when (ex is System.IO.IOException or UnauthorizedAccessException or InvalidOperationException or ArgumentException)
            {
                MainLlmError = this["Workbench.Llm.SaveFailed"] + " " + ex.Message;
            }
            finally { isMainLlmSaving = false; CommandManager.InvalidateRequerySuggested(); }
        }

        private void OpenMainHistory()
        {
            if (!OpenMainHistoryCommand.CanExecute(null)) { return; }
            mainHistoryError = string.Empty;
            try { mainHistoryRows = Recipes.ReadMainHistory(); }
            catch (Exception ex) when (ex is System.IO.IOException or UnauthorizedAccessException or ArgumentException)
            {
                mainHistoryRows = Array.Empty<OpenVisionRecipeBatchRunOption>();
                mainHistoryError = this["Workbench.History.ReadFailed"] + " " + ex.Message;
            }
            OnPropertyChanged(nameof(MainHistoryRows)); OnPropertyChanged(nameof(HasNoMainHistory)); OnPropertyChanged(nameof(MainHistoryError));
            IsMainHistoryOpen = true;
        }

        private bool CanOpenMainValidation(bool save) => !disposed && IsMainInteractive && Recipes?.CanUseMainValidationSets == true && Images?.CanEdit == true && (!save || Images.Count > 0);

        private void OpenMainValidation(bool save)
        {
            if (!CanOpenMainValidation(save)) { return; }
            isMainValidationSave = save;
            mainValidationRecipe = Recipes.MainRecipeName;
            mainValidationSets = Recipes.GetMainValidationSets(out string error);
            MainValidationName = string.Format(CultureInfo.CurrentCulture, this["Workbench.ValidationSet.DefaultName"], mainValidationSets.Count + 1);
            MainValidationError = error;
            OnPropertyChanged(string.Empty);
            IsMainValidationOpen = true;
        }

        private bool MainValidationStillCurrent() => Recipes?.CanUseMainValidationSets == true && Images?.CanEdit == true && string.Equals(mainValidationRecipe, Recipes.MainRecipeName, StringComparison.OrdinalIgnoreCase);

        private void SaveMainValidation()
        {
            if (!MainValidationStillCurrent()) { IsMainValidationOpen = false; return; }
            if (!Recipes.TrySaveMainValidationSet(MainValidationName, Images.CaptureValidationImages(), out string error)) { MainValidationError = error; return; }
            IsMainValidationOpen = false;
            ShowMainNotice("Workbench.ValidationSet.AddedFormat", MainValidationName.Trim());
        }

        private void LoadMainValidation(OpenVisionRecipeValidationSetOption option)
        {
            if (!MainValidationStillCurrent()) { IsMainValidationOpen = false; return; }
            if (!mainValidationSets.Contains(option)) { MainValidationError = this["Workbench.ValidationSet.SetNotCurrent"]; return; }
            OpenVisionRecipeValidationSetOption stored = Recipes.GetMainValidationSets(out string readError)
                .FirstOrDefault(value => string.Equals(value.Name, option.Name, StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrEmpty(readError)) { MainValidationError = readError; return; }
            if (stored == null || stored.Set.Images.Count != option.Set.Images.Count
                || !string.Equals(stored.Set.PipelineDefinitionSha256, option.Set.PipelineDefinitionSha256, StringComparison.OrdinalIgnoreCase)
                || !string.Equals(stored.Set.ImageSetSha256, option.Set.ImageSetSha256, StringComparison.OrdinalIgnoreCase)
                || stored.Set.Images.Where((image, index) => !string.Equals(image.Path, option.Set.Images[index].Path, StringComparison.OrdinalIgnoreCase)
                    || !string.Equals(image.Expected, option.Set.Images[index].Expected, StringComparison.OrdinalIgnoreCase)
                    || !string.Equals(image.Sha256, option.Set.Images[index].Sha256, StringComparison.OrdinalIgnoreCase)).Any())
            {
                MainValidationError = this["Workbench.ValidationSet.SetChanged"];
                return;
            }
            if (!Images.TryLoadValidationSet(option.Set, out string error)) { MainValidationError = error; return; }
            IsMainValidationOpen = false;
            ShowMainNotice("Workbench.ValidationSet.LoadedFormat", option.Name);
        }


        private void ExportMainPipelineXml()
        {
            if (!ExportMainPipelineXmlCommand.CanExecute(null)) { return; }
            var result = Recipes.ExportMainPipelineXml();
            if (!result.Succeeded && !result.Canceled) { ShowMainNotice("Workbench.Pipeline.ExportFailedFormat", result.Error); }
        }

        private void DuplicateMainPipeline()
        {
            var result = Recipes.DuplicateMainPipeline();
            if (!result.Succeeded && !result.Canceled) { ShowMainNotice("Workbench.Pipeline.CopyFailedFormat", result.Error); }
        }

        private void ShowMainNotice(string key, string argument)
        {
            mainNoticeKey = key ?? string.Empty;
            mainNoticeArgument = argument ?? string.Empty;
            OnPropertyChanged(nameof(MainNoticeText));
            ProductInfoRequested(this, EventArgs.Empty);
        }

        private bool CanOpenMainRecipe(bool saveAs) => !disposed && IsMainInteractive && Recipes?.CanCreateMainRecipe(saveAs) == true;

        private void OpenMainRecipe(bool saveAs)
        {
            isMainPipelineCreate = false;
            isMainPipelineRename = false;
            isMainRecipeSaveAs = saveAs;
            string sourceName = saveAs ? Recipes.MainRecipeName : string.Empty;
            string suffix = saveAs ? "_" + this["Workbench.Recipe.CopySuffix"] : string.Empty;
            int sourceLength = Math.Min(sourceName.Length, 60 - suffix.Length);
            if (sourceLength > 0 && sourceLength < sourceName.Length && char.IsHighSurrogate(sourceName[sourceLength - 1]) && char.IsLowSurrogate(sourceName[sourceLength])) { sourceLength--; }
            MainRecipeName = sourceName.Substring(0, sourceLength) + suffix;
            MainRecipeDescription = saveAs ? Recipes.MainRecipeDescription : string.Empty;
            MainRecipeFirstPipeline = this["Workbench.Recipe.DefaultPipeline"];
            MainRecipeCopyCurrent = false;
            SetMainRecipeError(string.Empty);
            foreach (string property in new[] { nameof(IsMainRecipeSaveAs), nameof(IsMainRecipeNew), nameof(MainRecipeTitle), nameof(MainRecipeConfirmText), nameof(IsMainPipelineCreate), nameof(IsMainPipelineRename), nameof(IsMainPipelineForm), nameof(IsMainRecipeDescriptionVisible), nameof(MainRecipeNameLabel) }) { OnPropertyChanged(property); }
            IsMainRecipeOpen = true;
        }

        private void OpenMainPipeline(bool rename)
        {
            isMainPipelineCreate = !rename;
            isMainPipelineRename = rename;
            isMainRecipeSaveAs = false;
            mainPipelineRecipe = Recipes.MainRecipeName;
            mainPipelineName = Recipes.SelectedPipelineOption?.PipelineName ?? string.Empty;
            MainRecipeName = rename ? mainPipelineName : Recipes.NewMainPipelineName();
            SetMainRecipeError(string.Empty);
            OnPropertyChanged(string.Empty);
            IsMainRecipeOpen = true;
        }

        private void SetMainRecipeError(string key, string detail = "")
        {
            mainRecipeErrorKey = key;
            mainRecipeErrorDetail = detail;
            OnPropertyChanged(nameof(MainRecipeError));
            OnPropertyChanged(nameof(HasMainRecipeError));
        }

        private async Task SubmitMainRecipeAsync()
        {
            if (disposed || isMainRecipeBusy || !isMainRecipeOpen) { return; }
            try
            {
                if (IsMainPipelineForm && !string.Equals(mainPipelineRecipe, Recipes.MainRecipeName, StringComparison.OrdinalIgnoreCase)) { SetMainRecipeError("Workbench.Pipeline.RecipeChanged"); return; }
                if (isMainPipelineRename && !string.Equals(mainPipelineName, Recipes.SelectedPipelineOption?.PipelineName, StringComparison.OrdinalIgnoreCase)) { SetMainRecipeError("Workbench.Pipeline.SelectionChanged"); return; }
                string validation = IsMainPipelineForm ? Recipes.ValidateMainPipelineName(MainRecipeName, isMainPipelineRename ? mainPipelineName : null) : Recipes.ValidateMainRecipe(MainRecipeName, MainRecipeDescription, MainRecipeFirstPipeline, isMainRecipeSaveAs);
                if (!string.IsNullOrEmpty(validation)) { SetMainRecipeError(validation); return; }
                isMainRecipeBusy = true;
                IsMainRecipeOpen = false;
                OnPropertyChanged(nameof(IsMainRecipeBusy));
                OnPropertyChanged(nameof(IsMainInteractive));
                CommandManager.InvalidateRequerySuggested();
                var result = isMainPipelineRename ? Recipes.RenameMainPipeline(mainPipelineName, MainRecipeName)
                    : isMainPipelineCreate ? Recipes.CreateMainPipeline(MainRecipeName)
                    : await Recipes.CreateMainRecipeAsync(MainRecipeName, MainRecipeDescription, MainRecipeFirstPipeline, MainRecipeCopyCurrent, isMainRecipeSaveAs);
                if (disposed) { return; }
                if (result.Succeeded) { if (!IsMainPipelineForm) { Images?.ClearRecipeResults(); } }
                else if (!result.Canceled) { SetMainRecipeError(string.Empty, result.Error); IsMainRecipeOpen = true; }
            }
            catch (Exception exception)
            {
                if (!disposed) { SetMainRecipeError(string.Empty, this["Workbench.Recipe.SaveFailed"] + " " + exception.GetBaseException().Message); IsMainRecipeOpen = true; }
            }
            finally
            {
                isMainRecipeBusy = false;
                OnPropertyChanged(nameof(IsMainRecipeBusy));
                OnPropertyChanged(nameof(IsMainInteractive));
                CommandManager.InvalidateRequerySuggested();
            }
        }

        private void SetTheme(string value)
        {
            bool changed = SetProperty(ref themeName, value, nameof(ThemeName));
            OnPropertyChanged(nameof(IsDarkTheme));
            OnPropertyChanged(nameof(IsLightTheme));
            if (changed) { CommitLayout(); }
        }

        private void Restore(SystemStateConfig preferences)
        {
            recentRecipeNames = (preferences.WorkbenchRecentRecipeNames ?? Array.Empty<string>()).Where(RecipeWorkspaceService.IsValidRecipeName).Select(name => name.Trim()).Distinct(StringComparer.OrdinalIgnoreCase).Take(5).ToArray();
            autoRunEnabled = preferences.WorkbenchAutoRunEnabled;
            themeName = preferences.WorkbenchThemeName == "Light" ? "Light" : "Dark";
            hasPreferences = preferences.WorkbenchLayoutVersion == 1;
            if (preferences.WorkbenchLayoutVersion != 0 && !hasPreferences) { layoutStatusKey = "Main.DefaultLayoutApplied"; return; }
            if (!hasPreferences) { return; }
            leftWidth = Normalize(preferences.WorkbenchLeftWidth, 248D, 900D);
            rightWidth = Normalize(preferences.WorkbenchRightWidth, 480D, 900D);
            bottomHeight = Normalize(preferences.WorkbenchBottomHeight, 200D, 1200D);
            leftCollapsed = preferences.WorkbenchLeftCollapsed;
            rightCollapsed = preferences.WorkbenchRightCollapsed;
            bottomCollapsed = preferences.WorkbenchBottomCollapsed;
            toolsCollapsed = preferences.WorkbenchToolsCollapsed;
            selectedBottomTab = Math.Clamp(preferences.WorkbenchBottomTab, 0, 2);
            if (leftWidth != preferences.WorkbenchLeftWidth || rightWidth != preferences.WorkbenchRightWidth
                || bottomHeight != preferences.WorkbenchBottomHeight || selectedBottomTab != preferences.WorkbenchBottomTab)
            {
                layoutStatusKey = "Main.DefaultLayoutApplied";
            }
        }

        private static double Normalize(double value, double minimum, double maximum) => value == 0D ? 0D : double.IsFinite(value) && value >= minimum && value <= maximum ? value : 0D;

        private void CommitLayout()
        {
            preferenceSnapshot = new SystemStateConfig
            {
                WorkbenchLayoutVersion = 1,
                WorkbenchLeftWidth = leftWidth,
                WorkbenchRightWidth = toolPanelWide ? 720D : rightWidth,
                WorkbenchBottomHeight = bottomHeight,
                WorkbenchLeftCollapsed = leftCollapsed,
                WorkbenchRightCollapsed = rightCollapsed,
                WorkbenchBottomCollapsed = bottomCollapsed,
                WorkbenchToolsCollapsed = toolsCollapsed,
                WorkbenchBottomTab = selectedBottomTab,
                WorkbenchAutoRunEnabled = autoRunEnabled,
                WorkbenchRecentRecipeNames = recentRecipeNames.ToArray(),
                WorkbenchThemeName = themeName
            };
            bool saved = savePreferences(preferenceSnapshot);
            recentRecipeSaveFailed = false;
            hasPreferences = true;
            layoutStatusKey = saved ? string.Empty : "Main.LayoutSaveFailed";
            OnPropertyChanged(nameof(LayoutStatusText));
            OnPropertyChanged(nameof(HasLayoutStatus));
        }

        private void ResetLayout()
        {
            leftWidth = rightWidth = bottomHeight = 0D;
            leftCollapsed = rightCollapsed = bottomCollapsed = toolPanelWide = false;
            toolsCollapsed = viewportHeight < 900D;
            selectedBottomTab = 0;
            RaiseLayout();
            OnPropertyChanged(nameof(SelectedBottomTab));
            CommitLayout();
        }

        private void OnViewerLayoutChanged(object sender, EventArgs e) => RaiseLayout();

        private void RaiseLayout()
        {
            foreach (string property in new[] { nameof(LeftPanelWidth), nameof(RightPanelWidth), nameof(BottomPanelHeight),
                nameof(LeftPanelMinimum), nameof(LeftPanelMaximum), nameof(RightPanelMinimum), nameof(RightPanelMaximum),
                nameof(BottomPanelMinimum), nameof(BottomPanelMaximum), nameof(ViewerMinimumHeight), nameof(IsLeftCollapsed), nameof(IsRightCollapsed),
                nameof(IsBottomCollapsed), nameof(IsLogPanelVisible), nameof(IsLeftExpanded), nameof(IsRightExpanded), nameof(IsBottomExpanded), nameof(IsToolPanelWide), nameof(BottomPanelChevron), nameof(BottomPanelToggleText),
                nameof(IsToolPaletteCollapsed), nameof(IsToolPaletteExpanded), nameof(IsToolPaletteSearchVisible), nameof(ToolPaletteChevron), nameof(ToolPaletteMaximum), nameof(IsCompact),
                nameof(ShowLongCommandLabels), nameof(ShowCommandLabels), nameof(ShowViewerFitCaption), nameof(ShowViewerZoomControls),
                nameof(ShortcutsMaximumHeight), nameof(MainRecipeMaximumHeight), nameof(MainHistoryMaximumWidth), nameof(ProductInfoHorizontalOffset), nameof(ProductInfoMaximumWidth) }) { OnPropertyChanged(property); }
        }

        private void OnBatchProgressed(int index, int planned, string name)
        {
            if (disposed || Pipeline?.IsStopRequested == true) { return; }
            mainBatchIndex = index;
            mainBatchPlanned = planned;
            mainBatchImageName = name;
            PresentMainAction("Workbench.Batch.Progress", "Running");
        }

        private void OnBatchFinished(string kind, int ok, int ng, int error)
        {
            if (disposed) { return; }
            mainBatchOk = ok;
            mainBatchNg = ng;
            mainBatchError = error;
            PresentMainAction(kind == "Error" ? "Workbench.Action.RunFailed" : kind == "Warning" ? "Workbench.Action.BatchStoppedFormat" : "Workbench.Action.BatchCompletedFormat", kind);
        }

        private void OnImportStarted(int attempted)
        {
            if (!disposed) { PresentMainAction(Images?.IsSampleImport == true ? "Workbench.Action.SampleImagesOpening" : "Workbench.Action.ImagesOpeningFormat", "Running", argument: attempted.ToString(CultureInfo.InvariantCulture)); }
        }

        private void OnImportCompleted(int added, int failed, int nestedExcluded)
        {
            if (disposed) { return; }
            mainImportAdded = added;
            mainImportFailed = failed;
            mainImportNestedExcluded = nestedExcluded;
            PresentMainAction(Images?.IsSampleImport == true && failed == 0 ? "Workbench.Action.SampleImagesOpenedFormat" : "Workbench.Action.ImagesAddedFormat", failed > 0 || nestedExcluded > 0 ? "Warning" : "");
        }

        private void PresentMainAction(string key, string kind = "", string argumentKey = "", string elapsed = "", string argument = "", string resultKind = "")
        {
            if (disposed) { return; }
            mainActionKey = key;
            mainActionKind = kind;
            mainActionArgumentKey = argumentKey;
            mainActionArgument = argument;
            mainActionResultKind = resultKind;
            mainActionElapsed = elapsed;
            OnPropertyChanged(nameof(MainActionText));
            OnPropertyChanged(nameof(MainActionHasPrefixTarget));
            OnPropertyChanged(nameof(MainActionPrefixBefore));
            OnPropertyChanged(nameof(MainActionPrefixTarget));
            OnPropertyChanged(nameof(MainActionPrefixAfter));
            OnPropertyChanged(nameof(MainActionKind));
            OnPropertyChanged(nameof(MainActionIcon));
        }

        private void OnMainInputChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!disposed && e.PropertyName == nameof(OpenVisionShellPreviewViewModel.HasWorkspaceInputImage))
            {
                PresentMainAction(Shell.HasWorkspaceInputImage ? "Workbench.Action.ImageOpened" : "Workbench.Action.ImageCleared");
            }
        }

        private void OnMainPipelineChanged(object sender, PropertyChangedEventArgs e)
        {
            if (disposed) { return; }
            if (e.PropertyName == nameof(OpenVisionWorkbenchPipelineViewModel.RestoreActionKey))
            {
                PresentMainAction(Pipeline.RestoreActionKey);
                return;
            }
            bool stopRequested = Pipeline.IsRunning && Pipeline.IsStopRequested;
            if (stopRequested && !mainStopRequested) { PresentMainAction("Workbench.Batch.Stopping", "Warning"); }
            mainStopRequested = stopRequested;
            if (mainSingleRunning == Pipeline.IsSingleRunning) { return; }
            mainSingleRunning = Pipeline.IsSingleRunning;
            if (mainSingleRunning) { PresentMainAction(Pipeline.SingleRunPrefix ? "Workbench.Action.PrefixRunStartedFormat" : "Workbench.Action.PipelineRunStarted", "Running", argument: Pipeline.SingleRunTarget); }
            else if (Pipeline.SingleRunCompleted) { PresentMainAction(Pipeline.SingleRunPrefix ? "Workbench.Action.PrefixRunCompletedFormat" : "Workbench.Action.PipelineRunCompletedFormat", Pipeline.SingleRunResultKind == "NoVerdict" ? "" : Pipeline.SingleRunResultKind, elapsed: Pipeline.SingleRunElapsedText, resultKind: Pipeline.SingleRunResultKind); }
            else if (Pipeline.HasStatus) { PresentMainAction("Workbench.Action.RunFailed", "Error"); }
            else if (Pipeline.IsVerdictPrevious) { PresentMainAction("Workbench.Action.RunPrevious", "Warning"); }
            else { PresentMainAction("Workbench.Action.RunEnded"); }
        }

        private void OnMainToolChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!disposed && e.PropertyName == nameof(OpenVisionWorkbenchToolViewModel.NImageActionKey))
            {
                mainNImageIndex = Tool.NImageIndex; mainNImagePlanned = Tool.NImagePlanned; mainNImagePrefix = Tool.NImagePrefix;
                PresentMainAction(Tool.NImageActionKey, Tool.NImageActionKind, argument: Tool.NImageActionArgument);
            }
            if (!disposed && e.PropertyName == nameof(OpenVisionWorkbenchToolViewModel.ExecutionActionKey))
            {
                PresentMainAction(Tool.ExecutionActionKey, Tool.ExecutionActionKind, argumentKey: Tool.ExecutionActionNameKey, argument: Tool.ExecutionActionName, resultKind: Tool.ExecutionActionResultKind);
            }
            if (!disposed && e.PropertyName == nameof(OpenVisionWorkbenchToolViewModel.RouteActionKey))
            {
                string argumentKey = Tool.RouteActionKey == "Workbench.Action.InputChangedFormat" && string.Equals(Tool.RouteActionName, VisionRecipeRunner.DefaultInputLayer, StringComparison.OrdinalIgnoreCase) ? "Workbench.Pane.Original" : string.Empty;
                PresentMainAction(Tool.RouteActionKey, argumentKey: argumentKey, argument: Tool.RouteActionName);
            }
            if (!disposed && e.PropertyName == nameof(OpenVisionWorkbenchToolViewModel.EditActionKey)) { PresentMainAction(Tool.EditActionKey, Tool.EditActionKey == "Workbench.Status.Error" ? "Error" : ""); }
            if (!disposed && e.PropertyName == nameof(OpenVisionWorkbenchToolViewModel.NewTestActionKey)) { PresentMainAction(Tool.NewTestActionKey, argumentKey: Tool.NewTestToolNameKey); }
            if (!disposed && e.PropertyName == nameof(OpenVisionWorkbenchToolViewModel.AppliedPresetNameKey) && Tool.HasPresetApplied)
            {
                PresentMainAction("Workbench.Tool.PresetAppliedFormat", argumentKey: Tool.AppliedPresetNameKey);
            }
        }

        private void OpenMainLearn()
        {
            if (!OpenMainLearnCommand.CanExecute(null)) { return; }
            if (Tool?.IsOpen == true && OpenVisionLearnTopicCatalog.TryResolveForToolType(Tool.ToolName, out _) && Tool.LearnCommand.CanExecute(null)) { Tool.LearnCommand.Execute(null); }
            else { Chrome.OpenLearnCommand.Execute(null); }
        }

        private void OnLanguageChanged(object sender, EventArgs e)
        {
            OnPropertyChanged(string.Empty);
            OnPropertyChanged(nameof(LayoutStatusText));
            OnPropertyChanged(nameof(DocumentTitle));
        }

        private void OnMainRecipeOpened(object sender, string name)
        {
            if (disposed) { return; }
            recentRecipeNames = new[] { name }.Concat(recentRecipeNames).Where(value => Recipes.RecipeOptions.Contains(value, StringComparer.OrdinalIgnoreCase)).Distinct(StringComparer.OrdinalIgnoreCase).Take(5).ToArray();
            preferenceSnapshot.WorkbenchRecentRecipeNames = recentRecipeNames.ToArray();
            recentRecipeSaveFailed = !savePreferences(preferenceSnapshot);
            OnPropertyChanged(nameof(LayoutStatusText));
            OnPropertyChanged(nameof(HasLayoutStatus));
            OnPropertyChanged(nameof(RecentRecipes));
        }

        private void OnRecipeChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(SavedRecipeRevision));
            if (e.PropertyName is nameof(OpenVisionShellHostRecipeCommandSurface.MainRecipeName) or nameof(OpenVisionShellHostRecipeCommandSurface.RecipeOptions)) { OnPropertyChanged(nameof(RecentRecipes)); }
            if (e.PropertyName == nameof(OpenVisionShellHostRecipeCommandSurface.SavedRevision)) { OnPropertyChanged(nameof(RecipeSaveStatusText)); OnPropertyChanged(nameof(RecipeSaveStatusToolTip)); }
            if (!disposed && e.PropertyName == nameof(OpenVisionShellHostRecipeCommandSurface.SaveActionKey)) { PresentMainAction(Recipes.SaveActionKey, "Ok"); }
            if (!disposed && e.PropertyName == nameof(OpenVisionShellHostRecipeCommandSurface.TransitionActionKey)) { PresentMainAction(Recipes.TransitionActionKey, argument: Recipes.TransitionActionName); }

            if (string.IsNullOrEmpty(e.PropertyName) || e.PropertyName is nameof(OpenVisionShellHostRecipeCommandSurface.MainRecipeName)
                or nameof(OpenVisionShellHostRecipeCommandSurface.SelectedRecipeName)
                or nameof(OpenVisionShellHostRecipeCommandSurface.SelectedPipelineOption)
                or nameof(OpenVisionShellHostRecipeCommandSurface.IsSwitchingRecipe)
                or nameof(OpenVisionShellHostRecipeCommandSurface.IsSelectedStepEditDirty)
                or nameof(OpenVisionShellHostRecipeCommandSurface.IsWorkbenchPipelineDirty)
                or nameof(OpenVisionShellHostRecipeCommandSurface.HasRecipeSaveFailure)
                or nameof(OpenVisionShellHostRecipeCommandSurface.HasSelectedRecipePersistenceFailure)
                or nameof(OpenVisionShellHostRecipeCommandSurface.SelectedRecipePersistenceHelpText)
                or nameof(OpenVisionShellHostRecipeCommandSurface.StatusText))
            {
                OnPropertyChanged(nameof(IsRecipeDirty));
                OnPropertyChanged(nameof(HasRecipeSaveStatus));
                OnPropertyChanged(nameof(RecipeSaveState));
                OnPropertyChanged(nameof(RecipeSaveStatusText));
                OnPropertyChanged(nameof(RecipeSaveStatusToolTip));
            }

            if (IsMainPipelineForm && isMainRecipeOpen && e.PropertyName == nameof(OpenVisionShellHostRecipeCommandSurface.MainRecipeName) && !string.Equals(mainPipelineRecipe, Recipes.MainRecipeName, StringComparison.OrdinalIgnoreCase)) { IsMainRecipeOpen = false; }
            if (isMainPipelineRename && isMainRecipeOpen && e.PropertyName == nameof(OpenVisionShellHostRecipeCommandSurface.SelectedPipelineOption) && !string.Equals(mainPipelineName, Recipes.SelectedPipelineOption?.PipelineName, StringComparison.OrdinalIgnoreCase)) { IsMainRecipeOpen = false; }
            if (isMainValidationOpen && e.PropertyName == nameof(OpenVisionShellHostRecipeCommandSurface.MainRecipeName) && !MainValidationStillCurrent()) { IsMainValidationOpen = false; }
            if (e.PropertyName is nameof(OpenVisionShellHostRecipeCommandSurface.MainRecipeName)
                or nameof(OpenVisionShellHostRecipeCommandSurface.SelectedPipelineOption)
                or nameof(OpenVisionShellHostRecipeCommandSurface.IsSelectedStepEditDirty)
                or nameof(OpenVisionShellHostRecipeCommandSurface.IsWorkbenchPipelineDirty)) { OnPropertyChanged(nameof(DocumentTitle)); }
        }

        public void Dispose()
        {
            if (disposed) { return; }
            IsLayerManagerOpen = false;
            IsShortcutsOpen = false;
            IsMainRecipeOpen = false;
            IsMainValidationOpen = false;
            IsMainLlmOpen = false; mainLlmWorking = Array.Empty<byte>(); mainLlmEditor = null; mainLlmImage = null;
            IsMainHistoryOpen = false;
            mainHistoryRows = Array.Empty<OpenVisionRecipeBatchRunOption>();
            mainValidationSets = Array.Empty<OpenVisionRecipeValidationSetOption>();
            disposed = true;
            if (Shell != null) { Shell.PropertyChanged -= OnMainInputChanged; }
            if (Pipeline != null) { Pipeline.PropertyChanged -= OnMainPipelineChanged; }
            if (Tool != null) { Tool.PropertyChanged -= OnMainToolChanged; }
            Results?.Dispose();
            if (Images != null) { Images.ImportStarted -= OnImportStarted; Images.ImportCompleted -= OnImportCompleted; Images.BatchProgressed -= OnBatchProgressed; Images.BatchFinished -= OnBatchFinished; }
            Images?.Dispose();
            if (Viewer != null) { Viewer.LayoutChanged -= OnViewerLayoutChanged; }
            Viewer?.Dispose();
            Tool?.Dispose();
            Pipeline?.Dispose();
            OpenVisionLanguageService.LanguageChanged -= OnLanguageChanged;
            if (Recipes != null) { Recipes.PropertyChanged -= OnRecipeChanged; Recipes.MainRecipeOpened -= OnMainRecipeOpened; }
        }
    }
}
