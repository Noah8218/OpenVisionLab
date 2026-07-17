using System;
using System.Collections.Generic;
using System.Globalization;

namespace OpenVisionLab
{
    // Formats read-only Guided Setup intent feedback from current fields and saved sample summaries.
    internal static class OpenVisionRecipeIntentFeedbackPresenter
    {
        internal static string BuildPinGapLatestRunText(
            OpenVisionRecipeSampleRunSummary sample,
            bool pixelOnly,
            string minimumText,
            string maximumText,
            string rangeMaximumText)
        {
            sample = sample ?? OpenVisionRecipeSampleRunSummary.Empty;
            string averageMetric = pixelOnly
                ? VisionPipelineKnownMetrics.DistancePxAvg
                : VisionPipelineKnownMetrics.DistanceMmAvg;
            string rangeMetric = pixelOnly
                ? VisionPipelineKnownMetrics.DistancePxRange
                : VisionPipelineKnownMetrics.DistanceMmRange;
            string expectedMetrics = averageMetric + "/" + rangeMetric;
            if (!sample.HasResult)
            {
                return OpenVisionRecipeText.Local("최근 샘플: 아직 실행 결과가 없습니다. ", "Latest sample: no run result yet. ")
                    + OpenVisionRecipeText.Local("Pin gap XML을 가져온 뒤 샘플 검사를 실행하면 ", "Import Pin gap XML and run the sample check to show ")
                    + expectedMetrics
                    + OpenVisionRecipeText.Local("가 여기에 표시됩니다.", " here.");
            }

            string metrics = sample.DistanceMetricText;
            if (string.IsNullOrWhiteSpace(metrics))
            {
                return OpenVisionRecipeText.Local("최근 샘플: 아직 ", "Latest sample: no ")
                    + expectedMetrics
                    + OpenVisionRecipeText.Local("가 없습니다. Pin gap XML을 가져온 뒤 샘플 검사를 실행하세요.", " yet. Import Pin gap XML and run the sample check.");
            }

            return OpenVisionRecipeText.Local("최근 샘플: ", "Latest sample: ")
                + metrics
                + " / "
                + ResolvePinGapMetricAdvice(
                    metrics,
                    pixelOnly,
                    averageMetric,
                    rangeMetric,
                    minimumText,
                    maximumText,
                    rangeMaximumText);
        }

        internal static string BuildPinGapCalibrationReviewText(
            bool pixelOnly,
            string minimumText,
            string maximumText,
            string rangeMaximumText,
            string scaleText)
        {
            if (!OpenVisionRecipePinGapIntentSkill.TryParsePositiveDouble(minimumText, out double minimum)
                || !OpenVisionRecipePinGapIntentSkill.TryParsePositiveDouble(maximumText, out double maximum)
                || !OpenVisionRecipePinGapIntentSkill.TryParsePositiveDouble(rangeMaximumText, out double rangeMaximum))
            {
                return OpenVisionRecipeText.Local(
                    "MISSING: Min/Max 거리와 Range 최대값은 양수여야 합니다.",
                    "MISSING: Min/Max distance and Range maximum must be positive.");
            }

            if (minimum > maximum)
            {
                return OpenVisionRecipeText.Local(
                    "MISSING: Min 값은 Max 값보다 클 수 없습니다.",
                    "MISSING: Min cannot be greater than Max.");
            }

            if (pixelOnly)
            {
                string pixelSummary = "PX-ONLY: "
                    + VisionPipelineKnownMetrics.DistancePxAvg
                    + " "
                    + minimum.ToString("0.###", CultureInfo.InvariantCulture)
                    + ".."
                    + maximum.ToString("0.###", CultureInfo.InvariantCulture)
                    + " px, "
                    + VisionPipelineKnownMetrics.DistancePxRange
                    + " <= "
                    + rangeMaximum.ToString("0.###", CultureInfo.InvariantCulture)
                    + " px. ";
                return pixelSummary + OpenVisionRecipeText.Local(
                    "실제 길이 단위는 표시하지 않습니다. mm 판정을 사용하려면 양수 mm/px 값을 입력하세요.",
                    "No physical-unit claim is made. Enter a positive mm/px value to enable calibrated mm gates.");
            }

            if (!OpenVisionRecipePinGapIntentSkill.TryParsePositiveDouble(scaleText, out double mmPerPixel))
            {
                return OpenVisionRecipeText.Local(
                    "MISSING: mm/px는 양수이거나 px-only 사용을 위해 비워 두어야 합니다.",
                    "MISSING: mm/px must be positive, or blank for px-only.");
            }

            double minimumPx = minimum / mmPerPixel;
            double maximumPx = maximum / mmPerPixel;
            double rangeMaximumPx = rangeMaximum / mmPerPixel;

            string calibratedSummary = "MM-READY: PIXELPERMM "
                + mmPerPixel.ToString("0.######", CultureInfo.InvariantCulture)
                + " mm/px. DistanceMmAvg "
                + minimum.ToString("0.###", CultureInfo.InvariantCulture)
                + ".."
                + maximum.ToString("0.###", CultureInfo.InvariantCulture)
                + " mm = "
                + minimumPx.ToString("0.#", CultureInfo.InvariantCulture)
                + ".."
                + maximumPx.ToString("0.#", CultureInfo.InvariantCulture)
                + " px. DistanceMmRange <= "
                + rangeMaximum.ToString("0.###", CultureInfo.InvariantCulture)
                + " mm = "
                + rangeMaximumPx.ToString("0.#", CultureInfo.InvariantCulture)
                + " px. ";
            return calibratedSummary + OpenVisionRecipeText.Local(
                "평균값만으로 판정하지 말고 Range 또는 이상 거리 제한을 함께 사용하세요.",
                "Keep a range/outlier gate; average-only measurement is not enough.");
        }

