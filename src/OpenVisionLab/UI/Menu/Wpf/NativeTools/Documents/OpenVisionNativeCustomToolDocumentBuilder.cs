using Lib.OpenCV.Pipeline;
using Lib.OpenCV.Tool;
using OpenCvSharp;
using OpenVisionLab.Core;
using System;
using System.Windows;

namespace OpenVisionLab
{
    internal sealed class OpenVisionNativeCustomToolDescriptor<TView, TPresenter, TProperty>
        where TView : FrameworkElement, ISingleInputVisionToolWpfView
    {
        public OpenVisionNativeCustomToolDescriptor(
            string toolName,
            string defaultOutputLayer,
            TPresenter presenter,
            Func<TPresenter, TView> createView,
            Func<TView, TProperty> createProperty,
            Func<TProperty, IVisionTool> createTool,
            Func<TProperty, string, string, VisionPipelineStep> createStep)
            : this(
                toolName,
                defaultOutputLayer,
                presenter,
                createView,
                (source, view) => ExecutePropertyTool(source, view, createProperty, createTool),
                (view, inputLayer, outputLayer) => createStep(createProperty(view), inputLayer, outputLayer))
        {
        }

        public OpenVisionNativeCustomToolDescriptor(
            string toolName,
            string defaultOutputLayer,
            TPresenter presenter,
            Func<TPresenter, TView> createView,
            Func<Mat, TView, VisionToolResult> executePreview,
            Func<TView, string, string, VisionPipelineStep> createStep)
        {
            ToolName = string.IsNullOrWhiteSpace(toolName) ? throw new ArgumentException("Tool name is required.", nameof(toolName)) : toolName;
            DefaultOutputLayer = string.IsNullOrWhiteSpace(defaultOutputLayer) ? ToolName + "_Preview" : defaultOutputLayer;
            Presenter = presenter ?? throw new ArgumentNullException(nameof(presenter));
            CreateView = createView ?? throw new ArgumentNullException(nameof(createView));
            ExecutePreview = executePreview ?? throw new ArgumentNullException(nameof(executePreview));
            CreateStep = createStep ?? throw new ArgumentNullException(nameof(createStep));
        }

        public string ToolName { get; }

        public string DefaultOutputLayer { get; }

        public TPresenter Presenter { get; }

        public Func<TPresenter, TView> CreateView { get; }

        public Func<Mat, TView, VisionToolResult> ExecutePreview { get; }

        public Func<TView, string, string, VisionPipelineStep> CreateStep { get; }

        private static VisionToolResult ExecutePropertyTool(
            Mat source,
            TView view,
            Func<TView, TProperty> createProperty,
            Func<TProperty, IVisionTool> createTool)
        {
            if (createProperty == null)
            {
                throw new ArgumentNullException(nameof(createProperty));
            }

            if (createTool == null)
            {
                throw new ArgumentNullException(nameof(createTool));
            }

            TProperty property = createProperty(view);
            IVisionTool tool = createTool(property);
            return tool.Execute(source);
        }
    }

    internal static class OpenVisionNativeCustomToolDocumentBuilder
    {
        public static OpenVisionNativeToolDocument Create<TView, TPresenter, TProperty>(
            IDisplayManager displayManager,
            OpenVisionNativeCustomToolDescriptor<TView, TPresenter, TProperty> descriptor)
            where TView : FrameworkElement, ISingleInputVisionToolWpfView
        {
            if (descriptor == null)
            {
                throw new ArgumentNullException(nameof(descriptor));
            }

            // Custom tools use hand-built controls, but document wiring stays identical to the PropertyGrid tools.
            TView view = descriptor.CreateView(descriptor.Presenter);
            view.DataContext = descriptor.Presenter;
            return OpenVisionNativeSingleInputToolDocumentBuilder.Create(
                displayManager,
                view,
                descriptor.ToolName,
                descriptor.DefaultOutputLayer,
                descriptor.ExecutePreview,
                descriptor.CreateStep);
        }
    }
}
