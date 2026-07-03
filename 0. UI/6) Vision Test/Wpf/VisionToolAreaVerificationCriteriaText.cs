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
                resolvedProperty.USE_MASKING ? VisionToolVerificationText.MaskOn : VisionToolVerificationText.MaskOff);
        }

        public static string CreateContour(ContourProperty property)
        {
            ContourProperty resolvedProperty = property ?? new ContourProperty();
            return CreateBaseText(
                resolvedProperty,
                resolvedProperty.MIN_AREA,
                resolvedProperty.MAX_AREA,
                resolvedProperty.DrawMode.ToString());
        }

        private static string CreateBaseText(
            OpenCvPropertyBase property,
            int minArea,
            int maxArea,
            string suffix)
        {
            string roiText = property.USE_ROI
                ? (property.USE_MULTI_ROI ? VisionToolVerificationText.MultiRoi : VisionToolVerificationText.RoiOn)
                : VisionToolVerificationText.FullImage;
            string thresholdText = property.USE_THRESHOLD
                ? VisionToolVerificationText.FormatThreshold(property.THRESHOLD)
                : VisionToolVerificationText.ThresholdOff;

            return VisionToolVerificationText.FormatAreaCriteria(
                minArea,
                maxArea,
                thresholdText,
                roiText,
                suffix ?? string.Empty);
        }
    }
}
