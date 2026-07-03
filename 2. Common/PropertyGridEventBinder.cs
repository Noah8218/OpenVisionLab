using OpenVisionLab._1._Core;
using OpenVisionLab.PropertyGrid;
using OpenVisionLab.Vision._1._Tools.OpenCV;
using System;
using System.ComponentModel;
using System.Reflection;

namespace OpenVisionLab._2._Common
{
    public sealed class PropertyGridEventBinder
    {
        private bool isApplyingVisibility;

        public PropertyGridEventBinder(Func<IDisplayManager> displayManagerAccessor)
        {
        }

        public void Wpg_SelectedObjectsChanged(object sender, EventArgs e)
        {
            ApplyVisibilityRules(sender as IPropertyGridView);
        }

        public void Wpg_PropertyValueChanged(object sender, PropertyGridPropertyValueChangedEventArgs e)
        {
            if (ShouldReapplyVisibility(e?.PropertyName))
            {
                ApplyVisibilityRules(sender as IPropertyGridView);
            }
        }

        public bool ApplyVisibilityRules(IPropertyGridView propertyGrid, bool refreshOnChange = true)
        {
            if (propertyGrid == null || propertyGrid.SelectedObject == null || isApplyingVisibility)
            {
                return false;
            }

            isApplyingVisibility = true;
            try
            {
                bool changed = ApplyDeclaredAndConditionalVisibility(propertyGrid);
                if (changed && refreshOnChange)
                {
                    // WPG rebuilds the property item tree on refresh, so do this only
                    // when visibility rules actually changed a property.
                    propertyGrid.RefreshSelectedObject();
                    return true;
                }

                return false;
            }
            finally
            {
                isApplyingVisibility = false;
            }
        }

        private static bool ApplyDeclaredAndConditionalVisibility(IPropertyGridView propertyGrid)
        {
            object selected = propertyGrid.SelectedObject;
            if (selected == null)
            {
                return false;
            }

            bool changed = false;
            foreach (PropertyInfo property in selected.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                BrowsableAttribute browsable = property.GetCustomAttribute<BrowsableAttribute>(true);
                changed |= SetBrowsableIfExists(propertyGrid, selected, property.Name, browsable?.Browsable ?? true);
            }

            changed |= ApplyOpenCvBaseVisibility(propertyGrid, selected);
            changed |= ApplyToolSpecificVisibility(propertyGrid, selected);
            return changed;
        }

        private static bool ApplyOpenCvBaseVisibility(IPropertyGridView propertyGrid, object selected)
        {
            if (!(selected is OpenCvPropertyBase property))
            {
                return false;
            }

            bool changed = false;
            changed |= SetBrowsableIfExists(propertyGrid, selected, nameof(OpenCvPropertyBase.THRESHOLD_TYPES), property.USE_THRESHOLD);
            changed |= SetBrowsableIfExists(propertyGrid, selected, nameof(OpenCvPropertyBase.THRESHOLD), property.USE_THRESHOLD);

            changed |= SetBrowsableIfExists(propertyGrid, selected, nameof(OpenCvPropertyBase.USE_BITWISENOT), false);

            changed |= SetBrowsableIfExists(propertyGrid, selected, nameof(OpenCvPropertyBase.ADAPTIVE_THRESHOLD), property.USE_ADAPTIVE_THRESHOLD);
            changed |= SetBrowsableIfExists(propertyGrid, selected, nameof(OpenCvPropertyBase.ADAPTIVE_THRESHOLD_TYPES), property.USE_ADAPTIVE_THRESHOLD);
            changed |= SetBrowsableIfExists(propertyGrid, selected, nameof(OpenCvPropertyBase.ADAPTIVE_THRESHOLD_ALGORITHM), property.USE_ADAPTIVE_THRESHOLD);
            changed |= SetBrowsableIfExists(propertyGrid, selected, nameof(OpenCvPropertyBase.BlockSize), property.USE_ADAPTIVE_THRESHOLD);
            changed |= SetBrowsableIfExists(propertyGrid, selected, nameof(OpenCvPropertyBase.Weight), property.USE_ADAPTIVE_THRESHOLD);

            changed |= SetBrowsableIfExists(propertyGrid, selected, nameof(OpenCvPropertyBase.USE_MULTI_ROI), property.USE_ROI);
            changed |= SetBrowsableIfExists(propertyGrid, selected, nameof(OpenCvPropertyBase.CvROI), property.USE_ROI && !property.USE_MULTI_ROI);
            changed |= SetBrowsableIfExists(propertyGrid, selected, nameof(OpenCvPropertyBase.CvROIS), property.USE_ROI && property.USE_MULTI_ROI);
            changed |= SetBrowsableIfExists(propertyGrid, selected, nameof(OpenCvPropertyBase.CvMASKS), property.USE_MASKING || property.CvMASKS?.Count > 0);
            return changed;
        }

