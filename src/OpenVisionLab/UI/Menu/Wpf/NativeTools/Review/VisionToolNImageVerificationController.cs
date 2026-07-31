using Lib.OpenCV.Pipeline;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using OpenVisionLab.Mvvm;

namespace OpenVisionLab
{
    internal sealed class VisionToolNImageVerificationController : INotifyPropertyChanged, IDisposable
    {
        private const string ImageFilter =
            "Image files (*.bmp;*.jpg;*.jpeg;*.png;*.tif;*.tiff)|*.bmp;*.jpg;*.jpeg;*.png;*.tif;*.tiff|All files (*.*)|*.*";

        private readonly string toolName;
        private readonly string recipeName;
        private readonly Func<VisionPipelineStep> createStep;
        private readonly bool normalizeInputToGray;
        private CancellationTokenSource cancellationSource;
        private VisionToolNImageVerificationSession session;
        private VisionToolNImageVerificationRow selectedRow;
        private BitmapImage selectedSourceImage;
        private BitmapImage selectedDrawingImage;
        private string statusText = string.Empty;
        private string progressText = string.Empty;
        private string selectedEvidenceText = string.Empty;
        private bool isRunning;
        private bool disposed;
        private VisionToolNImageValidationPromotionResult lastPromotionResult;
        private readonly RelayCommand<Window> addFilesCommand;
        private readonly RelayCommand<Window> addFolderCommand;
        private readonly RelayCommand<Window> clearImagesCommand;
        private readonly RelayCommand runCommand;
        private readonly RelayCommand stopCommand;
        private readonly RelayCommand<Window> exportHtmlCommand;
        private readonly RelayCommand<Window> promoteLocatorValidationCommand;

