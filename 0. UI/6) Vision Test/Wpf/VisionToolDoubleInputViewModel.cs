using OpenVisionLab.Mvvm;
using System;
using System.Windows.Input;

namespace OpenVisionLab
{
    internal sealed class VisionToolDoubleInputViewModel : ObservableObject
    {
        private readonly Action clearResultReview;
        private readonly Action refreshViewState;
        private readonly Action inputALayerChanged;
        private readonly Action inputBLayerChanged;
        private readonly Action outputLayerChanged;
        private readonly Action inputAPreviewClicked;
        private readonly Action inputBPreviewClicked;
        private readonly Action outputPreviewClicked;
        private readonly Action createOutputLayerRequested;
        private readonly Action runPreviewRequested;
        private readonly Action runOffsetRequested;
        private readonly Action addPipelineRequested;
        private readonly Action<VisionToolPreviewImageRole> loadPreviewImageRequested;
        private readonly Action<VisionToolPreviewImageRole> savePreviewImageRequested;
        private string selectedInputLayerA = string.Empty;
        private string selectedInputLayerB = string.Empty;
        private string selectedOutputLayer = string.Empty;

        public VisionToolDoubleInputViewModel(
            Action inputALayerChanged,
            Action inputBLayerChanged,
            Action outputLayerChanged,
            Action inputAPreviewClicked,
            Action inputBPreviewClicked,
            Action outputPreviewClicked,
            Action createOutputLayerRequested,
            Action runPreviewRequested,
            Action runOffsetRequested,
            Action addPipelineRequested,
            Action<VisionToolPreviewImageRole> loadPreviewImageRequested,
            Action<VisionToolPreviewImageRole> savePreviewImageRequested,
            Action refreshViewState = null,
            Action clearResultReview = null)
        {
            this.inputALayerChanged = inputALayerChanged;
            this.inputBLayerChanged = inputBLayerChanged;
            this.outputLayerChanged = outputLayerChanged;
            this.inputAPreviewClicked = inputAPreviewClicked;
            this.inputBPreviewClicked = inputBPreviewClicked;
            this.outputPreviewClicked = outputPreviewClicked;
            this.createOutputLayerRequested = createOutputLayerRequested;
            this.runPreviewRequested = runPreviewRequested;
            this.runOffsetRequested = runOffsetRequested;
            this.addPipelineRequested = addPipelineRequested;
            this.loadPreviewImageRequested = loadPreviewImageRequested;
            this.savePreviewImageRequested = savePreviewImageRequested;
            this.refreshViewState = refreshViewState;
            this.clearResultReview = clearResultReview;

            InputAPreviewClickCommand = new RelayCommand(RequestInputAPreviewClick);
            InputBPreviewClickCommand = new RelayCommand(RequestInputBPreviewClick);
            OutputPreviewClickCommand = new RelayCommand(RequestOutputPreviewClick);
            CreateOutputLayerCommand = new RelayCommand(RequestCreateOutputLayer);
            RunPreviewCommand = new RelayCommand(RequestRunPreview);
            RunOffsetCommand = new RelayCommand(RequestRunOffset);
            AddPipelineCommand = new RelayCommand(RequestAddPipeline);
        }

        public ICommand InputAPreviewClickCommand { get; }
        public ICommand InputBPreviewClickCommand { get; }
        public ICommand OutputPreviewClickCommand { get; }
        public ICommand CreateOutputLayerCommand { get; }
        public ICommand RunPreviewCommand { get; }
        public ICommand RunOffsetCommand { get; }
        public ICommand AddPipelineCommand { get; }

        public string SelectedInputLayerA
        {
            get => selectedInputLayerA;
            private set => SetProperty(ref selectedInputLayerA, value ?? string.Empty);
        }

        public string SelectedInputLayerB
        {
            get => selectedInputLayerB;
            private set => SetProperty(ref selectedInputLayerB, value ?? string.Empty);
        }

        public string SelectedOutputLayer
        {
            get => selectedOutputLayer;
            private set => SetProperty(ref selectedOutputLayer, value ?? string.Empty);
        }

        public void NotifyInputALayerChanged(string layerName)
        {
            SelectedInputLayerA = layerName;
            NotifyLayerChanged(inputALayerChanged);
        }

        public void NotifyInputBLayerChanged(string layerName)
        {
            SelectedInputLayerB = layerName;
            NotifyLayerChanged(inputBLayerChanged);
        }

        public void NotifyOutputLayerChanged(string layerName)
        {
            SelectedOutputLayer = layerName;
            NotifyLayerChanged(outputLayerChanged);
        }

        public void ApplyLayerSelection(string selectedInputLayerA, string selectedInputLayerB, string selectedOutputLayer)
        {
            SelectedInputLayerA = selectedInputLayerA;
            SelectedInputLayerB = selectedInputLayerB;
            SelectedOutputLayer = selectedOutputLayer;
        }

        public void RequestInputAPreviewClick()
        {
            inputAPreviewClicked?.Invoke();
        }

        public void RequestInputBPreviewClick()
        {
            inputBPreviewClicked?.Invoke();
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

        public void RequestRunOffset()
        {
            runOffsetRequested?.Invoke();
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
            // Two-input tools must keep Input A, Input B, and Output independent; only the changed route is notified here.
            clearResultReview?.Invoke();
            layerChanged?.Invoke();
            refreshViewState?.Invoke();
        }
    }
}