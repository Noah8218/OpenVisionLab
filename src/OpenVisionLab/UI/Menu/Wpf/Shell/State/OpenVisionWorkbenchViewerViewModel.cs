using MahApps.Metro.IconPacks;
using OpenVisionLab.Core;
using OpenVisionLab.Vision2D.Pipeline;
using OpenVisionLab.Vision2D.Tool;
using OpenVisionLab.ImageSpace.Core;
using OpenVisionLab.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Input;
using Size = System.Windows.Size;

namespace OpenVisionLab
{
    // Main display slots are separate from ImageSpace layers and inspection input routing.
    public sealed class OpenVisionWorkbenchViewerViewModel : ObservableObject, IDisposable
    {
        private readonly IDisplayManager displayManager;
        private readonly OpenVisionWorkbenchViewModel workbench;
        private readonly Dictionary<int, OpenVisionWorkbenchViewerPane[]> layouts;
        private readonly Dictionary<string, Bitmap> snapshots = new();
        private readonly Dictionary<string, string> snapshotNames = new();
        private readonly Dictionary<string, IReadOnlyList<OpenVisionMainPaneOverlay>> snapshotOverlays = new();
        private int layout = 2;
        private int activeIndex = 1;
        private bool disposed;
        private bool sync = true;
        private bool overlayVisible = true;
        private bool roiVisible = true;
        private string resultObjectLayer;
        private bool resultObjectsInFinal;
        private IReadOnlyList<VisionToolOverlay> rejectedResultObjects = Array.Empty<VisionToolOverlay>();
        private VisionToolOverlay selectedResultObject;

        internal OpenVisionWorkbenchViewerViewModel(IDisplayManager displayManager, OpenVisionWorkbenchViewModel workbench)
        {
            this.displayManager = displayManager ?? throw new ArgumentNullException(nameof(displayManager));
            this.workbench = workbench ?? throw new ArgumentNullException(nameof(workbench));
            layouts = new Dictionary<int, OpenVisionWorkbenchViewerPane[]>
            {
                [1] = CreatePanes(1, "follow:out"),
                [2] = CreatePanes(2, "follow:in", "follow:out"),
                [4] = CreatePanes(4, "layer:Main", "follow:in", "follow:out", "final")
            };
            ToggleOverlayCommand = new RelayCommand(() => IsOverlayVisible = !IsOverlayVisible, () => !disposed);
            ToggleRoiCommand = new RelayCommand(() => IsRoiVisible = !IsRoiVisible, () => !disposed);
            ToggleSyncCommand = new RelayCommand(() => IsSyncEnabled = !IsSyncEnabled, () => !disposed);
            SingleCommand = new RelayCommand(() => SetLayout(1));
            Split2Command = new RelayCommand(() => SetLayout(2));
            Split4Command = new RelayCommand(() => SetLayout(4));
            ClearSnapshotsCommand = new RelayCommand(ClearSnapshots, () => snapshots.Count > 0);
            workbench.Pipeline.PropertyChanged += OnSourceStateChanged;
            workbench.Tool.PropertyChanged += OnSourceStateChanged;
            workbench.Shell.PropertyChanged += OnSourceStateChanged;
            OpenVisionLanguageService.LanguageChanged += OnLanguageChanged;
        }

