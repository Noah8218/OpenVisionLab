using OpenVisionLab.Core;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using static OpenVisionLab.DEFINE;

namespace OpenVisionLab
{
    internal sealed class OpenVisionShellHostToolWindowController
    {
        private readonly IDisplayManager displayManager;
        private readonly OpenVisionShellHostDocumentController documentController;
        private readonly OpenVisionShellHostLayerViewerController layerViewerController;
        private readonly OpenVisionFloatingToolWindowHost floatingToolWindowHost;
        private readonly OpenVisionShellHostToolWindowLifecycleController toolWindowLifecycleController;
        private readonly Func<Window> ownerProvider;
        private readonly Func<OpenVisionRecipeContext> recipeContextProvider;
        private readonly Action setDirectRunPending;
        private readonly Action setDirectRunSucceeded;
        private readonly Action<string> setActiveDocumentText;
        private readonly Action refreshHostLayerRows;
        private readonly Func<string, bool> openWorkspaceSampleByName;
        private readonly Action returnToRecipeManager;
        private readonly Action<string, string, int> openPipelineStepEditor;
        private readonly Action<string> openLearnForToolType;

        public OpenVisionShellHostToolWindowController(
            IDisplayManager displayManager,
            OpenVisionShellHostDocumentController documentController,
            OpenVisionShellHostLayerViewerController layerViewerController,
            OpenVisionFloatingToolWindowHost floatingToolWindowHost,
            OpenVisionShellHostToolWindowLifecycleController toolWindowLifecycleController,
            Func<Window> ownerProvider,
            Func<OpenVisionRecipeContext> recipeContextProvider,
            Action setDirectRunPending,
            Action setDirectRunSucceeded,
            Action<string> setActiveDocumentText,
            Action refreshHostLayerRows,
            Func<string, bool> openWorkspaceSampleByName,
            Action returnToRecipeManager,
            Action<string, string, int> openPipelineStepEditor,
            Action<string> openLearnForToolType)
        {
            this.displayManager = displayManager ?? throw new ArgumentNullException(nameof(displayManager));
            this.documentController = documentController ?? throw new ArgumentNullException(nameof(documentController));
            this.layerViewerController = layerViewerController ?? throw new ArgumentNullException(nameof(layerViewerController));
            this.floatingToolWindowHost = floatingToolWindowHost ?? throw new ArgumentNullException(nameof(floatingToolWindowHost));
            this.toolWindowLifecycleController = toolWindowLifecycleController ?? throw new ArgumentNullException(nameof(toolWindowLifecycleController));
            this.ownerProvider = ownerProvider ?? throw new ArgumentNullException(nameof(ownerProvider));
            this.recipeContextProvider = recipeContextProvider ?? throw new ArgumentNullException(nameof(recipeContextProvider));
            this.setDirectRunPending = setDirectRunPending ?? throw new ArgumentNullException(nameof(setDirectRunPending));
            this.setDirectRunSucceeded = setDirectRunSucceeded ?? throw new ArgumentNullException(nameof(setDirectRunSucceeded));
            this.setActiveDocumentText = setActiveDocumentText ?? throw new ArgumentNullException(nameof(setActiveDocumentText));
            this.refreshHostLayerRows = refreshHostLayerRows ?? throw new ArgumentNullException(nameof(refreshHostLayerRows));
            this.openWorkspaceSampleByName = openWorkspaceSampleByName ?? throw new ArgumentNullException(nameof(openWorkspaceSampleByName));
            this.returnToRecipeManager = returnToRecipeManager ?? throw new ArgumentNullException(nameof(returnToRecipeManager));
            this.openPipelineStepEditor = openPipelineStepEditor ?? throw new ArgumentNullException(nameof(openPipelineStepEditor));
            this.openLearnForToolType = openLearnForToolType ?? throw new ArgumentNullException(nameof(openLearnForToolType));
        }

        public OpenVisionToolOpenTiming LastTiming { get; private set; }

        public bool HasPreparedPipelineReview =>
            documentController.HasPipelineReviewFor(ResolveRecipeContext());

