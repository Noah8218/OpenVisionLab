using OpenVisionLab.Vision2D.Pipeline;
using OpenVisionLab.Vision2D.Property;
using OpenVisionLab.Vision2D.Tool;
using OpenVisionLab.Core;
using OpenVisionLab.Composition;
using OpenVisionLab.Contracts;
using System;
using System.Windows;

namespace OpenVisionLab
{
    internal static class OpenVisionNativeCustomToolFactory
    {
        // Custom tools keep dedicated WPF parameter views, while document/layer/pipeline wiring stays shared.
        public static OpenVisionNativeToolDocument CreateLine(IDisplayManager displayManager, bool forWorkbench = false)
        {
            LineGaugeProperty lineAProperty = forWorkbench ? OpenVisionNativeToolPropertySessionStore.LoadExisting("Line(L)_1", new LineGaugeProperty("Line(L)_1")) : OpenVisionNativeToolPropertySessionStore.GetRepositoryProperty(
                "Line(L)_1",
                repository => repository.Lines_L,
                () => new LineGaugeProperty("Line(L)_1"));
            LineGaugeProperty lineBProperty = forWorkbench ? OpenVisionNativeToolPropertySessionStore.LoadExisting("Line(R)_1", new LineGaugeProperty("Line(R)_1")) : OpenVisionNativeToolPropertySessionStore.GetRepositoryProperty(
                "Line(R)_1",
                repository => repository.Lines_R,
                () => new LineGaugeProperty("Line(R)_1"));
            var viewModel = VisionToolCompositionService.CreateLineToolViewModel(lineAProperty, lineBProperty);
            OpenVisionNativeToolDocument document = null;
            LineToolPresenter presenter = new LineToolPresenter(
                viewModel,
                () => document?.RecordParameterChange(() =>
                {
                    OpenVisionNativeToolPropertySessionStore.Save("Line(L)_1", lineAProperty);
                    OpenVisionNativeToolPropertySessionStore.Save("Line(R)_1", lineBProperty);
                }));
            document =
                OpenVisionNativeCustomToolDocumentBuilder.Create(
                displayManager,
                new OpenVisionNativeCustomToolDescriptor<LineToolWpfView, LineToolPresenter, LineGaugeProperty>(
                    "Line",
                    "Line_Preview",
                    presenter,
                    viewPresenter => new LineToolWpfView(viewPresenter),
                    null,
                    OpenVisionNativeToolStepFactory.CreateLineGaugeStep,
                    view =>
                    {
                        using System.Drawing.Bitmap input = displayManager.GetLayerImageSnapshot(view.SelectedInputLayer);
                        return OpenVisionNativeToolPreviewExecutor.CaptureLineGaugePreview(view, input?.Width ?? 0, input?.Height ?? 0);
                    }));
            LineToolWpfView lineView = (LineToolWpfView)document.View;
            var defaults = Tuple.Create(VisionPipelineStepPropertyMapper.CaptureNativeParameters(new LineGaugeProperty("Line(L)_1")),
                VisionPipelineStepPropertyMapper.CaptureNativeParameters(new LineGaugeProperty("Line(R)_1")), nameof(LineToolPurpose.Edge));
            document.ConfigureParameterState(forWorkbench,
                () => Tuple.Create(VisionPipelineStepPropertyMapper.CaptureNativeParameters(lineAProperty), VisionPipelineStepPropertyMapper.CaptureNativeParameters(lineBProperty), lineView.SelectedPurpose),
                snapshot =>
                {
                    var pair = (Tuple<byte[], byte[], string>)snapshot;
                    VisionPipelineStepPropertyMapper.RestoreNativeParameters(pair.Item1, lineAProperty);
                    VisionPipelineStepPropertyMapper.RestoreNativeParameters(pair.Item2, lineBProperty);
                    lineView.RefreshRestoredParameters(pair.Item3);
                },
                context => Tuple.Create(
                    VisionPipelineStepPropertyMapper.CaptureNativeParameters(OpenVisionNativeToolPropertySessionStore.LoadExisting("Line(L)_1", new LineGaugeProperty("Line(L)_1"), context.Name)),
                    VisionPipelineStepPropertyMapper.CaptureNativeParameters(OpenVisionNativeToolPropertySessionStore.LoadExisting("Line(R)_1", new LineGaugeProperty("Line(R)_1"), context.Name)), nameof(LineToolPurpose.Edge)),
                step =>
                {
                    object property = VisionPipelineStepPropertyMapper.CreateProperty(step);
                    if (VisionPipelineStepPropertyMapper.TryCreateLineGaugePair(property, out LineGaugeProperty left, out LineGaugeProperty right))
                    {
                        VisionPipelineStepPropertyMapper.ApplyNativeParameters(left, lineAProperty);
                        VisionPipelineStepPropertyMapper.ApplyNativeParameters(right, lineBProperty);
                    }
                    else if (property is LineGaugeProperty line) { VisionPipelineStepPropertyMapper.ApplyNativeParameters(line, lineAProperty); }
                    string purpose = step.Parameters.TryGetValue(LineToolWpfView.LinePurposeParameterName, out string selectedPurpose) ? selectedPurpose
                        : string.Equals(step.ToolType, "LineDistance", StringComparison.OrdinalIgnoreCase) ? nameof(LineToolPurpose.Measure)
                        : string.Equals(step.ToolType, "LineIntersection", StringComparison.OrdinalIgnoreCase) ? nameof(LineToolPurpose.Intersection) : nameof(LineToolPurpose.Edge);
                    lineView.RefreshRestoredParameters(purpose);
                }, createDefaults: () => defaults);
            return OpenVisionNativePropertyGridToolFactory.ApplyLoadFailureStatus(
                document,
                lineAProperty.NAME,
                lineBProperty.NAME);
        }