        internal event EventHandler LayoutChanged = delegate { };
        internal event EventHandler SourcesChanged = delegate { };
        internal bool IsDisposed => disposed;
        public int Layout => layout;
        public bool IsSyncEnabled
        {
            get => sync;
            set
            {
                if (disposed || !SetProperty(ref sync, value)) { return; }
                OnPropertyChanged(nameof(SyncIcon));
                if (value && Panes.ElementAtOrDefault(activeIndex)?.Viewport is OpenVisionImageViewport viewport) { PublishViewport(Panes[activeIndex], viewport); }
            }
        }
        public PackIconMaterialKind SyncIcon => sync ? PackIconMaterialKind.LinkVariant : PackIconMaterialKind.LinkVariantOff;
        public ICommand ToggleSyncCommand { get; }
        internal void PublishViewport(OpenVisionWorkbenchViewerPane source, OpenVisionImageViewport viewport)
        {
            if (disposed || !source.HasImage || !viewport.IsValid) { return; }
            bool changed = false;
            foreach (OpenVisionWorkbenchViewerPane pane in sync ? layouts.Values.SelectMany(value => value) : new[] { source }) { changed |= pane.SetViewport(viewport); }
            if (changed) { NotifySources(); }
        }
        public bool IsOverlayVisible { get => overlayVisible; set { if (!disposed && SetProperty(ref overlayVisible, value)) { NotifySources(); } } }
        public bool IsRoiVisible { get => roiVisible; set { if (!disposed && SetProperty(ref roiVisible, value)) { NotifySources(); } } }
        public ICommand ToggleOverlayCommand { get; }
        public ICommand ToggleRoiCommand { get; }
        public bool IsSingle => layout == 1;
        public bool IsSplit2 => layout == 2;
        public bool IsSplit4 => layout == 4;
        public IReadOnlyList<OpenVisionWorkbenchViewerPane> Panes => layouts[layout];
        public string ActivePaneId => Panes[Math.Clamp(activeIndex, 0, Panes.Count - 1)].Id;
        public ICommand SingleCommand { get; }
        public ICommand Split2Command { get; }
        public ICommand Split4Command { get; }
        public ICommand ClearSnapshotsCommand { get; }
        public string this[string key] => OpenVisionLanguageService.T(key);
        internal OpenVisionShellHostWorkspaceCommandSurface Workspace => workbench.Workspace;
        internal OpenVisionWorkbenchPipelineViewModel Pipeline => workbench.Pipeline;
        internal bool HasInputImage => workbench.Shell.WorkspaceInputPreview != null;
        internal Func<string, bool> OpenLayerWindow { get; set; }
        internal Action<string, Bitmap, IReadOnlyList<OpenVisionMainPaneOverlay>> OpenSnapshotWindow { get; set; }
        internal Action<string, Bitmap> SaveImage { get; set; }

        internal OpenVisionWorkbenchViewerPane FindPane(string id) => Panes.FirstOrDefault(pane => pane.Id == id);
        internal string[] DocumentIds => Panes.Select(pane => pane.Id).ToArray();

        internal void Activate(string id)
        {
            int index = Array.FindIndex(layouts[layout], pane => pane.Id == id);
            if (index < 0) { return; }
            activeIndex = index;
            foreach (OpenVisionWorkbenchViewerPane pane in Panes) { pane.NotifyActive(); }
        }

        internal string ResolveLayer(string source)
        {
            if (disposed) { return null; }
            if (source.StartsWith("layer:", StringComparison.Ordinal)) { return source[6..]; }
            if (source == "final") { return "Main"; }
            var step = workbench.Tool.IsOpen ? workbench.Tool.CurrentStep : workbench.Pipeline.GetStep(workbench.Pipeline.SelectedStepIndex);
            return source == "follow:in" ? step?.InputLayer ?? "Main" : step?.OutputLayer;
        }

        internal void ReadImage(OpenVisionWorkbenchViewerPane pane, Action<Bitmap> read)
        {
            if (disposed) { read(null); return; }
            if (pane.Source.StartsWith("snap:", StringComparison.Ordinal))
            {
                snapshots.TryGetValue(pane.Source, out Bitmap snapshot);
                read(snapshot);
                return;
            }
            string layerName = ResolveLayer(pane.Source);
            if (workbench.Pipeline.TryAcquireResultSnapshot(layerName, out Bitmap result))
            {
                using (result) { read(result); }
                return;
            }
            using ImageSpaceImageLease lease = string.IsNullOrWhiteSpace(layerName) ? null : displayManager.ImageSpace.AcquireImage(layerName);
            read(lease?.Image);
        }

