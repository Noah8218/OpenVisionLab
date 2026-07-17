using Lib.OpenCV.Result;
using Lib.OpenCV.Tool;
using OpenVisionLab.Contracts;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace OpenVisionLab
{
    internal readonly struct LineToolResultExplanation
    {
        private LineToolResultExplanation(
            bool isSuccess,
            string decision,
            string reason,
            string resultNextAction,
            string guideNextAction)
        {
            IsSuccess = isSuccess;
            Decision = decision ?? string.Empty;
            Reason = reason ?? string.Empty;
            ResultNextAction = resultNextAction ?? string.Empty;
            GuideNextAction = guideNextAction ?? string.Empty;
        }

        public bool IsSuccess { get; }
        public string Decision { get; }
        public string Reason { get; }
        public string ResultNextAction { get; }
        public string GuideNextAction { get; }

        public static LineToolResultExplanation CreateLineResult(
            IEnumerable<LineGaugeResult> results,
            LineToolPurpose purpose)
        {
            List<LineGaugeResult> lines = results?.Where(item => item != null).ToList() ?? new List<LineGaugeResult>();
            int edgePointCount = lines.Sum(item => item.EdgePointCount);
            bool isSuccess = lines.Count > 0 && edgePointCount > 0;
            string reason = isSuccess
                ? CreateLineSuccessReason(lines, edgePointCount, purpose)
                : CreateLineFailureReason(purpose);

            return Create(isSuccess, reason);
        }

        public static LineToolResultExplanation CreateDistanceResult(VisionToolResult result)
        {
            bool isSuccess = result?.Success == true;
            string reason;
            if (!isSuccess || result.Metrics == null)
            {
                reason = Combine(
                    VisionToolVerificationText.LineDistanceFailureReason,
                    T("VisionTool.LineReview.DistanceFailureCause", "Cause candidates: Line A/B ROI, scan direction, polarity, contrast, or sampling interval."));
                return Create(false, reason);
            }

            result.Metrics.TryGetValue(VisionPipelineKnownMetrics.DistanceCount, out double count);
            result.Metrics.TryGetValue(VisionPipelineKnownMetrics.DistancePxAvg, out double distancePx);
            result.Metrics.TryGetValue(VisionPipelineKnownMetrics.DistanceMmAvg, out double distanceMm);
            string distanceText = distanceMm > 0D
                ? string.Format(
                    CultureInfo.CurrentCulture,
                    T("VisionTool.LineReview.DistanceSuccessWithMmFormat", "Distance {0:0.#} px / {1:0.###} mm / detected {2:0}."),
                    distancePx,
                    distanceMm,
                    count)
                : VisionToolVerificationText.CreateLineDistanceReason(distancePx, count);
            return Create(
                true,
                Combine(
                    distanceText,
                    T("VisionTool.LineReview.DistanceSuccessHint", "Line A/B scan directions produced a measurable edge-to-edge distance.")));
        }

        public static LineToolResultExplanation CreateIntersectionResult(bool crosses)
        {
            string reason = crosses
                ? Combine(
                    VisionToolVerificationText.LineIntersectionSuccessReason,
                    T("VisionTool.LineReview.IntersectionSuccessHint", "Line A/B fitted lines crossed; check the point position and edge support."))
                : Combine(
                    VisionToolVerificationText.LineIntersectionFailureReason,
                    T("VisionTool.LineReview.IntersectionFailureCause", "Cause candidates: missing Line A/B edges, parallel fitted lines, ROI mismatch, or low contrast."));
            return Create(crosses, reason);
        }

        private static LineToolResultExplanation Create(bool isSuccess, string reason)
        {
            return new LineToolResultExplanation(
                isSuccess,
                isSuccess ? VisionToolVerificationText.PreviewOk : VisionToolVerificationText.PreviewNg,
                reason,
                isSuccess ? VisionToolVerificationText.CheckResultThenAddPipeline : VisionToolVerificationText.AdjustRoiContrastScan,
                isSuccess ? VisionToolVerificationText.AddToPipeline : VisionToolVerificationText.AdjustRoiContrastScan);
        }

        private static string CreateLineSuccessReason(
            List<LineGaugeResult> lines,
            int edgePointCount,
            LineToolPurpose purpose)
        {
            LineGaugeResult best = lines
                .OrderByDescending(item => item.EdgePointCount)
                .ThenByDescending(item => item.FitLine?.Distance() ?? 0D)
                .FirstOrDefault();
            double length = best?.FitLine?.Distance() ?? 0D;
            string metricText = string.Format(
                CultureInfo.CurrentCulture,
                T("VisionTool.LineReview.EdgeSuccessFormat", "Lines {0} / edge points {1} / fit length {2:0.#} px."),
                lines.Count,
                edgePointCount,
                length);
            return Combine(metricText, CreatePurposeHint(purpose));
        }

        private static string CreateLineFailureReason(LineToolPurpose purpose)
        {
            string detail = purpose == LineToolPurpose.Measure
                ? T("VisionTool.LineReview.MeasureFailureCause", "Cause candidates: Line A/B ROI, scan direction, polarity, contrast, or sampling interval.")
                : purpose == LineToolPurpose.Intersection
                    ? T("VisionTool.LineReview.IntersectionFailureCause", "Cause candidates: missing Line A/B edges, parallel fitted lines, ROI mismatch, or low contrast.")
                    : T("VisionTool.LineReview.EdgeFailureCause", "Cause candidates: ROI, contrast, polarity, threshold, scan angle, or sampling interval.");
            return Combine(VisionToolVerificationText.LineEdgeFailureReason, detail);
        }

        private static string CreatePurposeHint(LineToolPurpose purpose)
        {
            if (purpose == LineToolPurpose.Measure)
            {
                return T("VisionTool.LineReview.MeasureSuccessHint", "Edge support was found; distance mode will compare Line A scan lines with Line B edges.");
            }

            if (purpose == LineToolPurpose.Intersection)
            {
                return T("VisionTool.LineReview.IntersectionLineSuccessHint", "Line candidates were found; intersection mode checks whether the fitted Line A/B pair crosses.");
            }

            return T("VisionTool.LineReview.EdgeSuccessHint", "Edge points were found; check fitted line length and stability before adding to the pipeline.");
        }

        private static string Combine(string reason, string detail)
        {
            if (string.IsNullOrWhiteSpace(detail))
            {
                return reason ?? string.Empty;
            }

            if (string.IsNullOrWhiteSpace(reason))
            {
                return detail;
            }

            return reason + " / " + detail;
        }

        private static string T(string key, string fallbackText)
        {
            return VisionToolVerificationText.T(key, fallbackText);
        }
    }
}
