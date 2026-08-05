using OpenVisionLab.Vision2D.Property;
using OpenVisionLab.Vision2D.Tool;
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
            OpenVisionNativeToolDocument document = OpenVisionToolOpenProfiler.Measure(
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
            return ApplyLoadFailureStatus(document, property.NAME);
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
            OpenVisionNativeToolDocument document = OpenVisionToolOpenProfiler.Measure(
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
            return ApplyLoadFailureStatus(document, property.NAME);
        }

        internal static OpenVisionNativeToolDocument ApplyLoadFailureStatus(
            OpenVisionNativeToolDocument document,
            params string[] propertyKeys)
        {
            if (document == null || propertyKeys == null)
            {
                return document;
            }

            foreach (string propertyKey in propertyKeys)
            {
                if (!OpenVisionNativeToolPropertySessionStore.TryGetLoadFailure(
                        propertyKey,
                        out OpenVisionNativeToolPropertyLoadFailure failure))
                {
                    continue;
                }

                string recipeName = string.IsNullOrWhiteSpace(failure.RecipeName)
                    ? LocalizedText(
                        "VisionTool.Persistence.DefaultRecipe",
                        "default Recipe")
                    : failure.RecipeName;
                string errorMessage = string.IsNullOrWhiteSpace(failure.ErrorMessage)
                    ? LocalizedText(
                        "VisionTool.Persistence.UnknownError",
                        "unknown error")
                    : failure.ErrorMessage;
                string format;
                object[] arguments;
                if (failure.PreviousFileWasBackedUp)
                {
                    format = LocalizedText(
                        "VisionTool.Persistence.LoadRecoveredInvalidFormat",
                        "{0} / Recipe {1}: Saved settings were invalid or incompatible, "
                        + "so this Tool opened with default values. Do not assume prior teaching was restored. "
                        + "Review the values. The previous file was preserved at {2}. Cause: {3}");
                    arguments = new object[]
                    {
                        failure.ToolName,
                        recipeName,
                        failure.BackupPath,
                        errorMessage
                    };
                }
                else
                {
                    format = LocalizedText(
                        "VisionTool.Persistence.LoadFailedFormat",
                        "{0} / Recipe {1}: Saved settings could not be loaded, "
                        + "so this Tool opened with default values. Do not assume prior teaching was restored. "
                        + "Review the values; the saved file was not changed. Cause: {2}");
                    arguments = new object[]
                    {
                        failure.ToolName,
                        recipeName,
                        errorMessage
                    };
                }

                document.SetPropertyPersistenceStatus(
                    string.Format(
                        System.Globalization.CultureInfo.CurrentCulture,
                        format,
                        arguments));
                break;
            }

            return document;
        }

        private static string LocalizedText(string key, string fallbackText)
        {
            string value = OpenVisionLanguageService.T(key);
            return string.IsNullOrWhiteSpace(value)
                || string.Equals(value, key, StringComparison.Ordinal)
                ? fallbackText ?? string.Empty
                : value;
        }
    }
}
