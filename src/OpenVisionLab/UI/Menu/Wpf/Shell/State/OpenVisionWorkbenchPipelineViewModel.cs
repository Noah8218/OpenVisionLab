using MahApps.Metro.IconPacks;
using OpenVisionLab.Core;
using OpenVisionLab.Mvvm;
using OpenVisionLab.Pipeline.Controls;
using OpenVisionLab.Vision2D.Pipeline;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;

namespace OpenVisionLab
{
    public sealed class OpenVisionWorkbenchPipelineViewModel : ObservableObject, IDisposable
    {
        private readonly OpenVisionShellHostRecipeCommandSurface recipes;
        private readonly IDisplayManager displayManager;
        private readonly Action<Action> invokeOnUi;
        private readonly Func<string, bool> confirmDelete;
        private readonly IReadOnlyList<OpenVisionShellNavGroup> tools;
        private readonly OpenVisionPipelineReviewExecutionController execution;
        private readonly OpenVisionRecipePendingEditTransitionController pending;
        private readonly Stack<(VisionPipeline Pipeline, int Selection, Guid[] StepIds)> undo = new Stack<(VisionPipeline, int, Guid[])>();
        private readonly Stack<(VisionPipeline Pipeline, int Selection, Guid[] StepIds)> redo = new Stack<(VisionPipeline, int, Guid[])>();
        private readonly Dictionary<Guid, (VisionPipelineStep Step, byte[] Xml)> savedSteps = new Dictionary<Guid, (VisionPipelineStep, byte[])>();
        private readonly Dictionary<int, VisionPipelineStepResultSummary> results = new Dictionary<int, VisionPipelineStepResultSummary>();
        private readonly HashSet<int> stale = new HashSet<int>();
        private readonly HashSet<string> knownOutputs = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        private VisionPipeline pipeline = new VisionPipeline();
        private VisionPipelineValidationResult validation;
        private readonly ObservableCollection<OpenVisionWorkbenchStepViewModel> rows = new ObservableCollection<OpenVisionWorkbenchStepViewModel>();
        private string recipeName = string.Empty;
        private string pipelineName = string.Empty;
        private byte[] savedXml = Array.Empty<byte>();
        private List<Guid> stepIds = new List<Guid>();
        private int selectedIndex = -1;
        private int runningIndex = -1;
        private int runningStepCount;
        private int runProgressIndex;
        private string renameText = string.Empty;
        private string statusText = string.Empty;
        private string restoreActionKey = string.Empty;
        private bool renaming;
        private bool running;
        private bool batchRunning;
        private bool imageEditPending;
        private bool nImageRunning;
        private Action nImageStop;
        private bool restoredImageResults;
        private OpenVisionWorkbenchImageResult restoredResult;
        private double lastRunMilliseconds;
        private bool singleRunPrefix;
        private bool singleRunCompleted;
        private string singleRunTarget = string.Empty;
        private string singleRunResultKind = string.Empty;
        private bool stopRequested;
        private bool hasFinalResult;
        private bool finalStale;
        private string finalValue = "—";
        private string finalDetail = string.Empty;
        private string finalKind = string.Empty;
        private string finalDetailKey = string.Empty;
        private object[] finalDetailArguments = Array.Empty<object>();
        private int finalNotRunCount;
        private object mainImage;
        private long inputRevision;
        private long recipeRevision;
        private bool disposed;
        private bool raisingState;

        internal OpenVisionWorkbenchPipelineViewModel(OpenVisionShellHostRecipeCommandSurface recipes, IDisplayManager displayManager,
            Action<Action> invokeOnUi, Func<OpenVisionRecipePendingEditRequest, OpenVisionRecipePendingEditDecision> decidePending,
            Func<string, bool> confirmDelete, IReadOnlyList<OpenVisionShellNavGroup> tools)
        {
            this.recipes = recipes ?? throw new ArgumentNullException(nameof(recipes));
            this.displayManager = displayManager ?? throw new ArgumentNullException(nameof(displayManager));
            this.invokeOnUi = invokeOnUi ?? throw new ArgumentNullException(nameof(invokeOnUi));
            this.confirmDelete = confirmDelete ?? throw new ArgumentNullException(nameof(confirmDelete));
            this.tools = tools ?? Array.Empty<OpenVisionShellNavGroup>();
            execution = new OpenVisionPipelineReviewExecutionController(displayManager, invokeOnUi, captureMainOutputSnapshots: true);
            pending = new OpenVisionRecipePendingEditTransitionController(decidePending, recipes.TrySaveSelectedRecipe, Discard);
            RunCommand = new RelayCommand(async () => await RunAsync(false), () => CanRun(false));
            RunToSelectedCommand = new RelayCommand(async () => await RunAsync(true), () => CanRun(true));
            RunToRowCommand = new RelayCommand<int>(async index => { SelectStep(index); await RunAsync(true); }, index => CanRun(false) && !renaming && index >= 0 && index < pipeline.Steps.Count);
            ToggleRowEnabledCommand = new RelayCommand<int>(index => Edit(VisionPipelineStepEditService.SetEnabled(pipeline, index, !pipeline.Steps[index].Enabled, SourceLayers())),
                index => CanEdit && index >= 0 && index < pipeline.Steps.Count);
            MoveStepCommand = new RelayCommand<Tuple<int, int>>(move => MoveStep(move.Item1, move.Item2),
                move => CanEdit && move != null && move.Item1 >= 0 && move.Item1 < pipeline.Steps.Count && move.Item2 >= 0 && move.Item2 < pipeline.Steps.Count && move.Item1 != move.Item2);
            StopCommand = new RelayCommand(() => { stopRequested = true; nImageStop?.Invoke(); RaiseState(); }, () => IsRunning && !stopRequested);
            MoveUpCommand = new RelayCommand(() => MoveSelected(-1), () => CanEditSelected && selectedIndex > 0);
            MoveDownCommand = new RelayCommand(() => MoveSelected(1), () => CanEditSelected && selectedIndex < pipeline.Steps.Count - 1);
            DuplicateCommand = new RelayCommand(DuplicateSelected, () => CanEditSelected);
            ToggleEnabledCommand = new RelayCommand(() => Edit(VisionPipelineStepEditService.SetEnabled(pipeline, selectedIndex, !pipeline.Steps[selectedIndex].Enabled, SourceLayers())), () => CanEditSelected);
            DeleteCommand = new RelayCommand(DeleteSelected, () => CanEditSelected);
            RenameCommand = new RelayCommand(BeginRename, () => CanEditSelected);
            CommitRenameCommand = new RelayCommand(CommitRename, () => CanEditSelected && renaming);
            CancelRenameCommand = new RelayCommand(() => { renaming = false; RaiseState(); });
            UndoCommand = new RelayCommand(() => Restore(undo, redo), () => CanEdit && undo.Count > 0);
            RedoCommand = new RelayCommand(() => Restore(redo, undo), () => CanEdit && redo.Count > 0);
            CloseCommand = new RelayCommand<CancelEventArgs>(args => args.Cancel = !recipes.TryCloseWorkbench());
            UnavailableCommand = new RelayCommand(() => { }, () => false);
            SelectStepCommand = new RelayCommand<int>(SelectStep, index => CanEdit && !renaming && index >= 0 && index < pipeline.Steps.Count);
            NewTestCommand = new RelayCommand(() => NewTestRequested(this, EventArgs.Empty), () => CanEditSelected);
            execution.StepUpdated += OnStepUpdated;
            recipes.PropertyChanged += OnRecipeChanged;
            displayManager.UpdateParameter += OnDisplayChanged;
            displayManager.UpdateResult += OnDisplayChanged;
            OpenVisionLanguageService.LanguageChanged += OnLanguageChanged;
            Reload();
        }

