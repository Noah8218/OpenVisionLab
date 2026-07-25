using Lib.Common;
using Lib.OpenCV;
using Lib.OpenCV.Blob;
using Lib.OpenCV.Property;
using Lib.OpenCV.Result;
using Lib.OpenCV.Tool;
using OpenCvSharp;
using OpenVisionLab.Contracts;
using OpenVisionLab.Vision._1._Tools.OpenCV;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace OpenVisionLab
{
    internal static class OpenVisionNativeToolPreviewExecutor
    {
        // Owns OpenCV preview execution; visual overlay drawing is isolated in OpenVisionNativeToolPreviewOverlayRenderer.
        public static VisionToolResult ExecuteBlobPreview(Mat source, BlobToolWpfView view)
        {
            BlobProperty property = view.CreateProperty();
            if (view.ConsumeThresholdTeachingPreviewRequest())
            {
                Mat visual = OpenVisionNativeToolPreviewOverlayRenderer.CreateThresholdTeachingPreviewImage(source, property);
                return VisionToolResult.Passed(visual, TimeSpan.Zero);
            }

            BlobTool tool = new BlobTool();
            tool.SetProperty(property);
            VisionToolResult result = tool.Execute(source);
            VisionPipelineObjectResultCaptureService.ApplyNativeFilter(property, tool, result);
            view.SetResultReview(tool.results);
            if (result?.Success == true)
            {
                // Blob teaching needs to see the inspected binary image, not the untouched input returned by some legacy tool paths.
                Mat visual = OpenVisionNativeToolPreviewOverlayRenderer.CreateBlobTeachingPreviewImage(source, property, tool.results);
                if (visual != null && !visual.Empty())
                {
                    result.ResultImage?.Dispose();
                    result.ResultImage = visual;
                }
            }

            return result;
        }

        public static VisionToolResult ExecuteContourPreview(Mat source, ContourToolWpfView view)
        {
            ContourProperty property = view.CreateProperty();
            if (view.ConsumeThresholdTeachingPreviewRequest())
            {
                Mat visual = OpenVisionNativeToolPreviewOverlayRenderer.CreateThresholdTeachingPreviewImage(source, property);
                return VisionToolResult.Passed(visual, TimeSpan.Zero);
            }

            ContourTool tool = new ContourTool();
            tool.SetProperty(property);
            VisionToolResult result = tool.Execute(source);
            VisionPipelineObjectResultCaptureService.ApplyNativeFilter(property, tool, result);
            view.SetResultReview(tool.results);

            if (result?.Success == true)
            {
                Mat visual = OpenVisionNativeToolPreviewOverlayRenderer.CreateContourTeachingPreviewImage(source, property, tool.results);
                if (visual != null && !visual.Empty())
                {
                    result.ResultImage?.Dispose();
                    result.ResultImage = visual;
                }
            }

            return result;
        }

        public static VisionToolResult ExecuteAffineTransformPreview(Mat source, AffineTransformToolWpfView view)
        {
            AffineTransformTool tool = new AffineTransformTool();
            tool.SetProperty(view.CreateProperty());
            VisionToolResult result = tool.Execute(source);
            view.SetResultReview(result);
            if (result?.ResultImage != null && !result.ResultImage.Empty())
            {
                Mat visual = OpenVisionNativeToolPreviewOverlayRenderer.CreateAffineTransformPreviewImage(
                    result.ResultImage,
                    result.Overlays);
                result.ResultImage.Dispose();
                result.ResultImage = visual;
            }

            return result;
        }

        public static VisionToolResult ExecuteLineGaugePreview(Mat source, LineToolWpfView view)
        {
            view.EnsureDefaultRoi(source.Width, source.Height);
            LineGaugeProperty selectedProperty = view.CreateSelectedLineProperty();
            if (view.ConsumeThresholdTeachingPreviewRequest() && HasInternalThreshold(selectedProperty))
            {
                Mat visual = OpenVisionNativeToolPreviewOverlayRenderer.CreateThresholdTeachingPreviewImage(source, selectedProperty);
                return VisionToolResult.Passed(visual, TimeSpan.Zero);
            }

            if (string.Equals(view.SelectedPurpose, nameof(LineToolPurpose.Measure), StringComparison.Ordinal))
            {
                return ExecuteLineDistancePreview(source, view);
            }

            if (string.Equals(view.SelectedPurpose, nameof(LineToolPurpose.Intersection), StringComparison.Ordinal))
            {
                return ExecuteLineIntersectionPreview(source, view);
            }

            LineGaugeTool tool = new LineGaugeTool();
            tool.SetProperty(selectedProperty);
            VisionToolResult result = tool.Execute(source);
            view.SetResultReview(tool.resultList);

            if (result?.Success == true)
            {
                Mat drawn = OpenVisionNativeToolPreviewOverlayRenderer.CreateLineGaugePreviewImage(source, tool);
                result.ResultImage?.Dispose();
                result.ResultImage = drawn;
            }

            return result;
        }

        private static bool HasInternalThreshold(LineGaugeProperty property)
        {
            return property != null && (property.USE_THRESHOLD || property.USE_ADAPTIVE_THRESHOLD);
        }

        public static VisionToolResult ExecuteMatchingPreview(Mat source, MatchingToolWpfView view)
        {
            MatchingProperty property = view.CreateProperty();
            MatchingTool tool = new MatchingTool();
            tool.SetProperty(property);
            if (!OpenCvHelper.IsImageEmpty(property.ImageTemplate))
            {
                tool.SetTemplateImage(property.ImageTemplate);
            }

            return ExecuteMatchingPreviewCore(source, tool, () => tool.results, view.SetResultReview);
        }

        public static VisionToolResult ExecuteEdgeBasedMatchingPreview(Mat source, EdgeBasedMatchingToolWpfView view)
        {
            EdgeBasedMatchingProperty property = view.CreateProperty();
            EdgeBasedTemplateMatchingTool tool = new EdgeBasedTemplateMatchingTool();
            tool.SetProperty(property);
            if (!OpenCvHelper.IsImageEmpty(property.ImageTemplate))
            {
                tool.SetTemplateImage(property.ImageTemplate);
            }

            return ExecuteMatchingPreviewCore(
                source,
                tool,
                () => tool.results,
                view.SetResultReview,
                // EdgeBasedMatching draws the taught edge model outline in Lib.Noah; do not add a box overlay on top.
                drawResultBoxes: false);
        }

        public static VisionToolResult ExecuteFeatureMatchingPreview(Mat source, FeatureMatchingToolWpfView view)
        {
            FeatureMatchingProperty property = view.CreateProperty();
            SiftTool tool = new SiftTool();
            tool.SetProperty(property);
            if (!OpenCvHelper.IsImageEmpty(property.ImageTemplate))
            {
                tool.SetTemplateImage(property.ImageTemplate);
            }

            return ExecuteMatchingPreviewCore(source, tool, () => tool.results, view.SetResultReview);
        }

        private static VisionToolResult ExecuteMatchingPreviewCore(
            Mat source,
            IVisionTool tool,
            Func<IEnumerable<MatchingResult>> getResults,
            Action<IEnumerable<MatchingResult>, TimeSpan?> setResultReview,
            bool drawResultBoxes = true)
        {
            // Matching variants use different tool/property types; keep setup visible and share only execution/review/overlay.
            Stopwatch stopwatch = Stopwatch.StartNew();
            VisionToolResult result = tool.Execute(source);
            stopwatch.Stop();
            TimeSpan elapsed = result != null && result.Elapsed > TimeSpan.Zero
                ? result.Elapsed
                : stopwatch.Elapsed;
            IEnumerable<MatchingResult> results = getResults() ?? Array.Empty<MatchingResult>();
            setResultReview(results, elapsed);
            if (result?.Success == true)
            {
                Mat drawn = OpenVisionNativeToolPreviewOverlayRenderer.CreateMatchingOverlayImage(
                    result.ResultImage,
                    source,
                    results,
                    drawResultBoxes);
                if (!ReferenceEquals(drawn, result.ResultImage))
                {
                    result.ResultImage?.Dispose();
                }

                result.ResultImage = drawn;
            }

            return result;
        }

        private static VisionToolResult ExecuteLineDistancePreview(Mat source, LineToolWpfView view)
        {
            VisionPipelineLineDistanceTool tool = new VisionPipelineLineDistanceTool(
                "LineDistance_Preview",
                view.CreateLineAProperty(),
                view.CreateLineBProperty());
            VisionToolResult result = tool.Execute(source);
            view.SetDistanceResultReview(result);
            return result;
        }

        private static VisionToolResult ExecuteLineIntersectionPreview(Mat source, LineToolWpfView view)
        {
            VisionPipelineLineIntersectionTool tool = new VisionPipelineLineIntersectionTool(
                "LineIntersection_Preview",
                view.CreateLineAProperty(),
                view.CreateLineBProperty());
            VisionToolResult result = tool.Execute(source);
            view.SetIntersectionResultReview(result);
            return result;
        }
    }
}
