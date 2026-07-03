using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace OpenVisionLab
{
    internal sealed class OpenVisionWorkspaceSamplePairDecisionGuide
    {
        public static readonly OpenVisionWorkspaceSamplePairDecisionGuide Empty =
            new OpenVisionWorkspaceSamplePairDecisionGuide(false, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);

        public OpenVisionWorkspaceSamplePairDecisionGuide(
            bool hasGuide,
            string summaryText,
            string metricText,
            string checklistText,
            string nextActionText,
            string workflowText,
            string pairReviewText)
        {
            HasGuide = hasGuide;
            SummaryText = summaryText ?? string.Empty;
            MetricText = metricText ?? string.Empty;
            ChecklistText = checklistText ?? string.Empty;
            NextActionText = nextActionText ?? string.Empty;
            WorkflowText = workflowText ?? string.Empty;
            PairReviewText = pairReviewText ?? string.Empty;
        }

        public bool HasGuide { get; }
        public string SummaryText { get; }
        public string MetricText { get; }
        public string ChecklistText { get; }
        public string NextActionText { get; }
        public string WorkflowText { get; }
        public string PairReviewText { get; }
    }

    internal static class OpenVisionWorkspaceSamplePairDecisionGuidePresenter
    {
        public static OpenVisionWorkspaceSamplePairDecisionGuide Create(
            VisionPipelineSampleCatalogItem selectedSample,
            IReadOnlyList<VisionPipelineSampleCatalogItem> pairSamples)
        {
            if (selectedSample == null || pairSamples == null || pairSamples.Count <= 1)
            {
                return OpenVisionWorkspaceSamplePairDecisionGuide.Empty;
            }

            List<VisionPipelineSampleCatalogItem> counterparts = ResolveCounterparts(selectedSample, pairSamples);
            if (counterparts.Count == 0)
            {
                return OpenVisionWorkspaceSamplePairDecisionGuide.Empty;
            }

            string group = string.IsNullOrWhiteSpace(selectedSample.PairGroup)
                ? LocalText("현재 쌍", "this pair")
                : selectedSample.PairGroup.Trim();
            string selectedRole = FormatRole(selectedSample);
            string counterpartRoles = string.Join(", ", counterparts.Take(2).Select(FormatRoleAndName));

            string summary = string.Format(
                CultureInfo.CurrentCulture,
                LocalText(
                    "선택 샘플은 {0} 기준입니다. {1}에서는 OK 샘플이 허용 범위 안에 들어오고 NG 샘플은 같은 metric에서 분리되어야 합니다.",
                    "The selected sample is the {0} reference. In {1}, OK samples should stay inside the accepted range and NG samples should separate on the same metrics."),
                selectedRole,
                group);

            string metric = BuildMetricSeparationText(selectedSample, counterparts);
            string checklist = BuildValidationChecklistText(selectedSample, counterparts);
            string nextAction = BuildNextActionText(selectedSample, counterparts);
            string pairReview = BuildPairReviewText(selectedSample, counterparts);
            string workflow = string.Format(
                CultureInfo.CurrentCulture,
                LocalText(
                    "검증 순서: OK 샘플을 먼저 Preview/Run으로 확인하고, 같은 파이프라인으로 {0}을 실행해 분리 margin을 확인하세요. 이 안내는 표시 전용이며 자동 실행하지 않습니다.",
                    "Review order: run Preview/Run on the OK sample first, then run the same pipeline on {0} and check the separation margin. This guide is display-only and does not auto-run."),
                counterpartRoles);

            return new OpenVisionWorkspaceSamplePairDecisionGuide(true, summary, metric, checklist, nextAction, workflow, pairReview);
        }

        private static List<VisionPipelineSampleCatalogItem> ResolveCounterparts(
            VisionPipelineSampleCatalogItem selectedSample,
            IReadOnlyList<VisionPipelineSampleCatalogItem> pairSamples)
        {
            bool selectedIsOk = IsOkReference(selectedSample);
            bool selectedIsNg = IsNgReference(selectedSample);
            return pairSamples
                .Where(item => item != null && !ReferenceEquals(item, selectedSample))
                .Where(item =>
                    selectedIsOk
                        ? IsNgReference(item)
                        : selectedIsNg
                            ? IsOkReference(item)
                            : true)
                .OrderBy(item => IsOkReference(item) ? 0 : 1)
                .ThenBy(item => item.SampleName, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        private static string BuildMetricSeparationText(
            VisionPipelineSampleCatalogItem selectedSample,
            IReadOnlyList<VisionPipelineSampleCatalogItem> counterparts)
        {
            List<string> comparisons = new List<string>();
            foreach (VisionPipelineSampleCatalogItem counterpart in counterparts.Take(2))
            {
                Dictionary<string, VisionPipelineSampleExpectedMetric> selectedMetrics = selectedSample.ExpectedMetrics
                    .Where(metric => metric != null && !string.IsNullOrWhiteSpace(metric.Name))
                    .GroupBy(metric => metric.Name.Trim(), StringComparer.OrdinalIgnoreCase)
                    .ToDictionary(group => group.Key, group => group.First(), StringComparer.OrdinalIgnoreCase);
                Dictionary<string, VisionPipelineSampleExpectedMetric> counterpartMetrics = counterpart.ExpectedMetrics
                    .Where(metric => metric != null && !string.IsNullOrWhiteSpace(metric.Name))
                    .GroupBy(metric => metric.Name.Trim(), StringComparer.OrdinalIgnoreCase)
                    .ToDictionary(group => group.Key, group => group.First(), StringComparer.OrdinalIgnoreCase);

                List<string> commonNames = selectedMetrics.Keys
                    .Where(counterpartMetrics.ContainsKey)
                    .Take(3)
                    .ToList();

                if (commonNames.Count == 0)
                {
                    comparisons.Add(FormatReferenceRange(selectedSample) + " / " + FormatReferenceRange(counterpart));
                    continue;
                }

                foreach (string metricName in commonNames)
                {
                    comparisons.Add(string.Format(
                        CultureInfo.CurrentCulture,
                        "{0}: {1} {2} / {3} {4}",
                        metricName,
                        FormatRole(selectedSample),
                        FormatMetricRange(selectedMetrics[metricName]),
                        FormatRole(counterpart),
                        FormatMetricRange(counterpartMetrics[metricName])));
                }
            }

            if (comparisons.Count == 0)
            {
                return LocalText(
                    "분리 metric: 선택 샘플과 반대 샘플의 기대 metric을 Pipeline Review에서 비교하세요.",
                    "Separating metrics: compare the selected and opposite reference metrics in Pipeline Review.");
            }

            return LocalText("분리 metric: ", "Separating metrics: ") + string.Join("; ", comparisons);
        }

        private static string BuildValidationChecklistText(
            VisionPipelineSampleCatalogItem selectedSample,
            IReadOnlyList<VisionPipelineSampleCatalogItem> counterparts)
        {
            List<string> metricNames = ResolveDecisionMetricNames(selectedSample, counterparts);
            string metrics = metricNames.Count == 0
                ? LocalText("\uacb0\uacfc metric", "result metrics")
                : string.Join(", ", metricNames.Take(4));
            string okReference = FormatReferenceList(new[] { selectedSample }.Concat(counterparts).Where(IsOkReference));
            string ngReference = FormatReferenceList(new[] { selectedSample }.Concat(counterparts).Where(IsNgReference));

            if (string.IsNullOrWhiteSpace(okReference))
            {
                okReference = LocalText("OK \uae30\uc900 \uc0d8\ud50c", "the OK reference");
            }

            if (string.IsNullOrWhiteSpace(ngReference))
            {
                ngReference = LocalText("NG \uae30\uc900 \uc0d8\ud50c", "the NG reference");
            }

            return string.Format(
                CultureInfo.CurrentCulture,
                LocalText(
                    "\uac80\uc99d \uccb4\ud06c: OK \uae30\uc900({0}) \uae30\ub85d -> \uac19\uc740 \ud30c\uc774\ud504\ub77c\uc778\uc73c\ub85c NG \uae30\uc900({1}) \uc2e4\ud589 -> {2} \ubd84\ub9ac \ud655\uc778 -> OK \uc2e4\ud328\ub294 \uc785\ub825/ROI/\ud15c\ud50c\ub9bf, NG \ud1b5\uacfc\ub294 \uae30\uc900\uac12 \uc870\uc815",
                    "Validation checklist: OK reference ({0}) -> run the same pipeline on NG reference ({1}) -> confirm {2} separation -> fix input/ROI/template if OK fails, tighten limits if NG passes."),
                okReference,
                ngReference,
                metrics);
        }

        private static string BuildNextActionText(
            VisionPipelineSampleCatalogItem selectedSample,
            IReadOnlyList<VisionPipelineSampleCatalogItem> counterparts)
        {
            List<string> metricNames = ResolveDecisionMetricNames(selectedSample, counterparts);
            string metrics = metricNames.Count == 0
                ? LocalText("\uacb0\uacfc metric", "result metrics")
                : string.Join(", ", metricNames.Take(4));
            string counterpartReference = FormatReferenceList(counterparts);
            if (string.IsNullOrWhiteSpace(counterpartReference))
            {
                counterpartReference = LocalText("\ubc18\ub300 \uae30\uc900 \uc0d8\ud50c", "the opposite reference");
            }

            if (IsOkReference(selectedSample))
            {
                return string.Format(
                    CultureInfo.CurrentCulture,
                    LocalText(
                        "\ub2e4\uc74c \ud589\ub3d9: \uac19\uc740 PairGroup\uc758 NG \uae30\uc900({0})\uc744 \uac19\uc740 Pipeline\uc73c\ub85c \uc2e4\ud589\ud558\uace0 {1} \ubd84\ub9ac \uc5ec\ubd80\ub97c \ud655\uc778\ud569\ub2c8\ub2e4.",
                        "Next action: run the NG reference ({0}) in the same PairGroup with the same pipeline, then confirm {1} separation."),
                    counterpartReference,
                    metrics);
            }

            if (IsNgReference(selectedSample))
            {
                return string.Format(
                    CultureInfo.CurrentCulture,
                    LocalText(
                        "\ub2e4\uc74c \ud589\ub3d9: OK \uae30\uc900({0})\uc744 \uba3c\uc800 \ud655\uc778\ud55c \ub4a4 \uc774 NG \uc0d8\ud50c\uc744 \uac19\uc740 Pipeline\uc73c\ub85c \ub2e4\uc2dc \uc2e4\ud589\ud574 {1} \ubd84\ub9ac \uc5ec\ubd80\ub97c \ud655\uc778\ud569\ub2c8\ub2e4.",
                        "Next action: verify the OK reference ({0}) first, then rerun this NG sample with the same pipeline and confirm {1} separation."),
                    counterpartReference,
                    metrics);
            }

            return LocalText(
                "\ub2e4\uc74c \ud589\ub3d9: OK\uc640 NG \uae30\uc900\uc744 \uac19\uc740 Pipeline\uc73c\ub85c \uc2e4\ud589\ud558\uace0 \uacb0\uacfc \uc774\ubbf8\uc9c0, overlay, metric, log\ub97c \ud568\uaed8 \ube44\uad50\ud569\ub2c8\ub2e4.",
                "Next action: run OK and NG references with the same pipeline, then compare result image, overlay, metric, and log together.");
        }

        private static string BuildPairReviewText(
            VisionPipelineSampleCatalogItem selectedSample,
            IReadOnlyList<VisionPipelineSampleCatalogItem> counterparts)
        {
            string selectedReference = FormatRoleAndName(selectedSample);
            string counterpartReference = FormatReferenceList(counterparts);
            if (string.IsNullOrWhiteSpace(counterpartReference))
            {
                counterpartReference = LocalText("\ubc18\ub300 \uae30\uc900 \uc0d8\ud50c", "the opposite reference");
            }

            List<string> metricNames = ResolveDecisionMetricNames(selectedSample, counterparts);
            string metrics = metricNames.Count == 0
                ? LocalText("\uacb0\uacfc metric", "result metrics")
                : string.Join(", ", metricNames.Take(4));
            string group = string.IsNullOrWhiteSpace(selectedSample.PairGroup)
                ? LocalText("\ud604\uc7ac \uc30d", "this pair")
                : selectedSample.PairGroup.Trim();

            return string.Format(
                CultureInfo.CurrentCulture,
                LocalText(
                    "Good/Bad \uc30d: {0} / \ubc18\ub300 \uae30\uc900 {1} / PairGroup {2} / \ube44\uad50 metric {3}",
                    "Good/Bad pair: {0} / opposite reference {1} / PairGroup {2} / compare metric {3}"),
                selectedReference,
                counterpartReference,
                group,
                metrics);
        }

        private static List<string> ResolveDecisionMetricNames(
            VisionPipelineSampleCatalogItem selectedSample,
            IReadOnlyList<VisionPipelineSampleCatalogItem> counterparts)
        {
            HashSet<string> selectedMetricNames = new HashSet<string>(
                selectedSample.ExpectedMetrics
                    .Where(metric => metric != null && !string.IsNullOrWhiteSpace(metric.Name))
                    .Select(metric => metric.Name.Trim()),
                StringComparer.OrdinalIgnoreCase);
            if (selectedMetricNames.Count == 0)
            {
                return new List<string>();
            }

            HashSet<string> counterpartMetricNames = new HashSet<string>(
                counterparts
                    .Where(item => item != null)
                    .SelectMany(item => item.ExpectedMetrics)
                    .Where(metric => metric != null && !string.IsNullOrWhiteSpace(metric.Name))
                    .Select(metric => metric.Name.Trim()),
                StringComparer.OrdinalIgnoreCase);

            List<string> commonNames = selectedMetricNames
                .Where(counterpartMetricNames.Contains)
                .OrderBy(name => name, StringComparer.OrdinalIgnoreCase)
                .ToList();
            return commonNames.Count > 0
                ? commonNames
                : selectedMetricNames.OrderBy(name => name, StringComparer.OrdinalIgnoreCase).ToList();
        }

        private static string FormatReferenceList(IEnumerable<VisionPipelineSampleCatalogItem> samples)
        {
            List<string> names = samples
                .Where(item => item != null)
                .Select(FormatRoleAndName)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Take(3)
                .ToList();
            return names.Count == 0 ? string.Empty : string.Join(", ", names);
        }

        private static string FormatReferenceRange(VisionPipelineSampleCatalogItem sample)
        {
            return string.Format(
                CultureInfo.CurrentCulture,
                "{0} {1}",
                FormatRole(sample),
                string.IsNullOrWhiteSpace(sample.ExpectedText) ? "-" : sample.ExpectedText);
        }

        private static string FormatMetricRange(VisionPipelineSampleExpectedMetric metric)
        {
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

        private static string FormatRoleAndName(VisionPipelineSampleCatalogItem item)
        {
            return FormatRole(item) + " " + item.SampleName;
        }

        private static string FormatRole(VisionPipelineSampleCatalogItem item)
        {
            if (IsNgReference(item))
            {
                return LocalText("NG", "NG");
            }

            if (IsOkReference(item))
            {
                return LocalText("OK", "OK");
            }

            return LocalText("참조", "Reference");
        }

        private static bool IsOkReference(VisionPipelineSampleCatalogItem item)
        {
            return item != null
                && !item.ExpectsFailure
                && string.Equals(item.PairRole?.Trim(), "Good", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsNgReference(VisionPipelineSampleCatalogItem item)
        {
            return item != null
                && (item.ExpectsFailure
                    || string.Equals(item.PairRole?.Trim(), "Bad", StringComparison.OrdinalIgnoreCase));
        }

        private static string LocalText(string korean, string english)
        {
            return OpenVisionLanguageService.CurrentLanguage == OpenVisionLanguage.English
                ? english ?? korean ?? string.Empty
                : korean ?? english ?? string.Empty;
        }
    }
}
