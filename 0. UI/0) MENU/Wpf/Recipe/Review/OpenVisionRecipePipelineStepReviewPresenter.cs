using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace OpenVisionLab
{
    // Formats selected-step review, correction guidance, and branch/output comparison from supplied DTO state.
    internal static class OpenVisionRecipePipelineStepReviewPresenter
    {
        internal static string BuildOperatorContext(
            OpenVisionRecipePipelineStepPreview step,
            OpenVisionRecipePairSampleRunSummary pairResult,
            OpenVisionRecipeBatchSampleResultOption batchSample,
            OpenVisionRecipeBatchRunComparisonRow batchComparison)
        {
            if (step == null)
            {
                return OpenVisionRecipeText.Local(
                    "Step을 선택하면 선택 이유, 입력/출력 경로, 다음 검토 순서가 여기에 표시됩니다.",
                    "Select a step to see why it is under review, its input/output route, and the next review action.");
            }

            List<string> lines = new List<string>
            {
                OpenVisionRecipeText.Local("선택 Step: ", "Selected step: ") + step.DisplayText,
                OpenVisionRecipeText.Local("경로: ", "Route: ") + step.InputLayer + " -> " + step.OutputLayer
            };

            if (pairResult?.CanOpenFailedStep == true)
            {
                lines.Add(
                    OpenVisionRecipeText.Local("Good/Bad 실패 연결: ", "Good/Bad failure link: ")
                    + pairResult.Role
                    + " / "
                    + pairResult.SampleName
                    + " / "
                    + pairResult.ResultText);
            }
            else if (!string.IsNullOrWhiteSpace(batchSample?.FailedStep))
            {
                lines.Add(
                    OpenVisionRecipeText.Local("실행 이력 실패 연결: ", "Run-history failure link: ")
                    + batchSample.DisplayText);
            }
            else if (!string.IsNullOrWhiteSpace(batchComparison?.FailedStep))
            {
                lines.Add(
                    OpenVisionRecipeText.Local("비교 이력 실패 연결: ", "Comparison failure link: ")
                    + batchComparison.DisplayText);
            }
            else
            {
                lines.Add(OpenVisionRecipeText.Local("실패 연결: 없음", "Failure link: none"));
            }

            lines.Add(OpenVisionRecipeText.Local(
                "다음: 출력 보기 -> 입력과 비교 -> PropertyGrid 검토 -> Good/Bad 명시 재검사",
                "Next: view output -> compare input -> review PropertyGrid -> explicitly rerun Good/Bad."));

            return string.Join(Environment.NewLine, lines);
        }

        internal static string BuildFailureReviewText(
            OpenVisionRecipePipelineStepPreview step,
            OpenVisionRecipePairSampleRunSummary pairResult,
            OpenVisionRecipeBatchSampleResultOption batchSample)
        {
            if (step == null)
            {
                return OpenVisionRecipeText.Local(
                    "Good/Bad 역할 또는 실행 이력에서 실패 Step을 선택하면 입력/출력 레이어 비교와 재검사 경로가 여기에 표시됩니다.",
                    "Select a failed step from Good/Bad roles or run history to see input/output comparison and rerun actions here.");
            }

            List<string> lines = new List<string>
            {
                OpenVisionRecipeText.Local("선택 Step: ", "Selected step: ") + step.DisplayText,
                OpenVisionRecipeText.Local("비교: ", "Compare: ") + step.InputLayer + " -> " + step.OutputLayer,
                OpenVisionRecipeText.Local("다음: 출력 보기로 결과 레이어를 확인하고, 입력 보기로 원본 기준을 확인한 뒤 Good/Bad 재검사를 실행하세요.",
                    "Next: view the output layer, compare it against the input layer, then rerun Good/Bad.")
            };

            if (pairResult?.CanOpenFailedStep == true)
            {
                lines.Insert(
                    1,
                    OpenVisionRecipeText.Local("역할 실패: ", "Role failure: ")
                    + pairResult.Role
                    + " / "
                    + pairResult.SampleName);
            }
            else if (!string.IsNullOrWhiteSpace(batchSample?.FailedStep))
            {
                lines.Insert(
                    1,
                    OpenVisionRecipeText.Local("이력 실패: ", "History failure: ")
                    + batchSample.DisplayText);
            }

            return string.Join(Environment.NewLine, lines);
        }

        internal static string BuildCorrectedOutputReviewText(
            OpenVisionRecipePipelineStepPreview step,
            bool isStepEditDirty,
            object selectedStepEditObject)
        {
            if (step == null)
            {
                return OpenVisionRecipeText.Local(
                    "Step을 선택하면 XML 수정 후 출력 확인 순서가 표시됩니다.",
                    "Select a step to see the XML edit and corrected-output check sequence.");
            }

            if (isStepEditDirty)
            {
                return OpenVisionRecipeText.Local(
                    "편집됨: XML 반영을 누른 뒤 출력 보기 또는 Good/Bad 재검사로 수정 결과를 확인하세요.",
                    "Edited: apply to XML, then use View output or Rerun Good/Bad to check the correction.");
            }

            if (selectedStepEditObject == null)
            {
                return OpenVisionRecipeText.Local(
                    "파라미터 불러오기 -> PropertyGrid 검토 -> XML 반영 -> 출력 보기/Good-Bad 재검사 순서로 확인하세요.",
                    "Load parameters -> review in PropertyGrid -> apply to XML -> view output or rerun Good/Bad.");
            }

            return OpenVisionRecipeText.Local(
                "PropertyGrid 값을 검토 중입니다. 변경 후 XML 반영을 눌러야 corrected output 검토가 시작됩니다.",
                "Reviewing PropertyGrid values. Apply to XML after edits to start corrected-output review.");
        }

        internal static string BuildCorrectedOutputAppliedText(
            OpenVisionRecipePipelineStepPreview step,
            string pipelineName,
            int selectedIndex,
            string validationMessage)
        {
            string route = step == null
                ? "-"
                : step.InputLayer + " -> " + step.OutputLayer;

            return OpenVisionRecipeText.Local(
                    "XML 반영 완료: ",
                    "Applied to XML: ")
                + pipelineName
                + " / Step "
                + selectedIndex.ToString(CultureInfo.InvariantCulture)
                + Environment.NewLine
                + OpenVisionRecipeText.Local("확인 경로: ", "Check route: ")
                + route
                + Environment.NewLine
                + OpenVisionRecipeText.Local(
                    "다음: 출력 보기로 corrected output 레이어를 확인하고, Good/Bad 재검사를 명시 실행해 판정 기준을 다시 확인하세요.",
                    "Next: view the corrected output layer, then explicitly rerun Good/Bad to recheck acceptance gates.")
                + Environment.NewLine
                + OpenVisionRecipeText.Local("검증: ", "Validation: ")
                + validationMessage;
        }

        internal static string BuildStepFlowReview(
            IReadOnlyList<OpenVisionRecipePipelineStepPreview> steps,
            OpenVisionRecipePipelineStepPreview selected,
            OpenVisionRecipePipelineStepPreview previous,
            OpenVisionRecipePipelineStepPreview next)
        {
            if (steps == null || steps.Count == 0)
            {
                return OpenVisionRecipeText.Local("선택한 파이프라인에 검토할 Step이 없습니다.", "The selected pipeline has no steps to review.");
            }

            if (selected == null)
            {
                return OpenVisionRecipeText.Local("Step을 선택하면 입력/출력 흐름과 앞뒤 Step을 여기에서 확인할 수 있습니다.", "Select a step to review its input/output flow and neighboring steps here.");
            }

            string position = selected.Index.ToString(CultureInfo.InvariantCulture)
                + "/"
                + steps.Count.ToString(CultureInfo.InvariantCulture);
            string flow = selected.InputLayer + " -> " + selected.OutputLayer;
            return OpenVisionRecipeText.Local("현재 Step ", "Current step ")
                + position
                + " | "
                + selected.ToolType
                + " | "
                + flow
                + " | "
                + OpenVisionRecipeText.Local("이전: ", "Previous: ")
                + (previous == null ? "-" : previous.OutputLayer)
                + " | "
                + OpenVisionRecipeText.Local("다음: ", "Next: ")
                + (next == null ? "-" : next.InputLayer);
        }

        internal static string BuildBranchOutputComparisonText(
            IReadOnlyList<OpenVisionRecipePipelineStepPreview> steps,
            OpenVisionRecipePipelineStepPreview selected)
        {
            steps ??= Array.Empty<OpenVisionRecipePipelineStepPreview>();
            if (selected == null || steps.Count == 0)
            {
                return OpenVisionRecipeText.Local(
                    "Step을 선택하면 같은 입력의 출력 후보와 downstream 소비 Step을 비교합니다.",
                    "Select a step to compare same-input output candidates and downstream consumers.");
            }

            int sameInputBranches = steps.Count(step =>
                step != null
                && step.Index != selected.Index
                && !HasDeclaredSourceLayers(selected)
                && !HasDeclaredSourceLayers(step)
                && string.Equals(step.InputLayer, selected.InputLayer, StringComparison.OrdinalIgnoreCase));
            int outputConsumers = steps.Count(step =>
                step != null
                && step.Index != selected.Index
                && ConsumesOutputLayer(step, selected.OutputLayer));
            int upstreamProducers = steps.Count(step =>
                step != null
                && step.Index != selected.Index
                && (string.Equals(step.OutputLayer, selected.InputLayer, StringComparison.OrdinalIgnoreCase)
                    || UsesDeclaredSourceLayer(selected, step.OutputLayer)));

            return string.Format(
                CultureInfo.CurrentCulture,
                OpenVisionRecipeText.Local(
                    "분기/출력 비교: 같은 입력 후보 {0}, 출력 소비 Step {1}, 입력 생성 Step {2}",
                    "Branch/output comparison: same-input candidates {0}, output consumers {1}, input producers {2}"),
                sameInputBranches,
                outputConsumers,
                upstreamProducers);
        }

        internal static IReadOnlyList<OpenVisionRecipeBranchOutputComparisonRow> BuildBranchOutputComparisonRows(
            IReadOnlyList<OpenVisionRecipePipelineStepPreview> steps,
            OpenVisionRecipePipelineStepPreview selected)
        {
            steps ??= Array.Empty<OpenVisionRecipePipelineStepPreview>();
            if (selected == null || steps.Count == 0)
            {
                return new[]
                {
                    new OpenVisionRecipeBranchOutputComparisonRow(
                        OpenVisionRecipeText.Local("대기", "Waiting"),
                        "-",
                        "-",
                        OpenVisionRecipeText.Local("Step 선택 필요", "Select a step"))
                };
            }

            List<OpenVisionRecipeBranchOutputComparisonRow> rows = new List<OpenVisionRecipeBranchOutputComparisonRow>
            {
                CreateBranchOutputRow(
                    OpenVisionRecipeText.Local("선택", "Selected"),
                    selected,
                    OpenVisionRecipeText.Local("수정 대상 출력", "Correction target output"))
            };

            foreach (OpenVisionRecipePipelineStepPreview producer in steps
                .Where(step => step != null
                    && step.Index != selected.Index
                    && string.Equals(step.OutputLayer, selected.InputLayer, StringComparison.OrdinalIgnoreCase))
                .OrderBy(step => step.Index))
            {
                rows.Add(CreateBranchOutputRow(
                    OpenVisionRecipeText.Local("입력 생성", "Input producer"),
                    producer,
                    OpenVisionRecipeText.Local("선택 Step 입력을 만듦", "Feeds selected input")));
            }

            foreach (OpenVisionRecipePipelineStepPreview producer in steps
                .Where(step => step != null
                    && step.Index != selected.Index
                    && UsesDeclaredSourceLayer(selected, step.OutputLayer)
                    && !string.Equals(step.OutputLayer, selected.InputLayer, StringComparison.OrdinalIgnoreCase))
                .OrderBy(step => step.Index))
            {
                rows.Add(CreateBranchOutputRelationRow(
                    OpenVisionRecipeText.Local("오버레이 소스", "Overlay source"),
                    producer,
                    producer.OutputLayer + " -> " + selected.OutputLayer,
                    OpenVisionRecipeText.Local("선택 검토 출력에 병합", "Merged into selected review")));
            }

            foreach (OpenVisionRecipePipelineStepPreview consumer in steps
                .Where(step => step != null
                    && step.Index != selected.Index
                    && ConsumesOutputLayer(step, selected.OutputLayer))
                .OrderBy(step => step.Index))
            {
                if (UsesDeclaredSourceLayer(consumer, selected.OutputLayer))
                {
                    rows.Add(CreateBranchOutputRelationRow(
                        OpenVisionRecipeText.Local("검토 병합", "Review merge"),
                        consumer,
                        selected.OutputLayer + " -> " + consumer.OutputLayer,
                        OpenVisionRecipeText.Local("검토 출력에 병합", "Merged into review output")));
                }
                else
                {
                    rows.Add(CreateBranchOutputRow(
                        OpenVisionRecipeText.Local("출력 소비", "Output consumer"),
                        consumer,
                    OpenVisionRecipeText.Local("선택 출력 이후 영향", "Affected after selected output")));
                }
            }

            foreach (OpenVisionRecipePipelineStepPreview branch in steps
                .Where(step => step != null
                    && step.Index != selected.Index
                    && !HasDeclaredSourceLayers(selected)
                    && !HasDeclaredSourceLayers(step)
                    && string.Equals(step.InputLayer, selected.InputLayer, StringComparison.OrdinalIgnoreCase))
                .OrderBy(step => step.Index))
            {
                rows.Add(CreateBranchOutputRow(
                    OpenVisionRecipeText.Local("같은 입력", "Same input"),
                    branch,
                    OpenVisionRecipeText.Local("대체 출력 후보", "Alternative output candidate")));
            }

            if (rows.Count == 1)
            {
                rows.Add(new OpenVisionRecipeBranchOutputComparisonRow(
                    OpenVisionRecipeText.Local("단일 경로", "Single path"),
                    "-",
                    selected.OutputLayer,
                    OpenVisionRecipeText.Local("분기/소비 Step 없음", "No branch or consumer step")));
            }

            return rows;
        }

        private static OpenVisionRecipeBranchOutputComparisonRow CreateBranchOutputRow(
            string status,
            OpenVisionRecipePipelineStepPreview step,
            string action)
        {
            return new OpenVisionRecipeBranchOutputComparisonRow(
                status,
                step == null ? "-" : step.Index.ToString(CultureInfo.InvariantCulture) + ". " + step.Name,
                step == null ? "-" : step.InputLayer + " -> " + step.OutputLayer,
                action);
        }

        private static OpenVisionRecipeBranchOutputComparisonRow CreateBranchOutputRelationRow(
            string status,
            OpenVisionRecipePipelineStepPreview step,
            string route,
            string action)
        {
            return new OpenVisionRecipeBranchOutputComparisonRow(
                status,
                step == null ? "-" : step.Index.ToString(CultureInfo.InvariantCulture) + ". " + step.Name,
                string.IsNullOrWhiteSpace(route) ? "-" : route,
                action);
        }

        private static bool HasDeclaredSourceLayers(OpenVisionRecipePipelineStepPreview step)
        {
            return step?.SourceLayers != null && step.SourceLayers.Count > 0;
        }

        private static bool UsesDeclaredSourceLayer(OpenVisionRecipePipelineStepPreview step, string layerName)
        {
            return !string.IsNullOrWhiteSpace(layerName)
                && step?.SourceLayers != null
                && step.SourceLayers.Any(source => string.Equals(source, layerName, StringComparison.OrdinalIgnoreCase));
        }

        private static bool ConsumesOutputLayer(OpenVisionRecipePipelineStepPreview step, string outputLayer)
        {
            return step != null
                && !string.IsNullOrWhiteSpace(outputLayer)
                && (string.Equals(step.InputLayer, outputLayer, StringComparison.OrdinalIgnoreCase)
                    || UsesDeclaredSourceLayer(step, outputLayer));
        }

        internal static string BuildStepSlotText(OpenVisionRecipePipelineStepPreview step, string emptyText)
        {
            if (step == null)
            {
                return emptyText ?? string.Empty;
            }

            return step.Index.ToString(CultureInfo.InvariantCulture)
                + ". "
                + step.Name
                + " / "
                + step.ToolType
                + " | "
                + step.InputLayer
                + " -> "
                + step.OutputLayer;
        }
    }
}
