using Lib.OpenCV;
using Lib.OpenCV.Pipeline;
using Lib.OpenCV.Property;
using OpenVisionLab.Mvvm;
using OpenVisionLab.Vision._1._Tools.OpenCV;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using static OpenVisionLab.DEFINE;

namespace OpenVisionLab
{
    public sealed class OpenVisionRecipeSampleRunSummary
    {
        public static OpenVisionRecipeSampleRunSummary Empty { get; } = new OpenVisionRecipeSampleRunSummary(
            OpenVisionRecipeText.Local("아직 실행 안 됨.", "Not run yet."),
            OpenVisionRecipeText.Local("샘플을 선택한 뒤 명시적으로 검사를 실행하세요.", "Select a sample and run an explicit check."),
            false);

        private OpenVisionRecipeSampleRunSummary(
            string statusText,
            string detailText,
            bool hasResult,
            string compactText = null,
            bool succeeded = false,
            string distanceMetricText = null,
            string recipeName = null,
            string pipelineName = null,
            string sampleName = null)
        {
            StatusText = statusText ?? string.Empty;
            DetailText = detailText ?? string.Empty;
            HasResult = hasResult;
            Succeeded = succeeded;
            CompactText = string.IsNullOrWhiteSpace(compactText) ? StatusText : compactText.Trim();
            DistanceMetricText = string.IsNullOrWhiteSpace(distanceMetricText) ? string.Empty : distanceMetricText.Trim();
            RecipeName = recipeName?.Trim() ?? string.Empty;
            PipelineName = pipelineName?.Trim() ?? string.Empty;
            SampleName = sampleName?.Trim() ?? string.Empty;
        }

        public string StatusText { get; }

        public string DetailText { get; }

        public bool HasResult { get; }

        public bool Succeeded { get; }

        public string CompactText { get; }

        public string DistanceMetricText { get; }

        public string RecipeName { get; }

        public string PipelineName { get; }

        public string SampleName { get; }

        public bool HasExecutionContext =>
            !string.IsNullOrWhiteSpace(RecipeName)
            && !string.IsNullOrWhiteSpace(PipelineName);

        public string DisplayText => StatusText + Environment.NewLine + DetailText;

        public static OpenVisionRecipeSampleRunSummary CreatePending(OpenVisionRecipeSampleOption sampleOption)
        {
            if (sampleOption == null)
            {
                return Empty;
            }

            return new OpenVisionRecipeSampleRunSummary(
                OpenVisionRecipeText.Local("아직 실행 안 됨.", "Not run yet."),
                OpenVisionRecipeText.Local("선택 샘플 실행 준비: ", "Ready to run selected sample: ") + sampleOption.SampleName,
                false,
                OpenVisionRecipeText.Local("준비: ", "Ready: ") + sampleOption.SampleName);
        }

        public static OpenVisionRecipeSampleRunSummary CreateRunning(
            OpenVisionRecipeSampleOption sampleOption,
            string pipelineName)
        {
            return CreateRunning(sampleOption, string.Empty, pipelineName);
        }

        public static OpenVisionRecipeSampleRunSummary CreateRunning(
            OpenVisionRecipeSampleOption sampleOption,
            string recipeName,
            string pipelineName)
        {
            return new OpenVisionRecipeSampleRunSummary(
                OpenVisionRecipeText.Local("샘플 검사 실행 중...", "Running sample check..."),
                FormatSampleAndPipeline(sampleOption, pipelineName),
                false,
                compactText: OpenVisionRecipeText.Local("실행 중: ", "Running: ") + (string.IsNullOrWhiteSpace(sampleOption?.SampleName) ? "-" : sampleOption.SampleName),
                recipeName: recipeName,
                pipelineName: pipelineName,
                sampleName: sampleOption?.SampleName);
        }

        internal static OpenVisionRecipeSampleRunSummary FromResult(
            OpenVisionRecipeSampleOption sampleOption,
            string recipeName,
            string pipelineName,
            VisionPipelineSampleCheckResult result)
        {
            if (result == null)
            {
                return new OpenVisionRecipeSampleRunSummary(
                    OpenVisionRecipeText.Local("샘플 검사 ERROR", "Sample check ERROR"),
                    FormatSampleAndPipeline(sampleOption, pipelineName),
                    true,
                    compactText: OpenVisionRecipeText.Local("샘플 검사 ERROR", "Sample check ERROR"),
                    recipeName: recipeName,
                    pipelineName: pipelineName,
                    sampleName: sampleOption?.SampleName);
            }

            string status = string.IsNullOrWhiteSpace(result.Status) ? "-" : result.Status;
            string metric = string.IsNullOrWhiteSpace(result.MetricText) ? "-" : result.MetricText;
            List<string> lines = new List<string>
            {
                FormatSampleAndPipeline(sampleOption, pipelineName),
                OpenVisionRecipeText.Local("지표: ", "Metric: ") + metric,
                OpenVisionRecipeText.Local("동작: ", "Action: ") + (string.IsNullOrWhiteSpace(result.ActionSummaryText) ? "-" : result.ActionSummaryText),
                OpenVisionRecipeText.Local("다음: ", "Next: ") + BuildSampleNextAction(result)
            };

            if (!string.IsNullOrWhiteSpace(result.FailedStepText))
            {
                lines.Add(OpenVisionRecipeText.Local("실패 단계: ", "Failed step: ") + result.FailedStepText);
            }

            if (!string.IsNullOrWhiteSpace(result.Message))
            {
                lines.Add(OpenVisionRecipeText.Local("메시지: ", "Message: ") + result.Message);
            }

            string compact = OpenVisionRecipeText.Local("샘플 검사 ", "Sample check ") + status + " | " + metric;
            if (!result.Success && !string.IsNullOrWhiteSpace(result.FailedStepText))
            {
                compact += " | " + result.FailedStepText;
            }

            if (!result.Success)
            {
                compact += OpenVisionRecipeText.Local(" | 다음: ", " | Next: ") + BuildSampleNextAction(result);
            }

            return new OpenVisionRecipeSampleRunSummary(
                OpenVisionRecipeText.Local("샘플 검사 ", "Sample check ") + status,
                string.Join(Environment.NewLine, lines),
                true,
                compact,
                result.Success,
                result.DistanceMetricText,
                recipeName,
                pipelineName,
                sampleOption?.SampleName);
        }

        public bool IsForRecipePipeline(string recipeName, string pipelineName)
        {
            return HasExecutionContext
                && string.Equals(RecipeName, recipeName?.Trim(), StringComparison.OrdinalIgnoreCase)
                && string.Equals(PipelineName, pipelineName?.Trim(), StringComparison.OrdinalIgnoreCase);
        }

        private static string BuildSampleNextAction(VisionPipelineSampleCheckResult result)
        {
            if (result?.Success == true)
            {
                return OpenVisionRecipeText.Local("추가 조치가 필요 없습니다.", "No action needed.");
            }

            string message = result?.Message ?? string.Empty;
            if (message.IndexOf("missing", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return OpenVisionRecipeText.Local("기대 지표 이름과 해당 지표를 생성해야 하는 출력 단계를 확인하세요.", "Check the expected metric name and the output step that should produce it.");
            }

            if (!string.IsNullOrWhiteSpace(result?.FailedStepText))
            {
                return OpenVisionRecipeText.Local("실패 단계를 열어 입력/출력 레이어를 확인한 뒤 해당 도구 파라미터를 조정하세요.", "Open the failed step, review input/output layers, then tune that tool parameter.");
            }

            if (string.Equals(result?.Status, "ERROR", StringComparison.OrdinalIgnoreCase))
            {
                return OpenVisionRecipeText.Local("XML, 샘플 이미지 경로, 참조 템플릿 파일을 검증하세요.", "Validate XML, sample image path, and referenced template files.");
            }

            return OpenVisionRecipeText.Local("지표 기준과 실제값을 비교한 뒤 임계값/ROI/템플릿 파라미터를 조정하세요.", "Compare metric gate versus actual value, then tune threshold/ROI/template parameters.");
        }

        private static string FormatSampleAndPipeline(OpenVisionRecipeSampleOption sampleOption, string pipelineName)
        {
            string sample = string.IsNullOrWhiteSpace(sampleOption?.SampleName) ? "-" : sampleOption.SampleName;
            string pipeline = string.IsNullOrWhiteSpace(pipelineName) ? "-" : pipelineName;
            return OpenVisionRecipeText.Local("샘플: ", "Sample: ") + sample
                + " / "
                + OpenVisionRecipeText.Local("파이프라인: ", "Pipeline: ") + pipeline;
        }
    }