        public string this[string key] => OpenVisionLanguageService.T(key);
        internal event EventHandler StepSelected = delegate { };
        internal event EventHandler NewTestRequested = delegate { };
        internal event EventHandler InspectionCompleted = delegate { };
        internal bool IsPublishingState => raisingState;
        public IReadOnlyList<OpenVisionWorkbenchStepViewModel> Rows => rows;
        public bool HasSteps => pipeline.Steps.Count > 0;
        public bool IsEmpty => !HasSteps;
        public bool HasMultiplePipelines => recipes.PipelineOptions.Count > 1;
        public bool HasSinglePipeline => !HasMultiplePipelines;
        public string SelectedPipelineName => recipes.SelectedPipelineOption?.PipelineName ?? string.Empty;
        public bool IsDefaultPipeline => recipes.SelectedPipelineOption?.IsActive == true;
        public bool IsRunning => running || batchRunning || nImageRunning;
        public bool IsSingleRunning => running && !batchRunning;
        internal bool SingleRunPrefix => singleRunPrefix;
        internal bool SingleRunCompleted => singleRunCompleted;
        internal string SingleRunTarget => singleRunTarget;
        internal string SingleRunResultKind => singleRunResultKind;
        internal string SingleRunElapsedText => OpenVisionPipelineReviewResultPresenter.FormatWorkbenchElapsed(lastRunMilliseconds);
        public string RunProgressText => IsSingleRunning ? Format(stopRequested ? "Workbench.Run.StoppingProgress" : "Workbench.Run.Progress", runProgressIndex, runningStepCount) : string.Empty;
        public double RunProgressValue => runningStepCount > 0 ? Math.Clamp(100D * runProgressIndex / runningStepCount, 0D, 100D) : 0D;
        internal bool HasPersistenceFailure => recipes.HasRecipeSaveFailure || recipes.HasSelectedRecipePersistenceFailure
            || (VisionPipelineStorage.TryGetPersistenceState(recipeName, pipelineName, out VisionPipelinePersistenceState state) && state.IsFailure);
        public bool CanEdit => !disposed && !IsRunning && !imageEditPending && recipes.CanUseWorkbenchContext;
        internal bool IsBatchRunning => batchRunning;
        internal bool IsStopRequested => stopRequested;
        public bool CanEditSelected => CanEdit && selectedIndex >= 0 && selectedIndex < pipeline.Steps.Count;
        public bool IsDirty => !savedXml.SequenceEqual(VisionPipelineExecutionPlan.SerializePipeline(pipeline));
        public int ChangeCount => IsDirty ? Math.Max(1, undo.Count) : 0;
        public string StepCountText => Format("Workbench.Pipeline.StepCountFormat", pipeline.Steps.Count);
        public string JudgedCountText => Format("Workbench.Pipeline.JudgedCountFormat", pipeline.Steps.Count(IsJudged));
        public string StatusText => statusText;
        public string RestoreActionKey => restoreActionKey;
        public bool HasStatus => !string.IsNullOrWhiteSpace(statusText);
        public bool IsRenaming => renaming;
        public string RenameText { get => renameText; set => SetProperty(ref renameText, value ?? string.Empty); }
        public string VerdictValue => !hasFinalResult ? "—" : finalKind == "Error" ? this["Workbench.Status.Error"] : finalKind == "Done" ? this["Workbench.Status.Done"] : finalValue;
        public string VerdictDetail => hasFinalResult ? (finalStale ? this["Workbench.Verdict.Previous"] + " · " : string.Empty)
            + (string.IsNullOrEmpty(finalDetailKey) ? finalDetail : Format(finalDetailKey, finalDetailArguments))
            + (finalNotRunCount > 0 ? " · " + Format("Workbench.Verdict.NotRunAfterFormat", finalNotRunCount) : string.Empty)
            : this[HasMainImage ? "Workbench.Verdict.NotRun" : "Workbench.Verdict.NoImage"];
        public string VerdictKind => finalStale ? "Previous" : finalKind;
        public bool IsVerdictPrevious => finalStale;
        public bool HasVerdict => hasFinalResult;
        public string VerdictElapsedText => HasVerdict ? OpenVisionPipelineReviewResultPresenter.FormatWorkbenchElapsed(results.Values.Sum(summary => summary.ElapsedMilliseconds)) : string.Empty;
        public bool HasSelection => selectedIndex >= 0;
        public bool HasNoSelection => !HasSelection;
        public string RunDisabledReason => DescribeRunBlocker(GetRunBlockerKey(false));
        public string RunToDisabledReason => DescribeRunBlocker(GetRunBlockerKey(true));
        public string EditDisabledReason => CanEdit ? string.Empty : this[IsRunning ? "Workbench.Status.Running" : "Workbench.Command.ContextUnavailable"];
        public string StopDisabledReason => IsRunning ? (stopRequested ? this["Workbench.Batch.Stopping"] : string.Empty) : this["Workbench.Command.NoActiveRun"];
        public string RunDisabledToolTip => this["Workbench.Command.RunPipeline"] + " (F5) — " + RunDisabledReason;
        public string RunToDisabledToolTip => this["Workbench.Command.RunToStep"] + " (Ctrl+F5) — " + RunToDisabledReason;
        public string StopDisabledToolTip => this["Workbench.Command.StopTooltip"] + " — " + StopDisabledReason;
        public string EditDisabledToolTip => recipes.MainRecipeName + " — " + EditDisabledReason;
        public string SelectedResultText => selectedIndex < 0 || !results.TryGetValue(selectedIndex, out VisionPipelineStepResultSummary summary)
            ? this["Workbench.Results.None"] : stale.Contains(selectedIndex) ? this["Workbench.Status.Previous"]
            : OpenVisionPipelineReviewResultPresenter.FormatResultSummary(pipeline.Steps[selectedIndex], summary);

        public int SelectedStepIndex
        {
            get => selectedIndex;
            set
            {
                if (raisingState || IsRunning || value == selectedIndex || (value < 0 && rows.Count > 0)) { return; }
                selectedIndex = value;
                renaming = false;
                RaiseState();
                StepSelected(this, EventArgs.Empty);
            }
        }

