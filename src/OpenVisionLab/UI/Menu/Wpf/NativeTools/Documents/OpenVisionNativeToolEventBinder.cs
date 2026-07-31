using System;

namespace OpenVisionLab
{
    internal sealed class OpenVisionNativeToolEventBinder : IDisposable
    {
        private readonly Action detach;
        private bool disposed;

        private OpenVisionNativeToolEventBinder(Action detach)
        {
            this.detach = detach ?? throw new ArgumentNullException(nameof(detach));
        }

        public static OpenVisionNativeToolEventBinder BindSingle(
            ISingleInputVisionToolWpfView view,
            EventHandler sourceLayerChanged,
            EventHandler destinationLayerChanged,
            EventHandler inputPreviewClicked,
            EventHandler outputPreviewClicked,
            EventHandler createOutputLayerRequested,
            EventHandler runPreviewRequested,
            EventHandler addPipelineRequested,
            EventHandler<VisionToolPreviewImageCommandEventArgs> loadPreviewImageRequested,
            EventHandler<VisionToolPreviewImageCommandEventArgs> savePreviewImageRequested,
            EventHandler editSelectedRoiRequested)
        {
            if (view == null)
            {
                throw new ArgumentNullException(nameof(view));
            }

            view.SourceLayerChanged += sourceLayerChanged;
            view.DestinationLayerChanged += destinationLayerChanged;
            view.InputPreviewClicked += inputPreviewClicked;
            view.OutputPreviewClicked += outputPreviewClicked;
            view.CreateOutputLayerRequested += createOutputLayerRequested;
            view.RunPreviewRequested += runPreviewRequested;
            view.AddPipelineRequested += addPipelineRequested;
            if (view is IVisionToolPreviewImageCommands previewCommands)
            {
                previewCommands.LoadPreviewImageRequested += loadPreviewImageRequested;
                previewCommands.SavePreviewImageRequested += savePreviewImageRequested;
            }

            if (view is LineToolWpfView lineView)
            {
                lineView.EditSelectedRoiRequested += editSelectedRoiRequested;
            }

            // Keep detach beside attach so new tool events cannot be added one-sided.
            return new OpenVisionNativeToolEventBinder(() =>
            {
                view.SourceLayerChanged -= sourceLayerChanged;
                view.DestinationLayerChanged -= destinationLayerChanged;
                view.InputPreviewClicked -= inputPreviewClicked;
                view.OutputPreviewClicked -= outputPreviewClicked;
                view.CreateOutputLayerRequested -= createOutputLayerRequested;
                view.RunPreviewRequested -= runPreviewRequested;
                view.AddPipelineRequested -= addPipelineRequested;
                if (view is IVisionToolPreviewImageCommands detachPreviewCommands)
                {
                    detachPreviewCommands.LoadPreviewImageRequested -= loadPreviewImageRequested;
                    detachPreviewCommands.SavePreviewImageRequested -= savePreviewImageRequested;
                }

                if (view is LineToolWpfView detachLineView)
                {
                    detachLineView.EditSelectedRoiRequested -= editSelectedRoiRequested;
                }
            });
        }

        public static OpenVisionNativeToolEventBinder BindArithmetic(
            IArithmeticVisionToolWpfView view,
            EventHandler inputALayerChanged,
            EventHandler inputBLayerChanged,
            EventHandler outputLayerChanged,
            EventHandler inputAPreviewClicked,
            EventHandler inputBPreviewClicked,
            EventHandler outputPreviewClicked,
            EventHandler createOutputLayerRequested,
            EventHandler runPreviewRequested,
            EventHandler runOffsetRequested,
            EventHandler addPipelineRequested,
            EventHandler<VisionToolPreviewImageCommandEventArgs> loadPreviewImageRequested,
            EventHandler<VisionToolPreviewImageCommandEventArgs> savePreviewImageRequested)
        {
            if (view == null)
            {
                throw new ArgumentNullException(nameof(view));
            }

            view.InputALayerChanged += inputALayerChanged;
            view.InputBLayerChanged += inputBLayerChanged;
            view.OutputLayerChanged += outputLayerChanged;
            view.InputAPreviewClicked += inputAPreviewClicked;
            view.InputBPreviewClicked += inputBPreviewClicked;
            view.OutputPreviewClicked += outputPreviewClicked;
            view.CreateOutputLayerRequested += createOutputLayerRequested;
            view.RunPreviewRequested += runPreviewRequested;
            view.RunOffsetRequested += runOffsetRequested;
            view.AddPipelineRequested += addPipelineRequested;
            if (view is IVisionToolPreviewImageCommands previewCommands)
            {
                previewCommands.LoadPreviewImageRequested += loadPreviewImageRequested;
                previewCommands.SavePreviewImageRequested += savePreviewImageRequested;
            }

            return new OpenVisionNativeToolEventBinder(() =>
            {
                view.InputALayerChanged -= inputALayerChanged;
                view.InputBLayerChanged -= inputBLayerChanged;
                view.OutputLayerChanged -= outputLayerChanged;
                view.InputAPreviewClicked -= inputAPreviewClicked;
                view.InputBPreviewClicked -= inputBPreviewClicked;
                view.OutputPreviewClicked -= outputPreviewClicked;
                view.CreateOutputLayerRequested -= createOutputLayerRequested;
                view.RunPreviewRequested -= runPreviewRequested;
                view.RunOffsetRequested -= runOffsetRequested;
                view.AddPipelineRequested -= addPipelineRequested;
                if (view is IVisionToolPreviewImageCommands detachPreviewCommands)
                {
                    detachPreviewCommands.LoadPreviewImageRequested -= loadPreviewImageRequested;
                    detachPreviewCommands.SavePreviewImageRequested -= savePreviewImageRequested;
                }
            });
        }

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            disposed = true;
            detach();
        }
    }
}
