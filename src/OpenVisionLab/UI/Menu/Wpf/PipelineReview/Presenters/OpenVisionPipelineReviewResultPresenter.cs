using OpenVisionLab.Vision2D.Pipeline;
using OpenVisionLab.Pipeline.Controls;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace OpenVisionLab
{
    internal static class OpenVisionPipelineReviewResultPresenter
    {
        private static readonly IReadOnlyList<OpenVisionLocalizationEntry> workbenchValidationTemplates =
            OpenVisionLanguageService.GetDefaultEntries("Workbench.Validate.Detail.");

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
                TF("PipelineReview.RunLog.ResultFormat", "Result: {0}", FormatResultSummary(step, summary)),
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

        public static string FormatResultSummary(VisionPipelineStep step, VisionPipelineStepResultSummary summary)
        {
            string status = FormatResultSummary(summary);
            if (summary == null || step == null)
            {
                return status;
            }

            return status + " / " + FormatProcessingAndAcceptance(step, summary);
        }

        private static string FormatProcessingAndAcceptance(
            VisionPipelineStep step,
            VisionPipelineStepResultSummary summary)
        {
            if (summary.Skipped
                || string.Equals(
                    summary.ExecutionState,
                    VisionPipelineResultSummaryService.DisabledState,
                    StringComparison.Ordinal))
            {
                return LocalText("건너뜀 / 판정 미평가", "Skipped / Judgement not evaluated");
            }

            if (string.Equals(
                summary.ExecutionState,
                VisionPipelineResultSummaryService.NotRunAfterFailureState,
                StringComparison.Ordinal))
            {
                return LocalText("이전 Step 실패로 미실행", "Not run after an earlier step failed");
            }

            if (string.Equals(
                summary.ExecutionState,
                VisionPipelineResultSummaryService.CancelledState,
                StringComparison.Ordinal))
            {
                return LocalText("취소됨 / 판정 미평가", "Cancelled / Judgement not evaluated");
            }

            bool toolSucceeded = summary.IsAcceptanceNg || (summary.Success && !summary.IsToolError);
            if (!toolSucceeded)
            {
                return LocalText("처리 NG / 판정 미평가", "Tool NG / Judgement not evaluated");
            }

            if (summary.IsAcceptanceNg)
            {
                return LocalText("처리 OK / 판정 NG", "Tool OK / Judgement NG");
            }

            return step.UseAcceptance
                ? LocalText("처리 OK / 판정 PASS", "Tool OK / Judgement PASS")
                : LocalText("처리 OK / 판정 미평가", "Tool OK / Judgement not evaluated");
        }

        public static string FormatResultDetails(VisionPipelineStep step, VisionPipelineStepResultSummary summary)
        {
            if (summary == null)
            {
                return T("PipelineReview.NoRunResultForStep", "No run result for selected step.");
            }

            if (!summary.Executed)
            {
                return summary.Message;
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

        internal static IEnumerable<KeyValuePair<string, double>> OrderResultMetrics(VisionPipelineStep step, IDictionary<string, double> metrics)
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

        internal static string FormatWorkbenchValidation(string error, VisionPipelineStep step = null, int index = -1)
        {
            string detail = error ?? string.Empty;
            string namedPrefix = $"Step {index + 1} '{step?.Name}': ";
            string prefix = $"Step {index + 1}: ";
            if (detail.StartsWith(namedPrefix, StringComparison.Ordinal)) { detail = detail.Substring(namedPrefix.Length); }
            else if (detail.StartsWith(prefix, StringComparison.Ordinal)) { detail = detail.Substring(prefix.Length); }
            if ((detail.StartsWith("input layer B '", StringComparison.Ordinal) && detail.EndsWith("does not exist before this step.", StringComparison.Ordinal))
                || detail.StartsWith("InputLayerB is required", StringComparison.Ordinal)) { return T("Workbench.Validate.InputBMissing", "No earlier step creates input B."); }
            if ((detail.StartsWith("input layer '", StringComparison.Ordinal) && detail.EndsWith("does not exist before this step.", StringComparison.Ordinal))
                || detail == "InputLayer is required.") { return TF("Workbench.Validate.InputMissingFormat", "No earlier step creates input ‘{0}’.", step?.InputLayer ?? string.Empty); }
            if (detail == "OutputLayer is required." || detail == "output layer must not be Main.") { return T("Workbench.Validate.OutputInvalid", "The output name is empty or Main."); }
            if (detail.StartsWith("output layer '", StringComparison.Ordinal) && detail.EndsWith("duplicates an earlier step.", StringComparison.Ordinal))
            {
                return TF("Workbench.Validate.OutputDuplicateFormat", "Output name ‘{0}’ is used by an earlier step.", step?.OutputLayer ?? string.Empty);
            }
            if (detail == "ToolType is required." || detail.StartsWith("unsupported ToolType '", StringComparison.Ordinal)) { return T("Workbench.Validate.ToolNotInPipeline", "This tool cannot run in a pipeline."); }
            foreach (OpenVisionLocalizationEntry template in workbenchValidationTemplates)
            {
                string pattern = Regex.Replace(Regex.Escape(template.English), @"\\\{(\d+)\}", match => "(?<p" + match.Groups[1].Value + ">.+?)");
                Match matched = Regex.Match(detail, "\\A" + pattern + "\\z", RegexOptions.CultureInvariant | RegexOptions.Singleline, TimeSpan.FromMilliseconds(100));
                if (!matched.Success) { continue; }
                int count = Regex.Matches(template.English, @"\{\d+\}").Count;
                object[] args = Enumerable.Range(0, count).Select(number => (object)matched.Groups["p" + number].Value).ToArray();
                return string.Format(CultureInfo.CurrentCulture, OpenVisionLanguageService.T(template.Key), args);
            }
            return detail;
        }

        internal static string FormatWorkbenchSummary(VisionPipelineStepResultSummary summary)
        {
            if (summary == null || !summary.Executed || summary.Skipped || (!summary.Success && !summary.IsAcceptanceNg)) { return string.Empty; }
            string Parameter(string name) => summary.Parameters.TryGetValue(name, out string value) ? value : string.Empty;
            if (string.Equals(summary.ToolType, "Threshold", StringComparison.OrdinalIgnoreCase) && summary.Parameters.ContainsKey("Mode"))
            {
                string mode = Parameter("Mode");
                if (mode == "Range")
                {
                    return T("Threshold.ModeRange", "Range") + " · " + Parameter("RangeMin") + "–" + Parameter("RangeMax")
                        + (Parameter("Invert") == "True" ? " · " + T("Threshold.Invert", "Invert") : string.Empty);
                }
                if (mode == "Adaptive")
                {
                    return T("Threshold.ModeAdaptive", "Adaptive") + " · " + T("Threshold.BlockSize", "Block size") + " " + Parameter("BlockSize")
                        + " · " + T("Threshold.Weight", "Correction (C)") + " " + Parameter("Weight");
                }
                return Parameter("ThresholdType") + " · " + T("PropertyGrid.Property.THRESHOLD.DisplayName", "Threshold") + " " + Parameter("Threshold");
            }
            if (string.Equals(summary.ToolType, VisionPipelineArithmeticStep.ToolType, StringComparison.OrdinalIgnoreCase)
                && Parameter(VisionPipelineArithmeticStep.ParameterMode) == VisionPipelineArithmeticStep.ModeOffset)
            {
                return T("Arithmetic.OffsetMode", "Offset") + " · X " + Parameter(VisionPipelineArithmeticStep.ParameterOffsetX)
                    + " · Y " + Parameter(VisionPipelineArithmeticStep.ParameterOffsetY);
            }
            string metrics = string.Join(" · ", OrderResultMetrics(new VisionPipelineStep { ToolType = summary.ToolType }, summary.Metrics)
                .OrderBy(metric => OpenVisionLanguageService.T("Workbench.Metric." + metric.Key) == "Workbench.Metric." + metric.Key ? 1 : 0).Take(2)
                .Select(metric => WorkbenchMetricName(metric.Key) + " " + WorkbenchMetricValue(metric.Key, metric.Value)));
            return !string.IsNullOrEmpty(metrics) ? metrics : summary.HasResultImage ? summary.ResultImageWidth + " × " + summary.ResultImageHeight + " px" : string.Empty;
        }

        internal static string FormatWorkbenchNextAction(VisionPipelineStepResultSummary summary)
        {
            if ((summary?.Success != true && summary?.IsAcceptanceNg != true) || (summary.ToolType != "Blob" && summary.ToolType != "Contour")) { return string.Empty; }
            return !summary.IsAcceptanceNg && summary.Metrics.TryGetValue(VisionPipelineKnownMetrics.ResultCount, out double count) && count > 0
                ? VisionToolVerificationText.CheckResultThenAddPipeline : VisionToolVerificationText.AdjustAreaThreshold;
        }

        internal static string FormatWorkbenchElapsed(double milliseconds) => milliseconds.ToString(milliseconds < 10 ? "N1" : "N0", CultureInfo.CurrentCulture) + " ms";

        internal static string WorkbenchResultKind(VisionPipelineStep step, VisionPipelineStepResultSummary summary) => summary == null ? "NotRun"
            : summary.ExecutionState == VisionPipelineResultSummaryService.NotRunAfterFailureState ? "NotRun"
            : summary.ExecutionState == VisionPipelineResultSummaryService.CancelledState ? "Cancelled"
            : summary.Skipped ? "Disabled" : summary.IsAcceptanceNg ? "Ng" : !summary.Success ? "Error" : step?.UseAcceptance == true ? "Ok" : "Done";

        internal static string FormatWorkbenchAcceptance(VisionPipelineStep step)
        {
            if (!step.UseAcceptance) { return string.Empty; }
            string metric = WorkbenchMetricName(step.AcceptanceMetricName);
            string minimum = WorkbenchMetricValue(step.AcceptanceMetricName, step.AcceptanceMetricMinimum);
            string maximum = WorkbenchMetricValue(step.AcceptanceMetricName, step.AcceptanceMetricMaximum);
            return metric + " " + (step.UseAcceptanceMetricMinimum && step.UseAcceptanceMetricMaximum
                ? WorkbenchMetricValue(step.AcceptanceMetricName, step.AcceptanceMetricMinimum, false) + "~" + maximum
                : step.UseAcceptanceMetricMinimum ? TF("Workbench.Acc.MinimumFormat", "{0} or more", minimum)
                : step.UseAcceptanceMetricMaximum ? TF("Workbench.Acc.MaximumFormat", "{0} or less", maximum) : string.Empty);
        }

        internal static string FormatWorkbenchFailure(VisionPipelineStep step, VisionPipelineStepResultSummary summary)
        {
            if (!summary.IsAcceptanceNg)
            {
                return summary.ErrorName == "MatchingTemplateMissing" || summary.ErrorName == "FeatureTemplateMissing" || summary.ErrorName == "TemplateImageMissing"
                    ? T("Workbench.Tool.TemplateMissing", "No template") : summary.Message;
            }
            bool intrinsicMean = !step.UseAcceptance && string.Equals(step.ToolType, "Mean", StringComparison.OrdinalIgnoreCase);
            string metricName = step.AcceptanceMetricName ?? string.Empty;
            bool useMinimum = step.UseAcceptanceMetricMinimum, useMaximum = step.UseAcceptanceMetricMaximum;
            double minimum = step.AcceptanceMetricMinimum, maximum = step.AcceptanceMetricMaximum;
            if (intrinsicMean)
            {
                metricName = "MeanValueAvg";
                useMinimum = useMaximum = true;
                minimum = step.Parameters.TryGetValue("MEAN_MIN", out string minimumText) && int.TryParse(minimumText, NumberStyles.Integer, CultureInfo.InvariantCulture, out int meanMinimum) ? meanMinimum : 100;
                maximum = step.Parameters.TryGetValue("MEAN_MAX", out string maximumText) && int.TryParse(maximumText, NumberStyles.Integer, CultureInfo.InvariantCulture, out int meanMaximum) ? meanMaximum : 240;
            }
            List<string> reasons = new List<string>();
            if (!intrinsicMean && step.MaxElapsedMilliseconds > 0 && summary.ElapsedMilliseconds > step.MaxElapsedMilliseconds)
            {
                reasons.Add(TF("Workbench.Acc.TimeOverFormat", "Time {0} · over {1} ms", FormatWorkbenchElapsed(summary.ElapsedMilliseconds),
                    step.MaxElapsedMilliseconds.ToString(step.MaxElapsedMilliseconds < 10 ? "N1" : "N0", CultureInfo.CurrentCulture)));
            }
            if (summary.Metrics.TryGetValue(metricName, out double value))
            {
                string actual = WorkbenchMetricName(metricName) + " " + WorkbenchMetricValue(metricName, value);
                bool equal = useMinimum && useMaximum && minimum == maximum;
                if (useMinimum && value < minimum)
                {
                    reasons.Add(TF("Workbench.Acc.BelowFormat", "{0} · below {1} {2}", actual, T(equal ? "Workbench.Acc.Target" : "Workbench.Acc.Lower", "Lower"),
                        WorkbenchMetricValue(metricName, minimum)));
                }
                if (useMaximum && value > maximum)
                {
                    reasons.Add(TF("Workbench.Acc.AboveFormat", "{0} · above {1} {2}", actual, T(equal ? "Workbench.Acc.Target" : "Workbench.Acc.Upper", "Upper"),
                        WorkbenchMetricValue(metricName, maximum)));
                }
            }
            return reasons.Count > 0 ? string.Join(" · ", reasons) : summary.AcceptanceMessage;
        }

        internal static string WorkbenchMetricName(string name) => T("Workbench.Metric." + name, name ?? string.Empty);

        internal static (string Unit, int Digits) WorkbenchMetricFormat(string name) => name switch
        {
                "ResultCount" or "EdgeCount" or "DistanceCount" => ("count", 0),
                "AreaMax" or "AreaMin" or "AreaAvg" => ("px²", 0),
                "BoundsWidthMax" or "BoundsHeightMax" or "MaskPixelCount" or "ResultImageWidth" => ("px", 0),
                "ScoreMax" or "MaskPixelRatio" => (string.Empty, 3),
                "LineLengthMax" or "IntersectionX" or "IntersectionY" => ("px", 1),
                "LineAngleAvg" => ("°", 2),
                "DistancePxAvg" => ("px", 2),
                "DistanceMmAvg" or "DistanceMmRange" => ("mm", 3),
                "MeanValueAvg" or "MeanStdDev" => ("GV", 1),
                "ForegroundRatio" => ("%", 2),
                _ => (string.Empty, 3)
        };

        internal static string WorkbenchMetricValue(string name, double value, bool includeUnit = true)
        {
            (string unit, int digits) = WorkbenchMetricFormat(name);
            string number = value.ToString("N" + digits.ToString(CultureInfo.InvariantCulture), CultureInfo.CurrentCulture);
            return !includeUnit || string.IsNullOrEmpty(unit) ? number : unit == "count"
                ? TF("Workbench.Unit.CountFormat", "{0} items", number) : number + " " + unit;
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
