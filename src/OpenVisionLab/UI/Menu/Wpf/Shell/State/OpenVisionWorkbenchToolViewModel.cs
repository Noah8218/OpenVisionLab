using MahApps.Metro.IconPacks;
using OpenVisionLab.Core;
using OpenVisionLab.Mvvm;
using OpenVisionLab.Vision2D.Pipeline;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows.Input;

namespace OpenVisionLab
{
    public sealed class OpenVisionWorkbenchToolViewModel : ObservableObject, IDisposable, IDataErrorInfo
    {
        private readonly OpenVisionWorkbenchPipelineViewModel pipeline;
        private readonly Dictionary<string, TestDraft> tests = new Dictionary<string, TestDraft>(StringComparer.Ordinal);
        private readonly Dictionary<string, string> acceptanceInputs = new Dictionary<string, string>(StringComparer.Ordinal);
        private IReadOnlyList<KeyValuePair<string, string>> acceptanceMetrics = Array.Empty<KeyValuePair<string, string>>();
        private string acceptanceMetricsKey = string.Empty;
        private EditorBinding editor;
        private TestDraft test;
        private VisionPipelineStep baseline;
        private string contextKey = string.Empty;
        private string statusText = string.Empty;
        private string appliedPresetNameKey = string.Empty;
        private string nImageActionKey = string.Empty;
        private string nImageActionKind = string.Empty;
        private string nImageActionArgument = string.Empty;
        private string routeActionKey = string.Empty;
        private string routeActionName = string.Empty;
        private string editActionKey = string.Empty;
        private string newTestActionKey = string.Empty;
        private string newTestToolNameKey = string.Empty;
        private string executionActionKey = string.Empty;
        private string executionActionKind = string.Empty;
        private string executionActionName = string.Empty;
        private string executionActionNameKey = string.Empty;
        private string executionActionResultKind = string.Empty;
        private bool templateFailed;
        private int stepIndex = -1;
        private long generation;
        private bool changing;
        private bool disposed;
        private Func<bool> getAutoRunPreference = () => true;
        private Action<bool> saveAutoRunPreference;
        private byte[] observedStep = Array.Empty<byte>();

        internal OpenVisionWorkbenchToolViewModel(OpenVisionWorkbenchPipelineViewModel pipeline)
        {
            this.pipeline = pipeline ?? throw new ArgumentNullException(nameof(pipeline));
            pipeline.CanCommitToolEdits = () => !IsStepEditing || !HasAcceptanceError;
            pipeline.CommitToolEdits = CommitPendingEdits;
            pipeline.PropertyChanged += OnPipelineChanged;
            pipeline.StepSelected += OnStepSelected;
            pipeline.NewTestRequested += OnNewTestRequested;
            OpenVisionLanguageService.LanguageChanged += OnLanguageChanged;
            RestoreDefaultsCommand = new RelayCommand(RestoreDefaults, () => IsNewTest && CanEditParameters && editor.CreateDefaults != null);
            NewTestCommand = new RelayCommand(NewTestFromCurrent, () => IsStepEditing && CanEditParameters && OpenTestRequested != null);
            RenameCommand = new RelayCommand(() => ExecuteStepCommand(pipeline.RenameCommand), () => IsStepEditing && CanEditParameters && pipeline.RenameCommand.CanExecute(null));
            DuplicateCommand = new RelayCommand(() => ExecuteStepCommand(pipeline.DuplicateCommand), () => IsStepEditing && CanEditParameters && pipeline.DuplicateCommand.CanExecute(null));
            DeleteCommand = new RelayCommand(() => ExecuteStepCommand(pipeline.DeleteCommand), () => IsStepEditing && CanEditParameters && pipeline.DeleteCommand.CanExecute(null));
            ParameterGuideCommand = new RelayCommand(() => editor?.ShowParameterGuide?.Invoke(), () => IsOpen && !disposed && editor.ShowParameterGuide != null);
            LearnCommand = new RelayCommand(() => OpenLearnRequested?.Invoke(ToolName), () => IsOpen && !disposed && OpenLearnRequested != null);
            RevertCommand = new RelayCommand(() => RevertCurrent(generation), () => IsStepEditing && CanEditParameters && pipeline.CanRevertSavedStep(stepIndex));
            RunCommand = new RelayCommand(() => { if (RunCommand.CanExecute(null)) { editor.Run.Invoke(); } }, () => CanUseToolActions && !pipeline.HasPersistenceFailure && editor.Run != null);
            RunSelectedStepCommand = new RelayCommand<object>(RunSelectedStep, parameter => parameter is int index && CanChangeSelection && !pipeline.HasPersistenceFailure && OpenStepRequested != null && pipeline.SelectStepCommand.CanExecute(index)
                && (!IsStepEditing || stepIndex != index || RunCommand.CanExecute(null)));
            RunToCommand = new RelayCommand(() => ExecuteStepCommand(pipeline.RunToSelectedCommand), () => IsStepEditing && CanUseToolActions && pipeline.RunToSelectedCommand.CanExecute(null));
            AddAfterCommand = new RelayCommand(() => AddCurrent(generation), () => CanAddToPipeline);
            AddBeforeCommand = new RelayCommand(() => AddCurrent(generation, 1), () => CanAddToPipeline && pipeline.SelectedStepIndex >= 0);
            AddEndCommand = new RelayCommand(() => AddCurrent(generation, 2), () => CanAddToPipeline);
            CancelAcceptanceEditCommand = new RelayCommand<string>(name => { acceptanceInputs.Remove(name); EditAcceptance(_ => { }); }, _ => HasAcceptance && CanEditParameters);
            DrawTemplateCommand = new RelayCommand(() => ChangeTemplate(false), () => HasTemplateCard && CanEditParameters);
            TemplateFileCommand = new RelayCommand(() => ChangeTemplate(true), () => HasTemplateCard && CanEditParameters);
        }

