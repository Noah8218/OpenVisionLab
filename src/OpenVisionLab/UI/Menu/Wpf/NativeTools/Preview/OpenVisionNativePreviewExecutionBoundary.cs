using OpenCvSharp;
using OpenVisionLab.Vision2D.Tool;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace OpenVisionLab
{
    internal sealed class OpenVisionNativePreviewExecutionBoundary : IDisposable
    {
        private readonly object executionSync = new object();
        private readonly OpenVisionNativePreviewExecutionController executionController;
        private readonly Action<Action> dispatchToUi;
        private CancellationTokenSource activeCancellationSource;
        private long executionGeneration;
        private bool isRunning;
        private bool disposed;

        public OpenVisionNativePreviewExecutionBoundary(
            OpenVisionNativePreviewExecutionController executionController,
            Action<Action> dispatchToUi)
        {
            this.executionController = executionController ?? throw new ArgumentNullException(nameof(executionController));
            this.dispatchToUi = dispatchToUi ?? throw new ArgumentNullException(nameof(dispatchToUi));
        }

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

        public bool TryStartSingleInput(
            string inputLayer,
            string outputLayer,
            string activationLayer,
            bool normalizeSingleChannelInput,
            Func<Mat, VisionToolResult> executePreview,
            Action<OpenVisionNativePreviewExecutionResult> applyResult,
            Action started)
        {
            if (executePreview == null)
            {
                throw new ArgumentNullException(nameof(executePreview));
            }

            if (applyResult == null)
            {
                throw new ArgumentNullException(nameof(applyResult));
            }

            CancellationTokenSource cancellationSource;
            long generation;
            lock (executionSync)
            {
                if (disposed || isRunning)
                {
                    return false;
                }

                isRunning = true;
                generation = ++executionGeneration;
                cancellationSource = new CancellationTokenSource();
                activeCancellationSource = cancellationSource;
            }

            if (!executionController.TryCaptureSingleInput(
                inputLayer,
                outputLayer,
                activationLayer,
                normalizeSingleChannelInput,
                out OpenVisionNativePreviewInputSnapshot snapshot,
                out OpenVisionNativePreviewExecutionResult captureFailure))
            {
                EndRun(generation, cancellationSource);
                applyResult(captureFailure);
                return true;
            }

            started?.Invoke();
            _ = RunSingleInputAsync(
                generation,
                cancellationSource,
                snapshot,
                executePreview,
                applyResult);
            return true;
        }

        public bool Cancel()
        {
            CancellationTokenSource cancellationSource;
            lock (executionSync)
            {
                if (!isRunning || disposed)
                {
                    return false;
                }

                cancellationSource = activeCancellationSource;
            }

            cancellationSource?.Cancel();
            return true;
        }

        public bool CancelAndDiscard()
        {
            CancellationTokenSource cancellationSource;
            lock (executionSync)
            {
                if (!isRunning || disposed)
                {
                    return false;
                }

                executionGeneration++;
                cancellationSource = activeCancellationSource;
            }

            cancellationSource?.Cancel();
            return true;
        }

        public void Dispose()
        {
            CancellationTokenSource cancellationSource;
            lock (executionSync)
            {
                if (disposed)
                {
                    return;
                }

                disposed = true;
                executionGeneration++;
                isRunning = false;
                cancellationSource = activeCancellationSource;
                activeCancellationSource = null;
            }

            cancellationSource?.Cancel();
            cancellationSource?.Dispose();
        }

        private async Task RunSingleInputAsync(
            long generation,
            CancellationTokenSource cancellationSource,
            OpenVisionNativePreviewInputSnapshot snapshot,
            Func<Mat, VisionToolResult> executePreview,
            Action<OpenVisionNativePreviewExecutionResult> applyResult)
        {
            OpenVisionNativePreviewComputation computation = null;
            try
            {
                computation = await Task.Run(
                    () => executionController.ComputeSingleInput(snapshot, executePreview, cancellationSource.Token))
                    .ConfigureAwait(false);

                OpenVisionNativePreviewComputation completedComputation = computation;
                dispatchToUi(() => CompleteOnUi(
                    generation,
                    cancellationSource,
                    snapshot,
                    completedComputation,
                    applyResult));
                computation = null;
            }
            catch (Exception ex)
            {
                computation?.Dispose();
                computation = null;
                try
                {
                    dispatchToUi(() => CompleteOnUi(
                        generation,
                        cancellationSource,
                        snapshot,
                        OpenVisionNativePreviewComputation.Failed(ex.GetBaseException().Message),
                        applyResult));
                }
                catch
                {
                    snapshot.Dispose();
                    EndRun(generation, cancellationSource);
                }
            }
        }

        private void CompleteOnUi(
            long generation,
            CancellationTokenSource cancellationSource,
            OpenVisionNativePreviewInputSnapshot snapshot,
            OpenVisionNativePreviewComputation computation,
            Action<OpenVisionNativePreviewExecutionResult> applyResult)
        {
            try
            {
                if (!IsCurrentRun(generation, cancellationSource))
                {
                    return;
                }

                OpenVisionNativePreviewExecutionResult result = executionController.PublishComputedResult(snapshot, computation);
                applyResult(result);
            }
            catch (Exception ex)
            {
                if (IsCurrentRun(generation, cancellationSource))
                {
                    applyResult(OpenVisionNativePreviewExecutionResult.Failed(
                        "Preview NG / " + VisionToolVerificationText.InspectionJudgmentNotEvaluated + " / " + ex.GetBaseException().Message));
                }
            }
            finally
            {
                computation?.Dispose();
                snapshot.Dispose();
                EndRun(generation, cancellationSource);
            }
        }

        private bool IsCurrentRun(long generation, CancellationTokenSource cancellationSource)
        {
            lock (executionSync)
            {
                return !disposed
                    && isRunning
                    && executionGeneration == generation
                    && ReferenceEquals(activeCancellationSource, cancellationSource);
            }
        }

        private void EndRun(long generation, CancellationTokenSource cancellationSource)
        {
            bool shouldDisposeCancellationSource = false;
            lock (executionSync)
            {
                if (ReferenceEquals(activeCancellationSource, cancellationSource))
                {
                    isRunning = false;
                    activeCancellationSource = null;
                    shouldDisposeCancellationSource = true;
                }
            }

            if (shouldDisposeCancellationSource)
            {
                cancellationSource.Dispose();
            }
        }
    }
}
