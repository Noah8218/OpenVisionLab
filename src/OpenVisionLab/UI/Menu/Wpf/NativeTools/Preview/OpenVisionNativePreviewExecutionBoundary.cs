using OpenCvSharp;
using OpenVisionLab.Vision2D.Tool;
using OpenVisionLab.Vision2D.Pipeline;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace OpenVisionLab
{
    internal sealed class OpenVisionNativePreviewExecutionBoundary : IDisposable
    {
        private readonly object executionSync = new object();
        private readonly OpenVisionNativePreviewExecutionController executionController;
        private readonly Action<Action> dispatchToUi;
        private readonly Action executionStateChanged;
        private CancellationTokenSource activeCancellationSource;
        private long executionGeneration;
        private bool isRunning;
        private bool disposed;

        public OpenVisionNativePreviewExecutionBoundary(
            OpenVisionNativePreviewExecutionController executionController,
            Action<Action> dispatchToUi,
            Action executionStateChanged = null)
        {
            this.executionController = executionController ?? throw new ArgumentNullException(nameof(executionController));
            this.dispatchToUi = dispatchToUi ?? throw new ArgumentNullException(nameof(dispatchToUi));
            this.executionStateChanged = executionStateChanged;
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
            Action started,
            Func<OpenVisionNativePreviewOperation> capturePreview = null,
            VisionPipelineStep resultStep = null)
        {
            if (executePreview == null && capturePreview == null)
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

            NotifyExecutionStateChanged();
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

            OpenVisionNativePreviewOperation operation = null;
            try
            {
                // Capture control-bound parameters on the caller's UI thread, before any worker starts.
                operation = capturePreview == null
                    ? new OpenVisionNativePreviewOperation(executePreview)
                    : capturePreview() ?? throw new InvalidOperationException("Preview operation was not captured.");
                if (!operation.IsTeachingPreview || resultStep == null) { started?.Invoke(); }
                _ = RunSingleInputAsync(generation, cancellationSource, snapshot, operation, applyResult, resultStep);
            }
            catch (Exception ex)
            {
                Exception cleanupFailure = ReleaseResources(operation, null, snapshot);
                EndRun(generation, cancellationSource);
                Exception failure = cleanupFailure == null ? ex : new AggregateException(ex, cleanupFailure);
                applyResult(OpenVisionNativePreviewExecutionResult.Failed(
                    "Preview NG / " + VisionToolVerificationText.InspectionJudgmentNotEvaluated + " / " + failure.GetBaseException().Message));
            }
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
            NotifyExecutionStateChanged();
        }

        private async Task RunSingleInputAsync(
            long generation,
            CancellationTokenSource cancellationSource,
            OpenVisionNativePreviewInputSnapshot snapshot,
            OpenVisionNativePreviewOperation operation,
            Action<OpenVisionNativePreviewExecutionResult> applyResult,
            VisionPipelineStep resultStep)
        {
            OpenVisionNativePreviewComputation computation = null;
            try
            {
                computation = await Task.Run(
                    () => executionController.ComputeSingleInput(snapshot, operation.Execute, cancellationSource.Token, operation.IsTeachingPreview ? null : resultStep))
                    .ConfigureAwait(false);

                OpenVisionNativePreviewComputation completedComputation = computation;
                dispatchToUi(() => CompleteOnUi(
                    generation,
                    cancellationSource,
                    snapshot,
                    completedComputation,
                    operation,
                    applyResult));
                computation = null;
            }
            catch (Exception ex)
            {
                Exception computationCleanupFailure = ReleaseResources(null, computation, null);
                computation = null;
                Exception failure = computationCleanupFailure == null ? ex : new AggregateException(ex, computationCleanupFailure);
                try
                {
                    dispatchToUi(() => CompleteOnUi(
                        generation,
                        cancellationSource,
                        snapshot,
                        OpenVisionNativePreviewComputation.Failed(failure.GetBaseException().Message),
                        operation,
                        applyResult));
                }
                catch (Exception dispatchFailure)
                {
                    Exception cleanupFailure = ReleaseResources(operation, null, snapshot);
                    EndRun(generation, cancellationSource);
                    Trace.TraceError("Native preview UI dispatch failed: {0}", new AggregateException(failure, dispatchFailure));
                    if (cleanupFailure != null) Trace.TraceError("Native preview cleanup failed: {0}", cleanupFailure);
                }
            }
        }

        private void CompleteOnUi(
            long generation,
            CancellationTokenSource cancellationSource,
            OpenVisionNativePreviewInputSnapshot snapshot,
            OpenVisionNativePreviewComputation computation,
            OpenVisionNativePreviewOperation operation,
            Action<OpenVisionNativePreviewExecutionResult> applyResult)
        {
            OpenVisionNativePreviewExecutionResult result = null;
            try
            {
                if (!IsCurrentRun(generation, cancellationSource))
                {
                    return;
                }

                result = executionController.PublishComputedResult(
                    snapshot,
                    cancellationSource.IsCancellationRequested ? OpenVisionNativePreviewComputation.CanceledResult() : computation).WithSummary(
                    cancellationSource.IsCancellationRequested ? null : computation?.Summary);
                if (!result.Canceled && (result.Success || operation.PublishEvidenceOnFailure))
                {
                    operation.PublishEvidence?.Invoke();
                }
            }
            catch (Exception ex)
            {
                result = OpenVisionNativePreviewExecutionResult.Failed(
                    "Preview NG / " + VisionToolVerificationText.InspectionJudgmentNotEvaluated + " / " + ex.GetBaseException().Message);
            }
            finally
            {
                Exception cleanupFailure = ReleaseResources(operation, computation, snapshot);
                try
                {
                    if (cleanupFailure != null)
                    {
                        Trace.TraceError("Native preview cleanup failed: {0}", cleanupFailure);
                        string status = result?.Success == false ? result.Status : "Preview NG / " + VisionToolVerificationText.InspectionJudgmentNotEvaluated;
                        result = OpenVisionNativePreviewExecutionResult.Failed(status + " / cleanup failed: " + cleanupFailure.GetBaseException().Message);
                    }
                    if (result != null && IsCurrentRun(generation, cancellationSource))
                        applyResult(result.WithSummary(operation.IsTeachingPreview ? null : result.Summary, operation.IsTeachingPreview));
                }
                finally
                {
                    EndRun(generation, cancellationSource);
                }
            }
        }

        private static Exception ReleaseResources(OpenVisionNativePreviewOperation operation, OpenVisionNativePreviewComputation computation, OpenVisionNativePreviewInputSnapshot snapshot)
        {
            Exception failure = null;
            foreach (IDisposable resource in new IDisposable[] { operation, computation, snapshot })
            {
                try
                {
                    resource?.Dispose();
                }
                catch (Exception ex)
                {
                    failure = failure == null ? ex : new AggregateException(failure, ex);
                }
            }
            return failure;
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
                NotifyExecutionStateChanged();
            }
        }

        private void NotifyExecutionStateChanged()
        {
            try { executionStateChanged?.Invoke(); }
            catch (Exception ex) { Trace.TraceError("Native preview execution state notification failed: {0}", ex); }
        }
    }

    // One accepted run owns its captured inputs and managed evidence until the guarded UI completion.
    internal sealed class OpenVisionNativePreviewOperation : IDisposable
    {
        private Action release;

        public OpenVisionNativePreviewOperation(Func<Mat, VisionToolResult> execute, Action publishEvidence = null, bool publishEvidenceOnFailure = false, Action release = null, bool isTeachingPreview = false)
        {
            Execute = execute ?? throw new ArgumentNullException(nameof(execute));
            PublishEvidence = publishEvidence;
            PublishEvidenceOnFailure = publishEvidenceOnFailure;
            IsTeachingPreview = isTeachingPreview;
            this.release = release;
        }

        public Func<Mat, VisionToolResult> Execute { get; }

        public Action PublishEvidence { get; }

        public bool PublishEvidenceOnFailure { get; }

        public bool IsTeachingPreview { get; }

        public void Dispose() => Interlocked.Exchange(ref release, null)?.Invoke();
    }
}