        public bool IsOpen => editor != null;
        public bool IsStepEditing => IsOpen && stepIndex >= 0;
        public bool IsNewTest => IsOpen && stepIndex < 0;
        public bool IsMainDraftVisible => IsNewTest && editor.SupportsPipeline;
        public string MainDraftRouteText => IsMainDraftVisible ? (string.Equals(InputLayer, VisionRecipeRunner.DefaultInputLayer, StringComparison.OrdinalIgnoreCase) ? this["Workbench.Pane.Original"] : InputLayer) + " → " + OutputLayer : string.Empty;
        public string ToolName => editor?.ToolName ?? string.Empty;
        internal string DisplayName => IsOpen ? editor.DisplayName() : string.Empty;
        public string Title => IsStepEditing ? (stepIndex + 1).ToString("00", CultureInfo.InvariantCulture) + " · " + (pipeline.Rows.ElementAtOrDefault(stepIndex)?.Name ?? string.Empty)
            : IsOpen ? string.Format(CultureInfo.CurrentCulture, this["Workbench.Tool.NewFormat"], editor.DisplayName()) : string.Empty;
        public string Subtitle
        {
            get
            {
                if (!IsOpen) { return string.Empty; }
                string displayName = editor.DisplayName();
                return string.Equals(displayName, editor.EnglishName, StringComparison.Ordinal) ? displayName : displayName + " · " + editor.EnglishName;
            }
        }
        public PackIconMaterialKind Icon => editor?.Icon ?? PackIconMaterialKind.Tools;
        public bool IsChanged => IsStepEditing && pipeline.Rows.ElementAtOrDefault(stepIndex)?.IsDirty == true;
        public string BadgeKind => IsStepEditing ? IsChanged ? "Changed" : "Saved" : "Testing";
        public PackIconMaterialKind BadgeIcon => IsStepEditing ? IsChanged ? PackIconMaterialKind.Circle : PackIconMaterialKind.CheckCircleOutline : PackIconMaterialKind.TestTube;
        public string RunText => this[IsStepEditing ? "Workbench.Tool.RunStep" : "Workbench.Tool.RunTool"];
        public bool CanUseToolActions => CanEditParameters && !HasAcceptanceError;
        public bool CanAddToPipeline => IsNewTest && CanUseToolActions && editor.SupportsPipeline;
        public string AddAfterText => this[pipeline.SelectedStepIndex >= 0 ? "Workbench.Tool.AddAfter" : "Workbench.Tool.AddEnd"];
        public string AddTooltip => this[editor?.SupportsPipeline == true ? "Workbench.Tool.AddTooltip" : "Workbench.Tool.NotAddableTooltip"];
        public string BadgeText => this[IsStepEditing ? IsChanged
            ? "Workbench.Tool.BadgeChanged" : "Workbench.Tool.BadgeSaved" : "Workbench.Tool.BadgeTesting"];
        public string StatusText => statusText;
        public bool HasStatus => !string.IsNullOrWhiteSpace(statusText);
        internal int NImageIndex { get; private set; }
        internal int NImagePlanned { get; private set; }
        internal int NImagePrefix { get; private set; }
        internal Func<IReadOnlyList<string>> NImagePaths { get; set; }
        public string NImageActionKey => nImageActionKey;
        public string NImageActionKind => nImageActionKind;
        public string NImageActionArgument => nImageActionArgument;
        public string NImageToolTip
        {
            get
            {
                int count = NImagePaths?.Invoke().Count ?? 0;
                string text = string.Format(CultureInfo.CurrentCulture, this["Workbench.Tool.NImageCountFormat"], count);
                try
                {
                    var snapshot = pipeline.CaptureNImagePipeline(CurrentStep);
                    if (snapshot.Steps.Count > 1) { text += string.Format(CultureInfo.CurrentCulture, this["Workbench.Tool.NImagePrefixFormat"], string.Join(" → ", snapshot.Steps.Take(snapshot.Steps.Count - 1).Select(step => step.Name))); }
                }
                catch (InvalidOperationException) { text += " · " + this["Workbench.Tool.Unavailable"]; }
                catch (ArgumentNullException) { }
                return text;
            }
        }
        internal IReadOnlyList<string> NImageExternalInputLayers(VisionPipeline frozen) => pipeline.NImageExternalInputLayers(frozen);
        internal string NImageContextKey => pipeline.ContextKey;
        internal VisionPipeline CaptureNImagePipeline(VisionPipelineStep step) => pipeline.CaptureNImagePipeline(step);
        internal void SetNImageRunning(long token, bool value, Action stop)
        {
            if (!value || IsActive(token)) { pipeline.SetNImageRunning(value, stop); }
        }
        internal void RequestNImageStop(long token)
        {
            if (IsActive(token) && pipeline.StopCommand.CanExecute(null)) { pipeline.StopCommand.Execute(null); }
        }
        internal void NotifyNImageAction(long token, string key, string kind, string argument, int index = 0, int planned = 0, int prefix = 0)
        {
            if (!IsActive(token)) { return; }
            NImageIndex = index; NImagePlanned = planned; NImagePrefix = prefix;
            nImageActionKey = key;
            nImageActionKind = kind;
            nImageActionArgument = argument;
            OnPropertyChanged(nameof(NImageActionKey));
        }
        public string RouteActionKey => routeActionKey;
        public string RouteActionName => routeActionName;
        public string EditActionKey => editActionKey;
        public string NewTestActionKey => newTestActionKey;
        public string NewTestToolNameKey => newTestToolNameKey;
        public string ExecutionActionKey => executionActionKey;
        public string ExecutionActionKind => executionActionKind;
        public string ExecutionActionName => executionActionName;
        public string ExecutionActionNameKey => executionActionNameKey;
        public string ExecutionActionResultKind => executionActionResultKind;
        public string AppliedPresetNameKey => appliedPresetNameKey;
        public bool HasPresetApplied => IsOpen && !string.IsNullOrEmpty(appliedPresetNameKey);
        public string PresetAppliedText => HasPresetApplied ? string.Format(CultureInfo.CurrentCulture, this["Workbench.Tool.PresetAppliedFormat"], this[appliedPresetNameKey]) : string.Empty;
        public bool HasTemplateCard => IsOpen && editor.GetTemplateStatus != null;
        public bool HasTemplateError => HasTemplateCard && templateFailed;
        public string TemplateErrorText => this["Workbench.Status.Error"];
        public bool HasTemplate => editor?.GetTemplateStatus?.Invoke()?.IsReady == true;
        public System.Windows.Media.ImageSource TemplateThumbnail => editor?.GetTemplateThumbnail?.Invoke();
        public string TemplatePath => editor?.GetTemplateStatus?.Invoke()?.Path ?? string.Empty;
        public string TemplateInfo
        {
            get
            {
                OpenVisionLab.Contracts.VisionToolTemplateStatus status = editor?.GetTemplateStatus?.Invoke();
                return status?.IsReady == true ? status.Width.ToString(CultureInfo.InvariantCulture) + "×"
                    + status.Height.ToString(CultureInfo.InvariantCulture) + " px · " + System.IO.Path.GetFileName(status.Path)
                    : this["Workbench.Tool.TemplateMissing"];
            }
        }
        public string DrawTemplateText => this[HasTemplate ? "Workbench.Tool.TemplateRedraw" : "Workbench.Tool.TemplateDraw"];
        internal VisionPipelineStepResultSummary CurrentResult => IsStepEditing ? pipeline.GetResult(stepIndex) : test?.Result;
        public bool HasResult => CurrentResult != null;
        public bool IsResultPrevious => HasResult && (IsStepEditing ? pipeline.IsResultPrevious(stepIndex) : test.ResultPrevious || test.InputRevision != pipeline.InputRevision);
        public string ResultKind => editor?.IsRunning() == true ? "Running" : IsResultPrevious ? "Previous"
            : OpenVisionPipelineReviewResultPresenter.WorkbenchResultKind(CurrentStep, CurrentResult);
        public bool SupportsAutoRun => IsOpen && ToolName.ToLowerInvariant() is "threshold" or "filter" or "morphology" or "arithmetic"
            or "edgedetection" or "rotatescale" or "hsv" or "histogram" or "mean" or "matching";
        public bool HasAutoRunOption => SupportsAutoRun && (ToolName != "Matching" || editor.AutoPreviewOptedIn?.Invoke() == true);
        public bool CanToggleAutoRun => SupportsAutoRun && CanEditParameters;
        public bool IsAutoRunEnabled
        {
            get => getAutoRunPreference();
            set { if (CanToggleAutoRun && value != getAutoRunPreference()) { saveAutoRunPreference?.Invoke(value); NotifyAutoRunPreferenceChanged(); } }
        }
        public string ResultStatusText => this[ResultKind == "NotRun" && !HasResult ? "PipelineReview.Execution.NotRun" : "Workbench.Status." + ResultKind];
        public string ResultElapsedText => CurrentResult?.Executed == true && !CurrentResult.Skipped
            ? OpenVisionPipelineReviewResultPresenter.FormatWorkbenchElapsed(CurrentResult.ElapsedMilliseconds) : string.Empty;
        public string ResultHint => IsResultPrevious ? this["Workbench.Result.Previous"] : !HasResult ? this[IsStepEditing ? "Workbench.Result.NotRunStep" : "Workbench.Result.NotRunTool"]
            : OpenVisionPipelineReviewResultPresenter.FormatWorkbenchNextAction(CurrentResult);
        public bool HasResultHint => !string.IsNullOrEmpty(ResultHint);
        public string ResultSummaryText => OpenVisionPipelineReviewResultPresenter.FormatWorkbenchSummary(CurrentResult);
        public IReadOnlyList<KeyValuePair<string, string>> ResultMetrics => !HasResult ? Array.Empty<KeyValuePair<string, string>>()
            : OpenVisionPipelineReviewResultPresenter.OrderResultMetrics(CurrentStep, CurrentResult.Metrics)
                .Concat(CurrentResult.Metrics.Where(metric => metric.Key == VisionPipelineKnownMetrics.ResultImageWidth))
                .OrderBy(metric => OpenVisionLanguageService.T("Workbench.Metric." + metric.Key) == "Workbench.Metric." + metric.Key ? 1 : 0).Take(6)
                .Select(metric => new KeyValuePair<string, string>(OpenVisionPipelineReviewResultPresenter.WorkbenchMetricName(metric.Key),
                    OpenVisionPipelineReviewResultPresenter.WorkbenchMetricValue(metric.Key, metric.Value))).ToArray();
        public string ResultReason => CurrentResult != null && (!CurrentResult.Success || CurrentResult.IsAcceptanceNg)
            ? OpenVisionPipelineReviewResultPresenter.FormatWorkbenchFailure(CurrentStep, CurrentResult) : string.Empty;
        public bool HasResultReason => !string.IsNullOrEmpty(ResultReason);
        public bool HasAcceptance => AcceptanceMetrics.Count > 0;
        public bool CanEditAcceptance => HasAcceptance && UseAcceptance && CanEditParameters;
        public bool CanToggleAcceptance => HasAcceptance && CanEditParameters;
        public bool CanEditAcceptanceMinimum => CanEditAcceptance && UseAcceptanceMinimum;
        public bool CanEditAcceptanceMaximum => CanEditAcceptance && UseAcceptanceMaximum;
        public bool HasAcceptanceError => !string.IsNullOrEmpty(AcceptanceError);
        public string AcceptanceError => !UseAcceptance ? string.Empty : new[]
            { nameof(AcceptanceMinimumText), nameof(AcceptanceMaximumText), nameof(AcceptanceTimeLimitText) }
            .Select(AcceptanceInputError).FirstOrDefault(error => !string.IsNullOrEmpty(error)) ?? string.Empty;
        string IDataErrorInfo.Error => AcceptanceError;
        string IDataErrorInfo.this[string name] => AcceptanceInputError(name);
        public IReadOnlyList<KeyValuePair<string, string>> AcceptanceMetrics
        {
            get
            {
                string key = ToolName + "|" + CurrentStep?.ToolType + "|" + OpenVisionLanguageService.CurrentLanguage;
                if (key == acceptanceMetricsKey) { return acceptanceMetrics; }
                string[] names = ToolName.ToLowerInvariant() switch
                {
                    "blob" or "contour" => new[] { "ResultCount", "AreaMax", "AreaMin", "AreaAvg", "BoundsWidthMax", "BoundsHeightMax" },
                    "matching" or "edgebasedmatching" or "featurematching" => new[] { "ScoreMax", "ResultCount" },
                    "mean" => new[] { "MeanValueAvg" },
                    "hsv" => new[] { "MaskPixelRatio", "MaskPixelCount" },
                    "line" => new[] { "EdgeCount", "LineLengthMax", "DistancePxAvg", "DistanceMmAvg", "DistanceMmRange", "IntersectionX", "IntersectionY" },
                    _ => Array.Empty<string>()
                };
                IReadOnlyList<string> produced = VisionPipelineKnownMetrics.GetMetricNamesForTool(CurrentStep?.ToolType ?? ToolName);
                acceptanceMetricsKey = key;
                acceptanceMetrics = names.Where(name => produced.Contains(name, StringComparer.OrdinalIgnoreCase)).Select(name =>
                {
                    string unit = OpenVisionPipelineReviewResultPresenter.WorkbenchMetricFormat(name).Unit;
                    string label = OpenVisionPipelineReviewResultPresenter.WorkbenchMetricName(name);
                    return new KeyValuePair<string, string>(name, string.IsNullOrEmpty(unit) || unit == "count" ? label : label + " (" + unit + ")");
                }).ToArray();
                return acceptanceMetrics;
            }
        }
        public bool UseAcceptance
        {
            get => CurrentStep?.UseAcceptance == true;
            set
            {
                if (!CanToggleAcceptance || value == UseAcceptance) { return; }
                EditAcceptance(step =>
                {
                    step.UseAcceptance = value;
                    if (value && string.IsNullOrEmpty(step.AcceptanceMetricName)) { step.AcceptanceMetricName = AcceptanceMetrics.First().Key; }
                });
            }
        }
        public string AcceptanceMetricName
        {
            get => string.IsNullOrEmpty(CurrentStep?.AcceptanceMetricName) ? AcceptanceMetrics.FirstOrDefault().Key : CurrentStep.AcceptanceMetricName;
            set
            {
                if (CanEditAcceptance && AcceptanceMetrics.Any(metric => metric.Key == value) && value != CurrentStep.AcceptanceMetricName)
                    EditAcceptance(step => step.AcceptanceMetricName = value);
            }
        }
        public KeyValuePair<string, string>? SelectedAcceptanceMetric
        {
            get
            {
                KeyValuePair<string, string> metric = AcceptanceMetrics.FirstOrDefault(item => item.Key == AcceptanceMetricName);
                return string.IsNullOrEmpty(metric.Key) ? null : metric;
            }
            set { if (value.HasValue) { AcceptanceMetricName = value.Value.Key; } }
        }
        public string AcceptanceUnit
        {
            get
            {
                string unit = OpenVisionPipelineReviewResultPresenter.WorkbenchMetricFormat(AcceptanceMetricName).Unit;
                return unit == "count" ? this["Workbench.Unit.Count"] : unit;
            }
        }
        public bool UseAcceptanceMinimum
        {
            get => CurrentStep?.UseAcceptanceMetricMinimum == true;
            set { if (CanEditAcceptance && value != UseAcceptanceMinimum) { EditAcceptance(step => step.UseAcceptanceMetricMinimum = value); } }
        }
        public bool UseAcceptanceMaximum
        {
            get => CurrentStep?.UseAcceptanceMetricMaximum == true;
            set { if (CanEditAcceptance && value != UseAcceptanceMaximum) { EditAcceptance(step => step.UseAcceptanceMetricMaximum = value); } }
        }
        public string AcceptanceMinimumText
        {
            get => AcceptanceInput(nameof(AcceptanceMinimumText), CurrentStep?.AcceptanceMetricMinimum ?? 0D);
            set => SetAcceptanceInput(nameof(AcceptanceMinimumText), value, CanEditAcceptanceMinimum);
        }
        public string AcceptanceMaximumText
        {
            get => AcceptanceInput(nameof(AcceptanceMaximumText), CurrentStep?.AcceptanceMetricMaximum ?? 0D);
            set => SetAcceptanceInput(nameof(AcceptanceMaximumText), value, CanEditAcceptanceMaximum);
        }
        public string AcceptanceTimeLimitText
        {
            get => AcceptanceInput(nameof(AcceptanceTimeLimitText), CurrentStep?.MaxElapsedMilliseconds ?? 0D);
            set => SetAcceptanceInput(nameof(AcceptanceTimeLimitText), value, CanEditAcceptance);
        }
        public string this[string key] => OpenVisionLanguageService.T(key);
        public ICommand RenameCommand { get; }
        public ICommand DuplicateCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand RestoreDefaultsCommand { get; }
        public ICommand NewTestCommand { get; }
        public ICommand ParameterGuideCommand { get; }
        public ICommand LearnCommand { get; }
        public ICommand RevertCommand { get; }
        public ICommand RunCommand { get; }
        public ICommand RunSelectedStepCommand { get; }
        public ICommand RunToCommand { get; }
        public ICommand AddAfterCommand { get; }
        public ICommand AddBeforeCommand { get; }
        public ICommand AddEndCommand { get; }
        public ICommand CancelAcceptanceEditCommand { get; }
        public ICommand DrawTemplateCommand { get; }
        public ICommand TemplateFileCommand { get; }
        public string LearnText => IsOpen ? string.Format(CultureInfo.CurrentCulture, this["Workbench.Tool.LearnFormat"], editor.DisplayName()) : string.Empty;
        internal Action<int> OpenStepRequested { get; set; }
        internal Action<string> OpenTestRequested { get; set; }
        internal Action<string> OpenLearnRequested { get; set; }
        internal Action RefreshRoute { get; set; }
        public bool CanEditParameters => IsOpen && !disposed && !changing && pipeline.CanEdit && !editor.IsRunning();
        internal bool CanChangeSelection => !disposed && pipeline.CanEdit && (editor == null || !editor.IsRunning());
        internal string InputLayer => CurrentStep?.InputLayer ?? VisionRecipeRunner.DefaultInputLayer;
        internal string InputLayerB => CurrentStep is VisionPipelineStep step && step.Parameters.TryGetValue("InputLayerB", out string value) ? value : string.Empty;
        internal string OutputLayer => CurrentStep?.OutputLayer ?? string.Empty;
        internal VisionPipelineStep CurrentStep => IsStepEditing ? pipeline.GetStep(stepIndex) : test?.Step;
        internal string GetStepToolType(int index) => pipeline.GetStep(index)?.ToolType;
        internal bool IsActive(long token) => !disposed && token == generation && editor != null;
        internal bool IsEditingSession(long token, int index) => IsActive(token) && !changing && stepIndex == index
            && string.Equals(contextKey, pipeline.ContextKey, StringComparison.Ordinal)
            && (index < 0 || observedStep.SequenceEqual(SerializeStep(pipeline.GetStep(index))));
        internal bool TryAcquireInputSnapshot(string layerName, out System.Drawing.Bitmap snapshot) => pipeline.TryAcquireResultSnapshot(layerName, out snapshot);