        public OpenVisionWorkbenchStepViewModel SelectedRow
        {
            get => rows.FirstOrDefault(row => row.Index == selectedIndex);
            set => SelectedStepIndex = value?.Index ?? -1;
        }

        public ICommand RunCommand { get; }
        public ICommand RunToSelectedCommand { get; }
        public ICommand RunToRowCommand { get; }
        public ICommand ToggleRowEnabledCommand { get; }
        public ICommand MoveStepCommand { get; }
        public ICommand StopCommand { get; }
        public ICommand MoveUpCommand { get; }
        public ICommand MoveDownCommand { get; }
        public ICommand DuplicateCommand { get; }
        public ICommand ToggleEnabledCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand RenameCommand { get; }
        public ICommand CommitRenameCommand { get; }
        public ICommand CancelRenameCommand { get; }
        public ICommand UndoCommand { get; }
        public ICommand RedoCommand { get; }
        public ICommand CloseCommand { get; }
        public ICommand UnavailableCommand { get; }
        public ICommand SelectStepCommand { get; }
        public ICommand NewTestCommand { get; }

        internal VisionPipeline CopyWorkingPipeline() => VisionPipelineStepEditService.Copy(pipeline);
        internal VisionPipeline WorkingCopyForTest => CopyWorkingPipeline();
        internal void SetImageEditPending(bool value) { if (disposed) { return; } imageEditPending = value; RaiseState(); }

        internal void SetNImageRunning(bool value, Action stop)
        {
            if (disposed) { return; }
            nImageRunning = value;
            nImageStop = value ? stop : null;
            stopRequested = false;
            RaiseState();
        }

        internal VisionPipeline CaptureNImagePipeline(VisionPipelineStep testStep)
        {
            if (testStep == null) { throw new ArgumentNullException(nameof(testStep)); }
            VisionPipeline frozen = CopyWorkingPipeline();
            var needed = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            void AddInputs(VisionPipelineStep step)
            {
                if (!string.Equals(step.InputLayer, VisionRecipeRunner.DefaultInputLayer, StringComparison.OrdinalIgnoreCase)) { needed.Add(step.InputLayer ?? string.Empty); }
                if (VisionPipelineArithmeticStep.RequiresInputLayerB(step))
                {
                    string second = step.Parameters.TryGetValue(VisionPipelineArithmeticStep.ParameterInputLayerB, out string value) ? value : string.Empty;
                    if (!string.Equals(second, VisionRecipeRunner.DefaultInputLayer, StringComparison.OrdinalIgnoreCase)) { needed.Add(second); }
                }
            }
            AddInputs(testStep);
            int before = selectedIndex >= 0 ? selectedIndex + 1 : frozen.Steps.Count;
            var prefix = new List<VisionPipelineStep>();
            for (int index = Math.Min(before, frozen.Steps.Count) - 1; index >= 0; index--)
            {
                VisionPipelineStep step = frozen.Steps[index];
                if (!step.Enabled || !needed.Remove(step.OutputLayer ?? string.Empty)) { continue; }
                step.UseAcceptance = false;
                prefix.Insert(0, step);
                AddInputs(step);
            }
            var external = new HashSet<string>(SourceLayers(), StringComparer.OrdinalIgnoreCase);
            if (needed.Any(layer => !external.Contains(layer))) { throw new InvalidOperationException("Unavailable input route: " + string.Join(", ", needed.Where(layer => !external.Contains(layer)))); }
            frozen.Steps.Clear();
            frozen.Steps.AddRange(prefix);
            VisionPipelineStep target = VisionPipelineStepEditService.CopyStep(testStep);
            string output = "NImageResult";
            while (prefix.Any(step => string.Equals(step.OutputLayer, output, StringComparison.OrdinalIgnoreCase))) { output += "_"; }
            target.OutputLayer = output;
            while (prefix.Any(step => string.Equals(step.Name, target.Name, StringComparison.OrdinalIgnoreCase))) { target.Name += "_"; }
            frozen.Steps.Add(target);
            return frozen;
        }

        internal IReadOnlyList<string> NImageExternalInputLayers(VisionPipeline frozen)
        {
            var available = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { VisionRecipeRunner.DefaultInputLayer };
            var external = new List<string>();
            foreach (var step in frozen.Steps)
            {
                var inputs = new List<string> { step.InputLayer };
                if (VisionPipelineArithmeticStep.RequiresInputLayerB(step)) { inputs.Add(step.Parameters[VisionPipelineArithmeticStep.ParameterInputLayerB]); }
                foreach (string input in inputs)
                {
                    if (available.Add(input)) { external.Add(input); }
                }
                available.Add(step.OutputLayer);
            }
            return external;
        }
        internal string ContextKey => recipeName + "\n" + pipelineName;
        internal int StepCount => pipeline.Steps.Count;
        internal VisionPipelineStep GetStep(int index) => index >= 0 && index < pipeline.Steps.Count ? VisionPipelineStepEditService.CopyStep(pipeline.Steps[index]) : null;

        internal bool ReplaceStep(int index, VisionPipelineStep step)
        {
            if (!CanEdit || step == null || index < 0 || index >= pipeline.Steps.Count) { return false; }
            if (SerializeStep(pipeline.Steps[index]).SequenceEqual(SerializeStep(step))) { return true; }
            Edit(VisionPipelineStepEditService.Replace(pipeline, index, step, SourceLayers()));
            return true;
        }

        internal VisionPipelineStep InsertStep(int index, VisionPipelineStep step)
        {
            if (!CanEdit || step == null || index < 0 || index > pipeline.Steps.Count) { return null; }
            List<Guid> nextIds = stepIds.ToList();
            nextIds.Insert(index, Guid.NewGuid());
            Edit(VisionPipelineStepEditService.Insert(pipeline, index, step, SourceLayers()), nextIds);
            return GetStep(selectedIndex);
        }

        internal bool CanRevertSavedStep(int index) => CanEdit && index >= 0 && index < stepIds.Count
            && savedSteps.ContainsKey(stepIds[index]) && IsStepChanged(index);

        internal bool RevertSavedStep(int index)
        {
            if (!CanRevertSavedStep(index)) { return false; }
            if (!ReplaceStep(index, savedSteps[stepIds[index]].Step)) { return false; }
            statusText = this["Workbench.Step.Reverted"];
            RaiseState();
            return true;
        }