        public bool ShowSelectedTool(OpenVisionShellNavItem item)
        {
            if (item == null)
            {
                return false;
            }

            Stopwatch totalStopwatch = Stopwatch.StartNew();
            Stopwatch phaseStopwatch = new Stopwatch();
            OpenVisionToolOpenTiming timing = new OpenVisionToolOpenTiming
            {
                Menu = item.Menu,
                Title = item.Title
            };
            OpenVisionToolOpenProfiler.Begin();
            bool showPipelineReviewDocked = item.Menu == VISION_MENU.Pipeline
                && toolWindowLifecycleController.ShouldShowPipelineReviewDocked;
            toolWindowLifecycleController.PrepareForToolSelection(item.Menu);

            if (item.Menu == VISION_MENU.Pipeline)
            {
                string title = OpenVisionLanguageService.T("PipelineReview.Title");
                timing.Title = title;
                timing.Path = "Pipeline";
                phaseStopwatch.Restart();
                OpenVisionRecipeContext recipeContext = ResolveRecipeContext();
                if (!documentController.TryRestorePipelineReview(recipeContext, out OpenVisionPipelineReviewDocument pipelineReviewDocument))
                {
                    pipelineReviewDocument = CreatePipelineReviewDocument(recipeContext);
                    documentController.ActivatePipelineReview(pipelineReviewDocument);
                }
                else
                {
                    pipelineReviewDocument.RefreshIfPipelineChanged();
                }
                timing.ActivateDocumentMs = phaseStopwatch.ElapsedMilliseconds;
                timing.Document = pipelineReviewDocument.View?.GetType().Name ?? pipelineReviewDocument.GetType().Name;
                if (showPipelineReviewDocked)
                {
                    ShowDockedDocumentWorkspace(pipelineReviewDocument.View, title, 1180, 760, timing);
                }
                else
                {
                    ShowWpfToolWindow(pipelineReviewDocument.View, title, 1180, 760, timing);
                }
                phaseStopwatch.Restart();
                CompleteToolSelection(title, hasDisplayablePreviewResult: false, refreshLayerRows: false);
                timing.CompleteSelectionMs = phaseStopwatch.ElapsedMilliseconds;
                timing.TotalMs = totalStopwatch.ElapsedMilliseconds;
                timing.DetailText = OpenVisionToolOpenProfiler.Consume();
                LastTiming = timing;
                return true;
            }

            phaseStopwatch.Restart();
            bool nativeDocumentWasCached = documentController.NativeToolDocuments.Contains(item.Menu);
            if (documentController.TryActivateNativeTool(item.Menu, displayManager, ResolveRecipeContext(), out OpenVisionNativeToolDocument nativeDocument))
            {
                timing.Path = "Native";
                timing.ActivateDocumentMs = phaseStopwatch.ElapsedMilliseconds;
                timing.Document = nativeDocument.View?.GetType().Name ?? nativeDocument.GetType().Name;
                nativeDocument.ConfigurePreviewViewer(
                    layerViewerController.OpenToolPreview,
                    layerViewerController.RefreshToolPreview,
                    layerViewerController.CloseToolPreview);
                phaseStopwatch.Restart();
                nativeDocument.RefreshLayerState();
                if (!nativeDocumentWasCached)
                {
                    WarmPrewarmedNativeToolDocument(nativeDocument);
                }
                timing.RefreshLayerStateMs = phaseStopwatch.ElapsedMilliseconds;
                phaseStopwatch.Restart();
                Size nativeToolSize = OpenVisionNativeToolPrewarmPolicy.GetPreferredWindowSize(item.Menu);
                timing.ResolveSizeMs = phaseStopwatch.ElapsedMilliseconds;
                ShowWpfToolWindow(nativeDocument.View, item.Title, nativeToolSize.Width, nativeToolSize.Height, timing, preferDockedWhenActive: true);
                phaseStopwatch.Restart();
                CompleteToolSelection(item.Title, HasDisplayablePreviewResult(nativeDocument));
                timing.CompleteSelectionMs = phaseStopwatch.ElapsedMilliseconds;
                timing.TotalMs = totalStopwatch.ElapsedMilliseconds;
                timing.DetailText = OpenVisionToolOpenProfiler.Consume();
                LastTiming = timing;
                return true;
            }
            timing.ActivateDocumentMs = phaseStopwatch.ElapsedMilliseconds;

            timing.Path = "Pending";
            ShowPendingToolWindow(item, timing);
            phaseStopwatch.Restart();
            CompleteToolSelection(item.Title, hasDisplayablePreviewResult: false);
            timing.CompleteSelectionMs = phaseStopwatch.ElapsedMilliseconds;
            timing.TotalMs = totalStopwatch.ElapsedMilliseconds;
            timing.DetailText = OpenVisionToolOpenProfiler.Consume();
            LastTiming = timing;
            return true;
        }