        public VisionToolNImageVerificationController(
            string toolName,
            string recipeName,
            Func<VisionPipelineStep> createStep,
            bool normalizeInputToGray)
        {
            this.toolName = string.IsNullOrWhiteSpace(toolName) ? "Tool" : toolName.Trim();
            this.recipeName = string.IsNullOrWhiteSpace(recipeName) ? "Default" : recipeName.Trim();
            this.createStep = createStep ?? throw new ArgumentNullException(nameof(createStep));
            this.normalizeInputToGray = normalizeInputToGray;
            addFilesCommand = new RelayCommand<Window>(AddFiles, _ => CanEditImages);
            addFolderCommand = new RelayCommand<Window>(AddFolder, _ => CanEditImages);
            clearImagesCommand = new RelayCommand<Window>(_ => ClearImages(), _ => CanEditImages);
            runCommand = new RelayCommand(Run, () => CanRun);
            stopCommand = new RelayCommand(Stop, () => CanStop);
            exportHtmlCommand = new RelayCommand<Window>(ExportHtml, _ => CanExport);
            promoteLocatorValidationCommand = new RelayCommand<Window>(
                PromptAndPromoteLocatorValidation,
                _ => CanPromoteLocatorValidation);
            StatusText = LocalText(
                "이미지 파일 또는 한 폴더를 선택한 뒤 실행하십시오.",
                "Select image files or one folder, then run.");
            RefreshState();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<string> ImagePaths { get; } = new ObservableCollection<string>();
        public ObservableCollection<VisionToolNImageVerificationRow> Rows { get; } =
            new ObservableCollection<VisionToolNImageVerificationRow>();

        public ICommand AddFilesCommand => addFilesCommand;

        public ICommand AddFolderCommand => addFolderCommand;

        public ICommand ClearImagesCommand => clearImagesCommand;

        public ICommand RunCommand => runCommand;

        public ICommand StopCommand => stopCommand;

        public ICommand ExportHtmlCommand => exportHtmlCommand;

        public ICommand PromoteLocatorValidationCommand => promoteLocatorValidationCommand;

        public string WindowTitle => LocalText(
            toolName + " · N장 검증",
            toolName + " · N-image verification");

        public string ScopeText => LocalText(
            "현재 Tool View 설정을 한 Step으로 고정해 순차 실행합니다. 레이어·라우팅·Preview/Run 상태는 바꾸지 않습니다.",
            "Freeze the current Tool View settings as one Step and run sequentially. Layers, routing, and Preview/Run state remain unchanged.");

        public string SelectedCountText => LocalText(
            $"선택 {ImagePaths.Count:N0}장",
            $"{ImagePaths.Count:N0} selected");

        public string ResultCountText => LocalText(
            $"결과 {Rows.Count:N0}장 · OK {Rows.Count(row => row.Success):N0} · NG/오류 {Rows.Count(row => !row.Success):N0}",
            $"Results {Rows.Count:N0} · OK {Rows.Count(row => row.Success):N0} · NG/error {Rows.Count(row => !row.Success):N0}");

        public string StatusText
        {
            get => statusText;
            private set => SetProperty(ref statusText, value);
        }

        public string ProgressText
        {
            get => progressText;
            private set => SetProperty(ref progressText, value);
        }

        public string SelectedEvidenceText
        {
            get => selectedEvidenceText;
            private set => SetProperty(ref selectedEvidenceText, value);
        }

        public bool IsRunning
        {
            get => isRunning;
            private set
            {
                if (SetProperty(ref isRunning, value))
                {
                    RefreshState();
                }
            }
        }

        public bool CanEditImages => !IsRunning;
        public bool CanRun => !IsRunning && ImagePaths.Count > 0;
        public bool CanStop => IsRunning;
        public bool CanExport => !IsRunning
            && session != null
            && !string.IsNullOrWhiteSpace(session.BatchSummaryPath)
            && File.Exists(session.BatchSummaryPath);
        public bool IsLocatorPromotionSupported => IsLocatorToolName(toolName);
        public bool CanPromoteLocatorValidation => IsLocatorPromotionSupported
            && !IsRunning
            && session != null
            && !session.WasCancelled
            && session.Rows.Count > 0
            && session.Rows.All(row => row.Success);
        public string PromoteLocatorValidationText => LocalText(
            "위치검출 세트 승격",
            "Promote locator set");
        public string PromoteLocatorValidationToolTipText => LocalText(
            "현재 동결 Pipeline과 모든 이미지를 Expected OK locator Validation Set으로 저장합니다. 실행은 시작하지 않습니다.",
            "Save the frozen Pipeline and every image as an Expected OK locator Validation Set. This does not start a run.");
        public string PromotionConfirmationText => LocalText(
            $"Recipe '{recipeName}'에 동결 Pipeline과 {Rows.Count:N0}개 이미지를 해시 잠금 위치검출 Validation Set으로 저장합니다. 모든 이미지는 locator 기대 성공(OK)으로 등록되며 Preview/Run은 시작하지 않습니다. 계속하시겠습니까?",
            $"Save the frozen Pipeline and {Rows.Count:N0} images into Recipe '{recipeName}' as a hash-locked locator Validation Set. Every image will be Expected OK for locator execution, and Preview/Run will not start. Continue?");
        internal VisionToolNImageValidationPromotionResult LastPromotionResult => lastPromotionResult;

        public VisionToolNImageVerificationRow SelectedRow
        {
            get => selectedRow;
            set
            {
                if (SetProperty(ref selectedRow, value))
                {
                    LoadSelectedEvidence();
                }
            }
        }

        public BitmapImage SelectedSourceImage
        {
            get => selectedSourceImage;
            private set => SetProperty(ref selectedSourceImage, value);
        }

        public BitmapImage SelectedDrawingImage
        {
            get => selectedDrawingImage;
            private set => SetProperty(ref selectedDrawingImage, value);
        }

        private void AddFiles(Window owner)
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Title = LocalText("N장 검증 이미지 추가", "Add N-image verification files"),
                Filter = ImageFilter,
                Multiselect = true,
                CheckFileExists = true
            };
            if (dialog.ShowDialog(owner) == true)
            {
                AddImagePaths(dialog.FileNames);
            }
        }

