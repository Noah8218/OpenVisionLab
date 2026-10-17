using MahApps.Metro.IconPacks;
using OpenVisionLab.Mvvm;
using OpenVisionLab.Vision2D.Pipeline;
using OpenVisionLab.Vision2D.Tool;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Input;

namespace OpenVisionLab
{
    // Projects retained Main results; selections and visibility never execute a tool.
    public sealed class OpenVisionWorkbenchResultsViewModel : ObservableObject, IDisposable
    {
        private readonly OpenVisionWorkbenchViewModel workbench;
        private VisionPipelineStepResultSummary currentSummary;
        private long inputRevision = -1;
        private long recipeRevision = -1;
        private bool detailPrevious;
        private int detailStepIndex = -1;
        private bool detailTrial;
        private bool trialSelected;
        private bool showRejected;
        private double viewportWidth;
        private double viewportHeight;
        private bool disposed;
        private bool refreshing;
        private OpenVisionWorkbenchResultObjectViewModel selectedObject;
        private IReadOnlyList<OpenVisionWorkbenchResultObjectViewModel> allObjects = Array.Empty<OpenVisionWorkbenchResultObjectViewModel>();
        private IReadOnlyList<OpenVisionWorkbenchResultObjectViewModel> objects = Array.Empty<OpenVisionWorkbenchResultObjectViewModel>();
        private IReadOnlyList<OpenVisionWorkbenchMetricViewModel> metrics = Array.Empty<OpenVisionWorkbenchMetricViewModel>();
        private IReadOnlyList<OpenVisionWorkbenchAreaBin> areaBins = Array.Empty<OpenVisionWorkbenchAreaBin>();
        private IReadOnlyList<OpenVisionWorkbenchResultRowViewModel> tableRows = Array.Empty<OpenVisionWorkbenchResultRowViewModel>();

        internal OpenVisionWorkbenchResultsViewModel(OpenVisionWorkbenchViewModel workbench)
        {
            this.workbench = workbench ?? throw new ArgumentNullException(nameof(workbench));
            UpdateViewportCommand = new RelayCommand<System.Windows.Size>(UpdateViewport);
            SelectTrialCommand = new RelayCommand(() => { trialSelected = true; Refresh(); }, () => !disposed && HasTrial && workbench.Pipeline.CanEdit);
            workbench.Pipeline.PropertyChanged += OnSourceChanged;
            workbench.Tool.PropertyChanged += OnSourceChanged;
            workbench.Shell.PropertyChanged += OnSourceChanged;
            OpenVisionLanguageService.LanguageChanged += OnLanguageChanged;
            Refresh();
        }

