using OpenVisionLab.Mvvm;
using OpenVisionLab.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Input;
using static OpenVisionLab.DEFINE;

namespace OpenVisionLab
{
    public sealed class OpenVisionShellHostWorkspaceCommandSurface : ObservableObject, IDisposable
    {
        private readonly OpenVisionShellHostCommandController commandController;
        private readonly OpenVisionShellHostWorkspacePreviewController workspacePreviewController;
        private readonly OpenVisionZoomableImageController workspaceFallbackZoomController;
        private readonly IOpenVisionDockedLayerWorkspaceCommands dockedLayerWorkspace;
        private IOpenVisionDockedLayerViewer observedZoomViewer;
        private readonly List<IOpenVisionDockedLayerViewer> observedPointerViewers = new();
        private OpenVisionZoomableImageStatus pointerStatus = OpenVisionZoomableImageStatus.Empty;
        private bool disposed;
        private readonly Action<VISION_MENU> selectTool;
        private readonly Func<VISION_MENU?> sampleFirstStepMenuProvider;
        private readonly Func<string> sampleCounterpartNameProvider;
        private readonly Action applySampleFirstStepParameters;

        internal OpenVisionShellHostWorkspaceCommandSurface(
            OpenVisionShellHostCommandController commandController,
            OpenVisionShellHostWorkspacePreviewController workspacePreviewController,
            OpenVisionZoomableImageController workspaceFallbackZoomController,
            IOpenVisionDockedLayerWorkspaceCommands dockedLayerWorkspace,
            Action<VISION_MENU> selectTool = null,
            Func<VISION_MENU?> sampleFirstStepMenuProvider = null,
            Func<string> sampleCounterpartNameProvider = null,
            Action applySampleFirstStepParameters = null)
        {
            this.commandController = commandController ?? throw new ArgumentNullException(nameof(commandController));
            this.workspacePreviewController = workspacePreviewController ?? throw new ArgumentNullException(nameof(workspacePreviewController));
            this.workspaceFallbackZoomController = workspaceFallbackZoomController ?? throw new ArgumentNullException(nameof(workspaceFallbackZoomController));
            this.dockedLayerWorkspace = dockedLayerWorkspace ?? throw new ArgumentNullException(nameof(dockedLayerWorkspace));
            this.selectTool = selectTool;
            this.sampleFirstStepMenuProvider = sampleFirstStepMenuProvider;
            this.sampleCounterpartNameProvider = sampleCounterpartNameProvider;
            this.applySampleFirstStepParameters = applySampleFirstStepParameters;

            LoadImageCommand = new RelayCommand<string>(commandController.PromptAndLoadWorkspaceImage);
            OpenFolderCommand = new RelayCommand<string>(commandController.PromptAndOpenWorkspaceFolder);
            OpenSampleCommand = new RelayCommand(commandController.PromptAndOpenSampleImages, commandController.HasRunnableSample);
            FitImageCommand = new RelayCommand(commandController.FitWorkspaceImage, HasWorkspaceImage);
            ZoomInImageCommand = new RelayCommand(() => commandController.ZoomWorkspaceImage(1.25D), CanZoomImage);
            ZoomOutImageCommand = new RelayCommand(() => commandController.ZoomWorkspaceImage(0.8D), CanZoomImage);
            OriginalSizeImageCommand = new RelayCommand(commandController.ShowWorkspaceOriginalSize, CanZoomImage);
            SaveImageCommand = new RelayCommand(commandController.PromptAndSaveWorkspaceImage, HasWorkspaceImage);
            OpenThresholdToolCommand = new RelayCommand(() => OpenTool(VISION_MENU.Threshold), CanOpenToolAfterImageReady);
            OpenMatchingToolCommand = new RelayCommand(() => OpenTool(VISION_MENU.Matching), CanOpenToolAfterImageReady);
            OpenLineToolCommand = new RelayCommand(() => OpenTool(VISION_MENU.Line), CanOpenToolAfterImageReady);
            OpenSamplePipelineCommand = new RelayCommand(OpenPipelineReview, CanOpenSampleNavigation);
            OpenSampleFirstStepCommand = new RelayCommand(OpenSampleFirstStepTool, CanOpenSampleFirstStepTool);
            OpenSampleCounterpartCommand = new RelayCommand(OpenSampleCounterpart, CanOpenSampleCounterpart);
            workspaceFallbackZoomController.ZoomChanged += OnZoomChanged;
            dockedLayerWorkspace.WorkspaceStateChanged += OnWorkspaceStateChanged;
            RefreshCanExecute();
        }

        public string PointerText => pointerStatus.HasPixel
            ? string.Format(CultureInfo.InvariantCulture, "X {0}  Y {1}  GV {2}", pointerStatus.ImageX, pointerStatus.ImageY, pointerStatus.GrayValue)
            : "X —  Y —  GV —";

        public string PointerCoordinatesText => pointerStatus.HasPixel ? pointerStatus.FormatCoordinates() : "X:- Y:-";

        public string PointerPixelText => pointerStatus.HasPixel ? pointerStatus.FormatPixel() : "GV - | RGB -";

        public string ZoomText
        {
            get
            {
                IOpenVisionDockedLayerViewer viewer = commandController.ActiveLayerViewer;
                double scale = viewer != null ? viewer.ZoomScale : workspaceFallbackZoomController.PixelZoomScale;
                return Math.Round((scale > 0D ? scale : 1D) * 100D, MidpointRounding.AwayFromZero).ToString(CultureInfo.InvariantCulture) + "%";
            }
        }

        public ICommand ZoomInImageCommand { get; }

        public ICommand ZoomOutImageCommand { get; }

        public ICommand OriginalSizeImageCommand { get; }