        internal string ValidateOutput(string output, int editingIndex = -1)
        {
            if (string.IsNullOrWhiteSpace(output) || string.Equals(output.Trim(), VisionRecipeRunner.DefaultInputLayer, StringComparison.OrdinalIgnoreCase))
            {
                return this["Workbench.Validate.OutputInvalid"];
            }
            return pipeline.Steps.Where((step, index) => index != editingIndex).Any(step => string.Equals(step?.OutputLayer, output.Trim(), StringComparison.OrdinalIgnoreCase))
                ? Format("Workbench.Validate.OutputDuplicateFormat", output.Trim()) : string.Empty;
        }
        internal bool ValidationSucceeded => validation?.Success == true;
        internal Func<bool> CanCommitToolEdits { get; set; }
        internal Func<bool> CommitToolEdits { get; set; }
        internal bool TryCommitPendingToolEdits() => CanCommitToolEdits?.Invoke() != false && CommitToolEdits?.Invoke() != false;
        internal int ResultCountForTest => results.Count;
        internal long InputRevision => inputRevision;
        internal long RecipeRevision => recipeRevision;
        internal VisionPipelineStepResultSummary GetResult(int index) => results.TryGetValue(index, out VisionPipelineStepResultSummary result) ? result : null;
        internal bool TryAcquireResultSnapshot(string layerName, out System.Drawing.Bitmap snapshot)
        {
            snapshot = null;
            if (disposed || string.IsNullOrWhiteSpace(layerName)) { return false; }
            for (int index = pipeline.Steps.Count - 1; index >= 0; index--)
            {
                if (string.Equals(pipeline.Steps[index]?.OutputLayer, layerName, StringComparison.OrdinalIgnoreCase))
                {
                    if (restoredImageResults) { snapshot = restoredResult?.CloneOutput(index); return true; }
                    return execution.TryAcquireMainOutputSnapshot(index, layerName, GetResult(index), out snapshot);
                }
            }
            return false;
        }
        internal bool IsResultPrevious(int index) => stale.Contains(index);
        internal void InvalidateToolResult(int index) { MarkStale(index); RefreshRows(); }
        internal void PublishToolResult(int index, VisionPipelineStepResultSummary summary, bool previous = false)
        {
            if (disposed || index < 0 || index >= pipeline.Steps.Count || summary == null) { return; }
            restoredImageResults = false;
            restoredResult = null;
            MarkStale(index);
            summary.Index = index + 1;
            results[index] = summary;
            if (previous) { stale.Add(index); }
            else { stale.Remove(index); }
            RefreshRows();
        }
        internal Task RunForTestAsync(bool prefix = false) => RunAsync(prefix);

        internal bool TryLeave(OpenVisionRecipePendingEditRequest request)
        {
            if (IsRunning) { return false; }
            if (request.Kind == OpenVisionRecipePendingEditTransitionKind.Step) { return true; }
            request.IsWorkbenchCopy = true;
            request.ChangeCount = ChangeCount;
            request.TrySave = recipes.TrySaveSelectedRecipe;
            request.SaveError = () => StatusText;
            return pending.TryLeave(IsDirty || recipes.HasRecipeSaveFailure, request);
        }

        internal bool TrySave()
        {
            if (IsRunning || !TryCommitPendingToolEdits()) { return false; }
            if (!IsDirty) { return true; }
            if (!recipes.SaveWorkbenchPipeline(VisionPipelineStepEditService.Copy(pipeline)))
            {
                statusText = this["Workbench.Save.Failed"] + Environment.NewLine + recipes.StatusText;
                RaiseState();
                return false;
            }

            savedXml = VisionPipelineExecutionPlan.SerializePipeline(pipeline);
            RememberSavedSteps();
            statusText = string.Empty;
            RefreshRows();
            return true;
        }

        private void Reload()
        {
            pipeline = recipes.LoadWorkbenchPipeline();
            recipeName = recipes.SelectedRecipeName;
            pipelineName = recipes.SelectedPipelineOption?.PipelineName ?? string.Empty;
            savedXml = VisionPipelineExecutionPlan.SerializePipeline(pipeline);
            stepIds = pipeline.Steps.Select(_ => Guid.NewGuid()).ToList();
            RememberSavedSteps();
            undo.Clear();
            redo.Clear();
            knownOutputs.Clear();
            RememberOutputs();
            selectedIndex = pipeline.Steps.Count > 0 ? 0 : -1;
            renaming = false;
            results.Clear();
            stale.Clear();
            hasFinalResult = false;
            finalStale = false;
            finalKind = string.Empty;
            finalDetailKey = string.Empty;
            finalNotRunCount = 0;
            statusText = string.Empty;
            recipeRevision++;
            mainImage = displayManager.ImageSpace.GetImage(VisionRecipeRunner.DefaultInputLayer);
            RefreshRows();
        }

        private void Discard() => Reload();

        private void RememberSavedSteps()
        {
            VisionPipeline snapshot = VisionPipelineStepEditService.Copy(pipeline);
            savedSteps.Clear();
            for (int index = 0; index < snapshot.Steps.Count; index++)
            {
                savedSteps.Add(stepIds[index], (snapshot.Steps[index], SerializeStep(snapshot.Steps[index])));
            }
        }

        private bool IsStepChanged(int index) => !savedSteps.TryGetValue(stepIds[index], out var saved)
            || !saved.Xml.SequenceEqual(SerializeStep(pipeline.Steps[index]));

        private void Edit(VisionPipelineStepEditResult result, IReadOnlyList<Guid> nextIds = null)
        {
            if (!CanEdit) { return; }
            int previousSelection = selectedIndex;
            undo.Push((VisionPipelineStepEditService.Copy(pipeline), selectedIndex, stepIds.ToArray()));
            redo.Clear();
            pipeline = result.Pipeline;
            if (nextIds != null) { RestoreResultIdentities(nextIds); }
            selectedIndex = result.SelectedIndex;
            renaming = false;
            MarkStale(result.AffectedIndex);
            RememberOutputs();
            recipeRevision++;
            RefreshRows();
            if (previousSelection != selectedIndex) { StepSelected(this, EventArgs.Empty); }
        }

        private void Restore(Stack<(VisionPipeline Pipeline, int Selection, Guid[] StepIds)> source, Stack<(VisionPipeline Pipeline, int Selection, Guid[] StepIds)> destination)
        {
            if (!CanEdit || source.Count == 0) { return; }
            int previousSelection = selectedIndex;
            destination.Push((VisionPipelineStepEditService.Copy(pipeline), selectedIndex, stepIds.ToArray()));
            (VisionPipeline restored, int selection, Guid[] identities) = source.Pop();
            pipeline = restored;
            selectedIndex = selection;
            RestoreResultIdentities(identities);
            renaming = false;
            MarkStale(0);
            recipeRevision++;
            RefreshRows();
            if (previousSelection != selectedIndex) { StepSelected(this, EventArgs.Empty); }
            restoreActionKey = ReferenceEquals(source, undo) ? "Workbench.Action.UndoCompleted" : "Workbench.Action.RedoCompleted";
            OnPropertyChanged(nameof(RestoreActionKey));
        }

