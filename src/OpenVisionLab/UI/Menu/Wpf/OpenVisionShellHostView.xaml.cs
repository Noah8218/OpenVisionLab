using OpenVisionLab.Core;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using WpfUserControl = System.Windows.Controls.UserControl;
using static OpenVisionLab.DEFINE;
using DrawingBitmap = System.Drawing.Bitmap;

namespace OpenVisionLab
{
    public sealed partial class OpenVisionShellHostView : WpfUserControl, IDisposable
    {
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
        private readonly OpenVisionShellHostDockedLayerWorkspaceComposition dockedLayerWorkspaceComposition;
        private readonly OpenVisionShellHostDocumentController documentController;
        private readonly OpenVisionFloatingToolWindowHost floatingToolWindowHost = new OpenVisionFloatingToolWindowHost();
        private readonly OpenVisionNativePreviewRouteCoordinator nativePreviewRouteCoordinator;
        private readonly OpenVisionShellHostLayerActivationController layerActivationController;
        private readonly OpenVisionShellHostLayerSelectionController layerSelectionController;
        private readonly OpenVisionShellHostLayerInteractionController layerInteractionController;
        private readonly OpenVisionShellHostToolWindowController toolWindowController;
        private readonly OpenVisionShellHostCommandController commandController;
        private readonly OpenVisionShellHostRecipeController recipeController;
        private readonly OpenVisionShellHostToolSelectionController toolSelectionController;
        private readonly OpenVisionShellHostToolRailPresenter toolRailPresenter = new OpenVisionShellHostToolRailPresenter();
        private readonly OpenVisionShellHostStatePresenter statePresenter;
        private readonly OpenVisionShellHostTestAdapter testAdapter;
        private readonly OpenVisionShellHostDockingTestFacade dockingTestFacade;
        private readonly OpenVisionShellHostLayerTestFacade layerTestFacade;
        private readonly OpenVisionShellHostToolTestFacade toolTestFacade;
        private readonly OpenVisionShellHostMenuPresenter menuPresenter;
        private readonly OpenVisionShellHostChromeController chromeController;
        private readonly OpenVisionDockedToolInspectorController dockedToolInspectorController;
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
        private readonly Queue<OpenVisionRecipePendingEditDecision> pendingRecipeEditDecisionsForTest =
            new Queue<OpenVisionRecipePendingEditDecision>();
        private VisionToolPropertyGridHost recipeStepPropertyGridHostController;
        private bool isRecipeManagerPanelDragging;
        private bool isRestoringRecipeManagerAfterCanceledClose;
        private bool failNextRecipeStepEditCommitForTest;
        private bool failNextRecipeStepSaveForTest;
        private bool failNextRecipeStepRoundTripValidationForTest;
        private Point recipeManagerPanelDragStartPoint;
        private double recipeManagerPanelDragStartX;
        private double recipeManagerPanelDragStartY;

        public OpenVisionShellHostView()
            : this(ApplicationRuntimeContext.CreateDefault())
        {
        }

