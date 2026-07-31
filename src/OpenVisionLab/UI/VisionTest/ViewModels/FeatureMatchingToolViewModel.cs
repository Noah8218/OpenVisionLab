using OpenVisionLab.Contracts;
using OpenVisionLab.Mvvm;
using OpenVisionLab.Vision._1._Tools.OpenCV;
using System;
using System.Globalization;

namespace OpenVisionLab.ViewModels
{
    internal sealed class FeatureMatchingToolViewModel : ObservableObject, IFeatureMatchingToolViewModel
    {
        private readonly FeatureMatchingProperty property;

        public FeatureMatchingToolViewModel(FeatureMatchingProperty property)
        {
            this.property = property ?? new FeatureMatchingProperty("FeatureMatching");
            ConfigureDefaults();
        }

        public FeatureMatchingProperty CreateProperty()
        {
            Normalize();
            property.NAME = string.IsNullOrWhiteSpace(property.NAME) ? "FeatureMatching" : property.NAME;
            property.ReloadTemplateImage();
            return property.DeepCopy();
        }

        public string Summary
        {
            get
            {
                Normalize();
                return string.Format(
                    CultureInfo.CurrentCulture,
                    "Ratio <= {0:0.###} / RANSAC {1:0.#} / {2} / {3}",
                    property.SCORE_MIN,
                    property.RANSAC_REPROJ_THRESHOLD,
                    VisionToolPropertySummaryViewModel.CreateThresholdSummary(property),
                    VisionToolPropertySummaryViewModel.CreateRoiSummary(property));
            }
        }

        public VisionToolTemplateStatus TemplateStatus
            => VisionToolPropertySummaryViewModel.CreateTemplateStatus(property.PATTERN_PATH, property.ImageTemplate);

        public void ConfigureDefaults()
        {
            VisionToolPropertySummaryViewModel.DisableImagePreprocessDefaults(property, includeCanny: false);
        }

        public void ApplyTemplatePathForTest(string path)
        {
            property.PATTERN_PATH = path ?? string.Empty;
            property.SCORE_MIN = 0.8D;
            property.RANSAC_REPROJ_THRESHOLD = 5D;
            ConfigureDefaults();
            property.ReloadTemplateImage();
        }

        public void ReloadTemplateIfPatternChanged(string propertyName)
        {
            if (string.Equals(propertyName, nameof(FeatureMatchingProperty.PATTERN_PATH), StringComparison.Ordinal))
            {
                property.ReloadTemplateImage();
            }
        }

        private void Normalize()
        {
            // Feature matching uses SCORE_MIN as a ratio threshold; keep it in a predictable range for tests and preview.
            property.SCORE_MIN = VisionToolPropertySummaryViewModel.ClampDouble(property.SCORE_MIN, 0D, 1D);
            property.RANSAC_REPROJ_THRESHOLD = Math.Max(0D, property.RANSAC_REPROJ_THRESHOLD);
        }
    }
}
