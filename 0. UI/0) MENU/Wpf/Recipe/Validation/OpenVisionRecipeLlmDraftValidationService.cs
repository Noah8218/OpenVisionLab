using Lib.OpenCV;
using Lib.OpenCV.Pipeline;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace OpenVisionLab
{
    internal sealed class OpenVisionRecipeLlmDraftValidationRequest
    {
        internal OpenVisionRecipeLlmDraftValidationRequest(
            string xmlText,
            string recipeName,
            string selectedTemplate,
            string referenceImagePath,
            bool applyIntentContract,
            OpenVisionRecipeReviewBundleInspection reviewBundleInspection,
            bool copyDependencies)
        {
            XmlText = xmlText ?? string.Empty;
            RecipeName = recipeName ?? string.Empty;
            SelectedTemplate = selectedTemplate ?? string.Empty;
            ReferenceImagePath = referenceImagePath ?? string.Empty;
            ApplyIntentContract = applyIntentContract;
            ReviewBundleInspection = reviewBundleInspection;
            CopyDependencies = copyDependencies;
        }

        internal string XmlText { get; }

        internal string RecipeName { get; }

        internal string SelectedTemplate { get; }

        internal string ReferenceImagePath { get; }

        internal bool ApplyIntentContract { get; }

        internal OpenVisionRecipeReviewBundleInspection ReviewBundleInspection { get; }

        internal bool CopyDependencies { get; }
    }

    internal sealed class OpenVisionRecipeLlmDraftValidationResult
    {
        internal OpenVisionRecipeLlmDraftValidationResult(
            bool success,
            VisionPipeline pipeline,
            string validationReport,
            string dependencyReport,
            IReadOnlyList<OpenVisionRecipeDependencyReviewRow> dependencyRows)
        {
            Success = success;
            Pipeline = pipeline;
            ValidationReport = validationReport ?? string.Empty;
            DependencyReport = dependencyReport ?? string.Empty;
            DependencyRows = dependencyRows ?? Array.Empty<OpenVisionRecipeDependencyReviewRow>();
        }

        internal bool Success { get; }

        internal VisionPipeline Pipeline { get; }

        internal string ValidationReport { get; }

        internal string DependencyReport { get; }

        internal IReadOnlyList<OpenVisionRecipeDependencyReviewRow> DependencyRows { get; }
    }

    internal static class OpenVisionRecipeLlmDraftValidationService
    {
        internal static OpenVisionRecipeLlmDraftValidationResult Validate(OpenVisionRecipeLlmDraftValidationRequest request)
        {
            List<string> validationLines = new List<string>
            {
                OpenVisionRecipeText.Local("LLM 초안 검증: 대기", "LLM draft validation: WAIT")
            };
            string xmlText = request?.XmlText ?? string.Empty;
            if (string.IsNullOrWhiteSpace(xmlText))
            {
                validationLines[0] = OpenVisionRecipeText.Local("LLM 초안 검증: NG", "LLM draft validation: NG");
                validationLines.Add(OpenVisionRecipeText.Local("XML 텍스트가 비어 있습니다.", "XML text is empty."));
                validationLines.Add(OpenVisionRecipeText.Local("다음: 검증 전에 VisionPipeline XML 초안을 붙여넣거나 로드하세요.", "Next: Paste or load a VisionPipeline XML draft before validation."));
                return Failure(
                    validationLines,
                    OpenVisionRecipeText.Local("의존 파일 스캔 건너뜀.", "Dependency scan skipped."),
                    OpenVisionRecipeText.Local("VisionPipeline XML 초안을 붙여넣거나 로드하세요.", "Paste or load a VisionPipeline XML draft."));
            }

            if (!OpenVisionRecipeLlmDraftValidationRules.TryValidateXmlSyntax(xmlText, validationLines))
            {
                return Failure(
                    validationLines,
                    OpenVisionRecipeText.Local("의존 파일 스캔 건너뜀.", "Dependency scan skipped."),
                    OpenVisionRecipeText.Local("XML 문법을 수정한 뒤 다시 검증하세요.", "Fix XML syntax, then validate again."));
            }

            if (!SerializeHelper.TryLoadFromXmlText(xmlText, out VisionPipeline pipeline, out string deserializeMessage) || pipeline == null)
            {
                validationLines[0] = OpenVisionRecipeText.Local("LLM 초안 검증: NG", "LLM draft validation: NG");
                validationLines.Add(OpenVisionRecipeText.Local("OpenVision 파이프라인 역직렬화: NG - ", "OpenVision pipeline deserialize: NG - ") + deserializeMessage);
                validationLines.Add(OpenVisionRecipeText.Local("다음: OpenVisionLab VisionPipeline 스키마에 맞는 XML을 LLM에 다시 생성하게 하세요.", "Next: Ask the LLM to regenerate XML that matches the OpenVisionLab VisionPipeline schema."));
                return Failure(
                    validationLines,
                    OpenVisionRecipeText.Local("의존 파일 스캔 건너뜀.", "Dependency scan skipped."),
                    OpenVisionRecipeText.Local("OpenVisionLab VisionPipeline XML 구조를 수정하세요.", "Fix the OpenVisionLab VisionPipeline XML structure."));
            }

            validationLines.Add(OpenVisionRecipeText.Local("OpenVision 파이프라인 역직렬화: OK", "OpenVision pipeline deserialize: OK"));
            validationLines.Add(OpenVisionRecipeText.Local("파이프라인: ", "Pipeline: ") + (string.IsNullOrWhiteSpace(pipeline.Name) ? "-" : pipeline.Name));
            validationLines.Add(OpenVisionRecipeText.Local("단계: ", "Steps: ") + (pipeline.Steps?.Count ?? 0).ToString(CultureInfo.InvariantCulture));
            VisionPipelineValidationResult validation = VisionPipelineValidator.Validate(pipeline, new[] { "Main" });
            validationLines.Add(string.Format(
                CultureInfo.CurrentCulture,
                OpenVisionRecipeText.Local("스키마/경로: {0} / 오류: {1} / 경고: {2}", "Schema/routing: {0} / Errors: {1} / Warnings: {2}"),
                validation.Success ? "OK" : "NG",
                validation.Errors.Count,
                validation.Warnings.Count));

            foreach (string error in validation.Errors.Take(4))
            {
                validationLines.Add(OpenVisionRecipeText.Local("오류: ", "Error: ") + error);
            }

            foreach (string warning in validation.Warnings.Take(4))
            {
                validationLines.Add(OpenVisionRecipeText.Local("경고: ", "Warning: ") + warning);
            }

            if (!validation.Success)
            {
                validationLines.Add(OpenVisionRecipeText.Local("다음: 나열된 경로/레이어/도구 오류를 수정한 뒤 가져오기 전에 다시 검증하세요.", "Next: Fix the listed route/layer/tool errors, then validate again before import."));
            }

            bool intentContractReady;
            if (request?.ApplyIntentContract == true)
            {
                intentContractReady = OpenVisionRecipeLlmDraftValidationRules.AppendIntentContractValidation(
                    pipeline,
                    request.SelectedTemplate,
                    validationLines);
            }
            else
            {
                validationLines.Add(OpenVisionRecipeText.Local(
                    "검토 번들 의도 계약: 내보낸 파이프라인을 그대로 검토하므로 현재 Guided setup 의도 필터를 적용하지 않습니다.",
                    "Review bundle intent contract: the exported pipeline is reviewed as-is, so the current Guided setup intent filter is not applied."));
                intentContractReady = true;
            }

            bool resultChannelsReady = OpenVisionRecipeLlmDraftValidationRules.AppendResultChannelValidation(pipeline, xmlText, validationLines);

            string referenceImagePath = request?.ReferenceImagePath ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(referenceImagePath))
            {
                validationLines.Add(File.Exists(referenceImagePath)
                    ? OpenVisionRecipeText.Local("참조 이미지: OK - ", "Reference image: OK - ") + referenceImagePath
                    : OpenVisionRecipeText.Local("참조 이미지: 없음 - ", "Reference image: missing - ") + referenceImagePath);
                if (!File.Exists(referenceImagePath))
                {
                    validationLines.Add(OpenVisionRecipeText.Local("다음: 존재하는 참조 이미지를 선택하거나 선택된 샘플 이미지를 사용하세요.", "Next: Choose an existing reference image or use the selected sample image."));
                }
            }

            OpenVisionRecipeDependencyReviewResult dependencyReview = OpenVisionRecipeDependencyReviewService.Review(
                pipeline,
                request?.RecipeName ?? string.Empty,
                request?.CopyDependencies == true,
                request?.ReviewBundleInspection);
            if (dependencyReview.BlockingDependencyCount > 0)
            {
                validationLines.Add(string.Format(
                    CultureInfo.CurrentCulture,
                    OpenVisionRecipeText.Local("오류: LLM XML 의존 파일 경로/내용 문제 {0}개가 있습니다.", "Error: {0} LLM XML dependency path/content issue(s) were found."),
                    dependencyReview.BlockingDependencyCount));
                validationLines.Add(OpenVisionRecipeText.Local(
                    "다음: 가져오기 전에 누락/변경 파일을 확인하고 XML 경로를 검증된 파일로 명시적으로 바꾸세요.",
                    "Next: review missing/changed files and explicitly replace XML paths with verified files before import."));
            }

            bool success = validation.Success
                && dependencyReview.BlockingDependencyCount == 0
                && resultChannelsReady
                && intentContractReady;
            validationLines[0] = success
                ? OpenVisionRecipeText.Local("LLM 초안 검증: OK", "LLM draft validation: OK")
                : OpenVisionRecipeText.Local("LLM 초안 검증: NG", "LLM draft validation: NG");
            return new OpenVisionRecipeLlmDraftValidationResult(
                success,
                pipeline,
                string.Join(Environment.NewLine, validationLines),
                dependencyReview.Report,
                dependencyReview.Rows);
        }

        private static OpenVisionRecipeLlmDraftValidationResult Failure(
            IReadOnlyList<string> validationLines,
            string dependencyReport,
            string dependencyAction)
        {
            return new OpenVisionRecipeLlmDraftValidationResult(
                false,
                null,
                string.Join(Environment.NewLine, validationLines ?? Array.Empty<string>()),
                dependencyReport,
                new[]
                {
                    new OpenVisionRecipeDependencyReviewRow(
                        OpenVisionRecipeText.Local("대기", "Waiting"),
                        "-",
                        "-",
                        "-",
                        dependencyAction)
                });
        }
    }
}
