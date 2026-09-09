using System;
using System.IO;
using System.Threading.Tasks;

namespace OpenVisionLab
{
    /// <summary>
    /// Owns pipeline XML loading and execution/storage for the selected-sample workflows.
    /// </summary>
    internal sealed class OpenVisionRecipeSelectedSampleExecutionOwner
    {
        public async Task<VisionPipelineSampleCheckResult> RunCheckAsync(
            string recipeName,
            string pipelineName,
            OpenVisionRecipeSampleOption sampleOption)
        {
            string pipelineXmlText = LoadPipelineXml(recipeName, pipelineName);
            return await VisionPipelineSampleCheckService.RunSampleCheckSafeAsync(
                sampleOption.Sample,
                pipelineXmlText);
        }

        public async Task<(VisionPipelineSampleCheckResult Result, string SummaryPath)> RunSuiteAsync(
            string recipeName,
            string pipelineName,
            OpenVisionRecipeSampleOption sampleOption,
            DateTime startedAt)
        {
            string pipelineXmlText = LoadPipelineXml(recipeName, pipelineName);
            VisionPipelineSampleCheckResult result =
                await VisionPipelineSampleCheckService.RunSampleCheckWithReportSafeAsync(
                    sampleOption.Sample,
                    pipelineXmlText,
                    recipeName);
            string summaryPath = VisionPipelineBatchRunSummaryStorage.Save(
                recipeName,
                pipelineName,
                startedAt,
                DateTime.Now,
                new[] { OpenVisionRecipeValidationRunSupport.CreateBatchSampleRunResult(sampleOption.Sample, result) },
                "Selected:" + sampleOption.SampleName,
                "SelectedSample");
            return (result, summaryPath);
        }

        private static string LoadPipelineXml(string recipeName, string pipelineName)
        {
            string pipelinePath = RecipeWorkspaceService.GetVisionPipelinePath(recipeName, pipelineName);
            return File.ReadAllText(pipelinePath);
        }
    }
}
