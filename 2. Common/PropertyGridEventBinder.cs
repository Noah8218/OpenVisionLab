using OpenVisionLab._1._Core;
using OpenVisionLab.Vision._1._Tools.OpenCV;
using Lib.Common;
using Lib.OpenCV;
using Lib.OpenCV.Property;
using OpenCvSharp;
using System;
using System.Drawing;
using System.Reflection;
using OpenVisionLab.PropertyGrid;

namespace OpenVisionLab._2._Common
{
    public sealed class PropertyGridEventBinder
    {
        private readonly Func<IDisplayManager> displayManagerAccessor;

        public PropertyGridEventBinder(Func<IDisplayManager> displayManagerAccessor)
        {
            this.displayManagerAccessor = displayManagerAccessor ?? (() => DisplayManagerService.Default);
        }

        private IDisplayManager DisplayManager => displayManagerAccessor() ?? DisplayManagerService.Default;

        public void Wpg_SelectedObjectsChanged(object sender, EventArgs e)
        {
            ApplyVisibilityRules(sender as IPropertyGridView);
        }

        public void Wpg_PropertyValueChanged(object sender, PropertyGridPropertyValueChangedEventArgs e)
        {
            ApplyVisibilityRules(sender as IPropertyGridView);
        }

        public void ApplyVisibilityRules(IPropertyGridView propertyGrid)
        {
            if (propertyGrid == null) { return; }
            if (propertyGrid.SelectedObject == null) return;
            SetCOpencvPropertyBase(propertyGrid);
            SetLineGaugePropertyVisibility(propertyGrid);
            SetCPropertyMatching(propertyGrid);
            SetCPropertyContour(propertyGrid);
            SetThresholdToolPropertyVisibility(propertyGrid);
            SetFilterToolPropertyVisibility(propertyGrid);
            SetEdgeDetectionToolPropertyVisibility(propertyGrid);
            SetPipelineStepAcceptanceVisibility(propertyGrid);
        }

        private static void SetCPropertyMatching(IPropertyGridView propertyGrid)
        {
            if (propertyGrid.SelectedObject is MatchingProperty)
            {
                var match = (MatchingProperty)propertyGrid.SelectedObject;
                if (match.USE_FIND_ANGLE)
                {
                    propertyGrid.Properties[nameof(match.FIND_ANGLE)].IsBrowsable = true;
                    propertyGrid.Properties[nameof(match.FIND_ANGLE_MAX)].IsBrowsable = true;
                    propertyGrid.Properties[nameof(match.FIND_ANGLE_MIN)].IsBrowsable = true;
                }
                else
                {
                    propertyGrid.Properties[nameof(match.FIND_ANGLE)].IsBrowsable = false;
                    propertyGrid.Properties[nameof(match.FIND_ANGLE_MAX)].IsBrowsable = false;
                    propertyGrid.Properties[nameof(match.FIND_ANGLE_MIN)].IsBrowsable = false;
                }

                if (match.USE_CANNY)
                {
                    propertyGrid.Properties[nameof(match.CANNY_HIGH)].IsBrowsable = true;
                    propertyGrid.Properties[nameof(match.CANNY_LOW)].IsBrowsable = true;
                }
                else
                {
                    propertyGrid.Properties[nameof(match.CANNY_HIGH)].IsBrowsable = false;
                    propertyGrid.Properties[nameof(match.CANNY_LOW)].IsBrowsable = false;
                }
            }
        }
        private static void SetCPropertyContour(IPropertyGridView propertyGrid)
        {
            if (propertyGrid.SelectedObject is ContourProperty)
            {
                var contour = (ContourProperty)propertyGrid.SelectedObject;
                if (contour.USE_DRAW_IMAGE)
                {
                    propertyGrid.Properties[nameof(contour.DrawColor)].IsBrowsable = true;
                    propertyGrid.Properties[nameof(contour.DrawThickness)].IsBrowsable = true;
                }
                else
                {
                    propertyGrid.Properties[nameof(contour.DrawColor)].IsBrowsable = false;
                    propertyGrid.Properties[nameof(contour.DrawThickness)].IsBrowsable = false;
                }

                if (contour.USE_APPROXPOLYDP)
                {
                    propertyGrid.Properties[nameof(contour.EPSILON)].IsBrowsable = true;
                }
                else
                {
                    propertyGrid.Properties[nameof(contour.EPSILON)].IsBrowsable = false;
                }
            }
        }