        internal IReadOnlyList<KeyValuePair<string, string>> GetInputLayerChoices(IEnumerable<string> openedLayers)
        {
            List<KeyValuePair<string, string>> choices = new List<KeyValuePair<string, string>>
                { new KeyValuePair<string, string>(VisionRecipeRunner.DefaultInputLayer, this["Workbench.Pane.Original"]) };
            HashSet<string> names = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { VisionRecipeRunner.DefaultInputLayer };
            int beforeIndex = IsStepEditing ? stepIndex : pipeline.SelectedStepIndex >= 0 ? pipeline.SelectedStepIndex + 1 : pipeline.StepCount;
            for (int index = 0; index < beforeIndex; index++)
            {
                VisionPipelineStep step = pipeline.GetStep(index);
                if (step?.Enabled == true && !string.IsNullOrWhiteSpace(step.OutputLayer) && names.Add(step.OutputLayer))
                {
                    choices.Add(new KeyValuePair<string, string>(step.OutputLayer, (index + 1).ToString("00", CultureInfo.InvariantCulture) + " · " + step.OutputLayer));
                }
            }
            HashSet<string> pipelineOutputs = new HashSet<string>(Enumerable.Range(0, pipeline.StepCount)
                .Select(index => pipeline.GetStep(index)?.OutputLayer).Where(layer => !string.IsNullOrWhiteSpace(layer)), StringComparer.OrdinalIgnoreCase);
            foreach (string layer in openedLayers ?? Array.Empty<string>())
            {
                if (!string.IsNullOrWhiteSpace(layer) && !string.Equals(layer, OutputLayer, StringComparison.OrdinalIgnoreCase) && !pipelineOutputs.Contains(layer) && names.Add(layer))
                {
                    choices.Add(new KeyValuePair<string, string>(layer, layer));
                }
            }
            foreach (string current in new[] { InputLayer, InputLayerB }.Distinct(StringComparer.OrdinalIgnoreCase))
            {
                if ((current == InputLayer || !string.IsNullOrWhiteSpace(current) || VisionPipelineArithmeticStep.RequiresInputLayerB(CurrentStep)) && names.Add(current ?? string.Empty))
                {
                    choices.Add(new KeyValuePair<string, string>(current ?? string.Empty, this["Workbench.Tool.Unavailable"] + " " + current));
                }
            }
            return choices;
        }

