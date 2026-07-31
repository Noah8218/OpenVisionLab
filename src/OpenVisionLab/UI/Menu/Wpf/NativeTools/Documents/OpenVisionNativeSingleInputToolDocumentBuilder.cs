using Lib.OpenCV.Pipeline;
using Lib.OpenCV.Tool;
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
            bool normalizeSingleChannelInput = true)
            where TView : FrameworkElement, ISingleInputVisionToolWpfView
        {
            if (view == null)
            {
                throw new ArgumentNullException(nameof(view));
            }

            if (executePreview == null)
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
                source => executePreview(source, view),
                stepFactory,
                normalizeSingleChannelInput);
        }
    }
}
