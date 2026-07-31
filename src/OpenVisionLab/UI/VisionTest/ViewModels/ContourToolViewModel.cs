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
                    "Area {0}-{1} / W {2}-{3} / H {4}-{5} / {6} / {7} / {8}",
                    property.MIN_AREA,
                    property.MAX_AREA,
                    property.MIN_WIDTH,
                    FormatMaximum(property.MAX_WIDTH),
                    property.MIN_HEIGHT,
                    FormatMaximum(property.MAX_HEIGHT),
                    VisionToolPropertySummaryViewModel.CreateThresholdSummary(property),
                    VisionToolPropertySummaryViewModel.CreateRoiSummary(property),
                    property.DrawMode);
            }
        }

        private void Normalize()
        {
            NormalizeRange(property.MIN_AREA, property.MAX_AREA, out int minArea, out int maxArea);
            NormalizeRange(property.MIN_WIDTH, property.MAX_WIDTH, out int minWidth, out int maxWidth);
            NormalizeRange(property.MIN_HEIGHT, property.MAX_HEIGHT, out int minHeight, out int maxHeight);
            property.MIN_AREA = minArea;
            property.MAX_AREA = maxArea;
            property.MIN_WIDTH = minWidth;
            property.MAX_WIDTH = maxWidth;
            property.MIN_HEIGHT = minHeight;
            property.MAX_HEIGHT = maxHeight;

            property.EPSILON = Math.Max(0D, property.EPSILON);
            property.DrawThickness = Math.Max(1, property.DrawThickness);
        }

        private static void NormalizeRange(int minimum, int maximum, out int normalizedMinimum, out int normalizedMaximum)
        {
            minimum = Math.Max(0, minimum);
            maximum = Math.Max(0, maximum);
            normalizedMinimum = Math.Min(minimum, maximum);
            normalizedMaximum = Math.Max(minimum, maximum);
        }

        private static string FormatMaximum(int value)
        {
            return value >= 1000000
                ? "*"
                : value.ToString(CultureInfo.CurrentCulture);
        }
    }
}
