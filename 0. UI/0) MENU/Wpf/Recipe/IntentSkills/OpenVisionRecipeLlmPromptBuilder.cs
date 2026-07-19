using System;
using System.Collections.Generic;

namespace OpenVisionLab
{
    internal sealed class OpenVisionRecipeLlmPromptRequest
    {
        public string RecipeName { get; set; }

        public string ActivePipelineName { get; set; }

        public string Template { get; set; }

        public string InspectionGoal { get; set; }

        public string DetectionPoints { get; set; }

        public string ReferenceImagePath { get; set; }

        public string PinGapAverageMetricName { get; set; }

        public string PinGapRangeMetricName { get; set; }

        public string PinGapRoiText { get; set; }

        public bool PinGapIsPixelOnly { get; set; }

        public string PinGapDistanceMinText { get; set; }

        public string PinGapDistanceMaxText { get; set; }

        public string PinGapRangeMaxText { get; set; }

        public string PinGapUnitText { get; set; }

        public string PinGapScaleText { get; set; }
    }

    internal static class OpenVisionRecipeLlmPromptBuilder
    {
        internal static string Build(OpenVisionRecipeLlmPromptRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            string template = request.Template ?? string.Empty;
            string goal = string.IsNullOrWhiteSpace(request.InspectionGoal)
                ? "Describe the inspection target and OK/NG criteria."
                : request.InspectionGoal.Trim();
            string detectionPoints = string.IsNullOrWhiteSpace(request.DetectionPoints)
                ? "List the target ROIs, features, expected pass/fail thresholds, and required output layers."
                : request.DetectionPoints.Trim();
            string referenceImage = string.IsNullOrWhiteSpace(request.ReferenceImagePath)
                ? "No reference image path is selected in OpenVisionLab."
                : request.ReferenceImagePath.Trim();

            List<string> lines = new List<string>
            {
                "Create an OpenVisionLab VisionPipeline XML draft.",
                "Product identity: OpenCvSharp4 rule-based vision workbench; no camera, lighting, PLC, or I/O setup.",
                "Use only OpenVisionLab pipeline tools and parameters. Keep algorithm tool parameters compatible with PropertyGrid-backed tools.",
                "Selected inspection intent: " + template,
                "Intent contract: " + OpenVisionRecipeLlmIntent.BuildLlmIntentContractText(template),
                "Hard rule: do not switch to another tool family unless the selected intent contract explicitly allows it.",
                "Never overwrite the input layer. Read from Main unless a previous step output is intentionally used.",
                "Use score and weight parameters such as SCORE_MIN, GREEDINESS, and HYBRID_VERIFY_IMAGE_WEIGHT as 0..1 decimals, not percentages.",
                "Use positive numeric values for MAGNIFIATION, RANSAC_REPROJ_THRESHOLD, and COARSE_ANGLE_STEP.",
                "Keep FIND_ANGLE_MIN less than or equal to FIND_ANGLE_MAX.",
                "Use only existing template/image dependency paths. If no real file is available, omit dependency path parameters and explain the missing file outside the XML request.",
                "Do not run Preview/Run automatically. The user will validate and import the XML explicitly.",
                "Recipe: " + (request.RecipeName ?? string.Empty),
                "Current active pipeline: " + (request.ActivePipelineName ?? string.Empty),
                "Preferred tool template: " + template,
                "Template guidance: " + OpenVisionRecipeLlmIntent.ResolveTemplateGuidance(template),
                "Reference image: " + referenceImage,
                "Inspection goal: " + goal,
                "Detection points: " + detectionPoints,
                string.Empty,
            };

            string intentPacket = BuildIntentSpecificPromptPacketText(request, template);
            if (!string.IsNullOrWhiteSpace(intentPacket))
            {
                lines.Add("[Intent-specific GPT packet]");
                lines.Add(intentPacket);
                lines.Add(string.Empty);
            }

            lines.AddRange(new[]
            {
                "[Result channel contract]",
                OpenVisionRecipeLlmIntent.BuildLlmResultChannelContractText(),
                "Required response: return only a VisionPipeline XML document that can be loaded by OpenVisionLab."
            });

            return string.Join(Environment.NewLine, lines);
        }

