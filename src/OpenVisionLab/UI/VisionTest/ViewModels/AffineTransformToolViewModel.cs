using OpenVisionLab.Contracts;
using OpenVisionLab.Mvvm;
using System.Globalization;

namespace OpenVisionLab.ViewModels
{
    internal sealed class AffineTransformToolViewModel : ObservableObject, IAffineTransformToolViewModel
    {
        private readonly AffineTransformProperty property;

        public AffineTransformToolViewModel(AffineTransformProperty property)
        {
            this.property = property ?? new AffineTransformProperty("AffineTransform");
        }

        public AffineTransformProperty CreateProperty()
        {
            property.NAME = string.IsNullOrWhiteSpace(property.NAME) ? "AffineTransform" : property.NAME;
            return property.DeepCopy();
        }

        public string Summary => string.Format(
            CultureInfo.CurrentCulture,
            "3-point affine / Output {0}x{1} / Min coverage {2:0.####}",
            property.OutputWidth == 0 ? "Input" : property.OutputWidth.ToString(CultureInfo.CurrentCulture),
            property.OutputHeight == 0 ? "Input" : property.OutputHeight.ToString(CultureInfo.CurrentCulture),
            property.MinimumValidPixelRatio);
    }
}