    public sealed class OpenVisionRecipeSampleMatrixRow
    {
        private OpenVisionRecipeSampleMatrixRow(
            string role,
            string sampleName,
            string expectedText,
            string resultText,
            string metricText,
            string failedStep,
            string nextActionText,
            bool hasResult,
            bool success,
            bool isPlaceholder)
        {
            Role = string.IsNullOrWhiteSpace(role) ? "-" : role.Trim();
            SampleName = sampleName ?? string.Empty;
            ExpectedText = string.IsNullOrWhiteSpace(expectedText) ? "-" : expectedText.Trim();
            ResultText = string.IsNullOrWhiteSpace(resultText) ? "WAIT" : resultText.Trim();
            MetricText = string.IsNullOrWhiteSpace(metricText) ? "-" : metricText.Trim();
            FailedStep = failedStep ?? string.Empty;
            NextActionText = string.IsNullOrWhiteSpace(nextActionText) ? "-" : nextActionText.Trim();
            HasResult = hasResult;
            Success = success;
            IsPlaceholder = isPlaceholder;
        }

        public string Role { get; }

        public string SampleName { get; }

        public string ExpectedText { get; }

        public string ResultText { get; }

        public string MetricText { get; }

        public string FailedStep { get; }

        public string NextActionText { get; }

        public bool HasResult { get; }

        public bool Success { get; }

        public bool IsPlaceholder { get; }

        public string FailedStepDisplayText =>
            string.IsNullOrWhiteSpace(FailedStep) ? "-" : FailedStep.Trim();

        public string ResultBadgeText =>
            !HasResult ? "WAIT" : (Success ? "OK" : "NG");

        public string DisplayText =>
            Role + " | " + (string.IsNullOrWhiteSpace(SampleName) ? "-" : SampleName)
            + " | " + ResultBadgeText;

        public string ReviewText =>
            OpenVisionRecipeText.Local("샘플: ", "Sample: ") + (string.IsNullOrWhiteSpace(SampleName) ? "-" : SampleName)
            + Environment.NewLine
            + OpenVisionRecipeText.Local("역할/결과: ", "Role/result: ") + Role + " / " + ResultBadgeText
            + Environment.NewLine
            + OpenVisionRecipeText.Local("기대 기준: ", "Expected gate: ") + ExpectedText
            + Environment.NewLine
            + OpenVisionRecipeText.Local("현재 지표: ", "Current metric: ") + MetricText
            + Environment.NewLine
            + OpenVisionRecipeText.Local("실패 Step: ", "Failed step: ") + FailedStepDisplayText
            + Environment.NewLine
            + OpenVisionRecipeText.Local("다음: ", "Next: ") + NextActionText;

        internal static OpenVisionRecipeSampleMatrixRow Create(
            VisionPipelineSampleCatalogItem sample,
            OpenVisionRecipePairSampleRunSummary result)
        {
            if (sample == null)
            {
                return CreateEmpty();
            }

            bool hasResult = result != null;
            string role = string.IsNullOrWhiteSpace(sample.PairRole) ? "Sample" : sample.PairRole.Trim();
            string expected = sample.ExpectsFailure
                ? OpenVisionRecipeText.Local("통제된 NG/no-result 기대", "Expected controlled NG/no-result")
                : sample.ExpectedText;
            string metric = hasResult ? result.MetricText : sample.ExpectedText;
            string next = hasResult
                ? result.NextActionText
                : OpenVisionRecipeText.Local("명시적으로 Good/Bad 쌍 검사를 실행하세요.", "Run the explicit Good/Bad pair check.");

            return new OpenVisionRecipeSampleMatrixRow(
                role,
                sample.SampleName,
                expected,
                hasResult ? result.ResultText : "WAIT",
                metric,
                result?.FailedStepText,
                next,
                hasResult,
                result?.Success ?? false,
                false);
        }

        public static OpenVisionRecipeSampleMatrixRow CreateEmpty()
        {
            return new OpenVisionRecipeSampleMatrixRow(
                "-",
                OpenVisionRecipeText.Local("샘플 없음", "No sample"),
                "-",
                "WAIT",
                "-",
                string.Empty,
                OpenVisionRecipeText.Local("샘플을 선택하세요.", "Select a sample."),
                false,
                false,
                true);
        }
    }

    public sealed class OpenVisionRecipeCatalogBenchmarkSummary
    {
        public static OpenVisionRecipeCatalogBenchmarkSummary Empty { get; } = new OpenVisionRecipeCatalogBenchmarkSummary(
            OpenVisionRecipeText.Local("카탈로그 벤치마크 미실행", "Catalog benchmark not run."),
            OpenVisionRecipeText.Local("현재 파이프라인을 Product sample catalog 전체에 대해 명시적으로 실행하면 결과가 여기에 표시됩니다.", "Run the current pipeline against the full Product sample catalog to show the result here."),
            false,
            false,
            string.Empty);

        private OpenVisionRecipeCatalogBenchmarkSummary(
            string compactText,
            string detailText,
            bool hasResult,
            bool succeeded,
            string summaryPath)
        {
            CompactText = compactText ?? string.Empty;
            DetailText = detailText ?? string.Empty;
            HasResult = hasResult;
            Succeeded = succeeded;
            SummaryPath = summaryPath ?? string.Empty;
        }

        public string CompactText { get; }

        public string DetailText { get; }

        public bool HasResult { get; }

        public bool Succeeded { get; }

        public string SummaryPath { get; }

        public static OpenVisionRecipeCatalogBenchmarkSummary CreateRunning(string pipelineName, int total)
        {
            string pipeline = string.IsNullOrWhiteSpace(pipelineName) ? "-" : pipelineName.Trim();
            return new OpenVisionRecipeCatalogBenchmarkSummary(
                OpenVisionRecipeText.Local("카탈로그 벤치마크 실행 중", "Catalog benchmark running"),
                OpenVisionRecipeText.Local("파이프라인: ", "Pipeline: ") + pipeline
                + Environment.NewLine
                + OpenVisionRecipeText.Local("대상 Product 샘플: ", "Target Product samples: ") + total.ToString(CultureInfo.InvariantCulture),
                false,
                false,
                string.Empty);
        }

        public static OpenVisionRecipeCatalogBenchmarkSummary CreateProgress(
            string pipelineName,
            int completed,
            int total,
            IReadOnlyList<VisionPipelineBatchSampleRunResult> results)
        {
            List<VisionPipelineBatchSampleRunResult> resultList =
                (results ?? Array.Empty<VisionPipelineBatchSampleRunResult>()).Where(result => result != null).ToList();
            int pass = resultList.Count(result => result.Success);
            int fail = resultList.Count(result => !result.Success);
            string compact = OpenVisionRecipeText.Local("진행: ", "Progress: ")
                + completed.ToString(CultureInfo.InvariantCulture)
                + "/"
                + total.ToString(CultureInfo.InvariantCulture)
                + " | OK "
                + pass.ToString(CultureInfo.InvariantCulture)
                + " / NG "
                + fail.ToString(CultureInfo.InvariantCulture);

            return new OpenVisionRecipeCatalogBenchmarkSummary(
                compact,
                OpenVisionRecipeText.Local("파이프라인: ", "Pipeline: ") + (string.IsNullOrWhiteSpace(pipelineName) ? "-" : pipelineName.Trim())
                + Environment.NewLine
                + compact
                + FormatFailurePreview(resultList),
                false,
                false,
                string.Empty);
        }

        public static OpenVisionRecipeCatalogBenchmarkSummary FromResults(
            string pipelineName,
            IReadOnlyList<VisionPipelineBatchSampleRunResult> results,
            string summaryPath)
        {
            List<VisionPipelineBatchSampleRunResult> resultList =
                (results ?? Array.Empty<VisionPipelineBatchSampleRunResult>()).Where(result => result != null).ToList();
            int total = resultList.Count;
            int pass = resultList.Count(result => result.Success);
            int fail = resultList.Count(result => !result.Success);
            bool ok = total > 0 && fail == 0;
            string compact = "Catalog "
                + (ok ? "OK" : "NG")
                + " | "
                + pass.ToString(CultureInfo.InvariantCulture)
                + "/"
                + total.ToString(CultureInfo.InvariantCulture)
                + OpenVisionRecipeText.Local(" 통과", " pass");

            List<string> lines = new List<string>
            {
                OpenVisionRecipeText.Local("파이프라인: ", "Pipeline: ") + (string.IsNullOrWhiteSpace(pipelineName) ? "-" : pipelineName.Trim()),
                OpenVisionRecipeText.Local("Product 샘플: ", "Product samples: ") + total.ToString(CultureInfo.InvariantCulture),
                OpenVisionRecipeText.Local("통과/실패: ", "Pass/fail: ") + pass.ToString(CultureInfo.InvariantCulture) + "/" + fail.ToString(CultureInfo.InvariantCulture),
                OpenVisionRecipeText.Local("다음: ", "Next: ") + (ok
                    ? OpenVisionRecipeText.Local("대량 샘플에서 회귀가 발견되지 않았습니다. Run History에서 summary.tsv를 보관하거나 비교하세요.", "No regression was found across the catalog. Keep or compare the summary.tsv from Run History.")
                    : OpenVisionRecipeText.Local("Run History에서 NG 샘플을 선택하고 실패 Step, 입력/출력 레이어, PropertyGrid 파라미터를 확인하세요.", "Select NG samples in Run History and review failed steps, input/output layers, and PropertyGrid parameters."))
            };
            string failurePreview = FormatFailurePreview(resultList);
            if (!string.IsNullOrWhiteSpace(failurePreview))
            {
                lines.Add(failurePreview.Trim());
            }

            if (!string.IsNullOrWhiteSpace(summaryPath))
            {
                lines.Add(OpenVisionRecipeText.Local("저장된 요약: ", "Saved summary: ") + summaryPath);
            }

            return new OpenVisionRecipeCatalogBenchmarkSummary(
                compact,
                string.Join(Environment.NewLine, lines),
                true,
                ok,
                summaryPath);
        }