        internal static string BuildBlobCountLatestRunText(
            OpenVisionRecipeSampleRunSummary sample,
            string minimumText,
            string maximumText)
        {
            sample = sample ?? OpenVisionRecipeSampleRunSummary.Empty;
            if (!sample.HasResult)
            {
                return OpenVisionRecipeText.Local(
                    "Latest sample: no run result yet. Import Blob count XML and run the sample check to show ResultCount here.",
                    "Latest sample: no run result yet. Import Blob count XML and run the sample check to show ResultCount here.");
            }

            string metrics = sample.DisplayText;
            if (!OpenVisionRecipeBlobCountIntentSkill.TryExtractMetricValue(metrics, VisionPipelineKnownMetrics.ResultCount, out double count))
            {
                return OpenVisionRecipeText.Local(
                    "Latest sample: no ResultCount yet. Import Blob count XML and run the sample check.",
                    "Latest sample: no ResultCount yet. Import Blob count XML and run the sample check.");
            }

            return OpenVisionRecipeText.Local("Latest sample: ResultCount=", "Latest sample: ResultCount=")
                + count.ToString("0.###", CultureInfo.InvariantCulture)
                + " / "
                + ResolveBlobCountMetricAdvice(count, minimumText, maximumText);
        }

        internal static string BuildContourCountLatestRunText(
            OpenVisionRecipeSampleRunSummary sample,
            string minimumText,
            string maximumText,
            string maximumAreaText)
        {
            sample = sample ?? OpenVisionRecipeSampleRunSummary.Empty;
            if (!sample.HasResult)
            {
                return OpenVisionRecipeText.Local(
                    "Latest sample: no run result yet. Import Contour XML and run the sample check to show ResultCount/AreaMax here.",
                    "Latest sample: no run result yet. Import Contour XML and run the sample check to show ResultCount/AreaMax here.");
            }

            string metrics = sample.DisplayText;
            bool hasCount = OpenVisionRecipeContourCountIntentSkill.TryExtractMetricValue(metrics, VisionPipelineKnownMetrics.ResultCount, out double count);
            bool hasAreaMax = OpenVisionRecipeContourCountIntentSkill.TryExtractMetricValue(metrics, VisionPipelineKnownMetrics.AreaMax, out double areaMax);
            if (!hasCount && !hasAreaMax)
            {
                return OpenVisionRecipeText.Local(
                    "Latest sample: no ResultCount/AreaMax yet. Import Contour XML and run the sample check.",
                    "Latest sample: no ResultCount/AreaMax yet. Import Contour XML and run the sample check.");
            }

            List<string> parts = new List<string>();
            if (hasCount)
            {
                parts.Add(VisionPipelineKnownMetrics.ResultCount + "=" + count.ToString("0.###", CultureInfo.InvariantCulture));
            }

            if (hasAreaMax)
            {
                parts.Add(VisionPipelineKnownMetrics.AreaMax + "=" + areaMax.ToString("0.###", CultureInfo.InvariantCulture));
            }

            return OpenVisionRecipeText.Local("Latest sample: ", "Latest sample: ")
                + string.Join(", ", parts)
                + " / "
                + ResolveContourCountMetricAdvice(count, hasCount, areaMax, hasAreaMax, minimumText, maximumText, maximumAreaText);
        }