        internal void ConfigureAutoRunPreference(Func<bool> get, Action<bool> save)
        {
            getAutoRunPreference = get ?? throw new ArgumentNullException(nameof(get));
            saveAutoRunPreference = save ?? throw new ArgumentNullException(nameof(save));
            NotifyAutoRunPreferenceChanged();
        }
        internal void NotifyAutoRunPreferenceChanged() => OnPropertyChanged(nameof(IsAutoRunEnabled));

        internal void NotifyPresetApplied(string displayNameKey)
        {
            if (!CanEditParameters || string.IsNullOrWhiteSpace(displayNameKey)) { return; }
            appliedPresetNameKey = displayNameKey;
            RaiseState();
            OnPropertyChanged(nameof(AppliedPresetNameKey));
        }

        internal void NotifyExecutionState(long token)
        {
            if (IsActive(token)) { RaiseState(); }
        }

        internal void NotifyExecutionStarted(long token, VisionPipelineStep step)
        {
            if (!IsActive(token) || step == null) { return; }
            PresentExecutionAction(IsStepEditing ? "Workbench.Action.StepRunStartedFormat" : "Workbench.Action.ToolRunStartedFormat", "Running",
                IsStepEditing ? step.Name : string.Empty, IsStepEditing ? string.Empty : newTestToolNameKey);
        }

