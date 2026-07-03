using System;

namespace OpenVisionLab
{
    internal sealed class VisionToolActionRequestController
    {
        private readonly object sender;
        private readonly Func<EventHandler> inputPreviewClicked;
        private readonly Func<EventHandler> inputBPreviewClicked;
        private readonly Func<EventHandler> outputPreviewClicked;
        private readonly Func<EventHandler> createOutputLayerRequested;
        private readonly Func<EventHandler> runPreviewRequested;
        private readonly Func<EventHandler> addPipelineRequested;
        private readonly Func<EventHandler> runOffsetRequested;

        private VisionToolActionRequestController(
            object sender,
            Func<EventHandler> inputPreviewClicked,
            Func<EventHandler> inputBPreviewClicked,
            Func<EventHandler> outputPreviewClicked,
            Func<EventHandler> createOutputLayerRequested,
            Func<EventHandler> runPreviewRequested,
            Func<EventHandler> addPipelineRequested,
            Func<EventHandler> runOffsetRequested)
        {
            this.sender = sender ?? throw new ArgumentNullException(nameof(sender));
            this.inputPreviewClicked = inputPreviewClicked;
            this.inputBPreviewClicked = inputBPreviewClicked;
            this.outputPreviewClicked = outputPreviewClicked;
            this.createOutputLayerRequested = createOutputLayerRequested;
            this.runPreviewRequested = runPreviewRequested;
            this.addPipelineRequested = addPipelineRequested;
            this.runOffsetRequested = runOffsetRequested;
        }

        public static VisionToolActionRequestController CreateSingle(
            object sender,
            Func<EventHandler> inputPreviewClicked,
            Func<EventHandler> outputPreviewClicked,
            Func<EventHandler> createOutputLayerRequested,
            Func<EventHandler> runPreviewRequested,
            Func<EventHandler> addPipelineRequested)
        {
            return new VisionToolActionRequestController(
                sender,
                inputPreviewClicked,
                null,
                outputPreviewClicked,
                createOutputLayerRequested,
                runPreviewRequested,
                addPipelineRequested,
                null);
        }

        public static VisionToolActionRequestController CreateArithmetic(
            object sender,
            Func<EventHandler> inputAPreviewClicked,
            Func<EventHandler> inputBPreviewClicked,
            Func<EventHandler> outputPreviewClicked,
            Func<EventHandler> createOutputLayerRequested,
            Func<EventHandler> runPreviewRequested,
            Func<EventHandler> addPipelineRequested,
            Func<EventHandler> runOffsetRequested)
        {
            return new VisionToolActionRequestController(
                sender,
                inputAPreviewClicked,
                inputBPreviewClicked,
                outputPreviewClicked,
                createOutputLayerRequested,
                runPreviewRequested,
                addPipelineRequested,
                runOffsetRequested);
        }

        public void RequestInputPreviewClick()
        {
            Raise(inputPreviewClicked);
        }

        public void RequestInputBPreviewClick()
        {
            Raise(inputBPreviewClicked);
        }

        public void RequestOutputPreviewClick()
        {
            Raise(outputPreviewClicked);
        }

        public void RequestCreateOutputLayer()
        {
            Raise(createOutputLayerRequested);
        }

        public void RequestRunPreview()
        {
            Raise(runPreviewRequested);
        }

        public void RequestAddPipeline()
        {
            Raise(addPipelineRequested);
        }

        public void RequestRunOffset()
        {
            Raise(runOffsetRequested);
        }

        private void Raise(Func<EventHandler> eventAccessor)
        {
            // Read the event delegate at execution time so subscribers added after View construction are included.
            eventAccessor?.Invoke()?.Invoke(sender, EventArgs.Empty);
        }
    }
}