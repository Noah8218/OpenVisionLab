using Lib.Common;
using Lib.OpenCV;
using Lib.OpenCV.Pipeline;
using Lib.OpenCV.Tool;
using OpenCvSharp;
using OpenVisionLab.Core;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;

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
            Bitmap sourceBitmap = displayManager.GetLayerImage(inputLayer);
            if (sourceBitmap == null)
            {
                return OpenVisionNativePreviewExecutionResult.Failed("Preview NG / input image missing");
            }

            Stopwatch stopwatch = Stopwatch.StartNew();
            VisionToolResult result = null;
            try
            {
                using Mat source = BitmapImageConverter.ToMat(sourceBitmap);
                if (normalizeSingleChannelInput)
                {
                    OpenCvHelper.SetImageChannel1(source);
                }

                result = executePreview(source);
                stopwatch.Stop();
                return PublishResult(outputLayer, activationLayer, result, stopwatch.Elapsed, "Preview");
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                return OpenVisionNativePreviewExecutionResult.Failed("Preview NG / " + ex.GetBaseException().Message);
            }
            finally
            {
                result?.ResultImage?.Dispose();
            }
        }

        public OpenVisionNativePreviewExecutionResult RunArithmetic(
            VisionPipelineStep step,
            string inputLayerA,
            string outputLayer,
            string activationLayer,
            bool useOffsetMode)
        {
            Bitmap sourceA = displayManager.GetLayerImage(inputLayerA);
            if (sourceA == null)
            {
                return OpenVisionNativePreviewExecutionResult.Failed("Preview NG / input A image missing");
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
                    Bitmap sourceB = displayManager.GetLayerImage(inputB);
                    if (sourceB == null)
                    {
                        return OpenVisionNativePreviewExecutionResult.Failed("Preview NG / input B image missing");
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
                return OpenVisionNativePreviewExecutionResult.Failed("Preview NG / " + ex.GetBaseException().Message);
            }
            finally
            {
                result?.ResultImage?.Dispose();
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
                return OpenVisionNativePreviewExecutionResult.Failed("Preview NG / " + message);
            }

            using Bitmap resultBitmap = BitmapImageConverter.ToBitmap(result.ResultImage);
            // Publishing may create/focus the output layer; the publisher restores activationLayer afterward.
            previewLayerPublisher.PublishPreviewBitmap(outputLayer, activationLayer, resultBitmap, elapsed);
            string status = string.Format(
                CultureInfo.CurrentCulture,
                "{0} OK / {1} / {2}x{3}",
                string.IsNullOrWhiteSpace(successLabel) ? "Preview" : successLabel,
                outputLayer,
                resultBitmap.Width,
                resultBitmap.Height);
            return OpenVisionNativePreviewExecutionResult.Passed(status);
        }
    }

    internal sealed class OpenVisionNativePreviewExecutionResult
    {
        private OpenVisionNativePreviewExecutionResult(bool success, string status)
        {
            Success = success;
            Status = status ?? string.Empty;
        }

        public bool Success { get; }

        public string Status { get; }

        public static OpenVisionNativePreviewExecutionResult Passed(string status)
        {
            return new OpenVisionNativePreviewExecutionResult(true, status);
        }

        public static OpenVisionNativePreviewExecutionResult Failed(string status)
        {
            return new OpenVisionNativePreviewExecutionResult(false, status);
        }
    }
}