        public static OpenVisionRecipeCatalogBenchmarkSummary FromError(string pipelineName, string message)
        {
            return new OpenVisionRecipeCatalogBenchmarkSummary(
                OpenVisionRecipeText.Local("카탈로그 벤치마크 ERROR", "Catalog benchmark ERROR"),
                OpenVisionRecipeText.Local("파이프라인: ", "Pipeline: ") + (string.IsNullOrWhiteSpace(pipelineName) ? "-" : pipelineName.Trim())
                + Environment.NewLine
                + OpenVisionRecipeText.Local("메시지: ", "Message: ") + (message ?? string.Empty),
                true,
                false,
                string.Empty);
        }

        private static string FormatFailurePreview(IReadOnlyList<VisionPipelineBatchSampleRunResult> results)
        {
            List<VisionPipelineBatchSampleRunResult> failures = (results ?? Array.Empty<VisionPipelineBatchSampleRunResult>())
                .Where(result => result != null && !result.Success)
                .Take(5)
                .ToList();
            if (failures.Count == 0)
            {
                return string.Empty;
            }

            return Environment.NewLine
                + OpenVisionRecipeText.Local("주요 실패: ", "Top failures: ")
                + string.Join(", ", failures.Select(FormatFailure));
        }

        private static string FormatFailure(VisionPipelineBatchSampleRunResult result)
        {
            string sample = string.IsNullOrWhiteSpace(result.SampleName) ? "-" : result.SampleName.Trim();
            string step = string.IsNullOrWhiteSpace(result.FailedStep) ? string.Empty : " @ " + result.FailedStep.Trim();
            return sample + step;
        }
    }

    public sealed class OpenVisionRecipePairRunSummary
    {
        public static OpenVisionRecipePairRunSummary Empty { get; } = new OpenVisionRecipePairRunSummary(
            OpenVisionRecipeText.Local("쌍 검사 미실행.", "Pair check not run."),
            OpenVisionRecipeText.Local("Good/Bad 샘플 쌍을 선택한 뒤 명시적으로 쌍 검사를 실행하세요.", "Select a Good/Bad sample pair and run an explicit pair check."),
            false);

        private OpenVisionRecipePairRunSummary(
            string statusText,
            string detailText,
            bool hasResult,
            string compactText = null,
            bool succeeded = false,
            IReadOnlyList<OpenVisionRecipePairSampleRunSummary> sampleResults = null)
        {
            StatusText = statusText ?? string.Empty;
            DetailText = detailText ?? string.Empty;
            HasResult = hasResult;
            Succeeded = succeeded;
            CompactText = string.IsNullOrWhiteSpace(compactText) ? StatusText : compactText.Trim();
            SampleResults = sampleResults ?? Array.Empty<OpenVisionRecipePairSampleRunSummary>();
        }

        public string StatusText { get; }

        public string DetailText { get; }

        public bool HasResult { get; }

        public bool Succeeded { get; }

        public string CompactText { get; }

        public IReadOnlyList<OpenVisionRecipePairSampleRunSummary> SampleResults { get; }

        public string DisplayText => StatusText + Environment.NewLine + DetailText;

        public static OpenVisionRecipePairRunSummary CreatePending(OpenVisionRecipeSampleOption sampleOption)
        {
            if (sampleOption?.Sample == null || string.IsNullOrWhiteSpace(sampleOption.Sample.PairGroup))
            {
                return Empty;
            }

            return new OpenVisionRecipePairRunSummary(
                OpenVisionRecipeText.Local("쌍 검사 미실행.", "Pair check not run."),
                OpenVisionRecipeText.Local("PairGroup 실행 준비: ", "Ready to run PairGroup: ") + sampleOption.Sample.PairGroup,
                false,
                OpenVisionRecipeText.Local("준비: ", "Ready: ") + sampleOption.Sample.PairGroup);
        }

        public static OpenVisionRecipePairRunSummary CreateRunning(
            OpenVisionRecipeSampleOption sampleOption,
            string pipelineName,
            int sampleCount)
        {
            string group = string.IsNullOrWhiteSpace(sampleOption?.Sample?.PairGroup) ? "-" : sampleOption.Sample.PairGroup.Trim();
            return new OpenVisionRecipePairRunSummary(
                OpenVisionRecipeText.Local("Good/Bad 쌍 검사 실행 중...", "Running Good/Bad pair check..."),
                "PairGroup: " + group + " / " + OpenVisionRecipeText.Local("파이프라인: ", "Pipeline: ") + (string.IsNullOrWhiteSpace(pipelineName) ? "-" : pipelineName),
                false,
                OpenVisionRecipeText.Local("실행 중: ", "Running: ") + group + " (" + sampleCount.ToString(CultureInfo.InvariantCulture) + OpenVisionRecipeText.Local("개 샘플", " samples") + ")");
        }

        internal static OpenVisionRecipePairRunSummary FromResults(
            OpenVisionRecipeSampleOption sampleOption,
            string pipelineName,
            IReadOnlyList<OpenVisionRecipePairSampleRunSummary> results,
            string summaryPath)
        {
            List<OpenVisionRecipePairSampleRunSummary> resultList = (results ?? Array.Empty<OpenVisionRecipePairSampleRunSummary>()).ToList();
            int total = resultList.Count;
            int pass = resultList.Count(result => result.Success);
            bool ok = total > 0 && pass == total;
            string group = string.IsNullOrWhiteSpace(sampleOption?.Sample?.PairGroup) ? "-" : sampleOption.Sample.PairGroup.Trim();
            string compact = OpenVisionRecipeText.Local("쌍 검사 ", "Pair check ") + (ok ? "OK" : "NG")
                + " | " + pass.ToString(CultureInfo.InvariantCulture)
                + "/" + total.ToString(CultureInfo.InvariantCulture)
                + OpenVisionRecipeText.Local(" 통과", " pass");

            if (resultList.Count > 0)
            {
                compact += " | " + string.Join(" | ", resultList.Select(result => result.CompactText));
            }

            List<string> lines = new List<string>
            {
                "PairGroup: " + group,
                OpenVisionRecipeText.Local("파이프라인: ", "Pipeline: ") + (string.IsNullOrWhiteSpace(pipelineName) ? "-" : pipelineName),
                OpenVisionRecipeText.Local("통과: ", "Pass: ") + pass.ToString(CultureInfo.InvariantCulture) + "/" + total.ToString(CultureInfo.InvariantCulture),
                OpenVisionRecipeText.Local("다음: ", "Next: ") + (ok
                    ? OpenVisionRecipeText.Local("추가 조치가 필요 없습니다.", "No action needed.")
                    : OpenVisionRecipeText.Local("아래 실패 샘플 역할을 열고 Good/Bad가 모두 기대와 맞을 때까지 활성 파이프라인을 조정하세요.", "Open the failed sample role below and tune the active pipeline until Good and Bad both match expectations."))
            };
            lines.AddRange(resultList.Select(result => result.DisplayText));
            if (!string.IsNullOrWhiteSpace(summaryPath))
            {
                lines.Add(OpenVisionRecipeText.Local("저장된 요약: ", "Saved summary: ") + summaryPath);
            }

            return new OpenVisionRecipePairRunSummary(
                OpenVisionRecipeText.Local("쌍 검사 ", "Pair check ") + (ok ? "OK" : "NG"),
                string.Join(Environment.NewLine, lines),
                true,
                compact,
                ok,
                resultList);
        }

        internal static OpenVisionRecipePairRunSummary FromError(
            OpenVisionRecipeSampleOption sampleOption,
            string pipelineName,
            string message)
        {
            string group = string.IsNullOrWhiteSpace(sampleOption?.Sample?.PairGroup) ? "-" : sampleOption.Sample.PairGroup.Trim();
            return new OpenVisionRecipePairRunSummary(
                OpenVisionRecipeText.Local("쌍 검사 ERROR", "Pair check ERROR"),
                "PairGroup: " + group
                + Environment.NewLine
                + OpenVisionRecipeText.Local("파이프라인: ", "Pipeline: ") + (string.IsNullOrWhiteSpace(pipelineName) ? "-" : pipelineName)
                + Environment.NewLine
                + OpenVisionRecipeText.Local("메시지: ", "Message: ") + (message ?? string.Empty),
                true,
                OpenVisionRecipeText.Local("쌍 검사 ERROR | ", "Pair check ERROR | ") + group);
        }
    }

