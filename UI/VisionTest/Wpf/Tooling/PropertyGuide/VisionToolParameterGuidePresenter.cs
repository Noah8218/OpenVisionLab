using System;

namespace OpenVisionLab
{
    internal sealed class VisionToolParameterGuidePresenter
    {
        private readonly VisionToolParameterGuideView view;
        private readonly Func<string, bool> focusProperty;
        private object selectedObject;
        private string selectedPropertyName = string.Empty;

        public VisionToolParameterGuidePresenter(
            VisionToolParameterGuideView view,
            object selectedObject,
            Func<string, bool> focusProperty)
        {
            this.view = view ?? throw new ArgumentNullException(nameof(view));
            this.focusProperty = focusProperty ?? throw new ArgumentNullException(nameof(focusProperty));
            this.selectedObject = selectedObject;
            view.ShowPrompt();
        }

        public string SelectedPropertyName => selectedPropertyName;

        public void SelectObject(object value)
        {
            selectedObject = value;
            Refresh();
        }

        public void SelectProperty(string propertyName)
        {
            selectedPropertyName = propertyName ?? string.Empty;
            Refresh();
        }

        public void Refresh()
        {
            if (selectedObject == null || string.IsNullOrWhiteSpace(selectedPropertyName))
            {
                view.ShowPrompt();
                return;
            }

            VisionToolParameterGuideContent content =
                VisionToolParameterGuideCatalog.Resolve(selectedObject, selectedPropertyName);
            view.ShowContent(content, FocusRelatedProperty);
        }

        private void FocusRelatedProperty(string propertyName)
        {
            if (focusProperty(propertyName))
            {
                SelectProperty(propertyName);
            }
        }
    }
}
