using Microsoft.Win32;
using OpenVisionLab.Vision2D.Pipeline;
using OpenVisionLab.Logging;
using OpenVisionLab.Logging.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
namespace OpenVisionLab
{
    internal sealed class OpenVisionShellHostCommandController
    {
        private readonly Func<Window> ownerProvider;
        private readonly Func<string, bool> loadWorkspaceImage;
        private readonly OpenVisionShellHostWorkspacePreviewController workspacePreviewController;
        private readonly OpenVisionZoomableImageController workspaceFallbackZoomController;
        private readonly IOpenVisionDockedLayerWorkspaceCommands dockedLayerWorkspace;
        private readonly Func<string> workspaceLayerTitleProvider;
        private readonly Func<OpenVisionRecipeContext> recipeContextProvider;
        private readonly Func<string, string, bool> prepareSampleWorkspaceContext;
        private readonly Action<string, string> sampleWorkspaceLoaded;
        private readonly Action manualWorkspaceImageLoaded;
        private readonly Action<string> rememberWorkspaceImagePath;
        private readonly bool hasRunnableSample;
        private bool importingWorkspaceImages;
        private string lastWorkspaceImageDirectory;
        private OpenVisionWorkbenchImagesViewModel workspaceImages;
        public OpenVisionShellHostCommandController(
            Func<Window> ownerProvider,
            Func<string, bool> loadWorkspaceImage,
            OpenVisionShellHostWorkspacePreviewController workspacePreviewController,
            OpenVisionZoomableImageController workspaceFallbackZoomController,
            IOpenVisionDockedLayerWorkspaceCommands dockedLayerWorkspace,
            Func<string> workspaceLayerTitleProvider,
            Func<OpenVisionRecipeContext> recipeContextProvider,
            Func<string, string, bool> prepareSampleWorkspaceContext = null,
            Action<string, string> sampleWorkspaceLoaded = null,
            Action manualWorkspaceImageLoaded = null,
            string initialWorkspaceImagePath = null,
            Action<string> rememberWorkspaceImagePath = null)
        {
            this.ownerProvider = ownerProvider ?? throw new ArgumentNullException(nameof(ownerProvider));
            this.loadWorkspaceImage = loadWorkspaceImage ?? throw new ArgumentNullException(nameof(loadWorkspaceImage));
            this.workspacePreviewController = workspacePreviewController ?? throw new ArgumentNullException(nameof(workspacePreviewController));
            this.workspaceFallbackZoomController = workspaceFallbackZoomController ?? throw new ArgumentNullException(nameof(workspaceFallbackZoomController));
            this.dockedLayerWorkspace = dockedLayerWorkspace ?? throw new ArgumentNullException(nameof(dockedLayerWorkspace));
            this.workspaceLayerTitleProvider = workspaceLayerTitleProvider ?? throw new ArgumentNullException(nameof(workspaceLayerTitleProvider));
            this.recipeContextProvider = recipeContextProvider ?? throw new ArgumentNullException(nameof(recipeContextProvider));
            this.prepareSampleWorkspaceContext = prepareSampleWorkspaceContext;
            this.sampleWorkspaceLoaded = sampleWorkspaceLoaded;
            this.manualWorkspaceImageLoaded = manualWorkspaceImageLoaded;
            this.rememberWorkspaceImagePath = rememberWorkspaceImagePath;
            lastWorkspaceImageDirectory = ResolveExistingImageDirectory(initialWorkspaceImagePath);
            OpenVisionImageDirectoryResolver.RememberImagePath(initialWorkspaceImagePath);
            hasRunnableSample = LoadRunnableSamples().Count > 0;
        }

        public void PromptAndLoadWorkspaceImage() => PromptAndLoadWorkspaceImage("");