        private void RestoreResultIdentities(IReadOnlyList<Guid> nextIds)
        {
            Dictionary<Guid, (VisionPipelineStepResultSummary Summary, bool Previous)> previousResults = results
                .Where(item => item.Key >= 0 && item.Key < stepIds.Count)
                .ToDictionary(item => stepIds[item.Key], item => (item.Value, stale.Contains(item.Key)));
            results.Clear();
            stale.Clear();
            stepIds = nextIds.ToList();
            for (int index = 0; index < stepIds.Count; index++)
            {
                if (!previousResults.TryGetValue(stepIds[index], out var previous)) { continue; }
                previous.Summary.Index = index + 1;
                results[index] = previous.Summary;
                if (previous.Previous) { stale.Add(index); }
            }
        }

        private void MoveSelected(int offset) => MoveStep(selectedIndex, selectedIndex + offset);

        private void MoveStep(int index, int targetIndex)
        {
            List<Guid> nextIds = stepIds.ToList();
            Guid identity = nextIds[index];
            nextIds.RemoveAt(index);
            nextIds.Insert(targetIndex, identity);
            Edit(VisionPipelineStepEditService.Move(pipeline, index, targetIndex, SourceLayers()), nextIds);
        }

        private void DuplicateSelected()
        {
            List<Guid> nextIds = stepIds.ToList();
            nextIds.Insert(selectedIndex + 1, Guid.NewGuid());
            Edit(VisionPipelineStepEditService.Duplicate(pipeline, selectedIndex, SourceLayers()), nextIds);
        }

        internal void MoveSelectedTo(int index)
        {
            if (CanEditSelected && index >= 0 && index < pipeline.Steps.Count && index != selectedIndex)
            {
                MoveStep(selectedIndex, index);
            }
        }

        private void SelectStep(int index)
        {
            if (!CanEdit || renaming || index < 0 || index >= pipeline.Steps.Count) { return; }
            if (index == selectedIndex) { StepSelected(this, EventArgs.Empty); }
            else { SelectedStepIndex = index; }
        }

        private void DeleteSelected()
        {
            if (!confirmDelete(pipeline.Steps[selectedIndex].Name)) { return; }
            List<Guid> nextIds = stepIds.ToList();
            nextIds.RemoveAt(selectedIndex);
            Edit(VisionPipelineStepEditService.Delete(pipeline, selectedIndex, SourceLayers()), nextIds);
        }

        private void BeginRename()
        {
            renameText = pipeline.Steps[selectedIndex].Name ?? string.Empty;
            renaming = true;
            RaiseState();
        }

        private void CommitRename()
        {
            if (string.Equals(renameText, pipeline.Steps[selectedIndex].Name, StringComparison.Ordinal)) { renaming = false; RaiseState(); return; }
            Edit(VisionPipelineStepEditService.Rename(pipeline, selectedIndex, renameText, SourceLayers()));
        }

        private bool CanRun(bool prefix, bool batch = false) => string.IsNullOrEmpty(GetRunBlockerKey(prefix, batch));

        private string GetRunBlockerKey(bool prefix, bool batch = false)
        {
            if (disposed) { return "Workbench.Command.ContextUnavailable"; }
            if (running || (batch ? !batchRunning : batchRunning)) { return "Workbench.Status.Running"; }
            if (HasPersistenceFailure) { return "Workbench.Save.Failed"; }
            if (!batch && !recipes.CanUseWorkbenchContext) { return "Workbench.Command.ContextUnavailable"; }
            if (!HasMainImage) { return "Workbench.Viewer.NoImage"; }
            if (!HasSteps) { return "Workbench.Pipeline.Empty"; }
            if (validation?.Success != true) { return "Workbench.Command.FixToolSettings"; }
            if (!batch && CanCommitToolEdits?.Invoke() == false) { return "Workbench.Command.FixToolSettings"; }
            if (prefix && selectedIndex < 0) { return "Workbench.Pane.SelectStep"; }
            return string.Empty;
        }

        private string DescribeRunBlocker(string key)
        {
            if (string.IsNullOrEmpty(key)) { return string.Empty; }
            if (key == "Workbench.Command.FixToolSettings" && validation?.Success != true)
            {
                string detail = rows.FirstOrDefault(row => row.HasError)?.ErrorText
                    ?? (validation == null ? null : OpenVisionPipelineReviewResultPresenter.FormatWorkbenchValidation(validation.Errors.FirstOrDefault()));
                if (!string.IsNullOrEmpty(detail)) { return detail; }
            }
            return this[key];
        }

        internal bool CanStartBatch => CanRun(false);
        internal bool TryBeginBatch()
        {
            if (!CanStartBatch || !TryCommitPendingToolEdits() || !CanStartBatch) { return false; }
            batchRunning = true;
            stopRequested = false;
            RaiseState();
            return true;
        }
        internal void EndBatch() { batchRunning = false; stopRequested = false; if (!disposed) { RefreshRows(); } }
        internal Task RunBatchImageAsync() => RunAsync(false, batch: true);

        internal OpenVisionWorkbenchImageResult CaptureImageResult(string source)
        {
            Dictionary<int, VisionPipelineStepResultSummary> retained = new();
            Dictionary<int, Bitmap> images = new();
            try
            {
                foreach (var item in results)
                {
                    bool judged = item.Key >= 0 && item.Key < pipeline.Steps.Count && IsJudged(pipeline.Steps[item.Key]);
                    retained[item.Key] = item.Value.CopyForMainRetention(judged);
                    if (judged && execution.TryAcquireMainOutputSnapshot(item.Key, item.Value.OutputLayer, item.Value, out Bitmap image) && image != null) { images[item.Key] = image; }
                }
                return new OpenVisionWorkbenchImageResult(ContextKey, recipeRevision, VisionPipelineStepEditService.Copy(pipeline),
                    retained, images, finalKind, lastRunMilliseconds, source);
            }
            catch { foreach (Bitmap image in images.Values) { image.Dispose(); } throw; }
        }

        internal OpenVisionWorkbenchImageResult CreateImageLoadError(string name) => new(ContextKey, recipeRevision,
            VisionPipelineStepEditService.Copy(pipeline), new(), new(), "Error", 0D, "batch",
            name);

        internal void RestoreImageResult(OpenVisionWorkbenchImageResult result)
        {
            if (disposed || IsRunning) { return; }
            restoredImageResults = true;
            restoredResult = result?.IsStale != false || result.Context != ContextKey || result.RecipeRevision != recipeRevision ? null : result;
            results.Clear();
            stale.Clear();
            hasFinalResult = false;
            finalStale = false;
            finalKind = string.Empty;
            if (restoredResult != null)
            {
                foreach (var item in restoredResult.Summaries) { results[item.Key] = item.Value; }
                if (restoredResult.Summaries.Count > 0) { UpdateVerdict(); }
                else { hasFinalResult = true; finalKind = restoredResult.Verdict; finalValue = this["Workbench.Status.Error"]; finalDetailKey = "Workbench.Batch.LoadError"; finalDetailArguments = new object[] { restoredResult.LoadErrorImageName }; finalNotRunCount = 0; }
            }
            RefreshRows();
        }

