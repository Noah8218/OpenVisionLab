using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenVisionLab
{
    public sealed partial class OpenVisionShellHostRecipeCommandSurface
    {
        private void RefreshRecentBatchRunOptions()
        {
            string recipeName = NormalizeRecipeName(selectedRecipeName);
            string pipelineName = selectedPipelineOption?.PipelineName ?? string.Empty;
            string previousSummaryPath = SelectedRecentBatchRunOption?.SummaryPath ?? string.Empty;
            OpenVisionRecipeRunHistorySelection selection = OpenVisionRecipeRunHistoryPresenter.BuildRecentRunSelection(
                VisionPipelineBatchRunSummaryStorage
                .List(recipeName, pipelineName)
                .Select(OpenVisionRecipeBatchRunOption.Create)
                .ToList(),
                previousSummaryPath);
            RecentBatchRunOptions = selection.Options;
            SelectedRecentBatchRunOption = selection.SelectedOption;
        }

        private void RefreshBenchmarkBaselineRunOptions()
        {
            string previousBaselinePath = selectedBenchmarkBaselineRunOption?.SummaryPath ?? string.Empty;
            OpenVisionRecipeRunHistorySelection selection = OpenVisionRecipeRunHistoryPresenter.BuildBaselineRunSelection(
                SelectedRecentBatchRunOption,
                RecentBatchRunOptions,
                previousBaselinePath);
            BenchmarkBaselineRunOptions = selection.Options;
            SelectedBenchmarkBaselineRunOption = selection.SelectedOption;
        }
    }
}
