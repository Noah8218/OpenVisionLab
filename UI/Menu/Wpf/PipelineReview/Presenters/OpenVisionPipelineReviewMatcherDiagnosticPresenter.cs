using Lib.OpenCV.Result;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace OpenVisionLab
{
    internal sealed class OpenVisionPipelineReviewMatcherDiagnosticRow
    {
        public string Section { get; init; } = string.Empty;
        public string Name { get; init; } = string.Empty;
        public string Value { get; init; } = string.Empty;
    }

    internal sealed class OpenVisionPipelineReviewMatcherDiagnosticState : IDisposable
    {
        public string State { get; init; } = string.Empty;
        public string SummaryText { get; init; } = string.Empty;
        public string EvidenceId { get; init; } = string.Empty;
        public int ModelPointCount { get; init; }
        public bool HasSelectedCandidate { get; init; }
        public bool HasStrongestSpatialAlternative { get; init; }
        public Bitmap ModelPreview { get; init; }
        public Bitmap CandidatePreview { get; init; }
        public IReadOnlyList<OpenVisionPipelineReviewMatcherDiagnosticRow> Rows { get; init; } =
            Array.Empty<OpenVisionPipelineReviewMatcherDiagnosticRow>();

        public void Dispose()
        {
            ModelPreview?.Dispose();
            CandidatePreview?.Dispose();
        }
    }

    internal static class OpenVisionPipelineReviewMatcherDiagnosticPresenter
    {
        public static OpenVisionPipelineReviewMatcherDiagnosticState Create(
            EdgeBasedMatchingDiagnosticEvidence evidence,
            IReadOnlyDictionary<string, double> metrics,
            Bitmap sourceImage)
        {
            if (evidence == null)
            {
                return null;
            }

            IReadOnlyDictionary<string, double> retainedMetrics =
                metrics ?? new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);
            List<OpenVisionPipelineReviewMatcherDiagnosticRow> rows = CreateRows(
                evidence,
                retainedMetrics);
            string evidenceId = CreateEvidenceId(evidence, retainedMetrics, sourceImage);
            string selectedLabel = GetSelectedCandidateLabel(evidence.State);
            string summary = string.Format(
                CultureInfo.CurrentCulture,
                "{0} | model {1}x{2}, {3} points | {4} {5} | alternative {6}",
                evidence.State,
                evidence.TemplateWidth,
                evidence.TemplateHeight,
                evidence.ModelPoints.Count,
                selectedLabel.ToLowerInvariant(),
                FormatCandidateScore(evidence.SelectedCandidate),
                FormatCandidateScore(evidence.StrongestSpatialAlternative));
            if (!string.IsNullOrWhiteSpace(evidence.Reason))
            {
                summary += " | " + evidence.Reason;
            }

            return new OpenVisionPipelineReviewMatcherDiagnosticState
            {
                State = evidence.State ?? string.Empty,
                SummaryText = summary,
                EvidenceId = evidenceId,
                ModelPointCount = evidence.ModelPoints.Count,
                HasSelectedCandidate = evidence.SelectedCandidate != null,
                HasStrongestSpatialAlternative = evidence.StrongestSpatialAlternative != null,
                ModelPreview = CreateModelPreview(evidence),
                CandidatePreview = CreateCandidatePreview(sourceImage, evidence),
                Rows = rows
            };
        }

        private static List<OpenVisionPipelineReviewMatcherDiagnosticRow> CreateRows(
            EdgeBasedMatchingDiagnosticEvidence evidence,
            IReadOnlyDictionary<string, double> metrics)
        {
            List<OpenVisionPipelineReviewMatcherDiagnosticRow> rows = new List<OpenVisionPipelineReviewMatcherDiagnosticRow>();
            Add(rows, "Decision", "State", evidence.State);
            Add(rows, "Decision", "Error", string.IsNullOrWhiteSpace(evidence.ErrorCode) ? "None" : evidence.ErrorCode);
            Add(rows, "Decision", "Exact reason", evidence.Reason);
            Add(rows, "Search", "Search ROI", FormatRectangle(evidence.SearchRoi));
            AddCandidateRows(rows, GetSelectedCandidateLabel(evidence.State), evidence.SelectedCandidate);
            AddCandidateRows(rows, "Alternative", evidence.StrongestSpatialAlternative);

            AddMetric(rows, metrics, "Model", "Raw edge points", "Model.RawEdgePointCount", "0");
            AddMetric(rows, metrics, "Model", "Retained edge points", "Model.EdgePointCount", "0");
            AddMetric(rows, metrics, "Model", "Point sample ratio", "Model.PointSampleRatio", "0.###");
            AddMetric(rows, metrics, "Model", "Edge coverage", "Model.EdgeCoverageArea", "0.###");
            AddMetric(rows, metrics, "Model", "Quadrant balance", "Model.QuadrantBalance", "0.###");
            AddMetric(rows, metrics, "Model", "Scale-search risk", "Model.ScaleSearchRisk", "0");

            AddMetric(rows, metrics, "Pyramid", "Highest usable level", "Model.Pyramid.HighestUsableLevel", "0");
            AddMetric(rows, metrics, "Pyramid", "Estimated levels", "Model.Pyramid.LevelCountEstimate", "0");
            for (int level = 0; level < 6; level++)
            {
                string prefix = "Model.Pyramid.Level" + level.ToString(CultureInfo.InvariantCulture);
                if (!metrics.ContainsKey(prefix + ".Width"))
                {
                    continue;
                }

                string value = string.Format(
                    CultureInfo.CurrentCulture,
                    "{0:0}x{1:0} / edges {2:0} / coverage {3:0.###} / {4}",
                    GetMetric(metrics, prefix + ".Width"),
                    GetMetric(metrics, prefix + ".Height"),
                    GetMetric(metrics, prefix + ".EdgePointCount"),
                    GetMetric(metrics, prefix + ".CoverageArea"),
                    GetMetric(metrics, prefix + ".Usable") > 0.5D ? "usable" : "not usable");
                Add(rows, "Pyramid", "Level " + level.ToString(CultureInfo.CurrentCulture), value);
            }

            AddMetric(rows, metrics, "Candidates", "Angle candidates", "Candidate.SearchAngleCount", "0");
            AddMetric(rows, metrics, "Candidates", "Coarse angle candidates", "Candidate.CoarseAngleCount", "0");
            AddMetric(rows, metrics, "Candidates", "Scale candidates", "Candidate.SearchScaleCount", "0");
            AddMetric(rows, metrics, "Candidates", "Estimated raw positions", "Candidate.EstimatedRawSearchPositions", "0");
            AddMetric(rows, metrics, "Candidates", "Estimated coarse positions", "Candidate.EstimatedCoarseSearchPositions", "0");
            AddMetric(rows, metrics, "Candidates", "Edge seeds", "Candidate.EdgeSeedCount", "0");
            AddMetric(rows, metrics, "Candidates", "Pyramid proposal scale", "Candidate.PyramidProposalScale", "0.###");
            AddMetric(rows, metrics, "Candidates", "Pyramid proposal attempts", "Candidate.PyramidProposalAttemptCount", "0");
            AddMetric(rows, metrics, "Candidates", "Pyramid proposals", "Candidate.PyramidProposalCandidateCount", "0");
            AddMetric(rows, metrics, "Candidates", "Pyramid verified", "Candidate.PyramidProposalVerifiedCount", "0");
            AddMetric(rows, metrics, "Candidates", "Pyramid accepted", "Candidate.PyramidProposalAcceptedCount", "0");
            AddMetric(rows, metrics, "Candidates", "Pyramid fallback", "Candidate.PyramidProposalFallbackCount", "0");
            AddMetric(rows, metrics, "Candidates", "Ambiguous alternatives", "Candidate.AmbiguousAlternativeCount", "0");

            AddMetric(rows, metrics, "Unique", "Enabled", "UniqueMatch.Enabled", "0");
            AddMetric(rows, metrics, "Unique", "Selected score", "UniqueMatch.SelectedScore", "0.###");
            AddMetric(rows, metrics, "Unique", "Strongest alternative", "UniqueMatch.StrongestAlternativeScore", "0.###");
            AddMetric(rows, metrics, "Unique", "Score margin", "UniqueMatch.ScoreMargin", "0.###");
            AddMetric(rows, metrics, "Unique", "Required margin", "UniqueMatch.MinimumScoreMargin", "0.###");
            AddMetric(rows, metrics, "Unique", "Plausible alternatives", "UniqueMatch.PlausibleAlternativeCount", "0");
            return rows;
        }

        private static void AddCandidateRows(
            ICollection<OpenVisionPipelineReviewMatcherDiagnosticRow> rows,
            string section,
            EdgeBasedMatchingCandidateDiagnostic candidate)
        {
            if (candidate == null)
            {
                Add(rows, section, "Candidate", "None retained");
                return;
            }

            Add(rows, section, "Score", candidate.Score.ToString("0.###", CultureInfo.CurrentCulture));
            Add(
                rows,
                section,
                "Pose",
                string.Format(
                    CultureInfo.CurrentCulture,
                    "center ({0:0.##}, {1:0.##}) / angle {2:0.###}° / scale {3:0.###}",
                    candidate.Center.X,
                    candidate.Center.Y,
                    candidate.Angle,
                    candidate.Scale));
            Add(rows, section, "Bounds", FormatRectangle(candidate.Bounds));
        }

        private static void AddMetric(
            ICollection<OpenVisionPipelineReviewMatcherDiagnosticRow> rows,
            IReadOnlyDictionary<string, double> metrics,
            string section,
            string name,
            string key,
            string format)
        {
            if (metrics == null || !metrics.TryGetValue(key, out double value))
            {
                return;
            }

            Add(rows, section, name, value.ToString(format, CultureInfo.CurrentCulture));
        }

        private static void Add(
            ICollection<OpenVisionPipelineReviewMatcherDiagnosticRow> rows,
            string section,
            string name,
            string value)
        {
            rows.Add(new OpenVisionPipelineReviewMatcherDiagnosticRow
            {
                Section = section ?? string.Empty,
                Name = name ?? string.Empty,
                Value = value ?? string.Empty
            });
        }

        private static double GetMetric(IReadOnlyDictionary<string, double> metrics, string key)
        {
            return metrics != null && metrics.TryGetValue(key, out double value) ? value : 0D;
        }

        private static string FormatCandidateScore(EdgeBasedMatchingCandidateDiagnostic candidate)
        {
            return candidate == null
                ? "none"
                : candidate.Score.ToString("0.###", CultureInfo.CurrentCulture);
        }

        private static string FormatRectangle(RectangleF rectangle)
        {
            return string.Format(
                CultureInfo.CurrentCulture,
                "{0:0.##}, {1:0.##}, {2:0.##}, {3:0.##}",
                rectangle.X,
                rectangle.Y,
                rectangle.Width,
                rectangle.Height);
        }

        private static Bitmap CreateModelPreview(EdgeBasedMatchingDiagnosticEvidence evidence)
        {
            const int width = 360;
            const int height = 260;
            Bitmap preview = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            using Graphics graphics = Graphics.FromImage(preview);
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.Clear(Color.FromArgb(18, 32, 38));

            float margin = 26F;
            float sourceWidth = Math.Max(1F, evidence.TemplateWidth);
            float sourceHeight = Math.Max(1F, evidence.TemplateHeight);
            float scale = Math.Min((width - (margin * 2F)) / sourceWidth, (height - (margin * 2F)) / sourceHeight);
            float offsetX = (width - (sourceWidth * scale)) / 2F;
            float offsetY = (height - (sourceHeight * scale)) / 2F;
            using Pen framePen = new Pen(Color.FromArgb(110, 155, 178), 1F);
            using Pen axisPen = new Pen(Color.FromArgb(70, 126, 148), 1F)
            {
                DashStyle = DashStyle.Dash
            };
            graphics.DrawRectangle(framePen, offsetX, offsetY, sourceWidth * scale, sourceHeight * scale);
            graphics.DrawLine(
                axisPen,
                offsetX + (evidence.ModelCenter.X * scale),
                offsetY,
                offsetX + (evidence.ModelCenter.X * scale),
                offsetY + (sourceHeight * scale));
            graphics.DrawLine(
                axisPen,
                offsetX,
                offsetY + (evidence.ModelCenter.Y * scale),
                offsetX + (sourceWidth * scale),
                offsetY + (evidence.ModelCenter.Y * scale));
            using Brush pointBrush = new SolidBrush(Color.LimeGreen);
            float pointSize = Math.Max(2F, Math.Min(4F, scale * 0.9F));
            foreach (PointF point in evidence.ModelPoints)
            {
                float x = offsetX + (point.X * scale);
                float y = offsetY + (point.Y * scale);
                graphics.FillEllipse(pointBrush, x - (pointSize / 2F), y - (pointSize / 2F), pointSize, pointSize);
            }

            DrawCaption(
                graphics,
                $"Trained edge model — {evidence.TemplateWidth}x{evidence.TemplateHeight}, {evidence.ModelPoints.Count} points",
                width,
                height);
            return preview;
        }

        private static Bitmap CreateCandidatePreview(
            Bitmap sourceImage,
            EdgeBasedMatchingDiagnosticEvidence evidence)
        {
            Bitmap preview = sourceImage == null
                ? new Bitmap(640, 420, PixelFormat.Format24bppRgb)
                : new Bitmap(sourceImage);
            using Graphics graphics = Graphics.FromImage(preview);
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            if (sourceImage == null)
            {
                graphics.Clear(Color.FromArgb(18, 32, 38));
            }

            float thickness = Math.Max(2F, Math.Min(preview.Width, preview.Height) / 180F);
            using Pen roiPen = new Pen(Color.DeepSkyBlue, thickness)
            {
                DashStyle = DashStyle.Dash
            };
            graphics.DrawRectangle(
                roiPen,
                evidence.SearchRoi.X,
                evidence.SearchRoi.Y,
                evidence.SearchRoi.Width,
                evidence.SearchRoi.Height);
            DrawCandidate(
                graphics,
                evidence,
                evidence.StrongestSpatialAlternative,
                Color.Orange,
                "Alternative",
                thickness);
            DrawCandidate(
                graphics,
                evidence,
                evidence.SelectedCandidate,
                Color.LimeGreen,
                GetSelectedCandidateLabel(evidence.State),
                thickness);
            DrawCaption(
                graphics,
                "Search ROI / retained primary hypothesis / strongest spatial alternative",
                preview.Width,
                preview.Height);
            return preview;
        }

        private static void DrawCandidate(
            Graphics graphics,
            EdgeBasedMatchingDiagnosticEvidence evidence,
            EdgeBasedMatchingCandidateDiagnostic candidate,
            Color color,
            string label,
            float thickness)
        {
            if (candidate == null)
            {
                return;
            }

            using Pen boundsPen = new Pen(color, thickness);
            graphics.DrawRectangle(
                boundsPen,
                candidate.Bounds.X,
                candidate.Bounds.Y,
                candidate.Bounds.Width,
                candidate.Bounds.Height);
            DrawCross(graphics, boundsPen, candidate.Center, Math.Max(6F, thickness * 3F));

            double radians = candidate.Angle * Math.PI / 180D;
            double cos = Math.Cos(radians);
            double sin = Math.Sin(radians);
            float modelScale = (float)Math.Max(0.0001D, candidate.Scale);
            using Brush modelBrush = new SolidBrush(Color.FromArgb(190, color));
            foreach (PointF modelPoint in evidence.ModelPoints)
            {
                double localX = (modelPoint.X - evidence.ModelCenter.X) * modelScale;
                double localY = (modelPoint.Y - evidence.ModelCenter.Y) * modelScale;
                float x = candidate.Center.X + (float)((localX * cos) - (localY * sin));
                float y = candidate.Center.Y + (float)((localX * sin) + (localY * cos));
                graphics.FillEllipse(modelBrush, x - 1F, y - 1F, 2F, 2F);
            }

            using Font font = new Font("Segoe UI", Math.Max(9F, thickness * 3F), FontStyle.Bold, GraphicsUnit.Pixel);
            using Brush textBrush = new SolidBrush(color);
            graphics.DrawString(
                $"{label} {candidate.Score:0.###} / {candidate.Angle:0.###}° / x{candidate.Scale:0.###}",
                font,
                textBrush,
                candidate.Bounds.X,
                Math.Max(2F, candidate.Bounds.Y - font.Height - 2F));
        }

        private static void DrawCross(Graphics graphics, Pen pen, PointF point, float size)
        {
            graphics.DrawLine(pen, point.X - size, point.Y, point.X + size, point.Y);
            graphics.DrawLine(pen, point.X, point.Y - size, point.X, point.Y + size);
        }

        private static string GetSelectedCandidateLabel(string state)
        {
            if (string.Equals(state, "NoMatch", StringComparison.OrdinalIgnoreCase))
            {
                return "Best observed (below gate)";
            }

            if (string.Equals(state, "Ambiguous", StringComparison.OrdinalIgnoreCase))
            {
                return "Rejected primary hypothesis";
            }

            return "Selected";
        }

        private static void DrawCaption(Graphics graphics, string text, int width, int height)
        {
            using Font font = new Font("Segoe UI", 10F, FontStyle.Bold, GraphicsUnit.Pixel);
            using Brush background = new SolidBrush(Color.FromArgb(205, 12, 28, 32));
            using Brush foreground = new SolidBrush(Color.White);
            SizeF size = graphics.MeasureString(text, font);
            float y = Math.Max(2F, height - size.Height - 8F);
            graphics.FillRectangle(background, 4F, y - 2F, Math.Min(width - 8F, size.Width + 8F), size.Height + 4F);
            graphics.DrawString(text, font, foreground, 8F, y);
        }

        private static string CreateEvidenceId(
            EdgeBasedMatchingDiagnosticEvidence evidence,
            IReadOnlyDictionary<string, double> metrics,
            Bitmap sourceImage)
        {
            string canonical = string.Join(
                "|",
                "EdgeBasedMatchingDiagnostics",
                evidence.State ?? string.Empty,
                evidence.Reason ?? string.Empty,
                evidence.ErrorCode ?? string.Empty,
                FormatRectangle(evidence.SearchRoi),
                evidence.TemplateWidth.ToString(CultureInfo.InvariantCulture),
                evidence.TemplateHeight.ToString(CultureInfo.InvariantCulture),
                string.Join(";", evidence.ModelPoints.Select(point => string.Format(
                    CultureInfo.InvariantCulture,
                    "{0:0.###},{1:0.###}",
                    point.X,
                    point.Y))),
                FormatCandidateCanonical(evidence.SelectedCandidate),
                FormatCandidateCanonical(evidence.StrongestSpatialAlternative),
                string.Join(
                    ";",
                    (metrics ?? new Dictionary<string, double>())
                        .Where(metric =>
                            metric.Key.StartsWith("Model.", StringComparison.Ordinal)
                            || metric.Key.StartsWith("Candidate.", StringComparison.Ordinal)
                            || metric.Key.StartsWith("UniqueMatch.", StringComparison.Ordinal))
                        .OrderBy(metric => metric.Key, StringComparer.Ordinal)
                        .Select(metric => string.Format(
                            CultureInfo.InvariantCulture,
                            "{0}={1:0.###############}",
                            metric.Key,
                            metric.Value))),
                ComputeBitmapSha256(sourceImage));
            return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(canonical)));
        }

        private static string FormatCandidateCanonical(EdgeBasedMatchingCandidateDiagnostic candidate)
        {
            if (candidate == null)
            {
                return "none";
            }

            return string.Format(
                CultureInfo.InvariantCulture,
                "{0:0.###############},{1:0.###############},{2:0.###############},{3:0.###############},{4:0.###############},{5:0.###############},{6:0.###############},{7:0.###############},{8:0.###############}",
                candidate.Score,
                candidate.Angle,
                candidate.Scale,
                candidate.Center.X,
                candidate.Center.Y,
                candidate.Bounds.X,
                candidate.Bounds.Y,
                candidate.Bounds.Width,
                candidate.Bounds.Height);
        }

        private static string ComputeBitmapSha256(Bitmap image)
        {
            if (image == null)
            {
                return new string('0', 64);
            }

            using MemoryStream stream = new MemoryStream();
            image.Save(stream, ImageFormat.Png);
            return Convert.ToHexString(SHA256.HashData(stream.ToArray()));
        }
    }
}
