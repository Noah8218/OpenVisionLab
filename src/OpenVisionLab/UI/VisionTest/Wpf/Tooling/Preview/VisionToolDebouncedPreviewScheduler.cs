using System;
using System.Windows;
using System.Windows.Threading;

namespace OpenVisionLab
{
    internal sealed class VisionToolDebouncedPreviewScheduler : IDisposable
    {
        private readonly FrameworkElement owner;
        private readonly Action runPreview;
        private readonly DispatcherTimer timer;
        private bool disposed;

        public VisionToolDebouncedPreviewScheduler(FrameworkElement owner, Action runPreview, int intervalMilliseconds = 90)
        {
            this.owner = owner ?? throw new ArgumentNullException(nameof(owner));
            this.runPreview = runPreview ?? throw new ArgumentNullException(nameof(runPreview));
            timer = new DispatcherTimer(DispatcherPriority.Background, owner.Dispatcher)
            {
                Interval = TimeSpan.FromMilliseconds(Math.Max(1, intervalMilliseconds))
            };
            timer.Tick += Timer_Tick;
        }

        public void Schedule()
        {
            if (disposed || !owner.IsLoaded)
            {
                return;
            }

            timer.Stop();
            timer.Start();
        }

        public void Cancel()
        {
            timer.Stop();
        }

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            disposed = true;
            timer.Stop();
            timer.Tick -= Timer_Tick;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            timer.Stop();
            runPreview();
        }
    }
}
