using OpenVisionLab.Vision2D.Pipeline;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace OpenVisionLab
{
    // Owns Recipe Manager summary and library text projection from explicit snapshots.
    internal sealed class OpenVisionRecipeManagerSummaryProjectionOwner
    {
        internal OpenVisionRecipeManagerSummary Project(
            OpenVisionRecipeManagerSummaryProjectionRequest request)
        {
            request = request ?? new OpenVisionRecipeManagerSummaryProjectionRequest();
            int stepCount = request.Pipeline?.Steps?.Count ?? 0;
            string llmValidationReport = OpenVisionRecipeStoredPipelineValidationReportBuilder.Build(
                new OpenVisionRecipeStoredPipelineValidationReportRequest
                {
                    PipelinePath = request.PipelinePath,
                    XmlOk = request.XmlValid,
                    Pipeline = request.Pipeline,
                    XmlMessage = request.XmlMessage
                });
            IReadOnlyList<OpenVisionRecipePipelineStepPreview> previewSteps =
                BuildPipelinePreviewSteps(request.Pipeline, request.LayerCardProvider);
            string updatedText = request.LastWriteTime.HasValue
                ? request.LastWriteTime.Value.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.CurrentCulture)
                : "-";
            string detail = string.Join(
                Environment.NewLine,
                string.Format(
                    CultureInfo.CurrentCulture,
                    OpenVisionRecipeText.Local("활성 파이프라인: {0}", "Active pipeline: {0}"),
                    request.ActivePipelineName),
                string.Format(
                    CultureInfo.CurrentCulture,
                    OpenVisionRecipeText.Local("파이프라인 수: {0}", "Pipelines: {0}"),
                    request.PipelineCount),
                string.Format(
                    CultureInfo.CurrentCulture,
                    OpenVisionRecipeText.Local("Step 수: {0}", "Steps: {0}"),
                    stepCount),
                string.Format(
                    CultureInfo.CurrentCulture,
                    OpenVisionRecipeText.Local("XML: {0}", "XML: {0}"),
                    request.XmlValid ? "OK" : "NG - " + request.XmlMessage),
                string.Format(
                    CultureInfo.CurrentCulture,
                    OpenVisionRecipeText.Local("수정: {0}", "Updated: {0}"),
                    updatedText),
                string.Format(
                    CultureInfo.CurrentCulture,
                    OpenVisionRecipeText.Local("경로: {0}", "Path: {0}"),
                    request.PipelinePath));

            detail = string.Format(
                    CultureInfo.CurrentCulture,
                    OpenVisionRecipeText.Local("선택 파이프라인: {0}", "Selected pipeline: {0}"),
                    request.PreviewPipelineName)
                + Environment.NewLine
                + detail;

            return new OpenVisionRecipeManagerSummary(
                request.RecipeName,
                request.ActivePipelineName,
                request.PreviewPipelineName,
                request.PipelineCount,
                stepCount,
                request.XmlValid,
                detail,
                llmValidationReport,
                previewSteps);
        }

        internal string ProjectLibrarySummary(
            string libraryText,
            int total,
            int visible)
        {
            if (total <= 0)
            {
                return libraryText ?? string.Empty;
            }

            return visible == total
                ? string.Format(CultureInfo.CurrentCulture, "{0} ({1})", libraryText, total)
                : string.Format(CultureInfo.CurrentCulture, "{0} ({1}/{2})", libraryText, visible, total);
        }

        private static IReadOnlyList<OpenVisionRecipePipelineStepPreview> BuildPipelinePreviewSteps(
            VisionPipeline pipeline,
            Func<string, OpenVisionRecipeLayerCard> layerCardProvider)
        {
            if (pipeline?.Steps == null || pipeline.Steps.Count == 0)
            {
                return Array.Empty<OpenVisionRecipePipelineStepPreview>();
            }

            List<OpenVisionRecipePipelineStepPreview> steps = new List<OpenVisionRecipePipelineStepPreview>();
            for (int i = 0; i < pipeline.Steps.Count; i++)
            {
                steps.Add(new OpenVisionRecipePipelineStepPreview(
                    i + 1,
                    pipeline.Steps[i],
                    layerCardProvider));
            }

            return steps;
        }
    }

    internal sealed class OpenVisionRecipeManagerSummaryProjectionRequest
    {
        internal string RecipeName { get; set; }

        internal string ActivePipelineName { get; set; }

        internal string PreviewPipelineName { get; set; }

        internal int PipelineCount { get; set; }

        internal DateTime? LastWriteTime { get; set; }

        internal bool XmlValid { get; set; }

        internal string XmlMessage { get; set; }

        internal string PipelinePath { get; set; }

        internal VisionPipeline Pipeline { get; set; }

        internal Func<string, OpenVisionRecipeLayerCard> LayerCardProvider { get; set; }
    }
}