        private static string BuildIntentSpecificPromptPacketText(
            OpenVisionRecipeLlmPromptRequest request,
            string template)
        {
            if (!OpenVisionRecipeLlmIntent.IsLineDistanceTemplate(template))
            {
                return string.Empty;
            }

            return string.Join(Environment.NewLine, new[]
            {
                "This is a self-contained GPT task packet for OpenVisionLab pin gap / pitch / edge-to-edge distance XML.",
                "Operator wording may be simple, such as 'measure pin-to-pin distance'. Do not require a second final XML-only message from the operator; this prompt already carries the XML-only response contract.",
                "Default scope: if the operator did not mark one specific pair or ROI, inspect the whole visible pin array with multiple ROI sample windows.",
                "Use only ToolType=LineDistance for measurement Steps. Do not use Contour, Blob, BoundsHeightAvg, or object bounding boxes to measure pin spacing.",
                "Create one " + (request.PinGapAverageMetricName ?? string.Empty) + " validation Step and one " + (request.PinGapRangeMetricName ?? string.Empty) + " consistency Step for each ROI sample. Reuse the same LineDistance parameters in both Steps with separate OutputLayer values.",
                "Add a final OverlayMerge review Step on Main with SourceLayers set to the consistency/review layers, BurnIn=true, DrawLabels=true, and AllowEmpty=false.",
                "ROI samples x,y,w,h: " + (string.IsNullOrWhiteSpace(request.PinGapRoiText) ? OpenVisionRecipePinGapIntentSkill.DefaultRoiSamplesText : request.PinGapRoiText.Trim()),
                "Unit mode: " + (request.PinGapIsPixelOnly ? "PX-ONLY; do not claim physical units" : "MM-READY"),
                "Nominal gate " + (request.PinGapAverageMetricName ?? string.Empty) + ": " + (request.PinGapDistanceMinText ?? string.Empty) + ".." + (request.PinGapDistanceMaxText ?? string.Empty) + " " + (request.PinGapUnitText ?? string.Empty),
                "Consistency gate " + (request.PinGapRangeMetricName ?? string.Empty) + " <= " + (request.PinGapRangeMaxText ?? string.Empty) + " " + (request.PinGapUnitText ?? string.Empty),
                "Scale mm/px: " + (request.PinGapIsPixelOnly ? "not provided; set PIXELPERMM=0" : request.PinGapScaleText ?? string.Empty),
                "Recommended LineDistance parameters: USE_ROI=true, LeftPRJ_DIR=X_LTOR, RightPRJ_DIR=X_RTOL, PRJ_PORALITY=WTOB, CONTRAST=18, THICKNESS=2, SAMPLING_STEP=16, POINT_RANGE=8, USE_MANUAL_ANGLE=true, MANUAL_ANGLE_VALUE=89, SHOW_EDGE=true, SHOW_VERTICAL_LINE=true.",
                "Response format: return XML only. No markdown fence, no prose, no explanation before or after the XML."
            });
        }
    }

    internal static class OpenVisionRecipeLlmIntent
    {
        internal static string BuildLlmResultChannelContractText()
        {
            return string.Join(Environment.NewLine, new[]
            {
                "- Inspection.Status: final OK/NG is derived by OpenVisionLab from XML validation plus explicit sample/Good/Bad checks after import.",
                "- Inspection.FailedStep: every enabled step must have a clear Name, InputLayer, OutputLayer, and ToolType so failures can point to the exact step.",
                "- Inspection.Evidence: create explicit output layers and measurable acceptance metric/range or parameter criteria such as SCORE_MIN, MIN/MAX, THRESHOLD, AREA, DISTANCE, or MEAN when the tool supports them.",
                "- Inspection.Benchmark: keep deterministic parameters and dependency paths so catalog and run-history comparison can be repeated.",
                "- Inspection.NextAction: do not add custom Inspection.* XML elements; these are logical review channels mapped by OpenVisionLab after import."
            });
        }

