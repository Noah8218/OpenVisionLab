using OpenVisionLab.Common;
using OpenVisionLab.Core;
using OpenVisionLab.Vision2D;
using OpenVisionLab.Vision2D.Pipeline;
using OpenVisionLab.Vision2D.Tool;
using OpenCvSharp;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Threading;

namespace OpenVisionLab
{
    internal sealed class OpenVisionNativePreviewExecutionController
    {
        private readonly IDisplayManager displayManager;
        private readonly OpenVisionNativePreviewLayerPublisher previewLayerPublisher;

        public OpenVisionNativePreviewExecutionController(
            IDisplayManager displayManager,
            OpenVisionNativePreviewLayerPublisher previewLayerPublisher)
        {
            this.displayManager = displayManager ?? throw new ArgumentNullException(nameof(displayManager));
            this.previewLayerPublisher = previewLayerPublisher ?? throw new ArgumentNullException(nameof(previewLayerPublisher));
        }

        public OpenVisionNativePreviewExecutionResult RunSingleInput(
            string inputLayer,
            string outputLayer,
            string activationLayer,
            bool normalizeSingleChannelInput,
            Func<Mat, VisionToolResult> executePreview)
        {
            if (!TryCaptureSingleInput(
                inputLayer,
                outputLayer,
                activationLayer,
                normalizeSingleChannelInput,
                out OpenVisionNativePreviewInputSnapshot snapshot,
                out OpenVisionNativePreviewExecutionResult captureFailure))
            {
                return captureFailure;
            }

            using (snapshot)
            {
                using OpenVisionNativePreviewComputation computation = ComputeSingleInput(
                    snapshot,
                    executePreview,
                    CancellationToken.None);
                return PublishComputedResult(outputLayer, activationLayer, computation, "Preview");
            }
        }

        public bool TryCaptureSingleInput(
            string inputLayer,
            string outputLayer,
            string activationLayer,
            bool normalizeSingleChannelInput,
            out OpenVisionNativePreviewInputSnapshot snapshot,
            out OpenVisionNativePreviewExecutionResult failure)
        {
            snapshot = null;
            try
            {
                using Bitmap sourceBitmap = displayManager.GetLayerImageSnapshot(inputLayer);
                if (sourceBitmap == null)
                {
                    failure = FailedStatus("Preview", "input image missing");
                    return false;
                }

                snapshot = new OpenVisionNativePreviewInputSnapshot(
                    new Bitmap(sourceBitmap),
                    outputLayer,
                    activationLayer,
                    normalizeSingleChannelInput);
                failure = null;
                return true;
            }
            catch (Exception ex)
            {
                failure = FailedStatus("Preview", ex.GetBaseException().Message);
                return false;
            }
        }

        public OpenVisionNativePreviewComputation ComputeSingleInput(
            OpenVisionNativePreviewInputSnapshot snapshot,
            Func<Mat, VisionToolResult> executePreview,
            CancellationToken cancellationToken)
        {
            if (snapshot == null)
            {
                return OpenVisionNativePreviewComputation.Failed("input snapshot missing");
            }

            Stopwatch stopwatch = Stopwatch.StartNew();
            VisionToolResult result = null;
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                using Mat source = BitmapImageConverter.ToMat(snapshot.SourceBitmap);
                if (snapshot.NormalizeSingleChannelInput)
                {
                    OpenCvHelper.SetImageChannel1(source);
                }

                cancellationToken.ThrowIfCancellationRequested();
                result = executePreview(source);
                stopwatch.Stop();
                cancellationToken.ThrowIfCancellationRequested();
                if (result == null || !result.Success || result.ResultImage == null || result.ResultImage.Empty())
                {
                    string message;
                    if (result == null)
                    {
                        message = "tool returned no result";
                    }
                    else if (!result.Success)
                    {
                        message = string.IsNullOrWhiteSpace(result.Message) ? "tool returned failure" : result.Message;
                    }
                    else if (result.ResultImage == null)
                    {
                        message = "tool returned no image";
                    }
                    else if (result.ResultImage.Empty())
                    {
                        message = string.Format(
                            CultureInfo.InvariantCulture,
                            "tool returned an empty image (source {0}x{1}/{2})",
                            source.Width,
                            source.Height,
                            source.Channels());
                    }
                    else
                    {
                        message = "tool returned an inconsistent result";
                    }
                    return OpenVisionNativePreviewComputation.Failed(message);
                }

                Bitmap resultBitmap = BitmapImageConverter.ToBitmap(result.ResultImage);
                return OpenVisionNativePreviewComputation.Passed(resultBitmap, stopwatch.Elapsed);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                stopwatch.Stop();
                return OpenVisionNativePreviewComputation.CanceledResult();
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                return OpenVisionNativePreviewComputation.Failed(ex.GetBaseException().Message);
            }
            finally
            {
                result?.Dispose();
            }
        }

