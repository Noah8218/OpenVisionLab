using OpenVisionLab.Vision2D.Pipeline;
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
        private readonly VisionToolLanguageChangeController languageChangeController;

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
            languageChangeController = VisionToolLanguageChangeController.Attach(RefreshLocalization);
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
            $"완료 {Rows.Count(row => row.IsCompleted):N0}장 · OK {Rows.Count(row => row.Status == "OK"):N0} · NG {Rows.Count(row => row.IsNg):N0} · 오류 {Rows.Count(row => row.IsError):N0} · 판정 없음 {Rows.Count(row => row.IsUngated):N0}",
            $"Completed {Rows.Count(row => row.IsCompleted):N0} · OK {Rows.Count(row => row.Status == "OK"):N0} · NG {Rows.Count(row => row.IsNg):N0} · Errors {Rows.Count(row => row.IsError):N0} · Ungated {Rows.Count(row => row.IsUngated):N0}");

        public string AddFilesText => LocalText("파일 추가", "Add files");
        public string AddFilesToolTipText => LocalText("검증할 이미지 파일을 선택합니다.", "Select image files to verify.");
        public string AddFolderText => LocalText("폴더 추가", "Add folder");
        public string AddFolderToolTipText => LocalText("한 폴더의 이미지 파일을 추가합니다.", "Add image files from one folder.");
        public string ClearImagesText => LocalText("목록 비우기", "Clear list");
        public string ClearImagesToolTipText => LocalText("선택한 이미지와 이전 실행 결과를 비웁니다.", "Clear selected images and retained results.");
        public string RunText => LocalText("순차 실행", "Run sequentially");
        public string RunToolTipText => LocalText("현재 설정을 고정해 선택 이미지를 순차 실행합니다.", "Freeze current settings and run the selected images sequentially.");
        public string StopText => LocalText("중지", "Stop");
        public string StopToolTipText => LocalText("현재 이미지 완료 후 중지합니다.", "Stop after the current image finishes.");
        public string ExportHtmlText => LocalText("HTML 내보내기", "Export HTML");
        public string ExportHtmlToolTipText => LocalText("저장된 현재 결과를 HTML 보고서로 내보냅니다.", "Export the retained results as an HTML report.");
        public string ImageListTitleText => LocalText("선택 이미지와 실행 결과", "Selected images and results");
        public string FileHeaderText => LocalText("파일", "File");
        public string StatusHeaderText => LocalText("판정", "Decision");
        public string MetricHeaderText => LocalText("측정값", "Metric");
        public string ReviewReasonHeaderText => LocalText("NG/오류 원인", "NG/error reason");
        public string SourceImageHeaderText => LocalText("검증 입력 이미지", "Verified source image");
        public string ResultImageHeaderText => LocalText("검증 결과 드로잉", "Verification result drawing");

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
                CheckFileExists = true,
                InitialDirectory = OpenVisionImageDirectoryResolver.ResolveOpenImageDirectory(null)
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
                Multiselect = false,
                InitialDirectory = OpenVisionImageDirectoryResolver.ResolveOpenImageDirectory(null)
            };
            if (dialog.ShowDialog(owner) != true)
            {
                return;
            }

            OpenVisionImageDirectoryResolver.RememberImagePath(dialog.FolderName);

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
                PopulatePendingRows();
                OpenVisionImageDirectoryResolver.RememberImagePath(ImagePaths.FirstOrDefault());
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
            PopulatePendingRows();
            session = null;
            IsRunning = true;
            StatusText = LocalText(
                "현재 설정을 고정하고 순차 검증을 시작합니다.",
                "Freezing current settings and starting sequential verification.");
            ProgressText = $"0 / {ImagePaths.Count:N0}";
            Progress<VisionToolNImageVerificationProgress> progress =
                new Progress<VisionToolNImageVerificationProgress>(item =>
                {
                    int rowIndex = item.CompletedCount - 1;
                    if (rowIndex >= 0 && rowIndex < Rows.Count)
                    {
                        Rows[rowIndex] = item.Row;
                    }
                    else
                    {
                        Rows.Add(item.Row);
                    }
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
                StatusText = session.HasAcceptance
                    ? LocalText(
                        $"순차 검증 {state}: {Rows.Count:N0}장. 현재 Tool View의 판정 기준을 적용했습니다.",
                        $"Sequential verification {state}: {Rows.Count:N0} images. The current Tool View acceptance gate was applied.")
                    : LocalText(
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
                int completedCount = Rows.Count(row => row.IsCompleted);
                ProgressText = completedCount == 0 ? string.Empty : $"{completedCount:N0} / {ImagePaths.Count:N0}";
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
                LocalText("위치검출 세트 승격", "Promote locator"),
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
                InitialDirectory = OpenVisionImageDirectoryResolver.ResolveOpenImageDirectory(null),
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
                    OpenVisionLanguageService.CurrentLanguage,
                    out string error))
            {
                OpenVisionImageDirectoryResolver.RememberImagePath(dialog.FileName);
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
            languageChangeController.Dispose();
        }

        private void ClearRetainedResults()
        {
            Rows.Clear();
            SelectedRow = null;
            session = null;
            ProgressText = string.Empty;
            NotifyResultCounts();
        }

        private void PopulatePendingRows()
        {
            Rows.Clear();
            int index = 0;
            foreach (string path in ImagePaths)
            {
                Rows.Add(new VisionToolNImageVerificationRow
                {
                    Index = ++index,
                    ImagePath = path,
                    Status = LocalText("대기", "READY"),
                    IsCompleted = false
                });
            }

            SelectedRow = Rows.FirstOrDefault();
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

            if (!SelectedRow.IsCompleted)
            {
                SelectedSourceImage = LoadBitmap(SelectedRow.ImagePath);
                SelectedEvidenceText = LocalText(
                    "실행 대기 · 순차 실행 후 판정과 드로잉이 표시됩니다.",
                    "Ready to run. The decision and drawing appear after sequential execution.");
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
            if (!string.IsNullOrWhiteSpace(SelectedRow.FailedStep))
            {
                parts.Add(LocalText("실패 Step: ", "Failed step: ") + SelectedRow.FailedStep);
            }
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

        private void RefreshLocalization()
        {
            for (int index = 0; index < Rows.Count; index++)
            {
                VisionToolNImageVerificationRow row = Rows[index];
                if (row.IsCompleted)
                {
                    continue;
                }

                Rows[index] = new VisionToolNImageVerificationRow
                {
                    Index = row.Index,
                    ImagePath = row.ImagePath,
                    Status = LocalText("대기", "READY"),
                    IsCompleted = false
                };
            }

            foreach (string propertyName in new[]
            {
                nameof(WindowTitle), nameof(ScopeText), nameof(SelectedCountText), nameof(ResultCountText),
                nameof(AddFilesText), nameof(AddFilesToolTipText), nameof(AddFolderText), nameof(AddFolderToolTipText),
                nameof(ClearImagesText), nameof(ClearImagesToolTipText), nameof(RunText), nameof(RunToolTipText),
                nameof(StopText), nameof(StopToolTipText), nameof(ExportHtmlText), nameof(ExportHtmlToolTipText),
                nameof(ImageListTitleText), nameof(FileHeaderText), nameof(StatusHeaderText), nameof(MetricHeaderText),
                nameof(ReviewReasonHeaderText), nameof(SourceImageHeaderText), nameof(ResultImageHeaderText),
                nameof(PromoteLocatorValidationText), nameof(PromoteLocatorValidationToolTipText),
                nameof(PromotionConfirmationText)
            })
            {
                OnPropertyChanged(propertyName);
            }

            if (SelectedRow != null)
            {
                LoadSelectedEvidence();
            }
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
