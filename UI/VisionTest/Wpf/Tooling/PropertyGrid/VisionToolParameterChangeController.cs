using System;

namespace OpenVisionLab
{
    internal sealed class VisionToolParameterChangeController
    {
        private readonly Func<bool> isSuppressed;
        private readonly Action refreshSummary;
        private readonly Action notifyParameterChanged;
        private readonly Action schedulePreview;

        public VisionToolParameterChangeController(
            Func<bool> isSuppressed,
            Action refreshSummary,
            Action notifyParameterChanged = null,
            Action schedulePreview = null)
        {
            this.isSuppressed = isSuppressed;
            this.refreshSummary = refreshSummary ?? throw new ArgumentNullException(nameof(refreshSummary));
            this.notifyParameterChanged = notifyParameterChanged;
            this.schedulePreview = schedulePreview;
        }

        public bool TryHandle(Action beforeRefresh = null, bool notifyChanged = false, bool schedulePreview = false)
        {
            if (isSuppressed?.Invoke() == true)
            {
                return false;
            }

            beforeRefresh?.Invoke();
            Refresh(notifyChanged, schedulePreview);
            return true;
        }

        public void RefreshProgrammatic(Action beforeRefresh = null, bool notifyChanged = false, bool schedulePreview = false)
        {
            // Programmatic updates run while UI events are suppressed, but the visible tool state still needs to settle.
            beforeRefresh?.Invoke();
            Refresh(notifyChanged, schedulePreview);
        }

        public void Refresh(bool notifyChanged = false, bool schedulePreview = false)
        {
            // Parameter edits should always settle in one order: update local state, refresh summary, then notify expensive work.
            refreshSummary();
            if (schedulePreview)
            {
                this.schedulePreview?.Invoke();
            }

            if (notifyChanged)
            {
                notifyParameterChanged?.Invoke();
            }
        }
    }
}