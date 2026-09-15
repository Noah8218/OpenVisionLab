using OpenVisionLab.Core;
using OpenVisionLab.PropertyGrid;
using OpenVisionLab.Vision._1._Tools.OpenCV;
using OpenCvSharp;
using System;
using System.ComponentModel;
using System.Reflection;

namespace OpenVisionLab.Common
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
                changed |= SetBrowsableIfExists(propertyGrid, selected, nameof(global::OpenVisionLab.EdgeBasedMatchingProperty.FIND_SCALE_MIN), edgeMatching.USE_FIND_SCALE);
                changed |= SetBrowsableIfExists(propertyGrid, selected, nameof(global::OpenVisionLab.EdgeBasedMatchingProperty.FIND_SCALE_MAX), edgeMatching.USE_FIND_SCALE);
                changed |= SetBrowsableIfExists(propertyGrid, selected, nameof(global::OpenVisionLab.EdgeBasedMatchingProperty.FIND_SCALE_STEP), edgeMatching.USE_FIND_SCALE);
                changed |= ApplyEdgeBasedMatchingAdvancedVisibility(propertyGrid, selected, edgeMatching);
            }

            if (selected is global::OpenVisionLab.LineGaugeProperty line)
            {
                changed |= SetBrowsableIfExists(propertyGrid, selected, nameof(global::OpenVisionLab.LineGaugeProperty.POINT_RANGE), !line.USE_MANUAL_ANGLE);
                changed |= SetBrowsableIfExists(propertyGrid, selected, nameof(global::OpenVisionLab.LineGaugeProperty.MANUAL_ANGLE_VALUE), line.USE_MANUAL_ANGLE);
                changed |= SetBrowsableIfExists(propertyGrid, selected, nameof(global::OpenVisionLab.LineGaugeProperty.EXTEND_FIT_LINE_VALUE), line.USE_EXTEND_FIT_LINE);
                changed |= SetBrowsableIfExists(propertyGrid, selected, nameof(global::OpenVisionLab.LineGaugeProperty.AVERAGE_Diff), line.USE_AVERAGE_FILTER);
                changed |= SetBrowsableIfExists(propertyGrid, selected, nameof(global::OpenVisionLab.LineGaugeProperty.AVERAGE_FILTER_TYPE), line.USE_AVERAGE_FILTER);
            }

            if (selected is global::OpenVisionLab.AffineTransformProperty affine)
            {
                string[] unrelatedInheritedProperties =
                {
                    nameof(OpenCvPropertyBase.PIXELPERMM),
                    nameof(OpenCvPropertyBase.USE_THRESHOLD),
                    nameof(OpenCvPropertyBase.USE_BITWISENOT),
                    nameof(OpenCvPropertyBase.THRESHOLD_TYPES),
                    nameof(OpenCvPropertyBase.THRESHOLD),
                    nameof(OpenCvPropertyBase.USE_ADAPTIVE_THRESHOLD),
                    nameof(OpenCvPropertyBase.ADAPTIVE_THRESHOLD),
                    nameof(OpenCvPropertyBase.ADAPTIVE_THRESHOLD_TYPES),
                    nameof(OpenCvPropertyBase.ADAPTIVE_THRESHOLD_ALGORITHM),
                    nameof(OpenCvPropertyBase.BlockSize),
                    nameof(OpenCvPropertyBase.Weight),
                    nameof(OpenCvPropertyBase.USE_ROI),
                    nameof(OpenCvPropertyBase.USE_MULTI_ROI),
                    nameof(OpenCvPropertyBase.CvROI),
                    nameof(OpenCvPropertyBase.CvROIS),
                    nameof(OpenCvPropertyBase.USE_MASKING),
                    nameof(OpenCvPropertyBase.CvMASKS),
                    nameof(OpenCvPropertyBase.CvROIXml),
                    nameof(OpenCvPropertyBase.CvROISXml),
                    nameof(OpenCvPropertyBase.CvMASKSXml)
                };

                foreach (string propertyName in unrelatedInheritedProperties)
                {
                    changed |= SetBrowsableIfExists(propertyGrid, selected, propertyName, false);
                }

                changed |= ApplyAffineAdvancedVisibility(
                    propertyGrid,
                    selected,
                    affine.ShowAdvancedSettings,
                    affine.BorderType);
            }

            PropertyInfo detectedSourceProperty = selected.GetType().GetProperty(
                "UseDetectedSourcePoints",
                BindingFlags.Instance | BindingFlags.Public);
            if (detectedSourceProperty?.PropertyType == typeof(bool))
            {
                bool useDetectedSourcePoints = (bool)detectedSourceProperty.GetValue(selected);
                foreach (string propertyName in new[]
                {
                    "SourcePoint1Feature",
                    "SourcePoint2Feature",
                    "SourcePoint3Feature"
                })
                {
                    changed |= SetBrowsableIfExists(propertyGrid, selected, propertyName, useDetectedSourcePoints);
                }

                foreach (string propertyName in new[]
                {
                    "SourcePoint1X",
                    "SourcePoint1Y",
                    "SourcePoint2X",
                    "SourcePoint2Y",
                    "SourcePoint3X",
                    "SourcePoint3Y"
                })
                {
                    changed |= SetBrowsableIfExists(propertyGrid, selected, propertyName, !useDetectedSourcePoints);
                }

                PropertyInfo showAdvancedProperty = selected.GetType().GetProperty(
                    nameof(global::OpenVisionLab.AffineTransformProperty.ShowAdvancedSettings),
                    BindingFlags.Instance | BindingFlags.Public);
                PropertyInfo borderTypeProperty = selected.GetType().GetProperty(
                    nameof(global::OpenVisionLab.AffineTransformProperty.BorderType),
                    BindingFlags.Instance | BindingFlags.Public);
                if (showAdvancedProperty?.PropertyType == typeof(bool)
                    && borderTypeProperty?.PropertyType == typeof(BorderTypes))
                {
                    changed |= ApplyAffineAdvancedVisibility(
                        propertyGrid,
                        selected,
                        (bool)showAdvancedProperty.GetValue(selected),
                        (BorderTypes)borderTypeProperty.GetValue(selected));
                }
            }

            return changed;
        }

        private static bool ApplyAffineAdvancedVisibility(
            IPropertyGridView propertyGrid,
            object selected,
            bool showAdvancedSettings,
            BorderTypes borderType)
        {
            bool changed = false;
            foreach (string propertyName in new[]
            {
                "OutputWidth",
                "OutputHeight",
                "Interpolation",
                "BorderType",
                "MinimumSourceTriangleArea",
                "MinimumDestinationTriangleArea",
                "MinimumValidPixelRatio"
            })
            {
                changed |= SetBrowsableIfExists(propertyGrid, selected, propertyName, showAdvancedSettings);
            }

            changed |= SetBrowsableIfExists(
                propertyGrid,
                selected,
                "BorderValue",
                showAdvancedSettings && borderType == BorderTypes.Constant);
            return changed;
        }

        private static bool ApplyEdgeBasedMatchingAdvancedVisibility(
            IPropertyGridView propertyGrid,
            object selected,
            global::OpenVisionLab.EdgeBasedMatchingProperty property)
        {
            bool changed = false;
            bool advanced = property.ShowAdvancedSettings;
            foreach (string propertyName in new[]
            {
                nameof(global::OpenVisionLab.EdgeBasedMatchingProperty.USE_UNIQUE_MATCH_VALIDATION),
                nameof(global::OpenVisionLab.EdgeBasedMatchingProperty.ALLOW_GLOBAL_POLARITY_REVERSAL),
                nameof(global::OpenVisionLab.EdgeBasedMatchingProperty.AUTO_MPOINT_USE_ANALYSIS_ROI),
                nameof(global::OpenVisionLab.EdgeBasedMatchingProperty.AUTO_MPOINT_PATTERN_WIDTH),
                nameof(global::OpenVisionLab.EdgeBasedMatchingProperty.AUTO_MPOINT_PATTERN_HEIGHT),
                nameof(global::OpenVisionLab.EdgeBasedMatchingProperty.AUTO_MPOINT_STRIDE),
                nameof(global::OpenVisionLab.EdgeBasedMatchingProperty.AUTO_MPOINT_MAX_RESULTS),
                nameof(global::OpenVisionLab.EdgeBasedMatchingProperty.AUTO_MPOINT_MIN_FEATURE_QUALITY),
                nameof(global::OpenVisionLab.EdgeBasedMatchingProperty.AUTO_MPOINT_MIN_UNIQUENESS),
                nameof(global::OpenVisionLab.EdgeBasedMatchingProperty.AUTO_MPOINT_MAX_POSITION_ERROR),
                nameof(global::OpenVisionLab.EdgeBasedMatchingProperty.AUTO_MPOINT_MIN_REPRESENTATIVE_IMAGES),
                nameof(global::OpenVisionLab.EdgeBasedMatchingProperty.AUTO_MPOINT_MIN_REPRESENTATIVE_SUCCESS_RATE),
                nameof(global::OpenVisionLab.EdgeBasedMatchingProperty.CANNY_APERTURE_SIZE),
                nameof(global::OpenVisionLab.EdgeBasedMatchingProperty.USE_L2_GRADIENT),
                nameof(global::OpenVisionLab.EdgeBasedMatchingProperty.CONTOUR_RETRIEVAL_MODE),
                nameof(global::OpenVisionLab.EdgeBasedMatchingProperty.CONTOUR_APPROXIMATION_MODE),
                nameof(global::OpenVisionLab.EdgeBasedMatchingProperty.MAX_TEMPLATE_POINTS),
                nameof(global::OpenVisionLab.EdgeBasedMatchingProperty.MIN_GRADIENT_MAGNITUDE),
                nameof(global::OpenVisionLab.EdgeBasedMatchingProperty.USE_COARSE_TO_FINE_ANGLE_SEARCH),
                nameof(global::OpenVisionLab.EdgeBasedMatchingProperty.USE_POSITION_REFINE),
                nameof(global::OpenVisionLab.EdgeBasedMatchingProperty.USE_SUBPIXEL_REFINE),
                nameof(global::OpenVisionLab.EdgeBasedMatchingProperty.GREEDINESS),
                nameof(global::OpenVisionLab.EdgeBasedMatchingProperty.USE_PYRAMID_POSITION_PROPOSAL),
                nameof(global::OpenVisionLab.EdgeBasedMatchingProperty.USE_HYBRID_VERIFY)
            })
            {
                changed |= SetBrowsableIfExists(propertyGrid, selected, propertyName, advanced);
            }

            changed |= SetBrowsableIfExists(
                propertyGrid,
                selected,
                nameof(global::OpenVisionLab.EdgeBasedMatchingProperty.CANNY_HIGH),
                false);
            changed |= SetBrowsableIfExists(
                propertyGrid,
                selected,
                nameof(global::OpenVisionLab.EdgeBasedMatchingProperty.UNIQUE_MATCH_MIN_SCORE_MARGIN),
                advanced && property.USE_UNIQUE_MATCH_VALIDATION);
            changed |= SetBrowsableIfExists(
                propertyGrid,
                selected,
                nameof(global::OpenVisionLab.EdgeBasedMatchingProperty.AUTO_MPOINT_ANALYSIS_ROI),
                advanced && property.AUTO_MPOINT_USE_ANALYSIS_ROI);
            changed |= SetBrowsableIfExists(
                propertyGrid,
                selected,
                nameof(global::OpenVisionLab.EdgeBasedMatchingProperty.COARSE_ANGLE_STEP),
                advanced && property.USE_FIND_ANGLE && property.USE_COARSE_TO_FINE_ANGLE_SEARCH);
            changed |= SetBrowsableIfExists(
                propertyGrid,
                selected,
                nameof(global::OpenVisionLab.EdgeBasedMatchingProperty.COARSE_ANGLE_TOP_K),
                advanced && property.USE_FIND_ANGLE && property.USE_COARSE_TO_FINE_ANGLE_SEARCH);
            changed |= SetBrowsableIfExists(
                propertyGrid,
                selected,
                nameof(global::OpenVisionLab.EdgeBasedMatchingProperty.PYRAMID_POSITION_TOP_N),
                advanced && property.USE_PYRAMID_POSITION_PROPOSAL);
            changed |= SetBrowsableIfExists(
                propertyGrid,
                selected,
                nameof(global::OpenVisionLab.EdgeBasedMatchingProperty.PYRAMID_POSITION_MIN_SCORE),
                advanced && property.USE_PYRAMID_POSITION_PROPOSAL);
            changed |= SetBrowsableIfExists(
                propertyGrid,
                selected,
                nameof(global::OpenVisionLab.EdgeBasedMatchingProperty.HYBRID_VERIFY_TOP_N),
                advanced && property.USE_HYBRID_VERIFY);
            changed |= SetBrowsableIfExists(
                propertyGrid,
                selected,
                nameof(global::OpenVisionLab.EdgeBasedMatchingProperty.HYBRID_VERIFY_IMAGE_WEIGHT),
                advanced && property.USE_HYBRID_VERIFY);
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
                case nameof(global::OpenVisionLab.EdgeBasedMatchingProperty.USE_FIND_SCALE):
                case nameof(global::OpenVisionLab.EdgeBasedMatchingProperty.USE_UNIQUE_MATCH_VALIDATION):
                case nameof(global::OpenVisionLab.EdgeBasedMatchingProperty.AUTO_MPOINT_USE_ANALYSIS_ROI):
                case nameof(global::OpenVisionLab.EdgeBasedMatchingProperty.USE_HYBRID_VERIFY):
                case nameof(global::OpenVisionLab.LineGaugeProperty.USE_MANUAL_ANGLE):
                case nameof(global::OpenVisionLab.LineGaugeProperty.USE_EXTEND_FIT_LINE):
                case nameof(global::OpenVisionLab.LineGaugeProperty.USE_AVERAGE_FILTER):
                case nameof(global::OpenVisionLab.AffineTransformProperty.ShowAdvancedSettings):
                case nameof(global::OpenVisionLab.AffineTransformProperty.BorderType):
                case "UseDetectedSourcePoints":
                    return true;
                default:
                    return false;
            }
        }
    }
}
