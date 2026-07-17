using Lib.OpenCV.Pipeline;
using OpenVisionLab.Pipeline.Controls;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;

namespace OpenVisionLab
{
    internal static class OpenVisionPipelineReviewResultPresenter
    {
        public static string ResolvePairActionText(VisionPipelineSampleCatalogItem counterpartSample)
        {
            if (counterpartSample == null)
            {
                return string.Empty;
            }

            string role = IsOkSampleReference(counterpartSample)
                ? LocalText("OK \uae30\uc900", "OK reference")
                : IsNgSampleReference(counterpartSample)
                    ? LocalText("NG \uae30\uc900", "NG reference")
                    : LocalText("\ubc18\ub300 \uae30\uc900", "opposite reference");
            return string.Format(
                CultureInfo.CurrentCulture,
                LocalText("{0} \uc5f4\uae30", "Open {0}"),
                role);
        }

        public static string FormatRunLog(
            VisionPipelineStep step,
            Bitmap inputImage,
            Bitmap outputImage,
            PipelineFlowPreviewMode mode,
            string statusText,
            string validationStatus,
            VisionPipelineStepResultSummary summary)
        {
            List<string> lines = new List<string>
            {
                TF("PipelineReview.RunLog.ReviewStateFormat", "Review state: {0}", SafeText(statusText, "WAIT")),
                TF("PipelineReview.RunLog.ValidationFormat", "Validation: {0}", SafeText(validationStatus, "NOT RUN")),
                TF("PipelineReview.RunLog.ResultFormat", "Result: {0}", FormatResultSummary(summary)),
                TF("PipelineReview.RunLog.PreviewModeFormat", "Preview mode: {0}", mode),
                TF("PipelineReview.RunLog.InputImageFormat", "Input image: {0}", FormatImageState(step?.InputLayer, inputImage)),
                TF("PipelineReview.RunLog.OutputImageFormat", "Output image: {0}", FormatImageState(step?.OutputLayer, outputImage))
            };

            return string.Join(Environment.NewLine, lines);
        }

        public static string FormatResultSummary(VisionPipelineStepResultSummary summary)
        {
            if (summary == null)
            {
                return T("PipelineReview.RunRequired", "Run review required");
            }

            string status = SafeText(summary.Status, summary.Success ? "OK" : "NG");
            if (summary.ElapsedMilliseconds > 0)
            {
                status += string.Format(CultureInfo.CurrentCulture, " / {0:0.0} ms", summary.ElapsedMilliseconds);
            }

            if (summary.ErrorCode > 0)
            {
                return string.Format(CultureInfo.CurrentCulture, "{0} / Error {1}:{2}", status, summary.ErrorCode, summary.ErrorName);
            }

            return status;
        }

        public static string FormatResultDetails(VisionPipelineStep step, VisionPipelineStepResultSummary summary)
        {
            if (summary == null)
            {
                return T("PipelineReview.NoRunResultForStep", "No run result for selected step.");
            }

            List<string> parts = new List<string>();
            string fixtureMetricText = FormatFixtureMetricText(summary.Metrics);
            bool hasFixtureMetrics = !string.IsNullOrWhiteSpace(fixtureMetricText);
            if (hasFixtureMetrics)
            {
                parts.Add(fixtureMetricText);
            }
            else
            {
                if (summary.HasResultImage)
                {
                    parts.Add(T("PipelineReview.Result.ImageLabel", "Image") + " " + summary.ResultImageSizeText.Replace(" ", string.Empty));
                }

                string metricText = FormatPrimaryMetricText(step, summary);
                if (!string.IsNullOrWhiteSpace(metricText))
                {
                    parts.Add(metricText);
                }

                if (summary.OverlayCount > 0)
                {
                    parts.Add(TF("PipelineReview.Result.OverlaysFormat", "Overlays {0}", summary.OverlayCount));
                }
            }

            if (summary.IsAcceptanceNg)
            {
                string localizedAcceptanceMessage = OpenVisionPipelineReviewGuidePresenter.FormatAcceptanceMetricNgReason(step, summary);
                if (string.IsNullOrWhiteSpace(localizedAcceptanceMessage))
                {
                    localizedAcceptanceMessage = summary.AcceptanceMessage;
                }

                if (!string.IsNullOrWhiteSpace(localizedAcceptanceMessage))
                {
                    parts.Add(Truncate(localizedAcceptanceMessage, 80));
                }
            }
            else if (!summary.Success && !string.IsNullOrWhiteSpace(summary.Message))
            {
                parts.Add(Truncate(summary.Message, 80));
            }

            return parts.Count == 0 ? SafeText(summary.Message, "-") : string.Join(" / ", parts);
        }