        public string this[string key] => OpenVisionLanguageService.T(key);
        public IReadOnlyList<OpenVisionWorkbenchStepViewModel> Rows => workbench.Pipeline.Rows;
        public IReadOnlyList<OpenVisionWorkbenchResultRowViewModel> TableRows => tableRows;
        public OpenVisionWorkbenchResultRowViewModel SelectedTableRow
        {
            get => tableRows.FirstOrDefault(row => trialSelected ? row.IsTrial : ReferenceEquals(row.Step, workbench.Pipeline.SelectedRow));
            set
            {
                if (disposed || refreshing || value == null || !CanSelect || !tableRows.Contains(value)) { return; }
                if (value.IsTrial) { trialSelected = true; Refresh(); }
                else { SelectedRow = value.Step; }
            }
        }
        public OpenVisionWorkbenchStepViewModel SelectedRow
        {
            get => trialSelected ? null : workbench.Pipeline.SelectedRow;
            set
            {
                if (disposed || refreshing || value == null || !workbench.Pipeline.CanEdit) { return; }
                trialSelected = false;
                workbench.Pipeline.SelectedRow = value;
                Refresh();
            }
        }
        public bool HasInput => workbench.Shell.HasWorkspaceInputImage;
        public bool HasTrial => workbench.Tool.IsNewTest && workbench.Tool.CurrentResult != null;
        public bool IsTrialSelected => trialSelected;
        public string TrialName => this["Workbench.Results.NewTool"] + " · " + workbench.Tool.DisplayName;
        public string TrialStatusText => workbench.Tool.ResultStatusText;
        public string TrialSummary => workbench.Tool.ResultSummaryText;
        public string TrialElapsed => workbench.Tool.ResultElapsedText;
        public ICommand SelectTrialCommand { get; }
        public ICommand UpdateViewportCommand { get; }
        public bool IsStacked => viewportWidth <= 780D;
        public double SectionHeight => Math.Max(180D, IsStacked ? viewportHeight / 2D : viewportHeight);
        public double ContentHeight => SectionHeight * (IsStacked ? 2D : 1D);
        public string InputName => workbench.Shell.WorkspaceInputName;
        public string FinalKind => workbench.Pipeline.HasVerdict ? workbench.Pipeline.VerdictKind : string.Empty;
        public bool HasFinal => workbench.Pipeline.HasVerdict;
        public string FinalText => workbench.Pipeline.HasVerdict ? this["Workbench.Results.Final"] + " " + workbench.Pipeline.VerdictValue : string.Empty;
        public bool CanSelect => workbench.Pipeline.CanEdit;
        public bool HasResult => currentSummary?.Executed == true && !currentSummary.Skipped;
        public bool IsObjects => HasResult && VisionPipelineNormalizer.NormalizeToolType(CurrentStep?.ToolType) is "blob" or "contour";
        public bool IsMatching => HasResult && VisionPipelineNormalizer.NormalizeToolType(CurrentStep?.ToolType).Contains("matching", StringComparison.OrdinalIgnoreCase) == true;
        public bool IsMetrics => HasResult && !IsObjects && !IsMatching;
        public bool HasObjects => objects.Count > 0;
        public string DetailName => trialSelected ? workbench.Tool.DisplayName : OpenVisionWorkbenchStepViewModel.DisplayName(CurrentStep?.Name);
        public string EmptyText => CurrentStep == null ? this["Workbench.Results.SelectStep"] : DetailName + " · " + this["Workbench.Results.None"];
        public string DetailKind => currentSummary == null ? "NotRun" : IsPrevious ? "Previous" : OpenVisionPipelineReviewResultPresenter.WorkbenchResultKind(CurrentStep, currentSummary);
        public string DetailStatus => this["Workbench.Status." + DetailKind];
        public string DetailHeader => IsObjects
            ? string.Format(CultureInfo.CurrentCulture, this["Workbench.Results.ObjectsFormat"], allObjects.Count(row => row.Accepted), allObjects.Count(row => !row.Accepted))
            : IsMatching ? string.Format(CultureInfo.CurrentCulture, this["Workbench.Results.CandidatesFormat"], allObjects.Count)
            : DetailName + " · " + this["Workbench.Results.Values"];
        public PackIconMaterialKind DetailIcon => IsObjects ? PackIconMaterialKind.VectorCircle : IsMatching ? PackIconMaterialKind.CrosshairsGps : PackIconMaterialKind.TableLarge;
        public string NoObjectsText => this[IsMatching ? "Workbench.Results.NoCandidates" : "Workbench.Results.NoObjects"];
        public IReadOnlyList<OpenVisionWorkbenchResultObjectViewModel> Objects => objects;
        public IReadOnlyList<OpenVisionWorkbenchMetricViewModel> Metrics => metrics;
        public string ReasonText => HasResult && (!currentSummary.Success || currentSummary.IsAcceptanceNg) ? OpenVisionPipelineReviewResultPresenter.FormatWorkbenchFailure(CurrentStep, currentSummary) : string.Empty;
        public bool HasReason => !string.IsNullOrWhiteSpace(ReasonText);
        public string OutputStatisticsText => currentSummary?.HasResultImage != true ? string.Empty
            : currentSummary.ResultImageWidth + "×" + currentSummary.ResultImageHeight
                + (currentSummary.OutputMean.HasValue && currentSummary.OutputMinimum.HasValue && currentSummary.OutputMaximum.HasValue
                    ? string.Format(CultureInfo.CurrentCulture, this["Workbench.Results.OutputStatisticsFormat"], currentSummary.OutputMean.Value, currentSummary.OutputMinimum.Value, currentSummary.OutputMaximum.Value) : string.Empty);
        public bool HasOutputStatistics => !string.IsNullOrWhiteSpace(OutputStatisticsText);
        public bool ShowRejected
        {
            get => showRejected;
            set
            {
                if (disposed || !SetProperty(ref showRejected, value)) { return; }
                FilterObjects();
                UpdateHighlight();
                OnPropertyChanged(nameof(Objects)); OnPropertyChanged(nameof(HasObjects)); OnPropertyChanged(nameof(SelectedObject));
            }
        }
        public OpenVisionWorkbenchResultObjectViewModel SelectedObject
        {
            get => selectedObject;
            set
            {
                if (disposed || refreshing || (value != null && !objects.Contains(value)) || !SetProperty(ref selectedObject, value)) { return; }
                UpdateHighlight();
            }
        }
        public IReadOnlyList<OpenVisionWorkbenchAreaBin> AreaBins => areaBins;
        public double MinimumMarkerX { get; private set; }
        public double MaximumMarkerX { get; private set; }
        public bool ShowMinimumMarker { get; private set; }
        public bool ShowMaximumMarker { get; private set; }
        public string AreaLimitsText { get; private set; } = string.Empty;
        public string MinimumLabel { get; private set; } = string.Empty;
        public string MaximumLabel { get; private set; } = string.Empty;
        public double MinimumLabelX => Math.Min(440D, MinimumMarkerX + 3D);
        public double MaximumLabelX => Math.Min(440D, MaximumMarkerX + 3D);
        internal bool IsDisposed => disposed;
        private VisionPipelineStep CurrentStep => trialSelected ? workbench.Tool.CurrentStep : workbench.Pipeline.GetStep(workbench.Pipeline.SelectedStepIndex);
        private bool IsPrevious => trialSelected ? workbench.Tool.IsResultPrevious : workbench.Pipeline.IsResultPrevious(workbench.Pipeline.SelectedStepIndex);

