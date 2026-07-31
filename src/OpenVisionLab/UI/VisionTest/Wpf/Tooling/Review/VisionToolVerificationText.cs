using System;
using System.Globalization;

namespace OpenVisionLab
{
    internal static class VisionToolVerificationText
    {
        public static string PreviewOk => T("VisionTool.Status.PreviewOk", "Preview OK");

        public static string PreviewNg => T("VisionTool.Status.PreviewNg", "Preview NG");

        public static string PreviewNotRun => T("VisionTool.Verification.PreviewNotRun", "Preview not run");

        public static string RunPreview => T("VisionTool.Verification.RunPreview", "Run Preview");

        public static string AddToPipeline => T("VisionTool.Verification.AddToPipeline", "Add to Pipeline");

        public static string CheckResultThenAddPipeline =>
            T("VisionTool.Verification.CheckResultThenAddPipeline", "Check the result, then add to pipeline");

        public static string AdjustAreaThreshold =>
            T("VisionTool.Verification.AdjustAreaThreshold", "Adjust threshold, area, or ROI");

        public static string AdjustRoiContrastScan =>
            T("VisionTool.Verification.AdjustRoiContrastScan", "Adjust ROI, contrast, or scan settings");

        public static string ResultNotRun => T("VisionTool.Review.ResultNotRun", "Result not run");

        public static string DecisionLabel => T("VisionTool.Review.Label.Decision", "Decision");

        public static string CriteriaLabel => T("VisionTool.Review.Label.Criteria", "Criteria");

        public static string CountLabel => T("VisionTool.Review.Label.Count", "Count");

        public static string StateLabel => T("VisionTool.Review.Label.State", "State");

        public static string MaxAreaLabel => T("VisionTool.Review.Label.MaxArea", "Max area");

        public static string CenterLabel => T("VisionTool.Review.Label.Center", "Center");

        public static string BoxLabel => T("VisionTool.Review.Label.Box", "Box");

        public static string ScoreLabel => T("VisionTool.Review.Label.Score", "Score");

        public static string AngleLabel => T("VisionTool.Review.Label.Angle", "Angle");

        public static string ScaleLabel => T("VisionTool.Review.Label.Scale", "Scale");

        public static string TactLabel => T("VisionTool.Review.Label.Tact", "Tact");

        public static string DistanceLabel => T("VisionTool.Review.Label.Distance", "Distance");

        public static string LengthLabel => T("VisionTool.Review.Label.Length", "Length");

        public static string LinesLabel => T("VisionTool.Review.Label.Lines", "Lines");

        public static string EdgeLabel => T("VisionTool.Review.Label.Edge", "Edge");

        public static string PointLabel => T("VisionTool.Review.Label.Point", "Point");

        public static string CrossLabel => T("VisionTool.Review.Label.Cross", "Cross");

        public static string MmLabel => T("VisionTool.Review.Label.Mm", "mm");

        public static string NoResult => T("VisionTool.Review.NoResult", "no result");

        public static string NoMatch => T("VisionTool.Review.NoMatch", "no match");

        public static string NoEdge => T("VisionTool.Review.NoEdge", "no edge");

        public static string NeedPair => T("VisionTool.Review.NeedPair", "need pair");

        public static string CrossYes => T("VisionTool.Review.CrossYes", "Yes");

        public static string CrossNo => T("VisionTool.Review.CrossNo", "No");

        public static string FullImage => T("VisionTool.Review.FullImage", "Full image");

        public static string FullImageRoiFallback =>
            T("VisionTool.Review.FullImageRoiFallback", "Full image (ROI not set)");

        public static string OriginalImage => T("VisionTool.Review.OriginalImage", "Original");

        public static string RoiOn => T("VisionTool.Review.RoiOn", "ROI on");

        public static string MultiRoi => T("VisionTool.Review.MultiRoi", "Multi ROI");

        public static string ThresholdOff => T("VisionTool.Review.ThresholdOff", "T off");

        public static string MaskOn => T("VisionTool.Review.MaskOn", "Mask on");

        public static string MaskOff => T("VisionTool.Review.MaskOff", "Mask off");

        public static string AngleOn => T("VisionTool.Review.AngleOn", "Angle on");

        public static string AngleOff => T("VisionTool.Review.AngleOff", "Angle off");

        public static string ScaleOn => T("VisionTool.Review.ScaleOn", "Scale on");

        public static string ScaleOff => T("VisionTool.Review.ScaleOff", "Scale off");

        public static string PyramidOn => T("VisionTool.Review.PyramidOn", "Pyramid on");

        public static string LinePurposeLabel => T("VisionTool.Line.PurposeLabel", "Purpose");

        public static string LineSettingLabel => T("VisionTool.Line.SettingLabel", "Setting");

