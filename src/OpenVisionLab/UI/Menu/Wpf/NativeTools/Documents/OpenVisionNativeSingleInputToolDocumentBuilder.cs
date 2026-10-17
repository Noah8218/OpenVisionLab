using OpenVisionLab.Vision2D.Pipeline;
using OpenVisionLab.Vision2D.Tool;
using OpenCvSharp;
using OpenVisionLab.Core;
using System;
using System.Windows;

namespace OpenVisionLab
{
    internal static class OpenVisionNativeSingleInputToolDocumentBuilder
    {
        public static OpenVisionNativeToolDocument Create<TView>(
            IDisplayManager displayManager,
            TView view,
            string toolName,
            string defaultOutputLayer,
            Func<Mat, TView, VisionToolResult> executePreview,
            Func<TView, string, string, VisionPipelineStep> createStep,
            bool normalizeSingleChannelInput = true,
            Func<TView, OpenVisionNativePreviewOperation> capturePreview = null)
            where TView : FrameworkElement, ISingleInputVisionToolWpfView
        {
            if (view == null)
            {
                throw new ArgumentNullException(nameof(view));
            }

            if (executePreview == null && capturePreview == null)
            {
                throw new ArgumentNullException(nameof(executePreview));
            }

            // Keep this builder deliberately thin: factories still show tool-specific intent, this only owns final document wiring.
            Func<string, string, VisionPipelineStep> stepFactory = createStep == null
                ? null
                : (inputLayer, outputLayer) => createStep(view, inputLayer, outputLayer);

            return new OpenVisionNativeToolDocument(
                displayManager,
                view,
                view,
                toolName,
                defaultOutputLayer,
                executePreview == null ? null : source => executePreview(source, view),
                stepFactory,
                normalizeSingleChannelInput,
                capturePreview == null ? null : () => capturePreview(view));
        }
    }
}
