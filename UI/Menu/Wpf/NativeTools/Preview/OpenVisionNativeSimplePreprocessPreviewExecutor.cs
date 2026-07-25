using Lib.OpenCV;
using Lib.OpenCV.Tool;
using OpenCvSharp;
using System;
using System.Drawing;
using System.Globalization;

namespace OpenVisionLab
{
    internal static class OpenVisionNativeSimplePreprocessPreviewExecutor
    {
        // Preview execution is isolated from document creation so tool behavior can be changed without touching shell wiring.
        public static VisionToolResult ExecuteEdgeDetectionPreview(Mat source, SimplePreprocessToolWpfView view)
        {
            EdgeDetectionTool tool = new EdgeDetectionTool();
            tool.SetProperty(OpenVisionNativeSimplePreprocessPropertyFactory.CreateEdgeDetectionProperty(view));
            return tool.Execute(source);
        }

        public static VisionToolResult ExecuteRotateScalePreview(Mat source, SimplePreprocessToolWpfView view)
        {
            RotateScaleTool tool = new RotateScaleTool();
            tool.SetProperty(OpenVisionNativeSimplePreprocessPropertyFactory.CreateRotateScaleProperty(view));
            return tool.Execute(source);
        }

        public static VisionToolResult ExecuteMeanPreview(Mat source, SimplePreprocessToolWpfView view)
        {
            MeanTool tool = new MeanTool();
            tool.SetProperty(OpenVisionNativeSimplePreprocessPropertyFactory.CreateMeanProperty(view));
            VisionToolResult result = tool.Execute(source);
            if (result == null || !result.Success)
            {
                view.ShowResultReview(SimplePreprocessResultExplanation.CreateMean(
                    Array.Empty<Lib.OpenCV.Result.MeanResult>(),
                    view.Parameters.GetEnum("MeanType", MeanType.Mean),
                    view.Parameters.GetInt("MeanMin", 100),
                    view.Parameters.GetInt("MeanMax", 240)));
                return result;
            }

            Mat visual = result.ResultImage == null || result.ResultImage.Empty()
                ? source.Clone()
                : result.ResultImage.Clone();
            result.ResultImage?.Dispose();

            if (visual.Channels() == 1)
            {
                Mat converted = new Mat();
                Cv2.CvtColor(visual, converted, ColorConversionCodes.GRAY2BGR);
                visual.Dispose();
                visual = converted;
            }

            foreach (Lib.OpenCV.Result.MeanResult meanResult in tool.results)
            {
                Rectangle bounds = meanResult.Bounding;
                if (bounds.Width <= 0 || bounds.Height <= 0)
                {
                    continue;
                }

                Cv2.Rectangle(
                    visual,
                    new OpenCvSharp.Rect(bounds.X, bounds.Y, bounds.Width, bounds.Height),
                    Scalar.Blue,
                    1);
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
            view.ShowResultReview(SimplePreprocessResultExplanation.CreateMean(
                tool.results,
                view.Parameters.GetEnum("MeanType", MeanType.Mean),
                view.Parameters.GetInt("MeanMin", 100),
                view.Parameters.GetInt("MeanMax", 240)));
            return result;
        }

        public static VisionToolResult ExecuteHsvPreview(Mat source, SimplePreprocessToolWpfView view)
        {
            using Mat hsv = ConvertToHsv(source);
            using Mat mask = new Mat();
            Mat preview = new Mat();

            int hueMin = Math.Min(view.Parameters.GetInt("HueMin", 0), view.Parameters.GetInt("HueMax", 179));
            int hueMax = Math.Max(view.Parameters.GetInt("HueMin", 0), view.Parameters.GetInt("HueMax", 179));
            int satMin = Math.Min(view.Parameters.GetInt("SatMin", 0), view.Parameters.GetInt("SatMax", 255));
            int satMax = Math.Max(view.Parameters.GetInt("SatMin", 0), view.Parameters.GetInt("SatMax", 255));
            int valMin = Math.Min(view.Parameters.GetInt("ValMin", 0), view.Parameters.GetInt("ValMax", 255));
            int valMax = Math.Max(view.Parameters.GetInt("ValMin", 0), view.Parameters.GetInt("ValMax", 255));

            Cv2.InRange(
                hsv,
                new Scalar(hueMin, satMin, valMin),
                new Scalar(hueMax, satMax, valMax),
                mask);
            Cv2.BitwiseAnd(source, source, preview, mask);
            view.ShowResultReview(SimplePreprocessResultExplanation.CreateHsv(
                mask,
                hueMin,
                hueMax,
                satMin,
                satMax,
                valMin,
                valMax));
            return VisionToolResult.Passed(preview, TimeSpan.Zero);
        }

        public static VisionToolResult ExecuteHistogramPreview(Mat source, SimplePreprocessToolWpfView view)
        {
            Mat result = source.Clone();
            HistogramPreviewType histogramType = view.Parameters.GetEnum("HistogramType", HistogramPreviewType.clahe);
            switch (histogramType)
            {
                case HistogramPreviewType.clahe:
                    using (CLAHE clahe = Cv2.CreateCLAHE())
                    {
                        int tileSize = Math.Max(1, view.Parameters.GetInt("TilesGridSize", 3));
                        clahe.ClipLimit = Math.Max(0d, view.Parameters.GetDouble("ClipLimit", 3));
                        clahe.TilesGridSize = new OpenCvSharp.Size(tileSize, tileSize);
                        clahe.Apply(result, result);
                    }
                    break;
                case HistogramPreviewType.equalizeHist:
                    Cv2.EqualizeHist(result, result);
                    break;
                case HistogramPreviewType.Normalize:
                    int alpha = view.Parameters.GetInt("Alpha", 0);
                    int beta = view.Parameters.GetInt("Beta", 100);
                    Cv2.Normalize(result, result, alpha, beta, NormTypes.MinMax);
                    break;
            }

            view.ShowResultReview(SimplePreprocessResultExplanation.CreateHistogram(
                source,
                result,
                histogramType,
                CreateHistogramCriteria(view, histogramType)));
            return VisionToolResult.Passed(result, TimeSpan.Zero);
        }

        private static string CreateHistogramCriteria(SimplePreprocessToolWpfView view, HistogramPreviewType histogramType)
        {
            switch (histogramType)
            {
                case HistogramPreviewType.clahe:
                    return string.Format(
                        CultureInfo.CurrentCulture,
                        "CLAHE / Clip {0:0.###} / Tile {1}",
                        view.Parameters.GetDouble("ClipLimit", 3),
                        view.Parameters.GetInt("TilesGridSize", 3));
                case HistogramPreviewType.Normalize:
                    return string.Format(
                        CultureInfo.CurrentCulture,
                        "Normalize / Alpha {0} / Beta {1}",
                        view.Parameters.GetInt("Alpha", 0),
                        view.Parameters.GetInt("Beta", 100));
                default:
                    return "Global equalize";
            }
        }

        private static Mat ConvertToHsv(Mat source)
        {
            Mat hsv = new Mat();
            if (source.Channels() == 1)
            {
                using (Mat rgb = new Mat())
                {
                    Cv2.CvtColor(source, rgb, ColorConversionCodes.GRAY2RGB);
                    Cv2.CvtColor(rgb, hsv, ColorConversionCodes.RGB2HSV);
                }

                return hsv;
            }

            if (source.Channels() == 4)
            {
                using (Mat rgb = new Mat())
                {
                    Cv2.CvtColor(source, rgb, ColorConversionCodes.RGBA2RGB);
                    Cv2.CvtColor(rgb, hsv, ColorConversionCodes.RGB2HSV);
                }

                return hsv;
            }

            Cv2.CvtColor(source, hsv, ColorConversionCodes.RGB2HSV);
            return hsv;
        }
    }
}
