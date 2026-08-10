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
            OpenVisionLanguage language,
            out string error)
        {
            error = string.Empty;
            try
            {
                VisionPipelineBatchRunSummary summary =
                    VisionPipelineBatchRunSummaryStorage.Load(batchSummaryPath)
                    ?? throw new InvalidOperationException(Text(language, "N장 검증 요약을 읽지 못했습니다.", "Could not read the N-image verification summary."));
                if (summary.Results == null || summary.Results.Count == 0)
                {
                    throw new InvalidOperationException(Text(language, "보고서로 내보낼 결과가 없습니다.", "There are no results to export."));
                }

                List<EvidenceRow> rows = CreateEvidenceRows(summary, language);
                ApplyReviewQueue(summary, rows, language);
                string html = BuildHtml(summary, rows, pipelineXml, stepDefinitionSha256, language);
                string fullPath = Path.GetFullPath(reportPath ?? string.Empty);
                string directory = Path.GetDirectoryName(fullPath)
                    ?? throw new InvalidOperationException(Text(language, "보고서 폴더를 확인하지 못했습니다.", "Could not resolve the report folder."));
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

        private static List<EvidenceRow> CreateEvidenceRows(
            VisionPipelineBatchRunSummary summary,
            OpenVisionLanguage language)
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
                    evidenceStates.Add(Text(language, "실행 보고서 없음", "run report missing"));
                }

                if (!sourceVerified)
                {
                    evidenceStates.Add(Text(language, "입력 스냅샷/해시 불일치", "source snapshot/hash mismatch"));
                }

                if (string.IsNullOrWhiteSpace(drawingPath))
                {
                    evidenceStates.Add(Text(language, "결과 드로잉 없음", "result drawing missing"));
                }

                rows.Add(new EvidenceRow
                {
                    Index = index + 1,
                    Result = result ?? new VisionPipelineBatchSampleRunResult(),
                    SourcePath = sourceVerified ? sourcePath : string.Empty,
                    DrawingPath = drawingPath,
                    SourceSha256 = runReport?.SourceImageSha256 ?? string.Empty,
                    EvidenceState = evidenceStates.Count == 0
                        ? Text(language, "보존 증거 확인됨", "retained evidence verified")
                        : string.Join("; ", evidenceStates)
                });
            }

            return rows;
        }

        private static void ApplyReviewQueue(
            VisionPipelineBatchRunSummary summary,
            IReadOnlyList<EvidenceRow> rows,
            OpenVisionLanguage language)
        {
            if (rows.Count <= 24)
            {
                foreach (EvidenceRow row in rows)
                {
                    row.ReviewReason = Text(language, "전체 행(N ≤ 24)", "all rows (N ≤ 24)");
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

                rows[entry.ResultIndex].ReviewReason = string.Join(
                    ", ",
                    (entry.Reasons ?? new List<string>()).Select(reason => TranslateReason(reason, language)));
            }
        }

        private static string BuildHtml(
            VisionPipelineBatchRunSummary summary,
            IReadOnlyList<EvidenceRow> rows,
            string pipelineXml,
            string stepDefinitionSha256,
            OpenVisionLanguage language)
        {
            List<EvidenceRow> reviewRows = rows
                .Where(row => !string.IsNullOrWhiteSpace(row.ReviewReason))
                .OrderBy(row => row.Index)
                .ToList();
            double averageMilliseconds = rows.Average(row => row.Result.TotalMilliseconds);
            double maximumMilliseconds = rows.Max(row => row.Result.TotalMilliseconds);
            string lang = language == OpenVisionLanguage.Korean ? "ko" : "en";
            StringBuilder html = new StringBuilder();
            html.Append("<!doctype html><html lang=\"").Append(lang).AppendLine("\"><head><meta charset=\"utf-8\"><meta name=\"viewport\" content=\"width=device-width,initial-scale=1\">");
            html.Append("<title>").Append(Encode(Text(language, "OpenVisionLab N장 검증 보고서", "OpenVisionLab N-image verification report"))).AppendLine("</title>");
            html.AppendLine("<style>body{margin:0;background:#08111f;color:#eef5ff;font-family:'Segoe UI','Malgun Gothic',sans-serif;line-height:1.48}.wrap{max-width:1500px;margin:auto;padding:28px}.panel{background:#111d2e;border:1px solid #2b3d56;border-radius:13px;padding:18px;margin:14px 0}.cards{display:grid;grid-template-columns:repeat(5,minmax(0,1fr));gap:10px}.card{min-width:0;background:#17263b;border-radius:9px;padding:13px}.value{font-size:22px;font-weight:800;overflow-wrap:anywhere;word-break:break-word}.label{color:#a9bad0;font-size:12px}.ok{color:#6ce9bb}.bad{color:#ff7b83}.warn{color:#ffc36c}.gallery{display:grid;grid-template-columns:repeat(2,minmax(0,1fr));gap:12px}.shot{background:#050a11;border:1px solid #31445d;border-radius:10px;padding:9px}.pair{display:grid;grid-template-columns:1fr 1fr;gap:8px}.pair img{width:100%;height:260px;object-fit:contain;background:#02050a}.caption{padding:8px 2px 0;color:#d7e4f4;font-size:12px}.scroll{overflow:auto}table{width:100%;border-collapse:collapse;font-size:12px}th,td{padding:8px;border-bottom:1px solid #2b3d56;text-align:left;white-space:nowrap}th{background:#1b2b42;position:sticky;top:0}code,pre{color:#c8e2ff;white-space:pre-wrap;word-break:break-all}.print{float:right;background:#24405e;color:#fff;border:1px solid #5e789b;border-radius:9px;padding:9px 13px}@media(max-width:1000px){.cards{grid-template-columns:repeat(2,1fr)}.gallery{grid-template-columns:1fr}.pair img{height:210px}}@media print{body{background:#fff;color:#111}.panel,.card,.shot{background:#fff;border-color:#aaa}.print{display:none}}</style></head><body><main class=\"wrap\">");
            html.Append("<button class=\"print\" onclick=\"window.print()\">").Append(Encode(Text(language, "인쇄 / PDF 저장", "Print / Save PDF"))).AppendLine("</button>");
            html.Append("<h1>").Append(Encode(Text(language, "Tool View N장 검증 보고서", "Tool View N-image verification report"))).Append("</h1><p>")
                .Append(Encode(summary.SuiteName))
                .Append(" · ")
                .Append(Encode(Text(language,
                    "현재 Tool View 설정을 한 Step으로 고정해 순차 실행한 보존 결과입니다. 이 문서를 열어도 검사를 다시 실행하지 않습니다.",
                    "Retained results from sequential execution with the current Tool View frozen as one Step. Opening this document does not rerun the inspection.")))
                .AppendLine("</p>");
            html.AppendLine("<section class=\"cards\">");
            AppendCard(html, summary.TotalCount.ToString("N0", CultureInfo.InvariantCulture), Text(language, "전체", "Total"));
            AppendCard(html, summary.PassCount.ToString("N0", CultureInfo.InvariantCulture), Text(language, "OK", "OK"));
            AppendCard(html, summary.FailCount.ToString("N0", CultureInfo.InvariantCulture), Text(language, "NG / 오류", "NG / error"));
            AppendCard(html, averageMilliseconds.ToString("0.###", CultureInfo.InvariantCulture) + " ms", Text(language, "평균", "Average"));
            AppendCard(html, maximumMilliseconds.ToString("0.###", CultureInfo.InvariantCulture) + " ms", Text(language, "최대", "Maximum"));
            AppendCard(html, reviewRows.Count.ToString("N0", CultureInfo.InvariantCulture), Text(language, "우선 검토", "Priority review"));
            AppendCard(html, Encode(summary.SuiteKind), Text(language, "검증 유형", "Suite kind"));
            AppendCard(html, Encode(summary.ReviewQueueSha256), Text(language, "검토 큐 SHA-256", "Review queue SHA-256"));
            AppendCard(html, Encode(stepDefinitionSha256), Text(language, "Step 정의 SHA-256", "Step definition SHA-256"));
            AppendCard(html, Text(language, "순차", "Sequential"), Text(language, "실행 방식", "Execution mode"));
            html.AppendLine("</section>");

            html.Append("<section class=\"panel\"><h2>").Append(Encode(Text(language, "판정 범위", "Decision scope"))).Append("</h2><p>")
                .Append(Encode(Text(language,
                    "OK/NG 기준이 설정된 Step은 해당 기준으로 판정합니다. 판정 기준이 없는 Step은 RUN OK로 표시하며, 검출 드로잉을 작업자가 확인해야 합니다.",
                    "Steps with an acceptance gate use that gate for OK/NG. Steps without a gate are marked RUN OK and require operator review of the result drawing.")))
                .AppendLine("</p></section>");

            html.Append("<section class=\"panel\"><h2>").Append(Encode(Text(language, "우선 검토 증거", "Priority review evidence"))).Append("</h2><p>")
                .Append(Encode(Text(language,
                    "24장 이하는 전체 결과를 표시합니다. 더 큰 묶음은 오류, NG, 증거 누락, 측정 극단값과 해시 감사 대상을 우선 표시합니다.",
                    "For 24 images or fewer, every result is shown. Larger sets prioritize errors, NG results, evidence gaps, metric extremes, and hash-audit samples.")))
                .AppendLine("</p><div class=\"gallery\">");
            foreach (EvidenceRow row in reviewRows)
            {
                html.Append("<article class=\"shot\"><div class=\"pair\">")
                    .Append(CreateImageTag(row.SourcePath, Text(language, "입력 이미지", "Input image"), language))
                    .Append(CreateImageTag(row.DrawingPath, Text(language, "결과 드로잉", "Result drawing"), language))
                    .Append("</div><div class=\"caption\"><strong>#")
                    .Append(row.Index.ToString("0000", CultureInfo.InvariantCulture))
                    .Append(" · ")
                    .Append(Encode(Path.GetFileName(row.Result.SampleImagePath)))
                    .Append("</strong><br>")
                    .Append(Encode(row.Result.Status)).Append(" · ")
                    .Append(Encode(row.Result.MetricText)).Append(" · ")
                    .Append(Encode(row.ReviewReason)).Append("<br>")
                    .Append(Encode(Text(language, "증거: ", "Evidence: ")))
                    .Append(Encode(row.EvidenceState))
                    .AppendLine("</div></article>");
            }
            html.AppendLine("</div></section>");

            html.Append("<section class=\"panel\"><h2>").Append(Encode(Text(language, "전체 결과", "All results"))).Append("</h2><div class=\"scroll\"><table><thead><tr><th>#</th><th>")
                .Append(Encode(Text(language, "파일", "File"))).Append("</th><th>")
                .Append(Encode(Text(language, "판정", "Decision"))).Append("</th><th>")
                .Append(Encode(Text(language, "측정값", "Metric"))).Append("</th><th>ms</th><th>")
                .Append(Encode(Text(language, "검토 이유", "Review reason"))).Append("</th><th>")
                .Append(Encode(Text(language, "입력 SHA-256", "Input SHA-256"))).Append("</th><th>")
                .Append(Encode(Text(language, "증거 상태", "Evidence state"))).Append("</th><th>")
                .Append(Encode(Text(language, "메시지", "Message"))).AppendLine("</th></tr></thead><tbody>");
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

            html.Append("<section class=\"panel\"><h2>").Append(Encode(Text(language, "고정 Step Pipeline XML", "Frozen Step Pipeline XML"))).Append("</h2><p>")
                .Append(Encode(Text(language, "Step 정의 SHA-256: ", "Step definition SHA-256: "))).Append("<code>")
                .Append(Encode(stepDefinitionSha256)).Append("</code></p><pre>")
                .Append(Encode(pipelineXml)).AppendLine("</pre></section>");
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

        private static string CreateImageTag(string path, string alt, OpenVisionLanguage language)
        {
            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
            {
                return "<div class=\"warn\">" + Encode(Text(language, "보존 이미지 없음", "Retained image unavailable")) + "</div>";
            }

            string mime = string.Equals(Path.GetExtension(path), ".jpg", StringComparison.OrdinalIgnoreCase)
                || string.Equals(Path.GetExtension(path), ".jpeg", StringComparison.OrdinalIgnoreCase)
                ? "image/jpeg"
                : "image/png";
            string dataUri = "data:" + mime + ";base64," + Convert.ToBase64String(File.ReadAllBytes(path));
            return "<img alt=\"" + Encode(alt) + "\" src=\"" + dataUri + "\">";
        }

        private static string TranslateReason(string reason, OpenVisionLanguage language)
        {
            if (language != OpenVisionLanguage.Korean || string.IsNullOrWhiteSpace(reason))
            {
                return reason ?? string.Empty;
            }

            return reason switch
            {
                "execution-error" => "실행 오류",
                "runtime-failure" => "런타임 실패",
                "evidence-gap" => "증거 누락",
                _ when reason.StartsWith("metric-min:", StringComparison.Ordinal) => "측정 최솟값:" + reason.Substring("metric-min:".Length),
                _ when reason.StartsWith("metric-max:", StringComparison.Ordinal) => "측정 최댓값:" + reason.Substring("metric-max:".Length),
                _ when reason.StartsWith("hash-audit:", StringComparison.Ordinal) => "해시 감사:" + reason.Substring("hash-audit:".Length),
                _ => reason
            };
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

        private static string Text(OpenVisionLanguage language, string korean, string english)
        {
            return language == OpenVisionLanguage.Korean ? korean : english;
        }

        private static string Encode(string value)
        {
            return WebUtility.HtmlEncode(value ?? string.Empty);
        }
    }
}