        public static string LinePurposeEdge => T("VisionTool.Line.Purpose.Edge", "Edge");

        public static string LinePurposeMeasure => T("VisionTool.Line.Purpose.Measure", "Measure");

        public static string LinePurposeIntersection => T("VisionTool.Line.Purpose.Intersection", "Intersection");

        public static string LineA => T("VisionTool.Line.LineA", "Line A");

        public static string LineB => T("VisionTool.Line.LineB", "Line B");

        public static string EditSelectedLineRoiTooltip => T("VisionTool.Line.EditSelectedRoiToolTip", "Edit selected line ROI");

        public static string FormatTeachingState(string toolName)
        {
            return string.Format(
                CultureInfo.CurrentCulture,
                T("VisionTool.Verification.TeachingStateFormat", "{0} teaching"),
                SafeText(toolName, "Tool"));
        }

        public static string FormatResultState(string toolName, string decision)
        {
            return string.Format(
                CultureInfo.CurrentCulture,
                T("VisionTool.Verification.ResultStateFormat", "{0} result / {1}"),
                SafeText(toolName, "Tool"),
                SafeText(decision, "-"));
        }

        public static string FormatVerificationHeader(string toolName)
        {
            return string.Format(
                CultureInfo.CurrentCulture,
                T("VisionTool.Verification.HeaderFormat", "{0} verification"),
                SafeText(toolName, "Tool"));
        }

        public static string FormatNextAction(string nextAction)
        {
            return string.Format(
                CultureInfo.CurrentCulture,
                T("VisionTool.Verification.NextActionFormat", "Next: {0}"),
                SafeText(nextAction, "-"));
        }

        public static string FormatCompactGuide(
            string header,
            string state,
            string criteria,
            string nextAction)
        {
            return string.Format(
                CultureInfo.CurrentCulture,
                T("VisionTool.Verification.CompactGuideFormat", "{0} / {1} / {2} / {3}"),
                SafeText(header, "-"),
                SafeText(state, "-"),
                SafeText(criteria, "-"),
                FormatNextAction(nextAction));
        }

        public static string FormatResultGuidance(
            string decision,
            string criteria,
            string reason,
            string nextAction)
        {
            return string.Format(
                CultureInfo.CurrentCulture,
                T("VisionTool.Verification.ResultGuidanceFormat", "{0} / Criteria: {1} / {2} / Next: {3}"),
                SafeText(decision, "-"),
                SafeText(criteria, "-"),
                SafeText(reason, "-"),
                SafeText(nextAction, "-"));
        }

        public static string CreateAreaSuccessReason(int count, double maxArea)
        {
            return string.Format(
                CultureInfo.CurrentCulture,
                T("VisionTool.Verification.AreaSuccessReasonFormat", "Count {0}, max area {1:0.#}."),
                count,
                maxArea);
        }

        public static string FormatAreaCriteria(int minArea, int maxArea, string thresholdText, string roiText, string suffix)
        {
            return string.Format(
                CultureInfo.CurrentCulture,
                T("VisionTool.Verification.AreaCriteriaFormat", "Area {0}-{1} / {2} / {3} / {4}"),
                minArea,
                maxArea,
                SafeText(thresholdText, "-"),
                SafeText(roiText, "-"),
                SafeText(suffix, "-"));
        }

        public static string FormatThreshold(double threshold)
        {
            return string.Format(
                CultureInfo.CurrentCulture,
                T("VisionTool.Verification.ThresholdFormat", "T {0:0.#}"),
                threshold);
        }

        public static string FormatAdaptiveThreshold(double threshold)
        {
            return string.Format(
                CultureInfo.CurrentCulture,
                T("VisionTool.Verification.AdaptiveThresholdFormat", "Adaptive {0:0.#}"),
                threshold);
        }

        public static string FormatTemplateReadyStatus(string detail)
        {
            return string.Format(
                CultureInfo.CurrentCulture,
                T("VisionTool.TemplateStatus.ReadyFormat", "Template ready / {0}"),
                SafeText(detail, "-"));
        }

        public static string FormatTemplateMissingStatus(string path)
        {
            return string.Format(
                CultureInfo.CurrentCulture,
                T("VisionTool.TemplateStatus.MissingFormat", "Template file missing / {0}"),
                SafeText(path, "-"));
        }

        public static string TemplateNotSelectedStatus =>
            T("VisionTool.TemplateStatus.NotSelected", "Template not selected");

        public static string AreaFailureReason =>
            T("VisionTool.Verification.AreaFailureReason", "No result passed the current area/threshold criteria.");