        public static OpenVisionNativeToolDocument CreateFilter(IDisplayManager displayManager, bool forWorkbench = false)
        {
            OpenVisionNativeToolDocument document = null;
            string configName = OpenVisionNativeToolSettingsStore.CreateConfigName("Filter");
            IFilterToolViewModel viewModel = VisionToolCompositionService.CreateFilterToolViewModel(
                settings => document?.RecordParameterChange(() => OpenVisionNativeToolSettingsStore.Save(configName, settings)), forWorkbench);
            FilterToolPresenter presenter = new FilterToolPresenter(viewModel);
            document = CreateSinglePropertyToolDocument<FilterToolWpfView, FilterToolPresenter, FilterToolProperty>(
                displayManager,
                "Filter",
                "Filter_Preview",
                presenter,
                viewPresenter => new FilterToolWpfView(viewPresenter),
                property =>
                {
                    FilterTool tool = new FilterTool();
                    try
                    {
                        tool.SetProperty(property);
                        return tool;
                    }
                    catch
                    {
                        tool.Dispose();
                        throw;
                    }
                },
                (property, inputLayer, outputLayer) => VisionPipelineStepBuilder.FromFilterProperty(
                    property,
                    "Filter",
                    inputLayer,
                    outputLayer));
            FilterToolWpfView view = (FilterToolWpfView)document.View;
            document.ConfigureParameterState(forWorkbench, () => viewModel.CaptureSettings(),
                snapshot => view.RefreshRestoredParameters(() => viewModel.ApplySettings((FilterToolSettings)snapshot)),
                context => OpenVisionNativeToolSettingsStore.LoadExisting(configName, new FilterToolSettings(), context.Name),
                step => view.RefreshRestoredParameters(() => viewModel.ApplySettings(VisionPipelineStepPropertyMapper.CreateFilterSettings(step))),
                captureStep: (input, output) => VisionPipelineStepBuilder.FromFilterProperty(viewModel.CreateProperty(), "Filter", input, output),
                createDefaults: () => new FilterToolSettings());
            return document;
        }

