using OpenVisionLab.Vision2D.Pipeline;
using OpenVisionLab.Vision2D.Tool;
using OpenCvSharp;
using OpenVisionLab.Core;
using OpenVisionLab.Vision._1._Tools.OpenCV;
using System;
using System.Collections.Generic;
using System.Windows;
using static OpenVisionLab.DEFINE;

namespace OpenVisionLab
{
    internal sealed class OpenVisionNativeToolDocument : IDisposable
    {
        private readonly IDisplayManager displayManager;
        private readonly ISingleInputVisionToolWpfView view;
        private readonly IArithmeticVisionToolWpfView arithmeticView;
        private readonly FrameworkElement element;
        private readonly string toolName;
        private readonly string defaultOutputLayer;
        private readonly Func<string, string, VisionPipelineStep> createStep;
        private readonly Func<Mat, VisionToolResult> executePreview;
        private readonly bool normalizeSingleChannelInput;
        private readonly OpenVisionNativePreviewLayerPublisher previewLayerPublisher;
        private readonly OpenVisionNativeLayerRouteController layerRouteController;
        private readonly OpenVisionNativeToolLayerViewController layerViewController;
        private readonly OpenVisionNativeToolRouteInteractionController routeInteractionController;
        private readonly OpenVisionNativePreviewExecutionController previewExecutionController;
        private readonly OpenVisionNativePreviewImageCommandController previewImageCommandController;
        private readonly OpenVisionNativePipelineCommandController pipelineCommandController;
        private readonly OpenVisionNativeRoiCommandController roiCommandController;
        private readonly OpenVisionNativeToolStatusPresenter statusPresenter;
        private readonly OpenVisionNativeToolEventBinder eventBinder;
        private VisionToolSingleInputPropertyToolShell nImageVerificationShell;
        private VisionToolLanguageChangeController nImageVerificationLanguageController;
        private OpenVisionRecipeContext recipeContext;
        private bool disposed;

        internal OpenVisionNativeToolDocument(
            IDisplayManager displayManager,
            ISingleInputVisionToolWpfView view,
            FrameworkElement element,
            string toolName,
            string defaultOutputLayer,
            Func<IVisionTool> createTool,
            Func<string, string, VisionPipelineStep> createStep,
            bool normalizeSingleChannelInput = true)
            : this(displayManager, view, element, toolName, defaultOutputLayer, source => createTool().Execute(source), createStep, normalizeSingleChannelInput)
        {
        }

        internal OpenVisionNativeToolDocument(
            IDisplayManager displayManager,
            ISingleInputVisionToolWpfView view,
            FrameworkElement element,
            string toolName,
            string defaultOutputLayer,
            Func<Mat, VisionToolResult> executePreview,
            Func<string, string, VisionPipelineStep> createStep,
            bool normalizeSingleChannelInput = true)
        {
            this.displayManager = displayManager ?? throw new ArgumentNullException(nameof(displayManager));
            previewLayerPublisher = new OpenVisionNativePreviewLayerPublisher(this.displayManager);
            this.view = view ?? throw new ArgumentNullException(nameof(view));
            this.element = element ?? throw new ArgumentNullException(nameof(element));
            statusPresenter = new OpenVisionNativeToolStatusPresenter(this.view, null);
            this.toolName = string.IsNullOrWhiteSpace(toolName) ? element.GetType().Name : toolName;
            this.defaultOutputLayer = string.IsNullOrWhiteSpace(defaultOutputLayer)
                ? this.toolName + "_Preview"
                : defaultOutputLayer;
            this.createStep = createStep;
            layerRouteController = new OpenVisionNativeLayerRouteController(
                this.displayManager,
                this.defaultOutputLayer,
                () => view?.SelectedInputLayer,
                () => view?.SelectedOutputLayer);
            layerViewController = new OpenVisionNativeToolLayerViewController(this.displayManager, layerRouteController, previewLayerPublisher);
            routeInteractionController = new OpenVisionNativeToolRouteInteractionController(layerRouteController, layerViewController, NotifyLayerStateChanged, SetStatus);
            previewExecutionController = new OpenVisionNativePreviewExecutionController(this.displayManager, previewLayerPublisher);
            previewImageCommandController = new OpenVisionNativePreviewImageCommandController(
                this.displayManager,
                previewLayerPublisher,
                this.element,
                ResolveLayerForPreviewRole,
                PrepareLayerForPreviewLoadRole,
                ResolvePrimaryInputLayer,
                ClearPreviewResult,
                RefreshLayerState,
                SetStatus);
            pipelineCommandController = new OpenVisionNativePipelineCommandController(
                this.toolName,
                ResolveInputLayer,
                ResolveOutputLayer,
                createStep,
                null,
                SetStatus,
                ResolveRecipeContext);
            roiCommandController = new OpenVisionNativeRoiCommandController(
                this.displayManager,
                this.element,
                ResolveInputLayer,
                SetStatus);
            this.executePreview = executePreview ?? throw new ArgumentNullException(nameof(executePreview));
            this.normalizeSingleChannelInput = normalizeSingleChannelInput;
            eventBinder = OpenVisionNativeToolEventBinder.BindSingle(
                view,
                OnSourceLayerChanged,
                OnDestinationLayerChanged,
                OnInputPreviewClicked,
                OnOutputPreviewClicked,
                OnCreateOutputLayerRequested,
                OnRunPreviewRequested,
                OnAddPipelineRequested,
                OnLoadPreviewImageRequested,
                OnSavePreviewImageRequested,
                OnLineEditSelectedRoiRequested);
            ConfigureNImageVerification();
            RefreshLayerState();
        }

