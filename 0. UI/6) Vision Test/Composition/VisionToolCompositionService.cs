using OpenVisionLab.Contracts;
using OpenVisionLab.ViewModels;
using OpenVisionLab.Vision._1._Tools.OpenCV;
using System.Collections.Generic;

namespace OpenVisionLab.Composition
{
    internal static class VisionToolCompositionService
    {
        // Composition roots ask this service for collaborators; WPF views receive contracts through constructors.
        // Add ViewModel factories here after choosing the tool lane documented in VISION_TOOL_NATIVE_WPF_EXTENSION_GUIDE.
        // For algorithm tools, ViewModels support summaries/normalization around PropertyGrid-owned property models; they do not replace the PropertyGrid UI contract.
        public static IThresholdToolViewModel CreateThresholdToolViewModel()
        {
            var viewModel = new ThresholdToolViewModel();
            viewModel.ApplySettings(OpenVisionNativeToolSettingsStore.Load(
                OpenVisionNativeToolSettingsStore.CreateConfigName("Threshold"),
                new ThresholdToolSettings()));
            return viewModel;
        }

        public static IFilterToolViewModel CreateFilterToolViewModel()
        {
            var viewModel = new FilterToolViewModel();
            viewModel.ApplySettings(OpenVisionNativeToolSettingsStore.Load(
                OpenVisionNativeToolSettingsStore.CreateConfigName("Filter"),
                new FilterToolSettings()));
            return viewModel;
        }

        public static IMorphologyToolViewModel CreateMorphologyToolViewModel()
        {
            var viewModel = new MorphologyToolViewModel();
            viewModel.ApplySettings(OpenVisionNativeToolSettingsStore.Load(
                OpenVisionNativeToolSettingsStore.CreateConfigName("Morphology"),
                new MorphologyToolSettings()));
            return viewModel;
        }

        public static IBlobToolViewModel CreateBlobToolViewModel(BlobProperty property)
        {
            return new BlobToolViewModel(property);
        }

        public static IContourToolViewModel CreateContourToolViewModel(ContourProperty property)
        {
            return new ContourToolViewModel(property);
        }

        public static IMatchingToolViewModel CreateMatchingToolViewModel(MatchingProperty property)
        {
            return new MatchingToolViewModel(property);
        }

        public static IFeatureMatchingToolViewModel CreateFeatureMatchingToolViewModel(FeatureMatchingProperty property)
        {
            return new FeatureMatchingToolViewModel(property);
        }

        public static IEdgeBasedMatchingToolViewModel CreateEdgeBasedMatchingToolViewModel(EdgeBasedMatchingProperty property)
        {
            return new EdgeBasedMatchingToolViewModel(property);
        }

        public static ILineToolViewModel CreateLineToolViewModel(LineGaugeProperty lineAProperty, LineGaugeProperty lineBProperty)
        {
            return new LineToolViewModel(lineAProperty, lineBProperty);
        }
        public static IVisionToolOpenGlPreviewCanvas CreateOpenGlPreviewCanvas(string textureName)
        {
            return new VisionToolOpenGlPreviewCanvasAdapter(textureName);
        }

        public static IVisionToolLayerSelectionViewModel CreateSingleLayerSelection(
            IEnumerable<string> layerNames,
            string selectedInputLayer,
            string selectedOutputLayer)
        {
            return VisionToolLayerSelectionViewModel.CreateSingle(layerNames, selectedInputLayer, selectedOutputLayer);
        }

        public static IVisionToolLayerSelectionViewModel CreateDualLayerSelection(
            IEnumerable<string> layerNames,
            string selectedInputLayerA,
            string selectedInputLayerB,
            string selectedOutputLayer)
        {
            return VisionToolLayerSelectionViewModel.CreateDual(
                layerNames,
                selectedInputLayerA,
                selectedInputLayerB,
                selectedOutputLayer);
        }
    }
}
