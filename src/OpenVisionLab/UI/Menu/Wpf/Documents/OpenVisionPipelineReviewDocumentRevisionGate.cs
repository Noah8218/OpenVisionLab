using System;

namespace OpenVisionLab
{
    internal readonly struct OpenVisionPipelineReviewDocumentRevision
    {
        internal OpenVisionPipelineReviewDocumentRevision(
            long inputRevision,
            long recipeRevision,
            long runGeneration)
        {
            InputRevision = inputRevision;
            RecipeRevision = recipeRevision;
            RunGeneration = runGeneration;
        }

        internal long InputRevision { get; }

        internal long RecipeRevision { get; }

        internal long RunGeneration { get; }
    }

    internal sealed class OpenVisionPipelineReviewDocumentRevisionGate : IDisposable
    {
        private readonly object revisionSync = new object();
        private long inputRevision;
        private long recipeRevision;
        private long runGeneration;
        private bool disposed;

        internal OpenVisionPipelineReviewDocumentRevision InvalidateRecipe()
        {
            lock (revisionSync)
            {
                recipeRevision++;
                inputRevision++;
                runGeneration++;
                return Snapshot();
            }
        }

        internal OpenVisionPipelineReviewDocumentRevision InvalidateInput()
        {
            lock (revisionSync)
            {
                inputRevision++;
                runGeneration++;
                return Snapshot();
            }
        }

        internal OpenVisionPipelineReviewDocumentRevision BeginRun()
        {
            lock (revisionSync)
            {
                runGeneration++;
                return Snapshot();
            }
        }

        internal bool IsCurrent(OpenVisionPipelineReviewDocumentRevision revision)
        {
            lock (revisionSync)
            {
                return !disposed
                    && inputRevision == revision.InputRevision
                    && recipeRevision == revision.RecipeRevision
                    && runGeneration == revision.RunGeneration;
            }
        }

        internal bool IsCurrentRevision(long inputRevision, long recipeRevision)
        {
            lock (revisionSync)
            {
                return !disposed
                    && this.inputRevision == inputRevision
                    && this.recipeRevision == recipeRevision;
            }
        }

        public void Dispose()
        {
            lock (revisionSync)
            {
                if (disposed)
                {
                    return;
                }

                disposed = true;
                runGeneration++;
            }
        }

        private OpenVisionPipelineReviewDocumentRevision Snapshot()
        {
            return new OpenVisionPipelineReviewDocumentRevision(
                inputRevision,
                recipeRevision,
                runGeneration);
        }
    }
}
