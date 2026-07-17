using Lib.OpenCV.Pipeline;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace OpenVisionLab
{
    public enum OpenVisionPipelineReviewReadinessLevel
    {
        Ready,
        Check,
        Advisory,
        NotApplicable
    }

    public sealed class OpenVisionPipelineReviewReadinessItem
    {
        public OpenVisionPipelineReviewReadinessItem(
            string key,
            string title,
            string statusText,
            OpenVisionPipelineReviewReadinessLevel level)
        {
            Key = key ?? string.Empty;
            Title = title ?? string.Empty;
            StatusText = statusText ?? string.Empty;
            Level = level;
        }

        public string Key { get; }
        public string Title { get; }
        public string StatusText { get; }
        public OpenVisionPipelineReviewReadinessLevel Level { get; }
        public string AutomationId => "PipelineReviewReadiness" + Key;
        public string StatusAutomationId => AutomationId + "Status";
        public bool IsReady => Level == OpenVisionPipelineReviewReadinessLevel.Ready;
        public bool IsCheck => Level == OpenVisionPipelineReviewReadinessLevel.Check;
        public bool IsAdvisory => Level == OpenVisionPipelineReviewReadinessLevel.Advisory;
        public bool IsNotApplicable => Level == OpenVisionPipelineReviewReadinessLevel.NotApplicable;
    }

    public sealed class OpenVisionPipelineReviewReadinessState
    {
        public OpenVisionPipelineReviewReadinessState(
            IReadOnlyList<OpenVisionPipelineReviewReadinessItem> items,
            string summaryText)
        {
            Items = items ?? Array.Empty<OpenVisionPipelineReviewReadinessItem>();
            SummaryText = summaryText ?? string.Empty;
        }

        public IReadOnlyList<OpenVisionPipelineReviewReadinessItem> Items { get; }
        public string SummaryText { get; }
    }

    internal static class OpenVisionPipelineReviewReadinessPresenter
    {
        public static OpenVisionPipelineReviewReadinessState Create(
            VisionPipeline pipeline,
            VisionPipelineValidationResult validationResult,
            Func<string, bool> hasLayerImage,
            bool hasGoodBadGuide,
            bool hasGoodBadCounterpart)
        {
            List<VisionPipelineStep> enabledSteps = (pipeline?.Steps ?? new List<VisionPipelineStep>())
                .Where(step => step?.Enabled == true)
                .ToList();
            List<string> externalInputs = ResolveExternalInputs(enabledSteps);
            List<string> missingInputs = externalInputs
                .Where(layer => hasLayerImage?.Invoke(layer) != true)
                .ToList();

            List<OpenVisionPipelineReviewReadinessItem> items = new List<OpenVisionPipelineReviewReadinessItem>
            {
                CreateInputItem(enabledSteps.Count, externalInputs.Count, missingInputs),
                CreateRouteItem(enabledSteps.Count, validationResult),
                CreateAcceptanceItem(enabledSteps),
                CreateGoodBadItem(hasGoodBadGuide, hasGoodBadCounterpart),
                CreateCalibrationItem(enabledSteps)
            };

            int checkCount = items.Count(item => item.Level == OpenVisionPipelineReviewReadinessLevel.Check);
            int advisoryCount = items.Count(item => item.Level == OpenVisionPipelineReviewReadinessLevel.Advisory);
            return new OpenVisionPipelineReviewReadinessState(
                items.AsReadOnly(),
                ResolveSummaryText(checkCount, advisoryCount));
        }

        private static OpenVisionPipelineReviewReadinessItem CreateInputItem(
            int enabledStepCount,
            int externalInputCount,
            IReadOnlyList<string> missingInputs)
        {
            if (enabledStepCount == 0)
            {
                return Item(
                    "Input",
                    "PipelineReview.Readiness.Input",
                    "Input image",
                    "PipelineReview.Readiness.NoSteps",
                    "No enabled steps",
                    OpenVisionPipelineReviewReadinessLevel.Check);
            }

            if (missingInputs.Count > 0)
            {
                return Item(
                    "Input",
                    "PipelineReview.Readiness.Input",
                    "Input image",
                    TF(
                        "PipelineReview.Readiness.MissingInputFormat",
                        "Missing: {0}",
                        string.Join(", ", missingInputs.Take(2))),
                    OpenVisionPipelineReviewReadinessLevel.Check);
            }

            return Item(
                "Input",
                "PipelineReview.Readiness.Input",
                "Input image",
                TF(
                    "PipelineReview.Readiness.InputReadyFormat",
                    "{0} source input(s) ready",
                    externalInputCount),
                OpenVisionPipelineReviewReadinessLevel.Ready);
        }

        private static OpenVisionPipelineReviewReadinessItem CreateRouteItem(
            int enabledStepCount,
            VisionPipelineValidationResult validationResult)
        {
            int errorCount = validationResult?.Errors?.Count ?? 0;
            if (errorCount > 0)
            {
                return Item(
                    "Route",
                    "PipelineReview.Readiness.Route",
                    "Steps / routes",
                    TF("PipelineReview.Readiness.ErrorsFormat", "{0} error(s)", errorCount),
                    OpenVisionPipelineReviewReadinessLevel.Check);
            }

            int warningCount = validationResult?.Warnings?.Count ?? 0;
            if (warningCount > 0)
            {
                return Item(
                    "Route",
                    "PipelineReview.Readiness.Route",
                    "Steps / routes",
                    TF("PipelineReview.Readiness.WarningsFormat", "Review {0} warning(s)", warningCount),
                    OpenVisionPipelineReviewReadinessLevel.Advisory);
            }

            return Item(
                "Route",
                "PipelineReview.Readiness.Route",
                "Steps / routes",
                TF("PipelineReview.Readiness.StepsReadyFormat", "{0} step(s) connected", enabledStepCount),
                OpenVisionPipelineReviewReadinessLevel.Ready);
        }

        private static OpenVisionPipelineReviewReadinessItem CreateAcceptanceItem(
            IReadOnlyCollection<VisionPipelineStep> enabledSteps)
        {
            int acceptanceCount = enabledSteps.Count(step => step.UseAcceptance);
            return acceptanceCount > 0
                ? Item(
                    "Acceptance",
                    "PipelineReview.Readiness.Acceptance",
                    "Acceptance",
                    TF("PipelineReview.Readiness.AcceptanceReadyFormat", "{0} judgment step(s)", acceptanceCount),
                    OpenVisionPipelineReviewReadinessLevel.Ready)
                : Item(
                    "Acceptance",
                    "PipelineReview.Readiness.Acceptance",
                    "Acceptance",
                    "PipelineReview.Readiness.AcceptanceMissing",
                    "No OK/NG criterion",
                    OpenVisionPipelineReviewReadinessLevel.Check);
        }

        private static OpenVisionPipelineReviewReadinessItem CreateGoodBadItem(
            bool hasGoodBadGuide,
            bool hasGoodBadCounterpart)
        {
            return hasGoodBadGuide && hasGoodBadCounterpart
                ? Item(
                    "GoodBad",
                    "PipelineReview.Readiness.GoodBad",
                    "Good / Bad evidence",
                    "PipelineReview.Readiness.GoodBadReady",
                    "Comparison pair ready",
                    OpenVisionPipelineReviewReadinessLevel.Ready)
                : Item(
                    "GoodBad",
                    "PipelineReview.Readiness.GoodBad",
                    "Good / Bad evidence",
                    "PipelineReview.Readiness.GoodBadMissing",
                    "Add a comparison pair",
                    OpenVisionPipelineReviewReadinessLevel.Advisory);
        }

        private static OpenVisionPipelineReviewReadinessItem CreateCalibrationItem(
            IReadOnlyCollection<VisionPipelineStep> enabledSteps)
        {
            List<VisionPipelineStep> calibratedSteps = enabledSteps
                .Where(UsesMillimeterAcceptance)
                .ToList();
            if (calibratedSteps.Count == 0)
            {
                return Item(
                    "Calibration",
                    "PipelineReview.Readiness.Calibration",
                    "Unit calibration",
                    "PipelineReview.Readiness.CalibrationNotApplicable",
                    "Pixel or unitless result",
                    OpenVisionPipelineReviewReadinessLevel.NotApplicable);
            }

            List<double> values = new List<double>();
            int missingCount = 0;
            foreach (VisionPipelineStep step in calibratedSteps)
            {
                if (TryGetPositiveCalibration(step, out double value))
                {
                    values.Add(value);
                }
                else
                {
                    missingCount++;
                }
            }

            if (missingCount > 0)
            {
                return Item(
                    "Calibration",
                    "PipelineReview.Readiness.Calibration",
                    "Unit calibration",
                    TF("PipelineReview.Readiness.CalibrationMissingFormat", "Check {0} step(s)", missingCount),
                    OpenVisionPipelineReviewReadinessLevel.Check);
            }

            int distinctCount = values
                .Select(value => value.ToString("R", CultureInfo.InvariantCulture))
                .Distinct(StringComparer.Ordinal)
                .Count();
            if (distinctCount > 1)
            {
                return Item(
                    "Calibration",
                    "PipelineReview.Readiness.Calibration",
                    "Unit calibration",
                    TF("PipelineReview.Readiness.CalibrationMultipleFormat", "Review {0} values", distinctCount),
                    OpenVisionPipelineReviewReadinessLevel.Advisory);
            }

            return Item(
                "Calibration",
                "PipelineReview.Readiness.Calibration",
                "Unit calibration",
                TF(
                    "PipelineReview.Readiness.CalibrationReferenceCheckFormat",
                    "Verify ref {0} mm/px",
                    values[0].ToString("0.######", CultureInfo.CurrentCulture)),
                OpenVisionPipelineReviewReadinessLevel.Advisory);
        }

        private static List<string> ResolveExternalInputs(IReadOnlyCollection<VisionPipelineStep> enabledSteps)
        {
            HashSet<string> producedLayers = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            List<string> externalInputs = new List<string>();
            foreach (VisionPipelineStep step in enabledSteps)
            {
                string inputLayer = step.InputLayer?.Trim();
                if (!string.IsNullOrWhiteSpace(inputLayer)
                    && !producedLayers.Contains(inputLayer)
                    && !externalInputs.Contains(inputLayer, StringComparer.OrdinalIgnoreCase))
                {
                    externalInputs.Add(inputLayer);
                }

                string outputLayer = step.OutputLayer?.Trim();
                if (!string.IsNullOrWhiteSpace(outputLayer))
                {
                    producedLayers.Add(outputLayer);
                }
            }

            return externalInputs;
        }

        private static bool UsesMillimeterAcceptance(VisionPipelineStep step)
        {
            return step?.UseAcceptance == true
                && (step.AcceptanceMetricName ?? string.Empty).IndexOf("Mm", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static bool TryGetPositiveCalibration(VisionPipelineStep step, out double value)
        {
            value = 0d;
            string rawValue = FindParameter(step, "PIXELPERMM");
            return !string.IsNullOrWhiteSpace(rawValue)
                && double.TryParse(rawValue, NumberStyles.Float, CultureInfo.InvariantCulture, out value)
                && !double.IsNaN(value)
                && !double.IsInfinity(value)
                && value > 0d;
        }

        private static string FindParameter(VisionPipelineStep step, string key)
        {
            return step?.Parameters?
                .FirstOrDefault(parameter => string.Equals(parameter.Key, key, StringComparison.OrdinalIgnoreCase))
                .Value;
        }

        private static string ResolveSummaryText(int checkCount, int advisoryCount)
        {
            if (checkCount > 0)
            {
                return advisoryCount > 0
                    ? TF("PipelineReview.Readiness.SummaryChecksAndAdviceFormat", "Check {0} / recommended {1}", checkCount, advisoryCount)
                    : TF("PipelineReview.Readiness.SummaryChecksFormat", "Check {0} before review", checkCount);
            }

            return advisoryCount > 0
                ? TF("PipelineReview.Readiness.SummaryAdviceFormat", "Runnable / recommended {0}", advisoryCount)
                : T("PipelineReview.Readiness.SummaryReady", "Ready to run review");
        }

        private static OpenVisionPipelineReviewReadinessItem Item(
            string key,
            string titleKey,
            string titleFallback,
            string statusKey,
            string statusFallback,
            OpenVisionPipelineReviewReadinessLevel level)
        {
            return new OpenVisionPipelineReviewReadinessItem(
                key,
                T(titleKey, titleFallback),
                T(statusKey, statusFallback),
                level);
        }

        private static OpenVisionPipelineReviewReadinessItem Item(
            string key,
            string titleKey,
            string titleFallback,
            string statusText,
            OpenVisionPipelineReviewReadinessLevel level)
        {
            return new OpenVisionPipelineReviewReadinessItem(
                key,
                T(titleKey, titleFallback),
                statusText,
                level);
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
    }
}
