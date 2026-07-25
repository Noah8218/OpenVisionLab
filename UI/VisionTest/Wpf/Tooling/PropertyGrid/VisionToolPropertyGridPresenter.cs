using OpenVisionLab.Contracts;
using OpenVisionLab.PropertyGrid;
using System;

namespace OpenVisionLab
{
    internal sealed class VisionToolPropertyGridPresenter<TProperty>
    {
        private readonly Func<TProperty> createProperty;
        private readonly Func<string> getSummary;
        private readonly Func<VisionToolTemplateStatus> getTemplateStatus;
        private readonly Action<string> applyTemplatePathForTest;
        private readonly Action<string> reloadTemplateIfPatternChanged;
        private readonly Action persistSelectedObject;

        public VisionToolPropertyGridPresenter(
            object selectedObject,
            Func<TProperty> createProperty,
            Func<string> getSummary,
            Func<VisionToolTemplateStatus> getTemplateStatus = null,
            Action<string> applyTemplatePathForTest = null,
            Action<string> reloadTemplateIfPatternChanged = null,
            Action persistSelectedObject = null)
        {
            SelectedObject = selectedObject ?? throw new ArgumentNullException(nameof(selectedObject));
            this.createProperty = createProperty ?? throw new ArgumentNullException(nameof(createProperty));
            this.getSummary = getSummary ?? throw new ArgumentNullException(nameof(getSummary));
            this.getTemplateStatus = getTemplateStatus;
            this.applyTemplatePathForTest = applyTemplatePathForTest;
            this.reloadTemplateIfPatternChanged = reloadTemplateIfPatternChanged;
            this.persistSelectedObject = persistSelectedObject;
        }

        public object SelectedObject { get; }

        public string Summary => getSummary() ?? string.Empty;

        public VisionToolTemplateStatus TemplateStatus
            => getTemplateStatus?.Invoke() ?? new VisionToolTemplateStatus(string.Empty, false);

        public TProperty CreateProperty()
        {
            return createProperty();
        }

        public void ApplyTemplatePathForTest(string path)
        {
            applyTemplatePathForTest?.Invoke(path);
        }

        public void ReloadTemplateIfPatternChanged(PropertyGridPropertyValueChangedEventArgs e)
        {
            // Template reload policy lives outside the View so PropertyGrid editor changes do not know about ViewModel types.
            reloadTemplateIfPatternChanged?.Invoke(e?.PropertyName);
        }

        public void PersistSelectedObject()
        {
            persistSelectedObject?.Invoke();
        }
    }
}