        private static bool ApplyToolSpecificVisibility(IPropertyGridView propertyGrid, object selected)
        {
            bool changed = false;
            if (selected is global::OpenVisionLab.ContourProperty contour)
            {
                changed |= SetBrowsableIfExists(propertyGrid, selected, nameof(global::OpenVisionLab.ContourProperty.EPSILON), contour.USE_APPROXPOLYDP);
                changed |= SetBrowsableIfExists(propertyGrid, selected, nameof(global::OpenVisionLab.ContourProperty.DrawColor), true);
                changed |= SetBrowsableIfExists(propertyGrid, selected, nameof(global::OpenVisionLab.ContourProperty.DrawThickness), true);
            }

            if (selected is global::OpenVisionLab.MatchingProperty matching)
            {
                changed |= SetBrowsableIfExists(propertyGrid, selected, nameof(global::OpenVisionLab.MatchingProperty.FIND_ANGLE), matching.USE_FIND_ANGLE);
                changed |= SetBrowsableIfExists(propertyGrid, selected, nameof(global::OpenVisionLab.MatchingProperty.FIND_ANGLE_MIN), matching.USE_FIND_ANGLE);
                changed |= SetBrowsableIfExists(propertyGrid, selected, nameof(global::OpenVisionLab.MatchingProperty.FIND_ANGLE_MAX), matching.USE_FIND_ANGLE);
                changed |= SetBrowsableIfExists(propertyGrid, selected, nameof(global::OpenVisionLab.MatchingProperty.USE_COARSE_TO_FINE_ANGLE_SEARCH), matching.USE_FIND_ANGLE);
                changed |= SetBrowsableIfExists(propertyGrid, selected, nameof(global::OpenVisionLab.MatchingProperty.COARSE_ANGLE_STEP), matching.USE_FIND_ANGLE && matching.USE_COARSE_TO_FINE_ANGLE_SEARCH);
                changed |= SetBrowsableIfExists(propertyGrid, selected, nameof(global::OpenVisionLab.MatchingProperty.COARSE_ANGLE_TOP_K), matching.USE_FIND_ANGLE && matching.USE_COARSE_TO_FINE_ANGLE_SEARCH);
                changed |= SetBrowsableIfExists(propertyGrid, selected, nameof(global::OpenVisionLab.MatchingProperty.PYRAMID_POSITION_TOP_N), matching.USE_PYRAMID_POSITION_PROPOSAL);
                changed |= SetBrowsableIfExists(propertyGrid, selected, nameof(global::OpenVisionLab.MatchingProperty.PYRAMID_POSITION_MIN_SCORE), matching.USE_PYRAMID_POSITION_PROPOSAL);
                changed |= SetBrowsableIfExists(propertyGrid, selected, nameof(global::OpenVisionLab.MatchingProperty.CANNY_LOW), matching.USE_CANNY);
                changed |= SetBrowsableIfExists(propertyGrid, selected, nameof(global::OpenVisionLab.MatchingProperty.CANNY_HIGH), matching.USE_CANNY);
            }

            if (selected is global::OpenVisionLab.EdgeBasedMatchingProperty edgeMatching)
            {
                changed |= SetBrowsableIfExists(propertyGrid, selected, nameof(global::OpenVisionLab.EdgeBasedMatchingProperty.FIND_ANGLE), edgeMatching.USE_FIND_ANGLE);
                changed |= SetBrowsableIfExists(propertyGrid, selected, nameof(global::OpenVisionLab.EdgeBasedMatchingProperty.FIND_ANGLE_MIN), edgeMatching.USE_FIND_ANGLE);
                changed |= SetBrowsableIfExists(propertyGrid, selected, nameof(global::OpenVisionLab.EdgeBasedMatchingProperty.FIND_ANGLE_MAX), edgeMatching.USE_FIND_ANGLE);
                changed |= SetBrowsableIfExists(propertyGrid, selected, nameof(global::OpenVisionLab.EdgeBasedMatchingProperty.USE_COARSE_TO_FINE_ANGLE_SEARCH), edgeMatching.USE_FIND_ANGLE);
                changed |= SetBrowsableIfExists(propertyGrid, selected, nameof(global::OpenVisionLab.EdgeBasedMatchingProperty.COARSE_ANGLE_STEP), edgeMatching.USE_FIND_ANGLE && edgeMatching.USE_COARSE_TO_FINE_ANGLE_SEARCH);
                changed |= SetBrowsableIfExists(propertyGrid, selected, nameof(global::OpenVisionLab.EdgeBasedMatchingProperty.COARSE_ANGLE_TOP_K), edgeMatching.USE_FIND_ANGLE && edgeMatching.USE_COARSE_TO_FINE_ANGLE_SEARCH);
                changed |= SetBrowsableIfExists(propertyGrid, selected, nameof(global::OpenVisionLab.EdgeBasedMatchingProperty.PYRAMID_POSITION_TOP_N), edgeMatching.USE_PYRAMID_POSITION_PROPOSAL);
                changed |= SetBrowsableIfExists(propertyGrid, selected, nameof(global::OpenVisionLab.EdgeBasedMatchingProperty.PYRAMID_POSITION_MIN_SCORE), edgeMatching.USE_PYRAMID_POSITION_PROPOSAL);
                changed |= SetBrowsableIfExists(propertyGrid, selected, nameof(global::OpenVisionLab.EdgeBasedMatchingProperty.HYBRID_VERIFY_TOP_N), edgeMatching.USE_HYBRID_VERIFY);
                changed |= SetBrowsableIfExists(propertyGrid, selected, nameof(global::OpenVisionLab.EdgeBasedMatchingProperty.HYBRID_VERIFY_IMAGE_WEIGHT), edgeMatching.USE_HYBRID_VERIFY);
            }

            if (selected is global::OpenVisionLab.LineGaugeProperty line)
            {
                changed |= SetBrowsableIfExists(propertyGrid, selected, nameof(global::OpenVisionLab.LineGaugeProperty.POINT_RANGE), !line.USE_MANUAL_ANGLE);
                changed |= SetBrowsableIfExists(propertyGrid, selected, nameof(global::OpenVisionLab.LineGaugeProperty.MANUAL_ANGLE_VALUE), line.USE_MANUAL_ANGLE);
                changed |= SetBrowsableIfExists(propertyGrid, selected, nameof(global::OpenVisionLab.LineGaugeProperty.EXTEND_FIT_LINE_VALUE), line.USE_EXTEND_FIT_LINE);
                changed |= SetBrowsableIfExists(propertyGrid, selected, nameof(global::OpenVisionLab.LineGaugeProperty.AVERAGE_Diff), line.USE_AVERAGE_FILTER);
                changed |= SetBrowsableIfExists(propertyGrid, selected, nameof(global::OpenVisionLab.LineGaugeProperty.AVERAGE_FILTER_TYPE), line.USE_AVERAGE_FILTER);
            }

            return changed;
        }

