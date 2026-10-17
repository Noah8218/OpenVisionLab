using OpenVisionLab.Vision2D;
using OpenVisionLab.Vision2D.Property;
using OpenVisionLab.Vision2D.Pipeline;
using OpenVisionLab.Vision2D.Tool;
using OpenCvSharp;
using System;
using System.Drawing;
using System.Globalization;

namespace OpenVisionLab
{
    internal static class OpenVisionNativeSimplePreprocessPreviewExecutor
    {
        // Preview execution is isolated from document creation so tool behavior can be changed without touching shell wiring.
        public static OpenVisionNativePreviewOperation CaptureEdgeDetectionPreview(SimplePreprocessToolWpfView view)
        {
            view.Dispatcher.VerifyAccess();
            EdgeDetectionToolProperty property = OpenVisionNativeSimplePreprocessPropertyFactory.CreateEdgeDetectionProperty(view);
            return new OpenVisionNativePreviewOperation(source => ExecuteEdgeDetectionPreview(source, property));
        }

        private static VisionToolResult ExecuteEdgeDetectionPreview(Mat source, EdgeDetectionToolProperty property)
        {
            using EdgeDetectionTool tool = new EdgeDetectionTool();
            tool.SetProperty(property);
            return tool.Execute(source);
        }

        public static OpenVisionNativePreviewOperation CaptureRotateScalePreview(SimplePreprocessToolWpfView view)
        {
            view.Dispatcher.VerifyAccess();
            RotateScaleToolProperty property = OpenVisionNativeSimplePreprocessPropertyFactory.CreateRotateScaleProperty(view);
            return new OpenVisionNativePreviewOperation(source => ExecuteRotateScalePreview(source, property));
        }

        private static VisionToolResult ExecuteRotateScalePreview(Mat source, RotateScaleToolProperty property)
        {
            VisionPipelineOutputAllocationPreflightResult allocationValidation =
                VisionPipelineOutputAllocationGuard.Validate(
                    VisionPipelineStepBuilder.FromRotateScaleProperty(
                        property,
                        "RotateScalePreview",
                        VisionRecipeRunner.DefaultInputLayer,
                        "Preview"),
                    source);
            if (!allocationValidation.Success)
            {
                return VisionToolResult.Failed(
                    allocationValidation.ErrorCode,
                    allocationValidation.Message,
                    TimeSpan.Zero,
                    null);
            }

            using RotateScaleTool tool = new RotateScaleTool();
            tool.SetProperty(property);
            return tool.Execute(source);
        }

        public static OpenVisionNativePreviewOperation CaptureMeanPreview(SimplePreprocessToolWpfView view) => CaptureMeanPreview(view, null);

        public static OpenVisionNativePreviewOperation CaptureMeanPreview(SimplePreprocessToolWpfView view, VisionPipelineStep workbenchStep)
        {
            view.Dispatcher.VerifyAccess();
            MeanProperty property = OpenVisionNativeSimplePreprocessPropertyFactory.CreateMeanProperty(view, workbenchStep);
            SimplePreprocessResultReview? review = null;
            return new OpenVisionNativePreviewOperation(
                source => ExecuteMeanPreview(source, property, out review),
                () => { if (review.HasValue) view.ShowResultReview(review.Value); },
                publishEvidenceOnFailure: true);
        }

