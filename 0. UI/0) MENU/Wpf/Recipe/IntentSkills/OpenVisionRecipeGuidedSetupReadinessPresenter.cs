using Lib.OpenCV;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace OpenVisionLab
{
    // Formats read-only Guided Setup required-input and readiness state from current fields.
    internal static class OpenVisionRecipeGuidedSetupReadinessPresenter
    {
        internal static string BuildReadinessText(string template)
        {
            template = template ?? string.Empty;
            string setupLocation = OpenVisionRecipeText.Local(
                " 위 값은 검사 설정 탭에서 입력합니다.",
                " Enter these values in the Build inspection tab.");

            if (OpenVisionRecipeLlmIntent.IsLineDistanceTemplate(template))
            {
                return OpenVisionRecipeText.Local(
                    "필수 입력: ROI 샘플, 최소/최대 거리, Range 판정값. mm 판정을 사용하려면 mm/px를 입력하고, 픽셀 판정만 사용하려면 비워 두세요.",
                    "Required inputs: ROI samples, min/max distance, and range gate. Enter mm/px for calibrated mm gates, or leave it blank for px-only.") + setupLocation;
            }

            if (OpenVisionRecipeLlmIntent.IsBlobTemplate(template))
            {
                return OpenVisionRecipeText.Local(
                    "필수 입력: ROI, Threshold, 예상 ResultCount, Blob 면적 범위.",
                    "Required inputs: ROI, threshold, expected ResultCount, and blob area limits.") + setupLocation;
            }

            if (OpenVisionRecipeLlmIntent.IsContourTemplate(template))
            {
                return OpenVisionRecipeText.Local(
                    "필수 입력: ROI, Threshold, 예상 ResultCount, Contour 면적 범위.",
                    "Required inputs: ROI, threshold, expected ResultCount, and contour area limits.") + setupLocation;
            }

            if (OpenVisionRecipeLlmIntent.IsFeatureMatchingTemplate(template))
            {
                return OpenVisionRecipeText.Local(
                    "필수 입력: 존재하는 Feature template 경로, Ratio 기준 0..1, RANSAC px, ScoreMax 최소 판정값. 검사 범위는 전체 이미지입니다.",
                    "Required inputs: existing Feature template path, Ratio min 0..1, RANSAC px, and a ScoreMax minimum gate. Inspection scope is the full image.") + setupLocation;
            }

            if (OpenVisionRecipeLlmIntent.IsMatchingTemplate(template))
            {
                return OpenVisionRecipeText.Local(
                    "필수 입력: 존재하는 템플릿 경로, 검색 ROI, SCORE_MIN 0..1, 예상 ResultCount.",
                    "Required inputs: existing template path, search ROI, SCORE_MIN 0..1, and expected ResultCount.") + setupLocation;
            }

            if (OpenVisionRecipeLlmIntent.IsMeanTemplate(template))
            {
                return OpenVisionRecipeText.Local(
                    "필수 입력: 선택 ROI(비우면 전체 이미지), Mean 종류, MeanValueAvg 최소/최대 GV 판정값.",
                    "Required inputs: optional ROI (blank means full image), Mean type, and MeanValueAvg min/max GV gate.") + setupLocation;
            }

            if (OpenVisionRecipeLlmIntent.IsReferenceDifferenceTemplate(template))
            {
                return OpenVisionRecipeText.Local(
                    "필수 입력: 존재하는 Good 기준 이미지 1~4개, 차이 임계값, 최소/최대 결함 면적.",
                    "Required inputs: 1-4 existing approved Good reference images, difference threshold, and min/max defect area.") + setupLocation;
            }

            if (OpenVisionRecipeLlmIntent.IsEdgeBasedTemplate(template))
            {
                return OpenVisionRecipeText.Local(
                    "필수 입력: 존재하는 Edge template 경로, 최소 점수, 검색 개수, Canny low/high, ScoreMax 최소 판정값. 검사 범위는 전체 이미지입니다.",
                    "Required inputs: existing Edge template path, min score, search count, Canny low/high, and a ScoreMax minimum gate. Inspection scope is the full image.") + setupLocation;
            }

            return OpenVisionRecipeText.Local(
                "필수 입력: 기준 템플릿 경로, Score 판정값, 매칭 개수, 허용 각도 범위.",
                "Required inputs: reference template path, score gate, match count, and allowed angle range.") + setupLocation;
        }

        internal static OpenVisionRecipeGuidedSetupReadinessStatus Evaluate(OpenVisionRecipeGuidedSetupReadinessInput input)
        {
            input = input ?? new OpenVisionRecipeGuidedSetupReadinessInput();
            string template = input.Template ?? string.Empty;
            if (OpenVisionRecipeLlmIntent.IsLineDistanceTemplate(template))
            {
                List<string> missing = new List<string>();
                bool roiReady = OpenVisionRecipePinGapIntentSkill.TryParseRoiSamples(
                    input.PinGapRoiText,
                    out IReadOnlyList<OpenVisionRecipePinGapIntentSkill.RoiSample> roiSamples,
                    out _);
                bool minReady = OpenVisionRecipePinGapIntentSkill.TryParsePositiveDouble(input.PinGapDistanceMinText, out double minimum);
                bool maxReady = OpenVisionRecipePinGapIntentSkill.TryParsePositiveDouble(input.PinGapDistanceMaxText, out double maximum);
                bool rangeReady = OpenVisionRecipePinGapIntentSkill.TryParsePositiveDouble(input.PinGapRangeMaxText, out _);
                bool scaleReady = input.PinGapPixelOnly
                    || OpenVisionRecipePinGapIntentSkill.TryParsePositiveDouble(input.PinGapScaleText, out _);
                string unit = input.PinGapPixelOnly ? "px" : "mm";

                if (!roiReady) missing.Add(OpenVisionRecipeText.Local("ROI 샘플", "ROI samples"));
                if (!minReady) missing.Add(OpenVisionRecipeText.Local("최소 ", "Min ") + unit);
                if (!maxReady) missing.Add(OpenVisionRecipeText.Local("최대 ", "Max ") + unit);
                if (!rangeReady) missing.Add("Range " + unit);
                if (!scaleReady) missing.Add(OpenVisionRecipeText.Local("유효한 mm/px 또는 PX-ONLY 사용을 위한 빈 값", "valid mm/px or blank for px-only"));
                if (minReady && maxReady && minimum > maximum) missing.Add(OpenVisionRecipeText.Local("최소값 <= 최대값", "Min <= Max"));

                if (missing.Count > 0)
                {
                    return Status(false, OpenVisionRecipeText.Local("MISSING · 입력 필요: ", "MISSING: ") + string.Join(", ", missing));
                }

                string averageMetricName = input.PinGapPixelOnly
                    ? VisionPipelineKnownMetrics.DistancePxAvg
                    : VisionPipelineKnownMetrics.DistanceMmAvg;
                string rangeMetricName = input.PinGapPixelOnly
                    ? VisionPipelineKnownMetrics.DistancePxRange
                    : VisionPipelineKnownMetrics.DistanceMmRange;
                return Status(
                    true,
                    OpenVisionRecipeText.Local("READY · 준비 완료: ", "READY: ")
                        + (input.PinGapPixelOnly ? "PX-ONLY" : "MM-READY")
                        + " / "
                        + roiSamples.Count.ToString(CultureInfo.InvariantCulture)
                        + OpenVisionRecipeText.Local("개 ROI 샘플 / ", " ROI samples / ")
                        + averageMetricName
                        + " + "
                        + rangeMetricName
                        + OpenVisionRecipeText.Local(" 판정", " gates"));
            }

            if (OpenVisionRecipeLlmIntent.IsBlobTemplate(template))
            {
                List<string> missing = new List<string>();
                bool roiReady = OpenVisionRecipeBlobCountIntentSkill.TryParseRoi(
                    input.BlobCountRoiText,
                    out _,
                    out _,
                    out _,
                    out _,
                    out _);
                bool thresholdReady = OpenVisionRecipeBlobCountIntentSkill.TryParseByte(input.BlobCountThresholdText, out _);
                bool minCountReady = OpenVisionRecipeBlobCountIntentSkill.TryParseNonNegativeInt(input.BlobCountMinCountText, out int minCount);
                bool maxCountReady = OpenVisionRecipeBlobCountIntentSkill.TryParseNonNegativeInt(input.BlobCountMaxCountText, out int maxCount);
                bool minAreaReady = OpenVisionRecipeBlobCountIntentSkill.TryParsePositiveInt(input.BlobCountMinAreaText, out int minArea);
                bool maxAreaReady = OpenVisionRecipeBlobCountIntentSkill.TryParsePositiveInt(input.BlobCountMaxAreaText, out int maxArea);

                if (!roiReady) missing.Add("ROI");
                if (!thresholdReady) missing.Add("Threshold 0..255");
                if (!minCountReady) missing.Add(OpenVisionRecipeText.Local("최소 개수", "Min count"));
                if (!maxCountReady) missing.Add(OpenVisionRecipeText.Local("최대 개수", "Max count"));
                if (!minAreaReady) missing.Add(OpenVisionRecipeText.Local("최소 면적", "Min area"));
                if (!maxAreaReady) missing.Add(OpenVisionRecipeText.Local("최대 면적", "Max area"));
                if (minCountReady && maxCountReady && minCount > maxCount) missing.Add(OpenVisionRecipeText.Local("최소 개수 <= 최대 개수", "Min count <= Max count"));
                if (minAreaReady && maxAreaReady && minArea > maxArea) missing.Add(OpenVisionRecipeText.Local("최소 면적 <= 최대 면적", "Min area <= Max area"));

                if (missing.Count > 0)
                {
                    return Status(false, OpenVisionRecipeText.Local("MISSING · 입력 필요: ", "MISSING: ") + string.Join(", ", missing));
                }

                return Status(
                    true,
                    OpenVisionRecipeText.Local(
                        "READY · 준비 완료: ROI + Threshold + ResultCount + 면적 판정",
                        "READY: ROI + Threshold + ResultCount + area gates"));
            }

            if (OpenVisionRecipeLlmIntent.IsContourTemplate(template))
            {
                List<string> missing = new List<string>();
                bool roiReady = OpenVisionRecipeContourCountIntentSkill.TryParseRoi(
                    input.ContourCountRoiText,
                    out _,
                    out _,
                    out _,
                    out _,
                    out _);
                bool thresholdReady = OpenVisionRecipeContourCountIntentSkill.TryParseByte(input.ContourCountThresholdText, out _);
                bool minCountReady = OpenVisionRecipeContourCountIntentSkill.TryParseNonNegativeInt(input.ContourCountMinCountText, out int minCount);
                bool maxCountReady = OpenVisionRecipeContourCountIntentSkill.TryParseNonNegativeInt(input.ContourCountMaxCountText, out int maxCount);
                bool minAreaReady = OpenVisionRecipeContourCountIntentSkill.TryParsePositiveInt(input.ContourCountMinAreaText, out int minArea);
                bool maxAreaReady = OpenVisionRecipeContourCountIntentSkill.TryParsePositiveInt(input.ContourCountMaxAreaText, out int maxArea);

                if (!roiReady) missing.Add("ROI");
                if (!thresholdReady) missing.Add("Threshold 0..255");
                if (!minCountReady) missing.Add(OpenVisionRecipeText.Local("최소 개수", "Min count"));
                if (!maxCountReady) missing.Add(OpenVisionRecipeText.Local("최대 개수", "Max count"));
                if (!minAreaReady) missing.Add(OpenVisionRecipeText.Local("최소 면적", "Min area"));
                if (!maxAreaReady) missing.Add(OpenVisionRecipeText.Local("최대 면적", "Max area"));
                if (minCountReady && maxCountReady && minCount > maxCount) missing.Add(OpenVisionRecipeText.Local("최소 개수 <= 최대 개수", "Min count <= Max count"));
                if (minAreaReady && maxAreaReady && minArea > maxArea) missing.Add(OpenVisionRecipeText.Local("최소 면적 <= 최대 면적", "Min area <= Max area"));

                if (missing.Count > 0)
                {
                    return Status(false, OpenVisionRecipeText.Local("MISSING · 입력 필요: ", "MISSING: ") + string.Join(", ", missing));
                }

                return Status(
                    true,
                    OpenVisionRecipeText.Local(
                        "READY · 준비 완료: ROI + Threshold + ResultCount + AreaMax 판정",
                        "READY: ROI + Threshold + ResultCount + AreaMax gates"));
            }

            if (OpenVisionRecipeLlmIntent.IsEdgeBasedTemplate(template))
            {
                List<string> missing = new List<string>();
                string templatePath = (input.ReferenceImagePath ?? string.Empty).Trim();
                bool templateReady = !string.IsNullOrWhiteSpace(templatePath) && File.Exists(templatePath);
                bool scoreReady = OpenVisionRecipeEdgeBasedMatchingIntentSkill.TryParseScore(input.EdgeBasedScoreMinText, out _);
                bool countReady = OpenVisionRecipeEdgeBasedMatchingIntentSkill.TryParsePositiveInt(input.EdgeBasedSearchCountText, out _);
                bool cannyLowReady = OpenVisionRecipeEdgeBasedMatchingIntentSkill.TryParseByte(input.EdgeBasedCannyLowText, out int cannyLow);
                bool cannyHighReady = OpenVisionRecipeEdgeBasedMatchingIntentSkill.TryParseByte(input.EdgeBasedCannyHighText, out int cannyHigh);
                bool acceptanceReady = OpenVisionRecipeEdgeBasedMatchingIntentSkill.TryParseAcceptanceScoreMinimum(input.EdgeBasedAcceptanceScoreMinText, out _);

                if (!templateReady) missing.Add(OpenVisionRecipeText.Local("존재하는 Edge template 경로", "existing Edge template path"));
                if (!scoreReady) missing.Add("Min score 0..1");
                if (!countReady) missing.Add(OpenVisionRecipeText.Local("검색 개수 > 0", "Search count > 0"));
                if (!cannyLowReady) missing.Add("Canny low 0..255");
                if (!cannyHighReady) missing.Add("Canny high 0..255");
                if (cannyLowReady && cannyHighReady && cannyLow > cannyHigh) missing.Add("Canny low <= Canny high");
                if (!acceptanceReady) missing.Add("ScoreMax min > 0..100");

                if (missing.Count > 0)
                {
                    return Status(false, OpenVisionRecipeText.Local("MISSING · 입력 필요: ", "MISSING: ") + string.Join(", ", missing));
                }

                return Status(
                    true,
                    OpenVisionRecipeText.Local(
                        "READY · 준비 완료: Edge template + 전체 이미지 + 점수 + Canny + ScoreMax 판정",
                        "READY: Edge template + full image + score + Canny + ScoreMax gate"));
            }

            if (OpenVisionRecipeLlmIntent.IsFeatureMatchingTemplate(template))
            {
                List<string> missing = new List<string>();
                string templatePath = (input.ReferenceImagePath ?? string.Empty).Trim();
                bool templateReady = !string.IsNullOrWhiteSpace(templatePath) && File.Exists(templatePath);
                bool scoreReady = OpenVisionRecipeFeatureMatchingIntentSkill.TryParseScore(input.FeatureMatchingScoreMinText, out _);
                bool ransacReady = OpenVisionRecipeFeatureMatchingIntentSkill.TryParsePositiveDouble(input.FeatureMatchingRansacReprojThresholdText, out _);
                bool acceptanceReady = OpenVisionRecipeFeatureMatchingIntentSkill.TryParseAcceptanceScoreMinimum(input.FeatureMatchingAcceptanceScoreMinText, out _);

                if (!templateReady) missing.Add(OpenVisionRecipeText.Local("존재하는 Feature template 경로", "existing Feature template path"));
                if (!scoreReady) missing.Add("Ratio min 0..1");
                if (!ransacReady) missing.Add("RANSAC px > 0");
                if (!acceptanceReady) missing.Add("ScoreMax min > 0..100");

                if (missing.Count > 0)
                {
                    return Status(false, OpenVisionRecipeText.Local("MISSING · 입력 필요: ", "MISSING: ") + string.Join(", ", missing));
                }

                return Status(
                    true,
                    OpenVisionRecipeText.Local(
                        "READY · 준비 완료: Feature template + 전체 이미지 + Ratio + RANSAC + ScoreMax 판정",
                        "READY: Feature template + full image + Ratio + RANSAC + ScoreMax gate"));
            }

            if (OpenVisionRecipeLlmIntent.IsMatchingTemplate(template))
            {
                List<string> missing = new List<string>();
                string templatePath = (input.ReferenceImagePath ?? string.Empty).Trim();
                bool templateReady = !string.IsNullOrWhiteSpace(templatePath) && File.Exists(templatePath);
                bool roiReady = OpenVisionRecipeMatchingIntentSkill.TryParseRoi(
                    input.MatchingSearchRoiText,
                    out _,
                    out _,
                    out _,
                    out _,
                    out _);
                bool scoreReady = OpenVisionRecipeMatchingIntentSkill.TryParseScore(input.MatchingScoreMinText, out _);
                bool countReady = OpenVisionRecipeMatchingIntentSkill.TryParsePositiveInt(input.MatchingExpectedCountText, out _);

                if (!templateReady) missing.Add(OpenVisionRecipeText.Local("존재하는 템플릿 경로", "existing template path"));
                if (!roiReady) missing.Add("Search ROI x,y,w,h");
                if (!scoreReady) missing.Add("SCORE_MIN 0..1");
                if (!countReady) missing.Add(OpenVisionRecipeText.Local("예상 개수 > 0", "Expected count > 0"));

                if (missing.Count > 0)
                {
                    return Status(false, OpenVisionRecipeText.Local("MISSING · 입력 필요: ", "MISSING: ") + string.Join(", ", missing));
                }

                return Status(
                    true,
                    OpenVisionRecipeText.Local(
                        "READY · 준비 완료: 템플릿 + Search ROI + SCORE_MIN + ResultCount 판정",
                        "READY: template + Search ROI + SCORE_MIN + ResultCount gate"));
            }

            if (OpenVisionRecipeLlmIntent.IsMeanTemplate(template))
            {
                List<string> missing = new List<string>();
                bool roiReady = OpenVisionRecipeMeanIntentSkill.TryParseOptionalRoi(
                    input.MeanRoiText,
                    out bool useRoi,
                    out _,
                    out _,
                    out _,
                    out _,
                    out _);
                bool typeReady = OpenVisionRecipeMeanIntentSkill.TryParseMeanType(input.MeanTypeText, out MeanType meanType);
                bool minimumReady = OpenVisionRecipeMeanIntentSkill.TryParseByte(input.MeanMinimumText, out int minimum);
                bool maximumReady = OpenVisionRecipeMeanIntentSkill.TryParseByte(input.MeanMaximumText, out int maximum);

                if (!roiReady) missing.Add(OpenVisionRecipeText.Local("ROI x,y,w,h 또는 빈 값", "ROI x,y,w,h or blank"));
                if (!typeReady) missing.Add(OpenVisionRecipeText.Local("Mean 종류", "Mean type"));
                if (!minimumReady) missing.Add(OpenVisionRecipeText.Local("최소 GV 0..255", "Min GV 0..255"));
                if (!maximumReady) missing.Add(OpenVisionRecipeText.Local("최대 GV 0..255", "Max GV 0..255"));
                if (minimumReady && maximumReady && minimum > maximum) missing.Add(OpenVisionRecipeText.Local("최소 GV <= 최대 GV", "Min GV <= Max GV"));

                if (missing.Count > 0)
                {
                    return Status(false, OpenVisionRecipeText.Local("MISSING · 입력 필요: ", "MISSING: ") + string.Join(", ", missing));
                }

                return Status(
                    true,
                    OpenVisionRecipeText.Local("READY · 준비 완료: ", "READY: ")
                        + (useRoi ? "ROI" : OpenVisionRecipeText.Local("전체 이미지", "full image"))
                        + " + "
                        + meanType
                        + " + MeanValueAvg "
                        + minimum.ToString(CultureInfo.InvariantCulture)
                        + ".."
                        + maximum.ToString(CultureInfo.InvariantCulture)
                        + OpenVisionRecipeText.Local(" 판정", " gate"));
            }

            if (OpenVisionRecipeLlmIntent.IsReferenceDifferenceTemplate(template))
            {
                List<string> missing = new List<string>();
                bool referencesReady = OpenVisionRecipeReferenceDifferenceIntentSkill.TryCollectReferencePaths(
                    input.ReferenceImagePath,
                    input.ReferenceDifferencePath2,
                    input.ReferenceDifferencePath3,
                    input.ReferenceDifferencePath4,
                    out IReadOnlyList<string> referencePaths);
                bool thresholdReady = OpenVisionRecipeReferenceDifferenceIntentSkill.TryParseThreshold(
                    input.ReferenceDifferenceThresholdText,
                    out _);
                bool minimumAreaReady = OpenVisionRecipeReferenceDifferenceIntentSkill.TryParsePositiveArea(
                    input.ReferenceDifferenceMinimumAreaText,
                    out int minimumArea);
                bool maximumAreaReady = OpenVisionRecipeReferenceDifferenceIntentSkill.TryParsePositiveArea(
                    input.ReferenceDifferenceMaximumAreaText,
                    out int maximumArea);

                if (!referencesReady) missing.Add(OpenVisionRecipeText.Local("존재하는 Good 기준 이미지 1~4개", "1-4 existing Good references"));
                if (!thresholdReady) missing.Add(OpenVisionRecipeText.Local("차이 임계값 0..255", "Difference threshold 0..255"));
                if (!minimumAreaReady) missing.Add(OpenVisionRecipeText.Local("최소 결함 면적 > 0", "Min defect area > 0"));
                if (!maximumAreaReady) missing.Add(OpenVisionRecipeText.Local("최대 결함 면적 > 0", "Max defect area > 0"));
                if (minimumAreaReady && maximumAreaReady && minimumArea > maximumArea) missing.Add(OpenVisionRecipeText.Local("최소 면적 <= 최대 면적", "Min area <= Max area"));

                if (missing.Count > 0)
                {
                    return Status(false, OpenVisionRecipeText.Local("MISSING · 입력 필요: ", "MISSING: ") + string.Join(", ", missing));
                }

                return Status(
                    true,
                    OpenVisionRecipeText.Local("READY · Good 기준 ", "READY: ")
                        + referencePaths.Count.ToString(CultureInfo.InvariantCulture)
                        + OpenVisionRecipeText.Local("개 + 정합 + 차이 영역 + ResultCount=0 판정", " Good references + registration + difference regions + ResultCount=0 gate"));
            }

            if (OpenVisionRecipeLlmIntent.IsEdgeBasedTemplate(template) || !OpenVisionRecipeLlmIntent.IsMeanTemplate(template))
            {
                if (string.IsNullOrWhiteSpace(input.ReferenceImagePath) || !File.Exists(input.ReferenceImagePath))
                {
                    return Status(false, OpenVisionRecipeText.Local("MISSING · 입력 필요: 기준 템플릿 이미지", "MISSING: reference template image"));
                }

                return Status(true, OpenVisionRecipeText.Local("READY · 준비 완료: 기준 템플릿 이미지", "READY: reference template image"));
            }

            return Status(
                true,
                OpenVisionRecipeText.Local(
                    "READY · 준비 완료: 기본값 사용, 가져오기 전에 파라미터를 검토하세요.",
                    "READY: built-in defaults; review parameters before Import"));
        }

        private static OpenVisionRecipeGuidedSetupReadinessStatus Status(bool isReady, string text)
        {
            return new OpenVisionRecipeGuidedSetupReadinessStatus(isReady, text);
        }
    }

    internal sealed class OpenVisionRecipeGuidedSetupReadinessInput
    {
        internal string Template { get; set; }

        internal string ReferenceImagePath { get; set; }

        internal string PinGapRoiText { get; set; }

        internal bool PinGapPixelOnly { get; set; }

        internal string PinGapDistanceMinText { get; set; }

        internal string PinGapDistanceMaxText { get; set; }

        internal string PinGapRangeMaxText { get; set; }

        internal string PinGapScaleText { get; set; }

        internal string BlobCountRoiText { get; set; }

        internal string BlobCountThresholdText { get; set; }

        internal string BlobCountMinCountText { get; set; }

        internal string BlobCountMaxCountText { get; set; }

        internal string BlobCountMinAreaText { get; set; }

        internal string BlobCountMaxAreaText { get; set; }

        internal string ContourCountRoiText { get; set; }

        internal string ContourCountThresholdText { get; set; }

        internal string ContourCountMinCountText { get; set; }

        internal string ContourCountMaxCountText { get; set; }

        internal string ContourCountMinAreaText { get; set; }

        internal string ContourCountMaxAreaText { get; set; }

        internal string MatchingSearchRoiText { get; set; }

        internal string MatchingScoreMinText { get; set; }

        internal string MatchingExpectedCountText { get; set; }

        internal string FeatureMatchingScoreMinText { get; set; }

        internal string FeatureMatchingRansacReprojThresholdText { get; set; }

        internal string FeatureMatchingAcceptanceScoreMinText { get; set; }

        internal string EdgeBasedScoreMinText { get; set; }

        internal string EdgeBasedSearchCountText { get; set; }

        internal string EdgeBasedCannyLowText { get; set; }

        internal string EdgeBasedCannyHighText { get; set; }

        internal string EdgeBasedAcceptanceScoreMinText { get; set; }

        internal string MeanRoiText { get; set; }

        internal string MeanTypeText { get; set; }

        internal string MeanMinimumText { get; set; }

        internal string MeanMaximumText { get; set; }

        internal string ReferenceDifferencePath2 { get; set; }

        internal string ReferenceDifferencePath3 { get; set; }

        internal string ReferenceDifferencePath4 { get; set; }

        internal string ReferenceDifferenceThresholdText { get; set; }

        internal string ReferenceDifferenceMinimumAreaText { get; set; }

        internal string ReferenceDifferenceMaximumAreaText { get; set; }
    }

    internal sealed class OpenVisionRecipeGuidedSetupReadinessStatus
    {
        internal OpenVisionRecipeGuidedSetupReadinessStatus(bool isReady, string text)
        {
            IsReady = isReady;
            Text = text ?? string.Empty;
        }

        internal bool IsReady { get; }

        internal string Text { get; }
    }
}