        internal IReadOnlyList<KeyValuePair<string, string>> SourceOptions
        {
            get
            {
                if (disposed) { return Array.Empty<KeyValuePair<string, string>>(); }
                List<KeyValuePair<string, string>> choices = new()
                {
                    new("follow:in", this["Workbench.Pane.Input"]),
                    new("follow:out", this["Workbench.Pane.Output"]),
                    new("final", this["Workbench.Pane.Final"]),
                    new("layer:Main", this["Workbench.Pane.Original"])
                };
                foreach (string name in workbench.Pipeline.Rows.Select(row => row.OutputLayer).Concat(workbench.Shell.LayerOptions)
                    .Append(workbench.Tool.OutputLayer).Where(name => !string.IsNullOrWhiteSpace(name)).Distinct(StringComparer.OrdinalIgnoreCase))
                {
                    string key = "layer:" + name;
                    if (choices.All(choice => !string.Equals(choice.Key, key, StringComparison.OrdinalIgnoreCase))) { choices.Add(new(key, name)); }
                }
                choices.AddRange(snapshotNames.Select(item => new KeyValuePair<string, string>(item.Key, item.Value)));
                return choices;
            }
        }

        internal bool IsTrialOutput(OpenVisionWorkbenchViewerPane pane) => !disposed && workbench.Tool.IsNewTest
            && pane.Source != "final" && !pane.Source.StartsWith("snap:", StringComparison.Ordinal)
            && !string.Equals(ResolveLayer(pane.Source), "Main", StringComparison.OrdinalIgnoreCase)
            && !string.IsNullOrWhiteSpace(workbench.Tool.OutputLayer) && string.Equals(ResolveLayer(pane.Source), workbench.Tool.OutputLayer, StringComparison.OrdinalIgnoreCase);

        internal int ProducerIndex(OpenVisionWorkbenchViewerPane pane) => pane.Source == "final" || pane.Source.StartsWith("snap:", StringComparison.Ordinal) || IsTrialOutput(pane) ? -1
            : workbench.Pipeline.Rows.ToList().FindLastIndex(row => string.Equals(row.OutputLayer, ResolveLayer(pane.Source), StringComparison.OrdinalIgnoreCase));

        internal bool IsPrevious(OpenVisionWorkbenchViewerPane pane) => pane.Source.StartsWith("snap:", StringComparison.Ordinal) ? false
            : pane.Source == "final" ? workbench.Pipeline.IsVerdictPrevious : IsTrialOutput(pane) ? workbench.Tool.IsResultPrevious : workbench.Pipeline.IsResultPrevious(ProducerIndex(pane));

        internal bool CanRunToSource(OpenVisionWorkbenchViewerPane pane) => !disposed && workbench.Pipeline.RunToRowCommand.CanExecute(ProducerIndex(pane));
        internal void RunToSource(OpenVisionWorkbenchViewerPane pane) { if (CanRunToSource(pane)) { workbench.Pipeline.RunToRowCommand.Execute(ProducerIndex(pane)); } }

        private string SnapshotParameterHint(OpenVisionWorkbenchViewerPane pane)
        {
            VisionPipelineStepResultSummary result = IsTrialOutput(pane) ? workbench.Tool.CurrentResult : workbench.Pipeline.GetResult(ProducerIndex(pane));
            if (result?.Executed != true || result.Skipped) { return string.Empty; }
            if (result.ToolType == "Threshold") { return OpenVisionPipelineReviewResultPresenter.FormatWorkbenchSummary(result); }
            if (result.ToolType == "Filter" && result.Parameters.TryGetValue("FilterType", out string type))
            {
                result.Parameters.TryGetValue(type == "MedianBlur" ? "MedianKernelSize" : "KernelWidth", out string kernel);
                return type + " " + kernel;
            }
            return string.Empty;
        }

        internal string DisplayName(OpenVisionWorkbenchViewerPane pane)
        {
            if (snapshotNames.TryGetValue(pane.Source, out string snapshotName)) { return snapshotName; }
            if (pane.Source == "final") { return this["Workbench.Pane.FinalName"]; }
            string name = ResolveLayer(pane.Source);
            if (string.IsNullOrWhiteSpace(name)) { return this["Workbench.Pane.NoSelection"]; }
            if (string.Equals(name, "Main", StringComparison.OrdinalIgnoreCase)) { return this["Workbench.Pane.Original"]; }
            int index = workbench.Pipeline.Rows.ToList().FindIndex(row => string.Equals(row.OutputLayer, name, StringComparison.OrdinalIgnoreCase));
            return (index >= 0 ? (index + 1).ToString("00", CultureInfo.InvariantCulture) + " · " : string.Empty) + name;
        }