        internal void PublishResult(long token, VisionPipelineStepResultSummary summary, VisionPipelineStep executedStep = null)
        {
            if (!IsActive(token) || summary == null) { return; }
            bool stepEditing = IsStepEditing;
            VisionPipelineStep step = executedStep ?? CurrentStep;
            string name = stepEditing ? step?.Name ?? string.Empty : string.Empty;
            string nameKey = stepEditing ? string.Empty : newTestToolNameKey;
            string resultKind = OpenVisionPipelineReviewResultPresenter.WorkbenchResultKind(step, summary);
            appliedPresetNameKey = string.Empty;
            if (stepEditing) { pipeline.PublishToolResult(stepIndex, summary); }
            else { test.Result = summary; test.ResultPrevious = false; test.InputRevision = pipeline.InputRevision; }
            RaiseState();
            if (!IsActive(token)) { return; }
            if (resultKind is "Cancelled" or "Disabled" or "NotRun") { PresentExecutionAction("Workbench.Action.RunEnded", "Warning", string.Empty, string.Empty); }
            else
            {
                string kind = resultKind == "Error" ? "Error" : stepEditing ? resultKind == "Ng" ? "Ng" : "Ok" : string.Empty;
                PresentExecutionAction(stepEditing ? "Workbench.Action.StepRunCompletedFormat" : "Workbench.Action.ToolRunCompletedFormat", kind, name, nameKey, resultKind);
            }
        }

        private void PresentExecutionAction(string key, string kind, string name, string nameKey, string resultKind = "")
        {
            executionActionKey = key;
            executionActionKind = kind;
            executionActionName = name;
            executionActionNameKey = nameKey;
            executionActionResultKind = resultKind;
            OnPropertyChanged(nameof(ExecutionActionKey));
        }

        internal void InvalidateResult(long token)
        {
            if (!IsActive(token)) { return; }
            if (IsStepEditing) { pipeline.InvalidateToolResult(stepIndex); }
            else if (test.Result != null) { test.ResultPrevious = true; }
            RaiseState();
        }

        internal long Activate(OpenVisionRecipeContext context, EditorBinding binding, int selectedStep = -1)
        {
            if (disposed) { throw new ObjectDisposedException(nameof(OpenVisionWorkbenchToolViewModel)); }
            changing = true;
            try
            {
                editor = binding ?? throw new ArgumentNullException(nameof(binding));
                contextKey = pipeline.ContextKey;
                stepIndex = selectedStep;
                generation++;
                statusText = string.Empty;
                appliedPresetNameKey = string.Empty;
                templateFailed = false;
                acceptanceInputs.Clear();
                if (selectedStep >= 0)
                {
                    VisionPipelineStep step = pipeline.GetStep(selectedStep) ?? throw new ArgumentOutOfRangeException(nameof(selectedStep));
                    editor.ApplyStep(step);
                    baseline = editor.CreateStep();
                    observedStep = SerializeStep(step);
                    test = null;
                }
                else
                {
                    string key = contextKey + "\n" + binding.ToolName;
                    if (!tests.TryGetValue(key, out test))
                    {
                        object parameters = editor.LoadParameters(context);
                        editor.RestoreParameters(parameters);
                        test = new TestDraft { Parameters = editor.CaptureParameters(), Step = editor.CreateStep() ?? new VisionPipelineStep { ToolType = binding.ToolName } };
                        test.Step.Name = editor.DisplayName();
                        test.Step.InputLayer = pipeline.GetStep(pipeline.SelectedStepIndex)?.OutputLayer ?? VisionRecipeRunner.DefaultInputLayer;
                        test.Step.OutputLayer = CreateTestOutput(binding.ToolName);
                        editor.InitializeTestRoute?.Invoke(test.Step);
                        tests.Add(key, test);
                    }
                    else { editor.RestoreParameters(test.Parameters); }
                    baseline = editor.CreateStep();
                }
            }
            finally { changing = false; }
            RaiseState();
            if (selectedStep < 0)
            {
                newTestActionKey = editor.SupportsPipeline ? "Workbench.Action.ToolTestOpenedFormat" : "Workbench.Action.ToolTestOpenedRunOnlyFormat";
                newTestToolNameKey = "VisionMenu." + (binding.ToolName == "RotateScale" ? "RotateAndScale" : binding.ToolName);
                OnPropertyChanged(nameof(NewTestActionKey));
            }
            return generation;
        }