        public OpenVisionNativePreviewExecutionResult PublishComputedResult(
            OpenVisionNativePreviewInputSnapshot snapshot,
            OpenVisionNativePreviewComputation computation)
        {
            return PublishComputedResult(
                snapshot?.OutputLayer,
                snapshot?.ActivationLayer,
                computation,
                "Preview");
        }

        public OpenVisionNativePreviewExecutionResult RunArithmetic(
            VisionPipelineStep step,
            string inputLayerA,
            string outputLayer,
            string activationLayer,
            bool useOffsetMode)
        {
            using Bitmap sourceA = displayManager.GetLayerImageSnapshot(inputLayerA);
            if (sourceA == null)
            {
                return FailedStatus("Preview", "input A image missing");
            }

            Stopwatch stopwatch = Stopwatch.StartNew();
            VisionToolResult result = null;
            try
            {
                using VisionPipelineContext context = new VisionPipelineContext();
                using Mat matA = BitmapImageConverter.ToMat(sourceA);

                if (VisionPipelineArithmeticStep.RequiresInputLayerB(step))
                {
                    string inputB = VisionPipelineArithmeticStep.GetInputLayerB(step);
                    using Bitmap sourceB = displayManager.GetLayerImageSnapshot(inputB);
                    if (sourceB == null)
                    {
                        return FailedStatus("Preview", "input B image missing");
                    }

                    using Mat matB = BitmapImageConverter.ToMat(sourceB);
                    // Arithmetic execution reads InputB from this context before matB leaves scope.
                    context.SetLayer(inputB, matB);
                    result = VisionPipelineArithmeticStep.Execute(step, matA, context);
                }
                else
                {
                    result = VisionPipelineArithmeticStep.Execute(step, matA, context);
                }

                stopwatch.Stop();
                return PublishResult(outputLayer, activationLayer, result, stopwatch.Elapsed, useOffsetMode ? "Offset" : "Preview");
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                return FailedStatus("Preview", ex.GetBaseException().Message);
            }
            finally
            {
                result?.Dispose();
            }
        }

        private OpenVisionNativePreviewExecutionResult PublishResult(
            string outputLayer,
            string activationLayer,
            VisionToolResult result,
            TimeSpan elapsed,
            string successLabel)
        {
            if (result == null || !result.Success || result.ResultImage == null || result.ResultImage.Empty())
            {
                string message = string.IsNullOrWhiteSpace(result?.Message)
                    ? "tool returned no result"
                    : result.Message;
                return FailedStatus("Preview", message);
            }

            using Bitmap resultBitmap = BitmapImageConverter.ToBitmap(result.ResultImage);
            return PublishResultBitmap(outputLayer, activationLayer, resultBitmap, elapsed, successLabel);
        }

        private OpenVisionNativePreviewExecutionResult PublishComputedResult(
            string outputLayer,
            string activationLayer,
            OpenVisionNativePreviewComputation computation,
            string successLabel)
        {
            if (computation == null)
            {
                return FailedStatus("Preview", "tool returned no result");
            }

            if (computation.Canceled)
            {
                return OpenVisionNativePreviewExecutionResult.CanceledResult(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        "Preview CANCELED / {0} / operation canceled",
                        VisionToolVerificationText.InspectionJudgmentNotEvaluated));
            }

            if (!computation.Success || computation.ResultBitmap == null)
            {
                return FailedStatus("Preview", computation.Status);
            }

