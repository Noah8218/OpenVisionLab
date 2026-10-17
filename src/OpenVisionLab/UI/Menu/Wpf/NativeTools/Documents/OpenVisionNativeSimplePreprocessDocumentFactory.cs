using OpenVisionLab.Vision2D.Pipeline;
using OpenVisionLab.Core;
using System;

namespace OpenVisionLab
{
    internal static class OpenVisionNativeSimplePreprocessDocumentFactory
    {
        // Builds SimplePreprocess-based native documents; UI rules, preview execution, and properties live in separate helpers.
        internal static OpenVisionNativeToolDocument CreateEdgeDetectionDocument(IDisplayManager displayManager, bool forWorkbench = false)
        {
            return CreateSimplePreprocessDocument(
                displayManager,
                CreateDescriptor(
                    "EdgeDetection",
                    OpenVisionNativeSimplePreprocessViewConfigurator.ConfigureEdgeDetectionView,
                    OpenVisionNativeSimplePreprocessPreviewExecutor.CaptureEdgeDetectionPreview,
                    (view, inputLayer, outputLayer) => VisionPipelineStepBuilder.FromEdgeDetectionProperty(
                        OpenVisionNativeSimplePreprocessPropertyFactory.CreateEdgeDetectionProperty(view),
                        "EdgeDetection",
                        inputLayer,
                        outputLayer)), forWorkbench);
        }

        internal static OpenVisionNativeToolDocument CreateRotateScaleDocument(IDisplayManager displayManager, bool forWorkbench = false)
        {
            return CreateSimplePreprocessDocument(
                displayManager,
                CreateDescriptor(
                    "RotateScale",
                    OpenVisionNativeSimplePreprocessViewConfigurator.ConfigureRotateScaleView,
                    OpenVisionNativeSimplePreprocessPreviewExecutor.CaptureRotateScalePreview,
                    (view, inputLayer, outputLayer) => VisionPipelineStepBuilder.FromRotateScaleProperty(
                        OpenVisionNativeSimplePreprocessPropertyFactory.CreateRotateScaleProperty(view),
                        "RotateScale",
                        inputLayer,
                        outputLayer)), forWorkbench);
        }

        internal static OpenVisionNativeToolDocument CreateMeanDocument(IDisplayManager displayManager, bool forWorkbench = false)
        {
            OpenVisionNativeToolDocument document = null;
            document = CreateSimplePreprocessDocument(
                displayManager,
                CreateDescriptor(
                    "Mean",
                    OpenVisionNativeSimplePreprocessViewConfigurator.ConfigureMeanView,
                    view => OpenVisionNativeSimplePreprocessPreviewExecutor.CaptureMeanPreview(view, document?.WorkbenchParameterStep),
                    (view, inputLayer, outputLayer) => VisionPipelineStepBuilder.FromProperty(
                        OpenVisionNativeSimplePreprocessPropertyFactory.CreateMeanProperty(view, document?.WorkbenchParameterStep),
                        inputLayer,
                        outputLayer)), forWorkbench);
            return document;
        }

        internal static OpenVisionNativeToolDocument CreateHsvDocument(IDisplayManager displayManager, bool forWorkbench = false)
        {
            return CreateSimplePreprocessDocument(
                displayManager,
                CreateDescriptor(
                    "HSV",
                    OpenVisionNativeSimplePreprocessViewConfigurator.ConfigureHsvView,
                    OpenVisionNativeSimplePreprocessPreviewExecutor.CaptureHsvPreview,
                    null,
                    normalizeSingleChannelInput: false), forWorkbench);
        }

        internal static OpenVisionNativeToolDocument CreateHistogramDocument(IDisplayManager displayManager, bool forWorkbench = false)
        {
            return CreateSimplePreprocessDocument(
                displayManager,
                CreateDescriptor(
                    "Histogram",
                    OpenVisionNativeSimplePreprocessViewConfigurator.ConfigureHistogramView,
                    OpenVisionNativeSimplePreprocessPreviewExecutor.CaptureHistogramPreview,
                    null), forWorkbench);
        }

        private static OpenVisionNativeSimplePreprocessToolDescriptor CreateDescriptor(
            string toolName,
            Action<SimplePreprocessToolWpfView> configureView,
            Func<SimplePreprocessToolWpfView, OpenVisionNativePreviewOperation> capturePreview,
            Func<SimplePreprocessToolWpfView, string, string, VisionPipelineStep> createStep,
            bool normalizeSingleChannelInput = true)
        {
            // SimplePreprocess tools use the same output naming rule, so new tools only declare the stable tool name.
            return new OpenVisionNativeSimplePreprocessToolDescriptor(
                toolName,
                toolName + "_Preview",
                configureView,
                capturePreview,
                createStep,
                normalizeSingleChannelInput);
        }

        private static OpenVisionNativeToolDocument CreateSimplePreprocessDocument(
            IDisplayManager displayManager,
            OpenVisionNativeSimplePreprocessToolDescriptor descriptor, bool forWorkbench)
        {
            if (descriptor == null)
            {
                throw new ArgumentNullException(nameof(descriptor));
            }

            // SimplePreprocess tools share one shell; only parameter setup, preview, and pipeline step differ.
            SimplePreprocessToolWpfView view = new SimplePreprocessToolWpfView();
            descriptor.ConfigureView(view);
            string settingsConfigName = OpenVisionNativeToolSettingsStore.CreateConfigName(descriptor.ToolName);
            SimplePreprocessToolSettings defaults = view.Parameters.CaptureSettings();
            view.Parameters.ApplySettings(forWorkbench ? OpenVisionNativeToolSettingsStore.LoadExisting(settingsConfigName, defaults)
                : OpenVisionNativeToolSettingsStore.Load(settingsConfigName, defaults));

            Func<string, string, VisionPipelineStep> stepFactory = descriptor.CreateStep == null
                ? null
                : (inputLayer, outputLayer) => descriptor.CreateStep(view, inputLayer, outputLayer);

            OpenVisionNativeToolDocument document = OpenVisionNativeSingleInputToolDocumentBuilder.Create(
                displayManager,
                view,
                descriptor.ToolName,
                descriptor.DefaultOutputLayer,
                null,
                stepFactory == null ? null : (activeView, inputLayer, outputLayer) => stepFactory(inputLayer, outputLayer),
                descriptor.NormalizeSingleChannelInput,
                descriptor.CapturePreview);
            view.ParameterChanged += (sender, e) => document.RecordParameterChange(() => OpenVisionNativeToolSettingsStore.Save(settingsConfigName, view.Parameters.CaptureSettings()));
            document.ConfigureParameterState(forWorkbench, () => view.Parameters.CaptureSettings(),
                snapshot => view.Parameters.ApplySettings((SimplePreprocessToolSettings)snapshot),
                context => OpenVisionNativeToolSettingsStore.LoadExisting(settingsConfigName, defaults, context.Name),
                step => view.Parameters.ApplySettings(VisionPipelineStepPropertyMapper.CreateSimplePreprocessSettings(step, defaults)), createDefaults: () => defaults);
            return document;
        }

    }
}
