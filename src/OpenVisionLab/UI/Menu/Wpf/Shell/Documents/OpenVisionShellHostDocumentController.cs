using OpenVisionLab.Core;
using System;
using static OpenVisionLab.DEFINE;

namespace OpenVisionLab
{
    internal sealed class OpenVisionShellHostDocumentController : IDisposable
    {
        private readonly EventHandler layerStateChanged;
        private readonly OpenVisionNativeToolDocumentCache nativeToolDocuments = new OpenVisionNativeToolDocumentCache();
        private OpenVisionPipelineReviewDocument cachedPipelineReviewDocument;
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

        public bool HasPipelineReviewDocument =>
            ActivePipelineReviewDocument != null || cachedPipelineReviewDocument != null;

        public bool TryCachePipelineReview(OpenVisionPipelineReviewDocument document)
        {
            ThrowIfDisposed();
            if (document == null)
            {
                throw new ArgumentNullException(nameof(document));
            }

            if (ActivePipelineReviewDocument != null || cachedPipelineReviewDocument != null)
            {
                return false;
            }

            cachedPipelineReviewDocument = document;
            return true;
        }

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

        public bool TryRestorePipelineReview(
            OpenVisionRecipeContext recipeContext,
            out OpenVisionPipelineReviewDocument document)
        {
            ThrowIfDisposed();
            document = null;
            if (ActivePipelineReviewDocument != null)
            {
                if (IsSamePipelineContext(ActivePipelineReviewDocument.RecipeContext, recipeContext))
                {
                    document = ActivePipelineReviewDocument;
                    return true;
                }

                CloseActivePipelineReviewDocument();
            }

            if (!IsSamePipelineContext(cachedPipelineReviewDocument?.RecipeContext, recipeContext))
            {
                CloseCachedPipelineReviewDocument();
                return false;
            }

            ActivePipelineReviewDocument = cachedPipelineReviewDocument;
            cachedPipelineReviewDocument = null;
            ActivePipelineReviewDocument.LayerStateChanged += layerStateChanged;
            document = ActivePipelineReviewDocument;
            return true;
        }

        public bool SuspendPipelineReviewForRecipeReturn()
        {
            ThrowIfDisposed();
            if (ActivePipelineReviewDocument == null)
            {
                return false;
            }

            CloseCachedPipelineReviewDocument();
            cachedPipelineReviewDocument = ActivePipelineReviewDocument;
            ActivePipelineReviewDocument = null;
            cachedPipelineReviewDocument.LayerStateChanged -= layerStateChanged;
            return true;
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
            CloseCachedPipelineReviewDocument();
            DisposeCachedNativeToolDocuments();
        }

        public void DeactivateForToolSwitch()
        {
            CloseVisibleDocuments();
            CloseCachedPipelineReviewDocument();
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

        private void CloseCachedPipelineReviewDocument()
        {
            OpenVisionPipelineReviewDocument document = cachedPipelineReviewDocument;
            cachedPipelineReviewDocument = null;
            document?.Dispose();
        }

        private static bool IsSamePipelineContext(
            OpenVisionRecipeContext current,
            OpenVisionRecipeContext requested)
        {
            return current != null
                && requested != null
                && string.Equals(current.Id, requested.Id, StringComparison.Ordinal)
                && string.Equals(current.Name, requested.Name, StringComparison.Ordinal)
                && string.Equals(current.PipelineName, requested.PipelineName, StringComparison.Ordinal)
                && string.Equals(current.SourcePath, requested.SourcePath, StringComparison.Ordinal);
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