        private static string FormatFixtureMetricText(IDictionary<string, double> metrics)
        {
            if (!TryGetMetricValue(metrics, VisionPipelineKnownMetrics.FixtureOffsetX, out double offsetX)
                || !TryGetMetricValue(metrics, VisionPipelineKnownMetrics.FixtureOffsetY, out double offsetY)
                || !TryGetMetricValue(metrics, VisionPipelineKnownMetrics.FixtureEffectiveRoiX, out double roiX)
                || !TryGetMetricValue(metrics, VisionPipelineKnownMetrics.FixtureEffectiveRoiY, out double roiY))
            {
                return string.Empty;
            }

            return string.Format(
                CultureInfo.CurrentCulture,
                "Fixture \u0394{0},{1} | ROI {2},{3}",
                FormatMetricValue(offsetX),
                FormatMetricValue(offsetY),
                FormatMetricValue(roiX),
                FormatMetricValue(roiY));
        }

        public static string ResolvePairMetricComparisonText(
            VisionPipelineStep step,
            VisionPipelineStepResultSummary summary,
            VisionPipelineSampleCatalogItem activeCatalogSample,
            VisionPipelineSampleCatalogItem activePairCounterpartSample,
            OpenVisionWorkspaceSamplePairDecisionGuide activeSamplePairGuide)
        {
            if (summary?.Metrics == null
                || summary.Metrics.Count == 0
                || activeCatalogSample == null
                || activeSamplePairGuide == null
                || string.IsNullOrWhiteSpace(activeSamplePairGuide.PairReviewText))
            {
                return string.Empty;
            }

            string metricName = ResolvePairComparisonMetricName(step, summary.Metrics, activeCatalogSample, activePairCounterpartSample);
            if (string.IsNullOrWhiteSpace(metricName)
                || !TryGetMetricValue(summary.Metrics, metricName, out double actualValue))
            {
                return string.Empty;
            }

            VisionPipelineSampleExpectedMetric selectedMetric = FindExpectedMetric(activeCatalogSample, metricName);
            VisionPipelineSampleExpectedMetric counterpartMetric = FindExpectedMetric(activePairCounterpartSample, metricName);
            if (selectedMetric == null && counterpartMetric == null)
            {
                return string.Empty;
            }

            string selectedRole = FormatSampleReferenceRole(activeCatalogSample);
            string counterpartRole = FormatSampleReferenceRole(activePairCounterpartSample);
            string selectedRange = FormatExpectedMetricRange(selectedMetric);
            string counterpartRange = FormatExpectedMetricRange(counterpartMetric);
            string acceptanceRange = FormatAcceptanceMetricRange(step, metricName);
            string selectedJudgment = FormatExpectedMetricJudgment(actualValue, selectedMetric, selectedRole);
            string counterpartJudgment = FormatExpectedMetricJudgment(actualValue, counterpartMetric, counterpartRole);

            return string.Format(
                CultureInfo.CurrentCulture,
                LocalText(
                    "\uce21\uc815: {0} {1} / Pipeline \ud310\uc815 \uae30\uc900 {2} / {3} \uc0d8\ud50c \ubc94\uc704 {4} ({5}) / \ubc18\ub300 {6} \uc0d8\ud50c \ubc94\uc704 {7} ({8})",
                    "Measured: {0} {1} / Pipeline gate {2} / {3} sample band {4} ({5}) / opposite {6} sample band {7} ({8})"),
                FormatMetricName(metricName),
                FormatMetricValue(actualValue),
                acceptanceRange,
                selectedRole,
                selectedRange,
                selectedJudgment,
                counterpartRole,
                counterpartRange,
                counterpartJudgment);
        }