        private void AddFolder(Window owner)
        {
            OpenFolderDialog dialog = new OpenFolderDialog
            {
                Title = LocalText("N장 검증 폴더 추가", "Add one N-image verification folder"),
                Multiselect = false
            };
            if (dialog.ShowDialog(owner) != true)
            {
                return;
            }

            if (!OpenVisionRecipeValidationSetStorage.TryGetTopLevelImagePaths(
                    dialog.FolderName,
                    out IReadOnlyList<string> paths,
                    out string error))
            {
                StatusText = LocalText("폴더를 읽지 못했습니다: ", "Could not read folder: ") + error;
                return;
            }

            AddImagePaths(paths);
        }

        public void AddImagePaths(IEnumerable<string> paths)
        {
            try
            {
                List<string> combined = VisionToolNImageVerificationService.NormalizeImagePaths(
                    ImagePaths.Concat(paths ?? Enumerable.Empty<string>()));
                ImagePaths.Clear();
                foreach (string path in combined)
                {
                    ImagePaths.Add(path);
                }

                ClearRetainedResults();
                StatusText = LocalText(
                    $"검증 이미지 {ImagePaths.Count:N0}장을 준비했습니다.",
                    $"Prepared {ImagePaths.Count:N0} verification images.");
            }
            catch (Exception ex)
            {
                StatusText = ex.GetBaseException().Message;
            }

            RefreshState();
        }

        private void ClearImages()
        {
            if (IsRunning)
            {
                return;
            }

            ImagePaths.Clear();
            ClearRetainedResults();
            StatusText = LocalText("이미지 목록을 비웠습니다.", "Cleared the image list.");
            RefreshState();
        }

        public async Task RunAsync()
        {
            if (!CanRun)
            {
                return;
            }

            cancellationSource?.Dispose();
            cancellationSource = new CancellationTokenSource();
            Rows.Clear();
            SelectedRow = null;
            session = null;
            IsRunning = true;
            StatusText = LocalText(
                "현재 설정을 고정하고 순차 검증을 시작합니다.",
                "Freezing current settings and starting sequential verification.");
            ProgressText = $"0 / {ImagePaths.Count:N0}";
            Progress<VisionToolNImageVerificationProgress> progress =
                new Progress<VisionToolNImageVerificationProgress>(item =>
                {
                    Rows.Add(item.Row);
                    ProgressText = $"{item.CompletedCount:N0} / {item.TotalCount:N0}";
                    NotifyResultCounts();
                });

            try
            {
                session = await VisionToolNImageVerificationService.RunAsync(
                    toolName,
                    recipeName,
                    createStep,
                    normalizeInputToGray,
                    ImagePaths.ToList(),
                    progress,
                    cancellationSource.Token);
                Rows.Clear();
                foreach (VisionToolNImageVerificationRow row in session.Rows)
                {
                    Rows.Add(row);
                }

                SelectedRow = Rows.FirstOrDefault(row => !row.Success)
                    ?? Rows.FirstOrDefault(row => !string.IsNullOrWhiteSpace(row.ReviewReasonText))
                    ?? Rows.FirstOrDefault();
                string state = session.WasCancelled
                    ? LocalText("중지된 부분 결과", "stopped partial result")
                    : LocalText("완료", "completed");
                StatusText = LocalText(
                    $"순차 검증 {state}: {Rows.Count:N0}장. 판정 기준은 자동으로 추정하지 않았습니다.",
                    $"Sequential verification {state}: {Rows.Count:N0} images. No acceptance gate was inferred.");
            }
            catch (Exception ex)
            {
                StatusText = LocalText("검증 실패: ", "Verification failed: ") + ex.GetBaseException().Message;
            }
            finally
            {
                IsRunning = false;
                ProgressText = Rows.Count == 0 ? string.Empty : $"{Rows.Count:N0} / {ImagePaths.Count:N0}";
                NotifyResultCounts();
            }
        }

