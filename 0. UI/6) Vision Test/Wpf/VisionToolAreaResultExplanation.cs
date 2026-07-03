using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace OpenVisionLab
{
    internal readonly struct VisionToolAreaResultExplanation
    {
        private VisionToolAreaResultExplanation(
            bool isSuccess,
            string decision,
            string reason,
            string nextAction)
        {
            IsSuccess = isSuccess;
            Decision = decision ?? string.Empty;
            Reason = reason ?? string.Empty;
            NextAction = nextAction ?? string.Empty;
        }

        public bool IsSuccess { get; }
        public string Decision { get; }
        public string Reason { get; }
        public string NextAction { get; }

        public static VisionToolAreaResultExplanation Create<TResult>(
            string toolName,
            IReadOnlyCollection<TResult> results,
            Func<TResult, double> getArea,
            Func<TResult, double> getBoxWidth,
            Func<TResult, double> getBoxHeight)
            where TResult : class
        {
            List<TResult> items = results?.Where(item => item != null).ToList() ?? new List<TResult>();
            bool isSuccess = items.Count > 0;
            string decision = isSuccess ? VisionToolVerificationText.PreviewOk : VisionToolVerificationText.PreviewNg;
            string reason = isSuccess
                ? CreateSuccessReason(toolName, items, getArea, getBoxWidth, getBoxHeight)
                : CreateFailureReason(toolName);
            string nextAction = isSuccess
                ? VisionToolVerificationText.CheckResultThenAddPipeline
                : VisionToolVerificationText.AdjustAreaThreshold;

            return new VisionToolAreaResultExplanation(isSuccess, decision, reason, nextAction);
        }

        private static string CreateSuccessReason<TResult>(
            string toolName,
            IReadOnlyCollection<TResult> items,
            Func<TResult, double> getArea,
            Func<TResult, double> getBoxWidth,
            Func<TResult, double> getBoxHeight)
            where TResult : class
        {
            TResult best = items.OrderByDescending(item => SafeValue(getArea, item)).First();
            double maxArea = SafeValue(getArea, best);
            List<string> parts = new List<string>
            {
                VisionToolVerificationText.CreateAreaSuccessReason(items.Count, maxArea)
            };

            if (getBoxWidth != null && getBoxHeight != null)
            {
                string boxFormat = T("VisionTool.AreaReview.BoxFormat", "Max box {0:0.#}x{1:0.#}.");
                parts.Add(string.Format(
                    CultureInfo.CurrentCulture,
                    boxFormat,
                    SafeValue(getBoxWidth, best),
                    SafeValue(getBoxHeight, best)));
            }

            parts.Add(CreateSuccessHint(toolName));
            if (items.Count > 1)
            {
                parts.Add(T(
                    "VisionTool.AreaReview.MultipleCandidateRisk",
                    "Multiple regions passed; if this is not expected, narrow the ROI or tighten area/threshold limits."));
            }

            return string.Join(" / ", parts.Where(item => !string.IsNullOrWhiteSpace(item)));
        }

        private static string CreateFailureReason(string toolName)
        {
            string reason = VisionToolVerificationText.AreaFailureReason;
            string detail = IsContour(toolName)
                ? T("VisionTool.AreaReview.ContourFailureCause", "Cause candidates: threshold range, ROI, area limits, retrieval mode, or weak object boundary.")
                : IsBlob(toolName)
                    ? T("VisionTool.AreaReview.BlobFailureCause", "Cause candidates: threshold range, ROI, area limits, masking, or morphology size.")
                    : T("VisionTool.AreaReview.FailureCause", "Cause candidates: threshold range, ROI, area limits, or weak contrast.");
            return reason + " / " + detail;
        }

        private static string CreateSuccessHint(string toolName)
        {
            if (IsContour(toolName))
            {
                return T("VisionTool.AreaReview.ContourSuccessHint", "Contour area/threshold/ROI criteria passed; check object outline and count.");
            }

            if (IsBlob(toolName))
            {
                return T("VisionTool.AreaReview.BlobSuccessHint", "Blob area/threshold/ROI criteria passed; check region count and largest area.");
            }

            return T("VisionTool.AreaReview.SuccessHint", "Area/threshold/ROI criteria passed; check region count and largest area.");
        }

        private static double SafeValue<TResult>(Func<TResult, double> getter, TResult item)
            where TResult : class
        {
            if (getter == null || item == null)
            {
                return 0D;
            }

            double value = getter(item);
            return double.IsNaN(value) || double.IsInfinity(value) ? 0D : value;
        }

        private static bool IsBlob(string toolName)
        {
            return (toolName ?? string.Empty).IndexOf("Blob", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static bool IsContour(string toolName)
        {
            return (toolName ?? string.Empty).IndexOf("Contour", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static string T(string key, string fallbackText)
        {
            return VisionToolVerificationText.T(key, fallbackText);
        }
    }
}
