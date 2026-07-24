using System;
using System.Collections.Generic;
using System.Globalization;

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

        public string PinArrayGapRoiText { get; set; }

        public string PinArrayGapPolarityText { get; set; }

        public string PinArrayGapMeasurementText { get; set; }

        public string PinArrayGapRangeMaxText { get; set; }

        public string PinArrayGapDarkThresholdText { get; set; }

        public string PinArrayGapMinDarkCoverageRatioText { get; set; }

        public string PinArrayGapMinPinWidthText { get; set; }

        public string PinArrayGapMaxPinBreakWidthText { get; set; }

        public string PinArrayGapMinGapWidthText { get; set; }

        public string DarkBandGapRoiText { get; set; }

        public string HybridReferencePoseText { get; set; }

        public string HybridRelativeRoiText { get; set; }

        public string HybridSearchRoiText { get; set; }

        public string HybridScoreMinimumText { get; set; }

        public string HybridScoreMarginText { get; set; }

        public string HybridAngleMinimumText { get; set; }

        public string HybridAngleMaximumText { get; set; }

        public string HybridScaleRatioMinimumText { get; set; }

        public string HybridScaleRatioMaximumText { get; set; }

        public string HybridMinimumValidPixelRatioText { get; set; }
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
            if (OpenVisionRecipeLlmIntent.IsPinArrayGapTemplate(template))
            {
                string roiText = FirstNonEmpty(request.PinArrayGapRoiText, request.PinGapRoiText, "MISSING; do not invent coordinates");
                string rangeMaximum = (request.PinArrayGapRangeMaxText ?? string.Empty).Trim();
                string judgementContract = string.IsNullOrWhiteSpace(rangeMaximum)
                    ? "Mode: MEASURE ONLY / NOT JUDGED. Omit UseAcceptance, AcceptanceMetricName, and all acceptance bounds."
                    : "Mode: JUDGED. Every row Step must set UseAcceptance=true, ExpectedSuccess=true, AcceptanceMetricName=DistancePxRange, UseAcceptanceMetricMaximum=true, and AcceptanceMetricMaximum=" + rangeMaximum + ".";

                return string.Join(Environment.NewLine, new[]
                {
                    "This is a self-contained GPT task packet for OpenVisionLab pin-row adjacent edge-gap consistency XML.",
                    "Use only ToolType=PinArrayGap. Do not substitute LineDistance, Contour, Blob, matching, or bounding-box measurements.",
                    "Supported measurement: " + FirstNonEmpty(request.PinArrayGapMeasurementText, OpenVisionRecipePinArrayGapIntentSkill.SupportedMeasurementDefinition) + ". Do not claim center-to-center pitch.",
                    "Supported polarity: " + FirstNonEmpty(request.PinArrayGapPolarityText, OpenVisionRecipePinArrayGapIntentSkill.SupportedPinPolarity) + ". Do not generate a bright-pin recipe.",
                    "Unit mode: " + OpenVisionRecipePinArrayGapIntentSkill.SupportedUnitMode + "-only. Do not add PIXELPERMM or claim physical units.",
                    "Create one PinArrayGap Step per reviewed row ROI. Every ROI must contain exactly one row of roughly vertical dark pins.",
                    "Every Step reads InputLayer=Main and writes a unique OutputLayer. Add ALLOW_BRANCH_INPUT=true after the first Step.",
                    "Row ROIs x,y,w,h separated by semicolons: " + roiText,
                    "DarkThreshold=" + FirstNonEmpty(request.PinArrayGapDarkThresholdText, OpenVisionRecipePinArrayGapIntentSkill.DefaultDarkThreshold.ToString(CultureInfo.InvariantCulture)),
                    "MinDarkCoverageRatio=" + FirstNonEmpty(request.PinArrayGapMinDarkCoverageRatioText, OpenVisionRecipePinArrayGapIntentSkill.DefaultMinimumDarkCoverageRatio.ToString(CultureInfo.InvariantCulture)),
                    "MinPinWidth=" + FirstNonEmpty(request.PinArrayGapMinPinWidthText, OpenVisionRecipePinArrayGapIntentSkill.DefaultMinimumPinWidth.ToString(CultureInfo.InvariantCulture)),
                    "MaxPinBreakWidth=" + FirstNonEmpty(request.PinArrayGapMaxPinBreakWidthText, OpenVisionRecipePinArrayGapIntentSkill.DefaultMaximumPinBreakWidth.ToString(CultureInfo.InvariantCulture)),
                    "MinGapWidth=" + FirstNonEmpty(request.PinArrayGapMinGapWidthText, OpenVisionRecipePinArrayGapIntentSkill.DefaultMinimumGapWidth.ToString(CultureInfo.InvariantCulture)),
                    judgementContract,
                    "Response format: return XML only. No markdown fence, no prose, no explanation before or after the XML."
                });
            }

            if (OpenVisionRecipeLlmIntent.IsHybridRelativeRoiGapTemplate(template))
            {
                return string.Join(Environment.NewLine, new[]
                {
                    "This is a self-contained GPT task packet for an OpenVisionLab locator-aligned dark-band Gap measurement XML.",
                    "Use exactly four enabled Steps in this order: Matching NUM_MATCH=2 ambiguity gate; Matching NUM_MATCH=1 fixture publisher; RotateScale FIXTURE_APPLY_MODE=NormalizeImage; LineDistance USE_GAP_EDGE_PAIR=true.",
                    "Do not substitute Blob, Contour, a raw-image fixed ROI, per-image coordinates, or a model detector.",
                    "Cropped locator template path: " + FirstNonEmpty(request.ReferenceImagePath, "MISSING; do not invent a path"),
                    "Matching search ROI in reference-image coordinates: " + FirstNonEmpty(request.HybridSearchRoiText, "MISSING; do not invent coordinates"),
                    "Reviewed reference pose x,y,angle,scale,imageWidth,imageHeight: " + FirstNonEmpty(request.HybridReferencePoseText, "MISSING; do not invent pose values"),
                    "Reference-coordinate measurement ROI: " + FirstNonEmpty(request.HybridRelativeRoiText, "MISSING; do not invent coordinates"),
                    "Locator gates: SCORE_MIN=" + request.HybridScoreMinimumText + ", ScoreMargin minimum=" + request.HybridScoreMarginText + " percentage points, angle=" + request.HybridAngleMinimumText + ".." + request.HybridAngleMaximumText + " degrees, scale ratio=" + request.HybridScaleRatioMinimumText + ".." + request.HybridScaleRatioMaximumText + ".",
                    "NormalizeImage minimum valid-pixel ratio: " + request.HybridMinimumValidPixelRatioText + ".",
                    "The LineDistance Step is measurement-only and px-only. Use the frozen direct dark-band parameters and do not add Gap OK/NG acceptance or PIXELPERMM calibration.",
                    "Missing, weak, ambiguous, out-of-angle, out-of-scale, or low-coverage location evidence must fail before measurement. Do not weaken a gate to force coverage.",
                    "Required review drawings: both locator candidates or the selected pose, normalized valid bounds/reference axes, reference ROI, candidate edges, selected upper/lower edges, and Gap samples.",
                    "Response format: return XML only. No markdown fence, no prose, no explanation before or after the XML."
                });
            }

            if (OpenVisionRecipeLlmIntent.IsDarkBandGapTemplate(template))
            {
                return string.Join(Environment.NewLine, new[]
                {
                    "This is a self-contained GPT task packet for OpenVisionLab direct dark-band Gap measurement XML.",
                    "Use exactly one enabled ToolType=LineDistance Step. Do not add Matching, a locator, template teaching, NormalizeImage, Blob, or Contour.",
                    "The operator supplies one coarse ROI containing the intended long dark band: " + FirstNonEmpty(request.DarkBandGapRoiText, OpenVisionRecipeDarkBandGapIntentSkill.DefaultRoiText),
                    "Set USE_ROI=true, USE_GAP_EDGE_PAIR=true, and PIXELPERMM=0. Keep the measurement px-only.",
                    "Use the frozen direct-Gap starter values: CANNY_LOW=10, CANNY_HIGH=45, GAP_MIN_PX=12, GAP_MAX_PX=60, GAP_MAX_ANGLE_DEG=8, GAP_MAX_PARALLEL_DELTA_DEG=4, GAP_MIN_SUPPORT_RATIO=0.26, GAP_MIN_DARK_CONTRAST=8, GAP_MIN_DARK_COVERAGE_RATIO=0.25, GAP_MIN_SCORE_MARGIN=0.05.",
                    "Semantic review rule: the magenta lower edge must follow the nearest sustained bright transition after the dark core immediately below the blue upper edge. Reject a farther Hough line even when the wider region is dark on average.",
                    "Do not add an acceptance gate or claim OK/NG. Operator tolerance and calibration are not supplied.",
                    "Required runtime review: green ROI, yellow candidates, blue upper edge, magenta lower edge, five red Gap samples, PASS/REJECT text, DistancePxAvg/Range, stage counts, support, dark coverage, and score margin.",
                    "Response format: return XML only. No markdown fence, no prose, no explanation before or after the XML."
                });
            }

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

        private static string FirstNonEmpty(params string[] values)
        {
            foreach (string value in values ?? Array.Empty<string>())
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    return value.Trim();
                }
            }

            return string.Empty;
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
            if (IsHybridRelativeRoiGapTemplate(template))
            {
                return "Use exactly Matching(2 candidates) -> Matching(1 fixture pose) -> RotateScale NormalizeImage -> LineDistance Gap edge pair. The locator establishes deterministic pose; the LLM does not detect production images. Keep one reviewed search ROI, one reviewed reference pose, and one fixed reference-coordinate measurement ROI. Fail closed before measurement on weak, ambiguous, out-of-angle, out-of-scale, or low-coverage location evidence. Keep the Gap result px-only and measurement-only until operator tolerance and calibration evidence exist.";
            }

            if (IsPinArrayGapTemplate(template))
            {
                return "Use ToolType=PinArrayGap only for all adjacent edge-to-edge clearances in each reviewed single-row ROI of roughly vertical dark pins. Keep units px-only and do not claim center-to-center pitch, bright-pin support, automatic ROI discovery, or calibration. A measurement-only draft has no acceptance fields and must be labelled MEASURE ONLY / NOT JUDGED. A judged draft requires a positive DistancePxRange maximum acceptance gate on every row Step.";
            }

            if (IsDarkBandGapTemplate(template))
            {
                return "Use exactly one ToolType=LineDistance Step with USE_GAP_EDGE_PAIR=true inside one operator-reviewed coarse ROI. Measure in px from a supported upper edge to the nearest sustained lower bright transition of the same dark core; a farther Hough line is not eligible. Do not add Matching, a locator, normalization, calibration, or OK/NG acceptance without operator evidence. Runtime review must retain candidate lines, selected upper/lower edges, Gap samples, stage metrics, support, dark coverage, and ambiguity margin.";
            }

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
            if (IsHybridRelativeRoiGapTemplate(template))
            {
                return "Matching fixture + NormalizeImage + relative-ROI LineDistance Gap";
            }

            if (IsPinArrayGapTemplate(template))
            {
                return "PinArrayGap / adjacent edge gaps + DistancePxRange";
            }

            if (IsDarkBandGapTemplate(template))
            {
                return "LineDistance Gap edge pair / px measurement + candidate/selected-edge evidence";
            }

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
            if (IsPinArrayGapTemplate(value) || IsDarkBandGapTemplate(value) || IsHybridRelativeRoiGapTemplate(value))
            {
                return false;
            }

            return value.IndexOf("LineDistance", StringComparison.OrdinalIgnoreCase) >= 0
                || value.IndexOf("gap", StringComparison.OrdinalIgnoreCase) >= 0
                || value.IndexOf("distance", StringComparison.OrdinalIgnoreCase) >= 0
                || string.Equals(value, "Line Measurement", StringComparison.OrdinalIgnoreCase);
        }

        internal static bool IsPinArrayGapTemplate(string template)
        {
            return string.Equals(
                (template ?? string.Empty).Trim(),
                OpenVisionGuidedSetupCatalog.PinArrayGapTemplate,
                StringComparison.OrdinalIgnoreCase);
        }

        internal static bool IsDarkBandGapTemplate(string template)
        {
            return string.Equals(
                (template ?? string.Empty).Trim(),
                OpenVisionGuidedSetupCatalog.DarkBandGapTemplate,
                StringComparison.OrdinalIgnoreCase);
        }

        internal static bool IsHybridRelativeRoiGapTemplate(string template)
        {
            return string.Equals(
                (template ?? string.Empty).Trim(),
                OpenVisionGuidedSetupCatalog.HybridRelativeRoiGapTemplate,
                StringComparison.OrdinalIgnoreCase);
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
            if (IsHybridRelativeRoiGapTemplate(template))
            {
                return "Use the reviewed locator to publish center/angle/scale, normalize the complete source into reference coordinates, and run the unchanged dark-band Gap ROI on DeviceAligned. Keep ambiguity, pose, scale, and valid-coverage gates fail-closed. Do not move coordinates per image or add Gap acceptance without operator truth.";
            }

            if (IsPinArrayGapTemplate(template))
            {
                return "Use PinArrayGap for every adjacent edge-to-edge clearance in one or more reviewed single-row ROIs of dark, roughly vertical pins. Keep the tool parameters fixed across samples, judge consistency with DistancePxRange maximum on every row, and keep a no-gate draft explicitly measurement-only.";
            }

            if (IsDarkBandGapTemplate(template))
            {
                return "Use one LineDistance Step with USE_GAP_EDGE_PAIR=true inside one reviewed coarse ROI. Keep the starter parameters and px-only measurement. The selected lower edge must be the nearest sustained bright transition after the same dark core, not a farther Hough line. Review candidate lines, selected edges, five Gap samples, stage counts, support, dark coverage, and score margin before any tolerance is added.";
            }

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
