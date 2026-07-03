using Lib.OpenCV.Pipeline;
using Lib.OpenCV.Tool;
using OpenCvSharp;
using OpenVisionLab._1._Core;
using OpenVisionLab.Contracts;
using OpenVisionLab.Vision._1._Tools.OpenCV;
using System;
using System.Windows;

namespace OpenVisionLab
{
    internal sealed class OpenVisionNativePropertyGridToolDescriptor<TView, TProperty>
        where TView : FrameworkElement, ISingleInputVisionToolWpfView
        where TProperty : OpenCvPropertyBase
    {
        public OpenVisionNativePropertyGridToolDescriptor(
            string toolName,
            string defaultOutputLayer,
            TProperty selectedObject,
            Func<TProperty> createProperty,
            Func<string> getSummary,
            Func<VisionToolPropertyGridPresenter<TProperty>, TView> createView,
            Func<Mat, TView, VisionToolResult> executePreview,
            Func<TProperty, string, string, VisionPipelineStep> createStep = null,
            Func<VisionToolTemplateStatus> getTemplateStatus = null,
            Action<string> applyTemplatePathForTest = null,
            Action<string> reloadTemplateIfPatternChanged = null,
            Action persistSelectedObject = null)
        {
            ToolName = string.IsNullOrWhiteSpace(toolName) ? throw new ArgumentException("Tool name is required.", nameof(toolName)) : toolName;
            DefaultOutputLayer = string.IsNullOrWhiteSpace(defaultOutputLayer) ? ToolName + "_Preview" : defaultOutputLayer;
            SelectedObject = selectedObject ?? throw new ArgumentNullException(nameof(selectedObject));
            CreateProperty = createProperty ?? throw new ArgumentNullException(nameof(createProperty));
            GetSummary = getSummary ?? throw new ArgumentNullException(nameof(getSummary));
            CreateView = createView ?? throw new ArgumentNullException(nameof(createView));
            ExecutePreview = executePreview ?? throw new ArgumentNullException(nameof(executePreview));
            CreateStep = createStep ?? VisionPipelineStepBuilder.FromProperty;
            GetTemplateStatus = getTemplateStatus;
            ApplyTemplatePathForTest = applyTemplatePathForTest;
            ReloadTemplateIfPatternChanged = reloadTemplateIfPatternChanged;
            PersistSelectedObject = persistSelectedObject;
        }

        public string ToolName { get; }

        public string DefaultOutputLayer { get; }

        public TProperty SelectedObject { get; }

        public Func<TProperty> CreateProperty { get; }

        public Func<string> GetSummary { get; }

        public Func<VisionToolPropertyGridPresenter<TProperty>, TView> CreateView { get; }

        public Func<Mat, TView, VisionToolResult> ExecutePreview { get; }

        public Func<TProperty, string, string, VisionPipelineStep> CreateStep { get; }

        public Func<VisionToolTemplateStatus> GetTemplateStatus { get; }

        public Action<string> ApplyTemplatePathForTest { get; }

        public Action<string> ReloadTemplateIfPatternChanged { get; }

        public Action PersistSelectedObject { get; }
    }

    internal static class OpenVisionNativePropertyGridToolDocumentBuilder
    {
        public static OpenVisionNativeToolDocument Create<TView, TProperty>(
            IDisplayManager displayManager,
            OpenVisionNativePropertyGridToolDescriptor<TView, TProperty> descriptor)
            where TView : FrameworkElement, ISingleInputVisionToolWpfView
            where TProperty : OpenCvPropertyBase
        {
            if (descriptor == null)
            {
                throw new ArgumentNullException(nameof(descriptor));
            }

            VisionToolPropertyGridPresenter<TProperty> presenter = OpenVisionToolOpenProfiler.Measure(
                "CreatePropertyGridPresenter",
                () => new VisionToolPropertyGridPresenter<TProperty>(
                descriptor.SelectedObject,
                descriptor.CreateProperty,
                descriptor.GetSummary,
                descriptor.GetTemplateStatus,
                descriptor.ApplyTemplatePathForTest,
                descriptor.ReloadTemplateIfPatternChanged,
                descriptor.PersistSelectedObject));

            TView view = OpenVisionToolOpenProfiler.Measure("CreatePropertyGridView", () => descriptor.CreateView(presenter));
            return OpenVisionToolOpenProfiler.Measure(
                "CreateNativeToolDocument",
                () => OpenVisionNativeSingleInputToolDocumentBuilder.Create(
                displayManager,
                view,
                descriptor.ToolName,
                descriptor.DefaultOutputLayer,
                descriptor.ExecutePreview,
                (activeView, inputLayer, outputLayer) => descriptor.CreateStep(descriptor.CreateProperty(), inputLayer, outputLayer)));
        }
    }
}
