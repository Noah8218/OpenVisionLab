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
        public static OpenVisionNativeToolDocument CreateBlob(IDisplayManager displayManager, bool forWorkbench = false)
        {
            BlobProperty property = OpenVisionToolOpenProfiler.Measure(
                "CreateBlobProperty",
                () => forWorkbench ? OpenVisionNativeToolPropertySessionStore.LoadExisting("Blob_1", new BlobProperty("Blob_1")) : OpenVisionNativeToolPropertySessionStore.GetRepositoryProperty(
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
                null,
                VisionToolAreaVerificationCriteriaText.CreateBlob,
                OpenVisionNativeToolPreviewExecutor.CaptureBlobPreview, forWorkbench);
        }

        public static OpenVisionNativeToolDocument CreateContour(IDisplayManager displayManager, bool forWorkbench = false)
        {
            ContourProperty property = OpenVisionToolOpenProfiler.Measure(
                "CreateContourProperty",
                () => forWorkbench ? OpenVisionNativeToolPropertySessionStore.LoadExisting("Contour_1", new ContourProperty("Contour_1")) : OpenVisionNativeToolPropertySessionStore.GetRepositoryProperty(
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
                null,
                VisionToolAreaVerificationCriteriaText.CreateContour,
                OpenVisionNativeToolPreviewExecutor.CaptureContourPreview, forWorkbench);
        }

        public static OpenVisionNativeToolDocument CreateAffineTransform(IDisplayManager displayManager, bool forWorkbench = false)
        {
            AffineTransformProperty property = OpenVisionToolOpenProfiler.Measure(
                "CreateAffineTransformProperty",
                () => forWorkbench ? OpenVisionNativeToolPropertySessionStore.LoadExisting("AffineTransform_1", new AffineTransformProperty("AffineTransform_1")) : OpenVisionNativeToolPropertySessionStore.GetOrLoad(
                    "AffineTransform_1",
                    () => new AffineTransformProperty("AffineTransform_1")));
            return CreatePropertyGridDocument(
                displayManager,
                "AffineTransform",
                "AffineTransform_Preview",
                property,
                item => VisionToolCompositionService.CreateAffineTransformToolViewModel(item),
                presenter => new AffineTransformToolWpfView(presenter),
                null,
                capturePreview: OpenVisionNativeToolPreviewExecutor.CaptureAffineTransformPreview, forWorkbench: forWorkbench);
        }

        public static OpenVisionNativeToolDocument CreateMatching(IDisplayManager displayManager, bool forWorkbench = false)
        {
            MatchingProperty property = OpenVisionToolOpenProfiler.Measure(
                "CreateMatchingProperty",
                () => forWorkbench ? LoadNativeProperty(new MatchingProperty("Matching_1")) : OpenVisionNativeToolPropertySessionStore.GetRepositoryProperty(
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
                OpenVisionNativeToolPreviewExecutor.CaptureMatchingPreview, forWorkbench, () => property.ImageTemplate);
        }

        public static OpenVisionNativeToolDocument CreateEdgeBasedMatching(IDisplayManager displayManager, bool forWorkbench = false)
        {
            EdgeBasedMatchingProperty property = OpenVisionToolOpenProfiler.Measure(
                "CreateEdgeBasedMatchingProperty",
                () => forWorkbench ? LoadNativeProperty(new EdgeBasedMatchingProperty("EdgeBasedMatching_1")) : OpenVisionNativeToolPropertySessionStore.GetRepositoryProperty(
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
                OpenVisionNativeToolPreviewExecutor.CaptureEdgeBasedMatchingPreview, forWorkbench, () => property.ImageTemplate);
        }

        public static OpenVisionNativeToolDocument CreateFeatureMatching(IDisplayManager displayManager, bool forWorkbench = false)
        {
            FeatureMatchingProperty property = OpenVisionToolOpenProfiler.Measure(
                "CreateFeatureMatchingProperty",
                () => forWorkbench ? LoadNativeProperty(new FeatureMatchingProperty("Feature_1")) : OpenVisionNativeToolPropertySessionStore.GetRepositoryProperty(
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
                OpenVisionNativeToolPreviewExecutor.CaptureFeatureMatchingPreview, forWorkbench, () => property.ImageTemplate);
        }

        private static OpenVisionNativeToolDocument CreatePropertyGridDocument<TView, TProperty>(
            IDisplayManager displayManager,
            string toolName,
            string defaultOutputLayer,
            TProperty property,
            Func<TProperty, IPropertyGridToolViewModel<TProperty>> createViewModel,
            Func<VisionToolPropertyGridPresenter<TProperty>, TView> createView,
            Func<Mat, TView, VisionToolResult> executePreview,
            Func<TProperty, string> createDisplaySummary = null,
            Func<TView, OpenVisionNativePreviewOperation> capturePreview = null,
            bool forWorkbench = false)
            where TView : FrameworkElement, ISingleInputVisionToolWpfView
            where TProperty : OpenCvPropertyBase
        {
            IPropertyGridToolViewModel<TProperty> viewModel = OpenVisionToolOpenProfiler.Measure("CreatePropertyGridViewModel", () => createViewModel(property));
            VisionToolPropertyGridPresenter<TProperty> activePresenter = null;
            OpenVisionNativeToolDocument document = null;
            document = OpenVisionToolOpenProfiler.Measure(
                "CreatePropertyGridDocument",
                () => OpenVisionNativePropertyGridToolDocumentBuilder.Create(
                displayManager,
                new OpenVisionNativePropertyGridToolDescriptor<TView, TProperty>(
                    toolName,
                    defaultOutputLayer,
                    property,
                    viewModel.CreateProperty,
                    () => createDisplaySummary == null ? viewModel.Summary : createDisplaySummary(property),
                    presenter => { activePresenter = presenter; return createView(presenter); },
                    executePreview,
                    persistSelectedObject: () => document?.RecordParameterChange(() => OpenVisionNativeToolPropertySessionStore.Save(toolName, property)),
                    capturePreview: capturePreview)));
            ConfigureParameterState(document, property, forWorkbench, () => activePresenter.NotifyParametersRestored());
            return ApplyLoadFailureStatus(document, property.NAME);
        }

        private static OpenVisionNativeToolDocument CreateTemplatePropertyGridDocument<TView, TProperty>(
            IDisplayManager displayManager,
            string toolName,
            string defaultOutputLayer,
            TProperty property,
            Func<TProperty, ITemplateBackedPropertyGridToolViewModel<TProperty>> createViewModel,
            Func<VisionToolPropertyGridPresenter<TProperty>, TView> createView,
            Func<TView, OpenVisionNativePreviewOperation> capturePreview,
            bool forWorkbench, Func<Mat> getTemplateImage)
            where TView : FrameworkElement, ISingleInputVisionToolWpfView
            where TProperty : OpenCvPropertyBase
        {
            ITemplateBackedPropertyGridToolViewModel<TProperty> viewModel = OpenVisionToolOpenProfiler.Measure("CreatePropertyGridViewModel", () => createViewModel(property));
            VisionToolPropertyGridPresenter<TProperty> activePresenter = null;
            OpenVisionNativeToolDocument document = null;
            document = OpenVisionToolOpenProfiler.Measure(
                "CreatePropertyGridDocument",
                () => OpenVisionNativePropertyGridToolDocumentBuilder.Create(
                displayManager,
                new OpenVisionNativePropertyGridToolDescriptor<TView, TProperty>(
                    toolName,
                    defaultOutputLayer,
                    property,
                    viewModel.CreateProperty,
                    () => viewModel.Summary,
                    presenter => { activePresenter = presenter; return createView(presenter); },
                    null,
                    getTemplateStatus: () => viewModel.TemplateStatus,
                    applyTemplatePathForTest: viewModel.ApplyTemplatePathForTest,
                    reloadTemplateIfPatternChanged: viewModel.ReloadTemplateIfPatternChanged,
                    persistSelectedObject: () => document?.RecordParameterChange(() => OpenVisionNativeToolPropertySessionStore.Save(toolName, property)),
                    capturePreview: capturePreview)));
            ConfigureParameterState(document, property, forWorkbench, () => activePresenter.NotifyParametersRestored(), viewModel);
            Mat thumbnailSource = null;
            System.Windows.Media.ImageSource thumbnail = null;
            document.ConfigureTemplateState(() => viewModel.TemplateStatus, () =>
            {
                Mat image = getTemplateImage();
                if (ReferenceEquals(image, thumbnailSource)) { return thumbnail; }
                thumbnailSource = image;
                if (image == null || image.Empty()) { thumbnail = null; }
                else
                {
                    using System.Drawing.Bitmap bitmap = OpenVisionLab.Common.BitmapImageConverter.ToBitmap(image);
                    thumbnail = OpenVisionBitmapImagePreviewFactory.Create(bitmap);
                }
                return thumbnail;
            }, path =>
            {
                if (!viewModel.ApplyTemplatePath(path)) { return false; }
                activePresenter.NotifyParametersRestored();
                return true;
            });
            return ApplyLoadFailureStatus(document, property.NAME);
        }

        private static void ConfigureParameterState<TProperty>(OpenVisionNativeToolDocument document, TProperty property, bool forWorkbench, Action refreshPresentation,
            ITemplateBackedPropertyGridToolViewModel<TProperty> template = null) where TProperty : OpenCvPropertyBase
        {
            TProperty defaultProperty = (TProperty)Activator.CreateInstance(typeof(TProperty), property.NAME);
            byte[] defaults;
            try { defaults = VisionPipelineStepPropertyMapper.CaptureNativeParameters(defaultProperty); }
            finally { VisionPipelineStepPropertyMapper.ReleaseNativeParameterResources(defaultProperty); }
            void Refresh()
            {
                template?.ReloadTemplateIfPatternChanged("PATTERN_PATH");
                refreshPresentation();
            }
            document.ConfigureParameterState(forWorkbench,
                () => VisionPipelineStepPropertyMapper.CaptureNativeParameters(property),
                snapshot => { VisionPipelineStepPropertyMapper.RestoreNativeParameters((byte[])snapshot, property); Refresh(); },
                context =>
                {
                    VisionPipelineStepPropertyMapper.RestoreNativeParameters(defaults, property);
                    TProperty loaded = OpenVisionNativeToolPropertySessionStore.LoadExisting(property.NAME, property, context.Name);
                    try { return VisionPipelineStepPropertyMapper.CaptureNativeParameters(loaded); }
                    finally { if (!ReferenceEquals(loaded, property)) { VisionPipelineStepPropertyMapper.ReleaseNativeParameterResources(loaded); } }
                },
                step =>
                {
                    object source = VisionPipelineStepPropertyMapper.CreateProperty(step);
                    try { VisionPipelineStepPropertyMapper.ApplyNativeParameters(source, property); Refresh(); }
                    finally { VisionPipelineStepPropertyMapper.ReleaseNativeParameterResources(source); }
                },
                () => template == null || template.TemplateStatus.IsReady,
                createDefaults: () => (byte[])defaults.Clone(),
                autoPreviewOptedIn: property is MatchingProperty matching ? () => matching.AUTO_PREVIEW : null);
        }

        private static TProperty LoadNativeProperty<TProperty>(TProperty defaults) where TProperty : OpenCvPropertyBase
        {
            TProperty loaded = OpenVisionNativeToolPropertySessionStore.LoadExisting(defaults.NAME, defaults);
            if (!ReferenceEquals(loaded, defaults)) { VisionPipelineStepPropertyMapper.ReleaseNativeParameterResources(defaults); }
            return loaded;
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