        private static bool SetBrowsableIfExists(IPropertyGridView propertyGrid, object selected, string propertyName, bool isBrowsable)
        {
            if (selected.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public) == null)
            {
                return false;
            }

            IPropertyGridProperty gridProperty = propertyGrid.Properties?[propertyName];
            bool wasBrowsable = gridProperty?.IsBrowsable ?? true;
            // Always write the rule into the bridge. The original WPG item state can be stale
            // during first render, while the bridge hidden table is the source used after refresh.
            propertyGrid.SetPropertyBrowsable(propertyName, isBrowsable);
            return wasBrowsable != isBrowsable;
        }

        private static bool ShouldReapplyVisibility(string propertyName)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
            {
                return false;
            }

            switch (propertyName)
            {
                case nameof(OpenCvPropertyBase.USE_THRESHOLD):
                case nameof(OpenCvPropertyBase.USE_ADAPTIVE_THRESHOLD):
                case nameof(OpenCvPropertyBase.USE_ROI):
                case nameof(OpenCvPropertyBase.USE_MULTI_ROI):
                case nameof(OpenCvPropertyBase.USE_MASKING):
                case nameof(global::OpenVisionLab.ContourProperty.USE_APPROXPOLYDP):
                case nameof(global::OpenVisionLab.MatchingProperty.USE_FIND_ANGLE):
                case nameof(global::OpenVisionLab.MatchingProperty.USE_COARSE_TO_FINE_ANGLE_SEARCH):
                case nameof(global::OpenVisionLab.MatchingProperty.USE_PYRAMID_POSITION_PROPOSAL):
                case nameof(global::OpenVisionLab.MatchingProperty.USE_CANNY):
                case nameof(global::OpenVisionLab.EdgeBasedMatchingProperty.USE_HYBRID_VERIFY):
                case nameof(global::OpenVisionLab.LineGaugeProperty.USE_MANUAL_ANGLE):
                case nameof(global::OpenVisionLab.LineGaugeProperty.USE_EXTEND_FIT_LINE):
                case nameof(global::OpenVisionLab.LineGaugeProperty.USE_AVERAGE_FILTER):
                    return true;
                default:
                    return false;
            }
        }
    }
}
