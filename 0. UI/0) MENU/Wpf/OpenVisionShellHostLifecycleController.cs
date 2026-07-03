using OpenVisionLab.Docking.Controls;
using System;
using System.Collections.Generic;

namespace OpenVisionLab
{
    internal sealed class OpenVisionShellHostLifecycleController : IOpenVisionDockLifecycle, IDisposable
    {
        private readonly List<Action> detachActions = new List<Action>();
        private bool disposed;

        public void Track(Action attach, Action detach)
        {
            if (disposed)
            {
                throw new ObjectDisposedException(nameof(OpenVisionShellHostLifecycleController));
            }

            attach?.Invoke();
            if (detach != null)
            {
                detachActions.Add(detach);
            }
        }

        public void Dispose()
        {
            if (disposed) { return; }
            disposed = true;

            // Detach in reverse order so nested subscriptions are unwound like a stack.
            for (int i = detachActions.Count - 1; i >= 0; i--)
            {
                detachActions[i]();
            }

            detachActions.Clear();
        }
    }
}