        public void PrewarmPipelineReview()
        {
            if (documentController.HasPipelineReviewDocument)
            {
                return;
            }

            OpenVisionPipelineReviewDocument document = CreatePipelineReviewDocument(ResolveRecipeContext());
            if (!documentController.TryCachePipelineReview(document))
            {
                document.Dispose();
                return;
            }

            FrameworkElement view = PrepareHostedWpfDocument(document.View);
            Size warmSize = new Size(1180, 760);
            if (documentController.ActiveNativeDocument == null
                && documentController.ActivePendingToolViewModel == null)
            {
                toolWindowLifecycleController.PrepareDockedDocumentWorkspace(
                    view,
                    OpenVisionLanguageService.T("PipelineReview.Title"),
                    warmSize.Width,
                    warmSize.Height);
            }
            view.ApplyTemplate();
            view.Measure(warmSize);
            view.Arrange(new Rect(0, 0, warmSize.Width, warmSize.Height));
            view.UpdateLayout();
        }

        private void OnPipelineReviewOpenWorkspaceSampleRequested(object sender, OpenVisionPipelineReviewSampleOpenRequestedEventArgs e)
        {
            if (e == null || string.IsNullOrWhiteSpace(e.SampleName))
            {
                return;
            }

            if (!openWorkspaceSampleByName(e.SampleName))
            {
                return;
            }

            documentController.ActivePipelineReviewDocument?.RefreshLayerState();
            setDirectRunPending();
            refreshHostLayerRows();
        }

        private void OnPipelineReviewReturnToRecipeRequested(object sender, EventArgs e)
        {
            returnToRecipeManager();
        }

        private void OnPipelineReviewOpenSelectedToolLearnRequested(object sender, EventArgs e)
        {
            if (sender is OpenVisionPipelineReviewDocument document)
            {
                openLearnForToolType(document.SelectedToolType);
            }
        }

        private void OnPipelineReviewEditSelectedStepRequested(object sender, EventArgs e)
        {
            if (sender is OpenVisionPipelineReviewDocument document)
            {
                openPipelineStepEditor(
                    document.RecipeContext.Name,
                    document.ActivePipelineName,
                    document.SelectedStepNumber);
            }
        }

        private OpenVisionRecipeContext ResolveRecipeContext()
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

        private OpenVisionPipelineReviewDocument CreatePipelineReviewDocument(OpenVisionRecipeContext recipeContext)
        {
            OpenVisionPipelineReviewDocument document = new OpenVisionPipelineReviewDocument(displayManager, recipeContext);
            document.OpenWorkspaceSampleRequested += OnPipelineReviewOpenWorkspaceSampleRequested;
            document.ReturnToRecipeRequested += OnPipelineReviewReturnToRecipeRequested;
            document.OpenSelectedToolLearnRequested += OnPipelineReviewOpenSelectedToolLearnRequested;
            document.EditSelectedStepRequested += OnPipelineReviewEditSelectedStepRequested;
            return document;
        }

        public void ShowWpfToolWindow(FrameworkElement content, string title, double width, double height)
        {
            ShowWpfToolWindow(content, title, width, height, null);
        }

        private void ShowWpfToolWindow(FrameworkElement content, string title, double width, double height, OpenVisionToolOpenTiming timing)
        {
            ShowWpfToolWindow(content, title, width, height, timing, preferDockedWhenActive: false);
        }

        private void ShowWpfToolWindow(
            FrameworkElement content,
            string title,
            double width,
            double height,
            OpenVisionToolOpenTiming timing,
            bool preferDockedWhenActive)
        {
            Stopwatch phaseStopwatch = Stopwatch.StartNew();
            FrameworkElement preparedContent = PrepareHostedWpfDocument(content);
            if (preferDockedWhenActive && toolWindowLifecycleController.IsDockedToolInspectorVisible)
            {
                ShowDockedToolWindow(preparedContent, title, width, height, timing);
                return;
            }

            OpenVisionToolDockModeHelper.Apply(preparedContent, false);
            if (timing != null)
            {
                timing.PrepareHostedDocumentMs = phaseStopwatch.ElapsedMilliseconds;
            }

            phaseStopwatch.Restart();
            bool reusedWindow = floatingToolWindowHost.Show(preparedContent, title, width, height, ownerProvider());
            if (timing != null)
            {
                timing.FloatingHostShowMs = phaseStopwatch.ElapsedMilliseconds;
                timing.ReusedFloatingWindow = reusedWindow;
            }
        }

        private bool ShowDockedToolWindow(FrameworkElement content, string title, double width, double height, OpenVisionToolOpenTiming timing)
        {
            Stopwatch phaseStopwatch = Stopwatch.StartNew();
            FrameworkElement preparedContent = PrepareHostedWpfDocument(content);
            if (timing != null)
            {
                timing.PrepareHostedDocumentMs = phaseStopwatch.ElapsedMilliseconds;
            }

            phaseStopwatch.Restart();
            bool shown = toolWindowLifecycleController.ShowDockedToolWindow(preparedContent, title, width, height);
            if (timing != null)
            {
                timing.FloatingHostShowMs = phaseStopwatch.ElapsedMilliseconds;
                timing.ReusedFloatingWindow = false;
            }

            return shown;
        }

