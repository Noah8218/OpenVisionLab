using OpenVisionLab.Vision2D.Result;
using OpenVisionLab.Vision2D.Tool;
using OpenVisionLab.Contracts;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace OpenVisionLab
{
    internal sealed class LineToolResultReviewPresenter
    {
        private readonly FrameworkElement owner;
        private readonly TextBlock summaryText;
        private readonly Panel chipPanel;
        private readonly Func<LineToolPurpose> selectedPurpose;
        private readonly Func<LineGaugeProperty> selectedLineProperty;

        public LineToolResultReviewPresenter(
            FrameworkElement owner,
            TextBlock summaryText,
            Panel chipPanel,
            Func<LineToolPurpose> selectedPurpose,
            Func<LineGaugeProperty> selectedLineProperty)
        {
            this.owner = owner ?? throw new ArgumentNullException(nameof(owner));
            this.summaryText = summaryText ?? throw new ArgumentNullException(nameof(summaryText));
            this.chipPanel = chipPanel ?? throw new ArgumentNullException(nameof(chipPanel));
            this.selectedPurpose = selectedPurpose ?? throw new ArgumentNullException(nameof(selectedPurpose));
            this.selectedLineProperty = selectedLineProperty ?? throw new ArgumentNullException(nameof(selectedLineProperty));
        }

        public void Clear()
        {
            VisionToolResultReviewPresenter.Clear(owner, summaryText, chipPanel);
        }

        public void Show(IEnumerable<LineGaugeResult> results)
        {
            List<LineGaugeResult> lines = results?.Where(item => item != null).ToList()
                ?? new List<LineGaugeResult>();
            int edgePointCount = lines.Sum(item => item.EdgePointCount);
            if (lines.Count == 0 || edgePointCount == 0)
            {
                ShowReview(
                    VisionToolVerificationText.FormatLineEdgeEmptySummary(),
                    false,
                    new[]
                    {
                        VisionToolResultReviewPresenter.Item(VisionToolVerificationText.LinesLabel, 0),
                        VisionToolResultReviewPresenter.Item(VisionToolVerificationText.EdgeLabel, 0)
                    });
                return;
            }

            LineGaugeResult best = lines
                .OrderByDescending(item => item.EdgePointCount)
                .ThenByDescending(item => item.FitLine?.Distance() ?? 0)
                .First();
            double length = best.FitLine?.Distance() ?? 0;
            switch (selectedPurpose())
            {
                case LineToolPurpose.Measure:
                    ShowMeasureReview(best, length, edgePointCount);
                    break;
                case LineToolPurpose.Intersection:
                    ShowLineIntersectionReview(lines, edgePointCount);
                    break;
                default:
                    ShowEdgeReview(lines.Count, edgePointCount, length);
                    break;
            }
        }

        public void ShowDistance(VisionToolResult result)
        {
            if (result?.Success != true || result.Metrics == null)
            {
                ShowReview(
                    VisionToolVerificationText.FormatLineDistanceEmptySummary(),
                    false,
                    new[]
                    {
                        VisionToolResultReviewPresenter.Item(VisionToolVerificationText.DistanceLabel, "-"),
                        VisionToolResultReviewPresenter.Item(VisionToolVerificationText.CountLabel, 0)
                    });
                return;
            }

            result.Metrics.TryGetValue(VisionPipelineKnownMetrics.DistanceCount, out double count);
            result.Metrics.TryGetValue(VisionPipelineKnownMetrics.DistancePxAvg, out double avgPx);
            bool hasMm = result.Metrics.TryGetValue(VisionPipelineKnownMetrics.DistanceMmAvg, out double avgMm);
            string distanceSummary = VisionToolVerificationText.FormatLineDistanceSummary(
                avgPx,
                hasMm ? avgMm : (double?)null,
                count);
            List<VisionToolResultReviewItem> distanceItems = new List<VisionToolResultReviewItem>
            {
                VisionToolResultReviewPresenter.Item(VisionToolVerificationText.DistanceLabel, string.Format(CultureInfo.CurrentCulture, "{0:0.#} px", avgPx)),
                VisionToolResultReviewPresenter.Item(VisionToolVerificationText.CountLabel, string.Format(CultureInfo.CurrentCulture, "{0:0}", count))
            };
            if (hasMm)
            {
                distanceItems.Insert(1, VisionToolResultReviewPresenter.Item(VisionToolVerificationText.MmLabel, string.Format(CultureInfo.CurrentCulture, "{0:0.###}", avgMm)));
            }

            ShowReview(distanceSummary, true, distanceItems);
        }

        public void ShowIntersection(LineGaugeTool lineA, LineGaugeTool lineB, OpenCvSharp.Point intersectionPoint)
        {
            int edgeCount = (lineA?.resultList?.Sum(item => item.EdgePointCount) ?? 0)
                + (lineB?.resultList?.Sum(item => item.EdgePointCount) ?? 0);
            string summary = VisionToolVerificationText.FormatLineIntersectionSummary(
                intersectionPoint.X,
                intersectionPoint.Y,
                edgeCount,
                true);
            ShowReview(
                summary,
                true,
                new[]
                {
                    VisionToolResultReviewPresenter.Item(VisionToolVerificationText.PointLabel, VisionToolResultReviewPresenter.FormatPoint(intersectionPoint.X, intersectionPoint.Y)),
                    VisionToolResultReviewPresenter.Item(VisionToolVerificationText.CrossLabel, VisionToolVerificationText.CrossYes),
                    VisionToolResultReviewPresenter.Item(VisionToolVerificationText.EdgeLabel, edgeCount)
                });
        }

        public void ShowIntersection(VisionToolResult result)
        {
            if (result?.Success != true || result.Metrics == null)
            {
                ShowReview(
                    VisionToolVerificationText.FormatLineIntersectionNoCrossSummary(0),
                    false,
                    new[]
                    {
                        VisionToolResultReviewPresenter.Item(VisionToolVerificationText.CrossLabel, VisionToolVerificationText.CrossNo),
                        VisionToolResultReviewPresenter.Item(VisionToolVerificationText.EdgeLabel, 0)
                    });
                return;
            }

            result.Metrics.TryGetValue("IntersectionCross", out double crosses);
            result.Metrics.TryGetValue("IntersectionX", out double x);
            result.Metrics.TryGetValue("IntersectionY", out double y);
            result.Metrics.TryGetValue(VisionPipelineKnownMetrics.EdgePointCount, out double edgeCount);
            if (crosses < 0.5)
            {
                string failSummary = VisionToolVerificationText.FormatLineIntersectionNoCrossSummary(edgeCount);
                ShowReview(
                    failSummary,
                    false,
                    new[]
                    {
                        VisionToolResultReviewPresenter.Item(VisionToolVerificationText.CrossLabel, VisionToolVerificationText.CrossNo),
                        VisionToolResultReviewPresenter.Item(VisionToolVerificationText.EdgeLabel, string.Format(CultureInfo.CurrentCulture, "{0:0}", edgeCount))
                    });
                return;
            }

            string successSummary = VisionToolVerificationText.FormatLineIntersectionSummary(
                x,
                y,
                edgeCount,
                true);
            ShowReview(
                successSummary,
                true,
                new[]
                {
                    VisionToolResultReviewPresenter.Item(VisionToolVerificationText.PointLabel, VisionToolResultReviewPresenter.FormatPoint(x, y)),
                    VisionToolResultReviewPresenter.Item(VisionToolVerificationText.CrossLabel, VisionToolVerificationText.CrossYes),
                    VisionToolResultReviewPresenter.Item(VisionToolVerificationText.EdgeLabel, string.Format(CultureInfo.CurrentCulture, "{0:0}", edgeCount))
                });
        }

        private void ShowMeasureReview(LineGaugeResult best, double length, int edgePointCount)
        {
            double angle = CalculateAngle(best);
            LineGaugeProperty property = selectedLineProperty();
            double lengthMm = property.PIXELPERMM > 0 ? length * property.PIXELPERMM : 0;
            string measureSummary = VisionToolVerificationText.FormatLineMeasureSummary(length, lengthMm, angle);
            ShowReview(
                measureSummary,
                true,
                new[]
                {
                    VisionToolResultReviewPresenter.Item(VisionToolVerificationText.LengthLabel, string.Format(CultureInfo.CurrentCulture, "{0:0.#} px", length)),
                    VisionToolResultReviewPresenter.Item(VisionToolVerificationText.MmLabel, string.Format(CultureInfo.CurrentCulture, "{0:0.###}", lengthMm)),
                    VisionToolResultReviewPresenter.Item(VisionToolVerificationText.AngleLabel, string.Format(CultureInfo.CurrentCulture, "{0:0.#} deg", angle)),
                    VisionToolResultReviewPresenter.Item(VisionToolVerificationText.EdgeLabel, edgePointCount)
                });
        }

        private void ShowEdgeReview(int lineCount, int edgePointCount, double length)
        {
            string edgeSummary = VisionToolVerificationText.FormatLineEdgeSummary(lineCount, edgePointCount, length);
            ShowReview(
                edgeSummary,
                true,
                new[]
                {
                    VisionToolResultReviewPresenter.Item(VisionToolVerificationText.LinesLabel, lineCount),
                    VisionToolResultReviewPresenter.Item(VisionToolVerificationText.EdgeLabel, edgePointCount),
                    VisionToolResultReviewPresenter.Item(VisionToolVerificationText.LengthLabel, string.Format(CultureInfo.CurrentCulture, "{0:0.#} px", length))
                });
        }

        private void ShowLineIntersectionReview(List<LineGaugeResult> lines, int edgePointCount)
        {
            // Line intersection review is derived result presentation; keep geometry and chip text out of the View.
            if (lines.Count < 2)
            {
                string summary = VisionToolVerificationText.FormatLineNeedPairSummary(edgePointCount);
                ShowReview(
                    summary,
                    false,
                    new[]
                    {
                        VisionToolResultReviewPresenter.Item(VisionToolVerificationText.LinesLabel, lines.Count),
                        VisionToolResultReviewPresenter.Item(VisionToolVerificationText.CrossLabel, VisionToolVerificationText.CrossNo),
                        VisionToolResultReviewPresenter.Item(VisionToolVerificationText.EdgeLabel, edgePointCount)
                    });
                return;
            }

            LineGaugeResult first = lines[0];
            LineGaugeResult second = lines[1];
            bool crosses = TryFindIntersection(first, second, out OpenCvSharp.Point point);
            string review = VisionToolVerificationText.FormatLineIntersectionSummary(
                point.X,
                point.Y,
                edgePointCount,
                crosses);
            ShowReview(
                review,
                crosses,
                new[]
                {
                    VisionToolResultReviewPresenter.Item(VisionToolVerificationText.PointLabel, VisionToolResultReviewPresenter.FormatPoint(point.X, point.Y)),
                    VisionToolResultReviewPresenter.Item(VisionToolVerificationText.CrossLabel, crosses ? VisionToolVerificationText.CrossYes : VisionToolVerificationText.CrossNo),
                    VisionToolResultReviewPresenter.Item(VisionToolVerificationText.EdgeLabel, edgePointCount)
                });
        }

        private void ShowReview(string summary, bool isSuccess, IEnumerable<VisionToolResultReviewItem> items)
        {
            VisionToolResultReviewPresenter.Show(owner, summaryText, chipPanel, summary, isSuccess, items);
        }

        private static double CalculateAngle(LineGaugeResult result)
        {
            if (result?.FitLine == null)
            {
                return 0;
            }

            OpenCvSharp.Point start = result.FitLine.Start;
            OpenCvSharp.Point end = result.FitLine.End;
            return Math.Atan2(end.Y - start.Y, end.X - start.X) * 180D / Math.PI;
        }

        private static bool TryFindIntersection(LineGaugeResult first, LineGaugeResult second, out OpenCvSharp.Point intersection)
        {
            intersection = new OpenCvSharp.Point();
            if (first?.FitLine == null || second?.FitLine == null)
            {
                return false;
            }

            OpenCvSharp.Point p = first.FitLine.Start;
            OpenCvSharp.Point p2 = first.FitLine.End;
            OpenCvSharp.Point q = second.FitLine.Start;
            OpenCvSharp.Point q2 = second.FitLine.End;
            double a1 = p2.Y - p.Y;
            double b1 = p.X - p2.X;
            double c1 = (a1 * p.X) + (b1 * p.Y);
            double a2 = q2.Y - q.Y;
            double b2 = q.X - q2.X;
            double c2 = (a2 * q.X) + (b2 * q.Y);
            double determinant = (a1 * b2) - (a2 * b1);
            if (Math.Abs(determinant) < 0.000001)
            {
                return false;
            }

            intersection = new OpenCvSharp.Point(
                (int)Math.Round(((b2 * c1) - (b1 * c2)) / determinant),
                (int)Math.Round(((a1 * c2) - (a2 * c1)) / determinant));
            return true;
        }
    }
}