        internal OpenVisionNativeToolDocument(
            IDisplayManager displayManager,
            IArithmeticVisionToolWpfView arithmeticView,
            FrameworkElement element,
            string toolName,
            string defaultOutputLayer)
        {
            this.displayManager = displayManager ?? throw new ArgumentNullException(nameof(displayManager));
            previewLayerPublisher = new OpenVisionNativePreviewLayerPublisher(this.displayManager);
            this.arithmeticView = arithmeticView ?? throw new ArgumentNullException(nameof(arithmeticView));
            this.element = element ?? throw new ArgumentNullException(nameof(element));
            statusPresenter = new OpenVisionNativeToolStatusPresenter(null, this.arithmeticView);
            this.toolName = string.IsNullOrWhiteSpace(toolName) ? element.GetType().Name : toolName;
            this.defaultOutputLayer = string.IsNullOrWhiteSpace(defaultOutputLayer)
                ? this.toolName + "_Preview"
                : defaultOutputLayer;
            layerRouteController = new OpenVisionNativeLayerRouteController(
                this.displayManager,
                this.defaultOutputLayer,
                null,
                null,
                () => arithmeticView?.SelectedInputLayerA,
                () => arithmeticView?.SelectedInputLayerB,
                () => arithmeticView?.SelectedOutputLayer);
            layerViewController = new OpenVisionNativeToolLayerViewController(this.displayManager, layerRouteController, previewLayerPublisher);
            routeInteractionController = new OpenVisionNativeToolRouteInteractionController(layerRouteController, layerViewController, NotifyLayerStateChanged, SetStatus);
            previewExecutionController = new OpenVisionNativePreviewExecutionController(this.displayManager, previewLayerPublisher);
            previewImageCommandController = new OpenVisionNativePreviewImageCommandController(
                this.displayManager,
                previewLayerPublisher,
                this.element,
                ResolveLayerForPreviewRole,
                PrepareLayerForPreviewLoadRole,
                ResolvePrimaryInputLayer,
                ClearPreviewResult,
                RefreshLayerState,
                SetStatus);
            pipelineCommandController = new OpenVisionNativePipelineCommandController(
                this.toolName,
                null,
                null,
                null,
                CreateArithmeticStep,
                SetStatus,
                ResolveRecipeContext);
            eventBinder = OpenVisionNativeToolEventBinder.BindArithmetic(
                arithmeticView,
                OnArithmeticInputALayerChanged,
                OnArithmeticInputBLayerChanged,
                OnArithmeticOutputLayerChanged,
                OnArithmeticInputAPreviewClicked,
                OnArithmeticInputBPreviewClicked,
                OnArithmeticOutputPreviewClicked,
                OnArithmeticCreateOutputLayerRequested,
                OnArithmeticRunPreviewRequested,
                OnArithmeticRunOffsetRequested,
                OnAddPipelineRequested,
                OnLoadPreviewImageRequested,
                OnSavePreviewImageRequested);
            RefreshLayerState();
        }

