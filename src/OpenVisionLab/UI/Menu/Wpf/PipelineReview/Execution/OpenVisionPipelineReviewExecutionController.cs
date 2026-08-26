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
        private readonly IDisplayManager displayManager;
        private readonly Action<Action> invokeOnUi;
        private readonly Dictionary<VisionPipelineStep, VisionPipelineStepResultSummary> stepResultSummaries = new Dictionary<VisionPipelineStep, VisionPipelineStepResultSummary>();
        private readonly Dictionary<string, Bitmap> reviewLayerImages = new Dictionary<string, Bitmap>(StringComparer.OrdinalIgnoreCase);
        private VisionPipeline activePipeline;
        private bool isRunning;
        private bool disposed;

        public OpenVisionPipelineReviewExecutionController(
            IDisplayManager displayManager,
            Action<Action> invokeOnUi)
        {
            this.displayManager = displayManager ?? throw new ArgumentNullException(nameof(displayManager));
            this.invokeOnUi = invokeOnUi ?? throw new ArgumentNullException(nameof(invokeOnUi));
        }

        public event EventHandler<OpenVisionPipelineReviewStepUpdatedEventArgs> StepUpdated = delegate { };

        public bool IsRunning => isRunning;

        public bool TryGetSummary(VisionPipelineStep step, out VisionPipelineStepResultSummary summary)
        {
            if (step == null)
            {
                summary = null;
                return false;
            }

            return stepResultSummaries.TryGetValue(step, out summary);
        }

        public IReadOnlyList<VisionPipelineGeometryFeatureResult> GetCurrentGeometryFeatures()
        {
            return stepResultSummaries.Values
                .SelectMany(summary => summary?.GeometryFeatures ?? Array.Empty<VisionPipelineGeometryFeatureResult>())
                .Where(item => item != null)
                .Select(item => item.Clone())
                .OrderBy(item => item.SourceStep, StringComparer.OrdinalIgnoreCase)
                .ThenBy(item => item.FeatureName, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        public Bitmap ResolveCachedOutput(string layerName)
        {
            if (string.IsNullOrWhiteSpace(layerName))
            {
                return null;
            }

            return reviewLayerImages.TryGetValue(layerName, out Bitmap image) ? image : null;
        }

        public void Reset()
        {
            ThrowIfDisposed();
            ClearState();
        }

        public async Task<OpenVisionPipelineReviewExecutionResult> RunAsync(
            VisionPipeline pipeline,
            int stepTimeoutMilliseconds)
        {
            ThrowIfDisposed();
            if (pipeline?.Steps == null || pipeline.Steps.Count == 0)
            {
                throw new InvalidOperationException("Pipeline review requires at least one step.");
            }

            if (isRunning)
            {
                throw new InvalidOperationException("Pipeline review is already running.");
            }

            isRunning = true;
            VisionPipelineExecutionPlan executionPlan = VisionPipelineExecutionPlan.Create(pipeline);
            activePipeline = executionPlan.EffectivePipeline;
            VisionPipelineRunResult runResult = null;
            try
            {
                VisionPipelineContext context = null;
                invokeOnUi(() => context = CreateReviewContextFromDisplayLayers());
                using (context)
                {
                    runResult = await VisionPipelineExecutionService.RunPreparedAsync(
                        executionPlan.EffectivePipeline,
                        context,
                        stepTimeoutMilliseconds,
                        CancellationToken.None,
                        OnStepExecutionUpdated,
                        executionPlan.NormalizationChanges);
                }

                OpenVisionPipelineReviewExecutionResult completedResult = null;
                invokeOnUi(() => completedResult = CompleteRun(executionPlan.EffectivePipeline, runResult));
                return completedResult ?? new OpenVisionPipelineReviewExecutionResult(0);
            }
            finally
            {
                DisposeRunResultImages(runResult);
                invokeOnUi(() =>
                {
                    isRunning = false;
                    activePipeline = null;
                });
            }
        }

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            disposed = true;
            StepUpdated = delegate { };
            ClearState();
        }

        private void OnStepExecutionUpdated(VisionPipelineStepExecutionUpdate update)
        {
            invokeOnUi(() =>
            {
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
                    StepUpdated(this, new OpenVisionPipelineReviewStepUpdatedEventArgs(update.Step, summary));
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
                    stepResultSummaries[step] = summary;
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
                Bitmap image = displayManager.GetLayerImage(index);
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

            if (onlyIfMissing && reviewLayerImages.ContainsKey(outputLayer))
            {
                return;
            }

            using Bitmap resultImage = BitmapImageConverter.ToBitmap(stepResult.ToolResult.ResultImage);
            Bitmap reviewImage = VisionPipelineRunReportImageRenderer.Render(
                resultImage,
                stepResult,
                GetStepDisplayIndex(stepResult.Step, activePipeline));
            ReplaceReviewLayerImage(outputLayer, reviewImage);
        }

        private static int GetStepDisplayIndex(VisionPipelineStep step, VisionPipeline pipeline)
        {
            int index = pipeline?.Steps?.IndexOf(step) ?? -1;
            return index < 0 ? 0 : index + 1;
        }

        private void ReplaceReviewLayerImage(string layerName, Bitmap image)
        {
            if (string.IsNullOrWhiteSpace(layerName) || image == null)
            {
                image?.Dispose();
                return;
            }

            if (reviewLayerImages.TryGetValue(layerName, out Bitmap existing))
            {
                existing?.Dispose();
            }

            reviewLayerImages[layerName] = image;
        }

        private void ClearState()
        {
            stepResultSummaries.Clear();
            foreach (Bitmap image in reviewLayerImages.Values)
            {
                image?.Dispose();
            }

            reviewLayerImages.Clear();
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
