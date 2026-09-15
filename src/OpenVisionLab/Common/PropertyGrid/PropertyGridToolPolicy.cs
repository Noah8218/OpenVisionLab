using System;
using System.Collections.Generic;
using System.Reflection;

namespace OpenVisionLab.Common
{
    /// <summary>
    /// Owns application-specific PropertyGrid row classification. The WPF bridge consumes
    /// the result but does not know OpenVisionLab tool property names or types.
    /// </summary>
    public static class PropertyGridToolPolicy
    {
        private static readonly HashSet<string> ChildParameterPropertyNames = new HashSet<string>(StringComparer.Ordinal)
        {
            "THRESHOLD_TYPES",
            "THRESHOLD",
            "ADAPTIVE_THRESHOLD",
            "ADAPTIVE_THRESHOLD_TYPES",
            "ADAPTIVE_THRESHOLD_ALGORITHM",
            "BlockSize",
            "Weight",
            "USE_MULTI_ROI",
            "CvROI",
            "CvROIS",
            "CvMASKS",
            "EPSILON",
            "FIND_ANGLE",
            "FIND_ANGLE_MIN",
            "FIND_ANGLE_MAX",
            "CANNY_LOW",
            "CANNY_HIGH",
            "POINT_RANGE",
            "MANUAL_ANGLE_VALUE",
            "EXTEND_FIT_LINE_VALUE",
            "AVERAGE_Diff",
            "AVERAGE_FILTER_TYPE",
            "USE_COARSE_TO_FINE_ANGLE_SEARCH",
            "COARSE_ANGLE_STEP",
            "COARSE_ANGLE_TOP_K",
            "PYRAMID_POSITION_TOP_N",
            "PYRAMID_POSITION_MIN_SCORE",
            "HYBRID_VERIFY_TOP_N",
            "HYBRID_VERIFY_IMAGE_WEIGHT"
        };

        private static readonly HashSet<string> AffineAdvancedPropertyNames = new HashSet<string>(StringComparer.Ordinal)
        {
            "OutputWidth",
            "OutputHeight",
            "Interpolation",
            "BorderType",
            "BorderValue",
            "MinimumSourceTriangleArea",
            "MinimumDestinationTriangleArea",
            "MinimumValidPixelRatio"
        };

        private static readonly HashSet<string> AffineSourceBindingPropertyNames = new HashSet<string>(StringComparer.Ordinal)
        {
            "SourcePoint1Feature",
            "SourcePoint2Feature",
            "SourcePoint3Feature",
            "SourcePoint1X",
            "SourcePoint1Y",
            "SourcePoint2X",
            "SourcePoint2Y",
            "SourcePoint3X",
            "SourcePoint3Y"
        };

        public static bool IsChildParameterProperty(object selectedObject, string propertyName)
        {
            if (selectedObject == null || string.IsNullOrWhiteSpace(propertyName))
            {
                return false;
            }

            if (ChildParameterPropertyNames.Contains(propertyName))
            {
                return true;
            }

            Type selectedType = selectedObject.GetType();
            return (selectedType.GetProperty("ShowAdvancedSettings", BindingFlags.Instance | BindingFlags.Public) != null
                    && AffineAdvancedPropertyNames.Contains(propertyName))
                || (selectedType.GetProperty("UseDetectedSourcePoints", BindingFlags.Instance | BindingFlags.Public) != null
                    && AffineSourceBindingPropertyNames.Contains(propertyName));
        }
    }
}