        private static VisionToolResult ExecuteMeanPreview(Mat source, MeanProperty property, out SimplePreprocessResultReview? review)
        {
            review = null;
            using MeanTool tool = new MeanTool();
            tool.SetProperty(property);
            VisionToolResult result = tool.Execute(source);
            Mat visual = null;
            try
            {
                if (result == null || !result.Success)
                {
                    review = SimplePreprocessResultExplanation.CreateMean(
                        Array.Empty<OpenVisionLab.Vision2D.Result.MeanResult>(), property.MEAN_TYPES, property.MEAN_MIN, property.MEAN_MAX);
                    return result;
                }

                visual = result.ResultImage == null || result.ResultImage.Empty() ? source.Clone() : result.ResultImage.Clone();
                result.ResultImage?.Dispose();
                if (visual.Channels() == 1)
                {
                    using Mat gray = visual;
                    visual = new Mat();
                    Cv2.CvtColor(gray, visual, ColorConversionCodes.GRAY2BGR);
                }
                foreach (OpenVisionLab.Vision2D.Result.MeanResult meanResult in tool.results)
                {
                    Rectangle bounds = meanResult.Bounding;
                    if (bounds.Width <= 0 || bounds.Height <= 0)
                    {
                        continue;
                    }
                    Cv2.Rectangle(visual, new OpenCvSharp.Rect(bounds.X, bounds.Y, bounds.Width, bounds.Height), Scalar.Blue, 1);
                    Cv2.PutText(
                        visual,
                        "Mean " + meanResult.meanValue.ToString("0.0", CultureInfo.InvariantCulture),
                        new OpenCvSharp.Point(Math.Max(0, bounds.X + 8), Math.Max(18, bounds.Y + 18)),
                        HersheyFonts.HersheySimplex,
                        0.45,
                        Scalar.Blue,
                        1);
                }
                result.ResultImage = visual;
                visual = null;
                review = SimplePreprocessResultExplanation.CreateMean(tool.results, property.MEAN_TYPES, property.MEAN_MIN, property.MEAN_MAX);
                return result;
            }
            catch
            {
                visual?.Dispose();
                result?.Dispose();
                throw;
            }
        }

        public static OpenVisionNativePreviewOperation CaptureHsvPreview(SimplePreprocessToolWpfView view)
        {
            view.Dispatcher.VerifyAccess();
            int hueMin = Math.Min(view.Parameters.GetInt("HueMin", 0), view.Parameters.GetInt("HueMax", 179));
            int hueMax = Math.Max(view.Parameters.GetInt("HueMin", 0), view.Parameters.GetInt("HueMax", 179));
            int satMin = Math.Min(view.Parameters.GetInt("SatMin", 0), view.Parameters.GetInt("SatMax", 255));
            int satMax = Math.Max(view.Parameters.GetInt("SatMin", 0), view.Parameters.GetInt("SatMax", 255));
            int valMin = Math.Min(view.Parameters.GetInt("ValMin", 0), view.Parameters.GetInt("ValMax", 255));
            int valMax = Math.Max(view.Parameters.GetInt("ValMin", 0), view.Parameters.GetInt("ValMax", 255));
            SimplePreprocessResultReview? review = null;
            return new OpenVisionNativePreviewOperation(
                source => ExecuteHsvPreview(source, hueMin, hueMax, satMin, satMax, valMin, valMax, out review),
                () => { if (review.HasValue) view.ShowResultReview(review.Value); });
        }

        private static VisionToolResult ExecuteHsvPreview(Mat source, int hueMin, int hueMax, int satMin, int satMax, int valMin, int valMax, out SimplePreprocessResultReview? review)
        {
            review = null;
            using Mat hsv = ConvertToHsv(source);
            using Mat mask = new Mat();
            Mat preview = new Mat();
            try
            {
                Cv2.InRange(hsv, new Scalar(hueMin, satMin, valMin), new Scalar(hueMax, satMax, valMax), mask);
                Cv2.BitwiseAnd(source, source, preview, mask);
                review = SimplePreprocessResultExplanation.CreateHsv(mask, hueMin, hueMax, satMin, satMax, valMin, valMax);
                return VisionToolResult.Passed(preview, TimeSpan.Zero);
            }
            catch
            {
                preview.Dispose();
                throw;
            }
        }