        public FrameworkElement View => element;
        public string ToolName => toolName;
        public string ActiveViewTypeName => element.GetType().Name;
        public string RouteInputLayerName => arithmeticView == null ? ResolveInputLayer() : ResolveArithmeticInputLayerA();
        public string RouteInputLayerBName => arithmeticView == null ? string.Empty : ResolveArithmeticInputLayerB();
        public string RouteOutputLayerName => arithmeticView == null ? ResolveOutputLayer() : ResolveArithmeticOutputLayer();
        public OpenVisionRecipeContext RecipeContext => ResolveRecipeContext();
        public string RecipeContextName => ResolveRecipeContext().Name;
        public string RecipeContextPipelineName => ResolveRecipeContext().PipelineName;
        public int PreviewRunCount { get; private set; }
        public bool HasPreviewResult { get; private set; }
        public string LastStatusText { get; private set; } = string.Empty;

        internal void SetPropertyPersistenceStatus(string status)
        {
            SetStatus(status);
        }

        public string ResultReviewText
        {
            get
            {
                if (view is LineToolWpfView lineView)
                {
                    return lineView.ResultReviewTextForTest;
                }

                if (view is MatchingToolWpfView matchingView)
                {
                    return matchingView.ResultReviewTextForTest;
                }

                if (view is EdgeBasedMatchingToolWpfView edgeBasedMatchingView)
                {
                    return edgeBasedMatchingView.ResultReviewTextForTest;
                }

                if (view is FeatureMatchingToolWpfView featureMatchingView)
                {
                    return featureMatchingView.ResultReviewTextForTest;
                }

                if (view is SimplePreprocessToolWpfView simplePreprocessView)
                {
                    return simplePreprocessView.ResultReviewTextForTest;
                }

                if (view is AffineTransformToolWpfView affineTransformView)
                {
                    return affineTransformView.ResultReviewTextForTest;
                }

                return string.Empty;
            }
        }
        public int LineInputRoiOverlayCount => view is LineToolWpfView lineView ? lineView.InputPreviewRoiOverlayCount : 0;
        public bool LineSignalInspectorHasEvidence =>
            view is LineToolWpfView lineView && lineView.SignalInspectorHasEvidenceForTest;
        public bool LineSignalInspectorOverlayVisible =>
            view is LineToolWpfView lineView && lineView.IsSignalInspectorOverlayVisibleForTest;
        public string LineSignalInspectorEvidenceId =>
            view is LineToolWpfView lineView ? lineView.SignalInspectorEvidenceIdForTest : string.Empty;
        public string LineSignalInspectorSourceSha256 =>
            view is LineToolWpfView lineView ? lineView.SignalInspectorSourceSha256ForTest : string.Empty;
        public int LineSignalInspectorSeriesCount =>
            view is LineToolWpfView lineView ? lineView.SignalInspectorSeriesCountForTest : 0;
        public int LineSignalInspectorMarkerCount =>
            view is LineToolWpfView lineView ? lineView.SignalInspectorMarkerCountForTest : 0;
        public event EventHandler LayerStateChanged = delegate { };
        private bool showOutputWorkspacePreviewOnNextLayerStateChanged;

        public bool LoadPreviewImageFromFileForTest(VisionToolPreviewImageRole role, string path)
        {
            return previewImageCommandController.LoadFromFile(role, path);
        }

        public bool SavePreviewImageToFileForTest(VisionToolPreviewImageRole role, string path)
        {
            return previewImageCommandController.SaveToFile(role, path);
        }

        public static bool TryCreate(VISION_MENU menu, IDisplayManager displayManager, out OpenVisionNativeToolDocument document)
        {
            return OpenVisionNativeToolDocumentFactory.TryCreate(menu, displayManager, out document);
        }

        public void ApplyRecipeContext(OpenVisionRecipeContext context)
        {
            recipeContext = context ?? CreateDefaultRecipeContext();
        }

        public void RefreshLayerState()
        {
            if (arithmeticView != null)
            {
                routeInteractionController.RefreshArithmeticLayerState(arithmeticView);
            }
            else
            {
                routeInteractionController.RefreshSingleLayerState(view);
            }
        }

        public void InvalidatePreviewResultForInputChange()
        {
            ClearPreviewResult();
        }

        public void RunPreview()
        {
            if (arithmeticView != null)
            {
                RunArithmeticPreview(arithmeticView.UseOffsetMode);
                return;
            }

            OpenVisionNativePreviewExecutionResult result = previewExecutionController.RunSingleInput(
                ResolveInputLayer(),
                ResolveOutputLayer(),
                ResolvePrimaryInputLayer(),
                normalizeSingleChannelInput,
                executePreview);
            ApplyPreviewExecutionResult(result, () => routeInteractionController.RefreshSinglePreviews(view));
        }