        internal IReadOnlyList<OpenVisionMainPaneOverlay> ReadOverlays(OpenVisionWorkbenchViewerPane pane)
        {
            if (disposed) { return Array.Empty<OpenVisionMainPaneOverlay>(); }
            if (pane.Source.StartsWith("snap:", StringComparison.Ordinal)) { return snapshotOverlays.TryGetValue(pane.Source, out var frozen) ? frozen : Array.Empty<OpenVisionMainPaneOverlay>(); }
            List<OpenVisionMainPaneOverlay> overlays = new();
            foreach (OpenVisionWorkbenchStepViewModel row in workbench.Pipeline.Rows)
            {
                VisionPipelineStep step = workbench.Pipeline.GetStep(row.Index);
                VisionPipelineStepResultSummary result = workbench.Pipeline.GetResult(row.Index);
                if (result == null || !result.Executed || result.Skipped || (pane.Source == "final" ? !OpenVisionWorkbenchPipelineViewModel.IsJudged(step) : !string.Equals(row.OutputLayer, ResolveLayer(pane.Source), StringComparison.OrdinalIgnoreCase))) { continue; }
                foreach (var overlay in result.Overlays) { overlays.Add(new(VisionPipelineOverlayMergeService.CloneOverlay(overlay), false, !result.Success)); }
            }
            VisionPipelineStep holder = workbench.Tool.IsOpen ? workbench.Tool.CurrentStep : workbench.Pipeline.GetStep(workbench.Pipeline.SelectedStepIndex);
            if (pane.Source != "final" && holder != null && string.Equals(holder.InputLayer, ResolveLayer(pane.Source), StringComparison.OrdinalIgnoreCase)
                && holder.Parameters.TryGetValue("USE_ROI", out string use) && bool.TryParse(use, out bool enabled) && enabled)
            {
                IEnumerable<OpenCvSharp.Rect> regions = holder.Parameters.TryGetValue("USE_MULTI_ROI", out string multi) && bool.TryParse(multi, out bool isMulti) && isMulti
                    ? VisionPipelineAppToolFactory.GetRectList(holder.Parameters, "CvROIS", null)
                    : new[] { VisionPipelineAppToolFactory.GetRect(holder.Parameters, "CvROI", default) };
                int index = 0;
                foreach (OpenCvSharp.Rect region in regions.Where(region => region.Width > 0 && region.Height > 0))
                {
                    overlays.Add(new(new OpenVisionLab.Vision2D.Tool.VisionToolOverlay { Kind = OpenVisionLab.Vision2D.Tool.VisionToolOverlayKind.Rectangle, Bounds = new RectangleF(region.X, region.Y, region.Width, region.Height), Label = "ROI " + (++index).ToString(CultureInfo.InvariantCulture) }, true, false));
                }
            }
            if (!string.IsNullOrWhiteSpace(resultObjectLayer) && (pane.Source == "final" ? resultObjectsInFinal : string.Equals(resultObjectLayer, ResolveLayer(pane.Source), StringComparison.OrdinalIgnoreCase)))
            {
                overlays.AddRange(rejectedResultObjects.Select(shape => new OpenVisionMainPaneOverlay(VisionPipelineOverlayMergeService.CloneOverlay(shape), false, false, IsRejected: true)));
                if (selectedResultObject != null) { overlays.Add(new(VisionPipelineOverlayMergeService.CloneOverlay(selectedResultObject), false, false, IsSelected: true)); }
            }
            return overlays;
        }

        internal void SetResultObjects(string layer, IReadOnlyList<VisionToolOverlay> rejected, VisionToolOverlay selected, bool includeInFinal)
        {
            if (disposed) { return; }
            resultObjectLayer = layer;
            resultObjectsInFinal = includeInFinal;
            rejectedResultObjects = rejected.Select(VisionPipelineOverlayMergeService.CloneOverlay).ToArray();
            selectedResultObject = selected == null ? null : VisionPipelineOverlayMergeService.CloneOverlay(selected);
            NotifySources();
        }

