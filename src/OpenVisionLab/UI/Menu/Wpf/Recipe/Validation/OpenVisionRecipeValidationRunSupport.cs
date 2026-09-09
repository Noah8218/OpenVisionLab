using OpenVisionLab.Vision2D;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace OpenVisionLab
{
    internal static class OpenVisionRecipeValidationRunSupport
    {
        internal static bool IsExpectedOutcomeCorrect(VisionPipelineBatchSampleRunResult result)
        {
            return VisionPipelineBatchOutcomeContract.ResolveJudgmentCorrect(result);
        }

        internal static string AppendPartialValidationSetNote(string notes, int completed, int total)
        {
            string partial = "Partial run: completed "
                + completed.ToString(CultureInfo.InvariantCulture)
                + "/"
                + total.ToString(CultureInfo.InvariantCulture)
                + ". This is not a full-set accuracy or timing baseline.";
            return string.IsNullOrWhiteSpace(notes) ? partial : notes.Trim() + " | " + partial;
        }

        internal static VisionPipelineSampleCatalogItem CreateLocalValidationSample(
            string setName,
            OpenVisionRecipeValidationSetImage image,
            int index)
        {
            return new VisionPipelineSampleCatalogItem
            {
                SampleName = (index + 1).ToString("000", CultureInfo.InvariantCulture)
                    + " "
                    + Path.GetFileName(image.Path),
                ImagePath = image.Path,
                ImageFullPath = image.Path,
                ValidationMode = image.IsExpectedNg ? "ExpectedFailure" : "ExpectedSuccess",
                PairGroup = setName ?? string.Empty,
                PairRole = image.Expected ?? OpenVisionRecipeValidationSetImage.ExpectedOk,
                ExpectedMetricName = image.ExpectedMetricName ?? string.Empty,
                ExpectedMetricMinimum = image.ExpectedMetricMinimum ?? string.Empty,
                ExpectedMetricMaximum = image.ExpectedMetricMaximum ?? string.Empty,
                Notes = image.Notes ?? string.Empty,
                CatalogSourceKind = VisionPipelineSampleCatalogSourceKind.Unknown
            };
        }

        internal static VisionPipelineBatchSampleRunResult CreateBatchSampleRunResult(
            VisionPipelineSampleCatalogItem sample,
            VisionPipelineSampleCheckResult result,
            string messageOverride = null)
        {
            string sampleImagePath = sample?.ImageFullPath ?? string.Empty;
            VisionPipelineBatchSampleRunResult storageResult = new VisionPipelineBatchSampleRunResult
            {
                SampleName = sample?.SampleName ?? string.Empty,
                Status = result?.Status ?? string.Empty,
                Success = result?.Success ?? false,
                TotalMilliseconds = result?.TotalMilliseconds ?? 0D,
                FailedStep = result?.FailedStepText ?? string.Empty,
                Message = messageOverride ?? result?.Message ?? string.Empty,
                ReportPath = sampleImagePath,
                SampleImagePath = sampleImagePath,
                PairGroup = sample?.PairGroup ?? string.Empty,
                PairRole = sample?.PairRole ?? string.Empty,
                ExpectedText = sample?.ExpectedText ?? string.Empty,
                MetricText = result?.MetricText ?? string.Empty,
                MetricReviewText = result?.MetricReviewText ?? string.Empty,
                FinalLayer = result?.FinalLayerText ?? string.Empty,
                OverlayCount = result?.OverlayCountText ?? string.Empty,
                ActionSummary = result?.ActionSummaryText ?? string.Empty,
                RunReportPath = result?.RunReportPath ?? string.Empty
            };
            VisionPipelineBatchOutcomeContract.Apply(
                storageResult,
                result?.ExecutionCompleted == true,
                result?.ActualSuccess == true,
                hasJudgment: false,
                expectedSuccess: true,
                judgmentCorrect: false);
            return storageResult;
        }
    }
}
