using System;
namespace OpenVisionLab
{
    public sealed partial class OpenVisionShellHostRecipeCommandSurface
    {
        private void RefreshRecentBatchRunOptions()
        {
            string recipeName = NormalizeRecipeName(selectedRecipeName);
            string pipelineName = selectedPipelineOption?.PipelineName ?? string.Empty;
            string previousSummaryPath = SelectedRecentBatchRunOption?.SummaryPath ?? string.Empty;
            OpenVisionRecipeRunHistorySelection selection = runHistoryOrchestrationOwner.BuildRecentRunSelection(
                recipeName,
                pipelineName,
                previousSummaryPath);
            RecentBatchRunOptions = selection.Options;
            SelectedRecentBatchRunOption = selection.SelectedOption;
        }

        private void RefreshBenchmarkBaselineRunOptions()
        {
            string previousBaselinePath = selectedBenchmarkBaselineRunOption?.SummaryPath ?? string.Empty;
            OpenVisionRecipeRunHistorySelection selection = runHistoryOrchestrationOwner.BuildBaselineRunSelection(
                SelectedRecentBatchRunOption,
                RecentBatchRunOptions,
                previousBaselinePath);
            BenchmarkBaselineRunOptions = selection.Options;
            SelectedBenchmarkBaselineRunOption = selection.SelectedOption;
        }
    }
}
