using DrawingBitmap = System.Drawing.Bitmap;
using TestBitmap = System.Drawing.Bitmap;
using OpenVisionDockingVisualSnapshot = OpenVisionLab.Docking.Controls.OpenVisionDockingVisualSnapshot;
using OpenVisionLab.Core;
using OpenVisionLab.Vision2D.Pipeline;
using static OpenVisionLab.DEFINE;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using WpfUserControl = System.Windows.Controls.UserControl;

namespace OpenVisionLab
{
    public sealed partial class OpenVisionShellHostView : WpfUserControl, IDisposable
    {
        #region Core

        #region Constants and Dependency Properties

        public const string ShellMode = "WpfShellHost";
        private const string LayerDragDataFormat = "OpenVisionLab.LayerTitle";

        public static readonly DependencyProperty IsToolRailCompactProperty =
            DependencyProperty.Register(
                nameof(IsToolRailCompact),
                typeof(bool),
                typeof(OpenVisionShellHostView),
                new PropertyMetadata(false, OnToolRailCompactChanged));

        public static readonly DependencyProperty LayerCommandsProperty =
            DependencyProperty.Register(
                nameof(LayerCommands),
                typeof(OpenVisionShellHostLayerCommandSurface),
                typeof(OpenVisionShellHostView),
                new PropertyMetadata(null));

        public static readonly DependencyProperty WorkspaceCommandsProperty =
            DependencyProperty.Register(
                nameof(WorkspaceCommands),
                typeof(OpenVisionShellHostWorkspaceCommandSurface),
                typeof(OpenVisionShellHostView),
                new PropertyMetadata(null));

        public static readonly DependencyProperty RecipeCommandsProperty =
            DependencyProperty.Register(
                nameof(RecipeCommands),
                typeof(OpenVisionShellHostRecipeCommandSurface),
                typeof(OpenVisionShellHostView),
                new PropertyMetadata(null));

        public static readonly DependencyProperty CommandSurfacesProperty =
            DependencyProperty.Register(
                nameof(CommandSurfaces),
                typeof(OpenVisionShellHostCommandSurfaces),
                typeof(OpenVisionShellHostView),
                new PropertyMetadata(null));

        public static readonly DependencyProperty ChromeCommandsProperty =
            DependencyProperty.Register(
                nameof(ChromeCommands),
                typeof(OpenVisionShellHostChromeCommandSurface),
                typeof(OpenVisionShellHostView),
                new PropertyMetadata(null));

        public static readonly DependencyProperty SessionCommandsProperty =
            DependencyProperty.Register(
                nameof(SessionCommands),
                typeof(OpenVisionShellHostSessionCommandSurface),
                typeof(OpenVisionShellHostView),
                new PropertyMetadata(null));

        #endregion

        #region Fields

        private readonly ApplicationRuntimeContext runtimeContext;
        private readonly IDisplayManager displayManager;
        private readonly OpenVisionRecipeContextStore recipeContextStore;
        private readonly OpenVisionShellPreviewViewModel viewModel;
        private readonly OpenVisionShellHostRecipeContextPresenter recipeContextPresenter;
        private readonly OpenVisionShellHostLayerListPresenter layerListPresenter;
        private readonly OpenVisionShellHostLayerDetailPresenter layerDetailPresenter;
        private readonly OpenVisionShellHostLayerWorkspacePresenter layerWorkspacePresenter;
        private readonly OpenVisionShellHostMainActionPresenter mainActionPresenter;
        private readonly OpenVisionShellHostSampleWorkflowPresenter sampleWorkflowPresenter;
        private readonly OpenVisionShellHostLayerRefreshController layerRefreshController;
        private readonly OpenVisionShellHostWorkspacePreviewController workspacePreviewController;
        private readonly OpenVisionShellHostWorkspaceImageController workspaceImageController;
        private readonly OpenVisionLayerViewerWindowRegistry layerViewerWindows = new OpenVisionLayerViewerWindowRegistry();
        private readonly OpenVisionShellHostLayerViewerController layerViewerController;
        private readonly OpenVisionShellHostLayerManagementController layerManagementController;
        private readonly ShellDockedLayerWorkspaceComposition dockedLayerWorkspaceComposition;
        private readonly OpenVisionShellHostDocumentController documentController;
        private readonly OpenVisionFloatingToolWindowHost floatingToolWindowHost = new OpenVisionFloatingToolWindowHost();
        private readonly OpenVisionNativePreviewRouteCoordinator nativePreviewRouteCoordinator;
        private readonly OpenVisionShellHostLayerActivationController layerActivationController;
        private readonly OpenVisionShellHostLayerSelectionController layerSelectionController;
        private readonly OpenVisionShellHostLayerInteractionController layerInteractionController;
        private readonly OpenVisionShellHostToolWindowController toolWindowController;
        private readonly OpenVisionShellHostCommandController commandController;
        private readonly OpenVisionShellHostLearnWindowController learnWindowController;
        private readonly OpenVisionShellHostRecipeController recipeController;
        private readonly RecipeDialogAdapter recipeDialogAdapter;
        private readonly OpenVisionShellHostToolSelectionController toolSelectionController;
        private readonly OpenVisionShellHostBusyPresenter busyPresenter;
        private readonly OpenVisionShellHostToolRailPresenter toolRailPresenter = new OpenVisionShellHostToolRailPresenter();
        private readonly OpenVisionShellHostStatePresenter statePresenter;
        private readonly OpenVisionShellHostTestAdapter testAdapter;
        private readonly ShellDockingTestFacade dockingTestFacade;
        private readonly ShellLayerTestFacade layerTestFacade;
        private readonly ShellToolTestFacade toolTestFacade;
        private readonly OpenVisionShellHostViewTestSurface testSurface;
        private readonly OpenVisionShellHostMenuPresenter menuPresenter;
        private readonly OpenVisionShellHostChromeController chromeController;
        private readonly OpenVisionDockedToolInspectorController dockedToolInspectorController;
        private readonly OpenVisionDockedDocumentWorkspaceController dockedDocumentWorkspaceController;
        private OpenVisionShellHostToolWindowLifecycleController toolWindowLifecycleController;
        private readonly OpenVisionShellHostDirectRunPresenter directRunPresenter;
        private readonly OpenVisionShellHostToolPrewarmController toolPrewarmController;
        private readonly OpenVisionShellHostLifecycleController lifecycle = new OpenVisionShellHostLifecycleController();
        private readonly OpenVisionShellHostRefreshCoordinator refreshCoordinator = new OpenVisionShellHostRefreshCoordinator();
        private readonly OpenVisionNativeToolPrewarmService nativeToolPrewarmService;
        private readonly OpenVisionZoomableImageController workspaceFallbackZoomController;
        private readonly OpenVisionShellHostSessionState sessionState = new OpenVisionShellHostSessionState();
        private readonly OpenVisionShellHostSessionController sessionController;
        private readonly OpenVisionRecipeLlmBrowserAssistController llmBrowserAssistController = new OpenVisionRecipeLlmBrowserAssistController();
        private readonly OpenVisionTcpIntegrationController tcpIntegrationController;
        private readonly OpenVisionShellHostRecipePanelDragController recipePanelDragController;
        private VisionToolPropertyGridHost recipeStepPropertyGridHostController;
        private bool isRestoringRecipeManagerAfterCanceledClose;
        private bool failNextRecipeStepEditCommitForTest;
        private bool disposed;

        #endregion

        #region Constructors

        public OpenVisionShellHostView()
            : this(ApplicationRuntimeContext.CreateDefault())
        {
        }