        private async Task RunAsync(bool prefix, bool batch = false)
        {
            if (!CanRun(prefix, batch)) { return; }
            if (!batch && (!TryCommitPendingToolEdits() || !CanRun(prefix, batch))) { return; }
            VisionPipeline snapshot = VisionPipelineStepEditService.Copy(pipeline);
            // Main counts Mean as a judgment step; use its existing GV range only on the run copy.
            foreach (VisionPipelineStep step in snapshot.Steps.Where(step => step.Enabled && !step.UseAcceptance && string.Equals(step.ToolType, "Mean", StringComparison.OrdinalIgnoreCase)))
            {
                step.UseAcceptance = true;
                step.ExpectedSuccess = true;
                step.MaxElapsedMilliseconds = 0D;
                step.RequiredMessageText = string.Empty;
                step.AcceptanceMetricName = "MeanValueAvg";
                step.UseAcceptanceMetricMinimum = true;
                step.AcceptanceMetricMinimum = step.Parameters.TryGetValue("MEAN_MIN", out string minimum) && int.TryParse(minimum, NumberStyles.Integer, CultureInfo.InvariantCulture, out int low) ? low : 100;
                step.UseAcceptanceMetricMaximum = true;
                step.AcceptanceMetricMaximum = step.Parameters.TryGetValue("MEAN_MAX", out string maximum) && int.TryParse(maximum, NumberStyles.Integer, CultureInfo.InvariantCulture, out int high) ? high : 240;
            }
            int count = prefix ? selectedIndex + 1 : snapshot.Steps.Count;
            if (prefix) { snapshot.Steps.RemoveRange(count, snapshot.Steps.Count - count); }
            if (!VisionPipelineStepEditService.Validate(snapshot, SourceLayers()).Success) { RefreshRows(); return; }
            if (!batch)
            {
                singleRunPrefix = prefix;
                singleRunTarget = prefix ? OpenVisionWorkbenchStepViewModel.DisplayName(snapshot.Steps[count - 1].Name) : string.Empty;
                singleRunCompleted = false;
                singleRunResultKind = string.Empty;
            }
            running = true;
            runningStepCount = count;
            runProgressIndex = 0;
            restoredImageResults = false;
            restoredResult = null;
            bool completed = false;
            long started = Stopwatch.GetTimestamp();
            if (hasFinalResult) { finalStale = true; }
            if (!batch) { stopRequested = false; }
            long runInput = inputRevision;
            long runRecipe = recipeRevision;
            foreach (int index in results.Keys.Where(index => index < count).ToArray()) { results.Remove(index); }
            foreach (int index in stale.Where(index => index < count).ToArray()) { stale.Remove(index); }
            if (prefix) { MarkStale(count); }
            RefreshRows();
            try
            {
                OpenVisionPipelineReviewExecutionResult result = await execution.RunAsync(snapshot, OpenVisionPipelineReviewExecutionController.DefaultStepTimeoutMilliseconds, runInput, runRecipe);
                if (disposed) { return; }
                if (result.WasSuperseded || runInput != inputRevision || runRecipe != recipeRevision)
                {
                    MarkStale(0);
                    return;
                }

                for (int index = 0; index < count; index++)
                {
                    if (execution.TryGetSummary(snapshot.Steps[index], out VisionPipelineStepResultSummary summary)) { results[index] = summary; }
                }

                if (!prefix) { UpdateVerdict(); completed = true; }
                if (!batch)
                {
                    IEnumerable<VisionPipelineStepResultSummary> fresh = results.Where(item => item.Key < count && !stale.Contains(item.Key)).Select(item => item.Value);
                    singleRunResultKind = !prefix ? (finalKind == "Done" ? "NoVerdict" : finalKind)
                        : fresh.Any(summary => summary.Executed && !summary.Skipped && !summary.Success && !summary.IsAcceptanceNg) ? "Error"
                        : fresh.Any(summary => summary.Executed && !summary.Skipped && summary.IsAcceptanceNg) ? "Ng"
                        : pipeline.Steps.Any(IsJudged) && pipeline.Steps.Select((step, index) => (Step: step, Index: index)).Where(item => IsJudged(item.Step))
                            .All(item => item.Index < count && results.TryGetValue(item.Index, out VisionPipelineStepResultSummary summary) && !stale.Contains(item.Index) && summary.Executed && !summary.Skipped && summary.Success && !summary.IsAcceptanceNg) ? "Ok" : "NoVerdict";
                    singleRunCompleted = true;
                }
                statusText = string.Empty;
            }
            catch (Exception ex)
            {
                if (!disposed)
                {
                    statusText = this["Workbench.Status.Error"] + ": " + ex.GetBaseException().Message;
                    if (!prefix) { completed = true; hasFinalResult = true; finalStale = false; finalValue = this["Workbench.Status.Error"]; finalKind = "Error"; finalDetailKey = string.Empty; finalNotRunCount = 0; finalDetail = statusText; }
                }
            }
            finally
            {
                running = false;
                runningIndex = -1;
                runningStepCount = runProgressIndex = 0;
                lastRunMilliseconds = Stopwatch.GetElapsedTime(started).TotalMilliseconds;
                if (!disposed) { RefreshRows(); if (completed) { InspectionCompleted(this, EventArgs.Empty); } }
            }
        }

        private void OnStepUpdated(object sender, OpenVisionPipelineReviewStepUpdatedEventArgs e)
        {
            if (disposed || e.InputRevision != inputRevision || e.RecipeRevision != recipeRevision) { return; }
            if (e.Summary != null) { results[e.Summary.Index - 1] = e.Summary; runningIndex = -1; }
            else { runningIndex = execution.GetCurrentStepIndex(e.Step); }
            runProgressIndex = Math.Clamp(e.Summary?.Index ?? runningIndex + 1, 0, runningStepCount);
            RefreshRows();
        }

        private void UpdateVerdict()
        {
            hasFinalResult = true;
            finalStale = false;
            KeyValuePair<int, VisionPipelineStepResultSummary>? failed = results.OrderBy(item => item.Key)
                .Where(item => item.Value.Executed && !item.Value.Skipped && (!item.Value.Success || item.Value.IsAcceptanceNg))
                .Select(item => (KeyValuePair<int, VisionPipelineStepResultSummary>?)item).FirstOrDefault();
            if (failed.HasValue)
            {
                int index = failed.Value.Key;
                bool ng = failed.Value.Value.IsAcceptanceNg;
                finalValue = ng ? "NG" : this["Workbench.Status.Error"];
                finalKind = ng ? "Ng" : "Error";
                finalDetailKey = ng ? "Workbench.Verdict.NgFormat" : "Workbench.Verdict.ErrorFormat";
                finalDetailArguments = new object[] { (index + 1).ToString("00", CultureInfo.InvariantCulture), OpenVisionWorkbenchStepViewModel.DisplayName(pipeline.Steps[index].Name) };
                finalNotRunCount = results.Count(item => item.Value.ExecutionState == VisionPipelineResultSummaryService.NotRunAfterFailureState);
                return;
            }

            int judged = pipeline.Steps.Count(IsJudged);
            finalValue = judged == 0 ? this["Workbench.Status.Done"] : "OK";
            finalKind = judged == 0 ? "Done" : "Ok";
            finalDetailKey = judged == 0 ? "Workbench.Status.NoVerdict" : "Workbench.Verdict.OkFormat";
            finalDetailArguments = judged == 0 ? Array.Empty<object>() : new object[] { judged, judged };
            finalNotRunCount = 0;
        }