        public void PromptAndLoadWorkspaceImage(string label)
        {
            if (importingWorkspaceImages || workspaceImages?.CanEdit == false) { return; }
            OpenFileDialog dialog = new OpenFileDialog
            {
                Title = OpenVisionLanguageService.T("Shell.WorkspaceLoadImage"),
                Filter = "Image files|*.bmp;*.png;*.jpg;*.jpeg;*.tif;*.tiff|Bitmap (*.bmp)|*.bmp|PNG (*.png)|*.png|JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|TIFF (*.tif;*.tiff)|*.tif;*.tiff|All files|*.*",
                Multiselect = workspaceImages != null,
                InitialDirectory = ResolveWorkspaceImageDirectory()
            };

            if (dialog.ShowDialog(ownerProvider()) != true)
            {
                return;
            }

            if (workspaceImages != null) { AddWorkspaceImages(dialog.FileNames, label ?? ""); return; }

            if (!loadWorkspaceImage(dialog.FileName))
            {
                ShowWorkspaceImageLoadFailedMessage(dialog.FileName);
                return;
            }

            RecordWorkspaceImagePath(dialog.FileName);
            manualWorkspaceImageLoaded?.Invoke();
        }

        internal void AttachWorkspaceImages(OpenVisionWorkbenchImagesViewModel images) => workspaceImages = images;
        internal void NotifyRestoredWorkspaceInput(string path) { RecordWorkspaceImagePath(path); manualWorkspaceImageLoaded?.Invoke(); }

        internal bool TryLoadListedImage(string path) => workspaceImages?.CanEdit != false && LoadListedImage(path, showFailure: true);
        internal bool TryLoadBatchImage(string path) => LoadListedImage(path, showFailure: false);

        private bool LoadListedImage(string path, bool showFailure)
        {
            if (!loadWorkspaceImage(path)) { if (showFailure) { ShowWorkspaceImageLoadFailedMessage(path); } return false; }
            RecordWorkspaceImagePath(path);
            try
            {
                using FileStream file = File.OpenRead(path);
                workspaceImages?.RecordSourceVersion(path, Convert.ToHexString(SHA256.HashData(file)));
            }
            catch (IOException) { workspaceImages?.RecordSourceVersion(path, ""); }
            catch (UnauthorizedAccessException) { workspaceImages?.RecordSourceVersion(path, ""); }
            manualWorkspaceImageLoaded?.Invoke();
            return true;
        }

        internal int AddWorkspaceImages(IEnumerable<string> paths, string label = "", int nestedExcludedCount = 0, bool sampleImages = false, IReadOnlyDictionary<string, string> imageLabels = null, Action<string> reportFailure = null)
        {
            if (importingWorkspaceImages || workspaceImages?.CanEdit == false) { return 0; }
            string[] submittedPaths = paths?.ToArray() ?? Array.Empty<string>();
            if (submittedPaths.Length == 0) { return 0; }
            importingWorkspaceImages = true;
            OpenVisionWorkbenchImagesViewModel images = workspaceImages;
            try
            {
                Window owner = ownerProvider();
                OpenVisionRecipeContext context = recipeContextProvider();
                if (images != null) { images.IsSampleImport = sampleImages; }
                images?.NotifyImportStarted(submittedPaths.Length);
                bool ownerWasLoaded = owner?.IsLoaded == true;
                var dispatcher = owner?.Dispatcher ?? System.Windows.Threading.Dispatcher.CurrentDispatcher;
                List<(string Path, string Name, System.Windows.Media.Imaging.BitmapSource Preview)> prepared = new();
                List<string> failed = new();
                Task preparation = Task.Run(() =>
                {
                    foreach (string path in submittedPaths)
                    {
                        try
                        {
                            string fullPath = Path.GetFullPath(path);
                            using System.Drawing.Bitmap decoded = new System.Drawing.Bitmap(fullPath);
                            var preview = OpenVisionBitmapImagePreviewFactory.TryCreateFromPath(fullPath, 160)
                                ?? OpenVisionBitmapImagePreviewFactory.Create(decoded, 160);
                            if (preview == null) { throw new InvalidDataException("Image preview missing."); }
                            prepared.Add((fullPath, Path.GetFileName(fullPath), preview));
                        }
                        catch (Exception ex)
                        {
                            failed.Add(path);
                            OVLog.Write(LogCategory.Main, LogLevel.Error, "Workspace image list load failed.", path, ex);
                        }
                    }
                });
                var frame = new System.Windows.Threading.DispatcherFrame();
                Task completion = preparation.ContinueWith(_ => dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, new Action(() => frame.Continue = false)), TaskScheduler.Default);
                try { System.Windows.Threading.Dispatcher.PushFrame(frame); }
                finally
                {
                    try { preparation.GetAwaiter().GetResult(); }
                    finally { completion.GetAwaiter().GetResult(); }
                }
                OpenVisionRecipeContext currentContext = recipeContextProvider();
                if (!ReferenceEquals(images, workspaceImages) || images?.CanEdit == false || !ReferenceEquals(owner, ownerProvider())
                    || dispatcher.HasShutdownStarted || (ownerWasLoaded && !owner.IsLoaded)
                    || context?.Name != currentContext?.Name || context?.PipelineName != currentContext?.PipelineName) { return 0; }
                List<OpenVisionWorkbenchImageItem> added = prepared.Select(image => new OpenVisionWorkbenchImageItem(image.Path, image.Name, image.Preview) { Label = imageLabels != null && imageLabels.TryGetValue(image.Path, out string imageLabel) ? imageLabel : label }).ToList();
                int count = workspaceImages?.AddImages(added) ?? 0;
                if (added.Count > 0 && (workspaceImages == null || workspaceImages.CurrentIndex < 0)) { TryLoadListedImage(added[0].Path); }
                if (failed.Count > 0) { (reportFailure ?? ShowWorkspaceImageLoadFailedMessage)(string.Join(Environment.NewLine, failed)); }
                if (added.Count + failed.Count > 0) { workspaceImages?.NotifyImportCompleted(count, failed.Count, nestedExcludedCount); }
                return count;
            }
            finally { if (images != null) { images.IsSampleImport = false; } importingWorkspaceImages = false; }
        }