        public OpenVisionShellHostView(ApplicationRuntimeContext runtimeContext)
        {
            // Composition order: runtime owners must exist before visual wiring.
            this.runtimeContext = runtimeContext ?? throw new ArgumentNullException(nameof(runtimeContext));
            displayManager = runtimeContext.DisplayManager ?? throw new ArgumentNullException(nameof(runtimeContext.DisplayManager));
            recipeContextStore = new OpenVisionRecipeContextStore(
                ResolveRuntimeRecipeName,
                () => layerListPresenter?.ActiveLayerTitle ?? "Main");
            PropertyGridEditorFactory.SetRuntimeContext(() => displayManager);
            PropertyGridEditorFactory.SetRecipeNameContext(() => recipeContextStore.CurrentRecipeName);
            OpenVisionNativeToolPropertySessionStore.SetRepositoryContext(() => this.runtimeContext.Global?.VisionTools);
            OpenVisionNativeToolSettingsStore.ResetContext();
            viewModel = OpenVisionShellPreviewViewModel.CreatePreview();
            documentController = new OpenVisionShellHostDocumentController(
                (sender, e) => toolWindowLifecycleController?.OnNativeDocumentLayerStateChanged(sender, e));
            PropertyGridEditorFactory.SetSourceLayerContext(() => documentController.ActiveNativeDocument?.RouteInputLayerName ?? string.Empty);
            layerListPresenter = new OpenVisionShellHostLayerListPresenter(displayManager);
            layerDetailPresenter = new OpenVisionShellHostLayerDetailPresenter(displayManager);
            workspacePreviewController = new OpenVisionShellHostWorkspacePreviewController(displayManager);
            layerViewerController = new OpenVisionShellHostLayerViewerController(
                displayManager,
                layerDetailPresenter,
                layerViewerWindows,
                () => Window.GetWindow(this));

            // Visual presenters and window adapters are created after XAML fields exist.
            InitializeComponent();
            recipePanelDragController = new OpenVisionShellHostRecipePanelDragController(
                rootShellHost,
                recipeManagerPanel,
                recipeManagerTitleBar,
                recipeManagerPanelTransform);
            tcpIntegrationController = new OpenVisionTcpIntegrationController(Dispatcher);
            recipeContextPresenter = new OpenVisionShellHostRecipeContextPresenter(
                txtHostRecipeContextLabel,
                txtHostRecipeContext,
                () => recipeContextStore.Current);
            recipeContextPresenter.Refresh();
            directRunPresenter = new OpenVisionShellHostDirectRunPresenter(
                this,
                directResultPanel,
                directResultBadge,
                txtHostDirectBadge,
                txtHostSelectedTool,
                txtHostSelectedRoute,
                txtHostDirectStatus,
                () => viewModel.SelectedDirectRunText);
            menuPresenter = new OpenVisionShellHostMenuPresenter(
                miWorkspaceLoadImage,
                miWorkspaceOpenLayerWindow,
                miWorkspaceFitImage,
                miWorkspaceSaveImage,
                miWorkspaceDockLayer,
                miWorkspaceClearDockedLayers,
                txtHostWorkspaceEmptyTitle,
                txtHostWorkspaceEmptyDetail,
                txtWorkspaceEmptyStepLoadTitle,
                txtWorkspaceEmptyStepLoadDetail,
                txtWorkspaceEmptyStepSelectTitle,
                txtWorkspaceEmptyStepSelectDetail,
                txtWorkspaceEmptyStepPipelineTitle,
                txtWorkspaceEmptyStepPipelineDetail,
                txtWorkspaceEmptyStepPreviewTitle,
                txtWorkspaceEmptyStepPreviewDetail,
                txtWorkspaceLoadImageButtonText,
                txtWorkspaceEmptySampleButtonText,
                txtWorkspaceEmptyGuideButtonText,
                txtWorkspaceEmptyPipelineButtonText,
                txtWorkspaceEmptyLogHint,
                txtOpenSelectedLayerWindowButton,
                 txtDockSelectedLayerButton,
                 btnFloatDockedTool,
                 btnCloseDockedTool,
                 btnFloatDockedDocument,
                 btnCloseDockedDocument);
            chromeController = new OpenVisionShellHostChromeController(
                menuPresenter,
                toolRailPresenter,
                directRunPresenter,
                txtHostActiveFormType,
                toolRailColumn,
                toolRailBorder,
                btnToggleToolRail,
                toolRailScroll,
                toolRailToggleContent,
                iconToolRailToggle,
                txtToolRailToggle,
                () => displayManager.TackTime,
                () => nativePreviewRouteCoordinator?.RefreshRouteText(),
                refreshCoordinator.RefreshLayerActionButtons);
            dockedLayerWorkspaceComposition = OpenVisionDockedLayerWorkspaceRuntimeFactory.CreateComposition(
                new OpenVisionDockedLayerWorkspaceRuntimeOptions(
                    dockedWorkspaceView,
                    displayManager,
                    refreshCoordinator.CreateWorkspaceLayerTitleSnapshot,
                    () => layerListPresenter.ActiveLayerTitle,
                    layerViewerController.BuildStatus,
                    layerViewerController.CanOpen,
                    () => sessionState.Loaded,
                    refreshCoordinator.ApplyDockedLayerRefreshResult,
                    refreshCoordinator.RefreshLayerActionButtons,
                    ActivateDockedLayer));
            dockedLayerWorkspaceComposition.Attach(refreshCoordinator, lifecycle);
            nativePreviewRouteCoordinator = new OpenVisionNativePreviewRouteCoordinator(
                displayManager,
                () => documentController.ActiveNativeDocument,
                () => documentController.ActivePipelineReviewDocument,
                () => viewModel.SelectedRouteText,
                chromeController.SetDirectRouteText,
                refreshCoordinator.RefreshHostLayerRows,
                refreshCoordinator.RefreshHostSelectedLayerDetail,
                layerTitle => dockedLayerWorkspaceComposition.Commands.ActivateLayerDocument(layerTitle));
            nativeToolPrewarmService = new OpenVisionNativeToolPrewarmService(
                Dispatcher,
                documentController.NativeToolDocuments,
                displayManager,
                () => sessionState.Loaded,
                OpenVisionShellHostToolWindowController.WarmPrewarmedNativeToolDocument);
            dockedToolInspectorController = new OpenVisionDockedToolInspectorController(
                dockedToolContentHost,
                txtDockedToolTitle,
                toolInspectorPanel,
                toolInspectorSplitter,
                toolInspectorSplitterColumn,
                toolInspectorColumn,
                floatingToolWindowHost.CloseSilently);
            dockedDocumentWorkspaceController = new OpenVisionDockedDocumentWorkspaceController(
                dockedDocumentContentHost,
                txtDockedDocumentTitle,
                documentWorkspacePanel,
                () => floatingToolWindowHost.CloseSilently());
            toolWindowLifecycleController = new OpenVisionShellHostToolWindowLifecycleController(
                documentController,
                floatingToolWindowHost,
                dockedToolInspectorController,
                dockedDocumentWorkspaceController,
                chromeController.SetDirectRunSucceeded,
                chromeController.SetActiveDocumentText,
                refreshCoordinator.RefreshHostLayerRows,
                hasPreviewResult => nativePreviewRouteCoordinator.RefreshAfterLayerStateChanged(hasPreviewResult),
                nativePreviewRouteCoordinator.RefreshLastVisibleNativeOutputWorkspacePreview);
            statePresenter = new OpenVisionShellHostStatePresenter(
                displayManager,
                layerListPresenter,
                documentController,
                floatingToolWindowHost,
                nativeToolPrewarmService,
                workspacePreviewController,
                layerViewerWindows);
            toolWindowController = new OpenVisionShellHostToolWindowController(
                displayManager,
                documentController,
                layerViewerController,
                floatingToolWindowHost,
                toolWindowLifecycleController,
                () => Window.GetWindow(this),
                () => recipeContextStore.Current,
                chromeController.SetDirectRunPending,
                chromeController.SetDirectRunSucceeded,
                chromeController.SetActiveDocumentText,
                refreshCoordinator.RefreshHostLayerRows,
                OpenWorkspaceSampleByNameFromReview,
                ReturnToRecipeManagerFromPipelineReview,
                OpenPipelineStepEditorFromPipelineReview,
                OpenLearnForPipelineReviewTool);
            toolPrewarmController = new OpenVisionShellHostToolPrewarmController(
                Dispatcher,
                nativeToolPrewarmService,
                floatingToolWindowHost,
                toolWindowController.PrewarmPipelineReview,
                () => sessionState.Loaded && !sessionState.Disposed,
                () => viewModel.SelectedItem?.Menu,
                () => Window.GetWindow(this));
            busyPresenter = new OpenVisionShellHostBusyPresenter(
                shellBusyOverlay,
                txtShellBusyTitle,
                txtShellBusyDetail);
            toolSelectionController = new OpenVisionShellHostToolSelectionController(
                viewModel,
                toolWindowController,
                toolPrewarmController,
                busyPresenter,
                txtToolOpenTimingDiagnostics,
                () => sessionState.Loaded);

            // Workspace and Recipe owners are connected after tool-window dependencies.
            recipeController = new OpenVisionShellHostRecipeController(
                this.runtimeContext,
                displayManager,
                documentController,
                toolPrewarmController,
                toolWindowLifecycleController,
                chromeController.SetActiveDocumentText,
                refreshCoordinator.RefreshHostLayerRows,
                refreshCoordinator.RefreshHostSelectedLayerDetail,
                chromeController.RefreshDirectRouteText);
            workspaceFallbackZoomController = new OpenVisionZoomableImageController(
                hostWorkspaceImageSurface,
                hostWorkspaceFallbackImage,
                refreshCoordinator.ApplyWorkspacePointerStatus);
            layerWorkspacePresenter = new OpenVisionShellHostLayerWorkspacePresenter(
                workspacePreviewController,
                workspaceFallbackZoomController,
                txtHostSelectedLayerTitle,
                txtHostSelectedLayerMeta,
                txtHostSelectedLayerRoute,
                imgHostSelectedLayerPreview,
                singleWorkspaceView,
                dockedWorkspaceView,
                hostWorkspaceCanvas,
                workspaceEmptyOverlay,
                workspaceLayerInfoOverlay,
                txtHostTopLayerNameEditor,
                txtHostWorkspaceLayerTitle,
                txtHostWorkspaceLayerMeta,
                txtHostWorkspaceStatus,
                txtHostWorkspaceCoordinates,
                txtHostWorkspacePixel,
                hostWorkspaceFallbackImage,
                btnOpenSelectedLayerWindow,
                btnDockSelectedLayer,
                btnClearDockedLayers);
            refreshCoordinator.AttachLayerWorkspacePresenter(layerWorkspacePresenter);
            layerRefreshController = new OpenVisionShellHostLayerRefreshController(
                Dispatcher,
                hostLayerRowsList,
                hostLayerRowsScrollViewer,
                layerListPresenter,
                layerDetailPresenter,
                layerWorkspacePresenter,
                () => layerListPresenter.GetSelectedLayerTitle(hostLayerRowsList.SelectedIndex),
                dockedLayerWorkspaceComposition.Synchronization,
                layerViewerController,
                (layerNames, selectedLayer) =>
                {
                    viewModel.SetLayerOptions(layerNames, selectedLayer);
                    RefreshToolReadiness();
                });
            refreshCoordinator.AttachLayerRefreshController(layerRefreshController);
            workspaceImageController = new OpenVisionShellHostWorkspaceImageController(
                runtimeContext,
                displayManager,
                documentController,
                chromeController.SetDirectRunPending,
                refreshCoordinator.RefreshHostLayerRows,
                refreshCoordinator.RefreshHostCommandCanExecute,
                chromeController.RefreshDirectRouteText);
            sampleWorkflowPresenter = new OpenVisionShellHostSampleWorkflowPresenter(
                workspaceSampleWorkflowOverlay,
                txtWorkspaceSampleWorkflowTitle,
                txtWorkspaceSampleWorkflowMeta,
                txtWorkspaceSampleWorkflowDetail,
                btnWorkspaceSampleCounterpart,
                txtWorkspaceSampleCounterpartButtonText,
                txtWorkspaceSamplePipelineButtonText,
                txtWorkspaceSampleFirstStepButtonText,
                () => recipeContextStore.Current);
            mainActionPresenter = new OpenVisionShellHostMainActionPresenter(
                workspaceMainActionOverlay,
                txtWorkspaceMainActionTitle,
                txtWorkspaceMainActionDetail,
                txtWorkspaceMainActionMeta,
                txtWorkspaceMainActionThresholdButtonText,
                txtWorkspaceMainActionMatchingButtonText,
                txtWorkspaceMainActionLineButtonText);
            commandController = new OpenVisionShellHostCommandController(
                () => Window.GetWindow(this),
                workspaceImageController.LoadImage,
                workspacePreviewController,
                workspaceFallbackZoomController,
                () => WorkspaceLayerTitle,
                () => recipeContextStore.Current,
                (sampleName, pipelineName) =>
                    RecipeCommands?.PrepareWorkspaceSampleContext(
                        sampleName,
                        pipelineName) != false,
                (sampleName, pipelineName) =>
                {
                    mainActionPresenter.Hide();
                    RefreshRecipeContext();
                    RecipeCommands?.SynchronizeWorkspaceSampleContext(
                        sampleName,
                        pipelineName);
                    sampleWorkflowPresenter.ShowForActiveSample();
                    chromeController.SetWorkspaceSampleReadyStatus();
                    WorkspaceCommands?.RefreshCanExecute();
                },
                () =>
                {
                    sampleWorkflowPresenter.Hide();
                    RefreshRecipeContext();
                    mainActionPresenter.ShowImageReady(txtHostWorkspaceLayerTitle?.Text, txtHostWorkspaceLayerMeta?.Text);
                    chromeController.SetWorkspaceImageReadyStatus();
                    WorkspaceCommands?.RefreshCanExecute();
                },
                initialWorkspaceImagePath: runtimeContext.Global.System.LastWorkspaceImagePath,
                rememberWorkspaceImagePath: workspaceImageController.RememberWorkspaceImagePath);
            learnWindowController = new OpenVisionShellHostLearnWindowController(
                () => Window.GetWindow(this),
                commandController.PromptAndOpenRunnableSample,
                SelectToolMenu);
            WorkspaceCommands = new OpenVisionShellHostWorkspaceCommandSurface(
                commandController,
                workspacePreviewController,
                SelectToolMenu,
                () => sampleWorkflowPresenter.FirstStepMenu,
                () => sampleWorkflowPresenter.CounterpartSampleName,
                ApplyActiveSampleFirstStepParameters);
            OpenVisionRecipeRunEvidenceViewerController runEvidenceViewerController =
                new OpenVisionRecipeRunEvidenceViewerController(
                    () => Window.GetWindow(this),
                    layerViewerWindows);
            recipeDialogAdapter = new RecipeDialogAdapter(() => Window.GetWindow(this));
            RecipeCommands = new OpenVisionShellHostRecipeCommandSurface(
                ResolveRuntimeRecipeName,
                recipeController.SwitchRuntimeRecipe,
                () =>
                {
                    RefreshRecipeContext();
                    WorkspaceCommands?.RefreshCanExecute();
                },
                recipeDialogAdapter.ConfirmDeleteRecipe,
                recipeDialogAdapter.ConfirmDeletePipeline,
                recipeDialogAdapter.SelectImportPipelineXmlPath,
                recipeDialogAdapter.SelectExportPipelineXmlPath,
                recipeDialogAdapter.SelectExportRecipeReviewBundlePath,
                BuildRecipeLayerCard,
                layerTitle => layerActivationController?.Activate(layerTitle) == true,
                (layerTitle, imagePath) => layerManagementController?.LoadImageIntoLayer(layerTitle, imagePath) == true,
                SelectToolMenu,
                CommitPendingRecipeStepEdit,
                OpenRecipeLlmXmlReview,
                recipeDialogAdapter.SelectValidationSetImagePaths,
                recipeDialogAdapter.SelectValidationSetFolderPath,
                recipeDialogAdapter.SelectValidationSetReplacementImagePath,
                recipeDialogAdapter.ConfirmDeleteValidationSet,
                OpenRecipePipelineReview,
                evidence => runEvidenceViewerController.Open(evidence),
                OpenRecipeImageListValidation,
                recipeDialogAdapter.DecidePendingEdit,
                confirmQualifiedSnapshotLifecycle:
                    recipeDialogAdapter.ConfirmQualifiedSnapshotLifecycle,
                openQualifiedSnapshotEvidence:
                    recipeDialogAdapter.OpenQualifiedSnapshotEvidence,
                openPipelineXmlSteps:
                    () => tabRecipePipelineXmlSteps.IsSelected = true,
                saveRecipe:
                    recipeController.SaveRuntimeRecipeTools,
                waitForRecipeSwitchCompletion:
                    () => recipeController.RecipePreparationTask,
                selectLocatorEvidencePacketPath:
                    recipeDialogAdapter.SelectLocatorEvidencePacketPath,
                selectLocatorEvidenceReviewDecisionPath:
                    recipeDialogAdapter.SelectLocatorEvidenceReviewDecisionPath,
                copyTextToClipboard:
                    text => System.Windows.Clipboard.SetText(text ?? string.Empty),
                clipboardContainsText:
                    () => System.Windows.Clipboard.ContainsText(),
                readClipboardText:
                    () => System.Windows.Clipboard.GetText(),
                yieldToUi:
                    async () => await System.Windows.Threading.Dispatcher.Yield(
                        System.Windows.Threading.DispatcherPriority.Background),
                flushUi:
                    () =>
                    {
                        System.Windows.Threading.Dispatcher dispatcher =
                            System.Windows.Application.Current?.Dispatcher;
                        if (dispatcher?.CheckAccess() == true)
                        {
                            dispatcher.Invoke(
                                System.Windows.Threading.DispatcherPriority.Render,
                                new Action(() => { }));
                        }
                    });
            InputBindings.Add(new KeyBinding(
                RecipeCommands.SaveRecipeCommand,
                new KeyGesture(Key.S, ModifierKeys.Control)));
            AttachRecipeStepPropertyGridHost();
            ChromeCommands = new OpenVisionShellHostChromeCommandSurface(
                () => IsToolRailCompact = !IsToolRailCompact,
                commandController,
                learnWindowController,
                toolWindowLifecycleController,
                toolWindowController,
                OpenGuidedSetupForTool,
                () => tcpIntegrationController.Show(Window.GetWindow(this)));

            // Command surfaces, session state, layer routing, and test adapters are wired last.
            sessionController = new OpenVisionShellHostSessionController(
                sessionState,
                dockedLayerWorkspaceComposition.Session,
                toolPrewarmController,
                lifecycle,
                viewModel,
                workspacePreviewController,
                workspaceFallbackZoomController,
                toolWindowLifecycleController,
                value => DataContext = value,
                refreshCoordinator.RefreshHostLayerRows,
                layerViewerController.CloseAll,
                ReleaseDisplayManager);
            SessionCommands = new OpenVisionShellHostSessionCommandSurface(
                sessionController,
                Dispose,
                () => Dispatcher);
            hostWorkspaceCanvas.DataContext = workspacePreviewController.CanvasViewModel;
            DataContext = viewModel;
            chromeController.ApplyLocalization(IsToolRailCompact);
            mainActionPresenter.ApplyLocalization();
            ApplyShellLogLocalization();

            hostLayerRowsList.ItemsSource = layerListPresenter.Rows;
            layerActivationController = new OpenVisionShellHostLayerActivationController(
                displayManager,
                refreshCoordinator.RefreshHostSelectedLayerDetail,
                refreshCoordinator.RefreshHostLayerRows);
            layerSelectionController = new OpenVisionShellHostLayerSelectionController(
                hostLayerRowsList,
                layerListPresenter,
                layerActivationController);
            layerManagementController = new OpenVisionShellHostLayerManagementController(
                displayManager,
                documentController,
                () => Window.GetWindow(this),
                refreshCoordinator.RefreshHostSelectedLayerDetail,
                refreshCoordinator.RefreshHostLayerRows,
                refreshCoordinator.RefreshDockedLayerViews,
                chromeController.RefreshDirectRouteText,
                 workspaceImageController.RememberWorkspaceImagePath);
            LayerCommands = new OpenVisionShellHostLayerCommandSurface(
                layerSelectionController,
                layerViewerController,
                layerManagementController,
                dockedLayerWorkspaceComposition.Commands,
                () => WorkspaceLayerTitle);
            layerInteractionController = new OpenVisionShellHostLayerInteractionController(
                hostLayerRowsList,
                layerListPresenter,
                LayerDragDataFormat,
                LayerCommands.CanDockLayerDocument,
                LayerCommands.DockLayerDocument,
                SetWorkspaceDropOverlay);
            CommandSurfaces = new OpenVisionShellHostCommandSurfaces(
                LayerCommands,
                WorkspaceCommands);
            refreshCoordinator.AttachCommandSurfaces(
                LayerCommands,
                WorkspaceCommands);
            chromeController.SetWorkspaceEmptyStatus();
            testAdapter = new OpenVisionShellHostTestAdapter(
                hostLayerRowsList,
                layerListPresenter,
                refreshCoordinator.RefreshHostLayerRows);
            dockingTestFacade = dockedLayerWorkspaceComposition.CreateTestFacade(UpdateLayout);
            layerTestFacade = new ShellLayerTestFacade(
                displayManager,
                statePresenter,
                layerListPresenter,
                layerViewerController,
                layerActivationController,
                testAdapter,
                refreshCoordinator,
                chromeController,
                workspacePreviewController,
                workspaceImageController,
                commandController,
                workspaceFallbackZoomController,
                new ShellLayerTestFacadeBindings
                {
                    SelectedHostLayerTitleText = () => txtHostSelectedLayerTitle?.Text ?? string.Empty,
                    SelectedHostLayerMetaText = () => txtHostSelectedLayerMeta?.Text ?? string.Empty,
                    HasSelectedHostLayerPreview = () => imgHostSelectedLayerPreview?.Source != null,
                    IsSingleWorkspaceVisible = () => singleWorkspaceView?.Visibility == Visibility.Visible,
                    IsWorkspaceLayerDropEnabled = () => wpfLayerWorkspace?.AllowDrop == true,
                    HasWorkspaceDropOverlay = () => workspaceDropOverlay != null,
                    IsWorkspaceDropOverlayVisible = () => workspaceDropOverlay?.Visibility == Visibility.Visible,
                    IsWorkspaceDropOverlayHitTestSafe = () => workspaceDropOverlay?.IsHitTestVisible == false,
                    IsWorkspaceEmptyPromptVisible = () => workspaceEmptyOverlay?.Visibility == Visibility.Visible,
                    WorkspaceCoordinatesText = () => txtHostWorkspaceCoordinates?.Text ?? string.Empty,
                    WorkspacePixelText = () => txtHostWorkspacePixel?.Text ?? string.Empty,
                    WorkspaceEmptyTitleText = () => txtHostWorkspaceEmptyTitle?.Text ?? string.Empty,
                    WorkspaceEmptyDetailText = () => txtHostWorkspaceEmptyDetail?.Text ?? string.Empty,
                    WorkspaceLayerTitleText = () => txtHostWorkspaceLayerTitle?.Text ?? string.Empty,
                    WorkspaceLayerMetaText = () => txtHostWorkspaceLayerMeta?.Text ?? string.Empty,
                    WorkspaceLoadImageMenuText = () => Convert.ToString(miWorkspaceLoadImage?.Header) ?? string.Empty,
                    WorkspaceLoadImageButtonText = () => txtWorkspaceLoadImageButtonText?.Text ?? string.Empty,
                    HasWorkspaceLoadImageMenu = () => miWorkspaceLoadImage != null,
                    IsWorkspaceLoadImageIntoLayerMenuVisible = () => miWorkspaceLoadImageIntoLayer?.Visibility == Visibility.Visible,
                    WorkspaceImageReady = () =>
                    {
                        mainActionPresenter.ShowImageReady(txtHostWorkspaceLayerTitle?.Text, txtHostWorkspaceLayerMeta?.Text);
                        chromeController.SetWorkspaceImageReadyStatus();
                    },
                    WorkspaceImageSurfaceWidth = () => hostWorkspaceImageSurface?.ActualWidth ?? 0D,
                    WorkspaceImageSurfaceHeight = () => hostWorkspaceImageSurface?.ActualHeight ?? 0D
                });
            toolTestFacade = new ShellToolTestFacade(
                viewModel,
                statePresenter,
                documentController,
                toolWindowController,
                toolWindowLifecycleController,
                floatingToolWindowHost,
                dockedToolInspectorController,
                dockedDocumentWorkspaceController,
                chromeController,
                refreshCoordinator,
                new ShellToolTestFacadeBindings
                {
                    IsShellLoaded = () => sessionState.Loaded,
                    IsToolRailCompact = () => IsToolRailCompact,
                    SetToolRailCompact = value => IsToolRailCompact = value,
                    ToolRailWidth = () => toolRailColumn?.ActualWidth ?? 0D,
                    IsToolRailNavigationVisible = () => toolRailScroll?.Visibility == Visibility.Visible,
                    IsToolRailCompactLabelHidden = () => IsToolRailCompact && txtToolRailToggle?.Visibility != Visibility.Visible,
                    DirectResultBadgeText = () => txtHostDirectBadge?.Text ?? string.Empty,
                    DirectResultTitleText = () => txtHostSelectedTool?.Text ?? string.Empty,
                    DirectResultStatusText = () => txtHostDirectStatus?.Text ?? string.Empty,
                    DirectResultRouteText = () => txtHostSelectedRoute?.Text ?? string.Empty,
                    DockedToolTitleText = () => txtDockedToolTitle?.Text ?? string.Empty,
                    IsDockedToolFloatButtonVisible = () => btnFloatDockedTool?.IsVisible == true,
                    IsDockedToolCloseButtonVisible = () => btnCloseDockedTool?.IsVisible == true,
                    DockedToolFloatButtonWidth = () => btnFloatDockedTool?.ActualWidth ?? 0D,
                    DockedToolCloseButtonWidth = () => btnCloseDockedTool?.ActualWidth ?? 0D,
                    DockedToolFloatButtonToolTipText = () => btnFloatDockedTool?.ToolTip?.ToString() ?? string.Empty,
                    DockedToolCloseButtonToolTipText = () => btnCloseDockedTool?.ToolTip?.ToString() ?? string.Empty,
                    DockedToolInspectorWidth = () => toolInspectorColumn?.ActualWidth ?? 0D,
                    SetDockedToolInspectorWidth = SetDockedToolInspectorWidthForTestCore
                });
            testSurface = new OpenVisionShellHostViewTestSurface(
                new OpenVisionShellHostViewTestSurfaceBindings
                {
                    RuntimeContext = runtimeContext,
                    DisplayManager = displayManager,
                    ViewModel = viewModel,
                    DocumentController = documentController,
                    DockedLayerWorkspaceComposition = dockedLayerWorkspaceComposition,
                    LayerManagementController = layerManagementController,
                    DockingTestFacade = dockingTestFacade,
                    LayerTestFacade = layerTestFacade,
                    ToolTestFacade = toolTestFacade,
                    TcpIntegrationController = tcpIntegrationController,
                    CommandController = commandController,
                    SampleWorkflowPresenter = sampleWorkflowPresenter,
                    MainActionPresenter = mainActionPresenter,
                    BusyPresenter = busyPresenter,
                    RecipeContextStore = recipeContextStore,
                    ShellLogToggle = btnShellLogToggle,
                    ShellLogToggleText = txtShellLogToggle,
                    RecipeManagerToggle = btnHostRecipeManager,
                    RecipeManagerPanelTransform = recipeManagerPanelTransform,
                    RecipeDialogAdapter = recipeDialogAdapter,
                    RecipeCommandsProvider = () => RecipeCommands,
                    WorkspaceCommandsProvider = () => WorkspaceCommands,
                    LayerCommandsProvider = () => LayerCommands,
                    GetOwnerWindow = () => Window.GetWindow(this),
                    ActivateDockedLayer = ActivateDockedLayer,
                    RefreshRecipeContext = RefreshRecipeContext,
                    ResolveRecipeName = ResolveRecipeName,
                    SetRecipeManagerPanelOffset = recipePanelDragController.SetOffset,
                    SetShellLogExpanded = SetShellLogExpanded,
                    GetFailNextRecipeStepEditCommitForTest = () => failNextRecipeStepEditCommitForTest,
                    SetFailNextRecipeStepEditCommitForTest = value => failNextRecipeStepEditCommitForTest = value
                });

            // Event subscriptions and release callbacks form the final lifetime boundary.
            wpfLayerWorkspace.AllowDrop = true;
            lifecycle.Track(
                () => hostLayerRowsList.PreviewMouseLeftButtonDown += layerInteractionController.HandleLayerTabPreviewMouseLeftButtonDown,
                () => hostLayerRowsList.PreviewMouseLeftButtonDown -= layerInteractionController.HandleLayerTabPreviewMouseLeftButtonDown);
            lifecycle.Track(
                () => hostLayerRowsList.PreviewMouseMove += layerInteractionController.HandleLayerTabPreviewMouseMove,
                () => hostLayerRowsList.PreviewMouseMove -= layerInteractionController.HandleLayerTabPreviewMouseMove);
            lifecycle.Track(
                () => wpfLayerWorkspace.PreviewDragOver += layerInteractionController.HandleWorkspacePreviewDragOver,
                () => wpfLayerWorkspace.PreviewDragOver -= layerInteractionController.HandleWorkspacePreviewDragOver);
            lifecycle.Track(
                () => wpfLayerWorkspace.PreviewDragLeave += layerInteractionController.HandleWorkspacePreviewDragLeave,
                () => wpfLayerWorkspace.PreviewDragLeave -= layerInteractionController.HandleWorkspacePreviewDragLeave);
            lifecycle.Track(
                () => wpfLayerWorkspace.PreviewDrop += layerInteractionController.HandleWorkspacePreviewDrop,
                () => wpfLayerWorkspace.PreviewDrop -= layerInteractionController.HandleWorkspacePreviewDrop);
            lifecycle.Track(
                () => floatingToolWindowHost.ClosedByUser += toolWindowLifecycleController.OnFloatingToolWindowClosedByUser,
                () => floatingToolWindowHost.ClosedByUser -= toolWindowLifecycleController.OnFloatingToolWindowClosedByUser);
            lifecycle.Track(
                () => floatingToolWindowHost.DockRequested += toolWindowLifecycleController.OnFloatingToolWindowDockRequested,
                () => floatingToolWindowHost.DockRequested -= toolWindowLifecycleController.OnFloatingToolWindowDockRequested);
            chromeController.ApplyToolRailCompactState(IsToolRailCompact);

            lifecycle.Track(() => OpenVisionLanguageService.LanguageChanged += OnLanguageChanged, () => OpenVisionLanguageService.LanguageChanged -= OnLanguageChanged);
            lifecycle.Track(
                () => recipeContextStore.ContextChanged += OnRecipeContextChanged,
                () => recipeContextStore.ContextChanged -= OnRecipeContextChanged);
            lifecycle.Track(
                () => viewModel.PropertyChanged += toolSelectionController.OnViewModelPropertyChanged,
                () => viewModel.PropertyChanged -= toolSelectionController.OnViewModelPropertyChanged);
            lifecycle.Track(
                () => this.runtimeContext.Global.Recipe.EventChangedRecipe += OnRuntimeRecipeChanged,
                () => this.runtimeContext.Global.Recipe.EventChangedRecipe -= OnRuntimeRecipeChanged);
            lifecycle.Track(
                () => OpenVisionNativeToolPropertySessionStore.PropertySaved += OnNativeToolPropertySaved,
                () => OpenVisionNativeToolPropertySessionStore.PropertySaved -= OnNativeToolPropertySaved);
            lifecycle.Track(
                () => OpenVisionNativeToolSettingsStore.SettingsSaved += OnNativeToolSettingsSaved,
                () => OpenVisionNativeToolSettingsStore.SettingsSaved -= OnNativeToolSettingsSaved);
            lifecycle.Track(null, tcpIntegrationController.Dispose);
            lifecycle.Track(null, recipePanelDragController.Dispose);
        }

