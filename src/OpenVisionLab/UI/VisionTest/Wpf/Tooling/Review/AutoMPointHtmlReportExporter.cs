using Lib.Common;
using Lib.OpenCV.Result;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace OpenVisionLab
{
    internal static class AutoMPointHtmlReportExporter
    {
        private sealed class ReportRow
        {
            public int Index { get; init; }
            public string ImagePath { get; init; } = string.Empty;
            public string ImageSha256 { get; init; } = string.Empty;
            public AutoMPointRepresentativeMatchResult Match { get; init; }
            public List<string> ReviewReasons { get; } = new List<string>();
        }

        internal static bool TryExport(
            Bitmap source,
            IReadOnlyList<string> representativeImagePaths,
            AutoMPointCandidateResult candidate,
            string analysisDefinition,
            string reportPath,
            out string error)
        {
            error = string.Empty;
            try
            {
                if (source == null)
                {
                    throw new InvalidOperationException("The Auto MPoint source image is unavailable.");
                }

                if (candidate == null || candidate.RepresentativeMatches.Count == 0)
                {
                    throw new InvalidOperationException("Select an analyzed candidate with representative-image evidence.");
                }

                IReadOnlyList<string> paths = representativeImagePaths ?? Array.Empty<string>();
                if (paths.Count != candidate.RepresentativeMatches.Count)
                {
                    throw new InvalidOperationException(
                        $"Representative evidence count changed. Paths={paths.Count}, Results={candidate.RepresentativeMatches.Count}.");
                }

                string fullReportPath = Path.GetFullPath(reportPath ?? string.Empty);
                string reportDirectory = Path.GetDirectoryName(fullReportPath)
                    ?? throw new InvalidOperationException("The report directory is unavailable.");
                Directory.CreateDirectory(reportDirectory);

                List<ReportRow> rows = CreateRows(paths, candidate);
                SelectReviewQueue(rows);
                string html = BuildHtml(source, candidate, analysisDefinition, rows);
                string temporaryPath = fullReportPath + ".tmp";
                File.WriteAllText(temporaryPath, html, new UTF8Encoding(false));
                File.Move(temporaryPath, fullReportPath, true);
                return true;
            }
            catch (Exception exception)
            {
                error = exception.GetBaseException().Message;
                return false;
            }
        }

        private static List<ReportRow> CreateRows(
            IReadOnlyList<string> paths,
            AutoMPointCandidateResult candidate)
        {
            Dictionary<int, AutoMPointRepresentativeMatchResult> matches =
                candidate.RepresentativeMatches.ToDictionary(match => match.ImageIndex);
            List<ReportRow> rows = new List<ReportRow>(paths.Count);
            for (int index = 0; index < paths.Count; index++)
            {
                string imagePath = Path.GetFullPath(paths[index]);
                if (!File.Exists(imagePath))
                {
                    throw new FileNotFoundException("A representative image is missing.", imagePath);
                }

                if (!matches.TryGetValue(index + 1, out AutoMPointRepresentativeMatchResult match))
                {
                    throw new InvalidOperationException($"Representative result {index + 1} is missing.");
                }

                rows.Add(new ReportRow
                {
                    Index = index + 1,
                    ImagePath = imagePath,
                    ImageSha256 = ComputeFileSha256(imagePath),
                    Match = match
                });
            }

            return rows;
        }

        private static void SelectReviewQueue(List<ReportRow> rows)
        {
            if (rows.Count <= 24)
            {
                foreach (ReportRow row in rows)
                {
                    AddReason(row, "all rows (N <= 24)");
                }

                return;
            }

            foreach (ReportRow row in rows.Where(row => row.Match?.Success != true))
            {
                AddReason(row, "failure");
            }

            AddRange(rows.Where(row => row.Match?.Success == true)
                    .OrderBy(row => row.Match.Score)
                    .ThenBy(row => row.Index)
                    .Take(6),
                "lowest score");
            AddRange(rows.Where(row => row.Match?.Success == true)
                    .OrderBy(row => row.Match.UniquenessMargin)
                    .ThenBy(row => row.Index)
                    .Take(6),
                "lowest uniqueness");
            AddRange(rows.OrderByDescending(row => row.Match?.RuntimeMilliseconds ?? 0D)
                    .ThenBy(row => row.Index)
                    .Take(4),
                "longest runtime");
            AddRange(rows.OrderByDescending(row => Math.Abs(row.Match?.Angle ?? 0D))
                    .ThenBy(row => row.Index)
                    .Take(4),
                "angle extreme");
            AddRange(rows.OrderByDescending(row => Math.Abs((row.Match?.Scale ?? 1D) - 1D))
                    .ThenBy(row => row.Index)
                    .Take(4),
                "scale extreme");

            List<ReportRow> hashOrdered = rows
                .OrderBy(row => row.ImageSha256, StringComparer.Ordinal)
                .ThenBy(row => row.Index)
                .ToList();
            int sampleCount = Math.Min(8, hashOrdered.Count);
            for (int sampleIndex = 0; sampleIndex < sampleCount; sampleIndex++)
            {
                int position = sampleCount == 1
                    ? 0
                    : (int)Math.Round(sampleIndex * (hashOrdered.Count - 1D) / (sampleCount - 1D));
                AddReason(hashOrdered[position], "SHA-256 spread");
            }
        }

        private static void AddRange(IEnumerable<ReportRow> rows, string reason)
        {
            foreach (ReportRow row in rows)
            {
                AddReason(row, reason);
            }
        }

        private static void AddReason(ReportRow row, string reason)
        {
            if (!row.ReviewReasons.Contains(reason, StringComparer.Ordinal))
            {
                row.ReviewReasons.Add(reason);
            }
        }

        private static string BuildHtml(
            Bitmap source,
            AutoMPointCandidateResult candidate,
            string analysisDefinition,
            IReadOnlyList<ReportRow> rows)
        {
            List<ReportRow> reviewRows = rows
                .Where(row => row.ReviewReasons.Count > 0)
                .OrderBy(row => row.Index)
                .ToList();
            int successCount = rows.Count(row => row.Match.Success);
            double minimumScore = rows.Count > 0 ? rows.Min(row => row.Match.Score) : 0D;
            double minimumUniqueness = rows.Count > 0 ? rows.Min(row => row.Match.UniquenessMargin) : 0D;
            double maximumRuntime = rows.Count > 0 ? rows.Max(row => row.Match.RuntimeMilliseconds) : 0D;
            string sourceSha256 = ComputeBitmapSha256(source);
            string templateDataUri = CreateTemplateDataUri(source, candidate.PatternRoi);

            StringBuilder html = new StringBuilder();
            html.AppendLine("<!doctype html><html lang=\"ko\"><head><meta charset=\"utf-8\"><meta name=\"viewport\" content=\"width=device-width,initial-scale=1\">");
            html.AppendLine("<title>OpenVisionLab Auto MPoint N-image evidence report</title>");
            html.AppendLine("<style>body{margin:0;background:#08111f;color:#eef5ff;font-family:'Segoe UI','Malgun Gothic',sans-serif;line-height:1.5}.wrap{max-width:1400px;margin:auto;padding:30px}.panel{background:#111d2e;border:1px solid #2b3d56;border-radius:14px;padding:20px;margin:16px 0}.cards{display:grid;grid-template-columns:repeat(4,1fr);gap:12px}.card{background:#17263b;border-radius:11px;padding:15px}.value{font-size:24px;font-weight:800}.label{color:#a9bad0;font-size:13px}.ok{color:#6ce9bb}.bad{color:#ff7b83}.warn{color:#ffc36c}.template{display:flex;justify-content:center;background:#050a11;border:1px solid #31445d;border-radius:10px;padding:8px}.template img{width:auto;max-width:420px;max-height:420px}.gallery{display:grid;grid-template-columns:repeat(3,minmax(0,1fr));gap:12px}.shot{background:#050a11;border:1px solid #31445d;border-radius:10px;overflow:hidden}.shot img{width:100%;height:auto;display:block}.caption{padding:8px 10px;color:#d7e4f4;font-size:12px}.scroll{overflow:auto}table{width:100%;border-collapse:collapse;font-size:12px}th,td{padding:8px;border-bottom:1px solid #2b3d56;text-align:left;white-space:nowrap}th{background:#1b2b42}.print{float:right;background:#24405e;color:#fff;border:1px solid #5e789b;border-radius:9px;padding:10px 14px}code{color:#c8e2ff;white-space:pre-wrap;word-break:break-all}@media(max-width:900px){.cards{grid-template-columns:repeat(2,1fr)}.gallery{grid-template-columns:1fr}}@media print{body{background:#fff;color:#111}.panel,.card{background:#fff;border-color:#aaa}.print{display:none}}</style></head><body><main class=\"wrap\">");
            html.AppendLine("<button class=\"print\" onclick=\"window.print()\">인쇄 / PDF 저장</button>");
            html.Append("<h1>Auto MPoint N-image evidence report</h1><p>이미 계산된 대표 이미지 결과를 저장했습니다. 이 내보내기는 매칭을 다시 실행하거나 패턴·Preview/Run·레이어·라우팅을 변경하지 않습니다.</p>");
            html.AppendLine("<section class=\"cards\">");
            AppendMetric(html, rows.Count.ToString(CultureInfo.InvariantCulture), "대표 이미지");
            AppendMetric(html, $"{successCount}/{rows.Count}", "매칭 성공");
            AppendMetric(html, minimumScore.ToString("0.0", CultureInfo.InvariantCulture), "최저 점수");
            AppendMetric(html, minimumUniqueness.ToString("0.000", CultureInfo.InvariantCulture), "최저 고유성");
            AppendMetric(html, reviewRows.Count.ToString(CultureInfo.InvariantCulture), "결정적 검토 큐");
            AppendMetric(html, maximumRuntime.ToString("0.0", CultureInfo.InvariantCulture) + " ms", "최대 실행시간");
            AppendMetric(
                html,
                $"{candidate.PatternRoi.X},{candidate.PatternRoi.Y},{candidate.PatternRoi.Width},{candidate.PatternRoi.Height}",
                "선택 ROI");
            AppendMetric(html, "#" + candidate.Rank, "선택 후보");
            html.AppendLine("</section>");

            html.Append("<section class=\"panel\"><h2>정체성 및 경계</h2><p>입력 이미지 PNG SHA-256: <code>")
                .Append(Encode(sourceSha256))
                .Append("</code></p><p>분석 정의: <code>")
                .Append(Encode(analysisDefinition))
                .AppendLine("</code></p><p class=\"warn\">이 보고서는 선택한 동일 제품·촬영 조건의 대표 이미지 결과입니다. 실제 생산 변동, 결함 영역과 패턴의 독립성, 현장 강건성 또는 합격 판정을 자동으로 증명하지 않습니다.</p></section>");

            html.AppendLine("<section class=\"panel\"><h2>선택 템플릿</h2>");
            html.Append("<div class=\"template\"><img alt=\"Auto MPoint selected template\" src=\"")
                .Append(templateDataUri)
                .AppendLine("\"></div></section>");

            html.AppendLine("<section class=\"panel\"><h2>결정적 검토 드로잉</h2><p>모든 실패, 최저 점수·고유성, 각도·배율·시간 극단, SHA-256 분산 표본을 중복 제거했습니다. N이 24 이하이면 전부 표시합니다.</p><div class=\"gallery\">");
            foreach (ReportRow row in reviewRows)
            {
                html.Append("<article class=\"shot\"><img alt=\"")
                    .Append(Encode(Path.GetFileName(row.ImagePath)))
                    .Append("\" src=\"")
                    .Append(CreateReviewDrawingDataUri(row, candidate.PatternRoi))
                    .Append("\"><div class=\"caption\">#")
                    .Append(row.Index)
                    .Append(" · ")
                    .Append(Encode(Path.GetFileName(row.ImagePath)))
                    .Append(" · ")
                    .Append(Encode(row.Match.Outcome))
                    .Append(" · ")
                    .Append(Encode(string.Join(", ", row.ReviewReasons)))
                    .AppendLine("</div></article>");
            }
            html.AppendLine("</div></section>");

            html.AppendLine("<section class=\"panel\"><h2>전체 N-image 결과</h2><div class=\"scroll\"><table><thead><tr><th>#</th><th>파일</th><th>결과</th><th>점수</th><th>고유성</th><th>중심 X</th><th>중심 Y</th><th>각도</th><th>배율</th><th>ms</th><th>검토 이유</th><th>SHA-256</th><th>메시지</th></tr></thead><tbody>");
            foreach (ReportRow row in rows)
            {
                html.Append("<tr data-row=\"").Append(row.Index).Append("\"><td>")
                    .Append(row.Index).Append("</td><td title=\"")
                    .Append(Encode(row.ImagePath)).Append("\">")
                    .Append(Encode(Path.GetFileName(row.ImagePath))).Append("</td><td class=\"")
                    .Append(row.Match.Success ? "ok" : "bad").Append("\">")
                    .Append(Encode(row.Match.Outcome)).Append("</td><td>")
                    .Append(row.Match.Score.ToString("0.000", CultureInfo.InvariantCulture)).Append("</td><td>")
                    .Append(row.Match.UniquenessMargin.ToString("0.000000", CultureInfo.InvariantCulture)).Append("</td><td>")
                    .Append(row.Match.Center.X.ToString("0.000", CultureInfo.InvariantCulture)).Append("</td><td>")
                    .Append(row.Match.Center.Y.ToString("0.000", CultureInfo.InvariantCulture)).Append("</td><td>")
                    .Append(row.Match.Angle.ToString("0.000", CultureInfo.InvariantCulture)).Append("</td><td>")
                    .Append(row.Match.Scale.ToString("0.000", CultureInfo.InvariantCulture)).Append("</td><td>")
                    .Append(row.Match.RuntimeMilliseconds.ToString("0.000", CultureInfo.InvariantCulture)).Append("</td><td>")
                    .Append(Encode(string.Join(", ", row.ReviewReasons))).Append("</td><td><code>")
                    .Append(Encode(row.ImageSha256)).Append("</code></td><td>")
                    .Append(Encode(row.Match.Message)).AppendLine("</td></tr>");
            }

            html.AppendLine("</tbody></table></div></section>");
            html.Append("<footer>OpenVisionLab · generated ")
                .Append(Encode(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.CurrentCulture)))
                .AppendLine("</footer></main></body></html>");
            return html.ToString();
        }

        private static void AppendMetric(StringBuilder html, string value, string label)
        {
            html.Append("<div class=\"card\"><div class=\"value\">")
                .Append(Encode(value))
                .Append("</div><div class=\"label\">")
                .Append(Encode(label))
                .AppendLine("</div></div>");
        }

        private static string CreateTemplateDataUri(Bitmap source, Rect roi)
        {
            using Mat sourceMat = BitmapImageConverter.ToMat(source);
            Rect imageBounds = new Rect(0, 0, sourceMat.Width, sourceMat.Height);
            Rect clipped = roi & imageBounds;
            if (clipped.Width != roi.Width || clipped.Height != roi.Height || clipped.Width <= 0 || clipped.Height <= 0)
            {
                throw new InvalidOperationException($"The selected template ROI is outside the source image: {roi}.");
            }

            using Mat template = sourceMat.SubMat(clipped).Clone();
            return ToPngDataUri(template);
        }

        private static string CreateReviewDrawingDataUri(ReportRow row, Rect patternRoi)
        {
            using Mat loaded = Cv2.ImRead(row.ImagePath, ImreadModes.Unchanged);
            if (loaded.Empty())
            {
                throw new InvalidOperationException("Failed to load representative image: " + row.ImagePath);
            }

            using Mat canvas = EnsureBgr(loaded);
            Scalar color = row.Match.Success ? new Scalar(60, 230, 90) : new Scalar(70, 70, 255);
            if (row.Match.Success)
            {
                Point2f[] points = CreateRotatedMatchBoxPoints(row.Match, patternRoi)
                    .ToArray();
                for (int index = 0; index < points.Length; index++)
                {
                    Cv2.Line(
                        canvas,
                        ToPoint(points[index]),
                        ToPoint(points[(index + 1) % points.Length]),
                        color,
                        3,
                        LineTypes.AntiAlias);
                }

                OpenCvSharp.Point center = ToPoint(row.Match.Center);
                Cv2.DrawMarker(canvas, center, new Scalar(20, 20, 255), MarkerTypes.Cross, 18, 2, LineTypes.AntiAlias);
            }

            Cv2.Rectangle(canvas, new Rect(0, 0, canvas.Width, Math.Min(36, canvas.Height)), new Scalar(0, 0, 0), -1);
            string label = $"#{row.Index} {row.Match.Outcome} S={row.Match.Score:0.0} U={row.Match.UniquenessMargin:0.000}";
            Cv2.PutText(
                canvas,
                label,
                new OpenCvSharp.Point(8, Math.Min(25, canvas.Height - 4)),
                HersheyFonts.HersheySimplex,
                0.62,
                color,
                2,
                LineTypes.AntiAlias);

            if (canvas.Width <= 960)
            {
                return ToPngDataUri(canvas);
            }

            double scale = 960D / canvas.Width;
            using Mat resized = new Mat();
            Cv2.Resize(canvas, resized, new OpenCvSharp.Size(), scale, scale, InterpolationFlags.Area);
            return ToPngDataUri(resized);
        }

        private static IEnumerable<Point2f> CreateRotatedMatchBoxPoints(
            AutoMPointRepresentativeMatchResult match,
            Rect patternRoi)
        {
            float width = Math.Max(1F, patternRoi.Width * (float)Math.Max(match.Scale, 0.0001D));
            float height = Math.Max(1F, patternRoi.Height * (float)Math.Max(match.Scale, 0.0001D));
            float halfWidth = width / 2F;
            float halfHeight = height / 2F;
            double radians = match.Angle * Math.PI / 180D;
            double cos = Math.Cos(radians);
            double sin = Math.Sin(radians);

            Point2f Transform(float localX, float localY)
            {
                return new Point2f(
                    (float)(match.Center.X + localX * cos + localY * sin),
                    (float)(match.Center.Y - localX * sin + localY * cos));
            }

            yield return Transform(-halfWidth, -halfHeight);
            yield return Transform(halfWidth, -halfHeight);
            yield return Transform(halfWidth, halfHeight);
            yield return Transform(-halfWidth, halfHeight);
        }

        private static Mat EnsureBgr(Mat source)
        {
            Mat canvas = new Mat();
            if (source.Channels() == 1)
            {
                Cv2.CvtColor(source, canvas, ColorConversionCodes.GRAY2BGR);
            }
            else if (source.Channels() == 4)
            {
                Cv2.CvtColor(source, canvas, ColorConversionCodes.BGRA2BGR);
            }
            else
            {
                source.CopyTo(canvas);
            }

            return canvas;
        }

        private static OpenCvSharp.Point ToPoint(Point2f point)
        {
            return new OpenCvSharp.Point((int)Math.Round(point.X), (int)Math.Round(point.Y));
        }

        private static string ToPngDataUri(Mat image)
        {
            Cv2.ImEncode(".png", image, out byte[] bytes);
            return "data:image/png;base64," + Convert.ToBase64String(bytes);
        }

        private static string ComputeBitmapSha256(Bitmap bitmap)
        {
            using MemoryStream stream = new MemoryStream();
            bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
            return Convert.ToHexString(SHA256.HashData(stream.ToArray()));
        }

        private static string ComputeFileSha256(string path)
        {
            using FileStream stream = File.OpenRead(path);
            return Convert.ToHexString(SHA256.HashData(stream));
        }

        private static string Encode(string value)
        {
            return WebUtility.HtmlEncode(value ?? string.Empty);
        }
    }
}