        public void Stop()
        {
            if (!IsRunning)
            {
                return;
            }

            cancellationSource?.Cancel();
            StatusText = LocalText(
                "중지 요청을 받았습니다. 현재 이미지가 끝난 뒤 부분 결과를 저장합니다.",
                "Stop requested. Partial results will be saved after the current image finishes.");
        }

        private async void Run()
        {
            await RunAsync();
            RefreshState();
        }

        private void PromptAndPromoteLocatorValidation(Window owner)
        {
            if (!CanPromoteLocatorValidation)
            {
                return;
            }

            MessageBoxResult result = MessageBox.Show(
                owner ?? Application.Current?.MainWindow,
                PromotionConfirmationText,
                LocalText("locator ?밴꺽", "Promote locator"),
                MessageBoxButton.YesNo,
                MessageBoxImage.Question,
                MessageBoxResult.No);
            if (result != MessageBoxResult.Yes)
            {
                return;
            }

            PromoteLocatorValidation();
        }

        private void ExportHtml(Window owner)
        {
            if (!CanExport)
            {
                return;
            }

            SaveFileDialog dialog = new SaveFileDialog
            {
                Title = LocalText("N장 검증 HTML 보고서 저장", "Save N-image verification HTML report"),
                Filter = "HTML report (*.html)|*.html",
                AddExtension = true,
                DefaultExt = ".html",
                FileName =
                    $"OpenVisionLab_{SanitizeFileName(toolName)}_NImage_{DateTime.Now:yyyyMMdd_HHmmss}.html"
            };
            if (dialog.ShowDialog(owner) != true)
            {
                return;
            }

            if (VisionToolNImageVerificationHtmlReportExporter.TryExport(
                    session.BatchSummaryPath,
                    session.PipelineXml,
                    session.StepDefinitionSha256,
                    dialog.FileName,
                    out string error))
            {
                StatusText = LocalText("HTML 보고서를 저장했습니다: ", "Saved HTML report: ") + dialog.FileName;
            }
            else
            {
                StatusText = LocalText("HTML 보고서를 저장하지 못했습니다: ", "Could not save HTML report: ") + error;
            }
        }