        private void UpdateViewport(System.Windows.Size size)
        {
            if (disposed || (viewportWidth == size.Width && viewportHeight == size.Height)) { return; }
            viewportWidth = size.Width; viewportHeight = size.Height;
            OnPropertyChanged(nameof(IsStacked)); OnPropertyChanged(nameof(SectionHeight)); OnPropertyChanged(nameof(ContentHeight));
        }

        private void Refresh()
        {
            if (disposed) { return; }
            refreshing = true;
            try
            {
                if (!HasTrial) { trialSelected = false; }
                VisionPipelineStepResultSummary next = trialSelected ? workbench.Tool.CurrentResult : workbench.Pipeline.GetResult(workbench.Pipeline.SelectedStepIndex);
                if (!ReferenceEquals(next, currentSummary) || inputRevision != workbench.Pipeline.InputRevision
                    || recipeRevision != workbench.Pipeline.RecipeRevision || detailPrevious != IsPrevious
                    || detailStepIndex != workbench.Pipeline.SelectedStepIndex || detailTrial != trialSelected)
                {
                    currentSummary = next;
                    inputRevision = workbench.Pipeline.InputRevision;
                    recipeRevision = workbench.Pipeline.RecipeRevision;
                    detailPrevious = IsPrevious;
                    detailStepIndex = workbench.Pipeline.SelectedStepIndex;
                    detailTrial = trialSelected;
                    selectedObject = null;
                    allObjects = IsObjects ? (currentSummary.ObjectResults ?? Array.Empty<VisionPipelineObjectResult>()).Select(OpenVisionWorkbenchResultObjectViewModel.FromObject).ToArray()
                        : IsMatching ? (currentSummary.MatchResults ?? Array.Empty<VisionPipelineMatchResultEvidence>()).Select((item, index) => OpenVisionWorkbenchResultObjectViewModel.FromMatch(item, index + 1)).ToArray()
                        : Array.Empty<OpenVisionWorkbenchResultObjectViewModel>();
                    FilterObjects();
                    BuildAreaHistogram();
                    UpdateHighlight();
                }
                metrics = currentSummary == null ? Array.Empty<OpenVisionWorkbenchMetricViewModel>()
                    : OpenVisionPipelineReviewResultPresenter.OrderResultMetrics(CurrentStep, currentSummary.Metrics)
                        .Select(item => new OpenVisionWorkbenchMetricViewModel(ResultMetricName(item.Key), ResultMetricValue(item.Key, item.Value))).ToArray();
                List<OpenVisionWorkbenchResultRowViewModel> nextRows = new();
                foreach (OpenVisionWorkbenchStepViewModel step in Rows)
                {
                    OpenVisionWorkbenchResultRowViewModel row = tableRows.FirstOrDefault(item => ReferenceEquals(item.Step, step)) ?? new OpenVisionWorkbenchResultRowViewModel(step);
                    row.Update(step.Number, step.Name, step.ToolName, step.ResultStatusKind, step.ResultStatusText, step.ResultValueText, step.ResultElapsedText);
                    nextRows.Add(row);
                }
                if (HasTrial)
                {
                    OpenVisionWorkbenchResultRowViewModel row = tableRows.FirstOrDefault(item => item.IsTrial) ?? new OpenVisionWorkbenchResultRowViewModel(null);
                    row.Update("＋", TrialName, this["Workbench.Results.Test"], workbench.Tool.ResultKind, TrialStatusText, TrialSummary, TrialElapsed);
                    nextRows.Add(row);
                }
                tableRows = nextRows;
                foreach (OpenVisionWorkbenchResultObjectViewModel row in allObjects) { row.NotifyLocalization(); }
                OnPropertyChanged(string.Empty);
                CommandManager.InvalidateRequerySuggested();
            }
            finally { refreshing = false; }
        }

