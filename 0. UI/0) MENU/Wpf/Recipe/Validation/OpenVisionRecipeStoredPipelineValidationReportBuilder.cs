using Lib.OpenCV.Pipeline;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace OpenVisionLab
{
    // Formats validation evidence for an already-loaded stored pipeline; storage and execution stay outside this builder.
    internal static class OpenVisionRecipeStoredPipelineValidationReportBuilder
    {
        internal static string Build(OpenVisionRecipeStoredPipelineValidationReportRequest request)
        {
            request = request ?? new OpenVisionRecipeStoredPipelineValidationReportRequest();
            string pipelinePath = request.PipelinePath ?? string.Empty;
            bool xmlOk = request.XmlOk;
            VisionPipeline activePipeline = request.Pipeline;
            string xmlMessage = request.XmlMessage ?? string.Empty;
            List<string> lines = new List<string>
            {
                OpenVisionRecipeText.Local("LLM XML 검증: ", "LLM XML validation: ") + (xmlOk ? "OK" : "NG"),
                OpenVisionRecipeText.Local("XML 로드: ", "XML load: ") + (xmlOk ? "OK" : xmlMessage),
                OpenVisionRecipeText.Local("가정한 소스 레이어: Main", "Assumed source layer: Main")
            };

            if (!xmlOk || activePipeline == null)
            {
                lines.Add(OpenVisionRecipeText.Local("조치: LLM에 지원되는 ToolType, InputLayer, OutputLayer, Parameters를 포함한 OpenVisionLab VisionPipeline XML을 다시 출력하게 하세요.", "Action: ask the LLM to output OpenVisionLab VisionPipeline XML with supported ToolType, InputLayer, OutputLayer, and Parameters."));
                return string.Join(Environment.NewLine, lines);
            }

            string filePipelineName = Path.GetFileNameWithoutExtension(pipelinePath) ?? string.Empty;
            lines.Add(string.Format(
                CultureInfo.CurrentCulture,
                OpenVisionRecipeText.Local("파이프라인: {0} / 단계: {1}", "Pipeline: {0} / Steps: {1}"),
                string.IsNullOrWhiteSpace(activePipeline.Name) ? "-" : activePipeline.Name,
                activePipeline.Steps.Count));

            if (!string.Equals(filePipelineName, activePipeline.Name ?? string.Empty, StringComparison.OrdinalIgnoreCase))
            {
                lines.Add(string.Format(
                    CultureInfo.CurrentCulture,
                    OpenVisionRecipeText.Local("경고: XML 이름 '{0}'이 파일 '{1}'과 다릅니다.", "Warning: XML Name '{0}' differs from file '{1}'."),
                    activePipeline.Name ?? string.Empty,
                    filePipelineName));
            }

            VisionPipelineValidationResult validation = VisionPipelineValidator.Validate(activePipeline, new[] { "Main" });
            lines.Add(string.Format(
                CultureInfo.CurrentCulture,
                OpenVisionRecipeText.Local("스키마/경로: {0} / 오류: {1} / 경고: {2}", "Schema/routing: {0} / Errors: {1} / Warnings: {2}"),
                validation.Success ? "OK" : "NG",
                validation.Errors.Count,
                validation.Warnings.Count));

            foreach (string error in validation.Errors.Take(4))
            {
                lines.Add(OpenVisionRecipeText.Local("오류: ", "Error: ") + error);
            }

            if (validation.Errors.Count > 4)
            {
                lines.Add(OpenVisionRecipeText.Local("오류: +", "Error: +") + (validation.Errors.Count - 4).ToString(CultureInfo.InvariantCulture) + OpenVisionRecipeText.Local("개 더 있음", " more"));
            }

            foreach (string warning in validation.Warnings.Take(4))
            {
                lines.Add(OpenVisionRecipeText.Local("경고: ", "Warning: ") + warning);
            }

            if (validation.Warnings.Count > 4)
            {
                lines.Add(OpenVisionRecipeText.Local("경고: +", "Warning: +") + (validation.Warnings.Count - 4).ToString(CultureInfo.InvariantCulture) + OpenVisionRecipeText.Local("개 더 있음", " more"));
            }

            return string.Join(Environment.NewLine, lines);
        }
    }

    internal sealed class OpenVisionRecipeStoredPipelineValidationReportRequest
    {
        internal string PipelinePath { get; set; }

        internal bool XmlOk { get; set; }

        internal VisionPipeline Pipeline { get; set; }

        internal string XmlMessage { get; set; }
    }
}
