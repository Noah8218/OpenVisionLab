using OpenVisionLab;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;

internal static class ValidationDatasetReviewQueueEvidence
{
    internal static void VerifyAndWrite(
        OpenVisionShellHostView shellHost,
        OpenVisionRecipeBatchRunOption run,
        VisionPipelineBatchRunSummary summary,
        string artifactDirectory,
        FrameworkElement reviewQueuePanel,
        Action<int> pump)
    {
        int previewRunsBeforeQueue = shellHost.NativePreviewRunCount;
        int layerCountBeforeQueue = shellHost.LayerDocumentCount;
        string inputRouteBeforeQueue = shellHost.ActiveNativeRouteInputLayerNameForTest;
        string outputRouteBeforeQueue = shellHost.ActiveNativeRouteOutputLayerNameForTest;
        bool containsPitchMetric = summary.ReviewQueue.Any(entry =>
            entry?.Reasons?.Any(reason => reason.Contains("PitchPx", StringComparison.Ordinal)) == true);
        shellHost.RecipeCommands.ShowRecentBatchReviewQueueOnly = true;
        pump(60);
        IReadOnlyList<OpenVisionRecipeBatchSampleResultOption> reviewRows =
            shellHost.RecipeCommands.FilteredRecentBatchRunSampleResults;
        if (summary.ReviewQueue.Count == 0
            || summary.ReviewQueueSha256.Length != 64
            || !containsPitchMetric
            || reviewRows.Count != summary.ReviewQueue.Count
            || reviewRows.Any(row => row?.IsInReviewQueue != true)
            || !shellHost.RecipeCommands.RecentBatchRunReviewQueueSummaryText.Contains(
                summary.ReviewQueueSha256.Substring(0, 12),
                StringComparison.Ordinal)
            || shellHost.NativePreviewRunCount != previewRunsBeforeQueue
            || shellHost.LayerDocumentCount != layerCountBeforeQueue
            || !string.Equals(shellHost.ActiveNativeRouteInputLayerNameForTest, inputRouteBeforeQueue, StringComparison.Ordinal)
            || !string.Equals(shellHost.ActiveNativeRouteOutputLayerNameForTest, outputRouteBeforeQueue, StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                "Dataset Run History review queue did not preserve Pitch metrics, identity, or workspace state. "
                + $"Rows={reviewRows.Count}/{summary.ReviewQueue.Count}, Hash='{summary.ReviewQueueSha256}', Pitch={containsPitchMetric}.");
        }

        reviewQueuePanel.BringIntoView();
        shellHost.UpdateLayout();
        pump(100);
        if (!string.IsNullOrWhiteSpace(run.SummaryPath) && File.Exists(run.SummaryPath))
        {
            File.Copy(run.SummaryPath, Path.Combine(artifactDirectory, "saved_batch_summary.xml"), overwrite: true);
        }

        File.WriteAllLines(
            Path.Combine(artifactDirectory, "review_queue_contract.txt"),
            BuildContractLines(
                summary,
                containsPitchMetric,
                shellHost.NativePreviewRunCount == previewRunsBeforeQueue,
                shellHost.LayerDocumentCount == layerCountBeforeQueue));
    }

    internal static IReadOnlyList<string> BuildContractLines(
        VisionPipelineBatchRunSummary summary,
        bool containsPitchMetric,
        bool previewRunsUnchanged,
        bool layerCountUnchanged)
    {
        return new[]
        {
            "Policy=" + summary.ReviewQueuePolicy,
            "Sha256=" + summary.ReviewQueueSha256,
            "Rows=" + summary.ReviewQueue.Count.ToString(CultureInfo.InvariantCulture),
            "Total=" + summary.Results.Count.ToString(CultureInfo.InvariantCulture),
            "ContainsPitchMetric=" + containsPitchMetric,
            "PreviewRunCountUnchanged=" + previewRunsUnchanged,
            "LayerCountUnchanged=" + layerCountUnchanged
        };
    }
}
