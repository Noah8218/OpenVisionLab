using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace OpenVisionLab
{
    // Formats read-only local-validation-set and Validation Suite status from already selected DTO state.
    internal static class OpenVisionRecipeValidationSetPresenter
    {
        internal static string BuildExpectedText(
            bool storageReady,
            OpenVisionRecipeValidationSetOption option)
        {
            if (!storageReady)
            {
                return OpenVisionRecipeText.Local(
                    "검증 세트 파일을 읽지 못했습니다.",
                    "The validation set file could not be read.");
            }

            if (option == null)
            {
                return OpenVisionRecipeText.Local(
                    "저장된 로컬 검증 세트가 없습니다.",
                    "No local validation set is selected.");
            }

            string summary = "OK " + option.OkCount.ToString(CultureInfo.InvariantCulture)
                + " / NG " + option.NgCount.ToString(CultureInfo.InvariantCulture)
                + " | " + OpenVisionRecipeText.Local("준비 ", "Ready ")
                + option.ReadyCount.ToString(CultureInfo.InvariantCulture)
                + "/" + option.ImageCount.ToString(CultureInfo.InvariantCulture);
            if (option.IsIdentityLocked)
            {
                return OpenVisionRecipeText.Local("위치검출 기대 성공 ", "Locator expected success ")
                    + option.OkCount.ToString(CultureInfo.InvariantCulture)
                    + "/"
                    + option.ImageCount.ToString(CultureInfo.InvariantCulture)
                    + " | "
                    + OpenVisionRecipeText.Local("파이프라인 ", "Pipeline ")
                    + option.PipelineName
                    + " | SHA "
                    + ShortHash(option.PipelineDefinitionSha256);
            }

            return option.MissingCount > 0
                ? summary + " | " + OpenVisionRecipeText.Local("누락 ", "Missing ") + option.MissingCount.ToString(CultureInfo.InvariantCulture)
                : summary;
        }

        internal static string BuildNextActionText(
            bool isValidationSuiteRunning,
            bool storageReady,
            OpenVisionRecipeValidationSetOption option,
            bool hasSelectedPipeline)
        {
            if (isValidationSuiteRunning)
            {
                return OpenVisionRecipeText.Local(
                    "현재 세트를 실행 중입니다. 저장 상태가 표시될 때까지 기다리세요.",
                    "The local set is running. Wait for the saved status.");
            }

            if (!storageReady)
            {
                return OpenVisionRecipeText.Local(
                    "validation-sets.xml 오류를 원본을 보존한 채 수정하세요.",
                    "Fix the validation-sets.xml error without overwriting the original file.");
            }

            if (option == null)
            {
                return OpenVisionRecipeText.Local(
                    "세트 이름을 입력하고 만들기를 누른 뒤 예상 OK/NG 이미지를 추가하세요.",
                    "Enter a set name, click Create, then add expected OK and NG images.");
            }

            if (option.IsIdentityLocked)
            {
                if (option.MissingCount > 0)
                {
                    return OpenVisionRecipeText.Local(
                        "해시 잠금 세트의 이미지가 누락되었습니다. 원본 N장 증거를 복원하거나 세트를 다시 승격하세요.",
                        "A hash-locked image is missing. Restore the original N-image evidence or promote the set again.");
                }

                if (!hasSelectedPipeline)
                {
                    return OpenVisionRecipeText.Local(
                        "연결된 위치검출 파이프라인을 선택하세요: ",
                        "Select the linked locator pipeline: ")
                        + option.PipelineName;
                }

                return OpenVisionRecipeText.Local(
                    "연결된 파이프라인을 선택한 뒤 명시적으로 Suite 실행을 누르세요. 실행 직전에 파이프라인·템플릿·이미지 해시를 다시 확인합니다: ",
                    "Select the linked pipeline, then explicitly click Run suite. Pipeline, template, and image hashes are checked immediately before execution: ")
                    + option.PipelineName;
            }

            if (option.MissingCount > 0)
            {
                return OpenVisionRecipeText.Local(
                    "누락 행을 선택하고 누락 경로 복구를 누른 뒤 Suite 실행을 누르세요.",
                    "Select a missing row, click Repair missing, then click Run suite.");
            }

            if (option.ImageCount == 0)
            {
                return OpenVisionRecipeText.Local(
                    "예상 OK와 예상 NG 이미지를 각각 하나 이상 추가한 뒤 Suite 실행을 누르세요.",
                    "Add at least one expected OK and one expected NG image, then click Run suite.");
            }

            if (option.OkCount == 0 || option.NgCount == 0)
            {
                return OpenVisionRecipeText.Local(
                    "비어 있는 예상 역할을 추가해 OK와 NG를 모두 준비한 뒤 Suite 실행을 누르세요.",
                    "Add the missing expected role so both OK and NG are represented, then click Run suite.");
            }

            if (!hasSelectedPipeline)
            {
                return OpenVisionRecipeText.Local(
                    "평가할 파이프라인을 선택한 뒤 Suite 실행을 누르세요.",
                    "Select the pipeline to evaluate, then click Run suite.");
            }

            return OpenVisionRecipeText.Local(
                "위의 판정 기준과 보정 적용 여부를 확인한 뒤 Suite 실행을 누르세요. 결과는 아래 이력에 저장됩니다.",
                "Review the gate and calibration above, then click Run suite. Results are saved in the history below.");
        }

        internal static string BuildSelectionSummaryText(
            bool storageReady,
            OpenVisionRecipeValidationSetOption option,
            IReadOnlyList<OpenVisionRecipeValidationSetImageRow> imageRows)
        {
            if (!storageReady)
            {
                return OpenVisionRecipeText.Local(
                    "validation-sets.xml을 읽을 수 없습니다. 원본 파일을 보존한 채 오류를 수정하세요.",
                    "validation-sets.xml could not be read. Fix the error without overwriting the original file.");
            }

            if (option == null)
            {
                return OpenVisionRecipeText.Local(
                    "저장된 로컬 세트가 없습니다. 새 이름을 입력하고 만들기를 선택하세요.",
                    "No local set is saved. Enter a new name and select Create.");
            }

            string summary = option.Name
                + " | "
                + OpenVisionRecipeText.Local("이미지 ", "Images ")
                + option.ImageCount.ToString(CultureInfo.InvariantCulture)
                + " | OK "
                + option.OkCount.ToString(CultureInfo.InvariantCulture)
                + " / NG "
                + option.NgCount.ToString(CultureInfo.InvariantCulture)
                + " | "
                + OpenVisionRecipeText.Local("준비 ", "Ready ")
                + option.ReadyCount.ToString(CultureInfo.InvariantCulture)
                + " / "
                + OpenVisionRecipeText.Local("누락 ", "Missing ")
                + option.MissingCount.ToString(CultureInfo.InvariantCulture);
            if (option.IsIdentityLocked)
            {
                summary += " | "
                    + OpenVisionRecipeText.Local("위치검출 기대 성공 · 해시 잠금", "Locator expected success · Hash locked")
                    + " | "
                    + option.PipelineName
                    + " | Step "
                    + ShortHash(option.PipelineDefinitionSha256)
                    + " | Images "
                    + ShortHash(option.ImageSetSha256);
            }

            if (option.MissingCount <= 0)
            {
                return summary;
            }

            string missing = string.Join(
                ", ",
                (imageRows ?? Array.Empty<OpenVisionRecipeValidationSetImageRow>())
                    .Where(row => row != null && row.IsMissing)
                    .Take(2)
                    .Select(row => row.FileName));
            return summary
                + Environment.NewLine
                + OpenVisionRecipeText.Local("누락 파일을 복구해야 실행할 수 있습니다: ", "Repair missing files before running: ")
                + missing;
        }

        internal static string BuildValidationSuiteSummaryText(
            string recipeName,
            string pipelineName,
            string scopeDisplayText,
            bool isLocalValidationSetSelected,
            string validationSetSelectionSummary,
            string sampleName,
            string latestSampleSummary,
            string latestPairSummary,
            string latestCatalogSummary)
        {
            string recipe = Normalize(recipeName);
            string pipeline = Normalize(pipelineName);
            string scope = Normalize(scopeDisplayText);
            string header = "Active: "
                + recipe
                + " / "
                + pipeline
                + " | "
                + OpenVisionRecipeText.Local("범위: ", "Scope: ")
                + scope;
            if (isLocalValidationSetSelected)
            {
                return header
                    + Environment.NewLine
                    + (validationSetSelectionSummary ?? string.Empty);
            }

            return header
                + " | "
                + OpenVisionRecipeText.Local("샘플: ", "Sample: ")
                + Normalize(sampleName)
                + Environment.NewLine
                + OpenVisionRecipeText.Local("최근: ", "Latest: ")
                + Normalize(latestSampleSummary)
                + " / "
                + Normalize(latestPairSummary)
                + " / "
                + Normalize(latestCatalogSummary);
        }

        private static string Normalize(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? "-" : value.Trim();
        }

        private static string ShortHash(string value)
        {
            string normalized = value?.Trim() ?? string.Empty;
            return normalized.Length <= 12 ? normalized : normalized.Substring(0, 12);
        }
    }
}
