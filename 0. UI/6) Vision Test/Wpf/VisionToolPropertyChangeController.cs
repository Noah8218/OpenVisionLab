using OpenVisionLab.PropertyGrid;
using System;

namespace OpenVisionLab
{
    internal sealed class VisionToolPropertyChangeController
    {
        private readonly Action<PropertyGridPropertyValueChangedEventArgs> beforeRefresh;
        private readonly Action refreshSummary;
        private readonly Action refreshOverlay;
        private readonly Action clearResultReview;
        private readonly Action schedulePreview;
        private readonly Action cancelPreview;
        private readonly Func<PropertyGridPropertyValueChangedEventArgs, bool> shouldSchedulePreview;

        public VisionToolPropertyChangeController(
            Action refreshSummary,
            Action clearResultReview,
            Action<PropertyGridPropertyValueChangedEventArgs> beforeRefresh = null,
            Action refreshOverlay = null,
            Action schedulePreview = null,
            Action cancelPreview = null,
            Func<PropertyGridPropertyValueChangedEventArgs, bool> shouldSchedulePreview = null)
        {
            this.refreshSummary = refreshSummary ?? throw new ArgumentNullException(nameof(refreshSummary));
            this.clearResultReview = clearResultReview ?? throw new ArgumentNullException(nameof(clearResultReview));
            this.beforeRefresh = beforeRefresh;
            this.refreshOverlay = refreshOverlay;
            this.schedulePreview = schedulePreview;
            this.cancelPreview = cancelPreview;
            this.shouldSchedulePreview = shouldSchedulePreview;
        }

        public void OnPropertyValueChanged(object sender, PropertyGridPropertyValueChangedEventArgs e)
        {
            beforeRefresh?.Invoke(e);
            RefreshViewState(e);
        }

        public void RefreshAfterExternalUpdate(VisionToolPropertyGridHost propertyGridHost, bool applyVisibilityRules = false)
        {
            if (propertyGridHost == null)
            {
                return;
            }

            if (applyVisibilityRules)
            {
                propertyGridHost.RefreshAndApplyVisibilityRules();
            }
            else
            {
                propertyGridHost.RefreshSelectedObject();
            }

            RefreshViewState(null);
        }

        public void RefreshViewState(PropertyGridPropertyValueChangedEventArgs e = null)
        {
            refreshSummary();
            refreshOverlay?.Invoke();
            bool canSchedulePreview = schedulePreview != null && (shouldSchedulePreview?.Invoke(e) ?? true);
            if (!canSchedulePreview)
            {
                cancelPreview?.Invoke();
                clearResultReview();
                return;
            }

            // Auto-preview tools keep the last review visible until the debounced result replaces it, avoiding slider-drag layout churn.
            schedulePreview();
        }
    }
}
