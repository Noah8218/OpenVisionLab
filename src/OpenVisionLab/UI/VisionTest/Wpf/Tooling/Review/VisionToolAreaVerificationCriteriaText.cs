using OpenVisionLab.Vision._1._Tools.OpenCV;
using System.Globalization;

namespace OpenVisionLab
{
    internal static class VisionToolAreaVerificationCriteriaText
    {
        public static string CreateBlob(BlobProperty property)
        {
            BlobProperty resolvedProperty = property ?? new BlobProperty();
            return CreateBaseText(
                resolvedProperty,
                resolvedProperty.MIN_AREA,
                resolvedProperty.MAX_AREA,
                resolvedProperty.MIN_WIDTH,
                resolvedProperty.MAX_WIDTH,
                resolvedProperty.MIN_HEIGHT,
                resolvedProperty.MAX_HEIGHT,
                resolvedProperty.USE_MASKING ? VisionToolVerificationText.MaskOn : VisionToolVerificationText.MaskOff);
        }

        public static string CreateContour(ContourProperty property)
        {
            ContourProperty resolvedProperty = property ?? new ContourProperty();
            return CreateBaseText(
                resolvedProperty,
                resolvedProperty.MIN_AREA,
                resolvedProperty.MAX_AREA,
                resolvedProperty.MIN_WIDTH,
                resolvedProperty.MAX_WIDTH,
                resolvedProperty.MIN_HEIGHT,
                resolvedProperty.MAX_HEIGHT,
                resolvedProperty.DrawMode.ToString());
        }

        private static string CreateBaseText(
            OpenCvPropertyBase property,
            int minArea,
            int maxArea,
            int minWidth,
            int maxWidth,
            int minHeight,
            int maxHeight,
            string suffix)
        {
            string roiText = CreateEffectiveRoiText(property);
            string thresholdText = property.USE_THRESHOLD
                ? VisionToolVerificationText.FormatThreshold(property.THRESHOLD)
                : VisionToolVerificationText.ThresholdOff;

            string areaText = VisionToolVerificationText.FormatAreaCriteria(
                minArea,
                maxArea,
                thresholdText,
                roiText,
                suffix ?? string.Empty);
            string maxWidthText = maxWidth >= 1000000 ? "*" : maxWidth.ToString(CultureInfo.CurrentCulture);
            string maxHeightText = maxHeight >= 1000000 ? "*" : maxHeight.ToString(CultureInfo.CurrentCulture);
            return string.Format(
                CultureInfo.CurrentCulture,
                "{0} / Bounding W {1}-{2}px / H {3}-{4}px",
                areaText,
                minWidth,
                maxWidthText,
                minHeight,
                maxHeightText);
        }

        private static string CreateEffectiveRoiText(OpenCvPropertyBase property)
        {
            if (!property.USE_ROI)
            {
                return VisionToolVerificationText.FullImage;
            }

            if (property.USE_MULTI_ROI)
            {
                return VisionToolVerificationText.MultiRoi;
            }

            return property.CvROI.Width == 0 || property.CvROI.Height == 0
                ? VisionToolVerificationText.FullImageRoiFallback
                : VisionToolVerificationText.RoiOn;
        }
    }
}