    public sealed class OpenVisionRecipePairSampleRunSummary
    {
        private OpenVisionRecipePairSampleRunSummary(
            string role,
            string sampleName,
            string status,
            bool success,
            string metricText,
            string message,
            string failedStepText)
        {
            Role = string.IsNullOrWhiteSpace(role) ? "Sample" : role.Trim();
            SampleName = sampleName ?? string.Empty;
            Status = status ?? string.Empty;
            Success = success;
            MetricText = metricText ?? string.Empty;
            Message = message ?? string.Empty;
            FailedStepText = failedStepText ?? string.Empty;
        }

        public string Role { get; }

        public string SampleName { get; }

        public string Status { get; }

        public bool Success { get; }

        public string MetricText { get; }

        public string Message { get; }

        public string FailedStepText { get; }

        public bool CanOpenFailedStep =>
            !Success && !string.IsNullOrWhiteSpace(FailedStepText) && FailedStepText.Trim() != "-";

        public string CompactText =>
            Role + " " + (string.IsNullOrWhiteSpace(Status) ? "-" : Status);

        public string ResultText =>
            Success ? "OK" : "NG";

        public string ActionText =>
            Success
                ? OpenVisionRecipeText.Local("기대 결과와 일치", "Matches expected result")
                : OpenVisionRecipeText.Local("실패 Step과 판정 기준 확인", "Review failed step and gate");

        public string DisplayText =>
            Role + ": "
            + (string.IsNullOrWhiteSpace(SampleName) ? "-" : SampleName)
            + " / "
            + (string.IsNullOrWhiteSpace(Status) ? "-" : Status)
            + " / "
            + (string.IsNullOrWhiteSpace(MetricText) ? "-" : MetricText)
            + (string.IsNullOrWhiteSpace(Message) ? string.Empty : " / " + Message);

        public string OpenFailedStepText =>
            CanOpenFailedStep
                ? OpenVisionRecipeText.Local("Step 보기", "View step")
                : OpenVisionRecipeText.Local("검토", "Review");

        public string NextActionText
        {
            get
            {
                if (Success)
                {
                    return OpenVisionRecipeText.Local("예상 결과와 일치합니다. 반대 역할도 OK인지 확인하세요.", "Matches the expected result. Confirm the counterpart role is also OK.");
                }

                if (CanOpenFailedStep)
                {
                    return OpenVisionRecipeText.Local("실패 Step을 선택한 뒤 입력/출력 레이어, 판정 기준, PropertyGrid 파라미터를 조정하세요.", "Select the failed step, then tune input/output layers, gates, and PropertyGrid parameters.");
                }

                return OpenVisionRecipeText.Local("실패 Step 기록이 없습니다. 실행 로그와 XML 경로를 먼저 확인하세요.", "No failed step was recorded. Check the run log and XML route first.");
            }
        }

        public string ReviewText =>
            Role + " / " + ResultText
            + Environment.NewLine
            + OpenVisionRecipeText.Local("샘플: ", "Sample: ") + (string.IsNullOrWhiteSpace(SampleName) ? "-" : SampleName)
            + Environment.NewLine
            + OpenVisionRecipeText.Local("지표: ", "Metric: ") + (string.IsNullOrWhiteSpace(MetricText) ? "-" : MetricText)
            + Environment.NewLine
            + OpenVisionRecipeText.Local("실패 Step: ", "Failed step: ") + (string.IsNullOrWhiteSpace(FailedStepText) ? "-" : FailedStepText)
            + Environment.NewLine
            + OpenVisionRecipeText.Local("다음: ", "Next: ") + NextActionText;

        internal static OpenVisionRecipePairSampleRunSummary FromResult(
            VisionPipelineSampleCatalogItem sample,
            VisionPipelineSampleCheckResult result)
        {
            return new OpenVisionRecipePairSampleRunSummary(
                sample?.PairRole,
                sample?.SampleName,
                result?.Status,
                result?.Success ?? false,
                result?.MetricText,
                result?.Message,
                result?.FailedStepText);
        }

        internal static OpenVisionRecipePairSampleRunSummary CreateForTest(
            string role,
            string sampleName,
            string status,
            bool success,
            string metricText,
            string message,
            string failedStepText)
        {
            return new OpenVisionRecipePairSampleRunSummary(
                role,
                sampleName,
                status,
                success,
                metricText,
                message,
                failedStepText);
        }
    }

    public sealed class OpenVisionRecipeBatchStepTimingRow
    {
        private OpenVisionRecipeBatchStepTimingRow(string displayText, string timingText)
        {
            DisplayText = displayText ?? string.Empty;
            TimingText = timingText ?? string.Empty;
        }

        public string DisplayText { get; }

        public string TimingText { get; }

        internal static OpenVisionRecipeBatchStepTimingRow Create(
            VisionPipelineBatchRunSummaryStorage.BatchStepTimingStatistics statistics)
        {
            string name = string.IsNullOrWhiteSpace(statistics?.Name) ? "Step" : statistics.Name.Trim();
            string toolType = string.IsNullOrWhiteSpace(statistics?.ToolType) ? "-" : statistics.ToolType.Trim();
            string display = "#"
                + (statistics?.Index ?? 0).ToString("00", CultureInfo.InvariantCulture)
                + " "
                + name
                + " · "
                + toolType;
            int timingCount = statistics?.TimingCount ?? 0;
            int reportCount = statistics?.ReportCount ?? 0;
            string coverage = OpenVisionRecipeText.Local("측정 ", "coverage ")
                + timingCount.ToString(CultureInfo.InvariantCulture)
                + "/"
                + reportCount.ToString(CultureInfo.InvariantCulture);
            if (timingCount <= 0)
            {
                return new OpenVisionRecipeBatchStepTimingRow(
                    display,
                    coverage + " · " + OpenVisionRecipeText.Local("실행 시간 없음", "no timing"));
            }

            string timing = coverage
                + " · avg "
                + statistics.AverageMilliseconds.ToString("0.0", CultureInfo.CurrentCulture)
                + " ms · p95 "
                + statistics.P95Milliseconds.ToString("0.0", CultureInfo.CurrentCulture)
                + " ms · max "
                + statistics.MaximumMilliseconds.ToString("0.0", CultureInfo.CurrentCulture)
                + " ms";
            return new OpenVisionRecipeBatchStepTimingRow(display, timing);
        }
    }

    public sealed class OpenVisionRecipeBatchRunOption
    {
        private OpenVisionRecipeBatchRunOption(
            string displayText,
            string detailText,
            string summaryPath,
            IReadOnlyList<OpenVisionRecipeBatchSampleResultOption> sampleResults,
            VisionPipelineBatchRunSummaryStorage.BatchRunStatistics statistics,
            VisionPipelineBatchRunSummaryStorage.BatchStepTimingAnalysis stepTimingAnalysis,
            VisionPipelineBatchRunSummary runSummary)
        {
            DisplayText = displayText ?? string.Empty;
            DetailText = detailText ?? string.Empty;
            SummaryPath = summaryPath ?? string.Empty;
            SampleResults = sampleResults ?? Array.Empty<OpenVisionRecipeBatchSampleResultOption>();
            Statistics = statistics ?? new VisionPipelineBatchRunSummaryStorage.BatchRunStatistics();
            StepTimingAnalysis = stepTimingAnalysis ?? new VisionPipelineBatchRunSummaryStorage.BatchStepTimingAnalysis();
            RunSummary = runSummary;
            AnalyticsText = FormatAnalyticsText(Statistics, IsJudgmentSuite ? SampleResults : null);
            StepTimingStatusText = FormatStepTimingStatusText(StepTimingAnalysis);
            StepTimingRows = StepTimingAnalysis.Steps
                .Select(OpenVisionRecipeBatchStepTimingRow.Create)
                .ToList();
        }

        public string DisplayText { get; }

        public string DetailText { get; }

        public string SummaryPath { get; }

        public IReadOnlyList<OpenVisionRecipeBatchSampleResultOption> SampleResults { get; }

        internal VisionPipelineBatchRunSummaryStorage.BatchRunStatistics Statistics { get; }

        internal VisionPipelineBatchRunSummaryStorage.BatchStepTimingAnalysis StepTimingAnalysis { get; }

        internal VisionPipelineBatchRunSummary RunSummary { get; }

        public string AnalyticsText { get; }

        public string StepTimingStatusText { get; }

        public IReadOnlyList<OpenVisionRecipeBatchStepTimingRow> StepTimingRows { get; }

