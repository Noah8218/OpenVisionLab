using OpenVisionLab.Core;
using OpenVisionLab.Vision2D;
using OpenVisionLab.Vision2D.Blob;
using OpenVisionLab.Vision2D.Property;
using OpenVisionLab.Vision2D.Result;
using OpenVisionLab.Vision2D.Tool;
using OpenCvSharp;
using OpenVisionLab.Contracts;
using OpenVisionLab.Vision._1._Tools.OpenCV;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace OpenVisionLab
{
    internal static class OpenVisionNativeToolPreviewExecutor
    {
        // Owns OpenCV preview execution; visual overlay drawing is isolated in OpenVisionNativeToolPreviewOverlayRenderer.
        public static OpenVisionNativePreviewOperation CaptureBlobPreview(BlobToolWpfView view)
        {
            view.Dispatcher.VerifyAccess();
            BlobProperty property = view.CreateProperty();
            property.CvROIS = property.CvROIS?.ToList();
            property.CvMASKS = property.CvMASKS?.ToList();
            bool teachingPreview = view.ConsumeThresholdTeachingPreviewRequest();
            BlobResult[] review = null;
            return new OpenVisionNativePreviewOperation(
                source => ExecuteBlobPreview(source, property, teachingPreview, out review),
                () => { if (review != null) view.SetResultReview(review, property); }, isTeachingPreview: teachingPreview);
        }

        private static VisionToolResult ExecuteBlobPreview(Mat source, BlobProperty property, bool teachingPreview, out BlobResult[] review)
        {
            review = null;
            if (teachingPreview)
            {
                Mat visual = OpenVisionNativeToolPreviewOverlayRenderer.CreateThresholdTeachingPreviewImage(source, property);
                return VisionToolResult.Passed(visual, TimeSpan.Zero);
            }

            using BlobTool tool = new BlobTool();
            tool.SetProperty(property);
            VisionToolResult result = tool.Execute(source);
            try
            {
                VisionPipelineObjectResultCaptureService.ApplyNativeFilter(property, tool, result);
                // Retain only managed review values; native tool/result lifetime ends on the worker.
                review = tool.results?.Where(item => item != null).Select(item => new BlobResult
                {
                    Area = item.Area,
                    Center = item.Center,
                    Bounding = item.Bounding
                }).ToArray() ?? Array.Empty<BlobResult>();
                if (result?.Success == true)
                {
                    Mat visual = OpenVisionNativeToolPreviewOverlayRenderer.CreateBlobResultPreviewImage(source, tool.results);
                    if (visual != null && !visual.Empty())
                    {
                        result.ResultImage?.Dispose();
                        result.ResultImage = visual;
                    }
                    else
                    {
                        visual?.Dispose();
                    }
                }
                return result;
            }
            catch
            {
                result?.Dispose();
                throw;
            }
        }

        public static OpenVisionNativePreviewOperation CaptureContourPreview(ContourToolWpfView view)
        {
            view.Dispatcher.VerifyAccess();
            ContourProperty property = view.CreateProperty();
            property.CvROIS = property.CvROIS?.ToList();
            property.CvMASKS = property.CvMASKS?.ToList();
            bool teachingPreview = view.ConsumeThresholdTeachingPreviewRequest();
            ContourResult[] review = null;
            return new OpenVisionNativePreviewOperation(
                source => ExecuteContourPreview(source, property, teachingPreview, out review),
                () => { if (review != null) view.SetResultReview(review, property); }, isTeachingPreview: teachingPreview);
        }

        private static VisionToolResult ExecuteContourPreview(Mat source, ContourProperty property, bool teachingPreview, out ContourResult[] review)
        {
            review = null;
            if (teachingPreview)
            {
                Mat visual = OpenVisionNativeToolPreviewOverlayRenderer.CreateThresholdTeachingPreviewImage(source, property);
                return VisionToolResult.Passed(visual, TimeSpan.Zero);
            }

            using ContourTool tool = new ContourTool();
            tool.SetProperty(property);
            VisionToolResult result = tool.Execute(source);
            try
            {
                VisionPipelineObjectResultCaptureService.ApplyNativeFilter(property, tool, result);
                review = tool.results?.Where(item => item != null).Select(item => new ContourResult(
                    item.Index,
                    item.Area,
                    item.Center,
                    new OpenCvSharp.Rect(item.Bounding.X, item.Bounding.Y, item.Bounding.Width, item.Bounding.Height),
                    Array.Empty<OpenCvSharp.Point>(),
                    item.Angle)).ToArray() ?? Array.Empty<ContourResult>();
                if (result?.Success == true)
                {
                    Mat visual = OpenVisionNativeToolPreviewOverlayRenderer.CreateContourResultPreviewImage(source, property, tool.results);
                    if (visual != null && !visual.Empty())
                    {
                        result.ResultImage?.Dispose();
                        result.ResultImage = visual;
                    }
                    else
                    {
                        visual?.Dispose();
                    }
                }
                return result;
            }
            catch
            {
                result?.Dispose();
                throw;
            }
        }

        public static OpenVisionNativePreviewOperation CaptureAffineTransformPreview(AffineTransformToolWpfView view)
        {
            view.Dispatcher.VerifyAccess();
            AffineTransformProperty property = SnapshotRegions(view.CreateProperty());
            VisionToolResult review = null;
            return new OpenVisionNativePreviewOperation(
                source =>
                {
                    VisionToolResult result = ExecuteAffineTransformPreview(source, property);
                    review = CaptureReviewValues(result);
                    return result;
                },
                () => { if (review != null) view.SetResultReview(review); },
                publishEvidenceOnFailure: true,
                release: () => review?.Dispose());
        }

        private static VisionToolResult ExecuteAffineTransformPreview(Mat source, AffineTransformProperty property)
        {
            VisionPipelineOutputAllocationPreflightResult allocationValidation =
                VisionPipelineOutputAllocationGuard.Validate(
                    VisionPipelineStepBuilder.FromAffineTransformProperty(
                        property, "AffineTransformPreview", VisionRecipeRunner.DefaultInputLayer, "Preview"),
                    source);
            if (!allocationValidation.Success)
            {
                return VisionToolResult.Failed(allocationValidation.ErrorCode, allocationValidation.Message, TimeSpan.Zero, null);
            }

            using AffineTransformTool tool = new AffineTransformTool();
            tool.SetProperty(property);
            VisionToolResult result = tool.Execute(source);
            try
            {
                if (result?.ResultImage != null && !result.ResultImage.Empty())
                {
                    Mat visual = OpenVisionNativeToolPreviewOverlayRenderer.CreateAffineTransformPreviewImage(result.ResultImage, result.Overlays);
                    result.ResultImage.Dispose();
                    result.ResultImage = visual;
                }
                return result;
            }
            catch
            {
                result?.Dispose();
                throw;
            }
        }

        public static OpenVisionNativePreviewOperation CaptureLineGaugePreview(LineToolWpfView view, int imageWidth, int imageHeight)
        {
            view.Dispatcher.VerifyAccess();
            view.ClearSignalEvidence();
            view.EnsureDefaultRoi(imageWidth, imageHeight);
            LineGaugeProperty selectedProperty = SnapshotRegions(view.CreateSelectedLineProperty());
            LineGaugeProperty lineA = SnapshotRegions(view.CreateLineAProperty());
            LineGaugeProperty lineB = SnapshotRegions(view.CreateLineBProperty());
            LineToolPurpose purpose = (LineToolPurpose)Enum.Parse(typeof(LineToolPurpose), view.SelectedPurpose);
            string lineName = view.SelectedLineName;
            string inputLayer = view.SelectedInputLayer;
            bool teachingPreview = view.ConsumeThresholdTeachingPreviewRequest() && HasInternalThreshold(selectedProperty);
            Action<IEnumerable<LineGaugeResult>, VisionToolResult> publishReview = view.CaptureResultReview(purpose, lineName, selectedProperty);
            LineGaugeResult[] lines = null;
            VisionToolResult review = null;
            VisionToolSignalEvidence signal = null;
            return new OpenVisionNativePreviewOperation(
                source =>
                {
                    VisionToolResult result = ExecuteLineGaugePreview(
                        source, selectedProperty, lineA, lineB, purpose, lineName, inputLayer, teachingPreview, out lines, out signal);
                    if (!teachingPreview) review = CaptureReviewValues(result);
                    return result;
                },
                () =>
                {
                    if (!teachingPreview && review != null) publishReview(lines, review);
                    if (signal != null) view.ShowSignalEvidence(signal);
                },
                publishEvidenceOnFailure: true,
                release: () => review?.Dispose(), isTeachingPreview: teachingPreview);
        }

        private static VisionToolResult ExecuteLineGaugePreview(
            Mat source, LineGaugeProperty selectedProperty, LineGaugeProperty lineA, LineGaugeProperty lineB,
            LineToolPurpose purpose, string lineName, string inputLayer, bool teachingPreview,
            out LineGaugeResult[] lines, out VisionToolSignalEvidence signal)
        {
            lines = null;
            signal = null;
            if (teachingPreview)
            {
                return VisionToolResult.Passed(
                    OpenVisionNativeToolPreviewOverlayRenderer.CreateThresholdTeachingPreviewImage(source, selectedProperty), TimeSpan.Zero);
            }

            if (purpose == LineToolPurpose.Measure)
            {
                return ExecuteLineDistancePreview(source, selectedProperty, lineA, lineB, lineName, inputLayer, out signal);
            }
            if (purpose == LineToolPurpose.Intersection)
            {
                return new VisionPipelineLineIntersectionTool("LineIntersection_Preview", lineA, lineB).Execute(source);
            }

            using LineGaugeTool tool = new LineGaugeTool();
            tool.SetProperty(selectedProperty);
            VisionToolResult result = tool.Execute(source);
            try
            {
                lines = tool.resultList?.Where(item => item != null).Select(item => new LineGaugeResult(
                    item.Results_List?.Where(edge => edge != null).Select(edge => new LineGaugeEdge(edge.NO, edge.MeasPos, edge.UseEdge)).ToList(),
                    item.FitLine == null ? null : new OpenVisionLab.Core.Geometry2D.LineSegment2D(item.FitLine.Start, item.FitLine.End),
                    item.Index)
                {
                    edgeList = item.edgeList?.ToList()
                }).ToArray() ?? Array.Empty<LineGaugeResult>();
                if (result?.Success == true)
                {
                    Mat drawn = OpenVisionNativeToolPreviewOverlayRenderer.CreateLineGaugePreviewImage(source, tool);
                    result.ResultImage?.Dispose();
                    result.ResultImage = drawn;
                    signal = CreateLineSignalEvidence(source, result, selectedProperty, tool.resultList, lineName, inputLayer);
                }
                return result;
            }
            catch
            {
                result?.Dispose();
                throw;
            }
        }

        private static bool HasInternalThreshold(LineGaugeProperty property)
        {
            return property != null && (property.USE_THRESHOLD || property.USE_ADAPTIVE_THRESHOLD);
        }

        public static OpenVisionNativePreviewOperation CaptureMatchingPreview(MatchingToolWpfView view)
        {
            view.Dispatcher.VerifyAccess();
            MatchingProperty property = SnapshotRegions(view.CreateProperty());
            Action<IEnumerable<MatchingResult>, TimeSpan?> publishReview = view.CaptureResultReview();
            property.ImageTemplate = property.ImageTemplate?.Clone();
            MatchingResult[] review = null;
            TimeSpan elapsed = TimeSpan.Zero;
            return new OpenVisionNativePreviewOperation(
                source =>
                {
                    using MatchingTool tool = new MatchingTool();
                    tool.SetProperty(property);
                    if (!OpenCvHelper.IsImageEmpty(property.ImageTemplate)) tool.SetTemplateImage(property.ImageTemplate);
                    return ExecuteMatchingPreviewCore(source, tool, () => tool.results, out review, out elapsed);
                },
                () => { if (review != null) publishReview(review, elapsed); },
                publishEvidenceOnFailure: true,
                release: () => property.ImageTemplate?.Dispose());
        }

        public static OpenVisionNativePreviewOperation CaptureEdgeBasedMatchingPreview(EdgeBasedMatchingToolWpfView view)
        {
            view.Dispatcher.VerifyAccess();
            EdgeBasedMatchingProperty property = SnapshotRegions(view.CreateProperty());
            Action<IEnumerable<MatchingResult>, TimeSpan?> publishReview = view.CaptureResultReview();
            property.ImageTemplate = property.ImageTemplate?.Clone();
            MatchingResult[] review = null;
            TimeSpan elapsed = TimeSpan.Zero;
            return new OpenVisionNativePreviewOperation(
                source =>
                {
                    using EdgeBasedTemplateMatchingTool tool = new EdgeBasedTemplateMatchingTool();
                    tool.SetProperty(property);
                    if (!OpenCvHelper.IsImageEmpty(property.ImageTemplate)) tool.SetTemplateImage(property.ImageTemplate);
                    // Lib.Noah draws the taught edge model outline; retain its no-extra-box behavior.
                    return ExecuteMatchingPreviewCore(source, tool, () => tool.results, out review, out elapsed, drawResultBoxes: false);
                },
                () => { if (review != null) publishReview(review, elapsed); },
                publishEvidenceOnFailure: true,
                release: () => property.ImageTemplate?.Dispose());
        }

        public static OpenVisionNativePreviewOperation CaptureFeatureMatchingPreview(FeatureMatchingToolWpfView view)
        {
            view.Dispatcher.VerifyAccess();
            FeatureMatchingProperty property = SnapshotRegions(view.CreateProperty());
            Action<IEnumerable<MatchingResult>, TimeSpan?> publishReview = view.CaptureResultReview();
            property.ImageTemplate = property.ImageTemplate?.Clone();
            MatchingResult[] review = null;
            TimeSpan elapsed = TimeSpan.Zero;
            return new OpenVisionNativePreviewOperation(
                source =>
                {
                    using SiftTool tool = new SiftTool();
                    tool.SetProperty(property);
                    if (!OpenCvHelper.IsImageEmpty(property.ImageTemplate)) tool.SetTemplateImage(property.ImageTemplate);
                    return ExecuteMatchingPreviewCore(source, tool, () => tool.results, out review, out elapsed);
                },
                () => { if (review != null) publishReview(review, elapsed); },
                publishEvidenceOnFailure: true,
                release: () => property.ImageTemplate?.Dispose());
        }

        private static VisionToolResult ExecuteMatchingPreviewCore(
            Mat source, IVisionTool tool, Func<IEnumerable<MatchingResult>> getResults,
            out MatchingResult[] review, out TimeSpan elapsed, bool drawResultBoxes = true)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            VisionToolResult result = tool.Execute(source);
            stopwatch.Stop();
            try
            {
                elapsed = result != null && result.Elapsed > TimeSpan.Zero ? result.Elapsed : stopwatch.Elapsed;
                review = (getResults() ?? Array.Empty<MatchingResult>()).Where(item => item != null).Select(item => new MatchingResult(
                    item.Index, item.Score, item.Center,
                    new OpenCvSharp.Rect2f(item.Bounding.X, item.Bounding.Y, item.Bounding.Width, item.Bounding.Height),
                    item.Angle, item.Scale)
                {
                    EdgeScore = item.EdgeScore,
                    ImageScore = item.ImageScore,
                    FinalScore = item.FinalScore,
                    ScoreMargin = item.ScoreMargin,
                    PolarityReversed = item.PolarityReversed
                }).ToArray();
                if (result?.Success == true)
                {
                    Mat drawn = OpenVisionNativeToolPreviewOverlayRenderer.CreateMatchingOverlayImage(result.ResultImage, source, review, drawResultBoxes);
                    if (!ReferenceEquals(drawn, result.ResultImage)) result.ResultImage?.Dispose();
                    result.ResultImage = drawn;
                }
                return result;
            }
            catch
            {
                result?.Dispose();
                throw;
            }
        }

        private static VisionToolResult ExecuteLineDistancePreview(
            Mat source, LineGaugeProperty selectedProperty, LineGaugeProperty lineA, LineGaugeProperty lineB,
            string lineName, string inputLayer, out VisionToolSignalEvidence signal)
        {
            signal = null;
            VisionToolResult result = new VisionPipelineLineDistanceTool("LineDistance_Preview", lineA, lineB).Execute(source);
            try
            {
                if (result?.Success == true)
                {
                    using LineGaugeTool diagnosticTool = new LineGaugeTool();
                    diagnosticTool.SetProperty(selectedProperty);
                    using VisionToolResult diagnosticResult = diagnosticTool.Execute(source);
                    if (diagnosticResult?.Success == true)
                    {
                        signal = CreateLineSignalEvidence(source, result, selectedProperty, diagnosticTool.resultList, lineName, inputLayer);
                    }
                }
                return result;
            }
            catch
            {
                result?.Dispose();
                throw;
            }
        }

        private static VisionToolSignalEvidence CreateLineSignalEvidence(
            Mat source, VisionToolResult result, LineGaugeProperty property,
            IEnumerable<LineGaugeResult> results, string lineName, string inputLayer)
        {
            if (result?.ResultImage == null || result.ResultImage.Empty()
                || !OpenVisionNativeLineSignalEvidenceFactory.TryCreate(source, property, results, lineName, out OpenVisionNativeLineSignalProfile profile, out _))
            {
                return null;
            }

            OpenVisionNativeToolPreviewOverlayRenderer.DrawLineSignalDiagnostic(result.ResultImage, profile);
            return profile.CreateEvidence(source, result.ResultImage, inputLayer);
        }

        private static TProperty SnapshotRegions<TProperty>(TProperty property) where TProperty : OpenCvPropertyBase
        {
            property.CvROIS = property.CvROIS?.ToList();
            property.CvMASKS = property.CvMASKS?.ToList();
            return property;
        }

        private static VisionToolResult CaptureReviewValues(VisionToolResult result)
        {
            if (result == null) return null;
            VisionToolResult review = new VisionToolResult
            {
                Success = result.Success,
                Message = result.Message,
                Elapsed = result.Elapsed,
                ErrorCode = result.ErrorCode,
                ResultStatus = result.ResultStatus
            };
            foreach (KeyValuePair<string, double> metric in result.Metrics) review.Metrics[metric.Key] = metric.Value;
            return review;
        }
    }
}
