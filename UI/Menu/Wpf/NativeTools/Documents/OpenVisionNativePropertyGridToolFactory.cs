using Lib.OpenCV.Property;
using Lib.OpenCV.Tool;
using OpenCvSharp;
using OpenVisionLab.Core;
using OpenVisionLab.Composition;
using OpenVisionLab.Contracts;
using OpenVisionLab.Vision._1._Tools.OpenCV;
using System;
using System.Windows;

namespace OpenVisionLab
{
    internal static class OpenVisionNativePropertyGridToolFactory
    {
        // PropertyGrid tools stay model-driven: edit the property model and let the grid bridge create the UI.
        public static OpenVisionNativeToolDocument CreateBlob(IDisplayManager displayManager)
        {
            BlobProperty property = OpenVisionToolOpenProfiler.Measure(
                "CreateBlobProperty",
                () => OpenVisionNativeToolPropertySessionStore.GetRepositoryProperty(
                    "Blob_1",
                    repository => repository.Blobs,
                    () => new BlobProperty("Blob_1")));
            return CreatePropertyGridDocument(
                displayManager,
                "Blob",
                "Blob_Preview",
                property,
                item => VisionToolCompositionService.CreateBlobToolViewModel(item),
                presenter => new BlobToolWpfView(presenter),
                OpenVisionNativeToolPreviewExecutor.ExecuteBlobPreview,
                VisionToolAreaVerificationCriteriaText.CreateBlob);
        }

        public static OpenVisionNativeToolDocument CreateContour(IDisplayManager displayManager)
        {
            ContourProperty property = OpenVisionToolOpenProfiler.Measure(
                "CreateContourProperty",
                () => OpenVisionNativeToolPropertySessionStore.GetRepositoryProperty(
                    "Contour_1",
                    repository => repository.Contours,
                    () => new ContourProperty("Contour_1")));
            return CreatePropertyGridDocument(
                displayManager,
                "Contour",
                "Contour_Preview",
                property,
                item => VisionToolCompositionService.CreateContourToolViewModel(item),
                presenter => new ContourToolWpfView(presenter),
                OpenVisionNativeToolPreviewExecutor.ExecuteContourPreview,
                VisionToolAreaVerificationCriteriaText.CreateContour);
        }

        public static OpenVisionNativeToolDocument CreateAffineTransform(IDisplayManager displayManager)
        {
            AffineTransformProperty property = OpenVisionToolOpenProfiler.Measure(
                "CreateAffineTransformProperty",
                () => OpenVisionNativeToolPropertySessionStore.GetOrLoad(
                    "AffineTransform_1",
                    () => new AffineTransformProperty("AffineTransform_1")));
            return CreatePropertyGridDocument(
                displayManager,
                "AffineTransform",
                "AffineTransform_Preview",
                property,
                item => VisionToolCompositionService.CreateAffineTransformToolViewModel(item),
                presenter => new AffineTransformToolWpfView(presenter),
                OpenVisionNativeToolPreviewExecutor.ExecuteAffineTransformPreview);
        }

        public static OpenVisionNativeToolDocument CreateMatching(IDisplayManager displayManager)
        {
            MatchingProperty property = OpenVisionToolOpenProfiler.Measure(
                "CreateMatchingProperty",
                () => OpenVisionNativeToolPropertySessionStore.GetRepositoryProperty(
                    "Matching_1",
                    repository => repository.Matchings,
                    () => new MatchingProperty("Matching_1")));
            return CreateTemplatePropertyGridDocument(
                displayManager,
                "Matching",
                "Matching_Preview",
                property,
                item => VisionToolCompositionService.CreateMatchingToolViewModel(item),
                presenter => new MatchingToolWpfView(presenter),
                OpenVisionNativeToolPreviewExecutor.ExecuteMatchingPreview);
        }

        public static OpenVisionNativeToolDocument CreateEdgeBasedMatching(IDisplayManager displayManager)
        {
            EdgeBasedMatchingProperty property = OpenVisionToolOpenProfiler.Measure(
                "CreateEdgeBasedMatchingProperty",
                () => OpenVisionNativeToolPropertySessionStore.GetRepositoryProperty(
                    "EdgeBasedMatching_1",
                    repository => repository.EdgeBasedMatchings,
                    () => new EdgeBasedMatchingProperty("EdgeBasedMatching_1")));
            return CreateTemplatePropertyGridDocument(
                displayManager,
                "EdgeBasedMatching",
                "EdgeBasedMatching_Preview",
                property,
                item => VisionToolCompositionService.CreateEdgeBasedMatchingToolViewModel(item),
                presenter => new EdgeBasedMatchingToolWpfView(presenter),
                OpenVisionNativeToolPreviewExecutor.ExecuteEdgeBasedMatchingPreview);
        }

