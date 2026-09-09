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
        private readonly Dictionary<VisionPipelineStep, VisionPipelineStepResultSummary> stepResultSummaries = new Dictionary<VisionPipelineStep, VisionPipelineStepResultSummary>();
        private readonly Dictionary<string, Bitmap> reviewLayerImages = new Dictionary<string, Bitmap>(StringComparer.OrdinalIgnoreCase);
        private readonly object executionSync = new object();
        private Task shutdownTask = Task.CompletedTask;
        private TaskCompletionSource<bool> activeRunCompletion;
        private CancellationTokenSource activeCancellationSource;
        private VisionPipeline activePipeline;
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
                if (stepResultSummaries.TryGetValue(step, out summary))
                {
                    return true;
                }

                // Run Review executes a serialized effective copy of the pipeline.
                // The document continues to present the original Step instances,
                // so use the stable Step identity when resolving its completed
                // summary after the effective copy has been released.
                KeyValuePair<VisionPipelineStep, VisionPipelineStepResultSummary> match =
                    stepResultSummaries.FirstOrDefault(item => AreSameStep(item.Key, step));
                summary = match.Value;
                return match.Key != null;
            }
        }

        private static bool AreSameStep(VisionPipelineStep left, VisionPipelineStep right)
        {
            return left != null
                && right != null
                && string.Equals(left.Name, right.Name, StringComparison.Ordinal)
                && string.Equals(left.ToolType, right.ToolType, StringComparison.OrdinalIgnoreCase)
                && string.Equals(left.InputLayer, right.InputLayer, StringComparison.OrdinalIgnoreCase)
                && string.Equals(left.OutputLayer, right.OutputLayer, StringComparison.OrdinalIgnoreCase);
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
                if (disposed || !reviewLayerImages.TryGetValue(layerName, out Bitmap image) || image == null)
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
                        context = CreateReviewContextFromDisplayLayers();
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
                        summary = VisionPipelineResultSummaryService.CreateStepSummary(
                            GetStepDisplayIndex(update.Step, activePipeline),
                            update.StepResult);
                        stepResultSummaries[update.Step] = summary;
                        CacheReviewOutput(update.StepResult);
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
            foreach (VisionPipelineStepResultSummary summary in VisionPipelineResultSummaryService.CreateStepSummaries(runResult))
            {
                VisionPipelineStep step = pipeline.Steps.FirstOrDefault(candidate =>
                    string.Equals(candidate?.Name, summary.Name, StringComparison.Ordinal)
                    && string.Equals(candidate?.OutputLayer, summary.OutputLayer, StringComparison.OrdinalIgnoreCase));
                if (step != null)
                {
                    lock (executionSync)
                    {
                        stepResultSummaries[step] = summary;
                    }
                }
            }

            return new OpenVisionPipelineReviewExecutionResult(runResult?.StepResults?.Count ?? 0);
        }

        private VisionPipelineContext CreateReviewContextFromDisplayLayers()
        {
            VisionPipelineContext context = new VisionPipelineContext();
            for (int index = 0; index < displayManager.LayerCount; index++)
            {
                string title = displayManager.GetLayerTitle(index);
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

        private void CacheMissingReviewOutputs(VisionPipelineRunResult runResult)
        {
            foreach (VisionPipelineStepResult stepResult in runResult?.StepResults ?? Enumerable.Empty<VisionPipelineStepResult>())
            {
                CacheReviewOutput(stepResult, onlyIfMissing: true);
            }
        }

        private void CacheReviewOutput(VisionPipelineStepResult stepResult, bool onlyIfMissing = false)
        {
            string outputLayer = stepResult?.Step?.OutputLayer;
            if (string.IsNullOrWhiteSpace(outputLayer)
                || stepResult.ToolResult?.ResultImage == null
                || stepResult.ToolResult.ResultImage.Empty())
            {
                return;
            }

            using Bitmap resultImage = BitmapImageConverter.ToBitmap(stepResult.ToolResult.ResultImage);
            Bitmap reviewImage = VisionPipelineRunReportImageRenderer.Render(
                resultImage,
                stepResult,
                GetStepDisplayIndex(stepResult.Step, activePipeline));
            ReplaceReviewLayerImage(outputLayer, reviewImage, onlyIfMissing);
        }

        private static int GetStepDisplayIndex(VisionPipelineStep step, VisionPipeline pipeline)
        {
            int index = pipeline?.Steps?.IndexOf(step) ?? -1;
            return index < 0 ? 0 : index + 1;
        }

        private void ReplaceReviewLayerImage(string layerName, Bitmap image, bool onlyIfMissing = false)
        {
            if (string.IsNullOrWhiteSpace(layerName) || image == null)
            {
                image?.Dispose();
                return;
            }

            lock (executionSync)
            {
                if (onlyIfMissing && reviewLayerImages.ContainsKey(layerName))
                {
                    image.Dispose();
                    return;
                }

                if (reviewLayerImages.TryGetValue(layerName, out Bitmap existing))
                {
                    existing?.Dispose();
                }

                reviewLayerImages[layerName] = image;
            }
        }

        private void ClearState()
        {
            lock (executionSync)
            {
                stepResultSummaries.Clear();
                foreach (Bitmap image in reviewLayerImages.Values)
                {
                    image?.Dispose();
                }

                reviewLayerImages.Clear();
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