        public bool IsJudgmentSuite => SampleResults.Any(result => result?.HasExpectedOutcome == true);

        public bool IsPartialRun => string.Equals(
            RunSummary?.SuiteKind,
            "LocalValidationSetPartial",
            StringComparison.OrdinalIgnoreCase);

        public int JudgmentCorrectCount => SampleResults.Count(result =>
            result != null && result.HasExpectedOutcome && result.JudgmentCorrect);

        public int MisclassificationCount => SampleResults.Count(result =>
            result != null && result.HasExpectedOutcome && !result.JudgmentCorrect);

        public int FalseAcceptCount => SampleResults.Count(result => result?.IsFalseAccept == true);

        public int FalseRejectCount => SampleResults.Count(result => result?.IsFalseReject == true);

        public int ReviewQueueCount => SampleResults.Count(result => result?.IsInReviewQueue == true);

        public string ReviewQueuePolicy => RunSummary?.ReviewQueuePolicy ?? string.Empty;

        public string ReviewQueueSha256 => RunSummary?.ReviewQueueSha256 ?? string.Empty;

        public bool HasPersistedReviewQueue => !string.IsNullOrWhiteSpace(ReviewQueuePolicy)
            && !string.IsNullOrWhiteSpace(ReviewQueueSha256);

        internal static OpenVisionRecipeBatchRunOption Create(
            VisionPipelineBatchRunSummaryStorage.BatchRunSummaryInfo summary)
        {
            if (summary == null)
            {
                return CreateEmpty();
            }

            VisionPipelineBatchRunSummary runSummary = VisionPipelineBatchRunSummaryStorage.Load(summary.SummaryPath);
            IReadOnlyList<OpenVisionRecipeBatchSampleResultOption> sampleResults = BuildSampleResults(runSummary);
            bool judgmentSuite = sampleResults.Any(result => result?.HasExpectedOutcome == true);
            int judgmentCorrect = sampleResults.Count(result =>
                result != null && result.HasExpectedOutcome && result.JudgmentCorrect);
            int misclassified = sampleResults.Count(result =>
                result != null && result.HasExpectedOutcome && !result.JudgmentCorrect);
            bool partial = string.Equals(runSummary?.SuiteKind, "LocalValidationSetPartial", StringComparison.OrdinalIgnoreCase);
            string status = partial
                ? OpenVisionRecipeText.Local("중단", "PARTIAL")
                : (judgmentSuite ? misclassified == 0 : summary.FailCount == 0) && summary.TotalCount > 0
                    ? "OK"
                    : "NG";
            int passedCount = judgmentSuite ? judgmentCorrect : summary.PassCount;
            string display = summary.StartedAt.ToString("MM-dd HH:mm:ss", CultureInfo.CurrentCulture)
                + " | " + status
                + " | " + passedCount.ToString(CultureInfo.InvariantCulture)
                + "/" + summary.TotalCount.ToString(CultureInfo.InvariantCulture);
            string detail = FormatBatchRunDetail(summary, runSummary, sampleResults)
                + " | "
                + OpenVisionRecipeText.Local("요약: ", "Summary: ")
                + summary.SummaryPath;
            VisionPipelineBatchRunSummaryStorage.BatchRunStatistics statistics =
                VisionPipelineBatchRunSummaryStorage.CalculateStatistics(runSummary?.Results);
            VisionPipelineBatchRunSummaryStorage.BatchStepTimingAnalysis stepTimingAnalysis =
                VisionPipelineBatchRunSummaryStorage.CalculateStepTimingAnalysis(runSummary);
            return new OpenVisionRecipeBatchRunOption(
                display,
                detail,
                summary.SummaryPath,
                sampleResults,
                statistics,
                stepTimingAnalysis,
                runSummary);
        }

        private static string FormatStepTimingStatusText(
            VisionPipelineBatchRunSummaryStorage.BatchStepTimingAnalysis analysis)
        {
            if (analysis == null || analysis.SampleCount <= 0)
            {
                return OpenVisionRecipeText.Local("저장된 샘플 결과가 없습니다.", "No saved sample results.");
            }

            string coverage = OpenVisionRecipeText.Local("리포트 ", "Reports ")
                + analysis.ReportCount.ToString(CultureInfo.InvariantCulture)
                + "/"
                + analysis.SampleCount.ToString(CultureInfo.InvariantCulture);
            if (analysis.IsAvailable)
            {
                return coverage
                    + " · p95 "
                    + OpenVisionRecipeText.Local("내림차순 · Step ", "descending · Steps ")
                    + analysis.Steps.Count.ToString(CultureInfo.InvariantCulture);
            }

            string reason;
            switch (analysis.Availability)
            {
                case VisionPipelineBatchRunSummaryStorage.StepTimingAvailability.MissingReportPath:
                    reason = OpenVisionRecipeText.Local("연결 리포트 경로 누락", "linked report path missing");
                    break;
                case VisionPipelineBatchRunSummaryStorage.StepTimingAvailability.MissingReportFile:
                    reason = OpenVisionRecipeText.Local("연결 리포트 파일 없음", "linked report file missing");
                    break;
                case VisionPipelineBatchRunSummaryStorage.StepTimingAvailability.InvalidReport:
                    reason = OpenVisionRecipeText.Local("연결 리포트를 읽을 수 없음", "linked report cannot be read");
                    break;
                case VisionPipelineBatchRunSummaryStorage.StepTimingAvailability.ReportIdentityMismatch:
                    reason = OpenVisionRecipeText.Local("레시피 또는 파이프라인 불일치", "recipe or pipeline mismatch");
                    break;
                case VisionPipelineBatchRunSummaryStorage.StepTimingAvailability.StepDefinitionMismatch:
                    reason = OpenVisionRecipeText.Local("Step 정의 불일치", "Step definitions differ");
                    break;
                case VisionPipelineBatchRunSummaryStorage.StepTimingAvailability.NoEnabledSteps:
                    reason = OpenVisionRecipeText.Local("활성 Step 없음", "no enabled Steps");
                    break;
                case VisionPipelineBatchRunSummaryStorage.StepTimingAvailability.NoStepTimings:
                    reason = OpenVisionRecipeText.Local("Step 실행 시간 없음", "no Step timing");
                    break;
                default:
                    reason = OpenVisionRecipeText.Local("Step 분석 불가", "Step analysis unavailable");
                    break;
            }

            string detail = string.IsNullOrWhiteSpace(analysis.Detail)
                ? string.Empty
                : " · " + analysis.Detail;
            return coverage + " · " + reason + detail;
        }

        private static string FormatAnalyticsText(
            VisionPipelineBatchRunSummaryStorage.BatchRunStatistics statistics,
            IReadOnlyList<OpenVisionRecipeBatchSampleResultOption> judgmentResults)
        {
            if (statistics == null || statistics.ResultCount <= 0)
            {
                return string.Empty;
            }

            int judgedCount = judgmentResults?.Count(result => result?.HasExpectedOutcome == true) ?? 0;
            int misclassified = judgmentResults?.Count(result =>
                result?.HasExpectedOutcome == true && !result.JudgmentCorrect) ?? 0;
            double failureRate = judgedCount > 0
                ? misclassified * 100.0 / judgedCount
                : statistics.FailureRatePercent;
            string correctness = OpenVisionRecipeText.Local("판정 실패율 ", "Judgment failure ")
                + failureRate.ToString("0.0", CultureInfo.CurrentCulture)
                + "%";
            if (statistics.TimingCount <= 0)
            {
                return correctness + " | " + OpenVisionRecipeText.Local("성능 기록 없음", "No performance timing");
            }

            return correctness
                + " | "
                + OpenVisionRecipeText.Local("성능 평균 ", "Performance avg ")
                + statistics.AverageMilliseconds.ToString("0.0", CultureInfo.CurrentCulture)
                + " ms · "
                + OpenVisionRecipeText.Local("중앙 ", "median ")
                + statistics.MedianMilliseconds.ToString("0.0", CultureInfo.CurrentCulture)
                + " ms · p95 "
                + statistics.P95Milliseconds.ToString("0.0", CultureInfo.CurrentCulture)
                + " ms · "
                + OpenVisionRecipeText.Local("최대 ", "max ")
                + statistics.MaximumMilliseconds.ToString("0.0", CultureInfo.CurrentCulture)
                + " ms";
        }

