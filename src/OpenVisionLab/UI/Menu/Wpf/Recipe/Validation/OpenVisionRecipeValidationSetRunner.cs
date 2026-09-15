using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OpenVisionLab
{
    internal sealed class OpenVisionRecipeValidationSetRunRequest
    {
        internal OpenVisionRecipeValidationSetRunRequest(
            string recipeName,
            string pipelineName,
            string pipelinePath,
            string setName,
            string setNotes,
            OpenVisionRecipeValidationSet validationSet,
            IReadOnlyList<OpenVisionRecipeValidationSetImage> images,
            Func<bool> stopRequested,
            Action<int, int> progress)
        {
            RecipeName = recipeName ?? string.Empty;
            PipelineName = pipelineName ?? string.Empty;
            PipelinePath = pipelinePath ?? string.Empty;
            SetName = setName ?? string.Empty;
            SetNotes = setNotes ?? string.Empty;
            ValidationSet = validationSet;
            Images = images ?? Array.Empty<OpenVisionRecipeValidationSetImage>();
            StopRequested = stopRequested ?? (() => false);
            Progress = progress;
        }

        internal string RecipeName { get; }

        internal string PipelineName { get; }

        internal string PipelinePath { get; }

        internal string SetName { get; }

        internal string SetNotes { get; }

        internal OpenVisionRecipeValidationSet ValidationSet { get; }

        internal IReadOnlyList<OpenVisionRecipeValidationSetImage> Images { get; }

        internal Func<bool> StopRequested { get; }

        internal Action<int, int> Progress { get; }
    }

    internal sealed class OpenVisionRecipeValidationSetRunResult
    {
        internal OpenVisionRecipeValidationSetRunResult(
            int completedCount,
            int totalCount,
            int correctCount,
            bool isPartial,
            string summaryPath)
        {
            CompletedCount = completedCount;
            TotalCount = totalCount;
            CorrectCount = correctCount;
            IsPartial = isPartial;
            SummaryPath = summaryPath ?? string.Empty;
        }

        internal int CompletedCount { get; }

        internal int TotalCount { get; }

        internal int CorrectCount { get; }

        internal bool IsPartial { get; }

        internal string SummaryPath { get; }
    }

    internal sealed class OpenVisionRecipeValidationSetRunner
    {
        internal OpenVisionRecipeValidationSetRunRequest CreateRunRequest(
            string recipeName,
            string pipelineName,
            OpenVisionRecipeValidationSetOption option,
            Func<bool> stopRequested,
            Action<int, int> progress)
        {
            string setName = option.Name;
            string setNotes = option.Set.Notes ?? string.Empty;
            List<OpenVisionRecipeValidationSetImage> images = option.Set.Images
                .Where(image => image != null)
                .Select(image => new OpenVisionRecipeValidationSetImage
                {
                    Expected = image.Expected,
                    Path = image.Path,
                    Notes = image.Notes,
                    VariantId = image.VariantId,
                    ExpectedMetricName = image.ExpectedMetricName,
                    ExpectedMetricMinimum = image.ExpectedMetricMinimum,
                    ExpectedMetricMaximum = image.ExpectedMetricMaximum
                })
                .ToList();
            string pipelinePath = RecipeWorkspaceService.GetVisionPipelinePath(recipeName, pipelineName);
            return new OpenVisionRecipeValidationSetRunRequest(
                recipeName,
                pipelineName,
                pipelinePath,
                setName,
                setNotes,
                option.Set,
                images,
                stopRequested,
                progress);
        }

        internal async Task<OpenVisionRecipeValidationSetRunResult> RunAsync(
            OpenVisionRecipeValidationSetRunRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            string pipelineXmlText = File.ReadAllText(request.PipelinePath);
            if (!OpenVisionRecipeValidationSetStorage.TryValidateFrozenIdentity(
                    request.ValidationSet,
                    request.PipelineName,
                    pipelineXmlText,
                    out string identityError))
            {
                throw new InvalidDataException(identityError);
            }

            DateTime startedAt = DateTime.Now;
            List<VisionPipelineBatchSampleRunResult> storageResults = new List<VisionPipelineBatchSampleRunResult>();
            for (int index = 0; index < request.Images.Count; index++)
            {
                if (request.StopRequested())
                {
                    break;
                }

                OpenVisionRecipeValidationSetImage image = request.Images[index];
                VisionPipelineSampleCatalogItem sample = OpenVisionRecipeValidationRunSupport.CreateLocalValidationSample(
                    request.SetName,
                    image,
                    index);
                VisionPipelineSampleCheckResult result =
                    await VisionPipelineSampleCheckService.RunSampleCheckWithReportSafeAsync(
                        sample,
                        pipelineXmlText,
                        request.RecipeName);
                VisionPipelineBatchSampleRunResult storageResult =
                    OpenVisionRecipeValidationRunSupport.CreateBatchSampleRunResult(sample, result);
                storageResult.VariantId = OpenVisionRecipeValidationSetStorage.GetVariantDisplayId(image);
                storageResult.ExpectedMetricName = image.ExpectedMetricName ?? string.Empty;
                storageResult.ExpectedMetricMinimum = image.ExpectedMetricMinimum ?? string.Empty;
                storageResult.ExpectedMetricMaximum = image.ExpectedMetricMaximum ?? string.Empty;
                storageResult.ExpectedText = "ExpectedActual: Expected "
                    + image.Expected
                    + " | Variant "
                    + storageResult.VariantId
                    + (string.IsNullOrWhiteSpace(image.ExpectedMetricName)
                        ? string.Empty
                        : " | " + OpenVisionRecipeValidationSetStorage.BuildExpectedMetricText(image));
                VisionPipelineBatchOutcomeContract.Apply(
                    storageResult,
                    result?.ExecutionCompleted == true,
                    result?.ActualSuccess == true,
                    hasJudgment: true,
                    expectedSuccess: !image.IsExpectedNg,
                    judgmentCorrect: result?.Success == true);
                if (!string.IsNullOrWhiteSpace(image.Notes))
                {
                    storageResult.Message = string.IsNullOrWhiteSpace(storageResult.Message)
                        ? "Note: " + image.Notes
                        : storageResult.Message + " | Note: " + image.Notes;
                }

                storageResults.Add(storageResult);
                request.Progress?.Invoke(index + 1, request.Images.Count);
            }

            bool isPartial = request.StopRequested() && storageResults.Count < request.Images.Count;
            string savedNotes = isPartial
                ? OpenVisionRecipeValidationRunSupport.AppendPartialValidationSetNote(
                    request.SetNotes,
                    storageResults.Count,
                    request.Images.Count)
                : request.SetNotes;
            string summaryPath = VisionPipelineBatchRunSummaryStorage.Save(
                request.RecipeName,
                request.PipelineName,
                startedAt,
                DateTime.Now,
                storageResults,
                request.SetName,
                isPartial ? "LocalValidationSetPartial" : "LocalValidationSet",
                savedNotes,
                inputSampleCount: request.Images.Count);
            int correct = storageResults.Count(OpenVisionRecipeValidationRunSupport.IsExpectedOutcomeCorrect);
            return new OpenVisionRecipeValidationSetRunResult(
                storageResults.Count,
                request.Images.Count,
                correct,
                isPartial,
                summaryPath);
        }
    }
}
