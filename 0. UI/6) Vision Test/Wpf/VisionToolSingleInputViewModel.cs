using OpenVisionLab.Mvvm;
using System;
using System.Windows.Input;

namespace OpenVisionLab
{
    internal sealed class VisionToolSingleInputViewModel : ObservableObject
    {
        private readonly Action refreshViewState;
        private readonly Action clearResultReview;
        private readonly Action sourceLayerChanged;
        private readonly Action destinationLayerChanged;
        private readonly Action inputPreviewClicked;
        private readonly Action outputPreviewClicked;
        private readonly Action createOutputLayerRequested;
        private readonly Action runPreviewRequested;
        private readonly Action addPipelineRequested;
        private readonly Action<VisionToolPreviewImageRole> loadPreviewImageRequested;
        private readonly Action<VisionToolPreviewImageRole> savePreviewImageRequested;
        private string selectedInputLayer = string.Empty;
        private string selectedOutputLayer = string.Empty;

        public VisionToolSingleInputViewModel(
            Action sourceLayerChanged,
            Action destinationLayerChanged,
            Action inputPreviewClicked,
            Action outputPreviewClicked,
            Action createOutputLayerRequested,
            Action runPreviewRequested,
            Action addPipelineRequested,
            Action<VisionToolPreviewImageRole> loadPreviewImageRequested,
            Action<VisionToolPreviewImageRole> savePreviewImageRequested,
            Action refreshViewState = null,
            Action clearResultReview = null)
        {
            this.sourceLayerChanged = sourceLayerChanged;
            this.destinationLayerChanged = destinationLayerChanged;
            this.inputPreviewClicked = inputPreviewClicked;
            this.outputPreviewClicked = outputPreviewClicked;
            this.createOutputLayerRequested = createOutputLayerRequested;
            this.runPreviewRequested = runPreviewRequested;
            this.addPipelineRequested = addPipelineRequested;
            this.loadPreviewImageRequested = loadPreviewImageRequested;
            this.savePreviewImageRequested = savePreviewImageRequested;
            this.refreshViewState = refreshViewState;
            this.clearResultReview = clearResultReview;

            InputPreviewClickCommand = new RelayCommand(RequestInputPreviewClick);
            OutputPreviewClickCommand = new RelayCommand(RequestOutputPreviewClick);
            CreateOutputLayerCommand = new RelayCommand(RequestCreateOutputLayer);
            RunPreviewCommand = new RelayCommand(RequestRunPreview);
            AddPipelineCommand = new RelayCommand(RequestAddPipeline);
        }

        public ICommand InputPreviewClickCommand { get; }

        public ICommand OutputPreviewClickCommand { get; }

        public ICommand CreateOutputLayerCommand { get; }

        public ICommand RunPreviewCommand { get; }

        public ICommand AddPipelineCommand { get; }

        public string SelectedInputLayer
        {
            get => selectedInputLayer;
            private set => SetProperty(ref selectedInputLayer, value ?? string.Empty);
        }

        public string SelectedOutputLayer
        {
            get => selectedOutputLayer;
            private set => SetProperty(ref selectedOutputLayer, value ?? string.Empty);
        }

        public void NotifyInputLayerChanged(string layerName)
        {
            SelectedInputLayer = layerName;
            NotifyLayerChanged(sourceLayerChanged);
        }

        public void NotifyOutputLayerChanged(string layerName)
        {
            SelectedOutputLayer = layerName;
            NotifyLayerChanged(destinationLayerChanged);
        }

        public void ApplyLayerSelection(string selectedInputLayer, string selectedOutputLayer)
        {
            SelectedInputLayer = selectedInputLayer;
            SelectedOutputLayer = selectedOutputLayer;
        }

        public void RequestInputPreviewClick()
        {
            inputPreviewClicked?.Invoke();
        }

        public void RequestOutputPreviewClick()
        {
            outputPreviewClicked?.Invoke();
        }

        public void RequestCreateOutputLayer()
        {
            createOutputLayerRequested?.Invoke();
        }

        public void RequestRunPreview()
        {
            runPreviewRequested?.Invoke();
        }

        public void RequestAddPipeline()
        {
            addPipelineRequested?.Invoke();
        }

        public void RequestLoadPreviewImage(VisionToolPreviewImageRole role)
        {
            loadPreviewImageRequested?.Invoke(role);
        }

        public void RequestSavePreviewImage(VisionToolPreviewImageRole role)
        {
            savePreviewImageRequested?.Invoke(role);
        }

        private void NotifyLayerChanged(Action layerChanged)
        {
            // Layer routing is centralized here so adding an output layer cannot silently rewrite input state.
            clearResultReview?.Invoke();
            layerChanged?.Invoke();
            refreshViewState?.Invoke();
        }
    }
}
