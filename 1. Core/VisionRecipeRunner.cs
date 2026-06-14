using Lib.OpenCV.Pipeline;
using Lib.OpenCV.Tool;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OpenVisionLab
{
    public sealed class VisionRecipeRunner
    {
        public const string DefaultInputLayer = "Main";
        public const int DefaultStepTimeoutMilliseconds = 60000;

        public async Task<VisionRecipeRunResult> RunAsync(
            string recipeXmlPath,
            Mat sourceImage,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return await RunAsync(recipeXmlPath, sourceImage, DefaultInputLayer, DefaultStepTimeoutMilliseconds, cancellationToken);
        }

        public async Task<VisionRecipeRunResult> RunAsync(
            string recipeXmlPath,
            Mat sourceImage,
            string inputLayerName,
            int stepTimeoutMilliseconds,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrWhiteSpace(recipeXmlPath))
            {
                throw new ArgumentException("Recipe XML path is required.", nameof(recipeXmlPath));
            }

            if (!File.Exists(recipeXmlPath))
            {
                throw new FileNotFoundException("Recipe XML file was not found.", recipeXmlPath);
            }

            if (sourceImage == null || sourceImage.Empty())
            {
                throw new ArgumentException("Source image is required.", nameof(sourceImage));
            }

            if (stepTimeoutMilliseconds <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(stepTimeoutMilliseconds), "Step timeout must be greater than 0.");
            }

            if (!SerializeHelper.TryLoadFromXmlFile(recipeXmlPath, out VisionPipeline pipeline) || pipeline == null)
            {
                throw new InvalidOperationException($"Recipe XML could not be loaded: {recipeXmlPath}");
            }

            return await RunAsync(pipeline, sourceImage, inputLayerName, stepTimeoutMilliseconds, cancellationToken);
        }

        public async Task<VisionRecipeRunResult> RunAsync(
            VisionPipeline pipeline,
            Mat sourceImage,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return await RunAsync(pipeline, sourceImage, DefaultInputLayer, DefaultStepTimeoutMilliseconds, cancellationToken);
        }

        public async Task<VisionRecipeRunResult> RunAsync(
            VisionPipeline pipeline,
            Mat sourceImage,
            string inputLayerName,
            int stepTimeoutMilliseconds,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (pipeline == null)
            {
                throw new ArgumentNullException(nameof(pipeline));
            }

            if (sourceImage == null || sourceImage.Empty())
            {
                throw new ArgumentException("Source image is required.", nameof(sourceImage));
            }

            if (stepTimeoutMilliseconds <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(stepTimeoutMilliseconds), "Step timeout must be greater than 0.");
            }

            string sourceLayer = string.IsNullOrWhiteSpace(inputLayerName) ? DefaultInputLayer : inputLayerName;
            using (VisionPipelineContext context = new VisionPipelineContext())
            {
                context.SetLayer(sourceLayer, sourceImage);
                VisionPipelineRunResult runResult = null;
                try
                {
                    runResult = await VisionPipelineExecutionService.RunAsync(
                        pipeline,
                        context,
                        stepTimeoutMilliseconds,
                        cancellationToken);

                    return CreateResult(pipeline, context, runResult);
                }
                finally
                {
                    DisposeStepResultImages(runResult);
                }
            }
        }

        private static VisionRecipeRunResult CreateResult(
            VisionPipeline pipeline,
            VisionPipelineContext context,
            VisionPipelineRunResult runResult)
        {
            List<VisionRecipeStepRunSummary> steps = CreateStepSummaries(runResult);
            VisionRecipeStepRunSummary lastOutputStep = steps
                .LastOrDefault(step => step.HasResultImage && !string.IsNullOrWhiteSpace(step.OutputLayer));

            string finalLayer = lastOutputStep?.OutputLayer ?? string.Empty;
            Mat resultImage = null;
            if (!string.IsNullOrWhiteSpace(finalLayer))
            {
                resultImage = context.GetLayer(finalLayer);
            }

            if (resultImage == null || resultImage.Empty())
            {
                resultImage?.Dispose();
                resultImage = CloneLastStepImage(runResult);
            }

            return new VisionRecipeRunResult
            {
                PipelineName = pipeline?.Name ?? string.Empty,
                Success = runResult?.Success == true,
                Message = ResolveRunMessage(runResult),
                FinalLayer = finalLayer,
                FinalStepName = lastOutputStep?.Name ?? string.Empty,
                FinalToolType = lastOutputStep?.ToolType ?? string.Empty,
                ResultImage = resultImage,
                ResultImageWidth = resultImage != null && !resultImage.Empty() ? resultImage.Width : 0,
                ResultImageHeight = resultImage != null && !resultImage.Empty() ? resultImage.Height : 0,
                TotalMilliseconds = steps.Sum(step => step.ElapsedMilliseconds),
                Steps = steps
            };
        }

        private static List<VisionRecipeStepRunSummary> CreateStepSummaries(VisionPipelineRunResult runResult)
        {
            List<VisionPipelineStepResult> stepResults = runResult?.StepResults ?? new List<VisionPipelineStepResult>();
            List<VisionRecipeStepRunSummary> summaries = new List<VisionRecipeStepRunSummary>();
            for (int i = 0; i < stepResults.Count; i++)
            {
                VisionPipelineStepResult stepResult = stepResults[i];
                VisionPipelineStep step = stepResult?.Step;
                summaries.Add(new VisionRecipeStepRunSummary
                {
                    Index = i + 1,
                    Name = step?.Name ?? string.Empty,
                    ToolType = step?.ToolType ?? string.Empty,
                    InputLayer = step?.InputLayer ?? string.Empty,
                    OutputLayer = step?.OutputLayer ?? string.Empty,
                    Status = VisionPipelineResultSummaryService.ResolveStatus(stepResult),
                    Success = VisionPipelineResultSummaryService.IsPassed(stepResult),
                    Skipped = stepResult?.Skipped == true,
                    AcceptancePassed = stepResult?.AcceptancePassed == true,
                    AcceptanceMessage = stepResult?.AcceptanceMessage ?? string.Empty,
                    ElapsedMilliseconds = stepResult?.ToolResult?.Elapsed.TotalMilliseconds ?? 0d,
                    Message = VisionPipelineResultSummaryService.ResolveMessage(stepResult),
                    HasResultImage = stepResult?.ToolResult?.ResultImage != null
                        && !stepResult.ToolResult.ResultImage.Empty(),
                    ResultImageWidth = stepResult?.ToolResult?.ResultImage != null
                        && !stepResult.ToolResult.ResultImage.Empty()
                            ? stepResult.ToolResult.ResultImage.Width
                            : 0,
                    ResultImageHeight = stepResult?.ToolResult?.ResultImage != null
                        && !stepResult.ToolResult.ResultImage.Empty()
                            ? stepResult.ToolResult.ResultImage.Height
                            : 0,
                    OverlayCount = stepResult?.ToolResult?.Overlays?.Count ?? 0,
                    MetricCount = stepResult?.ToolResult?.Metrics?.Count ?? 0,
                    ParameterCount = step?.Parameters?.Count ?? 0,
                    Metrics = VisionPipelineKnownMetrics.OrderMetrics(stepResult?.ToolResult?.Metrics)
                        .ToDictionary(metric => metric.Key, metric => metric.Value, StringComparer.OrdinalIgnoreCase),
                    MetricsText = VisionPipelineKnownMetrics.FormatMetrics(stepResult?.ToolResult?.Metrics),
                    Overlays = CreateOverlaySummaries(stepResult?.ToolResult?.Overlays)
                });
            }

            return summaries;
        }

        private static List<VisionRecipeOverlaySummary> CreateOverlaySummaries(IEnumerable<VisionToolOverlay> overlays)
        {
            return (overlays ?? Enumerable.Empty<VisionToolOverlay>())
                .Where(overlay => overlay != null)
                .Select(overlay => new VisionRecipeOverlaySummary
                {
                    Kind = overlay.Kind.ToString(),
                    Label = overlay.Label ?? string.Empty,
                    BoundsX = overlay.Bounds.X,
                    BoundsY = overlay.Bounds.Y,
                    BoundsWidth = overlay.Bounds.Width,
                    BoundsHeight = overlay.Bounds.Height,
                    CenterX = overlay.Center.X,
                    CenterY = overlay.Center.Y,
                    StartX = overlay.Start.X,
                    StartY = overlay.Start.Y,
                    EndX = overlay.End.X,
                    EndY = overlay.End.Y,
                    Angle = overlay.Angle,
                    PointCount = overlay.Points?.Count ?? 0
                })
                .ToList();
        }

        private static string ResolveRunMessage(VisionPipelineRunResult runResult)
        {
            VisionPipelineStepResult failedStep = VisionPipelineResultSummaryService.FindFirstFailedStep(runResult);
            if (failedStep == null)
            {
                return runResult?.StepResults.Count > 0
                    ? "Recipe run completed."
                    : "Recipe run produced no step result.";
            }

            string stepName = failedStep.Step?.Name ?? "Step";
            string message = VisionPipelineResultSummaryService.ResolveMessage(failedStep);
            return string.IsNullOrWhiteSpace(message)
                ? $"{stepName} failed."
                : $"{stepName} failed. {message}";
        }

        private static Mat CloneLastStepImage(VisionPipelineRunResult runResult)
        {
            VisionPipelineStepResult stepResult = runResult?.StepResults
                .LastOrDefault(result => result?.ToolResult?.ResultImage != null
                    && !result.ToolResult.ResultImage.Empty());
            return stepResult?.ToolResult?.ResultImage?.Clone();
        }

        private static void DisposeStepResultImages(VisionPipelineRunResult runResult)
        {
            foreach (VisionPipelineStepResult stepResult in runResult?.StepResults ?? Enumerable.Empty<VisionPipelineStepResult>())
            {
                stepResult?.ToolResult?.ResultImage?.Dispose();
            }
        }
    }

    public sealed class VisionRecipeRunResult : IDisposable
    {
        public string PipelineName { get; set; } = string.Empty;
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string FinalLayer { get; set; } = string.Empty;
        public string FinalStepName { get; set; } = string.Empty;
        public string FinalToolType { get; set; } = string.Empty;
        public Mat ResultImage { get; set; }
        public int ResultImageWidth { get; set; }
        public int ResultImageHeight { get; set; }
        public double TotalMilliseconds { get; set; }
        public List<VisionRecipeStepRunSummary> Steps { get; set; } = new List<VisionRecipeStepRunSummary>();
        public string ResultImageSizeText => ResultImageWidth > 0 && ResultImageHeight > 0
            ? $"{ResultImageWidth} x {ResultImageHeight}"
            : string.Empty;

        public void Dispose()
        {
            ResultImage?.Dispose();
            ResultImage = null;
        }
    }

    public sealed class VisionRecipeStepRunSummary
    {
        public int Index { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ToolType { get; set; } = string.Empty;
        public string InputLayer { get; set; } = string.Empty;
        public string OutputLayer { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public bool Success { get; set; }
        public bool Skipped { get; set; }
        public bool AcceptancePassed { get; set; }
        public string AcceptanceMessage { get; set; } = string.Empty;
        public double ElapsedMilliseconds { get; set; }
        public string Message { get; set; } = string.Empty;
        public bool HasResultImage { get; set; }
        public int ResultImageWidth { get; set; }
        public int ResultImageHeight { get; set; }
        public int OverlayCount { get; set; }
        public int MetricCount { get; set; }
        public int ParameterCount { get; set; }
        public Dictionary<string, double> Metrics { get; set; } = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);
        public string MetricsText { get; set; } = string.Empty;
        public List<VisionRecipeOverlaySummary> Overlays { get; set; } = new List<VisionRecipeOverlaySummary>();
        public string ResultImageSizeText => ResultImageWidth > 0 && ResultImageHeight > 0
            ? $"{ResultImageWidth} x {ResultImageHeight}"
            : string.Empty;
    }

    public sealed class VisionRecipeOverlaySummary
    {
        public string Kind { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
        public float BoundsX { get; set; }
        public float BoundsY { get; set; }
        public float BoundsWidth { get; set; }
        public float BoundsHeight { get; set; }
        public float CenterX { get; set; }
        public float CenterY { get; set; }
        public float StartX { get; set; }
        public float StartY { get; set; }
        public float EndX { get; set; }
        public float EndY { get; set; }
        public double Angle { get; set; }
        public int PointCount { get; set; }
    }
}
