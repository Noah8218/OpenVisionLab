using Lib.Common;
using Lib.OpenCV.Pipeline;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace OpenVisionLab
{
    internal sealed class VisionPipelineSampleCheckResult
    {
        public string Status { get; set; } = string.Empty;
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string MetricText { get; set; } = string.Empty;
        public string DistanceMetricText { get; set; } = string.Empty;
        public string MetricReviewText { get; set; } = string.Empty;
        public string FinalLayerText { get; set; } = string.Empty;
        public string OverlayCountText { get; set; } = string.Empty;
        public string FailedStepText { get; set; } = string.Empty;
        public string ActionSummaryText { get; set; } = string.Empty;
        public string StepSummaryText { get; set; } = string.Empty;
        public string RunReportPath { get; set; } = string.Empty;
        public double TotalMilliseconds { get; set; }
        public DateTime CheckedAt { get; set; }
    }

    internal static class VisionPipelineSampleCheckService
    {
        public static Task<VisionPipelineSampleCheckResult> RunSampleCheckSafeAsync(
            VisionPipelineSampleCatalogItem sample,
            string pipelineXmlText = null)
        {
            return Task.Run(() => RunSampleCheckSafe(sample, pipelineXmlText));
        }

        public static Task<VisionPipelineSampleCheckResult> RunSampleCheckWithReportSafeAsync(
            VisionPipelineSampleCatalogItem sample,
            string pipelineXmlText,
            string recipeName)
        {
            return Task.Run(() => RunSampleCheckSafe(sample, pipelineXmlText, recipeName));
        }

        public static VisionPipelineSampleCheckResult RunSampleCheckSafe(
            VisionPipelineSampleCatalogItem sample,
            string pipelineXmlText = null)
        {
            return RunSampleCheckSafe(sample, pipelineXmlText, null);
        }

        private static VisionPipelineSampleCheckResult RunSampleCheckSafe(
            VisionPipelineSampleCatalogItem sample,
            string pipelineXmlText,
            string reportRecipeName)
        {
            try
            {
                if (sample == null)
                {
                    return CreateErrorResult("Sample is null.");
                }

                if (string.IsNullOrWhiteSpace(sample.ImageFullPath) || !System.IO.File.Exists(sample.ImageFullPath))
                {
                    return CreateErrorResult($"Sample image is missing: {sample.SampleName}");
                }

                if (string.IsNullOrWhiteSpace(pipelineXmlText) && !sample.CanOpen)
                {
                    return CreateErrorResult($"Sample pipeline is missing: {sample.SampleName}");
                }

                return RunSampleCheck(sample, pipelineXmlText, reportRecipeName);
            }
            catch (Exception ex)
            {
                return CreateErrorResult(ex.GetBaseException().Message);
            }
        }

        public static VisionPipelineSampleCheckResult CreateErrorResult(string message)
        {
            return new VisionPipelineSampleCheckResult
            {
                Status = "ERROR",
                Success = false,
                Message = message ?? string.Empty,
                MetricText = "-",
                DistanceMetricText = string.Empty,
                MetricReviewText = "Metric review: check failed before metric evaluation.",
                FinalLayerText = "-",
                OverlayCountText = "-",
                FailedStepText = "-",
                ActionSummaryText = message ?? string.Empty,
                StepSummaryText = string.Empty,
                CheckedAt = DateTime.Now
            };
        }

        public static List<VisionPipelineSampleCatalogItem> GetPairSamples(VisionPipelineSampleCatalogItem sample)
        {
            if (sample == null || string.IsNullOrWhiteSpace(sample.PairGroup))
            {
                return new List<VisionPipelineSampleCatalogItem>();
            }

            string pairGroup = sample.PairGroup.Trim();
            return VisionPipelineSampleCatalogItem.LoadRunnable(sample.CatalogSourceKind)
                .Where(item => string.Equals(item.PairGroup?.Trim(), pairGroup, StringComparison.OrdinalIgnoreCase))
                .OrderBy(item => GetPairRoleOrder(item.PairRole))
                .ThenBy(item => item.SampleName, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        public static string BuildExpectedMetricRangeText(VisionPipelineSampleExpectedMetric expectedMetric)
        {
            if (expectedMetric == null)
            {
                return "-";
            }

            string minimum = string.IsNullOrWhiteSpace(expectedMetric.Minimum) ? string.Empty : expectedMetric.Minimum.Trim();
            string maximum = string.IsNullOrWhiteSpace(expectedMetric.Maximum) ? string.Empty : expectedMetric.Maximum.Trim();
            if (!string.IsNullOrWhiteSpace(minimum) && !string.IsNullOrWhiteSpace(maximum))
            {
                return string.Equals(minimum, maximum, StringComparison.OrdinalIgnoreCase)
                    ? minimum
                    : $"{minimum}..{maximum}";
            }

            if (!string.IsNullOrWhiteSpace(minimum))
            {
                return $">= {minimum}";
            }

            if (!string.IsNullOrWhiteSpace(maximum))
            {
                return $"<= {maximum}";
            }

            return "-";
        }

        private static VisionPipelineSampleCheckResult RunSampleCheck(
            VisionPipelineSampleCatalogItem sample,
            string pipelineXmlText,
            string reportRecipeName)
        {
            DateTime checkedAt = DateTime.Now;
            using (Bitmap bitmap = new Bitmap(sample.ImageFullPath))
            using (Mat source = BitmapImageConverter.ToMat(bitmap))
            using (VisionRecipeRunResult result = RunRecipe(sample, source, pipelineXmlText, out VisionPipeline pipeline))
            {
                DateTime finishedAt = DateTime.Now;
                string runReportPath = string.IsNullOrWhiteSpace(reportRecipeName)
                    ? string.Empty
                    : VisionPipelineRunReportStorage.Save(
                        reportRecipeName,
                        pipeline,
                        result,
                        checkedAt,
                        finishedAt);
                List<string> messages = new List<string>();
                bool expectedFailure = sample.ExpectsFailure;
                if (!result.Success && !expectedFailure && !string.IsNullOrWhiteSpace(result.Message))
                {
                    messages.Add(result.Message);
                }
                else if (result.Success && expectedFailure)
                {
                    messages.Add("Expected failure did not occur.");
                }

                if (sample.Width > 0
                    && sample.Height > 0
                    && (bitmap.Width != sample.Width || bitmap.Height != sample.Height))
                {
                    messages.Add($"Image size {bitmap.Width} x {bitmap.Height} does not match catalog {sample.Width} x {sample.Height}.");
                }

                bool success = expectedFailure ? !result.Success : result.Success;
                string metricText = "no metric gate";
                string metricReviewText = "Metric review: no metric gate";

                IReadOnlyList<VisionPipelineSampleExpectedMetric> expectedMetrics = sample.ExpectedMetrics;
                if (expectedMetrics.Count > 0)
                {
                    List<string> metricParts = new List<string>();
                    List<string> metricReviewLines = new List<string>();
                    foreach (VisionPipelineSampleExpectedMetric expectedMetric in expectedMetrics)
                    {
                        if (string.IsNullOrWhiteSpace(expectedMetric.Name))
                        {
                            continue;
                        }

                        string expectedRangeText = BuildExpectedMetricRangeText(expectedMetric);
                        if (!TryFindMetric(result, expectedMetric.Name, out double metricValue))
                        {
                            messages.Add($"Expected metric '{expectedMetric.Name}' was not produced.");
                            metricParts.Add($"{expectedMetric.Name}=missing");
                            metricReviewLines.Add($"{expectedMetric.Name}: expected {expectedRangeText}, actual missing, judgment MISSING");
                            continue;
                        }

                        metricParts.Add($"{expectedMetric.Name}={metricValue:0.###}");
                        bool metricPassed = true;
                        if (TryParseDouble(expectedMetric.Minimum, out double minimum) && metricValue < minimum)
                        {
                            messages.Add($"{expectedMetric.Name} {metricValue:0.###} < {minimum:0.###}.");
                            metricPassed = false;
                        }

                        if (TryParseDouble(expectedMetric.Maximum, out double maximum) && metricValue > maximum)
                        {
                            messages.Add($"{expectedMetric.Name} {metricValue:0.###} > {maximum:0.###}.");
                            metricPassed = false;
                        }

                        metricReviewLines.Add(
                            $"{expectedMetric.Name}: expected {expectedRangeText}, actual {metricValue:0.###}, judgment {(metricPassed ? "OK" : "NG")}");
                    }

                    metricText = metricParts.Count == 0 ? "no metric gate" : string.Join("; ", metricParts);
                    metricReviewText = metricReviewLines.Count == 0
                        ? "Metric review: no metric gate"
                        : "Metric review:" + Environment.NewLine + " - " + string.Join(Environment.NewLine + " - ", metricReviewLines);
                }

                success = success && messages.Count == 0;
                string message = messages.Count == 0
                    ? result.Message
                    : string.Join(" ", messages);

                return new VisionPipelineSampleCheckResult
                {
                    Status = success ? "OK" : "NG",
                    Success = success,
                    Message = message,
                    MetricText = metricText,
                    DistanceMetricText = BuildDistanceMetricText(result),
                    MetricReviewText = metricReviewText,
                    FinalLayerText = string.IsNullOrWhiteSpace(result.FinalLayer) ? "-" : result.FinalLayer,
                    OverlayCountText = ResolveOverlayCountText(result),
                    FailedStepText = ResolveFailedStepText(result),
                    ActionSummaryText = result.ActionSummaryText,
                    StepSummaryText = result.StepSummaryText,
                    RunReportPath = runReportPath,
                    TotalMilliseconds = result.TotalMilliseconds,
                    CheckedAt = checkedAt
                };
            }
        }

        private static VisionRecipeRunResult RunRecipe(
            VisionPipelineSampleCatalogItem sample,
            Mat source,
            string pipelineXmlText,
            out VisionPipeline pipeline)
        {
            VisionRecipeRunner runner = new VisionRecipeRunner();
            if (!string.IsNullOrWhiteSpace(pipelineXmlText))
            {
                if (!SerializeHelper.TryLoadFromXmlText(pipelineXmlText, out pipeline, out string loadError) || pipeline == null)
                {
                    throw new InvalidOperationException(string.IsNullOrWhiteSpace(loadError)
                        ? "Recipe XML could not be loaded."
                        : loadError);
                }

                return runner
                    .RunAsync(pipeline, source, "Main", VisionRecipeRunner.DefaultStepTimeoutMilliseconds)
                    .GetAwaiter()
                    .GetResult();
            }

            if (!SerializeHelper.TryLoadFromXmlFile(sample.PipelineFullPath, out pipeline) || pipeline == null)
            {
                throw new InvalidOperationException($"Recipe XML could not be loaded: {sample.PipelineFullPath}");
            }

            return runner
                .RunAsync(pipeline, source, "Main", VisionRecipeRunner.DefaultStepTimeoutMilliseconds)
                .GetAwaiter()
                .GetResult();
        }

        private static int GetPairRoleOrder(string pairRole)
        {
            string role = (pairRole ?? string.Empty).Trim();
            if (string.Equals(role, "Good", StringComparison.OrdinalIgnoreCase)
                || string.Equals(role, "OK", StringComparison.OrdinalIgnoreCase)
                || string.Equals(role, "Normal", StringComparison.OrdinalIgnoreCase))
            {
                return 0;
            }

            if (string.Equals(role, "Bad", StringComparison.OrdinalIgnoreCase)
                || string.Equals(role, "NG", StringComparison.OrdinalIgnoreCase)
                || string.Equals(role, "Abnormal", StringComparison.OrdinalIgnoreCase))
            {
                return 1;
            }

            return 2;
        }

        private static string ResolveOverlayCountText(VisionRecipeRunResult result)
        {
            VisionRecipeStepRunSummary finalStep = result?.Steps?.LastOrDefault();
            if (finalStep == null)
            {
                return "-";
            }

            return finalStep.OverlayCount.ToString(CultureInfo.InvariantCulture);
        }

        private static string ResolveFailedStepText(VisionRecipeRunResult result)
        {
            VisionRecipeStepRunSummary failedStep = result?.Steps?.FirstOrDefault(step => !step.Success);
            if (failedStep == null)
            {
                return string.Empty;
            }

            string message = string.IsNullOrWhiteSpace(failedStep.Message) ? string.Empty : $" - {failedStep.Message}";
            return $"{failedStep.Index:00} {failedStep.Name} [{failedStep.Status}]{message}";
        }

        private static bool TryFindMetric(VisionRecipeRunResult result, string metricName, out double value)
        {
            value = 0;
            foreach (VisionRecipeStepRunSummary step in result?.Steps?.AsEnumerable().Reverse() ?? Enumerable.Empty<VisionRecipeStepRunSummary>())
            {
                if (step.Metrics != null && step.Metrics.TryGetValue(metricName, out value))
                {
                    return true;
                }
            }

            return false;
        }

        private static string BuildDistanceMetricText(VisionRecipeRunResult result)
        {
            List<string> parts = new List<string>();
            AppendMetric(parts, result, VisionPipelineKnownMetrics.DistanceMmAvg);
            AppendMetric(parts, result, VisionPipelineKnownMetrics.DistanceMmRange);
            AppendMetric(parts, result, VisionPipelineKnownMetrics.DistancePxAvg);
            AppendMetric(parts, result, VisionPipelineKnownMetrics.DistancePxRange);
            return string.Join("; ", parts);
        }

        private static void AppendMetric(List<string> parts, VisionRecipeRunResult result, string metricName)
        {
            if (TryFindMetric(result, metricName, out double value))
            {
                parts.Add($"{metricName}={value:0.###}");
            }
        }

        private static bool TryParseDouble(string text, out double value)
        {
            return double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
        }
    }
}