        public void CreateOutputLayerForTest()
        {
            if (arithmeticView != null)
            {
                OnArithmeticCreateOutputLayerRequested(this, EventArgs.Empty);
                return;
            }

            OnCreateOutputLayerRequested(this, EventArgs.Empty);
        }

        public void SetLineRoiForTest(OpenCvSharp.Rect roi)
        {
            if (view is LineToolWpfView lineView)
            {
                lineView.SetRoiForTest(roi);
            }
        }

        public void SetSelectedLineRoiForTest(OpenCvSharp.Rect roi)
        {
            if (view is LineToolWpfView lineView)
            {
                lineView.ApplySelectedLineRoi(roi);
            }
        }

        public void SetLineSettingForTest(string setting)
        {
            if (view is LineToolWpfView lineView)
            {
                lineView.SetLineSettingForTest(setting);
            }
        }

        public void ConfigureSelectedLineForTest(string projectionDirection, string polarity, string verticalDirection = null)
        {
            if (view is LineToolWpfView lineView)
            {
                lineView.ConfigureSelectedLineForTest(projectionDirection, polarity, verticalDirection);
            }
        }

        public void ConfigureSelectedLineDrawForTest(bool showVerticalLine, bool showEdge, bool showContour, bool showFitLine)
        {
            if (view is LineToolWpfView lineView)
            {
                lineView.ConfigureSelectedLineDrawForTest(showVerticalLine, showEdge, showContour, showFitLine);
            }
        }

        public void ConfigureSelectedLineThresholdForTest(double threshold, bool invert)
        {
            if (view is LineToolWpfView lineView)
            {
                lineView.ConfigureSelectedLineThresholdForTest(threshold, invert);
            }
        }

        public void ConfigureSelectedLineMeasureTuningForTest(
            bool useThreshold,
            bool useAdaptiveThreshold,
            double contrast,
            double thickness,
            double samplingStep,
            int pointRange,
            bool useManualAngle,
            double manualAngleValue)
        {
            if (view is LineToolWpfView lineView)
            {
                lineView.ConfigureSelectedLineMeasureTuningForTest(
                    useThreshold,
                    useAdaptiveThreshold,
                    contrast,
                    thickness,
                    samplingStep,
                    pointRange,
                    useManualAngle,
                    manualAngleValue);
            }
        }

        public void SetLinePurposeForTest(string purpose)
        {
            if (view is LineToolWpfView lineView)
            {
                lineView.SetPurposeForTest(purpose);
            }
        }

        public void SetMatchingTemplatePathForTest(string path)
        {
            if (view is MatchingToolWpfView matchingView)
            {
                matchingView.SetTemplatePathForTest(path);
            }
        }

        public string GetLineSignalInspectorAttributeForTest(string name)
        {
            return view is LineToolWpfView lineView
                ? lineView.GetSignalInspectorAttributeForTest(name)
                : string.Empty;
        }

        public bool ExerciseLineSignalInspectorNavigationForTest()
        {
            return view is LineToolWpfView lineView
                && lineView.ExerciseSignalInspectorNavigationForTest();
        }

        public void ExportLineSignalEvidenceForTest(string path)
        {
            if (view is LineToolWpfView lineView)
            {
                lineView.ExportSignalEvidenceForTest(path);
            }
        }

        public void CloseLineSignalInspectorForTest()
        {
            if (view is LineToolWpfView lineView)
            {
                lineView.CloseSignalInspectorForTest();
            }
        }

        public void OpenLineSignalInspectorForTest()
        {
            if (view is LineToolWpfView lineView)
            {
                lineView.OpenSignalInspectorForTest();
            }
        }

        public void ConfigureMatchingForTest(Action<MatchingProperty> configure)
        {
            if (view is MatchingToolWpfView matchingView)
            {
                matchingView.ConfigurePropertyForTest(configure);
            }
        }