        public static string FormatAreaSummary(
            string title,
            int count,
            double maxArea,
            double centerX,
            double centerY,
            double boxWidth,
            double boxHeight)
        {
            return string.Format(
                CultureInfo.CurrentCulture,
                T("VisionTool.Review.AreaSummaryFormat", "{0} / Count {1} / Max area {2:0.#} / Center {3:0.#},{4:0.#} / Box {5:0.#}x{6:0.#}"),
                SafeText(title, "Area result"),
                count,
                maxArea,
                centerX,
                centerY,
                boxWidth,
                boxHeight);
        }

        public static string FormatAreaEmptySummary(string title, string emptyState)
        {
            return string.Format(
                CultureInfo.CurrentCulture,
                T("VisionTool.Review.AreaEmptySummaryFormat", "{0} / Count 0 / {1}"),
                SafeText(title, "Area result"),
                SafeText(emptyState, NoResult));
        }

        public static string CreateLineEdgeReason(int lineCount, int edgePointCount)
        {
            return string.Format(
                CultureInfo.CurrentCulture,
                T("VisionTool.Verification.LineEdgeReasonFormat", "Lines {0}, edge {1}."),
                lineCount,
                edgePointCount);
        }

        public static string LineEdgeFailureReason =>
            T("VisionTool.Verification.LineEdgeFailureReason", "No line edge passed the current criteria.");

        public static string CreateLineDistanceReason(double distancePx, double count)
        {
            return string.Format(
                CultureInfo.CurrentCulture,
                T("VisionTool.Verification.LineDistanceReasonFormat", "Distance {0:0.#} px, count {1:0}."),
                distancePx,
                count);
        }

        public static string LineDistanceFailureReason =>
            T("VisionTool.Verification.LineDistanceFailureReason", "Distance not found.");

        public static string LineIntersectionSuccessReason =>
            T("VisionTool.Verification.LineIntersectionSuccessReason", "Intersection cross found.");

        public static string LineIntersectionFailureReason =>
            T("VisionTool.Verification.LineIntersectionFailureReason", "Intersection cross not found.");

        public static string PreviewNotRunCurrentParameters =>
            T("VisionTool.Verification.PreviewNotRunCurrentParameters", "Preview not run / Run Preview to verify current parameters.");

        public static string PreviewNotRunCurrentRoute =>
            T("VisionTool.Verification.PreviewNotRunCurrentRoute", "Preview not run / Run Preview to verify the current input and output layer.");

        public static string FormatMatchCountCriteria(int count)
        {
            return string.Format(
                CultureInfo.CurrentCulture,
                T("VisionTool.Review.MatchCountCriteriaFormat", "Match {0}"),
                count);
        }

        public static string FormatCannyCriteria(string rangeText)
        {
            return string.Format(
                CultureInfo.CurrentCulture,
                T("VisionTool.Review.CannyCriteriaFormat", "Canny {0}"),
                SafeText(rangeText, "-"));
        }

        public static string FormatSearchCriteria(string searchText)
        {
            return string.Format(
                CultureInfo.CurrentCulture,
                T("VisionTool.Review.SearchCriteriaFormat", "Search {0}"),
                SafeText(searchText, "-"));
        }

        public static string FormatPointsCriteria(int points)
        {
            return string.Format(
                CultureInfo.CurrentCulture,
                T("VisionTool.Review.PointsCriteriaFormat", "Points {0}"),
                points);
        }

        public static string FormatFeatureRatioCriteria(double ratio)
        {
            return string.Format(
                CultureInfo.CurrentCulture,
                T("VisionTool.Review.FeatureRatioCriteriaFormat", "Ratio <= {0:0.###}"),
                ratio);
        }

        public static string FormatRansacCriteria(double threshold)
        {
            return string.Format(
                CultureInfo.CurrentCulture,
                T("VisionTool.Review.RansacCriteriaFormat", "RANSAC {0:0.#}"),
                threshold);
        }

        public static string FormatMatchingSummary(
            string title,
            int count,
            double score,
            double centerX,
            double centerY,
            double boxWidth,
            double boxHeight,
            double angle,
            string scaleSuffix,
            string tactSuffix)
        {
            return string.Format(
                CultureInfo.CurrentCulture,
                T("VisionTool.Review.MatchingSummaryFormat", "{0} / Count {1} / Score {2:0.000} / Center {3:0.#},{4:0.#} / Box {5:0.#}x{6:0.#} / Angle {7:0.###}{8}{9}"),
                SafeText(title, "Match"),
                count,
                score,
                centerX,
                centerY,
                boxWidth,
                boxHeight,
                angle,
                scaleSuffix ?? string.Empty,
                tactSuffix ?? string.Empty);
        }

        public static string FormatMatchingEmptySummary(string title)
        {
            return string.Format(
                CultureInfo.CurrentCulture,
                T("VisionTool.Review.MatchingEmptySummaryFormat", "{0} / Count 0 / no match"),
                SafeText(title, "Match"));
        }

