using OpenVisionLab.Vision2D.Pipeline;
using OpenVisionLab.Vision2D.Tool;
using OpenCvSharp;
using System;

namespace OpenVisionLab
{
    internal sealed class OpenVisionNativeSimplePreprocessToolDescriptor
    {
        public OpenVisionNativeSimplePreprocessToolDescriptor(
            string toolName,
            string defaultOutputLayer,
            Action<SimplePreprocessToolWpfView> configureView,
            Func<Mat, SimplePreprocessToolWpfView, VisionToolResult> executePreview,
            Func<SimplePreprocessToolWpfView, string, string, VisionPipelineStep> createStep,
            bool normalizeSingleChannelInput = true)
        {
            ToolName = string.IsNullOrWhiteSpace(toolName) ? throw new ArgumentException("Tool name is required.", nameof(toolName)) : toolName;
            DefaultOutputLayer = string.IsNullOrWhiteSpace(defaultOutputLayer) ? ToolName + "_Preview" : defaultOutputLayer;
            ConfigureView = configureView ?? throw new ArgumentNullException(nameof(configureView));
            ExecutePreview = executePreview ?? throw new ArgumentNullException(nameof(executePreview));
            CreateStep = createStep;
            NormalizeSingleChannelInput = normalizeSingleChannelInput;
        }

        public string ToolName { get; }

        public string DefaultOutputLayer { get; }

        public Action<SimplePreprocessToolWpfView> ConfigureView { get; }

        public Func<Mat, SimplePreprocessToolWpfView, VisionToolResult> ExecutePreview { get; }

        public Func<SimplePreprocessToolWpfView, string, string, VisionPipelineStep> CreateStep { get; }

        public bool NormalizeSingleChannelInput { get; }
    }
}