        internal bool ApplySampleStepParameters(VisionPipelineStep step)
        {
            if (step == null)
            {
                return false;
            }

            object sampleProperty = VisionPipelineStepPropertyMapper.CreateProperty(step);
            if (view is MatchingToolWpfView matchingView
                && sampleProperty is MatchingProperty matching)
            {
                matchingView.ApplySampleProperty(matching);
                return true;
            }

            if (view is LineToolWpfView lineView
                && VisionPipelineLinePropertyAdapter.TryCreateLineGaugePair(
                    sampleProperty,
                    out LineGaugeProperty lineA,
                    out LineGaugeProperty lineB))
            {
                string purpose = string.Equals(step.ToolType, "LineIntersection", StringComparison.OrdinalIgnoreCase)
                    ? "Intersection"
                    : "Measure";
                lineView.ApplySampleLinePair(lineA, lineB, purpose);
                return true;
            }

            return false;
        }

        public void ConfigureAffineTransformForTest(Action<AffineTransformProperty> configure)
        {
            if (view is AffineTransformToolWpfView affineTransformView)
            {
                affineTransformView.ConfigurePropertyForTest(configure);
            }
        }

        public void SetEdgeBasedMatchingTemplatePathForTest(string path)
        {
            if (view is EdgeBasedMatchingToolWpfView edgeBasedMatchingView)
            {
                edgeBasedMatchingView.SetTemplatePathForTest(path);
            }
        }

        public void ConfigureEdgeBasedMatchingForTest(Action<EdgeBasedMatchingProperty> configure)
        {
            if (view is EdgeBasedMatchingToolWpfView edgeBasedMatchingView)
            {
                edgeBasedMatchingView.ConfigurePropertyForTest(configure);
            }
        }

        public void SetAutoMPointRepresentativeImagesForTest(IEnumerable<string> paths)
        {
            if (view is EdgeBasedMatchingToolWpfView edgeBasedMatchingView)
            {
                edgeBasedMatchingView.SetAutoMPointRepresentativeImagesForTest(paths);
            }
        }

        public void SetFeatureMatchingTemplatePathForTest(string path)
        {
            if (view is FeatureMatchingToolWpfView featureMatchingView)
            {
                featureMatchingView.SetTemplatePathForTest(path);
            }
        }

        public VisionPipelineStep AddPipelineStep()
        {
            return arithmeticView != null
                ? pipelineCommandController.AddArithmeticStep()
                : pipelineCommandController.AddSingleInputStep();
        }

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            disposed = true;
            if (nImageVerificationShell != null)
            {
                nImageVerificationShell.NImageVerificationButton.Click -= OnNImageVerificationRequested;
                nImageVerificationShell = null;
            }

            nImageVerificationLanguageController?.Dispose();
            nImageVerificationLanguageController = null;
            eventBinder.Dispose();
            DisposeHostedView();
        }

        private void ConfigureNImageVerification()
        {
            if (createStep == null
                || element.FindName("toolShell") is not VisionToolSingleInputPropertyToolShell shell)
            {
                return;
            }

            nImageVerificationShell = shell;
            shell.NImageVerificationButton.Visibility = Visibility.Visible;
            shell.NImageVerificationButton.Click += OnNImageVerificationRequested;
            ApplyNImageVerificationLocalization();
            nImageVerificationLanguageController =
                VisionToolLanguageChangeController.Attach(ApplyNImageVerificationLocalization);
        }

        private void ApplyNImageVerificationLocalization()
        {
            if (nImageVerificationShell == null)
            {
                return;
            }

            string text = OpenVisionLanguageService.T("ToolView.NImageVerification");
            string tooltip = OpenVisionLanguageService.T("ToolView.NImageVerification.ToolTip");
            nImageVerificationShell.NImageVerificationText.Text = text;
            VisionToolChromePresenter.ApplyTooltip(
                nImageVerificationShell.NImageVerificationButton,
                tooltip);
        }

        private void OnNImageVerificationRequested(object sender, RoutedEventArgs e)
        {
            if (disposed || createStep == null)
            {
                return;
            }

            VisionToolNImageVerificationController controller =
                new VisionToolNImageVerificationController(
                    toolName,
                    ResolveRecipeContext().Name,
                    () => createStep("Main", "NImageResult"),
                    normalizeSingleChannelInput);
            VisionToolNImageVerificationWindow window =
                new VisionToolNImageVerificationWindow(controller);
            System.Windows.Window owner = System.Windows.Window.GetWindow(element);
            if (owner != null)
            {
                window.Owner = owner;
            }

            window.ShowDialog();
        }