        internal static bool IsJudged(VisionPipelineStep step) => step?.Enabled == true && (step.UseAcceptance || string.Equals(step.ToolType, "Mean", StringComparison.OrdinalIgnoreCase));

        private void MarkStale(int from)
        {
            foreach (int index in results.Keys.Where(index => index >= from)) { stale.Add(index); }
            if (hasFinalResult) { finalStale = true; }
        }

        private void RememberOutputs()
        {
            foreach (VisionPipelineStep step in pipeline.Steps.Where(step => step != null && !string.IsNullOrWhiteSpace(step.OutputLayer))) { knownOutputs.Add(step.OutputLayer); }
        }

        private IEnumerable<string> SourceLayers() => displayManager.GetLayerInfos().Select(layer => layer.Title)
            .Where(title => HasImage(title) && (string.Equals(title, VisionRecipeRunner.DefaultInputLayer, StringComparison.OrdinalIgnoreCase) || !knownOutputs.Contains(title)));

        private bool HasMainImage => HasImage(VisionRecipeRunner.DefaultInputLayer);
        private bool HasImage(string layer) => !DisplayManagerImageExtensions.IsPlaceholderBitmap(displayManager.ImageSpace.GetImage(layer));

        private void RefreshRows()
        {
            validation = VisionPipelineStepEditService.Validate(pipeline, SourceLayers());
            VisionPipeline displayPipeline = VisionPipelineStepEditService.Copy(pipeline);
            if (validation.Success) { VisionPipelineNormalizer.NormalizeForRun(displayPipeline); }
            IReadOnlyList<PipelineFlowStepItem> flow = OpenVisionPipelineReviewFlowPresenter.CreateItems(displayPipeline.Steps,
                layer => !knownOutputs.Contains(layer ?? string.Empty) && HasImage(layer),
                step => results.TryGetValue(displayPipeline.Steps.IndexOf(step), out VisionPipelineStepResultSummary summary) ? summary : null);
            OpenVisionWorkbenchStepViewModel[] nextRows = flow.Select(item => new OpenVisionWorkbenchStepViewModel(item, pipeline.Steps[item.Index],
                results.TryGetValue(item.Index, out VisionPipelineStepResultSummary summary) ? summary : null,
                stale.Contains(item.Index), IsStepChanged(item.Index), item.Index == runningIndex,
                validation.Errors.Where(error => error.StartsWith("Step " + (item.Index + 1).ToString(CultureInfo.InvariantCulture) + " ", StringComparison.Ordinal)
                    || error.StartsWith("Step " + (item.Index + 1).ToString(CultureInfo.InvariantCulture) + ":", StringComparison.Ordinal)).ToArray(),
                tools.SelectMany(group => group.Items).FirstOrDefault(tool => string.Equals(tool.CommandId, item.ToolType, StringComparison.OrdinalIgnoreCase)))).ToArray();
            raisingState = true;
            try
            {
                for (int index = 0; index < nextRows.Length; index++)
                {
                    if (index < rows.Count) { rows[index].UpdateFrom(nextRows[index]); }
                    else { rows.Add(nextRows[index]); }
                }
                while (rows.Count > nextRows.Length) { rows.RemoveAt(rows.Count - 1); }
            }
            finally { raisingState = false; }
            RaiseState();
        }

        private void RaiseState()
        {
            raisingState = true;
            try
            {
                OnPropertyChanged(nameof(Rows));
                OnPropertyChanged(string.Empty);
                OnPropertyChanged(nameof(SelectedStepIndex));
                OnPropertyChanged(nameof(SelectedRow));
            }
            finally { raisingState = false; }
            CommandManager.InvalidateRequerySuggested();
        }

        private static byte[] SerializeStep(VisionPipelineStep step)
        {
            VisionPipeline single = new VisionPipeline();
            single.Steps.Add(step);
            return VisionPipelineExecutionPlan.SerializePipeline(single);
        }

        private void OnRecipeChanged(object sender, PropertyChangedEventArgs e)
        {
            if (disposed || IsRunning) { return; }
            if (e.PropertyName == nameof(recipes.HasRecipeSaveFailure) || e.PropertyName == nameof(recipes.HasSelectedRecipePersistenceFailure)
                || e.PropertyName == nameof(recipes.PipelineOptions) || e.PropertyName == nameof(recipes.IsSwitchingRecipe)) { RaiseState(); return; }
            if (e.PropertyName != nameof(recipes.SelectedRecipeName) && e.PropertyName != nameof(recipes.SelectedPipelineOption)
                && e.PropertyName != nameof(recipes.SelectedRecipeSummary)) { return; }
            bool sameContext = string.Equals(recipeName, recipes.SelectedRecipeName, StringComparison.OrdinalIgnoreCase)
                && string.Equals(pipelineName, recipes.SelectedPipelineOption?.PipelineName ?? string.Empty, StringComparison.OrdinalIgnoreCase);
            if (sameContext && IsDirty) { RaiseState(); return; }
            VisionPipeline candidate = recipes.LoadWorkbenchPipeline();
            if (sameContext && savedXml.SequenceEqual(VisionPipelineExecutionPlan.SerializePipeline(candidate))) { RefreshRows(); return; }
            Reload();
        }

        private void OnDisplayChanged(object sender, EventArgs e) => RefreshInputState();

        internal void RefreshInputState()
        {
            invokeOnUi(() =>
            {
                if (disposed) { return; }
                object current = displayManager.ImageSpace.GetImage(VisionRecipeRunner.DefaultInputLayer);
                if (!ReferenceEquals(mainImage, current))
                {
                    mainImage = current;
                    inputRevision++;
                    MarkStale(0);
                }
                RefreshRows();
            });
        }

        private void OnLanguageChanged(object sender, EventArgs e) => RefreshRows();
        private static string Format(string key, params object[] args) => string.Format(CultureInfo.CurrentCulture, OpenVisionLanguageService.T(key), args);

        public void Dispose()
        {
            if (disposed) { return; }
            disposed = true;
            nImageStop?.Invoke();
            nImageStop = null;
            recipes.PropertyChanged -= OnRecipeChanged;
            recipes.DetachWorkbenchPipeline(this);
            displayManager.UpdateParameter -= OnDisplayChanged;
            displayManager.UpdateResult -= OnDisplayChanged;
            OpenVisionLanguageService.LanguageChanged -= OnLanguageChanged;
            execution.StepUpdated -= OnStepUpdated;
            execution.Dispose();
            undo.Clear();
            redo.Clear();
            savedSteps.Clear();
            stepIds.Clear();
            mainImage = null;
        }
    }

