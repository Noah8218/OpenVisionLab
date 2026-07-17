using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace OpenVisionLab
{
    internal sealed class OpenVisionRecipeOperatorDecisionRequest
    {
        internal OpenVisionRecipeOperatorDecisionRequest(
            OpenVisionRecipeManagerSummary summary,
            OpenVisionRecipeSampleRunSummary sample,
            OpenVisionRecipePairRunSummary pair,
            OpenVisionRecipeCatalogBenchmarkSummary catalog,
            IReadOnlyList<OpenVisionRecipeBatchRunComparisonRow> comparisonRows,
            string comparisonSummaryText,
            OpenVisionRecipeSampleMatrixRow selectedMatrix,
            OpenVisionRecipePairSampleRunSummary selectedRole,
            OpenVisionRecipeBatchSampleResultOption selectedBatchSample,
            OpenVisionRecipeBatchRunComparisonRow selectedBatchComparison,
            string selectedSampleExpectedText,
            OpenVisionRecipePipelineStepPreview evidenceStep,
            OpenVisionRecipePipelineStepPreview handoffStep)
        {
            Summary = summary;
            Sample = sample;
            Pair = pair;
            Catalog = catalog;
            ComparisonRows = comparisonRows;
            ComparisonSummaryText = comparisonSummaryText ?? string.Empty;
            SelectedMatrix = selectedMatrix;
            SelectedRole = selectedRole;
            SelectedBatchSample = selectedBatchSample;
            SelectedBatchComparison = selectedBatchComparison;
            SelectedSampleExpectedText = selectedSampleExpectedText ?? string.Empty;
            EvidenceStep = evidenceStep;
            HandoffStep = handoffStep;
        }

        internal OpenVisionRecipeManagerSummary Summary { get; }

        internal OpenVisionRecipeSampleRunSummary Sample { get; }

        internal OpenVisionRecipePairRunSummary Pair { get; }

        internal OpenVisionRecipeCatalogBenchmarkSummary Catalog { get; }

        internal IReadOnlyList<OpenVisionRecipeBatchRunComparisonRow> ComparisonRows { get; }

        internal string ComparisonSummaryText { get; }

        internal OpenVisionRecipeSampleMatrixRow SelectedMatrix { get; }

        internal OpenVisionRecipePairSampleRunSummary SelectedRole { get; }

        internal OpenVisionRecipeBatchSampleResultOption SelectedBatchSample { get; }

        internal OpenVisionRecipeBatchRunComparisonRow SelectedBatchComparison { get; }

        internal string SelectedSampleExpectedText { get; }

        internal OpenVisionRecipePipelineStepPreview EvidenceStep { get; }

        internal OpenVisionRecipePipelineStepPreview HandoffStep { get; }
    }

    internal sealed class OpenVisionRecipeOperatorDecisionPresentation
    {
        internal OpenVisionRecipeOperatorDecisionPresentation(
            string xmlCardText,
            string sampleCardText,
            string pairCardText,
            string summaryStatusText,
            string nextActionText,
            string evidenceText,
            IReadOnlyList<OpenVisionRecipeOperatorValidationRow> validationRows,
            IReadOnlyList<OpenVisionRecipeOperatorResultChannelRow> resultChannels,
            string handoffReportText)
        {
            XmlCardText = xmlCardText ?? string.Empty;
            SampleCardText = sampleCardText ?? string.Empty;
            PairCardText = pairCardText ?? string.Empty;
            SummaryStatusText = summaryStatusText ?? string.Empty;
            NextActionText = nextActionText ?? string.Empty;
            EvidenceText = evidenceText ?? string.Empty;
            ValidationRows = validationRows ?? Array.Empty<OpenVisionRecipeOperatorValidationRow>();
            ResultChannels = resultChannels ?? Array.Empty<OpenVisionRecipeOperatorResultChannelRow>();
            HandoffReportText = handoffReportText ?? string.Empty;
        }

        internal string XmlCardText { get; }

        internal string SampleCardText { get; }

        internal string PairCardText { get; }

        internal string SummaryStatusText { get; }

        internal string NextActionText { get; }

        internal string EvidenceText { get; }

        internal IReadOnlyList<OpenVisionRecipeOperatorValidationRow> ValidationRows { get; }

        internal IReadOnlyList<OpenVisionRecipeOperatorResultChannelRow> ResultChannels { get; }

        internal string HandoffReportText { get; }
    }

    // Derives operator-facing decision-board and handoff text without owning Host selection or commands.
    internal static class OpenVisionRecipeOperatorDecisionPresenter
    {
        internal static string ResolveEvidenceFailedStepName(
            OpenVisionRecipePairSampleRunSummary selectedRole,
            OpenVisionRecipeSampleMatrixRow selectedMatrix,
            OpenVisionRecipeBatchSampleResultOption selectedBatchSample,
            OpenVisionRecipeBatchRunComparisonRow selectedBatchComparison)
        {
            return FirstNonEmpty(
                selectedRole?.FailedStepText,
                selectedMatrix?.FailedStep,
                selectedBatchSample?.FailedStep,
                selectedBatchComparison?.FailedStep);
        }

        internal static OpenVisionRecipeOperatorDecisionPresentation Build(OpenVisionRecipeOperatorDecisionRequest request)
        {
            OpenVisionRecipeManagerSummary summary = request?.Summary ?? OpenVisionRecipeManagerSummary.Empty;
            OpenVisionRecipeSampleRunSummary sample = request?.Sample ?? OpenVisionRecipeSampleRunSummary.Empty;
            OpenVisionRecipePairRunSummary pair = request?.Pair ?? OpenVisionRecipePairRunSummary.Empty;
            OpenVisionRecipeCatalogBenchmarkSummary catalog = request?.Catalog ?? OpenVisionRecipeCatalogBenchmarkSummary.Empty;
            IReadOnlyList<OpenVisionRecipeBatchRunComparisonRow> comparisonRows =
                request?.ComparisonRows ?? Array.Empty<OpenVisionRecipeBatchRunComparisonRow>();
            IReadOnlyList<OpenVisionRecipeOperatorValidationRow> validationRows = BuildValidationRows(
                summary,
                sample,
                pair,
                catalog,
                comparisonRows,
                request?.ComparisonSummaryText ?? string.Empty);
            IReadOnlyList<OpenVisionRecipeOperatorResultChannelRow> resultChannels = BuildResultChannels(
                summary,
                sample,
                pair,
                comparisonRows,
                request?.SelectedRole,
                request?.SelectedBatchComparison,
                request?.SelectedBatchSample);
            string evidenceText = BuildEvidenceText(
                request?.SelectedMatrix,
                request?.SelectedRole,
                sample,
                request?.SelectedBatchSample,
                request?.SelectedSampleExpectedText,
                request?.EvidenceStep);
            string nextActionText = OpenVisionRecipeText.Local("다음 작업: ", "Next action: ")
                + OpenVisionRecipeRunReviewPresenter.BuildNextAction(summary, sample, pair);

            return new OpenVisionRecipeOperatorDecisionPresentation(
                BuildXmlCardText(summary),
                BuildSampleCardText(sample),
                BuildPairCardText(pair),
                BuildSummaryStatusText(resultChannels),
                nextActionText,
                evidenceText,
                validationRows,
                resultChannels,
                BuildHandoffReportText(
                    summary,
                    sample,
                    pair,
                    evidenceText,
                    validationRows,
                    resultChannels,
                    request?.SelectedRole,
                    request?.HandoffStep));
        }

        private static string BuildXmlCardText(OpenVisionRecipeManagerSummary summary)
        {
            string state = summary.XmlValid && summary.StepCount > 0
                ? OpenVisionRecipeText.Local("준비", "Ready")
                : OpenVisionRecipeText.Local("조치 필요", "Needs action");
            return OpenVisionRecipeText.Local("XML/Step", "XML/Steps")
                + Environment.NewLine
                + state
                + " | "
                + summary.XmlStatusDisplay
                + " | "
                + summary.StepCount.ToString(CultureInfo.InvariantCulture)
                + " Step";
        }

        private static string BuildSampleCardText(OpenVisionRecipeSampleRunSummary sample)
        {
            string state = !sample.HasResult
                ? OpenVisionRecipeText.Local("미실행", "Not run")
                : (sample.Succeeded ? "OK" : "NG");
            return OpenVisionRecipeText.Local("선택 샘플", "Selected sample")
                + Environment.NewLine
                + state
                + " | "
                + sample.CompactText;
        }

        private static string BuildPairCardText(OpenVisionRecipePairRunSummary pair)
        {
            string state = !pair.HasResult
                ? OpenVisionRecipeText.Local("미실행", "Not run")
                : (pair.Succeeded ? "OK" : "NG");
            return "Good/Bad"
                + Environment.NewLine
                + state
                + " | "
                + pair.CompactText;
        }

        private static string BuildSummaryStatusText(IReadOnlyList<OpenVisionRecipeOperatorResultChannelRow> resultChannels)
        {
            OpenVisionRecipeOperatorResultChannelRow status = resultChannels.FirstOrDefault(row =>
                row.ChannelText.IndexOf("Inspection.Status", StringComparison.OrdinalIgnoreCase) >= 0);
            OpenVisionRecipeOperatorResultChannelRow failedStep = resultChannels.FirstOrDefault(row =>
                row.ChannelText.IndexOf("Inspection.FailedStep", StringComparison.OrdinalIgnoreCase) >= 0);

            return (status?.ChannelText ?? "Inspection.Status")
                + ": "
                + (status?.ValueText ?? "WAIT")
                + " ("
                + (status?.SourceText ?? "-")
                + ") | "
                + (failedStep?.ChannelText ?? "Inspection.FailedStep")
                + ": "
                + (failedStep?.ValueText ?? "-");
        }

        private static string BuildEvidenceText(
            OpenVisionRecipeSampleMatrixRow selectedMatrix,
            OpenVisionRecipePairSampleRunSummary selectedRole,
            OpenVisionRecipeSampleRunSummary sample,
            OpenVisionRecipeBatchSampleResultOption selectedBatchSample,
            string selectedSampleExpectedText,
            OpenVisionRecipePipelineStepPreview evidenceStep)
        {
            string expected = FirstNonEmpty(selectedMatrix?.ExpectedText, selectedSampleExpectedText, "-");
            string actual = FirstNonEmpty(
                selectedRole?.MetricText,
                selectedMatrix?.MetricText,
                sample.DistanceMetricText,
                sample.CompactText,
                selectedBatchSample?.DetailText,
                "-");
            string family = ResolveParameterFamily(evidenceStep?.ToolType, actual);
            string evidenceRoute = evidenceStep == null ? "-" : evidenceStep.InputLayer + " -> " + evidenceStep.OutputLayer;

            return "Metric review: expected "
                + ShortEvidence(expected)
                + " / actual "
                + ShortEvidence(actual)
                + " / inspect "
                + family
                + " / evidence "
                + ShortEvidence(evidenceRoute);
        }

        private static IReadOnlyList<OpenVisionRecipeOperatorValidationRow> BuildValidationRows(
            OpenVisionRecipeManagerSummary summary,
            OpenVisionRecipeSampleRunSummary sample,
            OpenVisionRecipePairRunSummary pair,
            OpenVisionRecipeCatalogBenchmarkSummary catalog,
            IReadOnlyList<OpenVisionRecipeBatchRunComparisonRow> comparisonRows,
            string comparisonSummaryText)
        {
            List<OpenVisionRecipeOperatorValidationRow> rows = new List<OpenVisionRecipeOperatorValidationRow>
            {
                OpenVisionRecipeOperatorValidationRow.Create(
                    OpenVisionRecipeText.Local("XML/Step", "XML/Steps"),
                    summary.XmlValid && summary.StepCount > 0 ? "OK" : "NG",
                    summary.XmlStatusDisplay + " / " + summary.StepCount.ToString(CultureInfo.InvariantCulture) + " Step",
                    summary.XmlValid && summary.StepCount > 0
                        ? OpenVisionRecipeText.Local("샘플 검증으로 진행", "Proceed to sample validation")
                        : OpenVisionRecipeText.Local("LLM XML 검증 보고서와 Step 구성을 먼저 수정", "Fix the LLM XML validation report and step structure first")),
                OpenVisionRecipeOperatorValidationRow.Create(
                    OpenVisionRecipeText.Local("선택 샘플", "Selected sample"),
                    !sample.HasResult ? "WAIT" : (sample.Succeeded ? "OK" : "NG"),
                    sample.CompactText,
                    !sample.HasResult
                        ? OpenVisionRecipeText.Local("검사 실행", "Run check")
                        : (sample.Succeeded
                            ? OpenVisionRecipeText.Local("Good/Bad 쌍 검증 진행", "Proceed to Good/Bad pair validation")
                            : OpenVisionRecipeText.Local("실패 Step 입력/출력과 파라미터 확인", "Review failed step input/output and parameters"))),
                OpenVisionRecipeOperatorValidationRow.Create(
                    "Good/Bad",
                    !pair.HasResult ? "WAIT" : (pair.Succeeded ? "OK" : "NG"),
                    pair.CompactText,
                    !pair.HasResult
                        ? OpenVisionRecipeText.Local("쌍 검사 실행", "Run pair check")
                        : (pair.Succeeded
                            ? OpenVisionRecipeText.Local("카탈로그 또는 이력 비교로 확장", "Expand to catalog or run-history comparison")
                            : OpenVisionRecipeText.Local("NG 역할을 선택하고 실패 Step 조정", "Select the NG role and tune the failed step"))),
                OpenVisionRecipeOperatorValidationRow.Create(
                    OpenVisionRecipeText.Local("카탈로그", "Catalog"),
                    !catalog.HasResult ? "WAIT" : (catalog.Succeeded ? "OK" : "NG"),
                    catalog.CompactText,
                    !catalog.HasResult
                        ? OpenVisionRecipeText.Local("전체 샘플 검사 실행", "Run catalog benchmark")
                        : (catalog.Succeeded
                            ? OpenVisionRecipeText.Local("결과 고정 가능", "Ready to keep result")
                            : OpenVisionRecipeText.Local("실패 샘플 우선 재검토", "Review failing samples first")))
            };

            int comparable = comparisonRows.Count(row => row != null && row.IsComparable);
            int regression = comparisonRows.Count(row => row != null && row.IsRegression);
            string benchmarkState = comparable == 0 ? "WAIT" : (regression == 0 ? "OK" : "NG");
            rows.Add(OpenVisionRecipeOperatorValidationRow.Create(
                OpenVisionRecipeText.Local("회귀 비교", "Regression diff"),
                benchmarkState,
                comparisonSummaryText,
                comparable == 0
                    ? OpenVisionRecipeText.Local("이전 benchmark 기준 실행 확보", "Create or select a baseline benchmark run")
                    : (regression == 0
                        ? OpenVisionRecipeText.Local("회귀 없음. Still NG만 추적", "No regression. Track remaining Still NG rows")
                        : OpenVisionRecipeText.Local("REGRESSION 행부터 확인", "Start with REGRESSION rows"))));

            return rows;
        }

        private static IReadOnlyList<OpenVisionRecipeOperatorResultChannelRow> BuildResultChannels(
            OpenVisionRecipeManagerSummary summary,
            OpenVisionRecipeSampleRunSummary sample,
            OpenVisionRecipePairRunSummary pair,
            IReadOnlyList<OpenVisionRecipeBatchRunComparisonRow> comparisonRows,
            OpenVisionRecipePairSampleRunSummary selectedRole,
            OpenVisionRecipeBatchRunComparisonRow selectedBatchComparison,
            OpenVisionRecipeBatchSampleResultOption selectedBatchSample)
        {
            string finalStatus;
            string finalSource;
            if (!summary.XmlValid || summary.StepCount <= 0)
            {
                finalStatus = "NG";
                finalSource = "XML/Step";
            }
            else if (pair.HasResult)
            {
                finalStatus = pair.Succeeded ? "OK" : "NG";
                finalSource = "Good/Bad";
            }
            else if (sample.HasResult)
            {
                finalStatus = sample.Succeeded ? "OK" : "NG";
                finalSource = OpenVisionRecipeText.Local("선택 샘플", "Selected sample");
            }
            else
            {
                finalStatus = "WAIT";
                finalSource = "XML/Step";
            }

            string failedStep = selectedRole?.FailedStepText;
            if (string.IsNullOrWhiteSpace(failedStep))
            {
                failedStep = selectedBatchComparison?.FailedStep;
            }

            if (string.IsNullOrWhiteSpace(failedStep))
            {
                failedStep = selectedBatchSample?.FailedStep;
            }

            string evidence = pair.HasResult
                ? pair.CompactText
                : (sample.HasResult ? sample.CompactText : summary.XmlStatusDisplay);
            int comparable = comparisonRows.Count(row => row != null && row.IsComparable);
            int regression = comparisonRows.Count(row => row != null && row.IsRegression);
            string benchmark = comparable <= 0
                ? "WAIT"
                : (regression == 0 ? "OK" : "NG");

            return new[]
            {
                OpenVisionRecipeOperatorResultChannelRow.Create(
                    "Inspection.Status",
                    finalStatus,
                    finalSource,
                    OpenVisionRecipeText.Local("최종 OK/NG 판정", "Final OK/NG judgement")),
                OpenVisionRecipeOperatorResultChannelRow.Create(
                    "Inspection.FailedStep",
                    string.IsNullOrWhiteSpace(failedStep) ? "-" : failedStep,
                    string.IsNullOrWhiteSpace(failedStep) ? OpenVisionRecipeText.Local("실패 없음", "No failure") : OpenVisionRecipeText.Local("실패 추적", "Failure trace"),
                    OpenVisionRecipeText.Local("실패 원인 추적", "Failure triage")),
                OpenVisionRecipeOperatorResultChannelRow.Create(
                    "Inspection.Evidence",
                    evidence,
                    finalSource,
                    OpenVisionRecipeText.Local("리포트/LLM 재검토 근거", "Report/LLM review evidence")),
                OpenVisionRecipeOperatorResultChannelRow.Create(
                    "Inspection.Benchmark",
                    benchmark,
                    OpenVisionRecipeText.Local("이력 비교", "Run history"),
                    comparable <= 0
                        ? OpenVisionRecipeText.Local("기준 실행 필요", "Needs baseline run")
                        : OpenVisionRecipeText.Local("회귀 비교 결과", "Regression diff result")),
                OpenVisionRecipeOperatorResultChannelRow.Create(
                    "Inspection.NextAction",
                    OpenVisionRecipeRunReviewPresenter.BuildNextAction(summary, sample, pair),
                    OpenVisionRecipeText.Local("작업자 검토", "Operator review"),
                    OpenVisionRecipeText.Local("다음 작업 지시", "Next action instruction"))
            };
        }

        private static string BuildHandoffReportText(
            OpenVisionRecipeManagerSummary summary,
            OpenVisionRecipeSampleRunSummary sample,
            OpenVisionRecipePairRunSummary pair,
            string evidenceText,
            IReadOnlyList<OpenVisionRecipeOperatorValidationRow> validationRows,
            IReadOnlyList<OpenVisionRecipeOperatorResultChannelRow> resultChannels,
            OpenVisionRecipePairSampleRunSummary selectedRole,
            OpenVisionRecipePipelineStepPreview handoffStep)
        {
            List<string> lines = new List<string>
            {
                OpenVisionRecipeText.Local("OpenVisionLab 작업자 리포트", "OpenVisionLab operator report"),
                OpenVisionRecipeText.Local("레시피: ", "Recipe: ") + (string.IsNullOrWhiteSpace(summary.RecipeName) ? "-" : summary.RecipeName),
                OpenVisionRecipeText.Local("파이프라인: ", "Pipeline: ") + (string.IsNullOrWhiteSpace(summary.PreviewPipelineName) ? "-" : summary.PreviewPipelineName),
                OpenVisionRecipeText.Local("활성 파이프라인: ", "Active pipeline: ") + (string.IsNullOrWhiteSpace(summary.ActivePipelineName) ? "-" : summary.ActivePipelineName),
                OpenVisionRecipeText.Local("XML/Step: ", "XML/Steps: ") + summary.XmlStatusDisplay + " / " + summary.StepCount.ToString(CultureInfo.InvariantCulture),
                OpenVisionRecipeText.Local("샘플: ", "Sample: ") + sample.CompactText,
                "Good/Bad: " + pair.CompactText,
                "Metric evidence: " + evidenceText,
                OpenVisionRecipeText.Local("다음 작업: ", "Next action: ")
                    + OpenVisionRecipeRunReviewPresenter.BuildNextAction(summary, sample, pair)
            };

            lines.Add(OpenVisionRecipeText.Local("검증 체크리스트:", "Validation checklist:"));
            foreach (OpenVisionRecipeOperatorValidationRow row in validationRows)
            {
                lines.Add("- " + row.ItemText + ": " + row.StateText + " | " + row.EvidenceText + " | " + row.NextActionText);
            }

            lines.Add(OpenVisionRecipeText.Local("판정 출력 정의:", "Judgement outputs:"));
            foreach (OpenVisionRecipeOperatorResultChannelRow row in resultChannels)
            {
                lines.Add("- " + row.ChannelText + ": " + row.ValueText + " | " + row.SourceText + " | " + row.UseText);
            }

            if (selectedRole != null)
            {
                lines.Add(OpenVisionRecipeText.Local("선택 역할: ", "Selected role: ")
                    + selectedRole.Role
                    + " / "
                    + selectedRole.ResultText
                    + " / "
                    + selectedRole.SampleName);
            }

            if (handoffStep != null)
            {
                lines.Add(OpenVisionRecipeText.Local("검토 Step: ", "Review step: ") + handoffStep.DisplayText);
                lines.Add(OpenVisionRecipeText.Local("입출력: ", "Route: ") + handoffStep.InputLayer + " -> " + handoffStep.OutputLayer);
            }

            if (!string.IsNullOrWhiteSpace(summary.LlmXmlValidationReport))
            {
                lines.Add(OpenVisionRecipeText.Local("LLM XML: ", "LLM XML: ") + FirstReportLine(summary.LlmXmlValidationReport));
            }

            return string.Join(Environment.NewLine, lines);
        }

        private static string FirstReportLine(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return "-";
            }

            return text
                .Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries)
                .Select(line => line.Trim())
                .FirstOrDefault(line => line.Length > 0) ?? "-";
        }

        private static string FirstNonEmpty(params string[] values)
        {
            foreach (string value in values ?? Array.Empty<string>())
            {
                if (!string.IsNullOrWhiteSpace(value) && value.Trim() != "-")
                {
                    return value.Trim();
                }
            }

            return "-";
        }

        private static string ShortEvidence(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return "-";
            }

            string text = value.Replace("\r", " ").Replace("\n", " ").Trim();
            return text.Length <= 120 ? text : text.Substring(0, 117) + "...";
        }

        private static string ResolveParameterFamily(string toolType, string metricText)
        {
            string tool = toolType ?? string.Empty;
            string metric = metricText ?? string.Empty;
            if (tool.IndexOf("Line", StringComparison.OrdinalIgnoreCase) >= 0
                || metric.IndexOf("Distance", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return "LineDistance ROI / contrast / sampling / mm-per-pixel";
            }

            if (tool.IndexOf("Blob", StringComparison.OrdinalIgnoreCase) >= 0
                || tool.IndexOf("Contour", StringComparison.OrdinalIgnoreCase) >= 0
                || metric.IndexOf("Area", StringComparison.OrdinalIgnoreCase) >= 0
                || metric.IndexOf("ResultCount", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return "threshold / ROI / area gates";
            }

            if (tool.IndexOf("Matching", StringComparison.OrdinalIgnoreCase) >= 0
                || metric.IndexOf("Score", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return "template / score / search ROI";
            }

            if (tool.IndexOf("Mean", StringComparison.OrdinalIgnoreCase) >= 0
                || metric.IndexOf("Mean", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return "ROI / mean min-max gate";
            }

            return string.IsNullOrWhiteSpace(tool) || tool.Trim() == "-"
                ? "selected step parameters"
                : tool.Trim() + " parameters";
        }
    }
}