        internal void Deactivate(long token)
        {
            if (!IsActive(token)) { return; }
            editor = null;
            test = null;
            baseline = null;
            stepIndex = -1;
            generation++;
            RefreshRoute = null;
            RaiseState();
        }

        internal void ParametersChanged(long token)
        {
            if (!IsActive(token) || changing || !pipeline.CanEdit) { return; }
            changing = true;
            try
            {
                VisionPipelineStep captured = editor.CreateStep();
                if (IsNewTest)
                {
                    byte[] previous = SerializeStep(test.Step);
                    test.Parameters = editor.CaptureParameters();
                    if (captured != null) { MergeParameters(test.Step, baseline, captured); }
                    if (!previous.SequenceEqual(SerializeStep(test.Step)) && test.Result != null) { test.ResultPrevious = true; }
                }
                else if (captured != null)
                {
                    VisionPipelineStep next = pipeline.GetStep(stepIndex);
                    if (next != null)
                    {
                        MergeParameters(next, baseline, captured);
                        pipeline.ReplaceStep(stepIndex, next);
                        observedStep = SerializeStep(pipeline.GetStep(stepIndex));
                    }
                }
                baseline = captured;
            }
            finally { changing = false; }
            RaiseState();
        }

        internal bool CommitPendingEdits()
        {
            if (!IsOpen) { return true; }
            if (editor.CommitPendingEdits?.Invoke() == false) { return false; }
            ParametersChanged(generation);
            return !IsStepEditing || !HasAcceptanceError;
        }

        internal bool SetRoute(long token, string input, string inputB, string output)
        {
            if (!IsActive(token) || changing || !pipeline.CanEdit) { return false; }
            string error = pipeline.ValidateOutput(output, stepIndex);
            if (!string.IsNullOrEmpty(error))
            {
                statusText = error;
                RaiseState();
                return false;
            }

            string actionKey = string.Empty, actionName = string.Empty;
            changing = true;
            try
            {
                VisionPipelineStep next = IsStepEditing ? pipeline.GetStep(stepIndex) : test.Step;
                byte[] previous = SerializeStep(next);
                string previousInput = next.InputLayer, previousOutput = next.OutputLayer;
                string previousInputB = next.Parameters.TryGetValue("InputLayerB", out string value) ? value : string.Empty;
                next.InputLayer = input ?? VisionRecipeRunner.DefaultInputLayer;
                next.OutputLayer = output.Trim();
                if (string.Equals(ToolName, "Arithmetic", StringComparison.OrdinalIgnoreCase)) { next.Parameters["InputLayerB"] = inputB ?? string.Empty; }
                if (IsStepEditing) { pipeline.ReplaceStep(stepIndex, next); }
                else if (!previous.SequenceEqual(SerializeStep(next)) && test.Result != null) { test.ResultPrevious = true; }
                if (!string.Equals(previousOutput, next.OutputLayer, StringComparison.Ordinal))
                {
                    actionKey = "Workbench.Action.OutputChangedFormat";
                    actionName = next.OutputLayer;
                }
                else if (string.Equals(ToolName, "Arithmetic", StringComparison.OrdinalIgnoreCase) && !string.Equals(previousInputB, next.Parameters["InputLayerB"], StringComparison.Ordinal))
                {
                    actionKey = "Workbench.Action.InputChangedFormat";
                    actionName = next.Parameters["InputLayerB"];
                }
                else if (!string.Equals(previousInput, next.InputLayer, StringComparison.Ordinal))
                {
                    actionKey = "Workbench.Action.InputChangedFormat";
                    actionName = next.InputLayer;
                }
                statusText = string.Empty;
            }
            finally { changing = false; }
            RaiseState();
            if (!string.IsNullOrEmpty(actionKey))
            {
                routeActionKey = actionKey;
                routeActionName = actionName;
                OnPropertyChanged(nameof(RouteActionKey));
            }
            return true;
        }

        internal VisionPipelineStep AddCurrent(long token, int position = 0)
        {
            if (!IsActive(token) || !IsNewTest || !CanUseToolActions) { return null; }
            if (position == 1 && pipeline.SelectedStepIndex < 0) { return null; }
            if (!CommitPendingEdits()) { return null; }
            if (editor.CreateStep() == null) { statusText = this["Workbench.Tools.NotAddable"]; RaiseState(); return null; }
            if (!editor.TemplateReady()) { statusText = this["Workbench.Tool.TemplateMissing"]; RaiseState(); return null; }
            ParametersChanged(token);
            string error = pipeline.ValidateOutput(test.Step.OutputLayer);
            if (!string.IsNullOrEmpty(error)) { statusText = error; RaiseState(); return null; }
            VisionPipelineStep added;
            changing = true;
            try
            {
                VisionPipelineStep step = VisionPipelineStepEditService.CopyStep(test.Step);
                step.Name = editor.DisplayName();
                int selection = pipeline.SelectedStepIndex;
                int insertion = selection < 0 || position == 2 ? pipeline.StepCount : position == 1 ? selection : selection + 1;
                added = pipeline.InsertStep(insertion, step);
                if (added != null && test.Result != null) { pipeline.PublishToolResult(insertion, test.Result, IsResultPrevious); }
            }
            finally { changing = false; }
            if (added != null) { OpenStepRequested?.Invoke(pipeline.SelectedStepIndex); }
            RaiseState();
            if (added != null) { editActionKey = "Workbench.Action.StepAdded"; OnPropertyChanged(nameof(EditActionKey)); }
            return added;
        }

        internal bool RevertCurrent(long token)
        {
            if (!IsActive(token) || !IsStepEditing || !CanEditParameters || !pipeline.RevertSavedStep(stepIndex)) { return false; }
            statusText = string.Empty;
            appliedPresetNameKey = string.Empty;
            RaiseState();
            editActionKey = "Workbench.Step.Reverted";
            OnPropertyChanged(nameof(EditActionKey));
            return true;
        }

        private string CreateTestOutput(string toolName)
        {
            string output = toolName + "_1";
            for (int suffix = 2; !string.IsNullOrEmpty(pipeline.ValidateOutput(output)); suffix++) { output = toolName + "_" + suffix.ToString(CultureInfo.InvariantCulture); }
            return output;
        }

