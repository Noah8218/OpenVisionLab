using System;
using System.Windows;
using System.Windows.Threading;

namespace OpenVisionLab
{
    internal sealed class VisionToolDebouncedPreviewScheduler : IDisposable
    {
        internal static readonly DependencyProperty AllowAutoPreviewProperty = DependencyProperty.RegisterAttached("AllowAutoPreview", typeof(bool), typeof(VisionToolDebouncedPreviewScheduler),
            new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.Inherits, OnAllowAutoPreviewChanged));
        private static readonly DependencyProperty SchedulerProperty = DependencyProperty.RegisterAttached("Scheduler", typeof(VisionToolDebouncedPreviewScheduler), typeof(VisionToolDebouncedPreviewScheduler));
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
            owner.SetValue(SchedulerProperty, this);
        }

        internal static bool GetAllowAutoPreview(DependencyObject owner) => (bool)owner.GetValue(AllowAutoPreviewProperty);
        internal static void SetAllowAutoPreview(DependencyObject owner, bool value) => owner.SetValue(AllowAutoPreviewProperty, value);
        internal static void CancelPending(DependencyObject owner) => (owner.GetValue(SchedulerProperty) as VisionToolDebouncedPreviewScheduler)?.Cancel();

        private static void OnAllowAutoPreviewChanged(DependencyObject owner, DependencyPropertyChangedEventArgs args)
        {
            if (args.NewValue is false) { CancelPending(owner); }
        }

        public void Schedule()
        {
            if (disposed || !owner.IsLoaded || !GetAllowAutoPreview(owner))
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
            if (ReferenceEquals(owner.GetValue(SchedulerProperty), this)) { owner.ClearValue(SchedulerProperty); }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            timer.Stop();
            if (!disposed && owner.IsLoaded && GetAllowAutoPreview(owner)) { runPreview(); }
        }
    }
}