        public OpenVisionShellHostView(ApplicationRuntimeContext runtimeContext)
        {
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
            workspacePreviewController = new OpenVisionShellHostWorkspacePreviewController();
            layerViewerController = new OpenVisionShellHostLayerViewerController(
                displayManager,
                layerDetailPresenter,
                layerViewerWindows,
                () => Window.GetWindow(this));

            InitializeComponent();
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
                btnCloseDockedTool);
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
                    refreshCoordinator.RefreshLayerActionButtons));
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
            toolWindowLifecycleController = new OpenVisionShellHostToolWindowLifecycleController(
                documentController,
                floatingToolWindowHost,
                dockedToolInspectorController,
                chromeController.SetDirectRunSucceeded,
                chromeController.SetActiveDocumentText,
                refreshCoordinator.RefreshHostLayerRows,
                hasPreviewResult => nativePreviewRouteCoordinator.RefreshAfterLayerStateChanged(hasPreviewResult),
                nativePreviewRouteCoordinator.RefreshLastVisibleNativeOutputWorkspacePreview);
            toolPrewarmController = new OpenVisionShellHostToolPrewarmController(
                Dispatcher,
                nativeToolPrewarmService,
                floatingToolWindowHost,
                () => sessionState.Loaded && !sessionState.Disposed,
                () => viewModel.SelectedItem?.Menu,
                () => Window.GetWindow(this));
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
                floatingToolWindowHost,
                toolWindowLifecycleController.ShowDockedToolWindow,
                () => toolWindowLifecycleController.IsDockedToolInspectorVisible,
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
            toolSelectionController = new OpenVisionShellHostToolSelectionController(
                viewModel,
                toolWindowController,
                toolPrewarmController,
                txtToolOpenTimingDiagnostics,
                () => sessionState.Loaded);
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
                (layerNames, selectedLayer) =>
                {
                    viewModel.SetLayerOptions(layerNames, selectedLayer);
                    RefreshToolReadiness();
                });
            refreshCoordinator.AttachLayerRefreshController(layerRefreshController);
            workspaceImageController = new OpenVisionShellHostWorkspaceImageController(
                displayManager,
                documentController,
                chromeController.SetDirectRunPending,
                refreshCoordinator.RefreshHostSelectedLayerDetail,
                refreshCoordinator.RefreshHostLayerRows,
                chromeController.RefreshDirectRouteText);
            sampleWorkflowPresenter = new OpenVisionShellHostSampleWorkflowPresenter(
                workspaceSampleWorkflowOverlay,
                txtWorkspaceSampleWorkflowTitle,
                txtWorkspaceSampleWorkflowMeta,
                txtWorkspaceSampleWorkflowDetail,
                btnWorkspaceSampleCounterpart,
                txtWorkspaceSampleCounterpartButtonText,
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
                SelectToolMenu,
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
                });
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
            RecipeCommands = new OpenVisionShellHostRecipeCommandSurface(
                ResolveRuntimeRecipeName,
                recipeName => runtimeContext.Global.Recipe.Name = recipeName,
                () =>
                {
                    RefreshRecipeContext();
                    WorkspaceCommands?.RefreshCanExecute();
                },
                ConfirmDeleteRecipe,
                ConfirmDeletePipeline,
                SelectImportPipelineXmlPath,
                SelectExportPipelineXmlPath,
                SelectExportRecipeReviewBundlePath,
                BuildRecipeLayerCard,
                layerTitle => layerActivationController?.Activate(layerTitle) == true,
                (layerTitle, imagePath) => layerManagementController?.LoadImageIntoLayer(layerTitle, imagePath) == true,
                SelectToolMenu,
                CommitPendingRecipeStepEdit,
                OpenRecipeLlmXmlReview,
                SelectValidationSetImagePaths,
                SelectValidationSetFolderPath,
                SelectValidationSetReplacementImagePath,
                ConfirmDeleteValidationSet,
                OpenRecipePipelineReview,
                evidence => runEvidenceViewerController.Open(evidence),
                OpenRecipeImageListValidation,
                DecidePendingRecipeEdit,
                ValidateRecipeStepRoundTrip,
                SaveRecipeStepPipeline,
                confirmQualifiedSnapshotLifecycle:
                    ConfirmQualifiedSnapshotLifecycle,
                openQualifiedSnapshotEvidence:
                    OpenQualifiedSnapshotEvidence,
                openPipelineXmlSteps:
                    () => tabRecipePipelineXmlSteps.IsSelected = true);
            AttachRecipeStepPropertyGridHost();
            ChromeCommands = new OpenVisionShellHostChromeCommandSurface(
                () => IsToolRailCompact = !IsToolRailCompact,
                commandController,
                toolWindowLifecycleController,
                toolWindowController,
                OpenGuidedSetupForTool);
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
                layerViewerController.CloseAll);
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
                chromeController.RefreshDirectRouteText);
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
            layerTestFacade = new OpenVisionShellHostLayerTestFacade(
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
                new OpenVisionShellHostLayerTestFacadeBindings
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
            toolTestFacade = new OpenVisionShellHostToolTestFacade(
                viewModel,
                statePresenter,
                documentController,
                toolWindowController,
                toolWindowLifecycleController,
                floatingToolWindowHost,
                dockedToolInspectorController,
                chromeController,
                refreshCoordinator,
                new OpenVisionShellHostToolTestFacadeBindings
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
        }

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

        public void Dispose()
        {
            recipeLlmBrowserAssistWebView?.Dispose();
            llmBrowserAssistController.Dispose();

            if (!sessionController.DisposeSession())
            {
                return;
            }

            GC.SuppressFinalize(this);
        }

}

}
