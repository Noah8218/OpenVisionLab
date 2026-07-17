using System;
using System.Globalization;

namespace OpenVisionLab
{
    // Selects the next explicit Recipe Manager action from already-evaluated availability state.
    internal static class OpenVisionRecipeGuidedWorkflowPresenter
    {
        internal static string BuildSetupText(
            OpenVisionRecipeManagerSummary summary,
            OpenVisionRecipeSampleRunSummary sample,
            OpenVisionRecipePairRunSummary pair,
            bool hasSelectedSample)
        {
            summary = summary ?? OpenVisionRecipeManagerSummary.Empty;
            sample = sample ?? OpenVisionRecipeSampleRunSummary.Empty;
            pair = pair ?? OpenVisionRecipePairRunSummary.Empty;
            string sampleState = hasSelectedSample
                ? OpenVisionRecipeText.Local("샘플 준비", "Sample ready")
                : OpenVisionRecipeText.Local("샘플 선택", "Select sample");
            string xmlState = summary.XmlValid
                ? OpenVisionRecipeText.Local("XML OK", "XML OK")
                : OpenVisionRecipeText.Local("XML 검증", "Validate XML");
            string stepState = summary.StepCount > 0
                ? OpenVisionRecipeText.Local("Step ", "Steps ") + summary.StepCount.ToString(CultureInfo.InvariantCulture)
                : OpenVisionRecipeText.Local("Step 없음", "No steps");
            string sampleRunState = sample.HasResult
                ? (sample.Succeeded ? OpenVisionRecipeText.Local("샘플 OK", "Sample OK") : OpenVisionRecipeText.Local("샘플 NG", "Sample NG"))
                : OpenVisionRecipeText.Local("샘플 실행", "Run sample");
            string pairRunState = pair.HasResult
                ? (pair.Succeeded ? OpenVisionRecipeText.Local("Good/Bad OK", "Good/Bad OK") : OpenVisionRecipeText.Local("Good/Bad NG", "Good/Bad NG"))
                : OpenVisionRecipeText.Local("Good/Bad 실행", "Run Good/Bad");
            string next = OpenVisionRecipeRunReviewPresenter.BuildNextAction(summary, sample, pair);
            return OpenVisionRecipeText.Local("가이드", "Guide")
                + ": 1 " + sampleState
                + " -> 2 " + xmlState
                + " -> 3 " + stepState
                + " -> 4 " + sampleRunState
                + " -> 5 " + pairRunState
                + " | " + OpenVisionRecipeText.Local("다음: ", "Next: ") + next;
        }

        internal static OpenVisionRecipeGuidedWorkflowAction ResolveNextAction(OpenVisionRecipeGuidedWorkflowActionRequest request)
        {
            request = request ?? new OpenVisionRecipeGuidedWorkflowActionRequest();
            OpenVisionRecipeManagerSummary summary = request.Summary ?? OpenVisionRecipeManagerSummary.Empty;
            OpenVisionRecipeSampleRunSummary sample = request.Sample ?? OpenVisionRecipeSampleRunSummary.Empty;
            OpenVisionRecipePairRunSummary pair = request.Pair ?? OpenVisionRecipePairRunSummary.Empty;

            if (!summary.XmlValid && request.CanValidateLlmXmlDraft)
            {
                return OpenVisionRecipeGuidedWorkflowAction.ValidateLlmXmlDraft;
            }

            if (summary.StepCount <= 0 && request.CanDuplicatePipelineFromSample)
            {
                return OpenVisionRecipeGuidedWorkflowAction.DuplicatePipelineFromSample;
            }

            if (!string.Equals(summary.ActivePipelineName, summary.PreviewPipelineName, StringComparison.OrdinalIgnoreCase)
                && request.CanActivateSelectedPipeline)
            {
                return OpenVisionRecipeGuidedWorkflowAction.ActivateSelectedPipeline;
            }

            if (!sample.HasResult && request.CanRunSelectedSampleCheck)
            {
                return OpenVisionRecipeGuidedWorkflowAction.RunSelectedSampleCheck;
            }

            if (sample.HasResult && !sample.Succeeded && request.CanLoadSelectedStepParameters)
            {
                return OpenVisionRecipeGuidedWorkflowAction.LoadSelectedStepParameters;
            }

            if (!pair.HasResult && request.CanRunSelectedSamplePairCheck)
            {
                return OpenVisionRecipeGuidedWorkflowAction.RunSelectedSamplePairCheck;
            }

            if (pair.HasResult && !pair.Succeeded && request.CanOpenSelectedStepTool)
            {
                return OpenVisionRecipeGuidedWorkflowAction.OpenSelectedStepTool;
            }

            return OpenVisionRecipeGuidedWorkflowAction.None;
        }

        internal static string BuildNextActionText(OpenVisionRecipeGuidedWorkflowActionRequest request)
        {
            switch (ResolveNextAction(request))
            {
                case OpenVisionRecipeGuidedWorkflowAction.ValidateLlmXmlDraft:
                    return OpenVisionRecipeText.Local("XML 검증", "Validate XML");
                case OpenVisionRecipeGuidedWorkflowAction.DuplicatePipelineFromSample:
                    return OpenVisionRecipeText.Local("샘플 복제", "Duplicate sample");
                case OpenVisionRecipeGuidedWorkflowAction.ActivateSelectedPipeline:
                    return OpenVisionRecipeText.Local("활성화", "Activate");
                case OpenVisionRecipeGuidedWorkflowAction.RunSelectedSampleCheck:
                    return OpenVisionRecipeText.Local("검사 실행", "Run check");
                case OpenVisionRecipeGuidedWorkflowAction.LoadSelectedStepParameters:
                    return OpenVisionRecipeText.Local("파라미터 열기", "Load params");
                case OpenVisionRecipeGuidedWorkflowAction.RunSelectedSamplePairCheck:
                    return OpenVisionRecipeText.Local("Good/Bad 실행", "Run Good/Bad");
                case OpenVisionRecipeGuidedWorkflowAction.OpenSelectedStepTool:
                    return OpenVisionRecipeText.Local("도구 열기", "Open tool");
                default:
                    return OpenVisionRecipeText.Local("완료", "Complete");
            }
        }
    }

    internal sealed class OpenVisionRecipeGuidedWorkflowActionRequest
    {
        internal OpenVisionRecipeManagerSummary Summary { get; set; }

        internal OpenVisionRecipeSampleRunSummary Sample { get; set; }

        internal OpenVisionRecipePairRunSummary Pair { get; set; }

        internal bool CanValidateLlmXmlDraft { get; set; }

        internal bool CanDuplicatePipelineFromSample { get; set; }

        internal bool CanActivateSelectedPipeline { get; set; }

        internal bool CanRunSelectedSampleCheck { get; set; }

        internal bool CanLoadSelectedStepParameters { get; set; }

        internal bool CanRunSelectedSamplePairCheck { get; set; }

        internal bool CanOpenSelectedStepTool { get; set; }
    }

    internal enum OpenVisionRecipeGuidedWorkflowAction
    {
        None,
        ValidateLlmXmlDraft,
        DuplicatePipelineFromSample,
        ActivateSelectedPipeline,
        RunSelectedSampleCheck,
        LoadSelectedStepParameters,
        RunSelectedSamplePairCheck,
        OpenSelectedStepTool
    }
}