            return PublishResultBitmap(
                outputLayer,
                activationLayer,
                computation.ResultBitmap,
                computation.Elapsed,
                successLabel);
        }

        private OpenVisionNativePreviewExecutionResult PublishResultBitmap(
            string outputLayer,
            string activationLayer,
            Bitmap resultBitmap,
            TimeSpan elapsed,
            string successLabel)
        {
            if (resultBitmap == null)
            {
                return FailedStatus("Preview", "tool returned no result");
            }

            // Publishing may create/focus the output layer; the publisher restores activationLayer afterward.
            previewLayerPublisher.PublishPreviewBitmap(outputLayer, activationLayer, resultBitmap, elapsed);
            string status = string.Format(
                CultureInfo.CurrentCulture,
                "{0} OK / {1} / {2} / {3}x{4}",
                string.IsNullOrWhiteSpace(successLabel) ? "Preview" : successLabel,
                VisionToolVerificationText.InspectionJudgmentNotEvaluated,
                outputLayer,
                resultBitmap.Width,
                resultBitmap.Height);
            return OpenVisionNativePreviewExecutionResult.Passed(status);
        }

        private static OpenVisionNativePreviewExecutionResult FailedStatus(string statusLabel, string detail)
        {
            string prefix = string.IsNullOrWhiteSpace(statusLabel) ? "Preview" : statusLabel.Trim();
            string message = string.IsNullOrWhiteSpace(detail) ? "tool execution failed" : detail.Trim();
            return OpenVisionNativePreviewExecutionResult.Failed(
                string.Format(
                    CultureInfo.CurrentCulture,
                    "{0} NG / {1} / {2}",
                    prefix,
                    VisionToolVerificationText.InspectionJudgmentNotEvaluated,
                    message));
        }
    }

    internal sealed class OpenVisionNativePreviewExecutionResult
    {
        private OpenVisionNativePreviewExecutionResult(bool success, bool canceled, string status)
        {
            Success = success;
            Canceled = canceled;
            Status = status ?? string.Empty;
        }

        public bool Success { get; }

        public bool Canceled { get; }

        public string Status { get; }

        public static OpenVisionNativePreviewExecutionResult Passed(string status)
        {
            return new OpenVisionNativePreviewExecutionResult(true, false, status);
        }

        public static OpenVisionNativePreviewExecutionResult Failed(string status)
        {
            return new OpenVisionNativePreviewExecutionResult(false, false, status);
        }

        public static OpenVisionNativePreviewExecutionResult CanceledResult(string status)
        {
            return new OpenVisionNativePreviewExecutionResult(false, true, status);
        }
    }

    internal sealed class OpenVisionNativePreviewInputSnapshot : IDisposable
    {
        public OpenVisionNativePreviewInputSnapshot(
            Bitmap sourceBitmap,
            string outputLayer,
            string activationLayer,
            bool normalizeSingleChannelInput)
        {
            SourceBitmap = sourceBitmap ?? throw new ArgumentNullException(nameof(sourceBitmap));
            OutputLayer = outputLayer;
            ActivationLayer = activationLayer;
            NormalizeSingleChannelInput = normalizeSingleChannelInput;
        }

        public Bitmap SourceBitmap { get; }

        public string OutputLayer { get; }

        public string ActivationLayer { get; }

        public bool NormalizeSingleChannelInput { get; }

        public void Dispose()
        {
            SourceBitmap.Dispose();
        }
    }

    internal sealed class OpenVisionNativePreviewComputation : IDisposable
    {
        private OpenVisionNativePreviewComputation(
            bool success,
            bool canceled,
            string status,
            Bitmap resultBitmap,
            TimeSpan elapsed)
        {
            Success = success;
            Canceled = canceled;
            Status = status ?? string.Empty;
            ResultBitmap = resultBitmap;
            Elapsed = elapsed;
        }

        public bool Success { get; }

        public bool Canceled { get; }

        public string Status { get; }

        public Bitmap ResultBitmap { get; private set; }

        public TimeSpan Elapsed { get; }

        public static OpenVisionNativePreviewComputation Passed(Bitmap resultBitmap, TimeSpan elapsed)
        {
            return new OpenVisionNativePreviewComputation(true, false, string.Empty, resultBitmap, elapsed);
        }

        public static OpenVisionNativePreviewComputation Failed(string status)
        {
            return new OpenVisionNativePreviewComputation(false, false, status, null, TimeSpan.Zero);
        }

        public static OpenVisionNativePreviewComputation CanceledResult()
        {
            return new OpenVisionNativePreviewComputation(false, true, "operation canceled", null, TimeSpan.Zero);
        }

        public void Dispose()
        {
            ResultBitmap?.Dispose();
            ResultBitmap = null;
        }
    }
}