        private static string FormatBatchRunDetail(
            VisionPipelineBatchRunSummaryStorage.BatchRunSummaryInfo summary,
            VisionPipelineBatchRunSummary runSummary,
            IReadOnlyList<OpenVisionRecipeBatchSampleResultOption> sampleResults)
        {
            if (sampleResults?.Any(result => result?.HasExpectedOutcome == true) == true)
            {
                int falseAccepts = sampleResults?.Count(result => result?.IsFalseAccept == true) ?? 0;
                int falseRejects = sampleResults?.Count(result => result?.IsFalseReject == true) ?? 0;
                int misclassified = falseAccepts + falseRejects;
                string partial = string.Equals(
                    runSummary?.SuiteKind,
                    "LocalValidationSetPartial",
                    StringComparison.OrdinalIgnoreCase)
                    ? OpenVisionRecipeText.Local("부분 결과 · ", "Partial result · ")
                    : string.Empty;
                return partial
                    + (misclassified == 0
                        ? OpenVisionRecipeText.Local("오판 없음", "No misclassifications")
                        : OpenVisionRecipeText.Local("오판 ", "Misclassified ")
                            + misclassified.ToString(CultureInfo.InvariantCulture)
                            + " ("
                            + OpenVisionRecipeText.Local("미검 ", "false accept ")
                            + falseAccepts.ToString(CultureInfo.InvariantCulture)
                            + ", "
                            + OpenVisionRecipeText.Local("과검 ", "false reject ")
                            + falseRejects.ToString(CultureInfo.InvariantCulture)
                            + ")");
            }

            if (summary.FailCount <= 0)
            {
                return OpenVisionRecipeText.Local("모든 샘플 통과", "All samples passed");
            }

            List<VisionPipelineBatchSampleRunResult> failures = runSummary?.Results?
                .Where(result => result != null && !result.Success)
                .Take(2)
                .ToList() ?? new List<VisionPipelineBatchSampleRunResult>();

            if (failures.Count == 0)
            {
                return OpenVisionRecipeText.Local("실패: ", "Fail: ")
                    + summary.FailCount.ToString(CultureInfo.InvariantCulture);
            }

            string failedSamples = string.Join(", ", failures.Select(FormatFailure));
            int remaining = Math.Max(0, summary.FailCount - failures.Count);
            if (remaining > 0)
            {
                failedSamples += " +" + remaining.ToString(CultureInfo.InvariantCulture);
            }

            return OpenVisionRecipeText.Local("실패 샘플: ", "Failed: ") + failedSamples;
        }

        private static string FormatFailure(VisionPipelineBatchSampleRunResult result)
        {
            string sample = string.IsNullOrWhiteSpace(result.SampleName) ? "-" : result.SampleName.Trim();
            string step = string.IsNullOrWhiteSpace(result.FailedStep) ? string.Empty : " @ " + result.FailedStep.Trim();
            return sample + step;
        }

        private static IReadOnlyList<OpenVisionRecipeBatchSampleResultOption> BuildSampleResults(VisionPipelineBatchRunSummary runSummary)
        {
            Dictionary<int, VisionPipelineBatchReviewQueueEntry> queueByIndex = (runSummary?.ReviewQueue
                    ?? new List<VisionPipelineBatchReviewQueueEntry>())
                .Where(entry => entry != null)
                .GroupBy(entry => entry.ResultIndex)
                .ToDictionary(group => group.Key, group => group.First());
            List<OpenVisionRecipeBatchSampleResultOption> results = runSummary?.Results?
                .Select((result, index) => new { Result = result, Index = index })
                .Where(item => item.Result != null)
                .Select(item =>
                {
                    queueByIndex.TryGetValue(item.Index, out VisionPipelineBatchReviewQueueEntry entry);
                    return OpenVisionRecipeBatchSampleResultOption.Create(item.Result, entry?.Reasons);
                })
                .ToList() ?? new List<OpenVisionRecipeBatchSampleResultOption>();

            if (results.Count == 0)
            {
                results.Add(OpenVisionRecipeBatchSampleResultOption.CreateEmpty());
            }

            return results;
        }

        public static OpenVisionRecipeBatchRunOption CreateEmpty()
        {
            return new OpenVisionRecipeBatchRunOption(
                OpenVisionRecipeText.Local("저장된 쌍 검사 이력이 없습니다.", "No saved pair check runs."),
                OpenVisionRecipeText.Local("쌍 검사를 실행하면 최근 3건이 여기에 표시됩니다.", "Run a pair check to show the latest three runs here."),
                string.Empty,
                new[] { OpenVisionRecipeBatchSampleResultOption.CreateEmpty() },
                new VisionPipelineBatchRunSummaryStorage.BatchRunStatistics(),
                new VisionPipelineBatchRunSummaryStorage.BatchStepTimingAnalysis(),
                null);
        }
    }

    public sealed class OpenVisionRecipeBatchRunComparisonRow
    {
        private OpenVisionRecipeBatchRunComparisonRow(
            string sampleName,
            string stateText,
            string previousText,
            string currentText,
            string failedStep,
            string sampleImagePath,
            string reviewText,
            bool isComparable,
            bool isRegression,
            bool isRecovered,
            bool isStillFailing)
        {
            SampleName = sampleName ?? string.Empty;
            StateText = string.IsNullOrWhiteSpace(stateText) ? "-" : stateText.Trim();
            PreviousText = string.IsNullOrWhiteSpace(previousText) ? "-" : previousText.Trim();
            CurrentText = string.IsNullOrWhiteSpace(currentText) ? "-" : currentText.Trim();
            FailedStep = failedStep ?? string.Empty;
            SampleImagePath = sampleImagePath ?? string.Empty;
            ReviewText = string.IsNullOrWhiteSpace(reviewText) ? "-" : reviewText.Trim();
            IsComparable = isComparable;
            IsRegression = isRegression;
            IsRecovered = isRecovered;
            IsStillFailing = isStillFailing;
        }

        public string SampleName { get; }

        public string StateText { get; }

        public string PreviousText { get; }

        public string CurrentText { get; }

        public string FailedStep { get; }

        public string SampleImagePath { get; }

        public string ReviewText { get; }

        public bool IsComparable { get; }

        public bool IsRegression { get; }

        public bool IsRecovered { get; }

        public bool IsStillFailing { get; }

        public string DisplayText => StateText + " | " + (string.IsNullOrWhiteSpace(SampleName) ? "-" : SampleName);

        public string DetailText => PreviousText + " -> " + CurrentText;

        public static OpenVisionRecipeBatchRunComparisonRow Create(
            string sampleName,
            VisionPipelineBatchSampleRunResult previous,
            VisionPipelineBatchSampleRunResult current)
        {
            if (previous == null && current == null)
            {
                return CreateEmpty();
            }

            bool previousExists = previous != null;
            bool currentExists = current != null;
            bool previousSuccess = ResolveComparisonSuccess(previous);
            bool currentSuccess = ResolveComparisonSuccess(current);
            string state;
            bool regression = false;
            bool recovered = false;
            bool stillFailing = false;

            if (!previousExists)
            {
                state = currentSuccess ? "NEW OK" : "NEW NG";
                regression = !currentSuccess;
            }
            else if (!currentExists)
            {
                state = "MISSING";
            }
            else if (previousSuccess && !currentSuccess)
            {
                state = "REGRESSION";
                regression = true;
            }
            else if (!previousSuccess && currentSuccess)
            {
                state = "RECOVERED";
                recovered = true;
            }
            else if (!previousSuccess && !currentSuccess)
            {
                state = "STILL NG";
                stillFailing = true;
            }
            else
            {
                state = "OK";
            }

            string failedStep = !string.IsNullOrWhiteSpace(current?.FailedStep)
                ? current.FailedStep
                : previous?.FailedStep ?? string.Empty;
            string sampleImagePath = !string.IsNullOrWhiteSpace(current?.ReportPath)
                ? current.ReportPath
                : previous?.ReportPath ?? string.Empty;
            string review = BuildReviewText(sampleName, state, previous, current, failedStep);

            return new OpenVisionRecipeBatchRunComparisonRow(
                sampleName,
                state,
                FormatResult(previous),
                FormatResult(current),
                failedStep,
                sampleImagePath,
                review,
                previousExists && currentExists,
                regression,
                recovered,
                stillFailing);
        }

        public static OpenVisionRecipeBatchRunComparisonRow CreateEmpty()
        {
            return new OpenVisionRecipeBatchRunComparisonRow(
                OpenVisionRecipeText.Local("비교 결과 없음", "No comparison results"),
                "-",
                "-",
                "-",
                string.Empty,
                string.Empty,
                OpenVisionRecipeText.Local("비교할 benchmark 결과가 없습니다.", "No benchmark comparison result is available."),
                false,
                false,
                false,
                false);
        }

        public static OpenVisionRecipeBatchRunComparisonRow CreateNoBaseline(string currentRun)
        {
            return new OpenVisionRecipeBatchRunComparisonRow(
                OpenVisionRecipeText.Local("기준 이력 없음", "No baseline run"),
                "NO BASELINE",
                "-",
                string.IsNullOrWhiteSpace(currentRun) ? "-" : currentRun,
                string.Empty,
                string.Empty,
                OpenVisionRecipeText.Local("동일한 검증 세트를 다시 실행한 뒤, 이전 저장 실행을 기준 실행으로 선택하세요.", "Run the same validation suite again, then select an earlier saved run as the baseline."),
                false,
                false,
                false,
                false);
        }