        public static OpenVisionNativePreviewOperation CaptureHistogramPreview(SimplePreprocessToolWpfView view)
        {
            view.Dispatcher.VerifyAccess();
            view.ClearSignalEvidence();
            HistogramPreviewType histogramType = view.Parameters.GetEnum("HistogramType", HistogramPreviewType.clahe);
            int tileSize = Math.Max(1, view.Parameters.GetInt("TilesGridSize", 3));
            double clipLimit = Math.Max(0d, view.Parameters.GetDouble("ClipLimit", 3));
            int alpha = view.Parameters.GetInt("Alpha", 0);
            int beta = view.Parameters.GetInt("Beta", 100);
            string inputLayer = view.SelectedInputLayer;
            string criteria = CreateHistogramCriteria(histogramType, clipLimit, tileSize, alpha, beta);
            SimplePreprocessResultReview? review = null;
            VisionToolSignalEvidence evidence = null;
            return new OpenVisionNativePreviewOperation(
                source => ExecuteHistogramPreview(source, histogramType, clipLimit, tileSize, alpha, beta, inputLayer, criteria, out review, out evidence),
                () =>
                {
                    if (review.HasValue) view.ShowResultReview(review.Value);
                    if (evidence != null) view.ShowSignalEvidence(evidence);
                });
        }

        private static VisionToolResult ExecuteHistogramPreview(
            Mat source, HistogramPreviewType histogramType, double clipLimit, int tileSize, int alpha, int beta,
            string inputLayer, string criteria, out SimplePreprocessResultReview? review, out VisionToolSignalEvidence evidence)
        {
            review = null;
            evidence = null;
            Mat result = source.Clone();
            try
            {
                switch (histogramType)
                {
                    case HistogramPreviewType.clahe:
                        using (CLAHE clahe = Cv2.CreateCLAHE())
                        {
                            clahe.ClipLimit = clipLimit;
                            clahe.TilesGridSize = new OpenCvSharp.Size(tileSize, tileSize);
                            clahe.Apply(result, result);
                        }
                        break;
                    case HistogramPreviewType.equalizeHist:
                        Cv2.EqualizeHist(result, result);
                        break;
                    case HistogramPreviewType.Normalize:
                        Cv2.Normalize(result, result, alpha, beta, NormTypes.MinMax);
                        break;
                }
                review = SimplePreprocessResultExplanation.CreateHistogram(source, result, histogramType, criteria);
                evidence = OpenVisionNativeHistogramSignalEvidenceFactory.Create(source, result, "Histogram/" + histogramType, inputLayer, criteria);
                return VisionToolResult.Passed(result, TimeSpan.Zero);
            }
            catch
            {
                result.Dispose();
                throw;
            }
        }

        private static string CreateHistogramCriteria(HistogramPreviewType histogramType, double clipLimit, int tileSize, int alpha, int beta)
        {
            switch (histogramType)
            {
                case HistogramPreviewType.clahe:
                    return string.Format(
                        CultureInfo.CurrentCulture,
                        "CLAHE / Clip {0:0.###} / Tile {1}",
                        clipLimit,
                        tileSize);
                case HistogramPreviewType.Normalize:
                    return string.Format(
                        CultureInfo.CurrentCulture,
                        "Normalize / Alpha {0} / Beta {1}",
                        alpha,
                        beta);
                default:
                    return "Global equalize";
            }
        }

        private static Mat ConvertToHsv(Mat source)
        {
            Mat hsv = new Mat();
            try
            {
                if (source.Channels() == 1)
                {
                    using Mat rgb = new Mat();
                    Cv2.CvtColor(source, rgb, ColorConversionCodes.GRAY2RGB);
                    Cv2.CvtColor(rgb, hsv, ColorConversionCodes.RGB2HSV);
                    return hsv;
                }
                if (source.Channels() == 4)
                {
                    using Mat rgb = new Mat();
                    Cv2.CvtColor(source, rgb, ColorConversionCodes.RGBA2RGB);
                    Cv2.CvtColor(rgb, hsv, ColorConversionCodes.RGB2HSV);
                    return hsv;
                }
                Cv2.CvtColor(source, hsv, ColorConversionCodes.RGB2HSV);
                return hsv;
            }
            catch
            {
                hsv.Dispose();
                throw;
            }
        }
    }
}
