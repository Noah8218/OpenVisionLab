using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OpenVisionLab
{
    /// <summary>
    /// Owns Product catalog selection and execution/storage for Catalog benchmark workflows.
    /// </summary>
    internal sealed class OpenVisionRecipeCatalogExecutionOwner
    {
        public List<VisionPipelineSampleCatalogItem> GetBenchmarkSamples()
        {
            return VisionPipelineSampleCatalogItem.LoadRunnable(VisionPipelineSampleCatalogSourceKind.Product)
                .Where(sample => sample != null
                    && sample.CanOpen
                    && !string.IsNullOrWhiteSpace(sample.ImageFullPath)
                    && File.Exists(sample.ImageFullPath))
                .OrderBy(sample => string.IsNullOrWhiteSpace(sample.PairGroup) ? "~" : sample.PairGroup.Trim(), StringComparer.OrdinalIgnoreCase)
                .ThenBy(sample => sample.SampleName, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        public async Task<(IReadOnlyList<VisionPipelineBatchSampleRunResult> Results, string SummaryPath)> RunAsync(
            string recipeName,
            string pipelineName,
            IReadOnlyList<VisionPipelineSampleCatalogItem> samples,
            DateTime startedAt,
            Action<int, int, IReadOnlyList<VisionPipelineBatchSampleRunResult>> reportProgress)
        {
            string pipelineXmlText = LoadPipelineXml(recipeName, pipelineName);
            List<VisionPipelineBatchSampleRunResult> storageResults = new List<VisionPipelineBatchSampleRunResult>();
            for (int index = 0; index < samples.Count; index++)
            {
                VisionPipelineSampleCatalogItem sample = samples[index];
                VisionPipelineSampleCheckResult result =
                    await VisionPipelineSampleCheckService.RunSampleCheckWithReportSafeAsync(sample, pipelineXmlText, recipeName);

                storageResults.Add(OpenVisionRecipeValidationRunSupport.CreateBatchSampleRunResult(sample, result, FormatBenchmarkMessage(result)));

                if ((index + 1) == samples.Count || (index + 1) % 10 == 0)
                {
                    reportProgress(index + 1, samples.Count, storageResults);
                }
            }

            string summaryPath = VisionPipelineBatchRunSummaryStorage.Save(
                recipeName,
                pipelineName,
                startedAt,
                DateTime.Now,
                storageResults,
                "Catalog",
                "Catalog");
            return (storageResults, summaryPath);
        }

        private static string LoadPipelineXml(string recipeName, string pipelineName)
        {
            string pipelinePath = RecipeWorkspaceService.GetVisionPipelinePath(recipeName, pipelineName);
            return File.ReadAllText(pipelinePath);
        }

        private static string FormatBenchmarkMessage(VisionPipelineSampleCheckResult result)
        {
            if (result == null)
            {
                return string.Empty;
            }

            List<string> parts = new List<string>();
            if (!string.IsNullOrWhiteSpace(result.Message))
            {
                parts.Add(result.Message.Trim());
            }

            if (!string.IsNullOrWhiteSpace(result.MetricText))
            {
                parts.Add(result.MetricText.Trim());
            }

            return string.Join(" | ", parts);
        }
    }
}
