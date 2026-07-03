using Lib.OpenCV;
using Lib.OpenCV.Result;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace OpenVisionLab
{
    internal readonly struct SimplePreprocessResultReview
    {
        public SimplePreprocessResultReview(
            string summary,
            bool isSuccess,
            IEnumerable<VisionToolResultReviewItem> items,
            string guidance)
        {
            Summary = summary ?? string.Empty;
            IsSuccess = isSuccess;
            Items = items?.ToArray() ?? Array.Empty<VisionToolResultReviewItem>();
            Guidance = guidance ?? string.Empty;
        }

        public string Summary { get; }
        public bool IsSuccess { get; }
        public IReadOnlyList<VisionToolResultReviewItem> Items { get; }
        public string Guidance { get; }
    }

    internal static class SimplePreprocessResultExplanation
    {
        public static SimplePreprocessResultReview CreateMean(
            IEnumerable<MeanResult> results,
            MeanType meanType,
            int minimum,
            int maximum)
        {
            int min = Math.Min(minimum, maximum);
            int max = Math.Max(minimum, maximum);
            List<MeanResult> meanResults = results?.Where(item => item != null).ToList() ?? new List<MeanResult>();
            bool hasResult = meanResults.Count > 0;
            double minMean = hasResult ? meanResults.Min(item => item.meanValue) : 0D;
            double maxMean = hasResult ? meanResults.Max(item => item.meanValue) : 0D;
            double avgMean = hasResult ? meanResults.Average(item => item.meanValue) : 0D;
            bool inRange = hasResult && minMean >= min && maxMean <= max;
            string rangeText = FormatRange(min, max);
            string decision = inRange ? VisionToolVerificationText.PreviewOk : VisionToolVerificationText.PreviewNg;
            string summary = string.Format(
                CultureInfo.CurrentCulture,
                T("VisionTool.Review.MeanSummaryFormat", "Mean / Avg {0:0.0} / Range {1} / Count {2}"),
                avgMean,
                rangeText,
                meanResults.Count);

            List<VisionToolResultReviewItem> items = new List<VisionToolResultReviewItem>
            {
                VisionToolResultReviewPresenter.Item(VisionToolVerificationText.DecisionLabel, decision),
                VisionToolResultReviewPresenter.Item(Label("Mean", "Mean"), string.Format(CultureInfo.CurrentCulture, "{0:0.0}", avgMean)),
                VisionToolResultReviewPresenter.Item(Label("Range", "Range"), rangeText),
                VisionToolResultReviewPresenter.Item(VisionToolVerificationText.CountLabel, meanResults.Count),
                VisionToolResultReviewPresenter.Item(Label("Type", "Type"), meanType)
            };

            string reason = inRange
                ? string.Format(
                    CultureInfo.CurrentCulture,
                    T("VisionTool.SimplePreprocess.MeanPassReasonFormat", "Mean {0:0.0} is inside the configured range {1}."),
                    avgMean,
                    rangeText)
                : string.Format(
                    CultureInfo.CurrentCulture,
                    T("VisionTool.SimplePreprocess.MeanFailReasonFormat", "Mean result is missing or outside the configured range {0}. Cause candidates: ROI, brightness drift, or range limits."),
                    rangeText);
            string nextAction = inRange
                ? T("VisionTool.SimplePreprocess.MeanPassNext", "Compare the value on Good/Bad samples, then add to pipeline")
                : T("VisionTool.SimplePreprocess.MeanFailNext", "Adjust ROI or mean range, then run preview again");

            return new SimplePreprocessResultReview(
                summary,
                inRange,
                items,
                VisionToolVerificationText.FormatResultGuidance(decision, rangeText, reason, nextAction));
        }

        public static SimplePreprocessResultReview CreateHsv(
            Mat mask,
            int hueMin,
            int hueMax,
            int saturationMin,
            int saturationMax,
            int valueMin,
            int valueMax)
        {
            double selectedPixels = mask == null || mask.Empty() ? 0D : Cv2.CountNonZero(mask);
            double totalPixels = mask == null || mask.Empty() ? 0D : mask.Rows * mask.Cols;
            double selectedPercent = totalPixels <= 0D ? 0D : selectedPixels * 100D / totalPixels;
            string criteria = string.Format(
                CultureInfo.CurrentCulture,
                "H {0}-{1} / S {2}-{3} / V {4}-{5}",
                hueMin,
                hueMax,
                saturationMin,
                saturationMax,
                valueMin,
                valueMax);
            string decision = VisionToolVerificationText.PreviewOk;
            string summary = string.Format(
                CultureInfo.CurrentCulture,
                T("VisionTool.Review.HsvSummaryFormat", "HSV / Selected {0:0.0}% / {1}"),
                selectedPercent,
                criteria);

            List<VisionToolResultReviewItem> items = new List<VisionToolResultReviewItem>
            {
                VisionToolResultReviewPresenter.Item(VisionToolVerificationText.DecisionLabel, decision),
                VisionToolResultReviewPresenter.Item(Label("Pixels", "Pixels"), string.Format(CultureInfo.CurrentCulture, "{0:0.0}%", selectedPercent)),
                VisionToolResultReviewPresenter.Item(Label("Range", "Range"), criteria)
            };
            string reason = string.Format(
                CultureInfo.CurrentCulture,
                T("VisionTool.SimplePreprocess.HsvReasonFormat", "The current HSV range selected {0:0.0}% of pixels."),
                selectedPercent);
            string nextAction = T(
                "VisionTool.SimplePreprocess.HsvNext",
                "If background remains, narrow H/S/V ranges and compare the mask on Good/Bad samples");

            return new SimplePreprocessResultReview(
                summary,
                true,
                items,
                VisionToolVerificationText.FormatResultGuidance(decision, criteria, reason, nextAction));
        }

        public static SimplePreprocessResultReview CreateHistogram(
            Mat source,
            Mat result,
            HistogramPreviewType histogramType,
            string criteria)
        {
            (double InputMean, double InputContrast) inputStats = CalculateImageStats(source);
            (double OutputMean, double OutputContrast) outputStats = CalculateImageStats(result);
            string decision = VisionToolVerificationText.PreviewOk;
            string summary = string.Format(
                CultureInfo.CurrentCulture,
                T("VisionTool.Review.HistogramSummaryFormat", "Histogram / {0} / Mean {1:0.0}->{2:0.0} / Contrast {3:0.0}->{4:0.0}"),
                histogramType,
                inputStats.InputMean,
                outputStats.OutputMean,
                inputStats.InputContrast,
                outputStats.OutputContrast);

            List<VisionToolResultReviewItem> items = new List<VisionToolResultReviewItem>
            {
                VisionToolResultReviewPresenter.Item(VisionToolVerificationText.DecisionLabel, decision),
                VisionToolResultReviewPresenter.Item(Label("Type", "Type"), histogramType),
                VisionToolResultReviewPresenter.Item(Label("Input", "Input"), FormatMeanContrast(inputStats.InputMean, inputStats.InputContrast)),
                VisionToolResultReviewPresenter.Item(Label("Output", "Output"), FormatMeanContrast(outputStats.OutputMean, outputStats.OutputContrast))
            };
            string reason = string.Format(
                CultureInfo.CurrentCulture,
                T("VisionTool.SimplePreprocess.HistogramReasonFormat", "{0} changed mean/contrast from {1:0.0}/{2:0.0} to {3:0.0}/{4:0.0}."),
                histogramType,
                inputStats.InputMean,
                inputStats.InputContrast,
                outputStats.OutputMean,
                outputStats.OutputContrast);
            string nextAction = T(
                "VisionTool.SimplePreprocess.HistogramNext",
                "Compare output brightness and noise on Good/Bad samples before connecting the next tool");

            return new SimplePreprocessResultReview(
                summary,
                true,
                items,
                VisionToolVerificationText.FormatResultGuidance(decision, criteria, reason, nextAction));
        }

        private static (double Mean, double Contrast) CalculateImageStats(Mat image)
        {
            if (image == null || image.Empty())
            {
                return (0D, 0D);
            }

            Cv2.MeanStdDev(image, out Scalar mean, out Scalar standardDeviation);
            int channels = Math.Max(1, Math.Min(image.Channels(), 4));
            return (AverageScalar(mean, channels), AverageScalar(standardDeviation, channels));
        }

        private static double AverageScalar(Scalar scalar, int channels)
        {
            double sum = 0D;
            for (int index = 0; index < channels; index++)
            {
                sum += scalar[index];
            }

            return sum / Math.Max(1, channels);
        }

        private static string FormatMeanContrast(double mean, double contrast)
        {
            return string.Format(CultureInfo.CurrentCulture, "{0:0.0} / {1:0.0}", mean, contrast);
        }

        private static string FormatRange(int minimum, int maximum)
        {
            return string.Format(CultureInfo.CurrentCulture, "{0}-{1}", minimum, maximum);
        }

        private static string Label(string keySuffix, string fallbackText)
        {
            return T("VisionTool.Review.Label." + keySuffix, fallbackText);
        }

        private static string T(string key, string fallbackText)
        {
            return VisionToolVerificationText.T(key, fallbackText);
        }
    }
}