        private static string ResolvePinGapMetricAdvice(
            string metrics,
            bool pixelOnly,
            string averageMetric,
            string rangeMetric,
            string minimumText,
            string maximumText,
            string rangeMaximumText)
        {
            bool hasAverage = OpenVisionRecipePinGapIntentSkill.TryExtractMetricValue(metrics, averageMetric, out double average);
            bool hasRange = OpenVisionRecipePinGapIntentSkill.TryExtractMetricValue(metrics, rangeMetric, out double range);

            if (hasRange
                && OpenVisionRecipePinGapIntentSkill.TryParsePositiveDouble(rangeMaximumText, out double rangeMax)
                && range > rangeMax)
            {
                return OpenVisionRecipeText.Local(
                    "판정: Range NG -> ROI를 핀 간격만 남기고 줄인 뒤 edge contrast/sampling을 먼저 조정",
                    "Decision: Range NG -> narrow ROI to the pin gap first, then tune edge contrast/sampling");
            }

            bool hasMinimum = OpenVisionRecipePinGapIntentSkill.TryParsePositiveDouble(minimumText, out double minimum);
            bool hasMaximum = OpenVisionRecipePinGapIntentSkill.TryParsePositiveDouble(maximumText, out double maximum);
            if (hasAverage
                && ((hasMinimum && average < minimum) || (hasMaximum && average > maximum)))
            {
                return OpenVisionRecipeText.Local(
                    "판정: Avg NG -> " + (pixelOnly ? "Min/Max px 기준" : "mm/px 또는 Min/Max mm 기준") + "을 조정",
                    "Decision: Avg NG -> tune " + (pixelOnly ? "Min/Max px gates" : "mm/px or Min/Max mm gates"));
            }

            if (hasAverage && !hasRange)
            {
                return OpenVisionRecipeText.Local(
                    "판정: Avg만 있음 -> Range gate가 있는 Pin gap XML로 샘플을 다시 실행",
                    "Decision: Avg only -> rerun with Pin gap XML that includes the Range gate");
            }

            if (hasAverage || hasRange)
            {
                return OpenVisionRecipeText.Local(
                    "판정: 현재 입력 기준에서는 Distance gate가 OK",
                    "Decision: Distance gates are OK against the current fields");
            }

            return OpenVisionRecipeText.Local(
                "판정: Distance metric 없음 -> LineDistance/Pin gap XML로 샘플을 다시 실행",
                "Decision: no distance metric -> rerun with LineDistance/Pin gap XML");
        }

        private static string ResolveBlobCountMetricAdvice(double count, string minimumText, string maximumText)
        {
            bool hasMinimum = OpenVisionRecipeBlobCountIntentSkill.TryParseNonNegativeInt(minimumText, out int minimum);
            bool hasMaximum = OpenVisionRecipeBlobCountIntentSkill.TryParseNonNegativeInt(maximumText, out int maximum);
            if (hasMinimum && hasMaximum && minimum <= maximum && (count < minimum || count > maximum))
            {
                return OpenVisionRecipeText.Local(
                    "Decision: Count NG -> tune threshold, ROI, or area limits",
                    "Decision: Count NG -> tune threshold, ROI, or area limits");
            }

            if (hasMinimum && hasMaximum && minimum > maximum)
            {
                return OpenVisionRecipeText.Local(
                    "Decision: count field range is invalid",
                    "Decision: count field range is invalid");
            }

            return OpenVisionRecipeText.Local(
                "Decision: ResultCount gate is OK against the current fields",
                "Decision: ResultCount gate is OK against the current fields");
        }

        private static string ResolveContourCountMetricAdvice(
            double count,
            bool hasCount,
            double areaMax,
            bool hasAreaMax,
            string minimumText,
            string maximumText,
            string maximumAreaText)
        {
            bool hasMinimum = OpenVisionRecipeContourCountIntentSkill.TryParseNonNegativeInt(minimumText, out int minimum);
            bool hasMaximum = OpenVisionRecipeContourCountIntentSkill.TryParseNonNegativeInt(maximumText, out int maximum);
            if (hasCount && hasMinimum && hasMaximum && minimum <= maximum && (count < minimum || count > maximum))
            {
                return OpenVisionRecipeText.Local(
                    "Decision: Count NG -> tune threshold, ROI, or area limits",
                    "Decision: Count NG -> tune threshold, ROI, or area limits");
            }

            if (hasAreaMax
                && OpenVisionRecipeContourCountIntentSkill.TryParsePositiveInt(maximumAreaText, out int maxArea)
                && areaMax > maxArea)
            {
                return OpenVisionRecipeText.Local(
                    "Decision: AreaMax NG -> reduce oversized contour before accepting",
                    "Decision: AreaMax NG -> reduce oversized contour before accepting");
            }

            if (hasMinimum && hasMaximum && minimum > maximum)
            {
                return OpenVisionRecipeText.Local(
                    "Decision: count field range is invalid",
                    "Decision: count field range is invalid");
            }

            return OpenVisionRecipeText.Local(
                "Decision: Contour gates are OK against the current fields",
                "Decision: Contour gates are OK against the current fields");
        }
    }
}
