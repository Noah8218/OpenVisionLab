using OpenVisionLab.Vision2D.Pipeline;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace OpenVisionLab
{
    // Formats read-only LLM draft and pipeline-variant comparison evidence from supplied pipelines.
    internal static class OpenVisionRecipePipelineComparisonPresenter
    {
        internal static string BuildDraftImportReview(VisionPipeline activePipeline, VisionPipeline draftPipeline)
        {
            if (draftPipeline == null)
            {
                return OpenVisionRecipeText.Local("초안 가져오기 검토: NG - 파이프라인이 없습니다.", "Draft import review: NG - pipeline is null.");
            }

            List<string> lines = new List<string>
            {
                OpenVisionRecipeText.Local("초안 가져오기 검토: 준비됨", "Draft import review: READY"),
                OpenVisionRecipeText.Local("가져오기 동작: 새 고유 파이프라인으로 저장하고 활성화하며 Preview는 실행하지 않습니다.", "Import action: save as a new/unique pipeline, activate it, do not run Preview."),
                OpenVisionRecipeText.Local("현재 활성: ", "Current active: ") + FormatPipelineHeader(activePipeline),
                OpenVisionRecipeText.Local("초안: ", "Draft: ") + FormatPipelineHeader(draftPipeline),
                OpenVisionRecipeText.Local("단계 수 변화: ", "Step count delta: ") + FormatSignedNumber((draftPipeline.Steps?.Count ?? 0) - (activePipeline.Steps?.Count ?? 0)),
                OpenVisionRecipeText.Local("초안 의존 경로 수: ", "Draft dependency paths: ") + CountDependencyParameters(draftPipeline).ToString(CultureInfo.InvariantCulture)
            };

            int activeCount = activePipeline?.Steps?.Count ?? 0;
            int draftCount = draftPipeline?.Steps?.Count ?? 0;
            int compareCount = Math.Min(Math.Max(activeCount, draftCount), 6);
            for (int index = 0; index < compareCount; index++)
            {
                VisionPipelineStep activeStep = index < activeCount ? activePipeline.Steps[index] : null;
                VisionPipelineStep draftStep = index < draftCount ? draftPipeline.Steps[index] : null;
                lines.Add(OpenVisionRecipeText.Local("단계 ", "Step ") + (index + 1).ToString(CultureInfo.InvariantCulture) + ": " + FormatStepDiff(activeStep, draftStep));
            }

            if (Math.Max(activeCount, draftCount) > compareCount)
            {
                lines.Add(OpenVisionRecipeText.Local("검토에서 생략된 추가 단계: ", "More steps omitted from review: ")
                    + (Math.Max(activeCount, draftCount) - compareCount).ToString(CultureInfo.InvariantCulture));
            }

            return string.Join(Environment.NewLine, lines);
        }

        internal static string BuildDraftDiffReview(VisionPipeline activePipeline, VisionPipeline draftPipeline)
        {
            if (draftPipeline == null)
            {
                return OpenVisionRecipeText.Local("LLM XML 변경점: NG - 파이프라인이 없습니다.", "LLM XML diff review: NG - pipeline is null.");
            }

            return BuildPipelineDiffReport(
                activePipeline,
                draftPipeline,
                OpenVisionRecipeText.Local("LLM XML 변경점: 준비됨", "LLM XML diff review: READY"),
                OpenVisionRecipeText.Local("비교 기준: ", "Baseline: "),
                OpenVisionRecipeText.Local("초안: ", "Draft: "));
        }

        internal static string BuildVariantComparison(
            VisionPipeline activePipeline,
            VisionPipeline selectedPipeline,
            bool hasSelectedPipeline,
            bool selectedIsActive)
        {
            if (!hasSelectedPipeline)
            {
                return OpenVisionRecipeText.Local(
                    "비교할 파이프라인 변형을 선택하세요.",
                    "Select a pipeline variant to compare.");
            }

            if (selectedIsActive)
            {
                return string.Join(
                    Environment.NewLine,
                    OpenVisionRecipeText.Local("변형 비교: 활성 파이프라인이 선택되어 있습니다.", "Variant comparison: the active pipeline is selected."),
                    OpenVisionRecipeText.Local("활성/선택: ", "Active/selected: ") + FormatPipelineHeader(activePipeline),
                    OpenVisionRecipeText.Local("선택만으로 Preview/Run은 실행되지 않습니다.", "Selection alone does not run Preview/Run."));
            }

            return BuildPipelineDiffReport(
                    activePipeline,
                    selectedPipeline,
                    OpenVisionRecipeText.Local("변형 비교: 준비됨", "Variant comparison: READY"),
                    OpenVisionRecipeText.Local("활성 기준: ", "Active baseline: "),
                    OpenVisionRecipeText.Local("선택 변형: ", "Selected variant: "))
                + Environment.NewLine
                + OpenVisionRecipeText.Local("이 비교는 읽기 전용이며 활성화 또는 Preview/Run을 실행하지 않습니다.", "This review is read-only and does not activate or run Preview/Run.");
        }

        private static string BuildPipelineDiffReport(
            VisionPipeline baselinePipeline,
            VisionPipeline candidatePipeline,
            string title,
            string baselineLabel,
            string candidateLabel)
        {
            IReadOnlyList<VisionPipelineStep> baselineSteps = baselinePipeline?.Steps != null
                ? (IReadOnlyList<VisionPipelineStep>)baselinePipeline.Steps
                : Array.Empty<VisionPipelineStep>();
            IReadOnlyList<VisionPipelineStep> candidateSteps = candidatePipeline?.Steps != null
                ? (IReadOnlyList<VisionPipelineStep>)candidatePipeline.Steps
                : Array.Empty<VisionPipelineStep>();

            List<string> added = new List<string>();
            List<string> removed = new List<string>();
            List<string> changed = new List<string>();
            int compareCount = Math.Max(baselineSteps.Count, candidateSteps.Count);
            for (int index = 0; index < compareCount; index++)
            {
                VisionPipelineStep baselineStep = index < baselineSteps.Count ? baselineSteps[index] : null;
                VisionPipelineStep candidateStep = index < candidateSteps.Count ? candidateSteps[index] : null;
                string label = (index + 1).ToString(CultureInfo.InvariantCulture);
                if (baselineStep == null && candidateStep != null)
                {
                    added.Add(label + ". " + FormatStepBrief(candidateStep));
                    continue;
                }

                if (baselineStep != null && candidateStep == null)
                {
                    removed.Add(label + ". " + FormatStepBrief(baselineStep));
                    continue;
                }

                string stepDiff = FormatDetailedStepDiff(baselineStep, candidateStep);
                if (!string.IsNullOrWhiteSpace(stepDiff))
                {
                    changed.Add(label + ". " + stepDiff);
                }
            }

            List<string> lines = new List<string>
            {
                title,
                baselineLabel + FormatPipelineHeader(baselinePipeline),
                candidateLabel + FormatPipelineHeader(candidatePipeline),
                OpenVisionRecipeText.Local("단계 수 변화: ", "Step count delta: ") + FormatSignedNumber(candidateSteps.Count - baselineSteps.Count),
                OpenVisionRecipeText.Local("의존 경로 수 변화: ", "Dependency path delta: ") + FormatSignedNumber(CountDependencyParameters(candidatePipeline) - CountDependencyParameters(baselinePipeline)),
                string.Format(
                    CultureInfo.CurrentCulture,
                    OpenVisionRecipeText.Local("변경 요약: 추가 {0}, 삭제 {1}, 수정 {2}", "Change summary: added {0}, removed {1}, changed {2}"),
                    added.Count,
                    removed.Count,
                    changed.Count)
            };

            AddLimitedDiffLines(lines, OpenVisionRecipeText.Local("추가 단계", "Added steps"), added);
            AddLimitedDiffLines(lines, OpenVisionRecipeText.Local("삭제 예정 단계", "Removed steps"), removed);
            AddLimitedDiffLines(lines, OpenVisionRecipeText.Local("수정 단계", "Changed steps"), changed);
            if (added.Count == 0 && removed.Count == 0 && changed.Count == 0)
            {
                lines.Add(OpenVisionRecipeText.Local("구조/파라미터 변경 없음.", "No step structure or parameter changes detected."));
            }

            return string.Join(Environment.NewLine, lines);
        }

        private static void AddLimitedDiffLines(ICollection<string> lines, string title, IReadOnlyList<string> items)
        {
            if (items == null || items.Count == 0)
            {
                return;
            }

            lines.Add(title + ":");
            foreach (string item in items.Take(4))
            {
                lines.Add("  - " + item);
            }

            if (items.Count > 4)
            {
                lines.Add("  - ... +" + (items.Count - 4).ToString(CultureInfo.InvariantCulture));
            }
        }

        private static string FormatDetailedStepDiff(VisionPipelineStep activeStep, VisionPipelineStep draftStep)
        {
            if (activeStep == null || draftStep == null)
            {
                return string.Empty;
            }

            List<string> parts = new List<string>();
            string structureDiff = FormatStepDiff(activeStep, draftStep);
            if (!structureDiff.StartsWith(OpenVisionRecipeText.Local("구조 변경 없음", "No structural change"), StringComparison.Ordinal))
            {
                parts.Add(structureDiff);
            }

            string parameterDiff = FormatParameterDiff(activeStep.Parameters, draftStep.Parameters);
            if (!string.IsNullOrWhiteSpace(parameterDiff))
            {
                parts.Add(parameterDiff);
            }

            return string.Join("; ", parts);
        }

        private static string FormatParameterDiff(IDictionary<string, string> activeParameters, IDictionary<string, string> draftParameters)
        {
            IDictionary<string, string> active = activeParameters ?? new Dictionary<string, string>();
            IDictionary<string, string> draft = draftParameters ?? new Dictionary<string, string>();
            List<string> changedKeys = active.Keys
                .Concat(draft.Keys)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(item => item, StringComparer.OrdinalIgnoreCase)
                .Where(key =>
                {
                    active.TryGetValue(key, out string activeValue);
                    draft.TryGetValue(key, out string draftValue);
                    return !string.Equals(activeValue ?? string.Empty, draftValue ?? string.Empty, StringComparison.Ordinal);
                })
                .ToList();

            if (changedKeys.Count == 0)
            {
                return string.Empty;
            }

            List<string> details = changedKeys.Take(4)
                .Select(key =>
                {
                    active.TryGetValue(key, out string activeValue);
                    draft.TryGetValue(key, out string draftValue);
                    return key + " " + FormatValue(activeValue) + " -> " + FormatValue(draftValue);
                })
                .ToList();
            if (changedKeys.Count > details.Count)
            {
                details.Add("... +" + (changedKeys.Count - details.Count).ToString(CultureInfo.InvariantCulture));
            }

            return OpenVisionRecipeText.Local("파라미터 변경: ", "Parameter changes: ") + string.Join(", ", details);
        }

        private static string FormatPipelineHeader(VisionPipeline pipeline)
        {
            if (pipeline == null)
            {
                return "- / 0 step(s)";
            }

            return (string.IsNullOrWhiteSpace(pipeline.Name) ? "-" : pipeline.Name)
                + " / "
                + (pipeline.Steps?.Count ?? 0).ToString(CultureInfo.InvariantCulture)
                + " "
                + OpenVisionRecipeText.Local("단계", "step(s)");
        }

        private static string FormatSignedNumber(int value)
        {
            return value > 0
                ? "+" + value.ToString(CultureInfo.InvariantCulture)
                : value.ToString(CultureInfo.InvariantCulture);
        }

        private static string FormatStepDiff(VisionPipelineStep activeStep, VisionPipelineStep draftStep)
        {
            if (activeStep == null && draftStep == null)
            {
                return "-";
            }

            if (activeStep == null)
            {
                return OpenVisionRecipeText.Local("새 단계", "New") + " -> " + FormatStepBrief(draftStep);
            }

            if (draftStep == null)
            {
                return OpenVisionRecipeText.Local("초안에서 제거됨", "Removed from draft") + " -> " + FormatStepBrief(activeStep);
            }

            List<string> changes = new List<string>();
            if (!string.Equals(activeStep.ToolType, draftStep.ToolType, StringComparison.OrdinalIgnoreCase))
            {
                changes.Add(OpenVisionRecipeText.Local("도구 ", "tool ") + FormatValue(activeStep.ToolType) + " -> " + FormatValue(draftStep.ToolType));
            }

            string activeRoute = FormatRoute(activeStep);
            string draftRoute = FormatRoute(draftStep);
            if (!string.Equals(activeRoute, draftRoute, StringComparison.OrdinalIgnoreCase))
            {
                changes.Add(OpenVisionRecipeText.Local("경로 ", "route ") + activeRoute + " -> " + draftRoute);
            }

            if (!string.Equals(activeStep.Name, draftStep.Name, StringComparison.OrdinalIgnoreCase))
            {
                changes.Add(OpenVisionRecipeText.Local("이름 ", "name ") + FormatValue(activeStep.Name) + " -> " + FormatValue(draftStep.Name));
            }

            return changes.Count == 0
                ? OpenVisionRecipeText.Local("구조 변경 없음", "No structural change") + " -> " + FormatStepBrief(draftStep)
                : string.Join("; ", changes);
        }

        private static string FormatStepBrief(VisionPipelineStep step)
        {
            if (step == null)
            {
                return "-";
            }

            return FormatValue(step.Name) + " / " + FormatValue(step.ToolType) + " / " + FormatRoute(step);
        }

        private static string FormatRoute(VisionPipelineStep step)
        {
            if (step == null)
            {
                return "-";
            }

            return FormatValue(step.InputLayer) + " -> " + FormatValue(step.OutputLayer);
        }

        private static string FormatValue(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? "-" : value.Trim();
        }

        private static int CountDependencyParameters(VisionPipeline pipeline)
        {
            if (pipeline?.Steps == null)
            {
                return 0;
            }

            int count = 0;
            foreach (VisionPipelineStep step in pipeline.Steps)
            {
                if (step?.Parameters == null)
                {
                    continue;
                }

                foreach (KeyValuePair<string, string> parameter in step.Parameters)
                {
                    if (OpenVisionRecipeDependencyReviewService.LooksLikeDependencyPath(parameter.Key, parameter.Value))
                    {
                        count++;
                    }
                }
            }

            return count;
        }

    }
}