        private string ResultMetricName(string name) => name is "MeanValueMin" or "MeanValueMax" or "BoundsWidthMin" or "BoundsWidthAvg" or "BoundsHeightMin" or "BoundsHeightAvg"
            ? this["Workbench.Results.Metric." + name] : OpenVisionPipelineReviewResultPresenter.WorkbenchMetricName(name);

        private static string ResultMetricValue(string name, double value) => name is "MeanValueMin" or "MeanValueMax"
            ? value.ToString("N1", CultureInfo.CurrentCulture) + " GV"
            : name is "BoundsWidthMin" or "BoundsWidthAvg" or "BoundsHeightMin" or "BoundsHeightAvg"
                ? value.ToString("N0", CultureInfo.CurrentCulture) + " px" : OpenVisionPipelineReviewResultPresenter.WorkbenchMetricValue(name, value);

        private void FilterObjects()
        {
            objects = allObjects.Where(row => !IsObjects || row.Accepted || showRejected).ToArray();
            if (selectedObject != null && !objects.Contains(selectedObject)) { selectedObject = null; }
        }

        private void BuildAreaHistogram()
        {
            areaBins = Array.Empty<OpenVisionWorkbenchAreaBin>();
            ShowMinimumMarker = ShowMaximumMarker = false;
            AreaLimitsText = MinimumLabel = MaximumLabel = string.Empty;
            if (!IsObjects || allObjects.Count == 0) { return; }
            VisionPipelineObjectResult first = currentSummary.ObjectResults[0];
            double low = first.AppliedMinimumArea, high = first.AppliedMaximumArea;
            double max = Math.Max(allObjects.Max(row => row.Area), Math.Max(low * 1.2D, 10D));
            double cap = high > 0 && high < 1000000D ? Math.Max(high * 1.1D, max * 1.1D) : max * 1.2D;
            int[] accepted = new int[48], rejected = new int[48];
            foreach (OpenVisionWorkbenchResultObjectViewModel row in allObjects)
            {
                int bin = Math.Clamp((int)(Math.Max(0D, Math.Min(row.Area, cap)) / cap * 48D), 0, 47);
                if (row.Accepted) { accepted[bin]++; } else { rejected[bin]++; }
            }
            int top = Enumerable.Range(0, 48).Max(index => accepted[index] + rejected[index]);
            areaBins = Enumerable.Range(0, 48).Select(index => new OpenVisionWorkbenchAreaBin(10D + index * 10D,
                accepted[index] / (double)top * 40D, rejected[index] / (double)top * 40D)).ToArray();
            MinimumMarkerX = 10D + low / cap * 480D;
            MaximumMarkerX = 10D + high / cap * 480D;
            ShowMinimumMarker = low > 0D && low <= cap;
            ShowMaximumMarker = high > 0D && high <= cap;
            AreaLimitsText = string.Format(CultureInfo.CurrentCulture, this["Workbench.Results.AreaLimitsFormat"], low, high);
            MinimumLabel = string.Format(CultureInfo.CurrentCulture, this["Workbench.Results.MinimumAreaFormat"], low);
            MaximumLabel = string.Format(CultureInfo.CurrentCulture, this["Workbench.Results.MaximumAreaFormat"], high);
        }

