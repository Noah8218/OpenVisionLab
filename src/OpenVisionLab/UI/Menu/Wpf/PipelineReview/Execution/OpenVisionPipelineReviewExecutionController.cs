using OpenVisionLab.Common;
using OpenVisionLab.Core;
using OpenVisionLab.Vision2D.Pipeline;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OpenVisionLab
{
    // Owns explicit Run Review execution state and review-only result images.
    internal sealed class OpenVisionPipelineReviewExecutionController : IDisposable
    {
        private readonly struct ExecutionStamp
        {
            public ExecutionStamp(long runId, long inputRevision, long recipeRevision, CancellationToken cancellationToken)
            {
                RunId = runId;
                InputRevision = inputRevision;
                RecipeRevision = recipeRevision;
                CancellationToken = cancellationToken;
            }

            public long RunId { get; }

            public long InputRevision { get; }

            public long RecipeRevision { get; }

            public CancellationToken CancellationToken { get; }
        }

        private readonly IDisplayManager displayManager;
        private readonly Action<Action> invokeOnUi;
        private readonly Dictionary<int, VisionPipelineStepResultSummary> stepResultSummaries = new Dictionary<int, VisionPipelineStepResultSummary>();
        private readonly Dictionary<int, Bitmap> reviewStepImages = new Dictionary<int, Bitmap>();
        private readonly Dictionary<int, string> reviewStepOutputLayers = new Dictionary<int, string>();
        private readonly HashSet<string> reviewProducedOutputLayers = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        private readonly object executionSync = new object();
        private Task shutdownTask = Task.CompletedTask;
        private TaskCompletionSource<bool> activeRunCompletion;
        private CancellationTokenSource activeCancellationSource;
        private VisionPipeline activePipeline;
        private VisionPipeline sourcePipeline;
        private long executionGeneration;
        private long activeRunId;
        private long activeInputRevision;
        private long activeRecipeRevision;
        private bool isRunning;
        private bool isStopping;
        private bool disposed;

        public OpenVisionPipelineReviewExecutionController(
            IDisplayManager displayManager,
            Action<Action> invokeOnUi)
        {
            this.displayManager = displayManager ?? throw new ArgumentNullException(nameof(displayManager));
            this.invokeOnUi = invokeOnUi ?? throw new ArgumentNullException(nameof(invokeOnUi));
        }

        public event EventHandler<OpenVisionPipelineReviewStepUpdatedEventArgs> StepUpdated = delegate { };

        public bool IsRunning
        {
            get
            {
                lock (executionSync)
                {
                    return isRunning;
                }
            }
        }

        internal bool IsStopping
        {
            get
            {
                lock (executionSync)
                {
                    return isStopping;
                }
            }
        }

        internal long ActiveRunId
        {
            get
            {
                lock (executionSync)
                {
                    return activeRunId;
                }
            }
        }

        internal long ActiveInputRevision
        {
            get
            {
                lock (executionSync)
                {
                    return activeInputRevision;
                }
            }
        }

        internal long ActiveRecipeRevision
        {
            get
            {
                lock (executionSync)
                {
                    return activeRecipeRevision;
                }
            }
        }

        public bool TryGetSummary(VisionPipelineStep step, out VisionPipelineStepResultSummary summary)
        {
            if (step == null)
            {
                summary = null;
                return false;
            }

            lock (executionSync)
            {
                int stepIndex = ResolveStepIndex(step);
                if (stepIndex < 0)
                {
                    summary = null;
                    return false;
                }

                return stepResultSummaries.TryGetValue(stepIndex, out summary);
            }
        }

        public IReadOnlyList<VisionPipelineGeometryFeatureResult> GetCurrentGeometryFeatures()
        {
            lock (executionSync)
            {
                return stepResultSummaries.Values
                    .SelectMany(summary => summary?.GeometryFeatures ?? Array.Empty<VisionPipelineGeometryFeatureResult>())
                    .Where(item => item != null)
                    .Select(item => item.Clone())
                    .OrderBy(item => item.SourceStep, StringComparer.OrdinalIgnoreCase)
                    .ThenBy(item => item.FeatureName, StringComparer.OrdinalIgnoreCase)
                    .ToList();
            }
        }

        public Bitmap AcquireCachedOutputSnapshot(string layerName)
        {
            if (string.IsNullOrWhiteSpace(layerName))
            {
                return null;
            }

            lock (executionSync)
            {
                if (disposed)
                {
                    return null;
                }

                int latestStepIndex = -1;
                foreach (KeyValuePair<int, string> item in reviewStepOutputLayers)
                {
                    if (item.Key > latestStepIndex
                        && string.Equals(item.Value, layerName, StringComparison.OrdinalIgnoreCase)
                        && reviewStepImages.ContainsKey(item.Key))
                    {
                        latestStepIndex = item.Key;
                    }
                }

                return CloneCachedOutputSnapshot(latestStepIndex);
            }
        }

        public Bitmap AcquireCachedOutputSnapshot(int stepIndex, string layerName)
        {
            if (stepIndex < 0 || string.IsNullOrWhiteSpace(layerName))
            {
                return null;
            }

            lock (executionSync)
            {
                if (disposed
                    || !reviewStepOutputLayers.TryGetValue(stepIndex, out string cachedLayerName)
                    || !string.Equals(cachedLayerName, layerName, StringComparison.OrdinalIgnoreCase))
                {
                    return null;
                }

                return CloneCachedOutputSnapshot(stepIndex);
            }
        }

        public void Reset()
        {
            ThrowIfDisposed();
            CancellationTokenSource cancellationSource = null;
            lock (executionSync)
            {
                executionGeneration++;
                if (isRunning)
                {
                    isStopping = true;
                    cancellationSource = activeCancellationSource;
                }

                activePipeline = null;
            }

            cancellationSource?.Cancel();
            ClearState();
        }

        public Task<OpenVisionPipelineReviewExecutionResult> RunAsync(
            VisionPipeline pipeline,
            int stepTimeoutMilliseconds)
        {
            return RunAsync(pipeline, stepTimeoutMilliseconds, 0, 0);
        }

        internal async Task<OpenVisionPipelineReviewExecutionResult> RunAsync(
            VisionPipeline pipeline,
            int stepTimeoutMilliseconds,
            long inputRevision,
            long recipeRevision)
        {
            ThrowIfDisposed();
            if (pipeline?.Steps == null || pipeline.Steps.Count == 0)
            {
                throw new InvalidOperationException("Pipeline review requires at least one step.");
            }

            if (IsRunning)
            {
                throw new InvalidOperationException("Pipeline review is already running.");
            }

            ExecutionStamp stamp = BeginRun(inputRevision, recipeRevision);
            lock (executionSync)
            {
                sourcePipeline = pipeline;
            }

            VisionPipelineExecutionPlan executionPlan = null;
            VisionPipelineRunResult runResult = null;
            try
            {
                executionPlan = VisionPipelineExecutionPlan.Create(pipeline);
                activePipeline = executionPlan.EffectivePipeline;
                if (!IsCurrentRun(stamp))
                {
                    return new OpenVisionPipelineReviewExecutionResult(0, wasSuperseded: true);
                }

                VisionPipelineContext context = null;
                invokeOnUi(() =>
                {
                    if (IsCurrentRun(stamp))
                    {
                        context = CreateReviewContextFromDisplayLayers(executionPlan.EffectivePipeline);
                    }
                });
                if (context == null)
                {
                    return new OpenVisionPipelineReviewExecutionResult(0, wasSuperseded: true);
                }

                using (context)
                {
                    runResult = await VisionPipelineExecutionService.RunPreparedAsync(
                        executionPlan.EffectivePipeline,
                        context,
                        stepTimeoutMilliseconds,
                        stamp.CancellationToken,
                        update => OnStepExecutionUpdated(stamp, update),
                        executionPlan.NormalizationChanges).ConfigureAwait(false);
                }

                if (!IsCurrentRun(stamp))
                {
                    return new OpenVisionPipelineReviewExecutionResult(0, wasSuperseded: true);
                }

                OpenVisionPipelineReviewExecutionResult completedResult = null;
                invokeOnUi(() =>
                {
                    lock (executionSync)
                    {
                        if (IsCurrentRun(stamp))
                        {
                            completedResult = CompleteRun(executionPlan.EffectivePipeline, runResult);
                        }
                    }
                });
                return completedResult ?? new OpenVisionPipelineReviewExecutionResult(0, wasSuperseded: true);
            }
            finally
            {
                DisposeRunResultImages(runResult);
                EndRun(stamp);
            }
        }

        public void Dispose()
        {
            CancellationTokenSource cancellationSource = null;
            TaskCompletionSource<bool> completionSource = null;
            bool hasActiveRun = false;
            lock (executionSync)
            {
                if (disposed)
                {
                    return;
                }

                disposed = true;
                executionGeneration++;
                if (isRunning)
                {
                    hasActiveRun = true;
                    isStopping = true;
                    cancellationSource = activeCancellationSource;
                    completionSource = activeRunCompletion;
                    shutdownTask = completionSource == null
                        ? Task.CompletedTask
                        : ClearStateAfterCompletionAsync(completionSource.Task);
                }
                else
                {
                    shutdownTask = Task.CompletedTask;
                }

                activePipeline = null;
            }

            StepUpdated = delegate { };
            cancellationSource?.Cancel();
            if (!hasActiveRun || completionSource == null)
            {
                ClearState();
            }
        }

        public async Task DisposeAsync()
        {
            Dispose();
            Task completion;
            lock (executionSync)
            {
                completion = shutdownTask;
            }

            await completion.ConfigureAwait(false);
        }

        private ExecutionStamp BeginRun(long inputRevision, long recipeRevision)
        {
            lock (executionSync)
            {
                if (disposed)
                {
                    throw new ObjectDisposedException(nameof(OpenVisionPipelineReviewExecutionController));
                }

                if (isRunning)
                {
                    throw new InvalidOperationException("Pipeline review is already running.");
                }

                long runId = ++executionGeneration;
                CancellationTokenSource cancellationSource = new CancellationTokenSource();
                activeCancellationSource = cancellationSource;
                activeRunCompletion = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
                activeRunId = runId;
                activeInputRevision = inputRevision;
                activeRecipeRevision = recipeRevision;
                isRunning = true;
                isStopping = false;
                return new ExecutionStamp(runId, inputRevision, recipeRevision, cancellationSource.Token);
            }
        }

        private bool IsCurrentRun(ExecutionStamp stamp)
        {
            lock (executionSync)
            {
                return !disposed
                    && isRunning
                    && activeRunId == stamp.RunId
                    && executionGeneration == stamp.RunId
                    && activeInputRevision == stamp.InputRevision
                    && activeRecipeRevision == stamp.RecipeRevision;
            }
        }

        private void EndRun(ExecutionStamp stamp)
        {
            CancellationTokenSource cancellationSource = null;
            TaskCompletionSource<bool> completionSource = null;
            lock (executionSync)
            {
                if (activeRunId != stamp.RunId)
                {
                    return;
                }

                isRunning = false;
                isStopping = false;
                activePipeline = null;
                activeInputRevision = 0;
                activeRecipeRevision = 0;
                cancellationSource = activeCancellationSource;
                completionSource = activeRunCompletion;
                activeCancellationSource = null;
                activeRunCompletion = null;
            }

            cancellationSource?.Dispose();
            completionSource?.TrySetResult(true);
        }

        private async Task ClearStateAfterCompletionAsync(Task completion)
        {
            await completion.ConfigureAwait(false);
            ClearState();
        }

        private void OnStepExecutionUpdated(ExecutionStamp stamp, VisionPipelineStepExecutionUpdate update)
        {
            invokeOnUi(() =>
            {
                // Keep stamp validation and every observable update in one boundary so Reset/Close
                // cannot clear state between the check and a stale callback mutation.
                lock (executionSync)
                {
                    if (!IsCurrentRun(stamp))
                    {
                        return;
                    }

                    VisionPipelineStepResultSummary summary = null;
                    if (update?.StepResult != null && update.Step != null)
                    {
                        int stepIndex = GetStepIndex(update.Step, activePipeline);
                        summary = VisionPipelineResultSummaryService.CreateStepSummary(
                            stepIndex < 0 ? 0 : stepIndex + 1,
                            update.StepResult);
                        if (stepIndex >= 0)
                        {
                            stepResultSummaries[stepIndex] = summary;
                            CacheReviewOutput(update.StepResult, stepIndex);
                        }
                    }

                    if (update?.Step != null)
                    {
                        StepUpdated(this, new OpenVisionPipelineReviewStepUpdatedEventArgs(
                            update.Step,
                            summary,
                            stamp.InputRevision,
                            stamp.RecipeRevision));
                    }
                }
            });
        }

        private OpenVisionPipelineReviewExecutionResult CompleteRun(
            VisionPipeline pipeline,
            VisionPipelineRunResult runResult)
        {
            CacheMissingReviewOutputs(runResult);
            IReadOnlyList<VisionPipelineStepResultSummary> summaries = VisionPipelineResultSummaryService.CreateStepSummaries(
                pipeline,
                runResult);
            for (int stepIndex = 0; stepIndex < summaries.Count; stepIndex++)
            {
                VisionPipelineStepResultSummary summary = summaries[stepIndex];
                if (summary != null)
                {
                    lock (executionSync)
                    {
                        stepResultSummaries[stepIndex] = summary;
                    }
                }
            }

            return new OpenVisionPipelineReviewExecutionResult(runResult?.StepResults?.Count ?? 0);
        }

        private VisionPipelineContext CreateReviewContextFromDisplayLayers(VisionPipeline pipeline)
        {
            HashSet<string> currentPipelineOutputLayers = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            HashSet<string> explicitBranchInputLayers = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (VisionPipelineStep step in pipeline?.Steps ?? new List<VisionPipelineStep>())
            {
                if (step == null)
                {
                    continue;
                }

                if (!string.IsNullOrWhiteSpace(step.OutputLayer))
                {
                    currentPipelineOutputLayers.Add(step.OutputLayer.Trim());
                }

                if (step.Enabled
                    && VisionPipelineNormalizer.IsBranchInputAllowed(step)
                    && !string.IsNullOrWhiteSpace(step.InputLayer))
                {
                    explicitBranchInputLayers.Add(step.InputLayer.Trim());
                }
            }

            HashSet<string> blockedOutputLayers = new HashSet<string>(
                currentPipelineOutputLayers,
                StringComparer.OrdinalIgnoreCase);
            lock (executionSync)
            {
                blockedOutputLayers.UnionWith(reviewProducedOutputLayers);
            }

            foreach (string explicitBranchInputLayer in explicitBranchInputLayers)
            {
                if (!currentPipelineOutputLayers.Contains(explicitBranchInputLayer))
                {
                    blockedOutputLayers.Remove(explicitBranchInputLayer);
                }
            }

            VisionPipelineContext context = new VisionPipelineContext();
            for (int index = 0; index < displayManager.LayerCount; index++)
            {
                string title = displayManager.GetLayerTitle(index);
                if (!IsDefaultReviewInputLayer(title) && blockedOutputLayers.Contains(title))
                {
                    continue;
                }

                using Bitmap image = displayManager.GetLayerImageSnapshot(title);
                if (string.IsNullOrWhiteSpace(title) || image == null || DisplayManagerImageExtensions.IsPlaceholderBitmap(image))
                {
                    continue;
                }

                using Mat mat = BitmapImageConverter.ToMat(image);
                context.SetLayer(title, mat);
            }

            return context;
        }

        private static bool IsDefaultReviewInputLayer(string layerName)
        {
            return string.Equals(
                layerName?.Trim(),
                VisionRecipeRunner.DefaultInputLayer,
                StringComparison.OrdinalIgnoreCase);
        }

        private void CacheMissingReviewOutputs(VisionPipelineRunResult runResult)
        {
            foreach (VisionPipelineStepResult stepResult in runResult?.StepResults ?? Enumerable.Empty<VisionPipelineStepResult>())
            {
                CacheReviewOutput(stepResult, onlyIfMissing: true);
            }
        }

        private void CacheReviewOutput(VisionPipelineStepResult stepResult, bool onlyIfMissing = false)
        {
            CacheReviewOutput(stepResult, GetStepIndex(stepResult?.Step, activePipeline), onlyIfMissing);
        }

        private void CacheReviewOutput(
            VisionPipelineStepResult stepResult,
            int stepIndex,
            bool onlyIfMissing = false)
        {
            string outputLayer = stepResult?.Step?.OutputLayer;
            if (stepIndex < 0
                || string.IsNullOrWhiteSpace(outputLayer)
                || stepResult.ToolResult?.ResultImage == null
                || stepResult.ToolResult.ResultImage.Empty())
            {
                return;
            }

            using Bitmap resultImage = BitmapImageConverter.ToBitmap(stepResult.ToolResult.ResultImage);
            Bitmap reviewImage = VisionPipelineRunReportImageRenderer.Render(
                resultImage,
                stepResult,
                stepIndex + 1);
            ReplaceReviewStepImage(stepIndex, outputLayer, reviewImage, onlyIfMissing);
        }

        private static int GetStepIndex(VisionPipelineStep step, VisionPipeline pipeline)
        {
            return pipeline?.Steps?.IndexOf(step) ?? -1;
        }

        private int ResolveStepIndex(VisionPipelineStep step)
        {
            int index = GetStepIndex(step, sourcePipeline);
            return index >= 0 ? index : GetStepIndex(step, activePipeline);
        }

        private void ReplaceReviewStepImage(
            int stepIndex,
            string layerName,
            Bitmap image,
            bool onlyIfMissing = false)
        {
            if (stepIndex < 0 || string.IsNullOrWhiteSpace(layerName) || image == null)
            {
                image?.Dispose();
                return;
            }

            lock (executionSync)
            {
                if (onlyIfMissing && reviewStepImages.ContainsKey(stepIndex))
                {
                    image.Dispose();
                    return;
                }

                if (reviewStepImages.TryGetValue(stepIndex, out Bitmap existing))
                {
                    existing?.Dispose();
                }

                reviewStepImages[stepIndex] = image;
                reviewStepOutputLayers[stepIndex] = layerName;
                reviewProducedOutputLayers.Add(layerName.Trim());
            }
        }

        private Bitmap CloneCachedOutputSnapshot(int stepIndex)
        {
            if (stepIndex < 0 || !reviewStepImages.TryGetValue(stepIndex, out Bitmap image) || image == null)
            {
                return null;
            }

            try
            {
                return new Bitmap(image);
            }
            catch
            {
                return null;
            }
        }

        private void ClearState()
        {
            lock (executionSync)
            {
                stepResultSummaries.Clear();
                foreach (Bitmap image in reviewStepImages.Values)
                {
                    image?.Dispose();
                }

                reviewStepImages.Clear();
                reviewStepOutputLayers.Clear();
                sourcePipeline = null;
            }
        }

        private static void DisposeRunResultImages(VisionPipelineRunResult runResult)
        {
            foreach (VisionPipelineStepResult stepResult in runResult?.StepResults ?? Enumerable.Empty<VisionPipelineStepResult>())
            {
                stepResult?.ToolResult?.ResultImage?.Dispose();
            }
        }

        private void ThrowIfDisposed()
        {
            if (disposed)
            {
                throw new ObjectDisposedException(nameof(OpenVisionPipelineReviewExecutionController));
            }
        }
    }
}