        private void DisposeHostedView()
        {
            // Native tool views are cached and may be temporarily removed/reparented by WPF.
            // Only the document cache owns their real lifetime; disposing on View.Unloaded
            // clears hosted PropertyGrid controls and makes reopened tools appear empty.
            if (element is IVisionToolViewLifetime elementLifetime)
            {
                elementLifetime.DisposeView();
                return;
            }

            if (view is IVisionToolViewLifetime viewLifetime)
            {
                viewLifetime.DisposeView();
                return;
            }

            if (arithmeticView is IVisionToolViewLifetime arithmeticViewLifetime)
            {
                arithmeticViewLifetime.DisposeView();
            }
        }

        private void OnLoadPreviewImageRequested(object sender, VisionToolPreviewImageCommandEventArgs e)
        {
            previewImageCommandController.LoadWithDialog(e.Role);
        }

        private void OnSavePreviewImageRequested(object sender, VisionToolPreviewImageCommandEventArgs e)
        {
            previewImageCommandController.SaveWithDialog(e.Role);
        }

        private void OnSourceLayerChanged(object sender, EventArgs e)
        {
            routeInteractionController.HandleSingleInputLayerChanged(view);
        }

        private void OnDestinationLayerChanged(object sender, EventArgs e)
        {
            routeInteractionController.HandleSingleOutputLayerChanged(view);
        }

        private void OnInputPreviewClicked(object sender, EventArgs e)
        {
            routeInteractionController.HandleSingleInputPreviewClicked();
        }

        private void OnOutputPreviewClicked(object sender, EventArgs e)
        {
            routeInteractionController.HandleSingleOutputPreviewClicked();
        }

        private void OnCreateOutputLayerRequested(object sender, EventArgs e)
        {
            routeInteractionController.HandleSingleCreateOutputLayerRequested(view);
        }

        private void OnLineEditSelectedRoiRequested(object sender, EventArgs e)
        {
            roiCommandController?.EditSelectedLineRoi(view as LineToolWpfView);
        }

        private void OnRunPreviewRequested(object sender, EventArgs e)
        {
            RunPreview();
        }

        private void OnAddPipelineRequested(object sender, EventArgs e)
        {
            AddPipelineStep();
        }

        private void OnArithmeticInputALayerChanged(object sender, EventArgs e)
        {
            routeInteractionController.HandleArithmeticInputALayerChanged(arithmeticView);
        }

        private void OnArithmeticInputBLayerChanged(object sender, EventArgs e)
        {
            routeInteractionController.HandleArithmeticInputBLayerChanged(arithmeticView);
        }

        private void OnArithmeticOutputLayerChanged(object sender, EventArgs e)
        {
            routeInteractionController.HandleArithmeticOutputLayerChanged(arithmeticView);
        }

        private void OnArithmeticInputAPreviewClicked(object sender, EventArgs e)
        {
            routeInteractionController.HandleArithmeticInputAPreviewClicked();
        }

        private void OnArithmeticInputBPreviewClicked(object sender, EventArgs e)
        {
            routeInteractionController.HandleArithmeticInputBPreviewClicked();
        }

        private void OnArithmeticOutputPreviewClicked(object sender, EventArgs e)
        {
            routeInteractionController.HandleArithmeticOutputPreviewClicked();
        }

        private void OnArithmeticCreateOutputLayerRequested(object sender, EventArgs e)
        {
            routeInteractionController.HandleArithmeticCreateOutputLayerRequested(arithmeticView);
        }

        private void OnArithmeticRunPreviewRequested(object sender, EventArgs e)
        {
            RunArithmeticPreview(false);
        }

        private void OnArithmeticRunOffsetRequested(object sender, EventArgs e)
        {
            RunArithmeticPreview(true);
        }

        private string ResolveInputLayer()
        {
            return layerRouteController.ResolveInputLayer();
        }

        private string ResolveOutputLayer()
        {
            return layerRouteController.ResolveOutputLayer();
        }

        private OpenVisionRecipeContext ResolveRecipeContext()
        {
            return recipeContext ?? CreateDefaultRecipeContext();
        }

        private static OpenVisionRecipeContext CreateDefaultRecipeContext()
        {
            return new OpenVisionRecipeContext(
                id: "Default",
                name: "Default",
                pipelineName: VisionPipelineAppendService.DefaultPipelineName,
                sourcePath: string.Empty,
                isDirty: false,
                activeLayerName: "Main",
                lastReviewState: string.Empty);
        }


