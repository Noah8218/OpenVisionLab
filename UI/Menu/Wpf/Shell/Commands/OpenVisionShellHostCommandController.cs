using Microsoft.Win32;
using Lib.OpenCV.Pipeline;
using OpenVisionLab.Logging;
using OpenVisionLab.Logging.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using static OpenVisionLab.DEFINE;

namespace OpenVisionLab
{
    internal sealed class OpenVisionShellHostCommandController
    {
        private readonly Func<Window> ownerProvider;
        private readonly Func<string, bool> loadWorkspaceImage;
        private readonly OpenVisionShellHostWorkspacePreviewController workspacePreviewController;
        private readonly OpenVisionZoomableImageController workspaceFallbackZoomController;
        private readonly Func<string> workspaceLayerTitleProvider;
        private readonly Func<OpenVisionRecipeContext> recipeContextProvider;
        private readonly Action<VISION_MENU> selectToolMenu;
        private readonly Action sampleWorkspaceLoaded;
        private readonly Action manualWorkspaceImageLoaded;
        private string lastWorkspaceImageDirectory;
        private OpenVisionLearnWindow learnWindow;

        public OpenVisionShellHostCommandController(
            Func<Window> ownerProvider,
            Func<string, bool> loadWorkspaceImage,
            OpenVisionShellHostWorkspacePreviewController workspacePreviewController,
            OpenVisionZoomableImageController workspaceFallbackZoomController,
            Func<string> workspaceLayerTitleProvider,
            Func<OpenVisionRecipeContext> recipeContextProvider,
            Action<VISION_MENU> selectToolMenu,
            Action sampleWorkspaceLoaded = null,
            Action manualWorkspaceImageLoaded = null)
        {
            this.ownerProvider = ownerProvider ?? throw new ArgumentNullException(nameof(ownerProvider));
            this.loadWorkspaceImage = loadWorkspaceImage ?? throw new ArgumentNullException(nameof(loadWorkspaceImage));
            this.workspacePreviewController = workspacePreviewController ?? throw new ArgumentNullException(nameof(workspacePreviewController));
            this.workspaceFallbackZoomController = workspaceFallbackZoomController ?? throw new ArgumentNullException(nameof(workspaceFallbackZoomController));
            this.workspaceLayerTitleProvider = workspaceLayerTitleProvider ?? throw new ArgumentNullException(nameof(workspaceLayerTitleProvider));
            this.recipeContextProvider = recipeContextProvider ?? throw new ArgumentNullException(nameof(recipeContextProvider));
            this.selectToolMenu = selectToolMenu ?? throw new ArgumentNullException(nameof(selectToolMenu));
            this.sampleWorkspaceLoaded = sampleWorkspaceLoaded;
            this.manualWorkspaceImageLoaded = manualWorkspaceImageLoaded;
        }

        public void PromptAndLoadWorkspaceImage()
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Title = OpenVisionLanguageService.T("Shell.WorkspaceLoadImage"),
                Filter = "Image files|*.bmp;*.png;*.jpg;*.jpeg;*.tif;*.tiff|Bitmap (*.bmp)|*.bmp|PNG (*.png)|*.png|JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|TIFF (*.tif;*.tiff)|*.tif;*.tiff|All files|*.*",
                Multiselect = false,
                InitialDirectory = ResolveWorkspaceImageDirectory()
            };