        private void EditAcceptance(Action<VisionPipelineStep> edit)
        {
            changing = true;
            try
            {
                VisionPipelineStep step = CurrentStep;
                byte[] previous = SerializeStep(step);
                edit(step);
                if (step.UseAcceptance && new[] { nameof(AcceptanceMinimumText), nameof(AcceptanceMaximumText), nameof(AcceptanceTimeLimitText) }
                    .All(name => string.IsNullOrEmpty(AcceptanceInputError(name, step))))
                {
                    if (step.UseAcceptanceMetricMinimum) { step.AcceptanceMetricMinimum = double.Parse(AcceptanceInput(nameof(AcceptanceMinimumText), step.AcceptanceMetricMinimum), CultureInfo.CurrentCulture); }
                    if (step.UseAcceptanceMetricMaximum) { step.AcceptanceMetricMaximum = double.Parse(AcceptanceInput(nameof(AcceptanceMaximumText), step.AcceptanceMetricMaximum), CultureInfo.CurrentCulture); }
                    step.MaxElapsedMilliseconds = double.Parse(AcceptanceInput(nameof(AcceptanceTimeLimitText), step.MaxElapsedMilliseconds), CultureInfo.CurrentCulture);
                }
                if (IsStepEditing) { pipeline.ReplaceStep(stepIndex, step); observedStep = SerializeStep(pipeline.GetStep(stepIndex)); }
                else if (!previous.SequenceEqual(SerializeStep(step)) && test.Result != null) { test.ResultPrevious = true; }
            }
            finally { changing = false; }
            RaiseState();
        }

        private string AcceptanceInput(string name, double number) => acceptanceInputs.TryGetValue(name, out string text)
            ? text : number.ToString("G", CultureInfo.CurrentCulture);

        private void SetAcceptanceInput(string name, string text, bool canEdit)
        {
            if (!canEdit) { return; }
            if (!string.Equals(AcceptanceInput(name, name == nameof(AcceptanceMinimumText) ? CurrentStep.AcceptanceMetricMinimum
                : name == nameof(AcceptanceMaximumText) ? CurrentStep.AcceptanceMetricMaximum : CurrentStep.MaxElapsedMilliseconds), text, StringComparison.Ordinal))
            {
                if (IsStepEditing) { pipeline.InvalidateToolResult(stepIndex); }
                else if (test.Result != null) { test.ResultPrevious = true; }
            }
            acceptanceInputs[name] = text ?? string.Empty;
            EditAcceptance(_ => { });
        }

        private string AcceptanceInputError(string name) => AcceptanceInputError(name, CurrentStep);

        private string AcceptanceInputError(string name, VisionPipelineStep step)
        {
            if (step?.UseAcceptance != true || !HasAcceptance || (name == nameof(AcceptanceMinimumText) && !step.UseAcceptanceMetricMinimum)
                || (name == nameof(AcceptanceMaximumText) && !step.UseAcceptanceMetricMaximum)
                || (name != nameof(AcceptanceMinimumText) && name != nameof(AcceptanceMaximumText) && name != nameof(AcceptanceTimeLimitText))) { return string.Empty; }
            string label = this[name == nameof(AcceptanceMinimumText) ? "Workbench.Acc.Lower"
                : name == nameof(AcceptanceMaximumText) ? "Workbench.Acc.Upper" : "Workbench.Acc.TimeLimit"];
            string text = AcceptanceInput(name, name == nameof(AcceptanceMinimumText) ? step.AcceptanceMetricMinimum
                : name == nameof(AcceptanceMaximumText) ? step.AcceptanceMetricMaximum : step.MaxElapsedMilliseconds);
            if (!double.TryParse(text, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.CurrentCulture, out double number))
                return string.Format(CultureInfo.CurrentCulture, this["Workbench.Validate.Detail.91Numeric"], label);
            if (!double.IsFinite(number)) { return string.Format(CultureInfo.CurrentCulture, this["Workbench.Validate.Detail.84Finite"], label); }
            if (name == nameof(AcceptanceTimeLimitText)) { return number < 0D ? this["Workbench.Validate.Detail.06Time"] : string.Empty; }
            return step.UseAcceptanceMetricMinimum && step.UseAcceptanceMetricMaximum
                && double.TryParse(AcceptanceInput(nameof(AcceptanceMinimumText), step.AcceptanceMetricMinimum), NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.CurrentCulture, out double minimum)
                && double.TryParse(AcceptanceInput(nameof(AcceptanceMaximumText), step.AcceptanceMetricMaximum), NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.CurrentCulture, out double maximum) && minimum > maximum
                ? this["Workbench.Validate.Detail.07AcceptanceRange"] : string.Empty;
        }

        private void ExecuteStepCommand(ICommand command)
        {
            if (IsStepEditing && CanEditParameters && command.CanExecute(null)) { command.Execute(null); }
        }

        internal void NotifyEditAction(long token, string key)
        {
            if (!IsActive(token)) { return; }
            editActionKey = key;
            OnPropertyChanged(nameof(EditActionKey));
        }

        private void ChangeTemplate(bool fromFile)
        {
            if (!HasTemplateCard || !CanEditParameters || editor.ChangeTemplate == null) { return; }
            bool? result = editor.ChangeTemplate(fromFile);
            if (result.HasValue) { templateFailed = !result.Value; statusText = result.Value ? string.Empty : this["Workbench.Status.Error"]; }
            RaiseState();
        }

        private void RestoreDefaults()
        {
            if (!IsNewTest || !CanEditParameters || editor.CreateDefaults == null) { return; }
            changing = true;
            try
            {
                byte[] previous = SerializeStep(test.Step);
                editor.RestoreParameters(editor.CreateDefaults());
                VisionPipelineStep captured = editor.CreateStep();
                test.Parameters = editor.CaptureParameters();
                if (captured != null) { MergeParameters(test.Step, baseline, captured); }
                if (!previous.SequenceEqual(SerializeStep(test.Step)) && test.Result != null) { test.ResultPrevious = true; }
                baseline = captured;
                editor.ClearResult?.Invoke();
                statusText = string.Empty;
                appliedPresetNameKey = string.Empty;
                templateFailed = false;
                RefreshRoute?.Invoke();
            }
            finally { changing = false; }
            RaiseState();
            editActionKey = "Workbench.Action.DefaultsRestored";
            OnPropertyChanged(nameof(EditActionKey));
        }