        private void UpdateHighlight()
        {
            VisionPipelineStep step = CurrentStep;
            workbench.Viewer.SetResultObjects(step?.OutputLayer, allObjects.Where(row => !row.Accepted && showRejected).Select(row => row.Overlay).ToArray(),
                selectedObject?.Overlay, step?.UseAcceptance == true);
        }
        private void OnSourceChanged(object sender, PropertyChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.PropertyName)
                || (ReferenceEquals(sender, workbench.Shell) && e.PropertyName is nameof(OpenVisionShellPreviewViewModel.HasWorkspaceInputImage) or nameof(OpenVisionShellPreviewViewModel.WorkspaceInputName))) { Refresh(); }
        }
        private void OnLanguageChanged(object sender, EventArgs e) { BuildAreaHistogram(); Refresh(); }
        public void Dispose()
        {
            if (disposed) { return; }
            disposed = true;
            workbench.Pipeline.PropertyChanged -= OnSourceChanged;
            workbench.Tool.PropertyChanged -= OnSourceChanged;
            workbench.Shell.PropertyChanged -= OnSourceChanged;
            OpenVisionLanguageService.LanguageChanged -= OnLanguageChanged;
            workbench.Viewer.SetResultObjects(null, Array.Empty<VisionToolOverlay>(), null, false);
            allObjects = objects = Array.Empty<OpenVisionWorkbenchResultObjectViewModel>();
            metrics = Array.Empty<OpenVisionWorkbenchMetricViewModel>();
            areaBins = Array.Empty<OpenVisionWorkbenchAreaBin>();
            tableRows = Array.Empty<OpenVisionWorkbenchResultRowViewModel>();
            currentSummary = null; selectedObject = null;
            CommandManager.InvalidateRequerySuggested();
        }
    }

    public sealed class OpenVisionWorkbenchResultObjectViewModel : ObservableObject
    {
        private OpenVisionWorkbenchResultObjectViewModel() { }
        public string Number { get; private init; }
        public bool Accepted { get; private init; }
        public string StateKind => Accepted ? "Ok" : "Skipped";
        public double Area { get; private init; }
        public string AreaText => Area.ToString("N0", CultureInfo.CurrentCulture);
        public string StateText => OpenVisionLanguageService.T(Accepted ? "Workbench.Results.Detected" : "Workbench.Results.Rejected");
        public string CenterXText { get; private init; }
        public string CenterYText { get; private init; }
        public string WidthText { get; private init; }
        public string HeightText { get; private init; }
        public string ScoreText { get; private init; }
        public string AngleText { get; private init; }
        public string SizeText => WidthText + "×" + HeightText;
        public string Reason { get; private init; }
        internal VisionToolOverlay Overlay { get; private init; }
        internal void NotifyLocalization() => OnPropertyChanged(string.Empty);
        internal static OpenVisionWorkbenchResultObjectViewModel FromObject(VisionPipelineObjectResult item) => new()
        {
            Number = item.Accepted ? item.Number.ToString(CultureInfo.InvariantCulture) : "—", Accepted = item.Accepted, Area = item.Area,
            CenterXText = item.CenterX.ToString("N1", CultureInfo.CurrentCulture), CenterYText = item.CenterY.ToString("N1", CultureInfo.CurrentCulture),
            WidthText = item.BoundsWidth.ToString(CultureInfo.InvariantCulture), HeightText = item.BoundsHeight.ToString(CultureInfo.InvariantCulture),
            Reason = item.RejectReason,
            Overlay = new VisionToolOverlay { Kind = VisionToolOverlayKind.Rectangle, Bounds = new RectangleF((float)item.BoundsX, (float)item.BoundsY, (float)item.BoundsWidth, (float)item.BoundsHeight), Label = item.Accepted ? item.Number.ToString(CultureInfo.InvariantCulture) : string.Empty }
        };
        internal static OpenVisionWorkbenchResultObjectViewModel FromMatch(VisionPipelineMatchResultEvidence item, int number) => new()
        {
            Number = number.ToString(CultureInfo.InvariantCulture), Accepted = true,
            CenterXText = item.CenterX.ToString("N1", CultureInfo.CurrentCulture), CenterYText = item.CenterY.ToString("N1", CultureInfo.CurrentCulture),
            WidthText = item.BoundsWidth.ToString("0.##", CultureInfo.CurrentCulture), HeightText = item.BoundsHeight.ToString("0.##", CultureInfo.CurrentCulture),
            ScoreText = item.Score.ToString("F3", CultureInfo.CurrentCulture), AngleText = item.Angle.ToString("N2", CultureInfo.CurrentCulture) + "°",
            Reason = string.Empty,
            Overlay = new VisionToolOverlay { Kind = VisionToolOverlayKind.Rectangle, Bounds = new RectangleF((float)item.BoundsX, (float)item.BoundsY, (float)item.BoundsWidth, (float)item.BoundsHeight), Label = number.ToString(CultureInfo.InvariantCulture) }
        };
    }

    // Stable row identity keeps WPF selection while retained display values or language change.
    public sealed class OpenVisionWorkbenchResultRowViewModel : ObservableObject
    {
        internal OpenVisionWorkbenchResultRowViewModel(OpenVisionWorkbenchStepViewModel step) { Step = step; }
        public OpenVisionWorkbenchStepViewModel Step { get; }
        public string Number { get; private set; }
        public string Name { get; private set; }
        public string ToolName { get; private set; }
        public string ResultStatusKind { get; private set; }
        public string ResultStatusText { get; private set; }
        public string ResultValueText { get; private set; }
        public string ResultElapsedText { get; private set; }
        public bool IsTrial => Step == null;
        internal void Update(string number, string name, string toolName, string kind, string status, string value, string elapsed)
        {
            Number = number; Name = name; ToolName = toolName; ResultStatusKind = kind;
            ResultStatusText = status; ResultValueText = value; ResultElapsedText = elapsed;
            OnPropertyChanged(string.Empty);
        }
    }

    public sealed record OpenVisionWorkbenchMetricViewModel(string Name, string Value);
    public sealed record OpenVisionWorkbenchAreaBin(double X, double AcceptedHeight, double RejectedHeight)
    {
        public double AcceptedY => 52D - AcceptedHeight;
        public double RejectedY => 52D - AcceptedHeight - RejectedHeight;
    }
}