        private static string ResolvePairComparisonMetricName(
            VisionPipelineStep step,
            IDictionary<string, double> metrics,
            VisionPipelineSampleCatalogItem activeCatalogSample,
            VisionPipelineSampleCatalogItem activePairCounterpartSample)
        {
            if (metrics == null || metrics.Count == 0)
            {
                return string.Empty;
            }

            if (!string.IsNullOrWhiteSpace(step?.AcceptanceMetricName)
                && TryResolveMetricKey(metrics, step.AcceptanceMetricName, out string acceptanceMetricName))
            {
                return acceptanceMetricName;
            }

            foreach (string expectedMetricName in EnumerateExpectedMetricNames(activeCatalogSample, activePairCounterpartSample))
            {
                if (TryResolveMetricKey(metrics, expectedMetricName, out string actualMetricName))
                {
                    return actualMetricName;
                }
            }

            KeyValuePair<string, double> metric = OrderResultMetrics(step, metrics).FirstOrDefault();
            return metric.Key ?? string.Empty;
        }

        private static IEnumerable<string> EnumerateExpectedMetricNames(params VisionPipelineSampleCatalogItem[] samples)
        {
            HashSet<string> emitted = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (VisionPipelineSampleCatalogItem sample in samples)
            {
                if (sample?.ExpectedMetrics == null)
                {
                    continue;
                }

                foreach (VisionPipelineSampleExpectedMetric metric in sample.ExpectedMetrics)
                {
                    string name = metric?.Name?.Trim();
                    if (!string.IsNullOrWhiteSpace(name) && emitted.Add(name))
                    {
                        yield return name;
                    }
                }
            }
        }

        private static VisionPipelineSampleExpectedMetric FindExpectedMetric(
            VisionPipelineSampleCatalogItem sample,
            string metricName)
        {
            if (sample?.ExpectedMetrics == null || string.IsNullOrWhiteSpace(metricName))
            {
                return null;
            }

            return sample.ExpectedMetrics.FirstOrDefault(metric =>
                metric != null
                && string.Equals(metric.Name?.Trim(), metricName.Trim(), StringComparison.OrdinalIgnoreCase));
        }

        private static bool TryResolveMetricKey(IDictionary<string, double> metrics, string metricName, out string actualMetricName)
        {
            actualMetricName = string.Empty;
            if (metrics == null || string.IsNullOrWhiteSpace(metricName))
            {
                return false;
            }

            foreach (string key in metrics.Keys)
            {
                if (string.Equals(key, metricName, StringComparison.OrdinalIgnoreCase))
                {
                    actualMetricName = key;
                    return true;
                }
            }

            return false;
        }

        private static bool TryGetMetricValue(IDictionary<string, double> metrics, string metricName, out double value)
        {
            value = 0D;
            if (metrics == null || string.IsNullOrWhiteSpace(metricName))
            {
                return false;
            }

            foreach (KeyValuePair<string, double> metric in metrics)
            {
                if (string.Equals(metric.Key, metricName, StringComparison.OrdinalIgnoreCase))
                {
                    value = metric.Value;
                    return true;
                }
            }

            return false;
        }

        private static string FormatExpectedMetricRange(VisionPipelineSampleExpectedMetric metric)
        {
            if (metric == null)
            {
                return "-";
            }

            string minimum = metric.Minimum?.Trim() ?? string.Empty;
            string maximum = metric.Maximum?.Trim() ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(minimum) && !string.IsNullOrWhiteSpace(maximum))
            {
                return string.Equals(minimum, maximum, StringComparison.OrdinalIgnoreCase)
                    ? minimum
                    : minimum + "~" + maximum;
            }

            if (!string.IsNullOrWhiteSpace(minimum))
            {
                return ">= " + minimum;
            }

            if (!string.IsNullOrWhiteSpace(maximum))
            {
                return "<= " + maximum;
            }

            return "-";
        }