        internal void Pin(OpenVisionWorkbenchViewerPane pane)
        {
            if (pane.Source.StartsWith("follow:", StringComparison.Ordinal))
            {
                string name = ResolveLayer(pane.Source);
                if (!string.IsNullOrWhiteSpace(name)) { pane.Source = "layer:" + name; }
            }
            else { pane.Source = pane.DefaultSource; }
        }

        internal void Snapshot(OpenVisionWorkbenchViewerPane pane)
        {
            ReadImage(pane, image =>
            {
                if (image == null) { return; }
                string key = "snap:" + Guid.NewGuid().ToString("N");
                snapshots.Add(key, new Bitmap(image));
                snapshotOverlays.Add(key, ReadOverlays(pane).Select(overlay => overlay with { Shape = VisionPipelineOverlayMergeService.CloneOverlay(overlay.Shape) }).ToArray());
                string hint = SnapshotParameterHint(pane);
                snapshotNames.Add(key, DisplayName(pane) + (string.IsNullOrEmpty(hint) ? string.Empty : " (" + hint + ")") + " · " + DateTime.Now.ToString("HH:mm:ss", CultureInfo.InvariantCulture));
                pane.Source = key;
            });
            NotifySources();
        }

        internal void Save(OpenVisionWorkbenchViewerPane pane)
        {
            Bitmap copy = null;
            ReadImage(pane, image => copy = OpenVisionPreviewImageFileService.CloneBitmapPreservingPixelFormat(image));
            using (copy) { if (copy != null) { SaveImage?.Invoke(DisplayName(pane), copy); } }
        }
        internal void Open(OpenVisionWorkbenchViewerPane pane)
        {
            if (!pane.Source.StartsWith("snap:", StringComparison.Ordinal) && OpenLayerWindow?.Invoke(ResolveLayer(pane.Source)) == true) { return; }
            ReadImage(pane, image => { if (image != null) { OpenSnapshotWindow?.Invoke(DisplayName(pane), image, ReadOverlays(pane)); } });
        }

        internal void NotifySources()
        {
            if (disposed) { return; }
            foreach (OpenVisionWorkbenchViewerPane pane in layouts.Values.SelectMany(value => value)) { pane.NotifySource(); }
            SourcesChanged(this, EventArgs.Empty);
            CommandManager.InvalidateRequerySuggested();
        }

        private OpenVisionWorkbenchViewerPane[] CreatePanes(int count, params string[] sources) => sources.Select((source, index) => new OpenVisionWorkbenchViewerPane(this, $"MainViewer:{count}:{index}", source)).ToArray();
        private void SetLayout(int count)
        {
            if (disposed) { return; }
            if (layout == count) { OnPropertyChanged(nameof(IsSingle)); OnPropertyChanged(nameof(IsSplit2)); OnPropertyChanged(nameof(IsSplit4)); return; }
            layout = count;
            activeIndex = Math.Min(activeIndex, count - 1);
            OnPropertyChanged(nameof(Layout)); OnPropertyChanged(nameof(IsSingle)); OnPropertyChanged(nameof(IsSplit2)); OnPropertyChanged(nameof(IsSplit4)); OnPropertyChanged(nameof(Panes));
            foreach (OpenVisionWorkbenchViewerPane pane in Panes) { pane.NotifyActive(); }
            LayoutChanged(this, EventArgs.Empty);
        }

