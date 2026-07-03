using OpenVisionLab.Contracts;
using System;
using OpenVisionLab.Mvvm;
using System.Globalization;

namespace OpenVisionLab.ViewModels
{
    internal sealed class ContourToolViewModel : ObservableObject, IContourToolViewModel
    {
        private readonly ContourProperty property;

        public ContourToolViewModel(ContourProperty property)
        {
            this.property = property ?? new ContourProperty("Contour");
        }

        public ContourProperty CreateProperty()
        {
            Normalize();
            property.NAME = string.IsNullOrWhiteSpace(property.NAME) ? "Contour" : property.NAME;
            return property.DeepCopy();
        }

        public string Summary
        {
            get
            {
                Normalize();
                return string.Format(
                    CultureInfo.CurrentCulture,
                    "Area {0}-{1} / {2} / {3} / {4}",
                    property.MIN_AREA,
                    property.MAX_AREA,
                    VisionToolPropertySummaryViewModel.CreateThresholdSummary(property),
                    VisionToolPropertySummaryViewModel.CreateRoiSummary(property),
                    property.DrawMode);
            }
        }

        private void Normalize()
        {
            // Contour detection uses an area range; keep min/max ordered before preview or pipeline execution.
            if (property.MIN_AREA > property.MAX_AREA)
            {
                int min = property.MAX_AREA;
                property.MAX_AREA = property.MIN_AREA;
                property.MIN_AREA = min;
            }

            property.EPSILON = Math.Max(0D, property.EPSILON);
            property.DrawThickness = Math.Max(1, property.DrawThickness);
        }
    }
}
