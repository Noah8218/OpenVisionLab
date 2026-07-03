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
            NormalizeAreaRange();
            property.NAME = string.IsNullOrWhiteSpace(property.NAME) ? "Blob" : property.NAME;
            return property.DeepCopy();
        }

        public string Summary
        {
            get
            {
                NormalizeAreaRange();
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
                    "Area {0}-{1} / {2} / {3}",
                    property.MIN_AREA,
                    property.MAX_AREA,
                    threshold,
                    roi);
            }
        }

        public void NormalizeAreaRange()
        {
            // The Blob property grid still edits the legacy property object; keep the min/max pair ordered before execution.
            if (property.MIN_AREA > property.MAX_AREA)
            {
                int min = property.MAX_AREA;
                property.MAX_AREA = property.MIN_AREA;
                property.MIN_AREA = min;
            }
        }
    }
}