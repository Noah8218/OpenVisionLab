using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenVisionLab
{
    // Owns persisted Run History lookup and comparison assembly; the Shell keeps binding state and notifications.
    internal sealed class OpenVisionRecipeRunHistoryOrchestrationOwner
    {
        internal OpenVisionRecipeRunHistorySelection BuildRecentRunSelection(
            string recipeName,
            string pipelineName,
            string previousSummaryPath)
        {
            IReadOnlyList<OpenVisionRecipeBatchRunOption> runs = VisionPipelineBatchRunSummaryStorage
                .List(recipeName, pipelineName)
                .Select(OpenVisionRecipeBatchRunOption.Create)
                .ToList();
            return OpenVisionRecipeRunHistoryPresenter.BuildRecentRunSelection(runs, previousSummaryPath);
        }

        internal OpenVisionRecipeRunHistorySelection BuildBaselineRunSelection(
            OpenVisionRecipeBatchRunOption current,
            IReadOnlyList<OpenVisionRecipeBatchRunOption> recentRuns,
            string previousBaselinePath)
        {
            return OpenVisionRecipeRunHistoryPresenter.BuildBaselineRunSelection(
                current,
                recentRuns,
                previousBaselinePath);
        }

        internal OpenVisionRecipeRunHistoryComparison BuildComparison(
            OpenVisionRecipeBatchRunOption current,
            OpenVisionRecipeBatchRunOption selectedBaseline,
            IReadOnlyList<OpenVisionRecipeBatchRunOption> recentRuns)
        {
            OpenVisionRecipeBatchRunOption baseline = OpenVisionRecipeRunHistoryPresenter.ResolveBaselineRunOption(
                selectedBaseline,
                current,
                recentRuns);
            VisionPipelineBatchRunSummary currentSummary = null;
            VisionPipelineBatchRunSummary baselineSummary = null;
            if (current != null
                && baseline != null
                && !string.IsNullOrWhiteSpace(current.SummaryPath)
                && !string.IsNullOrWhiteSpace(baseline.SummaryPath))
            {
                currentSummary = VisionPipelineBatchRunSummaryStorage.Load(current.SummaryPath);
                baselineSummary = VisionPipelineBatchRunSummaryStorage.Load(baseline.SummaryPath);
            }

            IReadOnlyList<OpenVisionRecipeBatchRunComparisonRow> rows =
                OpenVisionRecipeRunHistoryPresenter.BuildComparisonRows(
                    current,
                    baseline,
                    currentSummary,
                    baselineSummary);
            return new OpenVisionRecipeRunHistoryComparison(
                baseline,
                rows,
                OpenVisionRecipeRunHistoryPresenter.SelectDefaultComparisonRow(rows));
        }
    }

    internal sealed class OpenVisionRecipeRunHistoryComparison
    {
        internal OpenVisionRecipeRunHistoryComparison(
            OpenVisionRecipeBatchRunOption baseline,
            IReadOnlyList<OpenVisionRecipeBatchRunComparisonRow> rows,
            OpenVisionRecipeBatchRunComparisonRow selectedRow)
        {
            Baseline = baseline;
            Rows = rows ?? Array.Empty<OpenVisionRecipeBatchRunComparisonRow>();
            SelectedRow = selectedRow;
        }

        internal OpenVisionRecipeBatchRunOption Baseline { get; }

        internal IReadOnlyList<OpenVisionRecipeBatchRunComparisonRow> Rows { get; }

        internal OpenVisionRecipeBatchRunComparisonRow SelectedRow { get; }
    }
}
