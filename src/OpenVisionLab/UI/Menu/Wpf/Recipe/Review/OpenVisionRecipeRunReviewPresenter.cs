using System;
using System.Collections.Generic;
using System.Globalization;

namespace OpenVisionLab
{
    // Keeps operator-facing review text and next-action policy independent of Host state and commands.
    internal static class OpenVisionRecipeRunReviewPresenter
    {
        internal static string BuildOperatorRunReviewText(
            OpenVisionRecipeManagerSummary summary,
            OpenVisionRecipeSampleRunSummary sample,
            OpenVisionRecipePairRunSummary pair)
        {
            summary = summary ?? OpenVisionRecipeManagerSummary.Empty;
            sample = sample ?? OpenVisionRecipeSampleRunSummary.Empty;
            pair = pair ?? OpenVisionRecipePairRunSummary.Empty;
            return string.Join(
                Environment.NewLine,
                OpenVisionRecipeText.Local("XML/단계: ", "XML/Steps: ") + summary.XmlStatusDisplay + " / " + summary.StepCount.ToString(CultureInfo.InvariantCulture),
                OpenVisionRecipeText.Local("샘플: ", "Sample: ") + sample.CompactText,
                OpenVisionRecipeText.Local("쌍 검사: ", "Pair: ") + pair.CompactText,
                OpenVisionRecipeText.Local("다음: ", "Next: ") + BuildNextAction(summary, sample, pair));
        }

        internal static string BuildSelectedPairRoleSuffix(OpenVisionRecipePairSampleRunSummary selectedRole)
        {
            if (selectedRole == null)
            {
                return string.Empty;
            }

            return Environment.NewLine
                + OpenVisionRecipeText.Local("역할 리뷰: ", "Role review: ") + selectedRole.Role + " / " + selectedRole.ResultText
                + Environment.NewLine
                + OpenVisionRecipeText.Local("역할 다음: ", "Role next: ") + selectedRole.NextActionText;
        }

        internal static string BuildSelectedBatchRunReviewText(
            OpenVisionRecipeBatchRunOption run,
            OpenVisionRecipeBatchSampleResultOption sample,
            OpenVisionRecipePipelineStepPreview linkedStep)
        {
            if (run == null || string.IsNullOrWhiteSpace(run.SummaryPath))
            {
                return OpenVisionRecipeText.Local("저장된 쌍 검사 이력을 선택하면 샘플별 결과와 실패 Step이 여기에 표시됩니다.", "Select a saved pair check run to review sample results and failed steps here.");
            }

            List<string> lines = new List<string>
            {
                OpenVisionRecipeText.Local("이력: ", "Run: ") + run.DisplayText,
                OpenVisionRecipeText.Local("샘플: ", "Sample: ") + (sample?.DisplayText ?? "-"),
                OpenVisionRecipeText.Local("결과: ", "Result: ") + (sample?.DetailText ?? "-")
            };

            if (sample?.IsInReviewQueue == true)
            {
                lines.Add(sample.ReviewReasonsToolTipText);
            }

            if (!string.IsNullOrWhiteSpace(sample?.FailedStep))
            {
                lines.Add(OpenVisionRecipeText.Local("다음: 실패 Step을 선택했습니다. 미리보기 목록에서 입력/출력과 기준을 확인하세요.", "Next: Failed step is selected. Review input/output and gates in the preview step list."));
                lines.Add(OpenVisionRecipeText.Local("연결 Step: ", "Linked step: ") + (linkedStep?.DisplayText ?? sample.FailedStep.Trim()));
            }
            else if (sample?.Success == true)
            {
                lines.Add(OpenVisionRecipeText.Local("다음: 이 샘플은 통과했습니다. NG 샘플을 선택하면 실패 Step이 연결됩니다.", "Next: This sample passed. Select an NG sample to link its failed step."));
            }

            lines.Add(OpenVisionRecipeText.Local("요약: ", "Summary: ") + run.DetailText);
            return string.Join(Environment.NewLine, lines);
        }

        internal static string BuildNextAction(
            OpenVisionRecipeManagerSummary summary,
            OpenVisionRecipeSampleRunSummary sample,
            OpenVisionRecipePairRunSummary pair)
        {
            if (summary == null || !summary.XmlValid)
            {
                return OpenVisionRecipeText.Local("LLM XML 검증 보고서를 먼저 수정하세요.", "Fix the LLM XML validation report first.");
            }

            if (summary.StepCount <= 0)
            {
                return OpenVisionRecipeText.Local("파이프라인 단계를 추가하거나 샘플에서 복제하세요.", "Add pipeline steps or duplicate from a sample.");
            }

            if (!string.Equals(summary.ActivePipelineName, summary.PreviewPipelineName, StringComparison.OrdinalIgnoreCase))
            {
                return OpenVisionRecipeText.Local("검토할 파이프라인을 활성화하거나 활성 파이프라인을 선택하세요.", "Activate the reviewed pipeline or select the active pipeline.");
            }

            if (sample == null || !sample.HasResult)
            {
                return OpenVisionRecipeText.Local("검사 실행으로 선택 샘플의 출력 레이어를 확인하세요.", "Run check to inspect the selected sample output layer.");
            }

            if (!sample.Succeeded)
            {
                return OpenVisionRecipeText.Local("샘플 실패 단계의 입력/출력 레이어와 파라미터를 조정하세요.", "Tune the failed sample step input/output layer and parameters.");
            }

            if (pair == null || !pair.HasResult)
            {
                return OpenVisionRecipeText.Local("Good/Bad 쌍 검사로 판정 기준을 확인하세요.", "Run Good/Bad pair check to verify acceptance gates.");
            }

            if (!pair.Succeeded)
            {
                return OpenVisionRecipeText.Local("Good/Bad가 모두 기준과 맞을 때까지 활성 파이프라인을 조정하세요.", "Tune the active pipeline until Good and Bad both match expectations.");
            }

            return OpenVisionRecipeText.Local("검토 완료: XML, 샘플 검사, 쌍 검사가 모두 통과했습니다.", "Review complete: XML, sample check, and pair check passed.");
        }
    }
}