        public void PromptAndOpenWorkspaceFolder(string label)
        {
            if (importingWorkspaceImages || workspaceImages?.CanEdit == false) { return; }
            OpenFolderDialog dialog = new OpenFolderDialog
            {
                Title = OpenVisionLanguageService.T("Workbench.Viewer.OpenFolder"),
                InitialDirectory = ResolveWorkspaceImageDirectory(),
                Multiselect = false
            };
            Window owner = ownerProvider();
            bool ownerWasLoaded = owner?.IsLoaded == true;
            if (dialog.ShowDialog(owner) != true) { return; }
            try
            {
                bool IsImagePath(string path) => new[] { ".bmp", ".png", ".jpg", ".jpeg", ".tif", ".tiff" }.Contains(Path.GetExtension(path), StringComparer.OrdinalIgnoreCase);
                string[] top = Directory.EnumerateFiles(dialog.FolderName).Where(IsImagePath).OrderBy(path => path, StringComparer.OrdinalIgnoreCase).ToArray();
                var options = new EnumerationOptions { RecurseSubdirectories = true, IgnoreInaccessible = true, AttributesToSkip = FileAttributes.ReparsePoint };
                int nested = Directory.EnumerateFiles(dialog.FolderName, "*", options).Count(path => IsImagePath(path) && !string.Equals(Path.GetDirectoryName(path), dialog.FolderName, StringComparison.OrdinalIgnoreCase));
                OpenVisionWorkbenchImagesViewModel images = workspaceImages;
                OpenVisionRecipeContext context = recipeContextProvider();
                AddWorkspaceImages(top, label ?? "", nested);
                OpenVisionRecipeContext currentContext = recipeContextProvider();
                if (!ReferenceEquals(images, workspaceImages) || images?.CanEdit == false || !ReferenceEquals(owner, ownerProvider())
                    || owner?.Dispatcher.HasShutdownStarted == true || (ownerWasLoaded && !owner.IsLoaded)
                    || context?.Name != currentContext?.Name || context?.PipelineName != currentContext?.PipelineName) { return; }
                if (nested > 0)
                {
                    string text = string.Format(CultureInfo.CurrentCulture, OpenVisionLanguageService.T("Workbench.Images.NestedIgnored"), nested);
                    OVLog.Write(LogCategory.Main, LogLevel.Info, text);
                    MessageBox.Show(ownerProvider(), text, "OpenVisionLab", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                OVLog.Write(LogCategory.Main, LogLevel.Error, "Workspace image folder load failed.", dialog.FolderName, ex);
                if (!ownerWasLoaded || owner?.IsLoaded == true) { ShowWorkspaceImageLoadFailedMessage(dialog.FolderName); }
            }
        }

        public void FitWorkspaceImage()
        {
            workspaceFallbackZoomController.Reset();
            workspacePreviewController.FitImageToView();
            dockedLayerWorkspace.FitLayerImages();
        }

        internal IOpenVisionDockedLayerViewer ActiveLayerViewer => dockedLayerWorkspace.ActiveLayerViewer;

        public void ZoomWorkspaceImage(double factor)
        {
            IOpenVisionDockedLayerViewer viewer = ActiveLayerViewer;
            if (viewer != null) { viewer.ZoomImage(factor); }
            else { workspaceFallbackZoomController.ZoomImage(factor); }
        }

        public void ShowWorkspaceOriginalSize()
        {
            IOpenVisionDockedLayerViewer viewer = ActiveLayerViewer;
            if (viewer != null) { viewer.ShowOriginalSize(); }
            else { workspaceFallbackZoomController.ShowOriginalSize(); }
        }

        public void PromptAndSaveWorkspaceImage()
        {
            if (!workspacePreviewController.HasImage)
            {
                return;
            }

            SaveFileDialog dialog = new SaveFileDialog
            {
                Title = OpenVisionLanguageService.T("ToolView.SaveImage"),
                Filter = "PNG (*.png)|*.png|Bitmap (*.bmp)|*.bmp|JPEG (*.jpg)|*.jpg|TIFF (*.tif)|*.tif",
                FileName = OpenVisionPreviewImageFileService.CreateDefaultImageFileName(workspaceLayerTitleProvider()),
                InitialDirectory = OpenVisionPreviewImageFileService.ResolveOpenImageDirectory(null),
                AddExtension = true,
                DefaultExt = ".png"
            };

            if (dialog.ShowDialog(ownerProvider()) == true)
            {
                workspacePreviewController.SaveCurrentImage(dialog.FileName);
                OpenVisionImageDirectoryResolver.RememberImagePath(dialog.FileName);
            }
        }

        public void PromptAndSaveDisplayImage(string name, System.Drawing.Bitmap image)
        {
            if (image == null) { return; }
            SaveFileDialog dialog = new SaveFileDialog
            {
                Title = OpenVisionLanguageService.T("ToolView.SaveImage"),
                Filter = "PNG (*.png)|*.png|Bitmap (*.bmp)|*.bmp|JPEG (*.jpg)|*.jpg|TIFF (*.tif)|*.tif",
                FileName = OpenVisionPreviewImageFileService.CreateDefaultImageFileName(name),
                AddExtension = true, DefaultExt = ".png"
            };
            if (dialog.ShowDialog(ownerProvider()) != true) { return; }
            if (TrySaveDisplayImage(image, dialog.FileName)) { OpenVisionImageDirectoryResolver.RememberImagePath(dialog.FileName); }
            else { MessageBox.Show(ownerProvider(), OpenVisionLanguageService.T("Workbench.Pane.SaveFailed"), "OpenVisionLab", MessageBoxButton.OK, MessageBoxImage.Warning); }
        }

        internal static bool TrySaveDisplayImage(System.Drawing.Bitmap image, string path)
        {
            if (image == null || string.IsNullOrWhiteSpace(path)) { return false; }
            try
            {
                using System.Drawing.Bitmap savable = OpenVisionPreviewImageFileService.CreateSavableBitmap(image);
                savable.Save(path, OpenVisionPreviewImageFileService.ResolveImageFormat(path));
                return true;
            }
            catch (Exception ex)
            {
                OVLog.Write(LogCategory.Main, LogLevel.Error, "Display image save failed.", path, ex);
                return false;
            }
        }

        public void RecordWorkspaceImagePath(string path)
        {
            try
            {
                string normalizedPath = string.IsNullOrWhiteSpace(path)
                    ? null
                    : Path.GetFullPath(path);
                string directory = string.IsNullOrWhiteSpace(normalizedPath)
                    ? null
                    : Path.GetDirectoryName(normalizedPath);
                if (!string.IsNullOrWhiteSpace(directory))
                {
                    lastWorkspaceImageDirectory = directory;
                    OpenVisionImageDirectoryResolver.RememberImagePath(normalizedPath);
                    rememberWorkspaceImagePath?.Invoke(normalizedPath);
                }
            }
            catch (Exception)
            {
            }
        }

        public void OpenImageCompare()
        {
            if (TryOpenImageCompare(AppPathService.InstallationRootDirectory, Process.Start, out string failureDetail)) { return; }
            OVLog.Write(LogCategory.Main, LogLevel.Error, "Image comparison tool open failed.", failureDetail);
            string message = string.Format(CultureInfo.CurrentCulture, OpenVisionLanguageService.T("Shell.ImageCompareOpenFailedMessage"), failureDetail);
            MessageBox.Show(ownerProvider(), message, "OpenVisionLab", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        internal static bool TryOpenImageCompare(string installationDirectory, Func<ProcessStartInfo, Process> startProcess, out string failureDetail)
        {
            try
            {
                string directory = Path.Combine(installationDirectory, "Tools", "ImageCompare");
                string executable = Path.Combine(directory, "OpenVisionLab.ImageCompare.exe");
                foreach (string name in new[] { "OpenVisionLab.ImageCompare.exe", "OpenVisionLab.ImageCompare.dll", "OpenVisionLab.ImageCompare.runtimeconfig.json" })
                {
                    if (!File.Exists(Path.Combine(directory, name)))
                    {
                        failureDetail = string.Format(CultureInfo.CurrentCulture, OpenVisionLanguageService.T("Shell.ImageCompareNotFoundMessage"), Path.Combine(directory, name));
                        return false;
                    }
                }
                using Process process = startProcess(new ProcessStartInfo { FileName = executable, WorkingDirectory = directory, UseShellExecute = true });
                if (process == null) { throw new InvalidOperationException("Image comparison process was not started."); }
                failureDetail = string.Empty;
                return true;
            }
            catch (Exception ex)
            {
                failureDetail = ex.Message;
                return false;
            }
        }

        public void OpenTutorial()
        {
            if (!TryResolveUserManualPath(out string tutorialPath, out string failureDetail))
            {
                string message = string.Format(
                    CultureInfo.CurrentCulture,
                    OpenVisionLanguageService.T("Shell.TutorialNotFoundMessage"),
                    failureDetail);
                MessageBox.Show(message, "OpenVisionLab", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = tutorialPath,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                string message = string.Format(
                    CultureInfo.CurrentCulture,
                    OpenVisionLanguageService.T("Shell.TutorialOpenFailedMessage"),
                    ex.Message);
                MessageBox.Show(message, "OpenVisionLab", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public bool HasRunnableSample()
        {
            return hasRunnableSample;
        }

        public void PromptAndOpenSampleImages()
        {
            if (importingWorkspaceImages || workspaceImages?.CanEdit == false) { return; }
            List<VisionPipelineSampleCatalogItem> samples = LoadRunnableSamples();
            if (samples.Count == 0) { return; }
            IReadOnlyList<VisionPipelineSampleCatalogItem> selection;
            bool useLabels;
            importingWorkspaceImages = true;
            try
            {
                if (!OpenVisionWorkspaceSamplePickerWindow.TrySelectImages(ownerProvider(), samples, out selection, out useLabels)) { return; }
            }
            finally { importingWorkspaceImages = false; }
            AddSampleImages(selection, useLabels);
        }

        internal int AddSampleImages(IEnumerable<VisionPipelineSampleCatalogItem> samples, bool useCatalogLabels, Action<string> reportFailure = null)
        {
            var selected = (samples ?? Enumerable.Empty<VisionPipelineSampleCatalogItem>()).Where(item => item != null && !string.IsNullOrWhiteSpace(item.ImageFullPath)).GroupBy(item => item.ImageFullPath, StringComparer.OrdinalIgnoreCase).Select(group => group.First()).ToArray();
            var labels = selected.ToDictionary(item => item.ImageFullPath, item => !useCatalogLabels ? "" : string.Equals(item.PairRole, "Good", StringComparison.OrdinalIgnoreCase) ? "good" : string.Equals(item.PairRole, "Bad", StringComparison.OrdinalIgnoreCase) ? "bad" : "", StringComparer.OrdinalIgnoreCase);
            return AddWorkspaceImages(selected.Select(item => item.ImageFullPath), sampleImages: true, imageLabels: labels, reportFailure: reportFailure);
        }

        public void PromptAndOpenRunnableSample()
        {
            PromptAndOpenRunnableSample(null);
        }

        public void PromptAndOpenRunnableSample(string preferredLearnPathId)
        {
            List<VisionPipelineSampleCatalogItem> samples = LoadRunnableSamples();
            if (samples.Count == 0)
            {
                ShowNoRunnableSampleMessage();
                return;
            }

            if (samples.Count == 1 && string.IsNullOrWhiteSpace(preferredLearnPathId))
            {
                OpenRunnableSample(samples[0]);
                return;
            }

            if (OpenVisionWorkspaceSamplePickerWindow.TrySelectSample(ownerProvider(), samples, preferredLearnPathId, out VisionPipelineSampleCatalogItem sample))
            {
                OpenRunnableSample(sample);
            }
        }

        public void OpenFirstRunnableSample()
        {
            VisionPipelineSampleCatalogItem sample = LoadRunnableSamples().FirstOrDefault();
            if (sample == null)
            {
                ShowNoRunnableSampleMessage();
                return;
            }

            OpenRunnableSample(sample);
        }

        public bool OpenRunnableSampleByName(string sampleName)
        {
            if (string.IsNullOrWhiteSpace(sampleName))
            {
                return false;
            }

            VisionPipelineSampleCatalogItem sample = LoadRunnableSamples()
                .FirstOrDefault(item => string.Equals(item.SampleName, sampleName, StringComparison.OrdinalIgnoreCase));
            return sample != null && OpenRunnableSample(sample);
        }

        private bool OpenRunnableSample(VisionPipelineSampleCatalogItem sample)
        {
            if (sample == null || !sample.CanOpen)
            {
                ShowNoRunnableSampleMessage();
                return false;
            }

            if (!SerializeHelper.TryLoadFromXmlFile(sample.PipelineFullPath, out VisionPipeline pipeline) || pipeline == null)
            {
                ShowSamplePipelineLoadFailedMessage(sample.PipelineFullPath);
                return false;
            }

            sample.ResolvePipelineDependencyPaths(pipeline);
            pipeline.Name = CreateSamplePipelineName(sample.SampleName);
            if (prepareSampleWorkspaceContext?.Invoke(sample.SampleName, pipeline.Name) == false)
            {
                return false;
            }

            if (!loadWorkspaceImage(sample.ImageFullPath))
            {
                ShowSampleImageLoadFailedMessage(sample.ImageFullPath);
                return false;
            }

            OpenVisionRecipeContext recipeContext = ResolveRecipeContextForSample();
            VisionPipelineStorage.Save(recipeContext.Name, pipeline);
            VisionPipelineStorage.SaveActivePipelineName(recipeContext.Name, pipeline.Name);

            RecordWorkspaceImagePath(sample.ImageFullPath);
            OVLog.Write(
                LogCategory.Main,
                LogLevel.Info,
                "Sample loaded",
                sample.SampleName,
                "Pipeline",
                pipeline.Name);
            sampleWorkspaceLoaded?.Invoke(sample.SampleName, pipeline.Name);
            return true;
        }

        private string ResolveWorkspaceImageDirectory()
        {
            return OpenVisionImageDirectoryResolver.ResolveOpenImageDirectory(lastWorkspaceImageDirectory);
        }

        internal static string ResolveExistingImageDirectory(string path)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(path))
                {
                    return null;
                }

                string normalized = Path.GetFullPath(path);
                if (File.Exists(normalized))
                {
                    return Path.GetDirectoryName(normalized);
                }

                return Directory.Exists(normalized) ? normalized : null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private OpenVisionRecipeContext ResolveRecipeContextForSample()
        {
            OpenVisionRecipeContext context = recipeContextProvider();
            return context ?? new OpenVisionRecipeContext(
                id: "Default",
                name: "Default",
                pipelineName: VisionPipelineAppendService.DefaultPipelineName,
                sourcePath: string.Empty,
                isDirty: false,
                activeLayerName: "Main",
                lastReviewState: string.Empty);
        }

        private static List<VisionPipelineSampleCatalogItem> LoadRunnableSamples()
        {
            return VisionPipelineSampleCatalogItem.LoadRunnable()
                .Where(item => item.CanOpen
                    && item.CatalogSourceKind != VisionPipelineSampleCatalogSourceKind.LocalLegacy)
                .ToList();
        }

        private static void ShowNoRunnableSampleMessage()
        {
            MessageBox.Show(
                OpenVisionLanguageService.CurrentLanguage == OpenVisionLanguage.English
                    ? "No runnable sample image and pipeline were found."
                    : "실행 가능한 샘플 이미지와 파이프라인을 찾을 수 없습니다.",
                "OpenVisionLab",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
        }

        private static void ShowSamplePipelineLoadFailedMessage(string path)
        {
            MessageBox.Show(
                string.Format(
                    CultureInfo.CurrentCulture,
                    OpenVisionLanguageService.CurrentLanguage == OpenVisionLanguage.English
                        ? "Sample pipeline could not be loaded: {0}"
                        : "샘플 파이프라인을 불러올 수 없습니다: {0}",
                    path),
                "OpenVisionLab",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
        }

        private static void ShowSampleImageLoadFailedMessage(string path)
        {
            MessageBox.Show(
                string.Format(
                    CultureInfo.CurrentCulture,
                    OpenVisionLanguageService.CurrentLanguage == OpenVisionLanguage.English
                        ? "Sample image could not be loaded: {0}"
                        : "샘플 이미지를 불러올 수 없습니다: {0}",
                    path),
                "OpenVisionLab",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
        }

        private static void ShowWorkspaceImageLoadFailedMessage(string path)
        {
            MessageBox.Show(
                string.Format(
                    CultureInfo.CurrentCulture,
                    OpenVisionLanguageService.CurrentLanguage == OpenVisionLanguage.English
                        ? "Image could not be loaded. The file format or image dimensions may be unsupported:\n{0}"
                        : "이미지를 불러올 수 없습니다. 파일 형식 또는 이미지 크기가 지원되지 않을 수 있습니다:\n{0}",
                    path),
                "OpenVisionLab",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
        }

        private static string CreateSamplePipelineName(string sampleName)
        {
            string rawName = string.IsNullOrWhiteSpace(sampleName) ? "Sample" : sampleName.Trim();
            char[] invalidChars = Path.GetInvalidFileNameChars();
            string safeName = new string(rawName.Select(ch => invalidChars.Contains(ch) ? '_' : ch).ToArray());
            return "Sample_" + (string.IsNullOrWhiteSpace(safeName) ? "Pipeline" : safeName);
        }

        internal static bool TryResolveUserManualPath(out string manualPath, out string failureDetail)
        {
            string language = OpenVisionLanguageService.CurrentLanguage == OpenVisionLanguage.English
                ? "en"
                : "ko";
            string packagedDirectory = Path.Combine(AppContext.BaseDirectory, "Guide");
            if (TryValidateUserManual(packagedDirectory, language, out manualPath, out failureDetail))
            {
                return true;
            }

            string packagedFailure = failureDetail;
            foreach (string root in EnumerateDevelopmentRepositoryRoots())
            {
                string developmentDirectory = Path.Combine(
                    root,
                    "docs",
                    "manual",
                    "generated");
                if (TryValidateUserManual(
                        developmentDirectory,
                        language,
                        out manualPath,
                        out failureDetail))
                {
                    return true;
                }
            }

            manualPath = Path.Combine(
                packagedDirectory,
                $"OpenVisionLab_User_Manual.{language}.html");
            failureDetail = packagedFailure;
            return false;
        }

        internal static bool TryValidateUserManual(
            string manualDirectory,
            string language,
            out string manualPath,
            out string failureDetail)
        {
            string expectedFileName = $"OpenVisionLab_User_Manual.{language}.html";
            manualPath = Path.Combine(manualDirectory, expectedFileName);
            string manifestPath = Path.Combine(manualDirectory, "guide-manifest.json");
            failureDetail = manifestPath;
            if (!File.Exists(manifestPath))
            {
                return false;
            }

            try
            {
                using JsonDocument manifest = JsonDocument.Parse(File.ReadAllText(manifestPath));
                JsonElement root = manifest.RootElement;
                if (!root.TryGetProperty("schemaVersion", out JsonElement schemaVersion) ||
                    schemaVersion.GetInt32() != 2 ||
                    !root.TryGetProperty("manuals", out JsonElement manualsElement) ||
                    manualsElement.ValueKind != JsonValueKind.Array)
                {
                    failureDetail = manifestPath;
                    return false;
                }

                JsonElement selectedManual = default;
                bool found = false;
                foreach (JsonElement candidate in manualsElement.EnumerateArray())
                {
                    if (!candidate.TryGetProperty("language", out JsonElement candidateLanguage) ||
                        !string.Equals(candidateLanguage.GetString(), language, StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    if (found)
                    {
                        failureDetail = manifestPath + $" (duplicate {language} manual)";
                        return false;
                    }

                    selectedManual = candidate;
                    found = true;
                }

                if (!found ||
                    !selectedManual.TryGetProperty("file", out JsonElement fileElement) ||
                    !string.Equals(fileElement.GetString(), expectedFileName, StringComparison.Ordinal) ||
                    !selectedManual.TryGetProperty("sha256", out JsonElement hashElement) ||
                    !File.Exists(manualPath))
                {
                    failureDetail = found ? manualPath : manifestPath + $" ({language} manual missing)";
                    return false;
                }

                string expectedHash = hashElement.GetString() ?? string.Empty;
                string actualHash = Convert.ToHexString(
                    SHA256.HashData(File.ReadAllBytes(manualPath)));
                if (!string.Equals(expectedHash, actualHash, StringComparison.OrdinalIgnoreCase))
                {
                    failureDetail = manualPath + " (SHA-256 mismatch)";
                    return false;
                }

                string html = File.ReadAllText(manualPath);
                if (!html.Contains("data-openvisionlab-manual-version=", StringComparison.Ordinal) ||
                    !html.Contains($"data-openvisionlab-manual-language=\"{language}\"", StringComparison.Ordinal) ||
                    !html.Contains($"<html lang=\"{language}\"", StringComparison.OrdinalIgnoreCase) ||
                    html.Contains("Moved to canonical location", StringComparison.OrdinalIgnoreCase))
                {
                    failureDetail = manualPath + " (invalid manual content)";
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                failureDetail = manualPath + " (" + ex.Message + ")";
                return false;
            }
        }

        private static IEnumerable<string> EnumerateDevelopmentRepositoryRoots()
        {
            HashSet<string> seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (string start in new[] { Environment.CurrentDirectory, AppDomain.CurrentDomain.BaseDirectory })
            {
                if (string.IsNullOrWhiteSpace(start))
                {
                    continue;
                }

                DirectoryInfo directory = new DirectoryInfo(start);
                for (int depth = 0; directory != null && depth < 8; depth++)
                {
                    bool hasSolution = File.Exists(Path.Combine(directory.FullName, "OpenVisionLab.sln"));
                    string gitPath = Path.Combine(directory.FullName, ".git");
                    bool hasGitMarker = Directory.Exists(gitPath) || File.Exists(gitPath);
                    if (hasSolution && hasGitMarker && seen.Add(directory.FullName))
                    {
                        yield return directory.FullName;
                    }

                    directory = directory.Parent;
                }
            }
        }
    }
}
