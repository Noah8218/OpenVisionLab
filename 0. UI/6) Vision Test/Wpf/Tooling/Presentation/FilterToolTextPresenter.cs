using System;
using System.Windows.Controls;

namespace OpenVisionLab
{
    internal sealed class FilterToolTextPresenter
    {
        private readonly GroupBox filterOptionsGroup;
        private readonly GroupBox kernelGroup;
        private readonly TextBlock filterTypeLabel;
        private readonly TextBlock borderTypeLabel;
        private readonly TextBlock kernelWidthLabel;
        private readonly TextBlock kernelHeightLabel;
        private readonly TextBlock medianKernelLabel;
        private readonly TextBlock diameterLabel;
        private readonly TextBlock sigmaColorLabel;
        private readonly TextBlock sigmaSpaceLabel;

        public FilterToolTextPresenter(
            GroupBox filterOptionsGroup,
            GroupBox kernelGroup,
            TextBlock filterTypeLabel,
            TextBlock borderTypeLabel,
            TextBlock kernelWidthLabel,
            TextBlock kernelHeightLabel,
            TextBlock medianKernelLabel,
            TextBlock diameterLabel,
            TextBlock sigmaColorLabel,
            TextBlock sigmaSpaceLabel)
        {
            this.filterOptionsGroup = filterOptionsGroup ?? throw new ArgumentNullException(nameof(filterOptionsGroup));
            this.kernelGroup = kernelGroup ?? throw new ArgumentNullException(nameof(kernelGroup));
            this.filterTypeLabel = filterTypeLabel ?? throw new ArgumentNullException(nameof(filterTypeLabel));
            this.borderTypeLabel = borderTypeLabel ?? throw new ArgumentNullException(nameof(borderTypeLabel));
            this.kernelWidthLabel = kernelWidthLabel ?? throw new ArgumentNullException(nameof(kernelWidthLabel));
            this.kernelHeightLabel = kernelHeightLabel ?? throw new ArgumentNullException(nameof(kernelHeightLabel));
            this.medianKernelLabel = medianKernelLabel ?? throw new ArgumentNullException(nameof(medianKernelLabel));
            this.diameterLabel = diameterLabel ?? throw new ArgumentNullException(nameof(diameterLabel));
            this.sigmaColorLabel = sigmaColorLabel ?? throw new ArgumentNullException(nameof(sigmaColorLabel));
            this.sigmaSpaceLabel = sigmaSpaceLabel ?? throw new ArgumentNullException(nameof(sigmaSpaceLabel));
        }

        public void ApplyLocalization()
        {
            filterOptionsGroup.Header = ResolveText("Arithmetic.Operation", "Operation");
            kernelGroup.Header = ResolveText("PropertyGrid.Category.Kernel", "Kernel");
            filterTypeLabel.Text = ResolveText("PropertyGrid.Property.FilterType.DisplayName", "Filter type");
            borderTypeLabel.Text = ResolveText("PropertyGrid.Property.BorderType.DisplayName", "Border type");
            kernelWidthLabel.Text = ResolveText("PropertyGrid.Property.KernelWidth.DisplayName", "Kernel width");
            kernelHeightLabel.Text = ResolveText("PropertyGrid.Property.KernelHeight.DisplayName", "Kernel height");
            medianKernelLabel.Text = ResolveText("PropertyGrid.Property.MedianKernelSize.DisplayName", "Median kernel size");
            diameterLabel.Text = ResolveText("PropertyGrid.Property.Diameter.DisplayName", "Diameter");
            sigmaColorLabel.Text = ResolveText("PropertyGrid.Property.SigmaColor.DisplayName", "Sigma Color");
            sigmaSpaceLabel.Text = ResolveText("PropertyGrid.Property.SigmaSpace.DisplayName", "Sigma Space");
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