            if (dialog.ShowDialog(ownerProvider()) == true
                && loadWorkspaceImage(dialog.FileName))
            {
                RecordWorkspaceImagePath(dialog.FileName);
                manualWorkspaceImageLoaded?.Invoke();
            }
        }

        public void FitWorkspaceImage()
        {
            workspaceFallbackZoomController.Reset();
            workspacePreviewController.RefreshCanvas();
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
            }
        }

        public void RecordWorkspaceImagePath(string path)
        {
            string directory = string.IsNullOrWhiteSpace(path) ? null : Path.GetDirectoryName(path);
            if (!string.IsNullOrWhiteSpace(directory))
            {
                lastWorkspaceImageDirectory = directory;
            }
        }

        public void OpenTutorial()
        {
            string tutorialPath = ResolveTutorialPath();
            if (!File.Exists(tutorialPath))
            {
                string message = string.Format(
                    CultureInfo.CurrentCulture,
                    OpenVisionLanguageService.T("Shell.TutorialNotFoundMessage"),
                    tutorialPath);
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

        public void OpenLearn()
        {
            OpenLearn(null);
        }

        public void OpenLearnForTool(VISION_MENU menu)
        {
            if (OpenVisionLearnTopicCatalog.TryResolveForTool(menu, out OpenVisionLearnTopicIndex topicIndex))
            {
                OpenLearn(topicIndex);
            }
        }

        public void OpenLearnForToolType(string toolType)
        {
            if (OpenVisionLearnTopicCatalog.TryResolveForToolType(toolType, out OpenVisionLearnTopicIndex topicIndex))
            {
                OpenLearn(topicIndex);
            }
        }

        public void OpenSamplesForTool(VISION_MENU menu)
        {
            if (OpenVisionLearnTopicCatalog.TryResolveForTool(menu, out OpenVisionLearnTopicIndex topicIndex))
            {
                PromptAndOpenRunnableSample(OpenVisionLearnTopicCatalog.Resolve(topicIndex).PracticePathId);
            }
        }

        private void OpenLearn(OpenVisionLearnTopicIndex? topicIndex)
        {
            if (learnWindow != null)
            {
                if (topicIndex.HasValue)
                {
                    learnWindow.SelectTopic(topicIndex.Value);
                }

                learnWindow.Activate();
                return;
            }

            learnWindow = topicIndex.HasValue
                ? new OpenVisionLearnWindow(127, 255, false, (int)topicIndex.Value)
                : new OpenVisionLearnWindow();
            learnWindow.Owner = ownerProvider();
            learnWindow.SetOpenPracticeSamplesAction(PromptAndOpenRunnableSample);
            learnWindow.SetOpenRelatedToolAction(selectToolMenu);
            learnWindow.Closed += LearnWindow_Closed;
            learnWindow.Show();
        }

        public bool HasRunnableSample()
        {
            return LoadRunnableSamples().Count > 0;
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

            if (!loadWorkspaceImage(sample.ImageFullPath))
            {
                ShowSampleImageLoadFailedMessage(sample.ImageFullPath);
                return false;
            }

            pipeline.Name = CreateSamplePipelineName(sample.SampleName);
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
            sampleWorkspaceLoaded?.Invoke();
            return true;
        }

        private string ResolveWorkspaceImageDirectory()
        {
            return OpenVisionImageDirectoryResolver.ResolveOpenImageDirectory(lastWorkspaceImageDirectory);
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

        private static string CreateSamplePipelineName(string sampleName)
        {
            string rawName = string.IsNullOrWhiteSpace(sampleName) ? "Sample" : sampleName.Trim();
            char[] invalidChars = Path.GetInvalidFileNameChars();
            string safeName = new string(rawName.Select(ch => invalidChars.Contains(ch) ? '_' : ch).ToArray());
            return "Sample_" + (string.IsNullOrWhiteSpace(safeName) ? "Pipeline" : safeName);
        }

        private void LearnWindow_Closed(object sender, EventArgs e)
        {
            if (learnWindow != null)
            {
                learnWindow.Closed -= LearnWindow_Closed;
                learnWindow = null;
            }
        }

        private static string ResolveTutorialPath()
        {
            foreach (string root in EnumerateTutorialSearchRoots())
            {
                string portablePath = Path.Combine(root, "docs", "OPENVISIONLAB_TUTORIAL_PORTABLE.html");
                if (File.Exists(portablePath))
                {
                    return portablePath;
                }

                string htmlPath = Path.Combine(root, "docs", "OPENVISIONLAB_TUTORIAL.html");
                if (File.Exists(htmlPath))
                {
                    return htmlPath;
                }
            }

            return Path.Combine(Environment.CurrentDirectory, "docs", "OPENVISIONLAB_TUTORIAL_PORTABLE.html");
        }

        private static IEnumerable<string> EnumerateTutorialSearchRoots()
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
                    if (seen.Add(directory.FullName))
                    {
                        yield return directory.FullName;
                    }

                    directory = directory.Parent;
                }
            }
        }
    }
}
