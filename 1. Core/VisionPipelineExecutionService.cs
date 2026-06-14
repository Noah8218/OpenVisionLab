using Lib.OpenCV.Pipeline;
using Lib.OpenCV.Tool;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OpenVisionLab
{
    internal sealed class VisionPipelineStepExecutionUpdate
    {
        public VisionPipelineStep Step { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public VisionPipelineStepResult StepResult { get; set; }
    }

    internal static class VisionPipelineExecutionService
    {
        public static async Task<VisionPipelineRunResult> RunAsync(
            VisionPipeline pipeline,
            VisionPipelineContext context,
            int stepTimeoutMilliseconds,
            CancellationToken cancellationToken,
            Action<VisionPipelineStepExecutionUpdate> stepUpdate = null)
        {
            if (pipeline == null) { throw new ArgumentNullException(nameof(pipeline)); }
            if (context == null) { throw new ArgumentNullException(nameof(context)); }

            VisionPipelineNormalizer.NormalizeChainedInspectionPreprocessing(pipeline);

            VisionPipelineRunResult runResult = new VisionPipelineRunResult();
            foreach (VisionPipelineStep step in pipeline.Steps)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    stepUpdate?.Invoke(new VisionPipelineStepExecutionUpdate
                    {
                        Status = "CANCEL",
                        Message = "Pipeline stopped before next step."
                    });
                    break;
                }

                if (step == null || !step.Enabled)
                {
                    VisionPipelineStepResult skippedResult = new VisionPipelineStepResult
                    {
                        Step = step,
                        Skipped = true,
                        AcceptancePassed = true,
                        AcceptanceMessage = "Step is disabled."
                    };
                    runResult.StepResults.Add(skippedResult);
                    stepUpdate?.Invoke(new VisionPipelineStepExecutionUpdate
                    {
                        Step = step,
                        Status = "SKIP",
                        Message = "Step is disabled.",
                        StepResult = skippedResult
                    });
                    continue;
                }

                stepUpdate?.Invoke(new VisionPipelineStepExecutionUpdate
                {
                    Step = step,
                    Status = "RUN",
                    Message = $"{step.InputLayer} -> {step.OutputLayer}"
                });

                Mat input = context.GetLayer(step.InputLayer);
                Stopwatch stepStopwatch = Stopwatch.StartNew();
                string validationMessage = ValidateStepInput(step, input);
                if (!string.IsNullOrWhiteSpace(validationMessage))
                {
                    stepStopwatch.Stop();
                    VisionToolResult failedResult = VisionToolResult.Failed(validationMessage, stepStopwatch.Elapsed);
                    VisionPipelineAcceptanceResult failedAcceptance = VisionPipelineAcceptanceEvaluator.Evaluate(step, failedResult);
                    VisionPipelineStepResult failedStepResult = new VisionPipelineStepResult
                    {
                        Step = step,
                        ToolResult = failedResult,
                        AcceptancePassed = failedAcceptance.Passed,
                        AcceptanceMessage = failedAcceptance.Message
                    };

                    runResult.StepResults.Add(failedStepResult);
                    stepUpdate?.Invoke(new VisionPipelineStepExecutionUpdate
                    {
                        Step = step,
                        Status = VisionPipelineResultSummaryService.ResolveStatus(failedStepResult),
                        Message = validationMessage,
                        StepResult = failedStepResult
                    });

                    input?.Dispose();
                    break;
                }

                Task<VisionToolResult> runTask = Task.Run(() => ExecuteStep(step, input));
                Task delayTask = Task.Delay(stepTimeoutMilliseconds, cancellationToken);
                Task completedTask = await Task.WhenAny(runTask, delayTask);

                VisionToolResult toolResult;
                bool disposeInputNow = true;
                if (completedTask != runTask)
                {
                    stepStopwatch.Stop();
                    disposeInputNow = false;
                    ReleaseInputWhenTaskCompletes(runTask, input);

                    string message = cancellationToken.IsCancellationRequested
                        ? "Step canceled before completion."
                        : $"Step timeout after {stepTimeoutMilliseconds / 1000} seconds.";
                    toolResult = VisionToolResult.Failed(message, stepStopwatch.Elapsed);
                }
                else
                {
                    toolResult = await runTask;
                }

                VisionPipelineAcceptanceResult acceptance = VisionPipelineAcceptanceEvaluator.Evaluate(step, toolResult);
                VisionPipelineStepResult stepResult = new VisionPipelineStepResult
                {
                    Step = step,
                    ToolResult = toolResult,
                    AcceptancePassed = acceptance.Passed,
                    AcceptanceMessage = acceptance.Message
                };
                runResult.StepResults.Add(stepResult);

                string status = VisionPipelineResultSummaryService.ResolveStatus(stepResult);
                string resultMessage = string.IsNullOrWhiteSpace(toolResult?.Message)
                    ? acceptance.Message
                    : toolResult.Message;
                stepUpdate?.Invoke(new VisionPipelineStepExecutionUpdate
                {
                    Step = step,
                    Status = status,
                    Message = resultMessage,
                    StepResult = stepResult
                });

                if (!toolResult.Success || !acceptance.Passed)
                {
                    if (disposeInputNow)
                    {
                        input?.Dispose();
                    }

                    break;
                }

                try
                {
                    context.SetLayer(step.OutputLayer, toolResult.ResultImage);
                }
                finally
                {
                    if (disposeInputNow)
                    {
                        input?.Dispose();
                    }
                }
            }

            return runResult;
        }

        private static VisionToolResult ExecuteStep(VisionPipelineStep step, Mat input)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            try
            {
                IVisionTool tool = VisionPipelineAppToolFactory.Create(step);
                if (tool == null)
                {
                    throw new InvalidOperationException($"Vision tool factory returned null for step '{step?.Name}'.");
                }

                return tool.Execute(input);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                return VisionToolResult.Failed(ex.GetBaseException().Message, stopwatch.Elapsed, ex);
            }
        }

        private static void ReleaseInputWhenTaskCompletes(Task<VisionToolResult> runTask, Mat input)
        {
            _ = runTask.ContinueWith(task =>
            {
                try
                {
                    if (task.Status == TaskStatus.RanToCompletion)
                    {
                        task.Result?.ResultImage?.Dispose();
                    }
                    else if (task.IsFaulted)
                    {
                        _ = task.Exception;
                    }
                }
                finally
                {
                    input?.Dispose();
                }
            }, TaskScheduler.Default);
        }

        private static string ValidateStepInput(VisionPipelineStep step, Mat input)
        {
            if (input == null || input.Empty())
            {
                return $"Input layer '{step?.InputLayer ?? "-"}' has no image.";
            }

            IDictionary<string, string> parameters = step?.Parameters;
            bool useRoi = GetBool(parameters, "USE_ROI", false);
            if (!useRoi)
            {
                return null;
            }

            bool useMultiRoi = GetBool(parameters, "USE_MULTI_ROI", false);
            IEnumerable<Rect> rois = useMultiRoi
                ? GetRectList(parameters, "CvROIS")
                : new[] { GetRect(parameters, "CvROI") };

            int imageWidth = input.Width;
            int imageHeight = input.Height;
            int index = 1;
            foreach (Rect roi in rois)
            {
                string message = ValidateRoi(roi, imageWidth, imageHeight, useMultiRoi ? $"ROI #{index}" : "ROI");
                if (!string.IsNullOrWhiteSpace(message))
                {
                    return $"{step?.Name ?? "Step"} {message}";
                }

                index++;
            }

            return null;
        }

        private static string ValidateRoi(Rect roi, int imageWidth, int imageHeight, string label)
        {
            if (roi.Width == 0 || roi.Height == 0)
            {
                return null;
            }

            if (roi.Width < 0 || roi.Height < 0)
            {
                return $"{label} has an invalid size. ROI=({roi.X},{roi.Y},{roi.Width},{roi.Height}), Image={imageWidth}x{imageHeight}.";
            }

            long right = (long)roi.X + roi.Width;
            long bottom = (long)roi.Y + roi.Height;
            if (roi.X < 0 || roi.Y < 0 || right > imageWidth || bottom > imageHeight)
            {
                return $"{label} is outside the input image. ROI=({roi.X},{roi.Y},{roi.Width},{roi.Height}), Image={imageWidth}x{imageHeight}.";
            }

            return null;
        }

        private static bool GetBool(IDictionary<string, string> parameters, string key, bool defaultValue)
        {
            string value = GetValue(parameters, key);
            return bool.TryParse(value, out bool result) ? result : defaultValue;
        }

        private static Rect GetRect(IDictionary<string, string> parameters, string key)
        {
            string value = GetValue(parameters, key);
            return TryParseRect(value, out Rect rect) ? rect : default;
        }

        private static List<Rect> GetRectList(IDictionary<string, string> parameters, string key)
        {
            string value = GetValue(parameters, key);
            if (string.IsNullOrWhiteSpace(value))
            {
                return new List<Rect>();
            }

            return value
                .Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(part => TryParseRect(part, out Rect rect) ? rect : default)
                .ToList();
        }

        private static bool TryParseRect(string value, out Rect rect)
        {
            rect = default;
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            string[] parts = value.Split(',');
            if (parts.Length != 4)
            {
                return false;
            }

            if (!int.TryParse(parts[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out int x)
                || !int.TryParse(parts[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out int y)
                || !int.TryParse(parts[2], NumberStyles.Integer, CultureInfo.InvariantCulture, out int width)
                || !int.TryParse(parts[3], NumberStyles.Integer, CultureInfo.InvariantCulture, out int height))
            {
                return false;
            }

            rect = new Rect(x, y, width, height);
            return true;
        }

        private static string GetValue(IDictionary<string, string> parameters, string key)
        {
            if (parameters == null || string.IsNullOrWhiteSpace(key))
            {
                return null;
            }

            foreach (KeyValuePair<string, string> item in parameters)
            {
                if (string.Equals(item.Key, key, StringComparison.OrdinalIgnoreCase))
                {
                    return item.Value;
                }
            }

            return null;
        }
    }
}
