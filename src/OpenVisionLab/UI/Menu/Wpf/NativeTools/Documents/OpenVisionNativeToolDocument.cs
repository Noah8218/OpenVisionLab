using OpenVisionLab.Vision2D.Pipeline;
using OpenVisionLab.Vision2D.Tool;
using OpenCvSharp;
using Bitmap = System.Drawing.Bitmap;
using OpenVisionLab.Core;
using OpenVisionLab.Vision._1._Tools.OpenCV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
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
        private readonly Func<OpenVisionNativePreviewOperation> capturePreview;
        private readonly bool normalizeSingleChannelInput;
        private readonly OpenVisionNativePreviewLayerPublisher previewLayerPublisher;
        private readonly OpenVisionNativeLayerRouteController layerRouteController;
        private readonly OpenVisionNativeToolLayerViewController layerViewController;
        private readonly OpenVisionNativeToolRouteInteractionController routeInteractionController;
        private readonly OpenVisionNativePreviewExecutionController previewExecutionController;
        private readonly OpenVisionNativePreviewExecutionBoundary previewExecutionBoundary;
        private readonly OpenVisionNativePreviewImageCommandController previewImageCommandController;
        private readonly OpenVisionNativePipelineCommandController pipelineCommandController;
        private readonly OpenVisionNativeRoiCommandController roiCommandController;
        private readonly OpenVisionNativeToolStatusPresenter statusPresenter;
        private readonly OpenVisionNativeToolEventBinder eventBinder;
        private Func<Mat, VisionToolResult> previewExecutionOverrideForTest;
        private VisionToolSingleInputPropertyToolShell nImageVerificationShell;
        private VisionToolLanguageChangeController nImageVerificationLanguageController;
        private OpenVisionRecipeContext recipeContext;
        private Func<string, VisionToolPreviewImageRole, string, bool> openPreviewViewer;
        private Func<string, VisionToolPreviewImageRole, string, bool> refreshPreviewViewer;
        private Action closePreviewViewer;
        private VisionToolPreviewImageRole? openPreviewViewerRole;
        private bool disposed;
        private OpenVisionWorkbenchToolViewModel workbenchEditor;
        private long workbenchSession;
        private OpenVisionWorkbenchViewModel workbenchRoot;
        private bool nImageWindowOpen;
        private bool isWorkbenchDocument;
        private bool restoringParameters;
        private bool refreshingLayerState;
        private bool capturingParameters;
        private Func<object> captureParameters;
        private Action<object> restoreParameters;
        private Func<OpenVisionRecipeContext, object> loadParameters;
        private Action<VisionPipelineStep> applyStepParameters;
        private Func<bool> templateReady = () => true;
        private Func<string, string, VisionPipelineStep> captureParameterStep;
        private Func<object> createDefaultParameters;
        private Func<bool> autoPreviewOptedIn;
        private Func<OpenVisionLab.Contracts.VisionToolTemplateStatus> getTemplateStatus;
        private Func<System.Windows.Media.ImageSource> getTemplateThumbnail;
        private Func<string, bool> applyTemplatePath;
        private Func<bool, string> templateSelectionOverrideForTest;

        internal OpenVisionNativeToolDocument(
            IDisplayManager displayManager,
            ISingleInputVisionToolWpfView view,
            FrameworkElement element,
            string toolName,
            string defaultOutputLayer,
            Func<Mat, VisionToolResult> executePreview,
            Func<string, string, VisionPipelineStep> createStep,
            bool normalizeSingleChannelInput = true,
            Func<OpenVisionNativePreviewOperation> capturePreview = null)
        {
            this.displayManager = displayManager ?? throw new ArgumentNullException(nameof(displayManager));
            previewLayerPublisher = new OpenVisionNativePreviewLayerPublisher(this.displayManager);
            this.view = view ?? throw new ArgumentNullException(nameof(view));
            this.element = element ?? throw new ArgumentNullException(nameof(element));
            this.element.AddHandler(
                VisionToolPreviewSlotBehavior.OpenPreviewImageRequestedEvent,
                new EventHandler<VisionToolPreviewImageOpenRequestedEventArgs>(OnPreviewImageOpenRequested));
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
            previewExecutionController = new OpenVisionNativePreviewExecutionController(this.displayManager, previewLayerPublisher, AcquireInputSnapshot);
            previewExecutionBoundary = new OpenVisionNativePreviewExecutionBoundary(
                previewExecutionController,
                action => this.element.Dispatcher.Invoke(action),
                QueueWorkbenchExecutionStateChanged);
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
                NotifyLayerStateChanged,
                ResolveRecipeContext);
            roiCommandController = new OpenVisionNativeRoiCommandController(
                this.displayManager,
                this.element,
                ResolveInputLayer,
                SetStatus);
            this.executePreview = executePreview;
            this.capturePreview = capturePreview;
            if (executePreview == null && capturePreview == null)
            {
                throw new ArgumentNullException(nameof(executePreview));
            }
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
            OpenVisionLanguageService.LanguageChanged += OnPreviewViewerLanguageChanged;
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
            this.element.AddHandler(
                VisionToolPreviewSlotBehavior.OpenPreviewImageRequestedEvent,
                new EventHandler<VisionToolPreviewImageOpenRequestedEventArgs>(OnPreviewImageOpenRequested));
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
            previewExecutionController = new OpenVisionNativePreviewExecutionController(this.displayManager, previewLayerPublisher, AcquireInputSnapshot);
            previewExecutionBoundary = null;
            previewImageCommandController = new OpenVisionNativePreviewImageCommandController(
                this.displayManager,
                previewLayerPublisher,
                this.element,
                ResolveLayerForPreviewRole,
                PrepareLayerForPreviewLoadRole,
                ResolvePrimaryInputLayer,
                ClearPreviewResult,
                RefreshLayerState,
                SetStatus,
                OnPreviewImageLoaded);
            pipelineCommandController = new OpenVisionNativePipelineCommandController(
                this.toolName,
                null,
                null,
                null,
                CreateArithmeticStep,
                SetStatus,
                NotifyLayerStateChanged,
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
        internal bool IsPreviewRunning => previewExecutionBoundary?.IsRunning == true;
        internal VisionPipelineStep WorkbenchParameterStep => isWorkbenchDocument && workbenchEditor?.IsActive(workbenchSession) == true ? workbenchEditor.CurrentStep : null;

        internal void ConfigureParameterState(bool forWorkbench, Func<object> capture, Action<object> restore,
            Func<OpenVisionRecipeContext, object> load, Action<VisionPipelineStep> applyStep, Func<bool> canUseTemplate = null,
            Func<string, string, VisionPipelineStep> captureStep = null, Func<object> createDefaults = null, Func<bool> autoPreviewOptedIn = null)
        {
            isWorkbenchDocument = forWorkbench;
            captureParameters = capture ?? throw new ArgumentNullException(nameof(capture));
            restoreParameters = restore ?? throw new ArgumentNullException(nameof(restore));
            loadParameters = load ?? throw new ArgumentNullException(nameof(load));
            applyStepParameters = applyStep ?? throw new ArgumentNullException(nameof(applyStep));
            templateReady = canUseTemplate ?? (() => true);
            captureParameterStep = captureStep;
            createDefaultParameters = createDefaults;
            this.autoPreviewOptedIn = autoPreviewOptedIn;
        }

        internal void ConfigureTemplateState(Func<OpenVisionLab.Contracts.VisionToolTemplateStatus> status,
            Func<System.Windows.Media.ImageSource> thumbnail, Func<string, bool> applyPath)
        {
            getTemplateStatus = status;
            getTemplateThumbnail = thumbnail;
            applyTemplatePath = applyPath;
        }

        internal void SetTemplateSelectionForTest(Func<bool, string> select) => templateSelectionOverrideForTest = select;

        private bool? ChangeTemplate(bool fromFile)
        {
            using IDisposable imageEditScope = fromFile ? null : PropertyGridEditorFactory.BeginImageEditWriteScope(element);
            if (disposed || workbenchEditor?.CanEditParameters != true || applyTemplatePath == null) { return null; }
            OpenVisionWorkbenchToolViewModel editor = workbenchEditor;
            long session = workbenchSession;
            Action<string> editAction = PropertyGridEditorFactory.GetEditActionCallback(element);
            VisionToolDebouncedPreviewScheduler.CancelPending(element);
            try
            {
                CaptureStep();
                editor.ParametersChanged(session);
                string path = templateSelectionOverrideForTest != null ? templateSelectionOverrideForTest(fromFile)
                    : PropertyGridEditorFactory.SelectTemplate(fromFile, getTemplateStatus().Path, System.Windows.Window.GetWindow(element), true, editAction, element);
                if (string.IsNullOrWhiteSpace(path) || !editor.IsActive(session)) { return null; }
                if (!applyTemplatePath(path)) { editAction?.Invoke("Workbench.Status.Error"); return false; }
                editor.ParametersChanged(session);
                editAction?.Invoke("Workbench.Action.TemplateRegistered");
                return true;
            }
            catch (Exception exception) { editAction?.Invoke("Workbench.Status.Error"); SetStatus(exception.GetBaseException().Message); return false; }
            finally { VisionToolDebouncedPreviewScheduler.CancelPending(element); }
        }

        internal void DeactivateWorkbenchEditor()
        {
            OpenVisionWorkbenchToolViewModel activeEditor = workbenchEditor;
            long activeSession = workbenchSession;
            if (workbenchEditor != null) { workbenchEditor.PropertyChanged -= OnNImageContextChanged; }
            if (workbenchRoot?.Images != null) { workbenchRoot.Images.PropertyChanged -= OnNImageContextChanged; }
            workbenchRoot = null;
            PropertyGridEditorFactory.SetEditActionCallback(element, null);
            PropertyGridEditorFactory.SetImageEditContext(element, null);
            workbenchEditor = null;
            workbenchSession = 0;
            activeEditor?.Deactivate(activeSession);
        }

        internal bool CanReuseWorkbenchEditor(OpenVisionWorkbenchToolViewModel editor, int selectedStep, OpenVisionWorkbenchViewModel theme) => !disposed
            && isWorkbenchDocument && ReferenceEquals(workbenchEditor, editor) && ReferenceEquals(workbenchRoot, theme)
            && editor?.IsEditingSession(workbenchSession, selectedStep) == true;

        internal bool ActivateWorkbenchEditor(OpenVisionWorkbenchToolViewModel editor, OpenVisionShellNavItem item, int selectedStep = -1, OpenVisionWorkbenchViewModel theme = null)
        {
            if (!isWorkbenchDocument || captureParameters == null) { return false; }
            VisionToolSingleInputPropertyToolShell parameterShell = element.FindName("toolShell") as VisionToolSingleInputPropertyToolShell;
            VisionToolDoubleInputCustomToolShell doubleParameterShell = element.FindName("toolShell") as VisionToolDoubleInputCustomToolShell;
            FrameworkElement themeShell = (FrameworkElement)parameterShell ?? doubleParameterShell;
            if (theme != null && themeShell != null) element.SetBinding(OpenVisionWorkbenchThemeBehavior.ThemeNameProperty, new Binding(nameof(theme.ThemeName)) { Source = theme });
            if (theme != null && themeShell != null) themeShell.SetBinding(OpenVisionWorkbenchThemeBehavior.ThemeNameProperty, new Binding(nameof(theme.ThemeName)) { Source = theme });
            if (workbenchEditor != null) { workbenchEditor.PropertyChanged -= OnNImageContextChanged; }
            workbenchEditor = editor ?? throw new ArgumentNullException(nameof(editor));
            if (workbenchRoot?.Images != null) { workbenchRoot.Images.PropertyChanged -= OnNImageContextChanged; }
            workbenchRoot = theme;
            editor.PropertyChanged -= OnNImageContextChanged;
            editor.PropertyChanged += OnNImageContextChanged;
            if (workbenchRoot?.Images != null) { workbenchRoot.Images.PropertyChanged += OnNImageContextChanged; }
            VisionToolDebouncedPreviewScheduler.CancelPending(element);
            workbenchSession = editor.Activate(ResolveRecipeContext(), new OpenVisionWorkbenchToolViewModel.EditorBinding
            {
                ToolName = toolName,
                DisplayName = () => OpenVisionShellCommandCatalog.DisplayName(item.Menu),
                EnglishName = OpenVisionLanguageService.GetEntries().FirstOrDefault(entry => entry.Key == "VisionMenu." + item.Menu)?.English ?? item.Menu.ToString(),
                Icon = item.IconKind,
                CaptureParameters = captureParameters,
                RestoreParameters = RestoreParameters,
                LoadParameters = loadParameters,
                CreateStep = CaptureStep,
                CommitPendingEdits = CommitPendingEdits,
                SupportsPipeline = createStep != null || arithmeticView != null,
                InitializeTestRoute = arithmeticView == null ? null : step =>
                    step.Parameters[VisionPipelineArithmeticStep.ParameterInputLayerB] = layerRouteController.ResolveDefaultArithmeticInputLayerB(step.InputLayer, step.OutputLayer),
                ApplyStep = RestoreStepParameters,
                TemplateReady = templateReady,
                GetTemplateStatus = getTemplateStatus,
                GetTemplateThumbnail = getTemplateThumbnail,
                ChangeTemplate = applyTemplatePath == null ? null : ChangeTemplate,
                IsRunning = () => IsPreviewRunning,
                AutoPreviewOptedIn = autoPreviewOptedIn,
                Run = RunPreview,
                CreateDefaults = createDefaultParameters,
                ClearResult = () => { ClearPreviewResult(); SetStatus(string.Empty); },
                ShowParameterGuide = parameterShell?.ParameterGuideVisibility == Visibility.Visible ? (Action)parameterShell.ToggleParameterGuide
                    : doubleParameterShell?.ParameterGuideVisibility == Visibility.Visible ? (Action)doubleParameterShell.ToggleParameterGuide : null
            }, selectedStep);
            long editSession = workbenchSession;
            PropertyGridEditorFactory.SetEditActionCallback(element, key => editor.NotifyEditAction(editSession, key));
            PropertyGridEditorFactory.SetImageEditContext(element, () => editor.IsActive(editSession));
            if (editor.SupportsAutoRun)
            {
                BindingOperations.SetBinding(element, VisionToolDebouncedPreviewScheduler.AllowAutoPreviewProperty,
                    new Binding(nameof(OpenVisionWorkbenchToolViewModel.IsAutoRunEnabled)) { Source = editor, Mode = BindingMode.OneWay });
            }
            if (element.FindName("toolShell") is VisionToolSingleInputPropertyToolShell singleShell)
            {
                VisionToolChromePresenter.ApplyWorkbenchEditor(singleShell.TitleText, singleShell.AddPipelineText, singleShell.RunPreviewText, singleShell.AddPipelineButton,
                    singleShell.RunPreviewButton, singleShell.WorkbenchStepActions, singleShell.WorkbenchDefaultsButton, editor, singleShell.NImageVerificationButton, singleShell.NImageVerificationText,
                    singleShell.WorkbenchRunShortcut, acceptanceHost: singleShell.WorkbenchAcceptanceHost);
                VisionToolChromePresenter.ApplyWorkbenchRoute(singleShell.InputLayerGroup, singleShell.OutputLayerGroup, editor);
                VisionToolChromePresenter.ApplyWorkbenchTemplate(singleShell, editor);
                VisionToolChromePresenter.ApplyWorkbenchResult(singleShell, editor);
            }
            else if (element.FindName("toolShell") is VisionToolDoubleInputCustomToolShell doubleShell)
            {
                VisionToolChromePresenter.ApplyWorkbenchEditor(doubleShell.TitleText, doubleShell.AddPipelineText, doubleShell.RunPreviewText, doubleShell.AddPipelineButton,
                    doubleShell.RunPreviewButton, doubleShell.WorkbenchStepActions, doubleShell.WorkbenchDefaultsButton, editor, runShortcut: doubleShell.WorkbenchRunShortcut,
                    alternateRunText: doubleShell.RunOffsetText, alternateRunShortcut: doubleShell.WorkbenchRunOffsetShortcut, alternateRunButton: doubleShell.RunOffsetButton);
                VisionToolChromePresenter.ApplyWorkbenchRoute(doubleShell.InputAGroup, doubleShell.OutputLayerGroup, editor, doubleShell.InputBGroup);
                VisionToolChromePresenter.ApplyWorkbenchResult(doubleShell, editor);
            }
            editor.RefreshRoute = RefreshLayerState;
            RefreshLayerState();
            return true;
        }

        internal void RecordParameterChange(Action legacySave)
        {
            if (restoringParameters || capturingParameters || disposed) { return; }
            if (!isWorkbenchDocument) { legacySave?.Invoke(); return; }
            workbenchEditor?.ParametersChanged(workbenchSession);
            RefreshWorkbenchArithmeticRoute();
        }

        private void QueueWorkbenchExecutionStateChanged()
        {
            if (disposed || !isWorkbenchDocument || workbenchEditor?.IsActive(workbenchSession) != true
                || element.Dispatcher.HasShutdownStarted || element.Dispatcher.HasShutdownFinished) { return; }
            OpenVisionWorkbenchToolViewModel editor = workbenchEditor;
            long session = workbenchSession;
            element.Dispatcher.BeginInvoke(new Action(() =>
            {
                if (!disposed && ReferenceEquals(workbenchEditor, editor) && workbenchSession == session) { editor.NotifyExecutionState(session); }
            }));
        }

        private VisionPipelineStep CaptureStep()
        {
            bool previous = capturingParameters;
            capturingParameters = true;
            try { return captureParameterStep != null ? captureParameterStep(ResolveInputLayer(), ResolveOutputLayer())
                : arithmeticView != null ? CreateArithmeticStep() : createStep?.Invoke(ResolveInputLayer(), ResolveOutputLayer()); }
            finally { capturingParameters = previous; }
        }

        private bool CommitPendingEdits()
        {
            bool previous = capturingParameters;
            capturingParameters = true;
            try { return VisionToolControlBinding.TryCommitPendingEdits(element); }
            finally { capturingParameters = previous; VisionToolDebouncedPreviewScheduler.CancelPending(element); }
        }

        private VisionPipelineStep CaptureWorkbenchResultStep(VisionPipelineStep captured = null)
        {
            if (workbenchEditor?.CurrentStep == null) { return null; }
            VisionPipelineStep actual;
            bool previous = capturingParameters;
            capturingParameters = true;
            try { actual = captured ?? createStep?.Invoke(ResolveInputLayer(), ResolveOutputLayer()) ?? CaptureStep(); }
            finally { capturingParameters = previous; }
            workbenchEditor.ParametersChanged(workbenchSession);
            VisionPipelineStep snapshot = VisionPipelineStepEditService.CopyStep(workbenchEditor.CurrentStep);
            if (snapshot == null) { return null; }
            if (actual?.Parameters != null)
            {
                snapshot.Parameters.Clear();
                foreach (var parameter in actual.Parameters) { snapshot.Parameters[parameter.Key] = parameter.Value; }
            }
            return snapshot;
        }

        private void RestoreParameters(object snapshot)
        {
            bool previous = restoringParameters;
            restoringParameters = true;
            VisionToolDebouncedPreviewScheduler.CancelPending(element);
            try { restoreParameters(snapshot); }
            finally { VisionToolDebouncedPreviewScheduler.CancelPending(element); restoringParameters = previous; }
        }

        private void RestoreStepParameters(VisionPipelineStep step)
        {
            bool previous = restoringParameters;
            restoringParameters = true;
            VisionToolDebouncedPreviewScheduler.CancelPending(element);
            try { applyStepParameters(step); }
            finally { VisionToolDebouncedPreviewScheduler.CancelPending(element); restoringParameters = previous; }
        }

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
        public bool LineSignalEvidenceCueVisible =>
            view is LineToolWpfView lineView && lineView.IsSignalEvidenceCueOpenForTest;
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
            VisionToolDebouncedPreviewScheduler.CancelPending(element);
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

        public void ConfigurePreviewViewer(
            Func<string, VisionToolPreviewImageRole, string, bool> open,
            Func<string, VisionToolPreviewImageRole, string, bool> refresh,
            Action close)
        {
            openPreviewViewer = open;
            refreshPreviewViewer = refresh;
            closePreviewViewer = close;
        }

        public void ClosePreviewViewer()
        {
            closePreviewViewer?.Invoke();
            openPreviewViewerRole = null;
        }

        private void RefreshWorkbenchArithmeticRoute()
        {
            if (arithmeticView == null || workbenchEditor?.IsActive(workbenchSession) != true
                || element.FindName("toolShell") is not VisionToolDoubleInputCustomToolShell shell) { return; }
            bool imageB = !arithmeticView.UseOffsetMode && !arithmeticView.UseConstantInput;
            shell.SetWorkbenchInputBRouteVisible(imageB);
            VisionToolChromePresenter.ApplyWorkbenchRoute(shell.InputAGroup, shell.OutputLayerGroup, workbenchEditor, imageB ? shell.InputBGroup : null);
        }

        public void RefreshLayerState()
        {
            if (refreshingLayerState) { return; }
            refreshingLayerState = true;
            try
            {
                if (workbenchEditor?.IsActive(workbenchSession) == true)
                {
                    RefreshWorkbenchArithmeticRoute();
                    string input = workbenchEditor.InputLayer;
                    string output = workbenchEditor.OutputLayer;
                    IReadOnlyList<string> layers = layerRouteController.GetWorkspaceLayerNames(input, workbenchEditor.InputLayerB, output);
                    IReadOnlyList<KeyValuePair<string, string>> inputChoices = workbenchEditor.GetInputLayerChoices(displayManager.GetLayerInfos().Select(layer => layer.Title));
                    if (element.FindName("toolShell") is FrameworkElement routeShell)
                    {
                        foreach (string name in new[] { "cbInputLayer", "cbInputA", "cbInputB" })
                        {
                            VisionToolLayerSelectionPresenter.SetWorkbenchInputChoices(routeShell.FindName(name) as ComboBox, inputChoices);
                        }
                    }
                    if (arithmeticView != null)
                    {
                        arithmeticView.SetLayerList(layers, input, workbenchEditor.InputLayerB, output);
                        using Bitmap inputA = AcquireInputSnapshot(input);
                        using Bitmap inputB = AcquireInputSnapshot(workbenchEditor.InputLayerB);
                        arithmeticView.SetInputAPreview(inputA);
                        arithmeticView.SetInputBPreview(inputB);
                        arithmeticView.SetOutputPreview(displayManager.GetLayerImage(output));
                    }
                    else
                    {
                        view.SetLayerList(layers, input, output);
                        using Bitmap inputImage = AcquireInputSnapshot(input);
                        view.SetInputPreview(inputImage);
                        view.SetOutputPreview(displayManager.GetLayerImage(output));
                    }
                    NotifyLayerStateChanged();
                    return;
                }
                if (arithmeticView != null)
                {
                    routeInteractionController.RefreshArithmeticLayerState(arithmeticView);
                }
                else
                {
                    routeInteractionController.RefreshSingleLayerState(view);
                }
            }
            finally { refreshingLayerState = false; }
        }

        public void InvalidatePreviewResultForInputChange()
        {
            bool wasRunning = previewExecutionBoundary?.IsRunning == true;
            ClearPreviewResult();
            workbenchEditor?.InvalidateResult(workbenchSession);
            if (wasRunning)
            {
                SetStatus("Preview CANCELED / " + VisionToolVerificationText.InspectionJudgmentNotEvaluated + " / input changed");
            }
        }

        internal void SetPreviewExecutionOverrideForTest(Func<Mat, VisionToolResult> previewExecutionOverride)
        {
            previewExecutionOverrideForTest = previewExecutionOverride;
        }

        public void RunPreview()
        {
            if (isWorkbenchDocument && (workbenchEditor?.IsActive(workbenchSession) != true || !workbenchEditor.CanUseToolActions)) { return; }
            if (restoringParameters || capturingParameters) { return; }
            if (isWorkbenchDocument && !workbenchEditor.CommitPendingEdits()) { return; }
            VisionToolDebouncedPreviewScheduler.CancelPending(element);
            if (arithmeticView != null)
            {
                RunArithmeticPreview(arithmeticView.UseOffsetMode);
                return;
            }

            if (previewExecutionBoundary.IsRunning)
            {
                SetStatus("Preview RUN / already running");
                return;
            }

            ClearPreviewResult();
            OpenVisionWorkbenchToolViewModel resultOwner = isWorkbenchDocument ? workbenchEditor : null;
            long resultSession = workbenchSession;
            VisionPipelineStep resultStep = resultOwner == null ? null : CaptureWorkbenchResultStep();
            previewExecutionBoundary.TryStartSingleInput(
                ResolveInputLayer(),
                ResolveOutputLayer(),
                ResolvePrimaryInputLayer(),
                normalizeSingleChannelInput,
                previewExecutionOverrideForTest ?? executePreview,
                result =>
                {
                    if (resultOwner != null && (!ReferenceEquals(workbenchEditor, resultOwner) || workbenchSession != resultSession || !resultOwner.IsActive(resultSession))) { return; }
                    PublishWorkbenchResult(resultOwner, resultSession, resultStep, result);
                    ApplyPreviewExecutionResult(result, () => routeInteractionController.RefreshSinglePreviews(view));
                    if (resultOwner != null && !result.IsTeachingPreview) { VisionToolChromePresenter.RevealWorkbenchResult(element.FindName("toolShell") as FrameworkElement); }
                },
                () =>
                {
                    SetStatus("Preview RUN / processing");
                    if (resultOwner != null && ReferenceEquals(workbenchEditor, resultOwner) && workbenchSession == resultSession && resultOwner.IsActive(resultSession))
                    {
                        resultOwner.NotifyExecutionStarted(resultSession, resultStep);
                    }
                },
                previewExecutionOverrideForTest == null ? capturePreview : null,
                resultStep);
            VisionToolDebouncedPreviewScheduler.CancelPending(element);
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
            if (isWorkbenchDocument)
            {
                VisionPipelineStep added = workbenchEditor?.AddCurrent(workbenchSession);
                if (added == null && workbenchEditor != null) { SetStatus(workbenchEditor.StatusText); }
                return added;
            }
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
            BindingOperations.ClearBinding(element, OpenVisionWorkbenchThemeBehavior.ThemeNameProperty);
            if (element.FindName("toolShell") is FrameworkElement themeShell) BindingOperations.ClearBinding(themeShell, OpenVisionWorkbenchThemeBehavior.ThemeNameProperty);
            DeactivateWorkbenchEditor();
            if (previewExecutionBoundary?.IsRunning == true)
            {
                SetStatus("Preview CANCELED / " + VisionToolVerificationText.InspectionJudgmentNotEvaluated + " / document disposed");
            }
            previewExecutionBoundary?.Dispose();
            previewExecutionOverrideForTest = null;
            ClosePreviewViewer();
            element.RemoveHandler(
                VisionToolPreviewSlotBehavior.OpenPreviewImageRequestedEvent,
                new EventHandler<VisionToolPreviewImageOpenRequestedEventArgs>(OnPreviewImageOpenRequested));
            OpenVisionLanguageService.LanguageChanged -= OnPreviewViewerLanguageChanged;
            if (nImageVerificationShell != null)
            {
                nImageVerificationShell.NImageVerificationButton.Click -= OnNImageVerificationRequested;
                nImageVerificationShell = null;
            }

            nImageVerificationLanguageController?.Dispose();
            if (workbenchEditor != null) { workbenchEditor.PropertyChanged -= OnNImageContextChanged; }
            if (workbenchRoot?.Images != null) { workbenchRoot.Images.PropertyChanged -= OnNImageContextChanged; }
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

            string text = OpenVisionLanguageService.T(isWorkbenchDocument ? "Workbench.Tool.NTest" : "ToolView.NImageVerification");
            string tooltip = isWorkbenchDocument ? workbenchEditor?.NImageToolTip ?? text : OpenVisionLanguageService.T("ToolView.NImageVerification.ToolTip");
            if (isWorkbenchDocument) { nImageVerificationShell.NImageVerificationButton.IsEnabled = workbenchEditor?.CanUseToolActions == true && (workbenchRoot?.Images?.Count ?? 0) > 0; }
            nImageVerificationShell.NImageVerificationText.Text = text;
            VisionToolChromePresenter.ApplyTooltip(
                nImageVerificationShell.NImageVerificationButton,
                tooltip);
        }

        private void OnNImageContextChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) => ApplyNImageVerificationLocalization();

        private void OnNImageVerificationRequested(object sender, RoutedEventArgs e)
        {
            if (disposed || nImageWindowOpen || createStep == null || (isWorkbenchDocument && (workbenchEditor?.IsNewTest != true || !workbenchEditor.CanUseToolActions)))
            {
                return;
            }

            if (isWorkbenchDocument && !workbenchEditor.CommitPendingEdits()) { return; }

            VisionToolNImageVerificationController controller =
                new VisionToolNImageVerificationController(
                    toolName,
                    ResolveRecipeContext().Name,
                    () => createStep("Main", "NImageResult"),
                    normalizeSingleChannelInput);
            if (isWorkbenchDocument)
            {
                var capturedEditor = workbenchEditor;
                long capturedSession = workbenchSession;
                string capturedContext = capturedEditor.NImageContextKey;
                var capturedImages = workbenchRoot?.Images;
                if (capturedImages == null) { controller.Dispose(); return; }
                var capturedItems = capturedImages.Images.ToArray();
                Dictionary<string, (string Path, string Name)> capturedNames = null;
                Dictionary<string, string> capturedInputs = null;
                VisionPipeline capturedPlan = null;
                bool Active() => !disposed && ReferenceEquals(workbenchEditor, capturedEditor) && workbenchSession == capturedSession && capturedEditor.IsActive(capturedSession) && capturedEditor.NImageContextKey == capturedContext && ReferenceEquals(workbenchRoot?.Images, capturedImages);
                string Identity()
                {
                    VisionPipeline plan = capturedEditor.CaptureNImagePipeline(CaptureWorkbenchResultStep());
                    plan.Name = "NImage_" + string.Concat(toolName.Select(ch => char.IsLetterOrDigit(ch) || ch == '_' || ch == '-' ? ch : '_'));
                    var values = new System.Text.StringBuilder(capturedContext).Append('|').Append(VisionToolNImageVerificationService.SerializePipeline(plan));
                    foreach (var item in capturedImages.Images)
                    {
                        values.Append('|').Append(item.Path).Append('|').Append(item.Name);
                        if (!string.IsNullOrWhiteSpace(item.Path))
                        {
                            using var stream = System.IO.File.OpenRead(item.Path);
                            values.Append('|').Append(Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(stream)));
                        }
                        else
                        {
                            using Bitmap bitmap = AcquireInputSnapshot(VisionRecipeRunner.DefaultInputLayer);
                            using var stream = new System.IO.MemoryStream();
                            if (bitmap == null) { throw new InvalidOperationException("Current input snapshot is unavailable."); }
                            bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                            values.Append('|').Append(Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(stream.ToArray())));
                        }
                    }
                    foreach (string layer in capturedEditor.NImageExternalInputLayers(plan))
                    {
                        using Bitmap bitmap = AcquireInputSnapshot(layer);
                        using var stream = new System.IO.MemoryStream();
                        if (bitmap == null) { throw new InvalidOperationException("Unavailable input snapshot: " + layer); }
                        bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                        values.Append('|').Append(layer).Append('|').Append(Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(stream.ToArray())));
                    }
                    return Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(values.ToString())));
                }
                VisionToolNImageVerificationSession retained = capturedImages.NImageTrialSession;
                try { if (retained?.InputIdentity != Identity()) { retained = null; } }
                catch { retained = null; }
                controller.ConfigureMain(capturedItems.Select(image => image.Path).ToArray(),
                    () => capturedPlan = capturedEditor.CaptureNImagePipeline(CaptureWorkbenchResultStep()), Active,
                    (running, stop) => { if (running && Active()) { capturedImages.BeginNImageTrial(); } capturedEditor.SetNImageRunning(capturedSession, running, stop); },
                    progress =>
                    {
                        if (!Active()) { return; }
                        if (progress.IsStarting)
                        {
                            capturedEditor.NotifyNImageAction(capturedSession, "Workbench.Action.NImageProgressFormat", "Running",
                                capturedNames?.TryGetValue(progress.ImagePath, out var identity) == true ? identity.Name : System.IO.Path.GetFileName(progress.ImagePath), progress.CompletedCount + 1, progress.TotalCount);
                        }
                        else { capturedImages.PublishNImageTrial(progress.Row); }
                    },
                    (session, failure) =>
                    {
                        if (!Active()) { return false; }
                        string key = !string.IsNullOrEmpty(failure) ? "Workbench.Status.Error" : session?.WasCancelled == true ? "Workbench.Action.NImageStoppedFormat" : "Workbench.Action.NImageCompletedFormat";
                        string kind = !string.IsNullOrEmpty(failure) || session?.Rows.Any(row => row.IsError) == true ? "Error" : session?.WasCancelled == true ? "Warning" : "";
                        capturedEditor.NotifyNImageAction(capturedSession, key, kind,
                            failure, session?.Rows.Count ?? 0, session?.PlannedCount ?? 0, session?.PrefixCount ?? 0);
                        return capturedImages.RetainNImageTrial(session);
                    },
                    () => { if (Active()) { capturedEditor.RequestNImageStop(capturedSession); } },
                    directory =>
                    {
                        capturedNames = new Dictionary<string, (string Path, string Name)>(StringComparer.OrdinalIgnoreCase);
                        foreach (var item in capturedItems)
                        {
                            string path = item.Path;
                            if (string.IsNullOrWhiteSpace(path))
                            {
                                Bitmap bitmap = AcquireInputSnapshot(VisionRecipeRunner.DefaultInputLayer);
                                if (bitmap == null) { throw new InvalidOperationException("Current input snapshot is unavailable."); }
                                using (bitmap)
                                {
                                    System.IO.Directory.CreateDirectory(System.IO.Path.Combine(directory, "inputs"));
                                    path = System.IO.Path.Combine(directory, "inputs", "current.png");
                                    bitmap.Save(path, System.Drawing.Imaging.ImageFormat.Png);
                                }
                            }
                            capturedNames.Add(path, (item.Path, item.Name));
                        }
                        capturedInputs = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                        foreach (string layer in capturedEditor.NImageExternalInputLayers(capturedPlan))
                        {
                            using Bitmap bitmap = AcquireInputSnapshot(layer);
                            if (bitmap == null || DisplayManagerImageExtensions.IsPlaceholderBitmap(bitmap)) { throw new InvalidOperationException("Unavailable input snapshot: " + layer); }
                            System.IO.Directory.CreateDirectory(System.IO.Path.Combine(directory, "inputs"));
                            string path = System.IO.Path.Combine(directory, "inputs", "aux-" + capturedInputs.Count + ".png");
                            bitmap.Save(path, System.Drawing.Imaging.ImageFormat.Png);
                            capturedInputs.Add(layer, path);
                        }
                        return capturedNames;
                    }, capturedItems.Select(item => item.Name).ToArray(), () => capturedInputs, Identity, retained);
            }
            VisionToolNImageVerificationWindow window =
                new VisionToolNImageVerificationWindow(controller);
            System.Windows.Window owner = System.Windows.Window.GetWindow(element);
            if (owner != null)
            {
                window.Owner = owner;
            }

            bool followsMainTheme = isWorkbenchDocument && workbenchRoot != null;
            if (followsMainTheme) { window.SetBinding(OpenVisionWorkbenchThemeBehavior.ThemeNameProperty, new Binding(nameof(workbenchRoot.ThemeName)) { Source = workbenchRoot }); }
            nImageWindowOpen = true;
            try { window.ShowDialog(); }
            finally
            {
                if (followsMainTheme) { BindingOperations.ClearBinding(window, OpenVisionWorkbenchThemeBehavior.ThemeNameProperty); }
                nImageWindowOpen = false;
                ApplyNImageVerificationLocalization();
            }
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
            VisionToolDebouncedPreviewScheduler.CancelPending(element);
            previewImageCommandController.LoadWithDialog(e.Role);
        }

        private void OnSavePreviewImageRequested(object sender, VisionToolPreviewImageCommandEventArgs e)
        {
            previewImageCommandController.SaveWithDialog(e.Role);
        }

        private void OnPreviewImageOpenRequested(
            object sender,
            VisionToolPreviewImageOpenRequestedEventArgs e)
        {
            if (e == null || openPreviewViewer == null)
            {
                return;
            }

            string layerTitle = ResolveLayerForPreviewRole(e.Role);
            if (openPreviewViewer(toolName, e.Role, layerTitle))
            {
                openPreviewViewerRole = e.Role;
                e.Handled = true;
            }
        }

        private void OnPreviewViewerLanguageChanged(object sender, EventArgs e)
        {
            RefreshOpenPreviewViewer();
        }

        private void OnSourceLayerChanged(object sender, EventArgs e)
        {
            if (TrySetWorkbenchRoute(view.SelectedInputLayer, null, view.SelectedOutputLayer)) { return; }
            InvalidatePreviewResultForInputChange();
            routeInteractionController.HandleSingleInputLayerChanged(view);
        }

        private void OnDestinationLayerChanged(object sender, EventArgs e)
        {
            if (TrySetWorkbenchRoute(view.SelectedInputLayer, null, view.SelectedOutputLayer)) { return; }
            InvalidatePreviewResultForInputChange();
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
            if (!isWorkbenchDocument) { AddPipelineStep(); }
        }

        private void OnArithmeticInputALayerChanged(object sender, EventArgs e)
        {
            if (TrySetWorkbenchRoute(arithmeticView.SelectedInputLayerA, arithmeticView.SelectedInputLayerB, arithmeticView.SelectedOutputLayer)) { return; }
            routeInteractionController.HandleArithmeticInputALayerChanged(arithmeticView);
        }

        private void OnArithmeticInputBLayerChanged(object sender, EventArgs e)
        {
            if (TrySetWorkbenchRoute(arithmeticView.SelectedInputLayerA, arithmeticView.SelectedInputLayerB, arithmeticView.SelectedOutputLayer)) { return; }
            routeInteractionController.HandleArithmeticInputBLayerChanged(arithmeticView);
        }

        private void OnArithmeticOutputLayerChanged(object sender, EventArgs e)
        {
            if (TrySetWorkbenchRoute(arithmeticView.SelectedInputLayerA, arithmeticView.SelectedInputLayerB, arithmeticView.SelectedOutputLayer)) { return; }
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

        private Bitmap AcquireInputSnapshot(string layerName)
        {
            if (workbenchEditor?.IsActive(workbenchSession) == true && workbenchEditor.TryAcquireInputSnapshot(layerName, out Bitmap snapshot)) { return snapshot; }
            return displayManager.GetLayerImageSnapshot(layerName);
        }

        private string ResolveInputLayer()
        {
            if (workbenchEditor?.IsActive(workbenchSession) == true) { return workbenchEditor.InputLayer; }
            return layerRouteController.ResolveInputLayer();
        }

        private string ResolveOutputLayer()
        {
            if (workbenchEditor?.IsActive(workbenchSession) == true) { return workbenchEditor.OutputLayer; }
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
                if (workbenchEditor?.IsActive(workbenchSession) == true)
                {
                    return layerRouteController.SelectArithmeticInputLayerBLoadTarget(workbenchEditor.InputLayer, workbenchEditor.InputLayerB, workbenchEditor.OutputLayer);
                }
                return layerRouteController.SelectArithmeticInputLayerBLoadTarget();
            }

            return ResolveLayerForPreviewRole(role);
        }

        private void OnPreviewImageLoaded(VisionToolPreviewImageRole role, string layerName)
        {
            VisionToolDebouncedPreviewScheduler.CancelPending(element);
            if (role == VisionToolPreviewImageRole.InputB && workbenchEditor?.IsActive(workbenchSession) == true)
            {
                workbenchEditor.SetRoute(workbenchSession, workbenchEditor.InputLayer, layerName, workbenchEditor.OutputLayer);
            }
        }

        private string ResolvePrimaryInputLayer()
        {
            return arithmeticView == null ? ResolveInputLayer() : ResolveArithmeticInputLayerA();
        }

        private void RunArithmeticPreview(bool useOffsetMode)
        {
            if (isWorkbenchDocument && (workbenchEditor?.IsActive(workbenchSession) != true || !workbenchEditor.CanUseToolActions)) { return; }
            if (isWorkbenchDocument && !workbenchEditor.CommitPendingEdits()) { return; }
            ClearPreviewResult();
            VisionToolDebouncedPreviewScheduler.CancelPending(element);
            VisionPipelineStep step = CreateArithmeticStep(useOffsetMode);
            OpenVisionWorkbenchToolViewModel resultOwner = isWorkbenchDocument ? workbenchEditor : null;
            long resultSession = workbenchSession;
            VisionPipelineStep resultStep = resultOwner == null ? null : CaptureWorkbenchResultStep(step);
            OpenVisionNativePreviewExecutionResult result = previewExecutionController.RunArithmetic(
                step,
                ResolveArithmeticInputLayerA(),
                ResolveArithmeticOutputLayer(),
                ResolvePrimaryInputLayer(),
                useOffsetMode,
                resultStep,
                () =>
                {
                    if (resultOwner != null && ReferenceEquals(workbenchEditor, resultOwner) && workbenchSession == resultSession && resultOwner.IsActive(resultSession))
                    {
                        resultOwner.NotifyExecutionStarted(resultSession, resultStep);
                    }
                });
            VisionToolDebouncedPreviewScheduler.CancelPending(element);
            if (resultOwner != null && (!ReferenceEquals(workbenchEditor, resultOwner) || workbenchSession != resultSession || !resultOwner.IsActive(resultSession))) { return; }
            PublishWorkbenchResult(resultOwner, resultSession, resultStep, result);
            ApplyPreviewExecutionResult(result, () => routeInteractionController.RefreshArithmeticPreviews(arithmeticView));
            if (isWorkbenchDocument) { VisionToolChromePresenter.RevealWorkbenchResult(element.FindName("toolShell") as FrameworkElement); }
        }

        private static void PublishWorkbenchResult(OpenVisionWorkbenchToolViewModel owner, long session, VisionPipelineStep step, OpenVisionNativePreviewExecutionResult result)
        {
            if (owner == null || !owner.IsActive(session) || step == null || result == null) { return; }
            if (result.IsTeachingPreview) { owner.InvalidateResult(session); return; }
            VisionPipelineStepResultSummary summary = result.Summary ?? OpenVisionNativePreviewExecutionController.CaptureSummary(step, null, result.Status);
            if (!result.Success)
            {
                if (summary.Success || string.IsNullOrEmpty(summary.Message)) { summary.Message = result.Status; }
                summary.Success = false;
                summary.IsToolError = !result.Canceled;
                summary.IsAcceptanceNg = false;
            }
            if (result.Canceled) { summary.ExecutionState = VisionPipelineResultSummaryService.CancelledState; }
            owner.PublishResult(session, summary, step);
        }

        private void ClearPreviewResult()
        {
            previewExecutionBoundary?.CancelAndDiscard();
            HasPreviewResult = false;
            if (view is VisionToolSingleInputPropertyToolViewBase propertyToolView)
            {
                propertyToolView.ClearResultReview();
            }
            else if (view is VisionToolSingleInputCustomToolViewBase customToolView)
            {
                customToolView.ClearResultReview();
            }

            if (view is LineToolWpfView lineView)
            {
                lineView.ClearSignalEvidence();
            }
        }

        private void ApplyPreviewExecutionResult(OpenVisionNativePreviewExecutionResult result, Action refreshPreviews)
        {
            if (result == null)
            {
                SetStatus("Preview NG / " + VisionToolVerificationText.InspectionJudgmentNotEvaluated + " / tool returned no result");
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
            string inputLayerB = useOffsetMode && !isWorkbenchDocument ? string.Empty : ResolveArithmeticInputLayerB();
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
            if (workbenchEditor?.IsActive(workbenchSession) == true) { return workbenchEditor.InputLayer; }
            return layerRouteController.ResolveArithmeticInputLayerA();
        }

        private string ResolveArithmeticInputLayerB()
        {
            if (workbenchEditor?.IsActive(workbenchSession) == true) { return workbenchEditor.InputLayerB; }
            return layerRouteController.ResolveArithmeticInputLayerB();
        }

        private string ResolveArithmeticOutputLayer()
        {
            if (workbenchEditor?.IsActive(workbenchSession) == true) { return workbenchEditor.OutputLayer; }
            return layerRouteController.ResolveArithmeticOutputLayer();
        }

        private void SetStatus(string status)
        {
            LastStatusText = status ?? string.Empty;
            statusPresenter.Present(status);
        }

        private bool TrySetWorkbenchRoute(string input, string inputB, string output)
        {
            if (!isWorkbenchDocument) { return false; }
            if (restoringParameters || capturingParameters || refreshingLayerState || workbenchEditor?.IsActive(workbenchSession) != true) { return true; }
            bool accepted = workbenchEditor.SetRoute(workbenchSession, input, inputB, output);
            if (accepted) { RefreshLayerState(); }
            else
            {
                OpenVisionWorkbenchToolViewModel rejectedEditor = workbenchEditor;
                long rejectedSession = workbenchSession;
                // Restore after ComboBox finishes its editable-text transaction; it can overwrite a synchronous rollback.
                element.Dispatcher.BeginInvoke(new Action(() =>
                {
                    if (!disposed && ReferenceEquals(workbenchEditor, rejectedEditor) && workbenchSession == rejectedSession
                        && rejectedEditor.IsActive(rejectedSession)) { RefreshLayerState(); }
                }));
            }
            return true;
        }

        private void NotifyLayerStateChanged()
        {
            RefreshOpenPreviewViewer();
            bool showOutputWorkspacePreview = showOutputWorkspacePreviewOnNextLayerStateChanged;
            showOutputWorkspacePreviewOnNextLayerStateChanged = false;
            LayerStateChanged(this, new OpenVisionNativeToolLayerStateChangedEventArgs(showOutputWorkspacePreview));
        }

        private void RefreshOpenPreviewViewer()
        {
            if (!openPreviewViewerRole.HasValue || refreshPreviewViewer == null)
            {
                return;
            }

            VisionToolPreviewImageRole role = openPreviewViewerRole.Value;
            if (!refreshPreviewViewer(toolName, role, ResolveLayerForPreviewRole(role)))
            {
                openPreviewViewerRole = null;
            }
        }

    }
}