        private string ResolveLayerForPreviewRole(VisionToolPreviewImageRole role)
        {
            if (arithmeticView != null)
            {
                return role switch
                {
                    VisionToolPreviewImageRole.InputA => ResolveArithmeticInputLayerA(),
                    VisionToolPreviewImageRole.InputB => ResolveArithmeticInputLayerB(),
                    VisionToolPreviewImageRole.Output => ResolveArithmeticOutputLayer(),
                    _ => ResolveArithmeticInputLayerA()
                };
            }

            return role == VisionToolPreviewImageRole.Output ? ResolveOutputLayer() : ResolveInputLayer();
        }

        private string PrepareLayerForPreviewLoadRole(VisionToolPreviewImageRole role)
        {
            if (arithmeticView != null && role == VisionToolPreviewImageRole.InputB)
            {
                return layerRouteController.SelectArithmeticInputLayerBLoadTarget();
            }

            return ResolveLayerForPreviewRole(role);
        }

        private string ResolvePrimaryInputLayer()
        {
            return arithmeticView == null ? ResolveInputLayer() : ResolveArithmeticInputLayerA();
        }

        private void RunArithmeticPreview(bool useOffsetMode)
        {
            VisionPipelineStep step = CreateArithmeticStep(useOffsetMode);
            OpenVisionNativePreviewExecutionResult result = previewExecutionController.RunArithmetic(
                step,
                ResolveArithmeticInputLayerA(),
                ResolveArithmeticOutputLayer(),
                ResolvePrimaryInputLayer(),
                useOffsetMode);
            ApplyPreviewExecutionResult(result, () => routeInteractionController.RefreshArithmeticPreviews(arithmeticView));
        }

        private void ClearPreviewResult()
        {
            HasPreviewResult = false;
            if (view is LineToolWpfView lineView)
            {
                lineView.ClearSignalEvidence();
            }
        }

        private void ApplyPreviewExecutionResult(OpenVisionNativePreviewExecutionResult result, Action refreshPreviews)
        {
            if (result == null)
            {
                SetStatus("Preview NG / tool returned no result");
                return;
            }

            if (!result.Success)
            {
                SetStatus(result.Status);
                return;
            }

            refreshPreviews?.Invoke();
            PreviewRunCount++;
            HasPreviewResult = true;
            SetStatus(result.Status);
            showOutputWorkspacePreviewOnNextLayerStateChanged = true;
            RefreshLayerState();
        }

        private VisionPipelineStep CreateArithmeticStep()
        {
            return CreateArithmeticStep(arithmeticView.UseOffsetMode);
        }

        private VisionPipelineStep CreateArithmeticStep(bool useOffsetMode)
        {
            string inputLayerA = ResolveArithmeticInputLayerA();
            string inputLayerB = useOffsetMode ? string.Empty : ResolveArithmeticInputLayerB();
            string outputLayer = ResolveArithmeticOutputLayer();
            return VisionPipelineStepBuilder.FromArithmetic(
                "Arithmetic",
                arithmeticView.SelectedArithmeticType,
                inputLayerA,
                inputLayerB,
                outputLayer,
                !useOffsetMode && arithmeticView.UseConstantInput,
                !useOffsetMode && arithmeticView.UseColorConstant,
                arithmeticView.GetGrayValue(1),
                arithmeticView.GetBValue(1),
                arithmeticView.GetGValue(1),
                arithmeticView.GetRValue(1),
                arithmeticView.GetOffsetX(1),
                arithmeticView.GetOffsetY(1),
                useOffsetMode ? VisionPipelineArithmeticStep.ModeOffset : VisionPipelineArithmeticStep.ModeOperation);
        }

        private string ResolveArithmeticInputLayerA()
        {
            return layerRouteController.ResolveArithmeticInputLayerA();
        }

        private string ResolveArithmeticInputLayerB()
        {
            return layerRouteController.ResolveArithmeticInputLayerB();
        }

        private string ResolveArithmeticOutputLayer()
        {
            return layerRouteController.ResolveArithmeticOutputLayer();
        }

        private void SetStatus(string status)
        {
            LastStatusText = status ?? string.Empty;
            statusPresenter.Present(status);
        }

        private void NotifyLayerStateChanged()
        {
            bool showOutputWorkspacePreview = showOutputWorkspacePreviewOnNextLayerStateChanged;
            showOutputWorkspacePreviewOnNextLayerStateChanged = false;
            LayerStateChanged(this, new OpenVisionNativeToolLayerStateChangedEventArgs(showOutputWorkspacePreview));
        }

    }
}
