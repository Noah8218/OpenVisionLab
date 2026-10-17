using OpenVisionLab.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace OpenVisionLab
{
    public sealed class OpenVisionWorkbenchImagesViewModel : ObservableObject, IDisposable
    {
        private readonly OpenVisionShellPreviewViewModel shell;
        private readonly Func<string, bool> loadImage;
        private readonly Action clearInput;
        private readonly ObservableCollection<OpenVisionWorkbenchImageItem> images = new();
        private string trialEvidenceDirectory = string.Empty;
        private VisionToolNImageVerificationSession trialSession;
        private readonly ObservableCollection<OpenVisionWorkbenchImageItem> visibleImages = new();
        private OpenVisionWorkbenchImageItem current;
        private string filter = "all";
        private bool listMode;
        private bool loading;
        private bool updatingItems;
        private bool disposed;
        private OpenVisionWorkbenchPipelineViewModel pipeline;
        private Func<string, bool> loadBatchImage;
        private Func<Bitmap> captureInput;
        private Action<Bitmap, string> restoreInput;
        private long observedRecipeRevision;
        private bool hasBatchSummary;
        private bool batchStopped;
        private bool batchSummaryStale;
        private int batchPlanned;
        private int batchIndex;
        private DateTime batchAt;
        private string batchImageName = "";
        private string batchError = "";

        internal OpenVisionWorkbenchImagesViewModel(OpenVisionShellPreviewViewModel shell, Func<string, bool> loadImage, Action clearInput)
        {
            this.shell = shell ?? throw new ArgumentNullException(nameof(shell));
            this.loadImage = loadImage ?? throw new ArgumentNullException(nameof(loadImage));
            this.clearInput = clearInput ?? throw new ArgumentNullException(nameof(clearInput));
            loadBatchImage = loadImage;
            SelectImageCommand = new RelayCommand<object>(SelectImage, _ => CanEdit);
            RunBatchCommand = new RelayCommand<string>(async scope => await RunBatchAsync(scope), CanRunBatch);
            PreviousCommand = new RelayCommand(() => TrySelect(images[CurrentIndex - 1]), () => CanEdit && CurrentIndex > 0);
            NextCommand = new RelayCommand(() => TrySelect(images[CurrentIndex + 1]), () => CanEdit && CurrentIndex >= 0 && CurrentIndex < images.Count - 1);
            RemoveCommand = new RelayCommand(RemoveSelected, () => CanEdit && SelectedCount > 0);
            LabelCommand = new RelayCommand<string>(SetLabel, _ => CanEdit && (SelectedCount > 0 || current != null));
            SelectAllCommand = new RelayCommand(() => SetSelection(true), () => CanEdit && images.Count > 0);
            ClearSelectionCommand = new RelayCommand(() => SetSelection(false), () => CanEdit && SelectedCount > 0);
            FilterCommand = new RelayCommand<string>(value => { filter = value ?? "all"; RaiseState(); });
            ListModeCommand = new RelayCommand(() => { listMode = true; RaiseState(); });
            ThumbnailModeCommand = new RelayCommand(() => { listMode = false; RaiseState(); });
            shell.PropertyChanged += OnInputChanged;
            OpenVisionLanguageService.LanguageChanged += OnLanguageChanged;
            UpdateCurrentInput();
        }

        internal event Action<int> ImportStarted;
        internal event Action<int, int, int> ImportCompleted;
        internal event Action<int, int, string> BatchProgressed;
        internal event Action<string, int, int, int> BatchFinished;

        internal bool IsSampleImport { get; set; }

        public IReadOnlyList<OpenVisionWorkbenchImageItem> Images => images;
        public IEnumerable<OpenVisionWorkbenchImageItem> VisibleImages => visibleImages;
        public int Count => images.Count;
        public string BatchCountText => Count > 0 ? Format("Workbench.Run.ImageCountFormat", Count) : string.Empty;
        public string RunScopeImageText => HasImages ? Format("Workbench.Run.ImagePositionFormat", CurrentIndex + 1, Count) : T("Workbench.Viewer.NoImage");
        public bool CanEdit => !disposed && pipeline?.IsRunning != true;
        public string BatchDisabledReason => CanRunBatch("all") ? string.Empty : !CanEdit ? T(disposed ? "Workbench.Command.ContextUnavailable" : "Workbench.Status.Running")
            : Count == 0 ? T("Workbench.Viewer.NoImage") : pipeline?.RunDisabledReason ?? T("Workbench.Command.ContextUnavailable");
        public string BatchDisabledToolTip => T("Workbench.Batch.All") + " (F6) — " + BatchDisabledReason;
        public string SelectedBatchDisabledReason => !CanEdit || Count == 0 || pipeline?.CanStartBatch != true ? BatchDisabledReason
            : SelectedCount == 0 ? T("Workbench.Pane.NoSelection") : string.Empty;
        public bool IsBatchRunning => pipeline?.IsBatchRunning == true;
        public int SelectedCount => images.Count(item => item.IsSelected);
        public int CurrentIndex => current == null ? -1 : images.IndexOf(current);
        public string PositionText => Count == 0 ? "0 / 0" : (CurrentIndex + 1) + " / " + Count;
        public OpenVisionWorkbenchImageItem CurrentImage => current;
        public string CurrentName => current?.Name ?? OpenVisionLanguageService.T("Workbench.Viewer.NoImage");
        public string HeaderText => OpenVisionLanguageService.T("Workbench.Bottom.Images") + (Count == 0 ? "" : " " + Count);
        public string LabelMenuText => OpenVisionLanguageService.T("Workbench.Images.Label") + (SelectedCount == 0 ? "" : " (" + SelectedCount + ")") + " ▾";
        public bool HasImages => Count > 0;
        public bool HasVisibleImages => VisibleImages.Any();
        public bool IsListMode => listMode;
        public bool IsThumbnailMode => !listMode;
        public string AllCountText => OpenVisionLanguageService.T("Workbench.Images.All") + " " + Count;
        public string BeforeCountText => OpenVisionLanguageService.T("Workbench.Images.Before") + " " + images.Count(item => item.Result == null || item.Result.IsStale);
        public string GoodCountText => OpenVisionLanguageService.T("Workbench.Images.Good") + " " + images.Count(item => item.Label == "good");
        public string BadCountText => OpenVisionLanguageService.T("Workbench.Images.Bad") + " " + images.Count(item => item.Label == "bad");
        public bool HasGoodImages => images.Any(item => item.Label == "good");
        public bool HasBadImages => images.Any(item => item.Label == "bad");
        public bool IsAllFilter => filter == "all";
        public bool IsBeforeFilter => filter == "before" || filter == "none";
        public bool IsGoodFilter => filter == "good";
        public bool IsBadFilter => filter == "bad";
        public bool HasBeforeImages => images.Any(item => item.Result == null || item.Result.IsStale);
        public bool HasOkImages => images.Any(item => item.FreshVerdict == "Ok");
        public bool HasNgImages => images.Any(item => item.FreshVerdict == "Ng");
        public bool HasErrorImages => images.Any(item => item.FreshVerdict == "Error");
        public bool HasOverImages => images.Any(item => item.MismatchKind == "over");
        public bool HasUnderImages => images.Any(item => item.MismatchKind == "under");
        public bool IsOkFilter => filter == "ok";
        public bool IsNgFilter => filter == "ng";
        public bool IsErrorFilter => filter == "err";
        public bool IsOverFilter => filter == "over";
        public bool IsUnderFilter => filter == "under";
        public string OkCountText => "OK " + images.Count(item => item.FreshVerdict == "Ok");
        public string NgCountText => "NG " + images.Count(item => item.FreshVerdict == "Ng");
        public string ErrorCountText => T("Workbench.Status.Error") + " " + images.Count(item => item.FreshVerdict == "Error");
        public string OverCountText => T("Workbench.Batch.Over") + " " + images.Count(item => item.MismatchKind == "over");
        public string UnderCountText => T("Workbench.Batch.Under") + " " + images.Count(item => item.MismatchKind == "under");
        public string this[string key] => T(key);
        public bool HasBatchDisplay => hasBatchSummary || IsBatchRunning || !string.IsNullOrEmpty(batchError);
        public bool HasBatchMessage => IsBatchRunning || !string.IsNullOrEmpty(batchError);
        public bool HasBatchSummary => hasBatchSummary;
        public bool IsBatchSummaryStale => batchSummaryStale;
        public string BatchRunLabelText => IsBatchRunning ? T(pipeline.IsStopRequested ? "Workbench.Batch.Stopping" : "Workbench.Command.Batch") : string.Empty;
        public string BatchRunPositionText => IsBatchRunning && batchPlanned > 0 ? batchIndex + "/" + batchPlanned : string.Empty;
        public double BatchProgressValue => IsBatchRunning && batchPlanned > 0 ? Math.Clamp(100D * batchIndex / batchPlanned, 0D, 100D) : 0D;
        public string BatchProgressText => IsBatchRunning ? Format(pipeline.IsStopRequested ? "Workbench.Batch.Stopping" : "Workbench.Batch.Progress", batchIndex, batchPlanned, batchImageName) : batchError;
        public string BatchSummaryText
        {
            get
            {
                OpenVisionWorkbenchImageItem[] done = images.Where(item => item.Result?.Source == "batch" && (!item.Result.IsStale || batchSummaryStale)).ToArray();
                int ok = done.Count(item => item.Result.Verdict == "Ok"), ng = done.Count(item => item.Result.Verdict == "Ng"), error = done.Count(item => item.Result.Verdict == "Error");
                int none = done.Count(item => item.Result.Verdict == "Done");
                OpenVisionWorkbenchImageItem[] labeled = done.Where(item => item.HasLabel && (item.Result.Verdict == "Ok" || item.Result.Verdict == "Ng")).ToArray();
                int over = labeled.Count(item => item.Label == "good" && item.Result.Verdict == "Ng"), under = labeled.Count(item => item.Label == "bad" && item.Result.Verdict == "Ok");
                double[] times = done.Select(item => item.Result.ElapsedMilliseconds).Where(value => double.IsFinite(value) && value > 0).OrderBy(value => value).ToArray();
                string text = Format("Workbench.Batch.Summary", batchAt.ToString("MM-dd HH:mm", CultureInfo.CurrentCulture), done.Length, ok, ng, error);
                if (batchStopped) { text += " · " + Format("Workbench.Batch.Stopped", batchPlanned); }
                if (none > 0) { text += " · " + T("Workbench.Status.NoVerdict") + " " + none; }
                text += " · " + (labeled.Length == 0 ? T("Workbench.Batch.Unlabeled") : Format("Workbench.Batch.LabelSummary", labeled.Length - over - under, labeled.Length, over, under, 100D * (labeled.Length - over - under) / labeled.Length));
                if (times.Length > 0) { text += " · " + Format("Workbench.Batch.Times", Ms(times.Average()), Ms(times[Math.Min(times.Length - 1, (int)Math.Ceiling(times.Length * .95D) - 1)]), Ms(times[^1])); }
                if (batchSummaryStale) { text += " · " + T("Workbench.Batch.Stale"); }
                return text;
            }
        }
        public ICommand RunBatchCommand { get; }
        public ICommand SelectImageCommand { get; }
        public ICommand PreviousCommand { get; }
        public ICommand NextCommand { get; }
        public ICommand RemoveCommand { get; }
        public ICommand LabelCommand { get; }
        public ICommand SelectAllCommand { get; }
        public ICommand ClearSelectionCommand { get; }
        public ICommand FilterCommand { get; }
        public ICommand ListModeCommand { get; }
        public ICommand ThumbnailModeCommand { get; }

        internal void AttachPipeline(OpenVisionWorkbenchPipelineViewModel value, Func<string, bool> batchLoader,
            Func<Bitmap> snapshotInput, Action<Bitmap, string> restoreSnapshot)
        {
            pipeline = value ?? throw new ArgumentNullException(nameof(value));
            loadBatchImage = batchLoader ?? loadImage;
            captureInput = snapshotInput;
            restoreInput = restoreSnapshot;
            observedRecipeRevision = pipeline.RecipeRevision;
            pipeline.InspectionCompleted += OnInspectionCompleted;
            pipeline.PropertyChanged += OnPipelineChanged;
            RaiseState();
        }

        internal bool CanRunBatch(string scope) => CanEdit && Count > 0 && pipeline?.CanStartBatch == true && (scope != "selected" || SelectedCount > 0);
        internal Task RunBatchForTestAsync(string scope = "all") => RunBatchAsync(scope);
        private async Task RunBatchAsync(string scope)
        {
            if (!CanRunBatch(scope) || !pipeline.TryBeginBatch()) { return; }
            OpenVisionWorkbenchImageItem original = current;
            OpenVisionWorkbenchImageItem[] targets = (scope == "selected" ? images.Where(item => item.IsSelected) : images).ToArray();
            Bitmap originalInput = null;
            batchPlanned = targets.Length;
            batchIndex = 0;
            batchStopped = false;
            batchError = "";
            try
            {
                originalInput = captureInput?.Invoke();
                for (int index = 0; index < targets.Length; index++)
                {
                    if (disposed || pipeline.IsStopRequested) { batchStopped = true; break; }
                    OpenVisionWorkbenchImageItem item = targets[index];
                    batchIndex = index + 1;
                    batchImageName = item.Name;
                    RaiseState();
                    bool loaded = string.IsNullOrEmpty(item.Path) && ReferenceEquals(item, original)
                        ? RestoreBatchInput(item, originalInput) : TrySelect(item, batch: true, reload: true);
                    if (!disposed) { BatchProgressed?.Invoke(batchIndex, batchPlanned, batchImageName); }
                    if (!loaded) { item.ReplaceResult(pipeline.CreateImageLoadError(item.Name)); continue; }
                    await pipeline.RunBatchImageAsync();
                    if (disposed) { return; }
                }
                hasBatchSummary = true;
                batchSummaryStale = false;
                batchAt = DateTime.Now;
            }
            catch (Exception exception) { if (!disposed) { batchError = T("Workbench.Status.Error") + ": " + exception.GetBaseException().Message; batchStopped = true; hasBatchSummary = true; batchAt = DateTime.Now; } }
            finally
            {
                try
                {
                    pipeline.EndBatch();
                    if (!disposed)
                    {
                        loading = true;
                        try
                        {
                            if (originalInput != null && restoreInput != null) { restoreInput(originalInput, original?.Path); SetCurrent(original); }
                            else if (original != null) { TrySelect(original); }
                        }
                        finally { loading = false; }
                        RaiseState();
                        OpenVisionWorkbenchImageItem[] done = images.Where(item => item.Result?.Source == "batch" && !item.Result.IsStale).ToArray();
                        BatchFinished?.Invoke(!string.IsNullOrEmpty(batchError) ? "Error" : batchStopped ? "Warning" : "Ok", done.Count(item => item.FreshVerdict == "Ok"), done.Count(item => item.FreshVerdict == "Ng"), done.Count(item => item.FreshVerdict == "Error"));
                    }
                }
                catch { if (!disposed) { BatchFinished?.Invoke("Error", 0, 0, 0); } throw; }
                finally { originalInput?.Dispose(); }
            }
        }

        private bool RestoreBatchInput(OpenVisionWorkbenchImageItem item, Bitmap image)
        {
            if (image == null || restoreInput == null) { return ReferenceEquals(current, item) && shell.HasWorkspaceInputImage; }
            loading = true;
            try { restoreInput(image, item.Path); SetCurrent(item); return true; }
            finally { loading = false; }
        }

        internal void RecordSourceVersion(string path, string version)
        {
            OpenVisionWorkbenchImageItem item = images.FirstOrDefault(value => string.Equals(value.Path, path, StringComparison.OrdinalIgnoreCase));
            if (item?.UpdateSourceVersion(version) == true && ReferenceEquals(item, current) && pipeline?.IsRunning != true) { pipeline?.RestoreImageResult(null); }
        }

        private void OnInspectionCompleted(object sender, EventArgs e)
        {
            if (disposed || current == null) { return; }
            current.ReplaceResult(pipeline.CaptureImageResult(pipeline.IsBatchRunning ? "batch" : "flow"));
        }

        internal void ClearRecipeResults()
        {
            foreach (OpenVisionWorkbenchImageItem item in images) { item.ReplaceResult(null); }
            hasBatchSummary = false; batchStopped = false; batchSummaryStale = false;
            batchPlanned = 0; batchIndex = 0; batchImageName = string.Empty; batchError = string.Empty;
            RaiseState();
        }

        private void OnPipelineChanged(object sender, PropertyChangedEventArgs e)
        {
            if (disposed) { return; }
            if (observedRecipeRevision != pipeline.RecipeRevision)
            {
                observedRecipeRevision = pipeline.RecipeRevision;
                if (hasBatchSummary) { batchSummaryStale = true; }
                foreach (OpenVisionWorkbenchImageItem item in images) { item.MarkResultStale(); }
                RaiseState();
            }
            OnPropertyChanged(nameof(SelectedBatchDisabledReason));
            OnPropertyChanged(nameof(BatchDisabledToolTip));
            OnPropertyChanged(nameof(BatchDisabledReason));
            OnPropertyChanged(nameof(CanEdit));
            OnPropertyChanged(nameof(IsBatchRunning));
            OnPropertyChanged(nameof(BatchProgressText));
            OnPropertyChanged(nameof(BatchProgressValue));
            OnPropertyChanged(nameof(BatchRunLabelText));
            OnPropertyChanged(nameof(BatchRunPositionText));
            OnPropertyChanged(nameof(HasBatchDisplay));
            OnPropertyChanged(nameof(HasBatchMessage));
            CommandManager.InvalidateRequerySuggested();
        }

        internal IReadOnlyList<OpenVisionRecipeValidationSetImage> CaptureValidationImages() => images.Where(item => item.HasLabel).Select(item => new OpenVisionRecipeValidationSetImage
        {
            Path = item.Path,
            Expected = item.Label == "good" ? OpenVisionRecipeValidationSetImage.ExpectedOk : OpenVisionRecipeValidationSetImage.ExpectedNg
        }).ToArray();

        internal bool TryLoadValidationSet(OpenVisionRecipeValidationSet set, out string error)
        {
            error = string.Empty;
            if (!CanEdit || set?.Images == null || set.Images.Count == 0) { error = T("Workbench.ValidationSet.Empty"); return false; }
            if (!OpenVisionRecipeValidationSetStorage.TryValidateImageContentIdentity(set, out error)) { return false; }
            var prepared = new List<OpenVisionWorkbenchImageItem>();
            foreach (OpenVisionRecipeValidationSetImage image in set.Images)
            {
                BitmapSource preview = OpenVisionBitmapImagePreviewFactory.TryCreateFromPath(image.Path, 136);
                if (preview == null)
                {
                    foreach (var item in prepared) { item.Dispose(); }
                    error = Format("Workbench.ValidationSet.ImageFailed", image.Path);
                    return false;
                }
                prepared.Add(new OpenVisionWorkbenchImageItem(image.Path, System.IO.Path.GetFileName(image.Path), preview) { Label = image.IsExpectedNg ? "bad" : "good" });
            }
            bool selectFirst = current == null;
            if (selectFirst)
            {
                loading = true;
                bool loaded;
                try { loaded = loadImage(prepared[0].Path); }
                catch (Exception exception) { error = exception.GetBaseException().Message; loaded = false; }
                finally { loading = false; }
                if (!loaded)
                {
                    foreach (var item in prepared) { item.Dispose(); }
                    if (string.IsNullOrEmpty(error)) { error = Format("Workbench.ValidationSet.ImageFailed", prepared[0].Path); }
                    return false;
                }
            }
            AddImages(prepared);
            foreach (var item in prepared)
            {
                OpenVisionWorkbenchImageItem existing = images.First(value => string.Equals(value.Path, item.Path, StringComparison.OrdinalIgnoreCase));
                existing.Label = item.Label;
                if (!ReferenceEquals(existing, item)) { item.Dispose(); }
            }
            if (selectFirst) { SetCurrent(images.First(value => string.Equals(value.Path, prepared[0].Path, StringComparison.OrdinalIgnoreCase))); }
            RaiseState();
            return true;
        }

        internal void NotifyImportStarted(int attempted)
        {
            if (CanEdit && attempted > 0) { ImportStarted?.Invoke(attempted); }
        }

        internal void NotifyImportCompleted(int added, int failed, int nestedExcluded)
        {
            if (CanEdit) { ImportCompleted?.Invoke(added, failed, nestedExcluded); }
        }

        internal VisionToolNImageVerificationSession NImageTrialSession => trialSession;

        internal void BeginNImageTrial()
        {
            ReleaseTrialEvidence();
            foreach (var image in images) { image.SetTrialResult(null); }
        }
        internal void PublishNImageTrial(VisionToolNImageVerificationRow row)
        {
            if (!disposed && row != null) { images.FirstOrDefault(image => string.Equals(image.Path, row.ImagePath, StringComparison.OrdinalIgnoreCase))?.SetTrialResult(row); }
        }
        internal bool RetainNImageTrial(VisionToolNImageVerificationSession value)
        {
            if (disposed || value == null) { return false; }
            trialSession = value;
            trialEvidenceDirectory = value.EvidenceDirectory;
            foreach (var row in value.Rows) { PublishNImageTrial(row); }
            return true;
        }
        private void ReleaseTrialEvidence()
        {
            string directory = trialEvidenceDirectory;
            trialEvidenceDirectory = string.Empty;
            trialSession = null;
            if (string.IsNullOrEmpty(directory)) { return; }
            try { if (System.IO.Directory.Exists(directory)) { System.IO.Directory.Delete(directory, true); } }
            catch (System.IO.IOException) { }
            catch (UnauthorizedAccessException) { }
        }

        internal int AddImages(IEnumerable<OpenVisionWorkbenchImageItem> added)
        {
            if (!CanEdit) { return 0; }
            int count = 0;
            foreach (OpenVisionWorkbenchImageItem item in added ?? Array.Empty<OpenVisionWorkbenchImageItem>())
            {
                if (item == null || images.Any(existing => string.Equals(existing.Path, item.Path, StringComparison.OrdinalIgnoreCase))) { continue; }
                item.PropertyChanged += OnItemChanged;
                images.Add(item);
                count++;
            }
            RefreshIndices();
            RaiseState();
            return count;
        }

        internal bool TrySelect(OpenVisionWorkbenchImageItem item, bool batch = false, bool reload = false)
        {
            if (disposed || (!batch && !CanEdit) || item == null || !images.Contains(item)) { return false; }
            if (!reload && ReferenceEquals(current, item)) { return true; }
            if (string.IsNullOrWhiteSpace(item.Path)) { return false; }
            loading = true;
            try
            {
                if (!(batch ? loadBatchImage : loadImage)(item.Path)) { return false; }
                item.UpdatePreview(shell.WorkspaceInputThumbnail);
                SetCurrent(item);
                return true;
            }
            finally { loading = false; }
        }

        private void SelectImage(object parameter)
        {
            Tuple<object, bool, bool> input = parameter as Tuple<object, bool, bool>;
            OpenVisionWorkbenchImageItem item = (input?.Item1 ?? parameter) as OpenVisionWorkbenchImageItem;
            if (item == null) { return; }
            if (input?.Item2 == true) { item.IsSelected = !item.IsSelected; return; }
            if (input?.Item3 == true && CurrentIndex >= 0)
            {
                int next = images.IndexOf(item);
                UpdateItems(() => { for (int i = Math.Min(CurrentIndex, next); i <= Math.Max(CurrentIndex, next); i++) { images[i].IsSelected = true; } });
                return;
            }
            if (TrySelect(item)) { SetSelection(false); }
        }

        private void RemoveSelected()
        {
            OpenVisionWorkbenchImageItem[] removed = images.Where(item => item.IsSelected).ToArray();
            if (removed.Length == 0) { return; }
            OpenVisionWorkbenchImageItem next = images.FirstOrDefault(item => !item.IsSelected);
            if (current?.IsSelected == true && next != null && !TrySelect(next)) { return; }
            foreach (OpenVisionWorkbenchImageItem item in removed) { item.PropertyChanged -= OnItemChanged; images.Remove(item); item.Dispose(); }
            if (images.Count == 0) { current = null; clearInput(); }
            RefreshIndices();
            RaiseState();
        }

        private void SetLabel(string label)
        {
            if (label != "" && label != "good" && label != "bad") { return; }
            IEnumerable<OpenVisionWorkbenchImageItem> targets = SelectedCount > 0 ? images.Where(item => item.IsSelected) : current == null ? Array.Empty<OpenVisionWorkbenchImageItem>() : new[] { current };
            UpdateItems(() => { foreach (OpenVisionWorkbenchImageItem item in targets) { item.Label = label; } });
        }

        private void SetSelection(bool selected)
        {
            UpdateItems(() => { foreach (OpenVisionWorkbenchImageItem item in images) { item.IsSelected = selected; } });
        }

        private void UpdateItems(Action update)
        {
            bool previous = updatingItems;
            updatingItems = true;
            try { update(); }
            finally { updatingItems = previous; if (!previous) { RaiseState(); } }
        }

        private static bool Matches(OpenVisionWorkbenchImageItem item, string value) => value == "all"
            || ((value == "before" || value == "none") && (item.Result == null || item.Result.IsStale))
            || (value == "ok" && item.FreshVerdict == "Ok") || (value == "ng" && item.FreshVerdict == "Ng")
            || (value == "err" && item.FreshVerdict == "Error") || (value == "over" && item.MismatchKind == "over")
            || (value == "under" && item.MismatchKind == "under") || (value == "good" && item.Label == "good") || (value == "bad" && item.Label == "bad");

        private void UpdateCurrentInput()
        {
            if (!shell.HasWorkspaceInputImage) { SetCurrent(null); return; }
            string path = shell.WorkspaceInputSourcePath;
            if (!string.IsNullOrWhiteSpace(path))
            {
                foreach (OpenVisionWorkbenchImageItem transient in images.Where(value => string.IsNullOrWhiteSpace(value.Path)).ToArray())
                {
                    transient.PropertyChanged -= OnItemChanged; images.Remove(transient); transient.Dispose();
                }
            }
            OpenVisionWorkbenchImageItem item = images.FirstOrDefault(value => string.Equals(value.Path, path, StringComparison.OrdinalIgnoreCase));
            if (item == null)
            {
                item = new OpenVisionWorkbenchImageItem(path, shell.WorkspaceInputName, shell.WorkspaceInputThumbnail);
                AddImages(new[] { item });
            }
            else { if (string.IsNullOrEmpty(path)) { item.MarkResultStale(); } item.UpdatePreview(shell.WorkspaceInputThumbnail); }
            RefreshIndices();
            SetCurrent(item);
        }

        private void SetCurrent(OpenVisionWorkbenchImageItem item)
        {
            current = item;
            foreach (OpenVisionWorkbenchImageItem image in images) { image.IsCurrent = ReferenceEquals(image, item); }
            if (pipeline?.IsRunning != true) { pipeline?.RestoreImageResult(item?.Result); }
            RaiseState();
        }

        private void RefreshIndices()
        {
            for (int i = 0; i < images.Count; i++) { images[i].Index = i + 1; }
        }

        private void RaiseState()
        {
            OpenVisionWorkbenchImageItem[] visible = images.Where(item => Matches(item, filter)).ToArray();
            if (!visibleImages.SequenceEqual(visible))
            {
                visibleImages.Clear();
                foreach (OpenVisionWorkbenchImageItem item in visible) { visibleImages.Add(item); }
            }
            foreach (string property in new[] { nameof(BatchDisabledToolTip), nameof(SelectedBatchDisabledReason), nameof(BatchDisabledReason), nameof(Count), nameof(BatchCountText), nameof(RunScopeImageText), nameof(SelectedCount), nameof(PositionText), nameof(CurrentImage), nameof(CurrentName), nameof(HeaderText), nameof(LabelMenuText), nameof(HasImages), nameof(HasVisibleImages), nameof(IsListMode), nameof(IsThumbnailMode), nameof(AllCountText), nameof(BeforeCountText), nameof(GoodCountText), nameof(BadCountText), nameof(HasGoodImages), nameof(HasBadImages), nameof(IsAllFilter), nameof(IsBeforeFilter), nameof(IsGoodFilter), nameof(IsBadFilter), nameof(CanEdit), nameof(IsBatchRunning), nameof(HasBeforeImages), nameof(HasOkImages), nameof(HasNgImages), nameof(HasErrorImages), nameof(HasOverImages), nameof(HasUnderImages), nameof(IsOkFilter), nameof(IsNgFilter), nameof(IsErrorFilter), nameof(IsOverFilter), nameof(IsUnderFilter), nameof(OkCountText), nameof(NgCountText), nameof(ErrorCountText), nameof(OverCountText), nameof(UnderCountText), nameof(HasBatchSummary), nameof(HasBatchDisplay), nameof(HasBatchMessage), nameof(IsBatchSummaryStale), nameof(BatchProgressText), nameof(BatchProgressValue), nameof(BatchRunLabelText), nameof(BatchRunPositionText), nameof(BatchSummaryText) }) { OnPropertyChanged(property); }
            CommandManager.InvalidateRequerySuggested();
        }

        private void OnInputChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(OpenVisionShellPreviewViewModel.HasWorkspaceInputImage) && !loading) { UpdateCurrentInput(); }
        }

        private void OnItemChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!updatingItems && (string.IsNullOrEmpty(e.PropertyName) || e.PropertyName == nameof(OpenVisionWorkbenchImageItem.Label) || e.PropertyName == nameof(OpenVisionWorkbenchImageItem.IsSelected))) { RaiseState(); }
        }

        private void OnLanguageChanged(object sender, EventArgs e)
        {
            foreach (OpenVisionWorkbenchImageItem item in images) { item.RefreshLanguage(); }
            OnPropertyChanged(string.Empty);
            RaiseState();
        }

        private static string T(string key) => OpenVisionLanguageService.T(key);
        private static string Format(string key, params object[] args) => string.Format(CultureInfo.CurrentCulture, T(key), args);
        private static string Ms(double value) => OpenVisionPipelineReviewResultPresenter.FormatWorkbenchElapsed(value);

        public void Dispose()
        {
            if (disposed) { return; }
            disposed = true;
            shell.PropertyChanged -= OnInputChanged;
            OpenVisionLanguageService.LanguageChanged -= OnLanguageChanged;
            ReleaseTrialEvidence();
            if (pipeline != null) { pipeline.InspectionCompleted -= OnInspectionCompleted; pipeline.PropertyChanged -= OnPipelineChanged; }
            foreach (OpenVisionWorkbenchImageItem item in images) { item.PropertyChanged -= OnItemChanged; item.Dispose(); }
            images.Clear();
            visibleImages.Clear();
            current = null;
        }
    }

    public sealed class OpenVisionWorkbenchImageItem : ObservableObject, IDisposable
    {
        private BitmapSource preview;
        private string label = "";
        private bool selected;
        private bool isCurrent;
        private int index;
        private string sourceVersion = "";
        internal OpenVisionWorkbenchImageResult Result { get; private set; }
        private VisionToolNImageVerificationRow trialResult;
        public string TrialText => trialResult == null ? string.Empty : string.Format(System.Globalization.CultureInfo.CurrentCulture,
            OpenVisionLanguageService.T("Workbench.Images.TrialFormat"), OpenVisionLanguageService.T("Workbench.Status." +
                (trialResult.Status == "ERROR" ? "Error" : trialResult.Status == "NG" ? "Ng" : trialResult.Status == "OK" ? "Ok" : "NoVerdict")));
        public string TrialDetail => trialResult?.ReviewDetailText ?? string.Empty;
        public string TrialMetricText => OpenVisionPipelineReviewResultPresenter.FormatWorkbenchSummary(trialResult?.TargetSummary);
        public string TrialElapsedText => trialResult == null ? string.Empty : OpenVisionPipelineReviewResultPresenter.FormatWorkbenchElapsed(trialResult.TotalMilliseconds);
        public string TrialFailedStepText => trialResult != null && (trialResult.IsError || trialResult.IsNg) ? trialResult.FailedStep : string.Empty;
        internal void SetTrialResult(VisionToolNImageVerificationRow row) { trialResult = row; OnPropertyChanged(nameof(TrialText)); OnPropertyChanged(nameof(TrialDetail)); OnPropertyChanged(nameof(TrialMetricText)); OnPropertyChanged(nameof(TrialElapsedText)); OnPropertyChanged(nameof(TrialFailedStepText)); }


        internal OpenVisionWorkbenchImageItem(string path, string name, BitmapSource preview)
        {
            Path = path ?? "";
            Name = name ?? "";
            this.preview = preview;
        }

        public string Path { get; }
        public string Name { get; }
        public BitmapSource Preview => preview;
        public string Label { get => label; internal set { if (SetProperty(ref label, value)) { OnPropertyChanged(nameof(LabelText)); OnPropertyChanged(nameof(HasLabel)); OnPropertyChanged(nameof(MismatchText)); OnPropertyChanged(nameof(MismatchKind)); } } }
        public string LabelText => label == "good" ? OpenVisionLanguageService.T("Workbench.Images.Good") : label == "bad" ? OpenVisionLanguageService.T("Workbench.Images.Bad") : "—";
        internal string FreshVerdict => Result?.IsStale == false ? Result.Verdict : "";
        public string ResultKind => Result == null ? "" : Result.IsStale ? "Previous" : Result.Verdict;
        public string ResultText => Result == null ? OpenVisionLanguageService.T("Workbench.Images.Before")
            : (Result.IsStale ? OpenVisionLanguageService.T("Workbench.Status.Previous") + " · " : "")
                + OpenVisionLanguageService.T(Result.Verdict == "Done" ? "Workbench.Status.NoVerdict" : "Workbench.Status." + Result.Verdict);
        public string ThumbnailResultToolTip => Result?.IsStale == true ? ResultText + " · " + OpenVisionLanguageService.T("Workbench.Batch.Stale") : ResultText;
        public string ThumbnailResultText => Result == null ? string.Empty
            : OpenVisionLanguageService.T(Result.Verdict == "Done" ? "Workbench.Status.NoVerdict" : "Workbench.Status." + Result.Verdict);
        public string MismatchKind => !HasLabel || (FreshVerdict != "Ok" && FreshVerdict != "Ng") || (label == "good" ? "Ok" : "Ng") == FreshVerdict ? "" : label == "good" ? "over" : "under";
        public string MismatchText => MismatchKind == "" ? "—" : OpenVisionLanguageService.T("Workbench.Batch." + (MismatchKind == "over" ? "Over" : "Under"));
        public string FailedStepText => FreshVerdict == "" ? "—" : string.IsNullOrEmpty(Result.FailedStep) ? "—" : Result.FailedStep;
        public string JudgmentText => FreshVerdict == "" || string.IsNullOrEmpty(Result.KeyValues) ? "—" : Result.KeyValues;
        public string ElapsedText => FreshVerdict == "" ? "—" : OpenVisionPipelineReviewResultPresenter.FormatWorkbenchElapsed(Result.ElapsedMilliseconds);
        public bool HasLabel => label.Length > 0;
        public bool HasFreshResult => Result?.IsStale == false;
        public bool IsSelected { get => selected; set => SetProperty(ref selected, value); }
        public bool IsCurrent { get => isCurrent; internal set => SetProperty(ref isCurrent, value); }
        public int Index { get => index; internal set => SetProperty(ref index, value); }
        internal void UpdatePreview(BitmapSource value) { preview = value; OnPropertyChanged(nameof(Preview)); }
        internal bool UpdateSourceVersion(string value)
        {
            bool changed = sourceVersion.Length > 0 && !string.Equals(sourceVersion, value, StringComparison.Ordinal);
            sourceVersion = value ?? "";
            if (changed) { MarkResultStale(); }
            return changed;
        }
        internal void ReplaceResult(OpenVisionWorkbenchImageResult value) { Result?.Dispose(); Result = value; OnPropertyChanged(string.Empty); }
        internal void MarkResultStale() { if (Result?.IsStale == false) { Result.IsStale = true; OnPropertyChanged(string.Empty); } }
        internal void RefreshLanguage() => OnPropertyChanged(string.Empty);
        public void Dispose() { Result?.Dispose(); Result = null; }
    }

}