        private static string FormatResult(VisionPipelineBatchSampleRunResult result)
        {
            if (result == null)
            {
                return "-";
            }

            string status;
            if (OpenVisionRecipeBatchSampleResultOption.TryResolveExpectedSuccess(result, out bool expectedSuccess))
            {
                status = OpenVisionRecipeBatchSampleResultOption.FormatJudgmentText(expectedSuccess, result.Success)
                    + " ("
                    + OpenVisionRecipeText.Local("기대 ", "expected ")
                    + (expectedSuccess ? "OK" : "NG")
                    + " → "
                    + OpenVisionRecipeText.Local("실제 ", "actual ")
                    + (result.Success ? "OK" : "NG")
                    + ")";
            }
            else
            {
                status = result.Success ? "OK" : "NG";
            }

            if (!string.IsNullOrWhiteSpace(result.FailedStep))
            {
                status += " @ " + result.FailedStep.Trim();
            }

            return status;
        }

        private static bool ResolveComparisonSuccess(VisionPipelineBatchSampleRunResult result)
        {
            if (result == null)
            {
                return false;
            }

            return OpenVisionRecipeBatchSampleResultOption.TryResolveExpectedSuccess(result, out bool expectedSuccess)
                ? expectedSuccess == result.Success
                : result.Success;
        }

        private static string BuildReviewText(
            string sampleName,
            string state,
            VisionPipelineBatchSampleRunResult previous,
            VisionPipelineBatchSampleRunResult current,
            string failedStep)
        {
            string next;
            if (string.Equals(state, "REGRESSION", StringComparison.OrdinalIgnoreCase)
                || string.Equals(state, "NEW NG", StringComparison.OrdinalIgnoreCase))
            {
                next = OpenVisionRecipeText.Local("신규 실패입니다. 실패 Step과 현재 XML 파라미터를 먼저 확인하세요.", "New failure. Review the failed step and current XML parameters first.");
            }
            else if (string.Equals(state, "STILL NG", StringComparison.OrdinalIgnoreCase))
            {
                next = OpenVisionRecipeText.Local("지속 실패입니다. 기준/현재 실패 Step이 같은지 확인하고 파라미터 조정을 이어가세요.", "Persistent failure. Check whether the failed step is unchanged and continue parameter tuning.");
            }
            else if (string.Equals(state, "RECOVERED", StringComparison.OrdinalIgnoreCase))
            {
                next = OpenVisionRecipeText.Local("복구된 샘플입니다. 변경한 파라미터를 유지하고 다른 NG만 확인하세요.", "Recovered sample. Keep the change and focus on remaining NG samples.");
            }
            else
            {
                next = OpenVisionRecipeText.Local("회귀 없음. 다른 Regression/Still NG 항목을 우선 확인하세요.", "No regression. Prioritize Regression or Still NG rows.");
            }

            return OpenVisionRecipeText.Local("샘플: ", "Sample: ") + (string.IsNullOrWhiteSpace(sampleName) ? "-" : sampleName)
                + Environment.NewLine
                + "Diff: " + state
                + Environment.NewLine
                + OpenVisionRecipeText.Local("이전: ", "Previous: ") + FormatResult(previous)
                + Environment.NewLine
                + OpenVisionRecipeText.Local("현재: ", "Current: ") + FormatResult(current)
                + Environment.NewLine
                + OpenVisionRecipeText.Local("실패 Step: ", "Failed step: ") + (string.IsNullOrWhiteSpace(failedStep) ? "-" : failedStep)
                + Environment.NewLine
                + OpenVisionRecipeText.Local("다음: ", "Next: ") + next;
        }
    }

    public sealed class OpenVisionRecipeBatchSampleResultOption
    {
        private OpenVisionRecipeBatchSampleResultOption(
            string displayText,
            string detailText,
            string reviewText,
            bool success,
            bool hasExpectedOutcome,
            bool expectedSuccess,
            string failedStep,
            string sampleName,
            string reportPath,
            string sampleImagePath,
            string runReportPath,
            IReadOnlyList<string> reviewReasons)
        {
            DisplayText = displayText ?? string.Empty;
            DetailText = detailText ?? string.Empty;
            ReviewText = reviewText ?? string.Empty;
            Success = success;
            HasExpectedOutcome = hasExpectedOutcome;
            ExpectedSuccess = expectedSuccess;
            FailedStep = failedStep ?? string.Empty;
            SampleName = sampleName ?? string.Empty;
            ReportPath = reportPath ?? string.Empty;
            SampleImagePath = sampleImagePath ?? string.Empty;
            RunReportPath = runReportPath ?? string.Empty;
            ReviewReasons = reviewReasons ?? Array.Empty<string>();
            ReviewReasonsText = FormatReviewReasons(ReviewReasons, compact: true);
            ReviewReasonsToolTipText = FormatReviewReasons(ReviewReasons, compact: false);
        }

        public string DisplayText { get; }

        public string DetailText { get; }

        public string ReviewText { get; }

        public bool Success { get; }

        public bool HasExpectedOutcome { get; }

        public bool ExpectedSuccess { get; }

        public bool JudgmentCorrect => !HasExpectedOutcome || ExpectedSuccess == Success;

        public bool IsFalseAccept => HasExpectedOutcome && !ExpectedSuccess && Success;

        public bool IsFalseReject => HasExpectedOutcome && ExpectedSuccess && !Success;

        public string FailedStep { get; }

        public string SampleName { get; }

        public string ReportPath { get; }

        // ReportPath predates persisted per-sample run reports and can point to the source image.
        // Keep the explicit paths so a Run History row can reopen its stored drawing evidence.
        public string SampleImagePath { get; }

        public string RunReportPath { get; }

        public IReadOnlyList<string> ReviewReasons { get; }

        public bool IsInReviewQueue => ReviewReasons.Count > 0;

        public string ReviewReasonsText { get; }

        public string ReviewReasonsToolTipText { get; }

        internal static OpenVisionRecipeBatchSampleResultOption Create(
            VisionPipelineBatchSampleRunResult result,
            IReadOnlyList<string> reviewReasons = null)
        {
            if (result == null)
            {
                return CreateEmpty();
            }

            bool hasExpectedOutcome = TryResolveExpectedSuccess(result, out bool expectedSuccess);
            string actualStatus = result.Success ? "OK" : "NG";
            string judgment = hasExpectedOutcome
                ? FormatJudgmentText(expectedSuccess, result.Success)
                : actualStatus;
            string expectedActual = hasExpectedOutcome
                ? OpenVisionRecipeText.Local("기대 ", "Expected ")
                    + (expectedSuccess ? "OK" : "NG")
                    + " → "
                    + OpenVisionRecipeText.Local("실제 ", "Actual ")
                    + actualStatus
                    + " | "
                : string.Empty;
            string display = judgment
                + " | "
                + expectedActual
                + (string.IsNullOrWhiteSpace(result.SampleName) ? "-" : result.SampleName.Trim())
                + " | "
                + result.TotalMilliseconds.ToString("0.0", CultureInfo.InvariantCulture)
                + " ms";
            string detail = string.IsNullOrWhiteSpace(result.FailedStep)
                ? OpenVisionRecipeText.Local("실패 Step 없음", "No failed step")
                : OpenVisionRecipeText.Local("실패 Step: ", "Failed step: ") + result.FailedStep.Trim();
            if (!string.IsNullOrWhiteSpace(result.Message))
            {
                detail += " | " + result.Message.Trim();
            }

            if (!string.IsNullOrWhiteSpace(result.MetricText))
            {
                detail += " | " + result.MetricText.Trim();
            }

            if (!string.IsNullOrWhiteSpace(result.FinalLayer))
            {
                detail += " | " + OpenVisionRecipeText.Local("최종: ", "Final: ") + result.FinalLayer.Trim();
            }

            string review = hasExpectedOutcome
                ? BuildJudgmentReview(expectedSuccess, result.Success, result.FailedStep)
                : result.Success
                    ? OpenVisionRecipeText.Local("판독: 통과. NG 샘플을 선택하면 실패 Step을 연결합니다.", "Review: Passed. Select an NG sample to link the failed step.")
                    : string.IsNullOrWhiteSpace(result.FailedStep)
                        ? OpenVisionRecipeText.Local("판독: 실패했지만 실패 Step이 기록되지 않았습니다. 실행 로그와 XML 경로를 확인하세요.", "Review: Failed, but no failed step was recorded. Check the run log and XML route.")
                        : OpenVisionRecipeText.Local("판독: 실패 Step을 선택했습니다. 입력/출력 레이어와 파라미터를 XML/Step 탭에서 확인하세요.", "Review: Failed step selected. Check input/output layers and parameters in XML/Steps.");
            if (!string.IsNullOrWhiteSpace(result.MetricReviewText))
            {
                review += Environment.NewLine + result.MetricReviewText.Trim();
            }

            if (!string.IsNullOrWhiteSpace(result.ActionSummary))
            {
                review += Environment.NewLine + OpenVisionRecipeText.Local("실행 요약: ", "Action summary: ") + result.ActionSummary.Trim();
            }

            return new OpenVisionRecipeBatchSampleResultOption(
                display,
                detail,
                review,
                result.Success,
                hasExpectedOutcome,
                expectedSuccess,
                result.FailedStep,
                result.SampleName,
                string.IsNullOrWhiteSpace(result.ReportPath) ? result.SampleImagePath : result.ReportPath,
                result.SampleImagePath,
                result.RunReportPath,
                reviewReasons);
        }