        private void NewTestFromCurrent()
        {
            if (!IsStepEditing || !CanEditParameters || OpenTestRequested == null) { return; }
            ParametersChanged(generation);
            VisionPipelineStep draft = VisionPipelineStepEditService.CopyStep(CurrentStep);
            if (draft == null) { return; }
            draft.Name = editor.DisplayName();
            draft.InputLayer = draft.OutputLayer;
            draft.OutputLayer = CreateTestOutput(ToolName);
            draft.Enabled = true;
            tests[contextKey + "\n" + ToolName] = new TestDraft { Parameters = editor.CaptureParameters(), Step = draft };
            editor.ClearResult?.Invoke();
            OpenTestRequested(ToolName);
        }

        private void OnNewTestRequested(object sender, EventArgs e)
        {
            if (!CanChangeSelection) { return; }
            OpenStepRequested?.Invoke(pipeline.SelectedStepIndex);
            if (IsStepEditing && stepIndex == pipeline.SelectedStepIndex) { NewTestFromCurrent(); }
        }

        private static void MergeParameters(VisionPipelineStep target, VisionPipelineStep previous, VisionPipelineStep captured)
        {
            if (previous != null && !string.Equals(previous.ToolType, captured.ToolType, StringComparison.Ordinal)) { target.ToolType = captured.ToolType; }
            foreach (KeyValuePair<string, string> parameter in captured.Parameters)
            {
                if (previous?.Parameters.TryGetValue(parameter.Key, out string oldValue) == true && string.Equals(oldValue, parameter.Value, StringComparison.Ordinal)) { continue; }
                target.Parameters[parameter.Key] = parameter.Value;
            }
        }

        private void RunSelectedStep(object parameter)
        {
            if (parameter is not int index || !RunSelectedStepCommand.CanExecute(parameter)) { return; }
            pipeline.SelectStepCommand.Execute(index);
            if (IsStepEditing && stepIndex == index && RunCommand.CanExecute(null)) { RunCommand.Execute(null); }
        }

        private void OnStepSelected(object sender, EventArgs e)
        {
            if (!changing && !disposed && pipeline.CanEditSelected) { OpenStepRequested?.Invoke(pipeline.SelectedStepIndex); }
        }

        private void OnPipelineChanged(object sender, PropertyChangedEventArgs e)
        {
            if (changing || disposed || editor == null || e.PropertyName != nameof(pipeline.Rows)) { return; }
            if (!string.Equals(contextKey, pipeline.ContextKey, StringComparison.Ordinal))
            {
                editor = null;
                test = null;
                baseline = null;
                stepIndex = -1;
                generation++;
                RaiseState();
                return;
            }
            if (IsStepEditing)
            {
                VisionPipelineStep current = pipeline.GetStep(stepIndex);
                if (current == null) { Deactivate(generation); return; }
                if (pipeline.SelectedStepIndex != stepIndex) { return; }
                if (baseline != null && !string.Equals(current.ToolType, baseline.ToolType, StringComparison.Ordinal))
                {
                    OpenStepRequested?.Invoke(stepIndex);
                    return;
                }
                byte[] nextSnapshot = SerializeStep(current);
                if (observedStep.SequenceEqual(nextSnapshot))
                {
                    changing = true;
                    try { RefreshRoute?.Invoke(); }
                    finally { changing = false; }
                    RaiseState();
                    return;
                }
                changing = true;
                try { editor.ApplyStep(current); baseline = editor.CreateStep(); observedStep = nextSnapshot; acceptanceInputs.Clear(); RefreshRoute?.Invoke(); }
                finally { changing = false; }
            }
            else if (IsNewTest)
            {
                changing = true;
                try { RefreshRoute?.Invoke(); }
                finally { changing = false; }
            }
            RaiseState();
        }

        private void OnLanguageChanged(object sender, EventArgs e)
        {
            RaiseState();
            if (IsOpen && !changing && !disposed)
            {
                changing = true;
                try { RefreshRoute?.Invoke(); }
                finally { changing = false; }
            }
            OnPropertyChanged(nameof(SelectedAcceptanceMetric));
        }
        private static byte[] SerializeStep(VisionPipelineStep step)
        {
            if (step == null) { return Array.Empty<byte>(); }
            VisionPipeline single = new VisionPipeline();
            single.Steps.Add(step);
            return VisionPipelineExecutionPlan.SerializePipeline(single);
        }
        private void RaiseState()
        {
            OnPropertyChanged(string.Empty);
            CommandManager.InvalidateRequerySuggested();
        }

        public void Dispose()
        {
            if (disposed) { return; }
            disposed = true;
            pipeline.PropertyChanged -= OnPipelineChanged;
            pipeline.StepSelected -= OnStepSelected;
            pipeline.NewTestRequested -= OnNewTestRequested;
            OpenVisionLanguageService.LanguageChanged -= OnLanguageChanged;
            tests.Clear();
            acceptanceInputs.Clear();
            pipeline.CanCommitToolEdits = null;
            pipeline.CommitToolEdits = null;
            editor = null;
            RefreshRoute = null;
            OpenStepRequested = null;
            OpenTestRequested = null;
            OpenLearnRequested = null;
        }

        internal sealed class EditorBinding
        {
            internal string ToolName { get; set; }
            internal Func<string> DisplayName { get; set; }
            internal string EnglishName { get; set; }
            internal PackIconMaterialKind Icon { get; set; }
            internal Func<OpenVisionRecipeContext, object> LoadParameters { get; set; }
            internal Func<object> CaptureParameters { get; set; }
            internal Action<object> RestoreParameters { get; set; }
            internal Func<VisionPipelineStep> CreateStep { get; set; }
            internal Func<bool> CommitPendingEdits { get; set; }
            internal bool SupportsPipeline { get; set; }
            internal Action<VisionPipelineStep> InitializeTestRoute { get; set; }
            internal Action<VisionPipelineStep> ApplyStep { get; set; }
            internal Func<bool> TemplateReady { get; set; }
            internal Func<OpenVisionLab.Contracts.VisionToolTemplateStatus> GetTemplateStatus { get; set; }
            internal Func<System.Windows.Media.ImageSource> GetTemplateThumbnail { get; set; }
            internal Func<bool, bool?> ChangeTemplate { get; set; }
            internal Func<bool> IsRunning { get; set; }
            internal Func<bool> AutoPreviewOptedIn { get; set; }
            internal Action Run { get; set; }
            internal Func<object> CreateDefaults { get; set; }
            internal Action ClearResult { get; set; }
            internal Action ShowParameterGuide { get; set; }
        }

        private sealed class TestDraft
        {
            internal object Parameters { get; set; }
            internal VisionPipelineStep Step { get; set; }
            internal VisionPipelineStepResultSummary Result { get; set; }
            internal bool ResultPrevious { get; set; }
            internal long InputRevision { get; set; }
        }
    }
}
