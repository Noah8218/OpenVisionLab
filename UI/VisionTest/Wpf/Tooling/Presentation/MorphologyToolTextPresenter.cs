using System;
using System.Windows.Controls;

namespace OpenVisionLab
{
    internal sealed class MorphologyToolTextPresenter
    {
        private readonly GroupBox operationGroup;
        private readonly GroupBox kernelGroup;
        private readonly TextBlock kernelWidthLabel;
        private readonly TextBlock kernelHeightLabel;
        private readonly TextBlock shapeLabel;
        private readonly Action refreshOperationAndShapeLabels;

        public MorphologyToolTextPresenter(
            GroupBox operationGroup,
            GroupBox kernelGroup,
            TextBlock kernelWidthLabel,
            TextBlock kernelHeightLabel,
            TextBlock shapeLabel,
            Action refreshOperationAndShapeLabels)
        {
            this.operationGroup = operationGroup ?? throw new ArgumentNullException(nameof(operationGroup));
            this.kernelGroup = kernelGroup ?? throw new ArgumentNullException(nameof(kernelGroup));
            this.kernelWidthLabel = kernelWidthLabel ?? throw new ArgumentNullException(nameof(kernelWidthLabel));
            this.kernelHeightLabel = kernelHeightLabel ?? throw new ArgumentNullException(nameof(kernelHeightLabel));
            this.shapeLabel = shapeLabel ?? throw new ArgumentNullException(nameof(shapeLabel));
            this.refreshOperationAndShapeLabels = refreshOperationAndShapeLabels ?? throw new ArgumentNullException(nameof(refreshOperationAndShapeLabels));
        }

        public void ApplyLocalization()
        {
            operationGroup.Header = ResolveText("Arithmetic.Operation", "Operation");
            kernelGroup.Header = ResolveText("PropertyGrid.Category.Kernel", "Kernel");
            kernelWidthLabel.Text = ResolveText("PropertyGrid.Property.KernelWidth.DisplayName", "Kernel width");
            kernelHeightLabel.Text = ResolveText("PropertyGrid.Property.KernelHeight.DisplayName", "Kernel height");
            shapeLabel.Text = ResolveText("PropertyGrid.Property.Shape.DisplayName", "Shape");
            refreshOperationAndShapeLabels();
        }

        private static string ResolveText(string localizationKey, string fallbackText)
        {
            string value = OpenVisionLanguageService.T(localizationKey);
            return string.IsNullOrWhiteSpace(value) || string.Equals(value, localizationKey, StringComparison.Ordinal)
                ? fallbackText ?? string.Empty
                : value;
        }
    }
}
