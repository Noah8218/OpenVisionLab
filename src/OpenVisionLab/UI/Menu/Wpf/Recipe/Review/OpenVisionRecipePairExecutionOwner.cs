using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace OpenVisionLab
{
    /// <summary>
    /// Owns pipeline XML loading and execution/storage for Good/Bad pair workflows.
    /// </summary>
    internal sealed class OpenVisionRecipePairExecutionOwner
    {
        public List<VisionPipelineSampleCatalogItem> GetPairSamples(VisionPipelineSampleCatalogItem sample)
        {
            return VisionPipelineSampleCheckService.GetPairSamples(sample);
        }

        public async Task<(IReadOnlyList<OpenVisionRecipePairSampleRunSummary> Results, string SummaryPath)> RunAsync(
            string recipeName,
            string pipelineName,
            OpenVisionRecipeSampleOption sampleOption,
            IReadOnlyList<VisionPipelineSampleCatalogItem> pairSamples,
            DateTime startedAt)
        {
            string pipelineXmlText = LoadPipelineXml(recipeName, pipelineName);
            List<OpenVisionRecipePairSampleRunSummary> pairResults = new List<OpenVisionRecipePairSampleRunSummary>();
            List<VisionPipelineBatchSampleRunResult> storageResults = new List<VisionPipelineBatchSampleRunResult>();
            foreach (VisionPipelineSampleCatalogItem sample in pairSamples)
            {
                VisionPipelineSampleCheckResult result =
                    await VisionPipelineSampleCheckService.RunSampleCheckWithReportSafeAsync(sample, pipelineXmlText, recipeName);
                pairResults.Add(OpenVisionRecipePairSampleRunSummary.FromResult(sample, result));
                storageResults.Add(OpenVisionRecipeValidationRunSupport.CreateBatchSampleRunResult(sample, result));
            }

            string summaryPath = VisionPipelineBatchRunSummaryStorage.Save(
                recipeName,
                pipelineName,
                startedAt,
                DateTime.Now,
                storageResults,
                "Pair:" + (sampleOption.Sample.PairGroup ?? string.Empty),
                "GoodBadPair");
            return (pairResults, summaryPath);
        }

        private static string LoadPipelineXml(string recipeName, string pipelineName)
        {
            string pipelinePath = RecipeWorkspaceService.GetVisionPipelinePath(recipeName, pipelineName);
            return File.ReadAllText(pipelinePath);
        }
    }
}
