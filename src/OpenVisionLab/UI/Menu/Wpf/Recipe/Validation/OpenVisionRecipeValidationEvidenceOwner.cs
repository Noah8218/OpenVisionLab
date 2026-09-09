using OpenVisionLab.Vision2D.Pipeline;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace OpenVisionLab
{
    // Owns the persisted Pipeline read and validation evidence policy used by the Shell.
    // It returns display-ready text without depending on WPF state or controls.
    internal sealed class OpenVisionRecipeValidationEvidenceOwner
    {
        internal OpenVisionRecipeValidationEvidence Build(
            string recipeName,
            string pipelineName,
            bool hasSelectedPipeline)
        {
            if (!hasSelectedPipeline)
            {
                return OpenVisionRecipeValidationEvidence.Failure(
                    OpenVisionRecipeText.Local(
                        "판정 기준을 보려면 파이프라인을 선택하십시오.",
                        "Select a pipeline to review the acceptance gate."));
            }

            string path = RecipeWorkspaceService.GetVisionPipelinePath(recipeName, pipelineName);
            if (!VisionPipelineStorage.TryLoadFromFile(
                    path,
                    out VisionPipeline pipeline,
                    out string loadError)
                || pipeline == null)
            {
                return OpenVisionRecipeValidationEvidence.Failure(
                    OpenVisionRecipeText.Local(
                        "파이프라인 XML을 읽지 못했습니다: ",
                        "Pipeline XML could not be read: ")
                    + loadError);
            }

            List<VisionPipelineStep> acceptanceSteps = GetEnabledAcceptanceSteps(pipeline);
            string acceptanceText = BuildAcceptanceText(acceptanceSteps);
            string calibrationText = BuildCalibrationText(acceptanceSteps);
            return OpenVisionRecipeValidationEvidence.Success(acceptanceText, calibrationText);
        }

        private static string BuildAcceptanceText(IReadOnlyList<VisionPipelineStep> acceptanceSteps)
        {
            if (acceptanceSteps == null || acceptanceSteps.Count == 0)
            {
                return OpenVisionRecipeText.Local(
                    "Metric 기준 없음: 파이프라인 OK/NG 결과를 기대 OK/NG와 비교합니다.",
                    "No metric gate: compare pipeline OK/NG against the expected OK/NG roles.");
            }

            List<string> gates = acceptanceSteps
                .Select(FormatValidationSetAcceptanceGate)
                .Where(text => !string.IsNullOrWhiteSpace(text))
                .ToList();
            string visible = string.Join(" | ", gates.Take(2));
            int remaining = Math.Max(0, gates.Count - 2);
            return OpenVisionRecipeText.Local("활성 기준: ", "Active gate: ")
                + visible
                + (remaining > 0
                    ? " +" + remaining.ToString(CultureInfo.InvariantCulture)
                    : string.Empty);
        }

        private static string BuildCalibrationText(IReadOnlyList<VisionPipelineStep> acceptanceSteps)
        {
            List<VisionPipelineStep> millimeterSteps = (acceptanceSteps ?? Array.Empty<VisionPipelineStep>())
                .Where(step => (step.AcceptanceMetricName ?? string.Empty)
                    .IndexOf("Mm", StringComparison.OrdinalIgnoreCase) >= 0)
                .ToList();
            if (millimeterSteps.Count == 0)
            {
                return OpenVisionRecipeText.Local(
                    "해당 없음: 현재 판정 기준은 mm 물리 단위를 사용하지 않습니다.",
                    "Not required: the active acceptance gates do not use physical mm units.");
            }

            List<double> scales = new List<double>();
            bool missingScale = false;
            foreach (VisionPipelineStep step in millimeterSteps)
            {
                string value = step.Parameters?
                    .Where(parameter => string.Equals(parameter.Key, "PIXELPERMM", StringComparison.OrdinalIgnoreCase))
                    .Select(parameter => parameter.Value)
                    .FirstOrDefault();
                if (!TryParsePositiveDouble(value, out double scale))
                {
                    missingScale = true;
                    continue;
                }

                scales.Add(scale);
            }

            if (missingScale)
            {
                return OpenVisionRecipeText.Local(
                    "필수: mm 판정 기준에 PIXELPERMM이 없거나 0입니다. 물리 단위 판정을 실행하지 마십시오.",
                    "Required: an mm gate has no positive PIXELPERMM. Do not use it for a physical-unit decision.");
            }

            string scaleText = string.Join(
                ", ",
                scales
                    .Distinct()
                    .OrderBy(value => value)
                    .Select(value => value.ToString("0.######", CultureInfo.InvariantCulture)));
            return OpenVisionRecipeText.Local("적용됨: PIXELPERMM ", "Applied: PIXELPERMM ")
                + scaleText
                + OpenVisionRecipeText.Local(
                    " mm/px. 현재 렌즈와 이미지의 보정값인지 확인하십시오.",
                    " mm/px. Confirm this scale matches the current lens and image.");
        }

        private static List<VisionPipelineStep> GetEnabledAcceptanceSteps(VisionPipeline pipeline)
        {
            return pipeline?.Steps?
                .Where(step => step != null && step.Enabled && step.UseAcceptance)
                .ToList()
                ?? new List<VisionPipelineStep>();
        }

        private static string FormatValidationSetAcceptanceGate(VisionPipelineStep step)
        {
            string metric = step?.AcceptanceMetricName?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(metric))
            {
                return step?.ExpectedSuccess == false
                    ? OpenVisionRecipeText.Local("Step 상태 = NG", "Step status = NG")
                    : OpenVisionRecipeText.Local("Step 상태 = OK", "Step status = OK");
            }

            if (step.UseAcceptanceMetricMinimum && step.UseAcceptanceMetricMaximum)
            {
                return metric + " "
                    + step.AcceptanceMetricMinimum.ToString("0.######", CultureInfo.InvariantCulture)
                    + ".."
                    + step.AcceptanceMetricMaximum.ToString("0.######", CultureInfo.InvariantCulture);
            }

            if (step.UseAcceptanceMetricMinimum)
            {
                return metric + " >= "
                    + step.AcceptanceMetricMinimum.ToString("0.######", CultureInfo.InvariantCulture);
            }

            if (step.UseAcceptanceMetricMaximum)
            {
                return metric + " <= "
                    + step.AcceptanceMetricMaximum.ToString("0.######", CultureInfo.InvariantCulture);
            }

            return metric;
        }

        private static bool TryParsePositiveDouble(string value, out double result)
        {
            return (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out result)
                    || double.TryParse(value, NumberStyles.Float, CultureInfo.CurrentCulture, out result))
                && result > 0D;
        }
    }

    internal sealed class OpenVisionRecipeValidationEvidence
    {
        private OpenVisionRecipeValidationEvidence(
            bool succeeded,
            string errorText,
            string acceptanceText,
            string calibrationText)
        {
            Succeeded = succeeded;
            ErrorText = errorText ?? string.Empty;
            AcceptanceText = acceptanceText ?? string.Empty;
            CalibrationText = calibrationText ?? string.Empty;
        }

        internal bool Succeeded { get; }

        internal string ErrorText { get; }

        internal string AcceptanceText { get; }

        internal string CalibrationText { get; }

        internal static OpenVisionRecipeValidationEvidence Failure(string errorText)
        {
            return new OpenVisionRecipeValidationEvidence(false, errorText, errorText, errorText);
        }

        internal static OpenVisionRecipeValidationEvidence Success(
            string acceptanceText,
            string calibrationText)
        {
            return new OpenVisionRecipeValidationEvidence(
                true,
                string.Empty,
                acceptanceText,
                calibrationText);
        }
    }
}