        private static void SetThresholdToolPropertyVisibility(IPropertyGridView propertyGrid)
        {
            if (!(propertyGrid.SelectedObject is ThresholdToolProperty threshold))
            {
                return;
            }

            bool useThreshold = threshold.Mode == ThresholdToolMode.Threshold;
            bool useRange = threshold.Mode == ThresholdToolMode.Range;
            bool useAdaptive = threshold.Mode == ThresholdToolMode.Adaptive;

            SetBrowsable(propertyGrid, nameof(threshold.Threshold), useThreshold);
            SetBrowsable(propertyGrid, nameof(threshold.ThresholdType), useThreshold);
            SetBrowsable(propertyGrid, nameof(threshold.MaxValue), useThreshold || useAdaptive);
            SetBrowsable(propertyGrid, nameof(threshold.RangeMin), useRange);
            SetBrowsable(propertyGrid, nameof(threshold.RangeMax), useRange);
            SetBrowsable(propertyGrid, nameof(threshold.Invert), useRange);
            SetBrowsable(propertyGrid, nameof(threshold.AdaptiveType), useAdaptive);
            SetBrowsable(propertyGrid, nameof(threshold.AdaptiveThresholdType), useAdaptive);
            SetBrowsable(propertyGrid, nameof(threshold.BlockSize), useAdaptive);
            SetBrowsable(propertyGrid, nameof(threshold.Weight), useAdaptive);
        }

        private static void SetFilterToolPropertyVisibility(IPropertyGridView propertyGrid)
        {
            if (!(propertyGrid.SelectedObject is FilterToolProperty filter))
            {
                return;
            }

            bool usesKernel = filter.FilterType == FilterToolType.Blur
                || filter.FilterType == FilterToolType.GaussianBlur
                || filter.FilterType == FilterToolType.BoxFilter;
            bool usesMedian = filter.FilterType == FilterToolType.MedianBlur;
            bool usesBilateral = filter.FilterType == FilterToolType.BilateralFilter;
            bool usesBorder = filter.FilterType == FilterToolType.Blur
                || filter.FilterType == FilterToolType.GaussianBlur
                || filter.FilterType == FilterToolType.BilateralFilter;

            SetBrowsable(propertyGrid, nameof(filter.KernelWidth), usesKernel);
            SetBrowsable(propertyGrid, nameof(filter.KernelHeight), usesKernel);
            SetBrowsable(propertyGrid, nameof(filter.MedianKernelSize), usesMedian);
            SetBrowsable(propertyGrid, nameof(filter.Diameter), usesBilateral);
            SetBrowsable(propertyGrid, nameof(filter.SigmaColor), usesBilateral);
            SetBrowsable(propertyGrid, nameof(filter.SigmaSpace), usesBilateral);
            SetBrowsable(propertyGrid, nameof(filter.BorderType), usesBorder);
        }

        private static void SetEdgeDetectionToolPropertyVisibility(IPropertyGridView propertyGrid)
        {
            if (!(propertyGrid.SelectedObject is EdgeDetectionToolProperty edge))
            {
                return;
            }

            bool usesCanny = edge.EdgeType == EdgeDetectionToolType.Canny;
            bool usesSobel = edge.EdgeType == EdgeDetectionToolType.Sobel;
            bool usesScharr = edge.EdgeType == EdgeDetectionToolType.Scharr;
            bool usesLaplacian = edge.EdgeType == EdgeDetectionToolType.Laplacian;

            SetBrowsable(propertyGrid, nameof(edge.CannyThresholdLow), usesCanny);
            SetBrowsable(propertyGrid, nameof(edge.CannyThresholdHigh), usesCanny);
            SetBrowsable(propertyGrid, nameof(edge.CannyApertureSize), usesCanny);
            SetBrowsable(propertyGrid, nameof(edge.UseL2Gradient), usesCanny);
            SetBrowsable(propertyGrid, nameof(edge.SobelDegreeX), usesSobel);
            SetBrowsable(propertyGrid, nameof(edge.SobelDegreeY), usesSobel);
            SetBrowsable(propertyGrid, nameof(edge.SobelKernelSize), usesSobel);
            SetBrowsable(propertyGrid, nameof(edge.ScharrDegreeX), usesScharr);
            SetBrowsable(propertyGrid, nameof(edge.ScharrDegreeY), usesScharr);
            SetBrowsable(propertyGrid, nameof(edge.LaplacianKernelSize), usesLaplacian);
        }

        private static void SetPipelineStepAcceptanceVisibility(IPropertyGridView propertyGrid)
        {
            object selected = propertyGrid.SelectedObject;
            if (selected == null || !HasProperty(selected, "UseAcceptance"))
            {
                return;
            }

            bool useAcceptance = GetBoolProperty(selected, "UseAcceptance");
            bool useMetricRange = useAcceptance && !string.IsNullOrWhiteSpace(GetStringProperty(selected, "AcceptanceMetricName"));

            SetBrowsable(propertyGrid, "ExpectedSuccess", useAcceptance);
            SetBrowsable(propertyGrid, "MaxElapsedMilliseconds", useAcceptance);
            SetBrowsable(propertyGrid, "RequiredMessageText", useAcceptance);
            SetBrowsable(propertyGrid, "AcceptanceMetricName", useAcceptance);
            SetBrowsable(propertyGrid, "UseAcceptanceMetricMinimum", false);
            SetBrowsable(propertyGrid, "AcceptanceMetricMinimum", useMetricRange);
            SetBrowsable(propertyGrid, "UseAcceptanceMetricMaximum", false);
            SetBrowsable(propertyGrid, "AcceptanceMetricMaximum", false);
        }

