using System;
using System.Windows.Threading;

namespace OpenVisionLab.Docking.Controls
{
    public sealed class OpenVisionDockWorkspaceStateSaveScheduler
    {
        private readonly DispatcherTimer timer;
        private readonly Func<bool> canQueueSave;
        private readonly Action saveWorkspaceState;

        public OpenVisionDockWorkspaceStateSaveScheduler(
            TimeSpan delay,
            Func<bool> canQueueSave,
            Action saveWorkspaceState)
        {
            if (delay <= TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(delay));
            }

            this.canQueueSave = canQueueSave ?? throw new ArgumentNullException(nameof(canQueueSave));
            this.saveWorkspaceState = saveWorkspaceState ?? throw new ArgumentNullException(nameof(saveWorkspaceState));
            timer = new DispatcherTimer { Interval = delay };
        }

        public void Attach(IOpenVisionDockLifecycle lifecycle)
        {
            if (lifecycle == null)
            {
                throw new ArgumentNullException(nameof(lifecycle));
            }

            lifecycle.Track(() => timer.Tick += OnTick, () =>
            {
                timer.Stop();
                timer.Tick -= OnTick;
            });
        }

        public void Queue()
        {
            if (!canQueueSave())
            {
                return;
            }

            timer.Stop();
            timer.Start();
        }

        public void Stop()
        {
            timer.Stop();
        }

        private void OnTick(object sender, EventArgs e)
        {
            timer.Stop();
            saveWorkspaceState();
        }
    }
}