        private void ClearSnapshots()
        {
            foreach (OpenVisionWorkbenchViewerPane pane in layouts.Values.SelectMany(value => value))
            {
                if (pane.Source.StartsWith("snap:", StringComparison.Ordinal)) { pane.RestoreDefault(); }
            }
            foreach (Bitmap image in snapshots.Values) { image.Dispose(); }
            snapshots.Clear(); snapshotNames.Clear(); snapshotOverlays.Clear();
            NotifySources();
        }
        private void OnSourceStateChanged(object sender, PropertyChangedEventArgs e)
        {
            // Pipeline publishes a full snapshot after Rows has synchronously refreshed the active Tool.
            if (ReferenceEquals(sender, workbench.Pipeline))
            {
                if (string.IsNullOrEmpty(e.PropertyName)) { NotifySources(); }
                return;
            }
            if (ReferenceEquals(sender, workbench.Tool) && workbench.Pipeline.IsPublishingState) { return; }
            if (string.IsNullOrEmpty(e.PropertyName) || e.PropertyName is "InputLayer" or "OutputLayer" or "IsOpen" or "CurrentStep" or "LayerOptions") { NotifySources(); }
        }
        private void OnLanguageChanged(object sender, EventArgs e) { OnPropertyChanged(string.Empty); NotifySources(); }
        public void Dispose()
        {
            if (disposed) { return; }
            disposed = true;
            workbench.Pipeline.PropertyChanged -= OnSourceStateChanged;
            workbench.Tool.PropertyChanged -= OnSourceStateChanged;
            workbench.Shell.PropertyChanged -= OnSourceStateChanged;
            OpenVisionLanguageService.LanguageChanged -= OnLanguageChanged;
            foreach (Bitmap image in snapshots.Values) { image.Dispose(); }
            snapshots.Clear(); snapshotNames.Clear(); snapshotOverlays.Clear();
            rejectedResultObjects = Array.Empty<VisionToolOverlay>(); selectedResultObject = null; resultObjectLayer = null;
            foreach (OpenVisionWorkbenchViewerPane pane in layouts.Values.SelectMany(value => value)) { pane.FitAction = null; pane.UpdateImage(null); }
            CommandManager.InvalidateRequerySuggested();
            LayoutChanged = delegate { }; SourcesChanged = delegate { };
            OpenLayerWindow = null; OpenSnapshotWindow = null; SaveImage = null;
        }
    }

    internal sealed record OpenVisionMainPaneOverlay(OpenVisionLab.Vision2D.Tool.VisionToolOverlay Shape, bool IsRoi, bool Failed, bool IsSelected = false, bool IsRejected = false);