        private bool ShowDockedDocumentWorkspace(
            FrameworkElement content,
            string title,
            double width,
            double height,
            OpenVisionToolOpenTiming timing)
        {
            Stopwatch phaseStopwatch = Stopwatch.StartNew();
            FrameworkElement preparedContent = PrepareHostedWpfDocument(content);
            if (timing != null)
            {
                timing.PrepareHostedDocumentMs = phaseStopwatch.ElapsedMilliseconds;
            }

            phaseStopwatch.Restart();
            bool shown = toolWindowLifecycleController.ShowDockedDocumentWorkspace(preparedContent, title, width, height);
            if (timing != null)
            {
                timing.FloatingHostShowMs = phaseStopwatch.ElapsedMilliseconds;
                timing.ReusedFloatingWindow = false;
            }

            return shown;
        }

        public static void WarmPrewarmedNativeToolDocument(OpenVisionNativeToolDocument document)
        {
            FrameworkElement view = document?.View;
            if (view == null)
            {
                return;
            }

            PrepareHostedWpfDocument(view);
            if (!OpenVisionNativeToolPrewarmPolicy.ShouldWarmHostedLayout(document))
            {
                return;
            }

            view.ApplyTemplate();
            if (view.IsLoaded || ContainsNativeAirspaceElement(view))
            {
                return;
            }

            // Only heavier PropertyGrid/template tools get layout warming; lightweight tools stay cached but avoid extra startup layout cost.
            Size layoutWarmSize = OpenVisionNativeToolPrewarmPolicy.GetLayoutWarmSize();
            view.Measure(layoutWarmSize);
            view.Arrange(new Rect(0, 0, layoutWarmSize.Width, layoutWarmSize.Height));
            view.UpdateLayout();
        }

        private void ShowPendingToolWindow(OpenVisionShellNavItem item, OpenVisionToolOpenTiming timing)
        {
            OpenVisionPendingToolViewModel pendingToolViewModel = new OpenVisionPendingToolViewModel(
                "VisionMenu." + item.Menu,
                item.Title,
                item.IconKind);
            documentController.ActivatePendingTool(pendingToolViewModel);
            OpenVisionPendingToolView view = new OpenVisionPendingToolView(pendingToolViewModel);
            if (timing != null)
            {
                timing.Document = view.GetType().Name;
            }

            ShowWpfToolWindow(view, item.Title, 820, 560, timing);
        }

        private void CompleteToolSelection(
            string title,
            bool hasDisplayablePreviewResult,
            bool refreshLayerRows = true)
        {
            if (hasDisplayablePreviewResult)
            {
                setDirectRunSucceeded();
            }
            else
            {
                setDirectRunPending();
            }

            setActiveDocumentText(title);
            if (refreshLayerRows)
            {
                refreshHostLayerRows();
            }
        }

        private bool HasDisplayablePreviewResult(OpenVisionNativeToolDocument document)
        {
            return document?.HasPreviewResult == true
                && !string.IsNullOrWhiteSpace(document.RouteOutputLayerName)
                && displayManager.GetLayerImage(document.RouteOutputLayerName) != null;
        }

        private static FrameworkElement PrepareHostedWpfDocument(FrameworkElement document)
        {
            if (document == null)
            {
                return null;
            }

            document.MinWidth = 0;
            document.MinHeight = 0;
            document.HorizontalAlignment = HorizontalAlignment.Stretch;
            document.VerticalAlignment = VerticalAlignment.Stretch;
            return document;
        }

        private static bool ContainsNativeAirspaceElement(DependencyObject root)
        {
            if (root == null)
            {
                return false;
            }

            string typeName = root.GetType().FullName ?? root.GetType().Name;
            if (typeName.Contains("WindowsFormsHost")
                || typeName.Contains("OpenGLControl")
                || typeName.Contains("ImageCanvasControl")
                || typeName.Contains("RoiImageCanvasView"))
            {
                return true;
            }

            int childCount;
            try
            {
                childCount = VisualTreeHelper.GetChildrenCount(root);
            }
            catch (InvalidOperationException)
            {
                return false;
            }

            for (int index = 0; index < childCount; index++)
            {
                if (ContainsNativeAirspaceElement(VisualTreeHelper.GetChild(root, index)))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
