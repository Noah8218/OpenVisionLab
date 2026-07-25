using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace OpenVisionLab
{
    internal sealed class OpenVisionRecipeRunEvidenceDrawing
    {
        internal OpenVisionRecipeRunEvidenceDrawing(
            VisionPipelineStepRunReport step,
            string drawingImagePath)
        {
            Index = step?.Index ?? 0;
            StepName = step?.Name ?? string.Empty;
            ToolType = step?.ToolType ?? string.Empty;
            StepStatus = step?.Status ?? string.Empty;
            AcceptancePassed = step?.AcceptancePassed == true;
            AcceptanceMessage = step?.AcceptanceMessage ?? string.Empty;
            DrawingImagePath = drawingImagePath ?? string.Empty;
            MetricSummaryText = BuildMetricSummary(step?.Metrics);
        }

        public int Index { get; }

        public string StepName { get; }

        public string ToolType { get; }

        public string StepStatus { get; }

        public bool AcceptancePassed { get; }

        public string AcceptanceMessage { get; }

        public string DrawingImagePath { get; }

        public string MetricSummaryText { get; }

        public string StepText => Index.ToString("00", CultureInfo.InvariantCulture)
            + " "
            + (string.IsNullOrWhiteSpace(StepName) ? "-" : StepName);

        public string DisplayText => StepText
            + " | "
            + (string.IsNullOrWhiteSpace(ToolType) ? "-" : ToolType)
            + " | "
            + (string.IsNullOrWhiteSpace(StepStatus) ? "-" : StepStatus);

        public string AcceptanceText => string.IsNullOrWhiteSpace(AcceptanceMessage)
            ? OpenVisionRecipeText.Local("판정 메시지 없음", "No acceptance message")
            : (AcceptancePassed ? "PASS: " : "NG: ") + AcceptanceMessage.Trim();

        private static string BuildMetricSummary(IEnumerable<VisionPipelineMetricRunReport> metrics)
        {
            string text = string.Join(
                " | ",
                (metrics ?? Enumerable.Empty<VisionPipelineMetricRunReport>())
                    .Where(metric => metric != null && !string.IsNullOrWhiteSpace(metric.Name))
                    .OrderBy(metric => metric.Name, StringComparer.OrdinalIgnoreCase)
                    .Select(metric => metric.Name.Trim()
                        + "="
                        + metric.Value.ToString("0.###", CultureInfo.InvariantCulture)));
            return string.IsNullOrWhiteSpace(text) ? "-" : text;
        }
    }

    // Resolves persisted batch evidence only. It deliberately does not run the pipeline again.
    internal sealed class OpenVisionRecipeRunEvidence
    {
        private OpenVisionRecipeRunEvidence(
            string sampleName,
            string originalImagePath,
            string expectedText,
            string actualStatus,
            string judgmentText,
            bool success,
            bool isStoredSourceVerified,
            string sourceProvenanceText,
            IReadOnlyList<OpenVisionRecipeRunEvidenceDrawing> drawings,
            OpenVisionRecipeRunEvidenceDrawing defaultDrawing)
        {
            SampleName = sampleName ?? string.Empty;
            OriginalImagePath = originalImagePath ?? string.Empty;
            ExpectedText = expectedText ?? string.Empty;
            ActualStatus = actualStatus ?? string.Empty;
            JudgmentText = judgmentText ?? string.Empty;
            Success = success;
            IsStoredSourceVerified = isStoredSourceVerified;
            SourceProvenanceText = sourceProvenanceText ?? string.Empty;
            Drawings = drawings ?? Array.Empty<OpenVisionRecipeRunEvidenceDrawing>();
            DefaultDrawing = defaultDrawing;
        }

        public string SampleName { get; }

        public string OriginalImagePath { get; }

        public string ExpectedText { get; }

        public string ActualStatus { get; }

        public string Status => ActualStatus;

        public string JudgmentText { get; }

        public bool Success { get; }

        public bool IsStoredSourceVerified { get; }

        public string SourceProvenanceText { get; }

        public IReadOnlyList<OpenVisionRecipeRunEvidenceDrawing> Drawings { get; }

        public OpenVisionRecipeRunEvidenceDrawing DefaultDrawing { get; }

        // Kept for existing callers that copy the selected report folder.
        public string DrawingImagePath => DefaultDrawing?.DrawingImagePath ?? string.Empty;

        public string StepText => DefaultDrawing?.StepText ?? string.Empty;

        public string StatusText => BuildStatusText(DefaultDrawing);

        public string BuildStatusText(OpenVisionRecipeRunEvidenceDrawing drawing)
        {
            List<string> lines = new List<string>
            {
                OpenVisionRecipeText.Local("샘플: ", "Sample: ") + (string.IsNullOrWhiteSpace(SampleName) ? "-" : SampleName),
                OpenVisionRecipeText.Local("판정: ", "Judgment: ") + (string.IsNullOrWhiteSpace(JudgmentText) ? "-" : JudgmentText)
                    + " | "
                    + OpenVisionRecipeText.Local("기대 ", "Expected ") + (string.IsNullOrWhiteSpace(ExpectedText) ? "-" : ExpectedText)
                    + " | "
                    + OpenVisionRecipeText.Local("실제 ", "Actual ") + (string.IsNullOrWhiteSpace(ActualStatus) ? "-" : ActualStatus)
            };

            if (drawing != null)
            {
                lines.Add(
                    OpenVisionRecipeText.Local("선택 Step: ", "Selected Step: ")
                    + drawing.DisplayText
                    + " | "
                    + OpenVisionRecipeText.Local("판정 기준: ", "Acceptance: ")
                    + drawing.AcceptanceText);
                lines.Add(OpenVisionRecipeText.Local("지표: ", "Metrics: ") + drawing.MetricSummaryText);
            }

            lines.Add(OpenVisionRecipeText.Local("원본 근거: ", "Source evidence: ")
                + (string.IsNullOrWhiteSpace(SourceProvenanceText) ? "-" : SourceProvenanceText));
            lines.Add(OpenVisionRecipeText.Local(
                "원본과 저장된 검출 드로잉을 읽기 전용으로 비교합니다. Preview 또는 Run을 다시 실행하지 않습니다.",
                "Read-only comparison of the original and persisted detection drawing. Preview and Run are not rerun."));
            return string.Join(Environment.NewLine, lines);
        }

        public static bool TryCreate(
            OpenVisionRecipeBatchSampleResultOption selectedSample,
            out OpenVisionRecipeRunEvidence evidence,
            out string reason)
        {
            evidence = null;
            reason = string.Empty;

            if (selectedSample == null || string.IsNullOrWhiteSpace(selectedSample.SampleName))
            {
                reason = OpenVisionRecipeText.Local(
                    "배치 이력 샘플을 먼저 선택하세요.",
                    "Select a batch-history sample first.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(selectedSample.RunReportPath)
                || !File.Exists(selectedSample.RunReportPath))
            {
                reason = OpenVisionRecipeText.Local(
                    "이 샘플의 저장된 Run Report를 찾을 수 없습니다. 원본 이미지로 다시 검증한 뒤 화면을 여세요.",
                    "The stored Run Report is unavailable. Rerun this source image before opening its drawing.");
                return false;
            }

            VisionPipelineRunReport report = VisionPipelineRunReportStorage.Load(selectedSample.RunReportPath);
            if (report?.Steps == null || report.Steps.Count == 0)
            {
                reason = OpenVisionRecipeText.Local(
                    "저장된 Run Report에 표시할 Step 화면이 없습니다.",
                    "The stored Run Report has no step drawing to display.");
                return false;
            }

            string reportDirectory = Path.GetDirectoryName(selectedSample.RunReportPath) ?? string.Empty;
            string storedSourcePath = ResolveReportImagePath(reportDirectory, report.SourceImageFile);
            bool hasStoredSource = !string.IsNullOrWhiteSpace(storedSourcePath);
            bool isStoredSourceVerified = hasStoredSource
                && VisionPipelineRunReportStorage.IsFileSha256Match(
                    storedSourcePath,
                    report.SourceImageSha256);
            bool declaresStoredSource = !string.IsNullOrWhiteSpace(report.SourceImageFile)
                || !string.IsNullOrWhiteSpace(report.SourceImageSha256);
            if (declaresStoredSource && !isStoredSourceVerified)
            {
                reason = OpenVisionRecipeText.Local(
                    "저장된 실행 시점 source snapshot 또는 SHA-256을 확인할 수 없습니다.",
                    "The stored run-time source snapshot or SHA-256 could not be verified.");
                return false;
            }

            string originalPath = isStoredSourceVerified
                ? storedSourcePath
                : FirstExistingPath(selectedSample.SampleImagePath, selectedSample.ReportPath);
            if (string.IsNullOrWhiteSpace(originalPath))
            {
                reason = OpenVisionRecipeText.Local(
                    "선택한 샘플의 원본 이미지 경로를 찾을 수 없습니다.",
                    "The selected sample's original image path is unavailable.");
                return false;
            }

            string sourceProvenanceText = isStoredSourceVerified
                ? OpenVisionRecipeText.Local(
                    "실행 시점 source snapshot / SHA-256 확인",
                    "Run-time source snapshot / SHA-256 verified")
                : OpenVisionRecipeText.Local(
                    "레거시 경고: 실행 시점 snapshot 없음; 현재 외부 이미지를 표시",
                    "LEGACY WARNING: no run-time snapshot; showing the current external image");

            List<OpenVisionRecipeRunEvidenceDrawing> drawings = report.Steps
                .Where(step => step != null)
                .Select(step => new
                {
                    Step = step,
                    DrawingPath = FirstExistingPath(
                        ResolveReportImagePath(reportDirectory, step.OverlayImageFile),
                        ResolveReportImagePath(reportDirectory, step.ResultImageFile))
                })
                .Where(item => !string.IsNullOrWhiteSpace(item.DrawingPath))
                .Select(item => new OpenVisionRecipeRunEvidenceDrawing(item.Step, item.DrawingPath))
                .ToList();
            if (drawings.Count == 0)
            {
                reason = OpenVisionRecipeText.Local(
                    "저장된 Run Report에 열 수 있는 드로잉 또는 결과 이미지가 없습니다.",
                    "The stored Run Report has no drawing or result image to open.");
                return false;
            }

            OpenVisionRecipeRunEvidenceDrawing defaultDrawing = ResolveDefaultDrawing(drawings, selectedSample.FailedStep);
            string expectedText = selectedSample.HasExpectedOutcome
                ? (selectedSample.ExpectedSuccess ? "OK" : "NG")
                : "-";
            string actualStatus = selectedSample.Success ? "OK" : "NG";
            string judgmentText = selectedSample.HasExpectedOutcome
                ? OpenVisionRecipeBatchSampleResultOption.FormatJudgmentText(selectedSample.ExpectedSuccess, selectedSample.Success)
                : actualStatus;
            evidence = new OpenVisionRecipeRunEvidence(
                selectedSample.SampleName,
                originalPath,
                expectedText,
                actualStatus,
                judgmentText,
                selectedSample.Success,
                isStoredSourceVerified,
                sourceProvenanceText,
                drawings.AsReadOnly(),
                defaultDrawing);
            return true;
        }

        private static OpenVisionRecipeRunEvidenceDrawing ResolveDefaultDrawing(
            IReadOnlyList<OpenVisionRecipeRunEvidenceDrawing> drawings,
            string failedStep)
        {
            if (!string.IsNullOrWhiteSpace(failedStep))
            {
                string indexToken = failedStep.TrimStart()
                    .Split(new[] { ' ', '\t', ':', '|' }, StringSplitOptions.RemoveEmptyEntries)
                    .FirstOrDefault();
                if (int.TryParse(indexToken, NumberStyles.Integer, CultureInfo.InvariantCulture, out int failedIndex))
                {
                    OpenVisionRecipeRunEvidenceDrawing indexed = drawings.FirstOrDefault(drawing => drawing.Index == failedIndex);
                    if (indexed != null)
                    {
                        return indexed;
                    }
                }
            }

            OpenVisionRecipeRunEvidenceDrawing failedStatus = drawings.FirstOrDefault(drawing =>
                    string.Equals(drawing.StepStatus, "NG", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(drawing.StepStatus, "TIMEOUT", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(drawing.StepStatus, "ERROR", StringComparison.OrdinalIgnoreCase));
            if (failedStatus != null)
            {
                return failedStatus;
            }

            if (!string.IsNullOrWhiteSpace(failedStep))
            {
                string normalizedFailedStep = failedStep.Trim();
                OpenVisionRecipeRunEvidenceDrawing exactName = drawings.FirstOrDefault(drawing =>
                    string.Equals(drawing.StepName, normalizedFailedStep, StringComparison.OrdinalIgnoreCase));
                if (exactName != null)
                {
                    return exactName;
                }

                OpenVisionRecipeRunEvidenceDrawing nameMatch = drawings
                    .Where(drawing => !string.IsNullOrWhiteSpace(drawing.StepName)
                        && failedStep.IndexOf(drawing.StepName, StringComparison.OrdinalIgnoreCase) >= 0)
                    .OrderByDescending(drawing => drawing.StepName.Length)
                    .FirstOrDefault();
                if (nameMatch != null)
                {
                    return nameMatch;
                }
            }

            return drawings.LastOrDefault();
        }

        private static string ResolveReportImagePath(string reportDirectory, string imageFile)
        {
            if (string.IsNullOrWhiteSpace(imageFile))
            {
                return string.Empty;
            }

            string path = Path.IsPathRooted(imageFile)
                ? imageFile
                : Path.Combine(reportDirectory, imageFile);
            return File.Exists(path) ? path : string.Empty;
        }

        private static string FirstExistingPath(params string[] paths)
        {
            return paths?.FirstOrDefault(path => !string.IsNullOrWhiteSpace(path) && File.Exists(path)) ?? string.Empty;
        }
    }
}