        #endregion

        #region Public View Contract

        public bool IsToolRailCompact
        {
            get => (bool)GetValue(IsToolRailCompactProperty);
            set => SetValue(IsToolRailCompactProperty, value);
        }

        public OpenVisionShellHostLayerCommandSurface LayerCommands
        {
            get => (OpenVisionShellHostLayerCommandSurface)GetValue(LayerCommandsProperty);
            private set => SetValue(LayerCommandsProperty, value);
        }

        public OpenVisionShellHostWorkspaceCommandSurface WorkspaceCommands
        {
            get => (OpenVisionShellHostWorkspaceCommandSurface)GetValue(WorkspaceCommandsProperty);
            private set => SetValue(WorkspaceCommandsProperty, value);
        }

        public OpenVisionShellHostRecipeCommandSurface RecipeCommands
        {
            get => (OpenVisionShellHostRecipeCommandSurface)GetValue(RecipeCommandsProperty);
            private set => SetValue(RecipeCommandsProperty, value);
        }

        public Task StartupPreparationTask => sessionController.StartupPreparationTask;

        private void ActivateDockedLayer(string layerTitle)
        {
            if (string.Equals(layerListPresenter.ActiveLayerTitle, layerTitle, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            layerActivationController?.Activate(layerTitle);
        }

        public OpenVisionShellHostCommandSurfaces CommandSurfaces
        {
            get => (OpenVisionShellHostCommandSurfaces)GetValue(CommandSurfacesProperty);
            private set => SetValue(CommandSurfacesProperty, value);
        }

        public OpenVisionShellHostChromeCommandSurface ChromeCommands
        {
            get => (OpenVisionShellHostChromeCommandSurface)GetValue(ChromeCommandsProperty);
            private set => SetValue(ChromeCommandsProperty, value);
        }

        public OpenVisionShellHostSessionCommandSurface SessionCommands
        {
            get => (OpenVisionShellHostSessionCommandSurface)GetValue(SessionCommandsProperty);
            private set => SetValue(SessionCommandsProperty, value);
        }

        #endregion

        #region Lifetime

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            disposed = true;
            recipeLlmBrowserAssistWebView?.Dispose();
            llmBrowserAssistController.Dispose();
            hostWorkspaceCanvas?.Dispose();

            if (!sessionController.DisposeSession())
            {
                return;
            }

            GC.SuppressFinalize(this);
        }

        private void ReleaseDisplayManager()
        {
            InputBindings.Clear();
            ClearValue(LayerCommandsProperty);
            ClearValue(WorkspaceCommandsProperty);
            ClearValue(RecipeCommandsProperty);
            ClearValue(CommandSurfacesProperty);
            ClearValue(ChromeCommandsProperty);
            ClearValue(SessionCommandsProperty);
            // The owning Window detaches this view after disposal. Clearing the
            // nested content here can make WPF tear down an active template or
            // popup animation with a null animation destination.
            // ApplicationRuntimeContext.CreateDefault() supplies the process-level
            // DisplayManagerService.Default. The View owns injected per-session
            // managers, but it must not dispose the shared application instance.
            if (displayManager is IDisposable disposable
                && !ReferenceEquals(displayManager, DisplayManagerService.Default))
            {
                disposable.Dispose();
            }
        }

        #endregion

        #endregion

        #region Interactions

        #region Recipe Property Grid

        private void AttachRecipeStepPropertyGridHost()
        {
            if (recipeStepPropertyGridHost == null || RecipeCommands == null)
            {
                return;
            }

            recipeStepPropertyGridHostController = VisionToolPropertyGridHost.Attach(
                recipeStepPropertyGridHost,
                RecipeCommands.SelectedStepEditObject,
                (_, __) => RecipeCommands?.MarkSelectedStepEditDirty());
            recipeStepPropertyGridHostController.SetCompactDensity(true);
            recipeStepPropertyGridHostController.SetThemeVariant(
                System.Windows.Controls.WpfPropertyGrid.PropertyGridThemeVariant.Dark);
            lifecycle.Track(
                () => RecipeCommands.PropertyChanged += OnRecipeCommandsPropertyChanged,
                () => RecipeCommands.PropertyChanged -= OnRecipeCommandsPropertyChanged);
            lifecycle.Track(
                () => { },
                () =>
                {
                    recipeStepPropertyGridHostController?.Dispose();
                    recipeStepPropertyGridHostController = null;
                });
        }

        private void OnRecipeCommandsPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e == null || e.PropertyName == nameof(OpenVisionShellHostRecipeCommandSurface.SelectedStepEditObject))
            {
                recipeStepPropertyGridHostController?.SelectObject(RecipeCommands?.SelectedStepEditObject);
            }
        }

