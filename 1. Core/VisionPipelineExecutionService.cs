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
        private sealed class StepRuntimeValidationResult
        {
            public VisionToolErrorCode ErrorCode { get; set; } = VisionToolErrorCode.None;
            public string Message { get; set; } = string.Empty;
            public bool HasError => ErrorCode != VisionToolErrorCode.None;
        }

        public static async Task<VisionPipelineRunResult> RunAsync(
            VisionPipeline pipeline,
            VisionPipelineContext context,
            int stepTimeoutMilliseconds,
            CancellationToken cancellationToken,
            Action<VisionPipelineStepExecutionUpdate> stepUpdate = null)
        {
            if (pipeline == null) { throw new ArgumentNullException(nameof(pipeline)); }
            if (context == null) { throw new ArgumentNullException(nameof(context)); }

            VisionPipelineRunResult runResult = new VisionPipelineRunResult();
            IReadOnlyList<VisionPipelineNormalizationChange> normalizationChanges = VisionPipelineNormalizer.NormalizeForRun(pipeline);
            foreach (VisionPipelineNormalizationChange change in normalizationChanges)
            {
                stepUpdate?.Invoke(new VisionPipelineStepExecutionUpdate
                {
                    Step = change.Step,
                    Status = "AUTO FIX",
                    Message = change.Message
                });
            }

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

                Stopwatch stepStopwatch = Stopwatch.StartNew();
                StepRuntimeValidationResult configurationValidation = ValidateStepConfiguration(step);
                if (configurationValidation.HasError)
                {
                    stepStopwatch.Stop();
                    VisionPipelineStepResult failedStepResult = CreateFailedStepResult(step, configurationValidation, stepStopwatch.Elapsed);
                    runResult.StepResults.Add(failedStepResult);
                    stepUpdate?.Invoke(new VisionPipelineStepExecutionUpdate
                    {
                        Step = step,
                        Status = VisionPipelineResultSummaryService.ResolveStatus(failedStepResult),
                        Message = configurationValidation.Message,
                        StepResult = failedStepResult
                    });
                    break;
                }

                stepUpdate?.Invoke(new VisionPipelineStepExecutionUpdate
                {
                    Step = step,
                    Status = "RUN",
                    Message = $"{step.InputLayer} -> {step.OutputLayer}"
                });

                Mat input = context.GetLayer(step.InputLayer);
                StepRuntimeValidationResult inputValidation = ValidateStepInput(step, input);
                if (inputValidation.HasError)
                {
                    stepStopwatch.Stop();
                    VisionPipelineStepResult failedStepResult = CreateFailedStepResult(step, inputValidation, stepStopwatch.Elapsed);
                    runResult.StepResults.Add(failedStepResult);
                    stepUpdate?.Invoke(new VisionPipelineStepExecutionUpdate
                    {
                        Step = step,
                        Status = VisionPipelineResultSummaryService.ResolveStatus(failedStepResult),
                        Message = inputValidation.Message,
                        StepResult = failedStepResult
                    });

                    input?.Dispose();
                    break;
                }

                Task<VisionToolResult> runTask = Task.Run(() =>
                    VisionPipelineOverlayMergeService.IsMergeTool(step.ToolType)
                        ? VisionPipelineOverlayMergeService.Execute(step, input, runResult)
                        : ExecuteStep(step, input));
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
                    toolResult = VisionToolResult.Failed(
                        cancellationToken.IsCancellationRequested
                            ? VisionToolErrorCode.StepCanceled
                            : VisionToolErrorCode.StepTimeout,
                        message,
                        stepStopwatch.Elapsed);
                }
                else
                {
                    toolResult = await runTask;
                }

                VisionPipelineMetricEnrichmentService.Enrich(toolResult, step);
                VisionPipelineAcceptanceResult acceptance = VisionPipelineAcceptanceEvaluator.Evaluate(step, toolResult);
                VisionPipelineStepResult stepResult = new VisionPipelineStepResult
                {
                    Step = step,
                    ToolResult = toolResult,
                    AcceptancePassed = acceptance.Passed,
                    AcceptanceMessage = acceptance.Message
                };

                if (!toolResult.Success || !acceptance.Passed)
                {
                    runResult.StepResults.Add(stepResult);
                    NotifyStepResult(stepUpdate, step, stepResult);

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
                catch (Exception ex)
                {
                    stepStopwatch.Stop();
                    toolResult.ResultImage?.Dispose();
                    StepRuntimeValidationResult outputWriteFailure = new StepRuntimeValidationResult
                    {
                        ErrorCode = VisionToolErrorCode.InvalidParameter,
                        Message = $"Output layer '{step.OutputLayer}' could not be written. {ex.GetBaseException().Message}"
                    };
                    VisionPipelineStepResult failedStepResult = CreateFailedStepResult(step, outputWriteFailure, stepStopwatch.Elapsed, ex);
                    runResult.StepResults.Add(failedStepResult);
                    stepUpdate?.Invoke(new VisionPipelineStepExecutionUpdate
                    {
                        Step = step,
                        Status = VisionPipelineResultSummaryService.ResolveStatus(failedStepResult),
                        Message = outputWriteFailure.Message,
                        StepResult = failedStepResult
                    });
                    break;
                }
                finally
                {
                    if (disposeInputNow)
                    {
                        input?.Dispose();
                    }
                }

                runResult.StepResults.Add(stepResult);
                NotifyStepResult(stepUpdate, step, stepResult);
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
                    stopwatch.Stop();
                    return VisionToolResult.Failed(
                        VisionToolErrorCode.ToolFactoryFailed,
                        $"Vision tool factory returned null for step '{step?.Name}'.",
                        stopwatch.Elapsed);
                }

                VisionToolResult result = tool.Execute(input);
                if (result == null)
                {
                    stopwatch.Stop();
                    return VisionToolResult.Failed(
                        VisionToolErrorCode.ToolExecutionException,
                        $"Vision tool '{step?.ToolType ?? "-"}' returned no result.",
                        stopwatch.Elapsed);
                }

                return result;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                return VisionToolResult.Failed(
                    ResolveServiceErrorCode(ex),
                    ex.GetBaseException().Message,
                    stopwatch.Elapsed,
                    ex);
            }
        }

        private static VisionToolErrorCode ResolveServiceErrorCode(Exception exception)
        {
            Exception baseException = exception?.GetBaseException() ?? exception;
            if (baseException is NotSupportedException)
            {
                return VisionToolErrorCode.ToolFactoryFailed;
            }

            if (baseException is ArgumentException
                || baseException is FormatException
                || baseException is InvalidCastException)
            {
                return VisionToolErrorCode.InvalidParameter;
            }

            string message = baseException?.Message ?? string.Empty;
            if (message.IndexOf("factory", StringComparison.OrdinalIgnoreCase) >= 0
                || message.IndexOf("Unsupported vision tool", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return VisionToolErrorCode.ToolFactoryFailed;
            }

            return VisionToolErrorCode.ToolExecutionException;
        }

        private static VisionPipelineStepResult CreateFailedStepResult(
            VisionPipelineStep step,
            StepRuntimeValidationResult validation,
            TimeSpan elapsed,
            Exception exception = null)
        {
            VisionToolResult failedResult = VisionToolResult.Failed(
                validation?.ErrorCode ?? VisionToolErrorCode.Unknown,
                validation?.Message ?? string.Empty,
                elapsed,
                exception);
            VisionPipelineAcceptanceResult failedAcceptance = VisionPipelineAcceptanceEvaluator.Evaluate(step, failedResult);
            return new VisionPipelineStepResult
            {
                Step = step,
                ToolResult = failedResult,
                AcceptancePassed = failedAcceptance.Passed,
                AcceptanceMessage = failedAcceptance.Message
            };
        }

        private static void NotifyStepResult(
            Action<VisionPipelineStepExecutionUpdate> stepUpdate,
            VisionPipelineStep step,
            VisionPipelineStepResult stepResult)
        {
            if (stepUpdate == null)
            {
                return;
            }

            VisionToolResult toolResult = stepResult?.ToolResult;
            VisionPipelineAcceptanceResult acceptance = new VisionPipelineAcceptanceResult
            {
                Passed = stepResult?.AcceptancePassed == true,
                Message = stepResult?.AcceptanceMessage ?? string.Empty
            };
            string resultMessage = string.IsNullOrWhiteSpace(toolResult?.Message)
                ? acceptance.Message
                : toolResult.Message;

            stepUpdate(new VisionPipelineStepExecutionUpdate
            {
                Step = step,
                Status = VisionPipelineResultSummaryService.ResolveStatus(stepResult),
                Message = resultMessage,
                StepResult = stepResult
            });
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

        private static StepRuntimeValidationResult ValidateStepConfiguration(VisionPipelineStep step)
        {
            if (step == null)
            {
                return new StepRuntimeValidationResult
                {
                    ErrorCode = VisionToolErrorCode.InvalidParameter,
                    Message = "Pipeline step is missing."
                };
            }

            if (string.IsNullOrWhiteSpace(step.InputLayer))
            {
                return new StepRuntimeValidationResult
                {
                    ErrorCode = VisionToolErrorCode.InputLayerMissing,
                    Message = $"{step.Name ?? "Step"} input layer is required."
                };
            }

            if (string.IsNullOrWhiteSpace(step.OutputLayer))
            {
                return new StepRuntimeValidationResult
                {
                    ErrorCode = VisionToolErrorCode.InvalidParameter,
                    Message = $"{step.Name} output layer is required."
                };
            }

            if (string.IsNullOrWhiteSpace(step.ToolType))
            {
                return new StepRuntimeValidationResult
                {
                    ErrorCode = VisionToolErrorCode.ToolFactoryFailed,
                    Message = $"{step.Name ?? "Step"} tool type is required."
                };
            }

            return new StepRuntimeValidationResult();
        }

        private static StepRuntimeValidationResult ValidateStepInput(VisionPipelineStep step, Mat input)
        {
            if (input == null || input.Empty())
            {
                return new StepRuntimeValidationResult
                {
                    ErrorCode = VisionToolErrorCode.InputLayerMissing,
                    Message = $"Input layer '{step?.InputLayer ?? "-"}' has no image."
                };
            }

            IDictionary<string, string> parameters = step?.Parameters;
            bool useRoi = GetBool(parameters, "USE_ROI", false);
            if (!useRoi)
            {
                return new StepRuntimeValidationResult();
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
                    return new StepRuntimeValidationResult
                    {
                        ErrorCode = VisionToolErrorCode.InvalidRoi,
                        Message = $"{step?.Name ?? "Step"} {message}"
                    };
                }

                index++;
            }

            return new StepRuntimeValidationResult();
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