        private static string FormatAcceptanceMetricRange(VisionPipelineStep step, string metricName)
        {
            if (step?.UseAcceptance != true
                || !string.Equals(step.AcceptanceMetricName?.Trim(), metricName?.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                return "-";
            }

            bool hasMinimum = step.UseAcceptanceMetricMinimum;
            bool hasMaximum = step.UseAcceptanceMetricMaximum;
            if (hasMinimum && hasMaximum)
            {
                return FormatMetricValue(step.AcceptanceMetricMinimum) + "~" + FormatMetricValue(step.AcceptanceMetricMaximum);
            }

            if (hasMinimum)
            {
                return ">= " + FormatMetricValue(step.AcceptanceMetricMinimum);
            }

            if (hasMaximum)
            {
                return "<= " + FormatMetricValue(step.AcceptanceMetricMaximum);
            }

            return "-";
        }

        private static string FormatExpectedMetricJudgment(
            double actualValue,
            VisionPipelineSampleExpectedMetric metric,
            string roleText)
        {
            bool? isInside = IsInsideExpectedMetricRange(actualValue, metric);
            if (!isInside.HasValue)
            {
                return LocalText("\uae30\ub85d \uc5c6\uc74c", "not recorded");
            }

            string role = string.IsNullOrWhiteSpace(roleText)
                ? LocalText("\ud604\uc7ac", "current")
                : roleText.Trim();
            return isInside.Value
                ? string.Format(CultureInfo.CurrentCulture, LocalText("{0} \uc0d8\ud50c \ubc94\uc704 \uc77c\uce58", "matches {0} sample band"), role)
                : string.Format(CultureInfo.CurrentCulture, LocalText("{0} \uc0d8\ud50c \ubc94\uc704 \ubd88\uc77c\uce58", "outside {0} sample band"), role);
        }

        private static bool? IsInsideExpectedMetricRange(double actualValue, VisionPipelineSampleExpectedMetric metric)
        {
            if (metric == null)
            {
                return null;
            }

            bool hasMinimum = TryParseMetricLimit(metric.Minimum, out double minimum);
            bool hasMaximum = TryParseMetricLimit(metric.Maximum, out double maximum);
            if (!hasMinimum && !hasMaximum)
            {
                return null;
            }

            if (hasMinimum && actualValue < minimum)
            {
                return false;
            }

            if (hasMaximum && actualValue > maximum)
            {
                return false;
            }

            return true;
        }

        private static bool TryParseMetricLimit(string text, out double value)
        {
            value = 0D;
            string normalized = text?.Trim() ?? string.Empty;
            if (normalized.Length == 0)
            {
                return false;
            }

            return double.TryParse(normalized, NumberStyles.Float, CultureInfo.InvariantCulture, out value)
                || double.TryParse(normalized, NumberStyles.Float, CultureInfo.CurrentCulture, out value);
        }

        private static string FormatSampleReferenceRole(VisionPipelineSampleCatalogItem sample)
        {
            if (IsOkSampleReference(sample))
            {
                return "OK";
            }

            if (IsNgSampleReference(sample))
            {
                return "NG";
            }

            return LocalText("\uae30\uc900", "Reference");
        }

        private static string FormatPrimaryMetricText(VisionPipelineStep step, VisionPipelineStepResultSummary summary)
        {
            if (summary?.Metrics == null || summary.Metrics.Count == 0)
            {
                return string.Empty;
            }

            KeyValuePair<string, double> metric = OrderResultMetrics(step, summary.Metrics).FirstOrDefault();
            return string.IsNullOrWhiteSpace(metric.Key)
                ? string.Empty
                : string.Format(CultureInfo.CurrentCulture, "{0} {1}", FormatMetricName(metric.Key), FormatMetricValue(metric.Value));
        }

        private static IEnumerable<KeyValuePair<string, double>> OrderResultMetrics(VisionPipelineStep step, IDictionary<string, double> metrics)
        {
            if (metrics == null)
            {
                yield break;
            }

            HashSet<string> emitted = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (string metricName in VisionPipelineKnownMetrics.GetMetricNamesForTool(step?.ToolType))
            {
                if (IsReviewDetailMetric(metricName)
                    && metrics.TryGetValue(metricName, out double value)
                    && emitted.Add(metricName))
                {
                    yield return new KeyValuePair<string, double>(metricName, value);
                }
            }

            foreach (KeyValuePair<string, double> metric in VisionPipelineKnownMetrics.OrderMetrics(metrics))
            {
                if (IsReviewDetailMetric(metric.Key) && emitted.Add(metric.Key))
                {
                    yield return metric;
                }
            }
        }

        private static bool IsReviewDetailMetric(string metricName)
        {
            return !string.Equals(metricName, VisionPipelineKnownMetrics.SourceImageWidth, StringComparison.OrdinalIgnoreCase)
                && !string.Equals(metricName, VisionPipelineKnownMetrics.SourceImageHeight, StringComparison.OrdinalIgnoreCase)
                && !string.Equals(metricName, VisionPipelineKnownMetrics.SourceImageChannels, StringComparison.OrdinalIgnoreCase)
                && !string.Equals(metricName, VisionPipelineKnownMetrics.ResultImageWidth, StringComparison.OrdinalIgnoreCase)
                && !string.Equals(metricName, VisionPipelineKnownMetrics.ResultImageHeight, StringComparison.OrdinalIgnoreCase)
                && !string.Equals(metricName, VisionPipelineKnownMetrics.ResultImageChannels, StringComparison.OrdinalIgnoreCase);
        }

        private static string FormatMetricName(string metricName)
        {
            string name = SafeText(metricName, string.Empty);
            if (string.IsNullOrWhiteSpace(name))
            {
                return "-";
            }

            string displayName = T(
                "PipelineReview.Metric." + name,
                VisionPipelineKnownMetrics.GetDisplayName(name));
            if (string.Equals(name, VisionPipelineKnownMetrics.ResultCount, StringComparison.OrdinalIgnoreCase)
                && !displayName.Contains("Result", StringComparison.OrdinalIgnoreCase))
            {
                return "Result (" + displayName + ")";
            }

            return displayName;
        }

        private static string FormatMetricValue(double value)
        {
            return Math.Abs(value - Math.Round(value)) < 0.000001
                ? Math.Round(value).ToString("0", CultureInfo.InvariantCulture)
                : value.ToString("0.###", CultureInfo.InvariantCulture);
        }

        private static string Truncate(string value, int maxLength)
        {
            string text = SafeText(value, string.Empty);
            return text.Length <= maxLength ? text : text.Substring(0, Math.Max(0, maxLength - 3)) + "...";
        }

        private static string FormatImageState(string layerName, Bitmap image)
        {
            string title = SafeText(layerName, "-");
            return image == null
                ? title + " / " + T("PipelineReview.ImageMissing", "missing")
                : string.Format(CultureInfo.CurrentCulture, "{0} / {1}x{2}", title, image.Width, image.Height);
        }

        private static bool IsOkSampleReference(VisionPipelineSampleCatalogItem item)
        {
            return item != null
                && !item.ExpectsFailure
                && string.Equals(item.PairRole?.Trim(), "Good", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsNgSampleReference(VisionPipelineSampleCatalogItem item)
        {
            return item != null
                && (item.ExpectsFailure
                    || string.Equals(item.PairRole?.Trim(), "Bad", StringComparison.OrdinalIgnoreCase));
        }

        private static string SafeText(string value, string fallback)
        {
            return string.IsNullOrWhiteSpace(value) ? fallback : value.Trim();
        }

        private static string T(string key, string fallbackText)
        {
            string value = OpenVisionLanguageService.T(key);
            return string.IsNullOrWhiteSpace(value) || string.Equals(value, key, StringComparison.Ordinal)
                ? fallbackText ?? string.Empty
                : value;
        }

        private static string TF(string key, string fallbackFormat, params object[] args)
        {
            return string.Format(CultureInfo.CurrentCulture, T(key, fallbackFormat), args);
        }

        private static string LocalText(string korean, string english)
        {
            return OpenVisionLanguageService.CurrentLanguage == OpenVisionLanguage.English
                ? english ?? korean ?? string.Empty
                : korean ?? english ?? string.Empty;
        }
    }
}