        private bool CommitPendingRecipeStepEdit()
        {
            if (failNextRecipeStepEditCommitForTest)
            {
                failNextRecipeStepEditCommitForTest = false;
                return false;
            }

            return recipeStepPropertyGridHostController?.CommitPendingEdit() ?? true;
        }

        #endregion

        #region Recipe Manager Navigation

        private void HandleRecipeManagerTitleBarMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            recipePanelDragController.HandleMouseLeftButtonDown(sender, e);
        }

        private void HandleRecipeManagerTitleBarMouseMove(object sender, MouseEventArgs e)
        {
            recipePanelDragController.HandleMouseMove(sender, e);
        }

        private void HandleRecipeManagerTitleBarMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            recipePanelDragController.HandleMouseLeftButtonUp(sender, e);
        }

        private void HandleRecipeManagerTitleBarLostMouseCapture(object sender, MouseEventArgs e)
        {
            recipePanelDragController.HandleLostMouseCapture(sender, e);
        }

        private void HandleRecipeManagerCloseClick(object sender, RoutedEventArgs e)
        {
            btnHostRecipeManager.IsChecked = false;
            e.Handled = true;
        }

        private void HandleRecipeManagerOpenUnchecked(object sender, RoutedEventArgs e)
        {
            if (RecipeCommands?.TryCloseRecipeManager() != false)
            {
                return;
            }

            isRestoringRecipeManagerAfterCanceledClose = true;
            try
            {
                btnHostRecipeManager.IsChecked = true;
            }
            finally
            {
                isRestoringRecipeManagerAfterCanceledClose = false;
            }

            e.Handled = true;
        }

        private void HandleRecipeManagerOpenChecked(object sender, RoutedEventArgs e)
        {
            if (isRestoringRecipeManagerAfterCanceledClose)
            {
                return;
            }

            RecipeCommands?.RefreshOptions();

            if (recipeAdvancedReviewToggle != null)
            {
                recipeAdvancedReviewToggle.IsChecked = false;
            }

            if (tabRecipeOverview != null)
            {
                tabRecipeOverview.IsSelected = true;
            }
        }

        private void HandleRecipeAdvancedReviewChecked(object sender, RoutedEventArgs e)
        {
            if (tabRecipePipeline != null)
            {
                tabRecipePipeline.IsSelected = true;
            }
        }

        private void HandleRecipeAdvancedReviewUnchecked(object sender, RoutedEventArgs e)
        {
            if (tabRecipeOverview != null)
            {
                tabRecipeOverview.IsSelected = true;
            }
        }

        private void OpenGuidedSetupForTool(VISION_MENU menu)
        {
            if (!RecipeCommands.SelectGuidedSetupForTool(menu))
            {
                return;
            }

            OpenRecipeGuidedSetup();
        }

        private void HandleOpenRecipeGuidedSetup(object sender, RoutedEventArgs e)
        {
            OpenRecipeGuidedSetup();
        }

        private void OpenRecipeGuidedSetup()
        {
            btnHostRecipeManager.IsChecked = true;
            recipeAdvancedReviewToggle.IsChecked = true;
            tabRecipeGuidedSetup.IsSelected = true;
            recipeGuidedSetupScrollViewer.ScrollToTop();
        }

        private void HandleOpenRecipeImageListValidation(object sender, RoutedEventArgs e)
        {
            OpenRecipeImageListValidation();
        }

        private void OpenRecipeImageListValidation()
        {
            btnHostRecipeManager.IsChecked = true;
            recipeAdvancedReviewToggle.IsChecked = true;
            tabRecipePipeline.IsSelected = true;
            tabRecipePipelineRunHistory.IsSelected = true;
            RecipeCommands?.SelectLocalValidationSetScope();
        }

        private void OpenRecipeLlmXmlReview()
        {
            btnHostRecipeManager.IsChecked = true;
            recipeAdvancedReviewToggle.IsChecked = true;
            tabRecipeLlmXml.IsSelected = true;
        }

        private void HandleOpenRecipeLlmBrowserAssist(object sender, RoutedEventArgs e)
        {
            OpenRecipeLlmBrowserAssist();
        }

        private void OpenRecipeLlmBrowserAssist()
        {
            btnHostRecipeManager.IsChecked = true;
            recipeAdvancedReviewToggle.IsChecked = true;
            tabRecipeLlmBrowserAssist.IsSelected = true;
        }

        private async void HandleOpenRecipeLlmBrowserAssistChatGpt(object sender, RoutedEventArgs e)
        {
            OpenRecipeLlmBrowserAssist();
            recipeLlmBrowserAssistPlaceholder.Visibility = Visibility.Collapsed;
            recipeLlmBrowserAssistWebView.Visibility = Visibility.Visible;

            OpenVisionRecipeLlmBrowserAssistOpenResult result =
                await llmBrowserAssistController.OpenChatGptAsync(recipeLlmBrowserAssistWebView);
            RecipeCommands?.SetLlmBrowserAssistStatus(result);

            if (result != OpenVisionRecipeLlmBrowserAssistOpenResult.EmbeddedChatGptOpened)
            {
                recipeLlmBrowserAssistWebView.Visibility = Visibility.Collapsed;
                recipeLlmBrowserAssistPlaceholder.Visibility = Visibility.Visible;
            }
        }

        private void HandleOpenRecipeLlmBrowserAssistExternal(object sender, RoutedEventArgs e)
        {
            OpenRecipeLlmBrowserAssist();
            RecipeCommands?.SetLlmBrowserAssistStatus(llmBrowserAssistController.OpenChatGptInExternalBrowser());
        }

        #endregion

        #region Pipeline Review Navigation

        private void OpenRecipePipelineReview()
        {
            if (btnHostRecipeManager.IsChecked == true
                && RecipeCommands?.TryCloseRecipeManager() == false)
            {
                return;
            }

            btnHostRecipeManager.IsChecked = false;
            SelectToolMenu(VISION_MENU.Pipeline);
        }

        private void ReturnToRecipeManagerFromPipelineReview()
        {
            if (!toolWindowLifecycleController.SuspendFloatingPipelineReviewForRecipeReturn())
            {
                toolWindowLifecycleController.CloseActiveWpfToolWindowByUser();
            }

            btnHostRecipeManager.IsChecked = true;
        }

        private void OpenPipelineStepEditorFromPipelineReview(string recipeName, string pipelineName, int stepNumber)
        {
            toolWindowLifecycleController.CloseActiveWpfToolWindowByUser();
            btnHostRecipeManager.IsChecked = true;

            if (RecipeCommands?.FocusPipelineStepForEdit(recipeName, pipelineName, stepNumber) != true)
            {
                return;
            }

            recipeAdvancedReviewToggle.IsChecked = true;
            tabRecipePipeline.IsSelected = true;
            tabRecipePipelineXmlSteps.IsSelected = true;

            Dispatcher.BeginInvoke(
                System.Windows.Threading.DispatcherPriority.Background,
                new Action(() =>
                {
                    recipePipelineTabScrollViewer.UpdateLayout();
                    recipePipelineTabScrollViewer.ScrollToEnd();
                    Dispatcher.BeginInvoke(
                        System.Windows.Threading.DispatcherPriority.Background,
                        new Action(() =>
                        {
                            recipePipelineTabScrollViewer.UpdateLayout();
                            recipePipelineTabScrollViewer.ScrollToEnd();
                        }));
                }));
        }

        private void OpenLearnForPipelineReviewTool(string toolType)
        {
            learnWindowController?.OpenLearnForToolType(toolType);
        }

        #endregion

        #region Runtime and Localization

        private void OnLanguageChanged(object sender, EventArgs e)
        {
            chromeController.ApplyLocalization(IsToolRailCompact);
            mainActionPresenter?.ApplyLocalization();
            sampleWorkflowPresenter?.ApplyLocalization();
            if (sampleWorkflowPresenter?.IsVisible == true)
            {
                sampleWorkflowPresenter.ShowForActiveSample();
            }
            recipeContextPresenter?.Refresh();
            RecipeCommands?.RefreshLocalization();
            LayerCommands?.RefreshLocalization();
            ApplyShellLogLocalization();
        }

        private void OnRecipeContextChanged(object sender, EventArgs e)
        {
            recipeContextPresenter?.Refresh();
        }

        private void OnRuntimeRecipeChanged(object sender, EventArgs e)
        {
            RefreshRecipeContext();
            recipeController.OnRecipeChanged(sender, e);
            RefreshToolReadiness();
            RecipeCommands?.RefreshOptions();
            WorkspaceCommands?.RefreshCanExecute();
        }

        private void OnNativeToolPropertySaved(
            object sender,
            OpenVisionNativeToolPropertySavedEventArgs e)
        {
            RefreshToolReadiness();
            OpenVisionNativeToolDocument document =
                documentController?.ActiveNativeDocument;
            if (document == null
                || e == null
                || !string.Equals(
                    document.ToolName,
                    e.ToolName,
                    StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            string recipeName = string.IsNullOrWhiteSpace(e.RecipeName)
                ? T(
                    "VisionTool.Persistence.DefaultRecipe",
                    "default Recipe")
                : e.RecipeName;
            if (!e.Succeeded)
            {
                string format = T(
                    "VisionTool.Persistence.SaveFailedFormat",
                    "Settings could not be saved for {0} / Recipe {1}. "
                    + "The current values remain in memory but may be lost after reopening. Cause: {2}");
                document.SetPropertyPersistenceStatus(
                    string.Format(
                        System.Globalization.CultureInfo.CurrentCulture,
                        format,
                        e.ToolName,
                        recipeName,
                        string.IsNullOrWhiteSpace(e.ErrorMessage)
                            ? T(
                                "VisionTool.Persistence.UnknownError",
                                "unknown error")
                            : e.ErrorMessage));
            }
            else if (e.RecoveredFromFailure)
            {
                string format = T(
                    "VisionTool.Persistence.SaveRecoveredFormat",
                    "Settings save recovered for {0} / Recipe {1}. "
                    + "The current values are now persisted.");
                document.SetPropertyPersistenceStatus(
                    string.Format(
                        System.Globalization.CultureInfo.CurrentCulture,
                        format,
                        e.ToolName,
                        recipeName));
            }
        }

        private void OnNativeToolSettingsSaved(
            object sender,
            OpenVisionNativeToolSettingsSavedEventArgs e)
        {
            RefreshToolReadiness();
            OpenVisionNativeToolDocument document =
                documentController?.ActiveNativeDocument;
            if (document == null
                || e == null
                || !string.Equals(
                    document.ToolName,
                    e.ToolName,
                    StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            if (!e.Succeeded)
            {
                document.SetPropertyPersistenceStatus(
                    OpenVisionNativeToolPersistenceStatusText.CreateSaveFailure(
                        e.ToolName,
                        e.RecipeName,
                        e.ErrorMessage));
            }
            else if (e.RecoveredFromFailure)
            {
                document.SetPropertyPersistenceStatus(
                    OpenVisionNativeToolPersistenceStatusText.CreateSaveRecovered(
                        e.ToolName,
                        e.RecipeName));
            }
        }

        private void RefreshToolReadiness()
        {
            viewModel.RefreshToolReadiness(displayManager, runtimeContext.Global?.VisionTools);
        }

        private static void OnToolRailCompactChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is OpenVisionShellHostView view)
            {
                view.chromeController?.ApplyToolRailCompactState(view.IsToolRailCompact);
            }
        }

        private string ResolveRecipeName()
        {
            return recipeContextStore?.CurrentRecipeName ?? ResolveRuntimeRecipeName();
        }

        private string ResolveRuntimeRecipeName()
        {
            string recipeName = runtimeContext.Global?.Recipe?.Name;
            if (!string.IsNullOrWhiteSpace(recipeName))
            {
                return recipeName;
            }

            recipeName = PropertyGridEditorFactory.GetRecipeName();
            return string.IsNullOrWhiteSpace(recipeName) ? "Default" : recipeName;
        }

        private void RefreshRecipeContext()
        {
            recipeContextStore.Refresh();
            recipeContextPresenter?.Refresh();
        }

        #endregion

        #region Sample and Layer Projection

        private void SelectToolMenu(VISION_MENU menu)
        {
            OpenVisionShellNavItem item = viewModel.NavigationGroups
                .SelectMany(group => group.Items)
                .FirstOrDefault(command => command.Menu == menu);
            if (item != null)
            {
                viewModel.SelectToolCommand.Execute(item);
            }
        }

        private void ApplyActiveSampleFirstStepParameters()
        {
            VisionPipelineStep step = sampleWorkflowPresenter?.FirstStep;
            if (step == null)
            {
                return;
            }

            documentController.ActiveNativeDocument?.ApplySampleStepParameters(step);
        }

        private bool OpenWorkspaceSampleByNameFromReview(string sampleName)
        {
            return commandController?.OpenRunnableSampleByName(sampleName) == true;
        }

        private OpenVisionRecipeLayerCard BuildRecipeLayerCard(string layerName)
        {
            if (string.IsNullOrWhiteSpace(layerName) || string.Equals(layerName, "-", StringComparison.Ordinal))
            {
                return OpenVisionRecipeLayerCard.CreateMissing(layerName);
            }

            int layerIndex = displayManager.FindIndex(layerName);
            if (layerIndex < 0)
            {
                return OpenVisionRecipeLayerCard.CreateMissing(layerName);
            }

            DrawingBitmap image = displayManager.GetLayerImage(layerName);
            string status = image == null
                ? LocalText("?대?吏 ?놁쓬", "No image")
                : string.Format(System.Globalization.CultureInfo.CurrentCulture, "{0}x{1}", image.Width, image.Height);
            return new OpenVisionRecipeLayerCard(
                layerName,
                status,
                OpenVisionBitmapImagePreviewFactory.Create(image),
                true);
        }

        private void SetWorkspaceDropOverlay(bool visible, bool canDrop)
        {
            if (workspaceDropOverlay == null)
            {
                return;
            }

            workspaceDropOverlay.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
            workspaceDropOverlay.BorderBrush = new SolidColorBrush(canDrop
                ? Color.FromRgb(0x8A, 0xD7, 0xDA)
                : Color.FromRgb(0x9A, 0x64, 0x00));
            workspaceDropOverlay.Background = new SolidColorBrush(canDrop
                ? Color.FromArgb(0x33, 0x15, 0x7C, 0x86)
                : Color.FromArgb(0x2A, 0x9A, 0x64, 0x00));
        }

        #endregion

        #region Shell Log Presentation

        private void ShellLogToggle_Checked(object sender, RoutedEventArgs e)
        {
            SetShellLogExpanded(true);
        }

        private void ShellLogToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            SetShellLogExpanded(false);
        }

        private void SetShellLogExpanded(bool expanded)
        {
            if (logPanelRow != null)
            {
                logPanelRow.Height = new GridLength(expanded ? 184D : 68D);
            }

            if (shellLogPanel != null)
            {
                shellLogPanel.Visibility = expanded ? Visibility.Visible : Visibility.Collapsed;
            }

            if (txtShellLogToggle != null)
            {
                txtShellLogToggle.Text = expanded
                    ? T("Shell.LogPanel.Close", LocalText("濡쒓렇 ?リ린", "Close Log"))
                    : T("Shell.LogPanel.Open", LocalText("濡쒓렇 ?닿린", "Open Log"));
            }
        }

        private void ApplyShellLogLocalization()
        {
            if (txtShellLogTitle != null)
            {
                txtShellLogTitle.Text = T("Pipeline.RunLog", LocalText("?ㅽ뻾 濡쒓렇", "Run Log"));
            }

            SetShellLogExpanded(btnShellLogToggle?.IsChecked == true);
        }

        #endregion

        #region Localization Helpers

        private static string T(string key, string fallbackText)
        {
            string value = OpenVisionLanguageService.T(key);
            return string.IsNullOrWhiteSpace(value) || string.Equals(value, key, StringComparison.Ordinal)
                ? fallbackText ?? string.Empty
                : value;
        }

        private static string LocalText(string korean, string english)
        {
            return OpenVisionLanguageService.CurrentLanguage == OpenVisionLanguage.English
                ? english ?? korean ?? string.Empty
                : korean ?? english ?? string.Empty;
        }

        #endregion

        #region Test Contract Helpers

        private void SetDockedToolInspectorWidthForTestCore(double width)
        {
            if (toolInspectorColumn == null || width <= 0D)
            {
                return;
            }

            toolInspectorColumn.Width = new System.Windows.GridLength(width);
            toolInspectorColumn.MinWidth = Math.Min(toolInspectorColumn.MinWidth, width);
            toolInspectorPanel?.UpdateLayout();
            UpdateLayout();
        }

        #endregion

        #region Test Contract Compatibility

        #region Surface and Test Hooks

        public OpenVisionShellHostViewTestSurface TestSurface => testSurface;

        internal OpenVisionNativeToolDocument ActiveNativeDocumentForTest => documentController?.ActiveNativeDocument;

        internal OpenVisionTcpIntegrationController TcpIntegrationControllerForTest => testSurface.TcpIntegrationControllerForTest;

        internal void OpenTcpIntegrationForTest() => testSurface.OpenTcpIntegrationForTest();

        internal Func<string, string, string, bool> QualifiedSnapshotLifecycleConfirmationForTest
        {
            get => testSurface.QualifiedSnapshotLifecycleConfirmationForTest;
            set => testSurface.QualifiedSnapshotLifecycleConfirmationForTest = value;
        }

        internal Func<string, bool> QualifiedSnapshotEvidenceOpenerForTest
        {
            get => testSurface.QualifiedSnapshotEvidenceOpenerForTest;
            set => testSurface.QualifiedSnapshotEvidenceOpenerForTest = value;
        }

        #endregion

        #region Test Properties

        public string ActiveToolFormTypeName => testSurface.ActiveToolFormTypeName;

        public string ActiveWpfToolWindowTypeName => testSurface.ActiveWpfToolWindowTypeName;

        public string ActiveWpfToolWindowTitle => testSurface.ActiveWpfToolWindowTitle;

        public string ActiveWpfToolWindowMinimizeToolTip => testSurface.ActiveWpfToolWindowMinimizeToolTip;

        public string ActiveWpfToolWindowMaximizeRestoreToolTip => testSurface.ActiveWpfToolWindowMaximizeRestoreToolTip;

        public string ActiveWpfToolWindowCloseToolTip => testSurface.ActiveWpfToolWindowCloseToolTip;

        public bool IsActiveWpfToolWindowVisibleForTest => testSurface.IsActiveWpfToolWindowVisibleForTest;

        public bool IsDockedToolInspectorVisibleForTest => testSurface.IsDockedToolInspectorVisibleForTest;

        public bool IsDockedDocumentWorkspaceVisibleForTest => testSurface.IsDockedDocumentWorkspaceVisibleForTest;

        public string ActivePendingToolTitle => testSurface.ActivePendingToolTitle;

        public string ActivePendingToolStatusText => testSurface.ActivePendingToolStatusText;

        public string ActiveNativeDocumentTypeName => testSurface.ActiveNativeDocumentTypeName;

        public string ActiveNativeStatusText => testSurface.ActiveNativeStatusText;

        public string ActiveNativeResultReviewText => testSurface.ActiveNativeResultReviewText;

        public string ActiveNativeRouteInputLayerNameForTest => testSurface.ActiveNativeRouteInputLayerNameForTest;

        public string ActiveNativeRouteInputLayerBNameForTest => testSurface.ActiveNativeRouteInputLayerBNameForTest;

        public string ActiveNativeRouteOutputLayerNameForTest => testSurface.ActiveNativeRouteOutputLayerNameForTest;

        public bool IsNativeToolPrewarmCompletedForTest => testSurface.IsNativeToolPrewarmCompletedForTest;

        public int NativeToolPrewarmCreatedCountForTest => testSurface.NativeToolPrewarmCreatedCountForTest;

        public long NativeToolPrewarmElapsedMillisecondsForTest => testSurface.NativeToolPrewarmElapsedMillisecondsForTest;

        public int NativeToolDocumentCacheCountForTest => testSurface.NativeToolDocumentCacheCountForTest;

        public string LastToolOpenTimingTextForTest => testSurface.LastToolOpenTimingTextForTest;

        public bool HasPipelineReviewDocumentForTest => testSurface.HasPipelineReviewDocumentForTest;

        public bool IsShellLoadedForTest => testSurface.IsShellLoadedForTest;

        public int HostedDocumentCount => testSurface.HostedDocumentCount;

        public bool IsNativeDocumentActive => testSurface.IsNativeDocumentActive;

        public int NativePreviewRunCount => testSurface.NativePreviewRunCount;

        public bool HasNativePreviewResult => testSurface.HasNativePreviewResult;

        public VisionToolRepository VisionToolRepositoryForTest => testSurface.VisionToolRepositoryForTest;

        public int ActiveLineInputRoiOverlayCount => testSurface.ActiveLineInputRoiOverlayCount;

        public bool ActiveLineSignalInspectorHasEvidenceForTest => testSurface.ActiveLineSignalInspectorHasEvidenceForTest;

        public bool ActiveLineSignalInspectorOverlayVisibleForTest => testSurface.ActiveLineSignalInspectorOverlayVisibleForTest;

        public bool ActiveLineSignalEvidenceCueVisibleForTest => testSurface.ActiveLineSignalEvidenceCueVisibleForTest;

        public string ActiveLineSignalInspectorEvidenceIdForTest => testSurface.ActiveLineSignalInspectorEvidenceIdForTest;

        public string ActiveLineSignalInspectorSourceSha256ForTest => testSurface.ActiveLineSignalInspectorSourceSha256ForTest;

        public int ActiveLineSignalInspectorSeriesCountForTest => testSurface.ActiveLineSignalInspectorSeriesCountForTest;

        public int ActiveLineSignalInspectorMarkerCountForTest => testSurface.ActiveLineSignalInspectorMarkerCountForTest;

        public int LayerDocumentCount => testSurface.LayerDocumentCount;

        public bool HasMainLayer => testSurface.HasMainLayer;

        public int HostLayerRowCount => testSurface.HostLayerRowCount;

        public string ActiveHostLayerTitle => testSurface.ActiveHostLayerTitle;

        public string SelectedHostLayerTitle => testSurface.SelectedHostLayerTitle;

        public string SelectedHostLayerMeta => testSurface.SelectedHostLayerMeta;

        public bool HasSelectedHostLayerPreview => testSurface.HasSelectedHostLayerPreview;

        public bool HasWorkspaceLayerPreview => testSurface.HasWorkspaceLayerPreview;

        public bool IsSingleWorkspaceVisibleForTest => testSurface.IsSingleWorkspaceVisibleForTest;

        public bool IsDockedWorkspaceVisibleForTest => testSurface.IsDockedWorkspaceVisibleForTest;

        public bool IsWorkspaceLayerDropEnabledForTest => testSurface.IsWorkspaceLayerDropEnabledForTest;

        public bool HasWorkspaceDropOverlayForTest => testSurface.HasWorkspaceDropOverlayForTest;

        public bool IsWorkspaceDropOverlayVisibleForTest => testSurface.IsWorkspaceDropOverlayVisibleForTest;

        public bool IsWorkspaceDropOverlayHitTestSafeForTest => testSurface.IsWorkspaceDropOverlayHitTestSafeForTest;

        public bool HasDockingGuideOverlayForTest => testSurface.HasDockingGuideOverlayForTest;

        public bool IsDockingGuideOverlayVisibleForTest => testSurface.IsDockingGuideOverlayVisibleForTest;

        public string ActiveDockingGuideZoneForTest => testSurface.ActiveDockingGuideZoneForTest;

        public bool IsDockingGuideOverlayHitTestSafeForTest => testSurface.IsDockingGuideOverlayHitTestSafeForTest;

        public int DockingGuideZoneCountForTest => testSurface.DockingGuideZoneCountForTest;

        public int WorkspaceTextureTileCount => testSurface.WorkspaceTextureTileCount;

        public bool IsWorkspaceEmptyPromptVisible => testSurface.IsWorkspaceEmptyPromptVisible;

        public string WorkspaceCoordinatesTextForTest => testSurface.WorkspaceCoordinatesTextForTest;

        public string WorkspacePixelTextForTest => testSurface.WorkspacePixelTextForTest;

        public string WorkspaceEmptyTitle => testSurface.WorkspaceEmptyTitle;

        public string WorkspaceEmptyDetail => testSurface.WorkspaceEmptyDetail;

        public string WorkspaceLayerTitle => testSurface.WorkspaceLayerTitle;

        public string WorkspaceLayerMeta => testSurface.WorkspaceLayerMeta;

        public string WorkspaceLoadImageMenuText => testSurface.WorkspaceLoadImageMenuText;

        public string WorkspaceLoadImageButtonText => testSurface.WorkspaceLoadImageButtonText;

        public bool HasWorkspaceLoadImageMenu => testSurface.HasWorkspaceLoadImageMenu;

        public bool IsWorkspaceLoadImageIntoLayerMenuVisibleForTest => testSurface.IsWorkspaceLoadImageIntoLayerMenuVisibleForTest;

        public int OpenLayerViewerWindowCount => testSurface.OpenLayerViewerWindowCount;

        public string OpenLayerViewerWindowTitles => testSurface.OpenLayerViewerWindowTitles;

        public int DockedLayerCount => testSurface.DockedLayerCount;

        public int DockedLayerTextureTileCount => testSurface.DockedLayerTextureTileCount;

        public int DockedLayerPaneCount => testSurface.DockedLayerPaneCount;

        public string DockedLayerRootOrientationForTest => testSurface.DockedLayerRootOrientationForTest;

        public int DockedLayerNestedLayoutPanelCountForTest => testSurface.DockedLayerNestedLayoutPanelCountForTest;

        public bool AreDockedLayerViewersCompactSizeReadyForTest => testSurface.AreDockedLayerViewersCompactSizeReadyForTest;

        public bool IsToolRailCompactForTest => testSurface.IsToolRailCompactForTest;

        public double ToolRailWidthForTest => testSurface.ToolRailWidthForTest;

        public bool IsToolRailNavigationVisibleForTest => testSurface.IsToolRailNavigationVisibleForTest;

        public bool IsToolRailCompactLabelHiddenForTest => testSurface.IsToolRailCompactLabelHiddenForTest;

        public string ToolSearchTextForTest => testSurface.ToolSearchTextForTest;

        public int VisibleToolSearchItemCountForTest => testSurface.VisibleToolSearchItemCountForTest;

        public string VisibleToolSearchCommandIdsForTest => testSurface.VisibleToolSearchCommandIdsForTest;

        public bool AreDockedLayersNativeFloatingDisabledForTest => testSurface.AreDockedLayersNativeFloatingDisabledForTest;

        public bool AreDockedLayerViewersCompactForTest => testSurface.AreDockedLayerViewersCompactForTest;

        public int DockedLayerTabHeaderCount => testSurface.DockedLayerTabHeaderCount;

        public bool AreDockedLayerTabHeadersGestureReadyForTest => testSurface.AreDockedLayerTabHeadersGestureReadyForTest;

        public bool AreDockedLayerTabHeadersReadableForTest => testSurface.AreDockedLayerTabHeadersReadableForTest;

        public bool AreDockedLayerTabHeaderGripsReadyForTest => testSurface.AreDockedLayerTabHeaderGripsReadyForTest;

        public bool AreDockedLayersNativeFloatingEnabledForTest => testSurface.AreDockedLayersNativeFloatingEnabledForTest;

        public string DockedLayerTabHeaderDiagnosticsForTest => testSurface.DockedLayerTabHeaderDiagnosticsForTest;

        public string DockedLayerTitles => testSurface.DockedLayerTitles;

        public OpenVisionDockingVisualSnapshot DockedLayerVisualSnapshotForTest => testSurface.DockedLayerVisualSnapshotForTest;

        public string DirectResultBadgeText => testSurface.DirectResultBadgeText;

        public string DirectResultTitleText => testSurface.DirectResultTitleText;

        public string DirectResultStatusText => testSurface.DirectResultStatusText;

        public string DirectResultRouteText => testSurface.DirectResultRouteText;

        public string ActiveNativeRecipeContextNameForTest => testSurface.ActiveNativeRecipeContextNameForTest;

        public string ActiveNativeRecipeContextPipelineNameForTest => testSurface.ActiveNativeRecipeContextPipelineNameForTest;

        public int PipelineReviewStepCount => testSurface.PipelineReviewStepCount;

        public string PipelineReviewRecipeContextNameForTest => testSurface.PipelineReviewRecipeContextNameForTest;

        public string PipelineReviewRecipeContextPipelineNameForTest => testSurface.PipelineReviewRecipeContextPipelineNameForTest;

        public string PipelineReviewSelectedStepName => testSurface.PipelineReviewSelectedStepName;

        public string PipelineReviewSelectedStatusText => testSurface.PipelineReviewSelectedStatusText;

        public string PipelineReviewFlowSummaryText => testSurface.PipelineReviewFlowSummaryText;

        public string PipelineReviewParameterSummaryText => testSurface.PipelineReviewParameterSummaryText;

        public string PipelineReviewValidationStatusText => testSurface.PipelineReviewValidationStatusText;

        public string PipelineReviewValidationDetailText => testSurface.PipelineReviewValidationDetailText;

        public string PipelineReviewResultSummaryText => testSurface.PipelineReviewResultSummaryText;

        public string PipelineReviewResultDetailText => testSurface.PipelineReviewResultDetailText;

        public string PipelineReviewRunLogText => testSurface.PipelineReviewRunLogText;

        public string PipelineReviewExecutionState => testSurface.PipelineReviewExecutionState;

        public string PipelineReviewProgressText => testSurface.PipelineReviewProgressText;

        public string PipelineReviewGuideStageText => testSurface.PipelineReviewGuideStageText;

        public string PipelineReviewGuideCurrentStepText => testSurface.PipelineReviewGuideCurrentStepText;

        public string PipelineReviewGuideNextActionText => testSurface.PipelineReviewGuideNextActionText;

        public string PipelineReviewGuideResultDecisionText => testSurface.PipelineReviewGuideResultDecisionText;

        public string PipelineReviewGuideDetailText => testSurface.PipelineReviewGuideDetailText;

        public string PipelineReviewGuidePairText => testSurface.PipelineReviewGuidePairText;

        public string PipelineReviewGuidePairActionText => testSurface.PipelineReviewGuidePairActionText;

        public string PipelineReviewGuidePairMetricText => testSurface.PipelineReviewGuidePairMetricText;

        public string PipelineReviewGuideChecklistText => testSurface.PipelineReviewGuideChecklistText;

        public string PipelineReviewGuideParameterFocusText => testSurface.PipelineReviewGuideParameterFocusText;

        public string PipelineReviewGuideTriageFailureText => testSurface.PipelineReviewGuideTriageFailureText;

        public string PipelineReviewGuideTriageAdjustmentText => testSurface.PipelineReviewGuideTriageAdjustmentText;

        public string PipelineReviewGuideTriageRerunText => testSurface.PipelineReviewGuideTriageRerunText;

        public bool CanOpenPipelineReviewPairSampleForTest => testSurface.CanOpenPipelineReviewPairSampleForTest;

        public bool CanSelectPreviousPipelineReviewStepForTest => testSurface.CanSelectPreviousPipelineReviewStepForTest;

        public bool CanSelectNextPipelineReviewStepForTest => testSurface.CanSelectNextPipelineReviewStepForTest;

        public bool CanSelectFirstIssuePipelineReviewStepForTest => testSurface.CanSelectFirstIssuePipelineReviewStepForTest;

        public bool HasPipelineReviewInputPreview => testSurface.HasPipelineReviewInputPreview;

        public bool HasPipelineReviewOutputPreview => testSurface.HasPipelineReviewOutputPreview;

        public int PipelineReviewObjectResultCountForTest => testSurface.PipelineReviewObjectResultCountForTest;

        public int PipelineReviewObjectMetricDistributionSeriesCountForTest => testSurface.PipelineReviewObjectMetricDistributionSeriesCountForTest;

        public int PipelineReviewObjectMetricDistributionMarkerCountForTest => testSurface.PipelineReviewObjectMetricDistributionMarkerCountForTest;

        public string PipelineReviewObjectMetricDistributionMetricForTest => testSurface.PipelineReviewObjectMetricDistributionMetricForTest;

        public string PipelineReviewObjectMetricDistributionEvidenceIdForTest => testSurface.PipelineReviewObjectMetricDistributionEvidenceIdForTest;

        public bool PipelineReviewMatcherDiagnosticTabVisibleForTest => testSurface.PipelineReviewMatcherDiagnosticTabVisibleForTest;

        public string PipelineReviewMatcherDiagnosticStateForTest => testSurface.PipelineReviewMatcherDiagnosticStateForTest;

        public string PipelineReviewMatcherDiagnosticEvidenceIdForTest => testSurface.PipelineReviewMatcherDiagnosticEvidenceIdForTest;

        public int PipelineReviewMatcherDiagnosticRowCountForTest => testSurface.PipelineReviewMatcherDiagnosticRowCountForTest;

        public int PipelineReviewMatcherDiagnosticModelPointCountForTest => testSurface.PipelineReviewMatcherDiagnosticModelPointCountForTest;

        public bool PipelineReviewMatcherDiagnosticHasSelectedCandidateForTest => testSurface.PipelineReviewMatcherDiagnosticHasSelectedCandidateForTest;

        public bool PipelineReviewMatcherDiagnosticHasAlternativeForTest => testSurface.PipelineReviewMatcherDiagnosticHasAlternativeForTest;

        public bool IsPipelineReviewFixtureDesignerVisibleForTest => testSurface.IsPipelineReviewFixtureDesignerVisibleForTest;

        public string PipelineReviewFixtureRelationshipTextForTest => testSurface.PipelineReviewFixtureRelationshipTextForTest;

        public int PipelineReviewFixtureProducerStepNumberForTest => testSurface.PipelineReviewFixtureProducerStepNumberForTest;

        public int PipelineReviewFixtureMeasurementStepNumberForTest => testSurface.PipelineReviewFixtureMeasurementStepNumberForTest;

        public int PipelineReviewSelectedObjectResultNumberForTest => testSurface.PipelineReviewSelectedObjectResultNumberForTest;

        public bool HasPipelineReviewObjectHighlightForTest => testSurface.HasPipelineReviewObjectHighlightForTest;

        #endregion

        #region Test Actions

        public void SelectToolForTest(VISION_MENU menu) => testSurface.SelectToolForTest(menu);

        public void ToggleToolRailForTest() => testSurface.ToggleToolRailForTest();

        public void SetToolSearchTextForTest(string text) => testSurface.SetToolSearchTextForTest(text);

        public void ClearToolSearchForTest() => testSurface.ClearToolSearchForTest();

        public bool HasNativeToolDocumentCachedForTest(VISION_MENU menu) => testSurface.HasNativeToolDocumentCachedForTest(menu);

        public void RunActiveNativePreviewForTest() => testSurface.RunActiveNativePreviewForTest();

        public void CreateActiveNativeOutputLayerForTest() => testSurface.CreateActiveNativeOutputLayerForTest();

        public bool OpenLayerViewerForTest(string layerTitle) => testSurface.OpenLayerViewerForTest(layerTitle);

        public bool HasLayerForTest(string layerTitle) => testSurface.HasLayerForTest(layerTitle);

        public string HostLayerTabTextsForTest => testSurface.HostLayerTabTextsForTest;

        public bool AreHostLayerTabsReadableForTest => testSurface.AreHostLayerTabsReadableForTest;

        public TestBitmap GetLayerImageCloneForTest(string layerTitle) => testSurface.GetLayerImageCloneForTest(layerTitle);

        public bool ActivateHostLayerForTest(string layerTitle) => testSurface.ActivateHostLayerForTest(layerTitle);

        public bool SelectHostLayerRowForTest(string layerTitle) => testSurface.SelectHostLayerRowForTest(layerTitle);

        public bool RightClickHostLayerRowForTest(string layerTitle) => testSurface.RightClickHostLayerRowForTest(layerTitle);

        public bool DockLayerForTest(string layerTitle) => testSurface.DockLayerForTest(layerTitle);

        public bool ActivateDockedLayerForTest(string layerTitle) => testSurface.ActivateDockedLayerForTest(layerTitle);

        public bool AddLayerImageForTest(string layerTitle, TestBitmap image) => testSurface.AddLayerImageForTest(layerTitle, image);

        public string CreateLayerForTest() => testSurface.CreateLayerForTest();

        public bool LoadImageIntoLayerForTest(string layerTitle, string path) => testSurface.LoadImageIntoLayerForTest(layerTitle, path);

        public bool SetLayerImageForTest(string layerTitle, TestBitmap image) => testSurface.SetLayerImageForTest(layerTitle, image);

        public bool RenameLayerForTest(string oldLayerTitle, string newLayerTitle) => testSurface.RenameLayerForTest(oldLayerTitle, newLayerTitle);

        public bool DeleteLayerForTest(string layerTitle) => testSurface.DeleteLayerForTest(layerTitle);

        public void ClearLayerImageHistoryForTest() => testSurface.ClearLayerImageHistoryForTest();

        public bool SplitDockedLayerForTest(string layerTitle) => testSurface.SplitDockedLayerForTest(layerTitle);

        public bool ArrangeDockedLayerPanesForTest(string orientationName, params string[] layerTitles) => testSurface.ArrangeDockedLayerPanesForTest(orientationName, layerTitles);

        public bool ArrangeDockedLayerGridForTest(params string[] layerTitles) => testSurface.ArrangeDockedLayerGridForTest(layerTitles);

        public bool MoveDockedLayerToPrimaryPaneForTest(string layerTitle) => testSurface.MoveDockedLayerToPrimaryPaneForTest(layerTitle);

        public bool DockLayerToGuideZoneForTest(string layerTitle, string zoneName) => testSurface.DockLayerToGuideZoneForTest(layerTitle, zoneName);

        public void ClearDockedLayersForTest() => testSurface.ClearDockedLayersForTest();

        public void ShowDockingGuideForTest(double xRatio = 0.5D, double yRatio = 0.5D) => testSurface.ShowDockingGuideForTest(xRatio, yRatio);

        public System.Windows.Point GetDockedWorkspaceScreenPointForTest(double x, double y) => testSurface.GetDockedWorkspaceScreenPointForTest(x, y);

        public bool ShowDockedLayerTabDragGuideForTest() => testSurface.ShowDockedLayerTabDragGuideForTest();

        public void HideDockingGuideForTest() => testSurface.HideDockingGuideForTest();

        public void SaveDockingWorkspaceStateForTest() => testSurface.SaveDockingWorkspaceStateForTest();

        public bool RestoreDockingLayoutStateForTest() => testSurface.RestoreDockingLayoutStateForTest();

        public bool SaveWorkspaceImageToFileForTest(string path) => testSurface.SaveWorkspaceImageToFileForTest(path);

        public bool SaveDockedLayerImageToFileForTest(string layerTitle, string path) => testSurface.SaveDockedLayerImageToFileForTest(layerTitle, path);

        public TestBitmap CloneDockedLayerImageForTest(string layerTitle) => testSurface.CloneDockedLayerImageForTest(layerTitle);

        public int GetDockedLayerImagePixelWidthForTest(string layerTitle) => testSurface.GetDockedLayerImagePixelWidthForTest(layerTitle);

        public int GetDockedLayerImagePixelHeightForTest(string layerTitle) => testSurface.GetDockedLayerImagePixelHeightForTest(layerTitle);

        public int GetDockedLayerTextureTileCountForTest(string layerTitle) => testSurface.GetDockedLayerTextureTileCountForTest(layerTitle);

        public int LiveLayerViewerInstanceCountForTest => testSurface.LiveLayerViewerInstanceCountForTest;

        public string LiveLayerViewerInstanceStatesForTest => testSurface.LiveLayerViewerInstanceStatesForTest;

        public bool CloseActiveWpfToolWindowForTest() => testSurface.CloseActiveWpfToolWindowForTest();

        public bool DockActiveWpfToolWindowForTest() => testSurface.DockActiveWpfToolWindowForTest();

        public bool FloatDockedWpfToolWindowForTest() => testSurface.FloatDockedWpfToolWindowForTest();

        public string DockedToolTitleForTest => testSurface.DockedToolTitleForTest;

        public bool IsDockedToolFloatButtonVisibleForTest => testSurface.IsDockedToolFloatButtonVisibleForTest;

        public bool IsDockedToolCloseButtonVisibleForTest => testSurface.IsDockedToolCloseButtonVisibleForTest;

        public double DockedToolFloatButtonWidthForTest => testSurface.DockedToolFloatButtonWidthForTest;

        public double DockedToolCloseButtonWidthForTest => testSurface.DockedToolCloseButtonWidthForTest;

        public string DockedToolFloatButtonToolTipForTest => testSurface.DockedToolFloatButtonToolTipForTest;

        public string DockedToolCloseButtonToolTipForTest => testSurface.DockedToolCloseButtonToolTipForTest;

        public double DockedToolInspectorWidthForTest => testSurface.DockedToolInspectorWidthForTest;

        public void SetDockedToolInspectorWidthForTest(double width) => testSurface.SetDockedToolInspectorWidthForTest(width);

        public void SetMainLayerImageForTest(TestBitmap image) => testSurface.SetMainLayerImageForTest(image);

        public bool LoadMainImageFromFileForTest(string path) => testSurface.LoadMainImageFromFileForTest(path);

        public bool IsShellLogExpandedForTest => testSurface.IsShellLogExpandedForTest;

        public string ShellLogToggleTextForTest => testSurface.ShellLogToggleTextForTest;

        public System.Windows.Point RecipeManagerPanelOffsetForTest => testSurface.RecipeManagerPanelOffsetForTest;

        public bool MoveRecipeManagerPanelForTest(double deltaX, double deltaY) => testSurface.MoveRecipeManagerPanelForTest(deltaX, deltaY);

        public void SetShellLogExpandedForTest(bool expanded) => testSurface.SetShellLogExpandedForTest(expanded);

        public bool HasRunnableWorkspaceSampleForTest => testSurface.HasRunnableWorkspaceSampleForTest;

        public void OpenFirstRunnableWorkspaceSampleForTest() => testSurface.OpenFirstRunnableWorkspaceSampleForTest();

        public bool OpenWorkspaceSampleForTest(string sampleName) => testSurface.OpenWorkspaceSampleForTest(sampleName);

        public bool IsWorkspaceSampleWorkflowVisibleForTest => testSurface.IsWorkspaceSampleWorkflowVisibleForTest;

        public bool IsWorkspaceMainActionVisibleForTest => testSurface.IsWorkspaceMainActionVisibleForTest;

        public string WorkspaceMainActionTitleForTest => testSurface.WorkspaceMainActionTitleForTest;

        public string WorkspaceMainActionDetailForTest => testSurface.WorkspaceMainActionDetailForTest;

        public string WorkspaceMainActionMetaForTest => testSurface.WorkspaceMainActionMetaForTest;

        public string WorkspaceSampleWorkflowTitleForTest => testSurface.WorkspaceSampleWorkflowTitleForTest;

        public string WorkspaceSampleWorkflowMetaForTest => testSurface.WorkspaceSampleWorkflowMetaForTest;

        public string WorkspaceSampleWorkflowDetailForTest => testSurface.WorkspaceSampleWorkflowDetailForTest;

        public bool CanOpenSamplePipelineForTest => testSurface.CanOpenSamplePipelineForTest;

        public bool CanOpenSampleFirstStepToolForTest => testSurface.CanOpenSampleFirstStepToolForTest;

        public bool CanOpenSampleCounterpartForTest => testSurface.CanOpenSampleCounterpartForTest;

        public bool CanOpenWorkspaceThresholdToolForTest => testSurface.CanOpenWorkspaceThresholdToolForTest;

        public bool CanOpenWorkspaceMatchingToolForTest => testSurface.CanOpenWorkspaceMatchingToolForTest;

        public bool CanOpenWorkspaceLineToolForTest => testSurface.CanOpenWorkspaceLineToolForTest;

        public string WorkspaceSampleFirstStepMenuForTest => testSurface.WorkspaceSampleFirstStepMenuForTest;

        public void OpenSamplePipelineForTest() => testSurface.OpenSamplePipelineForTest();

        public void OpenSampleFirstStepToolForTest() => testSurface.OpenSampleFirstStepToolForTest();

        public void OpenSampleCounterpartForTest() => testSurface.OpenSampleCounterpartForTest();

        public void OpenWorkspaceThresholdToolForTest() => testSurface.OpenWorkspaceThresholdToolForTest();

        public void OpenWorkspaceMatchingToolForTest() => testSurface.OpenWorkspaceMatchingToolForTest();

        public void OpenWorkspaceLineToolForTest() => testSurface.OpenWorkspaceLineToolForTest();

        public string ActivePipelineNameForTest => testSurface.ActivePipelineNameForTest;

        public int ActivePipelineStepCountForTest => testSurface.ActivePipelineStepCountForTest;

        public string ActiveRecipeContextNameForTest => testSurface.ActiveRecipeContextNameForTest;

        public string ActiveRecipeContextPipelineNameForTest => testSurface.ActiveRecipeContextPipelineNameForTest;

        public string ActiveRecipeContextDisplayTextForTest => testSurface.ActiveRecipeContextDisplayTextForTest;

        public string ActiveRecipeContextSourcePathForTest => testSurface.ActiveRecipeContextSourcePathForTest;

        public string ActiveRecipeContextLayerNameForTest => testSurface.ActiveRecipeContextLayerNameForTest;

        public string SelectedLanguageDisplayNameForTest => testSurface.SelectedLanguageDisplayNameForTest;

        public void SelectLanguageForTest(OpenVisionLanguage language) => testSurface.SelectLanguageForTest(language);

        public string SelectedRecipeNameForTest => testSurface.SelectedRecipeNameForTest;

        public IReadOnlyList<string> RecipeOptionsForTest => testSurface.RecipeOptionsForTest;

        public string RecipeManagerSelectedPipelineNameForTest => testSurface.RecipeManagerSelectedPipelineNameForTest;

        public string RecipeManagerSelectedSampleNameForTest => testSurface.RecipeManagerSelectedSampleNameForTest;

        public bool IsRecipeManagerOpenForTest => testSurface.IsRecipeManagerOpenForTest;

        public bool IsShellBusyOverlayVisibleForTest => testSurface.IsShellBusyOverlayVisibleForTest;

        public string ShellBusyTitleForTest => testSurface.ShellBusyTitleForTest;

        public void ShowPipelineLoadingForTest() => testSurface.ShowPipelineLoadingForTest();

        public void HideShellBusyForTest() => testSurface.HideShellBusyForTest();

        public void QueuePendingRecipeEditDecisionForTest( OpenVisionRecipePendingEditDecision decision) => testSurface.QueuePendingRecipeEditDecisionForTest(decision);

        public void FailNextRecipeStepEditCommitForTest() => testSurface.FailNextRecipeStepEditCommitForTest();

        public void FailNextRecipeStepSaveForTest() => testSurface.FailNextRecipeStepSaveForTest();

        public void FailNextRecipeStepRoundTripValidationForTest() => testSurface.FailNextRecipeStepRoundTripValidationForTest();

        public void SetRecipeManagerOpenForTest(bool isOpen) => testSurface.SetRecipeManagerOpenForTest(isOpen);

        public void CreateRecipeForTest() => testSurface.CreateRecipeForTest();

        public void SelectRecipeForTest(string recipeName) => testSurface.SelectRecipeForTest(recipeName);

        public void SwitchRecipeContextForTest(string recipeName) => testSurface.SwitchRecipeContextForTest(recipeName);

        public bool UpdateWorkspacePointerAtCenterForTest() => testSurface.UpdateWorkspacePointerAtCenterForTest();

        public string GetWorkspacePointerCoordinateForTest(double xRatio, double yRatio) => testSurface.GetWorkspacePointerCoordinateForTest(xRatio, yRatio);

        public void ZoomWorkspaceAtForTest(double xRatio, double yRatio, double factor) => testSurface.ZoomWorkspaceAtForTest(xRatio, yRatio, factor);

        public void PanWorkspaceByForTest(double surfaceDeltaX, double surfaceDeltaY) => testSurface.PanWorkspaceByForTest(surfaceDeltaX, surfaceDeltaY);

        public bool LoadActiveNativePreviewImageFromFileForTest( string path, VisionToolPreviewImageRole role = VisionToolPreviewImageRole.Input) => testSurface.LoadActiveNativePreviewImageFromFileForTest(path, role);

        public bool SaveActiveNativePreviewImageToFileForTest( string path, VisionToolPreviewImageRole role = VisionToolPreviewImageRole.Input) => testSurface.SaveActiveNativePreviewImageToFileForTest(path, role);

        public bool ConfigureActiveThresholdBasicInvertForTest(bool invert) => testSurface.ConfigureActiveThresholdBasicInvertForTest(invert);

        public VisionPipelineStep AddActiveNativePipelineStepForTest() => testSurface.AddActiveNativePipelineStepForTest();

        public void SetActiveLineRoiForTest(int x, int y, int width, int height) => testSurface.SetActiveLineRoiForTest(x, y, width, height);

        public void SetActiveLineSettingForTest(string setting) => testSurface.SetActiveLineSettingForTest(setting);

        public void SetActiveSelectedLineRoiForTest(int x, int y, int width, int height) => testSurface.SetActiveSelectedLineRoiForTest(x, y, width, height);

        public void ConfigureActiveSelectedLineForTest(string projectionDirection, string polarity, string verticalDirection = null) => testSurface.ConfigureActiveSelectedLineForTest(projectionDirection, polarity, verticalDirection);

        public void ConfigureActiveSelectedLineDrawForTest(bool showVerticalLine, bool showEdge, bool showContour, bool showFitLine) => testSurface.ConfigureActiveSelectedLineDrawForTest(showVerticalLine, showEdge, showContour, showFitLine);

        public void ConfigureActiveSelectedLineThresholdForTest(double threshold, bool invert) => testSurface.ConfigureActiveSelectedLineThresholdForTest(threshold, invert);

        public void ConfigureActiveSelectedLineMeasureTuningForTest( bool useThreshold, bool useAdaptiveThreshold, double contrast, double thickness, double samplingStep, int pointRange, bool useManualAngle, double manualAngleValue) => testSurface.ConfigureActiveSelectedLineMeasureTuningForTest(useThreshold, useAdaptiveThreshold, contrast, thickness, samplingStep, pointRange, useManualAngle, manualAngleValue);

        public void SetActiveLinePurposeForTest(string purpose) => testSurface.SetActiveLinePurposeForTest(purpose);

        public string GetActiveLineSignalInspectorAttributeForTest(string name) => testSurface.GetActiveLineSignalInspectorAttributeForTest(name);

        public bool ExerciseActiveLineSignalInspectorNavigationForTest() => testSurface.ExerciseActiveLineSignalInspectorNavigationForTest();

        public void ExportActiveLineSignalEvidenceForTest(string path) => testSurface.ExportActiveLineSignalEvidenceForTest(path);

        public void CloseActiveLineSignalInspectorForTest() => testSurface.CloseActiveLineSignalInspectorForTest();

        public void OpenActiveLineSignalInspectorForTest() => testSurface.OpenActiveLineSignalInspectorForTest();

        public void SetActiveMatchingTemplatePathForTest(string path) => testSurface.SetActiveMatchingTemplatePathForTest(path);

        public void ConfigureActiveMatchingForTest(Action<MatchingProperty> configure) => testSurface.ConfigureActiveMatchingForTest(configure);

        public void ConfigureActiveAffineTransformForTest(Action<AffineTransformProperty> configure) => testSurface.ConfigureActiveAffineTransformForTest(configure);

        public void SetActiveEdgeBasedMatchingTemplatePathForTest(string path) => testSurface.SetActiveEdgeBasedMatchingTemplatePathForTest(path);

        public void ConfigureActiveEdgeBasedMatchingForTest(Action<EdgeBasedMatchingProperty> configure) => testSurface.ConfigureActiveEdgeBasedMatchingForTest(configure);

        public void SetActiveAutoMPointRepresentativeImagesForTest(IEnumerable<string> paths) => testSurface.SetActiveAutoMPointRepresentativeImagesForTest(paths);

        public void SetActiveFeatureMatchingTemplatePathForTest(string path) => testSurface.SetActiveFeatureMatchingTemplatePathForTest(path);

        public bool RunActiveToolFormForTest() => testSurface.RunActiveToolFormForTest();

        public void SelectPipelineReviewStepForTest(int index, OpenVisionLab.Pipeline.Controls.PipelineFlowPreviewMode mode) => testSurface.SelectPipelineReviewStepForTest(index, mode);

        public Task RunPipelineReviewForTestAsync() => testSurface.RunPipelineReviewForTestAsync();

        public bool OpenPipelineReviewPairSampleForTest() => testSurface.OpenPipelineReviewPairSampleForTest();

        public void SelectPipelineReviewObjectResultForTest(int index) => testSurface.SelectPipelineReviewObjectResultForTest(index);

        public void SelectPipelineReviewObjectResultFromImageForTest(int index) => testSurface.SelectPipelineReviewObjectResultFromImageForTest(index);

        #endregion

        #endregion

        #endregion
    }
}
