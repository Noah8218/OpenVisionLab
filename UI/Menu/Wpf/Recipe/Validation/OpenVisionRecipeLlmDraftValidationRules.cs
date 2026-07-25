using Lib.OpenCV.Pipeline;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using static OpenVisionLab.OpenVisionRecipeLlmIntent;

namespace OpenVisionLab
{
    internal static class OpenVisionRecipeLlmDraftValidationRules
    {
        internal static bool AppendResultChannelValidation(
            VisionPipeline pipeline,
            string xmlText,
            ICollection<string> validationLines)
        {
            List<VisionPipelineStep> enabledSteps = (pipeline?.Steps ?? new List<VisionPipelineStep>())
                .Where(step => step != null && step.Enabled)
                .ToList();
            validationLines.Add(OpenVisionRecipeText.Local("판정 출력 채널: Inspection.Status / Inspection.FailedStep / Inspection.Evidence / Inspection.Benchmark / Inspection.NextAction", "Result channels: Inspection.Status / Inspection.FailedStep / Inspection.Evidence / Inspection.Benchmark / Inspection.NextAction"));

            if (enabledSteps.Count == 0)
            {
                validationLines.Add(OpenVisionRecipeText.Local("오류: Inspection.Status를 만들 수 없습니다. 사용 중인 Step이 없습니다.", "Error: Inspection.Status cannot be derived because there are no enabled steps."));
                return false;
            }

            bool hasOutputLayer = enabledSteps.Any(step => !string.IsNullOrWhiteSpace(step.OutputLayer));
            if (!hasOutputLayer)
            {
                validationLines.Add(OpenVisionRecipeText.Local("오류: Inspection.Evidence를 만들 수 없습니다. 사용 중인 Step의 OutputLayer가 없습니다.", "Error: Inspection.Evidence cannot be derived because enabled steps have no OutputLayer."));
                return false;
            }

            bool hasSeparateOutput = enabledSteps.Any(step =>
                !string.IsNullOrWhiteSpace(step.OutputLayer)
                && !string.Equals(step.InputLayer, step.OutputLayer, StringComparison.OrdinalIgnoreCase));
            if (!hasSeparateOutput)
            {
                validationLines.Add(OpenVisionRecipeText.Local("경고: 모든 출력이 입력과 같습니다. 입력 보존과 Evidence 추적을 위해 별도 OutputLayer를 권장합니다.", "Warning: all outputs match their inputs. Prefer separate OutputLayer values for input preservation and evidence tracing."));
            }

            bool pinArrayGapMeasurementOnly = enabledSteps.All(step =>
                    string.Equals(step.ToolType, "PinArrayGap", StringComparison.OrdinalIgnoreCase))
                && !enabledSteps.Any(HasExplicitAcceptance);
            bool darkBandGapMeasurementOnly = enabledSteps.Count == 1
                && string.Equals(enabledSteps[0].ToolType, "LineDistance", StringComparison.OrdinalIgnoreCase)
                && HasParameterValue(enabledSteps[0], VisionPipelineGapEdgePairTool.UseParameter, value => string.Equals(value, "true", StringComparison.OrdinalIgnoreCase))
                && !enabledSteps.Any(HasExplicitAcceptance);
            bool hybridRelativeRoiMeasurementOnly = enabledSteps.Count == 4
                && string.Equals(enabledSteps[0].ToolType, "Matching", StringComparison.OrdinalIgnoreCase)
                && string.Equals(enabledSteps[1].ToolType, "Matching", StringComparison.OrdinalIgnoreCase)
                && VisionPipelineFixtureFrameService.IsNormalizeImageConsumer(enabledSteps[2])
                && string.Equals(enabledSteps[3].ToolType, "LineDistance", StringComparison.OrdinalIgnoreCase)
                && !HasExplicitAcceptance(enabledSteps[3]);
            bool hasGateParameter = !darkBandGapMeasurementOnly
                && !hybridRelativeRoiMeasurementOnly
                && enabledSteps.Any(HasJudgementParameter);
            validationLines.Add(hybridRelativeRoiMeasurementOnly
                ? "Inspection.Evidence: LOCATION GATED / MEASURE ONLY / NOT JUDGED - locator gates may block measurement, but no product Gap acceptance is present."
                : pinArrayGapMeasurementOnly || darkBandGapMeasurementOnly
                    ? "Inspection.Evidence: MEASURE ONLY / NOT JUDGED - detection parameters are not product acceptance criteria."
                : hasGateParameter
                    ? OpenVisionRecipeText.Local("Inspection.Evidence: OK - 명시적 판정 기준이 있습니다.", "Inspection.Evidence: OK - explicit judgement criteria are present.")
                    : OpenVisionRecipeText.Local("경고: 판정 기준이 명확하지 않습니다. Acceptance metric/range 또는 SCORE_MIN, MIN/MAX, THRESHOLD, AREA, DISTANCE, MEAN 계열 값을 추가하세요.", "Warning: judgement criteria are not explicit. Add an acceptance metric/range or SCORE_MIN, MIN/MAX, THRESHOLD, AREA, DISTANCE, or MEAN style values."));

            AppendOuterCornerReviewRequirement(enabledSteps, validationLines);

            if ((xmlText ?? string.Empty).IndexOf("Inspection.", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                validationLines.Add(OpenVisionRecipeText.Local("오류: Inspection.* 이름은 XML 노드가 아니라 OpenVisionLab 리뷰 채널입니다. 사용자 정의 XML 노드나 파라미터를 제거하세요.", "Error: Inspection.* names are review channels, not XML nodes. Remove custom XML nodes or parameters."));
                return false;
            }

            validationLines.Add(OpenVisionRecipeText.Local("Inspection.Status: OK - XML 검증과 명시적 샘플/Good-Bad 실행 결과에서 파생됩니다.", "Inspection.Status: OK - derived from XML validation and explicit sample/Good-Bad runs."));
            validationLines.Add(OpenVisionRecipeText.Local("Inspection.FailedStep: OK - Step 이름과 경로로 실패 위치를 추적할 수 있습니다.", "Inspection.FailedStep: OK - failures can be traced through step names and routes."));
            validationLines.Add(OpenVisionRecipeText.Local("Inspection.Benchmark: WAIT - 가져오기 후 카탈로그/이력 비교 실행이 필요합니다.", "Inspection.Benchmark: WAIT - run catalog/history comparison after import."));
            validationLines.Add(OpenVisionRecipeText.Local("Inspection.NextAction: OK - 검증 리포트와 작업자 리포트에 다음 조치가 표시됩니다.", "Inspection.NextAction: OK - validation and operator reports expose the next action."));
            return true;
        }

        private static void AppendOuterCornerReviewRequirement(
            IReadOnlyList<VisionPipelineStep> enabledSteps,
            ICollection<string> validationLines)
        {
            List<VisionPipelineStep> cornerSteps = (enabledSteps ?? Array.Empty<VisionPipelineStep>())
                .Where(step => string.Equals(step.ToolType, "OuterCornerIntersection", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(step.ToolType, "BrightObjectCorner", StringComparison.OrdinalIgnoreCase))
                .ToList();
            foreach (VisionPipelineStep _ in cornerSteps)
            {
                validationLines.Add("Corner WAIT: same image; red/green + hough/projection/outer vs mark; no gate if fallback.");
            }
        }

        internal static bool AppendIntentContractValidation(
            VisionPipeline pipeline,
            string template,
            OpenVisionRecipePinArrayGapIntentValidationContext pinArrayGapIntentContext,
            OpenVisionRecipeDarkBandGapIntentValidationContext darkBandGapIntentContext,
            OpenVisionRecipeHybridRelativeRoiIntentValidationContext hybridRelativeRoiIntentContext,
            ICollection<string> validationLines)
        {
            template = template ?? string.Empty;
            if (IsHybridRelativeRoiGapTemplate(template))
            {
                return AppendHybridRelativeRoiIntentContractValidation(
                    pipeline,
                    hybridRelativeRoiIntentContext,
                    validationLines);
            }

            if (IsPinArrayGapTemplate(template))
            {
                return AppendPinArrayGapIntentContractValidation(
                    pipeline,
                    pinArrayGapIntentContext,
                    validationLines);
            }

            if (IsDarkBandGapTemplate(template))
            {
                return AppendDarkBandGapIntentContractValidation(
                    pipeline,
                    darkBandGapIntentContext,
                    validationLines);
            }

            if (IsLineDistanceTemplate(template))
            {
                return AppendRequiredLlmIntentToolValidation(
                    pipeline,
                    validationLines,
                    "LineDistance",
                    "Pin gap / edge distance",
                    "Use ToolType=LineDistance for pin-to-pin, edge-to-edge, gap, pitch, width, or clearance measurement. Do not substitute Contour or Blob.");
            }

            if (IsContourTemplate(template))
            {
                return AppendRequiredLlmIntentToolValidation(
                    pipeline,
                    validationLines,
                    "Contour",
                    "Shape boundary",
                    "Use ToolType=Contour for boundary, chip, scratch, shape, or region outline checks.");
            }

            if (IsBlobTemplate(template))
            {
                return AppendRequiredLlmIntentToolValidation(
                    pipeline,
                    validationLines,
                    "Blob",
                    "Threshold + Blob",
                    "Use ToolType=Blob after Threshold for connected-object count, area, position, or foreground presence checks.");
            }

            if (IsEdgeBasedTemplate(template))
            {
                return AppendEdgeBasedIntentContractValidation(pipeline, validationLines);
            }

            if (IsFeatureMatchingTemplate(template))
            {
                return AppendFeatureMatchingIntentContractValidation(pipeline, validationLines);
            }

            if (IsReferenceDifferenceTemplate(template))
            {
                return AppendReferenceDifferenceIntentContractValidation(pipeline, validationLines);
            }

            validationLines.Add("Intent contract: SKIP - selected intent has no strict tool-family gate.");
            return true;
        }

        private static bool AppendHybridRelativeRoiIntentContractValidation(
            VisionPipeline pipeline,
            OpenVisionRecipeHybridRelativeRoiIntentValidationContext intentContext,
            ICollection<string> validationLines)
        {
            if (intentContext == null
                || !OpenVisionRecipeHybridRelativeRoiIntentSkill.TryValidateInputs(
                    intentContext.LocatorTemplatePath,
                    intentContext.SearchRoiText,
                    intentContext.MeasurementRoiText,
                    intentContext.ReferencePoseText,
                    intentContext.ScoreMinimumText,
                    intentContext.ScoreMarginText,
                    intentContext.AngleMinimumText,
                    intentContext.AngleMaximumText,
                    intentContext.ScaleRatioMinimumText,
                    intentContext.ScaleRatioMaximumText,
                    intentContext.MinimumValidPixelRatioText,
                    out OpenVisionRecipePinGapIntentSkill.RoiSample searchRoi,
                    out OpenVisionRecipePinGapIntentSkill.RoiSample measurementRoi,
                    out OpenVisionRecipeHybridRelativeRoiIntentSkill.ReferencePose referencePose,
                    out double scoreMinimum,
                    out double scoreMargin,
                    out double angleMinimum,
                    out double angleMaximum,
                    out double scaleRatioMinimum,
                    out double scaleRatioMaximum,
                    out double minimumValidPixelRatio,
                    out _))
            {
                validationLines.Add("Hybrid relative-ROI contract: NG - the current Guided Setup locator, pose, ROI, or gate inputs are missing or invalid.");
                return false;
            }

            VisionPipeline expected = OpenVisionRecipeHybridRelativeRoiIntentSkill.CreateMeasurementPipeline(
                intentContext.LocatorTemplatePath,
                searchRoi,
                measurementRoi,
                referencePose,
                scoreMinimum,
                scoreMargin,
                angleMinimum,
                angleMaximum,
                scaleRatioMinimum,
                scaleRatioMaximum,
                minimumValidPixelRatio);
            List<VisionPipelineStep> actualSteps = (pipeline?.Steps ?? new List<VisionPipelineStep>())
                .Where(step => step != null && step.Enabled)
                .ToList();
            List<VisionPipelineStep> expectedSteps = expected.Steps
                .Where(step => step != null && step.Enabled)
                .ToList();
            if (actualSteps.Count != expectedSteps.Count)
            {
                validationLines.Add("Hybrid relative-ROI contract: NG - require exactly four enabled Steps in the locked order.");
                return false;
            }

            for (int index = 0; index < expectedSteps.Count; index++)
            {
                if (!MatchesLockedStep(actualSteps[index], expectedSteps[index]))
                {
                    validationLines.Add(
                        "Hybrid relative-ROI contract: NG - Step "
                        + (index + 1).ToString(CultureInfo.InvariantCulture)
                        + " must match the current reviewed tool, route, acceptance, ROI, fixture, and gate values exactly.");
                    return false;
                }
            }

            validationLines.Add("Hybrid relative-ROI contract: OK - Matching ambiguity -> Matching fixture -> NormalizeImage -> reference-ROI LineDistance is locked to the reviewed inputs.");
            validationLines.Add("Hybrid relative-ROI judgement: LOCATION GATED / MEASURE ONLY / NOT JUDGED - locator failures block measurement; no Gap OK/NG tolerance or calibration is present.");
            validationLines.Add("Hybrid relative-ROI drawings: WAIT - explicit Run must show locator candidates/pose, normalized valid bounds, the reference ROI, selected edges, and Gap samples.");
            return true;
        }

        private static bool MatchesLockedStep(VisionPipelineStep actual, VisionPipelineStep expected)
        {
            if (actual == null
                || expected == null
                || !string.Equals(actual.Name, expected.Name, StringComparison.Ordinal)
                || !string.Equals(actual.ToolType, expected.ToolType, StringComparison.OrdinalIgnoreCase)
                || !string.Equals(actual.InputLayer, expected.InputLayer, StringComparison.OrdinalIgnoreCase)
                || !string.Equals(actual.OutputLayer, expected.OutputLayer, StringComparison.OrdinalIgnoreCase)
                || actual.UseAcceptance != expected.UseAcceptance
                || actual.ExpectedSuccess != expected.ExpectedSuccess
                || !string.Equals(actual.AcceptanceMetricName ?? string.Empty, expected.AcceptanceMetricName ?? string.Empty, StringComparison.OrdinalIgnoreCase)
                || actual.UseAcceptanceMetricMinimum != expected.UseAcceptanceMetricMinimum
                || actual.UseAcceptanceMetricMaximum != expected.UseAcceptanceMetricMaximum
                || Math.Abs(actual.AcceptanceMetricMinimum - expected.AcceptanceMetricMinimum) > 0.000000001D
                || Math.Abs(actual.AcceptanceMetricMaximum - expected.AcceptanceMetricMaximum) > 0.000000001D
                || actual.Parameters == null
                || expected.Parameters == null
                || actual.Parameters.Count != expected.Parameters.Count)
            {
                return false;
            }

            foreach (KeyValuePair<string, string> pair in expected.Parameters)
            {
                if (!actual.Parameters.TryGetValue(pair.Key, out string actualValue)
                    || !MatchesLockedParameterValue(actualValue, pair.Value))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool MatchesLockedParameterValue(string actual, string expected)
        {
            string left = (actual ?? string.Empty).Trim();
            string right = (expected ?? string.Empty).Trim();
            if (bool.TryParse(left, out bool leftBoolean) && bool.TryParse(right, out bool rightBoolean))
            {
                return leftBoolean == rightBoolean;
            }

            if (double.TryParse(left, NumberStyles.Float, CultureInfo.InvariantCulture, out double leftNumber)
                && double.TryParse(right, NumberStyles.Float, CultureInfo.InvariantCulture, out double rightNumber)
                && !double.IsNaN(leftNumber)
                && !double.IsInfinity(leftNumber)
                && !double.IsNaN(rightNumber)
                && !double.IsInfinity(rightNumber))
            {
                return Math.Abs(leftNumber - rightNumber) < 0.000000001D;
            }

            return string.Equals(left, right, StringComparison.OrdinalIgnoreCase);
        }

        private static bool AppendDarkBandGapIntentContractValidation(
            VisionPipeline pipeline,
            OpenVisionRecipeDarkBandGapIntentValidationContext intentContext,
            ICollection<string> validationLines)
        {
            List<VisionPipelineStep> enabledSteps = (pipeline?.Steps ?? new List<VisionPipelineStep>())
                .Where(step => step != null && step.Enabled)
                .ToList();
            if (enabledSteps.Count != 1)
            {
                validationLines.Add("Dark-band Gap contract: NG - require exactly one enabled measurement Step.");
                return false;
            }

            if (!OpenVisionRecipeDarkBandGapIntentSkill.TryParseCoarseRoi(
                    intentContext?.CoarseRoiText,
                    out OpenVisionRecipePinGapIntentSkill.RoiSample expectedRoi,
                    out _))
            {
                validationLines.Add("Dark-band Gap contract: NG - one operator-reviewed coarse ROI is required.");
                return false;
            }

            VisionPipelineStep step = enabledSteps[0];
            bool structureReady = string.Equals(step.ToolType, "LineDistance", StringComparison.OrdinalIgnoreCase)
                && string.Equals(step.InputLayer, "Main", StringComparison.OrdinalIgnoreCase)
                && !string.IsNullOrWhiteSpace(step.OutputLayer)
                && !string.Equals(step.InputLayer, step.OutputLayer, StringComparison.OrdinalIgnoreCase)
                && !HasExplicitAcceptance(step);
            bool parametersReady = HasParameterValue(step, "USE_ROI", IsTrue)
                && HasParameterValue(step, "CvROI", value => MatchesRoi(value, expectedRoi))
                && HasParameterValue(step, VisionPipelineGapEdgePairTool.UseParameter, IsTrue)
                && HasParameterValue(step, "PIXELPERMM", value => MatchesDouble(value, 0D))
                && HasParameterValue(step, "CANNY_LOW", value => TryParseInteger(value, 0, 255, out int parsed) && parsed == OpenVisionRecipeDarkBandGapIntentSkill.DefaultCannyLow)
                && HasParameterValue(step, "CANNY_HIGH", value => TryParseInteger(value, 0, 255, out int parsed) && parsed == OpenVisionRecipeDarkBandGapIntentSkill.DefaultCannyHigh)
                && HasParameterValue(step, VisionPipelineGapEdgePairTool.MinimumGapParameter, value => MatchesDouble(value, OpenVisionRecipeDarkBandGapIntentSkill.DefaultMinimumGapPixels))
                && HasParameterValue(step, VisionPipelineGapEdgePairTool.MaximumGapParameter, value => MatchesDouble(value, OpenVisionRecipeDarkBandGapIntentSkill.DefaultMaximumGapPixels))
                && HasParameterValue(step, VisionPipelineGapEdgePairTool.MaximumAngleParameter, value => MatchesDouble(value, OpenVisionRecipeDarkBandGapIntentSkill.DefaultMaximumAngleDegrees))
                && HasParameterValue(step, VisionPipelineGapEdgePairTool.MaximumParallelDeltaParameter, value => MatchesDouble(value, OpenVisionRecipeDarkBandGapIntentSkill.DefaultMaximumParallelDeltaDegrees))
                && HasParameterValue(step, VisionPipelineGapEdgePairTool.MinimumSupportRatioParameter, value => MatchesDouble(value, OpenVisionRecipeDarkBandGapIntentSkill.DefaultMinimumSupportRatio))
                && HasParameterValue(step, VisionPipelineGapEdgePairTool.MinimumDarkContrastParameter, value => MatchesDouble(value, OpenVisionRecipeDarkBandGapIntentSkill.DefaultMinimumDarkContrast))
                && HasParameterValue(step, VisionPipelineGapEdgePairTool.MinimumDarkCoverageParameter, value => MatchesDouble(value, OpenVisionRecipeDarkBandGapIntentSkill.DefaultMinimumDarkCoverageRatio))
                && HasParameterValue(step, VisionPipelineGapEdgePairTool.MinimumScoreMarginParameter, value => MatchesDouble(value, OpenVisionRecipeDarkBandGapIntentSkill.DefaultMinimumScoreMargin));
            IReadOnlyList<string> metricNames = VisionPipelineKnownMetrics.GetMetricNamesForTool("LineDistance");
            string[] requiredMetrics =
            {
                VisionPipelineKnownMetrics.DistancePxAvg,
                VisionPipelineKnownMetrics.DistancePxRange,
                VisionPipelineKnownMetrics.GapCandidateLineCount,
                VisionPipelineKnownMetrics.GapOverlapPairCount,
                VisionPipelineKnownMetrics.GapSeparationPairCount,
                VisionPipelineKnownMetrics.GapParallelPairCount,
                VisionPipelineKnownMetrics.GapContrastPairCount,
                VisionPipelineKnownMetrics.GapSelectedSupportRatio,
                VisionPipelineKnownMetrics.GapDarkCoverageRatio,
                VisionPipelineKnownMetrics.GapScoreMargin
            };
            bool metricsReady = requiredMetrics.All(required => metricNames.Contains(required, StringComparer.OrdinalIgnoreCase));

            validationLines.Add(structureReady && parametersReady
                ? "Dark-band Gap contract: MEASURE ONLY / NOT JUDGED - one reviewed ROI and the frozen direct-Gap LineDistance parameters are present."
                : "Dark-band Gap contract: NG - require one Main->distinct-output LineDistance Step, the exact reviewed ROI, USE_GAP_EDGE_PAIR=true, PIXELPERMM=0, frozen direct-Gap starter parameters, and no acceptance gate.");
            validationLines.Add(metricsReady
                ? "Dark-band Gap evidence metrics: OK - distance, stage counts, support, dark coverage, and ambiguity margin are registered."
                : "Dark-band Gap evidence metrics: NG - one or more required runtime metrics are not registered.");
            validationLines.Add("Dark-band Gap drawings: WAIT - after explicit Run, review green ROI, yellow candidates, blue/magenta selected edges, five red Gap samples, and PASS/REJECT text.");
            return structureReady && parametersReady && metricsReady;
        }

        private static bool AppendPinArrayGapIntentContractValidation(
            VisionPipeline pipeline,
            OpenVisionRecipePinArrayGapIntentValidationContext intentContext,
            ICollection<string> validationLines)
        {
            List<VisionPipelineStep> enabledSteps = (pipeline?.Steps ?? new List<VisionPipelineStep>())
                .Where(step => step != null && step.Enabled)
                .ToList();
            if (enabledSteps.Count == 0)
            {
                validationLines.Add("PinArrayGap contract: NG - require at least one enabled row Step.");
                return false;
            }

            IReadOnlyList<OpenVisionRecipePinGapIntentSkill.RoiSample> expectedRowRois = Array.Empty<OpenVisionRecipePinGapIntentSkill.RoiSample>();
            int expectedDarkThreshold = 0;
            double expectedCoverageRatio = 0D;
            int expectedMinimumPinWidth = 0;
            int expectedMaximumBreakWidth = 0;
            int expectedMinimumGapWidth = 0;
            double expectedRangeMaximum = 0D;
            bool expectedRoisReady = intentContext != null
                && OpenVisionRecipePinArrayGapIntentSkill.TryParseRowRois(
                    intentContext.RowRoiText,
                    out expectedRowRois,
                    out _);
            bool supportedIntentReady = intentContext != null
                && string.Equals(
                    intentContext.PolarityText,
                    OpenVisionRecipePinArrayGapIntentSkill.SupportedPinPolarity,
                    StringComparison.OrdinalIgnoreCase)
                && string.Equals(
                    intentContext.MeasurementText,
                    OpenVisionRecipePinArrayGapIntentSkill.SupportedMeasurementDefinition,
                    StringComparison.OrdinalIgnoreCase);
            bool expectedThresholdReady = intentContext != null
                && TryParseInteger(intentContext.DarkThresholdText, 0, 255, out expectedDarkThreshold);
            bool expectedCoverageReady = intentContext != null
                && TryParseCoverageRatio(intentContext.MinimumDarkCoverageRatioText, out expectedCoverageRatio);
            bool expectedMinimumPinWidthReady = intentContext != null
                && TryParseInteger(intentContext.MinimumPinWidthText, 1, int.MaxValue, out expectedMinimumPinWidth);
            bool expectedMaximumBreakWidthReady = intentContext != null
                && TryParseInteger(intentContext.MaximumPinBreakWidthText, 0, int.MaxValue, out expectedMaximumBreakWidth);
            bool expectedMinimumGapWidthReady = intentContext != null
                && TryParseInteger(intentContext.MinimumGapWidthText, 1, int.MaxValue, out expectedMinimumGapWidth);
            string expectedRangeText = (intentContext?.RangeMaximumText ?? string.Empty).Trim();
            bool expectedMeasurementOnly = expectedRangeText.Length == 0;
            bool expectedRangeReady = expectedMeasurementOnly
                || (double.TryParse(expectedRangeText, NumberStyles.Float, CultureInfo.InvariantCulture, out expectedRangeMaximum)
                    && !double.IsNaN(expectedRangeMaximum)
                    && !double.IsInfinity(expectedRangeMaximum)
                    && expectedRangeMaximum > 0D);
            bool expectedSkillStateReady = expectedRoisReady
                && supportedIntentReady
                && expectedThresholdReady
                && expectedCoverageReady
                && expectedMinimumPinWidthReady
                && expectedMaximumBreakWidthReady
                && expectedMinimumGapWidthReady
                && expectedRangeReady
                && OpenVisionRecipePinArrayGapIntentSkill.TryValidateV1Inputs(
                    intentContext.MeasurementText,
                    intentContext.PolarityText,
                    OpenVisionRecipePinArrayGapIntentSkill.SupportedUnitMode,
                    expectedRowRois,
                    intentContext.SourceWidth,
                    intentContext.SourceHeight,
                    expectedDarkThreshold,
                    expectedCoverageRatio,
                    expectedMinimumPinWidth,
                    expectedMaximumBreakWidth,
                    expectedMinimumGapWidth,
                    out _);

            if (!expectedSkillStateReady)
            {
                validationLines.Add("PinArrayGap contract: NG - the current Guided Setup skill state or source-bounded ROI is missing, unsupported, or invalid.");
                return false;
            }

            List<string> wrongTools = enabledSteps
                .Where(step => !string.Equals(step.ToolType, "PinArrayGap", StringComparison.OrdinalIgnoreCase))
                .Select(step => step.ToolType ?? "-")
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(value => value, StringComparer.OrdinalIgnoreCase)
                .ToList();
            if (wrongTools.Count > 0)
            {
                validationLines.Add("PinArrayGap contract: NG - every enabled Step must use ToolType=PinArrayGap.");
                validationLines.Add("Draft non-PinArrayGap ToolTypes: " + string.Join(", ", wrongTools));
                return false;
            }

            bool structureReady = enabledSteps.Count == expectedRowRois.Count;
            if (!structureReady)
            {
                validationLines.Add(
                    "PinArrayGap row contract: NG - enabled row Step count "
                    + enabledSteps.Count.ToString(CultureInfo.InvariantCulture)
                    + " does not match the reviewed ROI count "
                    + expectedRowRois.Count.ToString(CultureInfo.InvariantCulture)
                    + ".");
            }

            HashSet<string> allowedParameterKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "Name",
                "USE_ROI",
                "CvROI",
                "DarkThreshold",
                "MinDarkCoverageRatio",
                "MinPinWidth",
                "MaxPinBreakWidth",
                "MinGapWidth",
                "ALLOW_BRANCH_INPUT"
            };
            HashSet<string> outputLayers = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            for (int index = 0; index < enabledSteps.Count; index++)
            {
                VisionPipelineStep step = enabledSteps[index];
                string stepName = string.IsNullOrWhiteSpace(step.Name) ? "row " + (index + 1).ToString(CultureInfo.InvariantCulture) : step.Name;
                bool matchesReviewedRoi = index < expectedRowRois.Count
                    && HasParameterValue(
                        step,
                        "CvROI",
                        value => MatchesRoi(value, expectedRowRois[index]));
                bool rowReady = string.Equals(step.InputLayer, "Main", StringComparison.OrdinalIgnoreCase)
                    && !string.IsNullOrWhiteSpace(step.OutputLayer)
                    && outputLayers.Add(step.OutputLayer)
                    && step.Parameters != null
                    && step.Parameters.Keys.All(allowedParameterKeys.Contains)
                    && HasParameterValue(step, "USE_ROI", value => string.Equals(value, "true", StringComparison.OrdinalIgnoreCase))
                    && matchesReviewedRoi
                    && HasParameterValue(step, "DarkThreshold", value => TryParseInteger(value, 0, 255, out int parsed) && parsed == expectedDarkThreshold)
                    && HasParameterValue(step, "MinDarkCoverageRatio", value => TryParseCoverageRatio(value, out double parsed) && Math.Abs(parsed - expectedCoverageRatio) < 0.000000001D)
                    && HasParameterValue(step, "MinPinWidth", value => TryParseInteger(value, 1, int.MaxValue, out int parsed) && parsed == expectedMinimumPinWidth)
                    && HasParameterValue(step, "MaxPinBreakWidth", value => TryParseInteger(value, 0, int.MaxValue, out int parsed) && parsed == expectedMaximumBreakWidth)
                    && HasParameterValue(step, "MinGapWidth", value => TryParseInteger(value, 1, int.MaxValue, out int parsed) && parsed == expectedMinimumGapWidth);
                if (index > 0)
                {
                    rowReady = rowReady
                        && HasParameterValue(step, "ALLOW_BRANCH_INPUT", value => string.Equals(value, "true", StringComparison.OrdinalIgnoreCase));
                }

                if (!rowReady)
                {
                    structureReady = false;
                    validationLines.Add("PinArrayGap row contract: NG - " + stepName + " requires Main input, unique output, valid ROI/detection parameters matching the reviewed state, and ALLOW_BRANCH_INPUT=true after the first row.");
                }
            }

            bool anyAcceptance = enabledSteps.Any(HasExplicitAcceptance);
            if (expectedMeasurementOnly && !anyAcceptance)
            {
                validationLines.Add("PinArrayGap contract: MEASURE ONLY / NOT JUDGED - no acceptance gate is present.");
                return structureReady;
            }

            if (expectedMeasurementOnly)
            {
                validationLines.Add("PinArrayGap contract: NG - the current Guided Setup state is measurement-only, so acceptance fields must be omitted.");
                return false;
            }

            if (!anyAcceptance)
            {
                validationLines.Add("PinArrayGap contract: NG - the current Guided Setup state requires a DistancePxRange maximum on every row.");
                return false;
            }

            bool everyRowJudged = enabledSteps.All(step =>
                step.UseAcceptance
                && step.ExpectedSuccess
                && string.Equals(step.AcceptanceMetricName, VisionPipelineKnownMetrics.DistancePxRange, StringComparison.OrdinalIgnoreCase)
                && !step.UseAcceptanceMetricMinimum
                && step.UseAcceptanceMetricMaximum
                && Math.Abs(step.AcceptanceMetricMaximum - expectedRangeMaximum) < 0.000000001D);
            bool oneFrozenMaximum = everyRowJudged
                && enabledSteps
                    .Select(step => step.AcceptanceMetricMaximum)
                    .Distinct()
                    .Count() == 1;
            validationLines.Add(structureReady && oneFrozenMaximum
                ? "PinArrayGap contract: OK - every row uses a positive DistancePxRange maximum acceptance gate with the same frozen maximum."
                : !structureReady
                    ? "PinArrayGap contract: NG - one or more row Steps do not match the reviewed ROI and locked detection state."
                    : "PinArrayGap contract: NG - once any acceptance field is present, every row must use ExpectedSuccess=true and the same positive DistancePxRange maximum-only acceptance gate.");
            return structureReady && oneFrozenMaximum;
        }

        private static bool AppendReferenceDifferenceIntentContractValidation(
            VisionPipeline pipeline,
            ICollection<string> validationLines)
        {
            bool toolReady = AppendRequiredLlmIntentToolValidation(
                pipeline,
                validationLines,
                "ReferenceDifference",
                "Golden-reference defect",
                "Use ToolType=ReferenceDifference for the selected Golden-reference defect intent.");
            VisionPipelineStep step = (pipeline?.Steps ?? new List<VisionPipelineStep>())
                .FirstOrDefault(candidate => candidate != null
                    && candidate.Enabled
                    && string.Equals(candidate.ToolType, "ReferenceDifference", StringComparison.OrdinalIgnoreCase));
            bool gateReady = step != null
                && step.UseAcceptance
                && string.Equals(step.AcceptanceMetricName, VisionPipelineKnownMetrics.ResultCount, StringComparison.OrdinalIgnoreCase)
                && step.UseAcceptanceMetricMinimum
                && Math.Abs(step.AcceptanceMetricMinimum) < 0.000001D
                && step.UseAcceptanceMetricMaximum
                && Math.Abs(step.AcceptanceMetricMaximum) < 0.000001D;
            validationLines.Add(gateReady
                ? "Golden-reference defect contract: OK - ReferenceDifference uses exact ResultCount=0 acceptance."
                : "Golden-reference defect contract: NG - require exact ResultCount=0 acceptance on the enabled ReferenceDifference Step.");
            return toolReady && gateReady;
        }

        private static bool AppendEdgeBasedIntentContractValidation(
            VisionPipeline pipeline,
            ICollection<string> validationLines)
        {
            bool toolReady = AppendRequiredLlmIntentToolValidation(
                pipeline,
                validationLines,
                "EdgeBasedMatching",
                "Edge Based Matching",
                "Use ToolType=EdgeBasedMatching for the selected Edge Based Matching intent.");
            VisionPipelineStep edgeStep = (pipeline?.Steps ?? new List<VisionPipelineStep>())
                .FirstOrDefault(step => step != null
                    && step.Enabled
                    && string.Equals(step.ToolType, "EdgeBasedMatching", StringComparison.OrdinalIgnoreCase));
            if (edgeStep == null)
            {
                return false;
            }

            bool hasScoreMinimum = edgeStep.Parameters != null
                && edgeStep.Parameters.Keys.Any(key => string.Equals(key, "SCORE_MIN", StringComparison.OrdinalIgnoreCase));
            bool hasSearchCount = edgeStep.Parameters != null
                && edgeStep.Parameters.Keys.Any(key => string.Equals(key, "NUM_MATCH", StringComparison.OrdinalIgnoreCase));
            bool hasCannyLow = edgeStep.Parameters != null
                && edgeStep.Parameters.Keys.Any(key => string.Equals(key, "CANNY_LOW", StringComparison.OrdinalIgnoreCase));
            bool hasCannyHigh = edgeStep.Parameters != null
                && edgeStep.Parameters.Keys.Any(key => string.Equals(key, "CANNY_HIGH", StringComparison.OrdinalIgnoreCase));
            bool hasFullImageScope = edgeStep.Parameters != null
                && edgeStep.Parameters.Any(pair => string.Equals(pair.Key, "USE_ROI", StringComparison.OrdinalIgnoreCase)
                    && string.Equals(pair.Value, "false", StringComparison.OrdinalIgnoreCase));
            bool hasScoreMaxGate = edgeStep.UseAcceptance
                && edgeStep.UseAcceptanceMetricMinimum
                && string.Equals(edgeStep.AcceptanceMetricName, VisionPipelineKnownMetrics.ScoreMax, StringComparison.OrdinalIgnoreCase);

            if (hasScoreMinimum && hasSearchCount && hasCannyLow && hasCannyHigh && hasFullImageScope && hasScoreMaxGate)
            {
                validationLines.Add("Edge Based Matching contract: OK - score, search count, Canny, full-image scope, and ScoreMax minimum gate are present.");
                return toolReady;
            }

            validationLines.Add("Error: Edge Based Matching requires SCORE_MIN, NUM_MATCH, CANNY_LOW/HIGH, USE_ROI=false, and a ScoreMax minimum acceptance gate.");
            validationLines.Add("Next: keep ResultCount as review evidence; add the missing EdgeBasedMatching score, Canny, scope, or ScoreMax gate before importing.");
            return false;
        }

        private static bool AppendFeatureMatchingIntentContractValidation(
            VisionPipeline pipeline,
            ICollection<string> validationLines)
        {
            bool toolReady = AppendRequiredLlmIntentToolValidation(
                pipeline,
                validationLines,
                "FeatureMatching",
                "Feature Matching",
                "Use ToolType=FeatureMatching for the selected Feature Matching intent.");
            VisionPipelineStep featureStep = (pipeline?.Steps ?? new List<VisionPipelineStep>())
                .FirstOrDefault(step => step != null
                    && step.Enabled
                    && string.Equals(step.ToolType, "FeatureMatching", StringComparison.OrdinalIgnoreCase));
            if (featureStep == null)
            {
                return false;
            }

            bool hasRatioMinimum = featureStep.Parameters != null
                && featureStep.Parameters.Keys.Any(key => string.Equals(key, "SCORE_MIN", StringComparison.OrdinalIgnoreCase));
            bool hasRansacThreshold = featureStep.Parameters != null
                && featureStep.Parameters.Keys.Any(key => string.Equals(key, "RANSAC_REPROJ_THRESHOLD", StringComparison.OrdinalIgnoreCase));
            bool hasScoreMaxGate = featureStep.UseAcceptance
                && featureStep.UseAcceptanceMetricMinimum
                && string.Equals(featureStep.AcceptanceMetricName, VisionPipelineKnownMetrics.ScoreMax, StringComparison.OrdinalIgnoreCase);

            if (hasRatioMinimum && hasRansacThreshold && hasScoreMaxGate)
            {
                validationLines.Add("Feature Matching contract: OK - SCORE_MIN, RANSAC_REPROJ_THRESHOLD, and ScoreMax minimum gate are present.");
                return toolReady;
            }

            validationLines.Add("Error: Feature Matching requires SCORE_MIN, RANSAC_REPROJ_THRESHOLD, and a ScoreMax minimum acceptance gate.");
            validationLines.Add("Next: keep ResultCount as review evidence; add the missing FeatureMatching ratio, RANSAC, or ScoreMax gate before importing.");
            return false;
        }

        private static bool AppendRequiredLlmIntentToolValidation(
            VisionPipeline pipeline,
            ICollection<string> validationLines,
            string requiredToolType,
            string intentName,
            string nextAction)
        {
            List<string> enabledToolTypes = (pipeline?.Steps ?? new List<VisionPipelineStep>())
                .Where(step => step != null && step.Enabled)
                .Select(step => step.ToolType ?? string.Empty)
                .Where(toolType => !string.IsNullOrWhiteSpace(toolType))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(toolType => toolType, StringComparer.OrdinalIgnoreCase)
                .ToList();

            bool hasRequiredTool = enabledToolTypes.Any(toolType =>
                string.Equals(toolType, requiredToolType, StringComparison.OrdinalIgnoreCase)
                || IsAcceptedToolAlias(requiredToolType, toolType));

            if (hasRequiredTool)
            {
                validationLines.Add("Intent contract: OK - " + intentName + " uses ToolType=" + requiredToolType + ".");
                return true;
            }

            validationLines.Add("Error: Intent contract mismatch. Selected intent '" + intentName + "' requires ToolType=" + requiredToolType + ".");
            validationLines.Add("Draft enabled ToolTypes: " + (enabledToolTypes.Count == 0 ? "-" : string.Join(", ", enabledToolTypes)));
            validationLines.Add("Next: " + nextAction);
            return false;
        }

        private static bool IsAcceptedToolAlias(string requiredToolType, string actualToolType)
        {
            if (string.Equals(requiredToolType, "LineDistance", StringComparison.OrdinalIgnoreCase))
            {
                return string.Equals(actualToolType, "LineDistanceGauge", StringComparison.OrdinalIgnoreCase);
            }

            return false;
        }

        private static bool HasExplicitAcceptance(VisionPipelineStep step)
        {
            return step != null
                && (step.UseAcceptance
                    || !string.IsNullOrWhiteSpace(step.AcceptanceMetricName)
                    || step.UseAcceptanceMetricMinimum
                    || step.UseAcceptanceMetricMaximum);
        }

        private static bool HasParameterValue(
            VisionPipelineStep step,
            string key,
            Func<string, bool> predicate)
        {
            if (step?.Parameters == null || predicate == null)
            {
                return false;
            }

            foreach (KeyValuePair<string, string> pair in step.Parameters)
            {
                if (string.Equals(pair.Key, key, StringComparison.OrdinalIgnoreCase))
                {
                    return predicate((pair.Value ?? string.Empty).Trim());
                }
            }

            return false;
        }

        private static bool IsTrue(string value)
        {
            return string.Equals(value, "true", StringComparison.OrdinalIgnoreCase);
        }

        private static bool MatchesDouble(string value, double expected)
        {
            return double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out double parsed)
                && !double.IsNaN(parsed)
                && !double.IsInfinity(parsed)
                && Math.Abs(parsed - expected) < 0.000000001D;
        }

        private static bool MatchesRoi(
            string value,
            OpenVisionRecipePinGapIntentSkill.RoiSample expected)
        {
            return OpenVisionRecipePinGapIntentSkill.TryParseRoi(
                    value,
                    out int x,
                    out int y,
                    out int width,
                    out int height,
                    out _)
                && x == expected.X
                && y == expected.Y
                && width == expected.Width
                && height == expected.Height;
        }

        private static bool TryParseInteger(
            string value,
            int minimum,
            int maximum,
            out int parsed)
        {
            return int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out parsed)
                && parsed >= minimum
                && parsed <= maximum;
        }

        private static bool TryParseCoverageRatio(string value, out double parsed)
        {
            return double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out parsed)
                && !double.IsNaN(parsed)
                && !double.IsInfinity(parsed)
                && parsed > 0
                && parsed <= 1;
        }

