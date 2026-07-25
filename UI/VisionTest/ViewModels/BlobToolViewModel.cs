using OpenVisionLab.Contracts;
using OpenVisionLab.Mvvm;
using System.Globalization;

namespace OpenVisionLab.ViewModels
{
    internal sealed class BlobToolViewModel : ObservableObject, IBlobToolViewModel
    {
        private readonly BlobProperty property;

        public BlobToolViewModel(BlobProperty property)
        {
            this.property = property ?? new BlobProperty("Blob");
        }

        public BlobProperty CreateProperty()
        {
            NormalizeRanges();
            property.NAME = string.IsNullOrWhiteSpace(property.NAME) ? "Blob" : property.NAME;
            return property.DeepCopy();
        }

        public string Summary
        {
            get
            {
                NormalizeRanges();
                string threshold = property.USE_THRESHOLD
                    ? string.Format(CultureInfo.CurrentCulture, "T {0:0.#}", property.THRESHOLD)
                    : property.USE_ADAPTIVE_THRESHOLD
                        ? string.Format(CultureInfo.CurrentCulture, "Adaptive {0:0.#}", property.ADAPTIVE_THRESHOLD)
                        : "Original";
                string roi = property.USE_ROI
                    ? property.USE_MULTI_ROI ? "Multi ROI" : "ROI"
                    : "Full image";

                return string.Format(
                    CultureInfo.CurrentCulture,
                    "Area {0}-{1} / W {2}-{3} / H {4}-{5} / {6} / {7}",
                    property.MIN_AREA,
                    property.MAX_AREA,
                    property.MIN_WIDTH,
                    FormatMaximum(property.MAX_WIDTH),
                    property.MIN_HEIGHT,
                    FormatMaximum(property.MAX_HEIGHT),
                    threshold,
                    roi);
            }
        }

        public void NormalizeAreaRange()
        {
            NormalizeRanges();
        }

        private void NormalizeRanges()
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
        }

        private static void NormalizeRange(int minimum, int maximum, out int normalizedMinimum, out int normalizedMaximum)
        {
            minimum = System.Math.Max(0, minimum);
            maximum = System.Math.Max(0, maximum);
            normalizedMinimum = System.Math.Min(minimum, maximum);
            normalizedMaximum = System.Math.Max(minimum, maximum);
        }

        private static string FormatMaximum(int value)
        {
            return value >= 1000000
                ? "*"
                : value.ToString(CultureInfo.CurrentCulture);
        }
    }
}