    public sealed class OpenVisionWorkbenchStepViewModel : ObservableObject
    {
        internal OpenVisionWorkbenchStepViewModel(PipelineFlowStepItem flow, VisionPipelineStep step, VisionPipelineStepResultSummary summary,
            bool previous, bool dirty, bool running, IReadOnlyList<string> errors, OpenVisionShellNavItem tool)
        {
            Index = flow.Index;
            Name = DisplayName(step.Name);
            ToolName = tool == null ? step.ToolType : OpenVisionShellCommandCatalog.DisplayName(tool.Menu);
            Icon = tool?.IconKind ?? PackIconMaterialKind.ChartTimelineVariant;
            InputLayer = flow.InputLayer;
            OutputLayer = flow.OutputLayer;
            IsEnabled = step.Enabled;
            IsBranch = flow.IsBranch;
            IsInputMissing = flow.IsInputMissing;
            ReadinessText = flow.StatusText;
            IsDirty = dirty;
            BranchTooltip = string.Format(CultureInfo.CurrentCulture, T("Workbench.Pipeline.BranchTooltipFormat"), flow.InputLayer);
            AcceptanceText = OpenVisionPipelineReviewResultPresenter.FormatWorkbenchAcceptance(step);
            ResultStatusKind = summary == null ? string.Empty : previous ? "Previous" : OpenVisionPipelineReviewResultPresenter.WorkbenchResultKind(step, summary);
            ResultStatusText = string.IsNullOrEmpty(ResultStatusKind) ? "—" : T("Workbench.Status." + ResultStatusKind);
            ResultElapsedText = summary?.Executed == true && !summary.Skipped ? OpenVisionPipelineReviewResultPresenter.FormatWorkbenchElapsed(summary.ElapsedMilliseconds) : string.Empty;
            ResultValueText = summary == null ? string.Empty : step.UseAcceptance && !string.IsNullOrWhiteSpace(step.AcceptanceMetricName) && summary.Metrics.TryGetValue(step.AcceptanceMetricName, out double acceptanceValue)
                ? OpenVisionPipelineReviewResultPresenter.WorkbenchMetricValue(step.AcceptanceMetricName, acceptanceValue) : OpenVisionPipelineReviewResultPresenter.FormatWorkbenchSummary(summary);
            if (!IsEnabled) { StatusText = T("Workbench.Status.Disabled"); StatusKind = "Disabled"; }
            else if (running) { StatusText = T("Workbench.Status.Running"); StatusKind = "Running"; }
            else if (previous && summary != null) { StatusText = T("Workbench.Status.Previous"); StatusKind = "Previous"; }
            else if (summary != null)
            {
                StatusKind = OpenVisionPipelineReviewResultPresenter.WorkbenchResultKind(step, summary);
                StatusText = T("Workbench.Status." + StatusKind);
                ElapsedText = summary.Executed && !summary.Skipped ? OpenVisionPipelineReviewResultPresenter.FormatWorkbenchElapsed(summary.ElapsedMilliseconds) : string.Empty;
                if (summary.Executed && !summary.Skipped && (!summary.Success || summary.IsAcceptanceNg)) { ErrorText = OpenVisionPipelineReviewResultPresenter.FormatWorkbenchFailure(step, summary); }
            }

            if (errors.Count > 0)
            {
                StatusText = T("Workbench.Status.Error");
                StatusKind = "Error";
                ErrorText = OpenVisionPipelineReviewResultPresenter.FormatWorkbenchValidation(errors[0], step, flow.Index);
            }
        }

        public int Index { get; }
        public string Number => (Index + 1).ToString("00", CultureInfo.InvariantCulture);
        public string Name { get; private set; }
        public string ToolName { get; private set; }
        public PackIconMaterialKind Icon { get; private set; }
        public string InputLayer { get; private set; }
        public string OutputLayer { get; private set; }
        public string PathText => (string.Equals(InputLayer, "Main", StringComparison.OrdinalIgnoreCase) ? T("Workbench.Pane.Original") : InputLayer) + " → " + OutputLayer;
        public string StatusText { get; private set; } = string.Empty;
        public string StatusKind { get; private set; } = string.Empty;
        public string ElapsedText { get; private set; } = string.Empty;
        public string ResultStatusKind { get; private set; } = string.Empty;
        public string ResultStatusText { get; private set; } = string.Empty;
        public string ResultValueText { get; private set; } = string.Empty;
        public string ResultElapsedText { get; private set; } = string.Empty;
        public string ErrorText { get; private set; } = string.Empty;
        public string AcceptanceText { get; private set; } = string.Empty;
        public string ReadinessText { get; private set; } = string.Empty;
        public string BranchTooltip { get; private set; }
        public bool IsEnabled { get; private set; }
        public bool IsDirty { get; private set; }
        public bool IsBranch { get; private set; }
        public bool IsInputMissing { get; private set; }
        public bool HasError => !string.IsNullOrWhiteSpace(ErrorText);
        public bool HasAcceptance => !string.IsNullOrWhiteSpace(AcceptanceText);
        public bool HasStatus => !string.IsNullOrWhiteSpace(StatusText);
        public bool HasElapsed => !string.IsNullOrWhiteSpace(ElapsedText);
        public bool ShowReadiness => IsEnabled && string.IsNullOrEmpty(ResultStatusKind) && (IsInputMissing || !HasStatus);

        internal void UpdateFrom(OpenVisionWorkbenchStepViewModel row)
        {
            Name = row.Name;
            ToolName = row.ToolName;
            Icon = row.Icon;
            InputLayer = row.InputLayer;
            OutputLayer = row.OutputLayer;
            StatusText = row.StatusText;
            StatusKind = row.StatusKind;
            ElapsedText = row.ElapsedText;
            ResultStatusKind = row.ResultStatusKind;
            ResultStatusText = row.ResultStatusText;
            ResultValueText = row.ResultValueText;
            ResultElapsedText = row.ResultElapsedText;
            ErrorText = row.ErrorText;
            AcceptanceText = row.AcceptanceText;
            ReadinessText = row.ReadinessText;
            BranchTooltip = row.BranchTooltip;
            IsEnabled = row.IsEnabled;
            IsDirty = row.IsDirty;
            IsBranch = row.IsBranch;
            IsInputMissing = row.IsInputMissing;
            OnPropertyChanged(string.Empty);
        }

        internal static string DisplayName(string name) => Regex.Replace(name ?? string.Empty, @"^[0-9]{1,3}[\s._-]+", string.Empty);

        private static string T(string key) => OpenVisionLanguageService.T(key);
    }
}
