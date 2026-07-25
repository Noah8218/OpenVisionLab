using System;

namespace OpenVisionLab
{
    internal sealed class OpenVisionRecipeLlmReviewBundleRequest
    {
        internal string RecipeName { get; init; } = string.Empty;

        internal string PipelineName { get; init; } = string.Empty;

        internal string Template { get; init; } = string.Empty;

        internal string SelectedStepOperatorContextText { get; init; } = string.Empty;

        internal string FailureReviewText { get; init; } = string.Empty;

        internal string ValidationReport { get; init; } = string.Empty;

        internal string DependencyReport { get; init; } = string.Empty;

        internal string DraftReviewReport { get; init; } = string.Empty;

        internal string DiffReport { get; init; } = string.Empty;

        internal string XmlDraftText { get; init; } = string.Empty;
    }

    internal static class OpenVisionRecipeLlmReviewBundleBuilder
    {
        internal static string Build(OpenVisionRecipeLlmReviewBundleRequest request)
        {
            request ??= new OpenVisionRecipeLlmReviewBundleRequest();
            return string.Join(Environment.NewLine, new[]
            {
                "OpenVisionLab LLM XML review bundle",
                "Instruction: revise the VisionPipeline XML using this feedback. Return only a VisionPipeline XML document.",
                "Recipe: " + (request.RecipeName ?? string.Empty),
                "Pipeline: " + DisplayOrDash(request.PipelineName),
                "Inspection intent: " + (request.Template ?? string.Empty),
                "Intent contract: " + OpenVisionRecipeLlmIntent.BuildLlmIntentContractText(request.Template),
                "",
                "[Correction rules]",
                "- Use only OpenVisionLab VisionPipeline XML and return only XML.",
                "- Use InputLayer=\"Main\" or the exact OutputLayer of a previous enabled step; do not invent layers.",
                "- Use supported OpenVisionLab ToolType names and PropertyGrid-compatible parameter values.",
                "- Do not switch to another tool family unless the selected intent contract explicitly allows it.",
                "- Replace missing template/image dependency paths with existing files, or remove those dependency parameters until a real file is selected.",
                "- Do not add camera, lighting, PLC, I/O, account, Preview, or Run instructions.",
                "",
                "[Result channel contract]",
                OpenVisionRecipeLlmIntent.BuildLlmResultChannelContractText(),
                "",
                "[Selected step operator context]",
                DisplayOrDash(request.SelectedStepOperatorContextText),
                "",
                "[Failure review]",
                DisplayOrDash(request.FailureReviewText),
                "",
                "[Validation report]",
                DisplayOrDash(request.ValidationReport),
                "",
                "[Dependency report]",
                DisplayOrDash(request.DependencyReport),
                "",
                "[Draft import review]",
                DisplayOrDash(request.DraftReviewReport),
                "",
                "[Diff review]",
                DisplayOrDash(request.DiffReport),
                "",
                "[Current XML draft]",
                DisplayOrDash(request.XmlDraftText)
            });
        }

        private static string DisplayOrDash(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? "-" : value;
        }
    }
}
