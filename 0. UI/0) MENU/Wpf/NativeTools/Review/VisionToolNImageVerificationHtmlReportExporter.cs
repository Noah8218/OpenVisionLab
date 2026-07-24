using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace OpenVisionLab
{
    internal static class VisionToolNImageVerificationHtmlReportExporter
    {
        private sealed class EvidenceRow
        {
            public int Index { get; init; }
            public VisionPipelineBatchSampleRunResult Result { get; init; }
            public string SourcePath { get; init; } = string.Empty;
            public string DrawingPath { get; init; } = string.Empty;
            public string SourceSha256 { get; init; } = string.Empty;
            public string EvidenceState { get; init; } = string.Empty;
            public string ReviewReason { get; set; } = string.Empty;
        }

        public static bool TryExport(
            string batchSummaryPath,
            string pipelineXml,
            string stepDefinitionSha256,
            string reportPath,
            out string error)
        {
            error = string.Empty;
            try
            {
                VisionPipelineBatchRunSummary summary =
                    VisionPipelineBatchRunSummaryStorage.Load(batchSummaryPath)
                    ?? throw new InvalidOperationException("저장된 N장 검증 요약을 읽지 못했습니다.");
                if (summary.Results == null || summary.Results.Count == 0)
                {
                    throw new InvalidOperationException("보고서로 내보낼 저장 결과가 없습니다.");
                }

                List<EvidenceRow> rows = CreateEvidenceRows(summary);
                ApplyReviewQueue(summary, rows);
                string html = BuildHtml(summary, rows, pipelineXml, stepDefinitionSha256);
                string fullPath = Path.GetFullPath(reportPath ?? string.Empty);
                string directory = Path.GetDirectoryName(fullPath)
                    ?? throw new InvalidOperationException("보고서 폴더를 확인하지 못했습니다.");
                Directory.CreateDirectory(directory);
                string temporaryPath = fullPath + ".tmp";
                File.WriteAllText(temporaryPath, html, new UTF8Encoding(false));
                File.Move(temporaryPath, fullPath, true);
                return true;
            }
            catch (Exception ex)
            {
                error = ex.GetBaseException().Message;
                return false;
            }
        }

        private static List<EvidenceRow> CreateEvidenceRows(VisionPipelineBatchRunSummary summary)
        {
            List<EvidenceRow> rows = new List<EvidenceRow>(summary.Results.Count);
            for (int index = 0; index < summary.Results.Count; index++)
            {
                VisionPipelineBatchSampleRunResult result = summary.Results[index];
                string runReportPath = !string.IsNullOrWhiteSpace(result?.RunReportPath)
                    ? result.RunReportPath
                    : result?.ReportPath;
                VisionPipelineRunReport runReport = VisionPipelineRunReportStorage.Load(runReportPath);
                string runDirectory = string.IsNullOrWhiteSpace(runReportPath)
                    ? string.Empty
                    : Path.GetDirectoryName(runReportPath) ?? string.Empty;
                string sourcePath = ResolveExistingPath(runDirectory, runReport?.SourceImageFile);
                VisionPipelineStepRunReport evidenceStep = runReport?.Steps?
                    .LastOrDefault(step =>
                        !string.IsNullOrWhiteSpace(step?.OverlayImageFile)
                        || !string.IsNullOrWhiteSpace(step?.ResultImageFile))
                    ?? runReport?.Steps?.LastOrDefault();
                string drawingPath = ResolveExistingPath(
                    runDirectory,
                    !string.IsNullOrWhiteSpace(evidenceStep?.OverlayImageFile)
                        ? evidenceStep.OverlayImageFile
                        : evidenceStep?.ResultImageFile);
                bool sourceVerified = VisionPipelineRunReportStorage.IsFileSha256Match(
                    sourcePath,
                    runReport?.SourceImageSha256);
                List<string> evidenceStates = new List<string>();
                if (runReport == null)
                {
                    evidenceStates.Add("run report missing");
                }

                if (!sourceVerified)
                {
                    evidenceStates.Add("source snapshot/hash mismatch");
                }

                if (string.IsNullOrWhiteSpace(drawingPath))
                {
                    evidenceStates.Add("drawing missing");
                }

                rows.Add(new EvidenceRow
                {
                    Index = index + 1,
                    Result = result ?? new VisionPipelineBatchSampleRunResult(),
                    SourcePath = sourceVerified ? sourcePath : string.Empty,
                    DrawingPath = drawingPath,
                    SourceSha256 = runReport?.SourceImageSha256 ?? string.Empty,
                    EvidenceState = evidenceStates.Count == 0
                        ? "verified retained evidence"
                        : string.Join("; ", evidenceStates)
                });
            }

            return rows;
        }

        private static void ApplyReviewQueue(
            VisionPipelineBatchRunSummary summary,
            IReadOnlyList<EvidenceRow> rows)
        {
            if (rows.Count <= 24)
            {
                foreach (EvidenceRow row in rows)
                {
                    row.ReviewReason = "all rows (N <= 24)";
                }

                return;
            }

            foreach (VisionPipelineBatchReviewQueueEntry entry in summary.ReviewQueue
                ?? new List<VisionPipelineBatchReviewQueueEntry>())
            {
                if (entry.ResultIndex < 0 || entry.ResultIndex >= rows.Count)
                {
                    continue;
                }

                rows[entry.ResultIndex].ReviewReason =
                    string.Join(", ", entry.Reasons ?? new List<string>());
            }
        }

        private static string BuildHtml(
            VisionPipelineBatchRunSummary summary,
            IReadOnlyList<EvidenceRow> rows,
            string pipelineXml,
            string stepDefinitionSha256)
        {
            List<EvidenceRow> reviewRows = rows
                .Where(row => !string.IsNullOrWhiteSpace(row.ReviewReason))
                .OrderBy(row => row.Index)
                .ToList();
            double averageMilliseconds = rows.Average(row => row.Result.TotalMilliseconds);
            double maximumMilliseconds = rows.Max(row => row.Result.TotalMilliseconds);
            StringBuilder html = new StringBuilder();
            html.AppendLine("<!doctype html><html lang=\"ko\"><head><meta charset=\"utf-8\"><meta name=\"viewport\" content=\"width=device-width,initial-scale=1\">");
            html.AppendLine("<title>OpenVisionLab Tool View N장 검증 보고서</title>");
            html.AppendLine("<style>body{margin:0;background:#08111f;color:#eef5ff;font-family:'Segoe UI','Malgun Gothic',sans-serif;line-height:1.48}.wrap{max-width:1500px;margin:auto;padding:28px}.panel{background:#111d2e;border:1px solid #2b3d56;border-radius:13px;padding:18px;margin:14px 0}.cards{display:grid;grid-template-columns:repeat(5,minmax(0,1fr));gap:10px}.card{min-width:0;background:#17263b;border-radius:9px;padding:13px}.value{font-size:22px;font-weight:800;overflow-wrap:anywhere;word-break:break-word}.label{color:#a9bad0;font-size:12px}.ok{color:#6ce9bb}.bad{color:#ff7b83}.warn{color:#ffc36c}.gallery{display:grid;grid-template-columns:repeat(2,minmax(0,1fr));gap:12px}.shot{background:#050a11;border:1px solid #31445d;border-radius:10px;padding:9px}.pair{display:grid;grid-template-columns:1fr 1fr;gap:8px}.pair img{width:100%;height:260px;object-fit:contain;background:#02050a}.caption{padding:8px 2px 0;color:#d7e4f4;font-size:12px}.scroll{overflow:auto}table{width:100%;border-collapse:collapse;font-size:12px}th,td{padding:8px;border-bottom:1px solid #2b3d56;text-align:left;white-space:nowrap}th{background:#1b2b42;position:sticky;top:0}code,pre{color:#c8e2ff;white-space:pre-wrap;word-break:break-all}.print{float:right;background:#24405e;color:#fff;border:1px solid #5e789b;border-radius:9px;padding:9px 13px}@media(max-width:1000px){.cards{grid-template-columns:repeat(2,1fr)}.gallery{grid-template-columns:1fr}.pair img{height:210px}}@media print{body{background:#fff;color:#111}.panel,.card,.shot{background:#fff;border-color:#aaa}.print{display:none}}</style></head><body><main class=\"wrap\">");
            html.AppendLine("<button class=\"print\" onclick=\"window.print()\">인쇄 / PDF 저장</button>");
            html.Append("<h1>Tool View N장 검증 보고서</h1><p>")
                .Append(Encode(summary.SuiteName))
                .AppendLine(" · 현재 설정을 한 Step으로 고정해 순차 실행한 저장 결과입니다. 이 문서를 열거나 내보내는 과정에서는 검사를 다시 실행하지 않습니다.</p>");
            html.AppendLine("<section class=\"cards\">");
            AppendCard(html, summary.TotalCount.ToString("N0", CultureInfo.InvariantCulture), "전체");
            AppendCard(html, summary.PassCount.ToString("N0", CultureInfo.InvariantCulture), "실행 OK");
            AppendCard(html, summary.FailCount.ToString("N0", CultureInfo.InvariantCulture), "NG / 오류");
            AppendCard(html, averageMilliseconds.ToString("0.###", CultureInfo.InvariantCulture) + " ms", "평균");
            AppendCard(html, maximumMilliseconds.ToString("0.###", CultureInfo.InvariantCulture) + " ms", "최대");
            AppendCard(html, reviewRows.Count.ToString("N0", CultureInfo.InvariantCulture), "드로잉 검토 큐");
            AppendCard(html, Encode(summary.SuiteKind), "세션 상태");
            AppendCard(html, Encode(summary.ReviewQueueSha256), "검토 큐 SHA-256");
            AppendCard(html, Encode(stepDefinitionSha256), "Step 정의 SHA-256");
            AppendCard(html, "Sequential", "실행 방식");
            html.AppendLine("</section>");

            html.Append("<section class=\"panel\"><h2>범위와 한계</h2><p>")
                .Append(Encode(summary.Notes))
                .AppendLine("</p><p class=\"warn\">이 세션은 각 이미지에서 Tool 실행 성공 여부와 측정값·드로잉을 수집합니다. 작업자가 OK/NG 허용 범위를 입력하지 않았으므로 산업 판정을 자동 추정하지 않습니다.</p></section>");

            html.AppendLine("<section class=\"panel\"><h2>결정적 드로잉 검토 큐</h2><p>24장 이하는 전부, 그보다 많으면 모든 실패·증거 누락·측정 극단·SHA-256 분산 표본을 표시합니다.</p><div class=\"gallery\">");
            foreach (EvidenceRow row in reviewRows)
            {
                html.Append("<article class=\"shot\"><div class=\"pair\">")
                    .Append(CreateImageTag(row.SourcePath, "입력 스냅샷"))
                    .Append(CreateImageTag(row.DrawingPath, "결과 드로잉"))
                    .Append("</div><div class=\"caption\"><strong>#")
                    .Append(row.Index.ToString("0000", CultureInfo.InvariantCulture))
                    .Append(" · ")
                    .Append(Encode(Path.GetFileName(row.Result.SampleImagePath)))
                    .Append("</strong><br>")
                    .Append(Encode(row.Result.Status))
                    .Append(" · ")
                    .Append(Encode(row.Result.MetricText))
                    .Append(" · ")
                    .Append(Encode(row.ReviewReason))
                    .Append("<br>증거: ")
                    .Append(Encode(row.EvidenceState))
                    .AppendLine("</div></article>");
            }
            html.AppendLine("</div></section>");

            html.AppendLine("<section class=\"panel\"><h2>전체 결과</h2><div class=\"scroll\"><table><thead><tr><th>#</th><th>파일</th><th>상태</th><th>측정값</th><th>ms</th><th>검토 이유</th><th>입력 SHA-256</th><th>증거 상태</th><th>메시지</th></tr></thead><tbody>");
            foreach (EvidenceRow row in rows)
            {
                html.Append("<tr><td>").Append(row.Index).Append("</td><td title=\"")
                    .Append(Encode(row.Result.SampleImagePath)).Append("\">")
                    .Append(Encode(Path.GetFileName(row.Result.SampleImagePath))).Append("</td><td class=\"")
                    .Append(row.Result.Success ? "ok" : "bad").Append("\">")
                    .Append(Encode(row.Result.Status)).Append("</td><td>")
                    .Append(Encode(row.Result.MetricText)).Append("</td><td>")
                    .Append(row.Result.TotalMilliseconds.ToString("0.###", CultureInfo.InvariantCulture)).Append("</td><td>")
                    .Append(Encode(row.ReviewReason)).Append("</td><td><code>")
                    .Append(Encode(row.SourceSha256)).Append("</code></td><td>")
                    .Append(Encode(row.EvidenceState)).Append("</td><td>")
                    .Append(Encode(row.Result.Message)).AppendLine("</td></tr>");
            }
            html.AppendLine("</tbody></table></div></section>");

            html.Append("<section class=\"panel\"><h2>고정된 한 Step Pipeline XML</h2><p>Step 정의 SHA-256: <code>")
                .Append(Encode(stepDefinitionSha256))
                .Append("</code></p><pre>")
                .Append(Encode(pipelineXml))
                .AppendLine("</pre></section>");
            html.Append("<footer>OpenVisionLab · retained-result export · ")
                .Append(Encode(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.CurrentCulture)))
                .AppendLine("</footer></main></body></html>");
            return html.ToString();
        }

        private static void AppendCard(StringBuilder html, string value, string label)
        {
            html.Append("<div class=\"card\"><div class=\"value\">")
                .Append(value)
                .Append("</div><div class=\"label\">")
                .Append(Encode(label))
                .AppendLine("</div></div>");
        }

        private static string CreateImageTag(string path, string alt)
        {
            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
            {
                return "<div class=\"warn\">저장 이미지 없음</div>";
            }

            string mime = string.Equals(Path.GetExtension(path), ".jpg", StringComparison.OrdinalIgnoreCase)
                || string.Equals(Path.GetExtension(path), ".jpeg", StringComparison.OrdinalIgnoreCase)
                ? "image/jpeg"
                : "image/png";
            string dataUri = "data:" + mime + ";base64," + Convert.ToBase64String(File.ReadAllBytes(path));
            return "<img alt=\"" + Encode(alt) + "\" src=\"" + dataUri + "\">";
        }

        private static string ResolveExistingPath(string directory, string fileName)
        {
            if (string.IsNullOrWhiteSpace(directory) || string.IsNullOrWhiteSpace(fileName))
            {
                return string.Empty;
            }

            string path = Path.Combine(directory, fileName);
            return File.Exists(path) ? path : string.Empty;
        }

        private static string Encode(string value)
        {
            return WebUtility.HtmlEncode(value ?? string.Empty);
        }
    }
}