        public static OpenVisionRecipeBatchSampleResultOption CreateEmpty()
        {
            return new OpenVisionRecipeBatchSampleResultOption(
                OpenVisionRecipeText.Local("샘플 결과 없음", "No sample results."),
                OpenVisionRecipeText.Local("쌍 검사 이력을 선택하세요.", "Select a pair check run."),
                OpenVisionRecipeText.Local("판독: 저장된 이력을 선택하세요.", "Review: Select a saved run."),
                true,
                false,
                true,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                Array.Empty<string>());
        }

        private static string FormatReviewReasons(IReadOnlyList<string> reasons, bool compact)
        {
            if (reasons == null || reasons.Count == 0)
            {
                return string.Empty;
            }

            IEnumerable<string> visibleReasons = compact ? reasons.Take(3) : reasons;
            string suffix = compact && reasons.Count > 3
                ? " +" + (reasons.Count - 3).ToString(CultureInfo.InvariantCulture)
                : string.Empty;
            return OpenVisionRecipeText.Local("검토 큐: ", "Review queue: ")
                + string.Join(" · ", visibleReasons.Select(FormatReviewReason))
                + suffix;
        }

        private static string FormatReviewReason(string reason)
        {
            if (string.Equals(reason, "runtime-failure", StringComparison.Ordinal))
            {
                return OpenVisionRecipeText.Local("실행 실패", "runtime failure");
            }

            if (string.Equals(reason, "false-accept", StringComparison.Ordinal))
            {
                return OpenVisionRecipeText.Local("미검출", "false accept");
            }

            if (string.Equals(reason, "false-reject", StringComparison.Ordinal))
            {
                return OpenVisionRecipeText.Local("과검출", "false reject");
            }

            if (string.Equals(reason, "evidence-gap", StringComparison.Ordinal))
            {
                return OpenVisionRecipeText.Local("도면 증거 없음", "drawing evidence missing");
            }

            if (reason?.StartsWith("metric-min:", StringComparison.Ordinal) == true)
            {
                return OpenVisionRecipeText.Local("최솟값 ", "minimum ") + reason.Substring("metric-min:".Length);
            }

            if (reason?.StartsWith("metric-max:", StringComparison.Ordinal) == true)
            {
                return OpenVisionRecipeText.Local("최댓값 ", "maximum ") + reason.Substring("metric-max:".Length);
            }

            if (reason?.StartsWith("hash-audit:", StringComparison.Ordinal) == true)
            {
                return OpenVisionRecipeText.Local("해시 표본 ", "hash audit ") + reason.Substring("hash-audit:".Length);
            }

            return reason ?? string.Empty;
        }

        internal static bool TryResolveExpectedSuccess(
            VisionPipelineBatchSampleRunResult result,
            out bool expectedSuccess)
        {
            string expected = result?.ExpectedText?.Trim() ?? string.Empty;
            if (!expected.StartsWith("ExpectedActual:", StringComparison.OrdinalIgnoreCase))
            {
                expectedSuccess = false;
                return false;
            }

            string role = result?.PairRole?.Trim();
            if (string.Equals(role, "OK", StringComparison.OrdinalIgnoreCase))
            {
                expectedSuccess = true;
                return true;
            }

            if (string.Equals(role, "NG", StringComparison.OrdinalIgnoreCase))
            {
                expectedSuccess = false;
                return true;
            }

            if (expected.EndsWith("OK", StringComparison.OrdinalIgnoreCase))
            {
                expectedSuccess = true;
                return true;
            }

            if (expected.EndsWith("NG", StringComparison.OrdinalIgnoreCase))
            {
                expectedSuccess = false;
                return true;
            }

            expectedSuccess = false;
            return false;
        }

        internal static string FormatJudgmentText(bool expectedSuccess, bool actualSuccess)
        {
            if (expectedSuccess)
            {
                return actualSuccess
                    ? OpenVisionRecipeText.Local("정상 수용", "Correct accept")
                    : OpenVisionRecipeText.Local("과검", "False reject");
            }

            return actualSuccess
                ? OpenVisionRecipeText.Local("미검", "False accept")
                : OpenVisionRecipeText.Local("정상 거부", "Correct reject");
        }

        private static string BuildJudgmentReview(
            bool expectedSuccess,
            bool actualSuccess,
            string failedStep)
        {
            if (expectedSuccess == actualSuccess)
            {
                return OpenVisionRecipeText.Local(
                    "판독: 기대 결과와 실제 Pipeline 판정이 일치합니다.",
                    "Review: Expected outcome matches the actual Pipeline decision.");
            }

            if (!expectedSuccess)
            {
                return OpenVisionRecipeText.Local(
                    "판독: 미검입니다. 기대 NG를 실제 OK로 수용했습니다. 판정 기준과 결함 검출 지표를 확인하세요.",
                    "Review: False accept. Expected NG was accepted as actual OK. Review the gate and defect metrics.");
            }

            return string.IsNullOrWhiteSpace(failedStep)
                ? OpenVisionRecipeText.Local(
                    "판독: 과검입니다. 기대 OK를 실제 NG로 거부했지만 실패 Step이 기록되지 않았습니다.",
                    "Review: False reject. Expected OK was rejected as actual NG, but no failed Step was recorded.")
                : OpenVisionRecipeText.Local(
                    "판독: 과검입니다. 기대 OK를 거부한 실패 Step의 판정 기준을 확인하세요.",
                    "Review: False reject. Review the failed Step gate that rejected expected OK.");
        }
    }

    public sealed class OpenVisionRecipeSampleOption
    {
        internal OpenVisionRecipeSampleOption(VisionPipelineSampleCatalogItem sample)
        {
            Sample = sample;
            SampleName = sample?.SampleName ?? string.Empty;
            PipelinePath = sample?.PipelineFullPath ?? string.Empty;
            DisplayText = FormatDisplayText(sample);
            DetailText = sample?.RecipeGuideText ?? string.Empty;
            AcceptanceSummaryText = FormatAcceptanceSummary(sample);
        }

        internal VisionPipelineSampleCatalogItem Sample { get; }

        public string SampleName { get; }

        public string PipelinePath { get; }

        public string DisplayText { get; }

        public string DetailText { get; }

        public string AcceptanceSummaryText { get; }

        private static string FormatDisplayText(VisionPipelineSampleCatalogItem sample)
        {
            if (sample == null)
            {
                return "-";
            }

            string source = string.IsNullOrWhiteSpace(sample.CatalogSourceId) ? "sample" : sample.CatalogSourceId;
            return "[" + source + "] " + sample.SampleName;
        }

        private static string FormatAcceptanceSummary(VisionPipelineSampleCatalogItem sample)
        {
            if (sample == null)
            {
                return "-";
            }

            List<string> lines = new List<string>
            {
                OpenVisionRecipeText.Local("샘플: ", "Sample: ") + Shorten(sample.SampleName, 48),
                OpenVisionRecipeText.Local("모드: ", "Mode: ") + (string.IsNullOrWhiteSpace(sample.ValidationMode) ? "-" : sample.ValidationMode.Trim()),
                OpenVisionRecipeText.Local("기대값: ", "Expected: ") + (string.IsNullOrWhiteSpace(sample.ExpectedText) ? "-" : sample.ExpectedText)
            };

            if (sample.HasPair)
            {
                lines.Add(OpenVisionRecipeText.Local("쌍: ", "Pair: ") + sample.PairText);
            }

            string checkGuide = sample.CheckGuideText;
            if (!string.IsNullOrWhiteSpace(checkGuide) && checkGuide != "-")
            {
                lines.Add(checkGuide);
            }

            string fixGuide = sample.FixGuideText;
            if (!string.IsNullOrWhiteSpace(fixGuide) && fixGuide != "-")
            {
                lines.Add(fixGuide);
            }

            return string.Join(Environment.NewLine, lines);
        }

        private static string Shorten(string value, int maxLength)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return "-";
            }

            string text = value.Trim();
            if (text.Length <= maxLength)
            {
                return text;
            }

            return text.Substring(0, Math.Max(1, maxLength - 3)) + "...";
        }
    }
}