        internal static string BuildLlmIntentContractText(string template)
        {
            if (IsLineDistanceTemplate(template))
            {
                return "Use ToolType=LineDistance only for edge-to-edge or pin-to-pin distance. If no specific pair/region is marked, treat pin gap/pitch as a whole-array consistency check and use multiple narrow ROI sample windows across the visible array. Use a single ROI only when the user explicitly marks one pair. Primary value metrics: DistancePxAvg or DistanceMmAvg when PIXELPERMM is known. Quality metrics: DistancePxRange/DistanceMmRange and DistancePxMax/DistanceMmMax must be checked so one long outlier line cannot pass through the average. If both nominal distance and consistency must be judged, duplicate the same LineDistance parameters into a second validation Step with a separate OutputLayer, then add a final OverlayMerge review Step. Do not use Blob or Contour to measure distance.";
            }

            if (IsContourTemplate(template))
            {
                return "Use ToolType=Contour only for boundary, chip, scratch, shape, or region outline checks. Primary metrics: ResultCount, AreaAvg, BoundsWidthAvg, BoundsHeightAvg. Do not use Contour for pin-to-pin gap measurement.";
            }

            if (IsBlobTemplate(template))
            {
                return "Use Threshold followed by Blob for connected object count, area, position, or foreground presence checks. Primary metrics: ResultCount and AreaAvg.";
            }

            if (IsEdgeBasedTemplate(template))
            {
                return "Use ToolType=EdgeBasedMatching for a taught edge shape when edge geometry is more stable than intensity. Start with a full-image search and require SCORE_MIN, NUM_MATCH, CANNY_LOW, CANNY_HIGH, and a ScoreMax acceptance gate. ResultCount is review evidence and must not be the only pass gate.";
            }

            if (IsFeatureMatchingTemplate(template))
            {
                return "Use ToolType=FeatureMatching for a feature-rich template that may move or rotate. Keep USE_ROI=false unless the operator explicitly marks a search region. Require SCORE_MIN and RANSAC_REPROJ_THRESHOLD. Use ScoreMax as the acceptance gate; ResultCount is review evidence and must not be the only pass gate.";
            }

            if (IsMeanTemplate(template))
            {
                return "Use ToolType=Mean for region brightness or intensity band judgment. Primary metric: MeanValueAvg.";
            }

            if (IsReferenceDifferenceTemplate(template))
            {
                return "Use ToolType=ReferenceDifference for registered comparison against one to four operator-approved Good images. Require explicit ReferencePath1..4 values, DifferenceThreshold, defect-area limits, and an exact ResultCount=0 acceptance gate. Do not learn or replace references automatically.";
            }

            return "Use ToolType=Matching for template position or presence checks with a real template path. Primary metrics: ScoreMax and ResultCount.";
        }

        internal static string ResolveIntentSummary(string template)
        {
            if (IsLineDistanceTemplate(template))
            {
                return "LineDistance / DistancePx or DistanceMm Avg + Range";
            }

            if (IsContourTemplate(template))
            {
                return "Contour / ResultCount, AreaAvg, bounds";
            }

            if (IsBlobTemplate(template))
            {
                return "Threshold + Blob / ResultCount, AreaAvg";
            }

            if (IsEdgeBasedTemplate(template))
            {
                return "EdgeBasedMatching / ScoreMax + Canny";
            }

            if (IsFeatureMatchingTemplate(template))
            {
                return "FeatureMatching / ScoreMax + RANSAC";
            }

            if (IsMeanTemplate(template))
            {
                return "Mean / MeanValueAvg";
            }

            if (IsReferenceDifferenceTemplate(template))
            {
                return "ReferenceDifference / ResultCount + registration evidence";
            }

            return "Matching / ScoreMax";
        }