        public bool PromoteLocatorValidation()
        {
            if (!CanPromoteLocatorValidation)
            {
                return false;
            }

            if (!VisionToolNImageValidationPromotionService.TryPromoteLocatorExpectedSuccess(
                    recipeName,
                    session,
                    out VisionToolNImageValidationPromotionResult promotion,
                    out string error))
            {
                StatusText = LocalText(
                    "Recipe Manager 승격 실패: ",
                    "Recipe Manager promotion failed: ") + error;
                return false;
            }

            lastPromotionResult = promotion;
            StatusText = string.Format(
                System.Globalization.CultureInfo.CurrentCulture,
                LocalText(
                    "승격 완료: Recipe={0}, Pipeline={1}, Set={2}, locator 기대 성공 {3}장. Preview/Run은 실행하지 않았습니다.",
                    "Promotion complete: Recipe={0}, Pipeline={1}, Set={2}, {3} locator expected-success images. Preview/Run was not started."),
                promotion.RecipeName,
                promotion.PipelineName,
                promotion.ValidationSetName,
                promotion.ImageCount);
            RefreshState();
            return true;
        }

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            disposed = true;
            cancellationSource?.Cancel();
            cancellationSource?.Dispose();
            cancellationSource = null;
        }

        private void ClearRetainedResults()
        {
            Rows.Clear();
            SelectedRow = null;
            session = null;
            ProgressText = string.Empty;
            NotifyResultCounts();
        }

        private void LoadSelectedEvidence()
        {
            SelectedSourceImage = null;
            SelectedDrawingImage = null;
            SelectedEvidenceText = string.Empty;
            if (SelectedRow == null)
            {
                return;
            }

            bool sourceVerified = VisionPipelineRunReportStorage.IsFileSha256Match(
                SelectedRow.SourceSnapshotPath,
                SelectedRow.SourceSha256);
            if (sourceVerified)
            {
                SelectedSourceImage = LoadBitmap(SelectedRow.SourceSnapshotPath);
            }

            SelectedDrawingImage = LoadBitmap(SelectedRow.DrawingPath);
            List<string> parts = new List<string>
            {
                $"#{SelectedRow.Index:0000} {SelectedRow.FileName}",
                SelectedRow.Status + $" · {SelectedRow.TotalMilliseconds:0.###} ms",
                SelectedRow.MetricText,
                "Source SHA-256: " + (sourceVerified ? SelectedRow.SourceSha256 : "EVIDENCE MISMATCH")
            };
            if (!string.IsNullOrWhiteSpace(SelectedRow.ReviewReasonText))
            {
                parts.Add(LocalText("검토 이유: ", "Review reason: ") + SelectedRow.ReviewReasonText);
            }

            if (!string.IsNullOrWhiteSpace(SelectedRow.Message))
            {
                parts.Add(LocalText("메시지: ", "Message: ") + SelectedRow.Message);
            }

            SelectedEvidenceText = string.Join(Environment.NewLine, parts);
        }

        private static BitmapImage LoadBitmap(string path)
        {
            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
            {
                return null;
            }

            try
            {
                using FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = stream;
                image.EndInit();
                image.Freeze();
                return image;
            }
            catch
            {
                return null;
            }
        }

        private void RefreshState()
        {
            addFilesCommand?.RaiseCanExecuteChanged();
            addFolderCommand?.RaiseCanExecuteChanged();
            clearImagesCommand?.RaiseCanExecuteChanged();
            runCommand?.RaiseCanExecuteChanged();
            stopCommand?.RaiseCanExecuteChanged();
            exportHtmlCommand?.RaiseCanExecuteChanged();
            promoteLocatorValidationCommand?.RaiseCanExecuteChanged();
            OnPropertyChanged(nameof(SelectedCountText));
            OnPropertyChanged(nameof(CanEditImages));
            OnPropertyChanged(nameof(CanRun));
            OnPropertyChanged(nameof(CanStop));
            OnPropertyChanged(nameof(CanExport));
            OnPropertyChanged(nameof(IsLocatorPromotionSupported));
            OnPropertyChanged(nameof(CanPromoteLocatorValidation));
            OnPropertyChanged(nameof(PromotionConfirmationText));
        }

        private void NotifyResultCounts()
        {
            OnPropertyChanged(nameof(ResultCountText));
            OnPropertyChanged(nameof(CanExport));
            OnPropertyChanged(nameof(CanPromoteLocatorValidation));
            OnPropertyChanged(nameof(PromotionConfirmationText));
        }

        private bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
            {
                return false;
            }

            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private static string LocalText(string korean, string english)
        {
            return OpenVisionLanguageService.CurrentLanguage == OpenVisionLanguage.Korean
                ? korean
                : english;
        }

        private static string SanitizeFileName(string value)
        {
            char[] invalid = Path.GetInvalidFileNameChars();
            string sanitized = new string((value ?? string.Empty)
                .Select(character => invalid.Contains(character) || char.IsWhiteSpace(character) ? '_' : character)
                .ToArray());
            return string.IsNullOrWhiteSpace(sanitized) ? "Tool" : sanitized;
        }

        private static bool IsLocatorToolName(string value)
        {
            return string.Equals(value, "Matching", StringComparison.OrdinalIgnoreCase)
                || string.Equals(value, "EdgeBasedMatching", StringComparison.OrdinalIgnoreCase)
                || string.Equals(value, "FeatureMatching", StringComparison.OrdinalIgnoreCase);
        }
    }
}
