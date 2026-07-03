using Lib.OpenCV.Pipeline;
using Lib.OpenCV.Tool;
using OpenCvSharp;
using OpenVisionLab._1._Core;
using System;

namespace OpenVisionLab
{
    internal static class OpenVisionNativeSimplePreprocessDocumentFactory
    {
        // Builds SimplePreprocess-based native documents; UI rules, preview execution, and properties live in separate helpers.
        internal static OpenVisionNativeToolDocument CreateEdgeDetectionDocument(IDisplayManager displayManager)
        {
            return CreateSimplePreprocessDocument(
                displayManager,
                CreateDescriptor(
                    "EdgeDetection",
                    OpenVisionNativeSimplePreprocessViewConfigurator.ConfigureEdgeDetectionView,
                    OpenVisionNativeSimplePreprocessPreviewExecutor.ExecuteEdgeDetectionPreview,
                    (view, inputLayer, outputLayer) => VisionPipelineStepBuilder.FromEdgeDetectionProperty(
                        OpenVisionNativeSimplePreprocessPropertyFactory.CreateEdgeDetectionProperty(view),
                        "EdgeDetection",
                        inputLayer,
                        outputLayer)));
        }

        internal static OpenVisionNativeToolDocument CreateRotateScaleDocument(IDisplayManager displayManager)
        {
            return CreateSimplePreprocessDocument(
                displayManager,
                CreateDescriptor(
                    "RotateScale",
                    OpenVisionNativeSimplePreprocessViewConfigurator.ConfigureRotateScaleView,
                    OpenVisionNativeSimplePreprocessPreviewExecutor.ExecuteRotateScalePreview,
                    (view, inputLayer, outputLayer) => VisionPipelineStepBuilder.FromRotateScaleProperty(
                        OpenVisionNativeSimplePreprocessPropertyFactory.CreateRotateScaleProperty(view),
                        "RotateScale",
                        inputLayer,
                        outputLayer)));
        }

        internal static OpenVisionNativeToolDocument CreateMeanDocument(IDisplayManager displayManager)
        {
            return CreateSimplePreprocessDocument(
                displayManager,
                CreateDescriptor(
                    "Mean",
                    OpenVisionNativeSimplePreprocessViewConfigurator.ConfigureMeanView,
                    OpenVisionNativeSimplePreprocessPreviewExecutor.ExecuteMeanPreview,
                    (view, inputLayer, outputLayer) => VisionPipelineStepBuilder.FromProperty(
                        OpenVisionNativeSimplePreprocessPropertyFactory.CreateMeanProperty(view),
                        inputLayer,
                        outputLayer)));
        }

        internal static OpenVisionNativeToolDocument CreateHsvDocument(IDisplayManager displayManager)
        {
            return CreateSimplePreprocessDocument(
                displayManager,
                CreateDescriptor(
                    "HSV",
                    OpenVisionNativeSimplePreprocessViewConfigurator.ConfigureHsvView,
                    OpenVisionNativeSimplePreprocessPreviewExecutor.ExecuteHsvPreview,
                    null,
                    normalizeSingleChannelInput: false));
        }

        internal static OpenVisionNativeToolDocument CreateHistogramDocument(IDisplayManager displayManager)
        {
            return CreateSimplePreprocessDocument(
                displayManager,
                CreateDescriptor(
                    "Histogram",
                    OpenVisionNativeSimplePreprocessViewConfigurator.ConfigureHistogramView,
                    OpenVisionNativeSimplePreprocessPreviewExecutor.ExecuteHistogramPreview,
                    null));
        }

        private static OpenVisionNativeSimplePreprocessToolDescriptor CreateDescriptor(
            string toolName,
            Action<SimplePreprocessToolWpfView> configureView,
            Func<Mat, SimplePreprocessToolWpfView, VisionToolResult> executePreview,
            Func<SimplePreprocessToolWpfView, string, string, VisionPipelineStep> createStep,
            bool normalizeSingleChannelInput = true)
        {
            // SimplePreprocess tools use the same output naming rule, so new tools only declare the stable tool name.
            return new OpenVisionNativeSimplePreprocessToolDescriptor(
                toolName,
                toolName + "_Preview",
                configureView,
                executePreview,
                createStep,
                normalizeSingleChannelInput);
        }

        private static OpenVisionNativeToolDocument CreateSimplePreprocessDocument(
            IDisplayManager displayManager,
            OpenVisionNativeSimplePreprocessToolDescriptor descriptor)
        {
            if (descriptor == null)
            {
                throw new ArgumentNullException(nameof(descriptor));
            }

            // SimplePreprocess tools share one shell; only parameter setup, preview, and pipeline step differ.
            SimplePreprocessToolWpfView view = new SimplePreprocessToolWpfView();
            descriptor.ConfigureView(view);
            string settingsConfigName = OpenVisionNativeToolSettingsStore.CreateConfigName(descriptor.ToolName);
            view.ApplyPersistedSettings(OpenVisionNativeToolSettingsStore.Load(settingsConfigName, new SimplePreprocessToolSettings()));
            view.ParameterChanged += (sender, e) =>
                OpenVisionNativeToolSettingsStore.Save(settingsConfigName, view.CaptureSettings());

            Func<string, string, VisionPipelineStep> stepFactory = descriptor.CreateStep == null
                ? null
                : (inputLayer, outputLayer) => descriptor.CreateStep(view, inputLayer, outputLayer);

            return OpenVisionNativeSingleInputToolDocumentBuilder.Create(
                displayManager,
                view,
                descriptor.ToolName,
                descriptor.DefaultOutputLayer,
                descriptor.ExecutePreview,
                stepFactory == null ? null : (activeView, inputLayer, outputLayer) => stepFactory(inputLayer, outputLayer),
                descriptor.NormalizeSingleChannelInput);
        }

    }
}