        public ICommand LoadImageCommand { get; }

        public ICommand OpenFolderCommand { get; }

        public ICommand OpenSampleCommand { get; }

        public ICommand FitImageCommand { get; }

        public ICommand SaveImageCommand { get; }

        public ICommand OpenThresholdToolCommand { get; }

        public ICommand OpenMatchingToolCommand { get; }

        public ICommand OpenLineToolCommand { get; }

        public ICommand OpenSamplePipelineCommand { get; }

        public ICommand OpenSampleFirstStepCommand { get; }

        public ICommand OpenSampleCounterpartCommand { get; }

        public void RefreshCanExecute()
        {
            if (disposed) { return; }
            IOpenVisionDockedLayerViewer viewer = commandController.ActiveLayerViewer;
            if (!ReferenceEquals(viewer, observedZoomViewer))
            {
                if (observedZoomViewer != null) { observedZoomViewer.ZoomChanged -= OnZoomChanged; }
                observedZoomViewer = viewer;
                if (observedZoomViewer != null) { observedZoomViewer.ZoomChanged += OnZoomChanged; }
            }
            RefreshPointerViewers();
            ApplyPointerStatus(OpenVisionZoomableImageStatus.Empty);
            OnPropertyChanged(nameof(ZoomText));
            CommandManager.InvalidateRequerySuggested();
        }

        public void Dispose()
        {
            if (disposed) { return; }
            disposed = true;
            workspaceFallbackZoomController.ZoomChanged -= OnZoomChanged;
            dockedLayerWorkspace.WorkspaceStateChanged -= OnWorkspaceStateChanged;
            if (observedZoomViewer != null) { observedZoomViewer.ZoomChanged -= OnZoomChanged; }
            observedZoomViewer = null;
            foreach (IOpenVisionDockedLayerViewer viewer in observedPointerViewers) { viewer.PointerStatusChanged -= OnPointerStatusChanged; }
            observedPointerViewers.Clear();
        }

        internal bool ApplyFallbackPointerStatus(OpenVisionZoomableImageStatus status)
        {
            if (disposed || commandController.ActiveLayerViewer != null) { return false; }
            ApplyPointerStatus(status);
            return true;
        }

        private void RefreshPointerViewers()
        {
            foreach (IOpenVisionDockedLayerViewer viewer in observedPointerViewers) { viewer.PointerStatusChanged -= OnPointerStatusChanged; }
            observedPointerViewers.Clear();
            foreach (string layerTitle in dockedLayerWorkspace.LayerTitles)
            {
                IOpenVisionDockedLayerViewer viewer = dockedLayerWorkspace.FindLayerViewer(layerTitle);
                if (viewer == null || observedPointerViewers.Contains(viewer)) { continue; }
                observedPointerViewers.Add(viewer);
                viewer.PointerStatusChanged += OnPointerStatusChanged;
            }
        }

        private void OnPointerStatusChanged(object sender, EventArgs e)
        {
            if (!disposed && sender is IOpenVisionDockedLayerViewer viewer) { ApplyPointerStatus(viewer.PointerStatus); }
        }

        private void ApplyPointerStatus(OpenVisionZoomableImageStatus status)
        {
            pointerStatus = status ?? OpenVisionZoomableImageStatus.Empty;
            OnPropertyChanged(nameof(PointerText));
            OnPropertyChanged(nameof(PointerCoordinatesText));
            OnPropertyChanged(nameof(PointerPixelText));
        }

        private void OnWorkspaceStateChanged(object sender, EventArgs e) => RefreshCanExecute();

        private void OnZoomChanged(object sender, EventArgs e)
        {
            if (!disposed) { OnPropertyChanged(nameof(ZoomText)); }
        }

        private bool CanZoomImage()
        {
            IOpenVisionDockedLayerViewer viewer = commandController.ActiveLayerViewer;
            return !disposed && (viewer != null ? viewer.HasImage : HasWorkspaceImage());
        }

        private bool HasWorkspaceImage()
        {
            return workspacePreviewController.HasImage;
        }

        private bool CanOpenSampleNavigation()
        {
            return selectTool != null;
        }

        private bool CanOpenToolAfterImageReady()
        {
            return selectTool != null && HasWorkspaceImage();
        }

        private void OpenTool(VISION_MENU menu)
        {
            selectTool?.Invoke(menu);
        }

        private bool CanOpenSampleFirstStepTool()
        {
            return selectTool != null && sampleFirstStepMenuProvider?.Invoke().HasValue == true;
        }

        private bool CanOpenSampleCounterpart()
        {
            return !string.IsNullOrWhiteSpace(sampleCounterpartNameProvider?.Invoke());
        }

        private void OpenPipelineReview()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            OVLog.Write(LogCategory.System, LogLevel.Info, "[PipelineOpenTrace] CommandBegin");
            try
            {
                selectTool?.Invoke(VISION_MENU.Pipeline);
            }
            finally
            {
                OVLog.Write(
                    LogCategory.System,
                    LogLevel.Info,
                    $"[PipelineOpenTrace] CommandReturnMs={stopwatch.ElapsedMilliseconds}");
            }
        }

        private void OpenSampleFirstStepTool()
        {
            VISION_MENU? menu = sampleFirstStepMenuProvider?.Invoke();
            if (menu.HasValue)
            {
                selectTool?.Invoke(menu.Value);
                applySampleFirstStepParameters?.Invoke();
            }
        }

        private void OpenSampleCounterpart()
        {
            string sampleName = sampleCounterpartNameProvider?.Invoke();
            if (!string.IsNullOrWhiteSpace(sampleName))
            {
                commandController.OpenRunnableSampleByName(sampleName);
            }
        }
    }
}