    public sealed class OpenVisionWorkbenchViewerPane : ObservableObject
    {
        private readonly OpenVisionWorkbenchViewerViewModel owner;
        private string source;
        private bool hasImage;
        private bool isNarrow;
        private bool isShort;
        private bool isMediumHeight;
        private bool isEmptyActionsStacked;
        private string sizeText = string.Empty;
        private OpenVisionImageViewport? viewport;
        internal OpenVisionWorkbenchViewerPane(OpenVisionWorkbenchViewerViewModel owner, string id, string source)
        {
            this.owner = owner; Id = id; this.source = DefaultSource = source;
            NavigationChangedCommand = new RelayCommand<OpenVisionImageViewport>(value => owner.PublishViewport(this, value), _ => !owner.IsDisposed);
            PinCommand = new RelayCommand(() => owner.Pin(this), () => !owner.IsDisposed && (IsPinned || !string.IsNullOrWhiteSpace(owner.ResolveLayer(Source))));
            FitCommand = new RelayCommand(() => FitAction?.Invoke(), () => HasImage);
            UpdateSizeCommand = new RelayCommand<Size>(size =>
            {
                SetProperty(ref isNarrow, size.Width <= 440D, nameof(IsNarrow));
                SetProperty(ref isShort, size.Height <= 220D, nameof(IsShort));
                SetProperty(ref isMediumHeight, size.Height <= 300D, nameof(IsMediumHeight));
                SetProperty(ref isEmptyActionsStacked, size.Height <= 220D && size.Width <= 260D, nameof(IsEmptyActionsStacked));
            });
            SnapshotCommand = new RelayCommand(() => owner.Snapshot(this), () => HasImage);
            SaveCommand = new RelayCommand(() => owner.Save(this), () => HasImage);
            OpenCommand = new RelayCommand(() => owner.Open(this), () => HasImage);
            RunToSourceCommand = new RelayCommand(() => owner.RunToSource(this), () => owner.CanRunToSource(this));
        }
        internal IReadOnlyList<OpenVisionMainPaneOverlay> Overlays => owner.ReadOverlays(this).Where(overlay => overlay.IsRoi ? owner.IsRoiVisible : owner.IsOverlayVisible).ToArray();
        public OpenVisionImageViewport? Viewport => viewport;
        public ICommand NavigationChangedCommand { get; }
        internal bool SetViewport(OpenVisionImageViewport value) => SetProperty(ref viewport, value, nameof(Viewport));
        public string Id { get; }
        internal string DefaultSource { get; }
        public string Source
        {
            get => source;
            set { if (!string.IsNullOrWhiteSpace(value) && owner.SourceOptions.Any(choice => choice.Key == value) && SetProperty(ref source, value)) { owner.NotifySources(); } }
        }
        public string this[string key] => owner[key];
        public bool IsNarrow => isNarrow;
        public bool IsShort => isShort;
        public bool IsMediumHeight => isMediumHeight;
        public bool IsEmptyActionsStacked => isEmptyActionsStacked;
        public ICommand UpdateSizeCommand { get; }
        public bool HasImage => hasImage;
        public bool IsEmpty => !hasImage;
        public bool IsSnapshot => HasImage && source.StartsWith("snap:", StringComparison.Ordinal);
        public bool IsPrevious => HasImage && owner.IsPrevious(this);
        public bool ShowFinalVerdict => source == "final" && HasImage && owner.Pipeline.HasVerdict && !owner.Pipeline.IsVerdictPrevious && owner.Pipeline.VerdictKind is "Ok" or "Ng" or "Error";
        public string FinalVerdictValue => owner.Pipeline.VerdictValue;
        public string FinalVerdictKind => owner.Pipeline.VerdictKind;
        public IReadOnlyList<OpenVisionWorkbenchStepViewModel> FinalFailureRows => ShowFinalVerdict ? owner.Pipeline.Rows.Where(row =>
        {
            VisionPipelineStepResultSummary result = owner.Pipeline.GetResult(row.Index);
            return row.HasError && result?.Executed == true && !result.Skipped && (!result.Success || result.IsAcceptanceNg);
        }).ToArray() : Array.Empty<OpenVisionWorkbenchStepViewModel>();
        public bool HasFinalReasons => FinalFailureRows.Count > 0;
        public bool ShowRunToAction => IsEmpty && owner.HasInputImage && owner.ProducerIndex(this) >= 0;
        public bool IsActive => owner.ActivePaneId == Id;
        public bool IsPinned => !source.StartsWith("follow:", StringComparison.Ordinal);
        public string Title => owner.DisplayName(this) + (owner.IsTrialOutput(this) ? " · " + owner["Workbench.Results.Test"] : string.Empty);
        public string Kind => owner[source == "final" ? "Workbench.Pane.Judgement" : source.StartsWith("snap:", StringComparison.Ordinal) ? "Workbench.Pane.Snapshot"
            : source == "follow:in" ? "Workbench.Pane.InputKind" : source == "follow:out" ? "Workbench.Pane.OutputKind" : "Workbench.Pane.Layer"];
        public PackIconMaterialKind Icon => source == "final" ? PackIconMaterialKind.Layers : source.StartsWith("snap:", StringComparison.Ordinal) ? PackIconMaterialKind.Pin
            : source.StartsWith("follow:", StringComparison.Ordinal) ? PackIconMaterialKind.ArrowRightThin : PackIconMaterialKind.ImageOutline;
        public PackIconMaterialKind PinIcon => IsPinned ? PackIconMaterialKind.Pin : PackIconMaterialKind.PinOff;
        public string SizeText => sizeText;
        public bool IsTrialEmpty => IsEmpty && owner.HasInputImage && owner.IsTrialOutput(this);
        public string EmptyTitle => owner[source == "follow:out" && string.IsNullOrWhiteSpace(owner.ResolveLayer(source)) ? "Workbench.Pane.NoStep" : IsTrialEmpty ? "Shell.DirectStatusReady" : "Workbench.Viewer.NoImage"];
        public PackIconMaterialKind EmptyIcon => source == "follow:out" && string.IsNullOrWhiteSpace(owner.ResolveLayer(source)) ? PackIconMaterialKind.CursorDefaultOutline
            : !owner.HasInputImage && !source.StartsWith("snap:", StringComparison.Ordinal) && !string.IsNullOrWhiteSpace(owner.ResolveLayer(source)) ? PackIconMaterialKind.ImagePlusOutline : PackIconMaterialKind.ImageOffOutline;
        public string EmptyDetail => source == "follow:out" && string.IsNullOrWhiteSpace(owner.ResolveLayer(source)) ? owner["Workbench.Pane.SelectStep"]
            : source.StartsWith("snap:", StringComparison.Ordinal) ? owner["Workbench.Pane.SelectImage"] : string.IsNullOrWhiteSpace(owner.ResolveLayer(source)) ? string.Empty
            : !owner.HasInputImage || owner.ResolveLayer(source) == "Main" ? owner["Workbench.Pane.EmptyImageDetail"] : IsTrialEmpty ? owner["ToolView.NoOutputImageDetail"] : string.Format(CultureInfo.CurrentCulture, owner["Workbench.Pane.NotMade"], Title);
        public string EmptyDetailPrefix => IsTrialEmpty ? OpenVisionLanguageService.CurrentLanguage == OpenVisionLanguage.English ? owner["Workbench.Pane.RunPrompt"] + " " : string.Empty : EmptyDetail;
        public string EmptyDetailAction => IsTrialEmpty ? owner["Workbench.Tool.RunTool"] + (OpenVisionLanguageService.CurrentLanguage == OpenVisionLanguage.Korean ? "(F8)" : " (F8)") : string.Empty;
        public string EmptyDetailSuffix => IsTrialEmpty ? (OpenVisionLanguageService.CurrentLanguage == OpenVisionLanguage.Korean ? owner["Workbench.Pane.RunPrompt"] : string.Empty) + "." : string.Empty;
        public bool ShowOpenActions => IsEmpty && !owner.HasInputImage && !source.StartsWith("snap:", StringComparison.Ordinal) && !string.IsNullOrWhiteSpace(owner.ResolveLayer(source));
        public IReadOnlyList<KeyValuePair<string, string>> Sources => owner.SourceOptions;
        internal Action FitAction { get; set; }
        public ICommand FitCommand { get; }
        public ICommand PinCommand { get; }
        public ICommand SnapshotCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand OpenCommand { get; }
        public ICommand RunToSourceCommand { get; }
        public ICommand ClearSnapshotsCommand => owner.ClearSnapshotsCommand;
        public ICommand LoadImageCommand => owner.Workspace.LoadImageCommand;
        public ICommand OpenFolderCommand => owner.Workspace.OpenFolderCommand;
        public ICommand OpenSampleCommand => owner.Workspace.OpenSampleCommand;
        internal void UpdateImage(Bitmap image)
        {
            hasImage = image != null;
            sizeText = image == null ? string.Empty : image.Width.ToString(CultureInfo.InvariantCulture) + "×" + image.Height.ToString(CultureInfo.InvariantCulture);
            OnPropertyChanged(nameof(HasImage)); OnPropertyChanged(nameof(IsEmpty)); OnPropertyChanged(nameof(SizeText)); OnPropertyChanged(nameof(ShowOpenActions));
            OnPropertyChanged(nameof(IsSnapshot)); OnPropertyChanged(nameof(IsPrevious)); OnPropertyChanged(nameof(ShowRunToAction));
            OnPropertyChanged(nameof(ShowFinalVerdict)); OnPropertyChanged(nameof(FinalFailureRows)); OnPropertyChanged(nameof(HasFinalReasons));
            OnPropertyChanged(nameof(IsTrialEmpty)); OnPropertyChanged(nameof(EmptyTitle)); OnPropertyChanged(nameof(EmptyDetail));
            OnPropertyChanged(nameof(EmptyDetailPrefix)); OnPropertyChanged(nameof(EmptyDetailAction)); OnPropertyChanged(nameof(EmptyDetailSuffix));
        }
        internal void RestoreDefault() { source = DefaultSource; NotifySource(); }
        internal void NotifyActive() => OnPropertyChanged(nameof(IsActive));
        internal void NotifySource()
        {
            OnPropertyChanged(string.Empty);
        }
    }
}
