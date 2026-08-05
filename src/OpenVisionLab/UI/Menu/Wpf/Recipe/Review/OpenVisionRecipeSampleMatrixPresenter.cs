using OpenVisionLab.Vision2D.Pipeline;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace OpenVisionLab
{
    // Derives read-only Good/Bad sample-matrix rows and selection priority from supplied sample/run state.
    internal static class OpenVisionRecipeSampleMatrixPresenter
    {
        internal static IReadOnlyList<OpenVisionRecipeSampleMatrixRow> BuildRows(
            VisionPipelineSampleCatalogItem selectedSample,
            OpenVisionRecipePairRunSummary pairRun)
        {
            if (selectedSample == null)
            {
                return new[] { OpenVisionRecipeSampleMatrixRow.CreateEmpty() };
            }

            List<VisionPipelineSampleCatalogItem> samples = VisionPipelineSampleCheckService.GetPairSamples(selectedSample);
            if (samples.Count == 0)
            {
                samples.Add(selectedSample);
            }

            Dictionary<string, OpenVisionRecipePairSampleRunSummary> resultsBySample =
                (pairRun?.SampleResults ?? Array.Empty<OpenVisionRecipePairSampleRunSummary>())
                .Where(result => result != null && !string.IsNullOrWhiteSpace(result.SampleName))
                .GroupBy(result => result.SampleName.Trim(), StringComparer.OrdinalIgnoreCase)
                .ToDictionary(group => group.Key, group => group.First(), StringComparer.OrdinalIgnoreCase);

            return samples
                .Select(sample =>
                {
                    resultsBySample.TryGetValue(sample.SampleName ?? string.Empty, out OpenVisionRecipePairSampleRunSummary result);
                    return OpenVisionRecipeSampleMatrixRow.Create(sample, result);
                })
                .ToList();
        }

        internal static OpenVisionRecipeSampleMatrixRow SelectDefaultRow(
            IReadOnlyList<OpenVisionRecipeSampleMatrixRow> rows,
            OpenVisionRecipeSampleMatrixRow previous)
        {
            if (rows == null || rows.Count == 0)
            {
                return null;
            }

            if (!string.IsNullOrWhiteSpace(previous?.SampleName))
            {
                OpenVisionRecipeSampleMatrixRow sameSample = rows.FirstOrDefault(row =>
                    string.Equals(row.SampleName, previous.SampleName, StringComparison.OrdinalIgnoreCase));
                if (sameSample != null)
                {
                    return sameSample;
                }
            }

            return rows.FirstOrDefault(row => row.HasResult && !row.Success)
                ?? rows.FirstOrDefault(row => !row.HasResult)
                ?? rows[0];
        }

        internal static string BuildSummaryText(
            IReadOnlyList<OpenVisionRecipeSampleMatrixRow> rows,
            VisionPipelineSampleCatalogItem selectedSample)
        {
            rows = rows ?? Array.Empty<OpenVisionRecipeSampleMatrixRow>();
            int runnableRows = rows.Count(row => row != null && !row.IsPlaceholder);
            if (runnableRows == 0)
            {
                return OpenVisionRecipeText.Local(
                    "샘플을 선택하면 Good/Bad 매트릭스가 표시됩니다.",
                    "Select a sample to show the Good/Bad matrix.");
            }

            int completed = rows.Count(row => row != null && !row.IsPlaceholder && row.HasResult);
            int pass = rows.Count(row => row != null && !row.IsPlaceholder && row.HasResult && row.Success);
            int fail = rows.Count(row => row != null && !row.IsPlaceholder && row.HasResult && !row.Success);
            string group = string.IsNullOrWhiteSpace(selectedSample?.PairGroup)
                ? "-"
                : selectedSample.PairGroup.Trim();

            return "PairGroup " + group
                + " | "
                + OpenVisionRecipeText.Local("행 ", "Rows ")
                + runnableRows.ToString(CultureInfo.InvariantCulture)
                + " | "
                + OpenVisionRecipeText.Local("실행 ", "Run ")
                + completed.ToString(CultureInfo.InvariantCulture)
                + "/"
                + runnableRows.ToString(CultureInfo.InvariantCulture)
                + " | OK "
                + pass.ToString(CultureInfo.InvariantCulture)
                + " / NG "
                + fail.ToString(CultureInfo.InvariantCulture);
        }
    }
}
