using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Threading;
using static OpenVisionLab.DEFINE;

namespace OpenVisionLab
{
    internal sealed class OpenVisionShellHostToolPrewarmController : IDisposable
    {
        private readonly Dispatcher dispatcher;
        private readonly OpenVisionNativeToolPrewarmService nativeToolPrewarmService;
        private readonly OpenVisionFloatingToolWindowHost floatingToolWindowHost;
        private readonly Func<bool> canRun;
        private readonly Func<VISION_MENU?> selectedMenuProvider;
        private readonly Func<Window> ownerProvider;
        private readonly DispatcherTimer resumeTimer;
        private bool disposed;

        public OpenVisionShellHostToolPrewarmController(
            Dispatcher dispatcher,
            OpenVisionNativeToolPrewarmService nativeToolPrewarmService,
            OpenVisionFloatingToolWindowHost floatingToolWindowHost,
            Func<bool> canRun,
            Func<VISION_MENU?> selectedMenuProvider,
            Func<Window> ownerProvider)
        {
            this.dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
            this.nativeToolPrewarmService = nativeToolPrewarmService ?? throw new ArgumentNullException(nameof(nativeToolPrewarmService));
            this.floatingToolWindowHost = floatingToolWindowHost ?? throw new ArgumentNullException(nameof(floatingToolWindowHost));
            this.canRun = canRun ?? throw new ArgumentNullException(nameof(canRun));
            this.selectedMenuProvider = selectedMenuProvider ?? throw new ArgumentNullException(nameof(selectedMenuProvider));
            this.ownerProvider = ownerProvider ?? throw new ArgumentNullException(nameof(ownerProvider));
            resumeTimer = new DispatcherTimer(DispatcherPriority.Normal, this.dispatcher)
            {
                Interval = TimeSpan.FromMilliseconds(800)
            };
            resumeTimer.Tick += ResumeTimer_Tick;
        }

        public void ScheduleStartupWork()
        {
            ScheduleFloatingToolWindowPrepareIfEnabled();
            ScheduleNativePrewarmIfEnabled();
        }

        public void ScheduleNativePrewarmIfEnabled()
        {
            if (IsNativeToolPrewarmDisabledForDiagnostics() || !canRun())
            {
                return;
            }

            // Start warming after the loaded/render work is queued, while preserving the fast-open cache contract.
            dispatcher.BeginInvoke(new Action(StartNativeToolPrewarm), DispatcherPriority.Background);
        }

        public bool PauseForOperatorSelection()
        {
            bool shouldResumePrewarm =
                !nativeToolPrewarmService.IsCompleted
                && !IsNativeToolPrewarmDisabledForDiagnostics();
            if (!shouldResumePrewarm)
            {
                return false;
            }

            // Operator selection has priority over background warming; resume after the tool window gets first paint.
            resumeTimer.Stop();
            nativeToolPrewarmService.Cancel();
            return true;
        }

        public void RecordSelection(VISION_MENU menu)
        {
            OpenVisionNativeToolPrewarmPolicy.RecordSelection(menu);
        }

        public void ResumeAfterOperatorSelection(bool shouldResume)
        {
            if (!shouldResume || !canRun())
            {
                return;
            }

            resumeTimer.Stop();
            resumeTimer.Start();
        }

        public void Cancel()
        {
            resumeTimer.Stop();
            nativeToolPrewarmService.Cancel();
        }

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            disposed = true;
            resumeTimer.Tick -= ResumeTimer_Tick;
            resumeTimer.Stop();
        }

        private void ScheduleFloatingToolWindowPrepareIfEnabled()
        {
            if (IsFloatingToolWindowPrepareDisabledForDiagnostics() || !canRun())
            {
                return;
            }

            dispatcher.BeginInvoke(new Action(PrepareFloatingToolWindow), DispatcherPriority.Background);
        }

        private void PrepareFloatingToolWindow()
        {
            if (!canRun())
            {
                return;
            }

            floatingToolWindowHost.Prepare(ownerProvider());
        }

        private void StartNativeToolPrewarm()
        {
            if (!canRun())
            {
                return;
            }

            IEnumerable<VISION_MENU> prewarmMenus;
            VISION_MENU? selectedMenu = selectedMenuProvider();
            if (selectedMenu.HasValue)
            {
                prewarmMenus = OpenVisionNativeToolPrewarmPolicy.GetMenus(selectedMenu.Value);
            }
            else
            {
                prewarmMenus = OpenVisionNativeToolPrewarmPolicy.GetDefaultMenus();
            }

            nativeToolPrewarmService.Start(prewarmMenus);
        }

        private void ResumeTimer_Tick(object sender, EventArgs e)
        {
            resumeTimer.Stop();
            ScheduleNativePrewarmIfEnabled();
        }

        private static bool IsNativeToolPrewarmDisabledForDiagnostics()
        {
            return string.Equals(
                Environment.GetEnvironmentVariable("OPENVISIONLAB_DISABLE_NATIVE_PREWARM"),
                "1",
                StringComparison.Ordinal);
        }

        private static bool IsFloatingToolWindowPrepareDisabledForDiagnostics()
        {
            return string.Equals(
                Environment.GetEnvironmentVariable("OPENVISIONLAB_DISABLE_FLOATING_PREPARE"),
                "1",
                StringComparison.Ordinal);
        }
    }
}