        private static bool HasJudgementParameter(VisionPipelineStep step)
        {
            if (step == null)
            {
                return false;
            }

            if (step.UseAcceptance
                && !string.IsNullOrWhiteSpace(step.AcceptanceMetricName)
                && (step.UseAcceptanceMetricMinimum || step.UseAcceptanceMetricMaximum))
            {
                return true;
            }

            if (string.Equals(step.ToolType, "PinArrayGap", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            if (step.Parameters == null || step.Parameters.Count == 0)
            {
                return false;
            }

            return step.Parameters.Keys.Any(key =>
            {
                string value = key ?? string.Empty;
                return value.IndexOf("SCORE", StringComparison.OrdinalIgnoreCase) >= 0
                    || value.IndexOf("THRESH", StringComparison.OrdinalIgnoreCase) >= 0
                    || value.IndexOf("MIN", StringComparison.OrdinalIgnoreCase) >= 0
                    || value.IndexOf("MAX", StringComparison.OrdinalIgnoreCase) >= 0
                    || value.IndexOf("AREA", StringComparison.OrdinalIgnoreCase) >= 0
                    || value.IndexOf("DISTANCE", StringComparison.OrdinalIgnoreCase) >= 0
                    || value.IndexOf("MEAN", StringComparison.OrdinalIgnoreCase) >= 0
                    || value.IndexOf("RATIO", StringComparison.OrdinalIgnoreCase) >= 0
                    || value.IndexOf("CONTRAST", StringComparison.OrdinalIgnoreCase) >= 0;
            });
        }

        internal static bool TryValidateXmlSyntax(string xmlText, ICollection<string> validationLines)
        {
            try
            {
                XDocument.Parse(xmlText, LoadOptions.SetLineInfo);
                validationLines.Add(OpenVisionRecipeText.Local("XML 구문: OK", "XML syntax: OK"));
                return true;
            }
            catch (XmlException ex)
            {
                validationLines.Clear();
                validationLines.Add(OpenVisionRecipeText.Local("LLM 초안 검증: NG", "LLM draft validation: NG"));
                validationLines.Add(string.Format(
                    CultureInfo.CurrentCulture,
                    OpenVisionRecipeText.Local("XML 구문: NG, 줄 {0}, 위치 {1}: {2}", "XML syntax: NG at line {0}, position {1}: {2}"),
                    ex.LineNumber,
                    ex.LinePosition,
                    ex.Message));
                validationLines.Add(OpenVisionRecipeText.Local("다음: 보고된 줄/위치의 잘못된 XML을 수정한 뒤 다시 검증하세요.", "Next: Fix malformed XML at the reported line/position, then validate again."));
                return false;
            }
        }

    }
}
