using Lib.OpenCV.Pipeline;
using Lib.OpenCV.Property;
using Lib.OpenCV.Tool;
using OpenVisionLab.Core;
using OpenVisionLab.Composition;
using System;
using System.Windows;

namespace OpenVisionLab
{
    internal static class OpenVisionNativeCustomToolFactory
    {
        // Custom tools keep dedicated WPF parameter views, while document/layer/pipeline wiring stays shared.
        public static OpenVisionNativeToolDocument CreateLine(IDisplayManager displayManager)
        {
            LineGaugeProperty lineAProperty = OpenVisionNativeToolPropertySessionStore.GetRepositoryProperty(
                "Line(L)_1",
                repository => repository.Lines_L,
                () => new LineGaugeProperty("Line(L)_1"));
            LineGaugeProperty lineBProperty = OpenVisionNativeToolPropertySessionStore.GetRepositoryProperty(
                "Line(R)_1",
                repository => repository.Lines_R,
                () => new LineGaugeProperty("Line(R)_1"));
            var viewModel = VisionToolCompositionService.CreateLineToolViewModel(lineAProperty, lineBProperty);
            LineToolPresenter presenter = new LineToolPresenter(viewModel);
            return OpenVisionNativeCustomToolDocumentBuilder.Create(
                displayManager,
                new OpenVisionNativeCustomToolDescriptor<LineToolWpfView, LineToolPresenter, LineGaugeProperty>(
                    "Line",
                    "Line_Preview",
                    presenter,
                    viewPresenter => new LineToolWpfView(viewPresenter),
                    OpenVisionNativeToolPreviewExecutor.ExecuteLineGaugePreview,
                    OpenVisionNativeToolStepFactory.CreateLineGaugeStep));
        }

        public static OpenVisionNativeToolDocument CreateFilter(IDisplayManager displayManager)
        {
            FilterToolPresenter presenter = new FilterToolPresenter(VisionToolCompositionService.CreateFilterToolViewModel());
            return CreateSinglePropertyToolDocument<FilterToolWpfView, FilterToolPresenter, FilterToolProperty>(
                displayManager,
                "Filter",
                "Filter_Preview",
                presenter,
                viewPresenter => new FilterToolWpfView(viewPresenter),
                property =>
                {
                    FilterTool tool = new FilterTool();
                    tool.SetProperty(property);
                    return tool;
                },
                (property, inputLayer, outputLayer) => VisionPipelineStepBuilder.FromFilterProperty(
                    property,
                    "Filter",
                    inputLayer,
                    outputLayer));
        }

        public static OpenVisionNativeToolDocument CreateThreshold(IDisplayManager displayManager)
        {
            ThresholdToolPresenter presenter = new ThresholdToolPresenter(VisionToolCompositionService.CreateThresholdToolViewModel());
            return OpenVisionNativeCustomToolDocumentBuilder.Create(
                displayManager,
                new OpenVisionNativeCustomToolDescriptor<ThresholdToolWpfView, ThresholdToolPresenter, ThresholdToolProperty>(
                    "Threshold",
                    "Threshold_Preview",
                    presenter,
                    viewPresenter => new ThresholdToolWpfView(viewPresenter),
                    OpenVisionNativeThresholdPreviewExecutor.Execute,
                    (view, inputLayer, outputLayer) => VisionPipelineStepBuilder.FromThresholdProperty(
                        view.CreateProperty(),
                        "Threshold",
                        inputLayer,
                        outputLayer)));
        }

        public static OpenVisionNativeToolDocument CreateMorphology(IDisplayManager displayManager)
        {
            MorphologyToolPresenter presenter = new MorphologyToolPresenter(VisionToolCompositionService.CreateMorphologyToolViewModel());
            return CreateSinglePropertyToolDocument<MorphologyToolWpfView, MorphologyToolPresenter, MorphologyToolProperty>(
                displayManager,
                "Morphology",
                "Morphology_Preview",
                presenter,
                viewPresenter => new MorphologyToolWpfView(viewPresenter),
                property =>
                {
                    MorphologyTool tool = new MorphologyTool();
                    tool.SetProperty(property);
                    return tool;
                },
                (property, inputLayer, outputLayer) => VisionPipelineStepBuilder.FromMorphologyProperty(
                    property,
                    "Morphology",
                    inputLayer,
                    outputLayer));
        }

        private static OpenVisionNativeToolDocument CreateSinglePropertyToolDocument<TView, TPresenter, TProperty>(
            IDisplayManager displayManager,
            string toolName,
            string defaultOutputLayer,
            TPresenter presenter,
            Func<TPresenter, TView> createView,
            Func<TProperty, IVisionTool> createTool,
            Func<TProperty, string, string, VisionPipelineStep> createStep)
            where TView : FrameworkElement, ISingleInputPropertyVisionToolWpfView<TProperty>
        {
            // Single-property custom tools differ in View/Tool/Step types, but their document wiring is identical.
            return OpenVisionNativeCustomToolDocumentBuilder.Create(
                displayManager,
                new OpenVisionNativeCustomToolDescriptor<TView, TPresenter, TProperty>(
                    toolName,
                    defaultOutputLayer,
                    presenter,
                    createView,
                    view => view.CreateProperty(),
                    createTool,
                    createStep));
        }
    }
}