        public static OpenVisionNativeToolDocument CreateThreshold(IDisplayManager displayManager, bool forWorkbench = false)
        {
            OpenVisionNativeToolDocument document = null;
            string configName = OpenVisionNativeToolSettingsStore.CreateConfigName("Threshold");
            IThresholdToolViewModel viewModel = VisionToolCompositionService.CreateThresholdToolViewModel(
                settings => document?.RecordParameterChange(() => OpenVisionNativeToolSettingsStore.Save(configName, settings)), forWorkbench);
            ThresholdToolPresenter presenter = new ThresholdToolPresenter(viewModel);
            document = OpenVisionNativeCustomToolDocumentBuilder.Create(
                displayManager,
                new OpenVisionNativeCustomToolDescriptor<ThresholdToolWpfView, ThresholdToolPresenter, ThresholdToolProperty>(
                    "Threshold",
                    "Threshold_Preview",
                    presenter,
                    viewPresenter => new ThresholdToolWpfView(viewPresenter),
                    null,
                    (view, inputLayer, outputLayer) => VisionPipelineStepBuilder.FromThresholdProperty(
                        view.CreateProperty(),
                        "Threshold",
                        inputLayer,
                        outputLayer),
                    OpenVisionNativeThresholdPreviewExecutor.Capture));
            ThresholdToolWpfView view = (ThresholdToolWpfView)document.View;
            document.ConfigureParameterState(forWorkbench, () => viewModel.CaptureSettings(),
                snapshot => { viewModel.ApplySettings((ThresholdToolSettings)snapshot); view.RefreshRestoredParameters(); },
                context => OpenVisionNativeToolSettingsStore.LoadExisting(configName, new ThresholdToolSettings(), context.Name),
                step => { viewModel.ApplySettings(VisionPipelineStepPropertyMapper.CreateThresholdSettings(step)); view.RefreshRestoredParameters(); },
                captureStep: (input, output) => VisionPipelineStepBuilder.FromThresholdProperty(viewModel.CreateProperty(), "Threshold", input, output),
                createDefaults: () => new ThresholdToolSettings());
            return document;
        }

        public static OpenVisionNativeToolDocument CreateMorphology(IDisplayManager displayManager, bool forWorkbench = false)
        {
            OpenVisionNativeToolDocument document = null;
            string configName = OpenVisionNativeToolSettingsStore.CreateConfigName("Morphology");
            IMorphologyToolViewModel viewModel = VisionToolCompositionService.CreateMorphologyToolViewModel(
                settings => document?.RecordParameterChange(() => OpenVisionNativeToolSettingsStore.Save(configName, settings)), forWorkbench);
            MorphologyToolPresenter presenter = new MorphologyToolPresenter(viewModel);
            document = CreateSinglePropertyToolDocument<MorphologyToolWpfView, MorphologyToolPresenter, MorphologyToolProperty>(
                displayManager,
                "Morphology",
                "Morphology_Preview",
                presenter,
                viewPresenter => new MorphologyToolWpfView(viewPresenter),
                property =>
                {
                    MorphologyTool tool = new MorphologyTool();
                    try
                    {
                        tool.SetProperty(property);
                        return tool;
                    }
                    catch
                    {
                        tool.Dispose();
                        throw;
                    }
                },
                (property, inputLayer, outputLayer) => VisionPipelineStepBuilder.FromMorphologyProperty(
                    property,
                    "Morphology",
                    inputLayer,
                    outputLayer));
            MorphologyToolWpfView view = (MorphologyToolWpfView)document.View;
            document.ConfigureParameterState(forWorkbench, () => viewModel.CaptureSettings(),
                snapshot => { viewModel.ApplySettings((MorphologyToolSettings)snapshot); view.RefreshRestoredParameters(); },
                context => OpenVisionNativeToolSettingsStore.LoadExisting(configName, new MorphologyToolSettings(), context.Name),
                step => { viewModel.ApplySettings(VisionPipelineStepPropertyMapper.CreateMorphologySettings(step)); view.RefreshRestoredParameters(); },
                captureStep: (input, output) => VisionPipelineStepBuilder.FromMorphologyProperty(viewModel.CreateProperty(), "Morphology", input, output),
                createDefaults: () => new MorphologyToolSettings());
            return document;
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
