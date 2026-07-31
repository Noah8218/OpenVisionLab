using OpenVisionLab.PropertyGrid;
using OpenVisionLab.Vision._1._Tools.OpenCV;
using System;
using System.ComponentModel;
using System.Reflection;

namespace OpenVisionLab
{
    internal static class VisionToolPropertyPreviewPolicy
    {
        public static bool ShouldScheduleAutoPreview(PropertyGridPropertyValueChangedEventArgs e)
        {
            string propertyName = ResolvePropertyName(e, out bool resolvedKnownProperty);
            if (string.IsNullOrWhiteSpace(propertyName))
            {
                return false;
            }

            if (e?.TargetObject is OpenCvPropertyBase && !resolvedKnownProperty)
            {
                return false;
            }

            if (propertyName.StartsWith("AUTO_MPOINT_", StringComparison.Ordinal))
            {
                return false;
            }

            if (e?.TargetObject is MatchingProperty matching)
            {
                if (string.Equals(propertyName, nameof(MatchingProperty.AUTO_PREVIEW), StringComparison.Ordinal))
                {
                    return false;
                }

                if (!matching.AUTO_PREVIEW)
                {
                    return false;
                }
            }

            // Teaching-only switches and editor results reshape the PropertyGrid/ROI setup.
            // They must not write output layers until the operator explicitly previews/runs.
            switch (propertyName)
            {
                case nameof(OpenCvPropertyBase.USE_THRESHOLD):
                case "Use threshold":
                case nameof(OpenCvPropertyBase.USE_ADAPTIVE_THRESHOLD):
                case "Use adaptive threshold":
                case nameof(OpenCvPropertyBase.USE_ROI):
                case "Use ROI":
                case nameof(OpenCvPropertyBase.USE_MULTI_ROI):
                case "Use multi ROI":
                case nameof(OpenCvPropertyBase.USE_MASKING):
                case "Use masking":
                case nameof(OpenCvPropertyBase.CvROI):
                case nameof(OpenCvPropertyBase.CvROIS):
                case nameof(OpenCvPropertyBase.CvMASKS):
                case nameof(ContourProperty.USE_APPROXPOLYDP):
                case "Use approx poly":
                case nameof(ContourProperty.DrawMode):
                case nameof(ContourProperty.DrawColor):
                case nameof(ContourProperty.DrawThickness):
                case nameof(BlobProperty.MIN_WIDTH):
                case nameof(BlobProperty.MAX_WIDTH):
                case nameof(BlobProperty.MIN_HEIGHT):
                case nameof(BlobProperty.MAX_HEIGHT):
                case nameof(MatchingProperty.USE_FIND_ANGLE):
                case nameof(MatchingProperty.USE_COARSE_TO_FINE_ANGLE_SEARCH):
                case nameof(MatchingProperty.USE_PYRAMID_POSITION_PROPOSAL):
                case nameof(MatchingProperty.USE_CANNY):
                case nameof(EdgeBasedMatchingProperty.USE_HYBRID_VERIFY):
                case nameof(EdgeBasedMatchingProperty.USE_UNIQUE_MATCH_VALIDATION):
                case nameof(EdgeBasedMatchingProperty.UNIQUE_MATCH_MIN_SCORE_MARGIN):
                case nameof(EdgeBasedMatchingProperty.ALLOW_GLOBAL_POLARITY_REVERSAL):
                case nameof(LineGaugeProperty.USE_MANUAL_ANGLE):
                case nameof(LineGaugeProperty.USE_EXTEND_FIT_LINE):
                case nameof(LineGaugeProperty.USE_AVERAGE_FILTER):
                    return false;
                default:
                    return true;
            }
        }

        private static string ResolvePropertyName(PropertyGridPropertyValueChangedEventArgs e, out bool resolvedKnownProperty)
        {
            resolvedKnownProperty = false;
            if (e == null)
            {
                return string.Empty;
            }

            string candidate = e.PropertyName ?? string.Empty;
            object target = e.TargetObject;
            Type targetType = target?.GetType();
            if (targetType == null || string.IsNullOrWhiteSpace(candidate))
            {
                resolvedKnownProperty = targetType == null && !string.IsNullOrWhiteSpace(candidate);
                return candidate;
            }

            foreach (PropertyInfo property in targetType.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                if (string.Equals(property.Name, candidate, StringComparison.OrdinalIgnoreCase))
                {
                    resolvedKnownProperty = true;
                    return property.Name;
                }

                DisplayNameAttribute displayName = property.GetCustomAttribute<DisplayNameAttribute>(true);
                if (displayName != null
                    && string.Equals(displayName.DisplayName, candidate, StringComparison.CurrentCultureIgnoreCase))
                {
                    resolvedKnownProperty = true;
                    return property.Name;
                }
            }

            return candidate;
        }
    }
}