        internal static bool IsLineDistanceTemplate(string template)
        {
            string value = template ?? string.Empty;
            return value.IndexOf("LineDistance", StringComparison.OrdinalIgnoreCase) >= 0
                || value.IndexOf("gap", StringComparison.OrdinalIgnoreCase) >= 0
                || value.IndexOf("distance", StringComparison.OrdinalIgnoreCase) >= 0
                || string.Equals(value, "Line Measurement", StringComparison.OrdinalIgnoreCase);
        }

        internal static bool IsContourTemplate(string template)
        {
            string value = template ?? string.Empty;
            return value.IndexOf("Contour", StringComparison.OrdinalIgnoreCase) >= 0
                || value.IndexOf("boundary", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        internal static bool IsBlobTemplate(string template)
        {
            string value = template ?? string.Empty;
            return value.IndexOf("Blob", StringComparison.OrdinalIgnoreCase) >= 0
                || value.IndexOf("count", StringComparison.OrdinalIgnoreCase) >= 0
                || value.IndexOf("area", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        internal static bool IsMatchingTemplate(string template)
        {
            return string.Equals((template ?? string.Empty).Trim(), "Template Matching", StringComparison.OrdinalIgnoreCase);
        }

        internal static bool IsFeatureMatchingTemplate(string template)
        {
            return string.Equals(
                (template ?? string.Empty).Trim(),
                OpenVisionGuidedSetupCatalog.FeatureMatchingTemplate,
                StringComparison.OrdinalIgnoreCase);
        }

        internal static bool IsEdgeBasedTemplate(string template)
        {
            string value = template ?? string.Empty;
            return value.IndexOf("Edge Based", StringComparison.OrdinalIgnoreCase) >= 0
                || value.IndexOf("EdgeBased", StringComparison.OrdinalIgnoreCase) >= 0
                || value.IndexOf("edge-shape", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        internal static bool IsMeanTemplate(string template)
        {
            string value = template ?? string.Empty;
            return value.IndexOf("Mean", StringComparison.OrdinalIgnoreCase) >= 0
                || value.IndexOf("brightness", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        internal static bool IsReferenceDifferenceTemplate(string template)
        {
            return string.Equals(
                (template ?? string.Empty).Trim(),
                OpenVisionGuidedSetupCatalog.ReferenceDifferenceTemplate,
                StringComparison.OrdinalIgnoreCase);
        }

        internal static string ResolveTemplateGuidance(string template)
        {
            if (IsLineDistanceTemplate(template))
            {
                return "Use LineDistance for pin-to-pin, edge-to-edge, gap, pitch, width, or clearance measurement. If no pair is marked, sample multiple narrow ROI windows across the whole visible pin array and finish with OverlayMerge review. Do not judge DistancePxAvg/DistanceMmAvg alone; also constrain DistancePxRange/DistanceMmRange or DistancePxMax/DistanceMmMax to reject outlier distance lines.";
            }

            if (IsBlobTemplate(template))
            {
                return "Use Threshold to isolate the foreground, then Blob to measure area/count/position.";
            }

            if (IsContourTemplate(template))
            {
                return "Use Contour only for boundary, chip, scratch, shape, or region outline checks; do not use it for pin-to-pin gap measurement.";
            }

            if (IsFeatureMatchingTemplate(template))
            {
                return "Use FeatureMatching for a feature-rich template that may move or rotate. Start with a full-image search, require Ratio and RANSAC settings, and judge ScoreMax rather than ResultCount alone.";
            }

            if (IsEdgeBasedTemplate(template))
            {
                return "Use EdgeBasedMatching for a taught edge shape. Start with a full-image search, tune score and Canny thresholds, then judge ScoreMax instead of ResultCount alone.";
            }

            if (IsMeanTemplate(template))
            {
                return "Use Mean when the judgment is based on brightness or region intensity.";
            }

            if (IsReferenceDifferenceTemplate(template))
            {
                return "Use ReferenceDifference for registered defect comparison against one to four approved Good references. Keep reference selection explicit and judge zero detected defect regions.";
            }

            return "Use Matching when a stable template image and score threshold define the target.";
        }
    }
}