        public static string FormatLineCriteria(string purpose, string lineName, double contrast, double samplingStep, string roiText)
        {
            return string.Format(
                CultureInfo.CurrentCulture,
                T("VisionTool.Verification.LineCriteriaFormat", "{0} {1} / C{2:0.#} / S{3:0.#} / {4}"),
                SafeText(purpose, "Line"),
                SafeText(lineName, "Line"),
                contrast,
                samplingStep,
                SafeText(roiText, "-"));
        }

        public static string CreateLinePurposeText(string purposeName)
        {
            if (string.Equals(purposeName, "Measure", StringComparison.OrdinalIgnoreCase))
            {
                return LinePurposeMeasure;
            }

            if (string.Equals(purposeName, "Intersection", StringComparison.OrdinalIgnoreCase))
            {
                return LinePurposeIntersection;
            }

            return LinePurposeEdge;
        }

        public static string CreateLinePurposeHint(string purposeName)
        {
            if (string.Equals(purposeName, "Measure", StringComparison.OrdinalIgnoreCase))
            {
                return T("VisionTool.Line.Purpose.MeasureHint", "Line A scan lines to Line B edge intersections");
            }

            if (string.Equals(purposeName, "Intersection", StringComparison.OrdinalIgnoreCase))
            {
                return T("VisionTool.Line.Purpose.IntersectionHint", "Intersection point from Line A and Line B fit-line crossing");
            }

            return T("VisionTool.Line.Purpose.EdgeHint", "Edge points and fitted line stability for the selected line");
        }

        public static string FormatLineEdgeEmptySummary()
        {
            return T("VisionTool.Review.LineEdgeEmptySummary", "Edge / Count 0 / no edge");
        }

        public static string FormatLineDistanceEmptySummary()
        {
            return T("VisionTool.Review.LineDistanceEmptySummary", "Measure / Distance none / Count 0");
        }

        public static string FormatLineDistanceSummary(double distancePx, double? distanceMm, double count)
        {
            if (distanceMm.HasValue)
            {
                return string.Format(
                    CultureInfo.CurrentCulture,
                    T("VisionTool.Review.LineDistanceSummaryWithMmFormat", "Measure / Distance {0:0.#} px / {1:0.###} mm / Count {2:0}"),
                    distancePx,
                    distanceMm.Value,
                    count);
            }

            return string.Format(
                CultureInfo.CurrentCulture,
                T("VisionTool.Review.LineDistanceSummaryFormat", "Measure / Distance {0:0.#} px / Count {1:0}"),
                distancePx,
                count);
        }

        public static string FormatLineIntersectionSummary(double x, double y, double edgeCount, bool crosses)
        {
            return string.Format(
                CultureInfo.CurrentCulture,
                T("VisionTool.Review.LineIntersectionSummaryFormat", "Intersection / Point {0:0},{1:0} / Cross {2} / Edge {3:0}"),
                x,
                y,
                crosses ? CrossYes : CrossNo,
                edgeCount);
        }

        public static string FormatLineIntersectionNoCrossSummary(double edgeCount)
        {
            return string.Format(
                CultureInfo.CurrentCulture,
                T("VisionTool.Review.LineIntersectionNoCrossSummaryFormat", "Intersection / Cross No / Edge {0:0}"),
                edgeCount);
        }

        public static string FormatLineMeasureSummary(double length, double lengthMm, double angle)
        {
            return string.Format(
                CultureInfo.CurrentCulture,
                T("VisionTool.Review.LineMeasureSummaryFormat", "Measure / Length {0:0.#} px / {1:0.###} mm / Angle {2:0.#} deg"),
                length,
                lengthMm,
                angle);
        }

        public static string FormatLineEdgeSummary(int lineCount, int edgePointCount, double length)
        {
            return string.Format(
                CultureInfo.CurrentCulture,
                T("VisionTool.Review.LineEdgeSummaryFormat", "Edge / Count {0} / Edge {1} / Length {2:0.#}"),
                lineCount,
                edgePointCount,
                length);
        }

        public static string FormatLineNeedPairSummary(int edgePointCount)
        {
            return string.Format(
                CultureInfo.CurrentCulture,
                T("VisionTool.Review.LineNeedPairSummaryFormat", "Intersection / Need pair / Edge {0}"),
                edgePointCount);
        }

        public static string T(string key, string fallbackText)
        {
            string value = OpenVisionLanguageService.T(key);
            return string.IsNullOrWhiteSpace(value) || string.Equals(value, key, StringComparison.Ordinal)
                ? fallbackText
                : value;
        }

        private static string SafeText(string text, string fallback)
        {
            return string.IsNullOrWhiteSpace(text) ? fallback : text.Trim();
        }
    }
}