        public static OpenVisionNativeToolDocument CreateFeatureMatching(IDisplayManager displayManager)
        {
            FeatureMatchingProperty property = OpenVisionToolOpenProfiler.Measure(
                "CreateFeatureMatchingProperty",
                () => OpenVisionNativeToolPropertySessionStore.GetRepositoryProperty(
                    "Feature_1",
                    repository => repository.Features,
                    () => new FeatureMatchingProperty("Feature_1")));
            return CreateTemplatePropertyGridDocument(
                displayManager,
                "FeatureMatching",
                "FeatureMatching_Preview",
                property,
                item => VisionToolCompositionService.CreateFeatureMatchingToolViewModel(item),
                presenter => new FeatureMatchingToolWpfView(presenter),
                OpenVisionNativeToolPreviewExecutor.ExecuteFeatureMatchingPreview);
        }

        private static OpenVisionNativeToolDocument CreatePropertyGridDocument<TView, TProperty>(
            IDisplayManager displayManager,
            string toolName,
            string defaultOutputLayer,
            TProperty property,
            Func<TProperty, IPropertyGridToolViewModel<TProperty>> createViewModel,
            Func<VisionToolPropertyGridPresenter<TProperty>, TView> createView,
            Func<Mat, TView, VisionToolResult> executePreview,
            Func<TProperty, string> createDisplaySummary = null)
            where TView : FrameworkElement, ISingleInputVisionToolWpfView
            where TProperty : OpenCvPropertyBase
        {
            IPropertyGridToolViewModel<TProperty> viewModel = OpenVisionToolOpenProfiler.Measure("CreatePropertyGridViewModel", () => createViewModel(property));
            return OpenVisionToolOpenProfiler.Measure(
                "CreatePropertyGridDocument",
                () => OpenVisionNativePropertyGridToolDocumentBuilder.Create(
                displayManager,
                new OpenVisionNativePropertyGridToolDescriptor<TView, TProperty>(
                    toolName,
                    defaultOutputLayer,
                    property,
                    viewModel.CreateProperty,
                    () => createDisplaySummary == null ? viewModel.Summary : createDisplaySummary(property),
                    createView,
                    executePreview,
                    persistSelectedObject: () => OpenVisionNativeToolPropertySessionStore.Save(toolName, property))));
        }

        private static OpenVisionNativeToolDocument CreateTemplatePropertyGridDocument<TView, TProperty>(
            IDisplayManager displayManager,
            string toolName,
            string defaultOutputLayer,
            TProperty property,
            Func<TProperty, ITemplateBackedPropertyGridToolViewModel<TProperty>> createViewModel,
            Func<VisionToolPropertyGridPresenter<TProperty>, TView> createView,
            Func<Mat, TView, VisionToolResult> executePreview)
            where TView : FrameworkElement, ISingleInputVisionToolWpfView
            where TProperty : OpenCvPropertyBase
        {
            ITemplateBackedPropertyGridToolViewModel<TProperty> viewModel = OpenVisionToolOpenProfiler.Measure("CreatePropertyGridViewModel", () => createViewModel(property));
            return OpenVisionToolOpenProfiler.Measure(
                "CreatePropertyGridDocument",
                () => OpenVisionNativePropertyGridToolDocumentBuilder.Create(
                displayManager,
                new OpenVisionNativePropertyGridToolDescriptor<TView, TProperty>(
                    toolName,
                    defaultOutputLayer,
                    property,
                    viewModel.CreateProperty,
                    () => viewModel.Summary,
                    createView,
                    executePreview,
                    getTemplateStatus: () => viewModel.TemplateStatus,
                    applyTemplatePathForTest: viewModel.ApplyTemplatePathForTest,
                    reloadTemplateIfPatternChanged: viewModel.ReloadTemplateIfPatternChanged,
                    persistSelectedObject: () => OpenVisionNativeToolPropertySessionStore.Save(toolName, property))));
        }
    }
}