        private static void SetLineGaugePropertyVisibility(IPropertyGridView propertyGrid)
        {
            if (propertyGrid.SelectedObject is LineGaugeProperty)
            {
                var line = (LineGaugeProperty)propertyGrid.SelectedObject;
                if (line.USE_MANUAL_ANGLE)
                {
                    propertyGrid.Properties[nameof(line.MANUAL_ANGLE_VALUE)].IsBrowsable = true;
                    propertyGrid.Properties[nameof(line.POINT_RANGE)].IsBrowsable = false;
                }
                else
                {
                    propertyGrid.Properties[nameof(line.MANUAL_ANGLE_VALUE)].IsBrowsable = false;
                    propertyGrid.Properties[nameof(line.POINT_RANGE)].IsBrowsable = true;
                }
            }
        }

        private static bool HasProperty(object target, string propertyName)
        {
            return target?.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public) != null;
        }

        private static bool GetBoolProperty(object target, string propertyName)
        {
            PropertyInfo property = target?.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);
            object value = property?.GetValue(target, null);
            return value is bool boolValue && boolValue;
        }

        private static string GetStringProperty(object target, string propertyName)
        {
            PropertyInfo property = target?.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);
            object value = property?.GetValue(target, null);
            return value?.ToString() ?? string.Empty;
        }

        private static void SetBrowsable(IPropertyGridView propertyGrid, string propertyName, bool isBrowsable)
        {
            var property = propertyGrid?.Properties?[propertyName];
            if (property != null)
            {
                property.IsBrowsable = isBrowsable;
            }
        }

        private static void SetCOpencvPropertyBase(IPropertyGridView propertyGrid)
        {
            if (propertyGrid.SelectedObject is OpenCvPropertyBase)
            {
                var Prop = (OpenCvPropertyBase)propertyGrid.SelectedObject;
                if (Prop.USE_ROI)
                {
                    propertyGrid.Properties[nameof(Prop.USE_MULTI_ROI)].IsBrowsable = true;

                    if (Prop.USE_MULTI_ROI)
                    {
                        propertyGrid.Properties[nameof(Prop.CvROI)].IsBrowsable = false;
                        propertyGrid.Properties[nameof(Prop.CvROIS)].IsBrowsable = true;
                    }
                    else
                    {
                        propertyGrid.Properties[nameof(Prop.CvROI)].IsBrowsable = true;
                        propertyGrid.Properties[nameof(Prop.CvROIS)].IsBrowsable = false;
                    }
                }
                else
                {
                    propertyGrid.Properties[nameof(Prop.USE_MULTI_ROI)].SetValue(false);
                    propertyGrid.Properties[nameof(Prop.USE_MULTI_ROI)].IsBrowsable = false;
                    propertyGrid.Properties[nameof(Prop.CvROI)].IsBrowsable = false;
                    propertyGrid.Properties[nameof(Prop.CvROIS)].IsBrowsable = false;
                }

                if (Prop.USE_THRESHOLD)
                {
                    propertyGrid.Properties[nameof(Prop.THRESHOLD)].IsBrowsable = true;
                    propertyGrid.Properties[nameof(Prop.THRESHOLD_TYPES)].IsBrowsable = true;
                }
                else
                {
                    propertyGrid.Properties[nameof(Prop.THRESHOLD)].IsBrowsable = false;
                    propertyGrid.Properties[nameof(Prop.THRESHOLD_TYPES)].IsBrowsable = false;
                }

                if (Prop.USE_ADAPTIVE_THRESHOLD)
                {
                    propertyGrid.Properties[nameof(Prop.ADAPTIVE_THRESHOLD)].IsBrowsable = true;
                    propertyGrid.Properties[nameof(Prop.ADAPTIVE_THRESHOLD_TYPES)].IsBrowsable = true;
                    propertyGrid.Properties[nameof(Prop.ADAPTIVE_THRESHOLD_ALGORITHM)].IsBrowsable = true;
                    propertyGrid.Properties[nameof(Prop.Weight)].IsBrowsable = true;
                    propertyGrid.Properties[nameof(Prop.BlockSize)].IsBrowsable = true;
                }
                else
                {
                    propertyGrid.Properties[nameof(Prop.ADAPTIVE_THRESHOLD)].IsBrowsable = false;
                    propertyGrid.Properties[nameof(Prop.ADAPTIVE_THRESHOLD_TYPES)].IsBrowsable = false;
                    propertyGrid.Properties[nameof(Prop.ADAPTIVE_THRESHOLD_ALGORITHM)].IsBrowsable = false;
                    propertyGrid.Properties[nameof(Prop.Weight)].IsBrowsable = false;
                    propertyGrid.Properties[nameof(Prop.BlockSize)].IsBrowsable = false;
                }
            }        
        }

    }
}
