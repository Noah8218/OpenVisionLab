using OpenVisionLab.Core;
using System;
using static OpenVisionLab.DEFINE;

namespace OpenVisionLab
{
    internal sealed class OpenVisionShellHostDocumentController : IDisposable
    {
        private readonly EventHandler layerStateChanged;
        private readonly OpenVisionNativeToolDocumentCache nativeToolDocuments = new OpenVisionNativeToolDocumentCache();
        private bool disposed;

        public OpenVisionShellHostDocumentController(EventHandler layerStateChanged)
        {
            this.layerStateChanged = layerStateChanged ?? throw new ArgumentNullException(nameof(layerStateChanged));
        }

        public OpenVisionNativeToolDocument ActiveNativeDocument { get; private set; }

        public OpenVisionPipelineReviewDocument ActivePipelineReviewDocument { get; private set; }

        public OpenVisionPendingToolViewModel ActivePendingToolViewModel { get; private set; }

        public OpenVisionNativeToolDocumentCache NativeToolDocuments => nativeToolDocuments;

        public int NativeToolDocumentCacheCount => nativeToolDocuments.Count;

        public int HostedDocumentCount =>
            (ActivePendingToolViewModel == null ? 0 : 1) +
            (ActiveNativeDocument == null && ActivePipelineReviewDocument == null ? 0 : 1);

        public bool IsNativeDocumentActive =>
            ActiveNativeDocument != null || ActivePipelineReviewDocument != null;

        public void ActivatePipelineReview(OpenVisionPipelineReviewDocument document)
        {
            ThrowIfDisposed();
            if (document == null)
            {
                throw new ArgumentNullException(nameof(document));
            }

            DeactivateForToolSwitch();
            ActivePipelineReviewDocument = document;
            ActivePipelineReviewDocument.LayerStateChanged += layerStateChanged;
        }

        public bool TryActivateNativeTool(
            VISION_MENU menu,
            IDisplayManager displayManager,
            OpenVisionRecipeContext recipeContext,
            out OpenVisionNativeToolDocument document)
        {
            ThrowIfDisposed();
            DeactivateForToolSwitch();

            if (!nativeToolDocuments.TryGetOrCreate(menu, displayManager, recipeContext, out document))
            {
                return false;
            }

            ActiveNativeDocument = document;
            ActiveNativeDocument.LayerStateChanged += layerStateChanged;
            return true;
        }

        public void ActivatePendingTool(OpenVisionPendingToolViewModel viewModel)
        {
            ThrowIfDisposed();
            if (viewModel == null)
            {
                throw new ArgumentNullException(nameof(viewModel));
            }

            DeactivateForToolSwitch();
            ActivePendingToolViewModel = viewModel;
        }

        public void CloseVisibleDocuments()
        {
            CloseActivePipelineReviewDocument();
            DetachActiveNativeDocument();
            CloseActivePendingTool();
        }

        public void CloseAllDocuments()
        {
            CloseVisibleDocuments();
            DisposeCachedNativeToolDocuments();
        }

        public void DeactivateForToolSwitch()
        {
            CloseVisibleDocuments();
        }

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            disposed = true;
            CloseAllDocuments();
        }

        private void CloseActivePipelineReviewDocument()
        {
            if (ActivePipelineReviewDocument == null)
            {
                return;
            }

            OpenVisionPipelineReviewDocument document = ActivePipelineReviewDocument;
            ActivePipelineReviewDocument = null;
            document.LayerStateChanged -= layerStateChanged;
            document.Dispose();
        }

        private void DetachActiveNativeDocument()
        {
            if (ActiveNativeDocument == null)
            {
                return;
            }

            OpenVisionNativeToolDocument document = ActiveNativeDocument;
            ActiveNativeDocument = null;
            document.LayerStateChanged -= layerStateChanged;
        }

        private void DisposeCachedNativeToolDocuments()
        {
            // Native tool documents are cached for warm tool switching; dispose them only when the host is closing.
            nativeToolDocuments.DisposeAll(document => document.LayerStateChanged -= layerStateChanged);
            ActiveNativeDocument = null;
        }

        private void CloseActivePendingTool()
        {
            if (ActivePendingToolViewModel == null)
            {
                return;
            }

            ActivePendingToolViewModel.Dispose();
            ActivePendingToolViewModel = null;
        }

        private void ThrowIfDisposed()
        {
            if (disposed)
            {
                throw new ObjectDisposedException(nameof(OpenVisionShellHostDocumentController));
            }
        }
    }
}
