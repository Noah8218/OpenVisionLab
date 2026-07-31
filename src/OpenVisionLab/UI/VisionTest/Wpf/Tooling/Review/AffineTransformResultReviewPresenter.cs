using Lib.OpenCV.Tool;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace OpenVisionLab
{
    internal static class AffineTransformResultReviewPresenter
    {
        public static void Show(
            VisionToolResult result,
            Action<string, bool, IEnumerable<VisionToolResultReviewItem>, string> showResultReview)
        {
            if (showResultReview == null)
            {
                return;
            }

            bool isSuccess = result?.Success == true;
            IReadOnlyDictionary<string, double> metrics = result?.Metrics;
            double validRatio = Metric(metrics, VisionPipelineKnownMetrics.AffineValidPixelRatio);
            string summary = string.Format(
                CultureInfo.CurrentCulture,
                T(
                    "VisionTool.Affine.ResultSummaryFormat",
                    "Affine 2x3 / {0} / valid pixels {1:0.0}%"),
                isSuccess ? VisionToolVerificationText.PreviewOk : VisionToolVerificationText.PreviewNg,
                validRatio * 100D);

            List<VisionToolResultReviewItem> items = new List<VisionToolResultReviewItem>
            {
                VisionToolResultReviewPresenter.Item(
                    T("VisionTool.Affine.MatrixLabel", "Matrix"),
                    FormatMatrix(metrics)),
                VisionToolResultReviewPresenter.Item(
                    T("VisionTool.Affine.ValidPixelLabel", "Valid pixels"),
                    string.Format(CultureInfo.CurrentCulture, "{0:0.0}%", validRatio * 100D)),
                VisionToolResultReviewPresenter.Item(
                    T("VisionTool.Affine.DeterminantLabel", "Determinant"),
                    Metric(metrics, VisionPipelineKnownMetrics.AffineDeterminant).ToString("0.####", CultureInfo.CurrentCulture)),
                VisionToolResultReviewPresenter.Item(
                    T("VisionTool.Affine.SourceAreaLabel", "Source triangle"),
                    Metric(metrics, VisionPipelineKnownMetrics.AffineSourceTriangleArea).ToString("0.### px²", CultureInfo.CurrentCulture)),
                VisionToolResultReviewPresenter.Item(
                    T("VisionTool.Affine.DestinationAreaLabel", "Destination triangle"),
                    Metric(metrics, VisionPipelineKnownMetrics.AffineDestinationTriangleArea).ToString("0.### px²", CultureInfo.CurrentCulture))
            };

            string guidance = isSuccess
                ? T(
                    "VisionTool.Affine.SuccessGuidance",
                    "Review the destination triangle, transformed source frame, matrix, and valid-pixel coverage before adding the Step.")
                : string.Format(
                    CultureInfo.CurrentCulture,
                    T(
                        "VisionTool.Affine.FailureGuidanceFormat",
                        "Adjust the three corresponding points, output size, or validation gates, then Preview again. {0}"),
                    result == null
                        ? string.Empty
                        : string.Format(
                            CultureInfo.CurrentCulture,
                            "{0}: {1}",
                            result.ErrorName,
                            result.Message));

            showResultReview(summary, isSuccess, items, guidance);
        }

        private static string FormatMatrix(IReadOnlyDictionary<string, double> metrics)
        {
            return string.Format(
                CultureInfo.CurrentCulture,
                "[{0:0.###} {1:0.###} {2:0.###}; {3:0.###} {4:0.###} {5:0.###}]",
                Metric(metrics, VisionPipelineKnownMetrics.AffineM11),
                Metric(metrics, VisionPipelineKnownMetrics.AffineM12),
                Metric(metrics, VisionPipelineKnownMetrics.AffineM13),
                Metric(metrics, VisionPipelineKnownMetrics.AffineM21),
                Metric(metrics, VisionPipelineKnownMetrics.AffineM22),
                Metric(metrics, VisionPipelineKnownMetrics.AffineM23));
        }

        private static double Metric(IReadOnlyDictionary<string, double> metrics, string name)
        {
            return metrics != null && metrics.TryGetValue(name, out double value) && double.IsFinite(value)
                ? value
                : 0D;
        }

        private static string T(string key, string fallback)
        {
            string translated = OpenVisionLanguageService.T(key);
            return string.IsNullOrWhiteSpace(translated) || string.Equals(translated, key, StringComparison.Ordinal)
                ? fallback
                : translated;
        }
    }
}
